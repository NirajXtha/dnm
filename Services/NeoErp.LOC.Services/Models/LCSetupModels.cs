using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class LCBankModels
    {
        public int BANK_CODE { get; set; }
        [Required]
        public string BANK_NAME { get; set; }
        [Required]
        public string ADDRESS { get; set; }
        public string BANK_ACC_NO { get; set; }
        public string SWIFT_CODE { get; set; }
        public string BRANCH { get; set; }
        public string PHONE_NO { get; set; }
        public string REMARKS { get; set; }

    }

    public class ApplicantBankModel
    {
        public string BANK_CODE { get; set; }
        [Required]
        public string BANK_NAME { get; set; }
        public string BANK_ACC_NO { get; set; }
        public string SWIFT_CODE { get; set; }
        public string LINK_ACC_CODE { get; set; }
        public string ADDRESS { get; set; }
        public string BRANCH { get; set; }
        public string PHONE_NO { get; set; }
        public string REMARKS { get; set; }
    }

    public class LC_ContractorModel
    {

        public string CONTRACTOR_CODE { get; set; }
        public string CONTRACTOR_EDESC { get; set; }
        public string CONTRACTOR_NDESC { get; set; }
        public string CONTRACTOR_ADDRESS { get; set; }
        public string PHONE_NUMBER { get; set; }
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

    public class LC_ClearingAgentModel
    {
        public string CAGENT_CODE { get; set; }
        public string CAGENT_EDESC { get; set; }
        public string CAGENT_NDESC { get; set; }
        public string CAGENT_ADDRESS { get; set; }
        public string PHONE_NUMBER { get; set; }
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

    public class LCTermModels
    {
        public int TermCode { get; set; }
        [Required]
        public string TermName { get; set; }

    }

    public class LCPTermModels
    {
        public int PTermCode { get; set; }
        [Required]
        public string PTermName { get; set; }

    }

    public class LCStatusModels
    {
        public int StatusCode { get; set; }
        [Required]
        public string StatusName { get; set; }

    }


    public class LCNumberModels
    {
        public int LocCode { get; set; }
        [Required]
        public string LcNumber { get; set; }

    }

    public class ItemNameModels
    {
        public string ItemCode { get; set; }
        [Required]
        public string ItemName { get; set; }

    }
    public class LCBeneficiaryModels
    {
        public int BNF_CODE { get; set; }
        public string BNF_EDESC { get; set; }
        public string BNF_NDESC { get; set; }
        public string ADDRESS { get; set; }
        public string COUNTRY_CODE { get; set; }
        public string COUNTRY_EDESC { get; set; }
        public string REMARKS { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }

    public class LCHSModals
    {
        public int HS_NO { get; set; }
        public string HS_EDESC { get; set; }
        public string HS_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string HS_CODE { get; set; }
        public int? DUTY_RATE { get; set; }
        public char DELETED_FLAG { get; set; }
    }

    public class LocationModels
    {
        public string CURRENCY_CODE { get; set; }
        public int LOCATION_CODE { get; set; }
        public string LOCATION_ID { get; set; }
        public string LOCATION_EDESC { get; set; }
        public int MAX_STORING_DAYS { get; set; }
        public int PER_DAY_CHARGE { get; set; }
        public char DELETED_FLAG { get; set; }
    }
    public class DocumentModels
    {
        public int DOCUMENT_CODE { get; set; }
        public string DOCUMENT_EDESC { get; set; }
        public string REMARKS { get; set; }
    }
    public class IssuingCarrierModels
    {
        public int CARRIER_CODE { get; set; }
        public string CARRIER_EDESC { get; set; }
        public string CARRIER_NDESC { get; set; }
        public string CARRIER_ADDRESS1 { get; set; }
        public string CARRIER_ADDRESS2 { get; set; }
        public string CARRIER_PHONE_NO { get; set; }
        public string REMARKS { get; set; }


    }
    public class ContainerModels
    {
        public int CONTAINER_CODE { get; set; }
        public string CONTAINER_EDESC { get; set; }
        public string CONTAINER_NDESC { get; set; }
        public string CONTAINER_NO { get; set; }
        public string CONTAINER_SIZE { get; set; }
        public string REMARKS { get; set; }


    }

    public class MasterTransactionModel
    {
        public string lcNumber { get; set; }
        public string amount { get; set; }
    }

    public class LC_LOCModel
    {
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
    }
}
