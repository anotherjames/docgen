using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DocGen.Web
{
    public static class Services
    {
        public static void Register(ServiceCollection services)
        {
            services.AddTransient<IWebBuilder, Impl.WebBuilder>();
            services.AddSingleton<IWebContextBuilder, Impl.WebContextBuilder>();
            services.AddSingleton<Hosting.IHostBuilder, Hosting.Impl.HostBuilder>();
            services.AddSingleton<IHostExporter, Impl.HostExporter>();
        }
    }
}