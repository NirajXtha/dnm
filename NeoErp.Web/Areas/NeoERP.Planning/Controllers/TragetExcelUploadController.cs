using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using NeoErp.Core.Domain;
using NeoErp.Core;
using Newtonsoft.Json;

namespace NeoErp.Planning.Controllers
{
    public class TragetExcelUploadController : Controller
    {
        private readonly ITargetExcelUploadRepo _targetExcelUploadRepo;
        private readonly IWorkContext _workContext;

        public TragetExcelUploadController(ITargetExcelUploadRepo targetExcelUploadRepo, IWorkContext workContext)
        {
            this._targetExcelUploadRepo = targetExcelUploadRepo;
            this._workContext = workContext;
        }

        // GET: TragetExcelUpload (Default View)
        public ActionResult ExcelUpload()
        {
            return View("Index");
        }

        // POST: Upload Excel File
        [HttpPost]
        public ContentResult UploadExcel()
        {
            try
            {
                if (Request.Files.Count == 0)
                {
                    var errorResult = JsonConvert.SerializeObject(new { success = false, message = "No file uploaded" }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                    return Content(errorResult, "application/json");
                }

                var file = Request.Files[0];
                if (file == null || file.ContentLength == 0)
                {
                    var errorResult = JsonConvert.SerializeObject(new { success = false, message = "Empty file uploaded" }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                    return Content(errorResult, "application/json");
                }

                var rows = new List<TargetExcelUploadRowViewModel>();

                using (var package = new ExcelPackage(file.InputStream))
                {
                    // EPPlus 4.x - use FirstOrDefault() for safer access
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    
                    if (worksheet == null)
                    {
                        var errorResult = JsonConvert.SerializeObject(new { success = false, message = "No worksheet found in Excel file" }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                        return Content(errorResult, "application/json");
                    }
                    
                    if (worksheet.Dimension == null)
                    {
                        var errorResult = JsonConvert.SerializeObject(new { success = false, message = "Worksheet is empty" }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                        return Content(errorResult, "application/json");
                    }
                    
                    int rowCount = worksheet.Dimension.Rows;

                    // Start from row 2 (assuming row 1 is header)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var rowData = new TargetExcelUploadRowViewModel();

                        try
                        {
                            // Read FROM_DATE (column 1)
                            var fromDateValue = worksheet.Cells[row, 1].Value;
                            if (fromDateValue != null)
                            {
                                if (fromDateValue is DateTime)
                                    rowData.FROM_DATE = (DateTime)fromDateValue;
                                else if (DateTime.TryParse(fromDateValue.ToString(), out DateTime fromDate))
                                    rowData.FROM_DATE = fromDate;
                            }

                            // Read TO_DATE (column 2)
                            var toDateValue = worksheet.Cells[row, 2].Value;
                            if (toDateValue != null)
                            {
                                if (toDateValue is DateTime)
                                    rowData.TO_DATE = (DateTime)toDateValue;
                                else if (DateTime.TryParse(toDateValue.ToString(), out DateTime toDate))
                                    rowData.TO_DATE = toDate;
                            }

                            // Read EMP_CODE (column 3)
                            rowData.EMP_CODE = worksheet.Cells[row, 3].Value?.ToString()?.Trim();

                            // Read DISTRIBUTOR_CODE (column 4)
                            rowData.DISTRIBUTOR_CODE = worksheet.Cells[row, 4].Value?.ToString()?.Trim();

                            // Read ITEM_CODE (column 5)
                            rowData.ITEM_CODE = worksheet.Cells[row, 5].Value?.ToString()?.Trim();

                            // Read QTY (column 6)
                            var qtyValue = worksheet.Cells[row, 6].Value;
                            if (qtyValue != null && decimal.TryParse(qtyValue.ToString(), out decimal qty))
                                rowData.QTY = qty;

                            // Read PRICE (column 7)
                            var priceValue = worksheet.Cells[row, 7].Value;
                            if (priceValue != null && decimal.TryParse(priceValue.ToString(), out decimal price))
                                rowData.PRICE = price;

                            // Read BRANCH_CODE (column 8)
                            rowData.BRANCH_CODE = worksheet.Cells[row, 8].Value?.ToString()?.Trim();

                            // Skip completely empty rows
                            if (string.IsNullOrWhiteSpace(rowData.EMP_CODE) &&
                                string.IsNullOrWhiteSpace(rowData.DISTRIBUTOR_CODE) &&
                                string.IsNullOrWhiteSpace(rowData.ITEM_CODE) &&
                                string.IsNullOrWhiteSpace(rowData.BRANCH_CODE))
                            {
                                continue;
                            }

                            rows.Add(rowData);
                        }
                        catch (Exception ex)
                        {
                            rowData.ValidationErrors.Add($"Error reading row {row}: {ex.Message}");
                            rowData.IsValid = false;
                            rows.Add(rowData);
                        }
                    }
                }

                var jsonResult = JsonConvert.SerializeObject(new { success = true, data = rows }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                var errorResult = JsonConvert.SerializeObject(new { success = false, message = $"Error: {ex.Message}" }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                return Content(errorResult, "application/json");
            }
        }

        // POST: Validate Data
        [HttpPost]
        public ContentResult ValidateData(List<TargetExcelUploadRowViewModel> rows)
        {
            try
            {
                string companyCode = _workContext.CurrentUserinformation.company_code;

                // Get all validation data from database
                var validDistributors = _targetExcelUploadRepo.ValidateDistributors(companyCode);
                var validItems = _targetExcelUploadRepo.ValidateItems(companyCode);
                var validBranches = _targetExcelUploadRepo.ValidateBranches(companyCode);
                var validEmployees = _targetExcelUploadRepo.ValidateEmployees(companyCode);

                // Create lookup dictionaries for faster validation (case-insensitive)
                var distributorDict = validDistributors.ToDictionary(
                    d => d.DISTRIBUTOR_CODE.ToUpper(),
                    d => d.DISTRIBUTOR_NAME,
                    StringComparer.OrdinalIgnoreCase
                );
                var distributorNameDict = validDistributors.ToDictionary(
                    d => d.DISTRIBUTOR_NAME.ToUpper(),
                    d => d.DISTRIBUTOR_CODE,
                    StringComparer.OrdinalIgnoreCase
                );

                var itemDict = validItems.ToDictionary(
                    i => i.ITEM_CODE.ToUpper(),
                    i => i.ITEM_NAME,
                    StringComparer.OrdinalIgnoreCase
                );
                var itemNameDict = validItems.ToDictionary(
                    i => i.ITEM_NAME.ToUpper(),
                    i => i.ITEM_CODE,
                    StringComparer.OrdinalIgnoreCase
                );

                var branchDict = validBranches.ToDictionary(
                    b => b.BRANCH_CODE.ToUpper(),
                    b => b.BRANCH_NAME,
                    StringComparer.OrdinalIgnoreCase
                );
                var branchNameDict = validBranches.ToDictionary(
                    b => b.BRANCH_NAME.ToUpper(),
                    b => b.BRANCH_CODE,
                    StringComparer.OrdinalIgnoreCase
                );

                var employeeDict = validEmployees.ToDictionary(
                    e => e.EMPLOYEE_CODE.ToUpper(),
                    e => e.EMPLOYEE_NAME,
                    StringComparer.OrdinalIgnoreCase
                );
                var employeeNameDict = validEmployees.ToDictionary(
                    e => e.EMPLOYEE_NAME.ToUpper(),
                    e => e.EMPLOYEE_CODE,
                    StringComparer.OrdinalIgnoreCase
                );

                // Validate each row
                foreach (var row in rows)
                {
                    row.ValidationErrors.Clear();
                    row.IsValid = true;

                    // Validate Employee
                    if (!string.IsNullOrWhiteSpace(row.EMP_CODE))
                    {
                        var empKey = row.EMP_CODE.ToUpper();

                        if (employeeDict.ContainsKey(empKey))
                        {
                            row.EMP_NAME = employeeDict[empKey];
                            row.IsEmployeeValid = true;
                        }
                        else if (employeeNameDict.ContainsKey(empKey))
                        {
                            row.EMP_CODE = employeeNameDict[empKey];
                            row.EMP_NAME = empKey;
                            row.IsEmployeeValid = true;
                        }
                        else
                        {
                            row.ValidationErrors.Add("Invalid Employee Code/Name");
                            row.IsEmployeeValid = false;
                            row.IsValid = false;
                        }
                    }
                    else
                    {
                        row.ValidationErrors.Add("Employee Code is required");
                        row.IsEmployeeValid = false;
                        row.IsValid = false;
                    }

                    // Validate Distributor
                    if (!string.IsNullOrWhiteSpace(row.DISTRIBUTOR_CODE))
                    {
                        var distKey = row.DISTRIBUTOR_CODE.ToUpper();
                        if (distributorDict.ContainsKey(distKey))
                        {
                            row.DISTRIBUTOR_NAME = distributorDict[distKey];
                            row.IsDistributorValid = true;
                        }
                        else if (distributorNameDict.ContainsKey(distKey))
                        {
                            row.DISTRIBUTOR_CODE = distributorNameDict[distKey];
                            row.DISTRIBUTOR_NAME = distKey;
                            row.IsDistributorValid = true;
                        }
                        else
                        {
                            row.ValidationErrors.Add("Invalid Distributor Code/Name");
                            row.IsDistributorValid = false;
                            row.IsValid = false;
                        }
                    }
                    else
                    {
                        row.ValidationErrors.Add("Distributor Code is required");
                        row.IsDistributorValid = false;
                        row.IsValid = false;
                    }

                    // Validate Item
                    if (!string.IsNullOrWhiteSpace(row.ITEM_CODE))
                    {
                        var itemKey = row.ITEM_CODE.ToUpper();
                        if (itemDict.ContainsKey(itemKey))
                        {
                            row.ITEM_NAME = itemDict[itemKey];
                            row.IsItemValid = true;
                        }
                        else if (itemNameDict.ContainsKey(itemKey))
                        {
                            row.ITEM_CODE = itemNameDict[itemKey];
                            row.ITEM_NAME = itemKey;
                            row.IsItemValid = true;
                        }
                        else
                        {
                            row.ValidationErrors.Add("Invalid Item Code/Name");
                            row.IsItemValid = false;
                            row.IsValid = false;
                        }
                    }
                    else
                    {
                        row.ValidationErrors.Add("Item Code is required");
                        row.IsItemValid = false;
                        row.IsValid = false;
                    }

                    // Validate Branch
                    if (!string.IsNullOrWhiteSpace(row.BRANCH_CODE))
                    {
                        var branchKey = row.BRANCH_CODE.ToUpper();
                        if (branchDict.ContainsKey(branchKey))
                        {
                            row.BRANCH_NAME = branchDict[branchKey];
                            row.IsBranchValid = true;
                        }
                        else if (branchNameDict.ContainsKey(branchKey))
                        {
                            row.BRANCH_CODE = branchNameDict[branchKey];
                            row.BRANCH_NAME = branchKey;
                            row.IsBranchValid = true;
                        }
                        else
                        {
                            row.ValidationErrors.Add("Invalid Branch Code/Name");
                            row.IsBranchValid = false;
                            row.IsValid = false;
                        }
                    }
                    else
                    {
                        row.ValidationErrors.Add("Branch Code is required");
                        row.IsBranchValid = false;
                        row.IsValid = false;
                    }

                    // Validate Dates
                    if (!row.FROM_DATE.HasValue)
                    {
                        row.ValidationErrors.Add("From Date is required");
                        row.IsValid = false;
                    }
                    if (!row.TO_DATE.HasValue)
                    {
                        row.ValidationErrors.Add("To Date is required");
                        row.IsValid = false;
                    }
                    if (row.FROM_DATE.HasValue && row.TO_DATE.HasValue && row.FROM_DATE > row.TO_DATE)
                    {
                        row.ValidationErrors.Add("From Date cannot be greater than To Date");
                        row.IsValid = false;
                    }

                    // Validate Quantity and Price
                    if (!row.QTY.HasValue || row.QTY <= 0)
                    {
                        row.ValidationErrors.Add("Quantity must be greater than 0");
                        row.IsValid = false;
                    }
                    if (!row.PRICE.HasValue || row.PRICE <= 0)
                    {
                        row.ValidationErrors.Add("Price must be greater than 0");
                        row.IsValid = false;
                    }
                }

                var validCount = rows.Count(r => r.IsValid);
                var invalidCount = rows.Count(r => !r.IsValid);

                var jsonResult = JsonConvert.SerializeObject(new
                {
                    success = true,
                    data = rows,
                    summary = new
                    {
                        total = rows.Count,
                        valid = validCount,
                        invalid = invalidCount
                    }
                }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                var errorResult = JsonConvert.SerializeObject(new { success = false, message = $"Validation Error: {ex.Message}" }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                return Content(errorResult, "application/json");
            }
        }

        // POST: Save Valid Data
        [HttpPost]
        public JsonResult SaveData(List<TargetExcelUploadRowViewModel> rows)
        {
            try
            {
                // Filter only valid rows
                var validRows = rows.Where(r => r.IsValid).ToList();

                if (!validRows.Any())
                {
                    return Json(new { success = false, message = "No valid rows to save" });
                }

                // Convert to model and save
                var modelsToSave = validRows.Select(r => new TargetExcelUpload
                {
                    FROM_DATE = r.FROM_DATE,
                    TO_DATE = r.TO_DATE,
                    EMP_CODE = r.EMP_CODE,
                    DISTRIBUTOR_CODE = r.DISTRIBUTOR_CODE,
                    ITEM_CODE = r.ITEM_CODE,
                    QTY = r.QTY,
                    PRICE = r.PRICE,
                    BRANCH_CODE = r.BRANCH_CODE
                }).ToList();

                int savedCount = _targetExcelUploadRepo.InsertTargetExcelUploadList(modelsToSave);

                return Json(new
                {
                    success = true,
                    message = $"Successfully saved {savedCount} records",
                    savedCount = savedCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Save Error: {ex.Message}" });
            }
        }

        // NEW API ENDPOINTS

        // 1. GET: Employee Filter List (for debouncing/autocomplete)
        [HttpGet]
        public ContentResult GetEmployeeFilterList(int limit = 50)
        {
            try
            {
                string companyCode = _workContext.CurrentUserinformation.company_code;
                var employees = _targetExcelUploadRepo.GetEmployeeFilterList(companyCode, limit);

                var jsonResult = JsonConvert.SerializeObject(new
                {
                    success = true,
                    data = employees
                });
                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                var errorResult = JsonConvert.SerializeObject(new { success = false, message = $"Error: {ex.Message}" });
                return Content(errorResult, "application/json");
            }
        }

        // 2. GET: Employee Target Data
        [HttpGet]
        public ContentResult GetEmployeeData(string empCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(empCode))
                {
                    var errorResult = JsonConvert.SerializeObject(new { success = false, message = "Employee code is required" }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                    return Content(errorResult, "application/json");
                }

                string companyCode = _workContext.CurrentUserinformation.company_code;
                var targetData = _targetExcelUploadRepo.GetEmployeeTargetData(companyCode, empCode);

                // Get validation data for enrichment (similar to ValidateData endpoint)
                var validDistributors = _targetExcelUploadRepo.ValidateDistributors(companyCode);
                var validItems = _targetExcelUploadRepo.ValidateItems(companyCode);
                var validBranches = _targetExcelUploadRepo.ValidateBranches(companyCode);
                var validEmployees = _targetExcelUploadRepo.ValidateEmployees(companyCode);

                // Create lookup dictionaries
                var distributorDict = validDistributors.ToDictionary(d => d.DISTRIBUTOR_CODE.ToUpper(), d => d.DISTRIBUTOR_NAME, StringComparer.OrdinalIgnoreCase);
                var itemDict = validItems.ToDictionary(i => i.ITEM_CODE.ToUpper(), i => i.ITEM_NAME, StringComparer.OrdinalIgnoreCase);
                var branchDict = validBranches.ToDictionary(b => b.BRANCH_CODE.ToUpper(), b => b.BRANCH_NAME, StringComparer.OrdinalIgnoreCase);
                var employeeDict = validEmployees.ToDictionary(e => e.EMPLOYEE_CODE.ToUpper(), e => e.EMPLOYEE_NAME, StringComparer.OrdinalIgnoreCase);

                // Convert to ViewModel format similar to ValidateData response
                var rows = targetData.Select(t => new TargetExcelUploadRowViewModel
                {
                    TARGETID = t.TARGETID,
                    FROM_DATE = t.FROM_DATE,
                    TO_DATE = t.TO_DATE,
                    EMP_CODE = t.EMP_CODE,
                    EMP_NAME = employeeDict.ContainsKey(t.EMP_CODE?.ToUpper() ?? "") ? employeeDict[t.EMP_CODE.ToUpper()] : "",
                    DISTRIBUTOR_CODE = t.DISTRIBUTOR_CODE,
                    DISTRIBUTOR_NAME = distributorDict.ContainsKey(t.DISTRIBUTOR_CODE?.ToUpper() ?? "") ? distributorDict[t.DISTRIBUTOR_CODE.ToUpper()] : "",
                    ITEM_CODE = t.ITEM_CODE,
                    ITEM_NAME = itemDict.ContainsKey(t.ITEM_CODE?.ToUpper() ?? "") ? itemDict[t.ITEM_CODE.ToUpper()] : "",
                    QTY = t.QTY,
                    PRICE = t.PRICE,
                    BRANCH_CODE = t.BRANCH_CODE,
                    BRANCH_NAME = branchDict.ContainsKey(t.BRANCH_CODE?.ToUpper() ?? "") ? branchDict[t.BRANCH_CODE.ToUpper()] : "",
                    IsValid = true,
                    IsEmployeeValid = employeeDict.ContainsKey(t.EMP_CODE?.ToUpper() ?? ""),
                    IsDistributorValid = distributorDict.ContainsKey(t.DISTRIBUTOR_CODE?.ToUpper() ?? ""),
                    IsItemValid = itemDict.ContainsKey(t.ITEM_CODE?.ToUpper() ?? ""),
                    IsBranchValid = branchDict.ContainsKey(t.BRANCH_CODE?.ToUpper() ?? "")
                }).ToList();

                var jsonResult = JsonConvert.SerializeObject(new
                {
                    success = true,
                    data = rows,
                    summary = new
                    {
                        total = rows.Count,
                        valid = rows.Count,
                        invalid = 0
                    }
                }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                var errorResult = JsonConvert.SerializeObject(new { success = false, message = $"Error: {ex.Message}" }, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
                return Content(errorResult, "application/json");
            }
        }

        // 3. POST: Delete By Target ID
        [HttpPost]
        public JsonResult DeleteByTargetId(string targetId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(targetId))
                {
                    return Json(new { success = false, message = "Target ID is required" });
                }

                int rowsAffected = _targetExcelUploadRepo.DeleteByTargetId(targetId);

                if (rowsAffected > 0)
                {
                    return Json(new { success = true, message = "Record deleted successfully", rowsAffected = rowsAffected });
                }
                else
                {
                    return Json(new { success = false, message = "No record found with the specified Target ID" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Delete Error: {ex.Message}" });
            }
        }
    }

    // View model classes for Excel upload
    public class TargetExcelUploadRowViewModel
    {
        public string TARGETID { get; set; }
        public DateTime? FROM_DATE { get; set; }
        public DateTime? TO_DATE { get; set; }
        public string EMP_CODE { get; set; }
        public string EMP_NAME { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
        public string DISTRIBUTOR_NAME { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public decimal? QTY { get; set; }
        public decimal? PRICE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_NAME { get; set; }

        // Validation properties
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; }
        public bool IsEmployeeValid { get; set; }
        public bool IsDistributorValid { get; set; }
        public bool IsItemValid { get; set; }
        public bool IsBranchValid { get; set; }

        public TargetExcelUploadRowViewModel()
        {
            ValidationErrors = new List<string>();
            IsValid = true;
        }
    }
}
