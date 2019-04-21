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
        public async Task AssessmentGroup_StartMessage_False_Ok()
        {
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);
            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            StringAssert.DoesNotContain("TestGroup", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroup_StartMessage_True_Ok()
        {
            assessmentGroups.First().ShowAutoMessageOnStart = true;
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            StringAssert.Contains("TestGroup", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroup_Without_PassMessage_Ok()
        {
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            StringAssert.DoesNotContain("TestGroup passed", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroup_With_PassMessage_Ok()
        {
            assessmentGroups.First().ShowAutoMessageOnPass = true;
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            StringAssert.Contains("TestGroup passed", console.ToString());
            Assert.IsTrue(result);
        }

        //+
        [Test, Category("Assessment")]
        public async Task AssessmentGroup_FailMessage_False_Ok()
        {
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            StringAssert.DoesNotContain("TestGroup failed", console.ToString());
            Assert.IsFalse(result);
        }
        //+
        [Test, Category("Assessment")]
        public async Task AssessmentGroup_FailMessage_True_Ok()
        {
            assessmentGroups.First().ShowAutoMessageOnFail = true;
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.Contains("TestGroup failed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task AssessmentGroup_No_FailMessage_OnPass_Ok()
        {
            assessmentGroups.First().ShowAutoMessageOnFail = true;
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestGroup failed", console.ToString());
            Assert.IsTrue(result);
        }

        //+
        [Test, Category("Assessment")]
        public async Task AssessmentGroup_No_PassMessage_OnFail_Ok()
        {
            assessmentGroups.First().ShowAutoMessageOnPass = true;
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestGroup passed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task Assessment_StartMessage_False_Ok()
        {
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestAssessment1", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task Assessment_StartMessage_True_Ok()
        {
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnStart = true;
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.Contains("TestAssessment1", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task Assessment_PassMessage_False_Ok()
        {
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestAssessment1 passed", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task Assessment_PassMessage_True_Ok()
        {
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnPass = true;
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.Contains("TestAssessment1 passed", console.ToString());
            Assert.IsTrue(result);
        }


        //+
        [Test, Category("Assessment")]
        public async Task Assessment_FailMessage_False_Ok()
        {
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestAssessment1 failed", console.ToString());
            Assert.IsFalse(result);
        }

        //+
        [Test, Category("Assessment")]
        public async Task Assessment_FailMessage_True_Ok()
        {
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnFail = true;
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.Contains("TestAssessment1 failed", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task Assessment_No_FailMessage_OnPass_Ok()
        {
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnFail = true;
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestAssessment1 failed", console.ToString());
            Assert.IsTrue(result);
        }

        //+
        [Test, Category("Assessment")]
        public async Task Assessment_No_PassMessage_OnFail_Ok()
        {
            assessmentGroups.First().Assessments.First().ShowAutoMessageOnPass = true;
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestAssessment1 passed", console.ToString());
            Assert.IsFalse(result);
        }

        //+
        [Test, Category("Assessment")]
        public async Task Assessment_Group_Abort_Ok()
        {
            assessmentGroups.First().Assessments.First().EndAssessmentGroupOnFail = true;
            assessmentGroups.First().Assessments.ElementAt(1).ShowAutoMessageOnStart = true;
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestAssessment2", console.ToString());
            Assert.IsFalse(result);
        }

        //+
        [Test, Category("Assessment")]
        public async Task Assessment_NoGroup_Abort_Ok()
        {
            // Arrange
            assessmentGroups.First().Assessments.First().EndAssessmentGroupOnFail = false;
            assessmentGroups.First().Assessments.ElementAt(1).ShowAutoMessageOnStart = true;
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.Contains("TestAssessment2", console.ToString());
            Assert.IsFalse(result);
        }

        //+
        [Test, Category("Assessment")]
        public async Task Assessments_Abort_Ok()
        {
            // Arrange

            assessmentGroups.First().EndAssessmentsOnFail = true;
            assessmentGroups.ElementAt(1).ShowAutoMessageOnStart = true;
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("TestSecondGroup", console.ToString());
            Assert.IsFalse(result);
        }
        
        //+
        [Test, Category("Assessment")]
        public async Task Assessments_No_Abort_Ok()
        {
            assessmentGroups.First().EndAssessmentsOnFail = false;
            assessmentGroups.ElementAt(1).ShowAutoMessageOnStart = true;
            const string testOuput = "Wrong Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.Contains("TestSecondGroup", console.ToString());
            Assert.IsFalse(result);
        }

        [Test, Category("Assessment")]
        public async Task Assessment_Include_Output_Ok()
        {
            ((AssessmentByRunningBase)assessmentGroups.First().Assessments.First()).ShowScriptRunning = true;
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.Contains("Test Output", console.ToString());
            Assert.IsTrue(result);
        }

        [Test, Category("Assessment")]
        public async Task Assessment_NotIncluded_Output_Ok()
        {
            ((AssessmentByRunningBase)assessmentGroups.First().Assessments.First()).ShowScriptRunning = false;
            const string testOuput = "Test Output";
            var assessmentRunner = ArrangeAssesmentGroup(testOuput);

            var result = await RunAssessmentAndDeattachConsoleWriter(assessmentRunner);

            // Assert
            StringAssert.DoesNotContain("Test Output", console.ToString());
            Assert.IsTrue(result);
        }

        private AssessmentRunner ArrangeAssesmentGroup(string testOutput)
        {
            var code = GetUsings(new string[] { "System" })
                .Append(WrapInMain($"Console.WriteLine(\"{testOutput}\");"));
            var assessmentRunner = new AssessmentRunner(runner, code.ToString());
            assessmentRunner.ConsoleWrite += OnConsoleWrite;
            return assessmentRunner;
        }

        private async Task<bool> RunAssessmentAndDeattachConsoleWriter(AssessmentRunner assessmentRunner)
        {
            bool result = await assessmentRunner.RunAssessmentsAsync(assessmentGroups);
            assessmentRunner.ConsoleWrite -= OnConsoleWrite;
            return result;
        }
    }
}
