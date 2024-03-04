using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyManagementSystem.Models
{
    public class Product
    {
        public string ProductId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? ProductImage { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss zzz}", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime UpdatedAt { get; set; }

        public Product()
        {
            UpdatedAt = CreatedAt;
        }

    }
}
