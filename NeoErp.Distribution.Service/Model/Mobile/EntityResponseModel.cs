using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class EntityResponseModel
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string ACC_CODE { get; set; }
        public string CONTACT_NO { get; set; }
        public string P_CONTACT_NO { get; set; }
        public string P_CONTACT_NAME { get; set; }
        public string SHIPPING_CONTACT { get; set; }
        public string CITY_CODE { get; set; }
        public string ADDRESS { get; set; }
        public string EMAIL { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string TYPE { get; set; }
        public string WHOLESELLER { get; set; }
        public string DEFAULT_PARTY_TYPE_CODE { get; set; }
        public string PARENT_DISTRIBUTOR_CODE { get; set; }
        public string PARENT_DISTRIBUTOR_NAME { get; set; }
        public string LAST_VISIT_DATE { get; set; }
        public string LAST_VISIT_BY { get; set; }
        public string LAST_VISIT_STATUS { get; set; }
        public string IS_VISITED { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
        public string SUBTYPE_EDESC { get; set; }
        public string DISTRIBUTOR_TYPE { get; set; }
        public string RESELLER_TYPE { get; set; }
    }

    public class SPEntityModel
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string ACC_CODE { get; set; }
        public string P_CONTACT_NO { get; set; }
        public string P_CONTACT_NAME { get; set; }
        public string ADDRESS { get; set; }
        public string ROUTE_CODE { get; set; }
        public string ROUTE_NAME { get; set; }
        public int ORDER_NO { get; set; }
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string TYPE { get; set; }
        public string DEFAULT_PARTY_TYPE_CODE { get; set; }
        public string PARENT_DISTRIBUTOR_CODE { get; set; }
        public string PARENT_DISTRIBUTOR_NAME { get; set; }
        public string LAST_VISIT_DATE { get; set; }
        public string LAST_VISIT_BY { get; set; }
        public string LAST_VISIT_STATUS { get; set; }
        public string IS_VISITED { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
    }

    public class GroupMapModel
    {
        public string GROUP_CODE { get; set; }
        public string MAPPED_GROUP_CODES { get; set; }
    }
    public class CustomerModels
    {
        public string CUSTOMER_EDESC { get; set; }
        public string COMPANY_NAME { get; set; }
        public string BRANCH_NAME { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string CONTACT { get; set; }
        public string ADDRESS { get; set; }

    }
    public class NotificationDataModel
    {
        public string NOTIFICATION_TITLE { get; set; }
        public string NOTIFICATION_TEXT { get; set; }
        public string NOTIFICATION_TYPE { get; set; }
        public string STATUS { get; set; }

    }
}