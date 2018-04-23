using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using static DocGen.Web.WebBuilderExtensions;

namespace DocGen.Web.Internal
{
    internal class Startup
    {
        List<IWebModule> _webModules;

        public Startup(List<IWebModule> webModules)
        {
            _webModules = webModules;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            foreach(var module in _webModules) {
                module.ConfigureServices(services);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            foreach(var module in _webModules) {
                module.Configure(app, env);
            }
        }
    }
}