using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Models;
using NeoERP.DocumentTemplate.Service.Repository;

namespace NeoERP.DocumentTemplate.Views.Shared.PrintTemplate.Sales
{
    public partial class CrystalReportSalesOrderDetail : System.Web.UI.Page
    {
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly NeoErpCoreEntity _coreEntity;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string formCode = Request.QueryString["formCode"];
                string orderNo = Request.QueryString["orderno"];
                if (!string.IsNullOrEmpty(formCode) && !string.IsNullOrEmpty(orderNo))
                {
                    LoadReport(formCode, orderNo);
                }
            }
        }
        private DataTable ToDataTable(List<COMMON_COLUMN> data)
        {
            DataTable table = new DataTable();

            // Add columns based on the properties of COMMON_COLUMN
            var properties = typeof(COMMON_COLUMN).GetProperties();
            foreach (var prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Add rows to the DataTable
            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        public void LoadReport(string formCode, string orderNo)
        {
            //var userid = _workContext.CurrentUserinformation.User_id;
            //var company_code = _workContext.CurrentUserinformation.company_code;
            //var branch_code = _workContext.CurrentUserinformation.branch_code;
            //var response = new List<COMMON_COLUMN>();
            //// Define the path to your report
            string reportPath = HttpContext.Current.Server.MapPath("~/Areas/NeoERP.DocumentTemplate/Views/Shared/PrintTemplate/Sales/HyattRegencySalesoperaInvoicePrintTemplate.rpt");

            //IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity coreentity;

            FormTemplateRepo _FormTemplateRepo = new FormTemplateRepo(_dbContext, _workContext, _cacheManager, _coreEntity);

            var SalesOrderDetailFormDetailByFormCodeAndOrderNo = _FormTemplateRepo.GetSalesOrderFormDetail("", "");
            //response = SalesOrderDetailFormDetailByFormCodeAndOrderNo;

            //// Check if the file exists
            //if (!System.IO.File.Exists(reportPath))
            //{
            //    return NotFound();
            //}

            // Load the report
            ReportDocument rprt = new ReportDocument();
            rprt.Load(reportPath);
            // Convert the response list to a DataTable
            DataTable dataTable = ToDataTable(SalesOrderDetailFormDetailByFormCodeAndOrderNo);

            //CrystalReportViewer crystalReportViewer = new CrystalReportViewer();

            // Set the DataSource of the report to the DataTable
            rprt.SetDataSource(dataTable);


            //Stream stream = rprt.ExportToStream(ExportFormatType.PortableDocFormat);
            //MemoryStream ms = new MemoryStream();
            //stream.CopyTo(ms);
            //return File(ms.ToArray(), "application/pdf");

            //CrystalReportViewer


            try
            {
                // Set parameters if any
                // Export the report to Crystal Report format
                var stream = rprt.ExportToStream(ExportFormatType.CrystalReport);

                //// Return the report as a FileResult
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(stream)
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-crystalreport");


                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                {
                    FileName = "report.rpt"
                    //FileName = "report.pdf"
                };
                //return ResponseMessage(result);



            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
            }
            finally
            {
                // Clean up the report document
                rprt.Close();
                rprt.Dispose();
            }
        }
    }
}