using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class OrderDispatchApiController : ApiController
    {
        private IOrderDispatch _orderDispatch;

        public OrderDispatchApiController(IOrderDispatch orderDispatch)
        {
            this._orderDispatch = orderDispatch;
        }

        #region DISPATCH ORDER MANAGEMENT API


        [HttpGet]
        public List<DispatchDocument> GetAllDocument()
        {
            List<DispatchDocument> documentModel = new List<DispatchDocument>();
            try
            {
                documentModel = _orderDispatch.ListAllDocument();
                return documentModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<DispatchDocument> GetDocumentsByBranches(string branchCodeList)
        {
            List<DispatchDocument> documentModel = new List<DispatchDocument>();
            try
            {
                documentModel = _orderDispatch.ListDocumentByBranches(branchCodeList);
                return documentModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public List<OrderDispatchModel> FindDataToDispatch(SearchParameter searchParameter)
        {
            List<OrderDispatchModel> dispatchModel = new List<OrderDispatchModel>();
            try
            {
                //searchParameter.FromDate = "17-Jul-2018";
                dispatchModel = _orderDispatch.FindAllDataToDispatch(searchParameter);
                return dispatchModel;
            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpPost]
        public string SaveDispatchOrder(List<OrderDispatchModel> model)
        {
            try
            {
                var dispatchResult = _orderDispatch.SaveDispatchedOrder(model);
                return dispatchResult;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public int? GenerateDispatchNo()
        {
            try
            {
                var dispatchNo = _orderDispatch.GenerateDispatchNo();
                return dispatchNo;
            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public string GetDispatcher()
        {
            try
            {
                var dispatcher = _orderDispatch.GetDispatcher();
                return dispatcher;
            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpPost]
        public List<OrderDispatchModel> GetAllPlannedReport(SearchParameter searchParameter)
        {

            try
            {
                var plannedReport = _orderDispatch.GetAllDispatchPlannedReport(searchParameter);
                return plannedReport;

            }catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        #endregion

        
        #region LOADING SLIP API 

              
        [HttpGet]
        public List<OrderDispatchModel> GetAllDispatchForLoadingSlip(string filter)
        {
            try
            {
                string transactionDate = string.Empty; 
                if (filter == "TODAY")
                {
                    transactionDate = DateTime.Now.ToString("dd-MMM-yyyy");

                }
                else if (filter == "YESTERDAY")
                {
                    transactionDate = DateTime.Now.AddDays(-1).ToString("dd-MMM-yyyy");
                }
                else if (filter == "LASTSEVENDAYS")
                {
                    transactionDate = DateTime.Now.AddDays(-7).ToString("dd-MMM-yyyy");
                }
                else
                {
                    transactionDate = null;
                }
                var plannedReport = _orderDispatch.GetAllDispatchForLoadingSlip(transactionDate);
                return plannedReport;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<VehicleRegistrationModel> GetRegisteredVehicle(string entryDate = "")
        {
            try
            {
                var registeredVehicleList = _orderDispatch.GetAllRegisteredVehicleToDispatch(entryDate);
                return registeredVehicleList;
            }catch(Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage GenerateLoadingSlip(LoadingSlipModal modal)
        {
            try
            {

                var result = this._orderDispatch.GenerateLoadingSlip(modal);
                var data = this._orderDispatch.GetLoadingSlipPrintListByDispatchNo(modal.DISPATCH_NO);
                if (result == "Insertbutzore")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK,data=data });
                }
                else if(result== "ERROR")
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError , data = data }); }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK, data = data });
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region LOADING SLIP PRINTER

        [HttpGet]
        public List<LoadingSlipModalForPrint> GetLoadingSlipPrintList(string dispatchNo="0")
        {
            List<LoadingSlipModalForPrint> lSModel = new List<LoadingSlipModalForPrint>();
            try
            {
                if (dispatchNo == "0")
                {
                    lSModel = _orderDispatch.GetLoadingSlipPrintList();
                }
                else
                {
                  lSModel = _orderDispatch.GetLoadingSlipPrintListByDispatchNo(dispatchNo);
                }
               
                return lSModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        #endregion
    }
}
