using System.Threading.Tasks;

namespace DocGen.Web.Requirements
{
    public interface IRequirementsConfiguration
    {
        Task Configure(IWebBuilder builder, string contentDirectory);
    }
}