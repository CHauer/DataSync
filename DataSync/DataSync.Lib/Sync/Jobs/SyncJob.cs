using System;
using DataSync.Lib.Log;

namespace DataSync.Lib.Sync.Jobs
{
    public class SyncJob : ISyncJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncJob" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="operation">The operation.</param>
        public SyncJob(ISyncItem item, ISyncOperation operation)
        {
            this.Item = item;
            this.Operation = operation;
        }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public ISyncItem Item { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public JobStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public ISyncOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public virtual void Run()
        {
            if (Operation == null || Item == null)
            {
                throw new InvalidOperationException("Invalid Sync Job Parameters!");
            }

            Operation.Execute(Item);
        }

    }
}
