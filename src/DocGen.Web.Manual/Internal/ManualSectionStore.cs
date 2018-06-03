using System;
using System.Collections.Generic;
using System.Linq;

namespace DocGen.Web.Manual.Internal
{
    public class ManualSectionStore
    {
        readonly List<ManualSection> _manualSections = new List<ManualSection>();
        
        public void AddMarkdown(string language, string title, int order, string markdown, string file)
        {
            _manualSections.Add(new ManualSection(language, title, order, markdown, file));
        }

        public List<ManualSection> GetSections(string language)
        {
            if(string.IsNullOrEmpty(language)) throw new ArgumentOutOfRangeException(nameof(language));
            return _manualSections.Where(x => x.Language == language).OrderBy(x => x.Order).ToList();
        }
    }
}