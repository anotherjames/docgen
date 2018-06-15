using DocGen.Requirements;

namespace DocGen.Web.Requirements
{
    public interface IRequirementsStore
    {
        UserNeed GetUserNeed(string number);

        ProductRequirement GetProductRequirement(string number);

        SoftwareSpecification GetSoftwareSpecification(string number);

        TestCase GetTest(string number);
    }
}