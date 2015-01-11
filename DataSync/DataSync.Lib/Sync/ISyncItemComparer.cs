using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.Lib.Sync
{
    public interface ISyncItemComparer
    {
        ISyncOperation Compare(ISyncItem compareItem);
    }
}
