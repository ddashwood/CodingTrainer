using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CSharpRunner.Assessment;
using CodingTrainer.CSharpRunner.Assessment.Methods.ByRunning;
using CodingTrainer.CSharpRunner.CodeHost;
using CodingTrainer.CSharpRunner.TestingCommon;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CSharpRunner.AssessmentTests
{
    public class AssessmentRunnerTests : CodeHostTestBase
    {
        #region Setup
        private List<AssessmentGroup> assessmentGroups;

        public override void OneTimeSetup()
        {
            OneTimeSetup(registerConsoleWrite: false);
        }
        public override void Setup()
        {
            base.Setup();

            assessmentGroups = new List<AssessmentGroup>
            {
                new AssessmentGroup
                {
                    Title = "TestGroup",
                    Assessments = new List<AssessmentBase>()
                    {
                        // This assessment will fail for some of the tests in this test class
                        new CheckLastLineOfOutputAssessment
                        {
                            Title = "TestAssessment1",
                            ExpectedResult = "Test Output",
                            RequiredAccuracy = 1,
                            ConsoleInText = string.Empty
                        },
                        // This assessment will pass for all of the tests
                        new CheckLastLineOfOutputAssessment
                        {
                            Title = "TestAssessment2",
                            ExpectedResult = "Output",
                            RequiredAccuracy = 0.1,
                            ConsoleInText = string.Empty
                        }
                    }
                },
                new AssessmentGroup
                {
                    Title = "TestSecondGroup",
                    Assessments = new List<AssessmentBase>()
                    {
                        // This assessment will pass for all of the tests
                        new CheckLastLineOfOutputAssessment
                        {
                            Title = "TestAssessment3",
                            ExpectedResult = "Output",
                            RequiredAccuracy = 0.1,
                            ConsoleInText = string.Empty
                        }
                    }
                }
            };
        }
        #endregion

        [Test, Category("Assessment")]
        public async Task AssessmentGroupStartMessageFalseTest()
        {
            // Arrange
            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestGroup", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroupStartMessageTrueTest()
        {
            // Arrange
            assessmentGroups.First().ShowAutoMessageOnStart = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("TestGroup", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroupPassMessageFalseTest()
        {
            // Arrange
            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestGroup passed", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroupPassMessageTrueTest()
        {
            // Arrange
            assessmentGroups.First().ShowAutoMessageOnPass = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("TestGroup passed", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroupFailMessageFalseTest()
        {
            // Arrange
            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestGroup failed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroupFailMessageTrueTest()
        {
            // Arrange
            assessmentGroups.First().ShowAutoMessageOnFail = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("TestGroup failed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroupNoFailMessageOnPassTest()
        {
            // Arrange
            assessmentGroups.First().ShowAutoMessageOnFail = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestGroup failed", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroupNoPassMessageOnFailTest()
        {
            // Arrange
            assessmentGroups.First().ShowAutoMessageOnPass = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestGroup passed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentStartMessageFalseTest()
        {
            // Arrange
            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestAssessment1", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentStartMessageTrueTest()
        {
            // Arrange
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnStart = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("TestAssessment1", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentPassMessageFalseTest()
        {
            // Arrange
            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestAssessment1 passed", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentPassMessageTrueTest()
        {
            // Arrange
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnPass = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("TestAssessment1 passed", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentFailMessageFalseTest()
        {
            // Arrange
            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestAssessment1 failed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentFailMessageTrueTest()
        {
            // Arrange
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnFail = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("TestAssessment1 failed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentNoFailMessageOnPassTest()
        {
            // Arrange
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnFail = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestAssessment1 failed", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentNoPassMessageOnFailTest()
        {
            // Arrange
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnPass = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestAssessment1 passed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroupAbortTest()
        {
            // Arrange
            assessmentGroups.First().Assessments.First().EndAssessmentGroupOnFail = true;
            assessmentGroups.First().Assessments.ElementAt(1).ShowAutoMessageOnStart = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestAssessment2", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentNoGroupAbortTest()
        {
            // Arrange
            assessmentGroups.First().Assessments.First().EndAssessmentGroupOnFail = false;
            assessmentGroups.First().Assessments.ElementAt(1).ShowAutoMessageOnStart = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("TestAssessment2", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentsAbortTest()
        {
            // Arrange

            assessmentGroups.First().EndAssessmentsOnFail = true;
            assessmentGroups.ElementAt(1).ShowAutoMessageOnStart = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("TestSecondGroup", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentsNoAbortTest()
        {
            // Arrange

            assessmentGroups.First().EndAssessmentsOnFail = false;
            assessmentGroups.ElementAt(1).ShowAutoMessageOnStart = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Wrong Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("TestSecondGroup", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentIncludeOutputTest()
        {
            // Arrange

            ((AssessmentByRunningBase)assessmentGroups.First().Assessments.First()).ShowScriptRunning = true;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.Contains("Test Output", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentDoesNotIncludeOutputTest()
        {
            // Arrange

            ((AssessmentByRunningBase)assessmentGroups.First().Assessments.First()).ShowScriptRunning = false;

            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain("Console.WriteLine(\"Test Output\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;

            // Act
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;

            // Assert
            StringAssert.DoesNotContain("Test Output", console.ToString());
            Assert.IsTrue(result);
        }
    }
}
