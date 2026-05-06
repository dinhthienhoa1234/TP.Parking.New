using System;
using THPARKING.Business.Sync;
using THPARKING.Core.Enums;
using THPARKING.Core.Models;
using THPARKING.Data.UnitOfWork;
using THPARKING.Licensing.Core;

namespace THPARKING.Business.InOut
{
    public class InOutBusinessService
    {
        private readonly LicenseGate _licenseGate;
        private readonly SyncOutboxService _syncOutboxService;
        private readonly ParkingFeeCalculator _feeCalculator;
        private readonly DuplicatePlateChecker _plateChecker;
        private readonly IUnitOfWork _unitOfWork;

        public InOutBusinessService(
            LicenseGate licenseGate,
            SyncOutboxService syncOutboxService,
            ParkingFeeCalculator feeCalculator,
            DuplicatePlateChecker plateChecker)
            : this(licenseGate, syncOutboxService, feeCalculator, plateChecker, null)
        {
        }

        public InOutBusinessService(
            LicenseGate licenseGate,
            SyncOutboxService syncOutboxService,
            ParkingFeeCalculator feeCalculator,
            DuplicatePlateChecker plateChecker,
            IUnitOfWork unitOfWork)
        {
            _licenseGate = licenseGate;
            _syncOutboxService = syncOutboxService;
            _feeCalculator = feeCalculator ?? new ParkingFeeCalculator();
            _plateChecker = plateChecker ?? new DuplicatePlateChecker();
            _unitOfWork = unitOfWork;
        }

        public VehicleInResult ProcessVehicleIn(VehicleInRequest request)
        {
            string validationMessage = ValidateVehicleInRequest(request);
            if (!string.IsNullOrWhiteSpace(validationMessage))
                return VehicleInResult.Fail(validationMessage);

            if (_licenseGate != null && !_licenseGate.Can(LicensePermission.AllowVehicleIn))
                return VehicleInResult.Fail("License không cho phép xe vào mới.");

            DateTime now = request.TimeIn == DateTime.MinValue ? DateTime.Now : request.TimeIn;

            if (_unitOfWork == null)
            {
                // Temporary fallback for bootstrap compatibility before Data UoW wiring is complete.
                ParkingSession fallbackSession = new ParkingSession
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
                    fallbackSession.ParkingSessionId.ToString(),
                    BuildVehicleInPayload(request, fallbackSession),
                    request.MachineName);

                return VehicleInResult.Ok(fallbackSession, "Xe vào hợp lệ.");
            }

            try
            {
                ParkingCard card = _unitOfWork.ParkingCards.FindByNormalizedCardCode(request.CardCodeNormalized);
                if (card == null)
                    return VehicleInResult.Fail("Thẻ chưa được đăng ký.");

                if (!IsCardUsable(card))
                    return VehicleInResult.Fail("Thẻ không khả dụng để xe vào.");

                ParkingSession openSession = _unitOfWork.ParkingSessions.FindOpenByNormalizedCardCode(request.CardCodeNormalized);
                if (openSession != null)
                    return VehicleInResult.Fail("Thẻ đang có xe trong bãi.");

                ParkingSession session = new ParkingSession
                {
                    ParkingSessionId = Guid.NewGuid(),
                    ParkingCardId = card.ParkingCardId,
                    CardCodeNormalized = request.CardCodeNormalized,
                    PlateNumber = request.PlateNumber,
                    PlateNumberNormalized = request.PlateNumberNormalized,
                    TimeIn = now,
                    EntryLaneCode = request.LaneCode,
                    Status = ParkingSessionStatus.Open,
                    CreatedAt = now
                };

                _unitOfWork.ParkingSessions.Add(session);

                AddAuditLog(
                    "VehicleIn",
                    session,
                    request.OperatorCode,
                    request.MachineName,
                    BuildAuditDescription(request.CardCodeNormalized, request.PlateNumberNormalized, request.LaneCode, null),
                    now);

                EnqueueSync(
                    SyncEventType.VehicleInCreated,
                    "ParkingSession",
                    session.ParkingSessionId.ToString(),
                    BuildVehicleInPayload(request, session),
                    request.MachineName);

                _unitOfWork.Commit();
                return VehicleInResult.Ok(session, "Xe vào hợp lệ.");
            }
            catch
            {
                TryRollback();
                return VehicleInResult.Fail("Có lỗi khi xử lý xe vào. Vui lòng thử lại.");
            }
        }

