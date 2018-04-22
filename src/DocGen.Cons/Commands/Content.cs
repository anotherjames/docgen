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
            app.Command("content", application =>
            {
                application.HelpOption("-? | -h | --help");
                
                application.Command("serve", serveApp =>
                {
                    serveApp.HelpOption("-? | -h | --help");
                    var contentDirectoryOption = serveApp.Option("-c |--content <content>", "The location of the content directory. Defaults to the current directory", CommandOptionType.SingleValue);

                    serveApp.OnExecute(() => Serve(serviceProvider,
                        contentDirectoryOption.Value()));
                });

                application.OnExecute(() =>
                {
                    application.ShowHelp();
                    return 0;
                });
            });
        }

        public static async Task<int> Serve(IServiceProvider serviceProvider, string contentDirectory)
        {
            var webBuilder = serviceProvider.GetRequiredService<IWebBuilder>();
            var webContextBuilder = serviceProvider.GetRequiredService<IWebContextBuilder>();

            if(string.IsNullOrEmpty(contentDirectory))
                contentDirectory = Directory.GetCurrentDirectory();

            var webContext = await webContextBuilder.Build(contentDirectory);

            using(var web = webBuilder.Build(webContext, DocGen.Web.WebDefaults.DefaultPort))
            {
                web.Listen();
                
                Log.Information("Listening on port {Port}.", DocGen.Web.WebDefaults.DefaultPort);
                Log.Information("Press enter to exit...");

                Console.ReadLine();
            }

            return 0;
        }
    }
}