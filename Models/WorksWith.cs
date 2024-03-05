using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Models
{
    public class WorksWith
    {
        [MaxLength(255)]
        [ForeignKey("Employee")]
        public string EmpId { get; set; }

        [MaxLength(255)]
        [ForeignKey("Client")]
        public string ClientId { get; set; }

        public double TotalSales { get; set; }

        public Employee? Employee { get; set; }
        public Client? Client { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        public List<SelectListItem>? EmployeeOptions { get; set; }
        [NotMapped]
        public List<SelectListItem>? ClientOptions { get; set; }

        public WorksWith()
        {
            UpdatedAt = CreatedAt;
        }
    }
}
