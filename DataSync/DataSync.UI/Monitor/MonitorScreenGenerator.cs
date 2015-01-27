// -----------------------------------------------------------------------
// <copyright file="MonitorScreenGenerator.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - MonitorScreenGenerator.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Monitor
{
    using System.Linq;

    using DataSync.Lib.Sync;
    using DataSync.UI.Monitor.Pipe;

    /// <summary>
    /// The monitor screen generator.
    /// </summary>
    public class MonitorScreenGenerator
    {
        /// <summary>
        /// The maximum view queues.
        /// </summary>
        private const int MaxViewQueues = 4;

        /// <summary>
        /// The pipe sender.
        /// </summary>
        private PipeSender<MonitorScreen> pipeSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorScreenGenerator"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public MonitorScreenGenerator(SyncManager manager)
        {
            this.pipeSender = new PipeSender<MonitorScreen>(MonitorType.Screen.ToString("g"));
            this.Manager = manager;
            this.Manager.StateUpdated += this.ManagerStateUpdated;

            // send initial screen
            this.SendInitialScreen();
        }

        /// <summary>
        /// Gets the monitor pair.
        /// </summary>
        /// <value>
        /// The monitor pair.
        /// </value>
        public SyncManager Manager { get; private set; }

        /// <summary>
        /// Sends the initial screen.
        /// </summary>
        public void SendInitialScreen()
        {
            this.pipeSender.SendMessage(new MonitorScreen(100, 50));
        }

        /// <summary>
        /// Generates the monitor screen.
        /// </summary>
        /// <returns>
        /// The <see cref="MonitorScreen"/>.
        /// </returns>
        private MonitorScreen GenerateMonitorScreen()
        {
            MonitorScreen screen = new MonitorScreen(100, 50);

            foreach (
                var pair in
                    this.Manager.SyncPairs.Where(sp => !sp.IsSynced)
                        .OrderBy(sp => sp.ConfigurationPair.Name)
                        .Take(MaxViewQueues))
            {
                screen.AddPairBlock(pair);
            }

            return screen;
        }

        /// <summary>
        /// Handles the synchronize pair updated event.
        /// </summary>
        /// <param name="sender">
        /// The sender value.
        /// </param>
        /// <param name="e">
        /// The exception.
        /// </param>
        private void ManagerStateUpdated(object sender, SyncPair e)
        {
            MonitorScreen screen = this.GenerateMonitorScreen();

            this.pipeSender.SendMessage(screen);
        }
    }
}