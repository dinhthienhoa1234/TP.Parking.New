using System;
using THPARKING.Business.Sync;
using THPARKING.Core.Enums;
using THPARKING.Core.Models;
using THPARKING.Licensing.Core;

namespace THPARKING.Business.InOut
{
    public class InOutBusinessService
    {
        private readonly LicenseGate _licenseGate;
        private readonly SyncOutboxService _syncOutboxService;
        private readonly ParkingFeeCalculator _feeCalculator;
        private readonly DuplicatePlateChecker _plateChecker;

        public InOutBusinessService(
            LicenseGate licenseGate,
            SyncOutboxService syncOutboxService,
            ParkingFeeCalculator feeCalculator,
            DuplicatePlateChecker plateChecker)
        {
            _licenseGate = licenseGate;
            _syncOutboxService = syncOutboxService;
            _feeCalculator = feeCalculator ?? new ParkingFeeCalculator();
            _plateChecker = plateChecker ?? new DuplicatePlateChecker();
        }

        public VehicleInResult ProcessVehicleIn(VehicleInRequest request)
        {
            var validationMessage = ValidateVehicleInRequest(request);
            if (!string.IsNullOrWhiteSpace(validationMessage))
                return VehicleInResult.Fail(validationMessage);

            if (_licenseGate != null && !_licenseGate.Can(LicensePermission.AllowVehicleIn))
                return VehicleInResult.Fail("License không cho phép xe vào mới.");

            var now = request.TimeIn == DateTime.MinValue ? DateTime.Now : request.TimeIn;

            var session = new ParkingSession
            {
                ParkingSessionId = Guid.NewGuid(),
                ParkingCardId = Guid.Empty,
                CardCodeNormalized = request.CardCodeNormalized,
                PlateNumber = request.PlateNumber,
                PlateNumberNormalized = request.PlateNumberNormalized,
                TimeIn = now,
                EntryLaneCode = request.LaneCode,
                Status = ParkingSessionStatus.Open,
                CreatedAt = now
            };

            EnqueueSync(
                SyncEventType.VehicleInCreated,
                "ParkingSession",
                session.ParkingSessionId.ToString(),
                BuildVehicleInPayload(request, session),
                request.MachineName);

            return VehicleInResult.Ok(session, "Xe vào hợp lệ.");
        }

        public VehicleOutResult ProcessVehicleOut(VehicleOutRequest request)
        {
            var validationMessage = ValidateVehicleOutRequest(request);
            if (!string.IsNullOrWhiteSpace(validationMessage))
                return VehicleOutResult.Fail(validationMessage);

            var normalOutAllowed = _licenseGate == null ||
                                   _licenseGate.Can(LicensePermission.AllowVehicleOut);

            var emergencyOutAllowed = _licenseGate == null ||
                                      _licenseGate.Can(LicensePermission.AllowEmergencyVehicleOut);

            if (!normalOutAllowed && !request.IsEmergencyExit)
                return VehicleOutResult.Fail("License không cho phép xe ra bình thường. Chỉ có thể xử lý xe ra emergency nếu còn xe trong bãi.");

            if (request.IsEmergencyExit && !emergencyOutAllowed)
                return VehicleOutResult.Fail("License không cho phép xe ra emergency.");

            if (_plateChecker.IsPlateMismatch(
                request.OriginalPlateNumberNormalized,
                request.PlateNumberNormalized))
            {
                return VehicleOutResult.Fail("Biển số không khớp. Vui lòng kiểm tra lại.");
            }

            var timeOut = request.TimeOut == DateTime.MinValue ? DateTime.Now : request.TimeOut;
            var timeIn = request.OriginalTimeIn.HasValue ? request.OriginalTimeIn.Value : timeOut;

            var feeAmount = _feeCalculator.Calculate(timeIn, timeOut);

            var session = new ParkingSession
            {
                ParkingSessionId = Guid.NewGuid(),
                ParkingCardId = Guid.Empty,
                CardCodeNormalized = request.CardCodeNormalized,
                PlateNumber = request.PlateNumber,
                PlateNumberNormalized = request.PlateNumberNormalized,
                TimeIn = timeIn,
                TimeOut = timeOut,
                ExitLaneCode = request.LaneCode,
                Status = ParkingSessionStatus.Closed,
                FeeAmount = feeAmount,
                IsEmergencyExit = request.IsEmergencyExit,
                EmergencyReason = request.EmergencyReason,
                UpdatedAt = timeOut
            };

            EnqueueSync(
                SyncEventType.VehicleOutCreated,
                "ParkingSession",
                session.ParkingSessionId.ToString(),
                BuildVehicleOutPayload(request, session, feeAmount),
                request.MachineName);

            return VehicleOutResult.Ok(
                session,
                feeAmount,
                request.IsEmergencyExit,
                request.IsEmergencyExit ? "Xe ra emergency hợp lệ." : "Xe ra hợp lệ.");
        }

