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
        /// The last changed element path.
        /// </summary>
        private string lastChangedElementPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncPair" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="configurationPair">The configuration pair.</param>
        public SyncPair(SyncConfiguration configuration, ConfigurationPair configurationPair)
        {
            this.Configuration = configuration;
            this.ConfigurationPair = configurationPair;

            //Standard sync item comparer
            this.ComparerInstance = new SyncItemComparer();
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
        /// Occurs when the sync pair state has updated.
        /// </summary>
        public event EventHandler StateUpdated;

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

            Task.Run(() =>
            {
                RunInitialSync();

                InitializeFileWatcher();

                //Filewatcher start - fire events
                fileSystemWatcherInstance.EnableRaisingEvents = true;
            });
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

            SyncQueue.QueueUpdated += (sender, e) => { OnStateUpdated(); };

            SyncQueue.StartQueue();
        }

        /// <summary>
        /// Stops the watcher.
        /// </summary>
        public void StopWatcher()
        {
            isWatching = false;

            //filewatcher stop
            fileSystemWatcherInstance.EnableRaisingEvents = false;

            SyncQueue.StopQueue();
        }

        /// <summary>
        /// Runs the initial synchronisation.
        /// </summary>
        private void RunInitialSync()
        {
            if (ComparerInstance == null)
            {
                throw new InvalidOperationException("Item comparer instance is not set!");
            }

            List<string> sourceFolders = ConfigurationPair.GetRelativeDirectories();
            List<string> sourceFiles = ConfigurationPair.GetRelativeFiles();

            RunSyncForRealtivePaths(sourceFolders, isFolders: true);
            RunSyncForRealtivePaths(sourceFiles);

            //Delte from Target where is no file/folder in source
            ConfigurationPair.GetRelativeItemsForTargets(ConfigurationPair.SearchItemType.File).ToList().ForEach((kvp) =>
            {
                var deleteFiles = kvp.Value.Except(sourceFiles).ToList();

                if (deleteFiles.Count > 0)
                {
                    DeleteNoSourceTargetItems(kvp.Key, deleteFiles, isFolders: false);
                }
            });

            ConfigurationPair.GetRelativeItemsForTargets(ConfigurationPair.SearchItemType.Folder).ToList().ForEach((kvp) =>
            {
                var deleteFolders = kvp.Value.Except(sourceFiles).ToList();

                if (deleteFolders.Count > 0)
                {
                    DeleteNoSourceTargetItems(kvp.Key, deleteFolders, isFolders: true);
                }
            });
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
            SyncOperation operation = null;
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

                    if (operation != null)
                    {
                        operation.Configuration = Configuration;

                        job = new SyncJob(item, operation)
                        {
                            Logger = Logger
                        };
                        SyncQueue.Enqueue(job);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the no source target items.
        /// </summary>
        /// <param name="targetFolder">The target folder.</param>
        /// <param name="deleteTargetRelativePaths">The delete target paths.</param>
        /// <param name="isFolders">if set to <c>true</c> [is folders].</param>
        private void DeleteNoSourceTargetItems(string targetFolder, List<string> deleteTargetRelativePaths, bool isFolders = false)
        {
            ISyncItem item = null;
            SyncOperation operation = null;
            ISyncJob job = null;

            foreach (string relativePath in deleteTargetRelativePaths)
            {
                string source = Path.Combine(ConfigurationPair.SoureFolder, relativePath);
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

                operation.Configuration = Configuration;

                job = new SyncJob(item, operation)
                {
                    Logger = Logger
                };
                SyncQueue.Enqueue(job);
                
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
        private List<string> CreateParallelJob(List<string> targetFolders, string relativePath, string source, bool isFolders, SyncOperation operation = null)
        {
            ISyncItem item;
            ISyncJob job;
            Dictionary<ISyncItem, SyncOperation> parallelJobs = new Dictionary<ISyncItem, SyncOperation>();

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

                if (operation == null)
                {
                    operation = ComparerInstance.Compare(item);
                }

                if (operation != null)
                {
                    operation.Configuration = Configuration;
                    operation.Logger = Logger;

                    parallelJobs.Add(item, operation);
                }

                //remove used target folder
                targetFolders.Remove(targetFolder);
            }

            if (parallelJobs.Count > 0)
            {
                job = new ParallelSyncJob(parallelJobs)
                {
                    Logger = Logger
                };

                SyncQueue.Enqueue(job);
            }

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
                IncludeSubdirectories = Configuration.IsRecursiv,
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.DirectoryName |
                               NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            fileSystemWatcherInstance.Changed += FileSystemWatcher_Changed;
            fileSystemWatcherInstance.Created += FileSystemWatcher_Created;
            fileSystemWatcherInstance.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcherInstance.Renamed += FileSystemWatcher_Renamed;
        }

        /// <summary>
        /// Handles the Renamed event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            ISyncJob job = null;
            SyncOperation operation;
            ISyncItem item;

            lastChangedElementPath = string.Empty;

            string source = e.FullPath;
            string relativePath = e.FullPath.Replace(ConfigurationPair.SoureFolder, string.Empty);
            List<string> targetFolders = new List<string>(ConfigurationPair.TargetFolders);

            if (Directory.Exists(e.FullPath))
            {
                Debug.WriteLine("Directory Renamed: From:{0} - To:{1} / {2}", e.OldFullPath, e.FullPath, e.ChangeType.ToString("g"));

                operation = new RenameFolder()
                {
                    Configuration = this.Configuration,
                    Logger = this.Logger
                };

                if (Configuration.IsParrallelSync)
                {
                    targetFolders = CreateParallelJob(targetFolders, relativePath, source,
                                                        isFolders: true, operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation)
                    {
                        Logger = Logger
                    };

                    SyncQueue.Enqueue(job);
                }
            }
            else
            {
                Debug.WriteLine("File Renamed: From:{0} - To:{1} / {2}", e.OldFullPath, e.FullPath, e.ChangeType.ToString("g"));

                operation = new RenameFile()
                {
                    Configuration = this.Configuration,
                    Logger = this.Logger
                };

                if (Configuration.IsParrallelSync)
                {
                    targetFolders = CreateParallelJob(targetFolders, relativePath, source, false, operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFile(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation)
                    {
                        Logger = Logger
                    };

                    SyncQueue.Enqueue(job);
                }
            }
        }

        /// <summary>
        /// Handles the Deleted event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            ISyncJob job = null;
            SyncOperation operation;
            ISyncItem item;

            lastChangedElementPath = string.Empty;

            string source = e.FullPath;
            string relativePath = e.FullPath.Replace(ConfigurationPair.SoureFolder, string.Empty);
            List<string> targetFolders = new List<string>(ConfigurationPair.TargetFolders);


            //IMPORTANT - directory.exist false way - directory already deleted
            if (String.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
            {
                Debug.WriteLine("Directory Deleted: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                //handle directory delete
                operation = new DeleteFolder()
                {
                    Configuration = this.Configuration,
                    Logger = this.Logger
                };

                if (Configuration.IsParrallelSync)
                {
                    targetFolders = CreateParallelJob(targetFolders, relativePath, source,
                                                        isFolders: true, operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation)
                    {
                        Logger = Logger
                    };

                    SyncQueue.Enqueue(job);
                }

            }
            else
            {
                Debug.WriteLine("File Deleted: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                operation = new DeleteFolder()
                {
                    Configuration = this.Configuration,
                    Logger = this.Logger
                };

                if (Configuration.IsParrallelSync)
                {
                    targetFolders = CreateParallelJob(targetFolders, relativePath, source,
                                                        isFolders: false, operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFile(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation)
                    {
                        Logger = Logger
                    };

                    SyncQueue.Enqueue(job);
                }

            }
        }

        /// <summary>
        /// Handles the Created event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            ISyncJob job = null;
            SyncOperation operation;
            ISyncItem item;

            lastChangedElementPath = string.Empty;

            string source = e.FullPath;
            string relativePath = e.FullPath.Replace(ConfigurationPair.SoureFolder, string.Empty);
            List<string> targetFolders = new List<string>(ConfigurationPair.TargetFolders);

            if (Directory.Exists(e.FullPath))
            {
                Debug.WriteLine("Directory Created: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                operation = new CreateFolder()
                {
                    Configuration = this.Configuration,
                    Logger = this.Logger
                };

                if (Configuration.IsParrallelSync)
                {
                    targetFolders = CreateParallelJob(targetFolders, relativePath, source,
                                                        isFolders: true, operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation)
                    {
                        Logger = Logger
                    };

                    SyncQueue.Enqueue(job);
                }
            }
            else
            {
                Debug.WriteLine("File Created: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                operation = new CopyFile()
                {
                    Configuration = this.Configuration,
                    Logger = this.Logger
                };

                if (Configuration.IsParrallelSync)
                {
                    targetFolders = CreateParallelJob(targetFolders, relativePath, source,
                                                        isFolders: true, operation: operation);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(source, Path.Combine(targetFolder, relativePath));

                    job = new SyncJob(item, operation)
                    {
                        Logger = Logger
                    };

                    SyncQueue.Enqueue(job);
                }
            }
        }

        /// <summary>
        /// Handles the Changed event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ISyncJob job = null;
            SyncOperation operation;
            ISyncItem item;

            string source = e.FullPath;
            string relativePath = e.FullPath.Replace(ConfigurationPair.SoureFolder, string.Empty);
            List<string> targetFolders = new List<string>(ConfigurationPair.TargetFolders);

            if (Directory.Exists(e.FullPath))
            {
                Debug.WriteLine("Directory Changed: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                if (Configuration.IsParrallelSync)
                {
                    targetFolders = CreateParallelJob(targetFolders, relativePath, source,
                                                        isFolders: true);
                }

                foreach (string targetFolder in targetFolders)
                {
                    item = new SyncFolder(source, Path.Combine(targetFolder, relativePath));

                    //compare item
                    operation = ComparerInstance.Compare(item);

                    if (operation == null)
                    {
                        //no sync operation needed
                        return;
                    }

                    job = new SyncJob(item, operation)
                    {
                        Logger = Logger
                    };

                    SyncQueue.Enqueue(job);
                }
            }
            else
            {
                Debug.WriteLine("File Changed: {0} {1}", e.FullPath, e.ChangeType.ToString("g"));

                if (!e.FullPath.Equals(lastChangedElementPath))
                {
                    lastChangedElementPath = e.FullPath;

                    if (Configuration.IsParrallelSync)
                    {
                        targetFolders = CreateParallelJob(targetFolders, relativePath, source,
                                                            isFolders: false);
                    }

                    foreach (string targetFolder in targetFolders)
                    {
                        item = new SyncFile(source, Path.Combine(targetFolder, relativePath));

                        //compare item
                        operation = ComparerInstance.Compare(item);

                        if (operation == null)
                        {
                            //no sync operation needed
                            return;
                        }

                        job = new SyncJob(item, operation)
                        {
                            Logger = Logger
                        };

                        SyncQueue.Enqueue(job);
                    }
                }
                else
                {
                    lastChangedElementPath = string.Empty;
                    return;
                }
            }
        }

        /// <summary>
        /// Called when the sync pair state has updated.
        /// </summary>
        protected virtual void OnStateUpdated()
        {
            // ReSharper disable once UseNullPropagation
            if (StateUpdated != null)
            {
                StateUpdated(this, EventArgs.Empty);
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

            builder.AppendLine(String.Format("Synchronization Pair {0}", ConfigurationPair.Name));
            builder.AppendLine(String.Format("Source Folder: {0}", ConfigurationPair.SoureFolder));

            foreach (string target in ConfigurationPair.TargetFolders)
            {
                builder.AppendLine(String.Format("Target Folder: {0}", target));
            }

            foreach (string except in ConfigurationPair.ExceptFolders)
            {
                builder.AppendLine(String.Format("Except Folder: {0}", except));
            }

            foreach (var job in SyncQueue.Jobs)
            {
                job.ToString(new List<int> { 20, 20, 20, 10, 10 });
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(LogMessage message)
        {
            if (Logger != null)
            {
                Logger.AddLogMessage(message);
            }
        }
    }
}
