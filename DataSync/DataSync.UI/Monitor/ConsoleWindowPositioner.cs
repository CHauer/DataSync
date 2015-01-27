// -----------------------------------------------------------------------
// <copyright file="ConsoleWindowPositioner.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ConsoleWindowPositioner.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Monitor
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The console window positioner.
    /// </summary>
    public class ConsoleWindowPositioner
    {
        /// <summary>
        /// The no size constant.
        /// </summary>
        private const int SwpNosize = 0x0001;

        /// <summary>
        /// The Pointer to the current console window.
        /// </summary>
        private IntPtr consoleWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleWindowPositioner"/> class.
        /// </summary>
        public ConsoleWindowPositioner()
        {
            this.consoleWindow = GetConsoleWindow();
        }

        /// <summary>
        /// Brings the console window to front.
        /// </summary>
        public void BringConsoleWindowToFront()
        {
            SetForegroundWindow(this.consoleWindow);
        }

        /// <summary>
        /// Sets the console window position.
        /// </summary>
        /// <param name="left">
        /// The left parameter.
        /// </param>
        /// <param name="top">
        /// The top parameter.
        /// </param>
        public void SetConsoleWindowPosition(int left, int top)
        {
            SetWindowPos(this.consoleWindow, 0, left, top, 0, 0, SwpNosize);
        }

        /// <summary>
        /// Gets the console window.
        /// </summary>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Sets the foreground window.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// The a boolean value.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr parameter);

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterInsertAfter">The parameter insert after.</param>
        /// <param name="x">The x parameter.</param>
        /// <param name="y">The y parameter.</param>
        /// <param name="cx">The cx parameter.</param>
        /// <param name="cy">The cy parameter.</param>
        /// <param name="flags">The flags parameter.</param>
        /// <returns>
        /// The windows position pointer.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern IntPtr SetWindowPos(
            IntPtr parameter,
            int parameterInsertAfter, 
            int x, 
            int y, 
            int cx, 
            int cy, 
            int flags);
    }
}