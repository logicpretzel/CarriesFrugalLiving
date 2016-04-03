using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CarriesFrugalLiving.Startup))]
namespace CarriesFrugalLiving
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
