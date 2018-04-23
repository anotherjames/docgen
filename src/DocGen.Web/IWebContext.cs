using System.Collections.Generic;
using DocGen.Requirements;

namespace DocGen.Web
{
    public interface IWebContext
    {
        string ContentRoot { get; }

        List<UserNeed> UserNeeds { get; }

        List<string> Pages { get; }
    }
}