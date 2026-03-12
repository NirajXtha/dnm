using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Models.WarrantyChecker
{
    public class WarrantyChekerModel
    {
        public string INVOICE_NO { get; set; }

        public string INVOICE_DATE { get; set; }

        public string SERIAL_NO { get; set; }

        public int WARRANTY_DAYS { get; set; }

        public string SALES_NO { get; set; }

        public string SALES_DATE { get; set; }

        public string VALID_DATE { get; set; }

        public int? EXPIARY_DAYS { get; set; }

        public string ACTIVATION_DATE { get; set; }

        public string ACTIVATE_FLAG { get; set; }

        public string THIEF_FLAG { get; set; }

        public string ACTIVATED_NAME { get; set; }

        public string CONTACT_NO { get; set; }

        public string SOLD_BY { get; set; }

        public string SERVICE_TYPE { get; set; }

        public int? SERVICE_COUNT { get; set; }

        public string COMPANY_CODE { get; set; }

        public string CREATED_BY { get; set; }

        public string CREATED_DATE { get; set; }

        public string DELETED_FLAG { get; set; }

        public string MODIFY_DATE { get; set; }

        public string MODIFY_BY { get; set; }

        public string Message { get; set; }
    }

    public class DefactModel {
        public string DEFECT_CODE { get; set; }

        public string DEFECT_NAME { get; set; }

        public string DEFECT_DESC { get; set; }
        
    }

    public class DefectMessage
    {
        public string DEFECT_CODE { get; set; }

        public string INVOICE_NO { get; set; }

        public string DEFECT_DESC { get; set; }
    }
}