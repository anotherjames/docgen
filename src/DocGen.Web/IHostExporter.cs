using System.Threading.Tasks;
using DocGen.Web.Hosting;

namespace DocGen.Web
{
    public interface IHostExporter
    {
        Task Export(IHost host, string destinationDirectory);
    }
}