﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerModels.Security;
using CodingTrainer.CSharpRunner.Assessment;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodingTrainer.CodingTrainerEntityFramework.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<ExceptionRunningUsersCode> ExceptionLogs { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<AssessmentGroup> AssessmentGroups { get; set; }
        public DbSet<AssessmentMethodBase> Assessments { get; set; }
        public DbSet<SavedWork> SavedWork { get; set; }

        public DbSet<UnhandledControllerException> UnhandledControllerExceptions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Submission>()
                .HasOptional(s => s.User)
                .WithMany(u => u.Submissions)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ExceptionRunningUsersCode>()
                .HasOptional(e => e.User)
                .WithMany(u => u.ExceptionLogs)
                .WillCascadeOnDelete();

            base.OnModelCreating(modelBuilder);
        }
    }
}