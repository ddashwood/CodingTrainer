using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CodingTrainer.CodingTrainerEntityFramework.Contexts;
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
            return userManager.FindById(principal.Identity.GetUserId());
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var principal = Thread.CurrentPrincipal;
            return await userManager.FindByIdAsync(principal.Identity.GetUserId());
        }

        public string GetCurrentUserId()
        {
            return Thread.CurrentPrincipal.Identity.GetUserId();
        }

        public string GetName()
        {
            var appUser = GetCurrentUser();
            return appUser == null ? "Unknown" : appUser.FirstName + " " + appUser.LastName;
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

        public async Task UpdateSettings(bool dark, string timeZoneId)
        {
            var user = await GetCurrentUserAsync();
            user.Dark = dark;
            user.TimeZoneId = timeZoneId;
            await userManager.UpdateAsync(user);
            await userStore.Context.SaveChangesAsync();
        }
    }
}