using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using DocGen.Requirements.Tests;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DocGen.Tests.Cons
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblies = new List<Assembly>
            {
                typeof(DocGen.Core.Tests.YamlParserTests).Assembly,
                typeof(DocGen.Requirements.Tests.RequirementsParserTests).Assembly
            };

            foreach(var assembly in assemblies)
            {
                TestAssembly(assembly);
            }
        }

        static void TestAssembly(Assembly assembly)
        {
            var messageSink = new ConsoleMessageSink();
            var assemblyInfo = new ReflectionAssemblyInfo(assembly);
            var framework = new XunitTestFramework(messageSink);
            var executor = new XunitTestFrameworkExecutor(assembly.GetName(), new NullSourceInformationProvider(), messageSink);
            
            var executionOptions = Xunit.TestFrameworkOptions.ForExecution();
            var discoveryOptions = Xunit.TestFrameworkOptions.ForDiscovery();
            executor.RunAll(messageSink, discoveryOptions, executionOptions);
            
            messageSink.Finished.WaitOne();
        }
    }

    public class ConsoleMessageSink : IMessageSink
    {
        public bool OnMessage(IMessageSinkMessage message)
        {
            if(message is ITestAssemblyFinished)
                Finished.Set();

            return true;
        }

        public ManualResetEvent Finished = new ManualResetEvent(initialState: false);
    }

    public class NullSourceInformationProvider : ISourceInformationProvider
    {
        public ISourceInformation GetSourceInformation(ITestCase testCase)
        {
            return null;
        }

        public void Dispose()
        {

        }
    }
}
