using System;

namespace NeoErp.Distribution.Service.Service.Scheme.models
{
    public class PointsTransactionModel
    {
        public object TRANSACTION_ID { get; set; }
        public object USER_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public object QR_ID { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string TYPE { get; set; }
        public object POINTS { get; set; }
        public object BALANCE_REMAINING { get; set; }
        public string QR_DATA { get; set; }
        public string OFFER_NAME { get; set; }
        public string OFFER_DESC { get; set; }
    }

}
