using System;

namespace NeoErp.Models.LogServiceModel
{
    public class LogServiceViewModel
    {
        public int LOG_ID { get; set; }

        public DateTime LOG_DATE { get; set; }

        public string LOG_LEVEL { get; set; }

        public string LOG_THREAD { get; set; }

        public string LOG_LOGGER { get; set; }

        public string LOG_MESSAGE { get; set; }

        public string LOG_USER { get; set; }

        public string LOG_COMPANY { get; set; }

        public string LOG_BRANCH { get; set; }

        public string LOG_TYPECODE { get; set; }

        public string LOG_MODULE { get; set; }
    }
}