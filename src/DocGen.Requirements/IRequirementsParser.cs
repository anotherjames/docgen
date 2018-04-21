namespace DocGen.Requirements
{
    public interface IRequirementsParser
    {
        UserNeed ParseUserNeed(string content);

        ProductRequirement ParseProductRequirement(string content);

        SoftwareSpecification ParseSoftwareSpecification(string content);

        TestCase ParseTestCase(string content);
    }
}