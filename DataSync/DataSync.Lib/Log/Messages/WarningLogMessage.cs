// -----------------------------------------------------------------------
// <copyright file="WarningLogMessage.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - WarningLogMessage.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Log.Messages
{
    using System;

    /// <summary>
    /// The warning log message.
    /// </summary>
    [Serializable]
    public class WarningLogMessage : LogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarningLogMessage"/> class. 
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public WarningLogMessage(string message)
            : base(message, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarningLogMessage"/> class. 
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="isDebug">
        /// If set to <c>true</c> [is debug].
        /// </param>
        public WarningLogMessage(string message, bool isDebug)
            : base(message, isDebug)
        {
        }

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
                return string.Format("{0:G} - Warning: {1}\nDEBUG:{2}", this.Date, this.Message, this.StackTrace);
            }

            return string.Format("{0:G} - Warning: {1}", this.Date, this.Message);
        }
    }
}