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
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly Audit _audit;

        public SalesController(ApplicationDbContext db, Audit audit)
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
            List<Sale> sales;
            if (SearchText != "" && SearchText != null)
            {
                sales = _db.Sales
                    .Where(work => work.ClientId.Contains(SearchText) || work.ProductType.Contains(SearchText))
                    .ToList();
            }
            else
                sales = _db.Sales.OrderByDescending(work => work.UpdatedAt).ToList();

            if (pg < 1) pg = 1;
            int recsCount = sales.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = sales.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "sales", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Action = "Create";
            var model = new Sale();
            var employees = _db.Employees.ToList();
            var clients = _db.Clients.ToList();
            var products = _db.Products.ToList();
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

            model.ProductOptions = products.Select(p => new SelectListItem
            {
                Value = p.Name,
                Text = p.Name
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Sale sale)
        {
            if (sale == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Sales.Add(sale);
                _db.SaveChanges();
                _audit.LogAudit("Create", "sales", sale.SaleId, User.Identity.Name, _db);
                TempData["AlertMessage"] = "Sale Made Successfully...";
                return RedirectToAction(nameof(Index));
            }

            return View(sale);
        }

        public IActionResult Edit(string saleId)
        {
            ViewBag.Action = "Edit";
            var model = _db.Sales.Find(saleId);
            if (model == null)
            {
                return NotFound();
            }

            var employee = _db.Employees.ToList();
            var clients = _db.Clients.ToList();
            var products = _db.Products.ToList();
            model.EmployeeOptions = employee.Select(b => new SelectListItem
            {
                Value = b.EmpId,
                Text = $"{b.FirstName} {b.LastName}"
            }).ToList();

            model.ClientOptions = clients.Select(c => new SelectListItem
            {
                Value = c.ClientId,
                Text = c.ClientName
            }).ToList();

            model.ProductOptions = products.Select(p => new SelectListItem
            {
                Value = p.Name,
                Text = p.Name
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Sale sale)
        {
            // Retrieve the employee to update from the database
            var modelToUpdate = _db.Sales.Find(sale.SaleId);

            if (modelToUpdate == null)
            {
                // If the employee is not found, return a NotFoundResult or handle the case appropriately
                return NotFound();
            }

            // Update the properties of the retrieved employee with the values from the posted model
            modelToUpdate.EmpId = sale.EmpId;
            modelToUpdate.ClientId = sale.ClientId;
            modelToUpdate.Cost = sale.Cost;
            modelToUpdate.ProductType = sale.ProductType;
            modelToUpdate.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));

            if (ModelState.IsValid)
            {
                try
                {
                    _db.SaveChanges();
                    _audit.LogAudit("Update", "sales", sale.SaleId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Sale Updated Successfully...";
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
            return View(sale);
        }

        public IActionResult Delete(string saleId)
        {
            try
            {
                var sale = _db.Sales.Find(saleId);
                if (sale != null)
                {
                    _db.Sales.Remove(sale);
                    _db.SaveChanges();
                    _audit.LogAudit("Delete", "sales", saleId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Sale Deleted Successfully...";
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
