using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

        public Branch Branch { get; set; }

        [Key]
        public string CompositeKey { get; set; }
    }
}
