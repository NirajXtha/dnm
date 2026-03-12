using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class LcLogisticPlanModel
    {
        public LC_LOGISTIC_PLAN LC_LOGISTIC_PLAN { get; set; }
        public LC_ENTRY_CURRENCY LC_ENTRY_CURRENCY { get; set; }
        public List<LC_LOGISTIC_PLAN_ITEM> LC_LOGISTIC_PLAN_ITEM { get; set; }
        public List<LC_LOGISTIC_PLAN_CONTAINER> LC_LOGISTIC_PLAN_CONTAINER { get; set; }
        public List<ItemDetails> ItemDetails { get; set; }
        public List<LC_PLAN_CONTAINER_LIST> LC_PLAN_CONTAINER_LIST { get; set; }

      
    }

    public class LC_PLAN_CONTAINER_LIST {
        public string CONTAINER_CODE { get; set; }
        public string LOAD_TYPE { get; set; }
        public string CARRIER_NUMBER { get; set; }
        public string CONTAINER_EDESC { get; set; }
        public string LOT_NO { get; set; }
        public string PLAN_CONTAINER_CODE { get; set; }
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string SHIPPING_TYPE { get; set; }
        public int ROWNUMBER { get; set; }
        public int CONTAINER_ROWNUMBER { get; set; }
    }
  

    public class LC_LOGISTIC_PLAN
    {
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string LC_TRACK_NO { get; set; }
        public string SNO { get; set; }
        public string CONSIGNEE_NAME { get; set; }
        public string CONSIGNEE_ADDRESS { get; set; }
        public string NOTIFY_APPLICANT_NAME { get; set; }
        public string NOTIFY_APPLICANT_ADDRESS { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SHIPPING_TYPE { get; set; }
        public string AIR_PACK { get; set; }
        public string LOT_NO { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string LC_NUMBER { get; set; }
        public string CARRIER_COUNT { get; set; }
      



    }

    public class LC_ENTRY_CURRENCY
    {
        public string EXCHANGE_RATE { get; set; }
        public string CURRENCY_CODE { get; set; }
    }

    public class LC_LOGISTIC_PLANVVIEWMODEL
    {
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string LC_TRACK_NO { get; set; }
        //public string SNO { get; set; }
        public string CONSIGNEE_NAME { get; set; }
        public string CONSIGNEE_ADDRESS { get; set; }
        public string NOTIFY_APPLICANT_NAME { get; set; }
        public string NOTIFY_APPLICANT_ADDRESS { get; set; }
        public string REMARKS { get; set; }
        //public string COMPANY_CODE { get; set; }
        //public string BRANCH_CODE { get; set; }
        //public string CREATED_BY { get; set; }
        //public string CREATED_DATE { get; set; }
        //public string LAST_MODIFIED_BY { get; set; }
        //public string LAST_MODIFIED_DATE { get; set; }
        //public string APPROVED_BY { get; set; }
        //public string APPROVED_DATE { get; set; }
        //public string DELETED_FLAG { get; set; }
        //public string SHIPPING_TYPE { get; set; }
        //public string AIR_PACK { get; set; }
        public string LOT_NO { get; set; }
        //public string SUPPLIER_EDESC { get; set; }
        public string LC_NUMBER { get; set; }

        public List<LC_LOGISTIC_PLAN_CONTAINER> LC_LOGISTIC_PLAN_CONTAINER { get; set; }
        //public string ACT_BOOKING_DATE { get; set; }
        //public string ACT_LOADING_DATE { get; set; }



    }

    public class LC_LOGISTIC_PLANITEMLIST
    {
        public string LC_TRACK_NO { get; set; }
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string LOT_NO { get; set; }
        public string LC_LOGISTIC_PLAN_ITEM_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string MU_CODE { get; set; }
        public string QUANTITY { get; set; }
        public string HS_CODE { get; set; }
        public string COUNTRY_EDESC { get; set; }
        public string TOTAL_QUANTITY { get; set; }
    }

    public class LC_LOGISTIC_PLANCONTAINERLIST
    {
        public string SHIPPING_TYPE { get; set; }
        public string EST_BOOKING_DATE { get; set; }
        public string EST_LOADING_DATE { get; set; }
        public string ACT_BOOKING_DATE { get; set; }
        public string ACT_LOADING_DATE { get; set; }
        public string CONTAINER_EDESC { get; set; }
        public string FROM_LOCATION { get; set; }
        public string TO_LOCATION { get; set; }
    }

    public class LC_LOGISTIC_PLANCONTAINERDATE
    {
        public string ACT_BOOKING_DATE { get; set; }
        public string ACT_LOADING_DATE { get; set; }
    
       // public Nullable<System.DateTime> ACT_BOOKING_DATE { get; set; }
       // public Nullable<System.DateTime> ACT_LOADING_DATE { get; set; }

    }
    public class LC_LOGISTIC_PLAN_CONTAINER
    {
        public string PLAN_CONTAINER_CODE { get; set; }
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string SNO { get; set; }
        public string LC_TRACK_NO { get; set; }
        public string CONTAINER_CODE { get; set; }
        public string LOAD_TYPE { get; set; }
        public string FROM_LOCATION_CODE { get; set; }
        public string TO_LOCATION_CODE { get; set; }
        public string EST_BOOKING_DATE { get; set; }
        public string EST_LOADING_DATE { get; set; }
        public string ACT_BOOKING_DATE { get; set; }
        public string ACT_LOADING_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string LOT_NO { get; set; }
        public string ROUTE_NO { get; set; }
        public string SHIPPING_TYPE { get; set; }
        public string AIR_PACK { get; set; }
        public string TO_LOCATION_EDESC { get; set; }
        public string FROM_LOCATION_EDESC { get; set; }
        public string CARRIER_NUMBER { get; set; }
        public int ROWNUMBER { get; set; }


    }
    public class LC_LOGISTIC_PLAN_ITEM
    {
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string LC_TRACK_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SHIPPMENT_QUANTITY { get; set; }
    }

       public class LogisticContainerPlan
    {
        public string SNO { get; set; }
        public string FROM_LOCATION_CODE { get; set; }
        public string TO_LOCATION_CODE { get; set; }
        public string FROM_LOCATION_EDESC { get; set; }
        public string TO_LOCATION_EDESC { get; set; }
        public string EST_BOOKING_DATE { get; set; }
        public string EST_LOADING_DATE { get; set; }
        public string SHIPPING_TYPE { get; set; }
        public int LOT_NO { get; set; }
        public int ROUTE_NO { get; set; }
        public string LC_TRACK_NO { get; set; }
        public string LOGISTIC_PLAN_CODE { get; set; }

    }






}