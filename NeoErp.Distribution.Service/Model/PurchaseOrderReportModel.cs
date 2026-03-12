using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
  public  class PurchaseOrderReportModel
    {
        public int? ORDER_NO { get; set; }
        public DateTime ORDER_DATE { get; set; }
        public string MITI { get; set; }
        public string RESELLER_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string MU_CODE { get; set; }
        public int? QUANTITY { get; set; }
        public int? UNIT_PRICE { get; set; }
        public int? TOTAL_PRICE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string APPROVED_FLAG { get; set; }
        public string REJECT_FLAG { get; set; }
        public string DELETED_FLAG { get; set; }
        public string DISPATCH_FLAG { get; set; }
        public string ACKNOWLEDGE_FLAG { get; set; }
        public int? GRAND_APPROVE_QUENTITY { get; set; }
        public string GROUP_EDESC { get; set; }
        public int? TOTAL_APPROVE_AMT { get; set; }
        public int? TOTAL_QUANTITY { get; set; }

        public string CUSTOMER_EDESC { get; set; }
        public string RESELLER_NAME { get; set; }
        public string ORDER_ENTITY { get; set; }
        public string EMPLOYEE_EDESC { get; set; }

        public string ApproveRemarks { get; set; }

        public decimal? APPROVEQTY { get; set; }
        public string REMARKS_REVIEW { get; set; }
        public string WHOLESELLER_EDESC { get; set; }
        public string DISPATCH_FROM { get; set; }

        public string IsDeleted { get; set; } = "update";
    }
}
