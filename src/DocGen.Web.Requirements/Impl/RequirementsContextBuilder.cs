using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Core.Markdown;
using DocGen.Requirements;
using DocGen.Web.Requirements.Internal;
using Microsoft.AspNetCore.Mvc;
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
        readonly IMarkdownRenderer _markdownRenderer;

        public RequirementsContextBuilder(IRequirementsBuilder requirementsBuilder,
            IServiceProvider serviceProvider,
            IMarkdownRenderer markdownRenderer)
        {
            _requirementsBuilder = requirementsBuilder;
            _serviceProvider = serviceProvider;
            _markdownRenderer = markdownRenderer;
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

                var markdown = await _markdownRenderer.RenderMarkdownFromFile(page);

                builder.RegisterMvc(url, new {
                    controller = "Markdown",
                    action = "Page",
                    page = page
                });
                menuStore.AddPage(url, (string)markdown.Yaml?.Title, 0);
            }

            foreach (var userNeed in userNeeds)
            {
                builder.RegisterMvc(((IUrlHelper)null).UserNeed(userNeed), new
                {
                    controller = "Requirements",
                    action = "UserNeed",
                    number = userNeed.NumberFullyQualified
                });
                menuStore.AddPage(((IUrlHelper)null).UserNeed(userNeed), userNeed.Title, userNeeds.IndexOf(userNeed));
                foreach (var productRequirement in userNeed.ProductRequirements)
                {
                    builder.RegisterMvc(((IUrlHelper)null).ProductRequirement(productRequirement), new
                    {
                        controller = "Requirements",
                        action = "ProductRequirement",
                        number = productRequirement.NumberFullyQualified
                    });
                    menuStore.AddPage(((IUrlHelper)null).ProductRequirement(productRequirement), productRequirement.Title, userNeed.ProductRequirements.IndexOf(productRequirement));
                    foreach (var softwareSpecification in productRequirement.SoftwareSpecifications)
                    {
                        builder.RegisterMvc(((IUrlHelper)null).SoftwareSpecification(softwareSpecification), new
                        {
                            controller = "Requirements",
                            action = "SoftwareSpecification",
                            number = softwareSpecification.NumberFullyQualified
                        });
                        menuStore.AddPage(((IUrlHelper)null).SoftwareSpecification(softwareSpecification), softwareSpecification.Title, productRequirement.SoftwareSpecifications.IndexOf(softwareSpecification));
                    }
                }
            }
            
            foreach(var test in userNeeds.SelectMany(x => x.TestCases.Union(
                x.ProductRequirements.SelectMany(y => y.TestCases.Union(
                    y.SoftwareSpecifications.SelectMany(z => z.TestCases))))))
            {
                builder.RegisterMvc(((IUrlHelper)null).TestCase(test), new
                {
                    controller = "Requirements",
                    action = "TestCase",
                    number = test.NumberFullyQualified
                });
            }

            builder.RegisterServices(services => {
                services.AddMvc();
                services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.FileProviders.Add(new PhysicalFileProvider("/Users/pknopf/git/docgen/src/DocGen.Web.Requirements/Internal/Resources"));
                });
                services.AddSingleton<IList<UserNeed>>(userNeeds);
                services.AddSingleton<IRequirementsStore, RequirementsStore>();
                services.AddSingleton<IMenuStore>(menuStore);
                // These regitrations are so that our controllers can inject core DocGen services.
                DocGen.Core.Services.Register(services);
                DocGen.Web.Services.Register(services);
            });

            return new RequirementsContext
            {
                Menu = menuStore,
                WebBuilder = builder
            };
        }
    }
}