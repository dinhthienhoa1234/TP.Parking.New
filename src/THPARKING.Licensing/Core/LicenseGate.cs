using System;
using THPARKING.Core.Enums;

namespace THPARKING.Licensing.Core
{
    public class LicenseGate
    {
        private readonly LocalLicenseStore _store;
        private readonly LicenseMachineIdProvider _machineIdProvider;
        private readonly LicenseTokenValidator _validator;

        public LicenseGate(
            LocalLicenseStore store,
            LicenseMachineIdProvider machineIdProvider,
            LicenseTokenValidator validator)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _machineIdProvider = machineIdProvider ?? throw new ArgumentNullException(nameof(machineIdProvider));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public LicenseCheckResult Check()
        {
            var machineId = _machineIdProvider.GetMachineId();
            var localData = _store.Load();

            return _validator.Validate(localData, machineId);
        }

        public bool Can(LicensePermission permission)
        {
            var result = Check();

            if (result.IsValid)
                return true;

            if (result.Status == LicenseStatus.Expired)
            {
                return permission == LicensePermission.AllowEmergencyVehicleOut;
            }

            return false;
        }
    }
}