// -----------------------------------------------------------------------
// <copyright file="JobStatusChangedEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - JobStatusChangedEventArgs.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Jobs
{
    using System;

    /// <summary>
    /// The job status changed event args class.
    /// </summary>
    public class JobStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobStatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public JobStatusChangedEventArgs(JobStatus status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public JobStatus Status { get; private set; }
    }
}