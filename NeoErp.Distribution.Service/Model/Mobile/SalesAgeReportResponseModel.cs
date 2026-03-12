using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class SalesAgeReportResponseModel
    {
        public SalesAgeReportResponseModel()
        {
            sales = new Dictionary<string, string>();
            age = new Dictionary<string, string>();
        }
        public Dictionary<string, string> sales { get; set; }
        public Dictionary<string, string> age { get; set; }
    }
    public class SalesReportResponseModel
    {
        public string MONTH { get; set; }
        public string AMOUNT { get; set; }
        public string QUANTITY { get; set; }
        public string MONTHINT { get; set; }
    }
    public class AgingDateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AgingReportModel
    {
        public string SUB_CODE { get; set; }
        public string SUB_EDESC { get; set; }
        public decimal? CR_AMOUNT { get; set; }
        public decimal? DR_AMOUNT0 { get; set; }
        public decimal? DR_AMOUNT1 { get; set; }
        public decimal? DR_AMOUNT2 { get; set; }
        public decimal? DR_AMOUNT3 { get; set; }
        public decimal? DR_AMOUNT4 { get; set; }
    }
}