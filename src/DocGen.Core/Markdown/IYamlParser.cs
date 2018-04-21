namespace DocGen.Core.Markdown
{
    public interface IYamlParser
    {
        dynamic ParseYaml(string markdown);
    }
}