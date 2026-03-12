using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.BrandingModule
{
    public class OpeningStockSetupModel
    {
        public int OPENING_STOCK_ID { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
        public string DISTRIBUTOR_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public int CURRENT_STOCK { get; set; }
        public int PURCHASE_QTY { get; set; }
        public string SP_CODE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }
    public class OpeningDetailModel
    {
        public OpeningDetailModel()
        {
            OpeningList = new List<OpeningStockSetupModel>();
        }
        public string DistributerCode { get; set; }
        public string DistributerName { get; set; }
        public string DistributerAddress { get; set; }
        public int OpeningStockId { get; set; }
        public DateTime StockDate { get; set; }
        public List<OpeningStockSetupModel> OpeningList { get; set; }
    }
}
