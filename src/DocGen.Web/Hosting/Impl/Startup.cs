using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using static DocGen.Web.WebBuilderExtensions;

namespace DocGen.Web.Hosting.Impl
{
    internal class Startup
    {
        List<IHostModule> _modules;

        public Startup(List<IHostModule> modules)
        {
            _modules = modules;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            foreach(var module in _modules)
            {
                module.ConfigureServices(services);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            foreach(var module in _modules)
            {
                module.Configure(app, env);
            }
        }
    }
}