using NeoErp.Core.Models;
//using NeoErp.Distribution.Service.Service.PushNotificationService;
using NeoErp.Distribution.Service.Service.Scheme.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service.Scheme.Points
{
    public class PointsService : IPointsService
    {


        //public async Task<string> SendNotificationAsync(string deviceToken, string title, string body)
        //{
        //    var message = new Message()
        //    {
        //        Token = deviceToken,
        //        Notification = new Notification()
        //        {
        //            Title = title,
        //            Body = body,
        //        },
        //        Android = new AndroidConfig()
        //        {
        //            Priority = Priority.High
        //        },
        //        Apns = new ApnsConfig()
        //        {
        //            Aps = new Aps()
        //            {
        //                ContentAvailable = true
        //            }
        //        }
        //    };

        //    string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        //    return $"Successfully sent message: {response}";
        //}

        // Firebase issue comment
        //public async Task<bool> SendTestNotification()

        //{
        //    string deviceToken = "d0P3H0vqRhyCCeLYCwA3hB:APA91bGMZkp_UPLvCs7BIX3DUbQXplzubXmaeTvbBK8ycbX5LOzWh0q8RQl3maEA3PsZ9BHtttAh9ztkOPr740lPFzgjeFRe5aNBMq-oKT9sJQ3trWKOZCo"; // Replace this
        //    string title = "Hello from ASP.NET";
        //    string body = "This is a test push notification.";
        //    try
        //    {
        //        PushNotificationServiceClass service = new PushNotificationServiceClass();
        //        await service.SendNotificationAsync("d0P3H0vqRhyCCeLYCwA3hB:APA91bGMZkp_UPLvCs7BIX3DUbQXplzubXmaeTvbBK8ycbX5LOzWh0q8RQl3maEA3PsZ9BHtttAh9ztkOPr740lPFzgjeFRe5aNBMq-oKT9sJQ3trWKOZCo", "Hello from ASP.NET", "This is a test push notification.");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //}

        public async Task<object> FetchPointsHistory(RequestModel request, NeoErpCoreEntity dbContext)
        {

            var pointsQuery = $@"select SUP.*,SQR.QR_DATA,so.OFFER_NAME,SO.OFFER_DESC from 
                                    scheme_user_points SUP 
                                    left join
                                    scheme_qr SQR on SUP.QR_ID = SQR.QR_ID
                                    left join 
                                    scheme_offers SO on SUP.OFFER_ID = SO.OFFER_ID
                                    where sup.user_id = {request.user_id} ORDER BY SUP.CREATED_DATE DESC";

            var result = dbContext.SqlQuery(pointsQuery);

            //var result = dbContext.SqlQuery(pointsQuery);
            if (result == null)
                return new
                {
                    Success = true,
                    Message = "Points Transactions fetched successfully",
                    Data = new { },

                };


            var total = $@"SELECT 
                                    SUM(
                                        CASE 
                                            WHEN TYPE = 'CLAIM' THEN POINTS
                                            WHEN TYPE = 'REDEEM' THEN -POINTS
                                            ELSE 0
                                        END
                                    )  AS BALANCE_REMAINING
                                FROM 
                                    SCHEME_USER_POINTS 
                                WHERE 
                                    USER_ID = {request.user_id}";

            var totalresult = dbContext.SqlQuery<double>(total).ToList().FirstOrDefault();



            return new
            {
                Success = true,
                Message = "Points Transactions fetched successfully",
                Data = new { Data = result, Balance = totalresult },

            };
        }


        public object LoadKhalti(LoadKhaltiRequestModel request, NeoErpCoreEntity dbContext)
        {
            var offersQuery = $@"UPDATE SCHEME_USERS
                    SET KHALTI_ACCOUNT = '{request.khalti_account}'
                    WHERE ID = '{request.user_id}'";
            var result = dbContext.ExecuteSqlCommand(offersQuery);
            if (result != 0)
            {
                return new
                {
                    Success = true,
                    Message = "Khalti Account Linked Successfully",
                    Data = result,

                };
            }
            else
            {
                return new
                {
                    Success = true,
                    Message = "Error Linking Khalti Account",
                };

            }
        }
        public async Task<object> FetchAllPointsHistory(RequestModel request, NeoErpCoreEntity dbContext)
        {

            var pointsQuery = $@"select 
                                    su.first_name,
                                    su.last_name,
                                    su.middle_name,
                                    SUP.transaction_id as transaction_id,
                                    sup.user_id as user_id,
                                    sup.qr_id as qr_id,
                                    sup.created_date as created_date,
                                    sup.points as points,
                                    sup.offer_id as offer_id,
                                    SQR.QR_DATA as Qr_data,
                                    SQR.CLAIMED_DATE as claimed_date,
                                    so.OFFER_NAME as Offer_name,
                                    SO.OFFER_DESC as offer_desc from 
                                    scheme_user_points SUP 
                                    left join
                                    scheme_qr SQR on SUP.QR_ID = SQR.QR_ID
                                    left join 
                                    scheme_users su on SUP.USER_ID = su.id
                                    left join 
                                    scheme_offers SO on SUP.OFFER_ID = SO.OFFER_ID
                                    where type = 'CLAIM'
                                    ORDER BY SUP.CREATED_DATE DESC";

            var result = dbContext.SqlQuery(pointsQuery);

            //var result = dbContext.SqlQuery(pointsQuery);
            //if (result.Count <= 0)
            //    throw new Exception("No records found");


            //var total = $@"SELECT 
            //                        SUM(
            //                            CASE 
            //                                WHEN TYPE = 'CLAIM' THEN POINTS
            //                                WHEN TYPE = 'REDEEM' THEN -POINTS
            //                                ELSE 0
            //                            END
            //                        )  AS BALANCE_REMAINING
            //                    FROM 
            //                        SCHEME_USER_POINTS 
            //                    WHERE 
            //                        USER_ID = {request.user_id}";

            //var totalresult = dbContext.SqlQuery<double>(total).ToList().FirstOrDefault();



            return new
            {
                Success = true,
                Message = "Points Transactions fetched successfully",
                Data = result,

            };
        }
        public async Task<object> FetchRedeemHistory(RequestModel request, NeoErpCoreEntity dbContext)
        {

            var pointsQuery = $@"select 
                                    su.first_name,
                                    su.last_name,
                                    su.middle_name,
                                    SUP.transaction_id as transaction_id,
                                    sup.user_id as user_id,
                                    sup.qr_id as qr_id,
                                    sup.created_date as created_date,
                                    sup.points as points,
                                    sup.offer_id as offer_id,
                                    SQR.QR_DATA as Qr_data,
                                    SQR.CLAIMED_DATE as claimed_date,
                                    so.OFFER_NAME as Offer_name,
                                    SO.OFFER_DESC as offer_desc from 
                                    scheme_user_points SUP 
                                    left join
                                    scheme_qr SQR on SUP.QR_ID = SQR.QR_ID
                                    left join 
                                    scheme_users su on SUP.USER_ID = su.id
                                    left join 
                                    scheme_offers SO on SUP.OFFER_ID = SO.OFFER_ID
                                    where type = 'REDEEM'
                                     ORDER BY SUP.CREATED_DATE DESC";

            var result = dbContext.SqlQuery(pointsQuery);

            //var result = dbContext.SqlQuery(pointsQuery);
            //if (result.Count <= 0)
            //    throw new Exception("No records found");


            //var total = $@"SELECT 
            //                        SUM(
            //                            CASE 
            //                                WHEN TYPE = 'CLAIM' THEN POINTS
            //                                WHEN TYPE = 'REDEEM' THEN -POINTS
            //                                ELSE 0
            //                            END
            //                        )  AS BALANCE_REMAINING
            //                    FROM 
            //                        SCHEME_USER_POINTS 
            //                    WHERE 
            //                        USER_ID = {request.user_id}";

            //var totalresult = dbContext.SqlQuery<double>(total).ToList().FirstOrDefault();



            return new
            {
                Success = true,
                Message = "Points Transactions fetched successfully",
                Data = result,

            };
        }


        private static bool AreByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }


        #region
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");

        public static string Decrypt(string cipherText)
        {

            if (cipherText == null) return null;

            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Extract IV (first 16 bytes)
                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);

                using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                using (var ms = new MemoryStream(fullCipher, 16, fullCipher.Length - 16))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        #endregion
        public object ClaimPoints(RequestModel request, NeoErpCoreEntity dbContext)
        {
            string decryptedJson = Decrypt(request.qr_id);

            var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(decryptedJson);


            int id = Convert.ToInt32(obj["id"]);

            var pointsQuery = $@"UPDATE SCHEME_QR
                                SET 
                                    IS_CLAIMED = 'Y',
                                    CLAIMED_BY = {request.user_id},
                                    CLAIMED_DATE = SYSDATE
                                WHERE 
                                    QR_ID = {id} AND IS_CLAIMED = 'N'";

            var result = dbContext.ExecuteSqlCommand(pointsQuery);

            if (result <= 0)
            {
                throw new Exception("Either the qr is invalid or this qr has already been claimed");

            }
            else
            {
                var transactionQuery = $@"INSERT INTO SCHEME_USER_POINTS (
                                                TRANSACTION_ID,
                                                USER_ID,
                                                QR_ID,
                                                TYPE,
                                                POINTS
                                            ) VALUES (
                                                (SELECT COALESCE(MAX(TRANSACTION_ID), 0) + 1 FROM SCHEME_USER_POINTS),
                                                {request.user_id},
                                                {id},
                                                'CLAIM',
                                                (select redemable_points from scheme_qr where qr_id = {id})
                                            )";

                var transactionResult = dbContext.SqlQuery(transactionQuery);

                if (false)
                {

                    throw new Exception("Something went wrong while claiming qr , Please retry");
                }
                else
                {

                    return new
                    {
                        Success = true,
                        Message = "Points claimed successfully.",
                        Data = new
                        {
                            QrId = request.qr_id,
                            ClaimedBy = request.user_id,
                            ClaimedAt = DateTime.Now
                        }
                    };
                }
            }

        }

        public object RedeemPoints(RequestModel request, NeoErpCoreEntity dbContext)
        {


            var pointsQuery = $@"Insert into scheme_offer_transaction (id,offer_id,user_id,transaction_date) values (
                                    (SELECT COALESCE(MAX(ID), 0) + 1 FROM scheme_offer_transaction),
                                    {request.offer_id},
                                    {request.user_id},
                                    sysdate
                                    )";

            var result = dbContext.ExecuteSqlCommand(pointsQuery);

            if (result <= 0)
            {
                throw new Exception("Something went wrong while claiming reward");

            }
            else
            {
                var transactionQuery = $@"INSERT INTO SCHEME_USER_POINTS (
                                                TRANSACTION_ID,
                                                USER_ID,
                                                OFFER_ID,
                                                TYPE,
                                                POINTS
                                            ) VALUES (
                                                (SELECT COALESCE(MAX(TRANSACTION_ID), 0) + 1 FROM SCHEME_USER_POINTS),
                                                {request.user_id},
                                                {request.offer_id},
                                                'REDEEM',
                                                (select required_points from scheme_offers where OFFER_ID = {request.offer_id})
                                            )";

                var transactionResult = dbContext.SqlQuery(transactionQuery);

                if (false)
                {

                    throw new Exception("Something went wrong while claiming reward , Please retry");
                }
                else
                {

                    return new
                    {
                        Success = true,
                        Message = "Reward claimed successfully.",
                        Data = new
                        {
                            QrId = request.qr_id,
                            ClaimedBy = request.user_id,
                            ClaimedAt = DateTime.Now
                        }
                    };
                }
            }

        }
    }
}
