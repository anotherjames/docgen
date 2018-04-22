using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocGen.Core;

namespace DocGen.Requirements.Impl
{
    public class RequirementsBuilder : IRequirementsBuilder
    {
        IRequirementsParser _requirementsParser;

        public RequirementsBuilder(IRequirementsParser requirementsParser)
        {
            _requirementsParser = requirementsParser;
        }

        public Task<List<UserNeed>> BuildRequirementsFromRepositoryCommit(string repository, string sha)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<UserNeed>> BuildRequirementsFromDirectory(string directory)
        {
            var result = new List<UserNeed>();

            foreach (var dir in await Task.Run(() => Directory.GetDirectories(directory).OrderBy(x => x)))
                result.Add(await BuildRequirementsFromDirectoryUserNeed(dir));

            var duplicateNumbers = result.GroupBy(x => x.Number)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateNumbers.Any())
            {
                var first = duplicateNumbers.First();
                var duplicateUserNeeds = result.Where(x => x.Number == first).ToList();
                throw new DocGenException($"Duplicate number '{first}' for user needs '{string.Join(",", duplicateUserNeeds.Select(x => x.Key).ToArray())}'");
            }

            result = result.OrderBy(x => x.Number).ToList();

            return result;
        }

        private async Task<UserNeed> BuildRequirementsFromDirectoryUserNeed(string directory)
        {
            var index = Path.Combine(directory, "index.md");
            var directoryName = Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar));

            if (!File.Exists(index))
                throw new DocGenException($"File 'index.md' doesn't exist for user need {directoryName}");

            UserNeed userNeed;

            try
            {
                userNeed = _requirementsParser.ParseUserNeed(File.ReadAllText(index));
            }
            catch (Exception ex)
            {
                throw new DocGenException($"Error parsing index.md for user need {directoryName}: {ex.Message}");
            }

            userNeed.Key = directoryName;

            foreach (var dir in Directory.GetDirectories(directory).OrderBy(x => x))
            {
                var childDirName = Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar));

                if (childDirName.Equals("tests", StringComparison.InvariantCultureIgnoreCase))
                {
                    // process tests
                    foreach (var file in Directory.GetFiles(dir, "*.md").OrderBy(x => x))
                    {
                        var testCase = _requirementsParser.ParseTestCase(File.ReadAllText(file));
                        testCase.UserNeed = userNeed;
                        testCase.Key = Path.GetFileNameWithoutExtension(file);
                        userNeed.TestCases.Add(testCase);
                    }
                }
                else
                {
                    // process product requirements
                    userNeed.ProductRequirements.Add(await BuildRequirementsFromDirectoryProductRequirement(dir, userNeed));
                }
            }

            var duplicateNumbers = userNeed.ProductRequirements.GroupBy(x => x.Number)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateNumbers.Any())
            {
                var first = duplicateNumbers.First();
                var duplicateUserNeeds = userNeed.ProductRequirements.Where(x => x.Number == first).ToList();
                throw new DocGenException($"Duplicate number '{first}' for product requirements '{string.Join(",", duplicateUserNeeds.Select(x => x.Key).ToArray())}'");
            }

            AssertUniqueNumbersForTestCases(userNeed.TestCases);

            userNeed.ProductRequirements = userNeed.ProductRequirements.OrderBy(x => x.Number).ToList();
            userNeed.TestCases = userNeed.TestCases.OrderBy(x => x.Number).ToList();

            return userNeed;
        }

        private async Task<ProductRequirement> BuildRequirementsFromDirectoryProductRequirement(string directory, UserNeed userNeed)
        {
            var index = Path.Combine(directory, "index.md");
            var directoryName = Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar));

            if (!File.Exists(index))
                throw new DocGenException($"File 'index.md' doesn't exist for product requirement {directoryName}");

            ProductRequirement productRequirement;

            try
            {
                productRequirement = _requirementsParser.ParseProductRequirement(File.ReadAllText(index));
            }
            catch (Exception ex)
            {
                throw new DocGenException($"Error parsing index.md for product requirement {directoryName}: {ex.Message}");
            }

            productRequirement.UserNeed = userNeed;
            productRequirement.Key = directoryName;

            foreach (var dir in Directory.GetDirectories(directory).OrderBy(x => x))
            {
                var childDirName = Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar));

                if (childDirName.Equals("tests", StringComparison.InvariantCultureIgnoreCase))
                {
                    // process tests
                    foreach(var file in Directory.GetFiles(dir, "*.md").OrderBy(x => x))
                    {
                        var testCase = _requirementsParser.ParseTestCase(File.ReadAllText(file));
                        testCase.ProductRequirement = productRequirement;
                        testCase.Key = Path.GetFileNameWithoutExtension(file);
                        productRequirement.TestCases.Add(testCase);
                    }
                }
                else
                {
                    // process software specification
                    productRequirement.SoftwareSpecifications.Add(await BuildRequirementsFromDirectorySoftwareSpecification(dir, productRequirement));
                }
            }

            var duplicateNumbers = productRequirement.SoftwareSpecifications.GroupBy(x => x.Number)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateNumbers.Any())
            {
                var first = duplicateNumbers.First();
                var duplicateUserNeeds = productRequirement.SoftwareSpecifications.Where(x => x.Number == first).ToList();
                throw new DocGenException($"Duplicate number '{first}' for software specifications '{string.Join(",", duplicateUserNeeds.Select(x => x.Key).ToArray())}'");
            }
            
            AssertUniqueNumbersForTestCases(productRequirement.TestCases);

            productRequirement.SoftwareSpecifications = productRequirement.SoftwareSpecifications.OrderBy(x => x.Number).ToList();
            productRequirement.TestCases = productRequirement.TestCases.OrderBy(x => x.Number).ToList();

            return productRequirement;
        }

        private Task<SoftwareSpecification> BuildRequirementsFromDirectorySoftwareSpecification(string directory, ProductRequirement productRequirement)
        {
            var index = Path.Combine(directory, "index.md");
            var directoryName = Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar));

            if (!File.Exists(index))
                throw new DocGenException($"File 'index.md' doesn't exist for software specification {directoryName}");

            SoftwareSpecification softwareSpecification;

            try
            {
                softwareSpecification = _requirementsParser.ParseSoftwareSpecification(File.ReadAllText(index));
            }
            catch (Exception ex)
            {
                throw new DocGenException($"Error parsing index.md for software specification {directoryName}: {ex.Message}");
            }

            softwareSpecification.ProductRequirement = productRequirement;
            softwareSpecification.Key = directoryName;

            foreach (var dir in Directory.GetDirectories(directory).OrderBy(x => x))
            {
                var childDirName = Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar));

                if (childDirName.Equals("tests", StringComparison.InvariantCultureIgnoreCase))
                {
                    // process tests
                    foreach (var file in Directory.GetFiles(dir, "*.md").OrderBy(x => x))
                    {
                        var testCase = _requirementsParser.ParseTestCase(File.ReadAllText(file));
                        testCase.SoftwareSpecification = softwareSpecification;
                        testCase.Key = Path.GetFileNameWithoutExtension(file);
                        softwareSpecification.TestCases.Add(testCase);
                    }
                }
                else
                {
                    throw new DocGenException($"Invalid directory {childDirName} in software specification {directoryName}");
                }
            }

            AssertUniqueNumbersForTestCases(softwareSpecification.TestCases);

            softwareSpecification.TestCases = softwareSpecification.TestCases.OrderBy(x => x.Number).ToList();

            return Task.FromResult(softwareSpecification);
        }

        private void AssertUniqueNumbersForTestCases(List<TestCase> testCases)
        {
            var duplicateNumbers = testCases.GroupBy(x => x.Number)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateNumbers.Any())
            {
                var first = duplicateNumbers.First();
                var duplicateTestCases = testCases.Where(x => x.Number == first).ToList();
                throw new DocGenException($"Duplicate number '{first}' for test cases '{string.Join(",", duplicateTestCases.Select(x => x.Key).ToArray())}'");
            }
        }
    }
}