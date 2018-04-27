using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using System.Collections.ObjectModel;

namespace DocGen.Web.Hosting.Impl
{
    public class HostBuilder : IHostBuilder
    {
        public IWebHost BuildWebHost(int port, params IHostModule[] modules)
        {
            return new InternalWebHost(modules.ToList(), port);
        }

        public IVirtualHost BuildVirtualHost(params IHostModule[] modules)
        {
            return new InternalVirtualHost(modules.ToList());
        }

        private class InternalWebHost : IWebHost
        {
            readonly Microsoft.AspNetCore.Hosting.IWebHost _webHost;
            readonly int _port;

            public InternalWebHost(List<IHostModule> modules,
                int port)
            {
                _port = port;
                Paths = new ReadOnlyCollection<string>(modules.SelectMany(x => x.Paths).ToList());
                _webHost = WebHost.CreateDefaultBuilder(new string[]{})
                    .UseUrls($"http://*:{port}")
                    .UseSetting(WebHostDefaults.ApplicationKey,  Assembly.GetEntryAssembly().GetName().Name)
                    .ConfigureLogging(factory => {
                        factory.AddConsole();
                    })
                    .ConfigureServices(services => {
                        services.AddSingleton<Startup>();
                        services.AddSingleton(modules);
                        services.AddSingleton(typeof(IStartup), sp =>
                        {
                            var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                            return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, typeof(Startup), hostingEnvironment.EnvironmentName));
                        });
                    })
                    .Build();
            }

            public IReadOnlyCollection<string> Paths { get; }

            public HttpClient CreateClient()
            {
                return new HttpClient {
                    BaseAddress = new System.Uri($"http://localhost:{_port}")
                };
            }

            public void Listen()
            {
                _webHost.Start();
            }

            public void Dispose()
            {
                _webHost.Dispose();
            }        
        }

        private class InternalVirtualHost : IVirtualHost
        {
            readonly TestServer _testServer;

            public InternalVirtualHost(List<IHostModule> modules)
            {
                Paths = new ReadOnlyCollection<string>(modules.SelectMany(x => x.Paths).ToList());
                _testServer = new TestServer(new WebHostBuilder()
                    .UseSetting(WebHostDefaults.ApplicationKey,  Assembly.GetEntryAssembly().GetName().Name)
                    .ConfigureLogging(factory => {
                        factory.AddConsole();
                    })
                    .ConfigureServices(services => {
                        services.AddSingleton<Startup>();
                        services.AddSingleton(modules);
                        services.AddSingleton(typeof(IStartup), sp =>
                        {
                            var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                            return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, typeof(Startup), hostingEnvironment.EnvironmentName));
                        });
                    }));
            }

            public IReadOnlyCollection<string> Paths { get; }

            public HttpClient CreateClient()
            {
                return _testServer.CreateClient();
            }

            public void Dispose()
            {
                _testServer.Dispose();
            }
        }
    }
}