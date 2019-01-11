namespace CodingTrainer.CodingTrainerEntityFramework.Migrations
{
    using CodingTrainer.CodingTrainerModels;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using System.IO;
    using CodingTrainer.CSharpRunner.Assessment.Methods;
    using System.Collections.Generic;
    using CodingTrainer.CSharpRunner.Assessment;

    internal sealed class Configuration : DbMigrationsConfiguration<Contexts.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Contexts.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            // Un-comment to debug:

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            var resourceName = "CodingTrainer.CodingTrainerEntityFramework.Data.SeedData.xml";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            // Read data
            Type[] referencedTypes =
            {
                typeof(Exercise),
                typeof(AlwaysPassAssessment),
                typeof(CheckAllOutputAssessment),
                typeof(CheckLastLineOfOutputAssessment),
                typeof(VariableTypeAssessment)
            };

            DataContractSerializer ser = new DataContractSerializer(typeof(List<Chapter>), referencedTypes);

            var chapters = (List<Chapter>)ser.ReadObject(stream);

            // Save the chapters
            context.Chapters.AddOrUpdate(c => c.ChapterNo, chapters.ToArray());
            chapters.ForEach(c =>
            {
                if (c.Exercises != null)
                {
                    // Save the exercises
                    foreach (Exercise e in c.Exercises)
                    {
                        // Some data is kept in files rather than in the XML, for readability. Fetch that data now
                        e.Content = LoadManifestResourceFile($"Content{c.ChapterNo}-{ e.ExerciseNo}.cshtml", "<p>No course material is available for this exercise</p>");
                        e.DefaultCode = LoadManifestResourceFile($"Default{c.ChapterNo}-{ e.ExerciseNo}.csx", "// Enter your code here:" + Environment.NewLine + Environment.NewLine);
                        e.HiddenCodeHeader = LoadManifestResourceFile($"Header{c.ChapterNo}-{ e.ExerciseNo}.csx");
                    }
                    context.Exercises.AddOrUpdate(e => new { e.ChapterNo, e.ExerciseNo }, c.Exercises.ToArray());

                    // Save the assessments
                    foreach (Exercise e in c.Exercises)
                    {
                        if (e.Assessments != null)
                        {
                            context.Assessments.AddOrUpdate<AssessmentMethodBase>(a => new { a.ChapterNo, a.ExerciseNo, a.Title }, e.Assessments.Select(a => (AssessmentMethodBase)a).ToArray());
                        }
                    }
                }
            });
        }

        private string LoadManifestResourceFile(string filename, string notFoundDefault = null)
        {
            var fullFilename = "CodingTrainer.CodingTrainerEntityFramework.Data." + filename;
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullFilename);
            if (fileStream == null)
            {
                return notFoundDefault;
            }
            else
            {
                var fileReader = new StreamReader(fileStream);
                return fileReader.ReadToEnd();
            }
        }
    }
}
