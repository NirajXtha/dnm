using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Sales.Modules.Services.Models.Voucher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
   public interface IVoucherService
    {
        List<VoucherDetailModel> GetVoucherDetails(ReportFiltersModel reportFilters,string formDate, string toDate, string AccountCode, NeoErp.Core.Domain.User userinfo, string BranchCode = null,string DataGeneric="DG");
        List<Accounts> GetAccounts();
        List<LedgerBreadCrumb> GetParentAccountByAccountCode(string AccountCode);
        List<AccountTreeModel> GetAccountListHavingChildrenTransaction();
        List<LedgerAutosearch> GetLedgerAutosearch(string MasterAccountCode, string preAccountCode);

        List<AccountTreeModel> AccountListAllParentNodes(NeoErp.Core.Domain.User userinfo);
        List<AccountTreeModel> GetAccountrListByAccountCode(string level, string masterCustomerCode, NeoErp.Core.Domain.User userinfo);

        List<LedgerModel> GetLedgerListByAccId(string accountCode, NeoErp.Core.Domain.User userinfo);
        List<AccountTreeModel> AccountListAllGroupNodesAutoComplete();
        List<VoucherModel> GetVoucherList(string accountCode, NeoErp.Core.Domain.User userinfo);
        List<VoucherDetailModel> GetLedgerDetailBySubCode(string accountCode, string SubAccCode, string formDate, string toDate);
        List<VoucherDetailModel> GetLedgerDetailBySubCode(string accountCode, string SubAccCode, string formDate, string toDate,User userinfo);
        List<VoucherModel> GetSubLedgerListAutoComplete();
        IEnumerable<AgeingGroupData> GetAccountData();
        //  List<LedgerDailySummaryModel> GetSubLedgerDetailBySubCode( string formDate, string toDate,string accountCode, string SubAccCode);
        List<LedgerModel> GetLedgerListByHierarchy(string accountCode);
        List<VoucherDetailReportModal> GetVoucherDetailFromCode(string voucherNo);

        List<CUSTOMTEMPLATEFORPL> GeneratePLLedger(string formDate, string ToDate, string companyCode, string BranchCode, User userinfo);
    }
}
