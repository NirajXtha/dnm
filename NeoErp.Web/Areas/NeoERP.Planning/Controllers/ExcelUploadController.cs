using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace NeoERP.Planning.Controllers
{
    public class ExcelUploadController : Controller
    {
        // GET: ExcelUpload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {

                    return Json(new { TYPE = "error", MESSAGE = "Empty File" }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    DataSet dsexcelRecords = new DataSet();
                    IExcelDataReader reader = null;
                    HttpPostedFile Inputfile = null;
                    Stream FileStream = null;
                    FileStream = file.InputStream;


                    if (file.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                    }
                    else if (file.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                    }
                    else
                    {
                        //message = "The file format is not supported.";
                    }
                    dsexcelRecords = reader.AsDataSet();
                    reader.Close();

                    if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                    {
                        DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                        for (int i = 0; i < dtStudentRecords.Rows.Count; i++)
                        {
                           
                            var RollNo = dtStudentRecords.Rows[i][0];
                            var EnrollmentNo = Convert.ToString(dtStudentRecords.Rows[i][1]);
                            var Name = Convert.ToString(dtStudentRecords.Rows[i][2]);
                            var Branch = Convert.ToString(dtStudentRecords.Rows[i][3]);
                            var  University = Convert.ToString(dtStudentRecords.Rows[i][4]);
                            

                        }
                    }

                        return null;
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}