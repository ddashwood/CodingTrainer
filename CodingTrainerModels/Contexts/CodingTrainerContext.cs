using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CodingTrainer.CodingTrainerModels.Contexts
{
    public class CodingTrainerContext : ApplicationDbContext
    {
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
    }
}