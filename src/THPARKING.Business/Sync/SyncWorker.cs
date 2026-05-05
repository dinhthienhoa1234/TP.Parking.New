using System;
using System.Timers;

namespace THPARKING.Business.Sync
{
    public class SyncWorker : IDisposable
    {
        private readonly ISyncOutboxStore _store;
        private readonly SyncOutboxService _outboxService;
        private readonly Timer _timer;
        private bool _isRunning;

        public SyncWorker(
            ISyncOutboxStore store,
            SyncOutboxService outboxService)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _outboxService = outboxService ?? throw new ArgumentNullException(nameof(outboxService));

            _timer = new Timer();
            _timer.Interval = TimeSpan.FromSeconds(15).TotalMilliseconds;
            _timer.Elapsed += OnTimerElapsed;
        }

        public event EventHandler<SyncOutboxItem> SyncSucceeded;

        public event EventHandler<SyncOutboxItem> SyncFailed;

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isRunning)
                return;

            try
            {
                _isRunning = true;
                ProcessPendingItems();
            }
            finally
            {
                _isRunning = false;
            }
        }

        public void ProcessPendingItems()
        {
            var now = DateTime.Now;
            var items = _store.GetPending(20, now);

            foreach (var item in items)
            {
                ProcessItem(item);
            }
        }

        private void ProcessItem(SyncOutboxItem item)
        {
            if (item == null)
                return;

            if (item.RetryCount >= item.MaxRetryCount)
                return;

            _store.MarkProcessing(item.SyncOutboxItemId);

            var result = SendToTarget(item);

            if (result.Success)
            {
                _store.MarkSucceeded(item.SyncOutboxItemId);
                RaiseSyncSucceeded(item);
                return;
            }

            var nextRetryAt = _outboxService.CalculateNextRetryAt(item.RetryCount + 1);

            _store.MarkFailed(
                item.SyncOutboxItemId,
                result.Message,
                nextRetryAt);

            RaiseSyncFailed(item);
        }

        private SyncResult SendToTarget(SyncOutboxItem item)
        {
            if (string.IsNullOrWhiteSpace(item.TargetAddress))
            {
                return SyncResult.Fail("Chưa có địa chỉ máy đích hoặc server đích.");
            }

            // TODO:
            // Sau này thay bằng:
            // - HTTP API client
            // - LAN peer client
            // - SQL replication bridge
            // - Message queue nếu có
            return SyncResult.Fail("Sync transport chưa được cài đặt.");
        }

        private void RaiseSyncSucceeded(SyncOutboxItem item)
        {
            var handler = SyncSucceeded;
            if (handler != null)
                handler(this, item);
        }

        private void RaiseSyncFailed(SyncOutboxItem item)
        {
            var handler = SyncFailed;
            if (handler != null)
                handler(this, item);
        }

        public void Dispose()
        {
            Stop();
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();
        }
    }
}