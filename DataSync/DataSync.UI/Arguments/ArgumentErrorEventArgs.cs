// -----------------------------------------------------------------------
// <copyright file="ArgumentErrorEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ArgumentErrorEventArgs.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Arguments
{
    using System;

    /// <summary>
    /// The argument error event args.
    /// </summary>
    public class ArgumentErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentErrorEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ArgumentErrorEventArgs(string message)
        {
            this.ErrorMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentErrorEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="ex">
        /// The exception.
        /// </param>
        public ArgumentErrorEventArgs(string message, Exception ex)
        {
            this.Exception = ex;
            this.ErrorMessage = message;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.Exception != null)
            {
                return string.Format("{0}\nDetails: {1}", this.ErrorMessage, this.Exception.Message);
            }

            return this.ErrorMessage;
        }
    }
}