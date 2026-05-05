using System;
using THPARKING.Business.InOut;
using THPARKING.Camera.Providers;
using THPARKING.Device.CardReaders;

namespace THPARKING.Business.Orchestration
{
    public class ParkingAppOrchestrator
    {
        private readonly ParkingAppContext _context;

        public ParkingAppOrchestrator(ParkingAppContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        public ParkingVehicleInFlowResult ProcessVehicleIn(ParkingVehicleInFlowRequest request)
        {
            var validationMessage = ValidateVehicleInFlowRequest(request);
            if (!string.IsNullOrWhiteSpace(validationMessage))
                return ParkingVehicleInFlowResult.Fail(validationMessage);

            var cardService = _context.CardCodeService ?? new CardCodeService();
            var cardResult = cardService.ProcessRawCode(request.RawCardCode);

            if (!cardResult.Accepted)
                return ParkingVehicleInFlowResult.Fail(cardResult.Message);

            var plateNormalized = NormalizePlate(request.PlateNumber);
            var laneCode = request.LaneCode;

            var plateSnapshot = CaptureOptionalSnapshot(
                _context.PlateCameraService,
                request.PlateCameraChannel);

            var faceSnapshot = CaptureOptionalSnapshot(
                _context.FaceCameraService,
                request.FaceCameraChannel);

            if (_context.InOutBusinessService == null)
                return ParkingVehicleInFlowResult.Fail("Chưa cấu hình InOutBusinessService.");

            var businessRequest = new VehicleInRequest
            {
                CardCode = request.RawCardCode,
                CardCodeNormalized = cardResult.NormalizedCardCode,
                PlateNumber = request.PlateNumber,
                PlateNumberNormalized = plateNormalized,
                LaneCode = laneCode,
                ReaderCode = request.ReaderCode,
                CameraDeviceCode = request.CameraDeviceCode,
                OperatorCode = _context.OperatorCode,
                MachineName = _context.MachineName,
                TimeIn = request.RequestedAt == DateTime.MinValue ? DateTime.Now : request.RequestedAt,
                ImageInPath = request.ImageInPath,
                FaceImageInPath = request.FaceImageInPath
            };

            var businessResult = _context.InOutBusinessService.ProcessVehicleIn(businessRequest);

            if (!businessResult.Success)
                return ParkingVehicleInFlowResult.Fail(businessResult.Message);

            return ParkingVehicleInFlowResult.Ok(
                cardResult.NormalizedCardCode,
                plateNormalized,
                laneCode,
                cardResult,
                plateSnapshot,
                faceSnapshot,
                businessResult);
        }

        public ParkingVehicleOutFlowResult ProcessVehicleOut(ParkingVehicleOutFlowRequest request)
        {
            var validationMessage = ValidateVehicleOutFlowRequest(request);
            if (!string.IsNullOrWhiteSpace(validationMessage))
                return ParkingVehicleOutFlowResult.Fail(validationMessage);

            var cardService = _context.CardCodeService ?? new CardCodeService();
            var cardResult = cardService.ProcessRawCode(request.RawCardCode);

            if (!cardResult.Accepted)
                return ParkingVehicleOutFlowResult.Fail(cardResult.Message);

            var plateNormalized = NormalizePlate(request.PlateNumber);
            var laneCode = request.LaneCode;

            var plateSnapshot = CaptureOptionalSnapshot(
                _context.PlateCameraService,
                request.PlateCameraChannel);

            var faceSnapshot = CaptureOptionalSnapshot(
                _context.FaceCameraService,
                request.FaceCameraChannel);

            if (_context.InOutBusinessService == null)
                return ParkingVehicleOutFlowResult.Fail("Chưa cấu hình InOutBusinessService.");

            var businessRequest = new VehicleOutRequest
            {
                CardCode = request.RawCardCode,
                CardCodeNormalized = cardResult.NormalizedCardCode,
                PlateNumber = request.PlateNumber,
                PlateNumberNormalized = plateNormalized,
                LaneCode = laneCode,
                ReaderCode = request.ReaderCode,
                CameraDeviceCode = request.CameraDeviceCode,
                OperatorCode = _context.OperatorCode,
                MachineName = _context.MachineName,
                TimeOut = request.RequestedAt == DateTime.MinValue ? DateTime.Now : request.RequestedAt,
                OriginalTimeIn = request.OriginalTimeIn,
                OriginalPlateNumberNormalized = request.OriginalPlateNumberNormalized,
                IsEmergencyExit = request.IsEmergencyExit,
                EmergencyReason = request.EmergencyReason,
                ImageOutPath = request.ImageOutPath,
                FaceImageOutPath = request.FaceImageOutPath
            };

            var businessResult = _context.InOutBusinessService.ProcessVehicleOut(businessRequest);

            if (!businessResult.Success)
                return ParkingVehicleOutFlowResult.Fail(businessResult.Message);

            return ParkingVehicleOutFlowResult.Ok(
                cardResult.NormalizedCardCode,
                plateNormalized,
                laneCode,
                cardResult,
                plateSnapshot,
                faceSnapshot,
                businessResult);
        }

        private string ValidateVehicleInFlowRequest(ParkingVehicleInFlowRequest request)
        {
            if (request == null)
                return "Thiếu dữ liệu luồng xe vào.";

            if (string.IsNullOrWhiteSpace(request.RawCardCode))
                return "Thiếu mã thẻ xe vào.";

            if (string.IsNullOrWhiteSpace(request.LaneCode))
                return "Thiếu mã làn xe vào.";

            return null;
        }

        private string ValidateVehicleOutFlowRequest(ParkingVehicleOutFlowRequest request)
        {
            if (request == null)
                return "Thiếu dữ liệu luồng xe ra.";

            if (string.IsNullOrWhiteSpace(request.RawCardCode))
                return "Thiếu mã thẻ xe ra.";

            if (string.IsNullOrWhiteSpace(request.LaneCode))
                return "Thiếu mã làn xe ra.";

            return null;
        }

        private CameraSnapshotResult CaptureOptionalSnapshot(
            IParkingCameraService cameraService,
            int channel)
        {
            if (cameraService == null)
                return null;

            if (!cameraService.IsConnected)
                return null;

            return cameraService.CaptureSnapshot(channel);
        }

        private string NormalizePlate(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                return null;

            return plateNumber
                .Trim()
                .Replace(" ", string.Empty)
                .Replace(".", string.Empty)
                .Replace("-", string.Empty)
                .ToUpperInvariant();
        }
    }
}