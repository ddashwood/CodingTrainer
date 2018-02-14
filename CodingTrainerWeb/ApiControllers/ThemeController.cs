using CodingTrainer.CodingTrainerModels.Contexts;
using CodingTrainer.CodingTrainerModels.Models.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CodingTrainer.CodingTrainerWeb.ApiControllers
{
    public class ThemeController : ApiController
    {
        private IPrincipal principal;
        private IPrincipal Principal
        {
            get
            {
                if (principal == null) principal = RequestContext.Principal;
                return principal;
            }
            set
            {
                principal = value;
            }
        }

        public ThemeController(IPrincipal principal)
        {
            Principal = principal;
        }
        public ThemeController()
        { }

        public async Task<string> Get()
        {
            var dbContext = new ApplicationDbContext();
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(dbContext));
            var user = await userManager.FindByIdAsync(Principal.Identity.GetUserId());

            return user.SelectedTheme;
        }

        public async Task Put([FromBody]string theme)
        {
            var dbContext = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(dbContext);
            var userManager = new ApplicationUserManager(store);
            var user = await userManager.FindByIdAsync(Principal.Identity.GetUserId());

            user.SelectedTheme = theme;

            await userManager.UpdateAsync(user);
            await store.Context.SaveChangesAsync();
        }

    }
}