using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Models
{
    public class Employee
    {
        [Key]
        [MaxLength(255)]
        public string EmpId { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(40)]
        public string FirstName { get; set; }

        [MaxLength(40)]
        public string LastName { get; set; }

        public DateTime BirthDay { get; set; }

        [MaxLength(1)]
        public string Sex { get; set; }

        public int Salary { get; set; }

        public virtual Employee Supervisor { get; set; }

        [MaxLength(255)]
        [ForeignKey("Supervisor")]
        public string? SupervisorId { get; set; }

        [MaxLength(255)]
        [ForeignKey("Branch")]
        public string BranchId { get; set; }

        public ICollection<Client> Clients { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime UpdatedAt { get; set; }

        [NotMapped] // This property is not mapped to the database
        public string SelectedSex { get; set; }

        [NotMapped] // This property is not mapped to the database
        public List<SelectListItem> SexOptions { get; set; }
        [NotMapped]
        public List<SelectListItem> SupervisorOptions { get; set; }
        [NotMapped]
        public List<SelectListItem> BranchOptions { get; set; }

        public Employee()
        {
            UpdatedAt = CreatedAt;
        }
    }
}
