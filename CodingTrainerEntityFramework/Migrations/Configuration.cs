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
    using System.Data.Entity.Validation;
    using System.Text;
    using CodingTrainer.CodingTrainerEntityFramework.Seed;

    internal sealed class Configuration : DbMigrationsConfiguration<Contexts.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Contexts.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            // Un-comment to debug:

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            Seeder.Seed(context);
        }
    }
}
