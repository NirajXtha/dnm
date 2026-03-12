//using Newtonsoft.Json;
//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;

//namespace NeoErp.Distribution.Service.Service
//{
//    public class FirebasePushService
//    {
//        // Replace with your actual Firebase Server key
//        private const string FCM_SERVER_KEY = "AAAAxxxxxx:APA91bHxxxxx";
//        private const string FCM_URL = "https://fcm.googleapis.com/fcm/send";

//        public string SendPushNotification(string deviceToken, string title, string message)
//        {
//            if (string.IsNullOrWhiteSpace(deviceToken))
//                throw new ArgumentException("Device token cannot be null or empty.");

//            if (string.IsNullOrWhiteSpace(FCM_SERVER_KEY))
//                throw new InvalidOperationException("Firebase Server key is not configured.");

//            try
//            {
//                // Enable TLS 1.2 (needed for Firebase)
//                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

//                // Create notification payload
//                var payload = new
//                {
//                    to = deviceToken,
//                    notification = new
//                    {
//                        title = title ?? "No Title",
//                        body = message ?? "",
//                        sound = "default"
//                    },
//                    priority = "high"
//                };

//                string jsonBody = JsonConvert.SerializeObject(payload);

//                using (var client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + FCM_SERVER_KEY);
//                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

//                    // Execute POST
//                    var response = client.PostAsync(FCM_URL, content).Result;
//                    string result = response.Content.ReadAsStringAsync().Result;

//                    if (!response.IsSuccessStatusCode)
//                    {
//                        return "FCM Error (" + (int)response.StatusCode + "): " + result;
//                    }

//                    return "Success: " + result;
//                }
//            }
//            catch (AggregateException ae)
//            {
//                var inner = ae.Flatten().InnerException;
//                return "Error (Aggregate): " + (inner != null ? inner.Message : ae.Message);
//            }
//            catch (Exception ex)
//            {
//                return "Exception: " + ex.Message +
//                       (ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "");
//            }
//        }
//    }
//}
