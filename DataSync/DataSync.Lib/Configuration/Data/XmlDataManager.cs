// -----------------------------------------------------------------------
// <copyright file="XmlDataManager.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - XmlDataManager.cs</summary>
// -----------------------------------------------------------------------

using System.IO;
using System.Xml.Serialization;

namespace DataSync.Lib.Configuration.Data
{
    /// <summary>
    /// The  XmlSerializer - implements ConfigurationSaver and Loader interface.
    /// </summary>
    public class XmlConfigurationSerializer : IConfigurationLoader, IConfigurationSaver
    {
        /// <summary>
        /// The serializer.
        /// </summary>
        private XmlSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlConfigurationSerializer"/> class.
        /// </summary>
        public XmlConfigurationSerializer()
        {
            this.serializer = new XmlSerializer(typeof (SyncConfiguration));
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns></returns>
        public SyncConfiguration LoadConfiguration()
        {
            return (SyncConfiguration) this.serializer.Deserialize(File.OpenRead(this.ConfigurationFile));
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void SaveConfiguration(SyncConfiguration configuration)
        {
            this.serializer.Serialize(File.Open(this.ConfigurationFile, FileMode.Create, FileAccess.Write),
                configuration);
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