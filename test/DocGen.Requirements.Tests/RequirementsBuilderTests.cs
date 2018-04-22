using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocGen.Tests;
using Xunit;

namespace DocGen.Requirements.Tests
{
    public class RequirementsBuilderTests : IDisposable
    {
        string _tempDirectory;
        IRequirementsBuilder _requirementsBuilder;

        public RequirementsBuilderTests()
        {
            _tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            while (Directory.Exists(_tempDirectory))
                _tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            _requirementsBuilder = new DocGen.Requirements.Impl.RequirementsBuilder(new DocGen.Requirements.Impl.RequirementsParser(new DocGen.Core.Markdown.Impl.YamlParser()));
        }

        [Fact]
        public async Task Throw_exception_when_missing_index_for_user_need()
        {
            CreateDirectory("UN1");
            await AssertExtensions.ExceptionAsync(async () =>
            {
                await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
            },
            ex =>
            {
                Assert.Equal(ex.Message, "File 'index.md' doesn't exist for user need UN1");
            });
        }

        [Fact]
        public async Task Throw_exception_when_missing_index_for_product_requirement()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/PR1");
            await AssertExtensions.ExceptionAsync(async () =>
            {
                await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
            },
            ex =>
            {
                Assert.Equal(ex.Message, "File 'index.md' doesn't exist for product requirement PR1");
            });
        }

