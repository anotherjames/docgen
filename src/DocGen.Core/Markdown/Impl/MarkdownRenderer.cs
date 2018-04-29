using System;
using System.IO;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions;

namespace DocGen.Core.Markdown.Impl
{
    public class MarkdownRenderer : IMarkdownRenderer
    {
        IYamlParser _yamlParser;

        public MarkdownRenderer(IYamlParser yamlParser)
        {
            _yamlParser = yamlParser;
        }

        public MarkdownRenderResult RenderMarkdown(string markdown)
        {
            var yaml = _yamlParser.ParseYaml(markdown);

            var html = Markdig.Markdown.ToHtml(yaml.Markdown,
                new Markdig.MarkdownPipelineBuilder()
                    .Configure("yaml")
                    .Build());
            
            if(!string.IsNullOrEmpty(html))
            {
                html = html.TrimEnd(Environment.NewLine.ToCharArray());
            }

            return new MarkdownRenderResult(yaml.Yaml, html);
        }

        public async Task<MarkdownRenderResult> RenderMarkdownFromFile(string file)
        {
            // TODO: cache this
            var content = await Task.Run(() => File.ReadAllText(file));
            return RenderMarkdown(content);
        }
    }
}