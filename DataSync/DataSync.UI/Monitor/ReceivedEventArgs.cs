using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSync.Lib.Log.Messages;

namespace DataSync.UI.Monitor
{
    public class ReceivedEventArgs<T> : EventArgs
    {
        public ReceivedEventArgs(T message)
        {
            this.Message = message;
        }

        public T Message { get; private set; }
    }
}
