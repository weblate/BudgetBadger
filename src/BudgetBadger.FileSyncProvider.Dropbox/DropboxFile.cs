using System;
using System.IO;
using System.Threading.Tasks;
using BudgetBadger.Core.CloudSync;
using BudgetBadger.Core.Settings;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace BudgetBadger.FileSyncProvider.Dropbox
{
	public class DropboxFile : IFile
	{
        readonly ISettings _settings;
        readonly string _appKey;

        public DropboxFile(ISettings settings, string appKey)
        {
            _settings = settings;
            _appKey = appKey;
        }

        public Task CopyAsync(string sourceFileName, string destFileName, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task MoveAsync(string sourceFileName, string destFileName, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            var refreshToken = await _settings.GetValueOrDefaultAsync(DropboxSettings.RefreshToken);
            using (var dbx = new DropboxClient(refreshToken, _appKey))
            {
                var downloadArg = new DownloadArg("/" + path);
                using (var dropboxResponse = await dbx.Files.DownloadAsync(downloadArg))
                {
                    return await dropboxResponse.GetContentAsByteArrayAsync();
                }
            }
        }

        public Task ReplaceAsync(string sourceFileName, string destFileName, string backupFileName)
        {
            throw new NotImplementedException();
        }

        public async Task WriteAllBytesAsync(string path, byte[] data)
        {
            var refreshToken = await _settings.GetValueOrDefaultAsync(DropboxSettings.RefreshToken);

            using (var fileStream = new MemoryStream(data))
            using (var dbx = new DropboxClient(refreshToken, _appKey))
            {
                var commitInfo = new CommitInfo("/" + path, mode: WriteMode.Overwrite.Instance);
                var dropBoxResponse = await dbx.Files.UploadAsync(commitInfo, fileStream);
            }
        }
    }
}

