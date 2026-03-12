using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.MongoDBRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeoErp.Sales.Modules.Services.Models
{


    public class CustomerWisePriceListModel
    {
        public string CustomerName { get; set; }
        public string Customer_Code { get; set; }
        public string ITEM_Code { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal Sales_Rate { get; set; }


    }

    public class CustomerWisePriceListViewModel
    {
        public List<CustomerWisePriceListModel> CustomerWisePriceListModel { get; set; }
        public decimal total { get; set; }
        public CustomerWisePriceListViewModel()
        {
            CustomerWisePriceListModel = new List<CustomerWisePriceListModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }



    public class ProductWisePriceListModel
    {
        public string Item_Code { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal Sales_Rate { get; set; }

    }

    public class ProductWisePriceListViewModel
    {
        public List<ProductWisePriceListModel> ProductWisePriceListModel { get; set; }
        public decimal total { get; set; }
        public ProductWisePriceListViewModel()
        {
            ProductWisePriceListModel = new List<ProductWisePriceListModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }

    public class SalesRegisterDetailModel
    {
        public SalesRegisterDetailModel()
        {
            this.Dealers = new List<SalesRegisterDetailModel>();
        }
        public string BRANCH_EDESC { get; set; }
        public string DIVISION_NAME { get; set; }
        public string EMONTH { get; set; }
        public string BSMONTH { get; set; }
        //public string EDay { get; set; }
        //public string BSDay { get; set; }
        public string MITI { get; set; }
        public DateTime? INV_DATE { get; set; }
        public string INVOICE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }
        public string BRAND_NAME { get; set; }
        public decimal? SERIAL_NO { get; set; }
        public List<SalesRegisterDetailModel> Dealers { get; set; }
        public int? QUANTITY { get; set; }
        public int? UNIT_PRICE { get; set; }
        public string ReportsTo { get; set; }
        public int? TOTAL_PRICE { get; set; }
        public double? DISCOUNT_AMT { get; set; }
        public double? DISCOUNT_AMT1 { get; set; }
        public double? DISCOUNTED_AMOUNT { get; set; }
        public double? Special_Discount_Scheme { get; set; }
        public double? EXCISE_DUTY { get; set; }
        public decimal? AREA_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string ITEM_SUBGROUP_EDESC { get; set; }
        public string ITEM_GROUP_EDESC { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string MR_NAME { get; set; }
        public string branch_code { get; set; }
        public string form_code { get; set; }
        public string company_code { get; set; }
        public string sales_no { get; set; }
        public decimal? VAT_AMOUNT { get; set; }
        public string MANUAL_NO { get; set; }
        public decimal? TOTAL_BILL_VALUE { get; set; }
        public string MODULE { get; set; }
        public string CUSTOMER_GROUP_EDESC { get; set; }
        public string DEALER_NAME { get; set; }
        public string DESTINATION { get; set; }
        public string VEHICLE_NAME { get; set; }
        public string VEHICLE_OWNER_NAME { get; set; }
        public string VEHICLE_OWNLER_NO { get; set; }
        public string DRIVER_NAME { get; set; }
        public string DRIVER_LICENSE_NO { get; set; }
        public string DRIVER_MOBILE_NO { get; set; }
        public string TRANSPORTER_NAME { get; set; }
        public decimal? FREGHT_AMOUNT { get; set; }
        public decimal? WB_WEIGHT { get; set; }
        public string WB_NO { get; set; }
        public DateTime? WB_DATE { get; set; }
        public string AREA_EDESC { get; set; }
        public decimal TargetBonus { get; set; } = 0;
        public decimal CollectionBonus { get; set; } = 0;
        public decimal MonopolyBonus { get; set; } = 0;
        public decimal BgBonus { get; set; } = 0;
        public decimal CMTPScheme { get; set; } = 0;
        public decimal VPBScheme { get; set; } = 0;
        public decimal? GROSS_REALISATION_AMOUNT { get; set; } = 0;
        public decimal? GROSS_REALISATION_PER_QUANTITY { get; set; } = 0;

        public string PreCode { get; set; }
        public string MasterCode { get; set; }


    }

    public class DynamicColumns
    {
        public string accountname { get; set; }
        public decimal dramount { get; set; }
        public string customer_code { get; set; }
        public string party_type_code { get; set; }
    }

    public class DailySalesTreeList
    {
        public DailySalesTreeList()
        {
            this.columns = new List<DynamicColumnForNCR>();
        }
        public string Description { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? UNIT_PRICE { get; set; }
        public double? DISCOUNT_AMT { get; set; }
        public double? DISCOUNTED_AMOUNT { get; set; }
        public decimal? Special_Discount_Scheme { get; set; }
        public int? AREA_CODE { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string parentId { get; set; }
        public string Id { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal? EXCISE_DUTY { get; set; }
        public decimal? VAT_AMOUNT { get; set; }
        public decimal? TOTAL_BILL_VALUE { get; set; }
        public decimal? GROSS_REALISATION_AMOUNT { get; set; }
        public decimal? GROSS_REALISATION_PER_QUANTITY { get; set; }
        public List<DynamicColumnForNCR> columns { get; set; }
        public decimal TargetBonus { get; set; }
        public decimal CollectionBonus { get; set; }
        public decimal MonopolyBonus { get; set; }
        public decimal BgBonus { get; set; }
        public decimal CMTPScheme { get; set; }
        public decimal VPBScheme { get; set; }
        public decimal? TotalBonus { get; set; }
        public decimal? TotalBonusPerQty { get; set; }
        public decimal? NCRAmount { get; set; }
        public decimal? NCRPerQty { get; set; }

    }

    public class DynamicColumnForNCR
    {
        public string Name { get; set; }
        public decimal? Value { get; set; }
    }

    public class SalesRegisterDetailGridModel
    {
        public List<SalesRegisterDetailModel> salesRegisterDetailModel { get; set; }
        public decimal total { get; set; }
        public SalesRegisterDetailGridModel()
        {
            salesRegisterDetailModel = new List<SalesRegisterDetailModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }

    public class DailySalesTreeListGridModel
    {
        public List<DailySalesTreeList> dailySalesTreeListModel { get; set; }
        public decimal total { get; set; }
        public DailySalesTreeListGridModel()
        {
            dailySalesTreeListModel = new List<DailySalesTreeList>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }


    // [Serializable]
    // public  class   GoodsReceiptNotesDetailModel:Entity
    public class GoodsReceiptNotesDetailModel
    {
        public Int32? Id { get; set; }
        public string MRR_NO { get; set; }
        public DateTime MRR_DATE { get; set; }
        public string MANUAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string SUPPLIER_INV_NO { get; set; }
        public string SUPPLIER_MRR_NO { get; set; }
        public DateTime? SUPPLIER_INV_DATE { get; set; }
        public string PP_NO { get; set; }
        public string REMARKS { get; set; }
        public string CURRENCY_CODE { get; set; }
        public decimal EXCHANGE_RATE { get; set; }
        public string LOCATION_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public int QUANTITY { get; set; }
        public int UNIT_PRICE { get; set; }
        public int TOTAL_PRICE { get; set; }
        public string FORM_EDESC { get; set; }
        public string ITEM_GROUP_EDESC { get; set; }
        public string ITEM_SUBGROUP_EDESC { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }



    }
    //[Serializable]
    public class GoodsReceiptNotesDetailModelMongo : Entity
    {
        public virtual string MRR_NO { get; set; }
        public virtual DateTime MRR_DATE { get; set; }
        public virtual string MANUAL_NO { get; set; }
        public virtual string SUPPLIER_CODE { get; set; }
        public virtual string SUPPLIER_NAME { get; set; }
        public virtual string SUPPLIER_INV_NO { get; set; }
        public virtual string SUPPLIER_MRR_NO { get; set; }
        public virtual DateTime? SUPPLIER_INV_DATE { get; set; }
        public virtual string PP_NO { get; set; }
        public virtual string REMARKS { get; set; }
        public virtual string CURRENCY_CODE { get; set; }
        public virtual decimal EXCHANGE_RATE { get; set; }
        public virtual string LOCATION_EDESC { get; set; }
        public virtual string ITEM_CODE { get; set; }
        public virtual string ITEM_EDESC { get; set; }
        public virtual decimal QUANTITY { get; set; }
        public virtual decimal UNIT_PRICE { get; set; }
        public virtual decimal TOTAL_PRICE { get; set; }
        public virtual string FORM_EDESC { get; set; }
        public virtual string ITEM_GROUP_EDESC { get; set; }
        public virtual string ITEM_SUBGROUP_EDESC { get; set; }
        public virtual string CATEGORY_CODE { get; set; }
        public virtual string CATEGORY_EDESC { get; set; }
        public virtual string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }



    }
    public class GoodsReceiptNotesGridModel
    {
        public List<GoodsReceiptNotesDetailModel> GoodsReceiptNotesDetailModel { get; set; }
        public decimal total { get; set; }
        public GoodsReceiptNotesGridModel()
        {
            GoodsReceiptNotesDetailModel = new List<GoodsReceiptNotesDetailModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }


    public class ConsumptionIssueRegisterDetailModel
    {
        public string ISSUE_NO { get; set; }
        public DateTime ISSUE_DATE { get; set; }
        public string MANUAL_NO { get; set; }
        public string ISSUE_TYPE_CODE { get; set; }
        public string FROM_LOCATION_CODE { get; set; }
        public string TO_LOCATION_CODE { get; set; }
        public string TO_BUDGET_FLAG { get; set; }
        public int SERIAL_NO { get; set; }
        public string MU_CODE { get; set; }
        public decimal? REQ_QUANTITY { get; set; }
        public decimal? QUANTITY { get; set; }
        public decimal? UNIT_PRICE { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal? CALC_QUANTITY { get; set; }
        public decimal? CALC_UNIT_PRICE { get; set; }
        public decimal? CALC_TOTAL_PRICE { get; set; }
        public decimal? COMPLETED_QUANTITY { get; set; }
        public string REMARKS { get; set; }
        public string CURRENCY_CODE { get; set; }
        public decimal? EXCHANGE_RATE { get; set; }
        public string LOCATION_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_EDESC { get; set; }
        public string ISSUE_TO { get; set; }
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string ITEM_GROUP_EDESC { get; set; }
        public string ITEM_SUBGROUP_EDESC { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }



    }

    public class ConsumptionIssueRegisterModel
    {
        public IEnumerable<ConsumptionIssueRegisterDetailModel> ConsumptionIssueRegisterDetailModel { get; set; }
        public decimal total { get; set; }
        public ConsumptionIssueRegisterModel()
        {
            ConsumptionIssueRegisterDetailModel = new List<ConsumptionIssueRegisterDetailModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }

    public class MaterializeModel
    {
        public string FISCAL_YEAR { get; set; }
        public string BILL_NO { get; set; }
        public string BILL_DATEAD { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_PAN { get; set; }
        public string BILL_DATE { get; set; }
        public double? AMOUNT { get; set; }
        public double? DISCOUNT { get; set; }
        public double? TAXABLE_AMOUNT { get; set; }
        public double? TAX_AMOUNT { get; set; }
        public double? TOTAL_AMOUNT { get; set; }
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
        public string TableName { get; set; }


    }
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class MaterializeViewModel
    {
        public List<MaterializeModel> RegisterDetails { get; set; }
        public int total { get; set; }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
        public MaterializeViewModel()
        {
            this.AggregationResult = new Dictionary<string, AggregationModel>();
            RegisterDetails = new List<MaterializeModel>();
        }
    }

}
