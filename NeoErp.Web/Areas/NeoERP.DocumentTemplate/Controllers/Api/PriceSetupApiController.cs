using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class PriceSetupApiController : ApiController
    {
        private IPriceSetup _priceSetup;

        public PriceSetupApiController(IPriceSetup priceSetup)
        {
            this._priceSetup = priceSetup;
        }


        [HttpGet]
        public List<PriceSetupModel> GetAllItemWithName()
        {
            List<PriceSetupModel> itemModel = new List<PriceSetupModel>();
            try
            {
                itemModel = _priceSetup.ListAllItemWithName();
                return itemModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<PriceSetupModel> GetItemByCompany(string selectedCompany)
        {
            List<PriceSetupModel> itemModel = new List<PriceSetupModel>();
            try
            {
                if (string.IsNullOrEmpty(selectedCompany)) return itemModel = null;
                else itemModel = _priceSetup.GetItemByCompany(selectedCompany);
                return itemModel;
            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpPost]
        public string SaveUpdatedCell(SaveModelForPriceSetup saveModel)
        {
            try
            {
                var savedResponse = _priceSetup.SaveUpdatedCell(saveModel);
                return savedResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<string> GetAllSavedItemsName()
        {
            try
            {
                var allSavedItem = _priceSetup.GetAllSavedItemsName();
                return allSavedItem;
            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public Tuple<List<PriceSetupModel>,MasterField> GetItemToEdit(string selectedPriceName)
        {
            try
            {
                var itemToEdit = _priceSetup.GetItemToEdit(selectedPriceName);
                return itemToEdit;
            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
    }
}
