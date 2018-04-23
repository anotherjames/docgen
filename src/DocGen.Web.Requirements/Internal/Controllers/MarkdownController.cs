using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Requirements.Internal.Controllers
{
    public class MarkdownController : Controller
    {
        public ActionResult Page(string page)
        {
            return View();
        }
    }
}