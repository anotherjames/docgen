using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Manual.Internal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}