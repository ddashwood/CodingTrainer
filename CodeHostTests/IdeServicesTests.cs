using CodingTrainer.CSharpRunner.CodeHost;
using CodingTrainer.CSharpRunner.TestingCommon;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.CodeHostTests
{
    class IdeServicesTests:CodeHostTestBase
    {
        private IIdeServices ideServices = new IdeServices(new CodeRunner());

        #region ParameterTests

        [Test]
        [Category("Paramters")]
        public async Task ParametersTest()
        {
            IEnumerable<ISymbol> actual = await ideServices.GetOverloadsAndParametersAsync("private static void X() { System.Console.WriteLine(\"Blah blah blah\")  }", 53);
            Assert.AreEqual(19, actual.Count()); 
        }

        [Test]
        [Category("Paramters")]
        public async Task InvalidParametersTest()
        {
            IEnumerable<ISymbol> actual = await ideServices.GetOverloadsAndParametersAsync("private static void X() { System.Console.WriteLine(\"Blah blah blah\")  }", 0);
            Assert.AreEqual(null, actual);
        }
        #endregion

        #region CompletionsTests

        [Test]
        [Category("Completions")]
        public async Task SuggestionsTest()
        {
            IEnumerable<string> actual = await ideServices.GetCompletionStringsAsync("using Sys",9);
            string[] expected = { "System", "Microsoft" };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        [Category("Completions")]
        public async Task MultiSuggestionsTest()
        {
            await ideServices.GetCompletionStringsAsync("using Sys", 9);
            IEnumerable<string> actual = await ideServices.GetCompletionStringsAsync("using System.", 13);
            string[] expected = { "Collections", "Configuration", "Deployment", "Diagnostics", "Globalization", "IO", "Reflection", "Resources", "Runtime", "Security", "Text", "Threading" };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        [Category("Completions")]
        public async Task ConcurrentSuggestionsTest()
        {
            Task t = ideServices.GetCompletionStringsAsync("using Sys", 9); // To test concurrency, don't await this task!
            IEnumerable<string> actual = await ideServices.GetCompletionStringsAsync("using System.", 13);
            string[] expected = { "Collections", "Configuration", "Deployment", "Diagnostics", "Globalization", "IO", "Reflection", "Resources", "Runtime", "Security", "Text", "Threading" };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        #endregion

        #region DiagnosticsTests
        [Test]
        [Category("Diagnostics")]
        public async Task NoDiagnosticsTest()
        {
            var diagnostics = await ideServices.GetDiagnosticsAsyc(GetUsings(new string[] { "System" })
                .Append(@"Console.WriteLine(""Hello World!"");").ToString());
            Assert.AreEqual(null, diagnostics);
        }

        [Test]
        [Category("Diagnostics")]
        public async Task ErrorTest()
        {
            var diagnostics = await ideServices.GetDiagnosticsAsyc(GetUsings(new string[] { "System" })
                .Append(@"Console.Writeline(""Hello World!"");").ToString());
            Assert.AreEqual(1, diagnostics.Count());
            Assert.AreEqual("'Console' does not contain a definition for 'Writeline'", diagnostics.First().GetMessage());
        }

        [Test]
        [Category("Diagnostics")]
        public async Task ComplexErrorTest()
        {
            var diagnostics = await ideServices.GetDiagnosticsAsyc(GetUsings(new string[] { "System" })
                .Append(@"
for (int i = 0; i < 5; i++)
{
    Console.Writeline(i);
}
Console.WriteLine(""Test"");
if (5)
{
    return;
}").ToString());
            diagnostics = diagnostics.OrderBy(e => e.Location.SourceSpan.Start);
            Assert.AreEqual(2, diagnostics.Count());
            Assert.AreEqual("'Console' does not contain a definition for 'Writeline'", diagnostics.First().GetMessage());
            Assert.AreEqual("Cannot implicitly convert type 'int' to 'bool'", diagnostics.Last().GetMessage());
        }
        #endregion


    }
}
