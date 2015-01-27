using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        /// Initializes a new instance of the <see cref="ConsoleMonitor" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ConsoleMonitor(MonitorType type)
        {
            monitorType = type;
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
                    Arguments = String.Format("{0}", monitorType.ToString("g").ToUpper()),
                    UseShellExecute = true
                },
            };

            processMonitor.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            try
            {
                processMonitor.CloseMainWindow();
            }
            catch (Exception ex)
            {
                try
                {
                    processMonitor.Kill();
                }
                catch (Exception exi)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(exi.Message);
                }
            }
        }

    }
}
