using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace DocGen.Web.Requirements.Internal.ViewComponents
{
    public class SideMenuViewComponent : ViewComponent
    {
        private readonly IMenuStore _menuStore;

        public SideMenuViewComponent(IMenuStore menuStore)
        {
            _menuStore = menuStore;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.CompletedTask;
            var menu = _menuStore.BuildMenu(HttpContext.Request.Path, 5);
            
            Console.WriteLine("Side menu...");
            
            Console.WriteLine(JsonConvert.SerializeObject(menu, Formatting.Indented));
            
            // The side menu shows only second-level navigation
            var selectedChild = menu.Children.FirstOrDefault(x => x.Active || x.Selected);
            
            Console.WriteLine("Side menu selected...");
            
            Console.WriteLine(JsonConvert.SerializeObject(selectedChild, Formatting.Indented));
            
            return View(selectedChild);
        }
    }
}