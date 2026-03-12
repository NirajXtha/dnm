using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
namespace NeoERP.DocumentTemplate.Controllers
{
    public class CustomerFileUploadController : Controller
    {
        private readonly IDocumentStup _iDocumentSetup;

        public CustomerFileUploadController(IDocumentStup iDocumentSetup)
        {
            _iDocumentSetup = iDocumentSetup;
        }
        [HttpPost]
        public JsonResult CustomerOwnerFileUpload(CustomerOwnerFileUploadModel customerDetail)
        {
            if (string.IsNullOrEmpty(customerDetail.CUSTOMER_CODE) ||
                string.IsNullOrEmpty(customerDetail.OWNER_NAME) ||
                string.IsNullOrEmpty(customerDetail.FILE_COLUMN_NAME))
            {
                return Json(new { success = false, message = "Missing required fields." }, JsonRequestBehavior.AllowGet);
            }

            if (customerDetail.file == null)
            {
                return Json(new { success = false, message = "No file uploaded." }, JsonRequestBehavior.AllowGet);
            }

            string companyCode = _iDocumentSetup.GetCurrentCompanyCode();
            string originalFileName = Path.GetFileName(customerDetail.file.FileName);
            string extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(extension))
            {
                return Json(new { success = false, message = "Invalid file format." }, JsonRequestBehavior.AllowGet);
            }

            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string folderPath = Server.MapPath("~/Pictures/CustomerOwnerFiles/");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fullFilePath = Path.Combine(folderPath, uniqueFileName);

            var existing = _iDocumentSetup.GetCustomerOwnerFile(customerDetail.CUSTOMER_CODE, customerDetail.OWNER_NAME, customerDetail.FILE_COLUMN_NAME);
            if (existing!= null && !string.IsNullOrEmpty(existing.FILE_URL) && !string.IsNullOrEmpty(existing.ORIGINAL_FILENAME) && !string.IsNullOrEmpty(existing.STORED_FILENAME) && !string.IsNullOrEmpty(existing.OWNER_NAME))
            {
                string existingPhysicalPath = Path.Combine(folderPath, existing.STORED_FILENAME);

                if (System.IO.File.Exists(existingPhysicalPath))
                {
                    System.IO.File.Delete(existingPhysicalPath); 
                }
            }

            customerDetail.file.SaveAs(fullFilePath);

            string fileUrl = Url.Content("~/Pictures/CustomerOwnerFiles/" + uniqueFileName);

            var record = new CustomerOwnerFileModel
            {
                COMPANY_CODE = companyCode,
                CUSTOMER_CODE = customerDetail.CUSTOMER_CODE,
                OWNER_NAME = customerDetail.OWNER_NAME,
                FILE_COLUMN_NAME = customerDetail.FILE_COLUMN_NAME,
                ORIGINAL_FILENAME = originalFileName,
                STORED_FILENAME = uniqueFileName,
                FILE_URL = fileUrl,
            };

            return Json(new { success = true, data = record}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCustomerOwnerFiles(string customerCode, List<string> ownerNames)
        {
            string companyCode = _iDocumentSetup.GetCurrentCompanyCode();

            if (string.IsNullOrEmpty(customerCode) || ownerNames == null || !ownerNames.Any() || string.IsNullOrEmpty(companyCode))
            {
                return Json(new { success = false, message = "Missing required parameters." }, JsonRequestBehavior.AllowGet);
            }

            var allOwnerFiles = _iDocumentSetup.GetOwnerFilesByCustomerCodeAndOwners(companyCode, customerCode, ownerNames);

            if (allOwnerFiles == null || !allOwnerFiles.Any())
            {
                return Json(new { success = false, message = "No files found." }, JsonRequestBehavior.AllowGet);
            }

            var result = allOwnerFiles
                .GroupBy(f => f.OWNER_NAME)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(
                        file => file.FILE_COLUMN_NAME,
                        file => new
                        {
                            fileName = file.ORIGINAL_FILENAME,
                            url = file.FILE_URL
                        }
                    )
                );

            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}