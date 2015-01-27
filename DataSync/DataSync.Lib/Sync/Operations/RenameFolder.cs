// -----------------------------------------------------------------------
// <copyright file="RenameFolder.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - RenameFolder.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Operations
{
    using System;
    using System.IO;

    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync.Items;

    /// <summary>
    /// The rename folder.
    /// </summary>
    public class RenameFolder : SyncOperation
    {
        /// <summary>
        /// The execute.
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

            try
            {
                Directory.Move(folder.SourcePath, folder.TargetPath);
            }
            catch (Exception ex)
            {
                this.LogMessage(new ErrorLogMessage("Rename Folder Error", ex));
                return false;
            }

            return true;
        }
    }
}