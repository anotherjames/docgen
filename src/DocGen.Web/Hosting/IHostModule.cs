using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Web.Hosting
{
    public interface IHostModule
    {
        void Configure(IApplicationBuilder app, IHostingEnvironment env);

        void ConfigureServices(IServiceCollection services);

        IReadOnlyCollection<string> Paths { get; }
    }
}