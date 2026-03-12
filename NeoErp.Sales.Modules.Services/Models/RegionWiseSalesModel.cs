using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class RegionWiseSalesModel
    {
        public string RegionName { get; set; }
        public decimal Qty { get; set; }
        public decimal SalesPercent { get; set; }
    }
    public class ProductWiseGpModel
    {
        public string Product { get; set; }
        public decimal Qty { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal GpPercent { get; set; }
    }

    public class WeeklyExpenseAnalysis
    {
        public string ACC_EDESC { get; set; }
        public DateTime? VOUCHER_DATE { get; set; }
        public decimal? AMOUNT { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string PRE_ACC_CODE { get; set; }
    }

    public class WeeklyVendorPaymentAnalysis
    {
        public string ACC_EDESC { get; set; }
        public DateTime? VOUCHER_DATE { get; set; }
        public decimal? AMOUNT { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string PRE_ACC_CODE { get; set; }
    }

    public class RegionProductGpModel
    {
        public RegionProductGpModel()
        {
            Region = new List<RegionWiseSalesModel>();
            ProductGp = new List<ProductWiseGpModel>();
        }
        public List<RegionWiseSalesModel> Region { get; set; }
        public List<ProductWiseGpModel> ProductGp { get; set; }
    }
}