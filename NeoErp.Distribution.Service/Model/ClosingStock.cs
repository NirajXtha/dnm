using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class ClosingStock
    {
        public ClosingStock()
        {
            OS_Details = new List<ClosingStockItem>();
        }
        public int? STOCK_ID { get; set; }
        public string DistCustomer { get; set; }
        public string DistributerName { get; set; }
        public string DistributerAddress { get; set; }
        public DateTime StockDate { get; set; }
        public List<ClosingStockItem> OS_Details { get; set; }
    }
    public class ClosingStockItem
    {
        public int? STOCK_ID { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
        public string DISTRIBUTOR_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string MU_CODE { get; set; }
        public int Current_STOCK { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string SP_CODE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_DATE_STRING { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }

    public class itemList
    {
        public int STOCK_ID { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
        public string DISTRIBUTOR_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string MU_CODE { get; set; }
        public int Current_STOCK { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string SP_CODE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }

    public class ResellerItemRate
    {
        public int USERID { get; set; }
        public string SP_CODE { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string ENTITY_CODE { get; set; }
        public string ENTITY_NAME { get; set; }
        public string RESELLER_CODE { get; set; }
        public string RESELLER_NAME { get; set; }
        public decimal? RATE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }
    public class MerchandisingStockModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string SP_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public int? CURRENT_STOCK { get; set; }
        public int? PURCHASE_QTY { get; set; }
        public string TYPE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string BRAND_NAME { get; set; }
        public string GROUP_EDESC { get; set; }
        public string TYPE_EDESC { get; set; }
        public string SUBTYPE_EDESC { get; set; }
        public int? REMAINING_STOCK { get; set; }
    }

}
