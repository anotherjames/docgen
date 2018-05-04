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
            document.Append("Test paragraph");

            var result = _markdownRenderer.RenderMarkdown(document.ToString());

            Assert.NotNull(result.Yaml);
            Assert.Equal("<p>Test paragraph</p>", result.Html);
        }

        [Fact]
        public void Can_replace_text()
        {
            var result = _markdownRenderer.TransformMarkdown("test", val => $"{val}-append");
            Assert.Equal("<p>test-append</p>", result);
        
            result = _markdownRenderer.TransformMarkdown("test*another*", val => $"{val}-append");
            Assert.Equal("<p>test-append<em>another-append</em></p>", result);
        
            var content = new StringBuilder();
            content.AppendLine("a | b");
            content.AppendLine("- | -");
            content.AppendLine("0 | 0");
            result = _markdownRenderer.TransformMarkdown(content.ToString(), val => $"{val}-append");
            content = new StringBuilder();
            content.AppendLine("<table>");
            content.AppendLine("<thead>");
            content.AppendLine("<tr>");
            content.AppendLine("<th>a-append</th>");
            content.AppendLine("<th>b-append</th>");
            content.AppendLine("</tr>");
            content.AppendLine("</thead>");
            content.AppendLine("<tbody>");
            content.AppendLine("<tr>");
            content.AppendLine("<td>0-append</td>");
            content.AppendLine("<td>0-append</td>");
            content.AppendLine("</tr>");
            content.AppendLine("</tbody>");
            content.Append("</table>");
            Assert.Equal(content.ToString(), result);
        }
    }
}
