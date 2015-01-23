using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSync.Lib.Sync;
using DataSync.Lib.Sync.Jobs;

namespace DataSync.UI.Monitor
{
    [Serializable]
    public class MonitorScreen 
    {
        /// <summary>
        /// The builder
        /// </summary>
        private StringBuilder builder;

        /// <summary>
        /// The maximum view jobs
        /// </summary>
        private const int MaxViewJobs = 10;

        /// <summary>
        /// The columns
        /// </summary>
        private List<int> columnWidths;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorScreen"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public MonitorScreen(int width, int height)
        {
            this.builder = new StringBuilder();
            this.Width = width;
            this.Height = height;

            columnWidths = new List<int>() { 30, 30, 15, 14, 7};
            AddHeader();
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }

        /// <summary>
        /// Adds the pair block.
        /// </summary>
        /// <param name="pair">The pair.</param>
        public void AddPairBlock(SyncPair pair)
        {
            builder.AppendFormat("# {0} #", pair.ConfigurationPair.Name);
            builder.AppendLine();

            var jobs = pair.SyncQueue.Jobs.Where(job => job.Status == JobStatus.Processing).ToList();
            jobs.AddRange(pair.SyncQueue.Jobs.Where(job => job.Status == JobStatus.Queued));

            foreach (var job in jobs.Take(MaxViewJobs))
            {
                builder.AppendLine(job.ToString(columnWidths));
            }
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        private void AddHeader()
        {
            builder.AppendLine(String.Format("{0} {1} {2} {3} {4}", 
                "Source".PadRight(30), "Target".PadRight(30), "File".PadRight(15), 
                "Operation".PadRight(14), "Status".PadRight(7)));
            builder.AppendLine();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
