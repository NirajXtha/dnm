using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.sales.Module.Models
{
    public class SalesMoniterViewModel
    {
        public int totalSalesOrder { get; set; } = 0;
        public int approvedSalesOrder { get; set; } = 0;
        public int vehiclerRegistration { get; set; } = 0;
        public int loadingSlipGenerate { get; set; } = 0;
        public int loadedVechicleOut { get; set; } = 0;
        public int pendingForDispatch { get; set; } = 0;
        public int distributionPurchaseorder { get; set; } = 0;
        public int vechicleIn { get; set; } = 0;
        public int totalBiil { get; set; } = 0;
    }
    
}