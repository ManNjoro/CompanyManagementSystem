using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Models
{
    public class BranchSupplier
    {
        [MaxLength(255)]
        [ForeignKey("Branch")]
        public string BranchId { get; set; }

        [MaxLength(40)]
        public string SupplierName { get; set; }

        [MaxLength(40)]
        public string SupplyType { get; set; }

        public Branch? Branch { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime UpdatedAt { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchOptions { get; set; }

        public BranchSupplier()
        {
            UpdatedAt = CreatedAt;
        }
    }
}
