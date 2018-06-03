using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Core.Markdown;
using DocGen.Web.Manual.Internal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;

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
        
        public async Task<ActionResult> Template(string language)
        {
            var model = new ManualModel();

            model.Coversheet = new CoversheetConfig
            {
                ProductImage = _coversheetConfig.ProductImage,
                ProductLogo = _coversheetConfig.ProductLogo,
                Model = _coversheetConfig.Model,
                Text = _coversheetConfig.Text
            };

            model.Coversheet.ProductImage = Helpers.ResolvePathPart("/", model.Coversheet.ProductImage);
            model.Coversheet.ProductLogo = Helpers.ResolvePathPart("/", model.Coversheet.ProductLogo);

            model.Coversheet.ProductImage = Url.Content($"~{model.Coversheet.ProductImage}");
            model.Coversheet.ProductLogo = Url.Content($"~{model.Coversheet.ProductLogo}");
            
            foreach (var section in _manualSectionStore.GetSections())
            {
                var sectionModel = new SectionModel();

                var markdown = section.Markdown;
                
                // Update the links in the rendered markdown document.
                
                markdown = _markdownRenderer.TransformLinks(markdown, link =>
                {
                    // Since all markdown files are in root,
                    // let's base the urls off of "/".
                    var resolved = Helpers.ResolvePathPart("/", link);
                    // This will append our app base to the url, if any.
                    return Url.Content($"~{resolved}");
                });
                
                var markdownResult = _markdownRenderer.RenderMarkdown(markdown);
                var toc = _markdownRenderer.ExtractTocEntries(section.Markdown);

                sectionModel.Title = section.Title;
                sectionModel.Html = markdownResult.Html;
                sectionModel.TableOfContents.AddRange(toc);
                
                // Replace some tokens in the markdown with some things we generate.
                if (sectionModel.Html.Contains("{{symbol-glossary}}"))
                {
                    var symbolGlossary = await RenderViewComponent("SymbolGlossary", new { });
                    sectionModel.Html = sectionModel.Html.Replace("{{symbol-glossary}}", symbolGlossary);
                }

                var matches = Regex.Matches(sectionModel.Html, "{{content:(.+)}}");
                foreach (Match match in matches)
                {
                    var contentName = match.Groups[1].Value;
                    var content = await RenderViewComponent("SharedContent", new
                    {
                        templateName = contentName
                    });
                    sectionModel.Html = Regex.Replace(sectionModel.Html, match.Value, content);
                }
                
                model.Sections.Add(sectionModel);
            }
            
            return View(model);
        }

        public async Task<string> RenderViewComponent(string viewComponent, object args)
        {
            var sp = HttpContext.RequestServices;
            
            var helper = new DefaultViewComponentHelper(
                sp.GetRequiredService<IViewComponentDescriptorCollectionProvider>(),
                HtmlEncoder.Default,
                sp.GetRequiredService<IViewComponentSelector>(),
                sp.GetRequiredService<IViewComponentInvokerFactory>(),
                sp.GetRequiredService<IViewBufferScope>());

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, NullView.Instance, ViewData, TempData, writer, new HtmlHelperOptions());
                helper.Contextualize(context);
                var result = await helper.InvokeAsync(viewComponent, args);
                result.WriteTo(writer, HtmlEncoder.Default);
                await writer.FlushAsync();
                return writer.ToString();
            }
        }
        
        public IActionResult Pdf(string language)
        {
            return new PrinceActionResult(language);
        }

        class PrinceActionResult : IActionResult
        {
            readonly string _language;

            public PrinceActionResult(string language)
            {
                _language = language;
            }
            
            public Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.ContentType = "application/pdf";
                var prince = new DocGen.Web.Manual.Prince.Prince("/usr/local/bin/prince");
                var url = new UrlHelper(context);
                prince.Convert(url.ServerBaseUrl() + $"/prince/{_language}/template", context.HttpContext.Response.Body);
                return Task.CompletedTask;
            }
        }
    }
}