using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Manual.Internal.ViewComponents
{
    public class SharedContentViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string templateName)
        {
            return Task.FromResult<IViewComponentResult>(View(templateName));
        }
    }
}