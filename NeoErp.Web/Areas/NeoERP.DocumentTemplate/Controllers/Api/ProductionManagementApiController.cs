using NeoERP.DocumentTemplate.Service.Interface.ProductionManagement;
using NeoERP.DocumentTemplate.Service.Models.ProductionManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;


namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class ProductionManagementApiController : ApiController
    {
        private IProductionManagement _productionManagement;

        public ProductionManagementApiController(IProductionManagement productionManagement)
        {
            this._productionManagement = productionManagement;
        }




        [HttpGet]
        public object GetProductionPlanningList([FromUri] ProductionPlanningFilterParamsModel requestParams)
        {
            try
            {

                var result = _productionManagement.GetAllProductionPlanningList(requestParams);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> GetProductTreeStructureListAsync()
        {
            try
            {
                var result = await _productionManagement.GetProductTreeStructureListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> GetParticularProductItemsAsync(string itemCode)
        {
            try
            {
                var result = await _productionManagement.GetParticularProductListAsync(itemCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public IHttpActionResult GetParticularProductItemsDetailsAsync(string itemCode)
        {
            try
            {
                var result = _productionManagement.GetProductItemDetailsAsync(itemCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public IHttpActionResult GetNewPlanCode()
        {
            try
            {
                var result = _productionManagement.GetNewPlanCode();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public IHttpActionResult GetRawPackingMaterialListAsync(string itemCode)
        {
            try
            {
                var result = _productionManagement.GetRowPackingMaterialAfterVerianceInfoInsert();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpPost]
        public IHttpActionResult PrepareRowMaterialPackingBasedOnCalcAsync([FromBody] ItemAndQtyForPrepareRawQtyModel model)
        {
            try
            {
                var result = _productionManagement.PrepareRowMaterialBasedOnCalcAndInsertAsync(model.ItemWithQtyList, model.RequestedQty, model.PlanNo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        //[HttpGet]
        //public async Task<IHttpActionResult> PrepareRowMaterialPackingBasedOnCalcAsync(string itemCode, decimal requestQty, string planNo)
        //{
        //    try
        //    {
        //        var result = await _productionManagement.PrepareRowMaterialBasedOnCalcAndInsertAsync(itemCode, requestQty, planNo);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.StackTrace);
        //    }
        //}

        [HttpGet]
        public async Task<IHttpActionResult> GetOrderListOfRelatedItem(string itemCode, string orderNo = "")
        {
            try
            {
                var result = await _productionManagement.GetOrderListOfSelectedItemCodeOrOrderNo(itemCode, orderNo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetOrderListByPlanDetailForEdit(string planNo)
        {
            try
            {
                var result = await _productionManagement.GetOrderListForEditData(planNo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetProductionPipeLineDataList(string itemCode, string planNo, string orderNo = "")
        {
            try
            {
                var result = await _productionManagement.GetProductionPipeDataList(itemCode, planNo, orderNo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetResourceDataList()
        {
            try
            {
                var result = await _productionManagement.GetResourceDataList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpPost]
        public IHttpActionResult InsertOrderPlanProcess([FromBody] OrderPlanProcess model)
        {
            try
            {
                dynamic errors = null;
                if (model == null)
                {
                    return BadRequest("Invalid input.");
                }

                // ✅ Manual model validation (required in Web API)
                var context = new ValidationContext(model, null, null);
                var validationResults = new List<ValidationResult>();
                // Key change: Set validateAllProperties to true
                bool isValid = Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true);

                if (!isValid)
                {
                    errors = validationResults.Select(v => new
                    {
                        PropertyName = string.Join(", ", v.MemberNames),
                        ErrorMessage = v.ErrorMessage
                    }).ToList();

                    return Content(HttpStatusCode.BadRequest, new
                    {
                        Message = "Validation failed.",
                        Errors = errors
                    });
                }

                // Example: Call a service/repository layer method to insert the record
                var result = _productionManagement.InsertOrderPlanProcess(model);
                // Return result or status
                return Ok(result); // or Created/NoContent/etc.
            }
            catch (Exception ex)
            {
                var exceptionType = ex.GetType().ToString();
                if (exceptionType == "System.ComponentModel.DataAnnotations.ValidationException")
                {
                    return Content(HttpStatusCode.BadRequest, new
                    {
                        Message = "Validation failed.",
                        Errors = ex.Message
                    });
                }

                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult FormCodeMappingSave([FromBody] SelectedFormDetails model)
        {
            try
            {


                if (model == null)
                {
                    return BadRequest("Invalid input.");
                }

                var sendToObj = new ProductionPreference()
                {
                    REQUISITION_FORM_CODE = model.RequisitionFormCodeValue,
                    INDENT_FORM_CODE = model.IndendFormCodeValue,

                    REQ_FROM_LOCATION_CODE = model.RequisitionFromLocationValue,
                    REQ_TO_LOCATION_CODE = model.RequisitionToLocationValue,

                    IND_FROM_LOCATION_CODE = model.IndentFromLocationValue,
                    IND_TO_LOCATION_CODE = model.IndentToLocationValue
                };

                var result = _productionManagement.InsertProductionPreference(sendToObj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetParticularProductListUsingSearchText(string search_text = "")
        {
            try
            {
                var result = await _productionManagement.GetParticularProductListUsingSearchTextAsync(search_text);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetPlanDetailsFoEdit(string plan_no)
        {
            try
            {
                var result = await _productionManagement.GetPlanDetailsForEdit(plan_no);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpPut]
        public IHttpActionResult UpateOrderPlanProcess([FromBody] OrderPlanProcess model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid input.");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                   .Where(v => v.Value.Errors.Count > 0)
                   .SelectMany(v => v.Value.Errors
                       .Select(e => new
                       {
                           PropertyName = v.Key,
                           ErrorMessage = e.ErrorMessage,
                           Exception = e.Exception?.Message
                       }))
                   .ToList();

                    return Content(HttpStatusCode.BadRequest, new
                    {
                        Message = "Validation failed.",
                        Errors = errors
                    });
                }

                // Example: Call a service/repository layer method to insert the record
                var result = _productionManagement.UpdateOrderPlanProcess(model);
                // Return result or status
                return Ok(result); // or Created/NoContent/etc.
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        public IHttpActionResult GetOrderListData(string searchText)
        {
            try
            {
                var result = _productionManagement.GetProductionOrderList(searchText);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> GetBatchTransectionInfo(string itemCode, string orderNo, string planNo)
        {
            try
            {
                var result = await _productionManagement.GetBatchTransectionInfo(itemCode, orderNo, planNo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpDelete]
        public IHttpActionResult DeleteVarianceInfo(string planNo)
        {
            try
            {
                if (!string.IsNullOrEmpty(planNo) && !string.IsNullOrWhiteSpace(planNo))
                {
                    var result = _productionManagement.DeleteVarianceInfo(planNo);
                    return Ok(result);
                }
                else
                {
                    return Ok("");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public IHttpActionResult GetProductionPreferences()
        {
            try
            {
                // Call the service layer method
                var result = _productionManagement.GetProductionPreferences();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log or throw exception as per your pattern
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public IHttpActionResult GetFormMappingListForPreferencesSetup()
        {
            try
            {
                // Call the service layer method
                var result = _productionManagement.GetFormMappingList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log or throw exception as per your pattern
                throw new Exception(ex.StackTrace);
            }
        }


        [HttpGet]
        public IHttpActionResult GetIndentFormMappingListForPreferencesSetup()
        {
            try
            {
                // Call the service layer method
                var result = _productionManagement.GetIndentFormMappingList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log or throw exception as per your pattern
                throw new Exception(ex.StackTrace);
            }
        }









    }
}
