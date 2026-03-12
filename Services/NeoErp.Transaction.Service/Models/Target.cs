using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Transaction.Service.Models
{
    public class Target
    {
        public string TARGET_CODE { get; set; }
        public string NAME { get; set; }
        public string YEAR_NAME { get; set; }
        public string CALENDAR_TYPE { get; set; }
        public string RANGE_TYPE { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string ITEM_CODE { get; set; }
        public string Item_Name { get; set; } // item display name
        public string CATEGORY_CODE { get; set; }
        public string Category_Name { get; set; } // category display name
        public string CUSTOMER_CODE { get; set; }
        public string Customer_Name { get; set; } // Customer display name
        public string EMPLOYEE_CODE { get; set; }
        public string Employee_Name { get; set; } // Employee display name
        public string SALES_TARGET { get; set; }
        public string COLLECTION_TARGET { get; set; }
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
    }
}
