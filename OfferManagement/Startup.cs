using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OfferManagement.Startup))]
namespace OfferManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
