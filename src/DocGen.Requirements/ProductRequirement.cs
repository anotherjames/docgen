using System;
using System.Collections.Generic;

namespace DocGen.Requirements
{
    public class ProductRequirement
    {
        public ProductRequirement()
        {
            SoftwareSpecifications = new List<SoftwareSpecification>();
            TestCases = new List<TestCase>();
        }

        public string Key { get; set; }

        public Version Number { get; set; }

        public string NumberFullyQualified
        {
            get
            {
                if (UserNeed == null) return Number.ToString();
                return $"{UserNeed.Number}-{Number}";
            }
        }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Requirement { get; set; }

        public string VerificationMethod { get; set; }

        public UserNeed UserNeed { get; set; }

        public List<SoftwareSpecification> SoftwareSpecifications { get; set; }
        
        public List<TestCase> TestCases { get; set; }
    }
}