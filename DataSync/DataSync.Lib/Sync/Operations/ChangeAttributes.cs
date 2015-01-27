// -----------------------------------------------------------------------
// <copyright file="ChangeAttributes.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ChangeAttributes.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Operations
{
    using System;
    using System.IO;

    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync.Items;

    /// <summary>
    /// The change attributes.
    /// </summary>
    public class ChangeAttributes : SyncOperation
    {
        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">
        /// The sync item.
        /// </param>
        /// <returns>
        /// The execution status.
        /// </returns>
        public override bool Execute(ISyncItem item)
        {
            FileAttributes attributes;
            FileSystemInfo infoObj;

            if (item == null)
            {
                this.LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
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
                    this.LogMessage(new ErrorLogMessage(ex));
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
                    this.LogMessage(new ErrorLogMessage(ex));
                    return false;
                }
            }

            try
            {
                infoObj.Attributes = attributes;
            }
            catch (Exception ex)
            {
                this.LogMessage(new ErrorLogMessage(ex));
                return false;
            }

            return true;
        }
    }
}