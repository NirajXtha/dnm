using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services.Services;
using Excel = Microsoft.Office.Interop.Excel;
using LinqToExcel;
using LinqToExcel.Query;

namespace NeoERP.LOC.Controllers
{
    public class ProformaInvoiceController : Controller
    {
        private IPerfomaInvoice _perfomaInvoice { get; set; }
        public ProformaInvoiceController(IPerfomaInvoice perfomaInvoice)
        {
            this._perfomaInvoice = perfomaInvoice;
        }
        // GET: ProformaInvoice
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult ProformaInvoice()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult FileUpload(IEnumerable<HttpPostedFileBase> inputFiles, FormCollection collection)
        {
            string pinvoicecode = Convert.ToString(collection["pinvoicecode"]);
            string lctrackno = Convert.ToString(collection["lctrackno"]);

            string fName = "";
            string dbstrMappath = "";
            string finalpath = "";
            int count = 0;
            LcImageModels lcmodels = new LcImageModels();
            if (inputFiles != null)
            {
                foreach (HttpPostedFileBase fileName in inputFiles)
                {
                    HttpPostedFileBase file = fileName;
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        string picture = System.IO.Path.GetFileName(file.FileName);
                        string underscore = "_";
                        string pic = string.Format("{0}{1}", lctrackno, underscore) + picture.PadLeft(0, '0');
                        string LCImages = "LCImages";
                        string PinvoiceImages = "Pinvoice Images";
                        string strMappath = "~/Pictures/" + LCImages + "/" + PinvoiceImages + "/" + lctrackno + "/";
                        string path = System.IO.Path.Combine(
                                               Server.MapPath(strMappath), pic);
                        string ext = Path.GetExtension(path);

                        dbstrMappath = "\\Pictures\\" + LCImages + "\\" + PinvoiceImages + "\\" + lctrackno + "\\" + pic;
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        file.SaveAs(path);

                    }
                    lcmodels.LocCode = Convert.ToInt32(lctrackno);
                    count++;
                    if (count > 1)
                    {
                        finalpath = finalpath + ":" + dbstrMappath;
                    }
                    else
                    {
                        finalpath = dbstrMappath;
                    }

                }
                lcmodels.Path = finalpath;
                _perfomaInvoice.UpdateImage(lcmodels, pinvoicecode);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult Import(FileUpload form_data)
        {
            Excel.Application application = new Excel.Application();
            try
            {
                if (form_data.file == null || form_data.file.ContentLength == 0)
                {

                    return Json("empty", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (form_data.file.FileName.EndsWith("xls") || form_data.file.FileName.EndsWith("xlsx"))
                    {


                        string paths = Server.MapPath("~/Pictures/" + "LCImages" + "/" + "ImportedExcelFiles" + "/" + form_data.file.FileName);
                        string strMappath = "~/Pictures/" + "LCImages" + "/" + "ImportedExcelFiles" + "/";
                        string path = System.IO.Path.Combine(
                                              Server.MapPath(strMappath), form_data.file.FileName);
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        form_data.file.SaveAs(path);
                        string sheetName = "Sheet1";
                        string filepath = paths;
                        var excel = new ExcelQueryFactory(filepath);
                        var worksheetNames = excel.GetWorksheetNames();
                        if (worksheetNames.ElementAt(0) != sheetName)
                        {
                            return Json("sheetmismatch", JsonRequestBehavior.AllowGet);
                        }
                        //var columnNames = excel.GetColumnNames(sheetName);
                        //var columns = columnNames.ToArray();
                        //for (int i = 0; i < columns.Count(); i++)
                        //{
                        //    if(columns[0] != "ITEM_CODE")
                        //    {

                        //    }
                        //}
                        //if (columnNames.ElementAt(0) != "ITEM_CODE" && columnNames.ElementAt(1) != "ITEM_EDESC" && columnNames.ElementAt(2) != "MU_CODE" && columnNames.ElementAt(3) != "QUANTITY" && columnNames.ElementAt(4) != "AMOUNT" && columnNames.ElementAt(5) != "HS_CODE" && columnNames.ElementAt(6) != "COUNTRY_OF_ORIGIN")
                        //{
                        //    return Json("formaterror", JsonRequestBehavior.AllowGet); 
                        //}
                        var exceldata = from a in excel.Worksheet<ImportItems>(sheetName) where !String.IsNullOrEmpty(a.ITEM_CODE) select a;
                        var listItems = exceldata.ToList();
                        var distinctItems = listItems.GroupBy(p => p.ITEM_CODE).Select(g => g.First()).ToList();
                        return Json(distinctItems, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        return Json("fileincorrect", JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                application.Quit();
            }

        }

    }
}

//Commented Lines


// Read data from excel file
//Excel.Workbook workbook = application.Workbooks.Open(path);
//Excel.Worksheet worksheet = workbook.ActiveSheet;
//Excel.Range range = worksheet.UsedRange;
//List<ImportItems> listItems = new List<ImportItems>();
////int count = 0;
//for (int row = 1; row <= range.Rows.Count; row++)
//{
//    if (count == 0)
//    {
//        if (((Excel.Range)range.Cells[row, 1]).Text != "ITEM_CODE" && ((Excel.Range)range.Cells[row, 1]).Text != "ITEM_EDESC" && ((Excel.Range)range.Cells[row, 2]).Text != "MU_CODE" && ((Excel.Range)range.Cells[row, 3]).Text != "QUANTITY" && ((Excel.Range)range.Cells[row, 4]).Text != "AMOUNT" && ((Excel.Range)range.Cells[row, 5]).Text != "HS_CODE" && ((Excel.Range)range.Cells[row, 6]).Text != "COUNTRY_OF_ORIGIN")
//        {
//            return Json("formaterror", JsonRequestBehavior.AllowGet);
//        }
//    }
//    else
//    {
//        int quantity, amount = 0;
//        ImportItems p = new ImportItems();
//        p.ITEM_CODE = ((Excel.Range)range.Cells[row, 1]).Text;
//        if (p.ITEM_CODE != "")
//        {
//            p.ITEM_EDESC = ((Excel.Range)range.Cells[row, 2]).Text;
//            p.MU_CODE = ((Excel.Range)range.Cells[row, 3]).Text;
//            int.TryParse(((Excel.Range)range.Cells[row, 4]).Text, out quantity);
//            p.QUANTITY = quantity;
//            int.TryParse(((Excel.Range)range.Cells[row, 5]).Text, out amount);
//            p.AMOUNT = amount;
//            p.HS_CODE = ((Excel.Range)range.Cells[row, 6]).Text;
//            p.COUNTRY_OF_ORIGIN = ((Excel.Range)range.Cells[row, 7]).Text;
//            listItems.Add(p);
//        }
//    }
//    count++;
//}