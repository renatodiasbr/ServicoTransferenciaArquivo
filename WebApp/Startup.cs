using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ServicoTransferenciaArquivo.WebApp.Startup))]
namespace ServicoTransferenciaArquivo.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
