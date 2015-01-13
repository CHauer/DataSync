using System;
using System.Linq;
using System.Collections.Generic;
using DataSync.Lib.Log;

namespace DataSync.Lib.Sync.Jobs
{
    public class ParallelSyncJob : ISyncJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelSyncJob" /> class.
        /// </summary>
        /// <param name="parallelJobs">The parallel jobs.</param>
        public ParallelSyncJob(Dictionary<ISyncItem, ISyncOperation> parallelJobs)
        {
            this.ParallelJobs = parallelJobs;
        }

        /// <summary>
        /// Gets the parallel jobs.
        /// </summary>
        /// <value>
        /// The parallel jobs.
        /// </value>
        public Dictionary<ISyncItem, ISyncOperation> ParallelJobs { get; private set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<ISyncItem> Items
        {
            get
            {
                if (ParallelJobs != null)
                {
                    return ParallelJobs.Keys.ToList();
                }
                return new List<ISyncItem>();
            }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public JobStatus Status { get; set; }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        public List<ISyncOperation> Operations
        {
            get
            {
                if (ParallelJobs != null)
                {
                    return ParallelJobs.Values.ToList();
                }

                return new List<ISyncOperation>();
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        public void Run()
        {
            throw new NotImplementedException();
        }


        
    }
}
