using THPARKING.Business.InOut;
using THPARKING.Camera.Providers;
using THPARKING.Device.CardReaders;

namespace THPARKING.Business.Orchestration
{
    public class ParkingVehicleOutFlowResult
    {
        public bool Success { get; private set; }

        public bool ShouldOpenBarrier { get; private set; }

        public string Message { get; private set; }

        public string CardCodeNormalized { get; private set; }

        public string PlateNumberNormalized { get; private set; }

        public string LaneCode { get; private set; }

        public decimal FeeAmount { get; private set; }

        public bool IsEmergencyExit { get; private set; }

        public CardBusinessResult CardResult { get; private set; }

        public CameraSnapshotResult PlateSnapshotResult { get; private set; }

        public CameraSnapshotResult FaceSnapshotResult { get; private set; }

        public VehicleOutResult BusinessResult { get; private set; }

        public static ParkingVehicleOutFlowResult Ok(
            string cardCodeNormalized,
            string plateNumberNormalized,
            string laneCode,
            CardBusinessResult cardResult,
            CameraSnapshotResult plateSnapshotResult,
            CameraSnapshotResult faceSnapshotResult,
            VehicleOutResult businessResult)
        {
            return new ParkingVehicleOutFlowResult
            {
                Success = true,
                ShouldOpenBarrier = businessResult != null && businessResult.ShouldOpenBarrier,
                Message = businessResult == null ? "Xe ra hợp lệ." : businessResult.Message,
                CardCodeNormalized = cardCodeNormalized,
                PlateNumberNormalized = plateNumberNormalized,
                LaneCode = laneCode,
                FeeAmount = businessResult == null ? 0 : businessResult.FeeAmount,
                IsEmergencyExit = businessResult != null && businessResult.IsEmergencyExit,
                CardResult = cardResult,
                PlateSnapshotResult = plateSnapshotResult,
                FaceSnapshotResult = faceSnapshotResult,
                BusinessResult = businessResult
            };
        }

        public static ParkingVehicleOutFlowResult Fail(string message)
        {
            return new ParkingVehicleOutFlowResult
            {
                Success = false,
                ShouldOpenBarrier = false,
                Message = message
            };
        }
    }
}