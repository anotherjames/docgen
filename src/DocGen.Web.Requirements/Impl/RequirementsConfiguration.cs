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
    public class RequirementsConfiguration : IRequirementsConfiguration
    {
        IRequirementsBuilder _requirementsBuilder;

        public RequirementsConfiguration(IRequirementsBuilder requirementsBuilder)
        {
            _requirementsBuilder = requirementsBuilder;
        }

        public async Task Configure(IWebBuilder builder, string contentDirectory)
        {
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
            
            // TODO: register user needs and pages.
            foreach(var page in pages) {
                builder.RegisterMvc("/test", new {
                    controller = "Markdown",
                    action = "Page",
                    page = "testp"
                });
            }

            builder.RegisterServices(services => {
                services.AddMvc();
                services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.FileProviders.Add(new PhysicalFileProvider("/Users/pknopf/git/docgen/src/DocGen.Web.Requirements/Internal/Resources"));
                });
            });
        }
    }
}