// -----------------------------------------------------------------------
// <copyright file="LogMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - LogMessage.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Log.Messages
{
    using System;
    using System.Linq;

    /// <summary>
    /// The log message.
    /// </summary>
    [Serializable]
    public class LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        public LogMessage()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public LogMessage(string message)
            : this(message, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="isDebug">
        /// If set to <c>true</c> [is debug].
        /// </param>
        public LogMessage(string message, bool isDebug)
        {
            this.Message = message;
            this.IsDebug = isDebug;
            this.Date = DateTime.Now;

            if (this.IsDebug)
            {
                this.StackTrace = this.GetStackTrance();
            }
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date value.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is debug.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debug; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebug { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>
        /// The stack trace.
        /// </value>
        protected string StackTrace { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.IsDebug)
            {
                return string.Format("{0:G} - {1}\nDEBUG:{2}", this.Date, this.Message, this.StackTrace);
            }

            return string.Format("{0:G} - {1}", this.Date, this.Message);
        }

        /// <summary>
        /// Gets the stack trance.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetStackTrance()
        {
            string[] lines = Environment.StackTrace.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(Environment.NewLine, lines.Skip(3));
        }
    }
}