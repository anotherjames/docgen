using MarkdownTranslator;
using MarkdownTranslator.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Web.Manual
{
    public static class Services
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IManualWebBuilder, Impl.ManualWebBuilder>();
            services.AddSingleton<IManualTranslations, Impl.ManualTranslations>();
            services.AddSingleton<IMarkdownTransformer, MarkdownTransformer>();
        }
    }
}