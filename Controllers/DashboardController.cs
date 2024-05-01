using CompanyManagementSystem.Data;
using FastReport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
                _db = db;
            _webHostEnvironment = webHostEnvironment;
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
            // Fetch recent activities for users and stores
            var recentEmployees = _db.Employees.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();
            var recentBranches = _db.Branches.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();
            var recentBranchesSuppliers = _db.BranchesSupplier.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();
            var recentClients = _db.Clients.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();
            var recentSales = _db.Sales.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();


            // Pass the counts to the view
            ViewBag.ItemCounts = itemCounts;
            ViewBag.recentEmployees = recentEmployees;
            ViewBag.recentBranches = recentBranches;
            ViewBag.recentBranchesSuppliers = recentBranchesSuppliers;
            ViewBag.recentSales = recentSales;
            ViewBag.recentClients = recentClients;
            return View();
        }

        public FileResult Generate()
        {
            FastReport.Utils.Config.WebMode = true;
            Report rep = new Report();
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Dashboard.frx");
            rep.Load(path);

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

            // Fetch recent activities
            var recentEmployees = _db.Employees.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();
            var recentBranches = _db.Branches.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();
            var recentBranchesSuppliers = _db.BranchesSupplier.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();
            var recentClients = _db.Clients.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();
            var recentSales = _db.Sales.OrderByDescending(u => u.UpdatedAt).Take(5).ToList();

            // Register data sources
            rep.RegisterData(itemCounts, "ItemRef");
            rep.RegisterData(recentEmployees, "EmployeesRef");
            rep.RegisterData(recentBranches, "BranchesRef");
            rep.RegisterData(recentBranchesSuppliers, "SuppliersRef");
            rep.RegisterData(recentClients, "ClientsRef");
            rep.RegisterData(recentSales, "SalesRef");

            if (rep.Report.Prepare())
            {
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfExport.ShowProgress = false;
                pdfExport.Subject = "Subject Report";
                pdfExport.Title = "Employee Report";
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                rep.Report.Export(pdfExport, ms);
                rep.Dispose();
                pdfExport.Dispose();
                ms.Position = 0;
                return File(ms, "application/pdf", "DashboardReport.pdf");
            }
            else
            {
                return null;
            }
        }
    }
}
