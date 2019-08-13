using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CodingTrainer.CodingTrainerWeb.Users;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using Unity;

namespace CodingTrainer.CodingTrainerWeb.ViewExtensions
{
    /// <summary>
    /// This class provides extensions to the IPrincipal interface, primarily so that
    /// views can user the sytnax @User.ExtensionMethod() or @Url.ExtensionMethod()
    /// to access them easily
    /// </summary>
    public static class UserExtensions
    {
        public static IUserServices UserServices
        {
            get
            {
                return UnityConfig.Container.Resolve<IUserServices>();
            }
        }

        public static string GetName(this IPrincipal user)
        {
            return UserServices.GetName();
        }

        public static string GetBootstrapTheme(this IPrincipal user)
        {
            return UserServices.GetBootstrapTheme();
        }
    }
}