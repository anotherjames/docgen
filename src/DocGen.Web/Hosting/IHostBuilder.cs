namespace DocGen.Web.Hosting
{
    public interface IHostBuilder
    {
        IWebHost BuildWebHost(int port, params IHostModule[] modules);

        IVirtualHost BuildVirtualHost(params IHostModule[] modules);
    }
}