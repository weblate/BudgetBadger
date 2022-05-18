using System.Threading.Tasks;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.Logic;
using BudgetBadger.Models;

namespace BudgetBadger.Core.CloudSync
{
    public interface ISyncEngine
    {
        Task<Result> ImportAsync(IDataAccess importDataAccess, IDataAccess appDataAccess);
        Task<Result> ExportAsync(IDataAccess appDataAccess, IDataAccess exportDataAccess);
        Task<Result> FileBasedImportAsync(IFileSystem importFileSystem,
            string importFile,
            bool decompressImportFile,
            IFileSystem tempFileSystem,
            string tempFile,
            IDataAccess tempDataAccess,
            IDataAccess appDataAccess);

        Task<Result> FileBasedExportAsync(IDataAccess appDataAccess,
            IDataAccess tempDataAccess,
            IFileSystem tempFileSystem,
            string tempFile,
            bool compressExportFile,
            IFileSystem exportFileSystem,
            string exportFile);
    }
}