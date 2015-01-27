// -----------------------------------------------------------------------
// <copyright file="SyncJobLogMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncJobLogMessage.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Log.Messages
{
    using System;

    using DataSync.Lib.Sync;

    /// <summary>
    /// The sync job log message class.
    /// </summary>
    [Serializable]
    public class SyncJobLogMessage : LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncJobLogMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="syncJob">
        /// The synchronize job.
        /// </param>
        public SyncJobLogMessage(string message, ISyncJob syncJob)
            : base(CreateMessage(message, syncJob), false)
        {
        }

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="syncJob">
        /// The synchronize job.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string CreateMessage(string message, ISyncJob syncJob)
        {
            return string.Format("{0}\n{1}", message, syncJob.ToString());
        }
    }
}