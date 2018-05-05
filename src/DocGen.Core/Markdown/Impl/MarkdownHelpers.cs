using System.IO;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace DocGen.Core.Markdown.Impl
{
    public static class MarkdownHelpers
    {
        public static string RenderLeafInlineRaw(LeafBlock leafBlock)
        {
            using (var stringWriter = new StringWriter())
            {
                var renderer = new RawRenderer(stringWriter);
                renderer.WriteLeafInline(leafBlock);
                return stringWriter.ToString();
            }
        }

        class RawRenderer : TextRendererBase<RawRenderer>
        {
            public RawRenderer(TextWriter writer) : base(writer)
            {
                ObjectRenderers.Add(new InlineLiteralRenderer());
            }

            class InlineLiteralRenderer : MarkdownObjectRenderer<RawRenderer, LiteralInline>
            {
                protected override void Write(RawRenderer renderer, LiteralInline obj)
                {
                    renderer.Write(ref obj.Content);
                }
            }
        }
    }
}