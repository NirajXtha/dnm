 using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NeoErp.LOC.Services.Services;

namespace NeoERP.LOC.Controllers.Api
{
    public class CommercialInvoiceController : ApiController
    {
        private ICInvoiceService _cinvoice { get; set; }

        public CommercialInvoiceController(ICInvoiceService cinvoice)
        {
            this._cinvoice = cinvoice;
        }


        public IHttpActionResult getItemsByLCNumber(string lcnumber)
        {
            try
            {
                var list = _cinvoice.GetItemsByLCNumber(lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetLogisticPlan(string lcnumber)
        {
            try
            {
                var list = _cinvoice.GetLogisticPlanList(lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        public IHttpActionResult GetLogisticItems(string lcnumber)
        {
            try
            {
                var list = _cinvoice.GetLogisticItemsList(lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        
        public IHttpActionResult getAllCommercialInvoice()
        {
            try
            {
                var list = _cinvoice.GetAllCommInvoice();
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }


        public IHttpActionResult getAllCommercialInvoiceFilter(string lcnumber)
        {
            try
            {
                var list = _cinvoice.GetAllCommInvoiceFilter(lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetAllLcIpPurchaseOrder(string filters)
        {
            try
            {
                var list = _cinvoice.GetAllLcNumbers(filters);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult GetAllLcIpPurchaseOrderfilter(string filters)
        {
            try
            {
                var list = _cinvoice.GetAllLcNumbersfilter(filters);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        
        public IHttpActionResult GetAllInvoiceNumbers(string filters, string lcnumber)
        {
            try
            {
                var list = _cinvoice.GetAllInvoiceNumbers(filters, lcnumber);
                return Ok(list);

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }



        //public IHttpActionResult CreateCommercialInvoice(CommercialInvoiceModel cidetails)
        //{
        //    var invoicenumberexist = "";
        //    if (cidetails.LC_TRACK_NO == 0)
        //    {
        //        invoicenumberexist = _cinvoice.InvoiceNumberExist(cidetails.INVOICE_NUMBER, "create", cidetails.INVOICE_CODE);
        //    }
        //    else
        //    {
        //        if (cidetails.TEMP_INVOICE_CODE == 0)
        //        {
        //            invoicenumberexist = _cinvoice.InvoiceNumberExist(cidetails.INVOICE_NUMBER, "edit", cidetails.INVOICE_CODE);

        //        }
        //        else
        //        {
        //            invoicenumberexist = _cinvoice.InvoiceNumberExist(cidetails.INVOICE_NUMBER, "edit", cidetails.TEMP_INVOICE_CODE);
        //        }

        //    }
        //    try
        //    {
        //        if (invoicenumberexist == null)
        //        {
        //            var result = "OK";
        //          //  var result = _cinvoice.CreateCommercialInvoice(cidetails);
        //            return Ok(result);
        //        }
        //        else
        //        {
        //            return Content(HttpStatusCode.NotModified, "Invoice Number Already Exist.");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.NotFound, ex.Message);
        //    }
        //}

        public IHttpActionResult CreateCommercialInvoice(MultiCommercialInvoiceModel multiCommercialInvoiceModel)
        {
            var invoicenumberexist = "";
            if (multiCommercialInvoiceModel.LC_TRACK_NO == 0)
            {
                invoicenumberexist = _cinvoice.InvoiceNumberExist(multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_NUMBER, "create", multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_CODE);
            }
            else
            {
                //if (multiCommercialInvoiceModel.CommercialInvoiceData[0].TEMP_INVOICE_CODE == 0)
                //{
                //    invoicenumberexist = _cinvoice.InvoiceNumberExist(multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_NUMBER, "edit", multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_CODE);
                //}
                //else
                //{
                //    invoicenumberexist = _cinvoice.InvoiceNumberExist(multiCommercialInvoiceModel.CommercialInvoiceData[0].INVOICE_NUMBER, "edit", multiCommercialInvoiceModel.CommercialInvoiceData[0].TEMP_INVOICE_CODE);
                //}

            }
            try
            {
                if (invoicenumberexist == null || invoicenumberexist=="")
                {
                    //var result = "";
                    var result = _cinvoice.CreateCommercialInvoice(multiCommercialInvoiceModel);
                    return Ok(result);
                }
                else
                {
                    return Content(HttpStatusCode.NotModified, "Invoice Number Already Exist.");
                }

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }



        public IHttpActionResult getAllCommercialItemsList(string filter)
        {
            try
            {
                var result = _cinvoice.GetAllCommercialItemsList(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public IHttpActionResult getDetailByInvoiceNo(string invoiceno)
        {
            try
            {
                var result = _cinvoice.GetDetailByInvoiceNo(invoiceno);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult RemoveCiImage(LcImageModels imageremovedetails)
        {
            try
            {
                _cinvoice.RemoveCiImage(imageremovedetails);
                return Ok();
           
               
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        //[HttpPost]
        //public IHttpActionResult UpdateQuantity(Items cdetail)
        //{
        //    try
        //    {
        //        _cinvoice.UpdateQuantity(cdetail);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.NotFound, ex.Message);
        //    }
        //}

        [HttpPost]
        public HttpResponseMessage UpdateQuantity(Items cdetail)
        {
            var result = _cinvoice.UpdateQuantity(cdetail);
            //if (result == "Exceeded")
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "EXCEEDED", STATUS_CODE = (int)HttpStatusCode.OK });
            //}
            //else if(result == "Success")
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "FAILED", STATUS_CODE = (int)HttpStatusCode.OK });
            //}
            try
            {
                if (result == "Exceeded")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "EXCEEDED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else if (result == "Success")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "FAILED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }

            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]
        public IHttpActionResult LoadCIBylcnumber(string lcnumber)
        {
            try
            {
                var result = _cinvoice.LoadCIBylcnumber(lcnumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }
        //
        [HttpGet]
        public IHttpActionResult EditCommercialInvoice(string lotNumber, string invoiceNumber, string lcNumber)
        {
            return Ok(_cinvoice.EditCommercialInvoice(lotNumber, invoiceNumber, lcNumber));
        }
        //

        public IHttpActionResult getAllHistoryCommercialInvoiceList(string lctrackno)
        {
            try
            {
                var list = _cinvoice.getAllHistoryCommercialInvoiceList(lctrackno);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public List<InvoiceNumberModels> GetAllInvoiceNumberByFilter(string filter)
        {
            return _cinvoice.GetAllInvoiceNumberByFilter(filter);
        }
        [HttpPost]
        public IHttpActionResult CreateCIItemDO(List<CIDOModel> cIDOModel)
        {
            try
            {
              _cinvoice.CreateCIDONumber(cIDOModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpPost]
        public List<CIDOModel> GetCIItemDOByItemCode(CIDOModel cIDOModel)
        {
            string LC_TRACK_NO = cIDOModel.LC_TRACK_NO;
            string INVOICE_CODE = cIDOModel.INVOICE_CODE;
            string ITEM_CODE = cIDOModel.ITEM_CODE;
            return _cinvoice.GetCIItemDOByItemCode(LC_TRACK_NO, INVOICE_CODE, ITEM_CODE);
            
        }
    
        
    }
}
