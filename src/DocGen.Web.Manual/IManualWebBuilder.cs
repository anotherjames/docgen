using System.Threading.Tasks;

namespace DocGen.Web.Manual
{
    public interface IManualWebBuilder
    {
        Task<IManualWeb> BuildManual();
    }
}