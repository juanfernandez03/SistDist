using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwitterSD.Startup))]
namespace TwitterSD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
