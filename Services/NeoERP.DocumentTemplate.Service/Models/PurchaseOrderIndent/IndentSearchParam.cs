using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models.PurchaseOrderIndent
{
    public class IndentSearchParam
    {
        public string Document { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public string IndentFilter { get; set; } = string.Empty;
    }
}
