using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace NeoErp.Core.Services.Scheduler
{
    public class MobileServerJob : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var siteName = ConfigurationManager.AppSettings["baseUrl"] != null ? ConfigurationManager.AppSettings["baseUrl"] : string.Empty;
                if (!string.IsNullOrEmpty(siteName))
                {
                    var client = new WebClient();
                    client.DownloadString(string.Format("{0}/api/MobileServer/getXmlData", siteName));
                }

            }
            catch (Exception ex)
            {
            }
        }

    //    public MobileNotificationModel getXmlData() {

    //        XDocument doc = XDocument.Load("XMLFile1.xml");

    //        var authors = doc.Descendants("Author");

    //        var obj = new MobileNotificationModel();

    //        foreach (var author in authors)
    //        {
    //            obj.SERVER_API_KEY = "";
    //            obj.SENDER_ID = "";
    //            obj.DEVICE_ID = "";
    //        }
    //        return obj;
    //    }
    //    public AndroidPushNotificationStatus SendNotification(string serverApiKey, string senderId, string deviceId, string message)
    //    {
    //        AndroidPushNotificationStatus result = new AndroidPushNotificationStatus();

    //        try
    //        {
    //            result.Successful = false;
    //            result.Error = null;

    //            var value = message;
    //            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
    //            tRequest.Method = "post";
    //            tRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
    //            tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
    //            tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

    //            string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";

    //            Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
    //            tRequest.ContentLength = byteArray.Length;

    //            using (Stream dataStream = tRequest.GetRequestStream())
    //            {
    //                dataStream.Write(byteArray, 0, byteArray.Length);

    //                using (WebResponse tResponse = tRequest.GetResponse())
    //                {
    //                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
    //                    {
    //                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
    //                        {
    //                            String sResponseFromServer = tReader.ReadToEnd();
    //                            result.Response = sResponseFromServer;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            result.Successful = false;
    //            result.Response = null;
    //            result.Error = ex;
    //        }
    //        return result;
    //    }

    //    public class MobileNotificationModel {
    //        public string SERVER_API_KEY { get; set; }

    //        public string SENDER_ID { get; set; }

    //        public string DEVICE_ID { get; set; }
    //}

    //    public class AndroidPushNotificationStatus
    //    {
    //        public bool Successful
    //        {
    //            get;
    //            set;
    //        }

    //        public string Response
    //        {
    //            get;
    //            set;
    //        }
    //        public Exception Error
    //        {
    //            get;
    //            set;
    //        }
    //    }
    }

    public class MobileWebNotificationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var siteName = ConfigurationManager.AppSettings["baseUrl"] != null ? ConfigurationManager.AppSettings["baseUrl"] : string.Empty;
                if (!string.IsNullOrEmpty(siteName))
                {
                    var client = new WebClient();
                    var client1 = new WebClient();
                    client.DownloadString(string.Format("{0}/api/MobileServer/getTaskWiseMsgByUser", siteName));
                    client1.DownloadString(string.Format("{0}/api/MobileServer/sendMsgByTaskName", siteName));
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}