using System.Threading.Tasks;

namespace DocGen.Web
{
    public interface IWebBuilder
    {
        IWeb Build(IWebContext webContext, int port = 8000);
    }
}