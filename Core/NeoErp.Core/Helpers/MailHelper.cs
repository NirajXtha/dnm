using NeoErp.Core.Infrastructure;
using NeoErp.Core.Integration;
using NeoErp.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public class MailHelper
    {
        #region Send Mail

        public static bool SendMail(string FromEmail, string Subject, string MailBody, string ToEmail = "", string ToCC = "", string ToBCC = "", string replyTo = "", string[] MailAttachment = null)
        {
            var mailSetting = (IMailSetting)EngineContext.Current.Resolve(typeof(IMailSetting));
            try
            {
                MailMessage msg = new MailMessage();

                MailAddress to = null;

                if (ToEmail != "")
                {
                    var tem = ToEmail.Split(',');
                    foreach (var item in tem)
                    {
                        to = new MailAddress(item);
                        msg.To.Add(to);
                    }
                }

                MailAddress cc = null;
                if (!string.IsNullOrEmpty(ToCC))
                {
                    var tem = ToCC.Split(',');
                    foreach (var item in tem)
                    {
                        cc = new MailAddress(item);
                        msg.CC.Add(cc);
                    }

                    //cc = new MailAddress(ToCC);
                }

                MailAddress bcc = null;
                if (!string.IsNullOrEmpty(ToBCC))
                {
                    var tem = ToBCC.Split(',');
                    foreach (var item in tem)
                    {
                        bcc = new MailAddress(item);
                        msg.Bcc.Add(bcc);
                    }

                    //bcc = new MailAddress(ToBCC);
                }


                MailAddress from = null;
                if (!string.IsNullOrEmpty(FromEmail))
                {
                    from = new MailAddress(FromEmail);
                    msg.From = from;
                }
                else
                {
                    //from = new MailAddress("sakya.sujan@itnepal.com");
                }

                MailAddress ReplyTo = null;
                if (!string.IsNullOrEmpty(replyTo))
                    ReplyTo = new MailAddress(replyTo);
                else
                {
                    if (!string.IsNullOrEmpty(FromEmail))
                    {
                        ReplyTo = new MailAddress(FromEmail);
                        msg.ReplyToList.Add(ReplyTo);
                    }
                }

                msg.Headers.Add("Message-Id", String.Concat("<", DateTime.Now.ToString("yyMMdd"), ".", DateTime.Now.ToString("HHmmss"), from!= null? from.Address.Substring(from.Address.IndexOf('@')):string.Empty)); //  "@zealtravels.com>"));


                if(from != null)
                msg.From =   from;

                //  msg.To.Add(to);
                // msg.To.Add(cc);
                msg.Subject = Subject;
                //  msg.Body = MailBody;
                //  msg.IsBodyHtml = true;


                if (MailAttachment != null)
                {
                    for (int i = 0; i < MailAttachment.Length; i++)
                    {
                        if (MailAttachment[i].ToString().Trim() == "") continue;
                        Attachment aa = new Attachment(MailAttachment[i]);
                        msg.Attachments.Add(aa);
                    }
                }

              
                msg.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                System.Net.Mail.AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString
                            (System.Text.RegularExpressions.Regex.Replace(MailBody, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(MailBody, null, "text/html");

                msg.AlternateViews.Add(plainView);
                msg.AlternateViews.Add(htmlView);

                msg.Priority = MailPriority.High;

                //SmtpClient emailClient = new SmtpClient("_Settings._S.Email.MailServerName", Convert.ToInt32(52222)); //  System.Web.Configuration.WebConfigurationManager.AppSettings["Smtp"].ToString());//"mail.esanjal.com",25); drvonf onr in port number
                // emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                //emailClient.EnableSsl = (bool)_Settings._S.Email.IsSPA;

                //   emailClient.Host = FromEmail.Substring(FromEmail.IndexOf('@')+1);

                //emailClient.Credentials = new NetworkCredential("_Settings._S.Email.ServerUserName", "_Settings._S.Email.ServerPassword"); // System.Web.Configuration.WebConfigurationManager.AppSettings["SmtpUser"].ToString(), System.Web.Configuration.WebConfigurationManager.AppSettings["SmtpPassword"].ToString()); // "feedback@esanjal.com", "Sebs1@#$");
                //emailClient.Send(msg);

                string Token = "Mail " + Subject + " Sent Successfully...";
                //emailClient.SendAsync(msg,Token);
                mailSetting.CurrentMailSetting.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
               // throw new Exception(ex.InnerException.Message);
               
            }

        }
        public static bool SendMailDirectAttach(string FromEmail, string Subject, string MailBody, string ToEmail = "", string ToCC = "", string ToBCC = "", string replyTo = "",  Attachment[] attachment = null)
        {
            var mailSetting = (IMailSetting)EngineContext.Current.Resolve(typeof(IMailSetting));
            try
            {
                //Debug.WriteLine(ToEmail);
                //Debug.WriteLine("mail body: ", MailBody);
                MailMessage msg = new MailMessage();

                MailAddress to = null;

                if (ToEmail != "")
                {
                    var tem = ToEmail.Split(',');
                    foreach (var item in tem)
                    {
                        to = new MailAddress(item);
                        msg.To.Add(to);
                    }
                }

                MailAddress cc = null;
                if (!string.IsNullOrEmpty(ToCC))
                {
                    var tem = ToCC.Split(',');
                    foreach (var item in tem)
                    {
                        cc = new MailAddress(item);
                        msg.CC.Add(cc);
                    }

                    //cc = new MailAddress(ToCC);
                }

                MailAddress bcc = null;
                if (!string.IsNullOrEmpty(ToBCC))
                {
                    var tem = ToBCC.Split(',');
                    foreach (var item in tem)
                    {
                        bcc = new MailAddress(item);
                        msg.Bcc.Add(bcc);
                    }

                    //bcc = new MailAddress(ToBCC);
                }


                MailAddress from = null;
                if (!string.IsNullOrEmpty(FromEmail))
                {
                    from = new MailAddress(FromEmail);
                    msg.From = from;
                }
                else
                {
                    //from = new MailAddress("sakya.sujan@itnepal.com");
                }

                MailAddress ReplyTo = null;
                if (!string.IsNullOrEmpty(replyTo))
                    ReplyTo = new MailAddress(replyTo);
                else
                {
                    if (!string.IsNullOrEmpty(FromEmail))
                    {
                        ReplyTo = new MailAddress(FromEmail);
                        msg.ReplyToList.Add(ReplyTo);
                    }
                }

                msg.Headers.Add("Message-Id", String.Concat("<", DateTime.Now.ToString("yyMMdd"), ".", DateTime.Now.ToString("HHmmss"), from != null ? from.Address.Substring(from.Address.IndexOf('@')) : string.Empty)); //  "@zealtravels.com>"));


                if (from != null)
                    msg.From = from;

                //  msg.To.Add(to);
                // msg.To.Add(cc);
                msg.Subject = Subject;
                //  msg.Body = MailBody;
                //  msg.IsBodyHtml = true;


                if (attachment != null)
                {
                    for (int i = 0; i < attachment.Length; i++)
                    {
                        
                        //Attachment aa = new Attachment(MailAttachment[i]);
                        msg.Attachments.Add(attachment[i]);
                    }
                }


                msg.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                System.Net.Mail.AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString
                            (System.Text.RegularExpressions.Regex.Replace(MailBody, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(MailBody, null, "text/html");


                msg.AlternateViews.Add(plainView);
                msg.AlternateViews.Add(htmlView);

                msg.Priority = MailPriority.High;

                //SmtpClient emailClient = new SmtpClient("_Settings._S.Email.MailServerName", Convert.ToInt32(52222)); //  System.Web.Configuration.WebConfigurationManager.AppSettings["Smtp"].ToString());//"mail.esanjal.com",25); drvonf onr in port number
                // emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                //emailClient.EnableSsl = (bool)_Settings._S.Email.IsSPA;

                //   emailClient.Host = FromEmail.Substring(FromEmail.IndexOf('@')+1);

                //emailClient.Credentials = new NetworkCredential("_Settings._S.Email.ServerUserName", "_Settings._S.Email.ServerPassword"); // System.Web.Configuration.WebConfigurationManager.AppSettings["SmtpUser"].ToString(), System.Web.Configuration.WebConfigurationManager.AppSettings["SmtpPassword"].ToString()); // "feedback@esanjal.com", "Sebs1@#$");
                //emailClient.Send(msg);

                string Token = "Mail " + Subject + " Sent Successfully...";
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        mailSetting.CurrentMailSetting.Send(msg);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
               // mailSetting.CurrentMailSetting.SendAsync(msg);
               // mailSetting.CurrentMailSetting.SendAsync(msg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                // throw new Exception(ex.InnerException.Message);

            }

        }

        private void SendAsync(SmtpClient emailClient, MailMessage GetEmail)
        {
            //string Token = "Mail Sent..";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    emailClient.Send(GetEmail);
                }
                catch(Exception ex) {
                    throw ex;
                }
            });
        }
        
        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, "^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$");
        }

        #endregion
    }
}