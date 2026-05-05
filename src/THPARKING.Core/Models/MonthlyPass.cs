using System;

namespace THPARKING.Core.Models
{
    public class MonthlyPass
    {
        public Guid MonthlyPassId { get; set; }

        public Guid ParkingCardId { get; set; }

        public string PlateNumber { get; set; }

        public string PlateNumberNormalized { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool IsActive { get; set; }

        public string CustomerName { get; set; }

        public string Note { get; set; }
    }
}