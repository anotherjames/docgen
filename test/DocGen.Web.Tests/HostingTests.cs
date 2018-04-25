using Xunit;
using DocGen.Web.Hosting;
using Moq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Web.Tests
{
    public class HostingTests
    {
        IHostBuilder _hostBuilder;

        public HostingTests()
        {
            _hostBuilder = new DocGen.Web.Hosting.Impl.HostBuilder();
        }

        [Fact]
        public async Task Can_create_web_host()
        {
            var hostModule = new Mock<IHostModule>();
            hostModule.Setup(x => x.Configure(It.IsAny<IApplicationBuilder>(), It.IsAny<IHostingEnvironment>()))
                .Callback((IApplicationBuilder app, IHostingEnvironment env) => {
                    app.Run(async context => {
                        await context.Response.WriteAsync("Hello, World! " + context.Request.Path);
                    });
                });
            hostModule.Setup(x => x.ConfigureServices(It.IsAny<IServiceCollection>()));
            hostModule.Setup(x => x.Paths).Returns(new ReadOnlyCollection<string>(new List<string>{ "/test" }));

            using(var host = _hostBuilder.BuildWebHost(5002, hostModule.Object)) {
                using(var client = host.CreateClient()) {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test", response);
                }
            }
        }

        [Fact]
        public async Task Can_create_virtual_host()
        {
            var hostModule = new Mock<IHostModule>();
            hostModule.Setup(x => x.Configure(It.IsAny<IApplicationBuilder>(), It.IsAny<IHostingEnvironment>()))
                .Callback((IApplicationBuilder app, IHostingEnvironment env) => {
                    app.Run(async context => {
                        await context.Response.WriteAsync("Hello, World! " + context.Request.Path);
                    });
                });
            hostModule.Setup(x => x.ConfigureServices(It.IsAny<IServiceCollection>()));
            hostModule.Setup(x => x.Paths).Returns(new ReadOnlyCollection<string>(new List<string>{ "/test" }));

            using(var host = _hostBuilder.BuildVirtualHost(hostModule.Object)) {
                using(var client = host.CreateClient()) {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test", response);
                }
            }
        }
    }
}