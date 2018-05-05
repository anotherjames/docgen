using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DocGen.Web.Manual.Internal.Controllers
{
    public class PrinceController : Controller
    {
        public IActionResult Pdf()
        {
            return new PrinceActionResult();
        }

        class PrinceActionResult : IActionResult
        {
            public Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.ContentType = "application/pdf";
                var prince = new DocGen.Web.Manual.Prince.Prince("/usr/local/bin/prince");
                var url = new UrlHelper(context);
                prince.Convert(url.ServerBaseUrl() + "/", context.HttpContext.Response.Body);
                //prince.SetBaseURL(url.ServerBaseUrl());
                return Task.CompletedTask;
            }
        }
    }
}