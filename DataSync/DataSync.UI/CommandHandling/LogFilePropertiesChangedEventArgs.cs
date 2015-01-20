// -----------------------------------------------------------------------
// <copyright file="LogFilePropertiesChangedEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - LogFilePropertiesChangedEventArgs.cs</summary>
// -----------------------------------------------------------------------

namespace DataSync.UI.CommandHandling
{
    public class LogFilePropertiesChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogFilePropertiesChangedEventArgs"/> class.
        /// </summary>
        /// <param name="logfilename">The logfilename.</param>
        /// <param name="logfilesize">The logfilesize.</param>
        public LogFilePropertiesChangedEventArgs(string logfilename, int logfilesize)
        {
            this.LogFileSize = logfilesize;
            this.LogFileName = logfilename;
        }

        /// <summary>
        /// Gets the size of the log file.
        /// </summary>
        /// <value>
        /// The size of the log file.
        /// </value>
        public int LogFileSize { get; private set; }

        /// <summary>
        /// Gets the name of the log file.
        /// </summary>
        /// <value>
        /// The name of the log file.
        /// </value>
        public string LogFileName { get; private set; }
    }
}