using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GowBoard.Startup))]
namespace GowBoard
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}