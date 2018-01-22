using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace CodingTrainer.CSharpRunner.SandboxHost
{
    // This clas is fully trusted. It runs inside the sandbox, but can request full permissions.
    // Used to change Console streams, and then to invoke the CompiledCodeRunner
    public class Sandboxer : MarshalByRefObject
    {
        public void ExecuteUntrustedCode(string assemblyName, string typeName, string entryPoint, Object[] parameters, StringWriter newConsoleOut, TextReader newConsoleIn)
        {
            MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);
            ExecuteTarget(target, parameters, newConsoleOut, newConsoleIn);
        }

        public void ExecuteTarget(MethodInfo target, Object[] parameters, StringWriter newConsoleOut, TextReader newConsoleIn)
        {
            TextWriter oldConsoleOut = null;
            TextReader oldConsoleIn = null;
            try
            {
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                oldConsoleOut = Console.Out;
                oldConsoleIn = Console.In;
                Console.SetOut(newConsoleOut);
                Console.SetIn(newConsoleIn);
                CodeAccessPermission.RevertAssert();

                // Invoke the CompiledCodeRunner
                target.Invoke(null, parameters);
            }

            finally
            {
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                Console.SetOut(oldConsoleOut);
                Console.SetIn(oldConsoleIn);
                CodeAccessPermission.RevertAssert();
            }
        }
    }
}
