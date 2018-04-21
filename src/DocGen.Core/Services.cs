using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Core
{
    public static class Services
    {
        public static void Register(ServiceCollection services)
        {
            services.AddSingleton<Markdown.IMarkdownParser, Markdown.Impl.MarkdownParser>();
            services.AddSingleton<Markdown.IYamlParser, Markdown.Impl.YamlParser>();
        }
    }
}