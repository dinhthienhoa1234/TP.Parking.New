using System;

namespace THPARKING.Business.InOut
{
    public class ParkingFeeCalculator
    {
        public decimal Calculate(DateTime timeIn, DateTime timeOut)
        {
            if (timeOut <= timeIn)
                return 0;

            var totalMinutes = (timeOut - timeIn).TotalMinutes;

            if (totalMinutes <= 15)
                return 0;

            if (totalMinutes <= 240)
                return 5000;

            if (totalMinutes <= 720)
                return 10000;

            return 15000;
        }
    }
}