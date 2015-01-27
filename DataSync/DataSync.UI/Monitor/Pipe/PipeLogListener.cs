// -----------------------------------------------------------------------
// <copyright file="PipeLogListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - PipeLogListener.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Monitor.Pipe
{
    using System;
    using System.Diagnostics;

    using DataSync.Lib.Log;
    using DataSync.Lib.Log.Messages;

    /// <summary>
    /// The pipe log listener.
    /// </summary>
    public class PipeLogListener : ILogListener
    {
        /// <summary>
        /// The pipe sender.
        /// </summary>
        private PipeSender<LogMessage> pipeSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeLogListener"/> class.
        /// </summary>
        public PipeLogListener()
        {
            this.Initialize();
        }

        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void WriteLogMessage(LogMessage message)
        {
            try
            {
                this.pipeSender.SendMessage(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Initializes the pipe.
        /// </summary>
        private void Initialize()
        {
            this.pipeSender = new PipeSender<LogMessage>(MonitorType.Log.ToString("g"));
        }
    }
}