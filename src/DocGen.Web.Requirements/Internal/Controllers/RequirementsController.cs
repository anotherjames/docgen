using DocGen.Web.Requirements.Internal.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Requirements.Internal.Controllers
{
    public class RequirementsController : Controller
    {
        readonly IRequirementsStore _requirementsStore;

        public RequirementsController(IRequirementsStore requirementsStore)
        {
            _requirementsStore = requirementsStore;
        }
        
        public ActionResult UserNeed(string number)
        {
            var model = new UserNeedModel();
            model.UserNeed = _requirementsStore.GetUserNeed(number);
            return View(model);
        }

        public ActionResult ProductRequirement(string number)
        {
            var model = new ProductRequirementModel();
            model.ProductRequirement = _requirementsStore.GetProductRequirement(number);
            return View(model);
        }

        public ActionResult SoftwareSpecification(string number)
        {
            var model = new SoftwareSpecificationModel();
            model.SoftwareSpecification = _requirementsStore.GetSoftwareSpecification(number);
            return View(model);
        }
        
        public ActionResult TestCase(string number)
        {
            var model = new TestCaseModel();
            model.TestCase = _requirementsStore.GetTest(number);
            return View(model);
        }
    }
}