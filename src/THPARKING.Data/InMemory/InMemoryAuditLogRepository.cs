using System;
using System.Collections.Generic;
using THPARKING.Core.Models;
using THPARKING.Data.Repositories;

namespace THPARKING.Data.InMemory
{
    public class InMemoryAuditLogRepository : IAuditLogRepository
    {
        private readonly IList<AuditLog> _auditLogs;

        public InMemoryAuditLogRepository(IList<AuditLog> auditLogs)
        {
            _auditLogs = auditLogs ?? throw new ArgumentNullException(nameof(auditLogs));
        }

        public void Add(AuditLog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            _auditLogs.Add(log);
        }
    }
}
