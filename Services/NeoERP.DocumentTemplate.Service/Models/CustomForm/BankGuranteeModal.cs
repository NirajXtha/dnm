using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models.CustomForm
{
    public class BankGuranteeModal
    {
        public string BG_NO { get; set; }
        public string VOUCHER_NO { get; set; }
        public DateTime BG_DATE { get; set; }
        public DateTime LOG_DATE_ENGLISH { get; set; }
        public string CS_CODE { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string SUPPLIER_CODE { get; set; }

        public string ACC_CODE { get; set; }
        public string BANK_GNO { get; set; }
        public decimal BG_AMOUNT { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string ADDRESS { get; set; }
        public decimal ALERT_PRIOR_DAYS { get; set; }
        public string REMARKS { get; set; }
        public string PARTY_NAME { get; set; }
        public DateTime START_DATE_ENGLISH { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime EXPIRY_DATE_ENGLISH { get; set; }
        public DateTime END_DATE { get; set; }
        public string CS_FLAG { get; set; }
        public string CLOSE_FLAG { get; set; }
        public int EXPAIRY_DUE_DAYS { get; set; }

    }


    public class LoggedInInfoModal
    {
        public string OFFICE_NAME { get; set; }
        public string OFFICE_ADDRESS { get; set; }
        public string OFFICE_ADDRESS1 { get; set; }
    }
}
