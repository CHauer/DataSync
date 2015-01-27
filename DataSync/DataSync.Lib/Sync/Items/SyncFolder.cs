// -----------------------------------------------------------------------
// <copyright file="SyncFolder.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncFolder.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Items
{
    using System.IO;

    /// <summary>
    /// The sync folder class.
    /// </summary>
    public class SyncFolder : ISyncItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncFolder"/> class.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="targetFolderPath">
        /// The target folder path.
        /// </param>
        public SyncFolder(string sourcePath, string targetFolderPath)
        {
            this.SourcePath = sourcePath;
            this.TargetPath = targetFolderPath;
        }

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        /// <value>
        /// The source path.
        /// </value>
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets a value indicating whether target file exists.
        /// </summary>
        /// <value>
        /// <c>true</c> if target file exists; otherwise, <c>false</c>.
        /// </value>
        public bool TargetExists
        {
            get
            {
                // ReSharper disable once ConvertPropertyToExpressionBody
                return Directory.Exists(this.TargetPath);
            }
        }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        /// <value>
        /// The target path.
        /// </value>
        public string TargetPath { get; set; }

        /// <summary>
        /// Gets the source file system information.
        /// </summary>
        /// <returns>
        /// The <see cref="FileSystemInfo"/> value.
        /// </returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Directory not found exception.
        /// </exception>
        public FileSystemInfo GetSourceInfo()
        {
            if (!Directory.Exists(this.SourcePath))
            {
                throw new DirectoryNotFoundException();
            }

            return new DirectoryInfo(this.SourcePath);
        }

        /// <summary>
        /// Gets the target file system information.
        /// </summary>
        /// <returns>
        /// The <see cref="FileSystemInfo"/> value.
        /// </returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Directory not found exception.
        /// </exception>
        public FileSystemInfo GetTargetInfo()
        {
            if (!Directory.Exists(this.TargetPath))
            {
                throw new DirectoryNotFoundException();
            }

            return new DirectoryInfo(this.TargetPath);
        }
    }
}