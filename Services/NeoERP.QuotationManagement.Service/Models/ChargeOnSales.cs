using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
    public class ChargeOnSales
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public int? PRIORITY_INDEX_NO { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public double? VALUE_PERCENT_AMOUNT { get; set; }
        public double? CHARGE_AMOUNT { get; set; }
        public double? CALC { get; set; }
        public DateTime APPLY_FROM_DATE { get; set; }
        public DateTime APPLY_TO_DATE { get; set; }
        public string APPLY_ON { get; set; }
        public string CHARGE_APPLY_ON { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string IMPACT_ON { get; set; }
        public string APPLY_QUANTITY { get; set; }
        public string SUB_CODE { get; set; }
        public string SERIAL_NO { get; set; }
        public string BUDGET_CODE { get; set; }
        public string GL_FLAG { get; set; }
        public string NON_GL_FLAG { get; set; }
        public string GL { get; set; }
        public string APPORTION_FLAG { get; set; }
        public string APPORTION { get; set; }
        public string ON_ITEM { get; set; }
        public string MANUAL_CALC_CHARGE { get; set; }
        public string SPECIFIC_CHARGE_FLAG { get; set; }

        //AA Added to check whether the charges are active or not 
        public string CHARGE_ACTIVE_FLAG { get; set; }
    }
    public class ChargeOnItem
    {
        public List<ChargeOnItemAmountWise> ChargeOnItemAmountWiseList { get; set; }
        public List<ChargeOnItemQuantityWise> ChargeOnItemQuantityWiseList { get; set; }
    }

    public class ChargeOnItemAmountWise
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public int? PRIORITY_INDEX_NO { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public double? VALUE_PERCENT_AMOUNT { get; set; }
        public double? CHARGE_AMOUNT { get; set; }
        public DateTime APPLY_FROM_DATE { get; set; }
        public DateTime APPLY_TO_DATE { get; set; }
        public string APPLY_ON { get; set; }
        public string CHARGE_APPLY_ON { get; set; }
        public string ACC_CODE { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string IMPACT_ON { get; set; }
        public string APPLY_QUANTITY { get; set; }
        public string SUB_CODE { get; set; }
        public string BUDEGT_CODE { get; set; }
        public string BUDGET_VAL { get; set; }
        public string GL { get; set; }
        public string APPORTION { get; set; }
        public string CALC { get; set; }

    }
    public class ChargeOnItemQuantityWise
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public int? PRIORITY_INDEX_NO { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public double? VALUE_PERCENT_AMOUNT { get; set; }
        public double? CHARGE_AMOUNT { get; set; }
        public DateTime APPLY_FROM_DATE { get; set; }
        public DateTime APPLY_TO_DATE { get; set; }
        public string APPLY_ON { get; set; }
        public string CHARGE_APPLY_ON { get; set; }
        public string ACC_CODE { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string IMPACT_ON { get; set; }
        public string APPLY_QUANTITY { get; set; }
        public string SUB_CODE { get; set; }
        public string BUDEGT_CODE { get; set; }
        public string BUDGET_VAL { get; set; }
        public string GL { get; set; }
        public string APPORTION { get; set; }
        public string CALC { get; set; }


    }

}
