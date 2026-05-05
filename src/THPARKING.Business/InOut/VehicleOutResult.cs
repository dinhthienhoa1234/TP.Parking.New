using System;
using THPARKING.Core.Models;

namespace THPARKING.Business.InOut
{
    public class VehicleOutResult
    {
        public bool Success { get; private set; }

        public bool ShouldOpenBarrier { get; private set; }

        public ParkingSession Session { get; private set; }

        public decimal FeeAmount { get; private set; }

        public bool IsEmergencyExit { get; private set; }

        public string Message { get; private set; }

        public static VehicleOutResult Ok(
            ParkingSession session,
            decimal feeAmount,
            bool isEmergencyExit,
            string message)
        {
            return new VehicleOutResult
            {
                Success = true,
                ShouldOpenBarrier = true,
                Session = session,
                FeeAmount = feeAmount,
                IsEmergencyExit = isEmergencyExit,
                Message = message
            };
        }

        public static VehicleOutResult Fail(string message)
        {
            return new VehicleOutResult
            {
                Success = false,
                ShouldOpenBarrier = false,
                Message = message
            };
        }
    }
}