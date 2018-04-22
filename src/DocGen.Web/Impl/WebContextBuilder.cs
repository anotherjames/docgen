using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Requirements;

namespace DocGen.Web.Impl
{
    public class WebContextBuilder : IWebContextBuilder
    {
        IRequirementsBuilder _requirementsBuilder;

        public WebContextBuilder(IRequirementsBuilder requirementsBuilder)
        {
            _requirementsBuilder = requirementsBuilder;
        }

        public async Task<IWebContext> Build(string contentDirectory)
        {
            if(!Directory.Exists(contentDirectory))
                throw new DocGenException($"Requirements directory {contentDirectory} doesn't exist");

            var requirementsDirectory = Path.Combine(contentDirectory, "requirements");
            var pagesDirectory = Path.Combine(contentDirectory, "pages");

            if(!Directory.Exists(requirementsDirectory))
                throw new DocGenException($"Requirements directory {requirementsDirectory} doesn't exist");

            if(!Directory.Exists(pagesDirectory))
                throw new DocGenException($"Pages directory {pagesDirectory} doesn't exist");

            var userNeeds = await _requirementsBuilder.BuildRequirementsFromDirectory(requirementsDirectory);
            var pages = await Task.Run(() => Directory.GetFiles(pagesDirectory, "*.md", System.IO.SearchOption.AllDirectories));

            return new WebContext(contentDirectory, userNeeds, pages.ToList());
        }
    }
}