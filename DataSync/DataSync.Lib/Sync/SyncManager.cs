// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncManager.cs" company="FH Wr.Neustadt">
//   Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>
//   DataSync.Lib - SyncManager.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DataSync.Lib.Sync
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    using DataSync.Lib.Configuration;
    using DataSync.Lib.Configuration.Data;
    using DataSync.Lib.Log;
    using DataSync.Lib.Log.Messages;

    /// <summary>
    /// Sync Manager - Sync logic.
    /// </summary>
    public class SyncManager
    {
        /// <summary>
        /// The compare instance.
        /// </summary>
        private ISyncItemComparer compareInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncManager"/> class.
        /// </summary>
        public SyncManager()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncManager"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public SyncManager(ILog logger)
            : this(null, null, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncManager"/> class.
        /// </summary>
        /// <param name="configLoader">
        /// The configuration loader.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public SyncManager(IConfigurationLoader configLoader, ILog logger)
            : this(configLoader, null, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncManager"/> class.
        /// </summary>
        /// <param name="configLoader">
        /// The configuration loader.
        /// </param>
        /// <param name="configSaver">
        /// The configuration saver.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public SyncManager(IConfigurationLoader configLoader, IConfigurationSaver configSaver, ILog logger)
        {
            this.ConfigurationLoader = configLoader;
            this.ConfigurationSaver = configSaver;
            this.Logger = logger;
            this.Initialize();
        }

        /// <summary>
        /// Occurs when a synchronize pair updated.
        /// </summary>
        public event EventHandler<SyncPair> StateUpdated;

        /// <summary>
        /// Gets the synchronize configuration.
        /// </summary>
        /// <value>
        /// The synchronize configuration.
        /// </value>
        public SyncConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets or sets the configuration data manager.
        /// When the data manager is valid the sync config gets loaded automatically.
        /// </summary>
        /// <value>
        /// The configuration data manager.
        /// </value>
        public IConfigurationLoader ConfigurationLoader { get; set; }

        /// <summary>
        /// Gets or sets the configuration data manager.
        /// When the data manager is valid the sync config gets loaded automatically.
        /// </summary>
        /// <value>
        /// The configuration data manager.
        /// </value>
        public IConfigurationSaver ConfigurationSaver { get; set; }

        /// <summary>
        /// Gets a value indicating whether the pair are all synced.
        /// </summary>
        /// <value>
        /// <c>true</c> if synced; otherwise, <c>false</c>.
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
        /// Gets a value indicating whether [is synchronize running].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is synchronize running]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSyncRunning { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets the synchronize pairs.
        /// </summary>
        /// <value>
        /// The synchronize pairs.
        /// </value>
        public List<SyncPair> SyncPairs { get; private set; }

        /// <summary>
        /// Adds the synchronize pair.
        /// </summary>
        /// <param name="addSyncPair">The add synchronize pair.</param>
        /// <returns>
        /// The <see cref="bool" /> return status.
        /// </returns>
        public bool AddSyncPair(ConfigurationPair addSyncPair)
        {
            // validate addSyncPair - name already exists
            if (this.Configuration.ConfigPairs.Any(cp => cp.Name.Equals(addSyncPair.Name)))
            {
                this.LogMessage(
                    new ErrorLogMessage(
                        string.Format(
                            "The Sync Pair with Name {0} " + "cant be added - name is already in Sync Pair List!", 
                            addSyncPair.Name)));
                return false;
            }

            // add Logger instance 
            addSyncPair.Logger = this.Logger;

            this.Configuration.ConfigPairs.Add(addSyncPair);
            this.SaveConfiguration();

            SyncPair syncPair = this.AddSyncPairFromConfigurationPair(addSyncPair);

            if (this.IsSyncRunning)
            {
                syncPair.StartWatcher();
            }

            return true;
        }

        /// <summary>
        /// Clears the synchronize pairs.
        /// </summary>
        public void ClearSyncPairs()
        {
            this.Configuration.ConfigPairs.Clear();
            this.SyncPairs.ForEach(sp => sp.StopWatcher());
            this.SyncPairs.Clear();
            this.SaveConfiguration();
        }

        /// <summary>
        /// Removes the synchronize pair.
        /// </summary>
        /// <param name="pairname">The pair name.</param>
        /// <returns>The remove status.</returns>
        public bool RemoveSyncPair(string pairname)
        {
            var delItem = this.Configuration.ConfigPairs.FirstOrDefault(cp => cp.Name.Equals(pairname));
            if (delItem == null)
            {
                this.LogMessage(
                    new ErrorLogMessage(
                        string.Format(
                            "The Configuration Pair with Name {0} " + "could'nt be found in Configuration Pair List!", 
                            pairname)));
                return false;
            }

            var delPair = this.SyncPairs.FirstOrDefault(sp => sp.ConfigurationPair.Name.Equals(delItem.Name));

            if (delPair == null)
            {
                this.LogMessage(
                    new ErrorLogMessage(
                        string.Format("The Sync Pair with Name {0} " + "could'nt be found in Sync Pair List!", pairname)));
                return false;
            }

            delPair.StopWatcher();
            this.SyncPairs.Remove(delPair);
            this.Configuration.ConfigPairs.Remove(delItem);
            this.SaveConfiguration();

            return true;
        }

        /// <summary>
        /// Starts the synchronize.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The sync process is already running!.
        /// </exception>
        public void StartSync()
        {
            if (this.IsSyncRunning)
            {
                throw new InvalidOperationException("The sync process is already running!");
            }

            this.SyncPairs.ForEach(sp => sp.StartWatcher());

            this.IsSyncRunning = true;
        }

        /// <summary>
        /// Stops the synchronization.
        /// </summary>
        public void StopSync()
        {
            this.SyncPairs.ForEach(sp => sp.StopWatcher());

            this.IsSyncRunning = false;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Synchronization Pairs:");
            foreach (SyncPair pair in this.SyncPairs)
            {
                builder.AppendLine(
                    string.Format("- {0} {1}", pair.ConfigurationPair.Name, pair.IsSynced ? "Synced" : "Not Synced"));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Called when a synchronize pair updated.
        /// </summary>
        /// <param name="e">
        /// The sync pair element.
        /// </param>
        protected virtual void OnSyncPairStateUpdated(SyncPair e)
        {
            // ReSharper disable once UseNullPropagation
            if (this.StateUpdated != null)
            {
                this.StateUpdated(this, e);
            }
        }

        /// <summary>
        /// Adds the synchronize pair from configuration pair.
        /// </summary>
        /// <param name="addSyncPair">
        /// The add synchronize pair.
        /// </param>
        /// <returns>
        /// The created sync pair instance.
        /// </returns>
        private SyncPair AddSyncPairFromConfigurationPair(ConfigurationPair addSyncPair)
        {
            var pair = new SyncPair(this.Configuration, addSyncPair)
            {
                Logger = this.Logger, 
                ComparerInstance = this.compareInstance
            };

            pair.StateUpdated += (sender, e) => { this.OnSyncPairStateUpdated(sender as SyncPair); };
            this.SyncPairs.Add(pair);

            return pair;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Configuration control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="PropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void ConfigurationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.SaveConfiguration();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.compareInstance = new SyncItemComparer() { Logger = this.Logger };

            this.InitializeConfiguration();
            this.InitializeSyncPairs();
        }

        /// <summary>
        /// Initializes the configuration.
        /// Load config from file if data manager is given or
        /// uses standard configuration.
        /// </summary>
        private void InitializeConfiguration()
        {
            if (this.ConfigurationLoader != null)
            {
                try
                {
                    this.Configuration = this.ConfigurationLoader.LoadConfiguration();
                }
                catch (Exception ex)
                {
                    this.LogMessage(new ErrorLogMessage("Configuration Load Error", ex));
                    this.Configuration = null;
                }
            }

            this.Configuration = this.Configuration ?? new SyncConfiguration();

            this.Configuration.PropertyChanged += this.ConfigurationPropertyChanged;
        }

        /// <summary>
        /// Initializes the synchronize pairs.
        /// </summary>
        private void InitializeSyncPairs()
        {
            this.SyncPairs = new List<SyncPair>();

            foreach (ConfigurationPair configPair in this.Configuration.ConfigPairs)
            {
                this.AddSyncPairFromConfigurationPair(configPair);
            }
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        private void SaveConfiguration()
        {
            if (this.ConfigurationSaver != null)
            {
                try
                {
                    this.ConfigurationSaver.SaveConfiguration(this.Configuration);
                }
                catch (Exception ex)
                {
                    this.Logger.AddLogMessage(new ErrorLogMessage(ex));
                }
            }
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void LogMessage(LogMessage message)
        {
            // ReSharper disable once UseNullPropagation
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(message);
            }
        }
    }
}