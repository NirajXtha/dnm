using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models.CustomModels
{
    public class MobileDataVoucherModel
    {
        public int SESSION_ROWID { get; set; }

        public string VOUCHER_NO { get; set; }

        public DateTime VOUCHER_DATE { get; set; }

        public string Miti { get; set; }

        public string FORM_CODE { get; set; }

        public decimal? VOUCHER_AMOUNT { get; set; }

        public string FORM_DESCRIPTION { get; set; }

        public string CREATED_BY { get; set; }

        public string TABLE_NAME { get; set; }

        public string MODULE_CODE { get; set; }

        public string FORM_TYPE { get; set; }

        public string REMARKS { get; set; }

        public string PARTICULARS { get; set; }

        public string ACC_CODE { get; set; }

        public string LEDGER_TITLE { get; set; }

        public string BRANCH_DESCRIPTION { get; set; }

        public string CHECKED_BY { get; set; }
        public string AUTHORISED_BY { get; set; }
        public string CHECK_FLAG { get; set; }
        public string POST_FLAG { get; set; }
        public string VERIFY_FLAG { get; set; }
    }

    public class MobileVoucherDetailData
    {
        public string CreatedBy { get; set; }

        public string CreatedDateString { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime VoucherDate { get; set; }

        public string TransactionType { get; set; }

        public string VoucherNo { get; set; }

        public int SubledgerCount { get; set; }

        public decimal? DebitAmount { get; set; }

        public decimal? CreditAmount { get; set; }

        public decimal? InQty { get; set; }

        public decimal? OutQty { get; set; }

        public string Unit { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public string ItemCode { get; set; }

        public string Description { get; set; }

        public string Particulars { get; set; }

        public string Remarks { get; set; }

        public string AccCode { get; set; }

        public  int SerialNo { get; set; }

    }

    public class MobileLedgerDetailData
    {
        public string CreatedBy { get; set; }
        public string VoucherNo { get; set; }

        public string TransactionType { get; set; }
                
        public decimal? DebitAmount { get; set; }

        public decimal? CreditAmount { get; set; }

        public string Description { get; set; }

        public string SubCode { get; set; }

        public int SerialNo { get; set; }

        public string AccCode { get; set; }
    }

    public class MobileCostCenterDetailData
    {
        public string VOUCHER_NO { get; set; }
        public DateTime VOUCHER_DATE { get; set; }
        public string PARTICULARS { get; set; }
        public decimal? DR_AMOUNT { get; set; }
        public decimal? CR_AMOUNT { get; set; }
        public int SERIAL_NO { get; set; }
        public string MITI { get; set; }
        public string BUDGET_EDESC { get; set; }
        public string BUDGET_CODE { get; set; }
    }

    public class CostCenterCategoryData
    {
        public string COST_CATEGORY_CODE { get; set; }
        public string COST_CATEGORY_EDESC { get; set; }
        public int COST_CATEGORY_STEP { get; set; }
    }

    public class MobileCostCenterVatDetails
    {
        public string MANUAL_NO { get; set; }
        public DateTime INVOICE_DATE { get; set; }
        public string CS_CODE { get; set; }
        public string DOC_TYPE { get; set; }
        public decimal? TAXABLE_AMOUNT { get; set; }
        public decimal? VAT_AMOUNT { get; set; }
        public int SERIAL_NO { get; set; }
        public string MITI { get; set; }
        public string P_TYPE { get; set; }

        
    }

    public class ModuleCount
    {
        public string MODULE_CODE { get; set; }
        public int MODULE_COUNT { get; set; }
    }

    public class ApproveVoucherModel
    {
        [Required]
        public string FORM_CODE { get; set; }

        [Required]
        public string VOUCHER_NO { get; set; }

        [Required]
        public string USER_NAME { get; set; }

        public string company_code { get; set; }

        public string branch_code { get; set; }
    }

    public class GetVoucherModel
    {
        public string moduleCode { get; set; } = string.Empty;

        public string companyCode { get; set; } = string.Empty;
        public string branchCode { get; set; } = string.Empty;

        public int sessionRowId { get; set; } = 0;
        public string append { get; set; } = LoadMode.top.ToString();
        [Required]
        public int UserId { get; set; }

    }

    public class GetSubLdegerDetail
    {
        public string CompanyCode { get; set; }

        public string BranchCode { get; set; }

        public string AccountCode { get; set; }
        public string PartyTypeCode { get; set; }
        //[Required]
        public string SubCode { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        [Required]
        public int UserId { get; set; }

    }

    public class GeneralLedgerDetail
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string AccountCode { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
        [Required]
        public int UserId { get; set; }
        public string DataGeneric { get; set; }
       public ReportFiltersModel Filter { get; set; }
        public GeneralLedgerDetail()
        {
            Filter = new ReportFiltersModel();
        }

    }

    public enum LoadMode
    {
        top,
        bottom,
    }

    public class LoginModel
    {
        [Required]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }
    }

    public class LoginResponseModel
    {
        public int USER_ID { get; set; }

        public string FULL_NAME { get; set; } = string.Empty;

        public string USER_NAME { get; set; }

        public string COMPANY_CODE { get; set; }

        public List<string> MODULE_PERMISSION { get; set; } = new List<string>();
    }

    public class GetVoucherDetailModel
    {
        // [Required]
        public string formCode { get; set; }

        //[Required]
        public string voucherCode { get; set; } 

        //[Required]
        public int userId { get; set; }

        public string tableName { get; set; }

        public string companyCode { get; set; }

        public string branchCode { get; set; }

    }

    public class AccountTreeModelMobile
    {
        public int Level { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string AccountTypeFlag { get; set; }
        public string MasterAccCode { get; set; }
        public string PreAccCode { get; set; }
        public string BranchCode { get; set; }
        public bool HasChildren { get; set; }
        public Decimal Child_rec { get; set; }
       
    }

    public class SubLedgerList
    {
        public string Sub_code { get; set; }
        public string SubLedgerName { get; set; }
    }

    public class DealerPartyTypeModel
    {
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string ACC_EDESC { get; set; }
    }

    public class AccountModel
    {
        public string Acc_code { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
    }

    public class VoucherDetailModelMobile
    {
        public DateTime voucher_date { get; set; }
        public string Miti { get; set; }
        public string manual_no { get; set; }
        public decimal? Balance { get; set; }
        public string BalanceHeader { get; set; }
        public string Voucher_no { get; set; }
        public string PARTICULARS { get; set; }
        public Decimal? dr_amount { get; set; }
        public Decimal? cr_amount { get; set; }
    }

    public class PayOrderModel
    {
        [Required]
        public string VOUCHER_NO { get; set; }
        public DateTime VOUCHER_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string PARTICULARS { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public decimal AMOUNT { get; set; }
        [Required]
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CHECKED_REMARKS { get; set; }
        public string VERIFY_REMARKS { get; set; }
        public string AUTHORISED_REMARKS { get; set; }
    }

    public class SalesReportModel
    {
        public SalesReportModel()
        {
            QUARTER_SALES = new List<QuarterlySalesModel>();
        }
        public string YTD_SALES { get; set; }
        public string MTD_SALES { get; set; }
        public string TODAY_SALES { get; set; }
        public string YTD_QSALES { get; set; }
        public string MTD_QSALES { get; set; }
        public string TODAY_QSALES { get; set; }
        public List<QuarterlySalesModel> QUARTER_SALES { get; set; }
        public int? PAPCOUNT { get; set; }
        public int? POPCOUNT { get; set; }
        public int? PRPCOUNT { get; set; }
    }
    public class QuarterlySalesModel
    {
        public string Quarter { get; set; }
        public string Amount { get; set; }
        public string Quantity { get; set; }
    }

    public class SalesTargetGraphMobile
    {
        public string Month { get; set; }
        public string MonthInt { get; set; }
        public string MonthYear { get; set; }
        public decimal Sales { get; set; }
        public decimal Target { get; set; }
        public decimal Qty_Sales { get; set; }
        public decimal Qty_Target { get; set; }
    }
    public class SalesTargetViewModelMobile
    {
        public string Branch_name { get; set; }
        public string Branch_code { get; set; }
        public string Month { get; set; }
        public string MonthInt { get; set; }
        public decimal? TargetQty { get; set; }
        public decimal? TargetAmount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? GrossAmount { get; set; }
        public string DataType { get; set; }
        public string Year { get; set; }


    }
    public class TopicSalesModel
    {
        public string EDESC { get; set; }
        public string AMOUNT { get; set; }
        public string QUANTITY { get; set; }
    }

    public class PapModel
    {
        public string ACC_NAME { get; set; }
        public string SUB_EDESC { get; set; }
        public string CHEQUE_NO { get; set; }
        public string DR_AMOUNT { get; set; }
    }
    public class PrpModel
    {
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public decimal? ORDER_QTY { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? AVG_RATE { get; set; }
        public decimal? PURCHASE_PRICE { get; set; }
        public string FROM_LOCATION_EDESC { get; set; }

        public string TO_LOCATION_EDESC { get; set; }
    }
    public class PopModel : PrpModel
    {
        public string SUPPLIER_EDESC { get; set; }
    }

    public class AccountSummaryModel
    {
        public string SUBCODE { get; set; }
        public string SUBEDESC { get; set; }
        public string COMPANYCODE { get; set; }
        public decimal? DROPENING { get; set; }
        public decimal? CROPENING { get; set; }
        public decimal? DRAMOUNT { get; set; }
        public decimal? CRAMOUNT { get; set; }
        public decimal? DRBALANCE { get; set; }

        public decimal? CRBALANCE { get; set; }



    }


    public class LedgerFilter
    {
        public string Name { get; set; }
      
    }

    public class ItemModel
    {
        public string EDESC { get; set; }
        public string CODE { get; set; }
    }
    public class TopEmployeeWithAmountQtyModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? QUANTITY { get; set; }
        public string QTYORAMT { get; set; }
    }
    public class TopDealerWithAmountQtyModel
    {
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? QUANTITY { get; set; }
        public string QTYORAMT { get; set; }
    }

    public class GateEntryModel
    {
        public string GATE_NO { get; set; }
        public DateTime? GATE_DATE { get; set; }
        public string MANUAL_NO { get; set; }
        public string VEHICLE_NAME { get; set; }
        public string IN_TIME { get; set; }
        public string OUT_TIME { get; set; }
        public string BILL_NO { get; set; }
        public DateTime? BILL_DATE { get; set; }
        public string TRANSPORT_NAME { get; set; }
        public string LOCATION_CODE { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public decimal? BILL_QTY { get; set; }
        public decimal? BILL_VALUE { get; set; }
        public decimal? GROSS_WT { get; set; }
        public decimal? TEAR_WT { get; set; }
        public decimal? NET_WT { get; set; }
        public string DRIVER_NAME { get; set; }
        public string PERSON { get; set; }
        public string RECEIVED_BY { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public char? DELETED_FLAG { get; set; }
        public char? GATE_IN_FLAG { get; set; }
        public string GATE_IN_BY { get; set; }
        public char? WEIGHT_BRIDGE_FLAG { get; set; }
        public string WEIGHT_BRIDGE_BY { get; set; }
        public char? UNLOADING_FLAG { get; set; }
        public string UNLOADING_BY { get; set; }
        public char? WB_OUT_FLAG { get; set; }
        public string WB_OUT_BY { get; set; }
        public char? GATE_OUT_FLAG { get; set; }
        public string GATE_OUT_BY { get; set; }
        public string TOTAL_VEHICLE_HR { get; set; }
        public DateTime? GATE_OUT_DATE { get; set; }
        public string INWARD_TYPE { get; set; }
        public char? CLOSE_FLAG { get; set; }
        public string DIVISION_CODE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string REFERENCE_NO { get; set; }
        public string VEHICLE_CODE { get; set; }
        public decimal? WB_CODE { get; set; }
        public DateTime? FIRST_DATE { get; set; }
        public DateTime? SECOND_DATE { get; set; }
        public string FIRST_TIME { get; set; }
        public string SECOND_TIME { get; set; }
        public decimal? PARTY_WEIGHT { get; set; }
        public decimal? NO_OF_PACKET { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string TRANSPORT_CODE { get; set; }
        public string REF_VOUCHER_NO { get; set; }
        public decimal? PRINT_COUNT { get; set; }
        public string GATE_MITI { get; set; }
        public string BILL_MITI { get; set; }
        public string LOCATION_EDESC { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal? Detail_BILL_QTY { get; set; }
        public string BILL_RATE { get; set; }
        public string FORM_CODE { get; set; }
    }

    public class GateEntryRequestModel
    {
        public DateTime? FROM_DATE { get; set; }
        public DateTime? TO_DATE { get; set; }
        public string PRODUCT_FILTER { get; set; }
    }


    public class NameCodeFlag
    {
        public string NAME { get; set; }
        public string CODE { get; set; }
        public string FLAG { get; set; }
    }

    public class GateReferenceDto
    {
        public string ORDER_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }

    public class PartyNameByReferenceDto
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public DateTime? ORDER_DATE { get; set; }
    }

    public class ReferenceItemDto
    {
        public int SERIAL_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public decimal? AVAILABLE_QTY { get; set; }
        public string FORM_CODE { get; set; }
        public decimal? UNIT_PRICE { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
    }

    public class ReferencePairDto
    {
        public string ReferenceNo { get; set; }
        public string FormCode { get; set; }
    }

    public class MultiReferenceRequest
    {
        public List<ReferencePairDto> References { get; set; }
        public DateTime? GateDate { get; set; }
    }

    public class SupplierInfoDto
    {
        public string SUPPLIER_EDESC { get; set; }
        public string SUPPLIER_CODE { get; set; }
    }
}
