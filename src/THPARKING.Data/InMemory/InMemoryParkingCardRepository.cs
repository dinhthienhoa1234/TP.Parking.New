using System;
using System.Collections.Generic;
using System.Linq;
using THPARKING.Core.Models;
using THPARKING.Data.Repositories;

namespace THPARKING.Data.InMemory
{
    public class InMemoryParkingCardRepository : IParkingCardRepository
    {
        private readonly IList<ParkingCard> _cards;

        public InMemoryParkingCardRepository(IList<ParkingCard> cards)
        {
            _cards = cards ?? throw new ArgumentNullException(nameof(cards));
        }

        public ParkingCard FindByNormalizedCardCode(string normalizedCardCode)
        {
            if (string.IsNullOrWhiteSpace(normalizedCardCode))
            {
                return null;
            }

            return _cards.FirstOrDefault(c =>
                c != null &&
                string.Equals(c.CardCodeNormalized, normalizedCardCode, StringComparison.OrdinalIgnoreCase));
        }
    }
}
