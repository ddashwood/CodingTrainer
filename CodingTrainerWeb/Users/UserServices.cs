using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CodingTrainer.CodingTrainerEntityFramework.Contexts;
using CodingTrainer.CodingTrainerModels;
using CodingTrainer.CodingTrainerModels.Security;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodingTrainer.CodingTrainerWeb.Users
{
    internal class UserServices : IUserServices
    {
        UserStore<ApplicationUser> userStore;
        ApplicationUserManager userManager;

        string emulatingId = null;

        public UserServices()
        {
            var dbContext = new ApplicationDbContext();
            userStore = new UserStore<ApplicationUser>(dbContext);
            userManager = new ApplicationUserManager(userStore);
        }

        // Get methods

        public ApplicationUser GetCurrentUser()
        {
            var principal = Thread.CurrentPrincipal;
            return userManager.FindById(emulatingId ?? principal.Identity.GetUserId());
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var principal = Thread.CurrentPrincipal;
            return await userManager.FindByIdAsync(emulatingId ?? principal.Identity.GetUserId());
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public string GetCurrentUserId()
        {
            return emulatingId ?? Thread.CurrentPrincipal.Identity.GetUserId();
        }

        public string GetName()
        {
            var appUser = GetCurrentUser();
            var name = appUser == null ? "Unknown" : appUser.FirstName + " " + appUser.LastName;
            if (emulatingId == null)
            {
                return name;
            }
            else
            {
                var principal = Thread.CurrentPrincipal;
                var realUser = userManager.FindById(principal.Identity.GetUserId());
                return $"{realUser.FirstName} {realUser.LastName} ({appUser.FirstName} {appUser.LastName})";
            }
        }

        public string GetBootstrapTheme()
        {
            var appUser = GetCurrentUser();
            return appUser == null ? "SpaceLab" : appUser.Dark ? "Cyborg" : "SpaceLab";
        }

        public string GetCodeMirrorTheme()
        {
            var appUser = GetCurrentUser();
            return appUser == null ? "eclipse" : appUser.Dark ? "twilight" : "eclipse";
        }

        public async Task<string> GetBootstrapThemeAsync()
        {
            var appUser = await GetCurrentUserAsync();
            return appUser == null ? "SpaceLab" : appUser.Dark ? "Cyborg" : "SpaceLab";
        }

        public async Task<string> GetCodeMirrorThemeAsync()
        {
            var appUser = await GetCurrentUserAsync();
            return appUser == null ? "eclipse" : appUser.Dark ? "twilight" : "eclipse";
        }


        // Update methods

        public async Task UpdateNameAsync(string firstName, string lastName)
        {
            var user = await GetCurrentUserAsync();
            user.FirstName = firstName;
            user.LastName = lastName;
            await userManager.UpdateAsync(user);
            await userStore.Context.SaveChangesAsync();
        }

        public async Task UpdateSettings(bool dark)
        {
            var user = await GetCurrentUserAsync();
            user.Dark = dark;
            await userManager.UpdateAsync(user);
            await userStore.Context.SaveChangesAsync();
        }

        public async Task AdvanceToExercise(Exercise newExercise)
        {
            var user = await GetCurrentUserAsync();
            user.AdvanceToExercise(newExercise);
            await userManager.UpdateAsync(user);
            await userStore.Context.SaveChangesAsync();
        }

        public void Emulate(string userId)
        {
            emulatingId = userId;
        }

        public async Task UpdateUser(ApplicationUser user)
        {
            var theUser = await userManager.FindByIdAsync(user.Id);

            theUser.FirstName = user.FirstName;
            theUser.LastName = user.LastName;
            theUser.CurrentChapterNo = user.CurrentChapterNo;
            theUser.CurrentExerciseNo = user.CurrentExerciseNo;
            theUser.Dark = user.Dark;
            theUser.AssessByRunningOnly = user.AssessByRunningOnly;
            theUser.Processed = user.Processed;

            await userManager.UpdateAsync(theUser);
            await userStore.Context.SaveChangesAsync();
        }
    }
}