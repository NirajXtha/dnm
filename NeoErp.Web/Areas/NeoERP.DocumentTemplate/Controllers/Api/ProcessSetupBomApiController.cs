using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models.ProcessSetupBom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class ProcessSetupBomApiController : ApiController
    {
        private IProcessSetupBom _processSetupBom;
        public ProcessSetupBomApiController(IProcessSetupBom processSetupBom)
        {
            this._processSetupBom = processSetupBom;
        }

        [HttpGet]
        public List<ProcessSetupBomModel> GetAllProcessCategoryRoutine()
        {
            List<ProcessSetupBomModel> bomModel = new List<ProcessSetupBomModel>();
            try
            {
                bomModel = _processSetupBom.GetAllProcessCategoryRoutine();
                return bomModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<ProcessCatRoutineForDDL> GetAllProcessForDDL()
        {
            var processDDLData = new List<ProcessCatRoutineForDDL>();
            try
            {
                processDDLData = _processSetupBom.GetAllProcessForDDL();
                return processDDLData;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<ProcessTypeCodeModel> GetAllProcessTypeCode()
        {
            List<ProcessTypeCodeModel> ptcModel = new List<ProcessTypeCodeModel>();
            try
            {
                ptcModel = _processSetupBom.GetAllProcessTypeCode();
                return ptcModel;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<BillAndOutputMaterialModel> GetBillOfMaterialList(string processCode = "0")
        {
            var bomList = new List<BillAndOutputMaterialModel>();
            try
            {
                bomList = _processSetupBom.GetBillOfMaterialsList(processCode.Replace("\"", ""));
                return bomList;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }
        }



        [HttpGet]
        public List<BillAndOutputMaterialModel> GetOutputMaterialList(string processCode = "0")
        {
            var omList = new List<BillAndOutputMaterialModel>();
            try
            {
                omList = _processSetupBom.GetOutputMaterialsList(processCode.Replace("\"", ""));
                return omList;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<BomRoutineModel> GetRoutineBasedOnProcessCode(string processCode = "0", string searchText = "")
        {
            List<BomRoutineModel> bomRoutineModel = new List<BomRoutineModel>();
            try
            {


                if (processCode == null)
                {
                    processCode = "0";
                }
                bomRoutineModel = _processSetupBom.GetRoutineByProcessCode(processCode, searchText);

                return bomRoutineModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<ProcessPeriodModal> GetProcessPeriod()
        {
            try
            {
                var periodData = _processSetupBom.GetProcessPeriod();
                return periodData;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public List<ProcessMuCodeModel> GetProcessMuCodeList()
        {
            var processMuList = new List<ProcessMuCodeModel>();
            try
            {
                processMuList = _processSetupBom.GetProcessMuCodeList();
                return processMuList;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ProcessResposponseForRoutine SaveProcessCategoryRoutine(ProcessCategoryRoutineSaveModel model)
        {
            var saveResponse = _processSetupBom.SaveProcessCategoryRoutine(model);
            return saveResponse;
        }


        [HttpPost]
        public IHttpActionResult ValidationIndexItemCode(RoutineDetailSaveModel model)
        {
            try
            {
                var result = _processSetupBom
                    .CheckValidationIndexItemCodeAlreadyUsedInAnotherRoutune(model, Convert.ToBoolean(model.IsShowMsgOnly));

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // For system errors
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        public IHttpActionResult SaveRoutineDetailSetup(RoutineDetailSaveModel model)
        {
            try
            {
                var routineResponse = _processSetupBom.SaveRoutineDetailSetup(model);
                return Ok(routineResponse);
            }
            catch (ValidationException ex)
            {
                // For business validation errors
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // For system errors
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public List<ProcessLocationModal> GetAllLocation()
        {
            var allLocation = _processSetupBom.GetAllLocation();
            return allLocation;
        }



        [HttpGet]
        public object GetAssignedResounceWithParticularRoutine(string processCode, string categoryType = "")
        {
            var allLocation = _processSetupBom.AssignedResounceWithParticularRoutine(processCode, categoryType);
            return allLocation;
        }

        [HttpGet]
        public object GetAssignedResounceUsingPrtclurLocation(decimal desiredQty, string location_code, string categoryType = "", string formCode = "", string voucherNo = "")
        {
            var allLocation = _processSetupBom.GetAssignedResounceUsingPrtclurLocation(desiredQty, location_code, categoryType, formCode, voucherNo);
            return allLocation;
        }



        //[HttpGet]
        //public string[] GetAllItemForInputOutput()
        //{
        //    var allItem = _processSetupBom.GetAllItemForInputOutput();
        //    string[] arr = allItem.Select(x => x.ITEM_EDESC).ToArray();
        //    return arr;
        //}

        [HttpGet]
        public List<ProcessItemModal> GetAllItemForInputOutput()
        {
            var allItem = _processSetupBom.GetAllItemForInputOutput();
            return allItem;
        }


        [HttpGet]
        public string GetChildProcessCode(string processCode)
        {
            if (processCode == null) return "0";
            var chilPCode = _processSetupBom.GetChildProcessCode(processCode);
            return chilPCode;
        }

        [HttpGet]
        public ProcessRoutineDetail GetChildProcessDetail(string processCode)
        {
            if (processCode == null) return new ProcessRoutineDetail();
            else
            {
                var childProcessDetail = _processSetupBom.GetChildProcessDetail(processCode);
                return childProcessDetail;
            }
        }

        [HttpGet]
        public object GetResourceList(string resourceType = "")
        {
            var reslutList = _processSetupBom.GetResourceList(resourceType);
            return reslutList;
        }


        [HttpGet]
        public object GetUnitList()
        {
            var reslutList = _processSetupBom.GetUnitList();
            return reslutList;
        }


        [HttpPost]
        public object SaveRoutineAndResourceMapping(MappingRoutineResourceModel model)
        {
            var result = _processSetupBom.SaveRoutineResourceMappingSetup(model);
            return result;
        }



        [HttpPost]
        public object SaveRoutineAndResourceMappingTest(MappingRoutineResourceModelTest1 model)
        {
            //var result = _processSetupBom.SaveRoutineResourceMappingSetup(model);
            return "";
        }


        [HttpGet]
        public object GetUnderLocationDetail(string process_code)
        {
            var result = _processSetupBom.GetUnderLocationDetailData(process_code);
            return result;
        }



        [HttpPost]
        public IHttpActionResult DeleteProcessSetup(string processCode)
        {
            try
            {
                var result = _processSetupBom.DeleteProcessSetup(processCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
