using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class UpdateEntityRequestModel:CommonRequestModel
    {
        public UpdateEntityRequestModel()
        {
            stock = new List<StockItemModel>();
        }
        public string sp_code { get; set; }
        public string customer_code { get; set; }
        public List<StockItemModel> stock { get; set; }
    }
    public class StockItemModel
    {
        public string item_code { get; set; }
        public string mu_code { get; set; }
        public string cs { get; set; }
        public string p_qty { get; set; }
        public string Sync_Id { get; set; }
    }

    public class UpdateEodUpdate : CommonRequestModel
    {
        public string sp_code { get; set; }
        public int PO_D_COUNT { get; set; }
        public int PO_R_COUNT { get; set; }
        public int reseller_detail { get; set; }
        public int reseller_master { get; set; }
        public int reseller_entity { get; set; }
        public int reseller_photo { get; set; }
        public int reseller_contact_photo { get; set; }
        public string Remarks { get; set; }
        public string Time_Eod { get; set; }
    }

    public class EODUpdate
    {
        public string GROUP_EDESC { get; set; }
        public string SP_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string TOD_ROUTE_CODE { get; set; }
        public string TOD_ROUTE_NAME { get; set; }
        public string TOM_ROUTE_CODE { get; set; }
        public string TOM_ROUTE_NAME { get; set; }
        public string ATN_IMAGE { get; set; }
        public DateTime? ATN_DATE { get; set; }
        public decimal? TARGET { get; set; }
        public decimal? VISITED { get; set; }
        public decimal? NOT_VISITED { get; set; }
        public decimal? PJP_PRODUCTIVE { get; set; }
        public decimal? PJP_NON_PRODUCTIVE { get; set; }
        public decimal? NPJP_PRODUCTIVE { get; set; }
        public decimal? NPJP_COUNT { get; set; }
        public decimal? PJP_COUNT { get; set; }
        public decimal? OUTLET_ADDED { get; set; }
        public decimal? TOTAL_AMOUNT { get; set; }
        public DateTime? EOD_DATE { get; set; }
    }
}
