using System.Collections.Generic;
using System.Linq;

namespace DocGen.Web.Manual.Internal
{
    public class ManualSectionStore
    {
        readonly List<ManualSection> _manualSections = new List<ManualSection>();
        
        public void AddMarkdown(string title, int order, string markdown, string file)
        {
            _manualSections.Add(new ManualSection(title, order, markdown, file));
        }

        public List<ManualSection> GetSections()
        {
            return _manualSections.OrderBy(x => x.Order).ToList();
        }
    }
}