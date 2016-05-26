using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ItLearn.Startup))]
namespace ItLearn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
