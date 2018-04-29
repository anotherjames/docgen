using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Core
{
    public static class Services
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<Markdown.IMarkdownRenderer, Markdown.Impl.MarkdownRenderer>();
            services.AddSingleton<Markdown.IYamlParser, Markdown.Impl.YamlParser>();
        }
    }
}