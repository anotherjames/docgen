using System;
using System.IO;
using System.Threading.Tasks;
using DocGen.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace DocGen.Web.Tests
{
    public class WebBuilderTests : IDisposable
    {
        IWebBuilder _webBuilder;
        IHostExporter _hostExporter;

        public WebBuilderTests()
        {
            _webBuilder = new Impl.WebBuilder(
                new DocGen.Web.Hosting.Impl.HostBuilder()
            );
            _hostExporter = new Impl.HostExporter();
        }

        [Fact]
        public async Task Can_export_from_file_provider()
        {
            using(var sourceDirectory = new WorkingDirectorySession())
            using(var destinationDirectory = new WorkingDirectorySession()) {
                File.WriteAllText(Path.Combine(sourceDirectory.Directory, "test.txt"), "test1");
                Directory.CreateDirectory(Path.Combine(sourceDirectory.Directory, "nested"));
                File.WriteAllText(Path.Combine(sourceDirectory.Directory, "nested", "test.txt"), "test2");
                _webBuilder.RegisterFiles(new PhysicalFileProvider(sourceDirectory.Directory));
                using(var host = _webBuilder.BuildVirtualHost()) {
                    await _hostExporter.Export(host, destinationDirectory.Directory);
                }
                Assert.True(File.Exists(Path.Combine(destinationDirectory.Directory, "test.txt")));
                Assert.Equal("test1", File.ReadAllText(Path.Combine(destinationDirectory.Directory, "test.txt")));
                Console.WriteLine(Path.Combine(destinationDirectory.Directory, "nested", "test.txt"));
                Assert.True(File.Exists(Path.Combine(destinationDirectory.Directory, "nested", "test.txt")));
                Assert.Equal("test2", File.ReadAllText(Path.Combine(destinationDirectory.Directory, "nested", "test.txt")));
            }
        }

        [Fact]
        public async Task Can_use_with_app_base_with_web()
        {
            _webBuilder.Register("/test", async context =>
            {
                await context.Response.WriteAsync("Hello, World! " + context.Request.Path + " " + context.Request.PathBase);
            });
            
            using(var host = _webBuilder.BuildWebHost("", 5003))
            {
                host.Listen();
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test ", response);
                }
            }
            
            using(var host = _webBuilder.BuildWebHost("/appbase", 5003))
            {
                host.Listen();
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test /appbase", response);
                }
            }
        }
        
        [Fact]
        public async Task Can_use_with_app_base_with_virtual()
        {
            _webBuilder.Register("/test", async context =>
            {
                await context.Response.WriteAsync("Hello, World! " + context.Request.Path + " " + context.Request.PathBase);
            });
            
            using(var host = _webBuilder.BuildVirtualHost(""))
            {
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test ", response);
                }
            }
            
            using(var host = _webBuilder.BuildVirtualHost("/appbase"))
            {
                using(var client = host.CreateClient())
                {
                    var responseMessage = await client.GetAsync("/test");
                    responseMessage.EnsureSuccessStatusCode();
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    Assert.Equal("Hello, World! /test /appbase", response);
                }
            }
        }

        public void Dispose()
        {
            
        }
    }
}