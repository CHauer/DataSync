// -----------------------------------------------------------------------
// <copyright file="SyncJob.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncJob.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using DataSync.Lib.Log;
    using DataSync.Lib.Log.Messages;

    /// <summary>
    /// The sync job.
    /// </summary>
    public class SyncJob : ISyncJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncJob"/> class.
        /// </summary>
        /// <param name="item">
        /// The item parameter.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        public SyncJob(ISyncItem item, SyncOperation operation)
        {
            this.Item = item;
            this.Operation = operation;
        }

        /// <summary>
        /// Occurs when a job status changed.
        /// </summary>
        public event EventHandler JobStatusChanged;

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item value.
        /// </value>
        public ISyncItem Item { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger value.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation value.
        /// </value>
        public SyncOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status value.
        /// </value>
        public JobStatus Status { get; set; }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public virtual void Run()
        {
            if (this.Operation == null || this.Item == null)
            {
                this.LogMessage(new ErrorLogMessage("Invalid Sync Job Parameters!"));
                this.Status = JobStatus.Error;
                this.OnJobStatusChanged();
                return;
            }

            this.Status = JobStatus.Processing;
            this.OnJobStatusChanged();
            this.LogMessage(new SyncJobLogMessage("SyncJob processing.", this));

            this.LogMessage(new SyncOperationLogMessage(this.Operation, this.Item));

            if (this.Logger != null)
            {
                this.Operation.Logger = this.Logger;
            }

            if (this.Operation.Execute(this.Item))
            {
                this.Status = JobStatus.Processing;
                this.OnJobStatusChanged();
                this.LogMessage(new SyncJobLogMessage("SyncJob ended.", this));
            }
            else
            {
                this.Status = JobStatus.Error;
                this.OnJobStatusChanged();
                this.LogMessage(new SyncJobLogMessage("SyncJob is in error state.", this));
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="columns">
        /// The columns.
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(List<int> columns)
        {
            string source = this.ShortenFolderPath(Path.GetDirectoryName(this.Item.SourcePath), columns[0]);
            string target = this.ShortenFolderPath(Path.GetDirectoryName(this.Item.TargetPath), columns[1]);
            string file = this.ShortenFolderPath(Path.GetFileName(this.Item.SourcePath), columns[2]);
            string operation = this.Operation.GetType().Name;
            string jostatus = this.Status == JobStatus.Processing ? "Progress" : this.Status.ToString("g");

            return string.Format(
                "{0} {1} {2} {3} {4}", 
                source.PadRight(columns[0]), 
                target.PadRight(columns[1]), 
                file.PadRight(columns[2]), 
                operation.PadRight(columns[3]), 
                jostatus.PadRight(columns[4]));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Source:{0}\nTarget:{1}\nOperation:{2} ", 
                this.Item.SourcePath, 
                this.Item.TargetPath, 
                this.Operation.GetType().Name);
        }

        /// <summary>
        /// Raises the <see cref="E:JobStatusChanged"/> event.
        /// </summary>
        protected virtual void OnJobStatusChanged()
        {
            // ReSharper disable once UseNullPropagation
            if (this.JobStatusChanged != null)
            {
                this.JobStatusChanged(this, EventArgs.Empty);
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

        /// <summary>
        /// Shortens the folder path.
        /// </summary>
        /// <param name="path">The path parameter.</param>
        /// <param name="length">The length parameter.</param>
        /// <returns>
        /// The type <see cref="string" /> folder path.
        /// </returns>
        private string ShortenFolderPath(string path, int length)
        {
            if (path.Length <= length)
            {
                return path;
            }

            int value = (length / 2) - 1;

            return string.Format(@"{0}\..\{1}", path.Substring(0, value), path.Substring(path.Length - value, value));
        }
    }
}