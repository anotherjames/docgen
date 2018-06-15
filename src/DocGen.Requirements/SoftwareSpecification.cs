using System;
using System.Collections.Generic;

namespace DocGen.Requirements
{
    public class SoftwareSpecification
    {
        public SoftwareSpecification()
        {
            TestCases = new List<TestCase>();
        }
        
        public string Key { get; set; }

        public Version Number { get; set; }

        public string NumberFullyQualified
        {
            get
            {
                if (ProductRequirement == null) return Number.ToString();
                return $"{ProductRequirement.NumberFullyQualified}-{Number}";
            }
        }
        
        public string Title { get; set; }

        public string Requirement { get; set; }

        public string VerificationMethod { get; set; }

        public ProductRequirement ProductRequirement { get; set; }

        public List<TestCase> TestCases { get; set; }
    }
}