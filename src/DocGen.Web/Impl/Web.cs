using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DocGen.Web.Impl
{
    public class Web : IWeb
    {
        IWebHost _webHost;
        
        public Web(int port)
        {
            _webHost = WebHost.CreateDefaultBuilder(new string[]{})
                .UseUrls($"http://*:{port}")
                .UseStartup<Startup>()
                .Build();
        }

        ~Web()
        {
            Dispose();
        }

        public void Listen()
        {
            _webHost.Start();
        }

        public void Dispose()
        {
            if(_webHost != null)
            {
                _webHost.Dispose();
                _webHost = null;
            }
        }
    }
}