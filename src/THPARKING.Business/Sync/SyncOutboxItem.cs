using System;

namespace THPARKING.Business.Sync
{
    public class SyncOutboxItem
    {
        public Guid SyncOutboxItemId { get; set; }

        public SyncEventType EventType { get; set; }

        public SyncTargetType TargetType { get; set; }

        public string EntityType { get; set; }

        public string EntityId { get; set; }

        public string PayloadJson { get; set; }

        public SyncOutboxStatus Status { get; set; }

        public int RetryCount { get; set; }

        public int MaxRetryCount { get; set; }

        public string LastErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastTriedAt { get; set; }

        public DateTime? SucceededAt { get; set; }

        public DateTime? NextRetryAt { get; set; }

        public string SourceMachineCode { get; set; }

        public string TargetAddress { get; set; }
    }
}