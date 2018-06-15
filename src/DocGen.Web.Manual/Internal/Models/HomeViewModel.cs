using System.Collections.Generic;

namespace DocGen.Web.Manual.Internal.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Languages = new List<string>();
        }
        
        public List<string> Languages { get; set; }
    }
}