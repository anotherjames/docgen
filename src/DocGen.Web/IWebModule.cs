using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Web
{
    public interface IWebModule
    {
        void Configure(IApplicationBuilder app, IHostingEnvironment env);

        void ConfigureServices(IServiceCollection services);
    }
}