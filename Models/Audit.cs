using CompanyManagementSystem.Controllers;
using CompanyManagementSystem.Data;
using Microsoft.AspNetCore.Identity;

namespace CompanyManagementSystem.Models
{
    public class Audit
    {
        public void LogAudit(string actionType, string tableName, string entityId, string userId, ApplicationDbContext db)
        {
            LogAuditTrail(actionType, tableName, entityId, userId, db);
        }
        private void LogAuditTrail(string actionType, string tableName, string entityId, string userId, ApplicationDbContext db)
        {
            var auditLog = new AuditLogs
            {
                UserId = userId, // Assuming you have authentication configured
                ActionType = actionType,
                TableName = tableName,
                Timestamp = DateTime.Now,
                EntityId = entityId
            };

            // Save audit log to the database
            db.AuditLogs.Add(auditLog);
            db.SaveChanges();
        }
    }
}
