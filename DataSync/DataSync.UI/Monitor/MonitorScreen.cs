// -----------------------------------------------------------------------
// <copyright file="MonitorScreen.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - MonitorScreen.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Monitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DataSync.Lib.Sync;
    using DataSync.Lib.Sync.Jobs;

    /// <summary>
    /// The monitor screen.
    /// </summary>
    [Serializable]
    public class MonitorScreen
    {
        /// <summary>
        /// The maximum view jobs.
        /// </summary>
        private const int MaxViewJobs = 10;

        /// <summary>
        /// The builder.
        /// </summary>
        private StringBuilder builder;

        /// <summary>
        /// The columns.
        /// </summary>
        private List<int> columnWidths;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorScreen"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public MonitorScreen(int width, int height)
        {
            this.builder = new StringBuilder();
            this.Width = width;
            this.Height = height;

            this.columnWidths = new List<int>() { 30, 30, 15, 14, 7 };
            this.AddHeader();
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Adds the pair block.
        /// </summary>
        /// <param name="pair">
        /// The pair value.
        /// </param>
        public void AddPairBlock(SyncPair pair)
        {
            this.builder.AppendFormat("# {0} #", pair.ConfigurationPair.Name);
            this.builder.AppendLine();

            var jobs = new List<ISyncJob>();
            if (pair.SyncQueue.CurrentJob != null)
            {
                jobs.Add(pair.SyncQueue.CurrentJob);
            }

            jobs.AddRange(pair.SyncQueue.Jobs.Where(job => job.Status == JobStatus.Queued));

            foreach (var job in jobs.Take(MaxViewJobs))
            {
                this.builder.AppendLine(job.ToString(this.columnWidths));
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.builder.ToString();
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        private void AddHeader()
        {
            this.builder.AppendLine(
                string.Format(
                    "{0} {1} {2} {3} {4}", 
                    "Source".PadRight(30), 
                    "Target".PadRight(30), 
                    "File".PadRight(15), 
                    "Operation".PadRight(14), 
                    "Status".PadRight(7)));
            this.builder.AppendLine();
        }
    }
}