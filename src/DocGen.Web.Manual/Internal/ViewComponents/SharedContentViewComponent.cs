using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

#pragma warning disable 1998

namespace DocGen.Web.Manual.Internal.ViewComponents
{
    public class SharedContentViewComponent : ViewComponent
    {
        readonly ICompositeViewEngine _compositeViewEngine;

        public SharedContentViewComponent(ICompositeViewEngine compositeViewEngine)
        {
            _compositeViewEngine = compositeViewEngine;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(string templateName, string language)
        {
            var defaultViewPath = $"~/{templateName}.cshtml";
            var localizedViewPath = $"~/{templateName}.{language}.cshtml";

            var view = _compositeViewEngine.GetView(null, localizedViewPath, false);
            if (view != null && view.Success)
            {
                return View(localizedViewPath);
            }
            
            return View(defaultViewPath);
        }
    }
}