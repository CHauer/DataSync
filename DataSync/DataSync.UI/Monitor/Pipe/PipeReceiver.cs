// -----------------------------------------------------------------------
// <copyright file="PipeReceiver.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - PipeReceiver.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Monitor.Pipe
{
    using System;
    using System.Diagnostics;
    using System.IO.Pipes;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

    /// <summary>
    /// The pipe receiver class.
    /// </summary>
    /// <typeparam name="T">
    /// Generic type class.
    /// </typeparam>
    public class PipeReceiver<T>
        where T : class
    {
        /// <summary>
        /// The is running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The serializer.
        /// </summary>
        private BinaryFormatter serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeReceiver{T}"/> class.
        /// </summary>
        /// <param name="pipeName">
        /// Name of the pipe.
        /// </param>
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
        /// Starts the receiving process.
        /// </summary>
        public void StartReceiving()
        {
            this.isRunning = true;
            Task.Run(() => this.Run());
        }

        /// <summary>
        /// Stops the receiving.
        /// </summary>
        public void StopReceiving()
        {
            this.isRunning = false;
        }

        /// <summary>
        /// Initializes the pipe.
        /// </summary>
        private void InitializePipe()
        {
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            var pipeServer = new NamedPipeServerStream(this.PipeName, PipeDirection.In);
            T receivedMessage = null;

            while (this.isRunning)
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
                        receivedMessage = (T)this.serializer.Deserialize(pipeServer);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    if (this.MessageReceived != null && receivedMessage != null)
                    {
                        this.MessageReceived(this, new ReceivedEventArgs<T>(receivedMessage));
                    }
                }
            }

            pipeServer.Close();
        }
    }
}