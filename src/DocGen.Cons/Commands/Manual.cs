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
    public partial class Manual
    {
        public static void Configure(CommandLineApplication app, IServiceProvider serviceProvider)
        {
            app.Command("manual", application =>
            {
                application.HelpOption("-? | -h | --help");

                Content.Configure(application, serviceProvider);
                Translations.Configure(application, serviceProvider);
                    
                application.OnExecute(() =>
                {
                    application.ShowHelp();
                    return 0;
                });
            });
        }
    }
}