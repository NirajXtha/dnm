using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models.SalesOrderIndent
{
    public class SalesOrderAdjustViewModel
    {
        public int ROW_NO { get; set; }
        public string ORDER_NO { get; set; }
        public string ORDER_DATE { get; set; }
        public string MANUAL_NO { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string SERIAL_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public int QUANTITY { get; set; }
        public int UNIT_PRICE { get; set; }
        public int TOTAL_PRICE { get; set; }
        public int CALC_QUANTITY { get; set; }
        public int CALC_UNIT_PRICE { get; set; }
        public int CALC_TOTAL_PRICE { get; set; }
        public int COMPLATED_QUANTITY { get; set; }
        public string REMARKS { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string MODIFY_BY { get; set; }
        public string MODIFY_DATE { get; set; }
        public int EXCHANGE_RATE { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string BRANCH_CODE { get; set; }
        public int ADJUST_QUANTITY { get; set; }
        public int CANCEL_QUANTITY { get; set; }
        public bool CONFIRM_CHANGE { get; set; } = false;
    }
}
