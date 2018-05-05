using DocGen.Core.Markdown;
using DocGen.Web.Manual.Internal.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocGen.Web.Manual.Internal.Controllers
{
    public class ManualController : Controller
    {
        readonly ManualSectionStore _manualSectionStore;
        readonly IMarkdownRenderer _markdownRenderer;
        readonly CoversheetConfig _coversheetConfig;

        public ManualController(ManualSectionStore manualSectionStore,
            IMarkdownRenderer markdownRenderer,
            CoversheetConfig coversheetConfig)
        {
            _manualSectionStore = manualSectionStore;
            _markdownRenderer = markdownRenderer;
            _coversheetConfig = coversheetConfig;
        }
        
        public ActionResult Index()
        {
            var model = new ManualModel();
            
            model.Coversheet = _coversheetConfig;
            foreach (var section in _manualSectionStore.GetSections())
            {
                var sectionModel = new SectionModel();
                
                var markdown = _markdownRenderer.RenderMarkdown(section.Markdown);
                var toc = _markdownRenderer.ExtractTocEntries(section.Markdown);

                sectionModel.Html = markdown.Html;
                sectionModel.TableOfContents.AddRange(toc);
                
                model.Sections.Add(sectionModel);
            }
            
            return View(model);
        }
    }
}