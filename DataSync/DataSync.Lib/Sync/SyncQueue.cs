﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSync.Lib.Log;
using DataSync.Lib.Sync.Jobs;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncQueue
    {
        /// <summary>
        /// The is running
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The job queue
        /// </summary>
        private Queue<ISyncJob> jobQueue;

        /// <summary>
        /// The job handler task
        /// </summary>
        private Task jobTask;

        /// <summary>
        /// The job task canceleler
        /// </summary>
        private CancellationTokenSource jobTaskCanceler;

        /// <summary>
        /// The current job
        /// </summary>
        private ISyncJob currentJob;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncQueue"/> class.
        /// </summary>
        public SyncQueue()
        {
            jobQueue = new Queue<ISyncJob>();
        }

        /// <summary>
        /// Gets the jobs.
        /// </summary>
        /// <value>
        /// The jobs.
        /// </value>
        public List<ISyncJob> Jobs
        {
            get
            {
                return jobQueue.ToList();
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
                return currentJob;
            }
        }

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
                return jobQueue.Count;
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
        /// <param name="job">The job.</param>
        public void Enqueue(ISyncJob job)
        {
            job.Status = JobStatus.Queued;
            job.Logger = Logger;

            jobQueue.Enqueue(job);
        }

        /// <summary>
        /// Starts the queue.
        /// </summary>
        public void StartQueue()
        {
            isRunning = true;

            jobTaskCanceler = new CancellationTokenSource();

            jobTask = Task.Run(() => RunHandleQueueJobs(), jobTaskCanceler.Token);
        }

        /// <summary>
        /// Stops the queue.
        /// </summary>
        public void StopQueue()
        {
            isRunning = false;

            jobTaskCanceler.Cancel();
        }

        /// <summary>
        /// Runs the handle queue jobs.
        /// </summary>
        private void RunHandleQueueJobs()
        {
            while (isRunning)
            {
                while (jobQueue.Count == 0)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 200));

                    if (jobTaskCanceler.Token.IsCancellationRequested)
                    {
                        return;
                    }
                }

                currentJob = jobQueue.Dequeue();

                currentJob.Run();
            }
        }
    }
}
