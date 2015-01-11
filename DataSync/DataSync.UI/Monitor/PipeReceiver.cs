using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataSync.Lib.Log.Messages;

namespace DataSync.UI.Monitor
{
    /// <summary>
    /// 
    /// </summary>
    public class PipeReceiver<T> where T : class
    {
        /// <summary>
        /// The client handle
        /// </summary>
        private string clientHandle;

        /// <summary>
        /// The pipe client
        /// </summary>
        private AnonymousPipeClientStream pipeClient;

        /// <summary>
        /// The serializer
        /// </summary>
        private BinaryFormatter serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeReceiver" /> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        public PipeReceiver(string handle)
        {
            this.clientHandle = handle;
            this.serializer = new BinaryFormatter();
            InitializePipe();
        }

        /// <summary>
        /// Occurs when log message is received.
        /// </summary>
        public event EventHandler<ReceivedEventArgs<T>> LogMessageReceived;

        /// <summary>
        /// Initializes the pipe.
        /// </summary>
        private void InitializePipe()
        {
            pipeClient = new AnonymousPipeClientStream(PipeDirection.In, clientHandle);
        }

        /// <summary>
        /// Starts the receiving process.
        /// </summary>
        public void StartReceiving()
        {
            Task.Run(() => Run());
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            T receivedMessage = null;

            while (pipeClient.IsConnected)
            {
                try
                {
                    receivedMessage = (T)serializer.Deserialize(pipeClient);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                if (LogMessageReceived != null && receivedMessage != null)
                {
                    LogMessageReceived(this, new ReceivedEventArgs<T>(receivedMessage));
                }
            }
        }

        /// <summary>
        /// Stops the receiving.
        /// </summary>
        public void StopReceiving()
        {
            pipeClient.Close();
        }
    }
}
