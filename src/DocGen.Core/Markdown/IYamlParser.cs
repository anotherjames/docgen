namespace DocGen.Core.Markdown
{
    public interface IYamlParser
    {
        YamlParseResult ParseYaml(string markdown);
    }
}