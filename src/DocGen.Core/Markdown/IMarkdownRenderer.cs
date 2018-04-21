namespace DocGen.Core.Markdown
{
    public interface IMarkdownRenderer
    {
        MarkdownRenderResult RenderMarkdown(string markdown);
    }
}