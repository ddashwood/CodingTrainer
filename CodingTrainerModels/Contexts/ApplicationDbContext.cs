using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodingTrainer.CodingTrainerModels.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private class CreateAndSeedDatabaseIfNotExists:CreateDatabaseIfNotExists<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
                var config = new Migrations.Configuration();
                config.RunSeed(context);
                base.Seed(context);
            }
        }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new CreateAndSeedDatabaseIfNotExists());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public DbSet<Chapter> Chapters { get; set; } 
    }
}