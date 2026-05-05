using System;
using System.Collections.Generic;

namespace THPARKING.Business.Sync
{
    public interface ISyncOutboxStore
    {
        void Add(SyncOutboxItem item);

        IList<SyncOutboxItem> GetPending(int maxItems, DateTime now);

        void MarkProcessing(Guid syncOutboxItemId);

        void MarkSucceeded(Guid syncOutboxItemId);

        void MarkFailed(Guid syncOutboxItemId, string errorMessage, DateTime nextRetryAt);

        SyncOutboxItem GetById(Guid syncOutboxItemId);
    }
}