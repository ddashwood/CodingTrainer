using CodingTrainer.CSharpRunner.PartiallyTrusted;
using CodingTrainer.CSharpRunner.SandboxHost;
using System;
using System.IO;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    partial class CodeRunner
    {
        private class SandboxManager
        {
            private CodeRunner parent;

            public void RunInSandbox(CodeRunner parent, byte[] compiledCode, byte[] pdb)
            {
                this.parent = parent;
                AppDomain newDomain = null;

                Task runner = null, timerTask = null, totalTimeTask = null, memoryTask = null;

                try
                {
                    using (var consoleInStream = new BlockingMemoryStream())
                    using (parent.consoleInWriter = TextWriter.Synchronized(new StreamWriter(consoleInStream)))
                    using (var newConsoleIn = TextReader.Synchronized(new StreamReader(consoleInStream)))
                    using (var newConsoleOut = new EventStringWriter())
                    {
                        // Create the sandbox for the code

                        newDomain = CreateSandbox();
                        AppDomain.MonitoringIsEnabled = true;

                        ObjectHandle handle = Activator.CreateInstanceFrom(newDomain,
                                typeof(SandboxerWithConsoleRedirect).Assembly.ManifestModule.FullyQualifiedName,
                                typeof(SandboxerWithConsoleRedirect).FullName);
                        var newDomainInstance = (SandboxerWithConsoleRedirect)handle.Unwrap();
                        newDomainInstance.RedirectConsole(newConsoleOut, newConsoleIn);

                        // Run the code

                        newConsoleOut.Flushed += ConsoleFlushed;

                        object threadLock = new object();
                        Thread thread = null;
                        Thread actualThread;
                        runner = Task.Run(() =>
                        {
                            // Set the thread on which the untrusted code is running - this can be used
                            // by the main thread to kill the untrusted code if required
                            lock (threadLock)
                            {
                                thread = Thread.CurrentThread;
                            }

                            newDomainInstance.ExecuteUntrustedCode(typeof(CompiledCodeRunner).Assembly.FullName, typeof(CompiledCodeRunner).FullName, "RunIt", new object[] { compiledCode, pdb });
                        });

                        // The task should set its thread almost immediately
                        do
                        {
                            lock (threadLock)
                            {
                                actualThread = thread;
                            }
                            if (actualThread == null) Thread.Sleep(1);
                        }
                        while (actualThread == null);
                        // Now the task has set its thread, begin checking its progress

                        ManualResetEvent finished = new ManualResetEvent(false);
                        bool outOfMemory = false;
                        bool timeOut = false;
                        bool totalTimeOut = false;
                        object outOfMemoryLock = new object();
                        object timeOutLock = new object();
                        object totalTimeOutLock = new object();

                        // Allow a maximum of 6 seconds of processor time,
                        // with an absolute time limit of 5 minutes.
                        // Also, allow maximum of 5MB of memory (n.b. limited accuracy - only
                        // gets updated after a full blocking garbage collection)

                        timerTask = Task.Run(() =>
                        {
                            bool isFinished = false;
                            while (!isFinished && (newDomain?.MonitoringTotalProcessorTime.Seconds ?? 0) < 6)
                            {
                                isFinished = finished.WaitOne(100);
                            }
                            lock (timeOutLock)
                            {
                                timeOut = !isFinished;
                            }
                        });
                        totalTimeTask = Task.Run(() =>
                        {
                            bool isFinished = finished.WaitOne(1000 * 60 * 5);
                            lock (totalTimeOutLock)
                            {
                                totalTimeOut = !isFinished;
                            }
                        });
                        memoryTask = Task.Run(() =>
                        {
                            bool isFinished = false;
                            long mem;
                            do
                            {
                                mem = newDomain?.MonitoringSurvivedMemorySize ?? 0;
                                isFinished = finished.WaitOne(10);
                            }
                            while (!isFinished && mem < 5 * 1024 * 1024);
                            lock (outOfMemoryLock)
                            {
                                outOfMemory = !isFinished;
                            }
                        });

                        // Wait until the task either finishes or one of the monitoring tasks finishes
                        Task.WaitAny(runner, timerTask, totalTimeTask, memoryTask);
                        finished.Set();

                        lock (outOfMemoryLock) lock (timeOutLock) lock (totalTimeOutLock)
                                {
                                    if (runner.IsCompleted && runner.Exception != null)
                                    {
                                        // Unwrap the exception, then wrap it in our own AggregateException
                                        Exception e = runner.Exception;
                                        while (e.InnerException != null) e = e.InnerException;

                                        throw new AggregateException("An error occured while running the code", e);
                                    }
                                    if (outOfMemory)
                                    {
                                        thread.Abort();
                                        throw new OutOfMemoryException("The memory limit of 5MB has been exceded");
                                    }
                                    if (timeOut || totalTimeOut)
                                    {
                                        // Timed out
                                        thread.Abort();
                                        throw new TimeoutException("The process timed out");
                                    }
                                }

                        // No exceptions thrown - it must have finished successfully
                        string remainingConsoleOut = newConsoleOut.ToString();
                        if (!string.IsNullOrEmpty(remainingConsoleOut))
                            DoConsoleWrite(newConsoleOut.ToString());
                    }
                }
                finally
                {
                    parent.consoleInWriter = null;

                    if (newDomain != null)
                        AppDomain.Unload(newDomain);
                    newDomain = null;
                    GC.Collect(); // Because AppDomain might use lots of mmemory - give GC a hint to clean it up
                }
            }

            private static AppDomain CreateSandbox()
            {
                PermissionSet permSet = new PermissionSet(PermissionState.None);
                permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

                StrongName fullTrustAssembly = typeof(SandboxerWithConsoleRedirect).Assembly.Evidence.GetHostEvidence<StrongName>();

                AppDomainSetup adSetup = new AppDomainSetup
                {
                    ApplicationBase = Path.GetFullPath(Path.GetDirectoryName(typeof(CompiledCodeRunner).Assembly.Location))
                };

                AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);
                return newDomain;
            }

            private void DoConsoleWrite(string message)
            {
                parent.ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs(message));
            }

            // Event handler for handling the ConsoleFlush event - this is raised
            // when the console output stream has just been flushed, so there is data
            // there for us to read
            private void ConsoleFlushed(object sender, EventArgs e)
            {
                string s = sender.ToString();
                if (!string.IsNullOrEmpty(s))
                {
                    DoConsoleWrite(s);
                    ((StringWriter)sender).GetStringBuilder().Remove(0, s.Length);
                }
            }
        }
    }
}
