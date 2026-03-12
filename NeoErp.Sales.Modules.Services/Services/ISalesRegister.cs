using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.Audit;
using NeoErp.Sales.Modules.Services.Models.BillwisePurchaseSummary;
using NeoErp.Sales.Modules.Services.Models.NetPurchaseModels;
using NeoErp.Sales.Modules.Services.Models.NetSalesReport;
using NeoErp.Sales.Modules.Services.Models.NetSalesReports;
using NeoErp.Sales.Modules.Services.Models.PurchaseSummaryReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public interface ISalesRegister
    {
        List<Charges> GetSalesItemCharges(ReportFiltersModel filters, string salesNo);
        List<CustomerSetupModel> GetGroupCustomerListByCustomerCode(string level, string masterCustomerCode);
        List<CustomerSetupModel> CustomerGroupListAllNodes();
        List<SalesRegisterCustomerModel> SaleRegisterGroupCustomers();
        List<CategoryModel> GetSalesRegisterItemCategory();
        List<CategoryModel> GetSalesRegisterItemCategory(User userinfo);
        List<PartyTypeModel> GetSalesRegisterPartyTypes();
        List<PartyTypeModel> GetSalesRegisterPartyTypes(User userinfo);
        List<AreaTypeModel> GetAreaTypes(User userinfo);
        List<VoucherModel> SalesRegisterVouchers();
        List<VoucherSetupModel> GetAllVoucherNodes();
        List<VoucherSetupModel> GetVoucherListByFormCode(string level, string masterSupplierCode);
        List<SalesRegisterSupplierModel> SalesRegisterSuppliers();
        List<SalesRegisterSupplierModel> SalesRegisterSuppliers(User userinfo);
        List<SalesRegisterSupplierModel> SalesRegisterDealer(User userinfo);
        List<SalesRegisterSupplierModel> SalesRegisterGroupSuppliers();
        List<SalesRegisterSupplierModel> SalesRegisterGroupSuppliers(User userinfo);
        List<SupplierSetupModel> SupplierAllNodes();
        List<SupplierSetupModel> SupplierAllNodes(User userinfo);
        List<SupplierSetupModel> DealerAllNodes(User userinfo);
        List<SupplierSetupModel> SupplierAllNodesGroup();
        List<SupplierSetupModel> SupplierAllNodesGroup(User userinfo);
        List<SupplierSetupModel> GetSupplierListBySupplierCode(string level, string masterSupplierCode);
        List<SupplierSetupModel> GetSupplierListBySupplierCode(string level, string masterSupplierCode, User userinfo);
        List<SupplierSetupModel> GetDealerListBySupplierCode(string level, string masterSupplierCode, User userinfo);
        List<SupplierSetupModel> GetSupplierListBySupplierCodeGroup(string masterSupplierCode);
        List<SupplierSetupModel> GetSupplierListBySupplierCodeGroup(string masterSupplierCode, User userinfo);
        List<SalesRegisterProductModel> SalesRegisterProducts();
        List<SalesRegisterProductModel> SalesRegisterProductsIndividual();
        //List<SalesRegisterProductModel> GetDistributorItems(User userInfo);
        List<SalesRegisterProductModel> SalesRegisterProducts(User userinfo);
        List<SalesRegisterProductModel> SalesRegisterProductsByCategory(User userinfo, string category);
        List<ProductSetupModel> GetProductsListByProductCode(string level, string masterProductCode);
        List<ProductSetupModel> GetProductsListByProductCode(string level, string masterProductCode, User userinfo);
        List<ProductSetupModel> GetProductsListWithChild(string level, string masterProductCode, User userinfo);

        List<ProductSetupModel> ProductListAllNodes();
        List<ProductSetupModel> ProductListAllNodes(User userinfo);
        List<CustomerSetupModel> GetCustomerListByCustomerCode(string level, string masterCustomerCode);
        List<CustomerSetupModel> GetCustomerListByCustomerCode(string level, string masterCustomerCode, User userinfo);
        List<CustomerSetupModel> CustomerListAllNodes();
        List<CustomerSetupModel> CustomerListAllNodes(User userinfo);
        List<ConsolidateTree> CompanyListAllNodes(User userinfo, string userNo);
        List<ConsolidateTree> branchListByCompanyCode(User userinfo, string company_code);
        //For mobile
        List<SalesRegisterCustomerModel> SaleRegisterCustomers(string companyCode, string branchCode);
        List<SalesRegisterCustomerModel> SaleRegisterSuppliers(string companyCode, string branchCode);
        //For web
        List<SalesRegisterCustomerModel> SaleRegisterCustomers();
        List<SalesRegisterModel> SaleRegisters();
        List<SalesRegisterModel> SaleRegistersDateWiseFilter(string formDate, string toDate);
        List<SalesRegisterModel> GetSaleRegisters(ReportFiltersModel filters);
        List<Charges> GetSalesCharges();
        List<Charges> GetSalesCharges(ReportFiltersModel filters);
        List<ChargesTitle> GetChargesTitle();
        List<VatRegisterModel> GetVatRegister();
        List<VatRegisterModel> GetVatRegisterDateWiseFilter(string formDate, string toDate);
        List<VatRegisterModel> GetVatRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<SalesRegisterMasterModel> SaleRegistersMasterDateWiseFilter(string formDate, string toDate);
        List<ChargesMap> GetChargesMapList();
        List<SalesChildModel> GetSalesItemBySalesId(filterOption filters, string salesId, string ItemCompanyCode);
        List<SalesVatWiseSummaryModel> GetSalesVatWiseSummaryDateWise(string formDate, string toDate);
        List<SalesVatWiseSummaryModel> GetSalesVatWiseSummary(ReportFiltersModel filters);
        List<SalesRegisterMasterModel> SaleRegistersMasterDynamicDateWiseFilter(string formDate, string toDate);
        List<SalesRegisterMasterModel> SaleRegistersMasterDynamic(ReportFiltersModel filters);
        List<MaterializedViewMasterModel> MaterializedViewReport(ReportFiltersModel filters);
        List<VatRegistrationIRDMasterModel> VatRegisterIRDReport(ReportFiltersModel filters);
        List<Charges> GetSumChargesDateWise(string formDate, string toDate);
        List<Charges> GetSumCharges(ReportFiltersModel filters);
        List<SalesRegistersDetail> GetSalesRegisterDateWise(string formDate, string toDate);
        List<SalesRegistersDetail> GetSalesRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<SalesRegistersDetail> GetSalesRegisterDateWisePaging(string formDate, string toDate, int pageSize, int pageNumber);
        List<BranchModel> getSalesRegisterBranch();
        List<BranchModel> getSalesRegisterBranch(User userinfo);
        int TotalSalesRegister(string formDate, string toDate);
        List<ChartSalesModel> GetCategorySales();
        List<ChartSalesModel> GetProductSalesByCategory(ReportFiltersModel filters, User userinfo, string categoryCode);
        List<ChartSalesModel> GetProductSalesByMonth(ReportFiltersModel reportFilters, User userinfo, string dateFormat, string month);
        List<ChartSalesModel> GetProductSalesByCategory(ReportFiltersModel filters, User userinfo, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<ChartSalesModel> GetCategorySales(ReportFiltersModel reportFilters, User userInfo);
        List<ChartSalesModel> GetCategorySales(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode);
        IList<CustomerWisePriceListModel> GetCustomerWisePriceList(ReportFiltersModel model, User userInfo);
        IList<ProductWisePriceListModel> GetProductWisePriceList(ReportFiltersModel model, User userInfo);
        IList<CustomerWiseProfileAnalysisModel> GetCustomerWiseProfitAnalysis(ReportFiltersModel model, User userInfo);
        List<DynamicMenu> GetDynamicMenu(int userId, int level, string modular_code);
        List<DynamicMenu> GetChlidMenu(string menuNo, int userid, string module_code);
        List<ChargesTitle> GetChargesItemTitle();
        List<ChartSalesModel> GetAreasSales();
        IList<SalesRegisterDetailModel> GetSalesRegisterModelPrivot(ReportFiltersModel model, User userInfo);
        List<DailySalesTreeList> GetSalesRegisterDailyReport(ReportFiltersModel model, User userInfo);
        IEnumerable<GoodsReceiptNotesDetailModel> GetGoodsReceiptNotesData(ReportFiltersModel model, User userInfo, bool liveData);
        List<ChartSalesModel> GetAreaSales(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<ChartSalesModel> GetProductSalesByArea(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<ChartSalesModel> GetNoOfbills(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<CategoryWiseSalesModel> GetCategoryStockLevel(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<ChartSalesModel> GetStockLevelByCategory(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<ChartSalesModel> GetProductSalesByAreaEmployee(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode);
        List<DynamicColumnForNCR> GetDynamiColumns();
        List<MaterializeModel> GetMaterializeReprot(ReportFiltersModel filters, User userInfo, bool sync = false);
        List<PurchaseReturnRegistersDetail> GetPurchaseReturnRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<SalesRegistersDetail> GetAgentWiseSalesRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<PurchaseReturnRegistersDetail> GetPurchaseRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<PurchasePendingDetailModel> GetPurchasePendingReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<PurchasePendingDetailModel> GetPurchaseOrderReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<PurchaseVatRegistrationDetailModel> GetPurchaseVatRegisterReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
       
        #region NewReports
        List<SalesExciseRegisterModel> SalesExciseRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<AuditTrailModel> AuditTrailReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<ProductWiseSalesSummaryModel> GetProductWiseSalesSummary(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<DateWiseSalesDetailsModel> GetDateWiseSalesDetails(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<DateWiseSalesReturnDetailsModel> GetDateWiseSalesReturnDetails(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<BillwiseSalesSummaryModel> GetBillwiseSalesSummary(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<BillwiseSalesReturnSummaryModel> GetBillwiseSalesReturnSummary(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<BranchwiseEmployeeNetSalesModel> BranchwiseEmployeeNetSales(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<VatSalesRegisterNewModel> GetVatSalesRegisterNew(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<VatSalesRegisterNewModel> GetVatSalesReturnRegisterNew(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<DateWiseSalesDetailsModel> GetDateWiseSalesDetailsJewellery(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        //dynamic GetMonthwiseCustProdNetSales(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);

        List<VatPurchaseRegisterNewModel> GetVatPurchaseRegisterNew(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<VatPurchaseRegisterNewModel> GetVatPurchaseReturnRegisterNew(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        //List<GroupwiseStockSummary> GetGroupwiseStockSummary(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        // dynamic GetGroupwiseStockSummary(dynamic data);
        dynamic GetGroupwiseStockSummary(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetAllItems(dynamic data, NeoErp.Core.Domain.User userinfo);
        List<DatewiseNetSalesModels> GetDatewiseNetSalesReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<ProductwiseNetSalesModel> GetProductwiseNetSalesReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        dynamic GetBranchwiseNetSalesReport(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetMonthwiseProductNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetMonthwiseCustomerSalesCollection(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetBranchwiseCustomerProductNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetMonthwiseCustomerProductNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetMonthwiseSalesTypeCustomerNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetMonthwiseEmployeeCustomerNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetMonthwiseEmployeeProductNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetMonthwiseRegionProductSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetMonthwiseRegionEmployeeNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetBrandwiseEmployeeCustomerNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        List<BillwisePurchaseSummaryViewModel> GetBillwisePurchaseSummary(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<BillwisePurchaseSummaryViewModel> GetBillwisePurchaseReturnSummary(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<ProductwisePurchaseSummaryViewModel> GetProductwisePurchaseSummary(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<BillwisePurchaseSummaryViewModel> GetDatewisePurchaseDetails(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<BillwisePurchaseSummaryViewModel> GetPurchaseLandedCost(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        dynamic GetCustomerwiseOrderTracking(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetGroupProductwiseOrderPending(dynamic data, NeoErp.Core.Domain.User userinfo);
        List<SalesAboveOneLakhModel> GetSalesAboveOneLakh(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        dynamic GetProductGroupwiseNetSales(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetCustomerwiseNetSalesRep(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetProductCustomerwiseNetSalesReport(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetCustomerBrandwiseNetSalesReport(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetEmployeeCustomerwiseNetSalesReport(dynamic data, NeoErp.Core.Domain.User userinfo);
        dynamic GetEmployeeProductwiseNetSalesReport(dynamic data, NeoErp.Core.Domain.User userinfo);
        List<ProductwiseNetSalesModel> GetDealerNetSalesReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<ProductwiseNetPurchaseViewModel> GetProductwiseNetPurchaseReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<ProductwiseNetPurchaseViewModel> GetDatewiseNetPurchaseReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<SalesAboveOneLakhModel> GetPurchaseAboveOneLakh(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        dynamic GetSupplierwiseNetPurchaseReport(dynamic data, NeoErp.Core.Domain.User userinfo);
        List<DatewisePurchaseDetailViewModel> GetDatewisePurchaseDetailReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<DatewisePurchaseDetailViewModel> GetBillwisePurchaseSummaryReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<DatewisePurchaseDetailViewModel> GetProductwisePurchaseSummaryReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<DatewisePurchaseDetailViewModel> GetBillwisePurchaseReturnSummaryReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<DatewisePurchaseDetailViewModel> GetPurchaseLandedCostSummaryReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<FormCodeData> GetFormCodeByCompany(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<SalesAboveOneLakhModel> PurchaseAboveOneLakhReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<AuditTransactionModel> GetAuditTrasactionReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        dynamic GetGroupstockSummaryReport(dynamic data,User userinfo);
        List<AllItem> GetAllItemByGroup(ReportFiltersModel filters,User userinfo);
        List<AllLocation> GetAllLocation(ReportFiltersModel filters, User userinfo);







        #endregion
    }
}
