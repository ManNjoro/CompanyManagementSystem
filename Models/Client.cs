using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Models
{
    public class Client
    {
        [Key]
        [MaxLength(255)]
        public string ClientId { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(40)]
        public string ClientName { get; set; }

        [MaxLength(255)]
        [ForeignKey("Branch")]
        public string BranchId { get; set; }

        public Branch? Branch { get; set; }
        public ICollection<Employee>? Employees { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime UpdatedAt { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchOptions { get; set; }

        public Client()
        {
            UpdatedAt = CreatedAt;
        }
    }
}
