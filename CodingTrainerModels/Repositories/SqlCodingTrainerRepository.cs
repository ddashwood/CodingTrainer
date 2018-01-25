using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CodingTrainer.CodingTrainerModels.Contexts;
using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.SignalR.Hubs;

namespace CodingTrainer.CodingTrainerModels.Repositories
{
    public class SqlCodingTrainerRepository : ICodingTrainerRepository
    {
        public async Task<Exercise> GetExerciseAsync(int id)
        {
            using (var context = new CodingTrainerContext())
            {
                return await context.Exercises.SingleAsync(e => e.ExerciseId == id);
            }
        }

        public async Task<ApplicationUser> GetUserAsync(string userName)
        {
            using (var context = new CodingTrainerContext())
            {
                return await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            }
        }

        public async Task<ApplicationUser> GetUserAsync(HubCallerContext hubContext)
        {
            return await GetUserAsync(hubContext.User.Identity.Name);
        }
    }
}