using System;
using System.Text;
using System.Threading;
using DocGen.Requirements.Impl;
using Xunit;

namespace DocGen.Requirements.Tests
{
    public class RequirementsParserTests
    {
        IRequirementsParser _requirementsParser;

        public RequirementsParserTests()
        {
            _requirementsParser = new RequirementsParser(new DocGen.Core.Markdown.Impl.YamlParser());
        }

        [Fact]
        public void Can_parse_user_need()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine("User need content....");
            document.AppendLine("# Validation Method");
            document.AppendLine("Validation content...");

            var result = _requirementsParser.ParseUserNeed(document.ToString());

            Assert.Equal(result.Number, new Version(1, 0));
            Assert.Equal(result.Title, "Test title");
            Assert.Equal(result.Category, "Test category");
            Assert.Equal(result.Description, "User need content....");
            Assert.Equal(result.ValidationMethod, "Validation content...");
        }

        [Fact]
        public void Exceptions_thrown_when_missing_user_need_title()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine("User need content....");
            document.AppendLine("# Validation Method");
            document.AppendLine("Validation content...");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "Title is required");
            });
        }

        [Fact]
        public void Exceptions_thrown_when_missing_user_need_category()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine("User need content....");
            document.AppendLine("# Validation Method");
            document.AppendLine("Validation content...");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "Category is required");
            });
        }

        [Fact]
        public void Exceptions_thrown_when_missing_user_need_number()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine("User need content....");
            document.AppendLine("# Validation Method");
            document.AppendLine("Validation content...");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "You must provider a number");
            });
        }

        [Fact]
        public void Exceptions_thrown_when_user_need_number_wrong_format()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.v");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine("User need content....");
            document.AppendLine("# Validation Method");
            document.AppendLine("Validation content...");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "Invalid number format 1.v");
            });
        }

        [Fact]
        public void Exceptions_thrown_when_missing_user_need_description()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine("# Validation Method");
            document.AppendLine("Validation content...");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "You must provide description");
            });

            document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# Validation Method");
            document.AppendLine("Validation content...");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "You must provide description");
            });
        }

        [Fact]
        public void Exceptions_thrown_when_missing_user_need_validation_method()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine("User need content....");
            document.AppendLine("# Validation Method");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "You must provide a validation method");
            });

            document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine("User need content....");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "You must provide a validation method");
            });
        }

        [Fact]
        public void Exceptions_thrown_when_user_need_has_rogue_content()
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine("Number: 1.0");
            document.AppendLine("Title: Test title");
            document.AppendLine("Category: Test category");
            document.AppendLine("---");
            document.AppendLine("# Random");

            AssertExtensions.Exception(() =>
            {
                _requirementsParser.ParseUserNeed(document.ToString());
            },
            ex =>
            {
                Assert.Equal(ex.Message, "Content '# Random' should be within a user need or validation method");
            });
        }

        // [Fact]
        // public void Can_parse_product_requirement()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("Some more....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");
        //     document.AppendLine("Some more....");

        //     var result = _requirementsParser.ParseProductRequirement(document.ToString());

        //     Assert.Equal(result.Number, new Version(1, 0));
        //     Assert.Equal(result.Title, "Test title");
        //     Assert.Equal(result.Category, "Test category");
        //     Assert.Equal(result.Requirement, "Requirement content...." + Environment.NewLine + "Some more....");
        //     Assert.Equal(result.VerificationMethod, "Verification content..." + Environment.NewLine + "Some more....");
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_product_requirement_title()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Title is required");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_product_requirement_category()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Category is required");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_product_requirement_number()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provider a number");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_product_requirement_number_wrong_format()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.V");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Invalid number format 1.V");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_product_requirement_description()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provide requirements");
        //     });

        //     document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provide requirements");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_product_requirement_verification_method()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provide a verification method");
        //     });

        //     document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provide a verification method");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_product_requirement_has_rogue_content()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Random");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseProductRequirement(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Content '# Random' should be within a requirement or verification method");
        //     });
        // }

        // //--

        // [Fact]
        // public void Can_parse_software_requirement()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     var result = _requirementsParser.ParseSoftwareSpecification(document.ToString());

        //     Assert.Equal(result.Number, new Version(1, 0));
        //     Assert.Equal(result.Title, "Test title");
        //     Assert.Equal(result.Requirement, "Requirement content....");
        //     Assert.Equal(result.VerificationMethod, "Verification content...");
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_software_requirement_title()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseSoftwareSpecification(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Title is required");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_software_requirement_number()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseSoftwareSpecification(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provider a number");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_software_requirement_number_wrong_format()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.V");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseSoftwareSpecification(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Invalid number format 1.V");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_software_requirement_description()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseSoftwareSpecification(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Requirement is required");
        //     });

        //     document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("-");
        //     document.AppendLine("# Verification Method");
        //     document.AppendLine("Verification content...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseSoftwareSpecification(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Requirement is required");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_software_requirement_verification_method()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");
        //     document.AppendLine("# Verification Method");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseSoftwareSpecification(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provide a verification method");
        //     });

        //     document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("-");
        //     document.AppendLine("# Requirement");
        //     document.AppendLine("Requirement content....");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseSoftwareSpecification(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provide a verification method");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_software_requirement_has_rogue_content()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Title: Test title");
        //     document.AppendLine("Category: Test category");
        //     document.AppendLine("-");
        //     document.AppendLine("# Random");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseSoftwareSpecification(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Content '# Random' should be within a requirement or verification method");
        //     });
        // }

        // [Fact]
        // public void Can_parse_test_case()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("ResponseType: Done");
        //     document.AppendLine("ValidationType: Validation");
        //     document.AppendLine("Type: Hardware");
        //     document.AppendLine("-");
        //     document.AppendLine("# Action");
        //     document.AppendLine("Action....");
        //     document.AppendLine("# Expected");
        //     document.AppendLine("Expected...");

        //     var result = _requirementsParser.ParseTestCase(document.ToString());

        //     Assert.Equal(result.Number, new Version(1, 0));
        //     Assert.Equal(result.ResponseType, TestCaseResponseTypeEnum.Done);
        //     Assert.Equal(result.ValidationType, TestCaseValidationTypeEnum.Validation);
        //     Assert.Equal(result.Type, TestCaseTypeEnum.Hardware);
        //     Assert.Equal(result.Action, "Action....");
        //     Assert.Equal(result.Expected, "Expected...");
        // }

        // [Fact]
        // public void Can_parse_test_case_with_defaults()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("-");
        //     document.AppendLine("# Action");
        //     document.AppendLine("Action....");
        //     document.AppendLine("# Expected");
        //     document.AppendLine("Expected...");

        //     var result = _requirementsParser.ParseTestCase(document.ToString());

        //     Assert.Equal(result.ResponseType, TestCaseResponseTypeEnum.PassFail);
        //     Assert.Equal(result.ValidationType, TestCaseValidationTypeEnum.Verification);
        //     Assert.Equal(result.Type, TestCaseTypeEnum.Software);
        //     Assert.Equal(result.Action, "Action....");
        //     Assert.Equal(result.Expected, "Expected...");
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_test_case_number()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("# Action");
        //     document.AppendLine("# Expected");
        //     document.AppendLine("Expected...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseTestCase(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "You must provider a number");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_test_case_number_wrong_format()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.V");
        //     document.AppendLine("-");
        //     document.AppendLine("# Action");
        //     document.AppendLine("# Expected");
        //     document.AppendLine("Expected...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseTestCase(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Invalid number format 1.V");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_test_case_action()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("-");
        //     document.AppendLine("# Action");
        //     document.AppendLine("# Expected");
        //     document.AppendLine("Expected...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseTestCase(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Action is required");
        //     });

        //     document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("-");
        //     document.AppendLine("# Expected");
        //     document.AppendLine("Expected...");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseTestCase(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Action is required");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_missing_test_case_expected()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("-");
        //     document.AppendLine("# Action");
        //     document.AppendLine("Action....");
        //     document.AppendLine("# Expected");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseTestCase(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Expected is required");
        //     });

        //     document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("-");
        //     document.AppendLine("# Action");
        //     document.AppendLine("Action....");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseTestCase(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Expected is required");
        //     });
        // }

        // [Fact]
        // public void Exceptions_thrown_when_test_case_has_rogue_content()
        // {
        //     var document = new StringBuilder();
        //     document.AppendLine("Number: 1.0");
        //     document.AppendLine("-");
        //     document.AppendLine("# Random");

        //     AssertExtensions.Exception(() =>
        //     {
        //         _requirementsParser.ParseTestCase(document.ToString());
        //     },
        //     ex =>
        //     {
        //         Assert.Equal(ex.Message, "Content '# Random' should be within a action or expected");
        //     });
        // }
    }
}