using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DataSync.UI.Monitor
{
    public class ConsoleMonitor
    {
        /// <summary>
        /// The process log monitor
        /// </summary>
        private Process processMonitor;

        /// <summary>
        /// The monitor type
        /// </summary>
        private MonitorType monitorType;

        /// <summary>
        /// The client pipe identifier
        /// </summary>
        private string clientPipeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleMonitor"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ConsoleMonitor(MonitorType type)
        {
            monitorType = type;
            clientPipeId = "1";
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            processMonitor = new Process
            {
                StartInfo =
                {
                    FileName = "DataSync.Monitor.exe",
                    Arguments = String.Format("{0} {1}", clientPipeId, monitorType.ToString("g").ToUpper())
                },
            };

            processMonitor.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            processMonitor.CloseMainWindow();
        }

    }
}
