using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Log
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsoleLogListener : ILogListener
    {
        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteLogMessage(LogMessage message)
        {
            if(message.IsDebug && !Debugger.IsAttached)
            {
                //donst display Debug Message 
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
            Console.WriteLine(CreateLine(99, '-'));
            Console.ResetColor();
        }

        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="caracter">The caracter.</param>
        /// <returns></returns>
        private string CreateLine(int length, char caracter)
        {
            StringBuilder builder = new StringBuilder();

            while (length > 0)
            {
                builder.Append(caracter);
                length--;
            }

            return builder.ToString();
        }
    }
}

