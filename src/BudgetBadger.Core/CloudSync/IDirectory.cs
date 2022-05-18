using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetBadger.Core.CloudSync
{
    public interface IDirectory
    {
        Task CreateDirectoryAsync(string path);
        Task<bool> ExistsAsync(string path);
        Task<IReadOnlyList<string>> GetFilesAsync(string path);
        Task<IReadOnlyList<string>> GetDirectoriesAsync(string path);
        Task DeleteAsync(string path, bool recursive = false);
        Task MoveAsync(string sourceDirName, string destDirName);
        Task CopyAsync(string sourceDirName, string destDirName);
    }
}