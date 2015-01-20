using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSync.Lib.Sync;
using DataSync.UI.Monitor.Pipe;

namespace DataSync.UI.Monitor
{
    public class MonitorScreenGenerator
    {
        /// <summary>
        /// The pipe sender
        /// </summary>
        private PipeSender<MonitorScreen> pipeSender;

        /// <summary>
        /// The maximum view queues
        /// </summary>
        private const int MaxViewQueues = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorScreenGenerator"/> class.
        /// </summary>
        public MonitorScreenGenerator(SyncManager manager)
        {
            this.pipeSender = new PipeSender<MonitorScreen>(MonitorType.Screen.ToString("g"));
            this.Manager = manager;
            this.Manager.SyncStateUpdated += ManagerSyncStateUpdated;
        }

        /// <summary>
        /// Gets the monitor pair.
        /// </summary>
        /// <value>
        /// The monitor pair.
        /// </value>
        public SyncManager Manager { get; private set; }

        /// <summary>
        /// Handles the synchronize pair updated event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ManagerSyncStateUpdated(object sender, SyncPair e)
        {
            MonitorScreen screen = GenerateMonitorScreen();

            pipeSender.SendMessage(screen);
        }

        /// <summary>
        /// Generates the monitor screen.
        /// </summary>
        /// <returns></returns>
        private MonitorScreen GenerateMonitorScreen()
        {
            MonitorScreen screen = new MonitorScreen(100, 50);

            foreach (var pair in Manager.SyncPairs.Where(sp => !sp.IsSynced)
                                        .OrderBy(sp => sp.ConfigurationPair.Name)
                                        .Take(MaxViewQueues))
            {
                screen.AddPairBlock(pair);
            }

            return screen;
        }
    }
}
