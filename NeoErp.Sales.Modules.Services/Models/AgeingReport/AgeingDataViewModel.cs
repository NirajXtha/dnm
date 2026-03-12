using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AgeingReport
{
    public class AgeingDataViewModel
    {
        public AgeingDataViewModel()
        {
            this.RangeColumnData = new List<AgeingColumnRangeData>();
        }
        public string Description { get; set; }
        public string SubCode { get; set; }
        public decimal? Total { get; set; }
        public int id { get; set; }
        public int? parentId { get; set; } = null;
        public bool hasChildren { get; set; } = false;
        public string AccCode { get; set; }
        public List<AgeingColumnRangeData> RangeColumnData { get; set; }

        public class AgeingColumnRangeData
        {
            public string ColumnRangeName { get; set; }
            public int OrderBy { get; set; }
            public decimal? NetAmount { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string Descriptions { get; set; }
            public string Branch { get; set; }
            public string colorCode { get; set; }
        }
        
    }

    public class testAgeing
    {
        public decimal? CR_AMOUNT { get; set; }
        public decimal? DR_AMOUNT1 { get; set; }
        public decimal? DR_AMOUNT2 { get; set; }
        public decimal? DR_AMOUNT3 { get; set; }
        public decimal? DR_AMOUNT4 { get; set; }
        public decimal? DR_AMOUNT0 { get; set; }


        //public decimal? DR_AMOUNT0 { get; set; }
        //public decimal? DR_AMOUNT1 { get; set; }
        //public decimal? DR_AMOUNT2 { get; set; }
        //public decimal? DR_AMOUNT3 { get; set; }
        //public decimal? DR_AMOUNT4 { get; set; }
    }
}
