using System;
using System.Collections.Generic;
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

            {
                var services = new ServiceCollection();
                DocGen.Core.Services.Register(services);
                DocGen.Requirements.Services.Register(services);
                DocGen.Web.Services.Register(services);
                DocGen.Web.Requirements.Services.Register(services);
                serviceProvider = services.BuildServiceProvider();
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            var app = new CommandLineApplication();
            app.Name = "docgen";
            app.FullName = "A tool to generate documentation from a source directory";

            app.HelpOption("-? | -h | --help");

            Commands.Content.Configure(app, serviceProvider);

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
    }
}
