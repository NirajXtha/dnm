using NeoErp.Models.ThirdPartyModels;
using NeoErp.Services.ThirdPatryServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.ThirdPartyApi
{
    public class CrmController : ApiController
    {
        private ICrmService _crmService;
        public CrmController(ICrmService crmService)
        {
            this._crmService = crmService;
        }
        public List<BranchModel> GetBranches()
        {
            return this._crmService.GetBranchList();
        }
        public List<ItemModel> GetItems()
        {
            return this._crmService.GetItemList();
        }
        public List<CategoryModel> GetCategories()
        {
            return this._crmService.GetCategoryList();
        }
        public List<UnitModel> GetUnits()
        {
            return this._crmService.GetUnitList();
        }
        public List<StockModel> GetStocks()
        {
            return this._crmService.GetStockList();
        }
        [HttpPost]
        public HttpResponseMessage SaveOrder(SalesOrderModel model)
        {
            var result = _crmService.SaveSalesOrder(model);
            if (result != "Error")
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = "SUCCESS", SalesNumber = result });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = "ERROR", Message = "Something went wrong. Please try again" });
        }
        [HttpGet]
        public HttpResponseMessage GetInvoiceDetails(string SalesNumber, string TransactionRefNo = null)
        {
            try
            {
                var result = _crmService.FetchSODetails(SalesNumber, TransactionRefNo);
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = "SUCCESS", Data = result });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = "ERROR", Message = "Something went wrong. Please try again" });
            }
        }
    }
}