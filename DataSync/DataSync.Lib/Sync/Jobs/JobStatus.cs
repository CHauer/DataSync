// -----------------------------------------------------------------------
// <copyright file="JobStatus.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - JobStatus.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync.Jobs
{
    /// <summary>
    /// The Job Status enumeration.
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// The queued state.
        /// </summary>
        Queued, 

        /// <summary>
        /// The processing state.
        /// </summary>
        Processing, 

        /// <summary>
        /// The done state.
        /// </summary>
        Done, 

        /// <summary>
        /// The error state.
        /// </summary>
        Error
    }
}