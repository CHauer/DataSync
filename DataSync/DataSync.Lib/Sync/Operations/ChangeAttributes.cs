using System;
using System.IO;
using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync.Items;

namespace DataSync.Lib.Sync.Operations
{
    public class ChangeAttributes : SyncOperation
    {

        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool Execute(ISyncItem item)
        {
            FileAttributes attributes;
            FileSystemInfo infoObj;

            if (item == null)
            {
                LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
                return false;
            }

            if (item is SyncFile)
            {
                if (!File.Exists(item.SourcePath) && !item.TargetExists)
                {
                    return false;
                }

                try
                {
                    attributes = new FileInfo(item.SourcePath).Attributes;
                    infoObj = new FileInfo(item.TargetPath);
                }
                catch (Exception ex)
                {
                    LogMessage(new ErrorLogMessage(ex));
                    return false;
                }

            }
            else
            {
                if (!Directory.Exists(item.SourcePath) || !item.TargetExists)
                {
                    return false;
                }
                try
                {
                    attributes = new DirectoryInfo(item.SourcePath).Attributes;
                    infoObj = new DirectoryInfo(item.TargetPath);
                }
                catch (Exception ex)
                {
                    LogMessage(new ErrorLogMessage(ex));
                    return false;
                }
            }

            try
            {
                infoObj.Attributes = attributes;
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage(ex));
                return false;
            }

            return true;
        }

    }
}
