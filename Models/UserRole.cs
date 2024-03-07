using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Models
{
    public class UserRole
    {
        public ApplicationUser applicationUser {  get; set; }
        public List<SelectListItem>? ApplicationRoles { get; set; }
        public string SelectedRole { get; set; }
    }
}
