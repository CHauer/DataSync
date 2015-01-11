namespace DataSync.Lib.Sync
{
    public interface ISyncItem
    {
        string SourcePath
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether target file exists.
        /// </summary>
        /// <value><c>true</c> if target file exists; otherwise, <c>false</c>.</value>
        bool TargetExists
        {
            get;
        }

        /// <summary>
        /// Gets the target path.
        /// </summary>
        /// <value>The target path.</value>
        string TargetPath
        {
            get;
            set;
        }
    }
}