        public VehicleOutResult ProcessVehicleOut(VehicleOutRequest request)
        {
            string validationMessage = ValidateVehicleOutRequest(request);
            if (!string.IsNullOrWhiteSpace(validationMessage))
                return VehicleOutResult.Fail(validationMessage);

            bool normalOutAllowed = _licenseGate == null ||
                                   _licenseGate.Can(LicensePermission.AllowVehicleOut);

            bool emergencyOutAllowed = _licenseGate == null ||
                                      _licenseGate.Can(LicensePermission.AllowEmergencyVehicleOut);

            if (!normalOutAllowed && !request.IsEmergencyExit)
                return VehicleOutResult.Fail("License không cho phép xe ra bình thường. Chỉ có thể xử lý xe ra emergency nếu còn xe trong bãi.");

            if (request.IsEmergencyExit && !emergencyOutAllowed)
                return VehicleOutResult.Fail("License không cho phép xe ra emergency.");

            DateTime timeOut = request.TimeOut == DateTime.MinValue ? DateTime.Now : request.TimeOut;

            if (_unitOfWork == null)
            {
                // Temporary fallback for bootstrap compatibility before Data UoW wiring is complete.
                if (_plateChecker.IsPlateMismatch(
                    request.OriginalPlateNumberNormalized,
                    request.PlateNumberNormalized))
                {
                    return VehicleOutResult.Fail("Biển số không khớp. Vui lòng kiểm tra lại.");
                }

                DateTime timeInFallback = request.OriginalTimeIn.HasValue ? request.OriginalTimeIn.Value : timeOut;
                decimal fallbackFeeAmount = _feeCalculator.Calculate(timeInFallback, timeOut);

                ParkingSession fallbackSession = new ParkingSession
                {
                    ParkingSessionId = Guid.NewGuid(),
                    ParkingCardId = Guid.Empty,
                    CardCodeNormalized = request.CardCodeNormalized,
                    PlateNumber = request.PlateNumber,
                    PlateNumberNormalized = request.PlateNumberNormalized,
                    TimeIn = timeInFallback,
                    TimeOut = timeOut,
                    ExitLaneCode = request.LaneCode,
                    Status = request.IsEmergencyExit ? ParkingSessionStatus.EmergencyClosed : ParkingSessionStatus.Closed,
                    FeeAmount = fallbackFeeAmount,
                    IsEmergencyExit = request.IsEmergencyExit,
                    EmergencyReason = request.EmergencyReason,
                    UpdatedAt = timeOut
                };

                EnqueueSync(
                    SyncEventType.VehicleOutCreated,
                    "ParkingSession",
                    fallbackSession.ParkingSessionId.ToString(),
                    BuildVehicleOutPayload(request, fallbackSession, fallbackFeeAmount),
                    request.MachineName);

                return VehicleOutResult.Ok(
                    fallbackSession,
                    fallbackFeeAmount,
                    request.IsEmergencyExit,
                    request.IsEmergencyExit ? "Xe ra emergency hợp lệ." : "Xe ra hợp lệ.");
            }

            try
            {
                ParkingSession session = _unitOfWork.ParkingSessions.FindOpenByNormalizedCardCode(request.CardCodeNormalized);
                bool isNewEmergencySession = false;

                if (session == null && !request.IsEmergencyExit)
                    return VehicleOutResult.Fail("Không tìm thấy xe đang gửi trong bãi.");

                if (session == null)
                {
                    session = new ParkingSession
                    {
                        ParkingSessionId = Guid.NewGuid(),
                        ParkingCardId = Guid.Empty,
                        CardCodeNormalized = request.CardCodeNormalized,
                        TimeIn = request.OriginalTimeIn.HasValue ? request.OriginalTimeIn.Value : timeOut,
                        CreatedAt = timeOut
                    };

                    isNewEmergencySession = true;
                }

                if (_plateChecker.IsPlateMismatch(
                    session.PlateNumberNormalized,
                    request.PlateNumberNormalized))
                {
                    return VehicleOutResult.Fail("Biển số không khớp. Vui lòng kiểm tra lại.");
                }

                DateTime timeIn = session.TimeIn == DateTime.MinValue
                    ? (request.OriginalTimeIn.HasValue ? request.OriginalTimeIn.Value : timeOut)
                    : session.TimeIn;

                decimal feeAmount = _feeCalculator.Calculate(timeIn, timeOut);

                if (!string.IsNullOrWhiteSpace(request.PlateNumber))
                    session.PlateNumber = request.PlateNumber;

                if (!string.IsNullOrWhiteSpace(request.PlateNumberNormalized))
                    session.PlateNumberNormalized = request.PlateNumberNormalized;

                session.TimeOut = timeOut;
                session.ExitLaneCode = request.LaneCode;
                session.Status = request.IsEmergencyExit ? ParkingSessionStatus.EmergencyClosed : ParkingSessionStatus.Closed;
                session.FeeAmount = feeAmount;
                session.IsEmergencyExit = request.IsEmergencyExit;
                session.EmergencyReason = request.EmergencyReason;
                session.UpdatedAt = timeOut;

                if (isNewEmergencySession)
                    _unitOfWork.ParkingSessions.Add(session);
                else
                    _unitOfWork.ParkingSessions.Update(session);

                string actionType = request.IsEmergencyExit ? "EmergencyVehicleOut" : "VehicleOut";
                AddAuditLog(
                    actionType,
                    session,
                    request.OperatorCode,
                    request.MachineName,
                    BuildAuditDescription(request.CardCodeNormalized, session.PlateNumberNormalized, request.LaneCode, feeAmount),
                    timeOut);

                EnqueueSync(
                    SyncEventType.VehicleOutCreated,
                    "ParkingSession",
                    session.ParkingSessionId.ToString(),
                    BuildVehicleOutPayload(request, session, feeAmount),
                    request.MachineName);

                _unitOfWork.Commit();

                return VehicleOutResult.Ok(
                    session,
                    feeAmount,
                    request.IsEmergencyExit,
                    request.IsEmergencyExit ? "Xe ra emergency hợp lệ." : "Xe ra hợp lệ.");
            }
            catch
            {
                TryRollback();
                return VehicleOutResult.Fail("Có lỗi khi xử lý xe ra. Vui lòng thử lại.");
            }
        }

