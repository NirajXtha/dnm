using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NeoERP.DocumentTemplate.Startup))]
namespace NeoERP.DocumentTemplate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
