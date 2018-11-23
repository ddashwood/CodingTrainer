using System;
using System.Reflection;

namespace CodingTrainer.CSharpRunner.SandboxHost
{
    // This clas is fully trusted. It runs inside the sandbox, but can request full permissions.
    // Used to change Console streams, and then to invoke the CompiledCodeRunner
    public class Sandboxer : MarshalByRefObject
    {
        public void ExecuteUntrustedCode(string assemblyName, string typeName, string entryPoint, Object[] parameters)
        {
            MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);
            ExecuteTarget(target, parameters);
        }

        protected virtual void ExecuteTarget(MethodInfo target, Object[] parameters)
        {
            // Invoke the CompiledCodeRunner
            target.Invoke(null, parameters);
        }
    }
}
