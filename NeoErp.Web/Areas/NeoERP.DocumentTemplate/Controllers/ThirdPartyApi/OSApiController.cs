using NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.ThirdPartyApi
{
    public class OSApiController : ApiController
    {

        private IOSPreferenceSetup _ossetupservice { get; set; }
        public OSApiController(IOSPreferenceSetup ossetupservice)
        {
            this._ossetupservice = ossetupservice;
        }


        #region Preference Setup
        
        [HttpPost]
        public HttpResponseMessage SavePreferenceSetupOS(OS_PREFERENCE_SETUP model)
        {
            try
            {
                var result = _ossetupservice.SavePreferenceSetup(model);
                if (result == "INSERTED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "INVALIDINSERTED")
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        MESSAGE = "INVALIDINSERTED",
                        STATUS_CODE = (int)HttpStatusCode.Conflict
                    });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdatePreferenceSetupOS(OS_PREFERENCE_SETUP model)
        {
            try
            {
                var result = _ossetupservice.UpdatePreferenceSetup(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpGet]
        public List<OS_PREFERENCE_SETUP> GetPreferenceSetup()
        {

            var result = this._ossetupservice.GetPreferenceSetup();

            return result;


        }

        [HttpGet]
        public HttpResponseMessage getPreffitem(string prefId)
        {
            try
            {
                var result = this._ossetupservice.getPreffitem(prefId);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK, DATA = result });

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        #endregion
    }
}
