using System.Threading.Tasks;
using DocGen.Core.Markdown;
using DocGen.Web.Manual.Internal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DocGen.Web.Manual.Internal.Controllers
{
    public class PrinceController : Controller
    {
        readonly ManualSectionStore _manualSectionStore;
        readonly IMarkdownRenderer _markdownRenderer;
        readonly CoversheetConfig _coversheetConfig;

        public PrinceController(ManualSectionStore manualSectionStore,
            IMarkdownRenderer markdownRenderer,
            CoversheetConfig coversheetConfig)
        {
            _manualSectionStore = manualSectionStore;
            _markdownRenderer = markdownRenderer;
            _coversheetConfig = coversheetConfig;
        }
        
        public ActionResult Template()
        {
            var model = new ManualModel();
            
            model.Coversheet = _coversheetConfig;
            foreach (var section in _manualSectionStore.GetSections())
            {
                var sectionModel = new SectionModel();
                
                var markdown = _markdownRenderer.RenderMarkdown(section.Markdown);
                var toc = _markdownRenderer.ExtractTocEntries(section.Markdown);

                sectionModel.Title = section.Title;
                sectionModel.Html = markdown.Html;
                sectionModel.TableOfContents.AddRange(toc);
                
                model.Sections.Add(sectionModel);
            }
            
            return View(model);
        }
        
        public IActionResult Pdf()
        {
            return new PrinceActionResult();
        }

        class PrinceActionResult : IActionResult
        {
            public Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.ContentType = "application/pdf";
                var prince = new DocGen.Web.Manual.Prince.Prince("/usr/local/bin/prince");
                var url = new UrlHelper(context);
                prince.Convert(url.ServerBaseUrl() + "/prince/template", context.HttpContext.Response.Body);
                //prince.SetBaseURL(url.ServerBaseUrl());
                return Task.CompletedTask;
            }
        }
    }
}