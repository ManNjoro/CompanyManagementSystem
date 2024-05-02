namespace CompanyManagementSystem.Models
{
    public class AuditLogs
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string ActionType { get; set; }
        public string TableName { get; set; }
        public DateTime Timestamp { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AffectedColumns { get; set; }
        public string EntityId { get; set; }
    }
}
