using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Core.Markdown;
using DocGen.Web.Manual.Internal;
using MarkdownTranslator;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DocGen.Web.Manual.Impl
{
    public class ManualTranslations : IManualTranslations
    {
        readonly IMarkdownTransformer _markdownTransformer;
        readonly IYamlParser _yamlParser;
        readonly DocGenOptions _options;

        public ManualTranslations(
            IOptions<DocGenOptions> options,
            IMarkdownTransformer markdownTransformer,
            IYamlParser yamlParser)
        {
            _options = options.Value;
            _markdownTransformer = markdownTransformer;
            _yamlParser = yamlParser;
        }
        
        public async Task RegenerateTemplate()
        {
            var translations = new List<string>();
            
            foreach (var markdownFile in await Task.Run(() => Directory.GetFiles(_options.ContentDirectory, "*.md")))
            {
                var content = await Task.Run(() => File.ReadAllText(markdownFile));
                var yaml = _yamlParser.ParseYaml(content);
                if (yaml.Yaml == null) yaml = new YamlParseResult(JsonConvert.DeserializeObject("{}"), yaml.Markdown);
                
                var type = "Content";
                if (!string.IsNullOrEmpty((string)yaml.Yaml.Type))
                {
                    type = yaml.Yaml.Type;
                }
                
                switch (type)
                {
                    case "Content":
                        var order = (int?)yaml.Yaml.Order;
                        _markdownTransformer.TransformMarkdown(yaml.Markdown,
                            DocgenDefaults.GetDefaultPipeline(),
                            value =>
                            {
                                if(!translations.Contains(value))
                                    translations.Add(value);
                                return value;
                            });
                        break;
                }
            }
            
            // Now that we have the translations of all of our documents, let's generate the POT file.
            var destination = Path.Combine(_options.ContentDirectory, "translations", "template.pot");
            await Task.Run(() =>
            {
                var parentDirectory = Path.GetDirectoryName(destination) ?? "";
                if (!Directory.Exists(parentDirectory))
                    Directory.CreateDirectory(parentDirectory);
                if(File.Exists(destination))
                    File.Delete(destination);
                using (var file = File.OpenWrite(destination))
                using (var writer = new StreamWriter(file))
                    _markdownTransformer.CreatePotFile(translations, writer);
            });
        }

        public List<string> GetLanguages()
        {
            throw new System.NotImplementedException();
        }
    }
}