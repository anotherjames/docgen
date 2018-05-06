using Statik.Hosting;

namespace DocGen.Web.Manual
{
    public interface IManualWeb
    {
        IWebHost BuildWebHost(string appBase = null, int port = Statik.StatikDefaults.DefaultPort);

        IVirtualHost BuildVirtualHost(string appBase = null);
    }
}