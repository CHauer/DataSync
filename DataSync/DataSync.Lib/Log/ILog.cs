// -----------------------------------------------------------------------
// <copyright file="ILog.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ILog.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Log
{
    using DataSync.Lib.Log.Messages;

    /// <summary>
    /// The Log interface.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void AddLogMessage(LogMessage message);
    }
}