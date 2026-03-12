using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.ProjectManagement.Service.Models
{
    public class EntryReport
    {
        public string VOUCHER_NO { get; set; }
        public DateTime VOUCHER_DATE { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string CREATED_BY { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public decimal CALC_QUANTITY { get; set; }
        public decimal CALC_UNIT_PRICE { get; set; }
        public decimal CALC_TOTAL_PRICE { get; set; }
        public string BUDGET_FLAG { get; set; }
        public decimal BUDGET_AMOUNT { get; set; }
        public string SUB_PROJECT_NAME { get; set; }
        public string PROJECT_NAME { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string LOCATION_EDESC { get; set; }
    }

}
