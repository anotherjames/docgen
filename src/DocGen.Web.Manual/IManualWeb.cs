using System;
using DocGen.Web.Hosting;

namespace DocGen.Web.Manual
{
    public interface IManualWeb
    {
        IWebHost BuildWebHost(string appBase = null, int port = WebDefaults.DefaultPort);

        IVirtualHost BuildVirtualHost(string appBase = null);
    }
}