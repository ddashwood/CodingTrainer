﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerModels.Security;
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
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public DbSet<Chapter> Chapters { get; set; } 
    }
}