using DataSync.Lib.Sync.Jobs;

namespace DataSync.Lib.Sync
{

    public interface ISyncJob
    {

        JobStatus Status
        {
            get;
            set;
        }

        ISyncOperation Operation
        {
            get;
            set;
        }

        void Run();
    }
}
