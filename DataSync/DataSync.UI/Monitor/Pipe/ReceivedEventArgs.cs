// -----------------------------------------------------------------------
// <copyright file="ReceivedEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ReceivedEventArgs.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Monitor.Pipe
{
    using System;

    /// <summary>
    /// The received event args.
    /// </summary>
    /// <typeparam name="T">
    /// Generic type.
    /// </typeparam>
    public class ReceivedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ReceivedEventArgs(T message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public T Message { get; private set; }
    }
}