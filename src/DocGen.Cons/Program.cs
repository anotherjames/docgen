using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DocGen.Core;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DocGen.Cons
{
    class Program
    {
        public static Task<int> Main(string[] args)
        {
            IServiceProvider serviceProvider;

            var app = new CommandLineApplication();
            app.Name = "docgen";
            app.FullName = "A tool to generate documentation from a source directory";

            app.HelpOption("-? | -h | --help");

            var contentDirectoryOption = app.Option("-c |--content <content>",
                "The location of the content directory. Defaults to the current directory.",
                CommandOptionType.SingleValue,
                true);

            GetServiceProvider = () =>
            {
                var services = new ServiceCollection();
                
                Services.Register(services);
                Requirements.Services.Register(services);
                Web.Requirements.Services.Register(services);
                Web.Manual.Services.Register(services);

                services.AddOptions();
                
                var contentDirectory = contentDirectoryOption.Value();
                if (string.IsNullOrEmpty(contentDirectory))
                    contentDirectory = Directory.GetCurrentDirectory();

                services.Configure<DocGenOptions>(options =>
                {
                    options.ContentDirectory = contentDirectory;
                });
                
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .CreateLogger();

                return serviceProvider = services.BuildServiceProvider();
            };
            
            Commands.Dhf.Configure(app);
            Commands.Manual.Configure(app);

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            try
            {
                return Task.FromResult(app.Execute(args));
            }
            catch (Exception ex)
            {
                var exceptions = new List<Exception>();
                Action<Exception> addException = null;
                addException = (exception) =>
                {
                    if (exception is AggregateException)
                    {
                        foreach (var inner in (exception as AggregateException).InnerExceptions)
                        {
                            addException(inner);
                        }
                    }
                    else
                    {
                        exceptions.Add(exception);
                        if(exception.InnerException != null)
                            addException(exception.InnerException);
                    }
                };

                addException(ex);

                exceptions.Reverse();

                foreach (var exception in exceptions)
                {
                    if (exception is CommandParsingException)
                    {
                        Log.Error(exception.Message);
                    }else if (exception is DocGenException)
                    {
                        Log.Error(exception.Message);
                    }
                    else
                    {
                        Log.Error("{@Exception}", exception);
                    }
                }

                return Task.FromResult(-1);
            }
        }

        public static Func<IServiceProvider> GetServiceProvider = null;
    }
}
