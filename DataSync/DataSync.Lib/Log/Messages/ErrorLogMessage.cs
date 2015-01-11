using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.Lib.Log.Messages
{
    public class ErrorLogMessage : LogMessage
    {

        public ErrorLogMessage(string message) : this(message, false, null) { }

        public ErrorLogMessage(Exception ex) : this(ex.Message, false, null) { }

        public ErrorLogMessage(string message, bool isDebug) : this(message, isDebug, null) { }

        public ErrorLogMessage(Exception ex, bool isDebug) : this(ex.Message, isDebug, ex) { }

        public ErrorLogMessage(string message, bool isDebug, Exception ex)
            : base(message, isDebug)
        {
            Exception = ex;
        }

        public Exception Exception { get; set; }
    }
}
