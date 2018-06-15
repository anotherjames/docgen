using System;
using System.Dynamic;
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
                yaml = new YamlParseResult(Newtonsoft.Json.JsonConvert.DeserializeObject("{}"), yaml.Markdown);

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
            if (!TryParseVersion(number, out Version version))
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

        public ProductRequirement ParseProductRequirement(string content)
        {
            var yaml = _yamlParser.ParseYaml(content);
            
            if(yaml.Yaml == null)
                yaml = new YamlParseResult(Newtonsoft.Json.JsonConvert.DeserializeObject("{}"), yaml.Markdown);

            StringBuilder requirement = new StringBuilder();
            StringBuilder verification = new StringBuilder();
            StringBuilder current = null;
            using(var stringReader = new StringReader(yaml.Markdown))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (line == "# Requirement")
                    {
                        current = requirement;
                    }
                    else if (line == "# Verification Method")
                    {
                        current = verification;
                    }
                    else
                    {
                        if (current == null)
                            throw new DocGenException($"Content '{line}' should be within a requirement or verification method");

                        current.AppendLine(line);
                    }
                }
            }

            var result = new ProductRequirement();
            string number = yaml.Yaml?.Number;
            if (string.IsNullOrEmpty(number))
                throw new DocGenException("You must provider a number");
            if (!TryParseVersion(number, out Version version))
                throw new DocGenException($"Invalid number format {number}");

            result.Number = version;
            result.Title = yaml.Yaml?.Title;
            result.Category = yaml.Yaml?.Category;
            result.Requirement = requirement.ToString();
            result.VerificationMethod = verification.ToString();

            result.Requirement = result.Requirement.TrimEnd(Environment.NewLine.ToCharArray());
            result.VerificationMethod = result.VerificationMethod.TrimEnd(Environment.NewLine.ToCharArray());

            if (string.IsNullOrEmpty(result.Title))
                throw new DocGenException("Title is required");
            if (string.IsNullOrEmpty(result.Category))
                throw new DocGenException("Category is required");
            if (string.IsNullOrEmpty(result.Requirement))
                throw new DocGenException("You must provide requirements");
            if (string.IsNullOrEmpty(result.VerificationMethod))
                throw new DocGenException("You must provide a verification method");

            return result;
        }

        public SoftwareSpecification ParseSoftwareSpecification(string content)
        {
            var yaml = _yamlParser.ParseYaml(content);
            
            if(yaml.Yaml == null)
                yaml = new YamlParseResult(Newtonsoft.Json.JsonConvert.DeserializeObject("{}"), yaml.Markdown);

            StringBuilder requirement = new StringBuilder();
            StringBuilder verification = new StringBuilder();
            StringBuilder current = null;
            using(var stringReader = new StringReader(yaml.Markdown))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (line == "# Requirement")
                    {
                        current = requirement;
                    }
                    else if (line == "# Verification Method")
                    {
                        current = verification;
                    }
                    else
                    {
                        if (current == null)
                            throw new DocGenException($"Content '{line}' should be within a requirement or verification method");

                        current.AppendLine(line);
                    }
                }
            }

            var result = new SoftwareSpecification();
            string number = yaml.Yaml?.Number;
            if (string.IsNullOrEmpty(number))
                throw new DocGenException("You must provider a number");
            if (!TryParseVersion(number, out Version version))
                throw new DocGenException($"Invalid number format {number}");

            result.Number = version;
            result.Title = yaml.Yaml?.Title;
            result.Requirement = requirement.ToString();
            result.VerificationMethod = verification.ToString();

            result.Requirement = result.Requirement.TrimEnd(Environment.NewLine.ToCharArray());
            result.VerificationMethod = result.VerificationMethod.TrimEnd(Environment.NewLine.ToCharArray());

            if (string.IsNullOrEmpty(result.Title))
                throw new DocGenException("Title is required");
            if (string.IsNullOrEmpty(result.Requirement))
                throw new DocGenException("Requirement is required");
            if (string.IsNullOrEmpty(result.VerificationMethod))
                throw new DocGenException("You must provide a verification method");

            return result;
        }

        public TestCase ParseTestCase(string content)
        {
            var yaml = _yamlParser.ParseYaml(content);
            
            if(yaml.Yaml == null)
                yaml = new YamlParseResult(Newtonsoft.Json.JsonConvert.DeserializeObject("{}"), yaml.Markdown);

            StringBuilder action = new StringBuilder();
            StringBuilder expected = new StringBuilder();
            StringBuilder current = null;
            using(var stringReader = new StringReader(yaml.Markdown))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (line == "# Action")
                    {
                        current = action;
                    }
                    else if (line == "# Expected")
                    {
                        current = expected;
                    }
                    else
                    {
                        if (current == null)
                            throw new DocGenException($"Content '{line}' should be within a action or expected");

                        current.AppendLine(line);
                    }
                }
            }

            var result = new TestCase();
            string number = yaml.Yaml?.Number;
            if (string.IsNullOrEmpty(number))
                throw new DocGenException("You must provider a number");
            if (!TryParseVersion(number, out Version version))
                throw new DocGenException($"Invalid number format {number}");

            result.Number = version;
            string responseType = yaml.Yaml?.ResponseType;
            if(string.IsNullOrEmpty(responseType))
                responseType = "PassFail";
            result.ResponseType = (TestCaseResponseTypeEnum)Enum.Parse(typeof(TestCaseResponseTypeEnum), responseType);
            string validationType = yaml.Yaml?.ValidationType;
            if(string.IsNullOrEmpty(validationType))
                validationType = "Verification";
            result.ValidationType = (TestCaseValidationTypeEnum)Enum.Parse(typeof(TestCaseValidationTypeEnum), validationType);
            string testType = yaml.Yaml?.Type;
            if(string.IsNullOrEmpty(testType))
                testType = "Software";
            result.Type = (TestCaseTypeEnum)Enum.Parse(typeof(TestCaseTypeEnum), testType);
            result.Action = action.ToString();
            result.Expected = expected.ToString();

            result.Action = result.Action.TrimEnd(Environment.NewLine.ToCharArray());
            result.Expected = result.Expected.TrimEnd(Environment.NewLine.ToCharArray());

            if (string.IsNullOrEmpty(result.Action))
                throw new DocGenException("Action is required");
            if (string.IsNullOrEmpty(result.Expected))
                throw new DocGenException("Expected is required");

            return result;
        }

        private bool TryParseVersion(string input, out Version version)
        {
            version = null;

            if (string.IsNullOrEmpty(input)) return false;
            
            if (int.TryParse(input, out int tmp))
            {
                // This is just a number, but let's treat it as if it is valid.
                version = new Version(tmp, 0);
                return true;
            }

            return Version.TryParse(input, out version);
        }
    }
}