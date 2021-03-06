﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public partial class CodeRunner
    {
        internal static class Compiler
        {
            public static async Task<Compilation> GetCompilationAsync(string code)
            {
                Compilation compilation = null;
                await Task.Run(() =>
                {
                    var script = CSharpScript.Create<object>(code);
                    var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication, scriptClassName: "CodingTrainerExercise");
                    compilation = script.GetCompilation().WithOptions(options);

                    compilation = compilation.AddReferences(References.ReferencedAssembliesData);
                });

                return compilation;
            }

            public static async Task<(byte[] result, byte[] pdb)> EmitAsync(Compilation compilation)
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
}
