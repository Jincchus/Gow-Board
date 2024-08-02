using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(GowBoard.Startup))]
namespace GowBoard
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 인증 설정
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Member/LogIn")
            });

            // SignalR 설정
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true
            };
            app.MapSignalR("/signalr",hubConfiguration);
        }
    }
}