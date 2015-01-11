using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.Lib.Configuration.Data
{
    public interface IConfigurationSaver
    {
        bool SaveConfiguration(SyncConfiguration configuration);
    }
}
