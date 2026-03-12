using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Distribution.Service.Model
{
    public class ItemModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string BRAND_NAME { get; set; }
        public string UNIT { get; set; }
        public string MU_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string CONVERSION_UNIT { get; set; }
        public string CONVERSION_FACTOR { get; set; }
        public string SALES_RATE { get; set; }
        public string APPLY_DATE { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string SP_CODE { get; set; }
        public string IMAGE_FILE_NAME { get; set; }
    }

    public class VisitImageModel
    {
        public string IMAGE_TITLE  {get;set;}
        public string IMAGE_DESC   {get;set;}
        public string FULL_NAME   {get;set;}
        public int? CATEGORYID   {get;set;}
        public string CATEGORY_EDESC { get; set; }
        public string SP_CODE      {get;set;}
        public string ENTITY_NAME { get; set; }
        public string ENTITY_CODE  {get;set;}
        public string TYPE         {get;set;}
        public DateTime? UPLOAD_DATE  {get;set;}
        public string UPLOAD_DATEString { get; set; }
        public string LATITUDE     {get;set;}
        public string LONGITUDE    {get;set;}
        public string COMPANY_CODE {get;set;}
        public string BRANCH_CODE  {get;set;}
        public string SYNC_ID      {get;set;}
        public int? IMAGE_CODE   {get;set;}
        public string IMAGE_NAME { get; set; }
    }

    public class NotVisitedCustomerModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_TYPE { get; set; }
    }


    public class ItemFilterModel
    {
        public string distributor { get; set; }
        public string Reseller { get; set; }
    }

    public class FreeQuantityModel
    {
        public string item_code { get; set; }
        public decimal qty { get; set; }
        public string unit { get; set; }
        public decimal free_qty { get; set; }
        public string free_unit { get; set; }
        public int main_serial { get; set; }  // Serial number from IP_ITEM_UNIT_SETUP
        public int free_serial { get; set; }  // Serial number from IP_ITEM_UNIT_SETUP
    }
}
