using CodingTrainer.CSharpRunner.Assessment;
using CodingTrainer.CSharpRunner.TestingCommon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.AssessmentTests
{
    class AssessmentRunnerTests: CodeHostTestBase
    {
        [Test]
        [Category("Assessment")]
        public async Task ConsoleInTest()
        {
            var compiled = await runner.Compile(GetUsings(new string[] { "System" }).
                Append(WrapInMain("Console.WriteLine(\"You entered \" + Console.ReadLine());")).ToString());

            await runner.Run(compiled, new PreProgrammedTextReader("Test1\r\n"));

            Assert.AreEqual("Test1\r\nYou entered Test1\r\n", console.ToString());
        }
    }
}
