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

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; } // Foreign key to AspNetUsers table
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(40)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(40)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDay { get; set; }

        [Required]
        [MaxLength(1)]
        public string Sex { get; set; }

        [Required]
        public int Salary { get; set; }

        public virtual Employee? Supervisor { get; set; }

        [MaxLength(255)]
        [ForeignKey("Supervisor")]
        public string? SupervisorId { get; set; }


        public Branch? Branch { get; set; }
        [MaxLength(255)]
        [ForeignKey("Branch")]
        public string? BranchId { get; set; }

        public ICollection<Client>? Clients { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime UpdatedAt { get; set; }

        [NotMapped] // This property is not mapped to the database
        public string? SelectedSex { get; set; }

        [NotMapped] // This property is not mapped to the database
        public List<SelectListItem>? SexOptions { get; set; }
        [NotMapped]
        public List<SelectListItem>? SupervisorOptions { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchOptions { get; set; }
        [NotMapped]
        public List<SelectListItem>? UserOptions { get; set; }
        [NotMapped]
        public List<SelectListItem>? FirstNames { get; set; }
        [NotMapped]
        public List<SelectListItem>? LastNames { get; set; }

        public Employee()
        {
            UpdatedAt = CreatedAt;
        }
    }
}
