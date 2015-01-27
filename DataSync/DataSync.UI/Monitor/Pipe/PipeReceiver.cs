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

namespace DataSync.UI.Monitor.Pipe
{
    /// <summary>
    /// 
    /// </summary>
    public class PipeReceiver<T> where T : class
    {

        /// <summary>
        /// The is running
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The serializer
        /// </summary>
        private BinaryFormatter serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeReceiver{T}" /> class.
        /// </summary>
        /// <param name="pipeName">Name of the pipe.</param>
        public PipeReceiver(string pipeName)
        {
            this.PipeName = pipeName;
            this.serializer = new BinaryFormatter();
        }

        /// <summary>
        /// Occurs when log message is received.
        /// </summary>
        public event EventHandler<ReceivedEventArgs<T>> MessageReceived;

        /// <summary>
        /// Gets the name of the pipe.
        /// </summary>
        /// <value>
        /// The name of the pipe.
        /// </value>
        public string PipeName { get; private set; }

        /// <summary>
        /// Initializes the pipe.
        /// </summary>
        private void InitializePipe()
        {

        }

        /// <summary>
        /// Starts the receiving process.
        /// </summary>
        public void StartReceiving()
        {
            isRunning = true;
            Task.Run(() => Run());
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.In);
            T receivedMessage = null;

            while (isRunning)
            {
                try
                {
                    pipeServer.WaitForConnection();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                while (pipeServer.IsConnected)
                {
                    try
                    {
                        receivedMessage = (T)serializer.Deserialize(pipeServer);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }


                    if (MessageReceived != null && receivedMessage != null)
                    {
                        MessageReceived(this, new ReceivedEventArgs<T>(receivedMessage));
                    }
                }
            }

            pipeServer.Close();
        }

        /// <summary>
        /// Stops the receiving.
        /// </summary>
        public void StopReceiving()
        {
            isRunning = false;
        }
    }
}
