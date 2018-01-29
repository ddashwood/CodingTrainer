namespace CodingTrainer.CodingTrainerModels.Migrations
{
    using CodingTrainer.CodingTrainerModels.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Contexts.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Contexts.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            var resourceName = "CodingTrainer.CodingTrainerModels.Data.SeedData.xml";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);
            var xml = XDocument.Load(stream);

            // Chapters
            var chapters = xml.Element("Data").Element("Chapters").Elements("Chapter")
                .Select(x => new Chapter()
                {
                    ChapterNumber = Convert.ToInt32(x.Attribute("Id").Value),
                    ChapterName = x.Element("Name").Value,
                    Description = x.Element("Description")?.Value
                }).ToArray();
            context.Chapters.AddOrUpdate(chapters);

            // Exercises
            var exercises = xml.Element("Data").Element("Exercises").Elements("Exercise")
                .Select(x => new Exercise()
                {
                    ExerciseId = Convert.ToInt32(x.Attribute("Id").Value),
                    DefaultCode = x.Element("DefaultCode").Value.Trim(),
                    ModelAnswer = x.Element("ModelAnswer")?.Value
                }).ToArray();
            context.Exercises.AddOrUpdate(exercises);
        }
    }
}
