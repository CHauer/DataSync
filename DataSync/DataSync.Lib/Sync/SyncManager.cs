// -----------------------------------------------------------------------
// <copyright file="SyncManager.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncManager.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DataSync.Lib.Configuration;
using DataSync.Lib.Configuration.Data;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncManager
    {
        /// <summary>
        /// The compare instance
        /// </summary>
        private ISyncItemComparer compareInstance; 

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncManager"/> class.
        /// </summary>
        public SyncManager() : this(null, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncManager" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SyncManager(ILog logger) : this(null, null, logger) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncManager" /> class.
        /// </summary>
        /// <param name="configLoader">The configuration loader.</param>
        /// <param name="logger">The logger.</param>
        public SyncManager(IConfigurationLoader configLoader, ILog logger)
            : this(configLoader, null, logger) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncManager" /> class.
        /// </summary>
        /// <param name="configLoader">The configuration loader.</param>
        /// <param name="configSaver">The configuration saver.</param>
        /// <param name="logger">The logger.</param>
        public SyncManager(IConfigurationLoader configLoader, IConfigurationSaver configSaver, ILog logger)
        {
            this.ConfigurationLoader = configLoader;
            this.ConfigurationSaver = configSaver;
            this.Logger = logger;
            Initialize();
        }

        /// <summary>
        /// Gets the synchronize configuration.
        /// </summary>
        /// <value>
        /// The synchronize configuration.
        /// </value>
        public SyncConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [is synchronize running].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is synchronize running]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSyncRunning { get; private set; }

        /// <summary>
        /// Gets or sets the configuration data manager.
        /// When the data manager is valid the sync config gets loaded automatically.
        /// </summary>
        /// <value>
        /// The configuration data manager.
        /// </value>
        public IConfigurationLoader ConfigurationLoader { get; private set; }

        /// <summary>
        /// Gets or sets the configuration data manager.
        /// When the data manager is valid the sync config gets loaded automatically.
        /// </summary>
        /// <value>
        /// The configuration data manager.
        /// </value>
        public IConfigurationSaver ConfigurationSaver { get; private set; }

        /// <summary>
        /// Gets the synchronize pairs.
        /// </summary>
        /// <value>
        /// The synchronize pairs.
        /// </value>
        public List<SyncPair> SyncPairs { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets a value indicating whether the pair are all synced.
        /// </summary>
        /// <value>
        ///   <c>true</c> if synced; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynced
        {
            get
            {
                if (this.SyncPairs.Count > 0)
                {
                    return this.SyncPairs.All(sp => sp.IsSynced);
                }

                return true;
            }
        }

        /// <summary>
        /// Occurs when a synchronize pair updated.
        /// </summary>
        public event EventHandler<SyncPair> StateUpdated;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            compareInstance = new SyncItemComparer()
            {
                Logger = Logger
            };

            InitializeConfiguration();
            InitializeSyncPairs();
        }

        /// <summary>
        /// Initializes the configuration.
        /// Load config from file if datamanager is given or
        /// uses standard configuration.
        /// </summary>
        private void InitializeConfiguration()
        {
            if (ConfigurationLoader != null)
            {
                try
                {
                    Configuration = ConfigurationLoader.LoadConfiguration();
                }
                catch (Exception ex)
                {
                    LogMessage(new ErrorLogMessage("Configuration Load Error", ex));
                    Configuration = null;
                }
            }

            Configuration = this.Configuration ?? new SyncConfiguration();

            Configuration.PropertyChanged += Configuration_PropertyChanged;
        }

        /// <summary>
        /// Initializes the synchronize pairs.
        /// </summary>
        private void InitializeSyncPairs()
        {
            SyncPairs = new List<SyncPair>();

            foreach (ConfigurationPair configPair in Configuration.ConfigPairs)
            {
                AddSyncPairFromConfigurationPair(configPair);
            }
        }

        /// <summary>
        /// Adds the synchronize pair.
        /// </summary>
        /// <param name="addSyncPair">The add synchronize pair.</param>
        /// <returns></returns>
        public bool AddSyncPair(ConfigurationPair addSyncPair)
        {
            //validate addSyncPair - name already exists
            if (Configuration.ConfigPairs.Any(cp => cp.Name.Equals(addSyncPair.Name)))
            {
                LogMessage(new ErrorLogMessage(String.Format("The Sync Pair with Name {0} " +
                                       "cant be added - name is already in Sync Pair List!",
                    addSyncPair.Name)));
                return false;
            }

            //add Logger instance 
            addSyncPair.Logger = Logger;

            Configuration.ConfigPairs.Add(addSyncPair);
            SaveConfiguration();

            SyncPair syncPair = AddSyncPairFromConfigurationPair(addSyncPair);

            if (IsSyncRunning)
            {
                syncPair.StartWatcher();
            }

            return true;
        }

        /// <summary>
        /// Adds the synchronize pair from configuration pair.
        /// </summary>
        /// <param name="addSyncPair">The add synchronize pair.</param>
        /// <returns>
        /// The created sync pair instance.
        /// </returns>
        private SyncPair AddSyncPairFromConfigurationPair(ConfigurationPair addSyncPair)
        {
            var pair = new SyncPair(Configuration, addSyncPair)
            {
                Logger = Logger,
                ComparerInstance = compareInstance
            };

            pair.StateUpdated += (sender, e) => { OnSyncPairStateUpdated(sender as SyncPair); };
            SyncPairs.Add(pair);

            return pair;
        }

        /// <summary>
        /// Removes the synchronize pair.
        /// </summary>
        /// <param name="pairname">The pairname.</param>
        /// <returns></returns>
        public bool RemoveSyncPair(string pairname)
        {
            var delItem = Configuration.ConfigPairs.FirstOrDefault(cp => cp.Name.Equals(pairname));

            if (delItem == null)
            {
                LogMessage(new ErrorLogMessage(String.Format("The Sync Pair with Name {0} " +
                                                                       "could'nt be found in Sync Pair List!",
                    pairname)));
                return false;
            }

            Configuration.ConfigPairs.Remove(delItem);
            SaveConfiguration();

            return true;
        }

        /// <summary>
        /// Clears the synchronize pairs.
        /// </summary>
        public void ClearSyncPairs()
        {
            Configuration.ConfigPairs.Clear();
            SaveConfiguration();
        }

        /// <summary>
        /// Starts the synchronize.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The sync process is already running!</exception>
        public void StartSync()
        {
            if (IsSyncRunning)
            {
                throw new InvalidOperationException("The sync process is already running!");
            }

            SyncPairs.ForEach(sp => sp.StartWatcher());

            IsSyncRunning = true;
        }

        /// <summary>
        /// Stops the synchronization.
        /// </summary>
        public void StopSync()
        {
            SyncPairs.ForEach(sp => sp.StopWatcher());

            IsSyncRunning = false;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Configuration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveConfiguration();
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        private void SaveConfiguration()
        {
            if (ConfigurationSaver != null)
            {
                try
                {
                    ConfigurationSaver.SaveConfiguration(Configuration);
                }
                catch (Exception ex)
                {
                    Logger.AddLogMessage(new ErrorLogMessage(ex));
                }
            }
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(LogMessage message)
        {
            // ReSharper disable once UseNullPropagation
            if (Logger != null)
            {
                Logger.AddLogMessage(message);
            }
        }

        /// <summary>
        /// Called when a synchronize pair updated.
        /// </summary>
        /// <param name="e">The e.</param>
        protected virtual void OnSyncPairStateUpdated(SyncPair e)
        {
            // ReSharper disable once UseNullPropagation
            if (StateUpdated != null)
            {
                StateUpdated(this, e);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Synchronization Pairs:");
            foreach (SyncPair pair in SyncPairs)
            {
                builder.AppendLine(String.Format("- {0} {1}", pair.ConfigurationPair.Name, pair.IsSynced ? "Synced" : "Not Synced"));
            }

            return builder.ToString();
        }
    }
}