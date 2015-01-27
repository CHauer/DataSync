// -----------------------------------------------------------------------
// <copyright file="MonitorChangeEventArgs.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - MonitorChangeEventArgs.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.CommandHandling
{
    using System;

    using DataSync.UI.Monitor;

    /// <summary>
    /// The monitor change event args.
    /// </summary>
    public class MonitorChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorChangeEventArgs"/> class.
        /// </summary>
        /// <param name="type">
        /// The type value.
        /// </param>
        /// <param name="hide">
        /// If set to <c>true</c> [hide].
        /// </param>
        public MonitorChangeEventArgs(MonitorType type, bool hide = false)
        {
            this.Type = type;
            this.Hide = hide;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MonitorChangeEventArgs"/> is hide.
        /// </summary>
        /// <value>
        /// <c>true</c> if hide; otherwise, <c>false</c>.
        /// </value>
        public bool Hide { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type value.
        /// </value>
        public MonitorType Type { get; set; }
    }
}