using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSync.Lib.Sync;

namespace DataSync.Lib.Log.Messages
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SyncJobLogMessage : LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncJobLogMessage" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="syncJob">The synchronize job.</param>
        public SyncJobLogMessage(string message, ISyncJob syncJob)
            : base(CreateMessage(message, syncJob), false)
        {

        }

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="syncJob">The synchronize job.</param>
        /// <returns></returns>
        public static string CreateMessage(string message, ISyncJob syncJob)
        {
            return String.Format("{0}\n{1}", message, syncJob.ToString());
        }

    }
}
