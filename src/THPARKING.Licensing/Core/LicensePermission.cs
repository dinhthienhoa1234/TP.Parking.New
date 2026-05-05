namespace THPARKING.Licensing.Core
{
    public enum LicensePermission
    {
        None = 0,

        AllowVehicleIn = 1,
        AllowVehicleOut = 2,
        AllowEmergencyVehicleOut = 3,

        AllowViewData = 4,
        AllowExportReport = 5,

        AllowCreateConfig = 6,
        AllowRegisterCard = 7,

        AllowRunWorker = 8
    }
}