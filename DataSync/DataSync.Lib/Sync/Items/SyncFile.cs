using System;
using System.IO;

namespace DataSync.Lib.Sync.Items
{
    public class SyncFile : ISyncItem
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncFile" /> class.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="targetPath">The target path.</param>
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
        /// Gets the source path.
        /// </summary>
        /// <value>
        /// The source path.
        /// </value>
        public string SourcePath { get; private set; }

        /// <summary>
        /// Gets the target path.
        /// </summary>
        /// <value>
        /// The target path.
        /// </value>
        public string TargetPath { get; private set; }

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
                return File.Exists(TargetPath);
            }
        }

        /// <summary>
        /// Gets the source file information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public FileInfo GetSourceFileInfo()
        {
            if (!File.Exists(SourcePath))
            {
                throw new FileNotFoundException();
            }
           
            return new FileInfo(SourcePath);
        }

        /// <summary>
        /// Gets the target file information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public FileInfo GetTargetFileInfo()
        {
            if (!File.Exists(TargetPath))
            {
                throw new FileNotFoundException();
            }

            return new FileInfo(TargetPath);
        }

    }
}
