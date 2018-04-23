using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Requirements.Internal.Controllers
{
    public class MarkdownController : Controller
    {
        public ActionResult Markdown(string page)
        {
            return Content(page, "application/text");
        }
    }
}