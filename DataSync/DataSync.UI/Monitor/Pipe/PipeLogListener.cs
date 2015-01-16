using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;

namespace DataSync.UI.Monitor.Pipe
{
    public class PipeLogListener : ILogListener
    {
        /// <summary>
        /// The pipe sender
        /// </summary>
        private PipeSender<LogMessage> pipeSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeLogListener"/> class.
        /// </summary>
        public PipeLogListener()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the pipe.
        /// </summary>
        private void Initialize()
        {
            pipeSender = new PipeSender<LogMessage>(MonitorType.Log.ToString("g"));
        }

        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteLogMessage(LogMessage message)
        {
            try
            {
                pipeSender.SendMessage(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }
}
