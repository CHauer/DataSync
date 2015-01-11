using System;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Log
{
    public class Logger : ILog
    {
        public event EventHandler MessageLogged;

        public void AddLogMessage(LogMessage message)
        {
            throw new NotImplementedException();
        }


        public int LogMessages
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int DebugMessages
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void AddListener()
        {
            throw new NotImplementedException();
        }

        public void ClearListeners()
        {
            throw new NotImplementedException();
        }
    }
}
