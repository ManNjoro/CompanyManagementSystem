using Microsoft.AspNetCore.Mvc;

namespace CompanyManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Print()
        {
            return View();
        }
    }
}
