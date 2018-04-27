using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocGen.Web.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DocGen.Web.Impl
{
    public class WebBuilder : IWebBuilder
    {
        readonly Dictionary<string, Page> _pages = new Dictionary<string, Page>();
        readonly List<Action<IServiceCollection>> _serviceActions = new List<Action<IServiceCollection>>();
        readonly IHostBuilder _hostBuilder;

        public WebBuilder(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        public void Register(string path, Func<HttpContext, Task> action)
        {
            _pages.Add(path, new Page(action));
        }

        public void RegisterFiles(IFileProvider fileProvider)
        {
            var contents = fileProvider.GetDirectoryContents("/");
            if(contents == null || !contents.Exists) return;

            foreach(var file in contents) {
                RegisterFileInfo(fileProvider, "/", file);
            }
        }

        public void RegisterServices(Action<IServiceCollection> action)
        {
            _serviceActions.Add(action);
        }

        public Hosting.IWebHost BuildWebHost(int port = 8000)
        {
            return _hostBuilder.BuildWebHost(port,
                new HostModule(
                    _pages.ToDictionary(x => x.Key, x => x.Value),
                    _serviceActions.ToList()));
        }

        public IVirtualHost BuildVirtualHost()
        {
            return _hostBuilder.BuildVirtualHost(new HostModule(
                _pages.ToDictionary(x => x.Key, x => x.Value),
                _serviceActions.ToList()));
        }


        private void RegisterFileInfo(IFileProvider fileProvider, string basePath, IFileInfo fileInfo)
        {
            if(fileInfo.IsDirectory)  {
                var content = fileProvider.GetDirectoryContents(fileInfo.Name);

                if(content == null || !content.Exists) {
                    return;
                }

                foreach(var child in content) {
                    RegisterFileInfo(fileProvider, Path.Combine(basePath,fileInfo.Name), child);
                }
            } else  {
                // We are passing NULL so that the middleware
                // will fall through and use the static file middleware.
                Register(Path.Combine(basePath, fileInfo.Name), async context =>
                {
                    var env = context.RequestServices.GetRequiredService<IHostingEnvironment>();
                    var options = Options.Create(new StaticFileOptions());
                    options.Value.FileProvider = fileProvider;
                    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                    var middleware = new StaticFileMiddleware(_ => Task.CompletedTask, env, options, loggerFactory);
                    await middleware.Invoke(context);
                });
            }
        }

        class Page
        {
            public Page(Func<HttpContext, Task> action)
            {
                Action = action;
            }

            public Func<HttpContext, Task> Action { get; }
        }

        class HostModule : IHostModule
        {
            readonly Dictionary<string, Page> _pages;
            readonly List<Action<IServiceCollection>> _serviceActions;

            public HostModule(Dictionary<string, Page> pages,
                List<Action<IServiceCollection>> serviceActions)
            {
                _pages = pages;
                _serviceActions = serviceActions;
                Paths = new ReadOnlyCollection<string>(_pages.Keys.ToList());
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.Use(async (context, next) => {
                    _pages.TryGetValue(context.Request.Path, out var page);
                    if(page != null) {
                        await page.Action(context);
                    } else {
                        await next();
                    }
                });
            }

            public void ConfigureServices(IServiceCollection services)
            {
                foreach(var serviceAction in _serviceActions) {
                    serviceAction(services);
                }
            }

            public IReadOnlyCollection<string> Paths { get; }
        }
    }
}