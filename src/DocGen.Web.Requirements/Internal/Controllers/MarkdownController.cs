using System.Threading.Tasks;
using DocGen.Core.Markdown;
using DocGen.Web.Requirements.Internal.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Requirements.Internal.Controllers
{
    public class MarkdownController : Controller
    {
        readonly IMarkdownRenderer _markdownRenderer;

        public MarkdownController(IMarkdownRenderer markdownRenderer)
        {
            _markdownRenderer = markdownRenderer;
        }
        
        public async Task<ActionResult> Page(string page)
        {
            var markdown = await _markdownRenderer.RenderMarkdownFromFile(page);
            
            var model = new MarkdownModel();
            model.Title = markdown.Yaml?.Title;
            model.Markdown = markdown.Html;
            
            return View(model);
        }
    }
}