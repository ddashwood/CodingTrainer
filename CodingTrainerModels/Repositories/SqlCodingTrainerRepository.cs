using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodingTrainer.CodingTrainerModels.Contexts;
using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.SignalR.Hubs;

namespace CodingTrainer.CodingTrainerModels.Repositories
{
    public class SqlCodingTrainerRepository : ICodingTrainerRepository
    {
        CodingTrainerContext context = new CodingTrainerContext();

        public Exercise GetExercise(int id)
        {
            return context.Exercises.Single(e => e.ExerciseId == id);
        }

        public ApplicationUser GetUser(string userName)
        {
            return context.Users.FirstOrDefault(u => u.UserName == userName);
        }

        public ApplicationUser GetUser(HubCallerContext hubContext)
        {
            return GetUser(hubContext.User.Identity.Name);
        }
    }
}