using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace NeoErp.sales.Module.Controllers.Api
{
    public class ScheduleController : ApiController
    {
        private IReceiptScheduleReportService _receiptScheduleService;
        private IWorkContext _workContext;

        public ScheduleController(IReceiptScheduleReportService receiptScheduleService, IWorkContext workContext)
        {
            this._receiptScheduleService = receiptScheduleService;
            this._workContext = workContext;
        }

        [HttpPost]
        public ReceiptScheduleViewModel GetCustomerWiseReceiptSchedule(filterOption model)
        {
            // return this._receiptScheduleService.GetCustomerWiseReceiptSchedule(model, _workContext.CurrentUserinformation).ToList();
            ReceiptScheduleViewModel reportData = new ReceiptScheduleViewModel();
            reportData.ReceiptScheduleModel = this._receiptScheduleService.GetCustomerWiseReceiptSchedule(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.ReceiptScheduleModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.ReceiptScheduleModel,model.aggregate);
            return reportData;
        }



        [HttpPost]
        public PaymentScheduleViewModel GetSupplierWisePaymentSchedule(filterOption model)
        {
            // return this._receiptScheduleService.GetCustomerWiseReceiptSchedule(model, _workContext.CurrentUserinformation).ToList();
            PaymentScheduleViewModel reportData = new PaymentScheduleViewModel();
            reportData.PaymentScheduleModel = this._receiptScheduleService.GetSupplierWisePaymentSchedule(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.PaymentScheduleModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.PaymentScheduleModel,model.aggregate);
            return reportData;
        }
    }
}
