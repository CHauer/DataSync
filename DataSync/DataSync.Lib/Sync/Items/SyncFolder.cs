using System;
using System.IO;

namespace DataSync.Lib.Sync.Items
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncFolder : ISyncItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncFolder"/> class.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetFolderPath">The target folder path.</param>
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
        public string SourcePath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether target file exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if target file exists; otherwise, <c>false</c>.
        /// </value>
        public bool TargetExists
        {
            get
            {
                // ReSharper disable once ConvertPropertyToExpressionBody
                return Directory.Exists(TargetPath);
            }
        }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        /// <value>
        /// The target path.
        /// </value>
        public string TargetPath { get; private set; }

        /// <summary>
        /// Gets the source file system information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public FileSystemInfo GetSourceInfo()
        {
            if (!Directory.Exists(SourcePath))
            {
                throw new DirectoryNotFoundException();
            }

            return new DirectoryInfo(SourcePath);
        }

        /// <summary>
        /// Gets the target file system information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public FileSystemInfo GetTargetInfo()
        {
            if (!Directory.Exists(TargetPath))
            {
                throw new DirectoryNotFoundException();
            }

            return new DirectoryInfo(TargetPath);
        }
    }
}
