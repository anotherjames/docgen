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
            DocGen.Tests.TestExecutor.RunTests(
                typeof(DocGen.Core.Tests.YamlParserTests).Assembly,
                typeof(DocGen.Requirements.Tests.RequirementsParserTests).Assembly,
                typeof(DocGen.Web.Tests.HostingTests).Assembly
            );
        }
    }
}
