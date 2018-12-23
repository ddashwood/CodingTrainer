using CodingTrainer.CodingTrainerWeb.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CodingTrainer.CodingTrainerWeb.ApiControllers
{
    public class ThemeController : ApiController
    {
        private IUserRepository userRep;

        public ThemeController(IUserRepository userRepository)
        {
            userRep = userRepository;
        }

        public async Task<string> Get()
        {
            var user = await userRep.GetCurrentUserAsync();

            return user?.SelectedTheme??"";
        }

        public async Task Put([FromBody]string theme)
        {
            var user = await userRep.GetCurrentUserAsync();
            user.SelectedTheme = theme;
            await userRep.SaveUserAsync(user);
        }

    }
}