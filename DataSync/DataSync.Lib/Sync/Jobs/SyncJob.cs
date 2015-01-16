// -----------------------------------------------------------------------
// <copyright file="SyncJob.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncJob.cs</summary>
// -----------------------------------------------------------------------

using System;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Sync.Jobs
{
    public class SyncJob : ISyncJob
    {
        /// <summary>
        /// The status
        /// </summary>
        private JobStatus status;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncJob" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="operation">The operation.</param>
        public SyncJob(ISyncItem item, SyncOperation operation)
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
        public JobStatus Status
        {
            get { return this.status; }
            set
            {
                if (this.status != value)
                {
                    this.status = value;

                    OnJobStatusChanged(new JobStatusChangedEventArgs(value));
                }
            }
        }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public SyncOperation Operation { get; set; }

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
        public virtual void Run()
        {
            if (this.Operation == null || this.Item == null)
            {
                LogMessage(new ErrorLogMessage("Invalid Sync Job Parameters!"));
                this.Status = JobStatus.Error;
                return;
            }

            this.Status = JobStatus.Processing;

            if (this.Operation.Execute(this.Item))
            {
                this.Status = JobStatus.Done;
            }
            else
            {
                this.Status = JobStatus.Error;
            }
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(LogMessage message)
        {
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(message);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:JobStatusChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="JobStatusChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnJobStatusChanged(JobStatusChangedEventArgs e)
        {
            // ReSharper disable once UseNullPropagation
            if (JobStatusChanged != null)
            {
                JobStatusChanged(this, e);
            }
        }
    }
}