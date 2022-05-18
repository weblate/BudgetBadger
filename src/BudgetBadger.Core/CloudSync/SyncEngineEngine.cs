using System;
using System.Threading.Tasks;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.Logic;
using BudgetBadger.Core.Utilities;
using BudgetBadger.Models;

namespace BudgetBadger.Core.CloudSync
{
    public class SyncEngineEngine : ISyncEngine
    {
        private readonly IMergeLogic _mergeLogic;

        public SyncEngineEngine(IMergeLogic mergeLogic)
        {
            _mergeLogic = mergeLogic;
        }

        public SyncEngineEngine() : this(new MergeLogic())
        {
        }

        /// <summary>
        ///     Merges all the data from the importDataAccess into the appDataAccess.
        ///     Will never delete data
        /// </summary>
        /// <param name="importDataAccess">The DataAccess going to be imported</param>
        /// <param name="appDataAccess">The DataAccess that will get imported to</param>
        /// <returns></returns>
        public async Task<Result> ImportAsync(IDataAccess importDataAccess, IDataAccess appDataAccess)
        {
            var result = new Result();

            try
            {
                await importDataAccess.Init();
                await appDataAccess.Init();

                _mergeLogic.MergeAllAsync(importDataAccess, appDataAccess);

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        ///     Merges all data from the appDataAccess into the exportDataAccess.
        ///     Will never delete data.
        /// </summary>
        /// <param name="appDataAccess"></param>
        /// <param name="exportDataAccess"></param>
        /// <returns></returns>
        public async Task<Result> ExportAsync(IDataAccess appDataAccess, IDataAccess exportDataAccess)
        {
            var result = new Result();

            try
            {
                await appDataAccess.Init();
                await exportDataAccess.Init();

                _mergeLogic.MergeAllAsync(appDataAccess, exportDataAccess);

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        ///     Reads the importFile from the importFileSystem
        ///     Decompresses importFile if needed
        ///     Copies the importFile from the importFileSystem to the tempFile on the tempFileSystem.
        ///     Merges all data from the tempDataAccess into the appDataAccess.
        ///     Will not delete data.
        /// </summary>
        /// <param name="importFileSystem">IFileSystem where the importFile exists</param>
        /// <param name="importFile">The file that has the data to import</param>
        /// <param name="decompressImportFile">Determines if import file needs decompressed</param>
        /// <param name="tempFileSystem">IFileSystem where tempFile will be saved to</param>
        /// <param name="tempFile">The file that tempDataAccess uses</param>
        /// <param name="tempDataAccess">IDataAccess backed by tempFile</param>
        /// <param name="appDataAccess">IDataAccess that is imported to</param>
        /// <returns></returns>
        public async Task<Result> FileBasedImportAsync(IFileSystem importFileSystem,
            string importFile,
            bool decompressImportFile,
            IFileSystem tempFileSystem,
            string tempFile,
            IDataAccess tempDataAccess,
            IDataAccess appDataAccess)
        {
            var result = new Result();

            try
            {
                var importedFile = await importFileSystem.File.ReadAllBytesAsync(importFile);
                if (decompressImportFile)
                {
                    importedFile = importedFile.Decompress();
                }

                await tempFileSystem.File.WriteAllBytesAsync(tempFile, importedFile);

                return await ImportAsync(tempDataAccess, appDataAccess);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        ///     Merges all data from the appDataAccess into the tempDataAccess.
        ///     Reads the tempFile from the tempFileSystem
        ///     Compresses tempFile if needed
        ///     Copies the tempFile from the tempFileSystem to the exportFile on the exportFileSystem.
        ///     Will not delete data.
        /// </summary>
        /// <param name="appDataAccess"></param>
        /// <param name="tempDataAccess"></param>
        /// <param name="tempFileSystem"></param>
        /// <param name="tempFile"></param>
        /// <param name="compressExportFile"></param>
        /// <param name="exportFileSystem"></param>
        /// <param name="exportFile"></param>
        /// <returns></returns>
        public async Task<Result> FileBasedExportAsync(IDataAccess appDataAccess,
            IDataAccess tempDataAccess,
            IFileSystem tempFileSystem,
            string tempFile,
            bool compressExportFile,
            IFileSystem exportFileSystem,
            string exportFile)
        {
            var result = new Result();

            try
            {
                var test = await ExportAsync(appDataAccess, tempDataAccess);

                if (test.Success)
                {
                    var tFile = await tempFileSystem.File.ReadAllBytesAsync(tempFile);
                    if (compressExportFile)
                    {
                        tFile = tFile.Compress();
                    }

                    await exportFileSystem.File.WriteAllBytesAsync(exportFile, tFile);

                    result.Success = true;
                }

                result = test;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}