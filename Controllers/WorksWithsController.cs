using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using CompanyManagementSystem.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Authorization;

namespace CompanyManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class WorksWithsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly Audit _audit;
        public WorksWithsController(ApplicationDbContext db, Audit audit)
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
            List<WorksWith> worksWith;
            if (SearchText != "" && SearchText != null)
            {
                worksWith = _db.WorksWith
                    .Where(work => work.ClientId.Contains(SearchText))
                    .ToList();
            }
            else
                worksWith = _db.WorksWith
                   .Include(w => w.Employee)
                   .Include(w => w.Client)
                   .OrderByDescending(w => w.UpdatedAt)
                   .ToList();

            // Calculate total sales for each WorksWith record
            foreach (var work in worksWith)
            {
                work.TotalSales = _db.Sales
                    .Where(s => s.EmpId == work.EmpId && s.ClientId == work.ClientId)
                    .Sum(s => s.Cost);
            }
            /*worksWith = _db.WorksWith.OrderByDescending(work => work.UpdatedAt).ToList();*/

            if (pg < 1) pg = 1;
            int recsCount = worksWith.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = worksWith.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "workswiths", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(data);
        }

        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            var model = new WorksWith();

            var employees = _db.Employees.ToList();
            var clients = _db.Clients.ToList();
            model.EmployeeOptions = employees.Select(b => new SelectListItem
            {
                Value = b.EmpId,
                Text = $"{b.FirstName} {b.LastName}"
            }).ToList();

            model.ClientOptions = clients.Select(c => new SelectListItem
            {
                Value = c.ClientId,
                Text = c.ClientName
            }).ToList();

            // Calculate TotalSales based on selected Employee and Client
            if (!string.IsNullOrEmpty(model.EmpId) && !string.IsNullOrEmpty(model.ClientId))
            {
                model.TotalSales = _db.Sales
                    .Where(s => s.EmpId == model.EmpId && s.ClientId == model.ClientId)
                    .Sum(s => s.Cost);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(WorksWith worksWith)
        {
            if (worksWith == null)
            {
                return NotFound();
            }

            try
            {


                if (ModelState.IsValid)
                {
                    worksWith.TotalSales = _db.Sales
                    .Where(s => s.EmpId == worksWith.EmpId && s.ClientId == worksWith.ClientId)
                    .Sum(s => s.Cost);
                    _db.WorksWith.Add(worksWith);
                    _db.SaveChanges();
                    _audit.LogAudit("Create", "workswith", $"{worksWith.EmpId} - {worksWith.ClientId}", User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Record Created Successfully...";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                TempData["error"] = "Record Already exists";
                return RedirectToAction(nameof(Add));
            }

            return View(worksWith);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string empId, string clientId)
        {
            ViewBag.Action = "Edit";
            var model = _db.WorksWith.Find(empId, clientId);
            if (model == null)
            {
                return NotFound();
            }

            var worksWiths = _db.Employees.ToList();
            var clients = _db.Clients.ToList();
            model.EmployeeOptions = worksWiths.Select(b => new SelectListItem
            {
                Value = b.EmpId,
                Text = $"{b.FirstName} {b.LastName}"
            }).ToList();

            model.ClientOptions = clients.Select(c => new SelectListItem
            {
                Value = c.ClientId,
                Text = c.ClientName
            }).ToList();
            // Calculate TotalSales based on selected Employee and Client
            if (!string.IsNullOrEmpty(model.EmpId) && !string.IsNullOrEmpty(model.ClientId))
            {
                model.TotalSales = _db.Sales
                    .Where(s => s.EmpId == model.EmpId && s.ClientId == model.ClientId)
                    .Sum(s => s.Cost);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(WorksWith worksWith)
        {
            // Retrieve the employee to update from the database
            var modelToUpdate = _db.WorksWith.Find(worksWith.EmpId, worksWith.ClientId);

            if (modelToUpdate == null)
            {
                // If the employee is not found, return a NotFoundResult or handle the case appropriately
                return NotFound();
            }

            // Update the properties of the retrieved employee with the values from the posted model
            modelToUpdate.TotalSales = worksWith.TotalSales;
            modelToUpdate.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));

            if (ModelState.IsValid)
            {
                try
                {
                    _db.SaveChanges();
                    _audit.LogAudit("Update", "workswith", $"{worksWith.EmpId} - {worksWith.ClientId}", User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Record Updated Successfully...";
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
            return View(worksWith);
        }

        public IActionResult Delete(string empId, string clientId)
        {
            try
            {
                var record = _db.WorksWith.Find(empId, clientId);
                if (record != null)
                {
                    _db.WorksWith.Remove(record);
                    _db.SaveChanges();
                    _audit.LogAudit("Delete", "workswith", $"{empId} - {clientId}", User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Record Deleted Successfully...";
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

        [HttpGet]
        public IActionResult GetTotalSales(string empId, string clientId)
        {
            // Calculate total sales based on empId and clientId
            var totalSales = _db.Sales
                .Where(s => s.EmpId == empId && s.ClientId == clientId)
                .Sum(s => s.Cost);

            return Json(totalSales); // Return total sales as JSON
        }

    }
}
