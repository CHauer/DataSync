// -----------------------------------------------------------------------
// <copyright file="SyncJob.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncJob.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
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
            LogMessage(new SyncJobLogMessage("SyncJob processing.", this));

            LogMessage(new SyncOperationLogMessage(this.Operation, this.Item));

            if (this.Operation.Execute(this.Item))
            {
                this.Status = JobStatus.Done;
                LogMessage(new SyncJobLogMessage("SyncJob ended.", this));
            }
            else
            {
                this.Status = JobStatus.Error;
                LogMessage(new SyncJobLogMessage("SyncJob is in error state.", this));
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(List<int> columns)
        {
            string source = ShortenFolderPath(Path.GetDirectoryName(Item.SourcePath), columns[0]);
            string target = ShortenFolderPath(Path.GetDirectoryName(Item.TargetPath), columns[1]);
            string file = ShortenFolderPath(Path.GetFileName(Item.SourcePath), columns[2]);
            string operation = Operation.GetType().Name;
            string jostatus = this.Status == JobStatus.Processing ? "Progress" : this.status.ToString("g");

            return String.Format("{0} {1} {2} {3} {4}",
                source.PadRight(columns[0]), target.PadRight(columns[1]), file.PadRight(columns[2]),
                 operation.PadRight(columns[3]), jostatus.PadRight(columns[4]));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} -> {1} - Operation: {2} ",
                                    Item.SourcePath, Item.TargetPath, 
                                    Operation.GetType().Name);
        }

        /// <summary>
        /// Shortens the folder path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private string ShortenFolderPath(string path, int length)
        {
            if (path.Length <= length)
            {
                return path;
            }

            int value = (length / 2) - 1;

            return String.Format(@"{0}\..\{1}", path.Substring(0, value), path.Substring(path.Length - value, value));
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