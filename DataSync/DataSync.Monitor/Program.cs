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
        /// The Pointer to the current console window
        /// </summary>
        private static IntPtr consoleWindow;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            string pipeID = string.Empty;
            MonitorType monitorType;

            if (CheckArguments(args)) return;

            consoleWindow = GetConsoleWindow();

            pipeID = args[0];

            Console.CursorVisible = false;

            monitorType = ExtractMonitorType(args);
            PrepareConsoleWindow(monitorType);

            Console.ReadLine();
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
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            int width = 100;
            int height = 25;
            int left = 0;
            int top = 0;


            if (type == MonitorType.Screen)
            {
                left = screenWidth / 2 + (int)(screenWidth * 0.1);
                height = 50;

                Console.Title = "Sync Job Queues";
                Console.SetBufferSize(width, height);
                Console.SetWindowSize(width, height);

                SetWindowPos(consoleWindow, 0, left, top, 0, 0, SwpNosize);
            }
            else
            {
                top = screenHeight / 2 + (int)(screenWidth * 0.1);

                Console.Title = "Log";

                Console.SetBufferSize(width, height * 5);
                Console.SetWindowSize(width, height);

                SetWindowPos(consoleWindow, 0, left, top, 0, 0, SwpNosize);
            }
        }

        /// <summary>
        /// The SWP nosize
        /// </summary>
        private const int SwpNosize = 0x0001;

        /// <summary>
        /// Gets the console window.
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="hWndInsertAfter">The h WND insert after.</param>
        /// <param name="x">The x.</param>
        /// <param name="Y">The y.</param>
        /// <param name="cx">The cx.</param>
        /// <param name="cy">The cy.</param>
        /// <param name="wFlags">The w flags.</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    }
}