        private string ValidateVehicleInRequest(VehicleInRequest request)
        {
            if (request == null)
                return "Thiếu dữ liệu xe vào.";

            if (string.IsNullOrWhiteSpace(request.CardCodeNormalized))
                return "Thiếu mã thẻ.";

            if (string.IsNullOrWhiteSpace(request.LaneCode))
                return "Thiếu mã làn xe vào.";

            return null;
        }

        private string ValidateVehicleOutRequest(VehicleOutRequest request)
        {
            if (request == null)
                return "Thiếu dữ liệu xe ra.";

            if (string.IsNullOrWhiteSpace(request.CardCodeNormalized))
                return "Thiếu mã thẻ.";

            if (string.IsNullOrWhiteSpace(request.LaneCode))
                return "Thiếu mã làn xe ra.";

            return null;
        }

        private void EnqueueSync(
            SyncEventType eventType,
            string entityType,
            string entityId,
            string payloadJson,
            string sourceMachineCode)
        {
            if (_syncOutboxService == null)
                return;

            _syncOutboxService.Enqueue(
                eventType,
                SyncTargetType.LocalServer,
                entityType,
                entityId,
                payloadJson,
                sourceMachineCode,
                "local-server");

            _syncOutboxService.Enqueue(
                eventType,
                SyncTargetType.OnlineServer,
                entityType,
                entityId,
                payloadJson,
                sourceMachineCode,
                "online-server");
        }

        private string BuildVehicleInPayload(VehicleInRequest request, ParkingSession session)
        {
            return "{"
                + "\"type\":\"vehicle_in\","
                + "\"sessionId\":\"" + session.ParkingSessionId + "\","
                + "\"cardCode\":\"" + Safe(request.CardCodeNormalized) + "\","
                + "\"plate\":\"" + Safe(request.PlateNumberNormalized) + "\","
                + "\"lane\":\"" + Safe(request.LaneCode) + "\","
                + "\"timeIn\":\"" + session.TimeIn.ToString("o") + "\""
                + "}";
        }

        private string BuildVehicleOutPayload(
            VehicleOutRequest request,
            ParkingSession session,
            decimal feeAmount)
        {
            return "{"
                + "\"type\":\"vehicle_out\","
                + "\"sessionId\":\"" + session.ParkingSessionId + "\","
                + "\"cardCode\":\"" + Safe(request.CardCodeNormalized) + "\","
                + "\"plate\":\"" + Safe(request.PlateNumberNormalized) + "\","
                + "\"lane\":\"" + Safe(request.LaneCode) + "\","
                + "\"timeOut\":\"" + session.TimeOut.Value.ToString("o") + "\","
                + "\"feeAmount\":" + feeAmount + ","
                + "\"isEmergencyExit\":" + request.IsEmergencyExit.ToString().ToLowerInvariant()
                + "}";
        }

        private string Safe(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}