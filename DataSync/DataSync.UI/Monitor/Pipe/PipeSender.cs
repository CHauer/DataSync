using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DataSync.UI.Monitor.Pipe
{
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PipeSender<T> where T : class
    {
        /// <summary>
        /// The serializer
        /// </summary>
        private BinaryFormatter serializer;
        /// <summary>
        /// The send message queue
        /// </summary>
        private Queue<T> sendMessageQueue;

        /// <summary>
        /// The is running
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeReceiver{T}" /> class.
        /// </summary>
        /// <param name="pipename">The pipename.</param>
        public PipeSender(string pipename)
        {
            this.serializer = new BinaryFormatter();
            this.PipeName = pipename;
            sendMessageQueue = new Queue<T>();
            isRunning = true;

            Task.Run(() => RunMessageSender());
        }

        /// <summary>
        /// Runs the message sender.
        /// </summary>
        private void RunMessageSender()
        {
            pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);

            while (isRunning)
            {
                while (sendMessageQueue.Count == 0)
                {
                    Thread.Sleep(100);
                }

                var message = sendMessageQueue.Dequeue();

                if (!pipeClient.IsConnected)
                {
                    try
                    {
                        pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                        pipeClient.Connect(10000);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                try
                {
                    serializer.Serialize(pipeClient, message);
                    pipeClient.Flush();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            pipeClient.Close();
        }

        /// <summary>
        /// Gets the client handle.
        /// </summary>
        /// <value>
        /// The client handle.
        /// </value>
        public String PipeName { get; private set; }

        /// <summary>
        /// The pipe client
        /// </summary>
        private NamedPipeClientStream pipeClient;

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void SendMessage(T message)
        {
            sendMessageQueue.Enqueue(message);
        }
    }
}
