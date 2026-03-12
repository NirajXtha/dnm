using NeoErp.Core.Helpers;
using NeoErp.Core.Infrastructure;
using NeoErp.Data;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace NeoErp.Core.Services.Scheduler
{
    public class EmailJob: IJob
    {
        //private IMessageService _messageService;
        //public EmailJob(IMessageService messageService)
        //{
        //    this._messageService = messageService;
        //}
        public void Execute(IJobExecutionContext context)
        {


            var siteName = ConfigurationManager.AppSettings["baseUrl"] != null? ConfigurationManager.AppSettings["baseUrl"]:string.Empty ;
            if(!string.IsNullOrEmpty(siteName))
            {
                var client = new WebClient();
                client.DownloadString(string.Format("{0}/api/common/SendMail", siteName));
            }


        }
    }

    public class EmailJobDaily : IJob
    {
        //private IMessageService _messageService;
        //public EmailJob(IMessageService messageService)
        //{
        //    this._messageService = messageService;
        //}
        public void Execute(IJobExecutionContext context)
        {


            var siteName = ConfigurationManager.AppSettings["baseUrl"] != null ? ConfigurationManager.AppSettings["baseUrl"] : string.Empty;
            if (!string.IsNullOrEmpty(siteName))
            {
                var client = new WebClient();
                client.DownloadString(string.Format("{0}/api/common/SendDaily", siteName));
            }


        }
    }

}

