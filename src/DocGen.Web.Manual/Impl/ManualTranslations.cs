using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Core.Markdown;
using DocGen.Web.Manual.Internal;
using MarkdownTranslator;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharpGettext;
using static SimpleExec.Command;

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
            
            // Add some translations that we have hardcoded in our templates.
            if(!translations.Contains("Table of contents"))
                translations.Add("Table of contents");
            
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
                    SharpGettext.SharpGettext.GeneratePOT(
                        new POTemplateHeader
                        {
                            Language = "en-US"
                        },
                        translations.Select(x => new POTranslation
                        {
                            Text = x
                        }).ToList(),
                        writer);
            });
            
            // Now that we have our POT files, let's update all of our translations.
            await Task.Run(async () =>
            {
                foreach (var file in Directory.GetFiles(Path.Combine(_options.ContentDirectory, "translations"), "*.po"))
                {
                    await RunAsync("msgmerge", $"-U \"{file}\" \"{destination}\"");
                }
            });
        }

        public async Task AddLanguage(string cultureCode)
        {
            var found = false;
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                Debug.WriteLine(culture.Name);
                found = culture.Name == cultureCode;
                if(found) break;
            }
            
            if (!found)
            {
                throw new DocGenException($"Invalid culture code $\"{cultureCode}\"");
            }
            
            var destination = Path.Combine(_options.ContentDirectory, "translations", $"{cultureCode}.po");

            if(await Task.Run(() => File.Exists(destination)))
            {
                throw new Exception($"Language already created for {cultureCode}");
            }

            var template = Path.Combine(_options.ContentDirectory, "translations", "template.pot");
            
            await RunAsync("msginit", $"-i \"{template}\" -o \"{destination}\" -l \"{cultureCode}\" --no-translator");
        }
        
        public async Task<List<string>> GetLanguages()
        {
            var languages = new List<string>();
            languages.Add("en-US");

            var translationDirectory = Path.Combine(_options.ContentDirectory, "translations");
            await Task.Run(() =>
            {
                foreach (var language in Directory.GetFiles(translationDirectory, "*.mo"))
                {
                    languages.Add(Path.GetFileNameWithoutExtension(language));
                }
            });
            
            return languages;
        }

        public async Task CompileLanguages()
        {
            // Clean all the existing .mo files.
            var translationDirectory = Path.Combine(_options.ContentDirectory, "translations");
            await Task.Run(() =>
            {
                foreach (var language in Directory.GetFiles(translationDirectory, "*.mo"))
                {
                    File.Delete(language);
                }
            });
            
            await Task.Run(() =>
            {
                foreach (var language in Directory.GetFiles(translationDirectory, "*.po"))
                {
                    var destination = Path.Combine(
                        // ReSharper disable AssignNullToNotNullAttribute
                        Path.GetDirectoryName(language),
                        // ReSharper restore AssignNullToNotNullAttribute
                        Path.GetFileNameWithoutExtension(language) + ".mo");
                    RunAsync("msgfmt", $"-o \"{destination}\" \"{language}\"");
                }
            });
        }
    }
}