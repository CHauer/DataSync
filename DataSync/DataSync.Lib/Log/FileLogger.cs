using System;
using System.Diagnostics;
using System.IO;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Log
{
    /// <summary>
    /// 
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
        /// <param name="logFilePath">The log file path.</param>
        /// <param name="logFileSize">Size of the log file.</param>
        /// <exception cref="System.ArgumentException">
        /// logFileSize
        /// or
        /// logFilePath
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
        /// Initializes the log file.
        /// </summary>
        private void OpenLogFile()
        {
            try
            {
                logWriter = new StreamWriter(File.Open(LogFilePath, FileMode.Append, FileAccess.Write));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
        public int LogFileSize{ get; private set; }

        /// <summary>
        /// Writes the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteLogMessage(LogMessage message)
        {
            try
            {
                OpenLogFile();

                logWriter.WriteLine(message.ToString());

                CloseLogFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            CheckBackupFile();
        }

        /// <summary>
        /// Closes the log file.
        /// </summary>
        private void CloseLogFile()
        {
            logWriter.Flush();
            logWriter.Close();
        }

        /// <summary>
        /// Checks the backup file.
        /// </summary>
        /// <returns></returns>
        private bool CheckBackupFile()
        {
            FileInfo info = null;
            string bakPath;

            try
            {
                 info = new FileInfo(LogFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                info = null;
            }

            if (info != null && info.Length > LogFileSize)
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
    }
}
