namespace CodingTrainer.CodingTrainerModels.Migrations
{
    using CodingTrainer.CodingTrainerModels.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CodingTrainer.CodingTrainerModels.Contexts.CodingTrainerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Contexts.CodingTrainerContext context)
        {
            //  This method will be called after migrating to the latest version.

            context.Exercises.AddOrUpdate(
                new Exercise
                {
                    ExerciseId = 0,
                    DefaultCode =
@"using static System.Console;

WriteLine(""Enter some text"");
string s = ReadLine();
WriteLine(""You entered: "" + s);"
                },
                new Exercise
                {
                    ExerciseId = 1,
                    DefaultCode =
@"int i = 0;
int j = 5/i;"
                });

        }
    }
}
