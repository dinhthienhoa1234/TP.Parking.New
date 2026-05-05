using System;
using THPARKING.Core.Models;

namespace THPARKING.Business.InOut
{
    public class VehicleInResult
    {
        public bool Success { get; private set; }

        public bool ShouldOpenBarrier { get; private set; }

        public ParkingSession Session { get; private set; }

        public string Message { get; private set; }

        public static VehicleInResult Ok(ParkingSession session, string message)
        {
            return new VehicleInResult
            {
                Success = true,
                ShouldOpenBarrier = true,
                Session = session,
                Message = message
            };
        }

        public static VehicleInResult Fail(string message)
        {
            return new VehicleInResult
            {
                Success = false,
                ShouldOpenBarrier = false,
                Message = message
            };
        }
    }
}