using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class SchemeModel: CommonRequestModel
    {
        public SchemeModel()
        {
            Answers = new List<GeneralSaveModel>();
        }
        public int CONTRACT_CODE { get; set; }
        public string RESELLER_CODE { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public string END_USER { get; set; }
        public string DIVISION_CODE { get; set; }
        public string BRAND_CODE { get; set; }
        public string GIFT_ITEM_CODE { get; set; }
        public int GIFT_ITEM_QUANTITY { get; set; }
        public DateTime HANDOVER_DATE { get; set; }
        public List<GeneralSaveModel> Answers { get; set; }
    }
}
