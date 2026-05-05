using System;
using System.IO;
using THPARKING.Business.InOut;
using THPARKING.Business.Orchestration;
using THPARKING.Business.Sync;
using THPARKING.Device.CardReaders;
using THPARKING.Licensing.Core;

namespace THPARKING.App.WinForms.Bootstrap
{
    public class ParkingAppBootstrapper
    {
        public ParkingAppRuntime BuildDefaultRuntime(
            string operatorCode,
            string machineName)
        {
            var licenseFilePath = BuildLicenseFilePath();

            var licenseStore = new LocalLicenseStore(licenseFilePath);
            var machineIdProvider = new LicenseMachineIdProvider();
            var tokenValidator = new LicenseTokenValidator();

            var licenseGate = new LicenseGate(
                licenseStore,
                machineIdProvider,
                tokenValidator);

            var licenseWorker = new LicenseWorker(licenseGate);

            var syncStore = new InMemorySyncOutboxStore();
            var syncOutboxService = new SyncOutboxService(syncStore);
            var syncWorker = new SyncWorker(syncStore, syncOutboxService);

            var feeCalculator = new ParkingFeeCalculator();
            var plateChecker = new DuplicatePlateChecker();

            var inOutBusinessService = new InOutBusinessService(
                licenseGate,
                syncOutboxService,
                feeCalculator,
                plateChecker);

            var appContext = new ParkingAppContext
            {
                OperatorCode = string.IsNullOrWhiteSpace(operatorCode) ? "admin" : operatorCode,
                MachineName = string.IsNullOrWhiteSpace(machineName) ? Environment.MachineName : machineName,

                CardCodeService = new CardCodeService(),

                // Camera sẽ gắn thật ở branch sau.
                PlateCameraService = null,
                FaceCameraService = null,

                InOutBusinessService = inOutBusinessService
            };

            var orchestrator = new ParkingAppOrchestrator(appContext);

            return new ParkingAppRuntime
            {
                Context = appContext,
                Orchestrator = orchestrator,
                LicenseGate = licenseGate,
                LicenseWorker = licenseWorker,
                SyncOutboxStore = syncStore,
                SyncOutboxService = syncOutboxService,
                SyncWorker = syncWorker
            };
        }

        private string BuildLicenseFilePath()
        {
            var appDataFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);

            var folder = Path.Combine(
                appDataFolder,
                "THPARKING",
                "License");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return Path.Combine(folder, "license.dat");
        }
    }
}