using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace DocGen.Web
{
    public interface IWebModule
    {
        void Configure(IApplicationBuilder app, IHostingEnvironment env);
    }
}