using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.DistributorServices
{
    public interface IDistributorService
    {
        User DistributorLogin(User model);
        string SavePurchaseOrder(DistPurchaseOrder model,User userInfo);
        List<SalesRegisterProductModel> GetDistributorItems(User userInfo, string distributerCode);
        PurchaseOrderReportModel GetMaxOrderNoFromDistributor();
        List<CollectionModel> GetCollections(User UserInfo);
        List<DivisionModel> GetCollectionDetail(string BillNo);
        string SaveCollection(CollectionModel model, User UserInfo);
        List<AccountStatementModel> GetAccountStatement(filterOption model, User UserInfo);
        List<DivisionModel> GetDivisions(User userInfo);
        List<ClosingReportModel> GetClosingStock(User UserInfo, filterOption model);
        List<ClosingReportModel> GetClosingStockNew(User UserInfo, filterOption model);
        List<SalesVsTargetModel> GetAllSalesVsTarget(User userInfo, filterOption model);
        List<DivisionWiseCreditLimitModel> GetDivisionWiseCreditLimitList(User userInfo,filterOption filter);
        List<VoucherDetailModel> GetLedgerDetailBySubCode(string SubAccCode);
        List<SalesVsTargetModel>  GetAllSalesTargetVsAchievement(User userInfo, filterOption model);
        List<SalesVsCustomerTargetModel> GetAllSalesTargetVsCustomerAchievement(User userInfo, filterOption model);
        List<SchemeModel> GetSchemeData(User userInfo, filterOption model);
    }
}
