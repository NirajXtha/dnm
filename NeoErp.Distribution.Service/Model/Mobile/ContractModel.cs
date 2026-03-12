using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class ContractModel
    {
        public ContractModel()
        {
            General = new List<GeneralModel>();
        }
        public int CONTRACT_CODE { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string CONTRACT_EDESC { get; set; }
        public string CONTRACT_NDESC { get; set; }
        public string BRAND_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string GIFT_ITEM_CODE { get; set; }
        public string RESELLER_CODE { get; set; }
        public string BRANDING_TYPE { get; set; }
        public int? SPROVIDER_CODE { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public string AREA_CODE { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string AMOUNT_TYPE { get; set; }
        public decimal? AMOUNT { get; set; }
        public DateTime? NEXT_PAYMENT_DATE { get; set; }
        public DateTime? PAYMENT_DATE { get; set; }
        public decimal? ADVANCE_AMOUNT { get; set; }
        public string CONTRACTOR_NAME { get; set; }
        public string CONTRACTOR_ADDRESS { get; set; }
        public string CONTRACTOR_EMAIL { get; set; }
        public string CONTRACTOR_PHONE { get; set; }
        public string CONTRACTOR_MOBILE { get; set; }
        public string CONTRACTOR_DESIGNATION { get; set; }
        public string CONTRACTOR_PAN_NO { get; set; }
        public string CONTRACTOR_VAT_NO { get; set; }
        public string OWNER_NAME { get; set; }
        public string OWNER_ADDRESS { get; set; }
        public string OWNER_EMAIL { get; set; }
        public string OWNER_PHONE { get; set; }
        public string OWNER_MOBILE { get; set; }
        public string OWNER_COMPANY_NAME { get; set; }
        public string OWNER_PAN_NO { get; set; }
        public string OWNER_VAT_NO { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public string CONTRACT_FILES { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public int? SET_CODE { get; set; }
        public string IS_ROUTE_PLAN { get; set; }
        public List<GeneralModel> General { get; set; }
    }
}
