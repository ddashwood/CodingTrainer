﻿using System;
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

        const string Emulating = "Emulating";

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
            return userManager.FindById(EmulatingId ?? principal.Identity.GetUserId());
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var principal = Thread.CurrentPrincipal;
            return await userManager.FindByIdAsync(EmulatingId ?? principal.Identity.GetUserId());
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public string GetCurrentUserId()
        {
            return EmulatingId ?? Thread.CurrentPrincipal.Identity.GetUserId();
        }

        public string GetName()
        {
            var appUser = GetCurrentUser();
            var name = appUser == null ? "Unknown" : appUser.FirstName + " " + appUser.LastName;
            if (EmulatingId == null)
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
            EmulatingId = userId;
        }


        public bool IsEmulating(HttpSessionStateBase session)
        {
            // HttpContext.Session appears to be null in some partial views, so insist on being given the session
            return (session[Emulating] != null);
        }

        public async Task UpdateUser(ApplicationUser user)
        {
            var theUser = await userManager.FindByIdAsync(user.Id);

            theUser.FirstName = user.FirstName;
            theUser.LastName = user.LastName;
            theUser.CurrentChapterNo = user.CurrentChapterNo;
            theUser.CurrentExerciseNo = user.CurrentExerciseNo;
            theUser.Dark = user.Dark;
            theUser.Processed = user.Processed;

            await userManager.UpdateAsync(theUser);
            await userStore.Context.SaveChangesAsync();
        }

        private string EmulatingId
        {
            get
            {
                if (HttpContext.Current == null) return null;
                if (HttpContext.Current.Session == null) return null;
                if (HttpContext.Current.Session[Emulating] == null) return null;
                return HttpContext.Current.Session[Emulating] as string;
            }
            set
            {
                HttpContext.Current.Session[Emulating] = value;
            }
        }
    }
}