using System;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync.Items;
using System.IO;

namespace DataSync.Lib.Sync.Operations
{
    public class RenameFolder : SyncOperation
    {
        public override bool Execute(ISyncItem item)
        {
            if (!(item is SyncFolder))
            {
                LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
                return false;
            }

            SyncFolder folder = item as SyncFolder;

            try
            {
                Directory.Move(folder.SourcePath, folder.TargetPath);
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage("Rename Folder Error", ex));
                return false;
            }

            return true;
        }

     }
}
