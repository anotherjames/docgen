using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace DocGen.Cons.Commands
{
    public class Content
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Command("content", application =>
            {
                application.HelpOption("-? | -h | --help");
                
                application.Command("serve", removeApp =>
                {
                    removeApp.HelpOption("-? | -h | --help");

                    removeApp.OnExecute(() => Serve());
                });

                application.OnExecute(() =>
                {
                    application.ShowHelp();
                    return 0;
                });
            });
        }

        public static Task<int> Serve()
        {
            
            return Task.FromResult(0);
        }
    }
}