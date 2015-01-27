// -----------------------------------------------------------------------
// <copyright file="SyncFile.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncFile.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Items
{
    using System.IO;

    /// <summary>
    /// The sync file.
    /// </summary>
    public class SyncFile : ISyncItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncFile"/> class.
        /// </summary>
        /// <param name="sourceFile">
        /// The source file.
        /// </param>
        /// <param name="targetPath">
        /// The target path.
        /// </param>
        public SyncFile(string sourceFile, string targetPath)
        {
            this.SourcePath = sourceFile;
            this.TargetPath = targetPath;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        /// <value>
        /// The source path.
        /// </value>
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        /// <value>
        /// The target path.
        /// </value>
        public string TargetPath { get; set; }

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
                return File.Exists(this.TargetPath);
            }
        }

        /// <summary>
        /// Gets the source file system information.
        /// </summary>
        /// <returns>
        /// The <see cref="FileSystemInfo"/>.
        /// </returns>
        /// <exception cref="System.IO.FileNotFoundException">
        /// File not found exception.
        /// </exception>
        public FileSystemInfo GetSourceInfo()
        {
            if (!File.Exists(this.SourcePath))
            {
                throw new FileNotFoundException();
            }

            return new FileInfo(this.SourcePath);
        }

        /// <summary>
        /// Gets the target file system information.
        /// </summary>
        /// <returns>
        /// The <see cref="FileSystemInfo"/>.
        /// </returns>
        /// <exception cref="System.IO.FileNotFoundException">
        /// File not found exception.
        /// </exception>
        public FileSystemInfo GetTargetInfo()
        {
            if (!File.Exists(this.TargetPath))
            {
                throw new FileNotFoundException();
            }

            return new FileInfo(this.TargetPath);
        }
    }
}