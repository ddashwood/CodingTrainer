using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHost
{
    public class IdeServices : IIdeServices
    {
        //Task creating;

        //AdhocWorkspace workspace;
        //Project project;
        //Document document;
        //HostLanguageServices cSharpServices;
        //object cSharpCompletionService;
        //object trigger;

        //public IdeServices()
        //{
        //    creating = Task.Run(() =>
        //    {
        //        // Create a document

        //        workspace = new AdhocWorkspace();
        //        project = workspace.AddProject("CodeRunner", "C#");
        //        project = project.AddMetadataReferences(References.ReferencedAssembliesData);
        //        document = project.AddDocument("script", string.Empty);
        //        cSharpServices = workspace.Services.GetLanguageServices("C#");

        //        // Get the completion services

        //        var completionServiceFactory = MakeObject("CSharpCompletionServiceFactory");
        //        var createLanguageServiceMethod = completionServiceFactory.GetType().GetMethod("CreateLanguageService");
        //        cSharpCompletionService = createLanguageServiceMethod.Invoke(completionServiceFactory, new object[] { cSharpServices });

        //        // Make a completion trigger

        //        trigger = MakeObject("CompletionTrigger");
        //    });
        //}

        public async Task<IEnumerable<Diagnostic>> GetDiagnostics(string code, CancellationToken token)
        {
            var compilation = await Compiler.GetCompilation(code);
            token.ThrowIfCancellationRequested();
            var diagnostics = await GetDiagnostics(compilation, token);
            token.ThrowIfCancellationRequested();

            return diagnostics;
        }

        private static async Task<IEnumerable<Diagnostic>> GetDiagnostics(Compilation compilation, CancellationToken token = default(CancellationToken))
        {
            var any = false;
            var diagnostics = new List<Diagnostic>();

            // Will we ever have more than one syntax tree? Probably not if we're just compiling
            // a script, but use a foreach just in case
            foreach (var tree in compilation.SyntaxTrees)
            {
                token.ThrowIfCancellationRequested();
                var treeDiagnostics = await Task.Run(() => compilation.GetSemanticModel(tree).GetDiagnostics());
                if (treeDiagnostics.Length > 0)
                {
                    any = true;
                    diagnostics.AddRange(treeDiagnostics);
                }
            }

            token.ThrowIfCancellationRequested();
            if (any) return diagnostics;
            return null;
        }

        public async Task<IEnumerable<string>> GetCompletions(string code, int position, CancellationToken token)
        {
            Workspace workspace = new AdhocWorkspace();
            List<string> symbolNames = new List<string>();

            var compilation = await Compiler.GetCompilation(code);
            foreach (var tree in compilation.SyntaxTrees)
            {
                SemanticModel sm = compilation.GetSemanticModel(tree);
                var symbols = await Recommender.GetRecommendedSymbolsAtPositionAsync(sm, position, workspace, cancellationToken: token);
                symbolNames.AddRange(symbols.OrderBy(s => s.Name).Select(s => s.Name).Distinct());
            }

            return symbolNames;
        }


        //public async Task<IEnumerable<string>> GetCompletions(string code, int position, CancellationToken token)
        //{
        //    await creating;

        //    document = document.WithText(SourceText.From(code));

        //    // Now get the completion suggestions

        //    // http://source.roslyn.io/#Microsoft.CodeAnalysis.Features/Completion/CompletionServiceWithProviders.cs,da97458ac8890982
        //    var parms = new object[] {
        //        document,
        //        position,
        //        trigger,
        //        null, // Roles
        //        null, // Options
        //        null  // CancellationToken
        //    };

        //    var getCompletionsAsyncMethod = cSharpCompletionService.GetType().GetMethod("GetCompletionsAsync");
        //    token.ThrowIfCancellationRequested();
        //    var completionList = await (Task<CompletionList>)getCompletionsAsyncMethod.Invoke(cSharpCompletionService, parms);
        //    token.ThrowIfCancellationRequested();
        //    return completionList?.Items.Select(c => c.DisplayText);
        //}

        //// Reflection services, used to generate internal class of Roslyn
        //// First of all some data structors for defining the supported classes
        //private struct ClassDetails
        //{
        //    public string Name { get; set; }
        //    public string AssemblyFullName { get; set; }
        //    public ClassDetails(string name, string assemblyFullName)
        //    {
        //        Name = name;
        //        AssemblyFullName = assemblyFullName;
        //    }
        //}
        //private ImmutableDictionary<string, ClassDetails> Methods = new Dictionary<string, ClassDetails>()
        //{
        //    { "CSharpCompletionServiceFactory", new ClassDetails("Microsoft.CodeAnalysis.CSharp.Completion.CSharpCompletionServiceFactory", "Microsoft.CodeAnalysis.CSharp.Features, Version=2.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35") },
        //    { "CompletionTrigger", new ClassDetails("Microsoft.CodeAnalysis.Completion.CompletionTrigger", "Microsoft.CodeAnalysis.Features, Version=2.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35") }
        //}.ToImmutableDictionary();




        //// N.b. no ability to pass constructor parameters (yet!)
        //private object MakeObject(string name)
        //{
        //    var classDetails = Methods[name];
        //    var assembly = Assembly.Load(classDetails.AssemblyFullName);
        //    var type = assembly.GetType(classDetails.Name);

        //    var instance = Activator.CreateInstance(type, true);
        //    return instance;
        //}
    }
}
