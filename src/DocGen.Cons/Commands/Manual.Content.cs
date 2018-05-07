using System;
using System.IO;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Web.Manual;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Statik.Hosting;

// ReSharper disable once MemberHidesStaticFromOuterClass

namespace DocGen.Cons.Commands
{
    public partial class Manual
    {
        private static class Content
        {
            public static void Configure(CommandLineApplication app)
            {
                app.Command("content", application =>
                {
                    application.HelpOption("-? | -h | --help");
                    
                    application.Command("serve", serveApp =>
                    {
                        serveApp.HelpOption("-? | -h | --help");
                       
                        serveApp.OnExecute(() => Serve());
                    });
    
                    application.Command("gen", genApp =>
                    {
                        genApp.HelpOption("-? | -h | --help");
                        
                        var destDirectoryOption = genApp.Option("-d |--dest <dest>", "The destination that the files will be written to.", CommandOptionType.SingleValue);
    
                        genApp.OnExecute(() => Generate(destDirectoryOption.Value()));
                    });
    
                    application.OnExecute(() =>
                    {
                        application.ShowHelp();
                        return 0;
                    });
                });
            }
    
            private static async Task<int> Serve()
            {
                var builder = Program.GetServiceProvider().GetRequiredService<IManualWebBuilder>();
    
                var manual = await builder.BuildManual();
    
                using(var web = manual.BuildWebHost())
                {
                    web.Listen();
                    
                    Log.Information("Listening on port {Port}.", Statik.StatikDefaults.DefaultPort);
                    Log.Information("Press enter to exit...");
    
                    Console.ReadLine();
                }
    
                return 0;
            }
    
            private static async Task<int> Generate(string destinationDirectory)
            {
                var serviceProvider = Program.GetServiceProvider();
                var hostExporter = serviceProvider.GetRequiredService<IHostExporter>();
                var builder = serviceProvider.GetRequiredService<IManualWebBuilder>();
                var options = serviceProvider.GetRequiredService<IOptions<DocGenOptions>>().Value;
                
                if(string.IsNullOrEmpty(destinationDirectory))
                    destinationDirectory = Path.Combine(options.ContentDirectory, "output");
    
                var context = await builder.BuildManual();
    
                using(var host = context.BuildVirtualHost())
                    await hostExporter.Export(host, destinationDirectory);
    
                return 0;
            }
        }
    }
}