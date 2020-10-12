using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MC.Website.Startup))]
namespace MC.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
