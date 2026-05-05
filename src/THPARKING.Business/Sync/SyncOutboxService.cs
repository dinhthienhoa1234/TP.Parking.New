using System;

namespace THPARKING.Business.Sync
{
    public class SyncOutboxService
    {
        private readonly ISyncOutboxStore _store;

        public SyncOutboxService(ISyncOutboxStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public SyncOutboxItem Enqueue(
            SyncEventType eventType,
            SyncTargetType targetType,
            string entityType,
            string entityId,
            string payloadJson,
            string sourceMachineCode,
            string targetAddress)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException("Entity type is required.", nameof(entityType));

            if (string.IsNullOrWhiteSpace(entityId))
                throw new ArgumentException("Entity id is required.", nameof(entityId));

            var item = new SyncOutboxItem
            {
                SyncOutboxItemId = Guid.NewGuid(),
                EventType = eventType,
                TargetType = targetType,
                EntityType = entityType,
                EntityId = entityId,
                PayloadJson = payloadJson,
                Status = SyncOutboxStatus.Pending,
                RetryCount = 0,
                MaxRetryCount = 100,
                CreatedAt = DateTime.Now,
                SourceMachineCode = sourceMachineCode,
                TargetAddress = targetAddress
            };

            _store.Add(item);

            return item;
        }

        public DateTime CalculateNextRetryAt(int retryCount)
        {
            if (retryCount <= 0)
                return DateTime.Now.AddSeconds(10);

            if (retryCount == 1)
                return DateTime.Now.AddSeconds(30);

            if (retryCount == 2)
                return DateTime.Now.AddMinutes(1);

            if (retryCount == 3)
                return DateTime.Now.AddMinutes(3);

            if (retryCount == 4)
                return DateTime.Now.AddMinutes(5);

            return DateTime.Now.AddMinutes(10);
        }
    }
}