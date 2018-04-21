using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocGen.Requirements
{
    public interface IRequirementsBuilder
    {
        Task<List<UserNeed>> BuildRequirementsFromRepositoryCommit(string repository, string sha);

        Task<List<UserNeed>> BuildRequirementsFromDirectory(string directory);
    }
}