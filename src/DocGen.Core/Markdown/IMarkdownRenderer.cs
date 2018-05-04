using System;
using System.Threading.Tasks;

namespace DocGen.Core.Markdown
{
    public interface IMarkdownRenderer
    {
        MarkdownRenderResult RenderMarkdown(string markdown);

        Task<MarkdownRenderResult> RenderMarkdownFromFile(string file);

        string TransformMarkdown(string markdown, Func<string, string> replacement);
    }
}