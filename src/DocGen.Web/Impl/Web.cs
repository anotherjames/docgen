using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace DocGen.Web.Impl
{
    public class Web : IWeb
    {
        IWebHost _webHost;

        public Web(List<IWebModule> webModules, List<string> paths, int port)
        {
            Paths = new ReadOnlyCollection<string>(paths);
            _webHost = WebHost.CreateDefaultBuilder(new string[]{})
                .UseUrls($"http://*:{port}")
                .UseSetting(WebHostDefaults.ApplicationKey,  Assembly.GetEntryAssembly().GetName().Name)
                .ConfigureLogging(factory =>
                {
                    factory.AddConsole();
                })
                .ConfigureServices(services => 
                {
                    services.AddSingleton<DocGen.Web.Internal.Startup>();
                    services.AddSingleton<List<IWebModule>>(webModules);
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

        public IReadOnlyCollection<string> Paths  { get; }

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