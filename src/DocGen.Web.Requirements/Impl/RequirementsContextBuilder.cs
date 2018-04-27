using System;
using System.IO;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Requirements;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DocGen.Web.Requirements.Impl
{
    public class RequirementsContextBuilder : IRequirementsContextBuilder
    {
        readonly IRequirementsBuilder _requirementsBuilder;
        readonly IServiceProvider _serviceProvider;

        public RequirementsContextBuilder(IRequirementsBuilder requirementsBuilder,
            IServiceProvider serviceProvider)
        {
            _requirementsBuilder = requirementsBuilder;
            _serviceProvider = serviceProvider;
        }

        public async Task<RequirementsContext> Build(string contentDirectory)
        {
            var builder = _serviceProvider.GetRequiredService<IWebBuilder>();
            
            if(!Directory.Exists(contentDirectory))
                throw new DocGenException($"Requirements directory {contentDirectory} doesn't exist");

            var requirementsDirectory = Path.Combine(contentDirectory, "requirements");
            var pagesDirectory = Path.Combine(contentDirectory, "pages");

            if(!Directory.Exists(requirementsDirectory))
                throw new DocGenException($"Requirements directory {requirementsDirectory} doesn't exist");

            if(!Directory.Exists(pagesDirectory))
                throw new DocGenException($"Pages directory {pagesDirectory} doesn't exist");

            // Register our static files.
            var staticFiles = new PhysicalFileProvider("/Users/pknopf/git/docgen/src/DocGen.Web.Requirements/Internal/Resources/wwwroot");
            builder.RegisterFiles(staticFiles);

            var userNeeds = await _requirementsBuilder.BuildRequirementsFromDirectory(requirementsDirectory);
            var pages = await Task.Run(() => Directory.GetFiles(pagesDirectory, "*.md", System.IO.SearchOption.AllDirectories));
            var menuStore = new MenuStore();
            
            foreach(var page in pages)
            {
                var url = page.Replace("\\", "/").Substring(pagesDirectory.Length);
                if (Path.GetFileNameWithoutExtension(page) == "index")
                {
                    // This is an extension-less url.
                    url = url.Substring(0, url.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase));
                    if (string.IsNullOrEmpty(url))
                        url = "/";
                }
                builder.RegisterMvc(url, new {
                    controller = "Markdown",
                    action = "Page",
                    page = page
                });
            }

            builder.RegisterServices(services => {
                services.AddMvc();
                services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.FileProviders.Add(new PhysicalFileProvider("/Users/pknopf/git/docgen/src/DocGen.Web.Requirements/Internal/Resources"));
                });
                services.AddSingleton<IMenuStore>(menuStore);
            });

            return new RequirementsContext
            {
                Menu = menuStore,
                WebBuilder = builder
            };
        }
    }
}