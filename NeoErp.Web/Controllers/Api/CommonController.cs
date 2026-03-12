using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Plugins;
using NeoErp.Core.Services;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Models.Common;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class CommonController : ApiController
    {
        private IMenuModel _menuService;
        private IControlService _controlService;
        private ISalesDashboardService _salesService;
        private IMessageService _messageService;
        private IPluginFinder _pluginFinder;
        private ISettingService _settingService;
        private IWorkContext _workContext;
        public CommonController(
            IMenuModel menuService,
            IControlService controlService,
            ISalesDashboardService salesService,
             IPluginFinder pluginFinder,
            IMessageService messageService, ISettingService settingservice, IWorkContext workContext)
        {
            this._menuService = menuService;
            this._controlService = controlService;
            this._salesService = salesService;
            this._messageService = messageService;
            this._pluginFinder = pluginFinder;
            this._settingService = settingservice;
            this._workContext = workContext;
        }


        [HttpGet]
        public HttpResponseMessage MenuList()
        {
            var menus = _menuService.GetMenuList();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, menus);
            return response;
        }

        [HttpGet]
        public List<DateFilterModel> GetDateFilters(string fiscalYear, string textToAppend = "", bool appendText = false)
        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var result = this._controlService.GetDateFilters(FincalYear, textToAppend, appendText).OrderByDescending(q => q.SortOrder).ToList();
            return result;
        }

        [HttpGet]
        public List<DateFilterModel> GetDateFiltersWithWeek(string fiscalYear, string textToAppend = "", bool appendText = false)
        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var result = this._controlService.GetDateFiltersWithWeek(FincalYear, textToAppend, appendText).OrderByDescending(q => q.SortOrder).ToList();
            return result;

        }


        public List<DateFilterModel> GetEnglishDateFilters(string fiscalYear, string textToAppend = "", bool appendText = false)
        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var result = this._controlService.GetEnglishDateFilters(FincalYear, textToAppend, appendText).OrderByDescending(q => q.SortOrder).ToList();
            return result;
        }

        [HttpPost]
        public SubmitResponse Save(FileUpload model)
        {
            var response = new SubmitResponse();
            string message = string.Empty;
            response.Success = FileHelper.SaveReportFile(model.base64, model.fileName, out message);
            response.Message = message;
            return response;
        }
        /// <summary>
        /// this is calling while send mail from report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        //[HttpPost]
        //public SubmitResponse SaveMessage(FileMailAttachment model)
        //{
        //    var response = new SubmitResponse();
        //    if (model == null)
        //        throw new ArgumentNullException("parameter not supplied");

        //    var messageQueue = new MessageQueueModel()
        //    {
        //        AttachmentFile = FileHelper.ReportFilePath + "/" + model.FileName,
        //        AttachmentFlag = ((char)YesNo.Yes).ToString(),
        //        EmailTo = model.Email,
        //        Message = model.Message,
        //        MessageFlag = ((char)MessageProcessStatus.Draft).ToString(),
        //        Subject = model.Subject
        //    };

        //    try
        //    {
        //        var result = this._messageService.CreateMessageQueue(messageQueue);
        //        if (result > 0)
        //        {
        //            response.Message = "Message has been successfully queued in Mail Queue";
        //            response.Success = true;
        //            return response;
        //        }

        //        response.Message = "Failed to queue the message";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = "Error occured while saving message " + ex.Message;
        //    }

        //    return response;
        //}
        [HttpPost]
        public SubmitResponse SaveMessage(FileMailAttachment model)
        {
            var response = new SubmitResponse();
            if (model == null)
                throw new ArgumentNullException("parameter not supplied");


            try
            {
                // Convert FileMailAttachment to MailListModel
                var mailListModel = new MailListModel()
                {
                    EMAIL_TO = model.Email,
                    SUBJECT = model.Subject,
                    MESSAGE = model.Message,
                    ATTACHMENT_FILE = FileHelper.ReportFilePath + "/" + model.FileName,
                    ATTACHMENT_FLAG = ((char)YesNo.Yes).ToString()
                };

                // Send mail directly using IMessageService
                var result = this._messageService.SendMailDerect(mailListModel);

                if (result > 0)
                {
                    // Clean up the temporary report file after successful send
                    // to prevent stale files accumulating in App_Data/ReportFiles
                    try
                    {
                        var tempFilePath = System.Web.HttpContext.Current.Server.MapPath(mailListModel.ATTACHMENT_FILE);
                        if (System.IO.File.Exists(tempFilePath))
                            System.IO.File.Delete(tempFilePath);
                    }
                    catch { /* non-critical: ignore cleanup errors */ }

                    response.Message = "Email has been sent successfully";
                    response.Success = true;
                    return response;
                }

                response.Message = "Failed to send the email";
            }
            catch (Exception ex)
            {
                response.Message = "Error occured while sending email: " + ex.Message;
            }

            return response;
        }

        [HttpGet]
        public void SendMail()
        {
            this._messageService.SendMail();
        }
        [HttpGet]
        public void SendDaily()
        {
            this._messageService.SendSchedularMailDaily();
        }
        [HttpGet]
        public List<PluginDescriptor> InstalledPuginListForGlobalMenu()
        {
            var result = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0).ToList();
            return result;
        }

        [HttpPost]
        public void Print()
        {

        }

        #region For Advanced Search Filter start
        [HttpGet]
        public HttpResponseMessage GetCurrencyType()
        {
            var menus = _salesService.GetCurrencyType();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, menus);
            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetBrandType()
        {
            var menus = _salesService.GetBrandType();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, menus);
            return response;
        }
        #endregion end

        #region Start Default dashboard page
        [HttpPost]
        public SubmitResponse SaveDefaultDashboard(string Path)
        {
            var response = new SubmitResponse();
            response.Success = true;
            response.Message = "Default Dashboard is saved";
            var filename = $"{_workContext.CurrentUserinformation.User_id.ToString()}{_workContext.CurrentUserinformation.company_code}";
            try
            {
                // var setting = _settingService.LoadSetting<UserDashboardSetting>(filename);
                var userDashboardSetting = new UserDashboardSetting();
                userDashboardSetting.UserId = _workContext.CurrentUserinformation.User_id;
                userDashboardSetting.DefaultPath = Path;
                if (_settingService.DeleteSetting(filename))
                {
                    _settingService.SaveSetting<UserDashboardSetting>(userDashboardSetting, filename);
                }


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion
    }
}
