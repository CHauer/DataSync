// -----------------------------------------------------------------------
// <copyright file="OutputEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - OutputEventArgs.cs</summary>
// -----------------------------------------------------------------------

using System;

namespace DataSync.UI.CommandHandling
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputEventArgs"/> class.
        /// </summary>
        public OutputEventArgs() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputEventArgs"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public OutputEventArgs(ConsoleColor color)
        {
            this.Color = color;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public ConsoleColor Color { get; private set; }
    }
}