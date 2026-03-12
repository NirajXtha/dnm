using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace NeoErp.Core.Services.Scheduler
{
    public class SchedularEmail : IJob
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
                client.DownloadString(string.Format("{0}/api/Scheduler/SendMail", siteName));
            }


        }
    }
}