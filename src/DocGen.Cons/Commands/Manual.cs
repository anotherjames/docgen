using System;
using System.IO;
using System.Threading.Tasks;
using DocGen.Web;
using DocGen.Web.Manual;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Statik.Hosting;

namespace DocGen.Cons.Commands
{
    public class Manual
    {
        public static void Configure(CommandLineApplication app, IServiceProvider serviceProvider)
        {
            app.Command("manual", application => {
                application.HelpOption("-? | -h | --help");

                Content.Configure(application, serviceProvider);
    
                application.OnExecute(() => {
                    application.ShowHelp();
                    return 0;
                });
            });
        }

        private static class Content
        {
            public static void Configure(CommandLineApplication app, IServiceProvider serviceProvider)
            {
                app.Command("content", application => {
                    application.HelpOption("-? | -h | --help");
                    
                    application.Command("serve", serveApp => {
                        serveApp.HelpOption("-? | -h | --help");
                        var contentDirectoryOption = serveApp.Option("-c |--content <content>", "The location of the content directory. Defaults to the current directory", CommandOptionType.SingleValue);
    
                        serveApp.OnExecute(() => Serve(serviceProvider,
                            contentDirectoryOption.Value()));
                    });
    
                    application.Command("gen", genApp => {
                        genApp.HelpOption("-? | -h | --help");
                        var contentDirectoryOption = genApp.Option("-c |--content <content>", "The location of the content directory. Defaults to the current directory", CommandOptionType.SingleValue);
                        var destDirectoryOption = genApp.Option("-d |--dest <dest>", "The destination that the files will be written to.", CommandOptionType.SingleValue);
    
                        genApp.OnExecute(() => Generate(serviceProvider,
                            contentDirectoryOption.Value(),
                            destDirectoryOption.Value()));
                    });
    
                    application.OnExecute(() => {
                        application.ShowHelp();
                        return 0;
                    });
                });
            }
    
            private static async Task<int> Serve(IServiceProvider serviceProvider, string contentDirectory)
            {
                var builder = serviceProvider.GetRequiredService<IManualWebBuilder>();
    
                if(string.IsNullOrEmpty(contentDirectory))
                    contentDirectory = Directory.GetCurrentDirectory();

                var manual = await builder.BuildManual(contentDirectory);
    
                using(var web = manual.BuildWebHost()) {
                    web.Listen();
                    
                    Log.Information("Listening on port {Port}.", Statik.StatikDefaults.DefaultPort);
                    Log.Information("Press enter to exit...");
    
                    Console.ReadLine();
                }
    
                return 0;
            }
    
            private static async Task<int> Generate(IServiceProvider serviceProvider,
                string contentDirectory,
                string destinationDirectory)
            {
                var hostExporter = serviceProvider.GetRequiredService<IHostExporter>();
                var builder = serviceProvider.GetRequiredService<IManualWebBuilder>();
    
                if(string.IsNullOrEmpty(contentDirectory))
                    contentDirectory = Directory.GetCurrentDirectory();
    
                if(string.IsNullOrEmpty(destinationDirectory))
                    destinationDirectory = Path.Combine(contentDirectory, "output");
    
                var context = await builder.BuildManual(contentDirectory);
    
                using(var host = context.BuildVirtualHost()) {
                    await hostExporter.Export(host, destinationDirectory);
                }
    
                return 0;
            }
        }
    }
}