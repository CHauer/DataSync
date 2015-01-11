using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Sync.Items;
using DataSync.Lib.Sync.Jobs;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncPair
    {
        /// <summary>
        /// is watching status flag.
        /// </summary>
        private bool isWatching;

        /// <summary>
        /// The file system watcher instance
        /// </summary>
        private FileSystemWatcher fileSystemWatcherInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncPair" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="configurationPair">The configuration pair.</param>
        public SyncPair(SyncConfiguration configuration, ConfigurationPair configurationPair)
        {
            this.Configuration = configuration;
            this.ConfigurationPair = configurationPair;

            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            SyncQueue = new SyncQueue();
        }

        /// <summary>
        /// Gets the synchronize queue.
        /// </summary>
        /// <value>
        /// The synchronize queue.
        /// </value>
        public SyncQueue SyncQueue { get; private set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets the configuration pair.
        /// </summary>
        /// <value>
        /// The configuration pair.
        /// </value>
        public SyncConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the configuration pair.
        /// </summary>
        /// <value>
        /// The configuration pair.
        /// </value>
        public ConfigurationPair ConfigurationPair { get; private set; }

        /// <summary>
        /// Gets or sets the comparer instance.
        /// </summary>
        /// <value>
        /// The comparer instance.
        /// </value>
        public ISyncItemComparer ComparerInstance { get; set; }

        /// <summary>
        /// Gets or sets the status if this pair is synced.
        /// If an SyncJob is in the Sync Queue its currently not synced!
        /// </summary>
        /// <value>
        ///   <c>true</c> if synced; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynced
        {
            get
            {
                if (SyncQueue != null && SyncQueue.Count > 0)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Starts the watcher.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The Watcher is already running!
        /// </exception>
        public void StartWatcher()
        {
            if (isWatching)
            {
                throw new InvalidOperationException("The Watcher is already running!");
            }

            isWatching = true;

            RunInitialSync();
            
            InitializeFileWatcher();
            fileSystemWatcherInstance.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stops the watcher.
        /// </summary>
        public void StopWatcher()
        {
            isWatching = false;
            fileSystemWatcherInstance.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Runs the initial synchronisation.
        /// </summary>
        /// <returns></returns>
        private void RunInitialSync()
        {
            RunFolderSync();

            RunFileSync();
        }

        private void RunFolderSync()
        {
            List<string> relativeDirectories = ConfigurationPair.GetRelativeDirectories();
            ISyncItem item = null;
            ISyncOperation operation = null;
            ISyncJob job = null;

            foreach (string relativeDir in relativeDirectories)
            {
                if (!Configuration.IsParrallelSync)
                {
                    foreach (string targetFolder in ConfigurationPair.TargetFolders)
                    {
                        string sourceDir = Path.Combine(ConfigurationPair.SoureFolder, relativeDir);
                        string targetDir = Path.Combine(targetFolder, relativeDir);

                        item = new SyncFolder(sourceDir, targetDir);
                        operation = ComparerInstance.Compare(item);
                        job = new SyncJob(item, operation);
                        SyncQueue.Enqueue(job);
                    }
                }
                else
                {
                    var groupedFolders = ConfigurationPair.TargetFolders.GroupBy(tf => tf[0]);
                    //TODO parallelSync initial sync Function
                }
            }
        }

        private void RunFileSync()
        {
            List<string> relativeFiles = ConfigurationPair.GetRelativeFiles();
            ISyncItem item = null;
            ISyncOperation operation = null;
            ISyncJob job = null;

            foreach (string relativeFile in relativeFiles)
            {
                foreach (string targetFolder in ConfigurationPair.TargetFolders)
                {
                    string sourceFile = Path.Combine(ConfigurationPair.SoureFolder, relativeFile);
                    string targetFile = Path.Combine(targetFolder, relativeFile);

                    item = new SyncFile(sourceDir, targetDir);
                }
            }
        }

        /// <summary>
        /// Initializes the file watcher.
        /// </summary>
        private void InitializeFileWatcher()
        {
            fileSystemWatcherInstance = new FileSystemWatcher(ConfigurationPair.SoureFolder)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.DirectoryName |
                               NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            fileSystemWatcherInstance.Changed += FileSystemWatcher_Changed;
            fileSystemWatcherInstance.Created += FileSystemWatcher_Created;
            fileSystemWatcherInstance.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcherInstance.Renamed += FileSystemWatcher_Renamed;
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
