using System;
using THPARKING.Core.Enums;

namespace THPARKING.Core.Models
{
    public class ParkingCard
    {
        public Guid ParkingCardId { get; set; }

        public string CardCode { get; set; }

        public string CardCodeNormalized { get; set; }

        public CardStatus Status { get; set; }

        public string OwnerName { get; set; }

        public string Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}