using System.Threading.Tasks;

namespace DocGen.Web.Manual
{
    public interface IManualTranslations
    {
        Task RegenerateTemplate(string contentDirectory);
    }
}