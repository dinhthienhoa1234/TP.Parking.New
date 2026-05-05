namespace THPARKING.Core.Enums
{
    public enum LicenseStatus
    {
        Unknown = 0,
        NotActivated = 1,
        Active = 2,
        Warning = 3,
        Expired = 4,
        Suspended = 5,
        Revoked = 6,
        ClockTamper = 7,
        FingerprintMismatch = 8
    }
}