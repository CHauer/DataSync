using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;

namespace DataSync.UI.Monitor
{
    public class PipeLogListener : ILogListener, IDisposable
    {
        /// <summary>
        /// The pipe server
        /// </summary>
        private AnonymousPipeServerStream pipeServer;

        /// <summary>
        /// The serializer
        /// </summary>
        private BinaryFormatter serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeLogListener"/> class.
        /// </summary>
        public PipeLogListener()
        {
            InitializePipe();

            serializer = new BinaryFormatter();
        }

        /// <summary>
        /// Initializes the pipe.
        /// </summary>
        private void InitializePipe()
        {
            pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            ClientHandle = pipeServer.GetClientHandleAsString();
        }

        /// <summary>
        /// Gets the client handle.
        /// </summary>
        /// <value>
        /// The client handle.
        /// </value>
        public String ClientHandle { get; private set; }

        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteLogMessage(LogMessage message)
        {
            serializer.Serialize(pipeServer, message);
            pipeServer.Flush();
        }

        public void Dispose()
        {
            pipeServer.DisposeLocalCopyOfClientHandle();
            pipeServer.Dispose();
        }
    }
}
