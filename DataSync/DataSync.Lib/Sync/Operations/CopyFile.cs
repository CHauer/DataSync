// -----------------------------------------------------------------------
// <copyright file="CopyFile.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - CopyFile.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync.Items;

namespace DataSync.Lib.Sync.Operations
{
    /// <summary>
    /// 
    /// </summary>
    [SupportedType(typeof (SyncFile))]
    public class CopyFile : ISyncOperation
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets the synchronize configuration.
        /// </summary>
        /// <value>
        /// The synchronize configuration.
        /// </value>
        public SyncConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is block copy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is block copy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlockCopy { get; private set; }

        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">item</exception>
        /// <exception cref="System.InvalidOperationException">The given item type is not valid for this sync operation!</exception>
        public bool Execute(ISyncItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var supportedTypes = GetType().GetCustomAttributes(typeof (SupportedTypeAttribute), false)
                .Select(attr => ((SupportedTypeAttribute) attr).SupportedType).ToList();

            if (!supportedTypes.Contains(item.GetType()))
            {
                throw new InvalidOperationException("The given item type is not valid for this sync operation!");
            }

            SyncFile file = item as SyncFile;

            if (file != null)
            {
                if (Configuration.IsBlockCompare)
                {
                    FileInfo fileInfo = file.GetSourceInfo() as FileInfo;

                    if (fileInfo != null && fileInfo.Length >= Configuration.BlockCompareFileSize)
                    {
                        IsBlockCopy = true;
                    }
                }

                if (IsBlockCopy)
                {
                    return ExecuteBlockCopy(file);
                }

                try
                {
                    File.Copy(file.SourcePath, file.TargetPath, true);
                    return true;
                }
                catch (Exception ex)
                {
                    if (Logger != null)
                    {
                        Logger.AddLogMessage(new ErrorLogMessage(ex));
                    }
                }
            }

            return false;
        }

        private bool ExecuteBlockCopy(SyncFile file)
        {
            //TODO block copy
            throw new NotImplementedException();
        }
    }
}