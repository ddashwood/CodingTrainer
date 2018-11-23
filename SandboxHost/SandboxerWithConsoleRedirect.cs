using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.SandboxHost
{
    public class SandboxerWithConsoleRedirect:Sandboxer
    {
        private StringWriter newConsoleOut;
        private TextReader newConsoleIn;
        bool consoleRedirected = false;

        public void RedirectConsole(StringWriter newConsoleOut, TextReader newConsoleIn)
        {
            this.newConsoleOut = newConsoleOut;
            this.newConsoleIn = newConsoleIn;
            consoleRedirected = true;
        }


        protected override void ExecuteTarget(MethodInfo target, object[] parameters)
        {
            TextWriter oldConsoleOut = null;
            TextReader oldConsoleIn = null;
            try
            {
                if (consoleRedirected)
                {
                    (new PermissionSet(PermissionState.Unrestricted)).Assert();
                    oldConsoleOut = Console.Out;
                    oldConsoleIn = Console.In;
                    Console.SetOut(newConsoleOut);
                    Console.SetIn(newConsoleIn);
                    CodeAccessPermission.RevertAssert();
                }

                base.ExecuteTarget(target, parameters);
            }
            finally
            {
                if (consoleRedirected)
                {
                    (new PermissionSet(PermissionState.Unrestricted)).Assert();
                    Console.SetOut(oldConsoleOut);
                    Console.SetIn(oldConsoleIn);
                    CodeAccessPermission.RevertAssert();
                }
            }
        }
    }
}
