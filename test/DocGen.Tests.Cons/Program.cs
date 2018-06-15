namespace DocGen.Tests.Cons
{
    class Program
    {
        static void Main(string[] args)
        {
            TestExecutor.RunTests(
                typeof(DocGen.Core.Tests.YamlParserTests).Assembly,
                typeof(DocGen.Requirements.Tests.RequirementsParserTests).Assembly,
                typeof(DocGen.Web.Requirements.Tests.MenuStoreTests).Assembly
            );
        }
    }
}
