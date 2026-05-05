using THPARKING.Core.Models;

namespace THPARKING.Data.Repositories
{
    public interface IAuditLogRepository
    {
        void Add(AuditLog log);
    }
}
