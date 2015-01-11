using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncQueue : Queue<ISyncJob>
    {
        /// <summary>
        /// The is running
        /// </summary>
        private bool isRunning;

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
                return this.ToList();
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
                while (Count == 0)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 200));

                    if (jobTaskCanceler.Token.IsCancellationRequested)
                    {
                        return;
                    }
                }

                currentJob = Dequeue();

                currentJob.Run();
            }
        }
    }
}
