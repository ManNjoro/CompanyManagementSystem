using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CompanyManagementSystem.Models
{
    public class WorksWith
    {
        [MaxLength(255)]
        [ForeignKey("Employee")]
        public string EmpId { get; set; }

        [MaxLength(255)]
        [ForeignKey("Client")]
        public string ClientId { get; set; }

        public int TotalSales { get; set; }

        public Employee Employee { get; set; }
        public Client Client { get; set; }
    }
}
