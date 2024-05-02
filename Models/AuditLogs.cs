using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyManagementSystem.Models
{
    public class AuditLogs
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string ActionType { get; set; }
        public string TableName { get; set; }
        public DateTime Timestamp { get; set; }
        public string EntityId { get; set; }
        [NotMapped]
        public List<SelectListItem>? TableColumns { get; set; }
    }
}
