using System.Collections.Generic;

namespace DocGen.Web.Requirements
{
    public class MenuItem
    {
        public MenuItem()
        {
            Children = new List<MenuItem>();
        }
        
        public string Title { get; set; }

        public string Path { get; set; }
        
        public bool Selected { get; set; }
        
        public bool Active { get; set; }
        
        public bool IsEmptyParent { get; set; }

        public List<MenuItem> Children { get; set; }
    }
}