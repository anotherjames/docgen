using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace DocGen.Web.Impl
{
    public class WebBuilder : IWebBuilder
    {
        Dictionary<string, Page> _pages = new Dictionary<string, Page>();
        List<Action<IApplicationBuilder, IHostingEnvironment>> _additionalAppActions = new List<Action<IApplicationBuilder, IHostingEnvironment>>();

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

        public IWeb BuildWeb(int port = 8000)
        {
            return new Web(
                new List<IWebModule>{new WebModule(_pages.ToDictionary(x => x.Key, x => x.Value), _additionalAppActions.ToList())},
                _pages.Keys.ToList(),
                port);
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

        class WebModule : IWebModule
        {
            Dictionary<string, Page> _pages;
            List<Action<IApplicationBuilder, IHostingEnvironment>> _additionalAppActions;

            public WebModule(Dictionary<string, Page> pages,
                List<Action<IApplicationBuilder, IHostingEnvironment>> additionalAppActions)
            {
                _pages = pages;
                _additionalAppActions = additionalAppActions;
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
        }
    }
}