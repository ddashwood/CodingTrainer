using CodingTrainer.CSharpRunner.Assessment;
using CodingTrainer.CSharpRunner.CodeHost;
using CodingTrainer.CSharpRunner.TestingCommon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.AssessmentTests
{
    class PreProgrammedTextReaderTests : CodeHostTestBase
    {
        [Test]
        [Category("Assessment")]
        public async Task ConsoleInTest()
        {
            var compiled = await runner.CompileAsync(GetUsings(new string[] { "System" }).
                Append(WrapInMain("Console.WriteLine(\"You entered \" + Console.ReadLine()); " +
                                  "Console.WriteLine(\"You entered \" + Console.ReadLine());")).ToString());

            await runner.RunAsync(compiled, new PreProgrammedTextReader("Test1\r\nTest2\r\n"));

            Assert.AreEqual("Test1\r\nYou entered Test1\r\nTest2\r\nYou entered Test2\r\n", console.ToString());
        }

        [Test]
        [Category("Assessment")]
        public async Task ReadTooMuchTest()
        {
            var compiled = await runner.CompileAsync(GetUsings(new string[] { "System" }).
                Append(WrapInMain("Console.ReadLine(); Console.ReadLine();")).ToString());

            var e = Assert.ThrowsAsync<ExceptionRunningUserCodeException>(() => runner.RunAsync(compiled, new PreProgrammedTextReader("Test1\r\n")));
            Assert.That(e.InnerException, Is.TypeOf<EndOfStreamException>());
        }
    }
}
