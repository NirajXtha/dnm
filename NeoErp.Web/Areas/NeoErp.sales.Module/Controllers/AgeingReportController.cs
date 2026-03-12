using NeoErp.Core.Caching;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class AgeingReportController : Controller
    {
        private IAgeingReportService _ageingReportService;
        public IVoucherService _voucherRegister { get; set; }
        private ICacheManager _cacheManager;

        public AgeingReportController(IAgeingReportService ageingReportService, ICacheManager cacheManager,IVoucherService voucherRegister)
        {
            this._ageingReportService = ageingReportService;
            this._cacheManager = cacheManager;
            this._voucherRegister = voucherRegister;
        }
        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult MonthWiseDebtorsIndex()
        {
            return PartialView();
        }
        public ActionResult MonthWiseDebtorsDivisioinIndex()
        {
            return PartialView();
        }
        public ActionResult DebtorsAgingChart()
        {
            return PartialView();
        }
        public ActionResult CrebtorsAgingChart()
        {
            return PartialView();
        }
        public ActionResult DynamicMonthlyColumnView(AgeingFilterModel model)
        {
            var dynamicColumns = this._ageingReportService.GenerateColumns(model.FrequencyInDay.Value, model.FixedInDay.Value, model.AsOnDate);
            this._cacheManager.Remove("ageingMonthly-report");
            return PartialView(dynamicColumns);
        }
        public ActionResult DynamicMonthlyDivisonColumnView(AgeingFilterModel model)
        {
            var dynamicColumns = this._ageingReportService.GenerateColumns(model.FrequencyInDay.Value, model.FixedInDay.Value, model.AsOnDate);
            this._cacheManager.Remove("ageingMonthly-report");
            return PartialView(dynamicColumns);
        }


        public ActionResult DynamicColumnView(AgeingFilterModel model)
        {
            var dynamicColumns = this._ageingReportService.GenerateColumns(model.FrequencyInDay.Value, model.FixedInDay.Value, model.AsOnDate);
            this._cacheManager.Remove("ageing-report");
            return PartialView(dynamicColumns);
        }

        public ActionResult AgeingVoucherView(string uid, string voucherNo)
        {
            NeoErp.Sales.Modules.Services.Models.Voucher.VoucherDetailReportModal modal = new Sales.Modules.Services.Models.Voucher.VoucherDetailReportModal();
            modal.uid = uid;
            modal.VoucherNumber = voucherNo;
            //var model = this._voucherRegister.GetVoucherDetailFromCode(voucherNo);
            return PartialView("_AgeingVoucherViewpartial",modal);
        }
        public ActionResult AgeingReportDealer()
        {
            return PartialView();
        }
    }
}