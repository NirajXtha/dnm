
using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Quotation;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models.ProcessSetupBom;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace NeoERP.DocumentTemplate.Service.Services
{
    public class ProcessSetupBomService : IProcessSetupBom
    {
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public ProcessSetupBomService(IWorkContext workContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public List<ProcessSetupBomModel> GetAllProcessCategoryRoutine()
        {
            try
            {
                var bomSetup = new List<ProcessSetupBomModel>();
                var bomSetupForTree = new List<ProcessSetupBomModel>();
                var bomQuery = $@"SELECT DISTINCT MPS.PROCESS_CODE as PROCESS_CODE , MPS.PROCESS_EDESC as PROCESS_EDESC,MPS.PROCESS_TYPE_CODE as PROCESS_TYPE_CODE,
                                 MPS.PROCESS_FLAG as PROCESS_FLAG,LEVEL,MPS.PRE_PROCESS_CODE as PRE_PROCESS_CODE,TO_CHAR(MPS.LOCATION_CODE) AS LOCATION_CODE,ILS.LOCATION_EDESC,TO_CHAR(MPS.PRIORITY_ORDER_NO) AS PRIORITY_ORDER_NO,MPS.REMARKS FROM MP_PROCESS_SETUP MPS LEFT JOIN IP_LOCATION_SETUP ILS ON ILS.LOCATION_CODE = MPS.LOCATION_CODE AND ILS.COMPANY_CODE = MPS.COMPANY_CODE WHERE MPS.DELETED_FLAG='N' 
                                  AND MPS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  CONNECT BY PRIOR PROCESS_CODE=PRE_PROCESS_CODE START WITH PRE_PROCESS_CODE='00'";
                //var bomQuery = $@"SELECT DISTINCT MPS.PROCESS_CODE as PROCESS_CODE , MPS.PROCESS_EDESC as PROCESS_EDESC,MPS.PROCESS_TYPE_CODE as PROCESS_TYPE_CODE,
                //                  MPS.PROCESS_FLAG as PROCESS_FLAG,MPS.PRE_PROCESS_CODE as PRE_PROCESS_CODE FROM MP_PROCESS_SETUP MPS WHERE MPS.DELETED_FLAG='N' 
                //                  AND MPS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                bomSetup = _objectEntity.SqlQuery<ProcessSetupBomModel>(bomQuery).ToList();

                // var processTree = generateProcessTree(bomSetup, bomSetupForTree, "00");
                return bomSetup;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting bill or payment tree : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        private List<ProcessSetupBomModel> generateProcessTree(List<ProcessSetupBomModel> model, List<ProcessSetupBomModel> processNodes, string preProcessCode)
        {
            try
            {
                foreach (var processes in model.Where(x => x.PRE_PROCESS_CODE == preProcessCode))
                {
                    var processNodesChild = new List<ProcessSetupBomModel>();
                    processNodes.Add(new ProcessSetupBomModel()
                    {
                        PROCESS_EDESC = processes.PROCESS_EDESC,
                        PROCESS_CODE = processes.PROCESS_CODE,
                        PRE_PROCESS_CODE = processes.PRE_PROCESS_CODE,
                        PROCESS_FLAG = processes.PROCESS_FLAG,
                        HAS_BRANCH = processes.PROCESS_FLAG == "R" ? false : true,
                        ITEMS = processes.PROCESS_FLAG != "R" ? generateProcessTree(model, processNodesChild, processes.PRE_PROCESS_CODE) : null,
                    });
                }
                return processNodes;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public List<ProcessTypeCodeModel> GetAllProcessTypeCode()
        {
            try
            {
                string processTypeQuery = $@"SELECT DISTINCT MPTC.PROCESS_TYPE_CODE , MPTC.PROCESS_TYPE_EDESC FROM MP_PROCESS_TYPE_CODE MPTC WHERE MPTC.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                                           AND MPTC.DELETED_FLAG='N'";
                var processTypeData = _objectEntity.SqlQuery<ProcessTypeCodeModel>(processTypeQuery).ToList();
                return processTypeData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all process type code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<BillAndOutputMaterialModel> GetBillOfMaterialsList(string processCode)
        {
            try
            {
                //var billOfMaterialsList = new List<BillAndOutputMaterialModel>();
                //return billOfMaterialsList;
                if (processCode == "0")
                {
                    return new List<BillAndOutputMaterialModel>();
                }
                else
                {
                    //var billOfMatQuery = $@"SELECT TO_CHAR(MRIS.PROCESS_CODE) as PROCESS_CODE,MRIS.ITEM_CODE,MRIS.QUANTITY,MPC.PERIOD_CODE,MPC.PERIOD_EDESC,MRIS.MU_CODE as UNIT_CODE,
                    //                    MRIS.MU_CODE,IMC.MU_EDESC,IIMS.ITEM_EDESC,MPS.PROCESS_EDESC,MRIS.REMARKS as REMARK,'INPUT' as MODAL_INFO,TO_CHAR(MPS.INDEX_CAPACITY) as INDEX_CAPACITY,TO_CHAR(MPS.INDEX_TIME_REQUIRED) as INDEX_TIME_REQUIRED
                    //                    FROM MP_ROUTINE_INPUT_SETUP MRIS
                    //                    INNER JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE=MRIS.ITEM_CODE
                    //                    INNER JOIN MP_PROCESS_SETUP MPS ON MPS.PROCESS_CODE = MRIS.PROCESS_CODE
                    //                    INNER JOIN IP_MU_CODE IMC ON IMC.MU_CODE=MRIS.MU_CODE
                    //                    INNER JOIN MP_PERIOD_CODE MPC on MPS.INDEX_PERIOD_CODE = MPC.PERIOD_CODE
                    //                    WHERE MRIS.PROCESS_CODE='{processCode}' AND MRIS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                    //var billofMatData = _objectEntity.SqlQuery<BillAndOutputMaterialModel>(billOfMatQuery).ToList();
                    //return billofMatData;

                    //var billOfMatQuery = $@"SELECT a.ITEM_CODE,
                    //                                 a.ITEM_EDESC,
                    //                                 TO_CHAR(b.QUANTITY) as QUANTITY,
                    //                                 b.MU_CODE,
                    //                                 b.REMARKS
                    //                            FROM IP_ITEM_MASTER_SETUP a, MP_ROUTINE_INPUT_SETUP b
                    //                           WHERE     a.ITEM_CODE = b.ITEM_CODE
                    //                                 AND b.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                    //                                 AND b.DELETED_FLAG = 'N'
                    //                                 AND a.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                    //                                 AND a.DELETED_FLAG = 'N'
                    //                                 AND b.PROCESS_CODE = '{processCode}'
                    //                        ORDER BY b.SERIAL_NO, b.created_date";


                    var billOfMatQuery = $@"SELECT
                                                        a.ITEM_CODE,
                                                        a.ITEM_EDESC,
                                                        TO_CHAR(b.QUANTITY) AS QUANTITY,
                                                        b.MU_CODE,
                                                        b.REMARKS,
                                                        c.INDEX_PERIOD_CODE AS PERIOD_CODE,
                                                        (select PERIOD_EDESC from MP_PERIOD_CODE where PERIOD_CODE=c.INDEX_PERIOD_CODE and company_code='{_workContext.CurrentUserinformation.company_code}') as PERIOD_EDESC,
                                                        c.INDEX_CAPACITY,
                                                        c.INPUT_INDEX_ITEM_CODE
                                                    FROM
                                                        IP_ITEM_MASTER_SETUP a
                                                    JOIN
                                                        MP_ROUTINE_INPUT_SETUP b
                                                        ON a.ITEM_CODE = b.ITEM_CODE
                                                        AND a.COMPANY_CODE = b.COMPANY_CODE
                                                    JOIN
                                                        MP_PROCESS_SETUP c
                                                        ON b.PROCESS_CODE = c.PROCESS_CODE
                                                        AND b.COMPANY_CODE = c.COMPANY_CODE
                                                    WHERE
                                                        b.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                                        AND b.DELETED_FLAG = 'N'
                                                        AND a.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                                        AND a.DELETED_FLAG = 'N'
                                                        AND b.PROCESS_CODE = '{processCode}'
                                                    ORDER BY
                                                        b.SERIAL_NO, 
                                                        b.CREATED_DATE";

                    var billofMatData = _objectEntity.SqlQuery<BillAndOutputMaterialModel>(billOfMatQuery).ToList();
                    return billofMatData;
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting bill of materials list : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<BillAndOutputMaterialModel> GetOutputMaterialsList(string processCode)
        {
            try
            {
                if (processCode == "0")
                {
                    return new List<BillAndOutputMaterialModel>();
                }
                else
                {
                    //var billOfMatOutputQuery = $@"SELECT TO_CHAR(MROS.PROCESS_CODE) as PROCESS_CODE,MPC.PERIOD_CODE,MPC.PERIOD_EDESC,MROS.ITEM_CODE,MROS.VALUATION_FLAG,
                    //                    CASE WHEN MROS.OUTPUT_PERCENT IS NOT NULL THEN MROS.OUTPUT_PERCENT ELSE 0 END AS OUT_PUT,MROS.QUANTITY,MROS.MU_CODE as UNIT_CODE,
                    //                    MROS.MU_CODE,IIMS.ITEM_EDESC,MPS.PROCESS_EDESC,MROS.REMARKS as REMARK,IMC.MU_EDESC,'OUTPUT' as MODAL_INFO,TO_CHAR(MPS.INDEX_CAPACITY) as INDEX_CAPACITY,TO_CHAR(MPS.INDEX_TIME_REQUIRED) as INDEX_TIME_REQUIRED
                    //                    FROM MP_ROUTINE_OUTPUT_SETUP MROS
                    //                    INNER JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE=MROS.ITEM_CODE
                    //                    INNER JOIN MP_PROCESS_SETUP MPS ON MPS.PROCESS_CODE = MROS.PROCESS_CODE
                    //                    INNER JOIN IP_MU_CODE IMC ON IMC.MU_CODE=MROS.MU_CODE
                    //                    INNER JOIN MP_PERIOD_CODE MPC on MPS.INDEX_PERIOD_CODE = MPC.PERIOD_CODE
                    //                    WHERE MROS.PROCESS_CODE='{processCode}' AND MROS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                    //var billofMatOutputData = _objectEntity.SqlQuery<BillAndOutputMaterialModel>(billOfMatOutputQuery).ToList();
                    //return billofMatOutputData;

                    //var billOfMatOutputQuery = $@"SELECT a.item_code as ITEM_CODE,
                    //                                   a.item_edesc as ITEM_EDESC,
                    //                                   TO_CHAR (b.quantity) as QUANTITY,
                    //                                   b.output_percent as OUTPUT_PERCENT,
                    //                                   b.valuation_flag as VALUATION_FLAG,
                    //                                   b.mu_code as MU_CODE,
                    //                                   b.remarks as REMARKS,
                    //                                   (
                    //                                        SELECT INDEX_PERIOD_CODE  from MP_PROCESS_SETUP where Process_Code ='{processCode}'
                    //                                        and company_code=a.company_code
                    //                                   ) AS PERIOD_CODE
                    //                              FROM ip_item_master_setup a, mp_routine_output_setup b
                    //                             WHERE     a.item_code = b.item_code
                    //                                   AND b.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                    //                                   AND b.DELETED_FLAG = 'N'
                    //                                   AND a.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                    //                                   AND a.DELETED_FLAG = 'N'
                    //                                   AND b.process_code = '{processCode}'";

                    var billOfMatOutputQuery = $@"
                                               SELECT a.item_code AS ITEM_CODE,
                                                    a.item_edesc AS ITEM_EDESC,
                                                    TO_CHAR (b.quantity) as QUANTITY,
                                                    b.output_percent as OUTPUT_PERCENT,
                                                    b.output_percent as out_put,
                                                    b.valuation_flag,
                                                    b.mu_code AS MU_CODE,
                                                    b.remarks AS REMARKS,
                                                    p.index_period_code AS PERIOD_CODE,
                                                    (select PERIOD_EDESC from MP_PERIOD_CODE where PERIOD_CODE=p.index_period_code and company_code='{_workContext.CurrentUserinformation.company_code}') as PERIOD_EDESC,
                                                    p.index_time_required AS INDEX_TIME_REQUIRED
                                            FROM ip_item_master_setup a
                                            JOIN mp_routine_output_setup b
                                                 ON a.item_code = b.item_code
                                                AND a.company_code = '{_workContext.CurrentUserinformation.company_code}'
                                            JOIN mp_process_setup p
                                                 ON p.process_code = b.process_code
                                                AND p.company_code = '{_workContext.CurrentUserinformation.company_code}'
                                            WHERE a.company_code = '{_workContext.CurrentUserinformation.company_code}'
                                              AND b.company_code = '{_workContext.CurrentUserinformation.company_code}'
                                              AND a.deleted_flag = 'N'
                                              AND b.deleted_flag = 'N'
                                              AND b.process_code = '{processCode}'";

                    var billofMatOutputData = _objectEntity.SqlQuery<BillAndOutputMaterialModel>(billOfMatOutputQuery).ToList();


                    var tempQ = $@"select index_item_code from MP_PROCESS_SETUP where Process_code = '{processCode}' and company_code = '{_workContext.CurrentUserinformation.company_code}'";
                    var result = _objectEntity.SqlQuery<string>(tempQ).FirstOrDefault();

                    if (billofMatOutputData.Count > 0)
                    {
                        var tmpIndex = billofMatOutputData.FindLastIndex(x => x.ITEM_CODE == result);
                        billofMatOutputData[tmpIndex].OUTPUT_INDEX_ITEM_CODE = result;
                    }


                    return billofMatOutputData;
                }

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting out put materials list : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<BomRoutineModel> GetRoutineByProcessCode(string processCode, string searchText = "")
        {
            try
            {
                //string rountineByCodeQuery = $@"SELECT TO_CHAR(MPS.PROCESS_CODE) AS SHORT_CUT,MPS.PROCESS_EDESC AS PROCESS_DESCRIPTION,MPS.INPUT_INDEX_ITEM_CODE,IIMS.ITEM_EDESC AS ITEM_DESCRIPTION,CASE WHEN MPS.INDEX_CAPACITY IS NULL THEN 0 ELSE MPS.INDEX_CAPACITY END AS CAPACITY,
                //                                CASE WHEN  MPS.INDEX_MU_CODE IS NULL THEN 'N/A' ELSE  MPS.INDEX_MU_CODE END AS MU_CODE,ILS.LOCATION_EDESC 
                //                                from MP_PROCESS_SETUP MPS
                //                                INNER JOIN IP_ITEM_MASTER_SETUP IIMS on IIMS.ITEM_CODE = MPS.INPUT_INDEX_ITEM_CODE
                //                                INNER JOIN IP_LOCATION_SETUP ILS on ILS.LOCATION_CODE = MPS.LOCATION_CODE
                //                                WHERE MPS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and MPS.PRE_PROCESS_CODE like '{processCode}%'";

                string rountineByCodeQuery = $@"
                                                SELECT 
                                               TO_CHAR(MPS.PROCESS_CODE) AS SHORT_CUT,
                                               MPS.PROCESS_EDESC         AS PROCESS_DESCRIPTION,
                                               MPS.INPUT_INDEX_ITEM_CODE,
                                               IIMS.ITEM_EDESC           AS ITEM_DESCRIPTION,
                                               NVL(MPS.INDEX_CAPACITY, 0)     AS CAPACITY,
                                               NVL(MPS.INDEX_MU_CODE, 'N/A')  AS MU_CODE,
                                               ILS.LOCATION_EDESC,
                                               MPS.PRE_PROCESS_CODE, MPS.PROCESS_FLAG
                                        FROM MP_PROCESS_SETUP MPS
                                        LEFT JOIN IP_ITEM_MASTER_SETUP IIMS
                                               ON IIMS.ITEM_CODE   = MPS.INPUT_INDEX_ITEM_CODE
                                              AND IIMS.COMPANY_CODE = MPS.COMPANY_CODE
                                        INNER JOIN IP_LOCATION_SETUP ILS
                                               ON ILS.LOCATION_CODE = MPS.LOCATION_CODE AND ILS.COMPANY_CODE = MPS.COMPANY_CODE
                                        WHERE MPS.COMPANY_CODE      = '{_workContext.CurrentUserinformation.company_code}'
                                          AND MPS.DELETED_FLAG      = 'N'
                                          --AND MPS.PROCESS_FLAG      = 'R'
                                          AND MPS.PRE_PROCESS_CODE LIKE  '{processCode}%'";

                if (!string.IsNullOrEmpty(searchText))
                {

                    rountineByCodeQuery = $@"SELECT DISTINCT
                                            TO_CHAR(MPS.PROCESS_CODE) AS SHORT_CUT,
                                            MPS.PROCESS_EDESC AS PROCESS_DESCRIPTION,
                                            MPS.INPUT_INDEX_ITEM_CODE AS ITEM_CODE,
                                            IIMS.ITEM_EDESC AS ITEM_DESCRIPTION,
                                            COALESCE(MPS.INDEX_CAPACITY, 0) AS CAPACITY,
                                            COALESCE(MPS.INDEX_MU_CODE, 'N/A') AS MU_CODE,
                                            ILS.LOCATION_EDESC AS LOCATION,
                                            MPS.PRE_PROCESS_CODE, MPS.PROCESS_FLAG
                                        FROM MP_PROCESS_SETUP MPS
                                        INNER JOIN IP_ITEM_MASTER_SETUP IIMS
                                            ON IIMS.ITEM_CODE = MPS.INPUT_INDEX_ITEM_CODE
                                        INNER JOIN IP_LOCATION_SETUP ILS
                                            ON ILS.LOCATION_CODE = MPS.LOCATION_CODE
                                        WHERE MPS.DELETED_FLAG = 'N'
                                          AND MPS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                          AND(UPPER(MPS.PROCESS_EDESC) LIKE '{searchText.ToUpper()}%'
                                               OR UPPER(MPS.PROCESS_CODE) LIKE '{searchText.ToUpper()}%')
                                        ORDER BY MPS.PROCESS_EDESC";

                }
                var routineByCodeData = _objectEntity.SqlQuery<BomRoutineModel>(rountineByCodeQuery).ToList();
                return routineByCodeData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting routine of given process code {processCode}" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<ProcessCatRoutineForDDL> GetAllProcessForDDL()
        {
            try
            {
                //Code to assign root node id

                var maxProcessCodeQuery = $@"SELECT COUNT(PROCESS_CODE) + 1 FROM MP_PROCESS_SETUP WHERE PRE_PROCESS_CODE like '00%'";
                var newRootProcessCode = _objectEntity.SqlQuery<int>(maxProcessCodeQuery).FirstOrDefault().ToString();
                var processLenght = newRootProcessCode.Length;
                if (processLenght == 1)
                {
                    if (!newRootProcessCode.StartsWith("0"))
                    {
                        newRootProcessCode = 0 + newRootProcessCode;
                    }
                }




                string queryForDDL = $@"SELECT  PROCESS_CODE , PRE_PROCESS_CODE , PROCESS_EDESC , PROCESS_FLAG , PROCESS_TYPE_CODE  from MP_PROCESS_SETUP MPS";

                var processDDLData = _objectEntity.SqlQuery<ProcessCatRoutineForDDL>(queryForDDL).ToList();
                processDDLData.Add(new ProcessCatRoutineForDDL { PROCESS_CODE = newRootProcessCode, PRE_PROCESS_CODE = "00", PROCESS_EDESC = "Primary", PROCESS_FLAG = "C" });
                // processDDLData.Add(new ProcessCatRoutineForDDL { PRO})
                return processDDLData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting routine of given process code {processCode}" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public ProcessResposponseForRoutine SaveProcessCategoryRoutine(ProcessCategoryRoutineSaveModel model)
        {

            var newmastercode = "";
            try
            {
                // check before saving : 
                //var checkQuery = $@"SELECT PROCESS_EDESC FROM MP_PROCESS_SETUP MPS WHERE MPS.PROCESS_CODE='{model.PROCESS_CODE}'";
                //var checkData = _objectEntity.SqlQuery<string>(checkQuery).FirstOrDefault();
                //if (!string.IsNullOrEmpty(checkData))
                //{
                //    var checkDelete = $@"DELETE FROM MP_PROCESS_SETUP MPS WHERE MPS.PROCESS_CODE='{model.PROCESS_CODE}'";
                //    _objectEntity.ExecuteSqlCommand(checkDelete);
                //}
                var response = new ProcessResposponseForRoutine();
                //return response;

                if (model.PROCESS_FLAG == "CATEGORY")
                {
                    if (model.IS_EDIT)
                    {
                        model.PROCESS_FLAG = "C";
                        model.ROOT_UNDER = "00";
                        var sqlquery = $@"UPDATE MP_PROCESS_SETUP SET PROCESS_EDESC='{model.IN_ENGLISH}', PROCESS_NDESC='{model.IN_NEPALI}',PROCESS_TYPE_CODE='{model.PROCESS_TYPE}',PROCESS_FLAG='{model.PROCESS_FLAG}',REMARKS='{model.REMARK}',LOCATION_CODE='{model.LOCATION}',PRIORITY_ORDER_NO='{model.PRIORITY_NUMBER}' WHERE PROCESS_CODE = '{model.PROCESS_CODE}'";
                        var result = _objectEntity.ExecuteSqlCommand(sqlquery);
                        response.MESSAGE = "Successful";
                        response.PROCESS_FLAG = model.PROCESS_FLAG;
                        response.ROOT_UNDER = model.ROOT_UNDER;
                        return response;

                    }
                    else
                    {
                        model.PROCESS_FLAG = "C";
                        model.ROOT_UNDER = "00";
                        var saveQuery = $@"INSERT INTO MP_PROCESS_SETUP(PROCESS_CODE,PROCESS_EDESC,PROCESS_TYPE_CODE,PROCESS_FLAG,PRIORITY_ORDER_NO,DELETED_FLAG,PRE_PROCESS_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,LOCATION_CODE,REMARKS) 
                                       VALUES('{model.PROCESS_CODE}','{model.IN_ENGLISH}','{model.PROCESS_TYPE}','{model.PROCESS_FLAG}','{model.PRIORITY_NUMBER}','N','{model.ROOT_UNDER}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'{model.LOCATION}','{model.REMARK}')";
                        _objectEntity.ExecuteSqlCommand(saveQuery);
                        response.MESSAGE = "Successful";
                        response.PROCESS_FLAG = model.PROCESS_FLAG;
                        response.ROOT_UNDER = model.ROOT_UNDER;
                        return response;
                    }


                }
                else if (model.PROCESS_FLAG == "PROCESS")
                {
                    if (model.IS_EDIT)
                    {
                        model.PROCESS_FLAG = "P";
                        //model.ROOT_UNDER = "00";
                        var sqlquery = $@"UPDATE MP_PROCESS_SETUP SET PROCESS_EDESC='{model.IN_ENGLISH}', PROCESS_NDESC='{model.IN_NEPALI}',PROCESS_TYPE_CODE='{model.PROCESS_TYPE}',PROCESS_FLAG='{model.PROCESS_FLAG}',REMARKS='{model.REMARK}',LOCATION_CODE='{model.LOCATION}',PRIORITY_ORDER_NO='{model.PRIORITY_NUMBER}' WHERE PROCESS_CODE = '{model.PROCESS_CODE}'";
                        var result = _objectEntity.ExecuteSqlCommand(sqlquery);
                        response.MESSAGE = "Successful";
                        response.PROCESS_FLAG = model.PROCESS_FLAG;
                        response.ROOT_UNDER = model.ROOT_UNDER;
                        return response;

                    }
                    else
                    {
                        model.PROCESS_FLAG = "P";
                        var saveQuery = $@"INSERT INTO MP_PROCESS_SETUP(PROCESS_CODE,PROCESS_EDESC,PROCESS_TYPE_CODE,PROCESS_FLAG,PRIORITY_ORDER_NO,DELETED_FLAG,PRE_PROCESS_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,LOCATION_CODE,REMARKS) 
                                       VALUES('{model.PROCESS_CODE}','{model.IN_ENGLISH}','{model.PROCESS_TYPE}','{model.PROCESS_FLAG}','{model.PRIORITY_NUMBER}','N','{model.ROOT_UNDER}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'{model.LOCATION}','{model.REMARK}')";
                        _objectEntity.ExecuteSqlCommand(saveQuery);
                        response.MESSAGE = "Successful";
                        response.PROCESS_FLAG = model.PROCESS_FLAG;
                        response.ROOT_UNDER = model.ROOT_UNDER;
                        return response;
                    }

                }
                else
                {
                    if (model.IS_EDIT)
                    {
                        var pf = string.Empty;
                        if (model.PROCESS_FLAG == "PROCESS")
                        {
                            pf = "P";
                        }
                        if (model.PROCESS_FLAG == "ROUTINE")
                        {
                            pf = "R";
                        }
                        var updateProcess = $@"UPDATE MP_PROCESS_SETUP MPS SET MPS.MODIFY_DATE=SYSDATE,MPS.MODIFY_BY='{_workContext.CurrentUserinformation.login_code}',PROCESS_EDESC  ='{model.IN_ENGLISH}',PROCESS_NDESC='{model.IN_NEPALI}',PROCESS_TYPE_CODE='{model.PROCESS_TYPE}',PRIORITY_ORDER_NO ='{model.PRIORITY_NUMBER}',REMARKS='{model.REMARK}', PROCESS_FLAG='{pf}' WHERE MPS.PROCESS_CODE='{model.PROCESS_CODE}'";
                        _objectEntity.ExecuteSqlCommand(updateProcess);

                        response.MESSAGE = "Successful";
                        response.PROCESS_FLAG = model.PROCESS_FLAG;
                        response.ROOT_UNDER = model.ROOT_UNDER;
                        response.SAVED_MODAL = model;
                        return response;
                    }
                    else
                    {
                        model.PROCESS_FLAG = "R";

                        if (model.LOCATION != null)
                        {
                            var maxPreCode = "";
                            var maxprecodequery = $@"select (count(*) + 1) as MAXCODE from IP_LOCATION_SETUP where pre_location_code like '{model.LOCATION}' and company_code = '{_workContext.CurrentUserinformation.company_code}'";
                            maxPreCode = this._objectEntity.SqlQuery<int>(maxprecodequery).FirstOrDefault().ToString();
                            if (maxPreCode != null)
                            {
                                if (Convert.ToInt32(maxPreCode) <= 9)
                                {
                                    maxPreCode = "0" + maxPreCode.ToString();
                                }
                                if (Convert.ToInt32(maxPreCode) > 9)
                                {
                                    int maxPrecodePlus = Convert.ToInt32(maxPreCode) + 1;
                                    maxPreCode = maxPrecodePlus.ToString();
                                }
                            }
                            newmastercode = model.LOCATION + "." + maxPreCode;
                            var childsqlquery = $@"INSERT INTO IP_LOCATION_SETUP (LOCATION_CODE,
                                                    LOCATION_EDESC,LOCATION_NDESC,PRE_LOCATION_CODE,ADDRESS,
                                                    AUTH_CONTACT_PERSON,TELEPHONE_MOBILE_NO,EMAIL,FAX,
                                                    LOCATION_TYPE_CODE,GROUP_SKU_FLAG,REMARKS,COMPANY_CODE,
                                                    CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE,STORAGE_CAPACITY,MU_CODE,LOCATION_ID)
                                                    VALUES('{newmastercode}','{model.IN_ENGLISH}',' ','{model.LOCATION}',' ',' ',' '
                                                        ,' ',' ','','I','','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),
                                                         'N','{_workContext.CurrentUserinformation.branch_code}','','','')";
                            var insertchild = _objectEntity.ExecuteSqlCommand(childsqlquery);
                        }

                        var saveQuery = $@"INSERT INTO MP_PROCESS_SETUP(PROCESS_CODE,PROCESS_EDESC,PROCESS_TYPE_CODE,PROCESS_FLAG,PRIORITY_ORDER_NO,DELETED_FLAG,PRE_PROCESS_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,LOCATION_CODE,REMARKS) 
                                       VALUES('{model.PROCESS_CODE}','{model.IN_ENGLISH}','{model.PROCESS_TYPE}','{model.PROCESS_FLAG}','{model.PRIORITY_NUMBER}','N','{model.ROOT_UNDER}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'{newmastercode}','{model.REMARK}')";
                        _objectEntity.ExecuteSqlCommand(saveQuery);
                        response.MESSAGE = "Successful";
                        response.PROCESS_FLAG = model.PROCESS_FLAG;
                        response.ROOT_UNDER = model.ROOT_UNDER;
                        response.SAVED_MODAL = model;
                        return response;
                    }

                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving process setup : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        private string SaveInputOutMaterial(List<InputOutMaterialSaveModel> model)
        {

            try
            {
                string flag = "";
                foreach (var ml in model)
                {
                    if (ml.MODAL_INFO == "INPUT")
                    {
                        var materialSaveQuery = $@"INSERT INTO MP_ROUTINE_INPUT_SETUP(PROCESS_CODE,ITEM_CODE,QUANTITY,MU_CODE,REMARKS,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                                          VALUES('{ml.PROCESS_CODE}','{ml.ITEM_CODE}','{ml.QUANTITY}','{ml.UNIT_CODE}','{ml.REMARKS}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                        _objectEntity.ExecuteSqlCommand(materialSaveQuery);

                    }
                    else
                    {
                        //if (ml.VALUATION_FLAG == true)
                        //{
                        //    flag = "Y";
                        //}
                        //else
                        //{
                        //    flag = "N";
                        //}
                        var materialSaveQuery = $@"INSERT INTO MP_ROUTINE_OUTPUT_SETUP(PROCESS_CODE,ITEM_CODE,QUANTITY,OUTPUT_PERCENT,VALUATION_FLAG,MU_CODE,REMARKS,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                                         VALUES('{ml.PROCESS_CODE}','{ml.ITEM_CODE}','{ml.QUANTITY}','{ml.OUT_PUT}','{ml.VALUATION_FLAG}','{ml.UNIT_CODE}','{ml.REMARKS}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                        _objectEntity.ExecuteSqlCommand(materialSaveQuery);

                    }

                }
                return "Successfull";
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving input , output material : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        //public string CheckValidationIndexItemCodeAlreadyUsedInAnotherRoutune(RoutineDetailSaveModel routineModel, bool isShowContinueMsg = false)
        //{
        //    var outPutIndexItemCode = routineModel.RoutineDetail.OUTPUT_INDEX_ITEM;

        //    var validationQuery = $@"SELECT Process_Code, Process_EDESC, INDEX_ITEM_CODE
        //                                  FROM MP_PROCESS_SETUP
        //                                 WHERE INDEX_ITEM_CODE = '{outPutIndexItemCode}' AND Company_Code = '{_workContext.CurrentUserinformation.company_code}'";

        //    var validationResult = this._objectEntity.SqlQuery<MpProcessSetupModel>(validationQuery).FirstOrDefault();
        //    if (validationResult != null && validationResult.PROCESS_CODE != routineModel.RoutineDetail.ROUTINE_CODE)
        //    {

        //        if (isShowContinueMsg == true)
        //        {
        //            throw new ValidationException(
        //              $"This item has already been taken in another BOM ({validationResult.PROCESS_CODE}) {validationResult.PROCESS_EDESC}"
        //          );
        //        }
        //        else
        //        {
        //            throw new ValidationException(
        //              $"This item has already been taken in another BOM ({validationResult.PROCESS_CODE}) {validationResult.PROCESS_EDESC} .\nDo you want to continue?"
        //          );
        //        }


        //    }
        //    return string.Empty;
        //}


        public string CheckValidationIndexItemCodeAlreadyUsedInAnotherRoutune(RoutineDetailSaveModel routineModel, bool isShowOnlyMsg = false)
        {

            if (string.IsNullOrEmpty(routineModel.RoutineDetail.OUTPUT_INDEX_ITEM))
            {
                throw new ValidationException("INDEX ITEM Required.");
            }


            var outPutIndexItemCode = routineModel.RoutineDetail.OUTPUT_INDEX_ITEM;

            var validationQuery = $@"SELECT Process_Code, Process_EDESC, INDEX_ITEM_CODE
                                       FROM MP_PROCESS_SETUP
                                      WHERE INDEX_ITEM_CODE = '{outPutIndexItemCode}' AND Company_Code = '{_workContext.CurrentUserinformation.company_code}'";

            var result = this._objectEntity.SqlQuery<MpProcessSetupModel>(validationQuery).FirstOrDefault();
            if (result == null || result.PROCESS_CODE == routineModel.RoutineDetail.ROUTINE_CODE)
                return string.Empty;

            var baseMessage = $"This item has already been taken in another BOM ({result.PROCESS_CODE}) {result.PROCESS_EDESC}";

            if (isShowOnlyMsg)
                throw new ValidationException(baseMessage);

            throw new ValidationException(baseMessage + ".\nDo you want to continue?");
            return string.Empty;
        }


        public class MpProcessSetupModel
        {
            public string INDEX_ITEM_CODE { get; set; }
            public string PROCESS_CODE { get; set; }
            public string PROCESS_EDESC { get; set; }
        }


        public string SaveRoutineDetailSetup(RoutineDetailSaveModel routineModel)
        {
            try
            {


                CheckValidationIndexItemCodeAlreadyUsedInAnotherRoutune(routineModel, true);

                //var outPutIndexItemCode = routineModel.RoutineDetail.OUTPUT_INDEX_ITEM;

                //var validationQuery = $@"SELECT Process_Code, Process_EDESC, INDEX_ITEM_CODE
                //                          FROM MP_PROCESS_SETUP
                //                         WHERE INDEX_ITEM_CODE = '{outPutIndexItemCode}' AND Company_Code = '{_workContext.CurrentUserinformation.company_code}'";

                //var validationResult = this._objectEntity.SqlQuery<int>(validationQuery).ToList();
                //if (validationResult != null && validationResult.Count > 1)
                //{
                //    throw new ValidationException("Item Index already assigned to other routine");
                //}
                //else if (validationResult != null && validationResult.Count == 1)
                //{

                //}

                string inputResponse = string.Empty;
                string outputResponse = string.Empty;
                if (routineModel.InputModel.Count > 0)
                {
                    var process_code = routineModel.RoutineDetail.ROUTINE_CODE;
                    var deleteInput = $@"DELETE FROM MP_ROUTINE_INPUT_SETUP WHERE PROCESS_CODE='{process_code}'";
                    var diffRow = _objectEntity.ExecuteSqlCommand(deleteInput);

                    foreach (var item in routineModel.InputModel)
                    {
                        item.PROCESS_CODE = process_code;
                    }

                    inputResponse = SaveInputOutMaterial(routineModel.InputModel);

                }

                if (routineModel.OutputModel.Count > 0)
                {
                    var process_code = routineModel.RoutineDetail.ROUTINE_CODE;
                    var deleteOutput = $@"DELETE FROM MP_ROUTINE_OUTPUT_SETUP WHERE PROCESS_CODE='{process_code}'";
                    var diffRow = _objectEntity.ExecuteSqlCommand(deleteOutput);
                    foreach (var item in routineModel.OutputModel)
                    {
                        item.PROCESS_CODE = process_code;
                    }
                    outputResponse = SaveInputOutMaterial(routineModel.OutputModel);
                }


                var response = new ProcessResposponseForRoutine();
                var updateQuery = $@"UPDATE MP_PROCESS_SETUP MPS SET MPS.INPUT_INDEX_ITEM_CODE='{routineModel.RoutineDetail.INPUT_INDEX_ITEM}',MPS.INDEX_PERIOD_CODE='{routineModel.RoutineDetail.INPUT_IN_PERIOD}',MPS.INDEX_TIME_PERIOD_CODE='{routineModel.RoutineDetail.OUTPUT_IN_PERIOD}',MPS.INDEX_CAPACITY='{routineModel.RoutineDetail.INPUT_CAPACITY}',MPS.INDEX_TIME_REQUIRED='{routineModel.RoutineDetail.OUTPUT_CAPACITY}',MPS.INDEX_MU_CODE='{routineModel.RoutineDetail.INPUT_UNIT}',MPS.INDEX_TIME_MU_CODE='{routineModel.RoutineDetail.OUTPUT_UNIT}',MPS.INDEX_ITEM_CODE='{routineModel.RoutineDetail.OUTPUT_INDEX_ITEM}' WHERE MPS.PROCESS_CODE='{routineModel.RoutineDetail.ROUTINE_CODE}'";
                _objectEntity.ExecuteSqlCommand(updateQuery);
                response.MESSAGE = "Successful";

                return response.MESSAGE;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving routine detail setup : " + ex.StackTrace);
                throw;
            }
        }

        public List<ProcessMuCodeModel> GetProcessMuCodeList()
        {
            try
            {
                var muQuery = $@"SELECT IMC.MU_CODE , IMC.MU_EDESC FROM IP_MU_CODE IMC WHERE IMC.DELETED_FLAG='N' AND IMC.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var muData = _objectEntity.SqlQuery<ProcessMuCodeModel>(muQuery).ToList();
                return muData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting process mu/Unit Code: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<ProcessLocationModal> GetAllLocation()
        {
            try
            {
                string locationQuery = $@"SELECT DISTINCT ILS.LOCATION_CODE , ILS.LOCATION_EDESC FROM IP_LOCATION_SETUP ILS WHERE ILS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                                           AND ILS.DELETED_FLAG='N' AND GROUP_SKU_FLAG='G'";
                var locationData = _objectEntity.SqlQuery<ProcessLocationModal>(locationQuery).ToList();
                return locationData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all process type code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<ProcessItemModal> GetAllItemForInputOutput()
        {
            try
            {
                //string itemQuery = $@"SELECT DISTINCT  IIMS.ITEM_EDESC FROM IP_ITEM_MASTER_SETUP IIMS WHERE IIMS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                //                           AND IIMS.DELETED_FLAG='N'";
                string itemQuery = $@"SELECT DISTINCT IIMS.ITEM_CODE, IIMS.ITEM_EDESC,IIMS.INDEX_MU_CODE as MU_CODE,IIMS.INDEX_MU_CODE as MU_EDESC FROM IP_ITEM_MASTER_SETUP IIMS WHERE IIMS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                                           AND IIMS.DELETED_FLAG='N'";
                var itemData = _objectEntity.SqlQuery<ProcessItemModal>(itemQuery).ToList();
                return itemData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all item code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string GetChildProcessCode(string processCode)
        {
            try
            {
                var childProcessQuery = $@"SELECT PRE_PROCESS_CODE FROM MP_PROCESS_SETUP MPS WHERE MPS.PROCESS_CODE='{processCode}' and MPS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var pre_code = _objectEntity.SqlQuery<string>(childProcessQuery).FirstOrDefault();
                //var finalCodeQuery = $@"SELECT COUNT(PROCESS_CODE) + 1 FROM MP_PROCESS_SETUP MPS WHERE MPS.PRE_PROCESS_CODE='{processCode}' AND MPS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";


                var finalCodeQuery = $@"SELECT 
                                                MAX (
                                                   TO_NUMBER (
                                                      SUBSTR (PROCESS_CODE, LENGTH (PRE_PROCESS_CODE) + 2, 4)))
                                              + 1
                                      FROM MP_PROCESS_SETUP
                                     WHERE     (PRE_PROCESS_CODE = '{processCode}' OR PROCESS_CODE LIKE '{processCode}.%')
                                           AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";



                var finalCodeIn = _objectEntity.SqlQuery<int>(finalCodeQuery).FirstOrDefault();

                // var finalCode = processCode + ".0" + finalCodeIn;

                var finalCode = processCode + "." + finalCodeIn.ToString("D2");
                return finalCode;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting chile process code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public ProcessRoutineDetail GetChildProcessDetail(string processCode)
        {
            try
            {
                var detailQuery = $@"SELECT MPS.PROCESS_CODE,MPS.PROCESS_EDESC,MPS.PROCESS_TYPE_CODE,MPSC.PROCESS_TYPE_EDESC,MPS.PROCESS_FLAG,TO_CHAR(MPS.LOCATION_CODE) AS LOCATION_CODE,ILS.LOCATION_EDESC,ILS.PRE_LOCATION_CODE,TO_CHAR(MPS.PRIORITY_ORDER_NO) AS PRIORITY_ORDER_NO,MPS.REMARKS 
                                     FROM MP_PROCESS_SETUP MPS
                                     INNER JOIN MP_PROCESS_TYPE_CODE MPSC ON MPSC.PROCESS_TYPE_CODE = MPS.PROCESS_TYPE_CODE
                                     INNER JOIN IP_LOCATION_SETUP ILS ON ILS.LOCATION_CODE = MPS.LOCATION_CODE
                                 WHERE PROCESS_CODE='{processCode}'";
                var detailData = _objectEntity.SqlQuery<ProcessRoutineDetail>(detailQuery).FirstOrDefault();
                return detailData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting child process detail : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<ProcessPeriodModal> GetProcessPeriod()
        {
            try
            {
                var periodQuery = $@"SELECT MPC.PERIOD_CODE,MPC.PERIOD_EDESC,MPC.YEARLY_PERIOD_NO,MPC.YEARLY_DAYS_NO FROM MP_PERIOD_CODE MPC WHERE MPC.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var periodData = _objectEntity.SqlQuery<ProcessPeriodModal>(periodQuery).ToList();
                return periodData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting process period : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        // added by mahesh
        public List<AssignedRoutineResourceSetupModel> AssignedResounceWithParticularRoutine(string processCode, string categoryType = "")
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            try
            {
                string query = $@"SELECT 
                                        a.RESOURCE_CODE,
                                        a.RESOURCE_EDESC,
                                        TO_CHAR (b.QUANTITY) as QUANTITY,
                                        b.MU_CODE,
                                        imc.MU_EDESC,
                                        b.REMARKS,
                                        b.STANDARD_OUTPUT_QTY,
                                        b.STANDARD_INPUT_QTY
                                    FROM MP_RESOURCE_SETUP a
                                    JOIN MP_ROUTINE_RESOURCE_SETUP b
                                        ON a.RESOURCE_CODE = b.RESOURCE_CODE
                                    LEFT JOIN IP_MU_CODE imc
                                        ON imc.MU_CODE = b.MU_CODE
                                     WHERE     a.RESOURCE_CODE = b.RESOURCE_CODE
                                           AND b.COMPANY_CODE = '{companyCode}'
                                           AND b.DELETED_FLAG = 'N'
                                           AND a.COMPANY_CODE = '{companyCode}'
                                           AND a.DELETED_FLAG = 'N'
                                           AND imc.COMPANY_CODE = '{companyCode}'
                                           AND b.PROCESS_CODE = '{processCode}'";

                if (!string.IsNullOrEmpty(categoryType))
                {
                    query += $@" and b.CATEGORY_TYPE = '{categoryType}'";
                }

                var resulstList = _objectEntity.SqlQuery<AssignedRoutineResourceSetupModel>(query).ToList();
                return resulstList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all process type code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public object GetAssignedResounceUsingPrtclurLocation(decimal desiredQty, string location_code, string categoryType = "", string formCode = "", string voucherNo = "")
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var branchCode = _workContext.CurrentUserinformation.branch_code;

            string processCodeQuery = $@"select Process_Code from MP_PROCESS_SETUP where Location_Code='{location_code}' and company_code='{companyCode}'";
            var processCode = _objectEntity.SqlQuery<string>(processCodeQuery).FirstOrDefault();

            if (processCode == null) { throw new Exception("location code not valid!"); }

            try
            {
                //string query = $@"SELECT 
                //                        a.RESOURCE_CODE,
                //                        a.RESOURCE_EDESC,
                //                        TO_CHAR (b.QUANTITY) as QUANTITY,
                //                        b.MU_CODE,
                //                        imc.MU_EDESC,
                //                        b.REMARKS
                //                    FROM MP_RESOURCE_SETUP a
                //                    JOIN MP_ROUTINE_RESOURCE_SETUP b
                //                        ON a.RESOURCE_CODE = b.RESOURCE_CODE
                //                    LEFT JOIN IP_MU_CODE imc
                //                        ON imc.MU_CODE = b.MU_CODE
                //                     WHERE     a.RESOURCE_CODE = b.RESOURCE_CODE
                //                           AND b.COMPANY_CODE = '{companyCode}'
                //                           AND b.DELETED_FLAG = 'N'
                //                           AND a.COMPANY_CODE = '{companyCode}'
                //                           AND a.DELETED_FLAG = 'N'
                //                           AND b.PROCESS_CODE = '{processCode}'";
                //if (!string.IsNullOrEmpty(categoryType))
                //{
                //    query += $@" and b.CATEGORY_TYPE = '{categoryType}'";
                //}

                string query = $@"
                                        SELECT 
                                        a.RESOURCE_CODE,
                                        a.RESOURCE_EDESC,
                                        TO_CHAR(b.QUANTITY) AS QUANTITY,
                                        b.MU_CODE,
                                        imc.MU_EDESC,
                                        b.REMARKS,
                                        b.STANDARD_INPUT_QTY,
                                        b.STANDARD_OUTPUT_QTY,
                                        CASE 
                                            WHEN b.STANDARD_OUTPUT_QTY > 0 THEN 
                                                ROUND(('{desiredQty}' * b.STANDARD_INPUT_QTY) / b.STANDARD_OUTPUT_QTY, 2)
                                            ELSE 
                                                NULL
                                        END AS REQUIRED_INPUT_QTY";

                query += $@",  CASE
                                WHEN EXISTS (
                                    SELECT 1
                                    FROM MP_RESOURCE_DETAIL d
                                    WHERE d.RESOURCE_CODE = a.RESOURCE_CODE
                                      AND d.COMPANY_CODE = '{companyCode}'
                                      AND d.DELETED_FLAG = 'N'
                                      AND d.SERIAL_NO IS NOT NULL
                                      AND TRIM(d.SERIAL_NO) IS NOT NULL
                                ) THEN 'TRUE'
                                ELSE 'FALSE'
                            END AS HAS_SERIAL";


                if (!string.IsNullOrEmpty(voucherNo) && !string.IsNullOrWhiteSpace(voucherNo))
                {
                    query += $@", MSDE.SERIAL_NO, MSDE.ACTUAL_QTY";
                }


                query += $@" 
                                    FROM MP_RESOURCE_SETUP a
                                    JOIN MP_ROUTINE_RESOURCE_SETUP b
                                        ON a.RESOURCE_CODE = b.RESOURCE_CODE
                                    LEFT JOIN IP_MU_CODE imc
                                        ON imc.MU_CODE = b.MU_CODE";



                if (!string.IsNullOrEmpty(voucherNo) && !string.IsNullOrWhiteSpace(voucherNo))
                {
                    query += $@" LEFT JOIN MP_RESOURCE_DETAIL_ENTRY msde on B.RESOURCE_CODE = MSDE.RESOURCE_CODE 
                                    and msde.VOUCHER_NO =  '{voucherNo}' and MSDE.FORM_CODE = '{formCode}'  and MSDE.COMPANY_CODE = '{companyCode}'
                                    and MSDE.BRANCH_CODE = '{branchCode}'";
                }


                query += $@" WHERE b.COMPANY_CODE = '{companyCode}'
                                        AND a.COMPANY_CODE = '{companyCode}'
                                        AND b.DELETED_FLAG = 'N'
                                        AND a.DELETED_FLAG = 'N'
                                        AND imc.COMPANY_CODE = '{companyCode}'
                                        AND b.PROCESS_CODE = '{processCode}'";

                if (!string.IsNullOrEmpty(categoryType))
                {
                    query += $@" AND b.CATEGORY_TYPE = '{categoryType}'";
                }

                var resultList = _objectEntity
                    .Database
                    .SqlQuery<AssignedRoutineResourceSetupModel>(query)
                    .ToList();

                var groupedList = resultList
                                    .GroupBy(x => new
                                    {
                                        x.RESOURCE_CODE,
                                        x.RESOURCE_EDESC,
                                        x.REQUIRED_INPUT_QTY,
                                        x.STANDARD_INPUT_QTY,
                                        x.STANDARD_OUTPUT_QTY,
                                        x.HAS_SERIAL,
                                    })
                                    .Select(g => new
                                    {
                                        RESOURCE_CODE = g.Key.RESOURCE_CODE,
                                        RESOURCE_EDESC = g.Key.RESOURCE_EDESC,
                                        MU_CODE = g.FirstOrDefault().MU_CODE,
                                        REMARKS = g.FirstOrDefault().REMARKS,
                                        STANDARD_INPUT_QTY = g.Key.STANDARD_INPUT_QTY,
                                        STANDARD_OUTPUT_QTY = g.Key.STANDARD_OUTPUT_QTY,
                                        REQUIRED_INPUT_QTY = g.Key.REQUIRED_INPUT_QTY,
                                        ACTUAL_QTY = g.FirstOrDefault().ACTUAL_QTY,
                                        HAS_SERIAL = g.Key.HAS_SERIAL,
                                        SERIAL_LIST = g.Key.HAS_SERIAL == "TRUE" ? g.ToList() : null
                                    }).ToList();


                return groupedList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all process type code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }



        public List<ResourceInfoModel> GetResourceList(string resourceType = "")
        {
            //string resopurce_type = "M"; // this will replace 

            var companyCode = _workContext.CurrentUserinformation.company_code;
            try
            {

                string query = $@"SELECT RESOURCE_EDESC, REMARKS, RESOURCE_CODE
                                            FROM MP_RESOURCE_SETUP
                                            WHERE COMPANY_CODE = '{companyCode}' AND group_sku_flag = 'I' AND DELETED_FLAG = 'N'";

                if (resourceType == "M")
                {
                    query += $@" And Pre_Resource_Code in (SELECT Resource_Code
                                                                FROM MP_RESOURCE_SETUP
                                                                WHERE UPPER(RESOURCE_EDESC) LIKE '%HUMAN RESOURCE%'
                                                                   OR UPPER(RESOURCE_EDESC) LIKE '%MANPOWER%'
                                                                   OR UPPER(RESOURCE_NDESC) LIKE '%HUMAN%'
                                                                   OR UPPER(RESOURCE_NDESC) LIKE '%MANPOWER%')";
                }

                if (resourceType == "R")
                {
                    query += $@" AND PRE_RESOURCE_CODE NOT IN (
                                    SELECT RESOURCE_CODE
                                    FROM MP_RESOURCE_SETUP
                                    WHERE UPPER(RESOURCE_EDESC) LIKE '%HUMAN RESOURCE%'
                                       OR UPPER(RESOURCE_EDESC) LIKE '%MANPOWER%'
                                       OR UPPER(RESOURCE_NDESC) LIKE '%HUMAN%'
                                       OR UPPER(RESOURCE_NDESC) LIKE '%MANPOWER%'
                              ) 
                                AND PRE_RESOURCE_CODE NOT IN (
                                      SELECT RESOURCE_CODE
                                      FROM MP_RESOURCE_SETUP
                                      WHERE UPPER(RESOURCE_EDESC) LIKE '%COMSUMABLE/POWER BASED%'
                                         OR UPPER(RESOURCE_EDESC) LIKE '%COMSUMABLE%'
                                         OR UPPER(RESOURCE_NDESC) LIKE '%COMSUMABLE%'
                                         OR UPPER(RESOURCE_NDESC) LIKE '%POWER%'
                                  )";
                }

                if (resourceType == "C")
                {


                    query += $@" And Pre_Resource_Code in (SELECT Resource_Code
                                                                FROM MP_RESOURCE_SETUP
                                                                WHERE UPPER(RESOURCE_EDESC) LIKE '%COMSUMABLE/POWER%'
                                                                   OR UPPER(RESOURCE_EDESC) LIKE '%COMSUMABLE%'
                                                                   OR UPPER(RESOURCE_NDESC) LIKE '%COMSUMABLE%'
                                                                   OR UPPER(RESOURCE_NDESC) LIKE '%POWER%')

                                                        And Pre_Resource_Code not in (SELECT Resource_Code
                                                                FROM MP_RESOURCE_SETUP
                                                                WHERE UPPER(RESOURCE_EDESC) LIKE '%HUMAN RESOURCE%'
                                                                   OR UPPER(RESOURCE_EDESC) LIKE '%MANPOWER%'
                                                                   OR UPPER(RESOURCE_NDESC) LIKE '%HUMAN%'
                                                                   OR UPPER(RESOURCE_NDESC) LIKE '%MANPOWER%')";


                }

                var resulstList = _objectEntity.SqlQuery<ResourceInfoModel>(query).ToList();
                return resulstList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all resorce code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<MuCodeModel> GetUnitList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            try
            {
                string query = $@"SELECT MU_EDESC, REMARKS, MU_CODE
                                  FROM IP_MU_CODE
                                 WHERE COMPANY_CODE = '{companyCode}' AND DELETED_FLAG = 'N'";

                var resulstList = _objectEntity.SqlQuery<MuCodeModel>(query).ToList();
                return resulstList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all unit list : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        public string SaveRoutineResourceMappingSetup(MappingRoutineResourceModel model)
        {
            try
            {
                string inputResponse = string.Empty;
                string outputResponse = string.Empty;

                var response = new ProcessResposponseForRoutine();
                var companyCode = _workContext.CurrentUserinformation.company_code;
                var CREATED_BY = _workContext.CurrentUserinformation.login_code;

                var selectExistQuery = $@"SELECT 1
                                            FROM MP_ROUTINE_RESOURCE_SETUP
                                            WHERE PROCESS_CODE = '{model.ProcessCode}'
                                              AND RESOURCE_CODE = '{model.ResourceCode}'
                                              AND DELETED_FLAG = 'N'";

                int recordExists = _objectEntity.Database.SqlQuery<int>(selectExistQuery).FirstOrDefault();

                if (recordExists > 0)
                {
                    var updateQuery = $@" UPDATE MP_ROUTINE_RESOURCE_SETUP
                                           SET QUANTITY = '{model.Quantity}', REMARKS = '{model.Remarks}', MU_CODE ='{model.UnitCode}', STANDARD_OUTPUT_QTY ='{model.StandardOutputQty}', STANDARD_INPUT_QTY='{model.StandardInputQty}'
                                         WHERE PROCESS_CODE = '{model.ProcessCode}'
                                               AND RESOURCE_CODE = '{model.ResourceCode}'
                                               AND COMPANY_CODE = '{companyCode}'";
                    _objectEntity.ExecuteSqlCommand(updateQuery);
                }
                else
                {
                    var insertQuery = $@"INSERT INTO MP_ROUTINE_RESOURCE_SETUP (PROCESS_CODE,
                                       RESOURCE_CODE,
                                       QUANTITY,
                                       MU_CODE,
                                       REMARKS,
                                       COMPANY_CODE,
                                       CREATED_BY,
                                       CREATED_DATE,
                                       DELETED_FLAG,CATEGORY_TYPE, STANDARD_OUTPUT_QTY, STANDARD_INPUT_QTY)
                                             VALUES ('{model.ProcessCode}',
                                                     '{model.ResourceCode}',
                                                     '{model.Quantity}',
                                                     '{model.UnitCode}',
                                                     '{model.Remarks}',
                                                     '{companyCode}',
                                                     '{CREATED_BY}',
                                                     SYSDATE,
                                                     'N',
                                                     '{model.CategoryType}','{model.StandardOutputQty}','{model.StandardInputQty}')";
                    _objectEntity.ExecuteSqlCommand(insertQuery);
                }

                response.MESSAGE = "Successful";
                //response.SAVED_MODAL = null;
                return response.MESSAGE;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving routine and resource mapping setup : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        public object GetUnderLocationDetailData(string process_code)
        {
            try
            {

                // select LOCATION_CODE from MP_PROCESS_SETUP where  PROCESS_CODE = '01.33';
                var tempQ = $@"select LOCATION_CODE from MP_PROCESS_SETUP where  PROCESS_CODE = '{process_code}' and Company_Code='{_workContext.CurrentUserinformation.company_code}'";
                var locationCode = _objectEntity.SqlQuery<string>(tempQ).FirstOrDefault();



                var tempQQ = $@"select PRE_LOCATION_CODE from IP_LOCATION_SETUP where Location_code = '{locationCode}' and Company_Code='{_workContext.CurrentUserinformation.company_code}'";
                var pre_location_code = _objectEntity.SqlQuery<string>(tempQQ).FirstOrDefault();


                return pre_location_code;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting process period : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public string DeleteProcessSetup(string processCode)
        {
            try
            {
                var companyCode = _workContext.CurrentUserinformation.company_code;
                var loginCode = _workContext.CurrentUserinformation.login_code;

                // Update MP_PROCESS_SETUP
                var updateProcessSetupQuery = $@"UPDATE MP_PROCESS_SETUP SET DELETED_FLAG='Y', MODIFY_BY='{loginCode}', MODIFY_DATE=SYSDATE 
                                                WHERE PROCESS_CODE='{processCode}' AND COMPANY_CODE='{companyCode}'";
                _objectEntity.ExecuteSqlCommand(updateProcessSetupQuery);

                // Update MP_ROUTINE_INPUT_SETUP
                var updateInputSetupQuery = $@"UPDATE MP_ROUTINE_INPUT_SETUP SET DELETED_FLAG='Y', MODIFY_BY='{loginCode}', MODIFY_DATE=SYSDATE 
                                               WHERE PROCESS_CODE='{processCode}' AND COMPANY_CODE='{companyCode}'";
                _objectEntity.ExecuteSqlCommand(updateInputSetupQuery);

                // Update MP_ROUTINE_OUTPUT_SETUP
                var updateOutputSetupQuery = $@"UPDATE MP_ROUTINE_OUTPUT_SETUP SET DELETED_FLAG='Y', MODIFY_BY='{loginCode}', MODIFY_DATE=SYSDATE 
                                                WHERE PROCESS_CODE='{processCode}' AND COMPANY_CODE='{companyCode}'";
                _objectEntity.ExecuteSqlCommand(updateOutputSetupQuery);

                // Update MP_ROUTINE_RESOURCE_SETUP
                var updateResourceSetupQuery = $@"UPDATE MP_ROUTINE_RESOURCE_SETUP SET DELETED_FLAG='Y', MODIFY_BY='{loginCode}', MODIFY_DATE=SYSDATE 
                                                 WHERE PROCESS_CODE='{processCode}' AND COMPANY_CODE='{companyCode}'";
                _objectEntity.ExecuteSqlCommand(updateResourceSetupQuery);

                return "Successful";
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while deleting process setup : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


    }
}
