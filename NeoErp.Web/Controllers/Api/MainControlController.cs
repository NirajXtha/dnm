using NeoErp.Core.Models.CustomModels;
using NeoErp.Models.Common;
using NeoErp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class MainControlController : ApiController
    {
        public IMailControl _mailControl;// { get; set; }
       
        public MainControlController(IMailControl mailcontrol)
        {
            this._mailControl = mailcontrol;
        }

        // [HttpPost]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public List<MailListModel> AllMailList()
        {
            var MailList = _mailControl.AllMailList();
            return MailList;
        }
    }
}
