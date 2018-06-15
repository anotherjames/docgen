using Markdig;

namespace DocGen.Core
{
    public static class DocgenDefaults
    {
        public static MarkdownPipeline GetDefaultPipeline()
        {
            return new MarkdownPipelineBuilder()
                .UseCustomContainers()
                .UseAdvancedExtensions()
                .Build();
        }
    }
}