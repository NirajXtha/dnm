using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class FORAGEING
    {
        public DateTime VOUCHER_DATE { get; set; }
        public string VOUCHER_NO { get; set; }
        public int SUB { get; set; }
        public decimal? PENDING_BAL { get; set; }
        public decimal? AGE { get; set; }
        public decimal? CLOSING_BALANCE { get; set; }
        public decimal? AMM { get; set; }
    }

    public class AgeingCustomerModel
    {
        public int CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
    }

    public class DateModel
    {
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
    }

    public class TransactionRequestModel
    {
        public string from_date { get; set; }
        public string to_date { get; set; }
        public string sub_code { get; set; }
        public string acc_code { get; set; }
        public string user_id { get; set; }
        public string BRANCH_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
    }
}
