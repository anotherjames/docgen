using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocGen.Web.Manual
{
    public interface ISymbolGlossaryStore
    {
        Task<List<SymbolEntry>> GetSymbols();
    }
}