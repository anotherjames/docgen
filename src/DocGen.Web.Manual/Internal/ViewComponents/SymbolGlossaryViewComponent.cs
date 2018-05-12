using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Manual.Internal.ViewComponents
{
    public class SymbolGlossaryViewComponent : ViewComponent
    {
        readonly ISymbolGlossaryStore _symbolGlossaryStore;

        public SymbolGlossaryViewComponent(ISymbolGlossaryStore symbolGlossaryStore)
        {
            _symbolGlossaryStore = symbolGlossaryStore;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _symbolGlossaryStore.GetSymbols());
        }
    }
}