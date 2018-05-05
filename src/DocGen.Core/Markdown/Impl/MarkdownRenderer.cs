using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions;
using Markdig.Helpers;
using Markdig.Renderers;
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

        public string TransformMarkdown(string markdown, Func<string, string> replacement)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            
            // Do a fake rendering that replaces content.
            var replacementRenderer = new TextReplacementRenderer(TextWriter.Null, replacement);
            pipeline.Setup(replacementRenderer);
            replacementRenderer.Render(document);
            
            using (var stringWriter = new StringWriter())
            {
                var htmlRenderer = new HtmlRenderer(stringWriter);
                pipeline.Setup(htmlRenderer);
                htmlRenderer.Render(document);
                stringWriter.Flush();
                return stringWriter.ToString().TrimEnd(Environment.NewLine.ToCharArray());
            }
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
        
        public class TextReplacementRenderer : TextRendererBase<TextReplacementRenderer>
        {
            public TextReplacementRenderer(TextWriter writer, Func<string, string> replacement) : base(writer)
            {
                ObjectRenderers.Add(new CustomParagraphRenderer());
                ObjectRenderers.Add(new CustomLiteralInlineRenderer(replacement));
            }
        
            class CustomParagraphRenderer : MarkdownObjectRenderer<TextReplacementRenderer, ParagraphBlock>
            {
                protected override void Write(TextReplacementRenderer renderer, ParagraphBlock obj)
                {
                    renderer.WriteLeafInline(obj);
                }
            }
        
            class CustomLiteralInlineRenderer : MarkdownObjectRenderer<TextReplacementRenderer, LiteralInline>
            {
                readonly Func<string, string> _replacement;
        
                public CustomLiteralInlineRenderer(Func<string, string> replacement)
                {
                    _replacement = replacement;
                }
                
                protected override void Write(TextReplacementRenderer renderer, LiteralInline obj)
                {
                    obj.Content = new StringSlice(_replacement(obj.Content.ToString()));
                }
            }
        }
    }
}