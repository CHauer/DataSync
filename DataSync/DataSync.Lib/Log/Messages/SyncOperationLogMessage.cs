// -----------------------------------------------------------------------
// <copyright file="SyncOperationLogMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncOperationLogMessage.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Log.Messages
{
    using System;

    using DataSync.Lib.Sync;

    /// <summary>
    /// The sync operation log message.
    /// </summary>
    [Serializable]
    public class SyncOperationLogMessage : LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncOperationLogMessage"/> class. 
        /// Initializes a new instance of the <see cref="SyncJobLogMessage"/> class.
        /// </summary>
        /// <param name="syncOperation">
        /// The synchronize job.
        /// </param>
        /// <param name="item">
        /// The item value.
        /// </param>
        public SyncOperationLogMessage(SyncOperation syncOperation, ISyncItem item)
            : base(CreateMessage(syncOperation, item), false)
        {
        }

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="syncOperation">
        /// The synchronize operation.
        /// </param>
        /// <param name="item">
        /// The item parameter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> created message value.
        /// </returns>
        public static string CreateMessage(SyncOperation syncOperation, ISyncItem item)
        {
            return string.Format(
                "Executing {0}: {1} -> {2}", 
                syncOperation.GetType().Name, 
                item.SourcePath, 
                item.TargetPath);
        }
    }
}