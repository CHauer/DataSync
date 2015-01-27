// -----------------------------------------------------------------------
// <copyright file="DeleteFile.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - DeleteFile.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Operations
{
    using System;
    using System.IO;

    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync.Items;

    /// <summary>
    /// The delete file.
    /// </summary>
    public class DeleteFile : SyncOperation
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
            if (!(item is SyncFile))
            {
                this.LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
                return false;
            }

            try
            {
                File.Delete(item.TargetPath);
            }
            catch (Exception ex)
            {
                this.LogMessage(new ErrorLogMessage("Delete File Error", ex));
                return false;
            }

            return true;
        }
    }
}