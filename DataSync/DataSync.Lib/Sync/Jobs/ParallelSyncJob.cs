// -----------------------------------------------------------------------
// <copyright file="ParallelSyncJob.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ParallelSyncJob.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Sync.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    public class ParallelSyncJob : ISyncJob
    {
        /// <summary>
        /// The status
        /// </summary>
        private JobStatus status;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelSyncJob" /> class.
        /// </summary>
        /// <param name="parallelJobs">The parallel jobs.</param>
        /// <exception cref="System.ArgumentNullException">parallelJobs</exception>
        /// <exception cref="System.InvalidOperationException">One parallel SyncJob is invalid - SyncItem or Operation is null!</exception>
        public ParallelSyncJob(Dictionary<ISyncItem, SyncOperation> parallelJobs)
        {
            if (parallelJobs == null)
            {
                throw new ArgumentNullException("parallelJobs");
            }

            //a sync item / key is null or a sync operation / value is null
            if (parallelJobs.Any(kvp => kvp.Key == null || kvp.Value == null))
            {
                throw new InvalidOperationException("One parallel SyncJob is invalid - SyncItem or Operation is null!");
            }

            this.ParallelJobs = parallelJobs;
        }

        /// <summary>
        /// Gets the parallel jobs.
        /// </summary>
        /// <value>
        /// The parallel jobs.
        /// </value>
        public Dictionary<ISyncItem, SyncOperation> ParallelJobs { get; private set; }

        /// <summary>
        /// Gets the jobs states.
        /// </summary>
        /// <value>
        /// The jobs states.
        /// </value>
        public Dictionary<ISyncItem, JobStatus> JobsStates { get; private set; }

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
                if (this.ParallelJobs != null)
                {
                    return this.ParallelJobs.Keys.ToList();
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
        public JobStatus Status
        {
            get { return this.status; }
            set
            {
                if (this.status != value)
                {
                    this.status = value;

                    if (JobStatusChanged != null)
                    {
                        JobStatusChanged(this, new JobStatusChangedEventArgs(value));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        public List<SyncOperation> Operations
        {
            get
            {
                if (this.ParallelJobs != null)
                {
                    return this.ParallelJobs.Values.ToList();
                }

                return new List<SyncOperation>();
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Occurs when a job status changed.
        /// </summary>
        public event EventHandler<JobStatusChangedEventArgs> JobStatusChanged;

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            this.Status = JobStatus.Processing;

            this.JobsStates = new Dictionary<ISyncItem, JobStatus>();
            this.Items.ForEach(item => this.JobsStates.Add(item, JobStatus.Processing));

            Parallel.ForEach(this.ParallelJobs, keyvaluepair =>
            {
                ISyncItem item = keyvaluepair.Key;
                SyncOperation operation = keyvaluepair.Value;

                if (operation == null || item == null)
                {
                    this.Status = JobStatus.Error;
                    return;
                }

                if (operation.Execute(item))
                {
                    this.JobsStates[item] = JobStatus.Done;
                }
                else
                {
                    this.JobsStates[item] = JobStatus.Error;
                }

                //check if end of parallel operation is error - parallel job is error state
                if (this.JobsStates.Any(i => i.Value == JobStatus.Error))
                {
                    this.Status = JobStatus.Error;
                }

                //if all part operations are done - parallel job done
                if (this.JobsStates.All(i => i.Value == JobStatus.Done))
                {
                    this.Status = JobStatus.Done;
                }
            });
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(LogMessage message)
        {
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(message);
            }
        }
    }
}