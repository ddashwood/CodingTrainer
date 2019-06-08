using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerModels.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CodingTrainer.CodingTrainerWeb.Dependencies
{
    public interface IUserServices
    {
        ApplicationUser GetCurrentUser();
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<ApplicationUser> GetUserByIdAsync(string id);
        string GetCurrentUserId();
        string GetName();
        string GetBootstrapTheme();
        string GetCodeMirrorTheme();
        Task<string> GetBootstrapThemeAsync();
        Task<string> GetCodeMirrorThemeAsync();

        Task UpdateNameAsync(string firstName, string lastName);
        Task UpdateSettings(bool dark);
        Task AdvanceToExercise(Exercise newExercise);
        Task UpdateUser(ApplicationUser user);

        void Emulate(string userId);
        bool IsEmulating(HttpSessionStateBase session);
    }
}
