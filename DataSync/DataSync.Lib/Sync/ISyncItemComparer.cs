// -----------------------------------------------------------------------
// <copyright file="ISyncItemComparer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ISyncItemComparer.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync
{
    using DataSync.Lib.Log;

    /// <summary>
    /// The sync item comparer interface.
    /// </summary>
    public interface ISyncItemComparer
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        ILog Logger { get; set; }

        /// <summary>
        /// Compares the specified compare item.
        /// </summary>
        /// <param name="compareItem">
        /// The compare item.
        /// </param>
        /// <returns>
        /// The <see cref="SyncOperation"/>.
        /// </returns>
        SyncOperation Compare(ISyncItem compareItem);
    }
}