using System;
using System.Collections.Generic;
using THPARKING.Core.Models;
using THPARKING.Data.Repositories;
using THPARKING.Data.UnitOfWork;

namespace THPARKING.Data.InMemory
{
    public class InMemoryParkingUnitOfWork : IUnitOfWork
    {
        private readonly IList<ParkingCard> _cards;
        private readonly IList<ParkingSession> _sessions;
        private readonly IList<ParkingImage> _images;
        private readonly IList<AuditLog> _auditLogs;

        private IList<ParkingCard> _committedCards;
        private IList<ParkingSession> _committedSessions;
        private IList<ParkingImage> _committedImages;
        private IList<AuditLog> _committedAuditLogs;

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

            _cards = cards;
            _sessions = sessions;
            _images = images;
            _auditLogs = auditLogs;

            CaptureCommittedSnapshot();

            ParkingCards = new InMemoryParkingCardRepository(_cards);
            ParkingSessions = new InMemoryParkingSessionRepository(_sessions);
            ParkingImages = new InMemoryParkingImageRepository(_images);
            AuditLogs = new InMemoryAuditLogRepository(_auditLogs);
        }

        public IParkingCardRepository ParkingCards { get; private set; }

        public IParkingSessionRepository ParkingSessions { get; private set; }

        public IParkingImageRepository ParkingImages { get; private set; }

        public IAuditLogRepository AuditLogs { get; private set; }

        public void Commit()
        {
            EnsureNotDisposed();
            CaptureCommittedSnapshot();
        }

        public void Rollback()
        {
            EnsureNotDisposed();

            RestoreList(_cards, _committedCards);
            RestoreList(_sessions, _committedSessions);
            RestoreList(_images, _committedImages);
            RestoreList(_auditLogs, _committedAuditLogs);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
        }

        private void CaptureCommittedSnapshot()
        {
            _committedCards = new List<ParkingCard>(_cards);
            _committedSessions = new List<ParkingSession>(_sessions);
            _committedImages = new List<ParkingImage>(_images);
            _committedAuditLogs = new List<AuditLog>(_auditLogs);
        }

        private static void RestoreList<T>(IList<T> target, IEnumerable<T> source)
        {
            target.Clear();

            foreach (var item in source)
            {
                target.Add(item);
            }
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
