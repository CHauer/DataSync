// -----------------------------------------------------------------------
// <copyright file="JobStatusChangedEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - JobStatusChangedEventArgs.cs</summary>
// -----------------------------------------------------------------------

using System;

namespace DataSync.Lib.Sync.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    public class JobStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobStatusChangedEventArgs"/> class.
        /// </summary>
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