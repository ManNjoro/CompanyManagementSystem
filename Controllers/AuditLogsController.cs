using CompanyManagementSystem.Data;
using CompanyManagementSystem.Models;
using CompanyManagementSystem.Views.Shared.Components.SearchBar;
using FastReport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyManagementSystem.Controllers
{
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly PageSize _pageSize;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuditLogsController(ApplicationDbContext db, PageSize pageSize, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _pageSize = pageSize;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: AuditLogsController
        public IActionResult Index(int pg = 1, string SearchText = "", int pageSize = 5, string SortBy = "", string direction="down")
        {
            List<AuditLogs> auditLogs;
            if (SearchText != "" && SearchText != null)
            {
                auditLogs = _db.AuditLogs
                    .Where(log => log.TableName.Contains(SearchText) || log.ActionType.Contains(SearchText))
                    .ToList();
            }
            else
            {

                auditLogs = _db.AuditLogs.ToList();
            }
            switch (SortBy)
            {
                case "ActionType":
                    if (direction == "down")
                    {
                        
                    auditLogs = auditLogs.OrderByDescending(log => log.ActionType).ToList();
                    } else if(direction == "up")
                    {
                        auditLogs = auditLogs.OrderBy(log => log.ActionType).ToList();
                    }
                    break;
                case "TableName":
                    if (direction == "down")
                        auditLogs = auditLogs.OrderByDescending(log => log.TableName).ToList();
                    else if (direction == "up")
                        auditLogs = auditLogs.OrderBy(log => log.TableName).ToList();
                    break;
                default:
                    if (direction == "down")
                        auditLogs = auditLogs.OrderByDescending(log => log.Timestamp).ToList();
                    else if(direction == "up")
                        auditLogs = auditLogs.OrderBy(log => log.Timestamp).ToList();
                    break;
            }

            List<SelectListItem> tableColumns = new List<SelectListItem>{
                new SelectListItem {Text = "ActionType"},
                new SelectListItem {Text = "TableName"},
                new SelectListItem {Text = "TimeStamp"},
            }.ToList();
            ViewBag.TableColumns = tableColumns;
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

        /// <summary>
        /// Generate a PDF report of employees
        /// </summary>
        /// <returns>PDF file result</returns>
        public FileResult Generate()
        {
            FastReport.Utils.Config.WebMode = true;
            Report rep = new Report();
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Logs.frx");
            rep.Load(path);

            // Add your report generation logic here
            var logs = _db.AuditLogs.OrderByDescending(u => u.Timestamp).ToList();
            rep.RegisterData(logs, "AuditRef");

            if (rep.Report.Prepare())
            {
                try
                {

                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfExport.ShowProgress = false;
                pdfExport.Subject = "Subject Report";
                pdfExport.Title = "AuditTrails Report";
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                rep.Report.Export(pdfExport, ms);
                rep.Dispose();
                pdfExport.Dispose();
                ms.Position = 0;
                return File(ms, "application/pdf", "AuditTrailsReport.pdf");
                } catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


    }
}
