namespace THPARKING.Business.Sync
{
    public class SyncResult
    {
        public bool Success { get; private set; }

        public string Message { get; private set; }

        public static SyncResult Ok(string message)
        {
            return new SyncResult
            {
                Success = true,
                Message = message
            };
        }

        public static SyncResult Fail(string message)
        {
            return new SyncResult
            {
                Success = false,
                Message = message
            };
        }
    }
}