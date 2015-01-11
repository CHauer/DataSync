using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSync.Lib.Sync.Items;
using System.IO;
using DataSync.Lib.Sync.Operations;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncItemComparer : ISyncItemComparer
    {
        /// <summary>
        /// Compares the specified compare item.
        /// </summary>
        /// <param name="compareItem">The compare item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">compareItem</exception>
        /// <exception cref="System.ArgumentException">CompareItem Type is not supported for the compare function!</exception>
        public ISyncOperation Compare(ISyncItem compareItem)
        {
            if (compareItem == null)
            {
                throw new ArgumentNullException("compareItem");
            }

            if (compareItem is SyncFile)
            {
                // ReSharper disable once TryCastAlwaysSucceeds
                return CompareFile(compareItem as SyncFile);
            }
            
            if (compareItem is SyncFolder)
            {
                // ReSharper disable once TryCastAlwaysSucceeds
                return CompareFolder(compareItem as SyncFolder);
            }

            throw new ArgumentException("CompareItem Type is not supported for the compare function!");
        }

        private ISyncOperation CompareFile(SyncFile syncFile)
        {
            FileInfo sourceFile = syncFile.GetSourceFileInfo();

            if (!syncFile.TargetExists)
            {
                return new CopyFile();
            }

            FileInfo targetFile = syncFile.GetSourceFileInfo();
        }

        private ISyncOperation CompareFolder(SyncFolder syncFolder)
        {
            throw new NotImplementedException();
        }
    }
}
