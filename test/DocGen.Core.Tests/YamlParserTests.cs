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
            document.Append("---");

            var result = _yamlParser.ParseYaml(document.ToString());

            Assert.Equal("1.0", (string)result.Yaml.Number);
            Assert.Equal("Test title", (string)result.Yaml.Title);
            Assert.Equal("Test category", (string)result.Yaml.Category);
            Assert.True(string.IsNullOrEmpty(result.Markdown));
        }

        [Fact]
        public void Returns_null_when_no_yaml()
        {
            var document = new StringBuilder();
            document.Append("Just some **markdown**...");

            var result = _yamlParser.ParseYaml(document.ToString());

            Assert.Null(result.Yaml);
            Assert.Equal("Just some **markdown**...", result.Markdown);
        }
    }
}
