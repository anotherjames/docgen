using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Requirements.Internal.Controllers
{
    public class RequirementsController : Controller
    {
        public ActionResult UserNeed()
        {
            return View();
        }

        public ActionResult ProductRequirement()
        {
            return View();
        }

        public ActionResult SoftwareSpecification()
        {
            return View();
        }
    }
}