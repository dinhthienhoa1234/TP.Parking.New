namespace THPARKING.Business.Sync
{
    public enum SyncEventType
    {
        Unknown = 0,

        VehicleInCreated = 1,
        VehicleOutCreated = 2,
        ParkingSessionUpdated = 3,

        CardRegistered = 10,
        CardUpdated = 11,
        CardDisabled = 12,

        MonthlyPassCreated = 20,
        MonthlyPassUpdated = 21,
        MonthlyPassExpired = 22,

        LicenseChecked = 30,
        AuditLogCreated = 40
    }
}