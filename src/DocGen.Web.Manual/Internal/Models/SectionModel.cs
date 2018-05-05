using System.Collections.Generic;
using DocGen.Core.Markdown;

namespace DocGen.Web.Manual.Internal.Models
{
    public class SectionModel
    {
        public SectionModel()
        {
            TableOfContents = new List<TocEntry>();
        }
        
        public string Html { get; set; }
        
        public List<TocEntry> TableOfContents { get; set; }
    }
}