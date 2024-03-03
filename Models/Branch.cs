using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Models
{
    public class Branch
    {
        [Key]
        [MaxLength(255)]
        public string BranchId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(40)]
        public string BranchName { get; set; }

        [MaxLength(255)]
        public string? ManagerId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ManagerStartDate { get; set; }

        [ForeignKey("ManagerId")]
        public virtual Employee? Manager { get; set; }
        public ICollection<Employee>? Employees { get; set; }
        public ICollection<Client>? Clients { get; set; }
        public ICollection<BranchSupplier>? BranchSuppliers { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        public List<SelectListItem>? EmployeeOptions { get; set; }

        public Branch()
        {
            UpdatedAt = CreatedAt;
        }
    }
}
