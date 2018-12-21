namespace CodingTrainer.CodingTrainerModels.Migrations
{
    using CodingTrainer.CodingTrainerModels.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    internal sealed class Configuration : DbMigrationsConfiguration<Contexts.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Contexts.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            RunSeed(context);
        }

        public void RunSeed(Contexts.ApplicationDbContext context)
        {
            var resourceName = "CodingTrainer.CodingTrainerModels.Data.SeedData.xml";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

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
                            e.DefaultCode = e.DefaultCode.Trim();
                            e.HiddenCodeHeader = e.HiddenCodeHeader?.Trim();
                            e.ChapterNo = c.ChapterNo;
                        }
                        context.Exercises.AddOrUpdate(e => new { e.ChapterNo, e.ExerciseNo }, c.Exercises.ToArray());
                    }
                });
            }
        }
    }
}
