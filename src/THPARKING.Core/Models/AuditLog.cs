using System;

namespace THPARKING.Core.Models
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }

        public string ActionType { get; set; }

        public string EntityType { get; set; }

        public Guid? EntityId { get; set; }

        public string OperatorCode { get; set; }

        public string MachineName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}