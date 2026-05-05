using System;
using THPARKING.Core.Enums;

namespace THPARKING.Licensing.Core
{
    public class LicenseClient
    {
        public LocalLicenseData ActivateTemporary(
            string gmailOwner,
            string machineId,
            int days)
        {
            if (string.IsNullOrWhiteSpace(gmailOwner))
                throw new ArgumentException("Gmail owner is required.", nameof(gmailOwner));

            if (string.IsNullOrWhiteSpace(machineId))
                throw new ArgumentException("Machine id is required.", nameof(machineId));

            if (days <= 0)
                days = 1;

            if (days > 3)
                days = 3;

            var fingerprintService = new LicenseFingerprintService();

            return new LocalLicenseData
            {
                GmailOwner = gmailOwner.Trim().ToLowerInvariant(),
                MachineId = machineId,
                Fingerprint = fingerprintService.CreateFingerprint(gmailOwner, machineId),
                Mode = LicenseMode.TemporaryOffline,
                ExpiredAt = DateTime.Now.Date.AddDays(days),
                LastCheckedAt = DateTime.Now,
                SignedToken = "TEMPORARY-OFFLINE-TOKEN"
            };
        }
    }
}