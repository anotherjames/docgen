using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocGen.Web.Manual
{
    public interface IManualTranslations
    {
        Task RegenerateTemplate();

        List<string> GetLanguages();
    }
}