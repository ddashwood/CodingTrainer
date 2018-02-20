using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using CodingTrainer.CSharpRunner.PartiallyTrusted;
using System;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    internal static class Compiler
    {
        public static async Task<Compilation> GetCompilation(string code)
        {
            Compilation compilation = null;
            await Task.Run(() =>
            {
                var script = CSharpScript.Create<object>(code);
                var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication, scriptClassName: "EntryPoint");
                compilation = script.GetCompilation().WithOptions(options);

                // Need to add the PartiallyTrusted assembly, as well as any assemblies the user might be using
                var referencedAssemblies = References.ReferencedAssembliesData.Add(References.FromType(typeof(CompiledCodeRunner)));

                compilation = compilation.AddReferences(referencedAssemblies);
            });

            return compilation;
        }

        public static async Task<(byte[] result, byte[] pdb)> Emit(Compilation compilation)
        {
            byte[] compiledCode = null;
            byte[] pdb = null;

            await Task.Run(() =>
            {
                using (var memoryStream = new MemoryStream())
                using (var pdbStream = new MemoryStream())
                {
                    var emitResult = compilation.Emit(memoryStream, pdbStream);

                    if (!emitResult.Success)
                    {
                        throw new CompilationErrorException("Error during compilation", emitResult.Diagnostics);
                    }

                    compiledCode = memoryStream.ToArray();
                    if (compiledCode.Length > 102400)
                    {
                        throw new PolicyException("The compiled code is too large");
                    }
                    pdb = pdbStream.ToArray();
                }
            });
            return (compiledCode, pdb);
        }
    }
}
