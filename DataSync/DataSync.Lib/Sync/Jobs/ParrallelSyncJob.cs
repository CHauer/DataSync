using System;

namespace DataSync.Lib.Sync.Jobs
{
    public class ParrallelSyncJob : DataSync.Lib.Sync.ISyncJob
    {
        public List<DataSync.Lib.Sync.ISyncItem> Items
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


        public override void Run()
        {
            throw new NotImplementedException();
        }

        public JobStatus Status
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

        public ISyncOperation Operation
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
