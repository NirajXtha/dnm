using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace NeoERP.Planning.Controllers.Api
{
    public class FrequencyApiController : ApiController
    {
        private IPlan _planService;
        public FrequencyApiController(
            IPlan planService)
        {
           this._planService = planService;
        }      

        [HttpPost]
        public HttpResponseMessage CreateFreq(FrequencyModels frequency)
        {
            if (ModelState.IsValid)
            {             
                if (_planService.checkifexists(frequency))
                {                    
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Alreadyexists", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    
                    var message=_planService.AddUpdateFrequencies(frequency);
                    if (message == "ExistsButDeleted")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ExistsButDeleted", STATUS_CODE = (int)HttpStatusCode.OK });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                    }
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public List<FrequencyModels> GetAllFrequencies()
        {
            return _planService.getAllFrequency();
        }


        [HttpGet]
        public HttpResponseMessage DeleteFrequency(int TIME_FRAME_CODE)
        {
            _planService.deleteFrequency(TIME_FRAME_CODE);
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }
    }
}
