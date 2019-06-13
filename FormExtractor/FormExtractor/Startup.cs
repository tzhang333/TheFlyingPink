using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FormExtractor.Startup))]
namespace FormExtractor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}