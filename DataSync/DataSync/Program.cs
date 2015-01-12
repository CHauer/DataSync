// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync - Program.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using DataSync.Lib.Configuration.Data;
using DataSync.Lib.Sync;
using DataSync.Properties;
using DataSync.UI;
using DataSync.UI.CommandHandling;
using DataSync.UI.Monitor;

namespace DataSync
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The _log monitor
        /// </summary>
        private static ConsoleMonitor _logMonitor;

        /// <summary>
        /// The _queue monitor
        /// </summary>
        private static ConsoleMonitor _queueMonitor;

        /// <summary>
        /// The instruction _handler
        /// </summary>
        private static InputInstructionHandler _handler;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            SyncManager syncManagerObj;

            if (args != null && args.Length > 0)
            {
                if (args.Contains("/?") || args.Contains("/h") || args.Contains("/help"))
                {
                    Console.WriteLine(Resources.HelpConsole);
                    Console.ReadLine();
                    return;
                }

                ArgumentConfigurationCreator creator = new ArgumentConfigurationCreator(args);
                creator.ErrorOccured += ArgumentCreator_ErrorOccuredHandler;

                syncManagerObj = new SyncManager(creator);
            }
            else
            {
                XmlConfigurationSerializer manager = new XmlConfigurationSerializer()
                {
                    ConfigurationFile = Resources.ConfigurationFile
                };

                syncManagerObj = new SyncManager(manager, manager);
            }

            //React to current process end
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            //TODO Handle unhandled exceptions - prevent close if possible
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //Start Instruction Decoder
            _handler = new InputInstructionHandler(Console.In, Console.Out);
            _handler.BeforeErrorOutput += (sender, e) => { Console.ForegroundColor = ConsoleColor.Red; };
            _handler.AfterErrorOutput += (sender, e) => { Console.ResetColor(); };

            _handler.StartHandler();

            //Start LogMonitor, Queue Monitor
            StartMonitors();

            Console.ReadLine();
            CloseMonitors();
        }

        /// <summary>
        /// Handles the ProcessExit event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _handler.StopHandler();
            CloseMonitors();
        }

        /// <summary>
        /// Starts the monitors.
        /// </summary>
        private static void StartMonitors()
        {
            _logMonitor = new ConsoleMonitor(MonitorType.Log);
            _logMonitor.Start();

            _queueMonitor = new ConsoleMonitor(MonitorType.Screen);
            _queueMonitor.Start();
        }

        /// <summary>
        /// Closes the monitors.
        /// </summary>
        private static void CloseMonitors()
        {
            _logMonitor.Stop();
            _queueMonitor.Stop();
        }

        /// <summary>
        /// Handles the ErrorOccuredHandler event of the ArgumentCreator.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ArgumentErrorEventArgs"/> instance containing the event data.</param>
        private static void ArgumentCreator_ErrorOccuredHandler(object sender, ArgumentErrorEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
            Console.WriteLine(Resources.Program_Main_EnterForEXIT);
            Console.ReadLine();
        }
    }
}