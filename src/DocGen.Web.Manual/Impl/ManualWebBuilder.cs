using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Core.Markdown;
using DocGen.Requirements;
using DocGen.Web.Hosting;
using DocGen.Web.Manual.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DocGen.Web.Manual.Impl
{
    public class ManualWebBuilder : IManualWebBuilder
    {
        readonly IServiceProvider _serviceProvider;
        readonly IYamlParser _yamlParser;

        public ManualWebBuilder(IServiceProvider serviceProvider,
            IYamlParser yamlParser)
        {
            _serviceProvider = serviceProvider;
            _yamlParser = yamlParser;
        }
        
        public async Task<IManualWeb> BuildManual(string contentDirectory)
        {
            var webBuilder = _serviceProvider.GetRequiredService<IWebBuilder>();
            
            if(!await Task.Run(() => Directory.Exists(contentDirectory)))
                throw new DocGenException($"Manual directory {contentDirectory} doesn't exist");

            // Find all the markdown files, and sort them by the order.
            webBuilder.RegisterMvc("/", new {
                controller = "Manual",
                action = "Index"
            });

            var sections = new ManualSectionStore();
            foreach (var markdownFile in await Task.Run(() => Directory.GetFiles(contentDirectory, "*.md")))
            {
                var content = await Task.Run(() => File.ReadAllText(markdownFile));
                var yaml = _yamlParser.ParseYaml(content);
                int order = 0;
                if (yaml.Yaml != null)
                {
                    order = yaml.Yaml.Order;
                }
                sections.AddMarkdown(order, yaml.Markdown, markdownFile);
            }
            
            webBuilder.RegisterServices(services => {
                services.AddMvc();
                services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.FileProviders.Add(new PhysicalFileProvider("/Users/pknopf/git/docgen/src/DocGen.Web.Manual/Internal/Resources"));
                });
                services.AddSingleton(sections);
                // These regitrations are so that our controllers can inject core DocGen services.
                DocGen.Core.Services.Register(services);
                DocGen.Web.Services.Register(services);
            });
            
            return new ManualWeb(webBuilder);
        }

        class ManualWeb : IManualWeb
        {
            readonly IWebBuilder _webBuilder;

            public ManualWeb(IWebBuilder webBuilder)
            {
                _webBuilder = webBuilder;
            }
            
            public IWebHost BuildWebHost(string appBase = null, int port = WebDefaults.DefaultPort)
            {
                return _webBuilder.BuildWebHost(appBase, port);
            }

            public IVirtualHost BuildVirtualHost(string appBase = null)
            {
                return _webBuilder.BuildVirtualHost(appBase);
            }
        }
    }
}