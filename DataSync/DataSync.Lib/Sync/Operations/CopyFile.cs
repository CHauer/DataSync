using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DataSync.Lib.Sync.Items;

namespace DataSync.Lib.Sync.Operations
{
    /// <summary>
    /// 
    /// </summary>
    [SupportedType(typeof(SyncFile))]
    public class CopyFile : ISyncOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyFile" /> class.
        /// </summary>
        public CopyFile() { }

        /// <summary>
        /// Runs the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException">item</exception>
        /// <exception cref="System.InvalidOperationException">The given item type is not valid for this sync operation!</exception>
        public void Execute(ISyncItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var supportedTypes = GetType().GetCustomAttributes(typeof(SupportedTypeAttribute), false)
                                .Select(attr => ((SupportedTypeAttribute)attr).SupportedType).ToList();

            if (!supportedTypes.Contains(item.GetType()))
            {
                throw new InvalidOperationException("The given item type is not valid for this sync operation!");
            }

            SyncFile file = item as SyncFile;

            if (file != null)
            {
                try
                {
                    File.Copy(file.SourcePath, file.TargetPath, true);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
