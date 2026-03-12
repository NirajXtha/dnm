using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
   public class TopEffectiveModel
    {
        public string SP_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public decimal EXTRA { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public decimal PJP_TOTAL_QUANTITY { get; set; }
        public decimal NPJP_TOTAL_QUANTITY { get; set; }
        public decimal PERCENT_EFFECTIVE_CALLS { get; set; }
        public decimal PERCENT_PRODUCTIVE_CALLS { get; set; }
        public decimal PERCENT_ACHIEVED_TARGET { get; set; }
        public int OUTLET_ADDED { get; set; }
        public int PRODUCTIVE_CALLS { get; set; }
        public int VISITED { get; set; }
        public int NOT_VISITED { get; set; }
        public int TARGET { get; set; }
        public int TARGET_VISITED { get; set; }
        public decimal PJP_TOTAL_AMOUNT { get; set; }
        public decimal NPJP_TOTAL_AMOUNT { get; set; }
        public string GROUPID { get; set; }
        public string GROUP_EDESC { get; set; }
        public int PJP_PRODUCTIVE { get; set; }
        public int NPJP_PRODUCTIVE { get; set; }
        public int TOTAL_PJP { get; set; }
        public int PJP_NON_PRODUCTIVE { get; set; }
    }
    public class TotalEffectiveDataModel : TopEffectiveModel
    {
        public decimal? NET_PERCENT_EFFECTIVE_CALLS { get; set; }
        public decimal? TOTAL_QUANTITY { get; set; }
        public decimal? TOTAL_AMOUNT { get; set; }
    }

    public class PairModel
    {
        public string EDESC { get; set; }
        public decimal QUANTITY { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
    }

    public class DetailTopEffective : TotalEffectiveDataModel
    {
        public DetailTopEffective()
        {
            OUTLET_TYPE = new List<PairModel>();
            OUTLET_SUB_TYPE = new List<PairModel>();
            BRANDWISE = new List<PairModel>();
        }
        public string ROUTE_NAME { get; set; }
        public string GROUP_EDESC { get; set; }
        public string ATN_TIME { get; set; }
        public string EOD_TIME { get; set; }
        public decimal WORKING_HOURS { get; set; }
        public string EOD_REMARKS { get; set; }
        public DateTime ASSIGN_DATE { get; set; }
        public List<PairModel> OUTLET_TYPE { get; set; }
        public List<PairModel> OUTLET_SUB_TYPE { get; set; }
        public List<PairModel> BRANDWISE { get; set; }
    }
}
