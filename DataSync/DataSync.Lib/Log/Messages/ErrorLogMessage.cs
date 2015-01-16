// -----------------------------------------------------------------------
// <copyright file="ErrorLogMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ErrorLogMessage.cs</summary>
// -----------------------------------------------------------------------

using System;

namespace DataSync.Lib.Log.Messages
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ErrorLogMessage : LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ErrorLogMessage(string message) : this(message, false, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogMessage"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public ErrorLogMessage(Exception ex) : this(null, false, ex) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public ErrorLogMessage(string message, Exception ex) : this(message, false, ex) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        public ErrorLogMessage(string message, bool isDebug) : this(message, isDebug, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogMessage"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        public ErrorLogMessage(Exception ex, bool isDebug) : this(null, isDebug, ex) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <param name="ex">The ex.</param>
        public ErrorLogMessage(string message, bool isDebug, Exception ex)
            : base(message, isDebug)
        {
            this.Exception = ex;
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string printMessage;

            if (String.IsNullOrEmpty(this.Message))
            {
                if (this.Exception != null)
                {
                    printMessage = this.Exception.Message;
                }
                else
                {
                    printMessage = "Unknown Error.";
                }
            }
            else
            {
                if (this.Exception != null)
                {
                    printMessage = string.Format("{0}\nDetails: {1}", this.Message, this.Exception.Message);
                }
                else
                {
                    printMessage = this.Message;
                }
            }

            if (this.IsDebug)
            {
                string exName = string.Empty;

                if (this.Exception != null)
                {
                    exName = this.Exception.GetType().Name;
                }

                return string.Format("{0:G} - Error: {1}\nDEBUG: {2} {3}", this.Date, printMessage, exName,
                    this.StackTrace);
            }

            return string.Format("{0:G} - Error: {1}", this.Date, printMessage);
        }
    }
}