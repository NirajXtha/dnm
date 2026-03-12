using System;

namespace NeoErp.Distribution.Service.Service.Scheme.models
{
    public class RequestModel
    {
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string user_id { get; set; }
        public dynamic qr_id { get; set; }
        public string offer_id { get; set; }
        public string points { get; set; }
        public QRData data { get; set; }
        public dynamic qrList { get; set; }
        public Offer offer { get; set; }
    }

    public class QrPayload
    {
        public string qr_id { get; set; }
        public string user_id { get; set; }
    }
    public class LinkKhaltiRequestModel
    {
        public string user_id { get; set; }
        public string khalti_account { get; set; }
    }
    public class LoadKhaltiRequestModel
    {
        public string user_id { get; set; }
        public string khalti_account { get; set; }
        public int Amount { get; set; }
    }

    public class QRData
    {
        public int count { get; set; }
        public string item_code { get; set; }
        public string points { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }
    }

    public class SchemeQR
    {
        public int QR_ID { get; set; }                // QR_ID
        public string QR_DATA { get; set; }           // QR_DATA
        public int REDEMABLE_POINTS { get; set; }     // REDEMABLE_POINTS
        public DateTime VALID_FROM { get; set; }      // VALID_FROM
        public DateTime VALID_TO { get; set; }        // VALID_TO
        public string IS_CLAIMED { get; set; }        // IS_CLAIMED ('Y'/'N')
        public string ITEM_CODE { get; set; }         // ITEM_CODE
        public int? CLAIMED_BY { get; set; }          // CLAIMED_BY (nullable)
        public DateTime? CLAIMED_DATE { get; set; }   // CLAIMED_DATE (nullable)
        public string IS_PRINTED { get; set; }        // IS_PRINTED ('Y'/'N')
        public string ENCODED_DATA { get; set; }        // IS_PRINTED ('Y'/'N')
    }
}
