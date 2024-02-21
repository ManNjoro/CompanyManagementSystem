using CompanyManagementSystem.Models.Company.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagementSystem.Models
{
    public class Branch
    {
        [Key]
        [MaxLength(255)]
        public string BranchId { get; set; }

        [MaxLength(40)]
        public string BranchName { get; set; }

        [MaxLength(255)]
        [ForeignKey("Employee")]
        public string ManagerId { get; set; }

        public DateTime ManagerStartDate { get; set; }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<BranchSupplier> BranchSuppliers { get; set; }
    }
}
