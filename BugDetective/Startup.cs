using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BugDetective.Startup))]
namespace BugDetective
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
