// -----------------------------------------------------------------------
// <copyright file="PipeSender.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - PipeSender.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Monitor.Pipe
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.Pipes;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The pipe sender class.
    /// </summary>
    /// <typeparam name="T">
    /// Generic type class.
    /// </typeparam>
    public class PipeSender<T>
        where T : class
    {
        /// <summary>
        /// The is running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The pipe client.
        /// </summary>
        private NamedPipeClientStream pipeClient;

        /// <summary>
        /// The send message queue.
        /// </summary>
        private Queue<T> sendMessageQueue;

        /// <summary>
        /// The serializer.
        /// </summary>
        private BinaryFormatter serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeSender{T}"/> class. 
        /// Initializes a new instance of the <see cref="PipeReceiver{T}"/> class.
        /// </summary>
        /// <param name="pipename">
        /// The pipe name.
        /// </param>
        public PipeSender(string pipename)
        {
            this.serializer = new BinaryFormatter();
            this.PipeName = pipename;
            this.sendMessageQueue = new Queue<T>();
            this.isRunning = true;

            Task.Run(() => this.RunMessageSender());
        }

        /// <summary>
        /// Gets the client handle.
        /// </summary>
        /// <value>
        /// The client handle.
        /// </value>
        public string PipeName { get; private set; }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void SendMessage(T message)
        {
            this.sendMessageQueue.Enqueue(message);
        }

        /// <summary>
        /// Runs the message sender.
        /// </summary>
        private void RunMessageSender()
        {
            this.pipeClient = new NamedPipeClientStream(".", this.PipeName, PipeDirection.Out);

            while (this.isRunning)
            {
                while (this.sendMessageQueue.Count == 0)
                {
                    Thread.Sleep(100);
                }

                var message = this.sendMessageQueue.Dequeue();

                if (!this.pipeClient.IsConnected)
                {
                    try
                    {
                        this.pipeClient = new NamedPipeClientStream(".", this.PipeName, PipeDirection.Out);
                        this.pipeClient.Connect(10000);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                try
                {
                    this.serializer.Serialize(this.pipeClient, message);
                    this.pipeClient.Flush();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            this.pipeClient.Close();
        }
    }
}