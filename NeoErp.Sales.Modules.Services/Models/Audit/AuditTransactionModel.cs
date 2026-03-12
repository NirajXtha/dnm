using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Audit
{
    public  class AuditTransactionModel
    {
       
            public double AUDIT_SR_NO { get; set; }
            public string VOUCHER_NO { get; set; }
            public DateTime? VOUCHER_DATE { get; set; }
            public string MANUAL_NO { get; set; }
            public string CUSTOMER_CODE { get; set; }
            public string SUPPLIER_CODE { get; set; }
            public string PARTY_NAME { get; set; }
            public string PARTY_TYPE_CODE { get; set; }
            public string DEALER_NAME { get; set; }
            public string BUDGET_FLAG { get; set; }
            public int? SERIAL_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string MU_CODE { get; set; }
            public double? QUANTITY { get; set; }
            public double? UNIT_PRICE { get; set; }
            public double? TOTAL_PRICE { get; set; }
            public double? CALC_QUANTITY { get; set; }
            public double? CALC_UNIT_PRICE { get; set; }
            public double? CALC_TOTAL_PRICE { get; set; }
            public string REMARKS { get; set; }
            public int? CREDIT_DAYS { get; set; }
            public string CURRENCY_CODE { get; set; } = "NRS";
            public double EXCHANGE_RATE { get; set; } = 1;
            public string FROM_LOCATION_CODE { get; set; }
            public string TO_LOCATION_CODE { get; set; }
            public string FORM_CODE { get; set; }
            public string COMPANY_CODE { get; set; }
            public string BRANCH_CODE { get; set; }
            public string CREATED_BY { get; set; }
            public DateTime? CREATED_DATE { get; set; }
            public DateTime? MODIFY_DATE { get; set; }
            public string MODIFY_BY { get; set; }
            public string DELETED_FLAG { get; set; }
            public string SESSION_ROWID { get; set; }
            public string BATCH_NO { get; set; }
            public double? FREE_QTY { get; set; }
            public string SMS_FLAG { get; set; }
            public string DESCRIPTION { get; set; }
            public double? ROLL_QTY { get; set; }
            public string PRIORITY_CODE { get; set; }
            public string PAYMENT_MODE { get; set; }
            public string MEMBER_SHIP_CARD { get; set; }
            public string PAYMODE_VALUE { get; set; }
            public string SHIPPING_ADDRESS { get; set; }
            public string SHIPPING_CONTACT_NO { get; set; }
            public string SALES_TYPE_CODE { get; set; }
            public string EMPLOYEE_CODE { get; set; }
            public string REASON { get; set; }
            public string MISC_CODE { get; set; }
            public string AGENT_CODE { get; set; }
            public string DIVISION_CODE { get; set; } 
            public double? NET_GROSS_RATE { get; set; }
            public double? NET_SALES_RATE { get; set; }
            public double? AREA_CODE { get; set; }
            public double? NET_TAXABLE_RATE { get; set; }
            public double? SECOND_QUANTITY { get; set; }
            public double? THIRD_QUANTITY { get; set; }
            public string PERSON_NAME { get; set; }
            public double? SECTOR_CODE { get; set; }
            public DateTime? START_DATE { get; set; }
            public DateTime? END_DATE { get; set; }
            public double? NO_OF_PAX { get; set; }
            public string SECTOR_NAME { get; set; }
            public double? LC_SERIAL_NO { get; set; }
            public double? EXCISE_AMOUNT { get; set; }
            public double? DISCOUNT_AMOUNT { get; set; }
            public double? VAT_AMOUNT { get; set; }
            public string IP_ADDRESS { get; set; }
            public string HOST { get; set; }
            public string OS_USER { get; set; }
            public string DB_USER { get; set; }
            public string ACTION_THROUGH { get; set; }
            public string ACTION_TYPE { get; set; }
            public string TABLE_NAME { get; set; }
            public DateTime? ACTION_DATE { get; set; }
    }

        public class AuditTransactionViewModel
    {
            public AuditTransactionViewModel()
            {
                AuditTransaction = new List<AuditTransactionModel>();
                AggregationResult = new Dictionary<string, AggregationModel>();
            }
            public List<AuditTransactionModel> AuditTransaction { get; set; }
            public Dictionary<string, AggregationModel> AggregationResult { get; set; }
            public int Total { get; set; }
        }
    
}
