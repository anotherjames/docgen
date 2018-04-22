using System;
using System.IO;
using System.Linq;
using System.Text;
using DocGen.Core;
using DocGen.Core.Markdown;
using Markdig;
using Markdig.Helpers;
using Markdig.Syntax;
using YamlDotNet.Serialization;

namespace DocGen.Requirements.Impl
{
    public class RequirementsParser : IRequirementsParser
    {
        IYamlParser _yamlParser;

        public RequirementsParser(IYamlParser yamlParser)
        {
            _yamlParser = yamlParser;
        }

        public UserNeed ParseUserNeed(string content)
        {
            var yaml = _yamlParser.ParseYaml(content);
            
            if(yaml.Yaml == null)
                yaml = new YamlParseResult(new Object(), yaml.Markdown);

            StringBuilder userNeed = new StringBuilder();
            StringBuilder validationMethod = new StringBuilder();
            StringBuilder current = null;
            using(var stringReader = new StringReader(yaml.Markdown))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (line == "# User Need")
                    {
                        current = userNeed;
                    }
                    else if (line == "# Validation Method")
                    {
                        current = validationMethod;
                    }
                    else
                    {
                        if (current == null)
                            throw new DocGenException($"Content '{line}' should be within a user need or validation method");

                        current.AppendLine(line);
                    }
                }
            }

            var result = new UserNeed();
            string number = yaml.Yaml?.Number;
            if (string.IsNullOrEmpty(number))
                throw new DocGenException("You must provider a number");
            if (!Version.TryParse(number, out Version version))
                throw new DocGenException($"Invalid number format {number}");

            result.Number = version;
            result.Title = yaml.Yaml?.Title;
            result.Category = yaml.Yaml?.Category;
            result.Description = userNeed.ToString();
            result.ValidationMethod = validationMethod.ToString();

            result.Description = result.Description.TrimEnd(Environment.NewLine.ToCharArray());
            result.ValidationMethod = result.ValidationMethod.TrimEnd(Environment.NewLine.ToCharArray());

            if (string.IsNullOrEmpty(result.Title))
                throw new DocGenException("Title is required");
            if (string.IsNullOrEmpty(result.Category))
                throw new DocGenException("Category is required");
            if (string.IsNullOrEmpty(result.Description))
                throw new DocGenException("You must provide description");
            if (string.IsNullOrEmpty(result.ValidationMethod))
                throw new DocGenException("You must provide a validation method");

            return result;
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