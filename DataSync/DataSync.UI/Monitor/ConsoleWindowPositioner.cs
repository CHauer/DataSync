// -----------------------------------------------------------------------
// <copyright file="ConsoleWindowPositioner.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ConsoleWindowPositioner.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace DataSync.UI.Monitor
{
    public class ConsoleWindowPositioner
    {
        /// <summary>
        /// The Pointer to the current console window
        /// </summary>
        private IntPtr consoleWindow;

        /// <summary>
        /// The SWP nosize
        /// </summary>
        private const int SwpNosize = 0x0001;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleWindowPositioner"/> class.
        /// </summary>
        public ConsoleWindowPositioner()
        {
            consoleWindow = GetConsoleWindow();
        }

        /// <summary>
        /// Sets the console window position.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public void SetConsoleWindowPosition(int left, int top)
        {
            SetWindowPos(consoleWindow, 0, left, top, 0, 0, SwpNosize);
        }

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
        private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy,
                                                  int wFlags);
    }
}