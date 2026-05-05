using System;
using System.Collections.Generic;
using System.Linq;

namespace THPARKING.Business.Sync
{
    public class InMemorySyncOutboxStore : ISyncOutboxStore
    {
        private readonly object _syncRoot = new object();

        private readonly List<SyncOutboxItem> _items = new List<SyncOutboxItem>();

        public void Add(SyncOutboxItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_syncRoot)
            {
                _items.Add(item);
            }
        }

        public IList<SyncOutboxItem> GetPending(int maxItems, DateTime now)
        {
            if (maxItems <= 0)
                maxItems = 10;

            lock (_syncRoot)
            {
                return _items
                    .Where(x =>
                        x.Status == SyncOutboxStatus.Pending ||
                        x.Status == SyncOutboxStatus.Failed)
                    .Where(x =>
                        !x.NextRetryAt.HasValue ||
                        x.NextRetryAt.Value <= now)
                    .OrderBy(x => x.CreatedAt)
                    .Take(maxItems)
                    .Select(Clone)
                    .ToList();
            }
        }

        public void MarkProcessing(Guid syncOutboxItemId)
        {
            lock (_syncRoot)
            {
                var item = _items.FirstOrDefault(x => x.SyncOutboxItemId == syncOutboxItemId);
                if (item == null)
                    return;

                item.Status = SyncOutboxStatus.Processing;
                item.LastTriedAt = DateTime.Now;
            }
        }

        public void MarkSucceeded(Guid syncOutboxItemId)
        {
            lock (_syncRoot)
            {
                var item = _items.FirstOrDefault(x => x.SyncOutboxItemId == syncOutboxItemId);
                if (item == null)
                    return;

                item.Status = SyncOutboxStatus.Succeeded;
                item.SucceededAt = DateTime.Now;
                item.LastErrorMessage = null;
            }
        }

        public void MarkFailed(Guid syncOutboxItemId, string errorMessage, DateTime nextRetryAt)
        {
            lock (_syncRoot)
            {
                var item = _items.FirstOrDefault(x => x.SyncOutboxItemId == syncOutboxItemId);
                if (item == null)
                    return;

                item.Status = SyncOutboxStatus.Failed;
                item.RetryCount++;
                item.LastErrorMessage = errorMessage;
                item.LastTriedAt = DateTime.Now;
                item.NextRetryAt = nextRetryAt;
            }
        }

        public SyncOutboxItem GetById(Guid syncOutboxItemId)
        {
            lock (_syncRoot)
            {
                var item = _items.FirstOrDefault(x => x.SyncOutboxItemId == syncOutboxItemId);
                return item == null ? null : Clone(item);
            }
        }

        private static SyncOutboxItem Clone(SyncOutboxItem source)
        {
            return new SyncOutboxItem
            {
                SyncOutboxItemId = source.SyncOutboxItemId,
                EventType = source.EventType,
                TargetType = source.TargetType,
                EntityType = source.EntityType,
                EntityId = source.EntityId,
                PayloadJson = source.PayloadJson,
                Status = source.Status,
                RetryCount = source.RetryCount,
                MaxRetryCount = source.MaxRetryCount,
                LastErrorMessage = source.LastErrorMessage,
                CreatedAt = source.CreatedAt,
                LastTriedAt = source.LastTriedAt,
                SucceededAt = source.SucceededAt,
                NextRetryAt = source.NextRetryAt,
                SourceMachineCode = source.SourceMachineCode,
                TargetAddress = source.TargetAddress
            };
        }
    }
}