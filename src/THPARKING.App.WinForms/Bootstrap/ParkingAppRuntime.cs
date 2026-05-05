using System;
using THPARKING.Business.Orchestration;
using THPARKING.Business.Sync;
using THPARKING.Licensing.Core;

namespace THPARKING.App.WinForms.Bootstrap
{
    public class ParkingAppRuntime : IDisposable
    {
        public ParkingAppContext Context { get; set; }

        public ParkingAppOrchestrator Orchestrator { get; set; }

        public LicenseGate LicenseGate { get; set; }

        public LicenseWorker LicenseWorker { get; set; }

        public SyncWorker SyncWorker { get; set; }

        public SyncOutboxService SyncOutboxService { get; set; }

        public ISyncOutboxStore SyncOutboxStore { get; set; }

        public void Start()
        {
            if (LicenseWorker != null)
                LicenseWorker.Start();

            if (SyncWorker != null)
                SyncWorker.Start();
        }

        public void Stop()
        {
            if (SyncWorker != null)
                SyncWorker.Stop();

            if (LicenseWorker != null)
                LicenseWorker.Stop();
        }

        public void Dispose()
        {
            Stop();

            if (SyncWorker != null)
                SyncWorker.Dispose();

            if (LicenseWorker != null)
                LicenseWorker.Dispose();
        }
    }
}