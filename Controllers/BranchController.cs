using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using CompanyManagementSystem.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class BranchController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly Audit _audit;
        public BranchController(ApplicationDbContext db, Audit audit)
        {
            _db = db;
            _audit = audit;
        }

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

        public IActionResult Index(int pg = 1, string SearchText = "", int pageSize = 5)
        {
            List<Branch> branches;
            if (SearchText != "" && SearchText != null)
            {
                branches = _db.Branches
                    .Where(cat => cat.BranchName.Contains(SearchText))
                    .ToList();
            }
            else
                branches = _db.Branches.OrderByDescending(branch => branch.UpdatedAt).ToList();

            if (pg < 1) pg = 1;
            int recsCount = branches.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = branches.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "branch", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(data);
        }

        public IActionResult Add ()
        {
            ViewBag.Action = "Add";
            var model = new Branch();
            var employees = _db.Employees.ToList();
           
            model.EmployeeOptions = employees.Select(e => new SelectListItem
            {
                Value = e.EmpId,
                Text = $"{e.FirstName} {e.LastName}"
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(Branch branch)
        {
            if (branch == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Branches.Add(branch);
                _db.SaveChanges();
                _audit.LogAudit("Create", "branches", branch.BranchId, User.Identity.Name, _db);
                TempData["AlertMessage"] = "Branch Created Successfully...";
                return RedirectToAction(nameof(Index));
            }

            return View(branch);
        }

        public IActionResult Edit(string id)
        {
            ViewBag.Action = "Edit";
            var branch = _db.Branches.Find(id);
            if (branch == null)
            {
                return NotFound();
            }


            var employees = _db.Employees.ToList();

            branch.EmployeeOptions = employees.Select(e => new SelectListItem
            {
                Value = e.EmpId,
                Text = $"{e.FirstName} {e.LastName}"
            }).ToList();

            return View(branch);
        }


        [HttpPost]
        public IActionResult Edit(Branch branch)
        {
            // Retrieve the employee to update from the database
            var branchToUpdate = _db.Branches.Find(branch.BranchId);

            if (branchToUpdate == null)
            {
                // If the employee is not found, return a NotFoundResult or handle the case appropriately
                return NotFound();
            }

            // Update the properties of the retrieved employee with the values from the posted model
            branchToUpdate.BranchName = branch.BranchName;
            branchToUpdate.ManagerId = branch.ManagerId;
            branchToUpdate.ManagerStartDate = branch.ManagerStartDate;
            branchToUpdate.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));

            if (ModelState.IsValid)
            {
                try
                {
                    _db.SaveChanges();
                    _audit.LogAudit("Update", "branches", branch.BranchId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Branch Updated Successfully...";
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
            return View(branch);
        }

        public IActionResult Delete(string branchId)
        {
            try
            {
                var branch = _db.Branches.Find(branchId);
                if (branch != null)
                {
                    _db.Branches.Remove(branch);
                    _db.SaveChanges();
                    _audit.LogAudit("Delete", "branches", branchId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Branch Deleted Successfully...";
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
    }
}
