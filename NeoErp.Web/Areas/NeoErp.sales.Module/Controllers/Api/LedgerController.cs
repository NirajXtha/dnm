using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Sales.Modules.Services.Models.TrialBalance;
using NeoErp.Sales.Modules.Services.Models.Voucher;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class LedgerController : ApiController
    {
        public IVoucherService _voucherRegister { get; set; }
        public ITrialBalanceService _trialBalanceRegister { get; set; }
        private ICacheManager _cacheManager;
        private IWorkContext _workContext;
        public LedgerController(IVoucherService voucherRegister, ITrialBalanceService trialBalanceRegister, ICacheManager cacheManager, IWorkContext workContext)
        {
            this._voucherRegister = voucherRegister;
            this._trialBalanceRegister = trialBalanceRegister;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }
        [HttpPost]
        public gridVoucherModel GetVouchersDetails(filterOption model, string formDate, string toDate, string AccoundCode, string branchCode = null,string DataGeneric="DG")
        {
            // model.take = 10;
            // model.pageSize = 10;
            var modelVoucher = new gridVoucherModel();

            var voucherList = _voucherRegister.GetVoucherDetails(model.ReportFilters, formDate, toDate, AccoundCode, _workContext.CurrentUserinformation, branchCode,DataGeneric).OrderBy(q => q.voucher_date).ToList();
            modelVoucher.total = voucherList.Count;
            voucherList = voucherList.ToList();
            modelVoucher.data = voucherList;
            return modelVoucher;
        }
        [HttpPost]
        public gridVoucherModel GetVouchersDetailsMobile(filterOption model, string formDate, string toDate, string AccoundCode, string branchCode = null, string DataGeneric = "DG")
        {
            // model.take = 10;
            // model.pageSize = 10;
            var modelVoucher = new gridVoucherModel();
            User user = new User();
            user.company_code = "01";
            user.branch_code = "01.01";
            var voucherList = _voucherRegister.GetVoucherDetails(model.ReportFilters, formDate, toDate, AccoundCode, user, branchCode, DataGeneric).OrderBy(q => q.voucher_date).ToList();
            modelVoucher.total = voucherList.Count;
            voucherList = voucherList.ToList();
            modelVoucher.data = voucherList;
            return modelVoucher;
        }
        [HttpPost]
        public List<VoucherDetailReportModal> GetVouchersDetails(string uid, string voucherNo)
        {
            var voucherDetail = _voucherRegister.GetVoucherDetailFromCode(voucherNo);
            return voucherDetail;
        }

        [HttpPost]
        public List<VoucherDetailModel> GetVouchersSummaryDetails(filterOption model, string formDate, string toDate, string AccoundCode)
        {
            var voucherList = _voucherRegister.GetVoucherDetails(model.ReportFilters,formDate, toDate, AccoundCode, _workContext.CurrentUserinformation);
            return voucherList;
        }
        public List<Accounts> GetAccounts()
        {
            return _voucherRegister.GetAccounts();
        }
        public List<LedgerDailySummaryModel> GetLedgersSummary(string formDate, string toDate, string id)
        {
            var trialBalances = _cacheManager.Get("NeoErptestss", () => _trialBalanceRegister.GetLedgersummaryByAccCode(formDate, toDate, id));
            //  var trialBalances = _trialBalanceRegister.GetLedgersummaryByAccCode(formDate, toDate, id);
            if (trialBalances == null || trialBalances.Count <= 0)
                trialBalances.Add(new LedgerDailySummaryModel() { ClosingCr = 0, ClosingDr = 0, CrAmount = 0, DrAmount = 0, Miti = formDate, OpeningCr = 0, OpeningDr = 0, VoucherDate = DateTime.Now });
            return trialBalances;
        }
        public List<LedgerDailySummaryModel> GetSubLedgersSummary(string formDate, string toDate, string accountCode, string subCode)
        {
            var trialBalances = _trialBalanceRegister.GetSubLedgerDetailBySubCode(formDate, toDate, accountCode, subCode);
            if (trialBalances == null || trialBalances.Count <= 0)
                trialBalances.Add(new LedgerDailySummaryModel() { ClosingCr = 0, ClosingDr = 0, CrAmount = 0, DrAmount = 0, Miti = formDate, OpeningCr = 0, OpeningDr = 0, VoucherDate = DateTime.Now });
            return trialBalances;
        }
        public List<LedgerAutosearch> GetLedgerSelect(string masterCode, string Parentcode)
        {
            return _voucherRegister.GetLedgerAutosearch(masterCode, Parentcode);
        }
        [HttpGet]
        public List<AccountTreeModel> GetAllParentAccountNodes()
        {
            var allAccountList = _voucherRegister.AccountListAllParentNodes(_workContext.CurrentUserinformation).ToList();
            foreach (var cust in allAccountList)
            {
                cust.HasChildren = cust.Child_rec > 0 ? true : false;
            }
            return allAccountList;
        }
        [HttpGet]
        public List<AccountTreeModel> GetAllAccountByAccId(string prodId)
        {
            var allAccountList = _voucherRegister.GetAccountrListByAccountCode("1", prodId, _workContext.CurrentUserinformation).ToList();
            foreach (var cust in allAccountList)
            {
                cust.HasChildren = cust.Child_rec > 0 ? true : false;
            }
            return allAccountList;

        }

        [HttpGet]
        public List<AccountTreeModel> GetAccountListHavingChildrenTransaction()
        {
            return _voucherRegister.GetAccountListHavingChildrenTransaction().ToList();
        }
        [HttpGet]
        public List<LedgerModel> GetAllLedgerByAccId(string accountCode)
        {

            return _voucherRegister.GetLedgerListByAccId(accountCode, _workContext.CurrentUserinformation).ToList();
        }
        [HttpGet]
        public List<AccountTreeModel> GetAccountGroupAutoComplete()
        {
            return _voucherRegister.AccountListAllGroupNodesAutoComplete().ToList();
        }
        [HttpPost]
        public gridSubVoucherModel GetSubAccountVouchersDetails(filterOption model, string formDate, string toDate, string AccoundCode, string SubCode)
        {
            //  model.take = 10;
            // model.pageSize = 10;
            var modelVoucher = new gridSubVoucherModel();
            User userinfo = this._workContext.CurrentUserinformation;
            //var voucherList = _voucherRegister.GetLedgerDetailBySubCode(AccoundCode,SubCode,formDate,toDate);
            var voucherList = _voucherRegister.GetLedgerDetailBySubCode(AccoundCode, SubCode, formDate, toDate, userinfo);
            //if (voucherList.Any(x => x.PARTICULARS.Trim().ToLower() == "Opening".Trim().ToLower()))
            //{
            //    var openingbalance = voucherList.Where(x => x.PARTICULARS.Trim().ToLower() == "Opening".Trim().ToLower());
            //    if(openingbalance.FirstOrDefault().dr_amount>0)
            //    {
            //        voucherList.Where(x => x.PARTICULARS.Trim().ToLower() == "Opening".Trim().ToLower()).FirstOrDefault().BalanceHeader = "DR";
            //    }
            //    else
            //    {
            //        voucherList.Where(x => x.PARTICULARS.Trim().ToLower() == "Opening".Trim().ToLower()).FirstOrDefault().BalanceHeader = "CR";
            //    }
            //}
            //var closingLedger = new VoucherDetailModel();
            //closingLedger
            modelVoucher.total = voucherList.Count;
            voucherList = voucherList.ToList();
            modelVoucher.data = voucherList;
            return modelVoucher;
        }

        //[HttpPost]
        //public gridSubVoucherModel GetSubAccountVouchersDetails1(string formDate, string toDate, string AccoundCode, string SubCode)
        //{
        //    return this.GetSubAccountVouchersDetails(null, formDate, toDate, AccoundCode, SubCode);
        //}
        [HttpGet]
        public List<VoucherModel> GetSubLedgerAutoComplete()
        {
            return _voucherRegister.GetSubLedgerListAutoComplete().ToList();
        }
        [HttpGet]
        public List<string> GetLedgerTransaction(string accountCode)
        {
            if (string.IsNullOrEmpty(accountCode))
            {
                return new List<string>();
            }
            return _voucherRegister.GetLedgerListByHierarchy(accountCode).Select(q => q.LedgerCode).ToList();
        }


    }
}
