using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    //public class DynamicFinancialBudgetTransaction
    //{
    //    public List<FinancialBudgetTransaction> dynamicModalData { get; set; }
    //}
   public class FinancialBudgetTransaction
    {
        public string ACC_CODE { get; set; }
        public string BUDGET_FLAG { get; set; }
        public int? SERIAL_NO { get; set; }
        public string BUDGET_VAL { get; set; }
        public string BUDGET_CODE { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? QUANTITY { get; set; }
        public string NARRATION { get; set; }
        public List<BUDGETDATA> BUDGET { get; set; }
        public int SUB_PROJECT_CODE { get; set; }
        public string SESSION_ROWID { get; set; }
    }
    public class BUDGETDATA
    {
        public int? SERIAL_NO { get; set; }
        public string BUDGET_VAL { get; set; }
        public string BUDGET_CODE { get; set; }
        public decimal? AMOUNT { get; set; }
        public string NARRATION { get; set; }
        public decimal? QUANTITY { get; set; }
        public string ACC_CODE { get; set; }
    }
    public class BATCHTRANSACTIONDATA
    {
        public string TRANSACTION_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string LOCATION_CODE { get; set; }
        public int? SERIAL_NO { get; set; }
        public string TRACKING_SERIAL_NO { get; set; }
        public string SOURCE_FLAG { get; set; }
        public List<TRACKINDATA> TRACK { get; set; }
        public string BATCH_NO { get; set; }
        public decimal? QUANTITY { get; set; }
        public char SERIAL_TRACKING_FLAG { get; set; }
        public char BATCH_TRACKING_FLAG { get; set; }
        public DateTime EXPIRY_DATE { get; set; }
        public bool REFERNCE_FROM_BATCH { get; set; }
    }

    public class BATCH_TRANSACTION_DATA
    {
        public string TRANSACTION_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string LOCATION_CODE { get; set; }
        public int? SERIAL_NO { get; set; }
        public string BATCH_NO { get; set; } = "";

        public DateTime? EXPIRY_DATE { get; set; }

    }
    public class TRACKINDATA
    {
        public int? SERIAL_NO { get; set; }
        public string TRACKING_SERIAL_NO { get; set; }
        public string SOURCE_FLAG { get; set; }
    }
    public class CHARGETRANSACTION
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public decimal CHARGE_AMOUNT { get; set; }
        public string CURRENCY_CODE { get; set; }
        public decimal EXCHANGE_RATE { get; set; }
        public string REFERENCE_NO { get; set; }
        public decimal CALC_UNIT_PRICE { get; set; }
        public decimal CALC_TOTAL_PRICE { get; set; }
        public string ITEM_CODE { get; set; }
        public decimal QUANTITY { get; set; }
        public string FORM_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string APPLY_ON { get; set; }

        public string GL_FLAG { get; set; }

        public string APPORTION_ON { get; set; }

        public string IMPACT_ON { get; set; }

        public string SERIAL_NO { get; set; }


        
    }
}
