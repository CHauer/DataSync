// -----------------------------------------------------------------------
// <copyright file="ConfigurationPair.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ConfigurationPair.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using DataSync.Lib.Log;
    using DataSync.Lib.Log.Messages;

    /// <summary>
    /// The ConfigurationPair pair class.
    /// </summary>
    [Serializable]
    public class ConfigurationPair
    {
        /// <summary>
        /// The search item type enumeration.
        /// </summary>
        public enum SearchItemType
        {
            /// <summary>
            /// The folder value.
            /// </summary>
            Folder, 

            /// <summary>
            /// The file value.
            /// </summary>
            File
        }

        /// <summary>
        /// Gets or sets the except folders.
        /// </summary>
        /// <value>
        /// The except folders.
        /// </value>
        public List<string> ExceptFolders { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [XmlIgnore]
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name property.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the source folder.
        /// </summary>
        /// <value>
        /// The source folder.
        /// </value>
        public string SoureFolder { get; set; }

        /// <summary>
        /// Gets or sets the target folders.
        /// </summary>
        /// <value>
        /// The target folders.
        /// </value>
        public List<string> TargetFolders { get; set; }

        /// <summary>
        /// Gets the relative source directories.
        /// </summary>
        /// <returns>
        /// The relative directories.
        /// </returns>
        public List<string> GetRelativeDirectories()
        {
            List<string> sourceDirectories = this.GetDirectories(this.SoureFolder);

            return sourceDirectories.Select(d => d.Replace(this.SoureFolder + @"\", string.Empty)).ToList();
        }

        /// <summary>
        /// Gets the relative source files.
        /// </summary>
        /// <returns>
        /// The relative files.
        /// </returns>
        public List<string> GetRelativeFiles()
        {
            List<string> sourceFiles = this.GetFiles(this.SoureFolder);

            return sourceFiles.Select(d => d.Replace(this.SoureFolder + @"\", string.Empty)).ToList();
        }

        /// <summary>
        /// Gets the relative items for targets.
        /// </summary>
        /// <param name="type">
        /// The type parameter.
        /// </param>
        /// <returns>
        /// The relative target files.
        /// </returns>
        public Dictionary<string, List<string>> GetRelativeItemsForTargets(SearchItemType type = SearchItemType.Folder)
        {
            Dictionary<string, List<string>> relativeItems = new Dictionary<string, List<string>>();

            this.TargetFolders.ForEach(
                targetFolder =>
                    {
                        List<string> items;

                        if (type == SearchItemType.Folder)
                        {
                            items = this.GetDirectories(targetFolder);
                        }
                        else
                        {
                            items = this.GetFiles(targetFolder);
                        }

                        relativeItems.Add(
                            targetFolder, 
                            items.Select(d => d.Replace(targetFolder + @"\", string.Empty)).ToList());
                    });

            return relativeItems;
        }

        /// <summary>
        /// Gets the directories of the given base path folder.
        /// </summary>
        /// <param name="path">
        /// The search path.
        /// </param>
        /// <returns>
        /// The directories.
        /// </returns>
        private List<string> GetDirectories(string path)
        {
            List<string> directories = new List<string>();

            if (this.ExceptFolders.Contains(path))
            {
                return directories;
            }

            try
            {
                foreach (var info in new DirectoryInfo(path).EnumerateDirectories())
                {
                    if (!this.ExceptFolders.Contains(info.FullName))
                    {
                        directories.Add(info.FullName);
                    }

                    try
                    {
                        directories.AddRange(this.GetDirectories(info.FullName));
                    }
                    catch (Exception ex)
                    {
                        this.LogMessage(new WarningLogMessage(ex.Message));
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogMessage(new WarningLogMessage(ex.Message));
                Debug.WriteLine(ex.Message);
            }

            return directories;
        }

        /// <summary>
        /// Gets the files of the given base path.
        /// </summary>
        /// <param name="path">
        /// The search path.
        /// </param>
        /// <returns>
        /// The list of found files.
        /// </returns>
        private List<string> GetFiles(string path)
        {
            List<string> files = new List<string>();
            DirectoryInfo currentDirInfo = null;

            if (this.ExceptFolders.Contains(path))
            {
                return files;
            }

            try
            {
                currentDirInfo = new DirectoryInfo(path);
            }
            catch (Exception ex)
            {
                this.LogMessage(new WarningLogMessage(ex.Message));
                Debug.WriteLine(ex.Message);
                return files;
            }

            try
            {
                files.AddRange(currentDirInfo.EnumerateFiles().Select(fi => fi.FullName));
            }
            catch (Exception ex)
            {
                this.LogMessage(new WarningLogMessage(ex.Message));
                Debug.WriteLine(ex.Message);
            }

            foreach (var dirinfo in currentDirInfo.EnumerateDirectories())
            {
                // Go To Sub Directories
                files.AddRange(this.GetFiles(dirinfo.FullName));
            }

            return files;
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