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
        /// Initializes a new instance of the <see cref="PipeReceiver{T}" /> class.
        /// </summary>
        /// <param name="pipename">The pipename.</param>
        public PipeSender(string pipename)
        {
            this.serializer = new BinaryFormatter();
            this.PipeName = pipename;
        }

        /// <summary>
        /// Gets the client handle.
        /// </summary>
        /// <value>
        /// The client handle.
        /// </value>
        public String PipeName { get; private set; }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void SendMessage(T message)
        {
            Task.Run(() =>
            {
                try
                {
                    var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);

                    pipeClient.Connect(10000);

                    serializer.Serialize(pipeClient, message);

                    pipeClient.Flush();
                    pipeClient.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
    }
}
