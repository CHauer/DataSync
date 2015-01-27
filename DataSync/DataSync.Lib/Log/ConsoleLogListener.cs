// -----------------------------------------------------------------------
// <copyright file="ConsoleLogListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ConsoleLogListener.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Log
{
    using System;
    using System.Diagnostics;
    using System.Text;

    using DataSync.Lib.Log.Messages;

    /// <summary>
    /// The Console log listener class.
    /// </summary>
    public class ConsoleLogListener : ILogListener
    {
        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void WriteLogMessage(LogMessage message)
        {
            if (message.IsDebug && !Debugger.IsAttached)
            {
                // donst display Debug Message 
                return;
            }

            if (message is ErrorLogMessage)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (message is WarningLogMessage)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else if (message is SyncOperationLogMessage)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }

            Console.WriteLine(message.ToString());
            Console.WriteLine(this.CreateLine(99, '-'));
            Console.ResetColor();
        }

        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CreateLine(int length, char character)
        {
            StringBuilder builder = new StringBuilder();

            while (length > 0)
            {
                builder.Append(character);
                length--;
            }

            return builder.ToString();
        }
    }
}