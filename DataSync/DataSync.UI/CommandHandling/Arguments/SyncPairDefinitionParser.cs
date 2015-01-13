using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSync.Lib.Configuration;
using DataSync.Lib.Sync;

namespace DataSync.UI.CommandHandling.Arguments
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
        public ConfigurationPair Parse()
        {
            ConfigurationPair pair = new ConfigurationPair();

            return pair;
        }
    }
}
