using CodingTrainer.CodingTrainerModels.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTrainer.CodingTrainerWeb.Dependencies
{
    public interface IUserRepository
    {
        string GetCurrentUserId();

        ApplicationUser GetCurrentUser();
        Task<ApplicationUser> GetCurrentUserAsync();

        void SaveUser(ApplicationUser user);
        Task SaveUserAsync(ApplicationUser user);
    }
}
