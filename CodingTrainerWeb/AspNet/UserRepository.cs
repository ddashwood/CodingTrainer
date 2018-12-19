using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CodingTrainer.CodingTrainerModels.Contexts;
using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodingTrainer.CodingTrainerWeb.AspNet
{
    internal class UserRepository : IUserRepository
    {
        UserStore<ApplicationUser> userStore;
        ApplicationUserManager userManager;

        public UserRepository()
        {
            var dbContext = new ApplicationDbContext();
            userStore = new UserStore<ApplicationUser>(dbContext);
            userManager = new ApplicationUserManager(userStore);
        }

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

        public void SaveUser(ApplicationUser user)
        {
            userManager.Update(user);
            userStore.Context.SaveChanges();
        }

        public async Task SaveUserAsync(ApplicationUser user)
        {

            await userManager.UpdateAsync(user);
            await userStore.Context.SaveChangesAsync();

        }
    }
}