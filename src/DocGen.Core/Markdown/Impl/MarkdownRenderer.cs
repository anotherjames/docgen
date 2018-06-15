using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions;
using Markdig.Extensions.CustomContainers;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownTranslator.Utilities;

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

            var html = Markdig.Markdown.ToHtml(yaml.Markdown, DocgenDefaults.GetDefaultPipeline());
            
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
            var pipeline = DocgenDefaults.GetDefaultPipeline();
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
                if (block is CustomContainer customContainer)
                {
                    foreach (var child in customContainer)
                    {
                        WalkBlock(child);
                    }
                }
            }

            foreach (var item in document)
            {
                WalkBlock(item);
            }

            return result;
        }

        public string TransformLinks(string markdown, Func<string, string> func)
        {
            using (var stringWriter = new StringWriter())
            {
                var transformRenderer = new MarkdownTransformRenderer(stringWriter, markdown);
                transformRenderer.ObjectRenderers.Add(new LinkTransformRenderer(func));

                var document = Markdig.Markdown.Parse(markdown, DocgenDefaults.GetDefaultPipeline());

                transformRenderer.Render(document);

                // Flush any remaining markdown content.
                transformRenderer.Writer.Write(transformRenderer.TakeNext(transformRenderer.OriginalMarkdown.Length - transformRenderer.LastWrittenIndex));
                
                stringWriter.Flush();
                
                return stringWriter.ToString();
            }
        }

        class LinkTransformRenderer : MarkdownObjectRenderer<MarkdownTransformRenderer, LinkInline>
        {
            readonly Func<string, string> _func;

            public LinkTransformRenderer(Func<string, string> func)
            {
                _func = func;
            }
            
            protected override void Write(MarkdownTransformRenderer renderer, LinkInline obj)
            {
                if (obj.IsAutoLink)
                {
                    return;
                }
                
                if (!obj.UrlSpan.HasValue)
                {
                    renderer.WriteChildren(obj);
                    return;
                }

                // Make sure we flush everything up to the url, which we will replace.
                renderer.Write(renderer.TakeNext(obj.UrlSpan.Value.Start - renderer.LastWrittenIndex));
                    
                var url = renderer.TakeNext(obj.UrlSpan.Value.Length);
                var newUrl = _func(url);
                
                renderer.Write(newUrl);
            }
        }
    }
}