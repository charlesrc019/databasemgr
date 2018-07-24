using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DatabaseManager.Startup))]
namespace DatabaseManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
