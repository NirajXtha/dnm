using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Models
{
    public class TargetExcelUpload
    {
        public string TARGETID { get; set; }
        public DateTime? FROM_DATE { get; set; }
        public DateTime? TO_DATE { get; set; }

        public string EMP_CODE { get; set; }
        public string DISTRIBUTOR_CODE { get; set; }
        public string ITEM_CODE { get; set; }

        public decimal? QTY { get; set; }
        public decimal? PRICE { get; set; }

        public string BRANCH_CODE { get; set; }        

        public string DELETED_FLAG { get; set; }

        public string CREATED_BY { get; set; }

        public DateTime? CREATED_DATE { get; set; }
    }

    // Validation models
    public class DistributorValidationModel
    {
        public string DISTRIBUTOR_CODE { get; set; }
        public string DISTRIBUTOR_NAME { get; set; }
    }

    public class ItemValidationModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_NAME { get; set; }
    }

    public class BranchValidationModel
    {
        public string BRANCH_CODE { get; set; }
        public string BRANCH_NAME { get; set; }
    }

    public class EmployeeValidationModel
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_NAME { get; set; }
    }
}



