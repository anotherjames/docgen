using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DocGen.Web;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DocGen.Cons.Commands
{
    public class Content
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
            var webBuilder = serviceProvider.GetRequiredService<DocGen.Web.IWebBuilder>();
            var requirementsConfiguration = serviceProvider.GetRequiredService<DocGen.Web.Requirements.IRequirementsConfiguration>();

            if(string.IsNullOrEmpty(contentDirectory))
                contentDirectory = Directory.GetCurrentDirectory();

            await requirementsConfiguration.Configure(webBuilder, contentDirectory);

            using(var web = webBuilder.BuildWebHost()) {
                web.Listen();
                
                Log.Information("Listening on port {Port}.", WebDefaults.DefaultPort);
                Log.Information("Press enter to exit...");

                Console.ReadLine();
            }

            return 0;
        }

        private static async Task<int> Generate(IServiceProvider serviceProvider,
            string contentDirectory,
            string destinationDirectory)
        {
            var webBuilder = serviceProvider.GetRequiredService<IWebBuilder>();
            var hostExporter = serviceProvider.GetRequiredService<IHostExporter>();
            var requirementsConfiguration = serviceProvider.GetRequiredService<DocGen.Web.Requirements.IRequirementsConfiguration>();

            if(string.IsNullOrEmpty(contentDirectory))
                contentDirectory = Directory.GetCurrentDirectory();

            if(string.IsNullOrEmpty(destinationDirectory))
                destinationDirectory = Path.Combine(contentDirectory, "output");

            await requirementsConfiguration.Configure(webBuilder, contentDirectory);

            using(var host = webBuilder.BuildVirtualHost()) {
                await hostExporter.Export(host, destinationDirectory);
            }

            return 0;
        }
    }
}