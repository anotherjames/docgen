using System;
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
                
                application.Command("serve", removeApp =>
                {
                    removeApp.HelpOption("-? | -h | --help");

                    removeApp.OnExecute(() => Serve(serviceProvider));
                });

                application.OnExecute(() =>
                {
                    application.ShowHelp();
                    return 0;
                });
            });
        }

        public static Task<int> Serve(IServiceProvider serviceProvider, int port = 8000)
        {
            var webBuilder = serviceProvider.GetService<IWebBuilder>();
            var web = webBuilder.Build(port);

            Log.Information($"Listening on port ${port}");

            return Task.FromResult(0);
        }
    }
}