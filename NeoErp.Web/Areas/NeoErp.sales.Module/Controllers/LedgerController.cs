using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.sales.Module.Models;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Sales.Modules.Services.Models.Voucher;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.sales.Module.Controllers
{
    public class LedgerController : Controller
    {
        public IVoucherService _voucherRegister { get; set; }
        private ICacheManager _cacheManager { get; set; }
        private IWorkContext _workContext;
        //private MemoryCacheManager _memoryCacheManager;
        public LedgerController(IVoucherService voucherRegister, ICacheManager cachemanager, IWorkContext workContext)
        {
            this._voucherRegister = voucherRegister;
            this._cacheManager = cachemanager;
            this._workContext = workContext;
            // this._memoryCacheManager = memoryCacheManager;

        }
        // GET: Ledger
        #region LedgerIndexReport
        public ActionResult LedgerDetails()
        {
            var setting = new LedgerSetting();
            setting.showBreadCrumb = true;
            setting.showDateTime = true;
            return PartialView(setting);
        }
        public ActionResult PopUpLedgerDetails(string formDate, string toDate, string AccountCode, int totalTab = 1, string BranchCode = null, string dataGeneric = "DG")
        {
            LedgerSearch model = new LedgerSearch();
            model.formDate = formDate;
            model.toDate = toDate;
            model.accountCode = AccountCode;
            model.BranchCode = BranchCode;
            model.DataGeneric = dataGeneric;
            model.GridName = "grid" + totalTab;
            ViewBag.totaltab = totalTab;
            return PartialView(model);
        }

        public ActionResult BreadCrumbs(string AccountCode)
        {
            // var test = new MemoryCacheManager();
            //var abc= test.Get<List<LedgerBreadCrumb>>("some_key_1");
            //test.Set("some_key_1", _voucherRegister.GetParentAccountByAccountCode(AccountCode), 60);
            if (!_cacheManager.IsSet("some_key_1"))
            {
                _cacheManager.Set("some_key_1", _voucherRegister.GetParentAccountByAccountCode(AccountCode), 60);

            }
            var accountBreadCrumbs = _cacheManager.Get<List<LedgerBreadCrumb>>("some_key_1");
            //   var accountBreadCrumbs = _voucherRegister.GetParentAccountByAccountCode(AccountCode);
            var desctictBeadCrumbs = new List<LedgerBreadCrumb>();
            desctictBeadCrumbs.AddRange(accountBreadCrumbs.Where(x => x.AccountTypeFlag == "N"));
            desctictBeadCrumbs.AddRange(accountBreadCrumbs.Where(x => x.AccountCode == AccountCode));

            return PartialView(desctictBeadCrumbs);
        }

        public ActionResult LedgerSummaryReport(string formDate, string toDate, string AccountCode, int totalTab = 1)
        {
            LedgerSearch model = new LedgerSearch();
            model.formDate = formDate;
            model.toDate = toDate;
            model.accountCode = AccountCode;

            model.GridName = "ledgerSummaryGrid" + totalTab;
            return PartialView(model);
        }

        public ActionResult LedgerIndex()
        {
            return PartialView();
        }
        public ActionResult ShowLedgerTransaction(string accountCode, string accountName = "ledger")
        {
            if (accountCode == null || accountCode == "0")
            {
                var ledgersEmpty = new List<LedgerModel>();
                return PartialView(ledgersEmpty);
            }
            ViewBag.accountname = accountName;
            var userinfo = _workContext.CurrentUserinformation;
            var ledgers = _voucherRegister.GetLedgerListByAccId(accountCode, userinfo);
            return PartialView(ledgers);
        }
        public ActionResult ShowSubAccount(string accountCode, string accountName = "ledger")
        {
            if (accountCode == null || accountCode == "0")
            {
                var SubAccountEmpty = new List<VoucherModel>();
                return PartialView(SubAccountEmpty);
            }
            ViewBag.accountname = accountName;
            ViewBag.accountcode = accountCode;
            var ledgers = _voucherRegister.GetVoucherList(accountCode, _workContext.CurrentUserinformation);
            return PartialView(ledgers);
        }
        public ActionResult PopUpSubLedgerDetails(string formDate, string toDate, string AccountCode, string subCode, int totalTab = 1)
        {
            LedgerSearch model = new LedgerSearch();
            model.formDate = formDate;
            if (string.IsNullOrEmpty(model.formDate))
                model.formDate = "2016/07/01";


            model.toDate = toDate;
            model.accountCode = AccountCode;
            model.SubCode = subCode;

            model.GridName = "grid" + totalTab;
            ViewBag.totaltab = totalTab;
            return PartialView(model);
        }
        public ActionResult SubLedgerDailySummaryReport(string formDate, string toDate, string AccountCode, string subCode, int totalTab = 1)
        {
            //if (abc == null)
            //    abc = "";
            LedgerSearch model = new LedgerSearch();
            model.formDate = formDate;
            model.toDate = toDate;
            model.accountCode = AccountCode;
            model.SubCode = subCode;
            model.GridName = "SubledgerSummaryGrid" + totalTab;
            return PartialView(model);
        }

        #endregion

        #region PLREPORT
        public ActionResult SimplePLAccount (string formDate, string toDate, string CompanyCode, string BranchCode, int totalTab = 1)
        {
            //if (abc == null)
            //    abc = "";
            LedgerSearch model = new LedgerSearch();
            model.formDate = formDate;
            model.toDate = toDate;
            model.BranchCode = BranchCode;
            model.CompanyCode = CompanyCode;
            model.GridName = "PLSimpleCode" + totalTab;
            return PartialView(model);
        }

        public ActionResult SimplePLAccountSigmentWise(string formDate, string toDate, string CompanyCode, string BranchCode,string SegmentName, int totalTab = 1)
        {
            //if (abc == null)
            //    abc = "";
            LedgerSearch model = new LedgerSearch();
            model.formDate = formDate;
            model.toDate = toDate;
            model.BranchCode = BranchCode;
            model.CompanyCode = CompanyCode;
            model.GridName = "PLSimpleCodeTable" + totalTab;
            User userinfo = this._workContext.CurrentUserinformation;
            var data=_voucherRegister.GeneratePLLedger(formDate, toDate, CompanyCode,  BranchCode, userinfo);
            return PartialView(data);
        }
        #endregion

    }
}