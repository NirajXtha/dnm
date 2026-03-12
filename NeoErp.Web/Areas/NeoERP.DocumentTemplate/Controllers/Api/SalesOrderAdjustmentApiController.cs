using NeoERP.DocumentTemplate.Service.Interface.SalesOrderAdjustment;
using NeoERP.DocumentTemplate.Service.Models.SalesOrderIndent;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class SalesOrderAdjustmentApiController : ApiController
    {
        private ISalesOrderAdjustment _salesOrderAdjustment;

        public SalesOrderAdjustmentApiController(ISalesOrderAdjustment salesOrderAdjustment)
        {
            this._salesOrderAdjustment = salesOrderAdjustment;
        }

        [HttpGet]
        public List<SalesOrderIndentDocument> GetSalesOrderDocument(string tableName)
        {
            List<SalesOrderIndentDocument> soiModel = new List<SalesOrderIndentDocument>();
            try
            {
                soiModel = _salesOrderAdjustment.GetDocForSalesOrderAdjustment(tableName);
                return soiModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }



        [HttpPost]
        public List<SalesOrderAdjustViewModel> GetAllSalesOrderAdjustment(string document = "", string fromDate = "", string toDate = "", string indentFilter = "")
        {
            List<SalesOrderAdjustViewModel> orderViewModel = new List<SalesOrderAdjustViewModel>();
            var param = new OrderSearchParams
            {
                Document = document,
                FromDate = fromDate,
                ToDate = toDate,
                IndentFilter = indentFilter
            };

            try
            {
                orderViewModel = _salesOrderAdjustment.GetAllSalesOrderAdjustment(param);
                return orderViewModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpPost]
        public string SaveSalesOrderAdjustment(List<SalesOrderAdjustViewModel> modelList)
        {
            var saveResponse = _salesOrderAdjustment.SaveSalesOrderAdjustment(modelList);
            return saveResponse;
        }


    }
}