        [Fact]
        public async Task Throw_exception_when_missing_index_for_software_requirement()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.0", "title", "category", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS1");
            await AssertExtensions.ExceptionAsync(async () =>
            {
                await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
            },
            ex =>
            {
                Assert.Equal(ex.Message, "File 'index.md' doesn't exist for software specification SS1");
            });
        }

        [Fact]
        public async Task No_exception_when_index_files_all_present()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.0", "title", "category", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS1");
            WriteSoftwareRequirement("UN1/PR1/SS1/index.md", "1.0", "title", "requirement", "verification");

            var result = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].Number, new Version(1,0));
            Assert.Equal(result[0].Title, "title");
            Assert.Equal(result[0].Category, "category");
            Assert.Equal(result[0].Description, "userneed");
            Assert.Equal(result[0].ValidationMethod, "validationmethod");
            Assert.Equal(result[0].ProductRequirements.Count, 1);
            Assert.Equal(result[0].ProductRequirements[0].Number, new Version(1,0));
            Assert.Equal(result[0].ProductRequirements[0].Title, "title");
            Assert.Equal(result[0].ProductRequirements[0].Category, "category");
            Assert.Equal(result[0].ProductRequirements[0].Requirement, "requirement");
            Assert.Equal(result[0].ProductRequirements[0].VerificationMethod, "verificationMethod");
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications.Count, 1);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].Number, new Version(1,0));
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].Title, "title");
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].Requirement, "requirement");
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].VerificationMethod, "verification");
        }

        [Fact]
        public async Task Can_parse_tests_on_user_need()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/tests");
            WriteTestCase("UN1/tests/test1.md");

            var result = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].TestCases.Count, 1);
        }

        [Fact]
        public async Task Can_parse_tests_on_product_requirement()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.0", "title", "category", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/tests");
            WriteTestCase("UN1/PR1/tests/test1.md", "1.0", TestCaseResponseTypeEnum.Done, TestCaseValidationTypeEnum.Verification, TestCaseTypeEnum.Hardware, "action1", "expected1");
            WriteTestCase("UN1/PR1/tests/test2.md", "1.1", TestCaseResponseTypeEnum.PassFail, TestCaseValidationTypeEnum.Validation, TestCaseTypeEnum.Software, "action2", "expected2");

            var result = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].ProductRequirements.Count, 1);
            Assert.Equal(result[0].ProductRequirements[0].TestCases.Count, 2);
            Assert.Equal(result[0].ProductRequirements[0].TestCases[0].ResponseType, TestCaseResponseTypeEnum.Done);
            Assert.Equal(result[0].ProductRequirements[0].TestCases[0].ValidationType, TestCaseValidationTypeEnum.Verification);
            Assert.Equal(result[0].ProductRequirements[0].TestCases[0].Type, TestCaseTypeEnum.Hardware);
            Assert.Equal(result[0].ProductRequirements[0].TestCases[0].Action, "action1");
            Assert.Equal(result[0].ProductRequirements[0].TestCases[0].Expected, "expected1");
            Assert.Equal(result[0].ProductRequirements[0].TestCases[1].ResponseType, TestCaseResponseTypeEnum.PassFail);
            Assert.Equal(result[0].ProductRequirements[0].TestCases[1].ValidationType, TestCaseValidationTypeEnum.Validation);
            Assert.Equal(result[0].ProductRequirements[0].TestCases[1].Type, TestCaseTypeEnum.Software);
            Assert.Equal(result[0].ProductRequirements[0].TestCases[1].Action, "action2");
            Assert.Equal(result[0].ProductRequirements[0].TestCases[1].Expected, "expected2");
        }

        [Fact]
        public async Task Can_parse_tests_on_software_specifications()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.0", "title", "category", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS1");
            WriteSoftwareRequirement("UN1/PR1/SS1/index.md", "1.0", "title", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS1/tests");
            WriteTestCase("UN1/PR1/SS1/tests/test1.md", "1.0", TestCaseResponseTypeEnum.Done, TestCaseValidationTypeEnum.Verification, TestCaseTypeEnum.Hardware, "action1", "expected1");
            WriteTestCase("UN1/PR1/SS1/tests/test2.md", "1.1", TestCaseResponseTypeEnum.PassFail, TestCaseValidationTypeEnum.Validation, TestCaseTypeEnum.Software, "action2", "expected2");

            var result = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].ProductRequirements.Count, 1);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications.Count, 1);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases.Count, 2);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[0].ResponseType, TestCaseResponseTypeEnum.Done);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[0].ValidationType, TestCaseValidationTypeEnum.Verification);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[0].Type, TestCaseTypeEnum.Hardware);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[0].Action, "action1");
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[0].Expected, "expected1");
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[1].ResponseType, TestCaseResponseTypeEnum.PassFail);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[1].ValidationType, TestCaseValidationTypeEnum.Validation);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[1].Type, TestCaseTypeEnum.Software);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[1].Action, "action2");
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[1].Expected, "expected2");
        }

        [Fact]
        public async Task User_need_numbers_should_be_unique()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN2");
            WriteUserNeed("UN2/index.md", "1.0", "title", "category", "userneed", "validationmethod");

            await AssertExtensions.ExceptionAsync(async () =>
            {
                await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
            },
            ex =>
            {
                Assert.Equal(ex.Message, "Duplicate number '1.0' for user needs 'UN1,UN2'");
            });

            DeleteFile("UN2/index.md");
            WriteUserNeed("UN2/index.md", "1.1", "title", "category", "userneed", "validationmethod");

            // shouldn't throw exception anymore
            await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
        }

        [Fact]
        public async Task Product_requirement_numbers_should_be_unique()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.0", "title", "category", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR2");
            WriteProductRequirement("UN1/PR2/index.md", "1.0", "title", "category", "requirement", "verificationMethod");

            await AssertExtensions.ExceptionAsync(async () =>
            {
                await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
            },
            ex =>
            {
                Assert.Equal(ex.Message, "Duplicate number '1.0' for product requirements 'PR1,PR2'");
            });

            DeleteFile("UN1/PR2/index.md");
            WriteProductRequirement("UN1/PR2/index.md", "1.1", "title", "category", "requirement", "verificationMethod");

            // shouldn't throw exception anymore
            await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
        }

        [Fact]
        public async Task Software_specification_numbers_should_be_unique()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.0", "title", "category", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS1");
            WriteSoftwareRequirement("UN1/PR1/SS1/index.md", "1.0", "title", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS2");
            WriteSoftwareRequirement("UN1/PR1/SS2/index.md", "1.0", "title", "requirement", "verificationMethod");

            await AssertExtensions.ExceptionAsync(async () =>
            {
                await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
            },
            ex =>
            {
                Assert.Equal(ex.Message, "Duplicate number '1.0' for software specifications 'SS1,SS2'");
            });

            DeleteFile("UN1/PR1/SS2/index.md");
            WriteSoftwareRequirement("UN1/PR1/SS2/index.md", "1.1", "title", "requirement", "verificationMethod");

            // shouldn't throw exception anymore
            await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
        }

        [Fact]
        public async Task Test_case_numbers_should_be_unique()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.0", "title", "category", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS1");
            WriteSoftwareRequirement("UN1/PR1/SS1/index.md", "1.0", "title", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS1/tests");
            WriteTestCase("UN1/PR1/SS1/tests/test1.md", "1.0", TestCaseResponseTypeEnum.PassFail, TestCaseValidationTypeEnum.Validation, TestCaseTypeEnum.Hardware, "action", "expected");
            WriteTestCase("UN1/PR1/SS1/tests/test2.md", "1.0", TestCaseResponseTypeEnum.PassFail, TestCaseValidationTypeEnum.Validation, TestCaseTypeEnum.Hardware, "action", "expected");

            await AssertExtensions.ExceptionAsync(async () =>
            {
                await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
            },
            ex =>
            {
                Assert.Equal(ex.Message, "Duplicate number '1.0' for test cases 'test1,test2'");
            });

            DeleteFile("UN1/PR1/SS1/tests/test2.md");
            WriteTestCase("UN1/PR1/SS1/tests/test2.md", "1.1", TestCaseResponseTypeEnum.PassFail, TestCaseValidationTypeEnum.Validation, TestCaseTypeEnum.Hardware, "action", "expected");

            await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
        }

        [Fact]
        public async Task Requirements_have_reference_to_parents()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            CreateDirectory("UN1/tests");
            WriteTestCase("UN1/tests/test1.md");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.0", "title", "category", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/tests");
            WriteTestCase("UN1/PR1/tests/test1.md", "1.0", TestCaseResponseTypeEnum.Done, TestCaseValidationTypeEnum.Verification, TestCaseTypeEnum.Hardware, "action1", "expected1");
            CreateDirectory("UN1/PR1/SS1");
            WriteSoftwareRequirement("UN1/PR1/SS1/index.md", "1.0", "title", "requirement", "verificationMethod");
            CreateDirectory("UN1/PR1/SS1/tests");
            WriteTestCase("UN1/PR1/SS1/tests/test1.md", "1.0", TestCaseResponseTypeEnum.Done, TestCaseValidationTypeEnum.Verification, TestCaseTypeEnum.Hardware, "action1", "expected1");

            var result = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].TestCases.Count, 1);
            Assert.Equal(result[0].ProductRequirements.Count, 1);
            Assert.Equal(result[0].ProductRequirements[0].TestCases.Count, 1);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications.Count, 1);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases.Count, 1);

            Assert.Equal(result[0].TestCases[0].UserNeed, result[0]);
            Assert.Null(result[0].TestCases[0].ProductRequirement);
            Assert.Null(result[0].TestCases[0].SoftwareSpecification);
            Assert.Equal(result[0].ProductRequirements[0].UserNeed, result[0]);
            Assert.Equal(result[0].ProductRequirements[0].TestCases[0].ProductRequirement, result[0].ProductRequirements[0]);
            Assert.Null(result[0].ProductRequirements[0].TestCases[0].SoftwareSpecification);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].ProductRequirement, result[0].ProductRequirements[0]);
            Assert.Equal(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[0].SoftwareSpecification, result[0].ProductRequirements[0].SoftwareSpecifications[0]);
            Assert.Null(result[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[0].ProductRequirement);
        }

        [Fact]
        public async Task User_needs_are_ordered_by_number()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.1");
            CreateDirectory("UN2");
            WriteUserNeed("UN2/index.md", "0.9");
            CreateDirectory("UN3");
            WriteUserNeed("UN3/index.md", "1.2");

            var userNeeds = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(userNeeds.Count, 3);
            Assert.Equal(userNeeds[0].Number, new Version(0, 9));
            Assert.Equal(userNeeds[1].Number, new Version(1, 1));
            Assert.Equal(userNeeds[2].Number, new Version(1, 2));
        }
        
        [Fact]
        public async Task Product_requirements_are_ordered_by_number()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.1");
            CreateDirectory("UN1/PR2");
            WriteProductRequirement("UN1/PR2/index.md", "0.9");
            CreateDirectory("UN1/PR3");
            WriteProductRequirement("UN1/PR3/index.md", "1.2");

            var userNeeds = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(userNeeds.Count, 1);
            Assert.Equal(userNeeds[0].ProductRequirements.Count, 3);
            Assert.Equal(userNeeds[0].ProductRequirements[0].Number, new Version(0, 9));
            Assert.Equal(userNeeds[0].ProductRequirements[1].Number, new Version(1, 1));
            Assert.Equal(userNeeds[0].ProductRequirements[2].Number, new Version(1, 2));
        }

        [Fact]
        public async Task Software_specifications_are_ordered_by_number()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.1");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md");
            CreateDirectory("UN1/PR1/SS1");
            WriteSoftwareRequirement("UN1/PR1/SS1/index.md", "1.1");
            CreateDirectory("UN1/PR1/SS2");
            WriteSoftwareRequirement("UN1/PR1/SS2/index.md", "0.9");
            CreateDirectory("UN1/PR1/SS3");
            WriteSoftwareRequirement("UN1/PR1/SS3/index.md", "1.2");

            var userNeeds = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(userNeeds.Count, 1);
            Assert.Equal(userNeeds[0].ProductRequirements.Count, 1);
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications.Count, 3);
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications[0].Number, new Version(0, 9));
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications[1].Number, new Version(1, 1));
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications[2].Number, new Version(1, 2));
        }

        [Fact]
        public async Task Test_cases_are_ordered_by_number()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md");
            CreateDirectory("UN1/tests");
            WriteTestCase("UN1/tests/test1.md", "1.1");
            WriteTestCase("UN1/tests/test2.md", "0.9");
            WriteTestCase("UN1/tests/test3.md", "1.2");
            CreateDirectory("UN1/PR1");
            WriteProductRequirement("UN1/PR1/index.md", "1.1");
            CreateDirectory("UN1/PR1/tests");
            WriteTestCase("UN1/PR1/tests/test1.md", "1.1");
            WriteTestCase("UN1/PR1/tests/test2.md", "0.9");
            WriteTestCase("UN1/PR1/tests/test3.md", "1.2");
            CreateDirectory("UN1/PR1/SS1");
            WriteSoftwareRequirement("UN1/PR1/SS1/index.md");
            CreateDirectory("UN1/PR1/SS1/tests");
            WriteTestCase("UN1/PR1/SS1/tests/test1.md", "1.1");
            WriteTestCase("UN1/PR1/SS1/tests/test2.md", "0.9");
            WriteTestCase("UN1/PR1/SS1/tests/test3.md", "1.2");

            var userNeeds = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);

            Assert.Equal(userNeeds.Count, 1);
            Assert.Equal(userNeeds[0].TestCases.Count, 3);
            Assert.Equal(userNeeds[0].TestCases[0].Number, new Version(0, 9));
            Assert.Equal(userNeeds[0].TestCases[1].Number, new Version(1, 1));
            Assert.Equal(userNeeds[0].TestCases[2].Number, new Version(1, 2));
            Assert.Equal(userNeeds[0].ProductRequirements.Count, 1);
            Assert.Equal(userNeeds[0].ProductRequirements[0].TestCases.Count, 3);
            Assert.Equal(userNeeds[0].ProductRequirements[0].TestCases[0].Number, new Version(0, 9));
            Assert.Equal(userNeeds[0].ProductRequirements[0].TestCases[1].Number, new Version(1, 1));
            Assert.Equal(userNeeds[0].ProductRequirements[0].TestCases[2].Number, new Version(1, 2));
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications.Count, 1);
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases.Count, 3);
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[0].Number, new Version(0, 9));
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[1].Number, new Version(1, 1));
            Assert.Equal(userNeeds[0].ProductRequirements[0].SoftwareSpecifications[0].TestCases[2].Number, new Version(1, 2));
        }

        [Fact]
        public async Task Approvers_files_are_ignored()
        {
            CreateDirectory("UN1");
            WriteUserNeed("UN1/index.md", "1.0", "title", "category", "userneed", "validationmethod");
            WriteFile(".approvers", "");
            CreateDirectory("UN1/tests");
            WriteFile("UN1/tests/.approvers", "");
            WriteTestCase("UN1/tests/test1.md");
            CreateDirectory("UN1/PR1");
            WriteFile("UN1/PR1/.approvers", "");
            WriteProductRequirement("UN1/PR1/index.md");
            CreateDirectory("UN1/PR1/tests");
            WriteFile("UN1/PR1/tests/.approvers", "");
            WriteTestCase("UN1/PR1/tests/test1.md");
            CreateDirectory("UN1/PR1/SS1");
            WriteFile("UN1/PR1/SS1/.approvers", "");
            WriteSoftwareRequirement("UN1/PR1/SS1/index.md");
            CreateDirectory("UN1/PR1/SS1/tests");
            WriteFile("UN1/PR1/SS1/tests/.approvers", "");
            WriteTestCase("UN1/PR1/SS1/tests/test1.md");

            // no exception thrown
            var result = await _requirementsBuilder.BuildRequirementsFromDirectory(_tempDirectory);
        }

        private void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(Path.Combine(_tempDirectory, directory));
        }

        private void DeleteDirectory(string directory)
        {
            Directory.Delete(Path.Combine(_tempDirectory, directory), true);
        }

        private void DeleteFile(string file)
        {
            File.Delete(Path.Combine(_tempDirectory, file));
        }

        private void WriteFile(string path, string content)
        {
            File.WriteAllText(Path.Combine(_tempDirectory, path), content);
        }

        private void WriteUserNeed(string path, string number = "1.0", string title = "title", string category = "category", string userNeed = "userNeed", string validationMethod = "validationMethod")
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine($"Number: {number}");
            document.AppendLine($"Title: {title}");
            document.AppendLine($"Category: {category}");
            document.AppendLine("---");
            document.AppendLine("# User Need");
            document.AppendLine(userNeed);
            document.AppendLine("# Validation Method");
            document.AppendLine(validationMethod);

            File.WriteAllText(Path.Combine(_tempDirectory, path), document.ToString());
        }

        private void WriteProductRequirement(string path, string number = "1.0", string title = "title", string category = "category", string requirement = "requirement", string verificationMethod = "verificationMethod")
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine($"Number: {number}");
            document.AppendLine($"Title: {title}");
            document.AppendLine($"Category: {category}");
            document.AppendLine("---");
            document.AppendLine("# Requirement");
            document.AppendLine(requirement);
            document.AppendLine("# Verification Method");
            document.AppendLine(verificationMethod);

            File.WriteAllText(Path.Combine(_tempDirectory, path), document.ToString());
        }

        private void WriteSoftwareRequirement(string path, string number = "1.0", string title = "title", string requirement = "requirement", string verificationMethod = "verificationMethod")
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine($"Number: {number}");
            document.AppendLine($"Title: {title}");
            document.AppendLine("---");
            document.AppendLine("# Requirement");
            document.AppendLine(requirement);
            document.AppendLine("# Verification Method");
            document.AppendLine(verificationMethod);

            File.WriteAllText(Path.Combine(_tempDirectory, path), document.ToString());
        }

        private void WriteTestCase(string path, string number = "1.0", TestCaseResponseTypeEnum responseType = TestCaseResponseTypeEnum.PassFail, TestCaseValidationTypeEnum validationType = TestCaseValidationTypeEnum.Validation, TestCaseTypeEnum type = TestCaseTypeEnum.Software, string action = "action", string expected = "expected")
        {
            var document = new StringBuilder();
            document.AppendLine("---");
            document.AppendLine($"Number: {number}");
            document.AppendLine($"ResponseType: {responseType}");
            document.AppendLine($"ValidationType: {validationType}");
            document.AppendLine($"Type: {type}");
            document.AppendLine("---");
            document.AppendLine("# Action");
            document.AppendLine(action);
            document.AppendLine("# Expected");
            document.AppendLine(expected);

            File.WriteAllText(Path.Combine(_tempDirectory, path), document.ToString());
        }

        public void Dispose()
        {
            Thread.Sleep(200);
            //Directory.Delete(_tempDirectory);
        }
    }
}