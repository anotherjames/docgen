using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocGen.Core.Markdown
{
    public interface IMarkdownRenderer
    {
        MarkdownRenderResult RenderMarkdown(string markdown);

        Task<MarkdownRenderResult> RenderMarkdownFromFile(string file);

        List<TocEntry> ExtractTocEntries(string markdown);
    }
}