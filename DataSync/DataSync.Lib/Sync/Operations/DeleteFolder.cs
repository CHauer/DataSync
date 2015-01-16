// -----------------------------------------------------------------------
// <copyright file="DeleteFolder.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - DeleteFolder.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.IO;
using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync.Items;

namespace DataSync.Lib.Sync.Operations
{
    public class DeleteFolder : SyncOperation
    {

        /// <summary>
        /// Runs the operation for the specified item.
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

            try
            {
                Directory.Delete(item.TargetPath, true);
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage("Delete Folder Error", ex));
                return false;
            }

            return true;
        }

    }
}