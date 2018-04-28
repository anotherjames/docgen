using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Requirements.Internal.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuStore _menuStore;

        public MenuViewComponent(IMenuStore menuStore)
        {
            _menuStore = menuStore;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.CompletedTask;
            return View(_menuStore.BuildMenu(HttpContext.Request.Path));
        }
    }
}