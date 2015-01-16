using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;
using DataSync.UI.Monitor;
using DataSync.UI.Monitor.Pipe;

namespace DataSync.Monitor
{
    public class Program
    {
        /// <summary>
        /// The _screen receiver
        /// </summary>
        private static PipeReceiver<MonitorScreen> _screenReceiver;

        /// <summary>
        /// The _log receiver
        /// </summary>
        private static PipeReceiver<LogMessage> _logReceiver;

        /// <summary>
        /// The _log listener
        /// </summary>
        private static ConsoleLogListener _logListener;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            // ReSharper disable once ConvertToConstant.Local
            bool running = true;

            if (!ValidateArguments(args)) return;

            InitializeAppDomain();

            Console.CursorVisible = false;

            MonitorType monitorType = ExtractMonitorType(args[0]);
            PrepareConsoleWindow(monitorType);

            if (monitorType == MonitorType.Log)
            {
                _logListener = new ConsoleLogListener();
                _logReceiver = new PipeReceiver<LogMessage>(monitorType.ToString("g"));
                _logReceiver.MessageReceived += LogReceiver_MessageReceived;
                _logReceiver.StartReceiving();
            }
            else
            {
                _screenReceiver = new PipeReceiver<MonitorScreen>(monitorType.ToString("g"));
                _screenReceiver.MessageReceived += ScreenReceiver_MessageReceived;
                _screenReceiver.StartReceiving();
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

        private static void ScreenReceiver_MessageReceived(object sender, ReceivedEventArgs<MonitorScreen> e)
        {
            e.Message.PrintToConsole();
        }

        private static void LogReceiver_MessageReceived(object sender, ReceivedEventArgs<LogMessage> e)
        {
            _logListener.WriteLogMessage(e.Message);
        }

        /// <summary>
        /// Extracts the type of the monitor.
        /// </summary>
        /// <param name="screenArg">The screen argument.</param>
        /// <returns></returns>
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
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
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
        /// <param name="type">The type.</param>
        private static void PrepareConsoleWindow(MonitorType type)
        {
            ConsoleWindowPositioner positioner = new ConsoleWindowPositioner();

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            int width = 100;
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
            //React to current process end
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        /// <summary>
        /// Handles the ProcessExit event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (_logReceiver != null)
            {
                _logReceiver.StopReceiving();
            }
            if (_screenReceiver != null)
            {
                _screenReceiver.StopReceiving();
            }
        }

    }
}
