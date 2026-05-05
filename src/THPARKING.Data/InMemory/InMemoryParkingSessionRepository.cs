using System;
using System.Collections.Generic;
using System.Linq;
using THPARKING.Core.Enums;
using THPARKING.Core.Models;
using THPARKING.Data.Repositories;

namespace THPARKING.Data.InMemory
{
    public class InMemoryParkingSessionRepository : IParkingSessionRepository
    {
        private readonly IList<ParkingSession> _sessions;

        public InMemoryParkingSessionRepository(IList<ParkingSession> sessions)
        {
            _sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));
        }

        public ParkingSession FindOpenByNormalizedCardCode(string normalizedCardCode)
        {
            if (string.IsNullOrWhiteSpace(normalizedCardCode))
            {
                return null;
            }

            return _sessions.FirstOrDefault(s =>
                s != null &&
                string.Equals(s.CardCodeNormalized, normalizedCardCode, StringComparison.OrdinalIgnoreCase) &&
                s.Status == ParkingSessionStatus.Open &&
                s.TimeOut == null);
        }

        public void Add(ParkingSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            _sessions.Add(session);
        }

        public void Update(ParkingSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            var existing = _sessions.FirstOrDefault(s => s != null && s.ParkingSessionId == session.ParkingSessionId);
            if (existing == null)
            {
                throw new InvalidOperationException("Parking session was not found for update.");
            }

            var index = _sessions.IndexOf(existing);
            _sessions[index] = session;
        }
    }
}
