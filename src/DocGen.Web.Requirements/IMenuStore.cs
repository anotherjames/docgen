using System.Collections.Generic;

namespace DocGen.Web.Requirements
{
    public interface IMenuStore
    {
        void AddPage(string path, string title, int order);

        MenuItem BuildMenu(string currentPath, int alwaysEpandToLevel = 0);
    }
}