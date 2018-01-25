using CodingTrainer.CodingTrainerModels.Models;
using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CodingTrainer.CodingTrainerModels.Repositories
{
    public interface ICodingTrainerRepository
    {
        Task <Exercise> GetExerciseAsync(int number);
        Task <ApplicationUser> GetUserAsync(string userName);
        Task <ApplicationUser> GetUserAsync(HubCallerContext hubContext);
    }
}