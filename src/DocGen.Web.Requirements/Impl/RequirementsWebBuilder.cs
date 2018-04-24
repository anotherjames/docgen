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
    public class RequirementsWebBuilder : IRequirementsWebBuilder
    {
        IServiceProvider _serviceProvider;
        IRequirementsBuilder _requirementsBuilder;

        public RequirementsWebBuilder(IServiceProvider serviceProvider,
            IRequirementsBuilder requirementsBuilder)
        {
            _serviceProvider = serviceProvider;
            _requirementsBuilder = requirementsBuilder;
        }

        public async Task<IWeb> Build(string contentDirectory, int port)
        {
            var webBuilder = _serviceProvider.GetRequiredService<IWebBuilder>();

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
            webBuilder.RegisterFiles(staticFiles);

            var userNeeds = await _requirementsBuilder.BuildRequirementsFromDirectory(requirementsDirectory);
            var pages = await Task.Run(() => Directory.GetFiles(pagesDirectory, "*.md", System.IO.SearchOption.AllDirectories));
            
            // TODO: register user needs and pages.
            foreach(var page in pages) {
                webBuilder.RegisterMvc("/test", new {
                    controller = "Markdown",
                    action = "Page",
                    page = "testp"
                });
            }

            webBuilder.RegisterServices(services => {
                services.AddMvc();
                services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.FileProviders.Add(new PhysicalFileProvider("/Users/pknopf/git/docgen/src/DocGen.Web.Requirements/Internal/Resources"));
                });
            });
            
            return webBuilder.BuildWeb(port);
        }
    }
}