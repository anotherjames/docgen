using System;
using System.IO;
using System.Linq;
using System.Text;
using Markdig;
using Markdig.Helpers;
using Markdig.Syntax;
using YamlDotNet.Serialization;

namespace DocGen.Requirements.Impl
{
    public class RequirementsParser : IRequirementsParser
    {
        public UserNeed ParseUserNeed(string content)
        {
            
            var builder = new MarkdownPipelineBuilder().Configure("yaml");
            //builder.Extensions.Add(new Markdig.Extensions.Yaml.YamlFrontMatterExtension());
            var pipeline = builder.Build();
            var document = Markdown.Parse(content, pipeline);
            var yaml = document.Descendants<Markdig.Extensions.Yaml.YamlFrontMatterBlock>()
                .FirstOrDefault();
            var c = yaml.Descendants().ToList();
            var r = yaml.ToPositionText();
            var s = ToString(yaml.Lines.ToCharIterator());

            // convert string/file to YAML object
//             var r = new StringReader(@"
// scalar: a scalar
// sequence:
//   - one
//   - two
// ");
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(new StringReader(s));

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);

            Console.WriteLine(json);

            return null;
        }

        private static string ToString(StringLineGroup.Iterator text)
        {
            var chars = new StringBuilder();
            while (text.CurrentChar != '\0')
            {
                chars.Append(text.CurrentChar);
                text.NextChar();
            }
            return chars.ToString();
        }

        public ProductRequirement ParseProductRequirement(string content)
        {
            return null;
        }

        public SoftwareSpecification ParseSoftwareSpecification(string content)
        {
            return null;
        }

        public TestCase ParseTestCase(string content)
        {
            return null;
        }
    }
}