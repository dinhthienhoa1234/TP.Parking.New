using System;
using THPARKING.Data.Repositories;

namespace THPARKING.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IParkingCardRepository ParkingCards { get; }

        IParkingSessionRepository ParkingSessions { get; }

        IParkingImageRepository ParkingImages { get; }

        IAuditLogRepository AuditLogs { get; }

        void Commit();

        void Rollback();
    }
}
