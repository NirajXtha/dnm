using DocumentFormat.OpenXml.Drawing.Charts;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoErp.Models.WarrantyChecker;
using NeoErp.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Controllers
{
    public class WarrantyCheckerController : Controller
    {
        IWarrantyChecker _warrantyChecker { get; set; }
        private NeoErpCoreEntity _objectEntity;
        public WarrantyCheckerController(IWarrantyChecker _warrantyChecker, NeoErpCoreEntity objectEntity)
        {
            this._warrantyChecker = _warrantyChecker;
            this._objectEntity = objectEntity;
        }

        // GET: WarrantyChecker
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateDefect()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetWarrantyInfo(string serialNo)
        {
            var result = this._warrantyChecker.GetWarrantyInfo(serialNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveWarrantyServiceMsg(DefectMessage param)
        {
            var result = this._warrantyChecker.SaveWarrantyMsgService(param);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDefact(string m = null)
        {
            var result = this._warrantyChecker.GetDefact();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadMappingExcel(HttpPostedFileBase FileUpload)
        {
            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("/JS/");

                    FileUpload.SaveAs(System.IO.Path.Combine(targetpath, filename));
                    string pathToExcelFile = System.IO.Path.Combine(targetpath, filename);
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    var dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Sheet1";

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][0].ToString()))
                        {
                            try
                            {
                                var datetimevalue = string.Empty;
                                var days ="365";
                                if(!string.IsNullOrEmpty(ds.Tables[0].Rows[i][2].ToString()))
                                {
                                    datetimevalue = Convert.ToDateTime(ds.Tables[0].Rows[i][2]).ToString("dd/MM/yyyy");


                                }
                                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][1].ToString()))
                                    days = ds.Tables[0].Rows[i][1].ToString();
                                if (string.IsNullOrEmpty(datetimevalue))
                                    continue;
                                var query = @"update ip_mac_serial_track
                                      set warranty_days=" + days + @",
                                          activation_date = to_date('" + datetimevalue + @"','DD/MM/YYYY'),
                                          activate_flag = 'Y'
                                       where serial_no = trim('" + ds.Tables[0].Rows[i][0].ToString() + "')";

                                var rowaffect = _objectEntity.ExecuteSqlCommand(query);
                            }
                            catch(Exception ex)
                            {
                                data.Add("<ul>");
                                data.Add("<li>'"+ex.Message+"'</li>");
                                data.Add("</ul>");
                                data.ToArray();
                                return RedirectToAction("CreateDefect", new { m = 'e' });
                            }
                        }
                    }


                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    //return Json("success", JsonRequestBehavior.AllowGet);                    
                    return RedirectToAction("CreateDefect", new { m = 's' });

                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return RedirectToAction("CreateDefect", new { m = 'e' });
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return RedirectToAction("CreateDefect");
            }
        }
    }
}