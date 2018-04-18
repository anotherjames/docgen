using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Web
{
    public static class Services
    {
        public static void Register(ServiceCollection services)
        {
            services.AddSingleton<IWebBuilder, Impl.WebBuilder>();
        }
    }
}