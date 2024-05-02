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
using Microsoft.CodeAnalysis.Operations;

namespace CompanyManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    public class BranchSuppliersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly Audit _audit;

        public BranchSuppliersController(ApplicationDbContext db, Audit audit)
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
            List<BranchSupplier> branchSuppliers;
            if (SearchText != "" && SearchText != null)
            {
                branchSuppliers = _db.BranchesSupplier
                    .Where(cat => cat.SupplierName.Contains(SearchText))
                    .ToList();
            }
            else
                branchSuppliers = _db.BranchesSupplier.OrderByDescending(branch => branch.UpdatedAt).ToList();

            if (pg < 1) pg = 1;
            int recsCount = branchSuppliers.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = branchSuppliers.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "branchsuppliers", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(data);
        }

        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            var model = new BranchSupplier();
            var branches = _db.Branches.ToList();
            model.BranchOptions = branches.Select(b => new SelectListItem
            {
                Value = b.BranchId,
                Text = b.BranchName
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(BranchSupplier branchSupplier)
        {
            if (branchSupplier == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.BranchesSupplier.Add(branchSupplier);
                _db.SaveChanges();
                _audit.LogAudit("Create", "branchessupplier", $"{branchSupplier.BranchId} - {branchSupplier.SupplierName}", User.Identity.Name, _db);
                TempData["AlertMessage"] = "Supplier Created Successfully...";
                return RedirectToAction(nameof(Index));
            }

            return View(branchSupplier);
        }

        public IActionResult Edit(string id, string supplierName)
        {
            ViewBag.Action = "Edit";
            var supplier = _db.BranchesSupplier.Find(id, supplierName);
            if (supplier == null)
            {
                return NotFound();
            }

            // Populate branch options
            // Assuming you have a Branch model with appropriate properties
            var branches = _db.Branches.ToList();
            supplier.BranchOptions = branches.Select(b => new SelectListItem
            {
                Value = b.BranchId,
                Text = b.BranchName,
            }).ToList();

            return View(supplier);
        }

        [HttpPost]
        public IActionResult Edit(BranchSupplier supplier)
        {
            // Retrieve the employee to update from the database
            var supplierToUpdate = _db.BranchesSupplier.Find(supplier.BranchId, supplier.SupplierName);

            if (supplierToUpdate == null)
            {
                // If the employee is not found, return a NotFoundResult or handle the case appropriately
                return NotFound();
            }

            // Update the properties of the retrieved employee with the values from the posted model
            supplierToUpdate.SupplyType = supplier.SupplyType;
            supplierToUpdate.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));

            if (ModelState.IsValid)
            {
                try
                {
                    _db.SaveChanges();
                    _audit.LogAudit("Update", "branchessupplier", $"{supplier.BranchId} - {supplier.SupplierName}", User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Supplier Updated Successfully...";
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
            return View(supplier);
        }

        public IActionResult Delete(string branchId, string supplierName)
        {
            try
            {
                var record = _db.BranchesSupplier.Find(branchId, supplierName);
                if (record != null)
                {
                    _db.BranchesSupplier.Remove(record);
                    _db.SaveChanges();
                    _audit.LogAudit("Delete", "branchessupplier", $"{ branchId} - {supplierName}", User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Supplier Deleted Successfully...";
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
