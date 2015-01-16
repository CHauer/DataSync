using System;
using System.Collections.Generic;
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
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns></returns>
        public ConfigurationPair Parse()
        {
            ConfigurationPair pair = new ConfigurationPair()
            {
                Logger = Logger
            };

            return pair;
        }
    }
}
