// -----------------------------------------------------------------------
// <copyright file="RenameFile.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - RenameFile.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.IO;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync.Items;

namespace DataSync.Lib.Sync.Operations
{
    /// <summary>
    /// 
    /// </summary>
    public class RenameFile : SyncOperation
    {
        /// <summary>
        /// Runs the operation for the specified item.
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

            SyncFile file = item as SyncFile;

            try
            {
                File.Move(file.SourcePath, file.TargetPath);
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage("Rename File Error", ex));
                return false;
            }

            return true;
        }
    }
}