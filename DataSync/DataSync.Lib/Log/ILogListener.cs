using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Log
{
    public interface ILogListener
    {
        void WriteLogMessage(LogMessage message);
    }
}
