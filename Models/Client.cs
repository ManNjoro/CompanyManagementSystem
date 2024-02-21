using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagementSystem.Models
{
    public class Client
    {
        [Key]
        [MaxLength(255)]
        public string ClientId { get; set; }

        [MaxLength(40)]
        public string ClientName { get; set; }

        [MaxLength(255)]
        [ForeignKey("Branch")]
        public string BranchId { get; set; }

        public Branch Branch { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
