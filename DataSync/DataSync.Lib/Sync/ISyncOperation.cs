// -----------------------------------------------------------------------
// <copyright file="ISyncOperation.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ISyncOperation.cs</summary>
// -----------------------------------------------------------------------

using DataSync.Lib.Configuration;
using DataSync.Lib.Log;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISyncOperation
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        ILog Logger { get; set; }

        /// <summary>
        /// Gets the synchronize configuration.
        /// </summary>
        /// <value>
        /// The synchronize configuration.
        /// </value>
        SyncConfiguration Configuration { get; set; }

        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        bool Execute(ISyncItem item);
    }
}