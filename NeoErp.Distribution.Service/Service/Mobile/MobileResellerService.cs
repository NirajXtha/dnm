using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    public class MobileResellerService : IMobileResellerService
    {
        private NeoErpCoreEntity _objectEntity;
        public MobileResellerService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }
        public void RegisterReseller(ResellerRegistration modal, out string message, out bool status)
        {
            string Query = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(modal.Email) && !string.IsNullOrEmpty(modal.UserName) && !string.IsNullOrEmpty(modal.Password))
                {
                    if (!string.IsNullOrEmpty(modal.Reseller_Contact))
                    {
                        bool result = CheckResellerExists(modal.UserName);
                        if(result == false)
                        {
                            var data = GetResellerDetailFrmMasterTbl(modal).FirstOrDefault();
                            if(data!=null)
                            {
                                Query = $@"INSERT INTO Dist_ResellerRegistration(
                                ID,COMPANYNAME,ADDRESS ,LAT,LONGT,EMAIL ,PASSWORD ,USERNAME ,REGISTRATIONDATE ,REGISTRATION_TYPE ,
                                DELETE_FLAG ,RESELLER_CONTACT,ENTITY_CODE
                               ) VALUES
                                (resellerregistrationincrement.NEXTVAL,'{modal.CompanyName}', '{modal.Address}','{modal.Lat}','{modal.Longt}','{modal.Email}','{modal.Password}'
                                    ,'{modal.UserName}', SYSDATE, '{modal.Registration_Type}','N','{modal.Reseller_Contact}','{data.Reseller_Code}')";
                                var count = _objectEntity.ExecuteSqlCommand(Query);
                                if (count > 0)
                                {
                                    message = string.Format("User '{0}' created successfully.", modal.UserName);
                                    status = true;
                                    sendNotification(modal);
                                    return;
                                }
                                else
                                {
                                    message = string.Format("User '{0}' created failed.", modal.UserName);
                                    status = false;
                                    return;
                                }
                            }
                           
                            else if (data==null)
                            {
                                Query = $@"INSERT INTO Dist_ResellerRegistration(
                                ID,COMPANYNAME,ADDRESS ,LAT,LONGT,EMAIL ,PASSWORD ,USERNAME ,REGISTRATIONDATE ,REGISTRATION_TYPE ,
                                DELETE_FLAG ,RESELLER_CONTACT
                               ) VALUES
                                (resellerregistrationincrement.NEXTVAL,'{modal.CompanyName}', '{modal.Address}','{modal.Lat}','{modal.Longt}','{modal.Email}','{modal.Password}'
                                    ,'{modal.UserName}', SYSDATE, '{modal.Registration_Type}','N','{modal.Reseller_Contact}')";
                                var count = _objectEntity.ExecuteSqlCommand(Query);
                                if (count > 0)
                                {
                                    message = string.Format("User '{0}' created successfully.", modal.UserName);
                                    status = true;
                                    sendNotification(modal);
                                    return;
                                }
                                else
                                {
                                    message = string.Format("User '{0}' created failed.", modal.UserName);
                                    status = false;
                                    return;
                                }
                            }
                            else
                            {
                                message = string.Format("User '{0}' created failed due to no record found in master table.", modal.UserName);
                                status = false;
                                return;
                            }

                            //else if (modal.Mode == "U")
                            //{
                            //    Query = $@"UPDATE Dist_ResellerRegistration SET 
                            //            COMPANYNAME ='{modal.CompanyName}',ADDRESS = '{modal.Address}',LAT = '{modal.Lat}',LONGT = '{modal.Longt}'
                            //            ,EMAIL ='{modal.Email}',PASSWORD = '{modal.Password}', REGISTRATION_TYPE = '{modal.Registration_Type}' 
                            //            WHERE ID = {modal.Id} AND APPROVED_FLAG = 'N'";
                            //    var count = _objectEntity.ExecuteSqlCommand(Query);
                            //    if (count > 0)
                            //    {
                            //        message = string.Format("User '{0}' updated successfully.", modal.UserName);
                            //        status = true;
                            //        return;
                            //    }
                            //    else
                            //    {
                            //        message = string.Format("User '{0}' update failed.", modal.UserName);
                            //        status = false;
                            //        return;
                            //    }
                            //}
                        }
                        else
                        {
                            message = "User already is exists.";
                            status = false;
                            return;
                        }                       
                    }
                    else
                    {
                        message = "ResellerContact is empty.";
                        status = false;
                        return;
                    }   
                }
                else
                {
                    message = "UserName or Password or Email is empty.";
                    status = false;
                    return;
                }                
            }
            catch (Exception e)
            {
                message = e.Message;
                status = false;
                return;
            }
        }

        public List<ResellerRegistration> GetResellerRegisteredList(string Id, out string message, out bool status)
        {
            string Query = string.Empty;
            if (string.IsNullOrEmpty(Id))
            {
                Query = $@"SELECT * FROM Dist_ResellerRegistration WHERE DELETE_FLAG = 'N' ORDER BY Id DESC";
                var result = _objectEntity.SqlQuery<ResellerRegistration>(Query).ToList();
                message = "Reseller List Obtained.";
                status = true;
                return result;
            }
            else
            {
                Query = $@"SELECT * FROM Dist_ResellerRegistration WHERE Id = {Id} AND  DELETE_FLAG = 'N'";
                var result = _objectEntity.SqlQuery<ResellerRegistration>(Query).ToList();
                if (result.Count > 0)
                {
                    message = "Reseller Data Obtained.";
                    status = true;
                    return result;
                }
                else
                {
                    message = string.Format("Reseller Data not found of Id '{0}'.",Id);
                    status = false;
                    return result;
                }
            } 
        }
        public ResellerRegistration ValidateLogin(string userName, string password, out string message,out bool status)
        {
            string Query = string.Empty;
            if(!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
              
                Query = $@"SELECT DRR.*,DRM.LATITUDE LATITUDE_MST,DRM.LONGITUDE LONGITUDE_MST,DRM.Reseller_Code FROM Dist_ResellerRegistration DRR 
                        INNER JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = CAST(DRR.ENTITY_CODE AS VARCHAR2(20))
                        WHERE DRR.USERNAME = '{userName}' AND PASSWORD = '{password}' AND DRR.APPROVED_FLAG = 'Y'";
                var result = _objectEntity.SqlQuery<ResellerRegistration>(Query).FirstOrDefault();

                if(result==null)
                {
                    Query = $@"      SELECT DRR.*,DRM.LATITUDE LATITUDE_MST,DRM.LONGITUDE LONGITUDE_MST,DRM.DISTRIBUTOR_CODE as reseller_code FROM Dist_ResellerRegistration DRR 
                        INNER JOIN DIST_DISTRIBUTOR_MASTER DRM ON DRM.DISTRIBUTOR_CODE = CAST(DRR.ENTITY_CODE AS VARCHAR2(20))
                        WHERE DRR.USERNAME = '{userName}' AND PASSWORD = '{password}' AND DRR.APPROVED_FLAG = 'Y'";
                    result= _objectEntity.SqlQuery<ResellerRegistration>(Query).FirstOrDefault();
                }
                if (result!=null)
                {
                    string q = string.Format("Select IMAGE_NAME From DIST_VISIT_IMAGE where Entity_Code='{0}' and ROWNUM<11 order by UPLOAD_DATE desc", result.Reseller_Code);
                    result.Image_Name = _objectEntity.SqlQuery<string>(q).ToList();
                    result.Image_Path = ConfigurationManager.AppSettings["baseUrl"].ToString() + "/Areas/NeoErp.Distribution/Images/EntityImages/";                    
                    message = string.Format("User '{0}' found.", userName);
                    status = true;
                    return result;
                }
                else
                {
                    message = string.Format("User '{0}' not found.",userName);
                    status = false;
                    return null;
                }
            }
            else
            {
                message = "User or Password is empty.";
                status = false;
                return null;
            }
        }
        public void ApproveRegisteredReseller(string Id, out string message, out bool status)
        {
            using (var trans = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(Id))
                    {
                        string Query = string.Empty;
                        string message2 = string.Empty;
                        bool status2 = false;
                        var data = GetResellerRegisteredList(Id, out  message2, out  status2);
                        if (data != null)
                        {
                            if (data[0].Approved_Flag == "Y")
                            {
                                message = string.Format("User '{0}' already approved",data[0].UserName);
                                status = false;
                                return;
                            }
                            Query = $@"UPDATE Dist_ResellerRegistration SET  APPROVED_FLAG = 'Y' WHERE   ID = {Id} AND APPROVED_FLAG = 'N'";
                            var result = _objectEntity.ExecuteSqlCommand(Query);
                            if (result > 0)
                            {
                                bool success=SendEmail(data[0].Password, data[0].UserName, data[0].Email);
                                if(success == false)
                                {
                                    trans.Rollback();
                                    message = "Email Send Failed.";
                                    status = false;
                                    return;
                                }
                                message = string.Format("User '{0}' Approved.Please Check Your Email For UserName and Password to login ResellerAndroApp.",data[0].UserName);
                                status = true;
                                trans.Commit();
                                return;
                            }
                            else
                            {
                                trans.Rollback();
                                message = string.Format("User '{0}' Not Approved.",data[0].UserName);
                                status = false;
                                return;
                            }
                        }
                        else
                        {
                            message = string.Format("Relevant Id '{0}' data not found.",Id);
                            status = false;
                            return;
                        }
                    }
                    else
                    {
                        message = "Id is empty.";
                        status = false;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    message = ex.Message;
                    status = false;
                }
            }
        }
        public void ChangePassword(string password,string userName,out string message,out bool status)
        {
            using(var trans = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(userName))
                    {
                        string Query = string.Empty;
                        Query = $@"SELECT * FROM Dist_ResellerRegistration WHERE  USERNAME = '{userName}'";
                        var data = _objectEntity.SqlQuery<ResellerRegistration>(Query).ToList();
                        if (data.Count > 0)
                        {
                            Query = $@"UPDATE Dist_ResellerRegistration SET  PASSWORD = '{password}' WHERE USERNAME = '{userName}' ";
                            var result = _objectEntity.ExecuteSqlCommand(Query);
                            if (result > 0)
                            {
                                bool success = SendEmail(password, userName, data[0].Email);
                                if (success == false)
                                {
                                    trans.Rollback();
                                    message = "Email Send Failed.";
                                    status = false;
                                    return;
                                }
                                message = string.Format("User '{0}' Password Changed. Please Check Your Email For UserName and Password to login ResellerAndroApp.", userName);
                                status = true;
                                trans.Commit();
                                return;
                            }
                            else
                            {
                                trans.Rollback();
                                message = string.Format("User '{0}' Password Changed Failed.",userName);
                                status = false;
                                return;
                            }
                        }
                        else
                        {
                            message = "Invalid UserName or Email.";
                            status = false;
                            return;
                        }
                    }
                    else
                    {
                        message = "Password or UserName or Email is empty or null.";
                        status = false;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    message = ex.Message;
                    status = false;
                }                
            }            
        }
        public bool SendEmail(string password, string userName, string email)
        {
            if(!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(email))
            {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(new MailAddress(email));
                mailMessage.Subject = "User Credential For Mobile Login of ResellerAndroApp";
                mailMessage.Body = "Your UserName and Password For ResellerAndroApp \r\n\r\n" + "UserName:-  " + userName + "\r\n Password:- " + password;
                smtpClient.Send(mailMessage);
                return true;
            }
            return false;
        }
        public bool CheckResellerExists(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                string Query = string.Empty;
                Query = $@"SELECT USERNAME FROM Dist_ResellerRegistration  WHERE USERNAME = '{username}'";
                var result = _objectEntity.SqlQuery<ResellerRegistration>(Query).ToList();
                if (result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }           
            return false;
        }
        public List<ResellerRegistration> GetResellerDetailFrmMasterTbl(ResellerRegistration modal)
        {
            string query = string.Empty;
            if(modal.Registration_Type.ToLower()=="distributor")
            {
                if (modal.Id != null && modal.Id != 0)
                {
                    query = $@"select distributor_code as reseller_code from DIST_DISTRIBUTOR_MASTER where distributor_code ='{modal.Id}'";
                }
                
            }
            else
            {
                if (modal.Id != null && modal.Id != 0)
                {
                    query = $@"select * from DIST_RESELLER_MASTER where reseller_code ='{modal.Id}'";
                }
                else
                {
                    query = $@"select * from DIST_RESELLER_MASTER where reseller_contact ='{modal.Reseller_Contact}'";
                }
            }
            if(string.IsNullOrEmpty(query))
            {
                var res = new List<ResellerRegistration>();
                return res;
            }
            var result = _objectEntity.SqlQuery<ResellerRegistration>(query).ToList();
            return result;
        }

        public void DeleteReseller(string id,out string message,out bool status)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string Query = string.Empty;
                Query = $@"SELECT * FROM Dist_ResellerRegistration WHERE Id = {id}";
                var data = _objectEntity.SqlQuery<ResellerRegistration>(Query).ToList();
                if (data.Count > 0)
                {
                    Query = $@"UPDATE Dist_ResellerRegistration SET  DELETE_FLAG = 'Y' WHERE ID = {id}";
                    var result = _objectEntity.ExecuteSqlCommand(Query);
                    if (result > 0)
                    {
                        message = string.Format("User Id '{0}' delete successfully.", id);
                        status = true;
                    }
                    else
                    {
                        message = string.Format("User Id '{0}' delete failed.",id);
                        status = false;
                    }
                }
                else
                {
                    message = string.Format("User Id '{0}' data not found.", id);
                    status = false;
                }
            }
            else
            {
                message = "Id is empty.";
                status = false;
            }
        }

        private void sendNotification(ResellerRegistration modal)
        {
            var data = new
            {
                to = ConfigurationManager.AppSettings["APP_KEY"],
                data = new
                {
                    message = "User Created.",
                    user = modal.UserName
                },
                notification = new
                {
                    title= "User Created.",
                    body="User Created Notification",
                    mutable_content=true,
                    sound = "Tri-tone"
                }
            };
            SendNotification(data);
        }

        private void SendNotification(object data)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            SendNotification(byteArray);
        }
        private void SendNotification(byte[] byteArray)
        {
            try
            {
                string server_api_key = ConfigurationManager.AppSettings["SERVER_API_KEY"];
                string sender_id = ConfigurationManager.AppSettings["SENDER_ID"];

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add($"Authorization: key={server_api_key}");
                tRequest.Headers.Add($"Sender: id={sender_id}");

                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tresponse = tRequest.GetResponse();
                dataStream = tresponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                string sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tresponse.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ResellerRegistration> GetResellerImgLst(string resellerCode, out string message,out bool status)
        {
            List<ResellerRegistration> objLst = new List<ResellerRegistration>();
            try
            {               
                if (!string.IsNullOrEmpty(resellerCode))
                {
                    string query = string.Empty;
                    query = $@"SELECT DVI.IMAGE_NAME,drm.latitude lat,drm.longitude longt
                        FROM DIST_VISIT_IMAGE DVI
                        INNER JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = DVI.ENTITY_CODE AND DRM.COMPANY_CODE = DVI.COMPANY_CODE
                        where drm.reseller_code = '{resellerCode}' and ROWNUM<11 order by dvi.upload_date desc";
                    objLst = _objectEntity.SqlQuery<ResellerRegistration>(query).ToList();
                    if (objLst.Count > 0)
                    {
                        message = "Data found.";
                        status = true;
                        return objLst;
                    }
                    else
                    {
                        message = "Data not found.";
                        status = false;
                        return objLst;
                    }
                }
                else
                {
                    message = "Reseller code is empty.";
                    status = false;
                    return objLst;
                }                
            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = false;
                return objLst;
            }
        }
    }
}
