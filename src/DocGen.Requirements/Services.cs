using Microsoft.Extensions.DependencyInjection;

namespace DocGen.Requirements
{
    public static class Services
    {
        public static void Register(ServiceCollection services)
        {
            services.AddSingleton<IRequirementsParser, Impl.RequirementsParser>();
            services.AddSingleton<IRequirementsBuilder, Impl.RequirementsBuilder>();
        }
    }
}