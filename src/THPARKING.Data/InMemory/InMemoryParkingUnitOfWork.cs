using System;
using System.Collections.Generic;
using THPARKING.Core.Models;
using THPARKING.Data.Repositories;
using THPARKING.Data.UnitOfWork;

namespace THPARKING.Data.InMemory
{
    public class InMemoryParkingUnitOfWork : IUnitOfWork
    {
        private bool _isDisposed;

        public InMemoryParkingUnitOfWork(
            IList<ParkingCard> cards,
            IList<ParkingSession> sessions,
            IList<ParkingImage> images,
            IList<AuditLog> auditLogs)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));
            if (sessions == null) throw new ArgumentNullException(nameof(sessions));
            if (images == null) throw new ArgumentNullException(nameof(images));
            if (auditLogs == null) throw new ArgumentNullException(nameof(auditLogs));

            ParkingCards = new InMemoryParkingCardRepository(cards);
            ParkingSessions = new InMemoryParkingSessionRepository(sessions);
            ParkingImages = new InMemoryParkingImageRepository(images);
            AuditLogs = new InMemoryAuditLogRepository(auditLogs);
        }

        public IParkingCardRepository ParkingCards { get; private set; }

        public IParkingSessionRepository ParkingSessions { get; private set; }

        public IParkingImageRepository ParkingImages { get; private set; }

        public IAuditLogRepository AuditLogs { get; private set; }

        public void Commit()
        {
            EnsureNotDisposed();
        }

        public void Rollback()
        {
            EnsureNotDisposed();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
        }

        private void EnsureNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(InMemoryParkingUnitOfWork));
            }
        }
    }
}
