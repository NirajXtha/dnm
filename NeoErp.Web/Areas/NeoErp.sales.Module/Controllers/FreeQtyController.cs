using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.Sales.Modules.Services.Services;
using NeoErp.Sales.Modules.Services.Models;

namespace NeoErp.sales.Module.Controllers
{
    

    public class FreeQtyController : Controller
    {
        private IFreeQty _freeQty;

        public FreeQtyController(IFreeQty freeQty)
        {
            this._freeQty = freeQty;        
        }

        

        // GET: FreeQty
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult all_customer(string searchTerm = "")
        {
            var js = _freeQty.AllCustomer(searchTerm);
            return Json(js, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_form_setup()
        {
            var js = _freeQty.GetFormSetup();
            return Json(js, JsonRequestBehavior.AllowGet);
        }

        public ActionResult get_item_tree()
        {
            var js = _freeQty.GetItemTreeView();
            return Json(js, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult save_free_quantity(List<FreeQuantityItem> data, string customerCode, List<string> formCodes)
        {
            try
            {
                if (formCodes == null || formCodes.Count == 0)
                {
                    return Json(new { success = false, message = "No form codes provided" });
                }

                // Get current user from session or context
                string createdBy = User.Identity.Name ?? "SYSTEM";
                
                int successCount = 0;
                int failCount = 0;
                string lastError = "";

                // Loop through each form code and save data
                foreach (var formCode in formCodes)
                {
                    try
                    {
                        var result = _freeQty.SaveFreeQuantity(data, customerCode, formCode, createdBy);
                        
                        // Check if result has success property
                        var resultDict = result as IDictionary<string, object>;
                        if (resultDict != null && resultDict.ContainsKey("success"))
                        {
                            bool success = (bool)resultDict["success"];
                            if (success)
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                                lastError = resultDict.ContainsKey("message") ? resultDict["message"].ToString() : "Unknown error";
                            }
                        }
                        else
                        {
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        lastError = ex.Message;
                    }
                }

                if (successCount == formCodes.Count)
                {
                    return Json(new { success = true, message = $"Data saved successfully to {successCount} form(s)" });
                }
                else if (successCount > 0)
                {
                    return Json(new { success = true, message = $"Data saved to {successCount} form(s). Failed: {failCount}. Last error: {lastError}" });
                }
                else
                {
                    return Json(new { success = false, message = $"Failed to save data. Error: {lastError}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult load_free_quantity_data(string customerCode, string formCode)
        {
            var js = _freeQty.LoadFreeQuantityData(customerCode, formCode);
            return Json(js, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadExcelTemplate()
        {
            try
            {
                // TODO: Implement Excel template generation
                // For now, return a placeholder response
                // You'll need to install EPPlus or ClosedXML NuGet package for Excel generation
                
                // Example structure:
                // Columns: Customer Code, Form Code, Item Code, Quantity, Free Qty, Unit, Free Unit
                
                return Json(new { success = false, message = "Excel template download will be implemented with EPPlus/ClosedXML" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { success = false, message = "No file uploaded" });
                }

                // Validate file extension
                var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    return Json(new { success = false, message = "Invalid file format. Please upload .xlsx or .xls file" });
                }

                // Get current user from session or context
                string createdBy = User.Identity.Name ?? "SYSTEM";

                // Process the Excel file
                var result = _freeQty.ProcessExcelUpload(file.InputStream, createdBy);
                
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}