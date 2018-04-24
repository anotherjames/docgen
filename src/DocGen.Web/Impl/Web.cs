using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace DocGen.Web.Impl
{
    public class Web : IWeb
    {
        IWebHost _webHost;
        string _host;

        public Web(List<IWebModule> webModules, List<string> paths, int port)
        {
            Paths = new ReadOnlyCollection<string>(paths);
            _host = $"http://localhost:{port}";
            _webHost = WebHost.CreateDefaultBuilder(new string[]{})
                .UseUrls($"http://*:{port}")
                .UseSetting(WebHostDefaults.ApplicationKey,  Assembly.GetEntryAssembly().GetName().Name)
                .ConfigureLogging(factory => {
                    factory.AddConsole();
                })
                .ConfigureServices(services => {
                    services.AddSingleton<DocGen.Web.Internal.Startup>();
                    services.AddSingleton<List<IWebModule>>(webModules);
                    services.AddSingleton(typeof(IStartup), sp =>
                    {
                        var hostingEnvironment = sp.GetRequiredService<IHostingEnvironment>();
                        return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, typeof(DocGen.Web.Internal.Startup), hostingEnvironment.EnvironmentName));
                    });
                })
                .Build();
        }

        ~Web()
        {
            Dispose();
        }

        public IReadOnlyCollection<string> Paths  { get; }

        public void Listen()
        {
            _webHost.Start();
        }

        public async Task ExportTo(string directory)
        {
            if(!(await Task.Run(() => Directory.Exists(directory)))) {
                await Task.Run(() => Directory.CreateDirectory(directory));
            }

            // Clean all the files currently in the directory
            foreach(var fileToDelete in await Task.Run(() => Directory.GetFiles(directory))) {
                await Task.Run(() => File.Delete(fileToDelete));
            }
            foreach(var directoryToDelete in await Task.Run(() => Directory.GetDirectories(directory))) {
                await Task.Run(() => Directory.Delete(directoryToDelete, true));
            }

            Listen();

            // Copy all the files
            foreach(var path in Paths) {
                await SaveUrlToFile($"{_host}{path}", $"{directory}{path}");
            }
        }

        private async Task SaveUrlToFile(string url, string file) {
            // Ensure the file's parent directories are created.
            var parentDirectory = Path.GetDirectoryName(file);
            if(!(await Task.Run(() => Directory.Exists(parentDirectory)))) {
                await Task.Run(() => Directory.CreateDirectory(parentDirectory));
            }
            using(var client = new HttpClient()) {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using(var requestStream = await response.Content.ReadAsStreamAsync()) {
                using(var fileStream = await Task.Run(() => File.OpenWrite(file)))
                    await requestStream.CopyToAsync(fileStream);
                }
            }
        }

        public void Dispose()
        {
            if(_webHost != null)
            {
                _webHost.Dispose();
                _webHost = null;
            }
        }
    }
}