using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class CompItemSetupModel
    {
        public int ITEM_ID { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal? RATE { get; set; }
        public string UNIT { get; set; }
        public string CATEGORY { get; set; }
    }

    public class CompItemModel
    {
        public CompItemModel()
        {
            COMP_ITEMS = new List<string>();
        }
        public int ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string COMP_ITEM_CODES { get; set; }
        public List<string> COMP_ITEMS { get; set; }
        public string COMP_ITEM_EDESC { get; set; }
    }

    public class EmployeeSetupModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EPERMANENT_ADDRESS1 { get; set; }
        public string COMPANY_EDESC { get; set; }
        public string BRANCH_EDESC { get; set; }
        public List<string> BRANCH_CODE { get; set; }
        public string BRANCH_CODES { get; set; } //to retrieve comma seperated
        public List<string> COMPANY_CODE { get; set; }
        public string COMPANY_CODES { get; set; }
        public int USER_NO { get; set; }
    }

   public class BranchModel
    {
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string COMPANY_CODE { get; set; }
   }

    public class CompanyModel
    {
        public string COMPANY_CODE { get; set; }
        public string COMPANY_EDESC { get; set; }
    }


    public class CompetitorItemFields
    {
        public CompetitorItemFields()
        {
            FIELDS = new List<FieldModel>();
        }
        public decimal ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public List<FieldModel> FIELDS { get; set; }
    }
    public class FieldModel
    {

        public string COL_NAME { get; set; }
        public string COL_DATA_TYPE { get; set; }
    }

    public class GroupMapModel
    {
        public GroupMapModel()
        {
            MAPPED_GROUPS = new List<string>();
        }
        public int GROUP_CODE { get; set; }
        public string GROUP_EDESC { get; set; }
        public string MAPPED_GROUP_CODES { get; set; }
        public List<string> MAPPED_GROUPS { get; set; }
    }

    public class UserMapModel
    {
        public UserMapModel()
        {
            MAPPED_USERS = new List<string>();
        }
        public string LOGIN_SP_CODE { get; set; }
        public string GROUP_EDESC { get; set; }
        public string DIST_SP_CODES { get; set; }
        public List<string> MAPPED_USERS { get; set; }
    }
    public class CompReportTempModel
    {
        public string ENTITY_CODE { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string COL_NAME { get; set; }
        public string ANSWER { get; set; }
        public string NEPALI_MONTH { get; set; }
        public string ENGLISH_MONTH { get; set; }
        public string CATEGORY { get; set; }
    }
    public class CompReportModel
    {
        public string ITEM_EDESC { get; set; }
        public int QUANTITY { get; set; }
        public decimal PERCENT { get; set; }
        public int TOTAL { get; set; }
        public string NEPALI_MONTH { get; set; }
        public string ENGLISH_MONTH { get; set; }
    }

    public class TroubleshootModel
    {
        public string FULL_NAME { get; set; }
        public string SP_CODE { get; set; }
        public string GROUP_EDESC { get; set; }
        public string ATTENDANCE { get; set; }
        public string DIST_COMPANY { get; set; }
        public DateTime EXPIRY_DATE { get; set; }
        public string SYN_COMPANY { get; set; }
    }

    public class BrandItemModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string BRAND_NAME { get; set; }
    }
}