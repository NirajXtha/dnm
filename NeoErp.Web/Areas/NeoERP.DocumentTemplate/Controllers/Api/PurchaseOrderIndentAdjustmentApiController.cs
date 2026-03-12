using NeoERP.DocumentTemplate.Service.Interface.PurchaseIndentOrderAdjustment;
using NeoERP.DocumentTemplate.Service.Models.PurchaseOrderIndent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class PurchaseOrderIndentAdjustmentApiController : ApiController
    {
        private IPurchaseIndentAdjustment _purchaseIndentAdjustment;
        private IPurchaseOrderAdjustment _purchaseOrderAdjustment;

        public PurchaseOrderIndentAdjustmentApiController(IPurchaseIndentAdjustment purchaseIndentAdjustment,IPurchaseOrderAdjustment purchaseOrderAdjustment)
        {
            this._purchaseIndentAdjustment = purchaseIndentAdjustment;
            this._purchaseOrderAdjustment = purchaseOrderAdjustment;
        }

        #region INDENT ADJUSTMENT

        [HttpGet]
        public List<IndentAdjustmentDoc> GetIndentAdjustmentDoc(string tableName)
        {
            List<IndentAdjustmentDoc> soiModel = new List<IndentAdjustmentDoc>();
            try
            {
                soiModel = _purchaseIndentAdjustment.GetDocForIndentAdjustment(tableName);
                return soiModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpPost]
        public List<IndentAdjustViewModel> GetAllPurchaseIndentAdjustment(string document="", string fromDate="", string toDate="", string indentFilter="")
        {
            List<IndentAdjustViewModel> indentViewModel = new List<IndentAdjustViewModel>();
            var param = new IndentSearchParam
            {
                Document = document,
                FromDate = fromDate,
                ToDate = toDate,
                IndentFilter = indentFilter
            };

            try
            {
                indentViewModel = _purchaseIndentAdjustment.GetAllPurchaseIndentAdjustment(param);
                return indentViewModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpPost]
        public string SaveIndentAdjustment(List<IndentAdjustViewModel> modelList)
        {
            var saveResponse = _purchaseIndentAdjustment.SaveIndentAdjustment(modelList);
            return saveResponse;
        }

        #endregion


        #region ORDER ADJUSTMENT

        [HttpGet]
        public List<OrderAdjustmentDoc> GetOrderAdjustmentDoc(string tableName)
        {
            List<OrderAdjustmentDoc> soiModel = new List<OrderAdjustmentDoc>();
            try
            {
                soiModel = _purchaseOrderAdjustment.GetDocForOrderAdjustment(tableName);
                return soiModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpPost]
        public List<OrderAdjustViewModel> GetAllPurchaseOrderAdjustment(string document = "", string fromDate = "", string toDate = "", string indentFilter = "")
        {
            List<OrderAdjustViewModel> orderViewModel = new List<OrderAdjustViewModel>();
            var param = new IndentSearchParam
            {
                Document = document,
                FromDate = fromDate,
                ToDate = toDate,
                IndentFilter = indentFilter
            };

            try
            {
                orderViewModel = _purchaseOrderAdjustment.GetAllPurchaseOrderAdjustment(param);
                return orderViewModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpPost]
        public string SaveOrderAdjustment(List<OrderAdjustViewModel> modelList)
        {
            var saveResponse = _purchaseOrderAdjustment.SaveOrderAdjustment(modelList);
            return saveResponse;
        }

        #endregion
    }
}
