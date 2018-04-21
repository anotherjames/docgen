using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Requirements
{
    public static class Services
    {
        public static void Register(ServiceCollection services)
        {
            services.AddSingleton<IRequirementsParser, DocGen.Requirements.Impl.RequirementsParser>();
            //services.AddSingleton<Markdown.IYamlParser, Markdown.Impl.YamlParser>();
        }
    }
}