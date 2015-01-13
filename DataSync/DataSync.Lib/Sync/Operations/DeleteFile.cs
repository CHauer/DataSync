using System;

namespace DataSync.Lib.Sync.Operations
{
    public class DeleteFile : ISyncOperation
    {

        public bool Execute(ISyncItem item)
        {
            throw new NotImplementedException();
        }

        public Log.ILog Logger
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public Configuration.SyncConfiguration Configuration
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
