// -----------------------------------------------------------------------
// <copyright file="ISyncJob.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ISyncJob.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DataSync.Lib.Log;
using DataSync.Lib.Sync.Jobs;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISyncJob
    {
       
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        JobStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        ILog Logger { get; set; }

        /// <summary>
        /// Occurs when a job status changed.
        /// </summary>
        event EventHandler<JobStatusChangedEventArgs> JobStatusChanged;

        /// <summary>
        /// Runs this instance.
        /// </summary>
        void Run();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        string ToString(List<int> columns);
    }
}