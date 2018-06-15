using System.Threading.Tasks;
using DocGen.Web.Manual.Internal.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Manual.Internal.Controllers
{
    public class HomeController : Controller
    {
        readonly IManualTranslations _manualTranslations;

        public HomeController(IManualTranslations manualTranslations)
        {
            _manualTranslations = manualTranslations;
        }
        
        public async Task<ActionResult> Index()
        {
            var model = new HomeViewModel();
            
            model.Languages.AddRange(await _manualTranslations.GetLanguages());
            
            return View(model);
        }
    }
}