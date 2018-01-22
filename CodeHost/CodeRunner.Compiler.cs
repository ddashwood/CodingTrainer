using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using CodingTrainer.CSharpRunner.PartiallyTrusted;
using System;
using System.IO;
using System.Security.Policy;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    partial class CodeRunner
    {
        private static class Compiler
        {
            private static readonly Type[] referencedTypes =
                { typeof (CompiledCodeRunner) };

            public static byte[] Compile(string code, out byte[] pdb)
            {
                using (var memoryStream = new MemoryStream())
                using (var pdbStream = new MemoryStream())
                {
                    var script = CSharpScript.Create<object>(code);
                    var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication, scriptClassName: "EntryPoint");
                    var compilation = script.GetCompilation().WithOptions(options);
                    foreach (Type t in referencedTypes)
                        compilation = compilation.AddReferences(MetadataReference.CreateFromFile(t.Assembly.Location));

                    var emitResult = compilation.Emit(memoryStream, pdbStream);

                    if (!emitResult.Success)
                    {
                        throw new CompilationErrorException("Error during compilation", emitResult.Diagnostics);
                    }

                    byte[] compiledCode = memoryStream.ToArray();
                    if (compiledCode.Length > 102400)
                    {
                        throw new PolicyException("The compiled code is too large");
                    }
                    pdb = pdbStream.ToArray();
                    return compiledCode;
                }
            }

        }
    }
}
