using System;
using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync.Items;
using System.IO;

namespace DataSync.Lib.Sync.Operations
{
    public class DeleteFile : SyncOperation
    {

        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool Execute(ISyncItem item)
        {
            if (!(item is SyncFile))
            {
                LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
                return false;
            }

            try
            {
                File.Delete(item.TargetPath);
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage("Delete File Error", ex));
                return false;
            }

            return true;
        }

    }
}
