namespace DocGen.Core.Markdown
{
    public class MarkdownRenderResult
    {
        public MarkdownRenderResult(dynamic yaml, string html)
        {
            Yaml = yaml;
            Html = html;
        }

        public dynamic Yaml { get; }

        public string Html { get ;}
    }
}