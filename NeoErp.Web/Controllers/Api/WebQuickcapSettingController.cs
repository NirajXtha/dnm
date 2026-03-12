using NeoErp.Core.Models.CustomModels.SettingsEntities;
using NeoErp.Core.Services.QuickCapSettingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace NeoErp.Controllers.Api
{
    public class WebQuickcapSettingController : ApiController
    {
        IQuickCapSetting _quickCapSetting;
        public WebQuickcapSettingController(IQuickCapSetting _quickCapSetting) {
            this._quickCapSetting = _quickCapSetting;
        }

        // Get quick cap list
        [HttpPost]
        public HttpResponseMessage GetQuickCap()
        {
            try
            {
                var List = _quickCapSetting.GetAllQuickCap();
                if (List != null && List.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Table is Found"), DATA = List, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, new { MESSAGE = string.Format("No Found"), STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage GetUserForQuickCap(int ID)
        {
            try
            {
                var List = _quickCapSetting.GetUserForQuickCapByID(ID);
                if (List != null && List.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Table is Found"), DATA = List, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, new { MESSAGE = string.Format("No Found"), STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        //string USERNO, int ID
        [HttpPost]
        public HttpResponseMessage AddBulkUserForQuickCap(QuickCapSettingEntities quickCap)
        {
            
            try
            {
                var count = _quickCapSetting.AddBulkUserForQuickCap(quickCap.USERNO, quickCap.ID);
                if (count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Table is Found"), DATA = count, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, new { MESSAGE = string.Format("No Found"), STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public List<QuickCapSettingEntities> GetAllQuickCapSettings()
        {
            var count = _quickCapSetting.GetAllQuickCap();
            return count.ToList(); 
        }
        
    }
}
