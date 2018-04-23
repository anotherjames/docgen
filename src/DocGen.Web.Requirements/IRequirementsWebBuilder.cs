using System.Threading.Tasks;

namespace DocGen.Web.Requirements
{
    public interface IRequirementsWebBuilder
    {
        Task<IWeb> Build(string contentDirectory, int port);
    }
}