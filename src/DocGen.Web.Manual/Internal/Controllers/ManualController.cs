using DocGen.Core.Markdown;
using DocGen.Web.Manual.Internal.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Manual.Internal.Controllers
{
    public class ManualController : Controller
    {
        readonly ManualSectionStore _manualSectionStore;
        readonly IMarkdownRenderer _markdownRenderer;

        public ManualController(ManualSectionStore manualSectionStore,
            IMarkdownRenderer markdownRenderer)
        {
            _manualSectionStore = manualSectionStore;
            _markdownRenderer = markdownRenderer;
        }
        
        public ActionResult Index()
        {
            var model = new ManualModel();
            foreach (var section in _manualSectionStore.GetSections())
            {
                var markdown = _markdownRenderer.RenderMarkdown(section.Markdown);
                model.Sections.Add(new SectionModel
                {
                    Html = markdown.Html
                });
            }
            return View(model);
        }
    }
}