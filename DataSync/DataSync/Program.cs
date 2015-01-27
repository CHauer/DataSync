// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync - Program.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync
{
    using System;
    using System.Linq;
    using System.Threading;

    using DataSync.Lib.Configuration.Data;
    using DataSync.Lib.Log;
    using DataSync.Lib.Sync;
    using DataSync.Properties;
    using DataSync.UI.Arguments;
    using DataSync.UI.CommandHandling;
    using DataSync.UI.Monitor;
    using DataSync.UI.Monitor.Pipe;

    /// <summary>
    /// The program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The instruction _instructionHandler.
        /// </summary>
        private static InputInstructionHandler instructionHandler;

        /// <summary>
        /// The log listener for the local pipe to console monitor.
        /// </summary>
        private static FileLogListener logFileListener;

        /// <summary>
        /// The log instance.
        /// </summary>
        private static Logger logInstance;

        /// <summary>
        /// The log listener for the local pipe to console monitor.
        /// </summary>
        private static PipeLogListener logListener;

        /// <summary>
        /// The _log monitor.
        /// </summary>
        private static ConsoleMonitor logMonitor;

        /// <summary>
        /// The monitor screen generator.
        /// </summary>
        private static MonitorScreenGenerator monitorScreenGenerator;

        /// <summary>
        /// The _queue monitor.
        /// </summary>
        private static ConsoleMonitor queueMonitor;

        /// <summary>
        /// The _sync manager object.
        /// </summary>
        private static SyncManager syncManagerObj;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
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
                creator.ErrorOccured += ArgumentCreatorErrorOccuredHandler;

                syncManagerObj = new SyncManager(creator, logInstance);
            }
            else
            {
                XmlConfigurationSerializer manager = new XmlConfigurationSerializer()
                {
                    ConfigurationFile = Resources.ConfigurationFile
                };

                syncManagerObj = new SyncManager(manager, manager, logInstance);
            }

            if (syncManagerObj.Configuration.IsLogToFile)
            {
                logFileListener = new FileLogListener(
                    syncManagerObj.Configuration.LogFileName, 
                    syncManagerObj.Configuration.LogFileSize);
                logInstance.AddListener(logFileListener);
            }

            InitializeAppDomain();

            // initialize Instruction Decoder
            InitializeInstructionHandler();

            // Start LogMonitor, Queue Monitor
            StartMonitors();

            // Prepare Input console
            PrepareConsoleWindow();

            // Start the Initial Sync and the file watcher
            syncManagerObj.StartSync();

            // start handle input from console
            instructionHandler.RunHandler();

            // programm end
            Console.WriteLine(Resources.Program_Main_EnterForEXIT);
            Console.ReadLine();

            if (!syncManagerObj.IsSynced)
            {
                Console.WriteLine(Resources.Program_Main_SyncRunning);
                Console.ReadLine();
            }

            CloseMonitors();
        }

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        private static void InitializeLogger()
        {
            logInstance = new Logger();

            logListener = new PipeLogListener();

            logInstance.AddListener(logListener);
        }

        /// <summary>
        /// Initializes the instruction handler.
        /// </summary>
        private static void InitializeInstructionHandler()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            instructionHandler = new InputInstructionHandler(Console.In, Console.Out);
            instructionHandler.SyncManager = syncManagerObj;
            instructionHandler.HelpInstructionOccured +=
                (sender, e) => { Console.WriteLine(Resources.HelpInstruction); };
            instructionHandler.BeforeOutput += (sender, e) => { Console.ForegroundColor = e.Color; };
            instructionHandler.AfterOutput += (sender, e) => { Console.ResetColor(); };
            instructionHandler.LogFileChangeOccured += InstructionHandlerLogFileChangeOccured;
            instructionHandler.MonitorChangeOccured += InstructionHandlerMonitorChangeOccured;
        }

        /// <summary>
        /// Handles the monitor changed event of the instruction input handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="MonitorChangeEventArgs"/> instance containing the event data.
        /// </param>
        private static void InstructionHandlerMonitorChangeOccured(object sender, MonitorChangeEventArgs e)
        {
            if (e.Type == MonitorType.Log)
            {
                if (e.Hide)
                {
                    logMonitor.Stop();
                }
                else
                {
                    logMonitor.Start();
                }
            }
            else
            {
                if (e.Hide)
                {
                    queueMonitor.Stop();
                }
                else
                {
                    queueMonitor.Start();
                    Thread.Sleep(new TimeSpan(0, 0, 0, 3));
                    monitorScreenGenerator.SendInitialScreen();
                }
            }
        }

        /// <summary>
        /// Handles the input instruction handler log file changed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="LogFilePropertiesChangedEventArgs"/> instance containing the event data.
        /// </param>
        private static void InstructionHandlerLogFileChangeOccured(object sender, LogFilePropertiesChangedEventArgs e)
        {
            if (logFileListener != null)
            {
                logInstance.RemoveListener(logFileListener);
            }

            logFileListener = new FileLogListener(e.LogFileName, e.LogFileSize);

            logInstance.AddListener(logFileListener);
        }

        /// <summary>
        /// Prepares the console window.
        /// </summary>
        private static void PrepareConsoleWindow()
        {
            Console.Title = @"DataSync";

            ConsoleWindowPositioner positioner = new ConsoleWindowPositioner();

            Console.WriteLine(Resources.Program_Main_IntroText);

            positioner.SetConsoleWindowPosition(50, 50);
        }

        /// <summary>
        /// Initializes the application domain.
        /// </summary>
        private static void InitializeAppDomain()
        {
            // React to current process end
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainProcessExit;

            // Handle unhandled exceptions - prevent close if possible
            // AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// Handles the ProcessExit event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        private static void CurrentDomainProcessExit(object sender, EventArgs e)
        {
            CloseMonitors();
        }

        /// <summary>
        /// Starts the monitors.
        /// </summary>
        private static void StartMonitors()
        {
            logMonitor = new ConsoleMonitor(MonitorType.Log);
            logMonitor.Start();

            monitorScreenGenerator = new MonitorScreenGenerator(syncManagerObj);
            queueMonitor = new ConsoleMonitor(MonitorType.Screen);
            queueMonitor.Start();
        }

        /// <summary>
        /// Closes the monitors.
        /// </summary>
        private static void CloseMonitors()
        {
            logMonitor.Stop();
            queueMonitor.Stop();
        }

        /// <summary>
        /// Handles the event of the ArgumentCreator.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="ArgumentErrorEventArgs"/> instance containing the event data.
        /// </param>
        private static void ArgumentCreatorErrorOccuredHandler(object sender, ArgumentErrorEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.ToString());
            Console.ResetColor();
            Console.WriteLine(Resources.Program_Main_EnterForEXIT);
            Console.ReadLine();

            // End application
            Environment.Exit(0);
        }
    }
}