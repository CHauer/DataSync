// -----------------------------------------------------------------------
// <copyright file="CreateFolder.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - CreateFolder.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Operations
{
    using System;
    using System.IO;

    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync.Items;

    /// <summary>
    /// The create folder.
    /// </summary>
    public class CreateFolder : SyncOperation
    {
        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">
        /// The item value.
        /// </param>
        /// <returns>
        /// The status of the execution.
        /// </returns>
        public override bool Execute(ISyncItem item)
        {
            if (!(item is SyncFolder))
            {
                this.LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
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
                    this.LogMessage(new ErrorLogMessage("Create Folder Error", ex));
                    return false;
                }
            }

            // Copy attributes
            try
            {
                folder.GetTargetInfo().Attributes = folder.GetSourceInfo().Attributes;
            }
            catch (Exception ex)
            {
                this.LogMessage(new ErrorLogMessage("Copy Folder Attributes Error", ex));
                return false;
            }

            // Copy security
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
                this.LogMessage(
                    new WarningLogMessage(string.Format("Copy Folder Security Problem. Details:{0}", ex.Message)));

                // Dont treat as error
                // return false;
            }

            return true;
        }
    }
}