using Microsoft.AspNetCore.Identity;

namespace CompanyManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ProfilePicture { get; set; }
    }
}
