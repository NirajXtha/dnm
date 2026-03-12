using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class CommonRequestModel
    {
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string user_id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string Sync_Id { get; set; }
        public string Saved_Date { get; set; } = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
    }

    public class NotificationModel
    {
        public int NOTIFICATION_ID { get; set; }
        public string NOTIFICATION_TITLE { get; set; }
        public string NOTIFICATION_TEXT { get; set; }
    }
}