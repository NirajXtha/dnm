using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Services
{
    public interface IMobileService
    {
        IEnumerable<MobileViewVoucherModel> GetVoucherDetails(int userId,string moduleCode, string append, int sessionRowId);

        MobileViewVoucheDetailModel GetVoucherLedgers(string voucherCode, string tableName, string companyCode, string branchCode);

        IEnumerable<ModuleCount> GetModuelCount();

        LoginResponseModel Login(string UserName, string Password, out string message,out int statusCode);

        List<LoginResponseModel> LoginM(string UserName, string Password, out string message, out int statusCode);

        bool UserExist(int userId, string companyCode);

        int AuthoriseVoucher(string formCode, string voucherCode, string userName, string companyCode, string branchCode, out string message);
        
        int ApproveVoucher(string formCode, string voucherCode, string userName, string companyCode,string branchCode, out string message);

        int PostVoucher(string formCode, string voucherCode, string userName, string companyCode, string branchCode, out string message);
        
        IEnumerable<string> GetModulePermission(int userId, string companyCode);

        IEnumerable<MobileDataVoucherModel> GetVoucherWithFlag(int userId,string companyCode, string branchCode, string moduleCode = "", string append = "top", int sessionRowId = 0);
        List<AccountTreeModelMobile> AccountListAllGroupNodesAutoComplete(string Company_code, string branchCode);
        List<SubLedgerList> SubLedgerList(string Company_code, string branchCode);
        List<DealerPartyTypeModel> DealerSubLedgerList(string Company_code, string branchCode);
        List<AccountModel> AccountSubLedgerList(string Company_code, string subCode);
        List<SubLedgerList> DealerCustomerList(string party_type_code);
        List<VoucherDetailModelMobile> GetLedgerDetailBySubCode(string accountCode, string SubAccCode, string formDate, string toDate, string company_code, string branchCode);
        List<VoucherDetailModelMobile> GetDealerLedgerDetail(string accountCode, string SubAccCode, string formDate, string toDate, string company_code, string branchCode, string partyTypeCode);
        List<VoucherDetailModelMobile> GetVoucherDetailsByAccountCode(ReportFiltersModel reportFilters, string formDate, string toDate, string AccountCode, string BranchCode, string DataGeneric = "DG");

        List<CRMModel> GetCRM();

        List<PayOrderModel> GetPayorders();

        int UpdatePayOrder(PayOrderModel model);

        SalesReportModel GetSalesReport(string company_code);

        List<TopicSalesModel> GetTopicWiseSales(string companyCode, string topic);

        List<TopicSalesModel> GetTopicWiseSalesMTD(string companyCode, string topic);

        List<TopicSalesModel> GetTopicWiseSalesDaily(string companyCode, string topic);

        List<TopicSalesModel> GetTopicWiseSalesQuaterly(string companyCode, string topic, string Quater);

        List<TopicSalesModel> GetTopicWiseQuaterlySales(string companyCode, string topic, string Quater);

        List<PapModel> GetPap(string company_code);

        List<PopModel> GetPop(string company_code);

        List<PrpModel> GetPrp(string company_code);
        List<AccountSummaryModel> GetSubLedgerWithFilter(string company_code, string filtername, string formdate, string todate);
        List<TopicSalesModel> GetAccountHeadBalance(string company_code, string branch_code);
        List<AccountSummaryModel> GetAccountHeadBalanceTopic(string company_code, string filtername, string topic);
        List<string> GetLedgerFiler();
        List<SalesTargetGraphMobile> GetMonthlySalesVsTarget(string company_code, string userCode);
        List<AccountModel> AgingDelarGroup(string Company_code);
        List<AccountModel> AgingPartyType(string Company_code, string DealerGroup = "All");
        List<ItemModel> GetItems(string company);
        List<ItemModel> GetDivisions(string company);
        List<TopEmployeeWithAmountQtyModel> GetTopEmployeesByTheirSalesAmtQty(string amtOrQtyWise, string company_code);
        List<TopDealerWithAmountQtyModel> GetTopDealerByTheirSalesAmtQty(string amtOrQtyWise, string company_code);
        List<TopEmployeeWithAmountQtyModel> GetTopEmployeesByTheirSalesAmtQtyForMonth(string company_code, string branch_code);
        List<TopDealerWithAmountQtyModel> GetTopDealerByTheirSalesAmtQtyForMonth(string company_code, string branch_code);
    }
}
