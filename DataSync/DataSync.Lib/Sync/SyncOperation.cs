// -----------------------------------------------------------------------
// <copyright file="SyncOperation.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncOperation.cs</summary>
// -----------------------------------------------------------------------

using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SyncOperation
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets the synchronize configuration.
        /// </summary>
        /// <value>
        /// The synchronize configuration.
        /// </value>
        public SyncConfiguration Configuration { get; set; }

        /// <summary>
        /// Runs the operation for the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public abstract bool Execute(ISyncItem item);

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void LogMessage(LogMessage message)
        {
            // ReSharper disable once UseNullPropagation
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(message);
            }
        }
    }
}