using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class UpdateRequestModel : CommonRequestModel
    {
        public string sp_code { get; set; }
        public string customer_code { get; set; }
        public string customer_type { get; set; }
        public string remarks { get; set; }
        public string is_visited { get; set; }
        public string destination { get; set; }
        public string Track_Type { get; set; } = "TRK";
        //bikalp change
        public int PO_DCOUNT { get; set; }
        public int PO_RCOUNT { get; set; }
        public int RES_DETAIL { get; set; }
        public int RES_MASTER { get; set; }
        public int RES_ENTITY { get; set; }
        public int RES_PHOTO { get; set; }
        public int RES_CONTACT_PHOTO { get; set; }
        public string Time_Eod { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
    }

    public class DeliveryApproveModel
    {
        public string TRANSACTION_NO { get; set; }
        public string REFERENCE_NO { get; set; }
        public string CHALAN_NO { get; set; }
        public string FORM_CODE { get; set; }
        public string REMARKS { get; set; }
        public string BRANCH_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DRIVER_MOBILE_NO { get; set; }
        public string LONGTITUDE { get; set; }
        public string LATITUDE { get; set; }
        public string RECEIVER_NAME { get; set; }
        public string RECEIVER_NUMBER { get; set; }
        public string TRANSPORTER_CODE { get; set; }
        public string BILTY_NUMBER { get; set; }
        public string TYPE { get; set; }
    }

    public class DistanceTrackingModel
    {
       public string sp_code { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string Type { get; set; }
        public string Km_Run { get; set; }
    }
}
