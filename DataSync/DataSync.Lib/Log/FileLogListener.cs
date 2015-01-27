// -----------------------------------------------------------------------
// <copyright file="FileLogListener.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - FileLogListener.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Log
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using DataSync.Lib.Log.Messages;

    /// <summary>
    /// The file log listener class.
    /// </summary>
    public class FileLogListener : ILogListener
    {
        /// <summary>
        /// The log writer instance.
        /// </summary>
        private TextWriter logWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener"/> class.
        /// </summary>
        /// <param name="logFilePath">
        /// The log file path.
        /// </param>
        /// <param name="logFileSize">
        /// Size of the log file.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// LogFileSize
        /// or
        /// logFilePath.
        /// </exception>
        public FileLogListener(string logFilePath, int logFileSize)
        {
            if (logFileSize <= 0)
            {
                throw new ArgumentException("logFileSize");
            }

            if (string.IsNullOrEmpty(logFilePath))
            {
                throw new ArgumentException("logFilePath");
            }

            this.LogFilePath = logFilePath;
            this.LogFileSize = logFileSize;
        }

        /// <summary>
        /// Gets the log file path.
        /// </summary>
        /// <value>
        /// The log file path.
        /// </value>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Gets the size of the log file.
        /// </summary>
        /// <value>
        /// The size of the log file.
        /// </value>
        public int LogFileSize { get; private set; }

        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void WriteLogMessage(LogMessage message)
        {
            try
            {
                this.OpenLogFile();

                this.logWriter.WriteLine(message.ToString());

                this.CloseLogFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            this.CheckBackupFile();
        }

        /// <summary>
        /// Checks the backup file.
        /// </summary>
        /// <returns>
        /// The status of check backup file.
        /// </returns>
        private bool CheckBackupFile()
        {
            FileInfo info = null;
            string bakPath;

            try
            {
                info = new FileInfo(this.LogFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                info = null;
            }

            if (info != null && info.Length > this.LogFileSize)
            {
                bakPath = info.FullName + ".bak";

                if (File.Exists(bakPath))
                {
                    try
                    {
                        File.Delete(bakPath);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                try
                {
                    File.Move(info.FullName, bakPath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Closes the log file.
        /// </summary>
        private void CloseLogFile()
        {
            this.logWriter.Flush();
            this.logWriter.Close();
        }

        /// <summary>
        /// Initializes the log file.
        /// </summary>
        private void OpenLogFile()
        {
            try
            {
                this.logWriter = new StreamWriter(File.Open(this.LogFilePath, FileMode.Append, FileAccess.Write));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}