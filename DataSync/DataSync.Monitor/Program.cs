using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataSync.Lib.Log.Messages;
using DataSync.UI;
using DataSync.UI.Monitor;

namespace DataSync.Monitor
{
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            bool running = true;
            string pipeID = string.Empty;
            MonitorType monitorType;

            if (CheckArguments(args)) return;

            pipeID = args[0];

            Console.CursorVisible = false;

            monitorType = ExtractMonitorType(args);
            PrepareConsoleWindow(monitorType);

            while (running)
            {
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Extracts the type of the monitor.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        private static MonitorType ExtractMonitorType(string[] args)
        {
            if (args[1].ToLower().Equals("screen"))
            {
                return MonitorType.Screen;
            }
            else if (args[1].ToLower().Equals("Log"))
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
        private static bool CheckArguments(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Wrong number of arguments given - program exit.");
                return true;
            }
            return false;
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

    }
}
