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


        public bool TargetExists
        {
            get { throw new System.NotImplementedException(); }
        }


        public string TargetPath
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public FileSystemInfo GetSourceInfo()
        {
            throw new System.NotImplementedException();
        }

        public FileSystemInfo GetTargetInfo()
        {
            throw new System.NotImplementedException();
        }
    }
}
