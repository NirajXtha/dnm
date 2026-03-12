using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
     public class VistitedPlanModel
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string CONTACT { get; set; }
        public string ADDRESS { get; set; }
        public string ROUTE_CODE { get; set; }
        public DateTime? ASSIGN_DATE { get; set; }
        public string ROUTE_NAME { get; set; }
        public string AREA_NAME { get; set; }
        public decimal? ORDER_NO { get; set; }
        public string COMPANY_CODE { get; set; }
        public string TYPE { get; set; }
        public string PARENT_DISTRIBUTOR_CODE { get; set; }
        public string PARENT_DISTRIBUTOR_NAME { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string LAST_VISIT_DATE { get; set; }
        public string LAST_VISIT_BY { get; set; }
        public string LAST_VISIT_STATUS { get; set; }
        public string IS_VISITED { get; set; }
        public string REMARKS { get; set; }
        public string TypeMr { get; set; } = "DType";
        public string LAST_VISIT_BY_CODE { get; set; }


    }
    public class VisitedPlainList
    {
        public List<VistitedPlanModel> distributor { get; set; }
        public List<VistitedPlanModel> reseller { get; set; }
        public List<MrVisitedTrackingModel> MrVisited { get; set; }
        public List<VistitedPlanModel> dealer { get; set; }
        public VisitedPlainList()
        {
            distributor = new List<VistitedPlanModel>();
            reseller = new List<VistitedPlanModel>();
            MrVisited = new List<MrVisitedTrackingModel>();
            dealer = new List<VistitedPlanModel>();
        }
    }

    public class MrVisitedTrackingModel
    {
        public string SP_CODE { get; set; }
        public string SUBMIT_DATE { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string TypeMr { get; set; } = "MrType";
        public string Track_type { get; set; }
        public string FILENAME { get; set; }        
        public int? CATEGORYID { get; set; }
        public string ImageFullPath { get; set; }

        public string GROUP_EDESC { get; set; }
        public string ATN_IMAGE { get; set; }
        public DateTime? CUR_POS_DATE { get; set; }
        public DateTime? CUR_DATE { get; set; }
        public string CUR_LATITUDE { get; set; }
        public string CUR_LONGITUDE { get; set; }
        public DateTime? ATN_DATE { get; set; }
        public string ATN_LATITUDE { get; set; }
        public string ATN_LONGITUDE { get; set; }
        public DateTime? EOD_DATE { get; set; }
        public string EOD_LATITUDE { get; set; }
        public string EOD_LONGITUDE { get; set; }
        public int? TARGET { get; set; }
        public int? VISITED { get; set; }
        public int? NOT_VISITED { get; set; }
        public int? PJP_PRODUCTIVE { get; set; }
        public int? PJP_NON_PRODUCTIVE { get; set; }
        public int? NPJP_PRODUCTIVE { get; set; }
        public int? PERCENT_EFFECTIVE_CALLS { get; set; }
        public decimal? PJP_TOTAL_QUANTITY { get; set; }
        public decimal? NPJP_TOTAL_QUANTITY { get; set; }
        public decimal? PJP_TOTAL_AMOUNT { get; set; }
        public decimal? NPJP_TOTAL_AMOUNT { get; set; }
        public decimal? OUTLET_ADDED { get; set; }

        public string ENTITY_CODE { get; set; }
        public string ENTITY_NAME { get; set; }
        public string IS_VISITED { get; set; }
        public string GET_ORDER { get; set; } = "N";
        public string LAST_VISIT_STATUS { get; set; }
        public string LAST_VISIT_BY { get; set; }
        public string LAST_VISIT_DATE { get; set; }


    }
    public class EmployeeWisePerformance
    {
        public EmployeeWisePerformance()
        {
            BRANDS = new List<PairModel>();
        }
        public string ENTITY_CODE { get; set; }
        public string ENTITY_NAME { get; set; }
        public string P_CONTACT_NO { get; set; }
        public string ADDRESS { get; set; }
        public string TRACK_TYPE { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string JOURNEY_PLAN { get; set; }
        public string LONGITUDE { get; set; }
        public string LATITUDE { get; set; }
        public string LAST_VISIT_DATE { get; set; }
        public string LAST_VISIT_BY { get; set; }
        public string LAST_VISIT_STATUS { get; set; }
        public string IS_VISITED { get; set; }
        public string REMARKS { get; set; }
        public DateTime? ASSIGN_DATE { get; set; }
        public DateTime? VISIT_DATE { get; set; }
        public string VISIT_TIME { get; set; }
        //added by Dushant
        public string SP_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string VISITED { get; set; }
        public string COMPANY_CODE { get; set; }
        public string STATUS { get; set; }
        public List<PairModel> BRANDS { get; set; }

        public string TYPE_EDESC { get; set; }
        public string SUBTYPE_EDESC { get; set; }
    }

    public class EmpBrandWiseModel
    {
        public string BRAND_NAME { get; set; }
        public int TOTAL_QUANTITY { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public string MU_CODE { get; set; }
        public decimal TOTAL_CASE { get; set; }
    }

    public class MrVisitedTrackingRecord
    {
        public string SP_CODE { get; set; }
        public DateTime? ACTIVITY_TIME { get; set; }
        public  string LATITUDE { get; set; }

        public string LONGITUDE { get; set; }
        public string VISITED { get; set; }
        public string REMARKS { get; set; }
        public string ENTITY_CODE { get; set; }
        public string ENTITY_NAME { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string SORT_RECORD { get; set; }
        public string TRACK_TYPE { get; set; }


        public string IsAssigned { get; set; }
        public string GET_ORDER { get; set; } = "N";

    }


    public class MRVisitedTrackingList
    {
       public List<MrVisitedTrackingRecord> VisitActivityList { get; set; }
       public List<MrVisitedTrackingModel> AssignRouteList { get; set; }
       public List<MrVisitedTrackingModel> VisitEffectiveCalls { get; set; }


        public MRVisitedTrackingList()
        {
            VisitActivityList = new List<MrVisitedTrackingRecord>();
            AssignRouteList = new List<MrVisitedTrackingModel>();
            VisitEffectiveCalls = new List<MrVisitedTrackingModel>();
        }
    }

    public class SalesDistrictModel
    {
        public string customer_code { get; set; }
        public decimal? sales { get; set; }
        public string district { get; set; }
        
    }
    public class SalesDistrictSum
    {
        public string district { get; set; }
        public string Amount { get; set; }
    }
    public class SalesDistrictViewModel
    {
        public Dictionary<string, areaboundary>  boundary
 { get; set; }

        public Dictionary<string, decimal?> sales { get; set; }
        public SalesDistrictViewModel()
        {
            boundary = new Dictionary<string, areaboundary>();
        }

    }

    public class areaboundary
    {
        public string code { get; set; }
        public string type { get; set; } = "path";
        public string data { get; set; }
        public string description { get; set; }
        public string district { get; set; }
    }
    public class AreaBoundaryViewModel
    {
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string ZONE_CODE { get; set; }
        public string DISTRICT_CODE { get; set; }
        public string VDC_CODE { get; set; }
        public string GEO_CODE { get; set; }
        public string REG_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public string AREA_PATH { get; set; }
        public string REMARKS { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
    }

    public class EmployeeActivityDetail
    {
        public DateTime? SUBMITDATE { get; set; }
        public string SALESPERSON { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string VISITTYPE { get; set; }
        public string TOTALAMOUNT { get; set; }

        public string REMARKS { get; set; }
    }



    public class VisitSummaryViewModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_TYPE { get; set; }

        public string AREA_NAME { get; set; }
        public string OUTLET_TYPE { get; set; }
        public string OUTLET_SUBTYPE { get; set; }
        public string GROUP_NAME { get; set; }

        public string CUST_LAT { get; set; }
        public string CUST_LONG { get; set; }

        public string Assigned_ROUTE { get; set; }
        public string Assigned_ROUTE_NAME { get; set; }
        public string Assigned_To { get; set; }
        public string Assigned_Employee { get; set; }
        public DateTime? ASSIGN_DATE { get; set; }
        public string MITI { get; set; }

        public string Visit_ROUTE_CODE { get; set; }
        public string Visited_ROUTE { get; set; }
        public string Visit_Type { get; set; }
        public string VISIT_BY { get; set; }
        public DateTime? Visit_Date { get; set; }
        public DateTime? Visit_Time { get; set; }
        public string VISIT_LAT { get; set; }
        public string VISIT_LONG { get; set; }
        
        public string Company_Code { get; set;}
        public string REMARKS { get; set; }
        public bool HAS_IMAGE { get; set; } = false;
        public string VISITED_BY { get; set; }

    }



    public class VisitSummaryModel
    {
        public List<VisitSummaryViewModel> VisitSummaryViewModel { get; set; }
        public decimal total { get; set; }
        public VisitSummaryModel()
        {
            VisitSummaryViewModel = new List<VisitSummaryViewModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
    public class CollectionviewModel
    {
        public List<CollectionModel> collectionViewModels { get; set; }
        public decimal total { get; set; }
        public CollectionviewModel()
        {
            collectionViewModels = new List<CollectionModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }


    public class FormSetupModel
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
    }
    public class CustomerIModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
    }
    public class DistAreaModel
    {
        public string DISTRIBUTOR_CODE { get; set; }
        public string DISTRIBUTOR_NAME { get; set; }
        public string REGD_OFFICE_ADDRESS { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public int? GROUPID { get; set; }
    }
    public class ResellerAreaModel
    {
        public string RESELLER_CODE { get; set; }
        public string RESELLER_NAME { get; set; }
        public string REGD_OFFICE_ADDRESS { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
    }
    public class HoardingAreaModel
    {
        public string ENTITY_CODE { get; set; }
        public string ENTITY_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
    }

    public class PartyTypeAreaModel
    {
        public string DEALER_CODE { get; set; }
        public string DEALER_NAME { get; set; }
        public string REGD_OFFICE_ADDRESS { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
    }


    public class AreaDistResellerModel
    {
        public List<DistAreaModel> distributors { get; set; }
        public List<ResellerAreaModel> resellers { get; set; }
        public List<PartyTypeAreaModel> dealers { get; set; }
        public List<HoardingAreaModel> hoardings { get; set; }
        public AreaDistResellerModel()
        {
            distributors = new List<DistAreaModel>();
            resellers = new List<ResellerAreaModel>();
            dealers = new List<PartyTypeAreaModel>();
            hoardings = new List<HoardingAreaModel>();
        }

    }

    public class DistCustomerInfo
    {
        public List<PhotoInfoModel> photoInfo { get; set; }
        public DistCustomerInfoModel distCustomerInfo { get; set; }
        public DistCustomerInfo()
        {
            photoInfo = new List<PhotoInfoModel>();        
        }
    }


    public class PhotoInfoModel
    {
        public string FILENAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string URL { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string ENTITY_CODE { get; set; }
        public string MEDIA_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string USER_NAME { get; set; }
    }



    public class DistCustomerInfoModel
    {
        public DistCustomerInfoModel()
        {
            ITEMS = new List<LastPoItems>();
        }
        public string TYPE { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT_NO { get; set; }
        public string PAN_NO { get; set; }
        public decimal TOTAL_SALES { get; set; }
        public decimal MONTH_SALES { get; set; }
        public decimal TOTAL_COLLECTION { get; set; }
        public decimal REMAINING_PO { get; set; }
        public List<LastPoItems> ITEMS { get; set; }
    }
    public class LastPoItems
    {
        public string ORDER_DATE { get; set; }
        public string ITEM_EDESC { get; set; }
        public int QUANTITY { get; set; }
        public decimal TOTAL_PRICE { get; set; }
    }

    public class VisitTimeModel
    {
        public string SP_CODE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string MITI { get; set; }
        public string UPDATE_TIME { get; set; }
        public string CUSTOMER_TYPE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string FULL_NAME { get; set; }
        public string REMARKS { get; set; }
        public string LAT_LON { get; set; }
        public TimeSpan? SPENT_TIME { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
    }
}