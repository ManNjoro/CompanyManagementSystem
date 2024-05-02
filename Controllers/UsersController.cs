using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using CompanyManagementSystem.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using NuGet.Versioning;

namespace CompanyManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly Audit _audit;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db, Audit audit)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
            List<ApplicationUser> users;
            if (SearchText != "" && SearchText != null)
            {
                users = _userManager.Users
                    .Where(cat => cat.Firstname.Contains(SearchText))
                    .ToList();
            }
            else
                users = _userManager.Users.OrderByDescending(employee => employee.UpdatedAt).ToList();

            if (pg < 1) pg = 1;
            int recsCount = users.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = users.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "Users", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(data);
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Action = "Edit";
            if (id == null)
            {
                return NotFound();
            }
            UserRole userRole = new UserRole();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userInRole = _db.UserRoles.Where(x => x.UserId == id).Select(x => x.RoleId).ToList();
            userRole.applicationUser = user;
            userRole.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem 
            { 
                Text = r.Name, 
                Value = r.Id,
                Selected = userInRole.Contains(r.Id)
            }).ToList();
            

            return View(userRole);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserRole model)
        {
            var user = await _userManager.FindByIdAsync(model.applicationUser.Id);
            if (user == null)
            {
                return NotFound();
            }


            // Update user properties
            user.Firstname = model.applicationUser.Firstname;
            user.Lastname = model.applicationUser.Lastname;
            user.Email = model.applicationUser.Email;
            user.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time"));

            try
            {
                if (ModelState.IsValid)
                {

                    // Update user role
                    var selectedRoleId = model.SelectedRole;
                    var role = await _roleManager.FindByIdAsync(selectedRoleId);
                    if (role != null)
                    {
                        // Clear existing roles
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);

                        // Add new role
                        var result = await _userManager.AddToRoleAsync(user, role.Name);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }
                    }

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (updateResult.Succeeded)
                    {
                        TempData["AlertMessage"] = "User updated successfully.";
                        _audit.LogAudit("Update", "aspnetusers", model.applicationUser.Id, User.Identity.Name, _db);
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = $"An error occurred while updating user: {ex.Message}";
                // Log the exception for further investigation
                ModelState.AddModelError(string.Empty, "An error occurred while updating user.");
            }

            // Repopulate the roles dropdown if there are validation errors
            if (!ModelState.IsValid)
            {
                var userInRole = _db.UserRoles.Where(x => x.UserId == model.applicationUser.Id).Select(x => x.RoleId).ToList();
                model.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id,
                    Selected = userInRole.Contains(r.Id)
                }).ToList();
            }

            return View(model);
        }




        // POST: /Users/Delete/{id}
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["AlertMessage"] = "User Deleted Successfully...";
                _audit.LogAudit("Delete", "aspnetusers", userId, User.Identity.Name, _db);
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If deletion fails, return to the edit page or handle the error appropriately
            return View("Edit", user);
        }

    }
}
