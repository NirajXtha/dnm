using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class DistSalesReturnViewModel
    {
        public string ENTITY_TYPE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC{ get; set; } 
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string RETURN_NO { get; set; }
        public DateTime RETURN_DATE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string AREA { get; set; }
        public string CONTACT_NUMBER { get; set; }
        public string CONDITION { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string SERIOUSNESS { get; set; }
        public string COMPLAIN_TYPE { get; set; }
        public string COMPLAIN { get; set; }
        public string SUGGESTION { get; set; }
        public string SALES_RETURN { get; set; }
        public string REMARKS_DIST { get; set; }
        public string REMARKS_ASM { get; set; }
        public string APPROVED_FLAG { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? EXP_DATE { get; set; }
        public DateTime? MFD_DATE { get; set; }

        public string ASM_NAME { get; set; }

        public List<DistSalesReturnItemViewModel> ReturnItemList = new List<DistSalesReturnItemViewModel>();

    }

    public class DistSalesReturnItemViewModel
    {
        public string ENTITY_TYPE { get; set; }
        public string RETURN_NO { get; set; }
        public string COMPANY_CODE { get; set; }
        public string SYNC_ID { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string MBF_DATA { get; set; }
        public string QUANTITY { get; set; }
        public DateTime EXP_DATE { get; set; }
        public DateTime MFD_DATE { get; set; }
        public string BATCH_NO { get; set; }
        public string SHIPPING_CONTACT { get; set; }
        public string CONDITION { get; set; }
        public string COMPLAIN_TYPE { get; set; }
        public string SERIOUSNESS { get; set; }
        public string REMARKS_DIST { get; set; }
        public string REMARKS_ASM { get; set; }
        public string BILLING_NAME { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
    }

    public class DSRResponse
    {
        public string UpdateFlag { get; set; }

        public string UpdateResponse { get; set; }
    }
}
