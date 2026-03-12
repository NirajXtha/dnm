using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class CompanyModel
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }

    public class LocationModel
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
    }

    public class CategoryModel
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
    }
    public class AccountsModel
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
    }
    public class PartyTypeModel
    {
        public string PartyTypeCode { get; set; }
        public string PartyTypeName { get; set; }
    }

    public class AreaTypeModel
    {
        public int AREA_CODE { get; set; }
        public string AREA_EDESC { get; set; }
    }

    public class BranchModel
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }

    public class VoucherModel
    {
        public string VoucherName { get; set; }
        public string VoucherCode { get; set; }
    }

    public class SalesRegisterCustomerModel
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
    }

    public class CustomerModel
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
    }


    public class SalesRegisterProductModel
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string PO_DISPLAY_DIST_ITEM { get; set; }
    }
    public class ProductModel
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string GroupFlag { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
    }

    public class SalesRegisterSupplierModel
    {
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
    }

    public class SupplierModel
    {
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string GroupFlag { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
    }


    public class SalesRegisterModel
    {
        public SalesRegisterModel()
        {
            charges = new List<Charges>();
        }
        public DateTime? SALES_DATE { get; set; }
        public string MITI { get; set; }
        public string SALES_NO { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public decimal CALC_QUANTITY { get; set; }
        public decimal? CALC_UNIT_PRICE { get; set; }
        public decimal? CALC_TOTAL_PRICE { get; set; }

        public decimal? DISCOUNT { get; set; }
        //   public 
        public decimal? VAT { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public List<Charges> charges { get; set; }
    }

    public class SalesRegisterMasterModel
    {
        public SalesRegisterMasterModel()
        {
            charges = new List<Charges>();
        }

        public String Miti { get; set; }
        public string SalesDate { get; set; }
        public string InvoiceNumber { get; set; }
        public String CustomerName { get; set; }
        public Decimal? GrossAmount { get; set; } = 0;
        //public Decimal? Discount { get; set; } = 0;
        // public Decimal? Vat { get; set; } = 0;
        public Decimal? NetAmount { get; set; } = 0;
        //public Decimal? ExciseDuty { get; set; } = 0;
        public Decimal? InvoiceAmount { get; set; } = 0;
        public List<Charges> charges { get; set; }
        public string COMPANY_CODE { get; set; }
    }

    public class SalesChildModel
    {
        public SalesChildModel()
        {
            charges = new List<Charges>();
        }
        public string ProductName { get; set; }
        public int Quanity { get; set; } = 0;
        public decimal? Rate { get; set; } = 0;
        public decimal? GrossAmount { get; set; } = 0;
        public decimal? Discount { get; set; } = 0;
        public decimal? NetAmount { get; set; } = 0;
        public string INVOICENO { get; set; }
        public string UNIT { get; set; }
        public List<Charges> charges { get; set; }
        public string ITEM_CODE { get; set; }


    }
    public class SalesRegisterDetialReport
    {
        public List<SalesRegisterMasterModel> gridSalesRegReport { get; set; }
        public int total { get; set; }
        public SalesRegisterDetialReport()
        {
            gridSalesRegReport = new List<SalesRegisterMasterModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }

        //public Aggration Aggrationresult { get; set; }
    }
    public class MaterializedViewDetailReport
    {
        //MaterializedViewMasterModel
        public List<MaterializedViewMasterModel> gridSalesRegReport { get; set; }
        public int total { get; set; }
        public MaterializedViewDetailReport()
        {
            gridSalesRegReport = new List<MaterializedViewMasterModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
    public class MaterializedViewMasterModel
    {
        public string FISCAL_YEAR { get; set; }
        public string BILL_NO { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_PAN { get; set; }
        public string BILL_DATE { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? DISCOUNT { get; set; }
        public decimal? TAXABLE_AMOUNT { get; set; }
        public decimal? TAX_AMOUNT { get; set; }
        public decimal? TOTAL_AMOUNT { get; set; }
        public string SYNC_WITH_IRD { get; set; }
        public string IS_BILL_PRINTED { get; set; }
        public string IS_BILL_ACTIVE { get; set; }
        public string PRINTED_TIME { get; set; }
        public string ENTERED_BY { get; set; }
        public string PRINTED_BY { get; set; }
        public string IS_REAL_TIME { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string FORM_CODE { get; set; }

    }
    public class VatRegistrationIRDDetailReport
    {
        //MaterializedViewMasterModel
        public List<VatRegistrationIRDMasterModel> gridSalesRegReport { get; set; }
        public int total { get; set; }
        public VatRegistrationIRDDetailReport()
        {
            gridSalesRegReport = new List<VatRegistrationIRDMasterModel>();
            AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
    public class VatRegistrationIRDMasterModel
    {
        public string MITI { get; set; }
        public string INVOICE_NO { get; set; }
        public string PARTY_NAME { get; set; }
        public string VAT_NO { get; set; }
        public decimal? GROSS_SALES { get; set; }
        public decimal? TAXABLE_SALES { get; set; }
        public decimal? VAT { get; set; }
        public decimal? TOTAL_SALES { get; set; }
        public string CREDIT_DAYS { get; set; }
        public string FORM_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public decimal? SALES_DISCOUNT { get; set; }
        public string MANUAL_NO { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public decimal? ZERO_RATE_EXPORT { get; set; }

    }
    public class Charges
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public decimal? CHARGE_AMOUNT { get; set; }
        public string REFERENCE_NO { get; set; }
        public string APPLY_ON { get; set; }
        public string CustomerId { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
    }

    public class ChargesTitle
    {
        public string ChargesHeaderTitle { get; set; }
        public string ChargesHeaderNo { get; set; }
    }


    public class gridViewSalesReg
    {
        public List<SalesRegisterModel> gridSalesRegReport { get; set; }
        public int total { get; set; }
        public gridViewSalesReg()
        {
            gridSalesRegReport = new List<SalesRegisterModel>();
            Aggrationresult = new Aggration();
        }
        public Aggration Aggrationresult { get; set; }
    }
    public class Aggration
    {
        public Aggration()
        {
            CALC_QUANTITY = new CALC_QUANTITY();
        }
        public CALC_QUANTITY CALC_QUANTITY { get; set; }
    }
    public class CALC_QUANTITY
    {
        public decimal? sum { get; set; }
    }

    public class ChargesMap
    {
        public string chargeFieldName { get; set; }
        public string chargeFieldSystemName { get; set; }
        public string chargeType { get; set; }
    }

    public class CustomerSetupModel
    {
        public int LEVEL { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public int? Childrens { get; set; }
    }

    public class ProductSetupModel
    {
        public int LEVEL { get; set; }
        public string ItemName { get; set; }
        public string GroupFlag { get; set; }
        public string ItemCode { get; set; }
        public string MasterItemCode { get; set; }
        public string PreItemCode { get; set; }
        public int? Childrens { get; set; }
    }

    public class SupplierSetupModel
    {
        public int LEVEL { get; set; }
        public string SupplierName { get; set; }
        public string GroupFlag { get; set; }
        public string SupplierCode { get; set; }
        public string MasterSupplierCode { get; set; }
        public string PreSupplierCode { get; set; }
        public int? Childrens { get; set; }
    }

    public class VoucherSetupModel
    {
        public int LEVEL { get; set; }
        public string VoucherName { get; set; }
        public string GroupFlag { get; set; }
        public string VoucherCode { get; set; }
        public string MasterFormCode { get; set; }
        public string PreFormCode { get; set; }
    }

    public class LocationSetupModel
    {
        public int LEVEL { get; set; }
        public string LocationName { get; set; }
        public string GroupFlag { get; set; }
        public string LocationCode { get; set; }
        public string MasterLocationCode { get; set; }
        public string PreLocationCode { get; set; }
    }

    public class ChartOfAccountSetupModel
    {
        public string ACC_EDESC { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_TYPE_FLAG { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string PRE_ACC_CODE { get; set; }
        public int LEVEL { get; set; } //BRANCH_CODE
        public bool hasAccount { get; set; }

    }

    public class ChartSalesModel
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public Decimal? Total { get; set; }
        public decimal? GrossAmount { get; set; }

        public decimal? Quantity { get; set; }
    }
    public class CategoryWiseSalesModel
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public Decimal? Total { get; set; }
        public decimal? GrossAmount { get; set; }

        public decimal? Quantity { get; set; }
    }

    public class DynamicMenu
    {
        public string MENU_EDESC { get; set; }

        public string MENU_NO { get; set; }

        public string VIRTUAL_PATH { get; set; }

        public string FULL_PATH { get; set; }

        public string GROUP_SKU_FLAG { get; set; }

        public string ICON_PATH { get; set; }
        public string MODULE_CODE { get; set; }

        public List<DynamicMenu> Items { get; set; }

        public string MODULE_ABBR { get; set; }
        public string COLOR { get; set; }
        public string DESCRIPTION { get; set; }

        public string DASHBOARD_FLAG { get; set; }
    }

}
