using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NeoErp.LOC.Services.Services;
using NeoErp.LOC.Services.Models;

namespace NeoERP.LOC.Controllers.Api
{
    public class SettlementInvoiceController : ApiController
    {
        private ISettlementInvoiceService _iSettlementInvoiceService { get; set; }

        public SettlementInvoiceController(ISettlementInvoiceService _settlementInvoiceService)
        {
            this._iSettlementInvoiceService = _settlementInvoiceService;
        }
        public HttpResponseMessage getAllSettlementInvoice()
        {
            try
            {
                var result = _iSettlementInvoiceService.GetAllSettlementInvoices();
                return Request.CreateResponse<List<SettlementInvoiceModel>>(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex });
            }
        }
        [HttpPost]
        public HttpResponseMessage getSettlementInvoiceByID(string id)
        {
            try
            {
                var result = _iSettlementInvoiceService.GetSettlementInvoicesByID(id);
                return Request.CreateResponse<List<SettlementInvoiceModel>>(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex });
            }
        }

        public List<SettlementInvocieddlModel> getInvoiceType()
        {
            var result = _iSettlementInvoiceService.GetInvoice();
            return result;
        }
        public List<SettlementInvocieddlModel> getInvoiceTypeByFilter(string filter)
        {
            var result = _iSettlementInvoiceService.GetInvoiceByFilter(filter);
            return result;
        }
        [HttpPost]
        public HttpResponseMessage saveSettlementInvoice(SettlementInvoiceModel SettlementInvoices)
        {
            try
            {
                var result = _iSettlementInvoiceService.SaveSettlementInvoices(SettlementInvoices);
                return Request.CreateResponse<string>(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex });
            }
        }
        [HttpPost]
        public HttpResponseMessage UpdateSettlementInvoice(SettlementInvoiceModel SettlementInvoices)
        {
            try
            {
                var result = _iSettlementInvoiceService.UpdateSettlementInvoices(SettlementInvoices);
                return Request.CreateResponse<string>(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex });
            }
        }
        [HttpPost]
        public HttpResponseMessage DeleteSettlementInvoice(string pfiCode)
        {
            try
            {
                var result = _iSettlementInvoiceService.DeleteSettlementInvoices(pfiCode);
                return Request.CreateResponse<string>(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex });
            }
        }
        [HttpPost]
        public HttpResponseMessage RemoveFile(LcUploadFileModels model)
        {

            try
            {
                _iSettlementInvoiceService.RemoveFile(model);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
    }
}
