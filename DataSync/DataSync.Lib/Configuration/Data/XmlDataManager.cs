using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.Lib.Configuration.Data
{
    public class XmlConfigurationSerializer : IConfigurationLoader, IConfigurationSaver
    {
        public SyncConfiguration LoadConfiguration()
        {
            throw new NotImplementedException();
        }

        public bool SaveConfiguration(SyncConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the configuration file.
        /// </summary>
        /// <value>
        /// The configuration file.
        /// </value>
        public string ConfigurationFile { get; set; }
    }
}
