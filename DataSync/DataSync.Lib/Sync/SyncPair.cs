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

            InitializeQueue();
            RunInitialSync();

            InitializeFileWatcher();

            //Filewatcher start - fire events
            fileSystemWatcherInstance.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Initializes the queue.
        /// </summary>
        private void InitializeQueue()
        {
            SyncQueue = new SyncQueue()
            {
                Logger = Logger
            };
        }

        /// <summary>
        /// Stops the watcher.
        /// </summary>
        public void StopWatcher()
        {
            isWatching = false;

            //filewatcher stop
            fileSystemWatcherInstance.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Runs the initial synchronisation.
        /// </summary>
        private void RunInitialSync()
        {
            RunSyncForRealtivePaths(ConfigurationPair.GetRelativeDirectories(), isFolders: true);

            RunSyncForRealtivePaths(ConfigurationPair.GetRelativeFiles());

        }

        /// <summary>
        /// Runs the synchronize.
        /// </summary>
        /// <param name="relativePaths">The relative paths.</param>
        /// <param name="isFolders">if set to <c>true</c> [is folders].</param>
        private void RunSyncForRealtivePaths(List<string> relativePaths, bool isFolders = false)
        {
            List<string> targetFolders;
            ISyncItem item = null;
            ISyncOperation operation = null;
            ISyncJob job = null;

            foreach (string relativePath in relativePaths)
            {
                string source = Path.Combine(ConfigurationPair.SoureFolder, relativePath);

                //copy target folders list
                targetFolders = new List<string>(ConfigurationPair.TargetFolders);

                if (Configuration.IsParrallelSync)
                {
                    targetFolders = CreateParallelJob(targetFolders, relativePath, source, isFolders);
                }

                foreach (string targetFolder in targetFolders)
                {
                    string target = Path.Combine(targetFolder, relativePath);

                    if (isFolders)
                    {
                        item = new SyncFolder(source, target);
                    }
                    else
                    {
                        item = new SyncFile(source, target);
                    }

                    operation = ComparerInstance.Compare(item);
                    operation.Configuration = Configuration;

                    job = new SyncJob(item, operation)
                    {
                        Logger = Logger                        
                    };
                    SyncQueue.Enqueue(job);
                }
            }
        }

        /// <summary>
        /// Creates the parallel job.
        /// </summary>
        /// <param name="targetFolders">The target folders.</param>
        /// <param name="relativePath">The relative dir.</param>
        /// <param name="source">The source.</param>
        /// <param name="isFolders">if set to <c>true</c> [is folders].</param>
        /// <returns></returns>
        private List<string> CreateParallelJob(List<string> targetFolders, string relativePath, string source, bool isFolders)
        {
            ISyncItem item;
            ISyncOperation operation;
            ISyncJob job;
            Dictionary<ISyncItem, ISyncOperation> parallelJobs = new Dictionary<ISyncItem, ISyncOperation>();

            //group by first letter without network shares 
            var groupedTargets = targetFolders.Where(tf => !tf.StartsWith(@"\\"))
                .GroupBy(tf => tf[0])
                .Where(gt => gt.Count() == 1)
                .Select(i => i.First())
                .ToList();

            //run through each with more than one target
            foreach (var targetFolder in groupedTargets)
            {
                string target = Path.Combine(targetFolder, relativePath);

                if (isFolders)
                {
                    item = new SyncFolder(source, target);
                }
                else
                {
                    item = new SyncFile(source, target);
                }

                operation = ComparerInstance.Compare(item);
                operation.Configuration = Configuration;

                parallelJobs.Add(item, operation);

                //remove used target folder
                targetFolders.Remove(targetFolder);
            }

            job = new ParallelSyncJob(parallelJobs)
            {
                Logger = Logger
            };

            SyncQueue.Enqueue(job);

            //return updates target folders
            return targetFolders;
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
