using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;

namespace DocGen.Web.Impl
{
    public class Web : IWeb
    {
        IWebHost _webHost;
        
        public Web(IWebContext webContext, int port)
        {
            _webHost = WebHost.CreateDefaultBuilder(new string[]{})
                .UseUrls($"http://*:{port}")
                .UseSetting(WebHostDefaults.ApplicationKey,  Assembly.GetEntryAssembly().GetName().Name)
                .ConfigureServices(services => 
                {
                    services.AddSingleton<IWebContext>(webContext);
                    services.AddSingleton(typeof(IStartup), sp =>
                    {
                        var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                        return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, typeof(DocGen.Web.Internal.Startup), hostingEnvironment.EnvironmentName));
                    });
                })
                .Build();
        }

        ~Web()
        {
            Dispose();
        }

        public void Listen()
        {
            _webHost.Start();
        }

        public void Dispose()
        {
            if(_webHost != null)
            {
                _webHost.Dispose();
                _webHost = null;
            }
        }
    }
}