using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(N01467577_PassionProject.Startup))]
namespace N01467577_PassionProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
