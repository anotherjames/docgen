using System;

namespace DocGen.Requirements
{
    public class TestCase
    {
        public string Key { get; set; }

        public Version Number { get; set; }

        public TestCaseResponseTypeEnum ResponseType { get; set; }

        public TestCaseValidationTypeEnum ValidationType { get; set; }

        public TestCaseTypeEnum Type { get; set; }

        public string Action { get; set; }

        public string Expected { get; set; }

        public UserNeed UserNeed { get; set; }

        public ProductRequirement ProductRequirement { get; set; }

        public SoftwareSpecification SoftwareSpecification { get; set; }
    }

    public enum TestCaseResponseTypeEnum
    {
        PassFail,
        Done
    }

    public enum TestCaseValidationTypeEnum
    {
        Verification,
        Validation
    }

    public enum TestCaseTypeEnum
    {
        Software,
        Hardware
    }
}