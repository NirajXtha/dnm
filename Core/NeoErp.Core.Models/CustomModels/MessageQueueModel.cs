using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NeoErp.Core.Models.CustomModels
{
    public class MessageQueueModel
    {
        public MessageQueueModel()
        {
          this.UniqueKey = Guid.NewGuid().ToString();
        }
        public string Message { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; } = string.Empty;
        public string EmailBcc { get; set; } = string.Empty;
        public string UniqueKey { get; set; }
        public string AttachmentFlag { get; set; } 
        public string AttachmentFile { get; set; }
        public string MessageFlag { get; set; }
        public string Subject { get; set; } 
        public string template { get; set; }
        public string SqlType { get; set; }
        public string SchedularName { get; set; }
        public string MESSAGE_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public DateTime? SCHEDULAR_STARTDATE { get; set; }
        public string FREQUENCY_FLAG { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string spCode { get; set; }
        public string REPORT_NAME { get; set; }
        public string REPORT_TYPE { get; set; }
        public string TIME_FRAME { get; set; }
    }
    public class MailListModel
    {
        public MailListModel()
        {
            this.UniqueKey = Guid.NewGuid().ToString();
        }
        public int MESSAGE_QUEUE_CODE { get; set; }
        public string UniqueKey { get; set; }
        public string EMAIL_TO { get; set; }
        public string EMAIL_BCC { get; set; }
        public string EMAIL_CC { get; set; }
        public string ATTACHMENT_FLAG { get; set; }
        public string MESSAGE_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public DateTime MODIFY_DATE { get; set; }
        [AllowHtml]
        public string MESSAGE { get; set; }
        public string ATTACHMENT_FILE { get; set; }
        public string SUBJECT { get; set; }
    }
    public class MailSuccessFailureModel {
        public List<string> failureGuids { get; set; }
        public List<string> successGuids { get; set; }
    }
}

