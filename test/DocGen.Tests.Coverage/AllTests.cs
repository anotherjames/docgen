using System;
using Xunit;

namespace DocGen.Tests.Coverage
{
    public class AllTests
    {
        [Fact]
        public void Run_all_tests()
        {
            DocGen.Tests.TestExecutor.RunTests(
                typeof(DocGen.Core.Tests.YamlParserTests).Assembly,
                typeof(DocGen.Requirements.Tests.RequirementsParserTests).Assembly
            );
        }
    }
}
