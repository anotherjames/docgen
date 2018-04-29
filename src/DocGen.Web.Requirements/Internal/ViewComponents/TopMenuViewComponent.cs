using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocGen.Web.Requirements.Internal.ViewComponents
{
    public class TopMenuViewComponent : ViewComponent
    {
        private readonly IMenuStore _menuStore;

        public TopMenuViewComponent(IMenuStore menuStore)
        {
            _menuStore = menuStore;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.CompletedTask;

            var menu = _menuStore.BuildMenu(HttpContext.Request.Path, 1);
            
//            Console.WriteLine("Top menu...");
//            
//            Console.WriteLine(JsonConvert.SerializeObject(menu, Formatting.Indented));
//            
            return View(menu);
        }
    }
}