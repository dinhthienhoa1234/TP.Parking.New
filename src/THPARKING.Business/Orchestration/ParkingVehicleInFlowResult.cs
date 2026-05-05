using THPARKING.Business.InOut;
using THPARKING.Camera.Providers;
using THPARKING.Device.CardReaders;

namespace THPARKING.Business.Orchestration
{
    public class ParkingVehicleInFlowResult
    {
        public bool Success { get; private set; }

        public bool ShouldOpenBarrier { get; private set; }

        public string Message { get; private set; }

        public string CardCodeNormalized { get; private set; }

        public string PlateNumberNormalized { get; private set; }

        public string LaneCode { get; private set; }

        public CardBusinessResult CardResult { get; private set; }

        public CameraSnapshotResult PlateSnapshotResult { get; private set; }

        public CameraSnapshotResult FaceSnapshotResult { get; private set; }

        public VehicleInResult BusinessResult { get; private set; }

        public static ParkingVehicleInFlowResult Ok(
            string cardCodeNormalized,
            string plateNumberNormalized,
            string laneCode,
            CardBusinessResult cardResult,
            CameraSnapshotResult plateSnapshotResult,
            CameraSnapshotResult faceSnapshotResult,
            VehicleInResult businessResult)
        {
            return new ParkingVehicleInFlowResult
            {
                Success = true,
                ShouldOpenBarrier = businessResult != null && businessResult.ShouldOpenBarrier,
                Message = businessResult == null ? "Xe vào hợp lệ." : businessResult.Message,
                CardCodeNormalized = cardCodeNormalized,
                PlateNumberNormalized = plateNumberNormalized,
                LaneCode = laneCode,
                CardResult = cardResult,
                PlateSnapshotResult = plateSnapshotResult,
                FaceSnapshotResult = faceSnapshotResult,
                BusinessResult = businessResult
            };
        }

        public static ParkingVehicleInFlowResult Fail(string message)
        {
            return new ParkingVehicleInFlowResult
            {
                Success = false,
                ShouldOpenBarrier = false,
                Message = message
            };
        }
    }
}