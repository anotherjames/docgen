using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Manual.Internal
{
    public static class Extensions
    {
        public static string ServerBaseUrl(this IUrlHelper urlHelper)
        {
            return $"{urlHelper.ActionContext.HttpContext.Request.Scheme}://{urlHelper.ActionContext.HttpContext.Request.Host}{urlHelper.ActionContext.HttpContext.Request.PathBase}";
        }
    }
}