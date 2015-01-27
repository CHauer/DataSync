// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Monitor - Program.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Monitor
{
    using System;
    using System.Windows.Forms;

    using DataSync.Lib.Log;
    using DataSync.Lib.Log.Messages;
    using DataSync.UI.Monitor;
    using DataSync.UI.Monitor.Pipe;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The _log listener.
        /// </summary>
        private static ConsoleLogListener logListener;

        /// <summary>
        /// The _log receiver.
        /// </summary>
        private static PipeReceiver<LogMessage> logReceiver;

        /// <summary>
        /// The _screen receiver.
        /// </summary>
        private static PipeReceiver<MonitorScreen> screenReceiver;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public static void Main(string[] args)
        {
            // ReSharper disable once ConvertToConstant.Local
            bool running = true;

            if (!ValidateArguments(args))
            {
                return;
            }

            InitializeAppDomain();

            Console.CursorVisible = false;

            MonitorType monitorType = ExtractMonitorType(args[0]);
            PrepareConsoleWindow(monitorType);

            if (monitorType == MonitorType.Log)
            {
                logListener = new ConsoleLogListener();
                logReceiver = new PipeReceiver<LogMessage>(monitorType.ToString("g"));
                logReceiver.MessageReceived += LogReceiverMessageReceived;
                logReceiver.StartReceiving();
            }
            else
            {
                screenReceiver = new PipeReceiver<MonitorScreen>(monitorType.ToString("g"));
                screenReceiver.MessageReceived += ScreenReceiverMessageReceived;
                screenReceiver.StartReceiving();
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (running)
            {
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
        }

        /// <summary>
        /// The screen receiver message received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The received event args.
        /// </param>
        private static void ScreenReceiverMessageReceived(object sender, ReceivedEventArgs<MonitorScreen> e)
        {
            Console.Clear();
            Console.Write(e.Message.ToString());
        }

        /// <summary>
        /// The log receiver message received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The received event arguments.
        /// </param>
        private static void LogReceiverMessageReceived(object sender, ReceivedEventArgs<LogMessage> e)
        {
            logListener.WriteLogMessage(e.Message);
        }

        /// <summary>
        /// Extracts the type of the monitor.
        /// </summary>
        /// <param name="screenArg">
        /// The screen argument.
        /// </param>
        /// <returns>
        /// The <see cref="MonitorType"/>.
        /// </returns>
        private static MonitorType ExtractMonitorType(string screenArg)
        {
            if (screenArg.ToLower().Equals("screen"))
            {
                return MonitorType.Screen;
            }
            else if (screenArg.ToLower().Equals("Log"))
            {
                return MonitorType.Log;
            }

            return MonitorType.Log;
        }

        /// <summary>
        /// Checks the arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The status of the execution.
        /// </returns>
        private static bool ValidateArguments(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Wrong number of arguments given - program exit.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Prepares the console window.
        /// </summary>
        /// <param name="type">
        /// The type parameter.
        /// </param>
        private static void PrepareConsoleWindow(MonitorType type)
        {
            ConsoleWindowPositioner positioner = new ConsoleWindowPositioner();

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            int width = 101;
            int height = 25;
            int left = 0;
            int top = 0;

            if (type == MonitorType.Screen)
            {
                left = (screenWidth / 2) - (int)(screenWidth * 0.05);
                height = 50;

                Console.Title = "Sync Job Queues";
                Console.SetBufferSize(width, height);
                Console.SetWindowSize(width, height);

                positioner.SetConsoleWindowPosition(left, top);
            }
            else
            {
                top = (screenHeight / 2) - (int)(screenWidth * 0.05);

                Console.Title = "Log";

                Console.SetBufferSize(width, height * 5);
                Console.SetWindowSize(width, height);

                positioner.SetConsoleWindowPosition(left, top);
            }
        }

        /// <summary>
        /// Initializes the application domain.
        /// </summary>
        private static void InitializeAppDomain()
        {
            // React to current process end
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainProcessExit;
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
            if (logReceiver != null)
            {
                logReceiver.StopReceiving();
            }

            if (screenReceiver != null)
            {
                screenReceiver.StopReceiving();
            }
        }
    }
}