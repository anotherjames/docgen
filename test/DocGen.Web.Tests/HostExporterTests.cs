using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using DocGen.Tests;
using DocGen.Web.Hosting;
using DocGen.Web.Hosting.Impl;
using DocGen.Web.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DocGen.Web.Tests
{
    public class HostExporterTests
    {
        IHostExporter _hostExporter;
        IHostBuilder _hostBuilder;

        public HostExporterTests()
        {
            _hostExporter = new HostExporter();
            _hostBuilder = new HostBuilder();
        }

        [Fact]
        public async Task Can_export_file_with_extension()
        {
            var module = BuildModuleForPath("/test.js", async context => {
                await context.Response.WriteAsync("response");
            });
            using(var host = _hostBuilder.BuildVirtualHost(module)) {
                using(var directory = new WorkingDirectorySession()) {
                    await _hostExporter.Export(host, directory.Directory);
                    Assert.True(File.Exists(Path.Combine(directory.Directory, "test.js")));
                    Assert.Equal("response", File.ReadAllText(Path.Combine(directory.Directory, "test.js")));
                }
            }
        }

        [Fact]
        public async Task Files_without_extension_get_directory_with_default_file()
        {
            var module = BuildModuleForPath("/test", async context => {
                await context.Response.WriteAsync("response");
            });
            using(var host = _hostBuilder.BuildVirtualHost(module)) {
                using(var directory = new WorkingDirectorySession()) {
                    await _hostExporter.Export(host, directory.Directory);
                    Assert.True(Directory.Exists(Path.Combine(directory.Directory, "test")));
                    Assert.Equal("response", File.ReadAllText(Path.Combine(directory.Directory, "test", "index.html")));
                }
            }
        }

        private IHostModule BuildModuleForPath(string path, Func<HttpContext, Task> action)
        {
            var hostModule = new Mock<IHostModule>();
            hostModule.Setup(x => x.Configure(It.IsAny<IApplicationBuilder>(), It.IsAny<IHostingEnvironment>()))
                .Callback((IApplicationBuilder app, IHostingEnvironment env) => {
                    app.Run(async context => {
                        if(context.Request.Path == path) {
                            await action(context);
                        }
                    });
                });
            hostModule.Setup(x => x.ConfigureServices(It.IsAny<IServiceCollection>()));
            hostModule.Setup(x => x.Paths).Returns(new ReadOnlyCollection<string>(new List<string>{ path }));
            return hostModule.Object;
        }
    }
}