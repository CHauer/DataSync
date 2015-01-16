﻿// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync - Program.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using DataSync.Lib.Configuration.Data;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;
using DataSync.Lib.Sync;
using DataSync.Properties;
using DataSync.UI.Arguments;
using DataSync.UI.CommandHandling;
using DataSync.UI.CommandHandling.Instructions;
using DataSync.UI.Monitor;
using DataSync.UI.Monitor.Pipe;

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
        /// The _sync manager object
        /// </summary>
        private static SyncManager _syncManagerObj;

        /// <summary>
        /// The log listener for the local pipe to console monitor
        /// </summary>
        private static PipeLogListener _logListener;

        /// <summary>
        /// The _jobs monitor pipe
        /// </summary>
        private static PipeSender<MonitorScreen> _jobsMonitorPipe;

        /// <summary>
        /// The log instance
        /// </summary>
        private static Logger _logInstance;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            InitializeLogger();

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

                _syncManagerObj = new SyncManager(creator, _logInstance);
            }
            else
            {
                XmlConfigurationSerializer manager = new XmlConfigurationSerializer()
                {
                    ConfigurationFile = Resources.ConfigurationFile
                };

                _syncManagerObj = new SyncManager(manager, manager, _logInstance);
            }

            InitializeAppDomain();

            //Start the Initial Sync and the file watcher
            _syncManagerObj.StartSync();

            //Start Instruction Decoder
            InitializeInstructionHandler();

            //Start LogMonitor, Queue Monitor
            StartMonitors();

            //Prepare Input console
            PrepareConsoleWindow();

            //handle input on console
            _handler.RunHandler();

            //programm end
            Console.WriteLine(Resources.Program_Main_EnterForEXIT);
            Console.ReadLine();
            CloseMonitors();
        }

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        private static void InitializeLogger()
        {
            _logInstance = new Logger();

            _logListener = new PipeLogListener();

            _logInstance.AddListener(_logListener);
        }

        /// <summary>
        /// Initializes the instruction handler.
        /// </summary>
        private static void InitializeInstructionHandler()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            _handler = new InputInstructionHandler(Console.In, Console.Out);
            _handler.SyncManager = _syncManagerObj;
            _handler.HelpInstructionOccured += (sender, e) => { Console.WriteLine(Resources.HelpInstruction); };
            _handler.BeforeOutput += (sender, e) => { Console.ForegroundColor = e.Color; };
            _handler.AfterOutput += (sender, e) => { Console.ResetColor(); };
        }

        /// <summary>
        /// Prepares the console window.
        /// </summary>
        private static void PrepareConsoleWindow()
        {
            Console.Title = @"DataSync";

            ConsoleWindowPositioner positioner = new ConsoleWindowPositioner();

            positioner.BringConsoleWindowToFront();

            positioner.SetConsoleWindowPosition(50, 50);

        }

        /// <summary>
        /// Initializes the application domain.
        /// </summary>
        private static void InitializeAppDomain()
        {
            //React to current process end
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            //TODO Handle unhandled exceptions - prevent close if possible
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// Handles the ProcessExit event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CloseMonitors();
        }

        /// <summary>
        /// Starts the monitors.
        /// </summary>
        private static void StartMonitors()
        {
            _logMonitor = new ConsoleMonitor(MonitorType.Log);
            _logMonitor.Start();

            _jobsMonitorPipe = new PipeSender<MonitorScreen>(MonitorType.Screen.ToString("g"));
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
        /// <param name="e">The <see cref="ArgumentErrorEventArgs" /> instance containing the event data.</param>
        private static void ArgumentCreator_ErrorOccuredHandler(object sender, ArgumentErrorEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
            Console.WriteLine(Resources.Program_Main_EnterForEXIT);
            Console.ReadLine();

            //End application
            Environment.Exit(0);
        }
    }
}