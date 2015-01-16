using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DataSync.Lib.Log;
using DataSync.Lib.Log.Messages;

namespace DataSync.Lib.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ConfigurationPair
    {
        /// <summary>
        /// Gets or sets the soure folder.
        /// </summary>
        /// <value>
        /// The soure folder.
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
        /// Gets or sets the except folders.
        /// </summary>
        /// <value>
        /// The except folders.
        /// </value>
        public List<string> ExceptFolders { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        [XmlIgnore]
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets the relative directories.
        /// </summary>
        /// <returns></returns>
        public List<string> GetRelativeDirectories()
        {
            List<string> sourceDirectories = GetDirectories(SoureFolder);

            return sourceDirectories.Select(d => d.Replace(SoureFolder + @"\", string.Empty)).ToList();
        }

        /// <summary>
        /// Gets the relative files.
        /// </summary>
        /// <returns></returns>
        public List<string> GetRelativeFiles()
        {
            List<string> sourceFiles = GetDirectories(SoureFolder);

            return sourceFiles.Select(d => d.Replace(SoureFolder + @"\", string.Empty)).ToList();
        }

        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private List<string> GetDirectories(string path)
        {
            List<string> directories = new List<string>();

            if (ExceptFolders.Contains(path))
            {
                return directories;
            }

            try
            {
                foreach (var info in new DirectoryInfo(path).EnumerateDirectories())
                {
                    directories.Add(info.FullName);
                    try
                    {
                        directories.AddRange(GetDirectories(info.FullName));
                    }
                    catch (Exception ex)
                    {
                        LogMessage(new WarningLogMessage(ex.Message)); 
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage(new WarningLogMessage(ex.Message));
                Debug.WriteLine(ex.Message);
            }

            return directories;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private List<string> GetFiles(string path)
        {
            List<string> files = new List<string>();
            DirectoryInfo currentDirInfo = null;

            if (ExceptFolders.Contains(path))
            {
                return files;
            }

            try
            {
                currentDirInfo = new DirectoryInfo(path);
            }
            catch (Exception ex)
            {
                LogMessage(new WarningLogMessage(ex.Message));
                Debug.WriteLine(ex.Message);
                return files;
            }

            try
            {
                files.AddRange(currentDirInfo.EnumerateFiles().Select(fi => fi.FullName));
            }
            catch (Exception ex)
            {
                LogMessage(new WarningLogMessage(ex.Message));
                Debug.WriteLine(ex.Message);
            }

            foreach (var dirinfo in currentDirInfo.EnumerateDirectories())
            {
                //Go To Sub Directories
                files.AddRange(GetFiles(dirinfo.FullName));
            }

            return files;
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
