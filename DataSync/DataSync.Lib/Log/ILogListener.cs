// -----------------------------------------------------------------------
// <copyright file="ILogListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ILogListener.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Log
{
    using DataSync.Lib.Log.Messages;

    /// <summary>
    /// The log listener interface.
    /// </summary>
    public interface ILogListener
    {
        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void WriteLogMessage(LogMessage message);
    }
}