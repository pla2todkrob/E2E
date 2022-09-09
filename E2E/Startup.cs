using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(E2E.Startup))]

namespace E2E
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}