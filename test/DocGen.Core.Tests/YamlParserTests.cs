using System;
using System.Text;
using DocGen.Core.Markdown;
using Xunit;

namespace DocGen.Core.Tests
{
    public class YamlParserTests
    {
        IYamlParser _yamlParser;

        public YamlParserTests()
        {
            _yamlParser = new DocGen.Core.Markdown.Impl.YamlParser();
        }

        [Fact]
        public void Can_parse_yaml()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");

            var yaml = _yamlParser.ParseYaml(document.ToString());

            Assert.Equal("1.0", (string)yaml.Number);
            Assert.Equal("Test title", (string)yaml.Title);
            Assert.Equal("Test category", (string)yaml.Category);
        }

        [Fact]
        public void Returns_null_when_no_yaml()
        {
            var document = new StringBuilder();
            document.AppendLine("Just some **markdown**...");

            var yaml = _yamlParser.ParseYaml(document.ToString());

            Assert.Null(yaml);
        }
    }
}
