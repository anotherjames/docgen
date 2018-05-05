using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocGen.Web.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
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
            RegisterFiles("/", fileProvider);
        }

        public void RegisterFiles(string prefix, IFileProvider fileProvider)
        {
            var contents = fileProvider.GetDirectoryContents("/");
            if(contents == null || !contents.Exists) return;

            foreach(var file in contents) {
                RegisterFileInfo(fileProvider, prefix, "/", file);
            }
        }

        public void RegisterServices(Action<IServiceCollection> action)
        {
            _serviceActions.Add(action);
        }

        public Hosting.IWebHost BuildWebHost(string appBase = null, int port = 8000)
        {
            return _hostBuilder.BuildWebHost(
                port,
                appBase,
                new HostModule(
                    appBase,
                    _pages.ToDictionary(x => x.Key, x => x.Value),
                    _serviceActions.ToList()));
        }

        public IVirtualHost BuildVirtualHost(string appBase = null)
        {
            return _hostBuilder.BuildVirtualHost(
                appBase,
                new HostModule(
                    appBase,
                    _pages.ToDictionary(x => x.Key, x => x.Value),
                    _serviceActions.ToList()));
        }


        private void RegisterFileInfo(IFileProvider fileProvider, string prefix, string basePath, IFileInfo fileInfo)
        {
            if(fileInfo.IsDirectory)  {
                var content = fileProvider.GetDirectoryContents(fileInfo.Name);

                if(content == null || !content.Exists) {
                    return;
                }

                foreach(var child in content) {
                    RegisterFileInfo(fileProvider, prefix, Path.Combine(basePath,fileInfo.Name), child);
                }
            } else
            {
                var path = new PathString().Add(prefix)
                    .Add(basePath)
                    .Add("/" + fileInfo.Name);
                Register(path.Value, async context =>
                {
                    var env = context.RequestServices.GetRequiredService<IHostingEnvironment>();
                    var options = Options.Create(new StaticFileOptions());
                    options.Value.FileProvider = fileProvider;
                    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                    var middleware = new StaticFileMiddleware(_ => Task.CompletedTask, env, options, loggerFactory);

                    var oldPath = context.Request.Path;
                    try
                    {
                        context.Request.Path = Path.Combine(basePath, fileInfo.Name);
                        await middleware.Invoke(context);
                    }
                    finally
                    {
                        context.Request.Path = oldPath;
                    }
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
            readonly PathString _appBase;
            readonly Dictionary<string, Page> _pages;
            readonly List<Action<IServiceCollection>> _serviceActions;

            public HostModule(
                string appBase,
                Dictionary<string, Page> pages,
                List<Action<IServiceCollection>> serviceActions)
            {
                _appBase = appBase;
                _pages = pages;
                _serviceActions = serviceActions;
                Paths = new ReadOnlyCollection<string>(_pages.Keys.ToList());
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.Use(async (context, next) => {
                   
                    async Task<bool> AttemptRunPage(HttpContext c)
                    {
                        _pages.TryGetValue(c.Request.Path, out var page);
                        if (page == null) return false;
                        await page.Action(context);
                        return true;
                    }

                    if (!_appBase.HasValue)
                    {
                        // No app base configured, just find and execute our page.
                        if (!await AttemptRunPage(context))
                        {
                            await next();
                        }
                    }
                    else
                    {
                        // Only serve requests at the app base.
                        if (context.Request.Path.StartsWithSegments(_appBase, out var matchedPath, out var remainingPath))
                        {
                            var originalPath = context.Request.Path;
                            var originalPathBase = context.Request.PathBase;
                            context.Request.Path = remainingPath;
                            context.Request.PathBase = originalPathBase.Add(matchedPath);

                            bool ran;
                            
                            try
                            {
                                ran = await AttemptRunPage(context);
                            }
                            finally
                            {
                                context.Request.Path = originalPath;
                                context.Request.PathBase = originalPathBase;
                            }

                            if (!ran)
                            {
                                await next();
                            }
                        }
                        else
                        {
                            await next();
                        }
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