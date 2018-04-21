using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}