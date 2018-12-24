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

            using (XmlReader reader = XmlReader.Create(stream))
            {
                // Read data

                XmlSerializer ser = new XmlSerializer(typeof(Chapter[]));
                var chapters = (Chapter[])ser.Deserialize(reader);

                // Save chapters
                context.Chapters.AddOrUpdate(c => c.ChapterNo, chapters);

                // Save exercises - set their chapter first
                Array.ForEach(chapters, c =>
                {
                    if (c.Exercises != null)
                    {
                        foreach (Exercise e in c.Exercises)
                        {
                            e.ChapterNo = c.ChapterNo;
                            e.Content = LoadManifestResourceFile($"Content{c.ChapterNo}-{ e.ExerciseNo}.cshtml", "<p>No course material is available for this exercise</p>");
                            e.DefaultCode = LoadManifestResourceFile($"Default{c.ChapterNo}-{ e.ExerciseNo}.csx", "// Enter your code here:" + Environment.NewLine + Environment.NewLine);
                            e.HiddenCodeHeader = LoadManifestResourceFile($"Header{c.ChapterNo}-{ e.ExerciseNo}.csx");
                        }
                        context.Exercises.AddOrUpdate(e => new { e.ChapterNo, e.ExerciseNo }, c.Exercises.ToArray());
                    }
                });
            }
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
