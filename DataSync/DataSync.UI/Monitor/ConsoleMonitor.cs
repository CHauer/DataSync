// -----------------------------------------------------------------------
// <copyright file="ConsoleMonitor.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ConsoleMonitor.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Monitor
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The console monitor.
    /// </summary>
    public class ConsoleMonitor
    {
        /// <summary>
        /// The monitor type.
        /// </summary>
        private MonitorType monitorType;

        /// <summary>
        /// The process log monitor.
        /// </summary>
        private Process processMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleMonitor"/> class.
        /// </summary>
        /// <param name="type">
        /// The type parameter.
        /// </param>
        public ConsoleMonitor(MonitorType type)
        {
            this.monitorType = type;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.processMonitor = new Process
            {
                StartInfo =
                {
                    FileName = "DataSync.Monitor.exe", 
                    Arguments = string.Format("{0}", this.monitorType.ToString("g").ToUpper()), 
                    UseShellExecute = true
                }, 
            };

            this.processMonitor.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            try
            {
                this.processMonitor.CloseMainWindow();
            }
            catch (Exception ex)
            {
                try
                {
                    this.processMonitor.Kill();
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