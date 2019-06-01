using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.PartiallyTrusted
{
    // N.b. this runs inside the sandbox, and is not trusted
    public class CompiledCodeRunner
    {
        // Find the most appropriate entry point in the compiled code
        // Then, run from that entry point.
        public static void RunIt(byte[] compiledCode, byte[] pdb)
        {
            var assembly = Assembly.Load(compiledCode, pdb);
            var type = assembly.GetType("CodingTrainerExercise");
            var main = type?.GetMethod("Main");

            object ret = null;
            if (main != null)
            {
                var parameters = new object[0];
                ret = main.Invoke(null, parameters);
            }
            else if (assembly.EntryPoint != null)
            {
                var submissionArray = new object[2];
                var parameters = new object[] { submissionArray };
                ret = assembly.EntryPoint.Invoke(null, parameters);
            }

            // If we invoke the entry point, rather than a method, it gets wrapped in a Task.
            // This hides the exception from us.
            // Safest to always check if this has happened.
            if (ret is Task task)
            {
                if (task.Status == TaskStatus.Running) task.Wait();
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }

        }
    }
}
