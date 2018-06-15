using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocGen.Core;
using DocGen.Web.Manual.Prince;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DocGen.Web.Manual.Impl
{
    public class SymbolGlossaryStore : ISymbolGlossaryStore
    {
        DocGenOptions _options;
        List<SymbolEntry> _defaults = new List<SymbolEntry>
        {
            new SymbolEntry
            {
                Title = "Stand by",
                References = "IEC 60417, 5009",
                Symbol = "/resources/symbols/stand-by.png"
            },
            new SymbolEntry
            {
                Title = "Direct current",
                References = "IEC 60417, 5031",
                Symbol = "/resources/symbols/direct-current.png"
            },
            new SymbolEntry
            {
                Title = "Equipotentiality",
                References = "IEC 60417, 5021",
                Symbol = "/resources/symbols/equipotentiality.png"
            },
            new SymbolEntry
            {
                Title = "Manufacturer",
                References = "ISO 15223-1: 2016, 5.1.1",
                Symbol = "/resources/symbols/manufacturer.png",
                
            },
            new SymbolEntry
            {
                Title = "FCC compliance",
                References = "47 CFR 15",
                Symbol = "/resources/symbols/fcc.png"
            },
            new SymbolEntry
            {
                Title = "CE compliance",
                References = "Directive 93/42/EEC",
                Symbol = "/resources/symbols/ce.png"
            },
            new SymbolEntry
            {
                Title = "\"New\" waste",
                References = "EN 50419: 2005 Directive 2002/96/EC (WEEE)",
                Symbol = "/resources/symbols/new-waste.png"
            },
            new SymbolEntry
            {
                Title = "Caution, consult accompanying documents",
                References = "ISO 15223-1: 2016, 5.4.4",
                Symbol = "/resources/symbols/symbol-caution.png"
            },
            new SymbolEntry
            {
                Title = "Refer to instruction manual/booklet",
                References = "ISO 7010, M002",
                Symbol = "/resources/symbols/refer-to-manual.png"
            },
            new SymbolEntry
            {
                Title = "TUV certification",
                References = "IEC 60601-1: 2005 IEC 60601-1-2: 2014",
                Symbol = "/resources/symbols/tuv.png"
            },
            new SymbolEntry
            {
                Title = "Serial number",
                References = "ISO 15223-1: 2016, 5.1.7",
                Symbol = "/resources/symbols/serial-number.png"
            },
            new SymbolEntry
            {
                Title = "Catalog number",
                References = "ISO 15223-1: 2016, 5.1.6",
                Symbol = "/resources/symbols/catalogue-number.png"
            },
            new SymbolEntry
            {
                Title = "Date of manufacture",
                References = "ISO 15223-1: 2016, 5.1.3",
                Symbol = "/resources/symbols/manufacture-date.png"
            },
            new SymbolEntry
            {
                Title = "Authorized representative of the European community",
                References = "ISO 15223-1: 2016, 5.1.2",
                Symbol = "/resources/symbols/ec-representative.png"
            },
            new SymbolEntry
            {
                Title = "Consult instructions for use",
                References = "ISO 15223-1: 2016, 5.4.3",
                Symbol = "/resources/symbols/read-documentation.png"
            },
            new SymbolEntry
            {
                Title = "Keep Dry",
                References = "ISO 15223-1: 2016, 5.3.4",
                Symbol = "/resources/symbols/keep-dry.png"
            },
            new SymbolEntry
            {
                Title = "This way up",
                References = "ISO 7000, 0623",
                Symbol = "/resources/symbols/upright.png"
            }
        };
        
        public SymbolGlossaryStore(IOptions<DocGenOptions> options)
        {
            _options = options.Value;
            
        }
        
        public async Task<List<SymbolEntry>> GetSymbols()
        {
            var symbolsJson = Path.Combine(_options.ContentDirectory, "symbol-glossary.json");

            if (await Task.Run(() => File.Exists(symbolsJson)))
            {
                var content = await Task.Run(() => File.ReadAllText(symbolsJson));
                return JsonConvert.DeserializeObject<List<SymbolEntry>>(content);
            }

            return _defaults.ToList();
        }
    }
}

