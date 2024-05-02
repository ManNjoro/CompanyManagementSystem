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
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly Audit _audit;

        public ClientsController(ApplicationDbContext db, Audit audit)
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
            List<Client> clients;
            if (SearchText != "" && SearchText != null)
            {
                clients = _db.Clients
                    .Where(cat => cat.ClientName.Contains(SearchText))
                    .ToList();
            }
            else
                clients = _db.Clients.OrderByDescending(client => client.UpdatedAt).ToList();

            if (pg < 1) pg = 1;
            int recsCount = clients.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = clients.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "clients", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(data);
        }

        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            var model = new Client();
            var branches = _db.Branches.ToList();
            model.BranchOptions = branches.Select(b => new SelectListItem
            {
                Value = b.BranchId,
                Text = b.BranchName
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(Client client)
        {
            if (client == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Clients.Add(client);
                _db.SaveChanges();
                _audit.LogAudit("Create", "clients", client.ClientId, User.Identity.Name, _db);
                TempData["AlertMessage"] = "Client Created Successfully...";
                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        public IActionResult Edit(string id)
        {
            ViewBag.Action = "Edit";
            var client = _db.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            // Populate branch options
            // Assuming you have a Branch model with appropriate properties
            var branches = _db.Branches.ToList();
            client.BranchOptions = branches.Select(b => new SelectListItem
            {
                Value = b.BranchId,
                Text = b.BranchName,
            }).ToList();

            return View(client);
        }

        [HttpPost]
        public IActionResult Edit(Client client)
        {
            // Retrieve the employee to update from the database
            var clientToUpdate = _db.Clients.Find(client.ClientId);

            if (clientToUpdate == null)
            {
                // If the employee is not found, return a NotFoundResult or handle the case appropriately
                return NotFound();
            }

            // Update the properties of the retrieved employee with the values from the posted model
            clientToUpdate.ClientName = client.ClientName;
            clientToUpdate.BranchId = client.BranchId;
            clientToUpdate.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));

            if (ModelState.IsValid)
            {
                try
                {
                    _db.SaveChanges();
                    _audit.LogAudit("Update", "clients", client.ClientId, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Client Updated Successfully...";
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
            return View(client);
        }

        public IActionResult Delete(string clientid)
        {
            try
            {
                var client = _db.Clients.Find(clientid);
                if (client != null)
                {
                    _db.Clients.Remove(client);
                    _db.SaveChanges();
                    _audit.LogAudit("Delete", "clients", clientid, User.Identity.Name, _db);
                    TempData["AlertMessage"] = "Client Deleted Successfully...";
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
