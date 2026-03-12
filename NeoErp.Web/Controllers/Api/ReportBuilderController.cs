using NeoErp.Core.Helpers;
using NeoErp.Core.Services;
using NeoErp.Models.ReportBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Expressions;
using System.Linq.Dynamic;

namespace NeoErp.Controllers.Api
{
    public class ReportBuilderController : ApiController
    {
        private IReportBuilderService _reportBuilderService;
        public ReportBuilderController(IReportBuilderService reportBuilderService)
        {
            this._reportBuilderService = reportBuilderService;
        }

        public bool SaveReport(ReportBuilderModel model)
        {
            return true;
        }

        [HttpPost]
        public  HttpResponseMessage GetData(ReportBuilderModel model)
        {

            model.Query = model.Query.ReplaceHtmlTag();
            var total = 0;
            var data = new DataTable();
            DataTable tdata = this._reportBuilderService.GetData(model.ReportName, model.Query);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
             
                return this.Request.CreateResponse(
                    HttpStatusCode.OK,
                    new { Message = "Data Successfully retrievied", Data = tdata.FilterDataTable(model.page, model.pageSize,out total,model.sort, model.filter.Logic, model.filter.Filters) });
            }
            
            
            return this.Request.CreateResponse(
            HttpStatusCode.OK,
            new { Message = "Data Successfully retrievied", Data = tdata.FilterDataTable(model.page, model.pageSize, out total, model.sort), Total= total });
        }
    }
}
