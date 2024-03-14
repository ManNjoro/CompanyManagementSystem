using AspNetCore.Reporting;
using FastReport;
using Microsoft.AspNetCore.Mvc;

namespace CompanyManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public FileResult Generate()
        {
            FastReport.Utils.Config.WebMode = true;
            Report rep = new Report();
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Employee.frx");
            rep.Load(path);

            // Add your report generation logic here

            // Return the generated report file
            return File(/* Your report file bytes here */ "application/pdf", "report.pdf");
        }
    }
}