        private bool IsCardUsable(ParkingCard card)
        {
            if (card == null)
                return false;

            return card.Status == CardStatus.Active;
        }

        private void AddAuditLog(
            string actionType,
            ParkingSession session,
            string operatorCode,
            string machineName,
            string description,
            DateTime createdAt)
        {
            if (_unitOfWork == null || _unitOfWork.AuditLogs == null || session == null)
                return;

            AuditLog log = new AuditLog
            {
                AuditLogId = Guid.NewGuid(),
                ActionType = actionType,
                EntityType = "ParkingSession",
                EntityId = session.ParkingSessionId,
                OperatorCode = operatorCode,
                MachineName = machineName,
                Description = description,
                CreatedAt = createdAt
            };

            _unitOfWork.AuditLogs.Add(log);
        }

        private string BuildAuditDescription(string cardCode, string plateNumber, string laneCode, decimal? feeAmount)
        {
            string feePart = feeAmount.HasValue ? ", phí=" + feeAmount.Value : string.Empty;
            return "Thẻ=" + Safe(cardCode) + ", biển số=" + Safe(plateNumber) + ", làn=" + Safe(laneCode) + feePart;
        }

        private void TryRollback()
        {
            if (_unitOfWork == null)
                return;

            try
            {
                _unitOfWork.Rollback();
            }
            catch
            {
                // Ignore rollback errors to preserve safe failure response.
            }
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
