using CodingTrainer.CodingTrainerEntityFramework.Contexts;
using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CSharpRunner.Assessment;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CodingTrainer.CodingTrainerEntityFramework.Seed
{
    internal static class Seeder
    {
        private const string seedRoot = "SeedData";
        
        public static void Seed(ApplicationDbContext context)
        {
            var codeBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var rootFolder = Path.Combine(new Uri(codeBase).LocalPath, seedRoot);
            var di = new DirectoryInfo(rootFolder);
            foreach(var dir in di.GetDirectories("Ch*"))
            {
                int chapterNo = int.Parse(new Regex(@"^Ch(?<num>-?\d+)(\-.*)?$").Match(dir.Name).Groups["num"].Value);
                SeedChapter(context, chapterNo, dir.FullName);
            }
        }

        private static void SeedChapter(ApplicationDbContext context, int chapterNo, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Chapter));
            using (var stream = new StreamReader(Path.Combine(path, "chapter.xml")))
            {
                Chapter chapter = (Chapter)serializer.Deserialize(stream);
                chapter.ChapterNo = chapterNo;
                context.Chapters.AddOrUpdate(chapter);
                context.SaveChanges();
            }

            var di = new DirectoryInfo(path);
            foreach (var dir in di.GetDirectories("Ex*"))
            {
                int exerciseNo = int.Parse(new Regex(@"^Ex(?<num>-?\d+)(\-.*)?$").Match(dir.Name).Groups["num"].Value);
                SeedExercise(context, chapterNo, exerciseNo, dir.FullName);
            }
        }

        private static void SeedExercise(ApplicationDbContext context, int chapterNo, int exerciseNo, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Exercise));
            using (var stream = new StreamReader(Path.Combine(path, "exercise.xml")))
            {
                Exercise exercise = (Exercise)serializer.Deserialize(stream);
                exercise.ChapterNo = chapterNo;
                exercise.ExerciseNo = exerciseNo;
                exercise.DefaultCode = ReadFile(path, "default.csx", "// Enter your code here:" + Environment.NewLine + Environment.NewLine);
                exercise.HiddenCodeHeader = ReadFile(path, "header.csx");
                exercise.Content = ReadFile(path, "content.cshtml", "<p>No course material is available for this exercise</p>");

                context.Exercises.AddOrUpdate(exercise);
                context.SaveChanges();
            }

            var di = new DirectoryInfo(path);
            foreach (var dir in di.GetDirectories("Ag*"))
            {
                int sequence = int.Parse(new Regex(@"^Ag(?<num>-?\d+)(\-.*)?$").Match(dir.Name).Groups["num"].Value);
                SeedAssessmentGroup(context, chapterNo, exerciseNo, sequence, dir.FullName);
            }
        }

        private static void SeedAssessmentGroup(ApplicationDbContext context, int chapterNo, int exerciseNo, int sequence, string path)
        {
            AssessmentGroup assessmentGroup;

            XmlSerializer serializer = new XmlSerializer(typeof(AssessmentGroup));
            using (var stream = new StreamReader(Path.Combine(path, "asmt-group.xml")))
            {
                assessmentGroup = (AssessmentGroup)serializer.Deserialize(stream);
                assessmentGroup.ChapterNo = chapterNo;
                assessmentGroup.ExerciseNo = exerciseNo;
                assessmentGroup.Sequence = sequence;
                context.AssessmentGroups.AddOrUpdate(g => new { g.ChapterNo, g.ExerciseNo, g.Sequence }, assessmentGroup);
                context.SaveChanges();
            }

            var di = new DirectoryInfo(path);
            foreach(var dir in di.GetDirectories("As*"))
            {
                int assessmentSequence = int.Parse(new Regex(@"^As(?<num>-?\d+)(\-.*)?$").Match(dir.Name).Groups["num"].Value);
                SeedAssessment(context, assessmentGroup.AssessmentGroupId, assessmentSequence, dir.FullName);
            }
        }

        private static void SeedAssessment(ApplicationDbContext context, int assessmentGroupId, int sequence, string path)
        {
            var assessmentAssembly = typeof(AssessmentMethodBase).Assembly;
            AssessmentMethodBase assessment;

            // Find the assessment class;
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(path, "assessment.xml"));
            string assessmentClass = doc.GetElementsByTagName("FullClassName").Item(0).InnerText;
            Type assessmentType = assessmentAssembly.GetType(assessmentClass);

            // Deserialize the assessment
            XmlSerializer serializer = new XmlSerializer(assessmentType);
            using (XmlReader reader = new XmlNodeReader(doc))
            {
                assessment = (AssessmentMethodBase)serializer.Deserialize(reader);
                assessment.AssessmentGroupId = assessmentGroupId;
                assessment.Sequence = sequence;
            }

            // Add any fields that have their own file
            var di = new DirectoryInfo(path);
            foreach(var file in di.GetFiles())
            {
                if (file.Name == "assessment.xml") continue;

                string fieldName = Path.GetFileNameWithoutExtension(file.Name);
                string value = File.ReadAllText(file.FullName);

                assessmentType.InvokeMember(fieldName, BindingFlags.SetProperty, Type.DefaultBinder, assessment, new object[] { value });
            }

            // Save the assessment
            context.Assessments.AddOrUpdate(a => new { a.AssessmentGroupId, a.Sequence }, assessment);
            context.SaveChanges();
        }

        private static string ReadFile(string folder, string file, string defaultText = null)
        {
            string path = Path.Combine(folder, file);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return defaultText;
        }
    }
}
