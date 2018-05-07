using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

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
                    .UseAdvancedExtensions()
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

        public List<TocEntry> ExtractTocEntries(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);

            var result = new List<TocEntry>();
            
            void WalkBlock(Block block)
            {
                if (block is HeadingBlock headingBlock)
                {
                    result.Add(new TocEntry
                    {
                        Level = headingBlock.Level,
                        Id = headingBlock.TryGetAttributes().Id,
                        Title = MarkdownHelpers.RenderLeafInlineRaw(headingBlock)
                    });
                }
            }

            foreach (var item in document)
            {
                WalkBlock(item);
            }

            return result;
        }
    }
}