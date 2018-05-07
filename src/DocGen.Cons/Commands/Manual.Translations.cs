using System;
using System.IO;
using System.Threading.Tasks;
using DocGen.Web.Manual;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Statik.Hosting;

namespace DocGen.Cons.Commands
{
    public partial class Manual
    {
        private static class Translations
        {
            public static void Configure(CommandLineApplication app, IServiceProvider serviceProvider)
            {
                app.Command("translations", application =>
                {
                    application.HelpOption("-? | -h | --help");
                    
                    application.Command("update", serveApp =>
                    {
                        serveApp.HelpOption("-? | -h | --help");
                        var contentDirectoryOption = serveApp.Option("-c |--content <content>", "The location of the content directory. Defaults to the current directory", CommandOptionType.SingleValue);
    
                        serveApp.OnExecute(() => Update(serviceProvider,
                            contentDirectoryOption.Value()));
                    });
    
                    application.OnExecute(() =>
                    {
                        application.ShowHelp();
                        return 0;
                    });
                });
            }
    
            private static async Task<int> Update(IServiceProvider serviceProvider, string contentDirectory)
            {
                var translations = serviceProvider.GetRequiredService<IManualTranslations>();
    
                if(string.IsNullOrEmpty(contentDirectory))
                    contentDirectory = Directory.GetCurrentDirectory();

                await translations.RegenerateTemplate(contentDirectory);

                return 0;
            }
        }
    }
}