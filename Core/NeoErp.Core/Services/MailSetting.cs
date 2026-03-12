using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace NeoErp.Core.Services
{
    public class MailSetting:IMailSetting
    {
        public virtual SmtpClient CurrentMailSetting
        {
            get
            {
                var smtp = new SmtpClient();
                return smtp;
            }
            set
            {
            }
        }
    }
}