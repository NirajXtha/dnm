using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NeoErp.Core.Services;
using NeoErp.Core.Models.CustomModels;

namespace NeoErp.Controllers
{
    public class MainControlController : Controller
    {
        public IMessageService _MessageService { get; set; }
        public MainControlController(IMessageService messageservice)
        {
            this._MessageService = messageservice;

        }
        // GET: MainControl
        public ActionResult SendMail()
        {
            return View();
        }

        

            //send mail
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendMailNew(MailListModel model)
        {
            if (ModelState.IsValid)
            {
                _MessageService.SendMail(model);
                return RedirectToAction("SendMail");
            }
            return View(model);
        }
        //get mailList in grid view
        public JsonResult AllMailList()
        {
            List<MailListModel> MailList = _MessageService.AllMailList().ToList();
            //return MailList;
            return Json(MailList, JsonRequestBehavior.AllowGet);
        }
        //update
        [ValidateInput(false)]
        public string UpdateMailList(MailListModel modal)
        {
            var updateMail = _MessageService.UpdateMailList(modal);
            return "success";

        }
    }
}