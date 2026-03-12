using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class NotificationModel
    {
        public NotificationModel()
        {
            SP_CODES = new List<string>();
        }
        public int NOTIFICATION_ID { get; set; }
        public string NOTIFICATION_TITLE { get; set; }
        public string NOTIFICATION_TEXT { get; set; }
        public string NOTIFICATION_TYPE { get; set; }
        public string SP_CODE { get; set; }
        public string SP_EDESC { get; set; }
        public string STATUS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public List<string> SP_CODES { get; set; }
    }
    public class FCMResult
    {
        public bool Successful { get; set; }
        public string Response { get; set; }
        public Exception Error { get; set; }
    }
}