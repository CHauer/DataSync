// -----------------------------------------------------------------------
// <copyright file="IConfigurationLoader.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - IConfigurationLoader.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Configuration.Data
{
    /// <summary>
    /// The Configuration loader interface.
    /// </summary>
    public interface IConfigurationLoader
    {
        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="SyncConfiguration"/>.
        /// </returns>
        SyncConfiguration LoadConfiguration();
    }
}