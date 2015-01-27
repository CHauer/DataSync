// -----------------------------------------------------------------------
// <copyright file="ISyncItem.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - ISyncItem.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Sync
{
    using System.IO;

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
        /// <c>true</c> if target file exists; otherwise, <c>false</c>.
        /// </value>
        bool TargetExists { get; }

        /// <summary>
        /// Gets the target path.
        /// </summary>
        /// <value>
        /// The target path.
        /// </value>
        string TargetPath { get; }

        /// <summary>
        /// Gets the source file system information.
        /// </summary>
        /// <returns>
        /// The <see cref="FileSystemInfo"/>.
        /// </returns>
        FileSystemInfo GetSourceInfo();

        /// <summary>
        /// Gets the target file system information.
        /// </summary>
        /// <returns>
        /// The <see cref="FileSystemInfo"/>.
        /// </returns>
        FileSystemInfo GetTargetInfo();
    }
}