using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocGen.Web.Manual
{
    public interface IManualTranslations
    {
        Task RegenerateTemplate();

        Task AddLanguage(string cultureCode);

        Task<List<string>> GetLanguages();
    }
}