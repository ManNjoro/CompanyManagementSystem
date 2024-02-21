using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagementSystem.Models
{
    public class Employee
    {
        [Key]
        [MaxLength(255)]
        public string EmpId { get; set; }

        [MaxLength(40)]
        public string FirstName { get; set; }

        [MaxLength(40)]
        public string LastName { get; set; }

        public DateTime BirthDay { get; set; }

        [MaxLength(1)]
        public string Sex { get; set; }

        public int Salary { get; set; }

        [MaxLength(255)]
        [ForeignKey("Employee")]
        public string SupervisorId { get; set; }

        [MaxLength(255)]
        [ForeignKey("Branch")]
        public string BranchId { get; set; }

        public ICollection<Client> Clients { get; set; }
    }
}
