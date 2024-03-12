using CompanyManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db)
        {
                _db = db;
        }
        public IActionResult Index()
        {
            // Retrieve counts for each item
            var itemCounts = new Dictionary<string, int>();

            // Count employees
            int employeeCount = _db.Employees.Count();
            itemCounts.Add("Employees", employeeCount);

            int productCount = _db.Products.Count();
            itemCounts.Add("Products", productCount);

            int saleCount = _db.Sales.Count();
            itemCounts.Add("Sales", saleCount);

            int clientCount = _db.Clients.Count();
            itemCounts.Add("Clients", clientCount);

            int branchSupplierCount = _db.BranchesSupplier.Count();
            itemCounts.Add("Branch Suppliers", branchSupplierCount);

            int branchCount = _db.Branches.Count();
            itemCounts.Add("Branches", branchCount);

            // Pass the counts to the view
            ViewBag.ItemCounts = itemCounts;
            return View();
        }
    }
}
