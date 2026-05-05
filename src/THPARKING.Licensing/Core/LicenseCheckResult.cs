using System;
using THPARKING.Core.Enums;

namespace THPARKING.Licensing.Core
{
    public class LicenseCheckResult
    {
        public bool IsValid { get; private set; }

        public LicenseStatus Status { get; private set; }

        public LicenseMode Mode { get; private set; }

        public string GmailOwner { get; private set; }

        public string MachineId { get; private set; }

        public DateTime? ExpiredAt { get; private set; }

        public int RemainingDays { get; private set; }

        public string Message { get; private set; }

        public static LicenseCheckResult Valid(
            LicenseMode mode,
            string gmailOwner,
            string machineId,
            DateTime expiredAt,
            string message)
        {
            var remainingDays = (int)Math.Ceiling((expiredAt.Date - DateTime.Now.Date).TotalDays);

            return new LicenseCheckResult
            {
                IsValid = true,
                Status = remainingDays <= 3 ? LicenseStatus.Warning : LicenseStatus.Active,
                Mode = mode,
                GmailOwner = gmailOwner,
                MachineId = machineId,
                ExpiredAt = expiredAt,
                RemainingDays = remainingDays,
                Message = message
            };
        }

        public static LicenseCheckResult Invalid(
            LicenseStatus status,
            LicenseMode mode,
            string gmailOwner,
            string machineId,
            DateTime? expiredAt,
            string message)
        {
            var remainingDays = 0;

            if (expiredAt.HasValue)
                remainingDays = (int)Math.Ceiling((expiredAt.Value.Date - DateTime.Now.Date).TotalDays);

            return new LicenseCheckResult
            {
                IsValid = false,
                Status = status,
                Mode = mode,
                GmailOwner = gmailOwner,
                MachineId = machineId,
                ExpiredAt = expiredAt,
                RemainingDays = remainingDays,
                Message = message
            };
        }
    }
}