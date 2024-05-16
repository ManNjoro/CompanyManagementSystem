using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using CompanyManagementSystem.Views.Shared.Components.SearchBar;
using FastReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<EmployeesController> _logger;
        private readonly Audit _audit;
        /// <summary>
        /// Constructor for EmployeesController
        /// </summary>
        /// <param name="db">ApplicationDbContext instance</param>
        /// <param name="userManager">UserManager instance</param>
        /// <param name="webHostEnvironment">IWebHostEnvironment instance</param>
        /// <param name="logger">ILogger instance</param>
        /// <param name="audit">Audit instance</param>
        public EmployeesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, ILogger<EmployeesController> logger, Audit audit)
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _audit = audit;
        }

        /// <summary>
        /// Get a list of page sizes
        /// </summary>
        /// <param name="selectedPageSize">Selected page size</param>
        /// <returns>List of SelectListItem for page sizes</returns>
        private List<SelectListItem> GetPageSizes(int selectedPageSize = 10)
        {
            var pagesSizes = new List<SelectListItem>();
            if (selectedPageSize == 5)
                pagesSizes.Add(new SelectListItem("5", "5", true));
            else
                pagesSizes.Add(new SelectListItem("5", "5"));

            for (int lp = 10; lp <= 100; lp += 10)
            {
                if (lp == selectedPageSize)
                {
                    pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString(), true));
                }
                else
                    pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString()));
            }
            return pagesSizes;
        }

        /// <summary>
        /// Display a paginated list of employees
        /// </summary>
        /// <param name="pg">Page number</param>
        /// <param name="SearchText">Search text</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="SortBy">Sort by</param>
        /// <param name="direction">Sort direction</param>
        /// <returns>View with a paginated list of employees</returns>
        public IActionResult Index(int pg = 1, string SearchText = "", int pageSize = 5, string SortBy = "", string direction = "down")
        {
            List<Employee> employees;
            if (SearchText != "" && SearchText != null)
            {
                employees = _db.Employees
                    .Where(cat => cat.FirstName.Contains(SearchText) || cat.LastName.Contains(SearchText))
                    .ToList();
            }
            else
                employees = _db.Employees.ToList();

            switch (SortBy)
            {
                case "name":
                    if (direction == "down")
                    {

                        employees = employees.OrderByDescending(log => log.FirstName).ToList();
                    }
                    else if (direction == "up")
                    {
                        employees = employees.OrderBy(log => log.FirstName).ToList();
                    }
                    break;
                case "sex":
                    if (direction == "down")
                        employees = employees.OrderByDescending(log => log.Sex).ToList();
                    else if (direction == "up")
                        employees = employees.OrderBy(log => log.Sex).ToList();
                    break;
                case "salary":
                    if (direction == "down")
                        employees = employees.OrderByDescending(log => log.Salary).ToList();
                    else if (direction == "up")
                        employees = employees.OrderBy(log => log.Salary).ToList();
                    break;
                default:

                    employees = employees.OrderByDescending(log => log.UpdatedAt).ToList();
                    break;
            }

            if (pg < 1) pg = 1;
            int recsCount = employees.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = employees.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "employees", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(data);
        }
        /// <summary>
        /// Display the form to add a new employee
        /// </summary>
        /// <returns>View with the form to add a new employee</returns>
        /// [Authorize(Roles = "Admin")
        /// 
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            var model = new Employee();
            var firstnames = _userManager.Users.ToList();
            var lastnames = _userManager.Users.ToList();
            var supervisors = _db.Employees.ToList();
            var branches = _db.Branches.ToList();
            var users = _userManager.Users.ToList();

            // Populate select options
            model.SexOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "M", Text = "Male" },
                new SelectListItem { Value = "F", Text = "Female" }
            };
            model.SupervisorOptions = supervisors.Select(s => new SelectListItem
            {
                Value = s.EmpId,
                Text = $"{s.FirstName} {s.LastName}" // Assuming you have FirstName and LastName properties
            }).ToList();

            model.BranchOptions = branches.Select(b => new SelectListItem
            {
                Value = b.BranchId,
                Text = b.BranchName
            }).ToList();

            model.UserOptions = users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.Firstname} {u.Lastname}"
            }).ToList();

            model.FirstNames = firstnames.Select(f => new SelectListItem
            {
                Value = f.Firstname,
                Text = f.Firstname
            }).ToList();

            model.LastNames = lastnames.Select(l => new SelectListItem
            {
                Value = l.Lastname,
                Text = l.Lastname
            }).ToList();

            return View(model);
        }

        /// <summary>
        /// Post method to add a new employee
        /// </summary>
        /// <param name="employee">Employee object to add</param>
        /// <returns>Redirect to index view</returns>
        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            if (employee == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Employees.Add(employee);
                    _db.SaveChanges();
                    _audit.LogAudit("Create", "employees", employee.EmpId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Employee Created Successfully...";
                }
                catch (Exception ex)
                {
                    TempData["AlertMessage"] = "Employee was not created";
                }
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        /// <summary>
        /// Display the form to edit an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>View with the form to edit an existing employee</returns>
        public IActionResult Edit(string id)
        {
            ViewBag.Action = "Edit";
            var employee = _db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Fetch all employees for supervisor options
            var employees = _db.Employees.ToList();
            var firstnames = _userManager.Users.ToList();
            var lastnames = _userManager.Users.ToList();

            employee.SexOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "M", Text = "Male" },
                new SelectListItem { Value = "F", Text = "Female" }
            };

            // Populate supervisor options
            employee.SupervisorOptions = employees.Select(e => new SelectListItem
            {
                Value = e.EmpId,
                Text = $"{e.FirstName} {e.LastName}"
            }).ToList();

            // Populate branch options
            // Assuming you have a Branch model with appropriate properties
            var branches = _db.Branches.ToList();
            employee.BranchOptions = branches.Select(b => new SelectListItem
            {
                Value = b.BranchId,
                Text = b.BranchName
            }).ToList();

            var users = _userManager.Users.ToList();
            employee.UserOptions = users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.Firstname} {u.Lastname}"
            }).ToList();

            employee.FirstNames = firstnames.Select(f => new SelectListItem
            {
                Value = f.Firstname,
                Text = f.Firstname
            }).ToList();

            employee.LastNames = lastnames.Select(l => new SelectListItem
            {
                Value = l.Lastname,
                Text = l.Lastname
            }).ToList();

            return View(employee);
        }


        /// <summary>
        /// Post method to edit an existing employee
        /// </summary>
        /// <param name="employee">Employee object to edit</param>
        /// <returns>Redirect to index view</returns>
        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            // Retrieve the employee to update from the database
            var employeeToUpdate = _db.Employees.Find(employee.EmpId);

            if (employeeToUpdate == null)
            {
                // If the employee is not found, return a NotFoundResult or handle the case appropriately
                return NotFound();
            }

            // Update the properties of the retrieved employee with the values from the posted model
            employeeToUpdate.FirstName = employee.FirstName;
            employeeToUpdate.LastName = employee.LastName;
            employeeToUpdate.BirthDay = employee.BirthDay;
            employeeToUpdate.Sex = employee.Sex;
            employeeToUpdate.Salary = employee.Salary;
            employeeToUpdate.SupervisorId = employee.SupervisorId;
            employeeToUpdate.BranchId = employee.BranchId;
            employeeToUpdate.UserId = employee.UserId;
            employeeToUpdate.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));

            if (ModelState.IsValid)
            {
                try
                {
                    _db.SaveChanges();
                    _audit.LogAudit("Update", "employees", employee.EmpId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Employee Updated Successfully...";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    Console.WriteLine($"Error updating employee: {ex.Message}");
                    throw; // Re-throw the exception to propagate it up the call stack
                }
            }

            // If ModelState is not valid, return the same view with validation errors
            return View(employee);
        }

        /// <summary>
        /// Delete an existing employee
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <returns>Redirect to index view</returns>
        public IActionResult Delete(string employeeId)
        {
            try
            {
                var employee = _db.Employees.Find(employeeId);
                if (employee != null)
                {
                    _db.Employees.Remove(employee);
                    _db.SaveChanges();
                    _audit.LogAudit("Delete", "employees", employeeId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Employee Deleted Successfully...";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error deleting category: {ex.Message}");
                throw; // Re-throw the exception to propagate it up the call stack
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Generate a PDF report of employees
        /// </summary>
        /// <returns>PDF file result</returns>
        public FileResult Generate()
        {
            FastReport.Utils.Config.WebMode = true;
            Report rep = new Report();
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Employee.frx");
            rep.Load(path);

            // Add your report generation logic here
            var employees = _db.Employees.ToList();
            rep.RegisterData(employees, "EmployeeRef");

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
                return File(ms, "application/pdf", "EmployeeReport.pdf");
            }
            else
            {
                return null;
            }
        }

    }
}
