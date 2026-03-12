using NeoErp.Core;
using NeoErp.Print.Service.Models;
using NeoErp.Print.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Print.Module.Controllers
{
    public class HomeController : Controller
    {
        private IPrintSetupService _printSetupService;
        private IWorkContext _workContext;

        public HomeController(IPrintSetupService printSetupService, IWorkContext workContext)
        {
            this._printSetupService = printSetupService;
            this._workContext = workContext;
        }

        // GET: Home/Index - Print Setup Page
        public ActionResult Index()
        {
            return View();
        }

        // API: Get Module List
        [HttpGet]
        public JsonResult GetModules()
        {
            try
            {
                var modules = _printSetupService.GetModuleList();
                return Json(new { success = true, data = modules }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Get Report Groups
        [HttpPost]
        public JsonResult GetReportGroups(string moduleCode)
        {
            try
            {
                string companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                var reportGroups = _printSetupService.GetReportGroups(moduleCode, companyCode);
                return Json(new { success = true, data = reportGroups }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Get Report Items
        [HttpPost]
        public JsonResult GetReportItems(string moduleCode, string masterFormCode)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetReportItems called - ModuleCode: {moduleCode}, MasterFormCode: {masterFormCode}");
                
                string companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                System.Diagnostics.Debug.WriteLine($"Company Code: {companyCode}");
                
                var reportItems = _printSetupService.GetReportItems(moduleCode, companyCode, masterFormCode);
                System.Diagnostics.Debug.WriteLine($"Report items found: {reportItems?.Count ?? 0}");
                
                if (reportItems != null && reportItems.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"First item FORM_CODE: {reportItems[0].FORM_CODE}");
                }
                
                var result = new { success = true, data = reportItems };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetReportItems: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        // ===== PRINT CONFIGURATION METHODS =====
        
        // API: Get Print Configuration
        

        // API: Get All Patterns
        [HttpPost]
        public JsonResult GetAllPatterns(string formCode, int? patternId = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetAllPatterns called - FormCode: {formCode}, PatternId: {patternId}");
                
                string companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                
                var patternConfig = _printSetupService.GetAllPatterns(formCode, companyCode, patternId);
                
                System.Diagnostics.Debug.WriteLine($"Patterns count: {patternConfig.Patterns?.Count ?? 0}");
                
                return Json(new { success = true, data = patternConfig }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in GetAllPatterns: {ex.Message}");
                return Json(new { success = false, message = $"{ex.GetType().Name}: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddPattern(string patternName, string formCode, string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(patternName))
                {
                    return Json(new { success = false, message = "Pattern name is required" }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(companyCode))
                {
                    companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                }

                int newPatternId = _printSetupService.AddPattern(patternName, formCode, companyCode);

                if (newPatternId > 0)
                {
                    return Json(new { success = true, message = "Pattern created successfully", patternId = newPatternId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Failed to create pattern" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.GetType().Name}: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SavePattern(PatternSaveRequest request)
        {
            try
            {
                if (request == null || request.PatternDetail == null)
                {
                    return Json(new { success = false, message = "Invalid pattern data" }, JsonRequestBehavior.AllowGet);
                }

                var headFields = request.HeadFields ?? new List<PatternHeadFieldModel>();
                var footerFields = request.FooterFields ?? new List<PatternFooterFieldModel>();
                var columnFields = request.ColumnFields ?? new List<PatternColumnFieldModel>();

                System.Diagnostics.Debug.WriteLine($"SavePattern - Head Fields Count: {headFields.Count}");
                System.Diagnostics.Debug.WriteLine($"SavePattern - Footer Fields Count: {footerFields.Count}");
                System.Diagnostics.Debug.WriteLine($"SavePattern - Column Fields Count: {columnFields.Count}");

                bool result = _printSetupService.SavePattern(request.PatternDetail, headFields, footerFields, columnFields);

                if (result)
                {
                    return Json(new { success = true, message = "Pattern saved successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save pattern" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.GetType().Name}: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AutoFillPattern(string formCode, int patternId)
        {
            try
            {
                if (string.IsNullOrEmpty(formCode))
                {
                    return Json(new { success = false, message = "Form code is required" }, JsonRequestBehavior.AllowGet);
                }

                if (patternId <= 0)
                {
                    return Json(new { success = false, message = "Invalid pattern ID" }, JsonRequestBehavior.AllowGet);
                }

                string companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";

                var result = _printSetupService.AutoFillPattern(formCode, patternId, companyCode);

                if (result != null)
                {
                    return Json(new 
                    { 
                        success = true, 
                        message = "Pattern data fetched successfully",
                        headFields = result.HeadFields,
                        footerFields = result.FooterFields,
                        columnFields = result.ColumnFields
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Failed to fetch pattern data" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.GetType().Name}: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GenerateSqlQuery(string formCode, int patternId)
        {
            try
            {
                string companyCode = "01"; // Hardcoded company code
                string sqlQuery = _printSetupService.GenerateSqlQuery(formCode, patternId, companyCode);
                
                return Json(new
                {
                    success = true,
                    sqlQuery = sqlQuery
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // API: Get Preview Data (JSON)
        [HttpGet]
        public JsonResult GetPreviewDataApi(string formCode, string companyCode = "01", string mainFieldValue = "", int activePatternId = 0, string filters = "")
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                }

                var previewData = _printSetupService.GetPreviewData(formCode, mainFieldValue, companyCode, activePatternId, filters);
                
                return Json(new { 
                    success = true, 
                    data = previewData 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // View: Preview Pattern Page
        [HttpGet]
        public ActionResult PreviewPattern(string formCode, string companyCode = "01", string mainFieldValue = "", int activePatternId = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                }

                // Get only the file name to determine the view
                var fileName = _printSetupService.GetPreviewFileName(formCode, companyCode, activePatternId);
                
                // Pass data to view via ViewBag
                ViewBag.FormCode = formCode;
                ViewBag.CompanyCode = companyCode;
                ViewBag.MainFieldValue = mainFieldValue;
                ViewBag.ActivePatternId = activePatternId;
                ViewBag.FileName = fileName;
                
                // Clean the filename - remove .cshtml extension if present
                string viewName = fileName;
                if (!string.IsNullOrEmpty(viewName))
                {
                    // Remove .cshtml, .html, .htm extensions
                    viewName = System.IO.Path.GetFileNameWithoutExtension(viewName);
                }
                
                // Return the view based on cleaned filename
                // For example: "salep.cshtml" → "salep"
                // This will look for Views/Home/salep.cshtml
                return View(viewName);
            }
            catch (Exception ex)
            {
                // If view not found or error, return error view
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View("PreviewError");
            }
        }

        // ===== MENU MANAGEMENT API ENDPOINTS =====

        [HttpGet]
        public JsonResult GetMenuModules()
        {
            try
            {
                var modules = _printSetupService.GetMenuModules();
                return Json(new { success = true, data = modules }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetMenuGroups(string moduleCode)
        {
            try
            {
                if (string.IsNullOrEmpty(moduleCode))
                {
                    return Json(new { success = false, message = "Module code is required" }, JsonRequestBehavior.AllowGet);
                }

                var groups = _printSetupService.GetMenuGroups(moduleCode);
                return Json(new { success = true, data = groups }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetNextMenuCode(string moduleCode, string groupMenuNo)
        {
            try
            {
                if (string.IsNullOrEmpty(moduleCode) || string.IsNullOrEmpty(groupMenuNo))
                {
                    return Json(new { success = false, message = "Module code and group menu number are required" }, JsonRequestBehavior.AllowGet);
                }

                string nextCode = _printSetupService.GetNextMenuCode(moduleCode, groupMenuNo);
                return Json(new { success = true, nextMenuCode = nextCode }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveNewMenu(SaveMenuRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Invalid request" }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(request.ModuleCode) || 
                    string.IsNullOrEmpty(request.GroupMenuNo) ||
                    string.IsNullOrEmpty(request.MenuEdesc))
                {
                    return Json(new { success = false, message = "Module, Group, and Menu Description are required" }, JsonRequestBehavior.AllowGet);
                }

                string companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                
                bool result = _printSetupService.SaveNewMenu(request, companyCode);

                if (result)
                {
                    string newMenuCode = _printSetupService.GetNextMenuCode(request.ModuleCode, request.GroupMenuNo);
                    // Actually the menu was already saved, so we need to return the previous next code
                    // But for simplicity, let's just return success
                    return Json(new { success = true, message = "Menu saved successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save menu" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetMenuInfo(string menuNo)
        {
            try
            {
                if (string.IsNullOrEmpty(menuNo))
                {
                    return Json(new { success = false, message = "Menu number is required" }, JsonRequestBehavior.AllowGet);
                }

                var menuInfo = _printSetupService.GetMenuInfo(menuNo);
                
                if (menuInfo != null)
                {
                    return Json(new { success = true, data = menuInfo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Menu not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ===== DYNAMIC FILTER API ENDPOINTS =====

        [HttpGet]
        public JsonResult GetQueryPlaceholders(string sqlQuery)
        {
            try
            {
                var placeholders = _printSetupService.ExtractPlaceholders(sqlQuery);
                return Json(new { success = true, placeholders = placeholders }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult SearchEntity(string entityType, string searchTerm = "", int pageNumber = 1, int pageSize = 100)
        {
            try
            {
                string companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                var result = _printSetupService.SearchEntity(entityType, searchTerm, companyCode, pageNumber, pageSize);
                
                return Json(new
                {
                    success = true,
                    data = result.Data,
                    totalCount = result.TotalCount,
                    hasMore = result.HasMore
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetEntityChildren(string entityType, string masterCode)
        {
            try
            {
                string companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";
                var codes = _printSetupService.GetEntityChildren(entityType, masterCode, companyCode);
                
                return Json(new { success = true, codes = codes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult PrintCount(string formCode, string mainFieldValue = "")
        {
            try
            {
                string companyCode = _workContext.CurrentUserinformation?.company_code ?? "01";

                var printCount = _printSetupService.PrintCount(formCode, mainFieldValue, companyCode);

                return Json(new { success = true, printCount = printCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }






    }



    // Helper class for receiving pattern save request
    public class PatternSaveRequest
    {
        public PatternDetailModel PatternDetail { get; set; }
        public List<PatternHeadFieldModel> HeadFields { get; set; }
        public List<PatternFooterFieldModel> FooterFields { get; set; }
        public List<PatternColumnFieldModel> ColumnFields { get; set; }
    }
}