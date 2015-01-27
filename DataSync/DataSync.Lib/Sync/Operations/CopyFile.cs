// -----------------------------------------------------------------------
// <copyright file="CopyFile.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - CopyFile.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Operations
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync.Items;

    /// <summary>
    /// The copy file operation class.
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
        /// <param name="item">
        /// The item parameter.
        /// </param>
        /// <returns>
        /// The boolean return value.
        /// </returns>
        public override bool Execute(ISyncItem item)
        {
            this.IsBlockCopy = false;

            if (item == null || this.Configuration == null || !(item is SyncFile))
            {
                this.LogMessage(new ErrorLogMessage("Execution not possible - Invalid  Operation Properties", true));
                return false;
            }

            SyncFile file = item as SyncFile;

            while (this.IsFileLocked(file.SourcePath))
            {
                Thread.Sleep(100);
            }

            if (item.TargetExists)
            {
                try
                {
                    var sourceLength = ((FileInfo)file.GetSourceInfo()).Length;
                    var targetLength = ((FileInfo)file.GetTargetInfo()).Length;

                    this.IsBlockCopy = sourceLength == targetLength
                                       && sourceLength >= this.Configuration.BlockCompareFileSize;
                }
                catch (Exception ex)
                {
                    this.LogMessage(new ErrorLogMessage(ex));
                    this.IsBlockCopy = false;
                }
            }

            if (this.IsBlockCopy)
            {
                return this.ExecuteBlockCopy(file);
            }

            try
            {
                File.Copy(file.SourcePath, file.TargetPath, true);
            }
            catch (Exception ex)
            {
                this.LogMessage(new ErrorLogMessage(ex));
                return false;
            }

            // Copy File Attributes
            try
            {
                File.SetAttributes(file.TargetPath, file.GetSourceInfo().Attributes);
            }
            catch (Exception ex)
            {
                this.LogMessage(new ErrorLogMessage(ex));
            }

            return true;
        }

        /// <summary>
        /// Determines whether the file is locked.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="access">The access parameter.</param>
        /// <returns>
        /// The boolean return value.
        /// </returns>
        private bool IsFileLocked(string filePath, FileAccess access = FileAccess.Read)
        {
            FileStream stream = null;
            FileInfo file;

            try
            {
                file = new FileInfo(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return true;
            }

            if (!file.Exists)
            {
                return false;
            }

            try
            {
                stream = file.Open(FileMode.Open, access, FileShare.None);
            }
            catch (IOException)
            {
                // the file is unavailable because it is:
                // still being written to
                // or being processed by another thread
                // or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            // file is not locked
            return false;
        }

        /// <summary>
        /// Executes the block copy.
        /// </summary>
        /// <param name="file">
        /// The file parameter.
        /// </param>
        /// <returns>
        /// The boolean return value.
        /// </returns>
        private bool ExecuteBlockCopy(SyncFile file)
        {
            int bufferSize = this.Configuration.BlockSize;

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

                for (int runner = 1; runner <= maxFullBlock; runner++)
                {
                    // end of file reached - change buffer size to "rest size"
                    if (runner == maxFullBlock)
                    {
                        bufferSize = (int)(sourceStream.Length - sourceStream.Position);
                    }

                    bufferSource = readerSource.ReadBytes(bufferSize);
                    bufferTarget = readerTarget.ReadBytes(bufferSize);

                    // compare buffer
                    if (!this.EqualByteArrays(bufferSource, bufferTarget))
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
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if byte arrays are equal.
        /// Safe managed .NET version - for speed gain use an unmanaged version.
        /// </summary>
        /// <param name="a">
        /// The a parameter.
        /// </param>
        /// <param name="b">
        /// The b parameter.
        /// </param>
        /// <returns>
        /// The boolean return value.
        /// </returns>
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