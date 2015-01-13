// -----------------------------------------------------------------------
// <copyright file="ISyncItem.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ISyncItem.cs</summary>
// -----------------------------------------------------------------------

using System.IO;

namespace DataSync.Lib.Sync
{
    /// <summary>
    /// The ISyncItem Interface.
    /// </summary>
    public interface ISyncItem
    {
        /// <summary>
        /// Gets the source path.
        /// </summary>
        /// <value>
        /// The source path.
        /// </value>
        string SourcePath { get; }

        /// <summary>
        /// Gets a value indicating whether target file exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if target file exists; otherwise, <c>false</c>.
        /// </value>
        bool TargetExists { get; }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        /// <value>
        /// The target path.
        /// </value>
        string TargetPath { get; }

        /// <summary>
        /// Gets the source file system information.
        /// </summary>
        /// <returns></returns>
        FileSystemInfo GetSourceInfo();

        /// <summary>
        /// Gets the target file system information.
        /// </summary>
        /// <returns></returns>
        FileSystemInfo GetTargetInfo();
    }
}