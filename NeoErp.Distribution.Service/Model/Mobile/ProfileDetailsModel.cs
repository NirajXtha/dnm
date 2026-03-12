using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class ProfileDetailsModel
    {
        public string userId {  get; set; }
        public string sp_code { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string data { get; set; }
    }
    public class SalesVsCollectionModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string MASTER_CODE { get; set; }
        public string PRE_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public decimal OPENING { get; set; }
        public decimal COLLECTION { get; set; }
        public decimal NET_SALES_VALUE { get; set; }
        public string COMPANY_CODE { get; set; }
        public int? ROW_LEVEL { get; set; }

    }
    public class MonthlySalesVsCollectionModel
    {
        public string MONTH_NAME { get; set; }
        public decimal OPENING { get; set; }
        public decimal COLLECTION { get; set; }
        public decimal SALES_VALUE { get; set; }
    }
    public class ClosingStockModel
    {
        public string sp_code { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string distributor_code { get; set; }
        public string reseller_code { get; set; }

    }
    public class ClosingStockDtlModel
    {
        public string CODE { get; set; }
        public decimal LVS { get; set; }
        public DateTime? LVS_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string ITEM_CODE { get; set; }
    }
    public class UserDetailsModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string USER_ID { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string START_DT { get; set; }
        public string END_DT { get; set; }
    }
}
