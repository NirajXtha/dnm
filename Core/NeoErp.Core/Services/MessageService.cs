using NeoErp.Core.Helpers;
using NeoErp.Core.Infrastructure;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Services
{
    public class MessageService : IMessageService
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;


        public MessageService(IDbContext dbContext, IWorkContext workContext)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
        }

        public int CreateMessageQueue(MessageQueueModel model)
        {
            var currentUser = this._workContext.CurrentUserinformation;
            return this.CreateMessageQueue(model, currentUser.login_code);
        }

        public int CreateMessageQueue(MessageQueueModel model, string userName)
        {
            var result = 0;
            if (model == null)
            {
                throw new ArgumentNullException("Message queue model not supplied");
            }

            string query = string.Format(@"INSERT INTO MESSAGE_QUEUE (MESSAGE_QUEUE_CODE,GUID, EMAIL_TO,EMAIL_BCC, EMAIL_CC,ATTACHMENT_FLAG,MESSAGE_FLAG,DELETE_FLAG,CREATED_BY,CREATED_DATE,MESSAGE,ATTACHMENT_FILE,SUBJECT,MODIFY_DATE)
                            VALUES(MESSGE_QUEUE_CODE_SEQUENCE.NEXTVAL,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',TO_DATE('{8}', 'yyyy/mm/dd hh24:mi:ss'),'{9}','{10}','{11}',TO_DATE('{8}', 'yyyy/mm/dd hh24:mi:ss'))",
                              model.UniqueKey, model.EmailTo, model.EmailBcc, model.EmailCc, model.AttachmentFlag, model.MessageFlag,
                            YesNo.NO.ToString().FirstOrDefault(),
                            userName,
                            DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss"), model.Message,
                            model.AttachmentFile, model.Subject);
            try
            {
                result = this._dbContext.ExecuteSqlCommand(query);
            }
            catch (Exception ex) { throw ex; }

            return result;
        }

        public IEnumerable<MessageQueueModel> GetMessageQueue(MessageProcessStatus processStatus)
        {
            return this.GetMessageQueue((char)processStatus);
        }

        public IEnumerable<MessageQueueModel> GetMessageQueue(char status)
        {
            var messageQueue = new List<MessageQueueModel>();
            string query = @"SELECT  GUID as UniqueKey, EMAIL_TO as EmailTo, EMAIL_BCC as EmailBcc, EMAIL_CC as EmailCc, ATTACHMENT_FLAG as AttachmentFlag, MESSAGE,
                    ATTACHMENT_FILE as AttachmentFile, MESSAGE_FLAG as MessageFlag, SUBJECT 
                    FROM MESSAGE_QUEUE WHERE DELETE_FLAG = 'N'";

            if (!Char.IsWhiteSpace(status))
            {

                query += string.Format(" AND MESSAGE_FLAG = '{0}'", status);

            }
            try
            {
                messageQueue = this._dbContext.SqlQuery<MessageQueueModel>(query).ToList();
            }
            catch { }

            return messageQueue;

        }

        public int UpdateMessageStatus(List<string> mailGuids, MessageProcessStatus processStatus, string userName)
        {
            string query = string.Format(@"UPDATE MESSAGE_QUEUE SET MESSAGE_FLAG = '{0}', MODIFY_DATE = TO_DATE('{1}', 'yyyy/mm/dd hh24:mi:ss'), MODIFY_BY= '{2}' WHERE GUID IN ({3})",
            (char)processStatus, DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss"), userName, "'" + string.Join("','", mailGuids) + "'");
            try
            {
                return this._dbContext.ExecuteSqlCommand(query);
            }
            catch
            {//log error here 
            }
            return 0;
        }

        public void SendMail()
        {
            var messages = this.GetMessageQueue(MessageProcessStatus.Draft);

            if (messages.Count() == 0)
                return;


            this.UpdateMessageStatus(messages.Select(q => q.UniqueKey).ToList(), MessageProcessStatus.InProgress, "email-scheduler");
            List<string> successGuids = new List<string>();
            List<string> failureGuids = new List<string>();
            foreach (var item in messages)
            {
                try
                {
                    var emailSuccess = false;
                    if (item.AttachmentFlag == ((char)YesNo.Yes).ToString())
                    {
                        var filePath = HttpContext.Current.Server.MapPath(item.AttachmentFile);
                        if (File.Exists(filePath))
                        {
                            string[] file = new string[] { filePath };

                            emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc, "", file);
                        }
                        else
                        {
                            emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc);

                        }
                    }
                    else
                    {
                        emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc);
                    }

                    if (emailSuccess)
                        successGuids.Add(item.UniqueKey);
                    else
                        failureGuids.Add(item.UniqueKey);
                }
                catch (Exception ex)
                {
                    failureGuids.Add(item.UniqueKey);
                }

                if (failureGuids.Count() > 0)
                    this.UpdateMessageStatus(failureGuids, MessageProcessStatus.Failed, "email-scheduler");

                if (successGuids.Count() > 0)
                    this.UpdateMessageStatus(successGuids, MessageProcessStatus.Send, "email-scheduler");

            }

        }
        public void SendSchedularMailDaily()
        {

            UpdateSchedularMessageStatusOfDaily();

        }

        private void UpdateSchedularMessageStatusOfDaily()
        {
            string query = @"update WEB_SCHEDULARMAIL_QUEUE set MESSAGE_FLAG='D', MODIFY_DATE= NULL WHERE FREQUENCY_FLAG='D' ";
            var result = _dbContext.ExecuteSqlCommand(query);

        }

        public void SendSchedularMail()
        {
            SendSchedularMails();
            //bool breakloop = false;
            //var messages = this.GetSchedularMessageQueue(MessageProcessStatus.NoCheck);
            //List<string> successGuids = new List<string>();
            //List<string> failureGuids = new List<string>();
            //foreach (var item in messages)
            //{
            //    var list = new List<string>() { item.UniqueKey };
            //    // this.UpdateSchedularMessageStatus(list, MessageProcessStatus.InProgress, "email-scheduler");
            //    breakloop = false;
            //    if (item.FREQUENCY_FLAG == ((char)FequencyTypeEnum.daily).ToString())
            //    {
            //        if (item.MODIFY_DATE != null)
            //        {
            //            if (item.MessageFlag == ((char)MessageProcessStatus.Send).ToString())
            //            {
            //                breakloop = true;

            //                if (breakloop)
            //                {
            //                    this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
            //                    continue;
            //                }

            //            }
            //            ////if (item.SCHEDULAR_STARTDATE > DateTime.Now)
            //            //else if (item.CREATED_DATE.Value.TimeOfDay <= DateTime.Now.TimeOfDay)
            //            //{
            //            //    breakloop = false;

            //            //    if (breakloop)
            //            //    {
            //            //        this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
            //            //        continue;
            //            //    }
            //            //}
            //            ////added
            //            //else
            //            //{
            //            //    breakloop = true;
            //            //}

            //        }
            //        else
            //        {   //change by manoj 
            //            //if (item.MODIFY_DATE.Value.DayOfYear == DateTime.Now.DayOfYear)
            //            //if (item.MODIFY_DATE >= DateTime.Now)
            //            //breakloop = true;
            //            //else
            //            //{
            //            //    if (item.MODIFY_DATE.Value.TimeOfDay >= DateTime.Now.TimeOfDay)
            //            //        breakloop = false;
            //            //}
            //            if (item.CREATED_DATE.Value.TimeOfDay <= DateTime.Now.TimeOfDay)
            //            {
            //                breakloop = false;

            //                if (breakloop)
            //                {
            //                    this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
            //                    continue;
            //                }
            //            }
            //            //added
            //            else
            //            {
            //                breakloop = true;
            //                return;
            //            }

            //        }


            //    }
            //    //CREATED_DATE
            //    //CREATED_DATE
            //    else if (item.FREQUENCY_FLAG == ((char)FequencyTypeEnum.onetime).ToString())
            //    {
            //        if (item.MODIFY_DATE != null)
            //        {
            //            if (item.MessageFlag == ((char)MessageProcessStatus.Send).ToString())
            //                breakloop = true;
            //            if (breakloop)
            //            {
            //                this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
            //                continue;
            //            }
            //            // else if(item.SCHEDULAR_STARTDATE.Value>=DateTime.Now)
            //            //else if (item.CREATED_DATE.Value.DayOfYear == DateTime.Now.DayOfYear && item.CREATED_DATE.Value.TimeOfDay <= DateTime.Now.TimeOfDay)
            //            //{
            //            //    breakloop = false;


            //            //}
            //            ////added
            //            //else
            //            //{
            //            //    breakloop = true;
            //            //}
            //            //else
            //            //    breakloop = false;

            //        }
            //        //else if (item.SCHEDULAR_STARTDATE.Value >= DateTime.Now)
            //        else if (item.CREATED_DATE.Value.DayOfYear == DateTime.Now.DayOfYear && item.CREATED_DATE.Value.TimeOfDay <= DateTime.Now.TimeOfDay)
            //        {
            //            breakloop = false;

            //            if (breakloop)
            //            {
            //                this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
            //                continue;
            //            }
            //        }
            //        //added
            //        else
            //        {
            //            breakloop = true;
            //            return;
            //        }
            //        //else
            //        //    breakloop = false;
            //    }

            //    //else
            //    //    breakloop = false;

            //    //else
            //    //    breakloop = false;

            //    //if (breakloop)
            //    //{
            //    //    this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
            //    //    continue;
            //    //}
            //    if (item.REPORT_TYPE == "RB")
            //    {
            //        var result = TypeWiseMailSend(item);
            //        if (result.failureGuids.Count() > 0)
            //            this.UpdateSchedularMessageStatus(result.failureGuids, MessageProcessStatus.Failed, "email-scheduler");

            //        if (result.successGuids.Count() > 0)
            //            this.UpdateSchedularMessageStatus(result.successGuids, MessageProcessStatus.Send, "email-scheduler");
            //        continue;
            //    }
            //    try
            //    {
            //        bool withattach = false;
            //        DataTable dataattach = new DataTable();
            //        if (!string.IsNullOrEmpty(item.template))
            //        {
            //            if (!string.IsNullOrEmpty(item.Message))
            //            {
            //                item.Message = item.Message.ReplaceHtmlTag();
            //                var errormsg = string.Empty;
            //                if (item.SqlType == "inline")
            //                {
            //                    try
            //                    {
            //                        DataTable data = _dbContext.SqlQuery(item.Message);
            //                        var htmlData = data.ConvertDataTableToHTMLWithFormat();
            //                        var message = item.template.Replace("#sqlquery#", htmlData.ToString());
            //                        item.Message = message;
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        errormsg = "Error To Fetch data from Database";
            //                    }
            //                }
            //                else
            //                {
            //                    dataattach = _dbContext.SqlQuery(item.Message);
            //                    withattach = true;
            //                    item.Message = item.template;
            //                }
            //            }
            //        }
            //        var emailSuccess = false;
            //        if (item.AttachmentFlag == ((char)YesNo.Yes).ToString())
            //        {
            //            if (withattach)
            //            {
            //                //if(item.SqlType== "attachDoc")

            //                var fileattach = new System.Net.Mail.Attachment(dataattach.DataToExcel(), string.Format("{0}.{1}", item.SchedularName, item.SqlType == "attachDoc" ? "doc" : "xls"));
            //                System.Net.Mail.Attachment[] file = new System.Net.Mail.Attachment[] { fileattach };
            //                emailSuccess = MailHelper.SendMailDirectAttach(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc, "", file);
            //                if (emailSuccess)
            //                    this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
            //            }
            //            else
            //            {

            //                var filePath = HttpContext.Current.Server.MapPath(item.AttachmentFile);
            //                if (File.Exists(filePath))
            //                {
            //                    string[] file = new string[] { filePath };

            //                    emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc, "", file);
            //                }
            //                else
            //                {
            //                    emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc);
            //                    if (emailSuccess)
            //                        this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
            //                }
            //            }
            //        }
            //        else
            //        {
            //            var Msg = string.IsNullOrEmpty(item.Message) ? (string.IsNullOrEmpty(item.template) ? item.Message : item.template) : item.Message;
            //            emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, Msg, item.EmailTo, item.EmailCc, item.EmailBcc);

            //        }

            //        if (emailSuccess)
            //            successGuids.Add(item.UniqueKey);
            //        else
            //            failureGuids.Add(item.UniqueKey);
            //    }
            //    catch (Exception ex)
            //    {
            //        failureGuids.Add(item.UniqueKey);
            //    }

            //    if (failureGuids.Count() > 0)
            //        this.UpdateSchedularMessageStatus(failureGuids, MessageProcessStatus.Failed, "email-scheduler");

            //    if (successGuids.Count() > 0)
            //        this.UpdateSchedularMessageStatus(successGuids, MessageProcessStatus.Send, "email-scheduler");

            //}

        }
        private bool Countfn(int a)
        {

            if (a % 100 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public void SendSchedularMails()
        //{
        //    var messages = this.GetSchedularMessageQueue(MessageProcessStatus.NoCheck);
        //    DateTime currentDateTime = DateTime.Now;
        //    foreach (var item in messages)
        //    {

        //        if (item.MODIFY_DATE != null && item.MessageFlag == ((char)MessageProcessStatus.Send).ToString())
        //        {
        //            if (item.FREQUENCY_FLAG == ((char)FequencyTypeEnum.fequencyrange).ToString())
        //            {
        //                var timeFrame = Convert.ToInt32(item.TIME_FRAME);
        //                if (item.MODIFY_DATE.Value.AddDays(timeFrame).ToString("yyyy/MM/dd") != DateTime.Now.ToString("yyyy/MM/dd") || !IsTimeMatch(item.SCHEDULAR_STARTDATE.Value,currentDateTime))
        //                {
        //                    continue;
        //                }
        //                //if (item.MODIFY_DATE.Value.AddDays(timeFrame).DayOfYear > DateTime.Now.DayOfYear || item.CREATED_DATE.Value.TimeOfDay >= DateTime.Now.TimeOfDay)
        //                //    continue;
        //            }
        //            else if(item.FREQUENCY_FLAG == ((char)FequencyTypeEnum.daily).ToString())
        //            {
        //                if (item.MODIFY_DATE.Value.AddHours(24).ToString("yyyy/MM/dd") != DateTime.Now.ToString("yyyy/MM/dd") || !IsTimeMatch(item.SCHEDULAR_STARTDATE.Value,currentDateTime))
        //                {
        //                    continue;
        //                }
        //                    //if (item.MODIFY_DATE.Value.AddHours(24).DayOfYear > DateTime.Now.DayOfYear || item.CREATED_DATE.Value.TimeOfDay >= DateTime.Now.TimeOfDay)
        //                    //    continue;
        //            }
        //            else continue;
        //        }
        //        else
        //        {
        //            //added by chandra for new scheduler execute
        //            int timeFrame = 1;
        //            timeFrame = item.FREQUENCY_FLAG == ((char)FequencyTypeEnum.fequencyrange).ToString() ? Convert.ToInt32(item.TIME_FRAME) : timeFrame;

        //            if (!IsDateMatch(item.SCHEDULAR_STARTDATE.Value, timeFrame) || !IsTimeMatch(item.SCHEDULAR_STARTDATE.Value,currentDateTime))
        //            {
        //                continue;
        //            }
        //            //old code
        //            //if (item.CREATED_DATE.Value.DayOfYear > DateTime.Now.DayOfYear || item.CREATED_DATE.Value.TimeOfDay >= DateTime.Now.TimeOfDay)
        //            //    continue;
        //        }
        //        if (item.EmailTo == "Customer")
        //        {

        //            var query = $@"SELECT EMAIL FROM SA_CUSTOMER_SETUP where deleted_flag='N' and company_code='02' and  REGEXP_LIKE (EMAIL,'^\w+(\.\w+)*+@\w+(\.\w+)+$')";
        //            var Records = _dbContext.SqlQuery<string>(query).ToList();
        //            foreach (var inv in Records)
        //            {
        //                item.EmailTo = inv.ToString().Trim();
        //                SendInvMail(item);

        //            }
        //            //var commonMail = string.Join(",", Records);
        //            //item.EmailTo = commonMail;


        //        }
        //        else if (item.EmailTo == "outlet")
        //        {

        //            var query = $@"SELECT EMAIL FROM dist_reseller_master  where deleted_flag='N' and company_code='02' and  REGEXP_LIKE (EMAIL,'^\w+(\.\w+)*+@\w+(\.\w+)+$')";
        //            var Records = _dbContext.SqlQuery<string>(query).ToList();
        //            //var commonMail = string.Join(",", Records);
        //            //item.EmailTo = commonMail;
        //            foreach (var inv in Records)
        //            {
        //                item.EmailTo = inv.ToString().Trim();
        //                SendInvMail(item);

        //            }
        //        }
        //        else if (item.EmailTo == "outletCustomer" || item.EmailTo == "Customeroutlet")
        //        {
        //            var query = $@"SELECT EMAIL FROM SA_CUSTOMER_SETUP  where deleted_flag='N' and company_code='02' and  REGEXP_LIKE (EMAIL,'^\w+(\.\w+)*+@\w+(\.\w+)+$')  UNION ALL SELECT EMAIL FROM dist_reseller_master  where deleted_flag='N' and company_code='02' and  REGEXP_LIKE (EMAIL,'^\w+(\.\w+)*+@\w+(\.\w+)+$')";
        //            var Records = _dbContext.SqlQuery<string>(query).Where(x => x != null).ToList();
        //            //var commonMail = string.Join(",", Records);
        //            //item.EmailTo = commonMail;
        //            foreach (var inv in Records)
        //            {
        //                item.EmailTo = inv.ToString().Trim();
        //                SendInvMail(item);

        //            }
        //        }
        //        else {
        //            if (item.EmailTo != "")
        //            {
        //                var tem = item.EmailTo.ToString().Split(',');
        //                foreach (var ie in tem)
        //                {
        //                    item.EmailTo = ie;
        //                    SendInvMail(item);
        //                }
        //            }

        //        }


        //    }

        //}
        public void SendSchedularMails()
        {
            var messages = this.GetSchedularMessageQueue(MessageProcessStatus.NoCheck);

            foreach (var item in messages)
            {

                if (item.MODIFY_DATE != null && item.MessageFlag == ((char)MessageProcessStatus.Send).ToString())
                {
                    if (item.FREQUENCY_FLAG == ((char)FequencyTypeEnum.fequencyrange).ToString())
                    {
                        var timeFrame = Convert.ToInt32(item.TIME_FRAME);
                        if (item.MODIFY_DATE.Value.AddDays(timeFrame).DayOfYear > DateTime.Now.DayOfYear || item.CREATED_DATE.Value.TimeOfDay >= DateTime.Now.TimeOfDay)
                            continue;
                    }
                    else if (item.FREQUENCY_FLAG == ((char)FequencyTypeEnum.daily).ToString())
                    {
                        if (item.MODIFY_DATE.Value.AddHours(24).DayOfYear > DateTime.Now.DayOfYear || item.CREATED_DATE.Value.TimeOfDay >= DateTime.Now.TimeOfDay)
                            continue;
                    }
                    else continue;
                }
                else
                {
                    if (item.CREATED_DATE.Value.DayOfYear > DateTime.Now.DayOfYear || item.CREATED_DATE.Value.TimeOfDay >= DateTime.Now.TimeOfDay)
                        continue;
                }
                if (item.EmailTo == "Customer")
                {

                    var query = $@"SELECT EMAIL FROM SA_CUSTOMER_SETUP where deleted_flag='N' and company_code='01' and  REGEXP_LIKE (EMAIL,'^\w+(\.\w+)*+@\w+(\.\w+)+$')";
                    var Records = _dbContext.SqlQuery<string>(query).ToList();
                    foreach (var inv in Records)
                    {
                        item.EmailTo = inv.ToString().Trim();
                        SendInvMail(item);

                    }
                    //var commonMail = string.Join(",", Records);
                    //item.EmailTo = commonMail;


                }
                else if (item.EmailTo == "outlet")
                {

                    var query = $@"SELECT EMAIL FROM dist_reseller_master  where deleted_flag='N' and company_code='02' and  REGEXP_LIKE (EMAIL,'^\w+(\.\w+)*+@\w+(\.\w+)+$')";
                    var Records = _dbContext.SqlQuery<string>(query).ToList();
                    //var commonMail = string.Join(",", Records);
                    //item.EmailTo = commonMail;
                    foreach (var inv in Records)
                    {
                        item.EmailTo = inv.ToString().Trim();
                        SendInvMail(item);

                    }
                }
                else if (item.EmailTo == "outletCustomer" || item.EmailTo == "Customeroutlet")
                {
                    var query = $@"SELECT EMAIL FROM SA_CUSTOMER_SETUP  where deleted_flag='N' and company_code='02' and  REGEXP_LIKE (EMAIL,'^\w+(\.\w+)*+@\w+(\.\w+)+$')  UNION ALL SELECT EMAIL FROM dist_reseller_master  where deleted_flag='N' and company_code='02' and  REGEXP_LIKE (EMAIL,'^\w+(\.\w+)*+@\w+(\.\w+)+$')";
                    var Records = _dbContext.SqlQuery<string>(query).Where(x => x != null).ToList();
                    //var commonMail = string.Join(",", Records);
                    //item.EmailTo = commonMail;
                    foreach (var inv in Records)
                    {
                        item.EmailTo = inv.ToString().Trim();
                        SendInvMail(item);

                    }
                }
                else
                {
                    if (item.EmailTo != "")
                    {
                        var tem = item.EmailTo.ToString().Split(',');
                        foreach (var ie in tem)
                        {
                            item.EmailTo = ie;
                            SendInvMail(item);
                        }
                    }

                }


            }

        }
        public bool IsTimeMatch(DateTime sendTime, DateTime currentDateTime)
        {
            if (sendTime.TimeOfDay <= currentDateTime.TimeOfDay && sendTime.AddMinutes(10).TimeOfDay >= currentDateTime.TimeOfDay)
                return true;
            return false;
        }
        public bool IsDateMatch(DateTime startDate, int days = 1)
        {
            if (startDate.AddDays(days).ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd") || startDate.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
            {
                return true;
            }
            return false;
        }

        public void UpdateDate(string id)
        {
            Guid gId = new Guid(id);
            string query = string.Format("UPDATE WEB_SCHEDULARMAIL_QUEUE SET MODIFY_DATE='{0}' WHERE GUID='{1}'", DateTime.Now.AddMinutes(10), id);
            _dbContext.ExecuteSqlCommand(query);

        }
        public void SendInvMail(MessageQueueModel item)
        {

            List<string> successGuids = new List<string>();
            List<string> failureGuids = new List<string>();
            var list = new List<string>() { item.UniqueKey };
            try
            {
                bool withattach = false;
                DataTable dataattach = new DataTable();
                if (!string.IsNullOrEmpty(item.template))
                {
                    if (item.REPORT_TYPE == "RB")
                        item.Message = getQueryByReport(item.REPORT_NAME, item.spCode);
                    if (item.template.Contains(".html"))
                    {
                        var path = HttpContext.Current.Server.MapPath($@"~/App_Data/SchedularMailTemplae/{item.template}");
                        if (System.IO.File.Exists(path))
                        {
                            using (System.IO.StreamReader readtext = new System.IO.StreamReader(path))
                            {
                                item.template = readtext.ReadLine();
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(item.Message))
                    {
                        item.Message = item.Message.ReplaceHtmlTag();

                        var errormsg = string.Empty;
                        if (item.SqlType == "inline")
                        {
                            try
                            {
                                DataTable data = _dbContext.SqlQuery(item.Message);
                                var htmlData = data.ConvertDataTableToHTMLWithFormat();
                                var message = item.template.Replace("#sqlquery#", htmlData.ToString());
                                item.Message = message;
                            }
                            catch (Exception ex)
                            {
                                errormsg = "Error To Fetch data from Database";
                            }
                        }
                        else
                        {
                            dataattach = _dbContext.SqlQuery(item.Message);
                            withattach = true;
                            item.Message = item.template;
                        }
                    }
                }
                var emailSuccess = false;
                if (item.AttachmentFlag == ((char)YesNo.Yes).ToString())
                {
                    if (withattach)
                    {
                        //if(item.SqlType== "attachDoc")

                        var fileattach = new System.Net.Mail.Attachment(dataattach.DataToExcel(), string.Format("{0}.{1}", item.SchedularName, item.SqlType == "attachDoc" ? "doc" : "xls"));
                        System.Net.Mail.Attachment[] file = new System.Net.Mail.Attachment[] { fileattach };
                        emailSuccess = MailHelper.SendMailDirectAttach(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc, "", file);
                        if (emailSuccess)
                            this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
                    }
                    else
                    {

                        var filePath = HttpContext.Current.Server.MapPath(item.AttachmentFile);
                        if (File.Exists(filePath))
                        {
                            string[] file = new string[] { filePath };

                            emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc, "", file);
                        }
                        else
                        {
                            emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc);
                            if (emailSuccess)
                                this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
                        }
                    }
                }
                else
                {
                    var Msg = string.IsNullOrEmpty(item.Message) ? (string.IsNullOrEmpty(item.template) ? item.Message : item.template) : item.Message;
                    emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, Msg, item.EmailTo, item.EmailCc, item.EmailBcc);

                }

                if (emailSuccess)
                    successGuids.Add(item.UniqueKey);
                else
                    failureGuids.Add(item.UniqueKey);
            }
            catch (Exception ex)
            {
                failureGuids.Add(item.UniqueKey);
            }

            if (failureGuids.Count() > 0)
                this.UpdateSchedularMessageStatus(failureGuids, MessageProcessStatus.Failed, "email-scheduler");

            if (successGuids.Count() > 0)
                this.UpdateSchedularMessageStatus(successGuids, MessageProcessStatus.Send, "email-scheduler");
        }
        public MailSuccessFailureModel TypeWiseMailSend(MessageQueueModel item)
        {
            List<string> successGuids = new List<string>();
            List<string> failureGuids = new List<string>();
            var list = new List<string>() { item.UniqueKey };
            try
            {
                bool withattach = false;
                DataTable dataattach = new DataTable();
                if (!string.IsNullOrEmpty(item.template))
                {
                    item.Message = getQueryByReport(item.REPORT_NAME, item.spCode);
                    item.Message = item.Message.ReplaceHtmlTag();
                    var errormsg = string.Empty;
                    if (item.SqlType == "inline")
                    {
                        try
                        {
                            DataTable data = _dbContext.SqlQuery(item.Message);
                            var htmlData = data.ConvertDataTableToHTMLWithFormat();
                            var message = item.template.Replace("#sqlquery#", htmlData.ToString());
                            item.Message = message;
                        }
                        catch (Exception ex)
                        {
                            errormsg = "Error To Fetch data from Database";
                        }
                    }
                    else
                    {
                        dataattach = _dbContext.SqlQuery(item.Message);
                        withattach = true;
                        item.Message = item.template;
                    }
                }
                var emailSuccess = false;
                if (item.AttachmentFlag == ((char)YesNo.Yes).ToString())
                {
                    if (withattach)
                    {
                        //if(item.SqlType== "attachDoc")

                        var fileattach = new System.Net.Mail.Attachment(dataattach.DataToExcel(), string.Format("{0}.{1}", item.SchedularName, item.SqlType == "attachDoc" ? "doc" : "xls"));
                        System.Net.Mail.Attachment[] file = new System.Net.Mail.Attachment[] { fileattach };
                        emailSuccess = MailHelper.SendMailDirectAttach(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc, "", file);
                        if (emailSuccess)
                            this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
                    }
                    else
                    {

                        var filePath = HttpContext.Current.Server.MapPath(item.AttachmentFile);
                        if (File.Exists(filePath))
                        {
                            string[] file = new string[] { filePath };

                            emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc, "", file);
                        }
                        else
                        {
                            emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, item.Message, item.EmailTo, item.EmailCc, item.EmailBcc);
                            if (emailSuccess)
                                this.UpdateSchedularMessageStatus(list, MessageProcessStatus.Send, "email-scheduler");
                        }
                    }
                }
                else
                {
                    var Msg = string.IsNullOrEmpty(item.Message) ? (string.IsNullOrEmpty(item.template) ? item.Message : item.template) : item.Message;
                    emailSuccess = MailHelper.SendMail(string.Empty, item.Subject, Msg, item.EmailTo, item.EmailCc, item.EmailBcc);

                }

                if (emailSuccess)
                    successGuids.Add(item.UniqueKey);
                else
                    failureGuids.Add(item.UniqueKey);
            }
            catch (Exception ex)
            {
                failureGuids.Add(item.UniqueKey);
            }
            var obj = new MailSuccessFailureModel
            {
                failureGuids = failureGuids,
                successGuids = successGuids
            };
            return obj;
        }
        public string getQueryByReport(string reportName, string sp_filter)
        {
            return getQueryByReportName(reportName);

            var spCode = string.Empty;
            if (!string.IsNullOrEmpty(sp_filter))
                spCode = $@" AND SP_CODE IN({sp_filter})";
            //var query1 = $@"SELECT ROUTE_NAME ""Beat's Name"", GROUP_EDESC ""Group"",SP_CODE, EMPLOYEE_EDESC ""Staff's Name"", ATN_TIME ""Attendance"",EOD_TIME ""EOD"",WORKING_HOURS ""Working Hour"",
            //         TARGET ""Target Calls"",VISITED ""Target Visited"",TOTAL_VISITED ""Visited Calls"",EXTRA ""Non PJP Calls"",NOT_VISITED ""Not Visited"",TOTAL_PJP ""Productive Calls"",PJP ""PJP Productive"",NON_PJP ""Non PJP Productive"",
            //         NON_N_PJP ""NPJP Productive"",TOTAL_QUANTITY ""PJP Total Quantity"",TOTAL_PRICE ""PJP Total Amount"",
            //         ROUND( (TOTAL_VISITED/DECODE(TARGET,0,1,TARGET)  * 100),2)  ""% Effective Calls"",
            //         ROUND( (TOTAL_PJP/DECODE(TOTAL_VISITED,0,1,TOTAL_VISITED) * 100),2)  ""% Productive Calls"",
            //         EOD_REMARKS ""EOD Remarks""
            //         FROM(
            //         SELECT WM_CONCAT(DISTINCT ROUTE_NAME) ROUTE_NAME,GROUP_EDESC,SP_CODE, FULL_NAME EMPLOYEE_EDESC,
            //         TO_CHAR(ATN_TIME,'HH:MI:SS A.M.') ATN_TIME,
            //         CASE WHEN ATN_TIME = EOD_TIME THEN NULL
            //                 ELSE TO_CHAR(EOD_TIME,'HH:MI:SS A.M.')
            //         END EOD_TIME,
            //         NVL(ROUND(24 * (EOD_TIME - ATN_TIME),2),0) WORKING_HOURS,
            //         SUM(TARGET) TARGET,
            //         SUM(VISITED) VISITED,SUM(TOTAL_VISITED) TOTAL_VISITED, SUM(TOTAL_VISITED - VISITED) EXTRA, SUM(TARGET- VISITED) NOT_VISITED,
            //         SUM(TOTAL_PJP) TOTAL_PJP,SUM(PJP) PJP, SUM(VISITED - PJP)  NON_PJP, SUM(TOTAL_PJP- PJP) NON_N_PJP, SUM(TOTAL_QUANTITY) TOTAL_QUANTITY, SUM(TOTAL_PRICE) TOTAL_PRICE,
            //         EOD_REMARKS
            //         FROM(
            //         SELECT  B.ROUTE_NAME ROUTE_NAME,  B.GROUP_EDESC, A.USERID, A.FULL_NAME, A.SP_CODE, B.ASSIGN_DATE
            //         ,(SELECT MIN(SUBMIT_DATE) FROM DIST_LM_LOCATION_TRACKING WHERE TRACK_TYPE='ATN' AND SP_CODE = A.SP_CODE AND TRUNC(SUBMIT_DATE) = TO_DATE(SYSDATE,'DD-MM-RRRR')) ATN_TIME
            //         ,(SELECT TO_DATE(TO_CHAR(MAX(ATTENDANCE_TIME),'DD/MM/YYYY HH:MI:SS AM'),'DD/MM/YYYY HH:MI:SS AM') FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.SP_CODE AND TRUNC(ATTENDANCE_DT) = TO_DATE(SYSDATE,'DD-MM-RRRR')) EOD_TIME
            //         ,(SELECT REMARKS FROM DIST_EOD_UPDATE WHERE TRUNC(CREATED_DATE) = TO_DATE(SYSDATE,'DD-MM-RRRR') AND SP_CODE =  A.SP_CODE AND ROWNUM = 1) EOD_REMARKS
            //         ,CASE WHEN WM_CONCAT(B.ENTITY_CODE) IS NULL THEN 0
            //             ELSE NVL(COUNT(*),0)
            //             END TARGET
            //         ,NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(UPDATE_DATE) = B.ASSIGN_DATE AND CUSTOMER_CODE IN (SELECT ENTITY_CODE FROM DIST_TARGET_ENTITY WHERE  USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(ASSIGN_DATE) = B.ASSIGN_DATE )),0) VISITED
            //         ,NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(UPDATE_DATE)= B.ASSIGN_DATE),0) TOTAL_VISITED
            //         ,NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(ORDER_DATE) = B.ASSIGN_DATE AND RESELLER_CODE IN (SELECT ENTITY_CODE FROM DIST_TARGET_ENTITY WHERE  USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(ASSIGN_DATE) = B.ASSIGN_DATE)),0) PJP
            //         ,NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(ORDER_DATE) = B.ASSIGN_DATE ),0) TOTAL_PJP
            //         ,NVL((SELECT SUM(QUANTITY)  FROM DIST_VISITED_PO WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(ORDER_DATE) = B.ASSIGN_DATE),0) TOTAL_QUANTITY
            //         ,NVL((SELECT SUM(TOTAL_PRICE)  FROM DIST_VISITED_PO WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(ORDER_DATE) =B.ASSIGN_DATE),0) TOTAL_PRICE
            //         FROM DIST_LOGIN_USER A, DIST_TARGET_ENTITY B  
            //         WHERE A.USERID = B.USERID
            //         AND A.COMPANY_CODE = B.COMPANY_CODE
            //         AND A.ACTIVE = 'Y'
            //         AND B.ASSIGN_DATE=TO_DATE(SYSDATE,'DD-MM-RRRR')
            //         GROUP BY A.USERID, A.FULL_NAME, A.SP_CODE, B.ASSIGN_DATE, A.COMPANY_CODE,B.ROUTE_NAME,B.GROUP_EDESC
            //         ORDER BY B.ASSIGN_DATE)
            //         WHERE 1=1 {spCode}
            //         GROUP BY ATN_TIME,EOD_TIME, SP_CODE,GROUP_EDESC,SP_CODE, FULL_NAME,EOD_REMARKS)";
            //if (reportName == "DailySalesReport")
            //{
            //    var 
            //}
            //else if (reportName != "")
            //{
            var query = $@" SELECT ROUTE_NAME ""Beat's Name"", GROUP_EDESC ""Group"",SP_CODE, EMPLOYEE_EDESC ""Staff's Name"", ATN_TIME ""Attendance"",EOD_TIME ""EOD"",WORKING_HOURS ""Working Hour"",
                             TARGET ""Target Calls"",VISITED ""Target Visited"",TOTAL_VISITED ""Visited Calls"",EXTRA ""Non PJP Calls"",NOT_VISITED ""Not Visited"",TOTAL_PJP ""Productive Calls"",PJP ""PJP Productive"",NON_PJP ""Non PJP Productive"",
                             NON_N_PJP ""NPJP Productive"",TOTAL_QUANTITY ""PJP Total Quantity"",TOTAL_PRICE ""PJP Total Amount"",
                             ROUND( (TOTAL_VISITED/DECODE(TARGET,0,1,TARGET)  * 100),2)  ""% Effective Calls"",
                             ROUND( (TOTAL_PJP/DECODE(TOTAL_VISITED,0,1,TOTAL_VISITED) * 100),2)  ""% Productive Calls"",
                             EOD_REMARKS ""EOD Remarks""
                                FROM(
                                SELECT WM_CONCAT(DISTINCT ROUTE_NAME) ROUTE_NAME,GROUP_EDESC,SP_CODE, FULL_NAME EMPLOYEE_EDESC,
                                TO_CHAR(ATN_TIME,'HH:MI:SS A.M.') ATN_TIME,
                                CASE WHEN ATN_TIME = EOD_TIME THEN NULL
                                ELSE TO_CHAR(EOD_TIME,'HH:MI:SS A.M.')
                                END EOD_TIME,
                                NVL(ROUND(24 * (EOD_TIME - ATN_TIME),2),0) WORKING_HOURS,
                                SUM(TARGET) TARGET,
                                SUM(VISITED) VISITED
                                ,NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(UPDATE_DATE)= TRUNC(AA.ASSIGN_DATE)),0) TOTAL_VISITED
                                , SUM(NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(UPDATE_DATE)= TRUNC(AA.ASSIGN_DATE)),0)  - VISITED) EXTRA, SUM(TARGET- VISITED) NOT_VISITED
                                ,SUM(PJP) PJP
                                , SUM(VISITED - PJP)  NON_PJP
                                , SUM(NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(ORDER_DATE) = TRUNC(AA.ASSIGN_DATE)),0)- PJP) NON_N_PJP
                                ,NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(ORDER_DATE) = TRUNC(AA.ASSIGN_DATE) ),0) TOTAL_PJP
                                ,NVL((SELECT SUM(QUANTITY)  FROM DIST_VISITED_PO WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(ORDER_DATE) = TRUNC(AA.ASSIGN_DATE)),0) TOTAL_QUANTITY
                                ,NVL((SELECT SUM(TOTAL_PRICE)  FROM DIST_VISITED_PO WHERE USERID = AA.USERID AND COMPANY_CODE = AA.COMPANY_CODE AND TRUNC(ORDER_DATE) =TRUNC(AA.ASSIGN_DATE)),0) TOTAL_PRICE                   
                                ,EOD_REMARKS
                                FROM(
                                SELECT  B.ROUTE_NAME ROUTE_NAME,  B.GROUP_EDESC, A.USERID, A.FULL_NAME, A.SP_CODE, B.ASSIGN_DATE, B.COMPANY_CODE
                                ,(SELECT MIN(SUBMIT_DATE) FROM DIST_LM_LOCATION_TRACKING WHERE TRACK_TYPE='ATN' AND SP_CODE = A.SP_CODE AND TRUNC(SUBMIT_DATE) = TO_DATE(SYSDATE,'DD-MM-RRRR')) ATN_TIME
                                ,(SELECT TO_DATE(TO_CHAR(MAX(ATTENDANCE_TIME),'DD/MM/YYYY HH:MI:SS AM'),'DD/MM/YYYY HH:MI:SS AM') FROM HRIS_ATTENDANCE WHERE EMPLOYEE_ID = A.SP_CODE AND TRUNC(ATTENDANCE_DT) = TO_DATE(SYSDATE,'DD-MM-RRRR')) EOD_TIME
                                ,(SELECT REMARKS FROM DIST_EOD_UPDATE WHERE TRUNC(CREATED_DATE) = TO_DATE(SYSDATE,'DD-MM-RRRR') AND SP_CODE =  A.SP_CODE AND ROWNUM = 1) EOD_REMARKS
                                ,CASE WHEN WM_CONCAT(B.ENTITY_CODE) IS NULL THEN 0
                                ELSE NVL(COUNT(*),0)
                                END TARGET
                                ,NVL((SELECT COUNT(DISTINCT CUSTOMER_CODE) FROM DIST_VISITED_ENTITY WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(UPDATE_DATE) = B.ASSIGN_DATE AND CUSTOMER_CODE IN (SELECT ENTITY_CODE FROM DIST_TARGET_ENTITY WHERE  USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE  AND ROUTE_CODE = B.ROUTE_CODE AND TRUNC(ASSIGN_DATE) = B.ASSIGN_DATE )),0) VISITED
                                ,NVL((SELECT COUNT(DISTINCT RESELLER_CODE) FROM DIST_VISITED_PO WHERE USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND TRUNC(ORDER_DATE) = B.ASSIGN_DATE AND RESELLER_CODE IN (SELECT ENTITY_CODE FROM DIST_TARGET_ENTITY WHERE  USERID = A.USERID AND COMPANY_CODE = A.COMPANY_CODE AND ROUTE_CODE = B.ROUTE_CODE AND TRUNC(ASSIGN_DATE) = B.ASSIGN_DATE)),0) PJP
                                FROM DIST_LOGIN_USER A, DIST_TARGET_ENTITY B  
                                WHERE A.USERID = B.USERID
                                AND A.COMPANY_CODE = B.COMPANY_CODE
                                AND A.ACTIVE = 'Y'
                                AND B.ASSIGN_DATE BETWEEN TO_DATE(SYSDATE,'DD-MM-RRRR') AND TO_DATE(SYSDATE,'DD-MM-RRRR')
                                GROUP BY A.USERID, A.FULL_NAME, A.SP_CODE, B.ASSIGN_DATE, A.COMPANY_CODE,B.ROUTE_CODE, B.ROUTE_NAME,B.GROUP_EDESC, B.COMPANY_CODE
                                ORDER BY B.ASSIGN_DATE) AA
                                WHERE 1=1  {spCode}
                                GROUP BY  USERID, COMPANY_CODE,TRUNC(ASSIGN_DATE),  ATN_TIME,EOD_TIME, SP_CODE,GROUP_EDESC,SP_CODE, FULL_NAME,EOD_REMARKS)  order by sp_code";


            query = query.Replace(@"""", "\"");
            return query;
            //}
        }
        //public void SendMail(MailListModel model)
        public int SendMailDerect(MailListModel model)
        {


            if (model == null)
                return 0;
            var mailguid = new List<string> { model.UniqueKey };
            this.UpdateMessageStatus(mailguid, MessageProcessStatus.InProgress, "Manually");
            List<string> successGuids = new List<string>();
            List<string> failureGuids = new List<string>();

            try
            {
                var emailSuccess = false;
                if (model.ATTACHMENT_FLAG == ((char)YesNo.Yes).ToString())
                {
                    var filePath = HttpContext.Current.Server.MapPath(model.ATTACHMENT_FILE);
                    if (File.Exists(filePath))
                    {
                        string[] file = new string[] { filePath };

                        emailSuccess = MailHelper.SendMail(string.Empty, model.SUBJECT, model.MESSAGE, model.EMAIL_TO, model.EMAIL_CC, model.EMAIL_BCC, "", file);
                    }
                    else
                    {
                        emailSuccess = MailHelper.SendMail(string.Empty, model.SUBJECT, model.MESSAGE, model.EMAIL_TO, model.EMAIL_CC, model.EMAIL_BCC);

                    }
                }
                else
                {
                    emailSuccess = MailHelper.SendMail(string.Empty, model.SUBJECT, model.MESSAGE, model.EMAIL_TO, model.EMAIL_CC, model.EMAIL_BCC);
                }

                if (emailSuccess)
                    successGuids.Add(model.UniqueKey);
                else
                    failureGuids.Add(model.UniqueKey);
            }
            catch (Exception ex)
            {
                failureGuids.Add(model.UniqueKey);
            }

            if (failureGuids.Count() > 0)
                this.UpdateMessageStatus(failureGuids, MessageProcessStatus.Failed, "Manually");

            if (successGuids.Count() > 0)
                this.UpdateMessageStatus(successGuids, MessageProcessStatus.Send, "Manually");

            // Return 1 if successful, 0 if failed
            return successGuids.Count() > 0 ? 1 : 0;


        }
        public void SendMail(MailListModel model)
        {


            if (model == null)
                return;
            var mailguid = new List<string> { model.UniqueKey };
            this.UpdateMessageStatus(mailguid, MessageProcessStatus.InProgress, "Manually");
            List<string> successGuids = new List<string>();
            List<string> failureGuids = new List<string>();

            try
            {
                var emailSuccess = false;
                if (model.ATTACHMENT_FLAG == ((char)YesNo.Yes).ToString())
                {
                    var filePath = HttpContext.Current.Server.MapPath(model.ATTACHMENT_FILE);
                    if (File.Exists(filePath))
                    {
                        string[] file = new string[] { filePath };

                        emailSuccess = MailHelper.SendMail(string.Empty, model.SUBJECT, model.MESSAGE, model.EMAIL_TO, model.EMAIL_CC, model.EMAIL_BCC, "", file);
                    }
                    else
                    {
                        emailSuccess = MailHelper.SendMail(string.Empty, model.SUBJECT, model.MESSAGE, model.EMAIL_TO, model.EMAIL_CC, model.EMAIL_BCC);

                    }
                }
                else
                {
                    emailSuccess = MailHelper.SendMail(string.Empty, model.SUBJECT, model.MESSAGE, model.EMAIL_TO, model.EMAIL_CC, model.EMAIL_BCC);
                }

                if (emailSuccess)
                    successGuids.Add(model.UniqueKey);
                else
                    failureGuids.Add(model.UniqueKey);
            }
            catch (Exception ex)
            {
                failureGuids.Add(model.UniqueKey);
            }

            if (failureGuids.Count() > 0)
                this.UpdateMessageStatus(failureGuids, MessageProcessStatus.Failed, "Manually");

            if (successGuids.Count() > 0)
                this.UpdateMessageStatus(successGuids, MessageProcessStatus.Send, "Manually");



        }
        public List<MailListModel> AllMailList()

        {
            string query = @"SELECT  GUID as UniqueKey ,MESSAGE_QUEUE_CODE, EMAIL_TO, EMAIL_BCC, EMAIL_CC, ATTACHMENT_FLAG, MESSAGE,
                    ATTACHMENT_FILE, MESSAGE_FLAG,CREATED_BY, SUBJECT ,MODIFY_BY ,ATTACHMENT_FLAG
                    FROM MESSAGE_QUEUE WHERE DELETE_FLAG = 'N'";
            //return _dbContext.SqlQuery<MailListModel>(query).ToList();
            var vatRegisters = _dbContext.SqlQuery<MailListModel>(query).ToList();
            return vatRegisters;

        }
        public string UpdateMailList(MailListModel modal)
        {
            // var message = modal.MESSAGE;
            //var message.TrimStart();
            //string query = "UPDATE MESSAGE_QUEUE SET EMAIL_TO='" + modal.EMAIL_TO+ "' SUBJECT= " + modal.SUBJECT+ "";
            string query = @"update message_queue set EMAIL_TO='" + modal.EMAIL_TO + "',SUBJECT='" + modal.SUBJECT + "',MESSAGE='" + modal.MESSAGE.Trim() + "' WHERE message_queue_code='" + modal.MESSAGE_QUEUE_CODE + "' ";
            var result = _dbContext.ExecuteSqlCommand(query);
            return result.ToString();

        }


        #region MailSchedularServices
        public IEnumerable<MessageQueueModel> GetSchedularMessageQueue(MessageProcessStatus processStatus)
        {
            return this.GetSchedularMessageQueue((char)processStatus);
        }

        public IEnumerable<MessageQueueModel> GetSchedularMessageQueue(char status)
        {
            var messageQueue = new List<MessageQueueModel>();
            string query = @"SELECT  GUID as UniqueKey, EMAIL_TO as EmailTo, EMAIL_BCC as EmailBcc, EMAIL_CC as EmailCc, ATTACHMENT_FLAG as AttachmentFlag, MESSAGE,TO_CHAR(TIME_FRAME)TIME_FRAME,
                    ATTACHMENT_FILE as AttachmentFile, MESSAGE_FLAG as MessageFlag, SUBJECT ,SCHEDULAR_TEMPLATE as template,SCHEDULAR_TYPE as SqlType,SCHEDULAR_NAME as SchedularName,MODIFY_DATE as MODIFY_DATE
                  ,SCHEDULAR_STARTDATE as SCHEDULAR_STARTDATE,CREATED_DATE as CREATED_DATE,FREQUENCY_FLAG as FREQUENCY_FLAG
                ,REPORT_NAME, REPORT_TYPE,EMPLOYEE_CODE as spCode  
                FROM WEB_SCHEDULARMAIL_QUEUE WHERE DELETE_FLAG = 'N'";

            if (!Char.IsWhiteSpace(status))
            {
                if (status != (char)MessageProcessStatus.NoCheck)
                    query += string.Format(" AND MESSAGE_FLAG = '{0}'", status);

            }
            try
            {
                messageQueue = this._dbContext.SqlQuery<MessageQueueModel>(query).ToList();
            }
            catch { }

            return messageQueue;

        }

        public int UpdateSchedularMessageStatus(List<string> mailGuids, MessageProcessStatus processStatus, string userName)
        {
            string query = string.Format(@"UPDATE WEB_SCHEDULARMAIL_QUEUE SET MESSAGE_FLAG = '{0}', MODIFY_DATE = TO_DATE('{1}', 'yyyy/mm/dd hh24:mi:ss'), MODIFY_BY= '{2}' WHERE GUID IN ({3})",
            (char)processStatus, DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss"), userName, "'" + string.Join("','", mailGuids) + "'");
            try
            {
                return this._dbContext.ExecuteSqlCommand(query);
            }
            catch
            {//log error here 
            }
            return 0;
        }
        #endregion
        private string getQueryByReportName(string reportName)
        {
            string query = string.Empty;
            switch (reportName)
            {
                case "BonusStatement":
                    query = BonusStatementQuery;
                    break;
                case "BankGuarantyByParty":
                    query = BankGuarantyByParty;
                    break;
                case "BankGuarantyBySalesPerson":
                    query = BankGuarantyBySalesPerson;
                    break;
                //case "":
                //    query = $@"";
                //    break;
                case "BankGuarantyByDealer":
                    query = BankGuarantyByDealer;
                    break;
                case "DailySalesReport":
                    query = DailySalesReport;
                    break;
                case "DealerwiseDiscountLedger":
                    query = DealerwiseDiscountLedger;
                    break;
                default:
                    query = DailySalesReport;
                    break;

            }
            return query;
        }
        private string BonusStatementQuery
        {
            get
            {
                return $@"SELECT TO_CHAR(SYSDATE,'WW YYYY') WEEK,VOUCHER_DATA.PARTY_TYPE_CODE, VOUCHER_DATA.PARTY_TYPE_EDESC, SUM(AMOUNT) BONUS_AMOUNT FROM(
                        SELECT VOUCHER_NO, AMOUNT FROM(
                        SELECT REFERENCE_NO VOUCHER_NO,
                        (CASE WHEN TABLE_NAME='SA_SALES_RETURN' THEN -CHARGE_AMOUNT ELSE CHARGE_AMOUNT END) AMOUNT
                        FROM CHARGE_TRANSACTION CT
                        WHERE REFERENCE_NO IN(SELECT SALES_NO FROM SA_SALES_INVOICE WHERE SALES_NO = CT.REFERENCE_NO AND SALES_DATE= trunc(sysdate) AND DELETED_FLAG='N' AND COMPANY_CODE='01'
                        UNION ALL SELECT RETURN_NO FROM SA_SALES_RETURN WHERE RETURN_NO = CT.REFERENCE_NO AND RETURN_DATE =trunc(sysdate)  AND DELETED_FLAG='N' AND COMPANY_CODE='01')
                        AND CT.CHARGE_CODE IN('BD0')
                        AND DELETED_FLAG='N' AND COMPANY_CODE='01')) BONUS_DATA,(
                        SELECT PARTY.PARTY_TYPE_CODE, PARTY_TYPE_EDESC,VOUCHER_NO  FROM (
                        SELECT DISTINCT PARTY_TYPE_CODE, SALES_NO VOUCHER_NO FROM SA_SALES_INVOICE WHERE  DELETED_FLAG='N' AND COMPANY_CODE='01'
                        UNION ALL SELECT DISTINCT PARTY_TYPE_CODE, RETURN_NO VOUCHER_NO FROM SA_SALES_RETURN WHERE  DELETED_FLAG='N' AND COMPANY_CODE='01') PARTY, IP_PARTY_TYPE_CODE PT
                        WHERE PARTY.PARTY_TYPE_CODE= PT.PARTY_TYPE_CODE) VOUCHER_DATA
                        WHERE BONUS_DATA.VOUCHER_NO= VOUCHER_DATA.VOUCHER_NO
                        --AND VOUCHER_DATA.PARTY_TYPE_CODE=:V_PARTY_TYPE_CODE
                        GROUP BY VOUCHER_DATA.PARTY_TYPE_CODE, VOUCHER_DATA.PARTY_TYPE_EDESC";
            }
        }
        private string BankGuarantyByParty
        {
            get
            {
                return $@"SELECT CS_CODE, CS.CUSTOMER_EDESC, CS.EMAIL, CS.TEL_MOBILE_NO1, CS.TEL_MOBILE_NO2, BG_DATE, START_DATE, END_DATE, END_DATE-TRUNC(SYSDATE) REMAINING_DAYS,BG.BG_AMOUNT*BG.EXCHANGE_RATE BG_AMOUNT, CS_FLAG,BG.ACC_CODE, BANK_GNO
                FROM FA_BANK_GUARANTEE BG, SA_CUSTOMER_SETUP CS
                 WHERE END_DATE-SYSDATE <=ALERT_PRIOR_DAYS
                AND CS_FLAG='C'
                AND BG.DELETED_FLAG='N'
                AND BG.CLOSE_FLAG='N'
                AND BG.CS_CODE=CS.CUSTOMER_CODE
                AND BG.COMPANY_CODE=CS.COMPANY_CODE
                AND BG.DELETED_FLAG=CS.DELETED_FLAG";
            }
        }
        private string BankGuarantyBySalesPerson
        {
            get
            {
                return $@"SELECT CS_CODE, CS.CUSTOMER_EDESC, CS.EMAIL, CS.TEL_MOBILE_NO1, CS.TEL_MOBILE_NO2, BG_DATE, BG.START_DATE, BG.END_DATE, BG.END_DATE-TRUNC(SYSDATE) REMAINING_DAYS,
                BG.BG_AMOUNT*BG.EXCHANGE_RATE BG_AMOUNT, CS_FLAG,BG.ACC_CODE, BANK_GNO, SI.EMPLOYEE_CODE, ES.EMPLOYEE_EDESC, ES.EMAIL EMPLOYEE_EMAIL, ES.MOBILE
                FROM FA_BANK_GUARANTEE BG, SA_CUSTOMER_SETUP CS ,
                (SELECT DISTINCT SALES_DATE,CUSTOMER_CODE,  EMPLOYEE_CODE,COMPANY_CODE FROM SA_SALES_INVOICE AA WHERE DELETED_FLAG='N'
                AND SALES_DATE= (SELECT MAX(SALES_DATE) FROM SA_SALES_INVOICE WHERE CUSTOMER_CODE =AA.CUSTOMER_CODE)) SI, HR_EMPLOYEE_SETUP ES
                WHERE BG.END_DATE-SYSDATE <=ALERT_PRIOR_DAYS
                AND CS_FLAG='C'
                AND BG.DELETED_FLAG='N'
                AND BG.CLOSE_FLAG='N'
                AND BG.CS_CODE=CS.CUSTOMER_CODE
                AND BG.COMPANY_CODE=CS.COMPANY_CODE
                AND BG.COMPANY_CODE= SI.COMPANY_CODE
                AND BG.DELETED_FLAG=CS.DELETED_FLAG
                AND SI.EMPLOYEE_CODE IS NOT NULL
                AND CS.CUSTOMER_CODE= SI.CUSTOMER_CODE
                AND SI.EMPLOYEE_CODE= ES.EMPLOYEE_CODE
                AND ES.COMPANY_CODE= BG.COMPANY_CODE
                AND ES.DELETED_FLAG= BG.DELETED_FLAG
                ORDER BY CS.CUSTOMER_EDESC";
            }
        }
        private string BankGuarantyByDealer { get { return $@"SELECT CS_CODE, CS.CUSTOMER_EDESC, CS.EMAIL, CS.TEL_MOBILE_NO1, CS.TEL_MOBILE_NO2, BG_DATE, BG.START_DATE, BG.END_DATE, BG.END_DATE-TRUNC(SYSDATE) REMAINING_DAYS,
        BG.BG_AMOUNT*BG.EXCHANGE_RATE BG_AMOUNT, CS_FLAG,BG.ACC_CODE, BANK_GNO, DEALER.PARTY_TYPE_EDESC, DEALER.TEL_NO
        FROM FA_BANK_GUARANTEE BG, SA_CUSTOMER_SETUP CS ,
        (SELECT SAA.CUSTOMER_CODE, CUSTOMER_EDESC, DM.PARTY_TYPE_CODE, PARTY_TYPE_EDESC, PTC.TEL_NO, PTC.TEL_NO2 FROM SA_CUSTOMER_SETUP SAA, FA_SUB_LEDGER_DEALER_MAP DM, IP_PARTY_TYPE_CODE PTC
        WHERE SAA.CUSTOMER_CODE= DM.CUSTOMER_CODE
        AND DM.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
        AND SAA.COMPANY_CODE=DM.COMPANY_CODE
        AND SAA.COMPANY_CODE=PTC.COMPANY_CODE
        AND SAA.DELETED_FLAG='N') DEALER
        WHERE BG.END_DATE-SYSDATE <=ALERT_PRIOR_DAYS
        AND CS_FLAG='C'
        AND BG.DELETED_FLAG='N'
        AND BG.CLOSE_FLAG='N'
        AND BG.CS_CODE=CS.CUSTOMER_CODE"; } }
        private string DailySalesReport { get { return $@"SELECT DISTINCT BS_DATE(A.SALES_DATE) MITI, A.SALES_NO, FN_FETCH_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP',A.ITEM_CODE) ITEM_EDESC,
                            FN_FETCH_DESC(A.COMPANY_CODE,'SA_CUSTOMER_SETUP',A.CUSTOMER_CODE) CUSTOMER_EDESC,
                             NVL(SUM(A.QUANTITY),0) QUANTITY, A.MU_CODE, A.ITEM_CODE, NVL(SUM(A.TOTAL_PRICE),0) TOTAL_PRICE,
                             FN_FETCH_DESC(A.COMPANY_CODE,'IP_PARTY_TYPE_CODE',A.PARTY_TYPE_CODE) DEALER_NAME, B.DESTINATION, FN_FETCH_DESC(A.COMPANY_CODE,'IP_VEHICLE_CODE',B.VEHICLE_CODE) VEHICLE_NAME, B.VEHICLE_OWNER_NAME, B.VEHICLE_OWNER_NO,
                             B.DRIVER_NAME, B.DRIVER_LICENSE_NO, B.DRIVER_MOBILE_NO, FN_FETCH_DESC(A.COMPANY_CODE,'TRANSPORTER_SETUP',B.TRANSPORTER_CODE) TRANSPORTER_NAME, B.FREGHT_AMOUNT, B.WB_WEIGHT, B.WB_NO, B.WB_DATE, A.CUSTOMER_CODE, A.FORM_CODE, NVL((SELECT FIELD_VALUE FROM CUSTOM_TRANSACTION WHERE VOUCHER_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE AND BRANCH_CODE = A.BRANCH_CODE AND FORM_CODE = A.FORM_CODE AND FIELD_NAME = 'LOADING_SLIP_NO'),0) LOADING_SLIP_NO,
                             NVL((SELECT FIELD_VALUE FROM CUSTOM_TRANSACTION WHERE VOUCHER_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE AND BRANCH_CODE = A.BRANCH_CODE AND FORM_CODE = A.FORM_CODE AND FIELD_NAME = 'GATE_PASS_NO'),0) GATE_PASS_NO FROM SA_SALES_INVOICE A, SHIPPING_TRANSACTION B WHERE A.SALES_NO = B.VOUCHER_NO(+) AND A.FORM_CODE = B.FORM_CODE(+) AND A.COMPANY_CODE = B.COMPANY_CODE(+) AND A.DELETED_FLAG='N' AND A.COMPANY_CODE='01'
                             --AND TO_DATE(TO_CHAR(A.SALES_DATE,'DD-MON-YYYY')) >= '17-Aug-2019' AND TO_DATE(TO_CHAR(A.SALES_DATE,'DD-MON-YYYY')) <= '17-Sep-2019'
                            AND a.sales_date=trunc(sysdate)
                             GROUP BY BS_DATE(A.SALES_DATE), A.SALES_NO, A.MU_CODE, A.ITEM_CODE, B.DESTINATION, B.VEHICLE_OWNER_NAME, B.VEHICLE_OWNER_NO, B.DRIVER_NAME, B.DRIVER_LICENSE_NO,B.DRIVER_MOBILE_NO, B.FREGHT_AMOUNT, B.WB_WEIGHT, B.WB_NO, B.WB_DATE, A.CUSTOMER_CODE, A.FORM_CODE, A.COMPANY_CODE, A.PARTY_TYPE_CODE, B.VEHICLE_CODE, B.TRANSPORTER_CODE,A.BRANCH_CODE  ORDER BY A.SALES_NO"; } }
        private string DealerwiseDiscountLedger { get { return $@"SELECT TO_CHAR (SYSDATE, 'WW YYYY') WEEK,
                 VOUCHER_DATA.PARTY_TYPE_CODE,
                 VOUCHER_DATA.PARTY_TYPE_EDESC,
                 CHARGE_CODE,
                 SUM (AMOUNT) BONUS_AMOUNT
            FROM (SELECT VOUCHER_NO, AMOUNT, CHARGE_CODE
                    FROM (SELECT REFERENCE_NO VOUCHER_NO,
                                 (CASE
                                     WHEN TABLE_NAME = 'SA_SALES_RETURN'
                                     THEN
                                        -CHARGE_AMOUNT
                                     ELSE
                                        CHARGE_AMOUNT
                                  END)
                                    AMOUNT,CHARGE_CODE
                            FROM CHARGE_TRANSACTION CT
                           WHERE REFERENCE_NO IN
                                    (SELECT SALES_NO
                                       FROM SA_SALES_INVOICE
                                      WHERE SALES_NO = CT.REFERENCE_NO
                                            AND SALES_DATE =trunc(sysdate)
                                            AND DELETED_FLAG = 'N'
                                            AND COMPANY_CODE = '01'
                                     UNION ALL
                                     SELECT RETURN_NO
                                       FROM SA_SALES_RETURN
                                      WHERE RETURN_NO = CT.REFERENCE_NO
                                            AND RETURN_DATE =trunc(sysdate)
                                            AND DELETED_FLAG = 'N'
                                            AND COMPANY_CODE ='01')
                                 AND CT.CHARGE_CODE IN (SELECT DISTINCT CHARGE_CODE FROM CHARGE_SETUP WHERE FORM_CODE IN(SELECT FORM_CODE FROM FORM_SETUP WHERE MODULE_CODE='04') AND COMPANY_CODE='01' AND CHARGE_TYPE_FLAG='D')
                                 AND DELETED_FLAG = 'N'
                                 AND COMPANY_CODE ='01')) BONUS_DATA,
                 (SELECT PARTY.PARTY_TYPE_CODE, PARTY_TYPE_EDESC, VOUCHER_NO
                    FROM (SELECT DISTINCT PARTY_TYPE_CODE, SALES_NO VOUCHER_NO
                            FROM SA_SALES_INVOICE
                           WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '01'
                          UNION ALL
                          SELECT DISTINCT PARTY_TYPE_CODE, RETURN_NO VOUCHER_NO
                            FROM SA_SALES_RETURN
                           WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '01') PARTY,
                         IP_PARTY_TYPE_CODE PT
                   WHERE PARTY.PARTY_TYPE_CODE = PT.PARTY_TYPE_CODE) VOUCHER_DATA
           WHERE BONUS_DATA.VOUCHER_NO = VOUCHER_DATA.VOUCHER_NO
                 --AND VOUCHER_DATA.PARTY_TYPE_CODE =
                   --     NVL (:V_PARTY_TYPE_CODE, VOUCHER_DATA.PARTY_TYPE_CODE)
        GROUP BY VOUCHER_DATA.PARTY_TYPE_CODE, VOUCHER_DATA.PARTY_TYPE_EDESC,CHARGE_CODE
        ORDER BY PARTY_TYPE_EDESC"; } }

    }
}