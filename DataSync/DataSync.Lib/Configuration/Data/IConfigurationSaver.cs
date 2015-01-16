// -----------------------------------------------------------------------
// <copyright file="IConfigurationSaver.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - IConfigurationSaver.cs</summary>
// -----------------------------------------------------------------------

namespace DataSync.Lib.Configuration.Data
{
    /// <summary>
    /// The Configuration saver interface.
    /// </summary>
    public interface IConfigurationSaver
    {
        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        void SaveConfiguration(SyncConfiguration configuration);
    }
}