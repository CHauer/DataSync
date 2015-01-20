using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataSync.Lib.Configuration;
using DataSync.Lib.Log;
using DataSync.Lib.Sync;

namespace DataSync.UI.Arguments
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncPairDefinitionParser
    {
        /// <summary>
        /// The synchronize pair definition
        /// </summary>
        private string syncPairDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncPairDefinitionParser"/> class.
        /// </summary>
        /// <param name="syncPairDefinition">The synchronize pair definition.</param>
        public SyncPairDefinitionParser(string syncPairDefinition)
        {
            this.syncPairDefinition = syncPairDefinition;
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">The syncpair is empty!</exception>
        /// <exception cref="System.IO.InvalidDataException">
        /// The syncpair definition can only contain oneexcept list - letter '&lt;' appears more than once!
        /// or
        /// The syncpair definition has to contain one source folder and at least one target folder!
        /// or
        /// The syncpair definition has to contain a sourcefolder - delimited by '&gt;'.
        /// </exception>
        public ConfigurationPair Parse()
        {
            string sourceFolder;
            string target;
            string exept = string.Empty;

            if (string.IsNullOrWhiteSpace(syncPairDefinition))
            {
                throw new ArgumentException("The syncpair is empty!");
            }

            syncPairDefinition = syncPairDefinition.Trim();

            //Except list
            if (syncPairDefinition.Contains("<"))
            {
                var parts = syncPairDefinition.Split(new string[] { "<" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    syncPairDefinition = parts[0];
                    exept = parts[1];
                }
                else
                {
                    throw new InvalidDataException("The syncpair definition can only contain one" +
                                                   " except list - letter '<' appears more than once!");
                }
            }

            //source > target
            if (syncPairDefinition.Contains(">"))
            {
                var parts = syncPairDefinition.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    sourceFolder = parts[0];
                    target = parts[1];

                    if (sourceFolder.Contains("\""))
                    {
                        sourceFolder = sourceFolder.Replace("\"", string.Empty);
                    }

                    //throws exception if sourcefolder is invalid!
                    sourceFolder = Path.GetFullPath(sourceFolder);
                }
                else
                {
                    throw new InvalidDataException("The syncpair definition has to contain one" +
                                                   " source folder and at least one target folder!");
                }
            }
            else
            {
                throw new InvalidDataException("The syncpair definition has to contain" +
                                               " a sourcefolder - delimited by '>'.");
            }

            return new ConfigurationPair()
            {
                SoureFolder = sourceFolder,
                TargetFolders = ParseFolderList(target),
                ExceptFolders = ParseFolderList(exept)
            };
        }

        /// <summary>
        /// Parses the folder list.
        /// </summary>
        /// <param name="folderlist">The folderlist.</param>
        /// <returns></returns>
        private List<string> ParseFolderList(string folderlist)
        {
            List<string> folders = new List<string>();

            var parts = folderlist.Split(new char[] { '|' });

            parts.ToList().ForEach(part =>
            {
                if (part.Contains("\""))
                {
                    part = part.Replace("\"", string.Empty);
                }

                //throws exception if sourcefolder is invalid!
                folders.Add(Path.GetFullPath(part));
            });

            return folders;
        }
    }
}
