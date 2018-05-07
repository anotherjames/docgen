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
            public static void Configure(CommandLineApplication app)
            {
                app.Command("translations", application =>
                {
                    application.HelpOption("-? | -h | --help");
                    
                    application.Command("update", serveApp =>
                    {
                        serveApp.HelpOption("-? | -h | --help");
                       
                        serveApp.OnExecute(() => Update());
                    });
    
                    application.OnExecute(() =>
                    {
                        application.ShowHelp();
                        return 0;
                    });
                });
            }
    
            private static async Task<int> Update()
            {
                var translations = Program.GetServiceProvider().GetRequiredService<IManualTranslations>();
    
                await translations.RegenerateTemplate();

                return 0;
            }
        }
    }
}