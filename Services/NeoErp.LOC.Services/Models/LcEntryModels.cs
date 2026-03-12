using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class LcEntryModels 
    {
        public int LOC_CODE { get; set; }
        public int PINVOICE_CODE { get; set; }
        public string PINVOICE_NO { get; set; }
        public string LC_NUMBER { get; set; }
        public string LEAD_TIME { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string ORDER_NO { get; set; }
        public int LC_TRACK { get; set; }
        public DateTime? OPEN_DATE { get; set; }
        public DateTime? EXPIRY_DATE { get; set; }
        public string EXPIRY_PLACE { get; set; }
        public int STATUS_CODE { get; set; }
        public string STATUS_EDESC { get; set; }
        public DateTime? LAST_SHIPMENT_DATE { get; set; }
        public int TOLERANCE_PER { get; set; }
        public int TERMS_CODE { get; set; }
        public string BNF_NAME { get; set; }
        public string TERMS_EDESC { get; set; }
        public int ADVISING_BANK_CODE { get; set; }
        public string ADVISING_BANK_EDESC { get; set; }
        public string CONFIRM_BANK_EDESC { get; set; }
        public string ISSUING_BANK_EDESC { get; set; }
        public int CONFIRM_BANK_CODE { get; set; }
        public int ISSUING_BANK_CODE { get; set; }
        public int PTERMS_CODE { get; set; }
        public string PTERMS_EDESC { get; set; }
        public int CREDIT_DAYS { get; set; }
        public string PARTIAL_SHIPMENT { get; set; }
        public string TRANSSHIPMENT { get; set; }
        public string CONFIRMATION_REQ { get; set; }
        public string TRANSFERABLE { get; set; }
        public string INSURANCE_FLAG { get; set; }
        public string APP_OUT_CHARGE { get; set; }
        public string BEF_OUT_CHARGE { get; set; }
        public string APP_CONFIRM_CHARGE { get; set; }
        public string BNF_CONFIRM_CHARGE { get; set; }
        public int DOC_REQ_DAYS { get; set; }
        public string ORIGIN_COUNTRY_CODE { get; set; }
        public string FILE_DETAIL { get; set; }
        public string REMARKS { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string APPROVED_BY { get; set; }
        public string APPROVED_DATE { get; set; }
        public List<Items> Itemlist { get; set; }
        public string[] mylist { get; set; }
        
    }

    public class LCItemsModels
    {
        public LCItemsModels()
        {
            Itemlist = new List<Items>();
        }
        public int? TERMS_CODE { get; set; }
        public int? PTERMS_CODE { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public string ORDER_NO { get; set; }
        public DateTime? VALIDITY_DATE { get; set; }
        public DateTime? EST_DELIVERY_DATE { get; set; }
        public DateTime? PINVOICE_DATE { get; set; }
        public int PINVOICE_CODE { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string PINVOICE_NO { get; set; }
        public string BNF_NAME { get; set; }
        public string LEAD_TIME { get; set; }

        public List<Items> Itemlist { get; set; }
    }



    public class LcImageModels
    {
        public int LocCode { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
    }

    public class SupplierModels
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
    }

  

    public class LCApplicantBank
    {
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
    }
}