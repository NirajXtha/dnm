using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.sales.Module.Models;
using NeoErp.Sales.Modules.Services.Models;
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
    public class SubsidiaryLedgerController : Controller
    {
        private IWorkContext _workContext;
        private ISubsidiaryLedger _subsidiaryLedger;
        private ICacheManager _cacheManager { get; set; }
        public SubsidiaryLedgerController(IWorkContext workContext, ISubsidiaryLedger subsidiaryLedger,ICacheManager cacheManager)
        {
            _workContext = workContext;
            _subsidiaryLedger = subsidiaryLedger;
            _cacheManager = cacheManager;
        }
        // GET: SubsidiaryLedger
        #region SubsidiaryLedgerReport

        public ActionResult SubsidiaryLedgerIndex()
        {
            return PartialView();
        }


        public ActionResult ShowSubLedgersTransaction(string Code,string listType, string accountName = "ledger")
        {
            //if (Convert.ToInt32(ChildRec) > 0)
            //{
            //    var ledgersEmpty = new List<TreeModels>();
            //    ViewBag.accountname = accountName;
            //    return PartialView(ledgersEmpty);
            //}
            ViewBag.accountname = accountName;
            var userinfo = _workContext.CurrentUserinformation;
            var ledgers = _subsidiaryLedger.GetListByCode(Code, userinfo, listType);
            return PartialView(ledgers);
        }

        [HttpPost]
        public ActionResult PopUpSubsidiaryLedgerDetails(string formDate, string toDate, string AccountCode, string listType, string linkSubCode, int totalTab = 1, string BranchCode = null, string groupSkuFlag="",string MasterCode = "",string actionname = "")
        {
            LedgerSearch model = new LedgerSearch();
            model.formDate = formDate;
            model.toDate = toDate;
            model.accountCode = AccountCode;
            model.BranchCode = BranchCode;
            model.groupSkuFlag = groupSkuFlag;
            model.GridName = "grid" + totalTab;
            model.linkSubCode = linkSubCode;
            model.listType = listType;
            model.MasterCode = MasterCode;
            model.actionName = actionname;
            ViewBag.totaltab = totalTab;
            return PartialView(model);
        }

        public ActionResult SubsidiaryBreadCrumbs(string AccountCode,string listType)
        {
            var userinfo = _workContext.CurrentUserinformation;
            if (!_cacheManager.IsSet("some_key_1"))
            {
                _cacheManager.Set("some_key_1", _subsidiaryLedger.GetParentListByCode(AccountCode, listType, userinfo), 60);

            }
            var accountBreadCrumbs = _cacheManager.Get<List<MultiSelectModels>>("some_key_1");
            //   var accountBreadCrumbs = _voucherRegister.GetParentAccountByAccountCode(AccountCode);
            var desctictBeadCrumbs = new List<MultiSelectModels>();
            desctictBeadCrumbs.AddRange(accountBreadCrumbs.Where(x => x.Code == AccountCode));

            return PartialView(desctictBeadCrumbs);
        }
        #endregion
    }
}