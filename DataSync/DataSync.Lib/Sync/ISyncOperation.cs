namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISyncOperation
    {
        
        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Execute(ISyncItem item);
    }
}
