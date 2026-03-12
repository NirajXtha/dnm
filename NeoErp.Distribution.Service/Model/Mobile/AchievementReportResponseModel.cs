using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class AchievementReportResponseModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRAND_NAME { get; set; }
        public int? NEPALI_MONTH { get; set; }
        public decimal? TARGET_QUANTITY { get; set; }
        public decimal? TARGET_VALUE { get; set; }
        public decimal? QUANTITY_ACHIVE { get; set; }
        public decimal? ACHIVE_VALUE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string NEPALI_MONTHINT { get; set; }
    }
}
