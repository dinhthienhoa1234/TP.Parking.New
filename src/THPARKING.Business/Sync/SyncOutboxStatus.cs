namespace THPARKING.Business.Sync
{
    public enum SyncOutboxStatus
    {
        Pending = 0,
        Processing = 1,
        Succeeded = 2,
        Failed = 3,
        Cancelled = 4
    }
}