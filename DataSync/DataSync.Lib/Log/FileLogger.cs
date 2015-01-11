using System;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Log
{
    public class FileLogListener : ILogListener
    {
        public int LogFilePath
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int LogFileSize
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public void WriteLogMessage(LogMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
