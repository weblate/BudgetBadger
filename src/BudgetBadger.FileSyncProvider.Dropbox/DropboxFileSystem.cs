using System;
using BudgetBadger.Core.CloudSync;

namespace BudgetBadger.FileSyncProvider.Dropbox
{
    public class DropboxFileSystem : IFileSystem
    {
        public IFile File => throw new NotImplementedException();

        public IDirectory Directory => throw new NotImplementedException();
    }
}

