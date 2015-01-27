// -----------------------------------------------------------------------
// <copyright file="DeleteFolder.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - DeleteFolder.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Operations
{
    using System;
    using System.IO;

    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync.Items;

    /// <summary>
    /// The delete folder.
    /// </summary>
    public class DeleteFolder : SyncOperation
    {
        /// <summary>
        /// Runs the operation for the specified item.
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

            try
            {
                Directory.Delete(item.TargetPath, true);
            }
            catch (Exception ex)
            {
                this.LogMessage(new ErrorLogMessage("Delete Folder Error", ex));
                return false;
            }

            return true;
        }
    }
}