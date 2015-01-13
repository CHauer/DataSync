using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.Lib.Log.Messages
{
    public class LogMessage
    {
        /// <summary>
        /// The stack trace
        /// </summary>
        private string stackTrace;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        public LogMessage() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LogMessage(string message) : this(message, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        public LogMessage(string message, bool isDebug)
        {
            Message = message;
            IsDebug = isDebug;
            Date = DateTime.Now;

            if (IsDebug)
            {
                stackTrace = GetStackTrance();
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is debug.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is debug; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebug { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets the stack trance.
        /// </summary>
        /// <returns></returns>
        private string GetStackTrance()
        {
            string[] lines = Environment.StackTrace.Split(new [] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return String.Join(Environment.NewLine, lines.Skip(3));
        }

    }
}
