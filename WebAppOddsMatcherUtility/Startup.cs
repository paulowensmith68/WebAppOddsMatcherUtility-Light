using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppOddsMatcherUtility.Startup))]
namespace WebAppOddsMatcherUtility
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
