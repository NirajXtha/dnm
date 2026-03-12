using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Models.Common;
using NeoErp.Data;

namespace NeoErp.Services
{
    public class MailControlService : IMailControl
    {
       // private IMailControl _mailControl;
        private IDbContext _dbContext;
        public MailControlService(IDbContext dbContext)
        {
           // _mailControl = mailcontrol;
            _dbContext = dbContext;
        }

        public List<MailListModel> AllMailList()
        {
            string query = @"select MESSAGE_QUEUE_CODE,GUID,EMAIL_TO,EMAIL_BCC,EMAIL_CC,ATTACHMENT_FLAG,MESSAGE_FLAG,CREATED_BY,CREATED_DATE,MODIFY_BY,MODIFY_DATE,MESSAGE,SUBJECT from WEB_SCHEDULARMAIL_QUEUE";
            return _dbContext.SqlQuery<MailListModel>(query).ToList();
            
        }

   
    }
}