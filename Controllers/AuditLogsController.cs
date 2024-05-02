using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using CompanyManagementSystem.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyManagementSystem.Controllers
{
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PageSize _pageSize;
        public AuditLogsController(ApplicationDbContext db, PageSize pageSize)
        {
            _db = db;
            _pageSize = pageSize;
        }
        // GET: AuditLogsController
        public IActionResult Index(int pg = 1, string SearchText = "", int pageSize = 5)
        {
            List<AuditLogs> auditLogs;
            if (SearchText != "" && SearchText != null)
            {
                auditLogs = _db.AuditLogs
                    .Where(log => log.TableName.Contains(SearchText) || log.ActionType.Contains(SearchText))
                    .ToList();
            }
            else
                auditLogs = _db.AuditLogs.OrderByDescending(log => log.Timestamp).ToList();

            if (pg < 1) pg = 1;
            int recsCount = auditLogs.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = auditLogs.Skip(recSkip).Take(pager.PageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "auditlogs", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;
            this.ViewBag.PageSizes = _pageSize.GetSize(pageSize);
            return View(data);
        }


    }
}
