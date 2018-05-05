using System.Collections.Generic;

namespace DocGen.Web.Manual.Internal.Models
{
    public class ManualModel
    {
        public ManualModel()
        {
            Sections = new List<SectionModel>();
        }
        
        public CoversheetConfig Coversheet { get; set; }
        
        public List<SectionModel> Sections { get; }
    }
}