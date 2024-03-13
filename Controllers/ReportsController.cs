using AspNetCore.Reporting;
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

        public IActionResult Print()
        {
            /*string mimtype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\Report1.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("rp1", "Welcome to Dunder Mifflin");
            LocalReport localReport = new LocalReport(path);
            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);
            return File(result.MainStream, "application/pdf");*/
            return View();
        }
    }
}
