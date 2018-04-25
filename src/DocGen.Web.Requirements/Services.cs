using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DocGen.Web.Requirements
{
    public static class Services
    {
        public static void Register(ServiceCollection services)
        {
            services.AddSingleton<IRequirementsConfiguration, Impl.RequirementsConfiguration>();
        }
    }
}