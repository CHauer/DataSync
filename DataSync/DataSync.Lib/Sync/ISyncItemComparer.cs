// -----------------------------------------------------------------------
// <copyright file="ISyncItemComparer.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ISyncItemComparer.cs</summary>
// -----------------------------------------------------------------------

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISyncItemComparer
    {
        /// <summary>
        /// Compares the specified compare item.
        /// </summary>
        /// <param name="compareItem">The compare item.</param>
        /// <returns></returns>
        ISyncOperation Compare(ISyncItem compareItem);
    }
}