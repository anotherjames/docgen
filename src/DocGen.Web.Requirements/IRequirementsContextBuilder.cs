using System.Threading.Tasks;

namespace DocGen.Web.Requirements
{
    public interface IRequirementsContextBuilder
    {
        Task<RequirementsContext> Build(string contentDirectory);
    }
}