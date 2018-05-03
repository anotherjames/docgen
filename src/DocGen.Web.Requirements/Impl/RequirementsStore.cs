using System.Collections.Generic;
using DocGen.Requirements;
using System.Linq;

namespace DocGen.Web.Requirements.Impl
{
    public class RequirementsStore : IRequirementsStore
    {
        readonly Dictionary<string, UserNeed> _userNeeds = new Dictionary<string, UserNeed>();
        readonly Dictionary<string, ProductRequirement> _productRequirements = new Dictionary<string, ProductRequirement>();
        readonly Dictionary<string, SoftwareSpecification> _softwareSpecifications = new Dictionary<string, SoftwareSpecification>();
        readonly Dictionary<string, TestCase> _testCases = new Dictionary<string, TestCase>();
        
        public RequirementsStore(IList<UserNeed> requirements)
        {
            foreach (var userNeed in requirements)
            {
                _userNeeds.Add(userNeed.NumberFullyQualified, userNeed);
                foreach (var productRequirement in userNeed.ProductRequirements)
                {
                    _productRequirements.Add(productRequirement.NumberFullyQualified, productRequirement);
                    foreach (var softwareSpecification in productRequirement.SoftwareSpecifications)
                    {
                        _softwareSpecifications.Add(softwareSpecification.NumberFullyQualified, softwareSpecification);
                        foreach (var test in softwareSpecification.TestCases)
                        {
                            _testCases.Add(test.NumberFullyQualified, test);
                        }
                    }
                    foreach (var test in productRequirement.TestCases)
                    {
                        _testCases.Add(test.NumberFullyQualified, test);
                    }
                }
                foreach (var test in userNeed.TestCases)
                {
                    _testCases.Add(test.NumberFullyQualified, test);
                }
            }
        }
        
        public UserNeed GetUserNeed(string number)
        {
            return _userNeeds[number];
        }

        public ProductRequirement GetProductRequirement(string number)
        {
            return _productRequirements[number];
        }

        public SoftwareSpecification GetSoftwareSpecification(string number)
        {
            return _softwareSpecifications[number];
        }

        public TestCase GetTest(string number)
        {
            return _testCases[number];
        }
    }
}