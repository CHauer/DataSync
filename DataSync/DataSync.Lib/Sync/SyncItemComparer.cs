// -----------------------------------------------------------------------
// <copyright file="SyncItemComparer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncItemComparer.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync.Items;
using DataSync.Lib.Sync.Operations;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncItemComparer : ISyncItemComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncItemComparer"/> class.
        /// </summary>
        public SyncItemComparer()
        {
            IsHashCompare = false;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Compares the specified compare item.
        /// </summary>
        /// <param name="compareItem">The compare item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">compareItem</exception>
        /// <exception cref="System.ArgumentException">CompareItem Type is not supported for the compare function!</exception>
        public SyncOperation Compare(ISyncItem compareItem)
        {
            if (compareItem == null)
            {
                throw new ArgumentNullException("compareItem");
            }

            if (compareItem is SyncFolder)
            {
                // ReSharper disable once TryCastAlwaysSucceeds
                return CompareFolder(compareItem);
            }

            if (compareItem is SyncFile)
            {
                // ReSharper disable once TryCastAlwaysSucceeds
                return CompareFile(compareItem);
            }

            throw new ArgumentException("CompareItem Type is not supported for the compare function!");
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hash compare.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hash compare; otherwise, <c>false</c>.
        /// </value>
        public bool IsHashCompare { get; set; }

        /// <summary>
        /// Compares the file.
        /// </summary>
        /// <param name="syncFile">The synchronize file.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">File Compare failed.</exception>
        private SyncOperation CompareFile(ISyncItem syncFile)
        {
            //Standard operation - copy/override file
            SyncOperation standardOperation = new CopyFile();

            if (!syncFile.TargetExists)
            {
                //copy file and attributes
                return standardOperation;
            }

            try
            {
                FileInfo sourceFile = null;
                FileInfo targetFile = null;

                try
                {
                    sourceFile = syncFile.GetSourceInfo() as FileInfo;
                    targetFile = syncFile.GetTargetInfo() as FileInfo;
                }
                catch (Exception ex)
                {
                    LogMessage(new ErrorLogMessage(ex, true));
                    targetFile = null;
                }

                if (sourceFile == null)
                {
                    LogMessage(new ErrorLogMessage(String.Format("{0} Source File not found!", syncFile.SourcePath), true));

                    //no operation possible
                    return null;
                }

                //error with target file - copy/override
                if (targetFile == null)
                {
                    return standardOperation;
                }

                //file length unequal
                if (sourceFile.Length != targetFile.Length)
                {
                    return standardOperation;
                }

                //source file lastwrite 
                if (sourceFile.LastWriteTime > targetFile.LastWriteTime)
                {
                    return standardOperation;
                }

                if (IsHashCompare)
                {
                    //file content (hash) different
                    if (!ComputeHash(syncFile.SourcePath).Equals(ComputeHash(syncFile.TargetPath)))
                    {
                        return standardOperation;
                    }
                }

                //attributes unequal 
                if (!sourceFile.Attributes.Equals(targetFile.Attributes))
                {
                    return new ChangeAttributes();
                }
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage(ex, true));
            }

            //no operation found
            return null;
        }

        /// <summary>
        /// Compares the folder.
        /// </summary>
        /// <param name="syncFolder">The synchronize folder.</param>
        /// <returns></returns>
        private SyncOperation CompareFolder(ISyncItem syncFolder)
        {
            //target folder does not exists  - create
            if (!syncFolder.TargetExists)
            {
                return new CreateFolder();
            }

            //attributes differ - change attributes
            if (syncFolder.GetTargetInfo().Attributes.Equals(syncFolder.GetSourceInfo().Attributes))
            {
                return new ChangeAttributes();
            }

            //no operation found
            return null;
        }

        /// <summary>
        /// Computes the hash for the given file
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <returns></returns>
        private string ComputeHash(string filepath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filepath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }

        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(LogMessage message)
        {
            // ReSharper disable once UseNullPropagation
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(message);
            }
        }

    }
}