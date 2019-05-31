﻿using System;
using System.Security;
using System.Security.Policy;
using System.Text;
using CodingTrainer.CSharpRunner.CodeHost;
using Microsoft.CodeAnalysis.Scripting;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using CodingTrainer.CSharpRunner.TestingCommon;

namespace CodingTrainer.CSharpRunner.CodeHostTests
{
    [TestFixture]
    class CodeRunnerTests:CodeHostTestBase
    {
        #region CompilerTests
        [Test]
        [Category("Compiler")]
        public void CompilerErrorTest()
        {
            Exception e = Assert.ThrowsAsync(typeof(CompilationErrorException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                    .Append(WrapInMain(@"int a = 1.5;")).ToString()));
            StringAssert.Contains("Cannot implicitly convert type 'double' to 'int'", ((CompilationErrorException)e).Diagnostics[0].GetMessage());
        }

        [Test]
        [Category("Compiler")]
        public void CompilationTooBigTest()
        {
            StringBuilder code = new StringBuilder("int i = 0;");
            for (int i = 0; i < 10000; i++)
            {
                code.Append(" i = 5 * i++ - 4 * 3;");
            }

            Assert.ThrowsAsync(typeof(PolicyException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                    .Append(code).ToString()));
        }
        #endregion

        #region MonitorTests
        [Explicit ("Doesn't work reliably yet")]
        [Test]
        [Category("Monitoring")]
        public void OutOfMemoryTest()
        {
            // The GC.Collect() and Sleep() make this test quite artificial - memory allocation checking
            // only works properly after garbage collection. Without this, it's down to chance
            // when the memory allocation will get updated
            string code = @"
class C { double[] a = new double[100000]; }
C[] ca = new C[500];
for(int i=0; i<ca.Length; i++) { ca[i]=new C(); GC.Collect(); System.Threading.Thread.Sleep(0); }
";
            Exception e = Assert.ThrowsAsync(typeof(OutOfMemoryException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System", "System.Threading" })
                    .Append(code).ToString()));
            Assert.AreEqual("The memory limit of 5MB has been exceded", e.Message);
        }

        [Test]
        [Category("Monitoring")]
        public void TimeOutTest()
        {
            Assert.ThrowsAsync(typeof(TimeoutException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                    .Append(WrapInMain(@"while(true) { }")).ToString()));
        }

        [Explicit("Solution for this problem not yet implemented")]
        [Test]
        [Category("Monitoring")]
        public void TimeOutInFinallyTest()
        {
            Assert.ThrowsAsync(typeof(TimeoutException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                    .Append(WrapInMain(@"try {} finally { while(true) { } }")).ToString()));

        }

        [Explicit("Solution for this problem not yet implemented")]
        [Test]
        [Category("Monitoring")]
        public void StackOverflowTest()
        {
            Assert.ThrowsAsync(typeof(StackOverflowException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                    .Append(WrapInMain(@"Main();")).ToString()));
        }
        #endregion

        #region NormalTests
        [Test]
        [Category("Normal")]
        public async Task ConsoleWriteLineTest()
        {
            await runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                .Append(WrapInMain(@"Console.WriteLine(""Hello World!"");")).ToString());
            Assert.AreEqual("Hello World!\r\n", console.ToString());
        }

        [Test]
        [Category("Normal")]
        public async Task ConsoleWriteLineWithoutMainTest()
        {
            await runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                .Append(@"Console.WriteLine(""Hello World!"");").ToString());
            Assert.AreEqual("Hello World!\r\n", console.ToString());
        }


        [Test]
        [Category("Normal")]
        public async Task ConsoleInTest()
        {
            runner.ConsoleWrite += ConsoleInTestSendString;
            await runner.CompileAndRunAsync(GetUsings(new string[] { "System" }).
                Append(WrapInMain("Console.WriteLine(); Console.WriteLine(Console.ReadLine());")).ToString());
            runner.ConsoleWrite -= ConsoleInTestSendString;
            StringAssert.Contains("From the unit test", console.ToString());
        }
        public void ConsoleInTestSendString(object sender, ConsoleWriteEventArgs e)
        {
            runner.ConsoleIn("From the unit test");
        }

        [Test]
        [Category("Normal")]
        public void ExceptionTest()
        {
            Exception e = Assert.ThrowsAsync(typeof(ExceptionRunningUserCodeException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                    .Append(WrapInMain(@"int i = 0; int j = 5 / i;")).ToString()));
            Assert.That(e.InnerException, Is.TypeOf<DivideByZeroException>());
        }

        [Test]
        [Category("Normal")]
        public void ExceptionWithoutMainTest()
        {
            Exception e = Assert.ThrowsAsync(typeof(ExceptionRunningUserCodeException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System" })
                            .Append(@"int i = 0; int j = 5 / i;").ToString()));
            Assert.That(e.InnerException, Is.TypeOf<DivideByZeroException>());
        }
        #endregion

        #region SecurityTests
        [Test]
        [Category("Security")]
        public void IOForbiddenTest()
        {
            Exception e = Assert.ThrowsAsync(typeof(ExceptionRunningUserCodeException), () =>
                runner.CompileAndRunAsync(GetUsings(new string[] { "System", "System.IO" })
                    .Append(WrapInMain(@"using (var s = new StreamWriter(""test.txt"")) { s.WriteLine(""Testing""); }"))
                    .ToString()));
            Assert.That(e.InnerException, Is.TypeOf<SecurityException>());
        }

        [Test]
        [Category("Security")]
        public void IOForbiddenWithoutMainTest()
        {
            Exception e = Assert.ThrowsAsync(typeof(ExceptionRunningUserCodeException), () =>
            runner.CompileAndRunAsync(GetUsings(new string[] { "System", "System.IO" })
                            .Append(@"using (var s = new StreamWriter(""test.txt"")) { s.WriteLine(""Testing""); }").ToString()));
            Assert.That(e.InnerException, Is.TypeOf<SecurityException>());
        }
        #endregion

        #region LoggerTests

        [Test]
        [Category("Logger")]
        public void ExceptionNotLoggedTest()
        {
            // Arrange
            var log = new Mock<IExceptionLogger>();
            CodeRunner runner = new CodeRunner(log.Object);
            string code = GetUsings(new string[] { "System" })
                    .Append(WrapInMain(@"int i = 0; int j = 5 / i;")).ToString();

            // Act
            Exception e = Assert.ThrowsAsync(typeof(ExceptionRunningUserCodeException), () =>
                runner.CompileAndRunAsync(code));

            // Assert
            log.Verify(l => l.LogException(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        [Category("Logger")]
        public void CompilerErrorNotLoggedTest()
        {
            // Arrange
            var log = new Mock<IExceptionLogger>();
            CodeRunner runner = new CodeRunner(log.Object);
            string code = GetUsings(new string[] { "System" })
                    .Append(WrapInMain(@"int i")).ToString();

            // Act
            Exception e = Assert.ThrowsAsync(typeof(CompilationErrorException), () =>
                runner.CompileAndRunAsync(code));

            // Assert
            log.Verify(l => l.LogException(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        [Category("Logger")]
        public void TimeoutLoggedTest()
        {
            // Arrange
            var log = new Mock<IExceptionLogger>();
            CodeRunner runner = new CodeRunner(log.Object);
            string code = GetUsings(new string[] { "System" })
                    .Append(WrapInMain(@"while(true) {}")).ToString();

            // Act
            Exception e = Assert.ThrowsAsync(typeof(TimeoutException), () =>
                runner.CompileAndRunAsync(code));

            // Assert
            log.Verify(l => l.LogException(e, code));
        }
        #endregion

    }
}
