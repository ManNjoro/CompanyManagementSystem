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
using CompanyManagementSystem.Services;
using Microsoft.AspNetCore.Identity;

namespace CompanyManagementSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileService _fileService;
        private readonly Audit _audit;
        public ProductsController(ApplicationDbContext db, IFileService fileService, Audit audit)
        {
            _db = db;
            this._fileService = fileService;
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
            List<Product> products;
            if (SearchText != "" && SearchText != null)
            {
                products = _db.Products
                    .Where(cat => cat.Name.Contains(SearchText) || cat.Description.Contains(SearchText))
                    .ToList();
            }
            else
                products = _db.Products.OrderByDescending(product => product.UpdatedAt).ToList();

            if (pg < 1) pg = 1;
            int recsCount = products.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "products", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Action = "Create";

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile? imageFile)
        {
            if (product == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    // Save the uploaded image and get the path
                    var result = _fileService.SaveImage(imageFile);
                    if (result.Item1 == 1)
                    {
                        var oldImage = product.ProductImage;
                        product.ProductImage = result.Item2;
                    }
                }
                try
                {

                    _db.Products.Add(product);
                    _db.SaveChanges();
                    _audit.LogAudit("Create", "products", product.ProductId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Product Created Successfully...";
                }
                catch (Exception e)
                {
                    TempData["AlertMessage"] = "Product was not created";
                }
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public IActionResult Edit(string id)
        {
            ViewBag.Action = "Edit";
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? imageFile)
        {
            // Retrieve the employee to update from the database
            var productToUpdate = _db.Products.Find(product.ProductId);

            if (productToUpdate == null)
            {
                // If the employee is not found, return a NotFoundResult or handle the case appropriately
                return NotFound();
            }

            // Update the properties of the retrieved employee with the values from the posted model
            productToUpdate.Name = product.Name;
            productToUpdate.Description = product.Description;
            productToUpdate.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));
            if (imageFile != null)
            {
                // Save the uploaded image and update the product image path
                var result = _fileService.SaveImage(imageFile);
                if (result.Item1 == 1)
                {
                    var oldImage = productToUpdate.ProductImage;
                    productToUpdate.ProductImage = result.Item2;
                    var deleteResult = _fileService.DeleteImage(oldImage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.SaveChanges();
                    TempData["AlertMessage"] = "Product Updated Successfully...";
                    _audit.LogAudit("Update", "products", product.ProductId, User.Identity.Name, _db);
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
            return View(product);
        }

        public IActionResult Delete(string productId)
        {
            try
            {
                var product = _db.Products.Find(productId);
                if (product != null)
                {
                    _db.Products.Remove(product);
                    _db.SaveChanges();
                    _audit.LogAudit("Delete", "products", productId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Product Deleted Successfully...";
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
