using System;
using THPARKING.Core.Enums;

namespace THPARKING.Core.Models
{
    public class ParkingSession
    {
        public Guid ParkingSessionId { get; set; }

        public Guid ParkingCardId { get; set; }

        public string CardCodeNormalized { get; set; }

        public string PlateNumber { get; set; }

        public string PlateNumberNormalized { get; set; }

        public DateTime TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public string EntryLaneCode { get; set; }

        public string ExitLaneCode { get; set; }

        public ParkingSessionStatus Status { get; set; }

        public decimal? FeeAmount { get; set; }

        public bool IsEmergencyExit { get; set; }

        public string EmergencyReason { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}