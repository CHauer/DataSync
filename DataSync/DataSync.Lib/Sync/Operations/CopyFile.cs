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
    public class CopyFile : SyncOperation
    {

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
        public override bool Execute(ISyncItem item)
        {
            IsBlockCopy = false;

            if (item == null || Configuration == null || !(item is SyncFile))
            {
                LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
                return false;
            }

            SyncFile file = item as SyncFile;

            if (Configuration.IsBlockCompare && item.TargetExists)
            {
                try
                {
                    var sourceLength = ((FileInfo)file.GetSourceInfo()).Length;
                    var targetLength = ((FileInfo)file.GetTargetInfo()).Length;

                    IsBlockCopy = sourceLength == targetLength && sourceLength >= this.Configuration.BlockCompareFileSize;
                }
                catch (Exception ex)
                {
                    LogMessage(new ErrorLogMessage(ex));
                    IsBlockCopy = false; 
                }
            }

            if (IsBlockCopy)
            {
                return ExecuteBlockCopy(file);
            }

            try
            {
                File.Copy(file.SourcePath, file.TargetPath, true);
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage(ex));
                return false;
            }

            //Copy File Attributes
            try
            {
                File.SetAttributes(file.TargetPath, file.GetSourceInfo().Attributes);
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage(ex));
            }

            return true;
        }

        /// <summary>
        /// Executes the block copy.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private bool ExecuteBlockCopy(SyncFile file)
        {
            int bufferSize = Configuration.BlockSize;
            
            try
            {
                // ReSharper disable once TooWideLocalVariableScope
                byte[] bufferSource;
                // ReSharper disable once TooWideLocalVariableScope                
                byte[] bufferTarget;

                FileStream sourceStream = File.Open(file.SourcePath, FileMode.Open, FileAccess.Read);
                FileStream targetStream = File.Open(file.TargetPath, FileMode.Open, FileAccess.ReadWrite);
                BinaryReader readerSource = new BinaryReader(sourceStream);
                BinaryReader readerTarget = new BinaryReader(targetStream);
                BinaryWriter writer = new BinaryWriter(targetStream);

                int maxFullBlock = (int)(sourceStream.Length / bufferSize);

                for (int runner = 1; runner <= maxFullBlock; maxFullBlock++)
                {
                    //end of file reached - change buffer size to "rest size"
                    if (runner == maxFullBlock)
                    {
                        bufferSize = (int)(sourceStream.Length - sourceStream.Position);
                    }

                    bufferSource = readerSource.ReadBytes(bufferSize);
                    bufferTarget = readerTarget.ReadBytes(bufferSize);

                    //compare buffer
                    if (!EqualByteArrays(bufferSource, bufferTarget))
                    {
                        writer.Seek(-bufferSize, SeekOrigin.Current);
                        writer.Write(bufferSource);
                    }
                }

                sourceStream.Close();
                writer.Flush();
                writer.Close();
                targetStream.Close();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if byte arrays are equal.
        /// Safe manged .NET version - for speed gain use an unmanaged version.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private bool EqualByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}