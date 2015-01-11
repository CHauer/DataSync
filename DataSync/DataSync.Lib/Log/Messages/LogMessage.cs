using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.Lib.Log.Messages
{
    public class LogMessage
    {
        public LogMessage(): this(null) { }

        public LogMessage(string message) : this(message, false) { }

        public LogMessage(string message, bool isDebug)
        {
            Message = message;
            IsDebug = isDebug;
            Date = DateTime.Now;
        }

        public string Message { get; set; }

        public bool IsDebug { get; set; }

        public DateTime Date { get; set; }
    }
}
