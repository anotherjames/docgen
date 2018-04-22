using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DocGen.Tests
{
    public class TestExecutor
    {
        public static void RunTests(params Assembly[] assemblies)
        {
            foreach(var assembly in assemblies)
            {
                TestAssembly(assembly);
            }
        }

        private static void TestAssembly(Assembly assembly)
        {
            var messageSink = new ConsoleMessageSink();
            var assemblyInfo = new ReflectionAssemblyInfo(assembly);
            var framework = new XunitTestFramework(messageSink);
            var executor = new XunitTestFrameworkExecutor(assembly.GetName(), new NullSourceInformationProvider(), messageSink);
            
            var executionOptions = TestFrameworkOptions.ForExecution();
            var discoveryOptions = TestFrameworkOptions.ForDiscovery();
            executor.RunAll(messageSink, discoveryOptions, executionOptions);
            
            messageSink.Finished.WaitOne();
        }

        class TestFrameworkOptions : ITestFrameworkDiscoveryOptions, ITestFrameworkExecutionOptions
        {
            readonly Dictionary<string, object> properties = new Dictionary<string, object>();

            // Force users to use one of the factory methods
            TestFrameworkOptions() { }

            public static ITestFrameworkDiscoveryOptions ForDiscovery()
            {
                ITestFrameworkDiscoveryOptions result = new TestFrameworkOptions();
                return result;
            }

            public static ITestFrameworkExecutionOptions ForExecution()
            {
                ITestFrameworkExecutionOptions result = new TestFrameworkOptions();
                return result;
            }

            public TValue GetValue<TValue>(string name)
            {
                object result;
                if (properties.TryGetValue(name, out result))
                    return (TValue)result;

                return default(TValue);
            }

            public void SetValue<TValue>(string name, TValue value)
            {
                if (value == null)
                    properties.Remove(name);
                else
                    properties[name] = value;
            }
        }

        class ConsoleMessageSink : IMessageSink
        {
            public bool OnMessage(IMessageSinkMessage message)
            {
                if(message is ITestAssemblyFinished)
                    Finished.Set();

                return true;
            }

            public ManualResetEvent Finished = new ManualResetEvent(initialState: false);
        }

        class NullSourceInformationProvider : ISourceInformationProvider
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
}
