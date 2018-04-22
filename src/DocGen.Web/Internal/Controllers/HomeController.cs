using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Internal
{
    internal class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}