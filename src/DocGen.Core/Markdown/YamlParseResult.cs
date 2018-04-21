namespace DocGen.Core.Markdown
{
    public class YamlParseResult
    {
        public YamlParseResult(dynamic yaml, string markdown)
        {
            Yaml = yaml;
            Markdown = markdown;
        }

        public dynamic Yaml { get; }

        public string Markdown { get; }
    }
}