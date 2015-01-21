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
    public class SyncOperationLogMessage : LogMessage
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncJobLogMessage" /> class.
        /// </summary>
        /// <param name="syncOperation">The synchronize job.</param>
        /// <param name="item">The item.</param>
        public SyncOperationLogMessage(SyncOperation syncOperation, ISyncItem item)
            : base(CreateMessage(syncOperation, item), false)
        {

        }

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="syncOperation">The synchronize operation.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string CreateMessage(SyncOperation syncOperation, ISyncItem item)
        {
            return String.Format("Executing {0}: {1} -> {2}",  syncOperation.GetType().Name,
                                     item.SourcePath, item.TargetPath);
        }

    }
}
