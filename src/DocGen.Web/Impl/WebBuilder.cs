using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DocGen.Web.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DocGen.Web.Impl
{
    public class WebBuilder : IWebBuilder
    {
        Dictionary<string, Page> _pages = new Dictionary<string, Page>();
        List<Action<IApplicationBuilder, IHostingEnvironment>> _additionalAppActions = new List<Action<IApplicationBuilder, IHostingEnvironment>>();
        List<Action<IServiceCollection>> _serviceActions = new List<Action<IServiceCollection>>();
        IHostBuilder _hostBuilder;

        public WebBuilder(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        public void Register(string path, Func<HttpContext, Task> action)
        {
            _pages.Add(path, new Page(path, action));
        }

        public void RegisterFiles(IFileProvider fileProvider)
        {
            var contents = fileProvider.GetDirectoryContents("/");
            if(contents == null || !contents.Exists) return;

            foreach(var file in contents) {
                RegisterFileInfo(fileProvider, "/", file);
            }

            _additionalAppActions.Add((app, env) => {
                app.UseStaticFiles(new StaticFileOptions() {
                    FileProvider = fileProvider
                });
            });
        }

        public void RegisterServices(Action<IServiceCollection> action)
        {
            _serviceActions.Add(action);
        }

        public DocGen.Web.Hosting.IWebHost BuildWebHost(int port = 8000)
        {
            return _hostBuilder.BuildWebHost(port,
                new HostModule(
                    _pages.ToDictionary(x => x.Key, x => x.Value),
                    _additionalAppActions.ToList(),
                    _serviceActions.ToList()));
        }

        public DocGen.Web.Hosting.IVirtualHost BuildVirtualHost()
        {
            return _hostBuilder.BuildVirtualHost(new HostModule(
                _pages.ToDictionary(x => x.Key, x => x.Value),
                _additionalAppActions.ToList(),
                _serviceActions.ToList()));
        }

        private async Task SaveUrlToFile(string url, string file) {
            // Ensure the file's parent directories are created.
            var parentDirectory = Path.GetDirectoryName(file);
            if(!(await Task.Run(() => Directory.Exists(parentDirectory)))) {
                await Task.Run(() => Directory.CreateDirectory(parentDirectory));
            }
            using(var client = new HttpClient()) {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using(var requestStream = await response.Content.ReadAsStreamAsync()) {
                using(var fileStream = await Task.Run(() => File.OpenWrite(file)))
                    await requestStream.CopyToAsync(fileStream);
                }
            }
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
                Register(Path.Combine(basePath, fileInfo.Name), null);
            }
        }

        class Page
        {
            public Page(string path, Func<HttpContext, Task> action)
            {
                Path = path;
                Action = action;
            }

            public string Path { get; }

            public Func<HttpContext, Task> Action { get; }
        }

        class HostModule : IHostModule
        {
            Dictionary<string, Page> _pages;
            List<Action<IApplicationBuilder, IHostingEnvironment>> _additionalAppActions;
            List<Action<IServiceCollection>> _serviceActions;

            public HostModule(Dictionary<string, Page> pages,
                List<Action<IApplicationBuilder, IHostingEnvironment>> additionalAppActions,
                List<Action<IServiceCollection>> serviceActions)
            {
                _pages = pages;
                _additionalAppActions = additionalAppActions;
                _serviceActions = serviceActions;
                Paths = new ReadOnlyCollection<string>(_pages.Keys.ToList());
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.Use(async (context, next) => {
                    Page page = null;
                    _pages.TryGetValue(context.Request.Path, out page);
                    if(page != null && page.Action != null) {
                        await page.Action(context);
                    } else {
                        await next();
                    }
                });
                foreach(var action in _additionalAppActions) {
                    action(app, env);
                }
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