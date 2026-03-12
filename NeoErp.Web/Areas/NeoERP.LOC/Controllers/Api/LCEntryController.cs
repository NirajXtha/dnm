using NeoErp.LOC.Services.Models;
using NeoErp.LOC.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using static NeoERP.LOC.Models.EnumList;

namespace NeoERP.LOC.Controllers.Api
{
    public class LCEntryController : ApiController
    {
        private ILcEntryService _lcentryservice { get; set; }

        public LCEntryController(ILcEntryService lcentryservice)
        {
            this._lcentryservice = lcentryservice;
        }

        public List<PerformaInvoiceModel> getAllLcIpPurchaseOrder(string filter)
        {
            return _lcentryservice.GetAllLcIpPurchaseOrder(filter);
        }

        [HttpGet]
        public List<string> GetCalenderDaysType()
        {
            var calenderType = Enum.GetNames(typeof(CalenderDaysType)).ToList();
            return calenderType;
        }
        public IHttpActionResult createLcEntry(LcEntryModels lcentry)
        {
            try
            {
                string Message = "";
                bool lcnumberexist;
                if (lcentry.LC_TRACK_NO == 0)
                {
                    lcnumberexist = _lcentryservice.LCNumberExist(lcentry.PINVOICE_CODE,  "create", lcentry.LOC_CODE, lcentry.LC_NUMBER, out Message);
                }
                else
                {
                    lcnumberexist = _lcentryservice.LCNumberExist(lcentry.PINVOICE_CODE, "edit", lcentry.LOC_CODE, lcentry.LC_NUMBER, out Message);
                }
                if (lcnumberexist == false)
                {
                    var result = _lcentryservice.CreateLCEntry(lcentry);
                    return Ok(result);
                }
                else
                {
                    return Ok(Message);
                }

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult getAllLcSuppliers(string filter)
        {
            try
            {
                var result = _lcentryservice.GetAllLcSuppliersFromSynergy(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult lcDetailsList(string tracknumber)
        {
            try
            {
                var result = _lcentryservice.LCDetailList(tracknumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult saveStatus(string status, string lctrack)
        {
            try
            {
                _lcentryservice.SaveStatus(status, lctrack);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public LCItemsModels getItemDetailsByOrderCode(string PinvoiceCode)
        {
            return _lcentryservice.GetAllItemsByOrderCode(PinvoiceCode);
        }

        [HttpGet]
        public List<LcEntryModels> GetAllLcList()
        {
            return _lcentryservice.getAllLcList();
        }

        [HttpGet]
        public List<LcEntryModels> GetAllLcListFilter(string perfomainvoice, string lcnumber)
        {
            return _lcentryservice.GetAllLcListFilter(perfomainvoice, lcnumber);
        }

        [HttpGet]
        public List<string> GetDeliveryPlaceType()
        {
            var shipmentLoadType = Enum.GetNames(typeof(DeliveryPlaceType)).ToList();
            return shipmentLoadType;
        }

        #region Global Dropdowns by Filter

        public List<LCTermModels> GetAllTermsByFilter(string filter)
        {
            return _lcentryservice.getTermsListByFlter(filter);
        }

        public List<LCBankModels> GetAllBanksByFilter(string filter)
        {
            return _lcentryservice.getBanksListByFlter(filter);
        }

     
        
        public IHttpActionResult GetAllSupplierBanksByFilter(string filter)
        {
            try
            {
                var result = _lcentryservice.getSupplierBanksListByFlter(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        public IHttpActionResult GetAllSupplierByFilter(string filter)
        {
            try
            {
                var result = _lcentryservice.getSupplierListByFlter(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }

        }

        public List<LCPTermModels> GetAllPTermsByFilter(string filter)
        {
            return _lcentryservice.getPaymentTermsListByFlter(filter);
        }

        public List<LCStatusModels> GetAllStatusByFilter(string filter)
        {
            return _lcentryservice.getStatusListByFlter(filter);
        }

        public List<LCNumberModels> GetAllLCNumberByFilter(string filter)
        {
            return _lcentryservice.GetAllLCNumberByFilter(filter);
        }

        public List<ItemNameModels> GetAllItemNameByFilter(string filter)
        {
            return _lcentryservice.GetAllItemNameByFilter(filter);
        }



        #endregion

        [HttpPost]
        public IHttpActionResult RemoveLcImage(LcImageModels imageremovedetails)
        {
            try
            {
                var result = _lcentryservice.RemoveLCImage(imageremovedetails);
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }


        }
        [HttpPost]
        public void insertIntoMasterTransaction( string lcNumber,string trackNumber)
        {
            this._lcentryservice.insertIntoMasterTransaction(lcNumber, trackNumber);
        }
    }
}
