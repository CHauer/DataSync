// -----------------------------------------------------------------------
// <copyright file="XmlConfigurationSerializer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - XmlConfigurationSerializer.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Configuration.Data
{
    using System.IO;
    using System.Xml.Serialization;

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
            this.serializer = new XmlSerializer(typeof(SyncConfiguration));
        }

        /// <summary>
        /// Gets or sets the configuration file.
        /// </summary>
        /// <value>
        /// The configuration file.
        /// </value>
        public string ConfigurationFile { get; set; }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="SyncConfiguration"/>.
        /// </returns>
        public SyncConfiguration LoadConfiguration()
        {
            using (var file = File.OpenRead(this.ConfigurationFile))
            {
                return (SyncConfiguration)this.serializer.Deserialize(file);
            }
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public void SaveConfiguration(SyncConfiguration configuration)
        {
            using (var file = File.Open(this.ConfigurationFile, FileMode.Create, FileAccess.Write))
            {
                this.serializer.Serialize(file, configuration);
            }
        }
    }
}