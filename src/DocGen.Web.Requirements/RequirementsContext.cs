using Statik.Web;

namespace DocGen.Web.Requirements
{
    public class RequirementsContext
    {
        public IWebBuilder WebBuilder { get; set; }
        
        public IMenuStore Menu { get; set; }
    }
}