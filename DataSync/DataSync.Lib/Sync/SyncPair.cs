// -----------------------------------------------------------------------
// <copyright file="SyncPair.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncPair.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataSync.Lib.Configuration;
    using DataSync.Lib.Log;
    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync.Items;
    using DataSync.Lib.Sync.Jobs;
    using DataSync.Lib.Sync.Operations;

    /// <summary>
    /// The sync pair class.
    /// </summary>
    public class SyncPair
    {
        /// <summary>
        /// The file system watcher instance.
        /// </summary>
        private FileSystemWatcher fileSystemWatcherInstance;

        /// <summary>
        /// Is watching status flag.
        /// </summary>
        private bool isWatching;

        /// <summary>
        /// The last changed element path.
        /// </summary>
        private string lastChangedElementPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncPair"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="configurationPair">
        /// The configuration pair.
        /// </param>
        public SyncPair(SyncConfiguration configuration, ConfigurationPair configurationPair)
        {
            this.Configuration = configuration;
            this.ConfigurationPair = configurationPair;

            // Standard sync item comparer
            this.ComparerInstance = new SyncItemComparer();
        }

        /// <summary>
        /// Occurs when the sync pair state has updated.
        /// </summary>
        public event EventHandler StateUpdated;

        /// <summary>
        /// Gets or sets the comparer instance.
        /// </summary>
        /// <value>
        /// The comparer instance.
        /// </value>
        public ISyncItemComparer ComparerInstance { get; set; }

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
        /// Gets a value indicating whether this instance is synced.
        /// Gets the status if this pair is synced.
        /// If an SyncJob is in the Sync Queue its currently not synced!.
        /// <value>
        ///   <c>true</c> if synced; otherwise, <c>false</c>.
        /// </value>
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is synced; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynced
        {
            get
            {
                if (this.SyncQueue != null && this.SyncQueue.Count > 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets the synchronize queue.
        /// </summary>
        /// <value>
        /// The synchronize queue.
        /// </value>
        public SyncQueue SyncQueue { get; private set; }

        /// <summary>
        /// Starts the watcher.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The Watcher is already running!.
        /// </exception>
        public void StartWatcher()
        {
            if (this.isWatching)
            {
                throw new InvalidOperationException("The Watcher is already running!");
            }

            this.isWatching = true;

            this.InitializeQueue();

            Task.Run(
                () =>
                    {
                        this.RunInitialSync();

                        this.InitializeFileWatcher();

                        // Filewatcher start - fire events
                        this.fileSystemWatcherInstance.EnableRaisingEvents = true;
                    });
        }

        /// <summary>
        /// Stops the watcher.
        /// </summary>
        public void StopWatcher()
        {
            this.isWatching = false;

            // filewatcher stop
            this.fileSystemWatcherInstance.EnableRaisingEvents = false;

            this.SyncQueue.StopQueue();
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

            builder.AppendLine(string.Format("Synchronization Pair {0}", this.ConfigurationPair.Name));
            builder.AppendLine(string.Format("Source Folder: {0}", this.ConfigurationPair.SoureFolder));

            foreach (string target in this.ConfigurationPair.TargetFolders)
            {
                builder.AppendLine(string.Format("Target Folder: {0}", target));
            }

            foreach (string except in this.ConfigurationPair.ExceptFolders)
            {
                builder.AppendLine(string.Format("Except Folder: {0}", except));
            }

            foreach (var job in this.SyncQueue.Jobs)
            {
                job.ToString(new List<int> { 20, 20, 20, 10, 10 });
            }

            return builder.ToString();
        }

        /// <summary>
        /// Called when the sync pair state has updated.
        /// </summary>
        protected virtual void OnStateUpdated()
        {
            // ReSharper disable once UseNullPropagation
            if (this.StateUpdated != null)
            {
                this.StateUpdated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Creates the parallel job.
        /// </summary>
        /// <param name="targetFolders">The target folders.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="source">The source.</param>
        /// <param name="isFolders">If set to <c>true</c> [is folders].</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// Returns the left target folders.
        /// </returns>
        private List<string> CreateParallelJob(
            List<string> targetFolders, 
            string relativePath, 
            string source, 
            bool isFolders, 
            SyncOperation operation = null)
        {
            ISyncItem item;
            ISyncJob job;
            Dictionary<ISyncItem, SyncOperation> parallelJobs = new Dictionary<ISyncItem, SyncOperation>();

            // group by first letter without network shares 
            var groupedTargets =
                targetFolders.Where(tf => !tf.StartsWith(@"\\"))
                    .GroupBy(tf => tf[0])
                    .Where(gt => gt.Count() == 1)
                    .Select(i => i.First())
                    .ToList();

            // run through each with more than one target
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

                if (operation == null)
                {
                    operation = this.ComparerInstance.Compare(item);
                }

                if (operation != null)
                {
                    operation.Configuration = this.Configuration;
                    operation.Logger = this.Logger;

                    parallelJobs.Add(item, operation);
                }

                // remove used target folder
                targetFolders.Remove(targetFolder);
            }

            if (parallelJobs.Count > 0)
            {
                job = new ParallelSyncJob(parallelJobs) { Logger = this.Logger };

                this.SyncQueue.Enqueue(job);
            }

            // return updates target folders
            return targetFolders;
        }

        /// <summary>
        /// Creates the parallel rename job.
        /// </summary>
        /// <param name="targetFolders">
        /// The target folders.
        /// </param>
        /// <param name="oldRelativePath">
        /// The old relative path.
        /// </param>
        /// <param name="newRelativePath">
        /// The new relative path.
        /// </param>
        /// <param name="isFolders">
        /// If set to <c>true</c> [is folders].
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// Returns the left target folders.
        /// </returns>
        private List<string> CreateParallelRenameJob(
            List<string> targetFolders, 
            string oldRelativePath, 
            string newRelativePath, 
            bool isFolders, 
            SyncOperation operation = null)
        {
            ISyncItem item;
            ISyncJob job;
            Dictionary<ISyncItem, SyncOperation> parallelJobs = new Dictionary<ISyncItem, SyncOperation>();

            // group by first letter without network shares 
            var groupedTargets =
                targetFolders.Where(tf => !tf.StartsWith(@"\\"))
                    .GroupBy(tf => tf[0])
                    .Where(gt => gt.Count() == 1)
                    .Select(i => i.First())
                    .ToList();

            // run through each with more than one target
            foreach (var targetFolder in groupedTargets)
            {
                string target = Path.Combine(targetFolder, newRelativePath);

                if (isFolders)
                {
                    item = new SyncFolder(Path.Combine(targetFolder, oldRelativePath), target);
                }
                else
                {
                    item = new SyncFile(Path.Combine(targetFolder, oldRelativePath), target);
                }

                if (operation == null)
                {
                    operation = this.ComparerInstance.Compare(item);
                }

                if (operation != null)
                {
                    operation.Configuration = this.Configuration;
                    operation.Logger = this.Logger;

                    parallelJobs.Add(item, operation);
                }

                // remove used target folder
                targetFolders.Remove(targetFolder);
            }

            if (parallelJobs.Count > 0)
            {
                job = new ParallelSyncJob(parallelJobs) { Logger = this.Logger };

                this.SyncQueue.Enqueue(job);
            }

            // return updates target folders
            return targetFolders;
        }

        /// <summary>
        /// Deletes the no source target items.
        /// </summary>
        /// <param name="targetFolder">
        /// The target folder.
        /// </param>
        /// <param name="deleteTargetRelativePaths">
        /// The delete target paths.
        /// </param>
        /// <param name="isFolders">
        /// If set to <c>true</c> [is folders].
        /// </param>
        private void DeleteNoSourceTargetItems(
            string targetFolder, 
            List<string> deleteTargetRelativePaths, 
            bool isFolders = false)
        {
            ISyncItem item = null;
            SyncOperation operation = null;
            ISyncJob job = null;

            foreach (string relativePath in deleteTargetRelativePaths)
            {
                string source = Path.Combine(this.ConfigurationPair.SoureFolder, relativePath);
                string target = Path.Combine(targetFolder, relativePath);

                if (isFolders)
                {
                    item = new SyncFolder(source, target);
                    operation = new DeleteFolder();
                }
                else
                {
                    item = new SyncFile(source, target);
                    operation = new DeleteFile();
                }

                operation.Configuration = this.Configuration;

                job = new SyncJob(item, operation) { Logger = this.Logger };
                this.SyncQueue.Enqueue(job);
            }
        }

        /// <summary>
        /// Handles the Changed event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="FileSystemEventArgs"/> instance containing the event data.
        /// </param>
        private void FileSystemWatcherChanged(object sender, FileSystemEventArgs e)
        {
            ISyncJob job = null;
            SyncOperation operation;
            ISyncItem item;

            string source = e.FullPath;
            string relativePath = e.FullPath.Replace(this.ConfigurationPair.SoureFolder + "\\", string.Empty);
            List<string> targetFolders = new List<string>(this.ConfigurationPair.TargetFolders);

            if (Directory.Exists(e.FullPath))
            {
                Debug.WriteLine("Directory Changed: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                if (!this.ConfigurationPair.GetRelativeDirectories().Contains(relativePath))
                {
                    return;
                }

                if (this.Configuration.IsParallelSync)
                {
                    targetFolders = this.CreateParallelJob(targetFolders, relativePath, source, isFolders: true);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(source, Path.Combine(targetFolder, relativePath));

                    // compare item
                    operation = this.ComparerInstance.Compare(item);

                    if (operation == null)
                    {
                        // no sync operation needed
                        return;
                    }

                    operation.Configuration = this.Configuration;

                    job = new SyncJob(item, operation) { Logger = this.Logger };

                    this.SyncQueue.Enqueue(job);
                }
            }
            else
            {
                Debug.WriteLine("File Changed: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                if (!this.ConfigurationPair.GetRelativeFiles().Contains(relativePath))
                {
                    return;
                }

                if (!e.FullPath.Equals(this.lastChangedElementPath))
                {
                    this.lastChangedElementPath = e.FullPath;

                    if (this.Configuration.IsParallelSync)
                    {
                        targetFolders = this.CreateParallelJob(targetFolders, relativePath, source, isFolders: false);
                    }

                    foreach (string targetFolder in targetFolders)
                    {
                        item = new SyncFile(source, Path.Combine(targetFolder, relativePath));

                        // compare item
                        operation = this.ComparerInstance.Compare(item);

                        if (operation == null)
                        {
                            // no sync operation needed
                            return;
                        }

                        operation.Configuration = this.Configuration;

                        job = new SyncJob(item, operation) { Logger = this.Logger };

                        this.SyncQueue.Enqueue(job);
                    }
                }
                else
                {
                    this.lastChangedElementPath = string.Empty;
                    return;
                }
            }
        }

        /// <summary>
        /// Handles the Created event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="FileSystemEventArgs"/> instance containing the event data.
        /// </param>
        private void FileSystemWatcherCreated(object sender, FileSystemEventArgs e)
        {
            ISyncJob job = null;
            SyncOperation operation;
            ISyncItem item;

            this.lastChangedElementPath = string.Empty;

            string source = e.FullPath;
            string relativePath = e.FullPath.Replace(this.ConfigurationPair.SoureFolder + "\\", string.Empty);
            List<string> targetFolders = new List<string>(this.ConfigurationPair.TargetFolders);

            if (Directory.Exists(e.FullPath))
            {
                Debug.WriteLine("Directory Created: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                if (!this.ConfigurationPair.GetRelativeDirectories().Contains(relativePath))
                {
                    return;
                }

                operation = new CreateFolder() { Configuration = this.Configuration, Logger = this.Logger };

                if (this.Configuration.IsParallelSync)
                {
                    targetFolders = this.CreateParallelJob(
                        targetFolders, 
                        relativePath, 
                        source, 
                        isFolders: true, 
                        operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation) { Logger = this.Logger };

                    this.SyncQueue.Enqueue(job);
                }
            }
            else
            {
                Debug.WriteLine("File Created: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                if (!this.ConfigurationPair.GetRelativeFiles().Contains(relativePath))
                {
                    return;
                }

                operation = new CopyFile() { Configuration = this.Configuration, Logger = this.Logger };

                if (this.Configuration.IsParallelSync)
                {
                    targetFolders = this.CreateParallelJob(
                        targetFolders, 
                        relativePath, 
                        source, 
                        isFolders: true, 
                        operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFile(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation) { Logger = this.Logger };

                    this.SyncQueue.Enqueue(job);
                }
            }
        }

        /// <summary>
        /// Handles the Deleted event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="FileSystemEventArgs"/> instance containing the event data.
        /// </param>
        private void FileSystemWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            ISyncJob job = null;
            SyncOperation operation;
            ISyncItem item;

            this.lastChangedElementPath = string.Empty;

            string source = e.FullPath;
            string relativePath = e.FullPath.Replace(this.ConfigurationPair.SoureFolder + "\\", string.Empty);
            List<string> targetFolders = new List<string>(this.ConfigurationPair.TargetFolders);

            // IMPORTANT - directory.exist is every time false  - directory already deleted by OS
            if (string.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
            {
                Debug.WriteLine("Directory Deleted: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                // if (!ConfigurationPair.GetRelativeDirectories().Contains(relativePath))
                // {
                // return;
                // }

                // handle directory delete
                operation = new DeleteFolder() { Configuration = this.Configuration, Logger = this.Logger };

                if (this.Configuration.IsParallelSync)
                {
                    targetFolders = this.CreateParallelJob(
                        targetFolders, 
                        relativePath, 
                        source, 
                        isFolders: true, 
                        operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation) { Logger = this.Logger };

                    this.SyncQueue.Enqueue(job);
                }
            }
            else
            {
                Debug.WriteLine("File Deleted: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                // if (!ConfigurationPair.GetRelativeFiles().Contains(relativePath))
                // {
                // return;
                // }
                operation = new DeleteFile() { Configuration = this.Configuration, Logger = this.Logger };

                if (this.Configuration.IsParallelSync)
                {
                    targetFolders = this.CreateParallelJob(
                        targetFolders, 
                        relativePath, 
                        source, 
                        isFolders: false, 
                        operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFile(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation) { Logger = this.Logger };

                    this.SyncQueue.Enqueue(job);
                }
            }
        }

        /// <summary>
        /// Handles the Renamed event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="RenamedEventArgs"/> instance containing the event data.
        /// </param>
        private void FileSystemWatcherRenamed(object sender, RenamedEventArgs e)
        {
            ISyncJob job = null;
            SyncOperation operation;
            ISyncItem item;

            this.lastChangedElementPath = string.Empty;

            string source = e.FullPath;
            string relativePath = e.FullPath.Replace(this.ConfigurationPair.SoureFolder + "\\", string.Empty);
            string oldRelativePath = e.OldFullPath.Replace(this.ConfigurationPair.SoureFolder + "\\", string.Empty);
            List<string> targetFolders = new List<string>(this.ConfigurationPair.TargetFolders);

            if (Directory.Exists(e.FullPath))
            {
                Debug.WriteLine(
                    "Directory Renamed: From:{0} - To:{1} / {2}", 
                    e.OldFullPath, 
                    e.FullPath, 
                    e.ChangeType.ToString("g"));

                operation = new RenameFolder() { Configuration = this.Configuration, Logger = this.Logger };

                if (this.Configuration.IsParallelSync)
                {
                    targetFolders = this.CreateParallelRenameJob(
                        targetFolders, 
                        oldRelativePath, 
                        relativePath, 
                        isFolders: true, 
                        operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(
                        Path.Combine(targetFolder, oldRelativePath), 
                        Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation) { Logger = this.Logger };

                    this.SyncQueue.Enqueue(job);
                }
            }
            else
            {
                Debug.WriteLine(
                    "File Renamed: From:{0} - To:{1} / {2}", 
                    e.OldFullPath, 
                    e.FullPath, 
                    e.ChangeType.ToString("g"));

                operation = new RenameFile() { Configuration = this.Configuration, Logger = this.Logger };

                if (this.Configuration.IsParallelSync)
                {
                    targetFolders = this.CreateParallelRenameJob(
                        targetFolders, 
                        oldRelativePath, 
                        relativePath, 
                        false, 
                        operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFile(
                        Path.Combine(targetFolder, oldRelativePath), 
                        Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation) { Logger = this.Logger };

                    this.SyncQueue.Enqueue(job);
                }
            }
        }

        /// <summary>
        /// Initializes the file watcher.
        /// </summary>
        private void InitializeFileWatcher()
        {
            this.fileSystemWatcherInstance = new FileSystemWatcher(this.ConfigurationPair.SoureFolder)
            {
                IncludeSubdirectories = this.Configuration.IsRecursive, 
                NotifyFilter =
                    NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName
                    | NotifyFilters.LastWrite
            };

            this.fileSystemWatcherInstance.Changed += this.FileSystemWatcherChanged;
            this.fileSystemWatcherInstance.Created += this.FileSystemWatcherCreated;
            this.fileSystemWatcherInstance.Deleted += this.FileSystemWatcherDeleted;
            this.fileSystemWatcherInstance.Renamed += this.FileSystemWatcherRenamed;
        }

        /// <summary>
        /// Initializes the queue.
        /// </summary>
        private void InitializeQueue()
        {
            this.SyncQueue = new SyncQueue() { Logger = this.Logger };

            this.SyncQueue.QueueUpdated += (sender, e) => { this.OnStateUpdated(); };

            this.SyncQueue.StartQueue();
        }

        /// <summary>
        /// Runs the initial synchronization.
        /// </summary>
        private void RunInitialSync()
        {
            if (this.ComparerInstance == null)
            {
                throw new InvalidOperationException("Item comparer instance is not set!");
            }

            List<string> sourceFolders = this.ConfigurationPair.GetRelativeDirectories();
            List<string> sourceFiles = this.ConfigurationPair.GetRelativeFiles();

            this.RunSyncForRealtivePaths(sourceFolders, isFolders: true);
            this.RunSyncForRealtivePaths(sourceFiles);

            // Delete from Target where is no file/folder in source
            this.ConfigurationPair.GetRelativeItemsForTargets(ConfigurationPair.SearchItemType.File)
                .ToList()
                .ForEach(
                    (kvp) =>
                        {
                            var deleteFiles = kvp.Value.Except(sourceFiles).ToList();

                            if (deleteFiles.Count > 0)
                            {
                                this.DeleteNoSourceTargetItems(kvp.Key, deleteFiles, isFolders: false);
                            }
                        });

            this.ConfigurationPair.GetRelativeItemsForTargets(ConfigurationPair.SearchItemType.Folder)
                .ToList()
                .ForEach(
                    (kvp) =>
                        {
                            var deleteFolders = kvp.Value.Except(sourceFolders).ToList();

                            if (deleteFolders.Count > 0)
                            {
                                this.DeleteNoSourceTargetItems(kvp.Key, deleteFolders, isFolders: true);
                            }
                        });
        }

        /// <summary>
        /// Runs the synchronize.
        /// </summary>
        /// <param name="relativePaths">
        /// The relative paths.
        /// </param>
        /// <param name="isFolders">
        /// If set to <c>true</c> [is folders].
        /// </param>
        private void RunSyncForRealtivePaths(List<string> relativePaths, bool isFolders = false)
        {
            List<string> targetFolders;
            ISyncItem item = null;
            SyncOperation operation = null;
            ISyncJob job = null;

            foreach (string relativePath in relativePaths)
            {
                string source = Path.Combine(this.ConfigurationPair.SoureFolder, relativePath);

                // copy target folders list
                targetFolders = new List<string>(this.ConfigurationPair.TargetFolders);

                if (this.Configuration.IsParallelSync)
                {
                    targetFolders = this.CreateParallelJob(targetFolders, relativePath, source, isFolders);
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

                    operation = this.ComparerInstance.Compare(item);

                    if (operation != null)
                    {
                        operation.Configuration = this.Configuration;

                        job = new SyncJob(item, operation) { Logger = this.Logger };
                        this.SyncQueue.Enqueue(job);
                    }
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
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(message);
            }
        }
    }
}