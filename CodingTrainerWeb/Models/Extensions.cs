using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CodingTrainer.CodingTrainerWeb.Models
{
    public static class Extensions
    {
        public static string GetName(this IPrincipal user)
        {
            string name = HttpContext.Current.Session["FullName"] as string;

            if (name == null)
            {
                // No name set yet - let's find it and set it

                var UserManager = HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var appUser = UserManager.FindById(user.Identity.GetUserId());
                name = appUser == null ? "Unknown" : appUser.FirstName + " " + appUser.LastName;
                HttpContext.Current.Session["FullName"] = name;
            }
            return name;
        }
    }
}