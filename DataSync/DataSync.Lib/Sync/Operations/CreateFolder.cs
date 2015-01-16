using System;
using System.IO;
using System.Security.AccessControl;
using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync.Items;

namespace DataSync.Lib.Sync.Operations
{
    public class CreateFolder : SyncOperation
    {

        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool Execute(ISyncItem item)
        {
            if (!(item is SyncFolder))
            {
                LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
                return false;
            }

            SyncFolder folder = item as SyncFolder;

            if (!folder.TargetExists)
            {
                try
                {
                    Directory.CreateDirectory(folder.TargetPath);
                }
                catch (Exception ex)
                {
                    LogMessage(new ErrorLogMessage("Create Folder Error", ex));
                    return false;
                }
            }

            //Copy attributes
            try
            {
                folder.GetTargetInfo().Attributes = folder.GetSourceInfo().Attributes;
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage("Copy Folder Attributes Error", ex));
                return false;
            }

            //Copy security
            try
            {
                var directoryInfo = folder.GetSourceInfo() as DirectoryInfo;

                if (directoryInfo != null)
                {
                    Directory.SetAccessControl(folder.TargetPath, directoryInfo.GetAccessControl());
                }
            }
            catch (Exception ex)
            {
                LogMessage(new WarningLogMessage(String.Format("Copy Folder Security Problem. Details:{0}", ex.Message)));
                
                //Dont treat as error
                //return false;
            }

            return true;
        }

    }
}
