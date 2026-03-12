using System;

namespace NeoErp.Distribution.Service.Service.Scheme.models
{
    public class OffersModel
    {
        public int OFFER_ID { get; set; }

        public string OFFER_NAME { get; set; }

        public string OFFER_DESC { get; set; }
        public string OFFER_IMAGE { get; set; }
        public DateTime VALID_FROM { get; set; }

        public DateTime VALID_TO { get; set; }

        public double REQUIRED_POINTS { get; set; }

    }
}
