// -----------------------------------------------------------------------
// <copyright file="SyncQueue.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncQueue.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using DataSync.Lib.Log;
    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync.Jobs;

    /// <summary>
    /// The Sync Queue class.
    /// </summary>
    public class SyncQueue
    {
        /// <summary>
        /// The current job.
        /// </summary>
        private ISyncJob currentJob;

        /// <summary>
        /// The is running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The job queue.
        /// </summary>
        private Queue<ISyncJob> jobQueue;

        /// <summary>
        /// The job handler task.
        /// </summary>
        private Task jobTask;

        /// <summary>
        /// The job task canceler.
        /// </summary>
        private CancellationTokenSource jobTaskCanceler;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncQueue"/> class.
        /// </summary>
        public SyncQueue()
        {
            this.isRunning = false;
            this.jobQueue = new Queue<ISyncJob>();
        }

        /// <summary>
        /// Occurs when the queue gets updated.
        /// </summary>
        public event EventHandler QueueUpdated;

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get
            {
                return this.jobQueue.Count;
            }
        }

        /// <summary>
        /// Gets the current job.
        /// </summary>
        /// <value>
        /// The current job.
        /// </value>
        public ISyncJob CurrentJob
        {
            get
            {
                return this.currentJob;
            }
        }

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        /// <value>
        /// The jobs value.
        /// </value>
        public List<ISyncJob> Jobs
        {
            get
            {
                return this.jobQueue.ToList();
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
        /// Enqueues the specified job.
        /// </summary>
        /// <param name="job">
        /// The job parameter.
        /// </param>
        public void Enqueue(ISyncJob job)
        {
            job.JobStatusChanged += (sender, e) => { this.OnQueueUpdated(); };
            job.Logger = this.Logger;

            job.Status = JobStatus.Queued;

            this.LogMessage(new SyncJobLogMessage("SyncJob enqueued.", job));
            this.jobQueue.Enqueue(job);
        }

        /// <summary>
        /// Starts the queue.
        /// </summary>
        public void StartQueue()
        {
            if (this.isRunning)
            {
                throw new InvalidOperationException("Queue already running!");
            }

            this.isRunning = true;

            this.jobTaskCanceler = new CancellationTokenSource();

            this.jobTask = Task.Run(() => this.RunHandleQueueJobs(), this.jobTaskCanceler.Token);
        }

        /// <summary>
        /// Stops the queue.
        /// </summary>
        public void StopQueue()
        {
            this.isRunning = false;

            this.jobTaskCanceler.Cancel();
        }

        /// <summary>
        /// Called when the queue gets updated.
        /// </summary>
        protected virtual void OnQueueUpdated()
        {
            // ReSharper disable once UseNullPropagation
            if (this.QueueUpdated != null)
            {
                this.QueueUpdated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Runs the handle queue jobs.
        /// </summary>
        private void RunHandleQueueJobs()
        {
            while (this.isRunning)
            {
                while (this.jobQueue.Count == 0)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 200));

                    if (this.jobTaskCanceler.Token.IsCancellationRequested)
                    {
                        return;
                    }
                }

                this.currentJob = this.jobQueue.Dequeue();

                this.currentJob.Run();
            }
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void LogMessage(LogMessage message)
        {
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(message);
            }
        }
    }
}