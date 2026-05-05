using System;
using THPARKING.Core.Enums;

namespace THPARKING.Licensing.Core
{
    public class LicenseTokenValidator
    {
        public LicenseCheckResult Validate(
            LocalLicenseData localData,
            string currentMachineId)
        {
            if (localData == null)
            {
                return LicenseCheckResult.Invalid(
                    LicenseStatus.NotActivated,
                    LicenseMode.Unknown,
                    null,
                    currentMachineId,
                    null,
                    "Phần mềm chưa được kích hoạt license.");
            }

            if (string.IsNullOrWhiteSpace(localData.GmailOwner))
            {
                return LicenseCheckResult.Invalid(
                    LicenseStatus.NotActivated,
                    localData.Mode,
                    null,
                    currentMachineId,
                    localData.ExpiredAt,
                    "License chưa có Gmail sở hữu.");
            }

            if (!string.Equals(localData.MachineId, currentMachineId, StringComparison.OrdinalIgnoreCase))
            {
                return LicenseCheckResult.Invalid(
                    LicenseStatus.FingerprintMismatch,
                    localData.Mode,
                    localData.GmailOwner,
                    currentMachineId,
                    localData.ExpiredAt,
                    "License không khớp với máy hiện tại.");
            }

            if (localData.ExpiredAt < DateTime.Now.Date)
            {
                return LicenseCheckResult.Invalid(
                    LicenseStatus.Expired,
                    localData.Mode,
                    localData.GmailOwner,
                    currentMachineId,
                    localData.ExpiredAt,
                    "License đã hết hạn.");
            }

            return LicenseCheckResult.Valid(
                localData.Mode,
                localData.GmailOwner,
                currentMachineId,
                localData.ExpiredAt,
                "License hợp lệ.");
        }
    }
}