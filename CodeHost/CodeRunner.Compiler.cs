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
    partial class CodeRunner
    {
        private static class Compiler
        {
            private static readonly Type[] referencedTypes =
                { typeof (CompiledCodeRunner) };

            public static async Task<Compilation> GetCompilation(string code)
            {
                Compilation compilation = null;
                await Task.Run(() =>
                {
                    var script = CSharpScript.Create<object>(code);
                    var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication, scriptClassName: "EntryPoint");
                    compilation = script.GetCompilation().WithOptions(options);
                    foreach (Type t in referencedTypes)
                        compilation = compilation.AddReferences(MetadataReference.CreateFromFile(t.Assembly.Location));

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

            public static async Task<IEnumerable<Diagnostic>> GetDiagnostics(Compilation compilation)
            {
                var any = false;
                var diagnostics = new List<Diagnostic>();

                // Will we ever have more than one syntax tree? Probably not if we're just compiling
                // a script, but use a foreach just in case
                foreach (var tree in compilation.SyntaxTrees)
                {
                    var treeDiagnostics = await Task.Run(() => compilation.GetSemanticModel(tree).GetDiagnostics());
                    if (treeDiagnostics.Length > 0)
                    {
                        any = true;
                        diagnostics.AddRange(treeDiagnostics);
                    }
                }

                if (any) return diagnostics;
                return null;
            }
        }
    }
}
