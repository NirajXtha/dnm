using NeoErp.Core.Models.CustomModels;
using NeoErp.Models.Mobiles;
using NeoErp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace NeoErp.Controllers.Api
{
    public class MobileServerController : ApiController
    {
        private IMobileNotificationService _imobileService { get; set; }
        public MobileServerController(IMobileNotificationService imobileService)
        {
            this._imobileService = imobileService;
        }

        public string RemoveDevice(string deviceid, string userId)
        {
            try
            {
                var path = HttpContext.Current.Server.MapPath("~/App_Data/MobileDeviceInfo.xml");
                XDocument xDocument = XDocument.Load(path);
                List<XElement> user = xDocument.Descendants("User").Where(x => (string)x.Attribute("ID") == userId.ToUpper() && (string)x.Element("DeviceID") == deviceid).ToList();
                if (user.Count > 0)
                {
                    foreach (var item in user)
                    {
                        item.Remove();
                    }
                    xDocument.Save(path);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return "Device removed successfully.";
        }

        public string RegisterDevice(string deviceid, string senderId, string userId, string serverApiKey, string notifyNo = "")
        {
            if (notifyNo == "")
                notifyNo = "0";
            var path = HttpContext.Current.Server.MapPath("~/App_Data/MobileDeviceInfo.xml");

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.NewLineOnAttributes = true;

            if (System.IO.File.Exists(path) == false) //file not exist case
            {
                using (XmlWriter writer = XmlWriter.Create(path, xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("MobileDeviceInfo");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            XDocument xDocument = XDocument.Load(path);
            List<XElement> user = xDocument.Descendants("User").Where(x => (string)x.Attribute("ID") == userId.ToUpper() && (string)x.Element("DeviceID") == deviceid).ToList();
            if (user.Count > 0)
            {
                foreach (var item in user)
                {
                    item.Remove();
                }
                xDocument.Save(path);
            }

            XElement root = xDocument.Element("MobileDeviceInfo");
            root.Add(new XElement("User",
                                new XAttribute("ID", userId.ToUpper()),
                                new XAttribute("Name", userId.ToUpper()),
                                new XElement("ServerApiKey", serverApiKey),
                            new XElement("SenderID", senderId),
                            new XElement("NotifyNo", notifyNo),
                            new XElement("DeviceID", deviceid)));
            xDocument.Save(path);
            return "Device Register successfully.";
        }

        [HttpGet]
        public void getTaskWiseMsgByUser(string userNo = null, string taskName = null, string taskDate = null, string msg = null, string taskId=null)
        {
            try
            {
                var xmlPath = HttpContext.Current.Server.MapPath("~/App_Data/MobileDeviceInfo.xml");
                var xml = XDocument.Load(xmlPath);
                var result = xml.Descendants("User").ToList();
                foreach (var item in result)
                {
                    var userId = item.Attribute("ID").Value;
                    var deviceId = item.Elements("DeviceID").FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(taskName))
                    {
                        if (userId == userNo)
                        {
                            var message = msg;
                            if (!string.IsNullOrEmpty(message))
                            {
                                SendCustomNotification(item.Attribute("ID").Value, item.Elements("ServerApiKey").FirstOrDefault()?.Value, item.Elements("SenderID").FirstOrDefault()?.Value, item.Elements("DeviceID").FirstOrDefault()?.Value, item.Elements("NotifyNo").FirstOrDefault()?.Value, message, taskDate, taskName,taskId);
                            }
                        }
                    }
                    else
                    {
                        var response = this._imobileService.GetUserMsgFromWeb(userId, taskName);
                        if (response.Count() > 0)
                        {
                            foreach (var items in response)
                            {
                                taskName = items.TASK_NAME;
                                var sent = checkMessageAlreadySent(userId, deviceId, taskName);
                                if (!sent)
                                {
                                    if (!string.IsNullOrEmpty(items.MESSAGE_FORMAT))
                                    {
                                        var message = items.MESSAGE_FORMAT;
                                        if (string.IsNullOrEmpty(taskId))
                                            taskId = taskName+"_"+userId+"_" + System.DateTime.Today.ToString("MM-dd-yyyy");
                                        SendCustomNotification(item.Attribute("ID").Value, item.Elements("ServerApiKey").FirstOrDefault()?.Value, item.Elements("SenderID").FirstOrDefault()?.Value, item.Elements("DeviceID").FirstOrDefault()?.Value, item.Elements("NotifyNo").FirstOrDefault()?.Value, message, taskDate, taskName,taskId);
                                    }
                                }
                            }
                        }
                    }


                }

            }
            catch (Exception ex)
            {

            }
        }
        [HttpGet]
        public void sendMsgByTaskName(string userNo = null, string taskName = null, string taskDate = null, string msg = null, string taskId=null)
        {
            try
            {
                var xmlPath = HttpContext.Current.Server.MapPath("~/App_Data/MobileDeviceInfo.xml");
                var xml = XDocument.Load(xmlPath);
                var result = xml.Descendants("User").ToList();
                foreach (var item in result)
                {
                    var userId = item.Attribute("ID").Value;
                    var deviceId = item.Elements("DeviceID").FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(taskName) && !string.IsNullOrEmpty(taskId))
                    {
                        if (userId == userNo)
                        {
                            var message = msg;
                            if (!string.IsNullOrEmpty(message))
                            {
                                SendCustomNotification(item.Attribute("ID").Value, item.Elements("ServerApiKey").FirstOrDefault()?.Value, item.Elements("SenderID").FirstOrDefault()?.Value, item.Elements("DeviceID").FirstOrDefault()?.Value, item.Elements("NotifyNo").FirstOrDefault()?.Value, message, taskDate, taskName, taskId);
                            }
                        }
                    }
                    else
                    {
                        var response = this._imobileService.GetUniqIdFromQry(userId, taskName);
                        if (response.Count() > 0)
                        {
                            foreach (var items in response)
                            {
                                var bankId = items.BANK_GNO;
                                var brMsg = this._imobileService.getMessageByBankID(userId, bankId);
                                if (!string.IsNullOrEmpty(brMsg.MSG_FORMAT))
                                {
                                    var message = brMsg.MSG_FORMAT;
                                    taskName = "BR";
                                    SendCustomNotification(item.Attribute("ID").Value, item.Elements("ServerApiKey").FirstOrDefault()?.Value, item.Elements("SenderID").FirstOrDefault()?.Value, item.Elements("DeviceID").FirstOrDefault()?.Value, item.Elements("NotifyNo").FirstOrDefault()?.Value, message, taskDate, taskName, bankId);
                                }
                            }
                        }
                    }


                }

            }
            catch (Exception ex)
            {

            }
        }

        private static bool checkBRMessageSent(string userId, string deviceId, string bankId)
        {
            throw new NotImplementedException();
        }

        private AndroidFCMPushNotificationStatus SendCustomNotification(string userId, string serverApiKey, string senderId, string deviceId, string notifyId, string message, string taskDate, string taskName,string idVal)
        {
            AndroidFCMPushNotificationStatus result = new AndroidFCMPushNotificationStatus();
            try
            {
                result.Successful = false;
                result.Error = null;

                var value = message;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                if (taskDate == null)
                    taskDate = DateTime.Today.ToString("MM-dd-yyyy");
                var title = string.Empty;
                switch (taskName)
                {
                    case "BR":
                        title = "Bank Receive";
                        break;
                    case "RM":
                        title = "Purchase RM";
                        break;
                    default:
                        title = taskName;
                        break;
                }
                var datas = new
                {
                    to = deviceId,
                    data = new
                    {
                        list = new MobileWebPurchaseRmModel()
                        {
                            MESSAGE = message,
                            TITLE = title,
                            TASK_DATE = taskDate,
                            TASK_NAME = taskName,
                            TASK_ID = idVal
                        }
                    }

                };

                LogFile("Task Id="+idVal+", Message="+message + " , User ID=" + userId + " , Device id= " + deviceId + ",Task Name= " + taskName);
                LogFileInDatabase(message, userId, deviceId, taskName, idVal);

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(datas);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                result.Response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successful = false;
                result.Response = null;
                result.Error = ex;
            }
            return result;
        }
        public void LogFileInDatabase(string message, string userId, string deviceId, string taskName,string idVal)
        {
            try
            {
                this._imobileService.LogFileInDatabase(userId, message, deviceId, taskName, idVal);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static bool checkMessageAlreadySent(string userId, string deviceId, string taskName)
        {
            var sent = false;
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;
            string logFilePath = HttpContext.Current.Server.MapPath("~/Log/Mobile Task Msg Log/");
            var today = System.DateTime.Today.ToString("MM-dd-yyyy");
            logFilePath = logFilePath + "MobileTaskMsg-LOG." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            string[] lines = System.IO.File.ReadAllLines(logFilePath);
            if (taskName == "RM")
            {
                foreach (string line in lines)
                {
                    if (line.Contains(" , User ID=" + userId + " , Device id= " + deviceId + ",Task Name= " + taskName) && line.Contains(today))
                    {
                        sent = true;
                        break;
                    }
                }
            }
            else if (taskName == "BR")
            {
                foreach (string line in lines)
                {
                    if (line.Contains(" , Bank GNO=" + userId + " , Device id= " + deviceId + ",Task Name= " + taskName) && line.Contains(today))
                    {
                        sent = true;
                        break;
                    }
                }
            }
            return sent;
        }
        public static void LogFile(string logMessage)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            var message = "Date = " + System.DateTime.Today.ToString("MM-dd-yyyy") + ", Message = " + logMessage;

            string logFilePath = HttpContext.Current.Server.MapPath("~/Log/Mobile Task Msg Log/");
            logFilePath = logFilePath + "MobileTaskMsg-LOG." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(message);
            log.Close();
        }

        [HttpGet]
        public void getXmlData()
        {

            try
            {
                var xmlPath = HttpContext.Current.Server.MapPath("~/App_Data/MobileDeviceInfo.xml");
                var xml = XDocument.Load(xmlPath);
                var result = xml.Descendants("User").ToList();
                foreach (var item in result)
                {
                    SendNotification(item.Attribute("ID").Value, item.Elements("ServerApiKey").FirstOrDefault()?.Value, item.Elements("SenderID").FirstOrDefault()?.Value, item.Elements("DeviceID").FirstOrDefault()?.Value, item.Elements("NotifyNo").FirstOrDefault()?.Value, "TEST");
                }
            }
            catch (Exception ex)
            {

            }
        }

        public AndroidFCMPushNotificationStatus SendNotification(string userId, string serverApiKey, string senderId, string deviceId, string notifyId, string message)
        {
            AndroidFCMPushNotificationStatus result = new AndroidFCMPushNotificationStatus();
            try
            {

                var maxNotifyNo = 0;
                var msgResponse = getMessage(userId, notifyId);
                if (msgResponse.Count > 0)
                {
                    var voucherNo = "";

                    var data = new
                    {
                        to = deviceId,
                        //notification = new
                        //{
                        //    //body = new List<testModel>(),
                        //    title = msgResponse.Count.ToString(),
                        //    sound = "Enabled",
                        //},
                        data = new
                        {
                            list = new List<testModel>(),
                            title = msgResponse.Count().ToString(),
                        }
                    };

                    var count = 1;
                    foreach (var item in msgResponse)
                    {
                        if (maxNotifyNo < Convert.ToInt32(item.SESSION_ROWID))
                            maxNotifyNo = Convert.ToInt32(item.SESSION_ROWID);
                        voucherNo = string.Join(",", item.VOUCHER_NO);

                        if (count <= 5)
                        {
                            data.data.list.Add(new testModel
                            {
                                VOUCHER_NO = item.VOUCHER_NO,
                                FORM_CODE = item.FORM_CODE,
                                CHECKED_BY = item.CHECKED_BY,
                                AUTHORISED_BY = item.AUTHORISED_BY,
                                VOUCHER_AMOUNT = item.VOUCHER_AMOUNT??0,
                                ACC_CODE = item.ACC_CODE,
                                FORM_DESCRIPTION = item.FORM_DESCRIPTION,
                                BRANCH_DESCRIPTION = item.BRANCH_DESCRIPTION,
                                FORM_TYPE = item.FORM_TYPE,
                                TABLE_NAME = item.TABLE_NAME
                            });
                            //SendMsgToMobile(data, userId, serverApiKey, senderId, deviceId, notifyId, message);
                            count++;
                        }
                        //SendMsgToMobile(data, userId, serverApiKey, senderId, deviceId, notifyId, message);
                    }

                    //if (msgResponse.Count > 10)
                    //{
                    //    foreach (var item in msgResponse)
                    //    {
                    //        if (maxNotifyNo < Convert.ToInt32(item.SESSION_ROWID))
                    //            maxNotifyNo = Convert.ToInt32(item.SESSION_ROWID);
                    //        voucherNo += string.Join(",", item.VOUCHER_NO);
                    //        //data.notification.body.Add(new MobileDataVoucherModel
                    //        //{
                    //        //    VOUCHER_NO = item.VOUCHER_NO,
                    //        //    FORM_CODE = item.FORM_CODE,
                    //        //    CHECKED_BY = item.CHECKED_BY,
                    //        //    AUTHORISED_BY = item.AUTHORISED_BY,
                    //        //    SESSION_ROWID = item.SESSION_ROWID,
                    //        //    VOUCHER_DATE = item.VOUCHER_DATE,
                    //        //    Miti = item.Miti,
                    //        //    VOUCHER_AMOUNT = item.VOUCHER_AMOUNT,
                    //        //    FORM_DESCRIPTION = item.FORM_DESCRIPTION,
                    //        //    CREATED_BY = item.CREATED_BY,
                    //        //    MODULE_CODE = item.MODULE_CODE,
                    //        //    FORM_TYPE = item.FORM_TYPE,
                    //        //    REMARKS = item.REMARKS,
                    //        //    PARTICULARS = item.PARTICULARS,
                    //        //    ACC_CODE = item.ACC_CODE,
                    //        //    LEDGER_TITLE = item.LEDGER_TITLE,
                    //        //    BRANCH_DESCRIPTION = item.BRANCH_DESCRIPTION,
                    //        //    CHECK_FLAG = item.CHECK_FLAG,
                    //        //    POST_FLAG = item.POST_FLAG,
                    //        //    VERIFY_FLAG = item.VERIFY_FLAG

                    //        //});
                    //    }


                    SendMsgToMobile(data, userId, serverApiKey, senderId, deviceId, notifyId, message);
                    //}
                    //else
                    //{
                    //    foreach (var item in msgResponse)
                    //    {
                    //        if (maxNotifyNo < Convert.ToInt32(item.SESSION_ROWID))
                    //            maxNotifyNo = Convert.ToInt32(item.SESSION_ROWID);
                    //        voucherNo = string.Join(",", item.VOUCHER_NO);

                    //        data.notification.body.Add(new MobileDataVoucherModel
                    //        {
                    //            VOUCHER_NO = item.VOUCHER_NO,
                    //            FORM_CODE = item.FORM_CODE,
                    //            CHECKED_BY = item.CHECKED_BY,
                    //            AUTHORISED_BY = item.AUTHORISED_BY,
                    //            SESSION_ROWID = item.SESSION_ROWID,
                    //            VOUCHER_DATE = item.VOUCHER_DATE,
                    //            Miti = item.Miti,
                    //            VOUCHER_AMOUNT=item.VOUCHER_AMOUNT,
                    //            FORM_DESCRIPTION = item.FORM_DESCRIPTION,
                    //            CREATED_BY = item.CREATED_BY,
                    //            MODULE_CODE = item.MODULE_CODE,
                    //            FORM_TYPE = item.FORM_TYPE,
                    //            REMARKS = item.REMARKS,
                    //            PARTICULARS = item.PARTICULARS,
                    //            ACC_CODE = item.ACC_CODE,
                    //            LEDGER_TITLE = item.LEDGER_TITLE,
                    //            BRANCH_DESCRIPTION= item.BRANCH_DESCRIPTION,
                    //            CHECK_FLAG = item.CHECK_FLAG,
                    //            POST_FLAG = item.POST_FLAG,
                    //            VERIFY_FLAG = item.VERIFY_FLAG
                    //        });
                    //        SendMsgToMobile(data, userId, serverApiKey, senderId, deviceId, notifyId, message);
                    //    }
                    //}
                    // update xml file with latest notified session id.
                    //RegisterDevice(deviceId, senderId, userId, serverApiKey, "155864");
                    RegisterDevice(deviceId, senderId, userId, serverApiKey, maxNotifyNo.ToString());
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }



        private AndroidFCMPushNotificationStatus SendMsgToMobile(Object datas, string userId, string serverApiKey, string senderId, string deviceId, string notifyId, string message)
        {
            AndroidFCMPushNotificationStatus result = new AndroidFCMPushNotificationStatus();
            try
            {
                result.Successful = false;
                result.Error = null;

                var value = message;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                //var datas = new
                //{
                //    to = deviceId,
                //    notification = new
                //    {
                //        body = voucherNo,
                //        title = "Voucher Pending",
                //        sound = "Enabled"
                //    }
                //};
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(datas);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                result.Response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successful = false;
                result.Response = null;
                result.Error = ex;
            }
            return result;
        }

        public List<MobileDataVoucherModel> getMessage(string userId, string lastNotifyId)
        {
            var result = new List<MobileDataVoucherModel>();
            try
            {
                //result = this._imobileService.getNotificationStatus(userId, lastNotifyId);
                result = this._imobileService.GetVoucherWithFlag(Convert.ToInt32(userId), "00", "top", Convert.ToInt32(lastNotifyId)).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
            return result;
        }
        public class MobileNotificationModel
        {
            public string SERVER_API_KEY { get; set; }

            public string SENDER_ID { get; set; }

            public string DEVICE_ID { get; set; }
        }
        public class MessageModel
        {
            public string to { get; set; }
            public List<NotificationModel> notification { get; set; }

        }
        public class NotificationModel
        {
            public List<MobileDataVoucherModel> body { get; set; }
            public string title { get; set; }
            public string sound { get; set; }

        }

        public class testModel
        {
            public string VOUCHER_NO { get; set; }

            public string FORM_CODE { get; set; }

            public string TABLE_NAME { get; set; }

            public decimal VOUCHER_AMOUNT { get; set; }

            public string ACC_CODE { get; set; }

            public string CHECKED_BY { get; set; }
            public string AUTHORISED_BY { get; set; }

            public string FORM_DESCRIPTION { get; set; }

            public string FORM_TYPE { get; set; }

            public string BRANCH_DESCRIPTION { get; set; }
        }
        public class MobileWebPurchaseRmModel
        {
            public string MESSAGE { get; set; }
            public string TITLE { get; set; }
            public string TASK_DATE { get; set; }
            public string TASK_NAME { get; set; }
            public string TASK_ID { get; set; }
        }
        public class AndroidFCMPushNotificationStatus
        {
            public bool Successful
            {
                get;
                set;
            }

            public string Response
            {
                get;
                set;
            }
            public Exception Error
            {
                get;
                set;
            }
        }
    }
}
