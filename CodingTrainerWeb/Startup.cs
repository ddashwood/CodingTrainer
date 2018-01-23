using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CodingTrainer.CodingTrainerWeb.Startup))]
namespace CodingTrainer.CodingTrainerWeb
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
