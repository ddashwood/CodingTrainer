using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CodingTrainerWeb.Startup))]
namespace CodingTrainerWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
