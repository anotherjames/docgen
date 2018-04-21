using System;
using System.IO;
using System.Linq;
using System.Text;
using Markdig;
using Markdig.Syntax;
using YamlDotNet.Serialization;

namespace DocGen.Core.Markdown.Impl
{
    public class YamlParser : IYamlParser
    {
        public dynamic ParseYaml(string markdown)
        {
            var builder = new Markdig.MarkdownPipelineBuilder();
            builder.Extensions.Add(new Markdig.Extensions.Yaml.YamlFrontMatterExtension());
            var pipeline = builder.Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            var yamlBlocks = document.Descendants<Markdig.Extensions.Yaml.YamlFrontMatterBlock>()
                .ToList();
            
            if(yamlBlocks.Count == 0)
            {
                return null;
            }

            if(yamlBlocks.Count > 1)
            {
                throw new InvalidOperationException();
            }

            var yamlBlock = yamlBlocks.First();

            var yamlBlockIterator = yamlBlock.Lines.ToCharIterator();
            var yaml = new StringBuilder();
            while (yamlBlockIterator.CurrentChar != '\0')
            {
                yaml.Append(yamlBlockIterator.CurrentChar);
                yamlBlockIterator.NextChar();
            }

            var yamlDeserializer = new DeserializerBuilder().Build();
            var yamlObject = yamlDeserializer.Deserialize(new StringReader(yaml.ToString()));

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);

            return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
        }
    }
}