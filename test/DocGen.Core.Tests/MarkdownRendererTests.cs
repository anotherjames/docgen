using System;
using System.Text;
using DocGen.Core.Markdown;
using Xunit;

namespace DocGen.Core.Tests
{
    public class MarkdownRendererTests
    {
        IMarkdownRenderer _markdownRenderer;

        public MarkdownRendererTests()
        {
            _markdownRenderer = new DocGen.Core.Markdown.Impl.MarkdownRenderer(new DocGen.Core.Markdown.Impl.YamlParser());
        }

        [Fact]
        public void Can_render_html()
        {
            var result = _markdownRenderer.RenderMarkdown("Test paragraph");

            Assert.Null(result.Yaml);
            Assert.Equal("<p>Test paragraph</p>", result.Html);
        }

        [Fact]
        public void Can_render_html_with_yaml()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("Test paragraph");

            var result = _markdownRenderer.RenderMarkdown(document.ToString());

            Assert.NotNull(result.Yaml);
            Assert.Equal("<p>Test paragraph</p>", result.Html);
        }
    }
}
