using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class CreateResellerModel : CommonRequestModel
    {
        public CreateResellerModel()
        {
            contact = new List<ContactModel>();
        }
        public string reseller_code { get; set; }
        public string reseller_name { get; set; }
        public string area_code { get; set; }
        public string address { get; set; }
        public string pan { get; set; }
        public string wholeseller { get; set; }
        public string type_id { get; set; }
        public string subtype_id { get; set; }
        public string Group_id { get; set; }
        public string email { get; set; }
        public string distributor_code { get; set; }
        public string wholeseller_code { get; set; }
        public string Reseller_contact { get; set; }
        public List<ContactModel> contact { get; set; }
        public string ROUTE_CODE { get; set; }
    }
    public class ContactModel
    {
        public string contact_suffix { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string designation { get; set; }
        public string primary { get; set; }
        public string Sync_Id { get; set; }
    }

    public class ResellerEntityModel
    {
        public string RESELLER_CODE { get; set; }
        public string ENTITY_CODE { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string COMPANY_CODE { get; set; }
    }
    public class BatteryModel
    {
        public string BATTERY { get; set; }
        public string SP_CODE { get; set; }
    }

    // add farmer
    public class FarmerModel
    {
        public string FARMER_EDESC { get; set; }
        public string FARM_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT_NO { get; set; }
        public string AREA_CODE { get; set; }
        public string FARM_LONGITUDE { get; set; }
        public string FARM_LATITUDE { get; set; }
        public string FARM_AREA { get; set; }
        public string FARM_CROPS { get; set; }
        public string EXPERIENCE { get; set; }
        public string REMARKS { get; set; }
        public List<DealersList> DEALERS { get; set; }
        public List<DealersList> SUB_DEALERS { get; set; }

    }

    public class DealersList
    {
        public string DEALER_NAME { get; set; }
        public string DEALER_CODE { get; set; }
    }

    //add doctor
    public class DoctorModel
    {
        public string NAME { get; set; }
        public string AREA_CODE { get; set; }
        public string DOB { get; set; }
        public string CLASS { get; set; }
        public string DEGREE_CODE { get; set; }
        public string DEGREE_EDESC { get; set; }
        public string SPECIALITY_CODE { get; set; }
        public string SPECIALITY_EDESC { get; set; }
        public string CONTACT_NO { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_C0DE { get; set; }
        public string USER_ID { get; set; }
        public string EMAIL { get; set; }
        public List<string> RESELLER_CODE { get; set; }

    }

    //models for dropdowns
    public class DoctorNameId
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string AREA { get; set; }
    }

    public class UserNameID
    {
        public int ID { get; set; }
        public string NAME { get; set; }

    }

    public class AreaNameCode
    {
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
    }

    public class AreaModel
    {
        public List<string> AREA_CODES { get; set; }
    }
    public class WorkingType
    {
        public string TYPE_CODE { get; set; }
        public string WORKING_TYPE { get; set; }
        public string IS_REQUIRED { get; set; }

    }

    //save visit plans
    public class DoctorVisitPlanList
    {
        public string spcode { get; set; }
        public string USER_ID { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public List<DoctorVisitPlanData> DataList { get; set; }
    }

   

    public class DoctorVisitPlanData
    {
        public string PLAN_DATE { get; set; }
        public string WORKING_TYPE { get; set; }

        public List<Doctor> DOCTORS { get; set; }
        public List<Reseller> RESELLERS { get; set; }
        public string REMARKS { get; set; }
    }

    public class Doctor
    {
        public string AREA_CODE { get; set; }
        public string DOCTOR_CODE { get; set; }
    }

    public class Reseller
    {
        public string AREA_CODE { get; set; }
        public string RESELLER_CODE { get; set; }
    }


    // get visit plan data 

    public class GetVisitPlanList
    {
        public string USER_ID { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public List<GetVisitPlanData> DataList { get; set; }
    }

    public class GetVisitPlanData
    {
        public string PLAN_DATE { get; set; }
        public string IS_APPROVED { get; set; }
        public string ROUTE_CODE { get; set; }
        public string WORKING_TYPE { get; set; }
        public string TYPE_CODE { get; set; }
        public List<AreaNameCode> AREAS { get; set; }
        public List<GetDoctor> DOCTORS { get; set; }
        public List<GetReseller> RESELLERS { get; set; }
        public string REMARKS { get; set; }
    }

    public class GetDoctor
    {
        public string ROUTE_ENTITY_CODE { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string AREA_CODE { get; set; }
        public string DOCTOR_CODE { get; set; }
        public string DOCTOR_NAME { get; set; }

    }

    public class GetReseller
    {
        public string ROUTE_ENTITY_CODE { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string AREA_CODE { get; set; }
        public string RESELLER_CODE { get; set; }
        public string RESELLER_NAME { get; set; }

    }

    public class selfVisit
    {
        public string sp_code { get; set; }
        public string plan_date { get; set; }
    }
    public class approveModel
    {
        public string sp_code { get; set; }
        public string user_id { get; set; }
        public List<string> PLAN_CODES { get; set; }

    }

    public class visitUpdate
    {
        public string ROUTE_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string sp_code { get; set; }
        public List<routeEntity> ROUTE_ENTITY { get; set; }
    }

    public class routeEntity
    {
        public string RE_CODE { get; set; }
        public string ENTITY_CODE { get; set; }
        public string ENTITY_TYPE { get; set; }

        public string AMOUNT { get; set; }
        public string REMARKS { get; set; }

    }

    public class LastConvoData
    {
        public string ENTITY_CODE { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string SP_CODE { get; set; }
        public string DATE { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
    }

    public class DriverModel
    {
        public string TRANSACTION_NO { get; set; }
        public string CHALAN_NO { get; set; }
        public string BILL_NO { get; set; }
        public string REFERENCE_NO { get; set; }
        public string REFERENCE_FORM_CODE { get; set; }
        public string VEHICLE_NAME { get; set; }
        public DateTime? TRANSACTION_DATE { get; set; }
        public string REMARKS { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string ACCESS_FLAG { get; set; }
        public string READ_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string SYN_ROWID { get; set; }
        public string DRIVER_NAME { get; set; }
        public string DRIVER_MOBILE_NO { get; set; }
        public string TRANSPORTER_MOBILE_NO { get; set; }
        public string DRIVER_LICENCE_NO { get; set; }
        public string IN_TIME { get; set; }
        public string OUT_TIME { get; set; }
        public string LOAD_IN_TIME { get; set; }
        public string LOAD_OUT_TIME { get; set; }
        public string TOTAL_VEHICLE_HR { get; set; }
        public decimal? TEAR_WT { get; set; }
        public decimal? GROSS_WT { get; set; }
        public decimal? NET_WT { get; set; }
        public decimal? QUANTITY { get; set; }
        public string ACCESS_BY { get; set; }
        public DateTime? ACCESS_DATE { get; set; }
        public string DESTINATION { get; set; }
        public string BROKER_NAME { get; set; }
        public string VEHICLE_OWNER_NAME { get; set; }
        public string VEHICLE_OWNER_NO { get; set; }
        public string TRANSPORT_NAME { get; set; }
        public DateTime? VEHICLE_OUT_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string WB_SLIP_NO { get; set; }
        public DateTime? VEHICLE_IN_DATE { get; set; }
        public string TRANSPORTER_CODE { get; set; }
        public string STATUS { get; set; }
        public string TYPE { get; set; }
        public string BILTY_NUMBER { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
    }
    public class AssignedDriverModel
    {
        public string TRANSPORTER_MOBILE_NO { get; set; }
        public string TRANSPORT_NAME { get; set; }
        public string TRANSPORTER_CODE { get; set; }
        public string BILTY_NUMBER { get; set; }
        public string TYPE { get; set; }
        public string STATUS { get; set; }
        public string COMPANY_CODE { get; set; }
        public List<DriverModel> DATA {  get; set; }
    }
    public class DeliveryStatus
    {
        public int? ID { get; set;  }
        public string STATUS_CODE { get; set; }
        public string STATUS_NAME { get; set; }
    }
    public class DeliveryAssignModel
    {
        public string NUMBER { get; set; }
        public string COMPANY_CODE { get; set; }
        public string ASSIGN_TO { get; set; }
        public string TYPE { get; set; }
        public string BILTY_NUMBER { get; set; }
        public string RECEIVER_NAME { get; set; }
        public string RECEIVER_CONTACT { get; set; }
        public string REMARKS { get; set; }
    }
    public class DeliveryUpdateModel
    {
        public int? ID { get; set; }
        public string NUMBER { get; set; }
        public string TYPE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string STATUS { get; set; }
        public string CHALAN_NO { get; set; }
        public string MOBILE_NO { get; set; }
        public string TRANSACTION_NO { get; set; }
        public string TRANSPORTER_CODE { get; set; }
        public string BILTY_NUMBER { get; set; }
    }

    public class FarmListModel2
    {
        public string FARMER_ID { get; set; }
        public string FARMER_EDESC { get; set; }
        public string FARM_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT_NO { get; set; }
        public string AREA_CODE { get; set; }
        public string FARM_LONGITUDE { get; set; }
        public string FARM_LATITUDE { get; set; }
        public string FARM_AREA { get; set; }
        public string FARMING_CROPS { get; set; }
        public string EXPERIENCE { get; set; }
        public string REMARKS { get; set; }


    }

    public class UpdateVisitFarmer
    {
        public string SP_CODE { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public char CUSTOMER_TYPE { get; set; }
        public string DESTINATION { get; set; }
        public string MAC_ADDRESS { get; set; }
        public string IS_VISITED { get; set; }
        public string REMARKS { get; set; }
        public string ROUTE_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string SYNC_ID { get; set; }
    }

}
