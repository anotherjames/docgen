using Microsoft.AspNetCore.Http;

namespace DocGen.Web.Hosting
{
    public interface IHostBuilder
    {
        IWebHost BuildWebHost(int port, PathString appBase, params IHostModule[] modules);

        IVirtualHost BuildVirtualHost(PathString appBase, params IHostModule[] modules);
    }
}