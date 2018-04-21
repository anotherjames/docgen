using System;
using System.Collections.Generic;

namespace DocGen.Requirements
{
    public class UserNeed
    {
        public UserNeed()
        {
            ProductRequirements = new List<ProductRequirement>();
            TestCases = new List<TestCase>();
        }
        
        public string Key { get; set; }

        public Version Number { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string ValidationMethod { get; set; }

        public List<ProductRequirement> ProductRequirements { get; set; }

        public List<TestCase> TestCases { get; set; }
    }
}