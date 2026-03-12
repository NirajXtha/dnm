using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
  public  interface IPurchaseService
    {
        List<PurchaseRegisterDetail> GetPurchaseRegister(string fromdate, string toDate,User userInfo);
        List<PurchaseRegisterDetail> GetPurchaseRegister(ReportFiltersModel filters,User userInfo);
        List<PurchaseVatRegisters> GetPurchaseVatRegisters(string fromdate, string toDate,User userInfo);
        List<PurchaseVatRegisters> GetPurchaseVatRegisters(ReportFiltersModel filters,User userInfo);
        List<ChargesTitle> GetChargesTitle(User userinfo);
        List<ItemsLandingCostViewModel> GetPurchaseItemsSummary(string fromDate, string toDate);
        List<ItemsLandingCostViewModel> GetPurchaseItemsSummary(ReportFiltersModel filters,User userinfo);
        List<PurchaseCharges> GetPurchaseCharges(string formDate, string toDate);
        List<PurchaseCharges> GetPurchaseCharges(ReportFiltersModel filters, User userInfo);
        List<InvoiceLandedCostSummary> GetPurchaseInvoiceSummary(string fromDate, string toDate);
        List<InvoiceLandedCostSummary> GetPurchaseInvoiceSummary(ReportFiltersModel filters,User userinfo);
        List<PurchaseCharges> GetPurchaseInvoiceCharges(string formDate, string toDate);
        List<PurchaseCharges> GetPurchaseInvoiceCharges(ReportFiltersModel filters, User userInfo);
        List<VoucherModel> GetPurchaseRegisterVouchers();
        List<VoucherModel> GetPurchaseRegisterVouchers(User userinfo);
        List<VoucherSetupModel> GetAllVoucherNodes();
        List<VoucherSetupModel> GetAllVoucherNodes(User userinfo);
        List<VoucherSetupModel> GetVoucherListByFormCode(string level, string masterSupplierCode);
        List<VoucherSetupModel> GetVoucherListByFormCode(string level, string masterSupplierCode,User userinfo);
        List<LocationModel> GetStorageLocations();
        List<LocationModel> GetStorageLocations(User userinfo);
        List<LocationSetupModel> GetAllStorageLocationNodes();
        List<LocationSetupModel> GetLocationListByLocationId(string level, string masterLocationCode);
        List<CompanyModel> GetCompaniesModelList();
        List<AccountsModel> GetAccountsList();
        List<PurchaseRegisterMoreDetail> GetPurchaseRegisterPivort(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo);
        List<ItemCode> GetItemList();
        string InsertItem(ItemSetupModal modal);
        List<ItemCode> GetItemCode(string ITEM_GROUP_CODE);
        string DeleteItemCode(string ITEM_GROUP_CODE);

    }
}
