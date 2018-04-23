using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace DocGen.Web
{
    public static class Services
    {
        public static void Register(ServiceCollection services)
        {
            services.AddSingleton<IWebBuilder, Impl.WebBuilder>();
            services.AddSingleton<IWebContextBuilder, Impl.WebContextBuilder>();
            services.AddMvc();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(new PhysicalFileProvider("/Users/pknopf/git/docgen/src/DocGen.Web.Requirements/Internal/Resources"));
            });
        }
    }
}