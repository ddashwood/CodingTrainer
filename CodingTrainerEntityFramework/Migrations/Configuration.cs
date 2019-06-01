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
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using CodingTrainer.CodingTrainerModels.Security;

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

            Seeder.Seed(context); // Seeds exercise data
            Exercise lastExercise = context.Exercises.FirstOrDefault(e => e.IsFinalExercise);

            // Now seed roles and users
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == "admin@codingtrainer.com"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "admin@codingtrainer.com", Email = "admin@codingtrainer.com", EmailConfirmed = true, FirstName = "Coding Trainer", LastName = "Admin" };
                user.CurrentChapterNo = lastExercise?.ChapterNo ?? 1;
                user.CurrentExerciseNo = lastExercise?.ExerciseNo ?? 1;

                manager.Create(user, "ChangeMe!");
                manager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
