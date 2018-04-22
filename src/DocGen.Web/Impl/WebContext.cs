using System.Collections.Generic;
using DocGen.Requirements;

namespace DocGen.Web.Impl
{
    public class WebContext : IWebContext
    {
        public WebContext(string contentRoot, List<UserNeed> userNeeds, List<string> pages)
        {
            ContentRoot = contentRoot;
            UserNeeds = userNeeds;
            Pages = pages;
        }

        public string ContentRoot { get; }

        public List<UserNeed> UserNeeds { get; }

        public List<string> Pages { get; }
    }
}