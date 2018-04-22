using System.Threading.Tasks;

namespace DocGen.Web
{
    public interface IWebContextBuilder
    {
        Task<IWebContext> Build(string contentDirectory);
    }
}