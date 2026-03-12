using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface.ProductionManagement;
using NeoERP.DocumentTemplate.Service.Models.ProductionManagement;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Services.ProductionManagement
{
    public class ProductionManagementService : IProductionManagement
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public ProductionManagementService(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public List<ProductionPlan> GetAllProductionPlanningList(ProductionPlanningFilterParamsModel requestParms)
        {
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;
                string startDate = requestParms.StartDate.ToString("dd-MMM-yyyy");
                string endDate = requestParms.EndDate.ToString("dd-MMM-yyyy");

                string itemQuery = $@"
                                    SELECT *
                                        FROM (SELECT A.PLAN_NO,
                                                     PLAN_DATE,
                                                     BS_DATE (PLAN_DATE) MITI,
                                                     PLAN_NAME,
                                                     A.ITEM_CODE,
                                                     B.ITEM_EDESC,
                                                     B.INDEX_MU_CODE,
                                                     ORDER_NO,
                                                     PLAN_BASE_ON,
                                                     ORDER_QUANTITY,
                                                     PLAN_QUANTITY,
                                                     NVL (
                                                        (SELECT SUM (NVL (PRODUCTION_QTY, 0))
                                                           FROM IP_PRODUCTION_ISSUE
                                                          WHERE SERIAL_NO = 1
                                                                AND PLAN_NO = A.PLAN_NO
                                                                AND COMPANY_CODE = A.COMPANY_CODE),
                                                        0)
                                                        PROD_ISS_QTY,
                                                     A.RESOURCE_CODE,

                                                    (CASE
                                                         WHEN A.RESOURCE_CODE IS NOT NULL
                                                         THEN (SELECT RS.RESOURCE_EDESC
                                                                 FROM MP_RESOURCE_SETUP RS
                                                                WHERE RS.RESOURCE_CODE = A.RESOURCE_CODE
                                                                  AND ROWNUM = 1)
                                                         ELSE ''
                                                     END) AS RESOURCE_EDESC,

                                                     A.DELETED_FLAG
                                                FROM MP_ORDER_PLAN_PROCESS A, IP_ITEM_MASTER_SETUP B
                                               WHERE     A.ITEM_CODE = B.ITEM_CODE(+)
                                                     AND A.COMPANY_CODE = B.COMPANY_CODE(+)
                                                     AND A.COMPANY_CODE = '{company_code}'
                                                     AND A.PLAN_DATE BETWEEN '{startDate}' AND '{endDate}'";



                if (!string.IsNullOrEmpty(requestParms.FilterListSearchText))
                {
                    string searchText = requestParms.FilterListSearchText.ToUpper();

                    itemQuery += $@"  AND (UPPER (A.PLAN_NO) LIKE ('{searchText}%')
                      OR UPPER (B.ITEM_EDESC) LIKE ('%{searchText}%')
                      OR UPPER (A.ORDER_NO) LIKE ('%{searchText}%') OR UPPER (A.PLAN_NAME) LIKE ('%{searchText}%')) 
                     ";
                }

                itemQuery += " ) ";




                itemQuery += " WHERE 1 = 1";
                if (requestParms.Status == "completed")
                {
                    itemQuery += " AND PLAN_QUANTITY = PROD_ISS_QTY  AND DELETED_FLAG = 'N'";
                }

                if (requestParms.Status == "incomplete")
                {
                    itemQuery += " AND PLAN_QUANTITY > PROD_ISS_QTY  AND DELETED_FLAG = 'N'";
                }

                itemQuery += " ORDER BY PLAN_NO, PLAN_DATE DESC";


                var result = this._dbContext.SqlQuery<ProductionPlan>(itemQuery).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production planning list " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<PlanItemModel> GetProductionPlanningListWhileInputProcess(string searchText)
        {
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;
                // string startDate = requestParms.StartDate.ToString("dd-MMM-yyyy");
                // string endDate = requestParms.EndDate.ToString("dd-MMM-yyyy");

                //string itemQuery = $@"
                //                SELECT DISTINCT PLAN_NO, PLAN_NO CODE, PLAN_NAME TT
                //                    FROM MP_ORDER_PLAN_PROCESS A, MP_VARIANCE_INFO B, MP_PROCESS_SETUP C
                //                   WHERE     A.COMPANY_CODE = '{company_code}'
                //                         AND A.PLAN_NO = B.PLAN_CODE
                //                         AND A.COMPANY_CODE = B.COMPANY_CODE
                //                         AND B.PROCESS_CODE = C.PROCESS_CODE
                //                         AND B.COMPANY_CODE = C.COMPANY_CODE
                //                         AND PLAN_NO NOT IN
                //                                (SELECT A.PLAN_NO
                //                                   FROM (  SELECT PLAN_NO,
                //                                                  COMPANY_CODE,
                //                                                  SUM (PRODUCTION_QTY) PRODUCTION_QTY
                //                                             FROM (SELECT DISTINCT ISSUE_NO,
                //                                                                   COMPANY_CODE,
                //                                                                   NVL (PLAN_NO, 0) PLAN_NO,
                //                                                                   PRODUCTION_QTY
                //                                                     FROM IP_PRODUCTION_ISSUE
                //                                                    WHERE COMPANY_CODE = '{company_code}'
                //                                                          AND DELETED_FLAG = 'N')
                //                                         GROUP BY PLAN_NO, COMPANY_CODE) A,
                //                                        MP_ORDER_PLAN_PROCESS B
                //                                  WHERE     a.PLAN_NO = b.PLAN_NO
                //                                        AND A.COMPANY_CODE = B.COMPANY_CODE
                //                                        AND PRODUCTION_QTY >= B.PLAN_QUANTITY)
                //                         AND A.DELETED_FLAG = 'N'
                //                ORDER BY PLAN_NO, PLAN_NAME";

                string itemQuery = "";

                if (!string.IsNullOrEmpty(searchText))
                {
                    itemQuery = GetPlanSearchWithQueryWhileIssueItemForPPlan(searchText.ToUpper());
                }
                else
                {
                    itemQuery = GetPlanSearchWithQueryWhileIssueItemForPPlan("");
                }

                var result = this._dbContext.SqlQuery<PlanItemModel>(itemQuery).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production planning list " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<PlanUsedInInputProcessModel> GetPlanDtlList()
        {
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;

                string itemQuery = $@"
                                    SELECT PLAN_NO, PLAN_NAME, PLAN_NO TYPE_CODE
                                      FROM MP_ORDER_PLAN_PROCESS
                                     WHERE     COMPANY_CODE = '{company_code}'
                                           AND PLAN_NO NOT IN
                                                  (SELECT A.PLAN_NO
                                                     FROM (  SELECT PLAN_NO,
                                                                    COMPANY_CODE,
                                                                    SUM (PRODUCTION_QTY) PRODUCTION_QTY
                                                               FROM (SELECT DISTINCT ISSUE_NO,
                                                                                     COMPANY_CODE,
                                                                                     NVL (PLAN_NO, 0) PLAN_NO,
                                                                                     PRODUCTION_QTY
                                                                       FROM IP_PRODUCTION_ISSUE
                                                                      WHERE     COMPANY_CODE = '{company_code}'
                                                                            AND DELETED_FLAG = 'N')
                                                           GROUP BY PLAN_NO, COMPANY_CODE) A,
                                                          MP_ORDER_PLAN_PROCESS B
                                                    WHERE     a.PLAN_NO = b.PLAN_NO
                                                          AND A.COMPANY_CODE = B.COMPANY_CODE
                                                          AND PRODUCTION_QTY >= B.PLAN_QUANTITY)";

                var result = this._dbContext.SqlQuery<PlanUsedInInputProcessModel>(itemQuery).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production planning list " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        public async Task<List<PMProductModelTreeModel>> GetProductTreeStructureListAsync()
        {
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;

                string group_sku_flag = "G";
                string itemQuery = $@"
                                    SELECT master_item_code AS MasterItemCode, item_edesc AS ItemEDesc, pre_item_code AS PreItemCode 
                                    FROM ip_item_master_setup 
                                    WHERE DELETED_FLAG = 'N' 
                                    AND COMPANY_CODE = '{company_code}' 
                                    AND group_sku_flag = '{group_sku_flag}' 
                                    ORDER BY master_item_code";

                // SELECT pre_item_code, item_code, item_edesc, product_code FROM ip_item_master_setup WHERE(UPPER(item_EDESC) LIKE 'AQUA%' or UPPER(item_code) LIKE 'AQUA%' or upper(PRODUCT_CODE) LIKE 'AQUA%') AND group_sku_flag = 'I' AND COMPANY_CODE = '01' AND DELETED_FLAG = 'N' AND FREEZE_FLAG = 'N'  ORDER BY UPPER(item_EDESC)





                // Wrap sync SqlQuery inside Task.Run to make it async
                var result = await Task.Run(() => _dbContext.SqlQuery<PMProductModel>(itemQuery).ToList());


                var lookup = result.ToDictionary(x => x.MasterItemCode, x => new PMProductModelTreeModel
                {
                    MasterItemCode = x.MasterItemCode,
                    ItemEDesc = x.ItemEDesc,
                    PreItemCode = x.PreItemCode,
                    Items = new List<PMProductModel>()
                });

                List<PMProductModelTreeModel> roots = new List<PMProductModelTreeModel>();

                foreach (var item in result)
                {
                    if (item.PreItemCode == "00")
                    {
                        roots.Add(lookup[item.MasterItemCode]);
                    }
                    else
                    {
                        if (lookup.ContainsKey(item.PreItemCode))
                        {
                            lookup[item.PreItemCode].Items.Add(lookup[item.MasterItemCode]);
                        }
                    }
                }

                return roots;
                //  return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production prodcut tree list " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ProductItemModel>> GetParticularProductListAsync(string item_code)
        {
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;

                string group_sku_flag = "I";
                string freez_flag = "N";
                string itemQuery = $@"
                        SELECT pre_item_code, item_code, item_edesc, product_code 
                        FROM ip_item_master_setup 
                        WHERE pre_item_code LIKE '{item_code}%' 
                          AND UPPER(item_EDESC) LIKE '%' 
                          AND group_sku_flag = '{group_sku_flag}' 
                          AND COMPANY_CODE = '{company_code}' 
                          AND DELETED_FLAG = 'N' 
                          AND FREEZE_FLAG = '{freez_flag}'  
                        ORDER BY UPPER(item_EDESC)";


                // Wrap sync SqlQuery inside Task.Run to make it async
                var result = await Task.Run(() => _dbContext.SqlQuery<ProductItemModel>(itemQuery).ToList());
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production items list " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ProductItemModel>> GetParticularProductListUsingSearchTextAsync(string searchText)
        {
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;

                string group_sku_flag = "I";
                string freez_flag = "N";

                searchText = searchText != null ? searchText.ToUpper() : "";

                string itemQuery = $@"
                          SELECT pre_item_code,
                             item_code,
                             item_edesc,
                             product_code
                        FROM ip_item_master_setup
                        WHERE (UPPER (item_EDESC) LIKE '{searchText}%'
                                  OR UPPER (item_code) LIKE '{searchText}%'
                                  OR UPPER (PRODUCT_CODE) LIKE '{searchText}%')
                             AND group_sku_flag = '{group_sku_flag}'
                             AND COMPANY_CODE = '{company_code}'
                             AND DELETED_FLAG = 'N'
                             AND FREEZE_FLAG = '{freez_flag}'
                    ORDER BY UPPER (item_EDESC)";

                // Wrap sync SqlQuery inside Task.Run to make it async
                var result = await Task.Run(() => _dbContext.SqlQuery<ProductItemModel>(itemQuery).ToList());
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production items list " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        public ProductItemDetailModel GetProductItemDetailsAsync(string item_code)
        {
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;

                string itemQuery = $@"SELECT 
                                            ITEM_CODE AS CODE, 
                                            ITEM_EDESC AS NAME, 
                                            CATEGORY_EDESC AS CATEGORY, 
                                            INDEX_MU_CODE AS UNIT, 
                                            (
                                                SELECT MU_CODE 
                                                FROM IP_ITEM_UNIT_SETUP 
                                                WHERE ITEM_CODE = A.ITEM_CODE 
                                                  AND COMPANY_CODE = A.COMPANY_CODE 
                                                  AND SERIAL_NO = 1
                                            ) AS ALT_UNIT, 
                                            A.HS_CODE  
                                        FROM 
                                            IP_ITEM_MASTER_SETUP A, 
                                            IP_CATEGORY_CODE B  
                                        WHERE 
                                            A.COMPANY_CODE = '{company_code}'  
                                            AND A.ITEM_CODE = '{item_code}'  
                                            AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                                            AND A.CATEGORY_CODE = B.CATEGORY_CODE (+)";


                string itemQueryStockAndLocation = $@"SELECT DISTINCT
                                                    INITCAP(b.LOCATION_EDESC) AS LocationEdesc,
                                                    NVL(SUM(NVL(a.IN_QUANTITY, 0)) - SUM(NVL(a.OUT_QUANTITY, 0)), 0) AS Quantity
                                                FROM V$VIRTUAL_STOCK_WIP_LEDGER1 a,
                                                     IP_LOCATION_SETUP b
                                                WHERE a.LOCATION_CODE = b.LOCATION_CODE
                                                  AND a.COMPANY_CODE = b.COMPANY_CODE
                                                  AND a.COMPANY_CODE = '{company_code}'
                                                  AND a.BRANCH_CODE = '{branch_code}'
                                                  AND a.ITEM_CODE = '{item_code}'
                                                GROUP BY INITCAP(b.LOCATION_EDESC)
                                                ORDER BY INITCAP(b.LOCATION_EDESC)";
                var stockInfoResult = _dbContext.SqlQuery<StockLocationInfo>(itemQueryStockAndLocation).FirstOrDefault();


                // Wrap sync SqlQuery inside Task.Run to make it async
                var result = _dbContext.SqlQuery<ProductItemDetailModel>(itemQuery).FirstOrDefault();
                result.stockLocationInfo = new StockLocationInfo();
                result.stockLocationInfo = stockInfoResult;
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production item details  " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<RawMaterialItemsModel> GetRowPackingMaterialAfterVerianceInfoInsert(decimal? plan_no = null)
        {
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;


                var planCode = plan_no;
                string itemQuery = $@"
                                        SELECT 
                                            B.ITEM_CODE, 
                                            B.ITEM_EDESC,
                                            B.INDEX_MU_CODE,  
                                            C.PROCESS_EDESC, 
                                            E.PROCESS_TYPE_EDESC, 
                                            E.STEP_NO, 
                                            F.SERIAL_NO, 
                                            A.REQUIRED_QUANTITY, 
                                            A.CATEGORY_CODE, 
                                            SUM(IN_QUANTITY - OUT_QUANTITY) AS STOCK,
                                            (
                                                SELECT SUM(REQUIRED_QUANTITY) 
                                                FROM MP_VARIANCE_INFO 
                                                WHERE RAW_ITEM_CODE = A.RAW_ITEM_CODE 
                                                --  AND FINISHED_ITEM_CODE = C.INDEX_ITEM_CODE  
                                                  AND COMPANY_CODE = A.COMPANY_CODE  
                                                  AND PLAN_CODE <> '{planCode}'  
                                                  AND PLAN_CODE NOT IN (
                                                      SELECT NVL(PLAN_NO, 0) 
                                                      FROM IP_PRODUCTION_MRR 
                                                      WHERE COMPANY_CODE = '{company_code}' 
                                                        AND DELETED_FLAG = 'N'
                                                  )
                                            ) AS PLAN_QUANTITY   
                                        FROM 
                                            MP_VARIANCE_INFO A
                                            JOIN IP_ITEM_MASTER_SETUP B 
                                                ON A.RAW_ITEM_CODE = B.ITEM_CODE  
                                                AND A.COMPANY_CODE = B.COMPANY_CODE  
                                            JOIN MP_PROCESS_SETUP C 
                                                ON A.PROCESS_CODE = C.PROCESS_CODE  
                                                AND A.COMPANY_CODE = C.COMPANY_CODE  
                                            JOIN MP_ROUTINE_INPUT_SETUP F 
                                                ON C.PROCESS_CODE = F.PROCESS_CODE  
                                                AND C.COMPANY_CODE = F.COMPANY_CODE  
                                                AND B.ITEM_CODE = F.ITEM_CODE  
                                                AND B.COMPANY_CODE = F.COMPANY_CODE  
                                            JOIN MP_PROCESS_TYPE_CODE E 
                                                ON C.PROCESS_TYPE_CODE = E.PROCESS_TYPE_CODE  
                                                AND C.COMPANY_CODE = E.COMPANY_CODE  
                                            LEFT JOIN V$VIRTUAL_STOCK_WIP_LEDGER1 D 
                                                ON B.ITEM_CODE = D.ITEM_CODE  
                                                AND B.COMPANY_CODE = D.COMPANY_CODE  
                                        WHERE 
                                            A.PLAN_CODE = '{planCode}'  
                                        GROUP BY 
                                            A.RAW_ITEM_CODE, 
                                            C.INDEX_ITEM_CODE, 
                                            A.COMPANY_CODE, 
                                            B.ITEM_CODE, 
                                            B.ITEM_EDESC,
                                            B.INDEX_MU_CODE, 
                                            C.PROCESS_EDESC, 
                                            E.PROCESS_TYPE_EDESC, 
                                            E.STEP_NO, 
                                            A.REQUIRED_QUANTITY, 
                                            A.CATEGORY_CODE, 
                                            F.SERIAL_NO  
                                        ORDER BY 
                                            E.STEP_NO, 
                                            C.PROCESS_EDESC, 
                                            F.SERIAL_NO, 
                                            B.ITEM_EDESC
                                    ";

                // Wrap sync SqlQuery inside Task.Run to make it async
                var result =
                    _dbContext.SqlQuery<RawMaterialItemsModel>(itemQuery).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production item details  " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public decimal GetNewPlanCode()
        {
            // required data to call the Query
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;

            string itemQuery = $@"SELECT NVL(MAX(NVL(PLAN_NO,1000)),1000) + 1 PLAN_NO FROM MP_ORDER_PLAN_PROCESS  
                                  WHERE COMPANY_CODE = '{company_code}' ";
            var result = _dbContext.SqlQuery<decimal>(itemQuery).FirstOrDefault();
            return result;
        }
        //public async Task<object> PrepareRowMaterialBasedOnCalcAndInsertAsync(string item_code, decimal requestedQty, string plan_no = "", List<VarianceInfoModel> varianceInfoModelList = null)
        //{
        //    List<VarianceInfoModel> varianceInfoModelList1 = new List<VarianceInfoModel>();
        //    try
        //    {
        //        // required data to call the Query
        //        var userid = _workContext.CurrentUserinformation.User_id;
        //        var company_code = _workContext.CurrentUserinformation.company_code;
        //        var branch_code = _workContext.CurrentUserinformation.branch_code;

        //        var main_item_code = item_code;

        //        var planCode = plan_no == "" ? await GetNewPlanCode() : Convert.ToDecimal(plan_no);
        //        var result = await PrepareVarianceInfoForInsertItemRecursiveM(company_code, main_item_code, planCode, requestedQty, item_code); // list prepare for veriance Info
        //                                                                                                                                        // InsertVarianceInfoTable(result); // insert in veriance info table
        //        InsertVarianceInfoTable(result);

        //        var materialList = await GetRowPackingMaterialAfterVerianceInfoInsertAsync(planCode);
        //        return materialList;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logErp.ErrorInDB("Get error while getting production item details  " + ex.StackTrace);
        //        throw new Exception(ex.Message);
        //    }
        //}


        public object PrepareRowMaterialBasedOnCalcAndInsertAsync(
            List<ItemAndQty> itemWithQtyList,
            decimal requestedQty,
            string plan_no = "",
            List<VarianceInfoModel> varianceInfoModelList = null)
        {
            List<VarianceInfoModel> varianceInfoModelList1 = new List<VarianceInfoModel>();
            try
            {
                // required data to call the Query
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;

                List<VarianceInfoModel> varianceInfoModelListObj = new List<VarianceInfoModel>();
                var planCode = plan_no == "" ? GetNewPlanCode() : Convert.ToDecimal(plan_no);

                foreach (var item in itemWithQtyList)
                {
                    var main_item_code = item.ItemCode;
                    requestedQty = item.Qty;

                    var result = PrepareVarianceInfoForInsertItemRecursiveM(company_code, main_item_code, planCode, requestedQty, item.ItemCode); // list prepare for veriance Info
                    varianceInfoModelListObj.AddRange(result);
                }

                InsertVarianceInfoTable(varianceInfoModelListObj);

                var materialList = GetRowPackingMaterialAfterVerianceInfoInsert(planCode);
                return materialList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production item details  " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        public int InsertOrderPlanProcess(OrderPlanProcess planObj)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.login_code;

            string planNameCheckQuery = $@"SELECT COUNT(*) AS TOTAL
                                                FROM MP_ORDER_PLAN_PROCESS
                                                WHERE UPPER(TRIM(PLAN_NAME)) = UPPER(TRIM('{planObj.PlanName}'))";

            var r = _dbContext.SqlQuery<int?>(planNameCheckQuery).FirstOrDefault();
            if (r != null && r > 0)
            {
                throw new ValidationException("Plan name already exists in the system.");
            }


            string planDate = planObj.PlanDate.ToString("dd-MMM-yyyy");
            int i = 0;
            var insertPlanQuery = $@"
                    INSERT INTO MP_ORDER_PLAN_PROCESS (
                        PLAN_NO,
                        PLAN_NAME,
                        PLAN_DATE,
                        ITEM_CODE,
                        ORDER_QUANTITY,
                        PLAN_QUANTITY,
                        COMPANY_CODE,
                        CREATED_BY,
                        CREATED_DATE,
                        PLAN_BASE_ON,
                        ORDER_NO,
                        BASE_FLAG,
                        RESOURCE_CODE
                    ) VALUES (
                        '{planObj.PlanNo}',
                        '{planObj.PlanName}',
                        TO_DATE('{planDate}', 'DD-MON-YYYY'),
                        '{planObj.ItemCode}',
                        '{planObj.OrderQuantity}',
                        '{planObj.PlanQuantity}',
                        '{company_code}',
                        '{user_type}',
                        SYSDATE,
                        '{planObj.PlanBaseOn}',
                        '{planObj.OrderNo}',
                        '{planObj.BaseFlag}',
                        '{planObj.ResourceCode}'
                    )";

            string deleteQueryPlanWiseOrder = $@"DELETE FROM MP_PLAN_WISE_ORDER WHERE  COMPANY_CODE = '{company_code}' AND PLAN_NO = '{planObj.PlanNo}'";

            string deleteBatchTransaction = $@"DELETE FROM batch_transaction WHERE form_code = '1' AND REFERENCE_NO = '{planObj.PlanNo}' AND COMPANY_CODE = '{company_code}'";

            StringBuilder insertOrderQuery = null;
            if (planObj.OrderDetailList.Count > 0)
            {
                insertOrderQuery = new StringBuilder();
                insertOrderQuery.AppendLine("BEGIN");
                foreach (var order in planObj.OrderDetailList)
                {
                    var planQty = order.PLAN_QUANTITY > 0 ? order.PLAN_QUANTITY : order.QUANTITY;
                    insertOrderQuery.AppendLine($@"
                            INSERT INTO MP_PLAN_WISE_ORDER (
                                PLAN_NO,
                                ORDER_NO,
                                FORM_CODE,
                                ITEM_CODE,
                                QUANTITY,
                                PLAN_QUANTITY,
                                COMPANY_CODE,
                                CREATED_BY,
                                CREATED_DATE,
                                DELETED_FLAG
                            ) VALUES (
                                '{planObj.PlanNo}',
                                '{order.ORDER_NO}',
                                '{order.FORM_CODE}',
                                '{order.ITEM_CODE}',
                                '{order.QUANTITY}',
                                '{planQty}',
                                '{company_code}',
                                '{user_type}',
                                SYSDATE,
                                'N'
                            );");
                }

                insertOrderQuery.AppendLine("EXCEPTION");
                insertOrderQuery.AppendLine("    WHEN OTHERS THEN");
                insertOrderQuery.AppendLine("        RAISE_APPLICATION_ERROR(-20002, 'Order Insert Failed: ' || SQLERRM);");
                insertOrderQuery.AppendLine("END;");
            }

            using (var prdTransaction = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    i = _dbContext.ExecuteSqlCommand(insertPlanQuery);



                    if (planObj.BatchQtyList.Count > 0)
                    {
                        i = _dbContext.ExecuteSqlCommand(deleteBatchTransaction);

                        var insertAllQuery = new StringBuilder();
                        insertAllQuery.AppendLine("DECLARE");
                        insertAllQuery.AppendLine("    v_transaction_no NUMBER;");
                        insertAllQuery.AppendLine("    v_mu_code VARCHAR2(20);");
                        insertAllQuery.AppendLine("BEGIN");

                        // Start from max + 1
                        insertAllQuery.AppendLine($@"
                                                SELECT NVL(MAX(TO_NUMBER(transaction_no)), 0) + 1
                                                INTO v_transaction_no
                                                FROM batch_transaction
                                                WHERE COMPANY_CODE = '{company_code}' AND BRANCH_CODE = '{branch_code}';");

                        foreach (var batch in planObj.BatchQtyList)
                        {
                            insertAllQuery.AppendLine($@"
                                                    -- Get mu_code for item '{batch.ITEM_CODE}'
                                                    SELECT INDEX_MU_CODE
                                                    INTO v_mu_code
                                                    FROM IP_ITEM_MASTER_SETUP
                                                    WHERE COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{batch.ITEM_CODE}';

                                                    INSERT INTO batch_transaction (
                                                        transaction_no,
                                                        reference_no,
                                                        REF_VOUCHER_NO,
                                                        SESSION_ROWID,
                                                        serial_no,
                                                        item_code,
                                                        mu_code,
                                                        batch_no,
                                                        quantity,
                                                        form_code,
                                                        company_code,
                                                        branch_code,
                                                        created_by,
                                                        created_date,
                                                        DELETED_FLAG,
                                                        source_flag
                                                    ) VALUES (
                                                        v_transaction_no,
                                                        '{planObj.PlanNo}',
                                                        '{batch.REFERENCE_NO}',
                                                        '{planObj.PlanNo}',
                                                        '1',
                                                        '{batch.ITEM_CODE}',
                                                        v_mu_code,
                                                        '{batch.BATCH_NO}',
                                                        '{batch.QUANTITY}',
                                                        '1',
                                                        '{company_code}',
                                                        '{branch_code}',
                                                        '{user_type}',
                                                        SYSDATE,
                                                        'N',
                                                        'P'
                                                    );

                                                    -- Increment for next row
                                                    v_transaction_no := v_transaction_no + 1;");
                        }

                        insertAllQuery.AppendLine("EXCEPTION");
                        insertAllQuery.AppendLine("    WHEN OTHERS THEN");
                        insertAllQuery.AppendLine("        RAISE_APPLICATION_ERROR(-20001, 'Batch Insert Failed: ' || SQLERRM);");
                        insertAllQuery.AppendLine("END;");

                        string fullQuery = insertAllQuery.ToString();
                        int fullQueryResult = _dbContext.ExecuteSqlCommand(fullQuery);
                        if (fullQueryResult == 0)
                        {
                            throw new Exception("No rows inserted into batch_transaction.");
                        }
                    }

                    // Order insert query
                    if (planObj.OrderDetailList.Count > 0)
                    {
                        i = _dbContext.ExecuteSqlCommand(deleteQueryPlanWiseOrder);

                        string fullInsertOrderQuery = insertOrderQuery.ToString();
                        int insertOrderResult = _dbContext.ExecuteSqlCommand(fullInsertOrderQuery);
                        if (insertOrderResult == 0)
                        {
                            throw new Exception("No rows inserted into MP_PLAN_WISE_ORDER.");
                        }
                    }

                    prdTransaction.Commit();
                }
                catch (Exception ex)
                {
                    prdTransaction.Rollback();
                    _logErp.ErrorInDB("Error during plan insert: " + ex.StackTrace);
                    throw new Exception(ex.Message);
                }
            }

            return i;
        }

        public int UpdateOrderPlanProcess(OrderPlanProcess planObj)
        {
            int i = 0;
            try
            {
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;
                var user_type = _workContext.CurrentUserinformation.login_code;

                //var prevDetails = await GetPlanDetailsForEdit(planObj.PlanNo);
                //string planDate = prevDetails.PLAN_DATE.ToString("dd-MMM-yyyy");
                //TO_DATE('{planDate}', 'DD-MON-YYYY'),


                var updatePlanQuery = $@"
                                    UPDATE MP_ORDER_PLAN_PROCESS SET
                                        PLAN_NAME = '{planObj.PlanName}',
                                        ITEM_CODE = '{planObj.ItemCode}',
                                        ORDER_QUANTITY = '{planObj.OrderQuantity}',
                                        PLAN_QUANTITY = '{planObj.PlanQuantity}',
                                        PLAN_BASE_ON = '{planObj.PlanBaseOn}',
                                        BASE_FLAG = '{planObj.BaseFlag}',
                                        RESOURCE_CODE = '{planObj.ResourceCode}',
                                        PLAN_DATE = TO_DATE('{planObj.PlanDate}', 'MM/DD/YYYY HH:MI:SS AM')
                                    WHERE
                                        PLAN_NO = '{planObj.PlanNo}'
                                        AND COMPANY_CODE = '{company_code}'";

                i = _dbContext.ExecuteSqlCommand(updatePlanQuery);
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error during plan insert: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
            return i;
        }



        public async Task<object> GetResourceDataList()
        {
            // required data to call the Query
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;

            string g_flag = "I";
            string itemQuery = $@"
                    SELECT 
                        A.RESOURCE_CODE, 
                        A.RESOURCE_EDESC 
                    FROM 
                        MP_RESOURCE_SETUP A 
                    WHERE 
                        A.DELETED_FLAG = 'N' 
                        AND A.COMPANY_CODE = '{company_code}' 
                        AND GROUP_SKU_FLAG = '{g_flag}' 
                    ORDER BY 
                        A.RESOURCE_EDESC";

            // Wrap synchronous SqlQuery in Task.Run to execute asynchronously
            var result = await Task.Run(() =>
                _dbContext.SqlQuery<ResourceDataModel>(itemQuery).ToList()
            );

            return result;
        }
        public async Task<object> GetOrderListOfSelectedItemCodeOrOrderNo(string item_code, string order_no = "")
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;

            string itemQuery = $@"
                                SELECT * FROM (
                                    SELECT 
                                        A.ORDER_NO, 
                                        A.ORDER_DATE, 
                                        A.DELIVERY_DATE, 
                                        A.ITEM_CODE, 
                                        E.ITEM_EDESC, 
                                        A.FORM_CODE, 
                                        A.CUSTOMER_CODE, 
                                        B.CUSTOMER_EDESC,
                                        A.QUANTITY, 
                                        (
                                            SELECT COUNT(*) 
                                            FROM REFERENCE_DETAIL 
                                            WHERE REFERENCE_NO = A.ORDER_NO   
                                              AND REFERENCE_FORM_CODE = A.FORM_CODE 
                                              AND COMPANY_CODE = A.COMPANY_CODE
                                        ) TAKEN,
                                        SUM(NVL(D.PLAN_QUANTITY, 0)) PLAN_QUANTITY 
                                    FROM 
                                        SA_SALES_ORDER A
                                        INNER JOIN SA_CUSTOMER_SETUP B 
                                            ON A.CUSTOMER_CODE = B.CUSTOMER_CODE  
                                           AND A.COMPANY_CODE = B.COMPANY_CODE
                                        INNER JOIN MASTER_TRANSACTION C 
                                            ON A.ORDER_NO = C.VOUCHER_NO  
                                           AND A.FORM_CODE = C.FORM_CODE  
                                           AND A.COMPANY_CODE = C.COMPANY_CODE
                                        LEFT JOIN MP_PLAN_WISE_ORDER D 
                                            ON A.COMPANY_CODE = D.COMPANY_CODE  
                                           AND A.ORDER_NO = D.ORDER_NO  
                                           AND A.FORM_CODE = D.FORM_CODE  
                                           AND A.ITEM_CODE = D.ITEM_CODE
                                        INNER JOIN IP_ITEM_MASTER_SETUP E 
                                            ON A.ITEM_CODE = E.ITEM_CODE  
                                           AND A.COMPANY_CODE = E.COMPANY_CODE
                                    WHERE 
                                        C.AUTHORISED_BY IS NOT NULL
                                        AND A.COMPANY_CODE = '{company_code}'
                                        AND A.ITEM_CODE = '{item_code}'
                                    GROUP BY 
                                        A.ORDER_NO, 
                                        A.ORDER_DATE, 
                                        A.DELIVERY_DATE, 
                                        A.ITEM_CODE, 
                                        E.ITEM_EDESC, 
                                        A.FORM_CODE, 
                                        A.CUSTOMER_CODE, 
                                        B.CUSTOMER_EDESC, 
                                        A.QUANTITY, 
                                        A.COMPANY_CODE
                                ) 
                                WHERE 
                                    TAKEN = 0  
                                    AND NVL(PLAN_QUANTITY, 0) <> QUANTITY  
                                ORDER BY ORDER_NO
                            ";


            if (!string.IsNullOrEmpty(order_no))
            {
                itemQuery = GetQueryStringForOrderListByOrderNo(order_no, company_code);
            }

            // Execute the query
            var result = await Task.Run(() => _dbContext.SqlQuery<OrderDetail>(itemQuery).ToList());

            // Return dummy or actual result
            return result; // Replace with actual return value if needed
        }
        public async Task<object> GetProductionPipeDataList(string item_code, string plan_no, string order_no = "")
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;

            // SQL query for production pipeline data
            string itemQuery = $@"
                                    SELECT 
                                    A.LOCATION_CODE,
                                    B.LOCATION_EDESC,
                                    A.ITEM_EDESC,
                                    A.MU_CODE,
                                    SUM(IN_QUANTITY - OUT_QUANTITY) AS BALANCE_QUANTITY,
                                    (
                                        SELECT SUM(PLAN_QUANTITY) 
                                        FROM MP_ORDER_PLAN_PROCESS 
                                        WHERE ITEM_CODE = A.ITEM_CODE
                                          AND COMPANY_CODE = A.COMPANY_CODE
                                          AND PLAN_NO <> '{plan_no}'
                                          AND PLAN_NO NOT IN (
                                              SELECT NVL(PLAN_NO, 0) 
                                              FROM IP_PRODUCTION_MRR 
                                              WHERE COMPANY_CODE = '{company_code}' 
                                                AND DELETED_FLAG = 'N'
                                          )
                                    ) AS PLAN_QUANTITY
                                FROM 
                                    V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                JOIN 
                                    IP_LOCATION_SETUP B
                                    ON A.LOCATION_CODE = B.LOCATION_CODE
                                    AND A.COMPANY_CODE = B.COMPANY_CODE
                                WHERE 
                                    A.COMPANY_CODE = '{company_code}'
                                    AND A.ITEM_CODE = '{item_code}'
                                GROUP BY 
                                    A.LOCATION_CODE,
                                    B.LOCATION_EDESC,
                                    A.ITEM_EDESC,
                                    A.MU_CODE,
                                    A.ITEM_CODE,
                                    A.COMPANY_CODE
                                ORDER BY 
                                    B.LOCATION_EDESC
                                        ";

            if (!string.IsNullOrEmpty(order_no))
            {
                itemQuery = GetQueryStringForPipeListItemsByOrderNo(order_no, plan_no, company_code);
            }

            // Execute the query
            var result = await Task.Run(() => _dbContext.SqlQuery<ProductionPipeDataModel>(itemQuery).ToList());

            // Return dummy or actual result
            return result; // Replace with actual return value if needed

        }
        public async Task<ProductionPlan> GetPlanDetailsForEdit(string plan_no)
        {
            // required data to call the Query
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;



            string itemQuery = $@"SELECT
                                        A.PLAN_NO,
                                        A.PLAN_DATE,
                                        BS_DATE(A.PLAN_DATE) AS MITI,
                                        A.PLAN_NAME,
                                        A.ITEM_CODE,
                                        B.ITEM_EDESC,
                                        A.ORDER_NO,
                                        A.BASE_FLAG,
                                        A.PLAN_BASE_ON,
                                        A.ORDER_QUANTITY,
                                        A.PLAN_QUANTITY,
                                        (
                                            SELECT COUNT(*)
                                            FROM IP_PRODUCTION_ISSUE PI
                                            WHERE PI.PLAN_NO = A.PLAN_NO 
                                              AND PI.COMPANY_CODE = A.COMPANY_CODE
                                        ) AS REC_FOUND,
                                        A.RESOURCE_CODE,
                                        R.RESOURCE_EDESC
                                    FROM MP_ORDER_PLAN_PROCESS A
                                    LEFT JOIN IP_ITEM_MASTER_SETUP B 
                                        ON A.ITEM_CODE = B.ITEM_CODE 
                                       AND A.COMPANY_CODE = B.COMPANY_CODE
                                    LEFT JOIN MP_RESOURCE_SETUP R 
                                        ON R.RESOURCE_CODE = A.RESOURCE_CODE   
                                    WHERE A.COMPANY_CODE = '{company_code}'
                                      AND A.PLAN_NO = '{plan_no}'
                                    ORDER BY A.PLAN_NO, A.PLAN_DATE DESC";
            var result = await Task.Run(() => _dbContext.SqlQuery<ProductionPlan>(itemQuery).FirstOrDefault());
            return result;
        }

        public List<ProductionOrderDataModel> GetProductionOrderList(string searchText)
        {

            var company_code = _workContext.CurrentUserinformation.company_code;

            string itemQuery = $@"
                            SELECT DISTINCT A.ORDER_NO, A.ORDER_DATE, B.CUSTOMER_EDESC
                              FROM SA_SALES_ORDER A, SA_CUSTOMER_SETUP B, MASTER_TRANSACTION C
                             WHERE A.DELETED_FLAG = 'N'
                               AND A.COMPANY_CODE = '{company_code}'
                               AND A.CUSTOMER_CODE = B.CUSTOMER_CODE
                               AND A.COMPANY_CODE = B.COMPANY_CODE
                               AND A.ORDER_NO = C.VOUCHER_NO
                               AND A.COMPANY_CODE = C.COMPANY_CODE
                               AND C.AUTHORISED_BY IS NOT NULL";

            // 🔍 Add search condition only if searchText is provided
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToUpper();

                itemQuery += $@"
           AND ( UPPER(A.ORDER_NO) LIKE '{searchText}%'
              OR UPPER(B.CUSTOMER_EDESC) LIKE '{searchText}%'
              OR UPPER(B.CUSTOMER_CODE) LIKE '{searchText}%')";
            }

            itemQuery += @"
                       AND ORDER_NO NOT IN (SELECT REFERENCE_NO
                                              FROM REFERENCE_DETAIL
                                             WHERE COMPANY_CODE = A.COMPANY_CODE)
                       AND ORDER_NO NOT IN (SELECT NVL (ORDER_NO, '00')
                                              FROM MP_ORDER_PLAN_PROCESS
                                             WHERE COMPANY_CODE = A.COMPANY_CODE)
                  ORDER BY ORDER_DATE DESC, ORDER_NO DESC";

            var result = _dbContext.SqlQuery<ProductionOrderDataModel>(itemQuery).ToList();
            return result;
        }


        public async Task<List<BatchTransactionResult>> GetBatchTransectionInfo(string item_code, string comma_seprated_order_no, string plan_no)
        {
            // required data to call the Query
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;

            var inClauseStr = "('" + string.Join("', '", comma_seprated_order_no.Split(',').Select(s => s.Trim())) + "')";

            string itemQuery = $@"SELECT REFERENCE_NO,
                                       BATCH_NO,
                                       ITEM_CODE,
                                         QUANTITY
                                       - NVL (
                                            (SELECT SUM (NVL (QUANTITY, 0))
                                               FROM BATCH_TRANSACTION
                                              WHERE     BATCH_NO = A.BATCH_NO
                                                    AND COMPANY_CODE = A.COMPANY_CODE
                                                    AND ITEM_CODE = A.ITEM_CODE
                                                    AND REFERENCE_NO <> '{plan_no}'
                                                    AND REF_VOUCHER_NO IN {inClauseStr}
                                                    AND FORM_CODE = '1'),
                                            0)
                                          QUANTITY
                                  FROM BATCH_TRANSACTION A
                                 WHERE     COMPANY_CODE = '{company_code}'
                                       AND REFERENCE_NO IN {inClauseStr}
                                       AND ITEM_CODE = '{item_code}'";

            var result = await Task.Run(() => _dbContext.SqlQuery<BatchTransactionResult>(itemQuery).ToList());
            return result;
        }

        public async Task<List<OrderDetail>> GetOrderListForEditData(string planNo)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;

            string itemQuery = $@"
                                  SELECT A.ORDER_NO,
                                     B.FORM_CODE,
                                     B.ORDER_DATE,
                                     B.DELIVERY_DATE,
                                     C.CUSTOMER_EDESC,
                                     A.QUANTITY,
                                     PLAN_QUANTITY,
                                     D.ITEM_CODE,
                                     D.ITEM_EDESC
                                FROM MP_PLAN_WISE_ORDER A,
                                     SA_SALES_ORDER B,
                                     SA_CUSTOMER_SETUP C,
                                     IP_ITEM_MASTER_SETUP D
                               WHERE     A.ITEM_CODE = B.ITEM_CODE
                                     AND A.COMPANY_CODE = B.COMPANY_CODE
                                     AND A.ORDER_NO = B.ORDER_NO
                                     AND A.FORM_CODE = B.FORM_CODE
                                     AND A.QUANTITY = B.QUANTITY
                                     AND B.COMPANY_CODE = C.COMPANY_CODE
                                     AND B.CUSTOMER_CODE = C.CUSTOMER_CODE
                                     AND A.COMPANY_CODE = '{company_code}'
                                     AND B.ITEM_CODE = D.ITEM_CODE
                                     AND B.COMPANY_CODE = D.COMPANY_CODE
                                     AND A.PLAN_NO = '{planNo}'
                            ORDER BY A.PLAN_NO, ORDER_NO DESC
                            ";

            // Execute the query
            var result = await Task.Run(() => _dbContext.SqlQuery<OrderDetail>(itemQuery).ToList());

            // Return dummy or actual result
            return result; // Replace with actual return value if needed
        }


        public object DeleteVarianceInfo(string planNo)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;


            string deleteQuery = $@"
                            DELETE FROM mp_variance_info v
                            WHERE v.plan_code = {planNo}
                              AND NOT EXISTS (
                                  SELECT 1 FROM MP_ORDER_PLAN_PROCESS p
                                  WHERE p.plan_no = {planNo}
                              )";

            // Execute the query
            var result = _dbContext.ExecuteSqlCommand(
                     deleteQuery
                 );

            return result;
        }


        //    public object GetPlanSearchWithQueryWhileIssueItemForPPlan(string searchText, int page, int pageSize)
        //    {
        //        var userid = _workContext.CurrentUserinformation.User_id;
        //        var company_code = _workContext.CurrentUserinformation.company_code;
        //        var branch_code = _workContext.CurrentUserinformation.branch_code;
        //        var user_type = _workContext.CurrentUserinformation.UserType;
        //        if (searchText == null)
        //        {
        //            searchText = "";
        //        }
        //        var offset = (page - 1) * pageSize;
        //        searchText = searchText.ToUpper();
        //        // Total count query
        //        var countSql = $@"
        //    SELECT COUNT(DISTINCT PLAN_NO) AS TOTAL_COUNT
        //    FROM MP_ORDER_PLAN_PROCESS A, MP_VARIANCE_INFO B, MP_PROCESS_SETUP C
        //    WHERE A.COMPANY_CODE = '{company_code}'
        //        AND A.PLAN_NO = B.PLAN_CODE
        //        AND A.COMPANY_CODE = B.COMPANY_CODE
        //        AND B.PROCESS_CODE = C.PROCESS_CODE
        //        AND B.COMPANY_CODE = C.COMPANY_CODE
        //        AND (UPPER(PLAN_NAME) LIKE '%{searchText}%' OR TO_CHAR(PLAN_NO) LIKE '%{searchText}%')
        //        AND A.DELETED_FLAG = 'N'
        //        AND (PLAN_NO, LOCATION_CODE) NOT IN
        //            (SELECT A.PLAN_NO, TO_LOCATION_CODE
        //               FROM (  SELECT PLAN_NO,
        //                              COMPANY_CODE,
        //                              TO_LOCATION_CODE,
        //                              SUM (PRODUCTION_QTY) PRODUCTION_QTY
        //                         FROM (SELECT DISTINCT ISSUE_NO,
        //                                               COMPANY_CODE,
        //                                               NVL (PLAN_NO, 0) PLAN_NO,
        //                                               PRODUCTION_QTY,
        //                                               TO_LOCATION_CODE
        //                               FROM IP_PRODUCTION_ISSUE
        //                              WHERE COMPANY_CODE = '{company_code}'
        //                                    AND DELETED_FLAG = 'N')
        //                     GROUP BY PLAN_NO, COMPANY_CODE, TO_LOCATION_CODE) A,
        //                    MP_ORDER_PLAN_PROCESS B
        //              WHERE a.PLAN_NO = b.PLAN_NO
        //                    AND A.COMPANY_CODE = B.COMPANY_CODE
        //                    AND PRODUCTION_QTY >= B.PLAN_QUANTITY)";

        //        var totalCount = this._dbContext.SqlQuery<int>(countSql).FirstOrDefault();

        //        // Paged query using OFFSET FETCH (Oracle 12c+)
        //        var pagedSql = $@"
        //    SELECT *
        //    FROM (
        //        SELECT DISTINCT PLAN_NO, PLAN_NO CODE, PLAN_NAME TT,
        //               ROW_NUMBER() OVER (ORDER BY PLAN_NAME) AS RN
        //          FROM MP_ORDER_PLAN_PROCESS A, MP_VARIANCE_INFO B, MP_PROCESS_SETUP C
        //         WHERE A.COMPANY_CODE = '{company_code}'
        //               AND A.PLAN_NO = B.PLAN_CODE
        //               AND A.COMPANY_CODE = B.COMPANY_CODE
        //               AND B.PROCESS_CODE = C.PROCESS_CODE
        //               AND B.COMPANY_CODE = C.COMPANY_CODE
        //               AND (UPPER (PLAN_NAME) LIKE '%{searchText}%' OR TO_CHAR (PLAN_NO) LIKE '%{searchText}%')
        //               AND A.DELETED_FLAG = 'N'
        //               AND (PLAN_NO, LOCATION_CODE) NOT IN
        //                    (SELECT A.PLAN_NO, TO_LOCATION_CODE
        //                       FROM (  SELECT PLAN_NO,
        //                                      COMPANY_CODE,
        //                                      TO_LOCATION_CODE,
        //                                      SUM (PRODUCTION_QTY) PRODUCTION_QTY
        //                                 FROM (SELECT DISTINCT ISSUE_NO,
        //                                                       COMPANY_CODE,
        //                                                       NVL (PLAN_NO, 0) PLAN_NO,
        //                                                       PRODUCTION_QTY,
        //                                                       TO_LOCATION_CODE
        //                                     FROM IP_PRODUCTION_ISSUE
        //                                    WHERE COMPANY_CODE = '{company_code}'
        //                                          AND DELETED_FLAG = 'N')
        //                             GROUP BY PLAN_NO, COMPANY_CODE, TO_LOCATION_CODE) A,
        //                            MP_ORDER_PLAN_PROCESS B
        //                      WHERE a.PLAN_NO = b.PLAN_NO
        //                            AND A.COMPANY_CODE = B.COMPANY_CODE
        //                            AND PRODUCTION_QTY >= B.PLAN_QUANTITY)
        //    )
        //    WHERE RN > {offset} AND RN <= {offset + pageSize}
        //    ORDER BY TT
        //";

        //        //  var data = ExecuteQuery(pagedSql); // helper to get list of rows
        //        var data = this._dbContext.SqlQuery<PlanItemModel>(pagedSql).ToList();
        //        return new
        //        {
        //            data = data,
        //            total = totalCount
        //        };
        //    }



        public object GetPlanSearchWithQueryWhileIssueItemForPPlan(string searchText, int page, int pageSize)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;

            if (string.IsNullOrEmpty(searchText))
            {
                searchText = "";
            }
            searchText = searchText.ToUpper();

            var offset = (page - 1) * pageSize;

            // Total count query (same as before)
            var countSql = $@"
        SELECT COUNT(DISTINCT PLAN_NO) AS TOTAL_COUNT
        FROM MP_ORDER_PLAN_PROCESS A, MP_VARIANCE_INFO B, MP_PROCESS_SETUP C
        WHERE A.COMPANY_CODE = '{company_code}'
            AND A.PLAN_NO = B.PLAN_CODE
            AND A.COMPANY_CODE = B.COMPANY_CODE
            AND B.PROCESS_CODE = C.PROCESS_CODE
            AND B.COMPANY_CODE = C.COMPANY_CODE
            AND (UPPER(PLAN_NAME) LIKE '%{searchText}%' OR TO_CHAR(PLAN_NO) LIKE '%{searchText}%')
            AND A.DELETED_FLAG = 'N'
            AND (PLAN_NO, LOCATION_CODE) NOT IN
                (SELECT A.PLAN_NO, TO_LOCATION_CODE
                   FROM (  SELECT PLAN_NO,
                                  COMPANY_CODE,
                                  TO_LOCATION_CODE,
                                  SUM(PRODUCTION_QTY) PRODUCTION_QTY
                             FROM (SELECT DISTINCT ISSUE_NO,
                                                   COMPANY_CODE,
                                                   NVL(PLAN_NO, 0) PLAN_NO,
                                                   PRODUCTION_QTY,
                                                   TO_LOCATION_CODE
                                     FROM IP_PRODUCTION_ISSUE
                                    WHERE COMPANY_CODE = '{company_code}'
                                          AND DELETED_FLAG = 'N')
                         GROUP BY PLAN_NO, COMPANY_CODE, TO_LOCATION_CODE) A,
                        MP_ORDER_PLAN_PROCESS B
                  WHERE A.PLAN_NO = B.PLAN_NO
                    AND A.COMPANY_CODE = B.COMPANY_CODE
                    AND PRODUCTION_QTY >= B.PLAN_QUANTITY)";

            var totalCount = this._dbContext.SqlQuery<int>(countSql).FirstOrDefault();

            // Paged query using OFFSET FETCH (Oracle 12c+)
            var pagedSql = $@"
                            SELECT *
                    FROM (
                        SELECT PLAN_NO, PLAN_NO AS CODE, PLAN_NAME AS TT,
                               ROW_NUMBER() OVER (ORDER BY PLAN_NAME) AS RN
                        FROM (
                            SELECT DISTINCT PLAN_NO, PLAN_NAME
                            FROM MP_ORDER_PLAN_PROCESS A
                                 JOIN MP_VARIANCE_INFO B ON A.PLAN_NO = B.PLAN_CODE AND A.COMPANY_CODE = B.COMPANY_CODE
                                 JOIN MP_PROCESS_SETUP C ON B.PROCESS_CODE = C.PROCESS_CODE AND B.COMPANY_CODE = C.COMPANY_CODE
                            WHERE A.COMPANY_CODE = '{company_code}'
                              AND (UPPER(PLAN_NAME) LIKE '%{searchText}%' OR TO_CHAR(PLAN_NO) LIKE '%{searchText}%')
                              AND A.DELETED_FLAG = 'N'
                              AND (PLAN_NO, LOCATION_CODE) NOT IN
                                  (SELECT A.PLAN_NO, TO_LOCATION_CODE
                                     FROM ( SELECT PLAN_NO, COMPANY_CODE, TO_LOCATION_CODE,
                                                   SUM(PRODUCTION_QTY) PRODUCTION_QTY
                                              FROM (SELECT DISTINCT ISSUE_NO, COMPANY_CODE,
                                                                   NVL(PLAN_NO, 0) PLAN_NO,
                                                                   PRODUCTION_QTY, TO_LOCATION_CODE
                                                      FROM IP_PRODUCTION_ISSUE
                                                     WHERE COMPANY_CODE = '{company_code}'
                                                       AND DELETED_FLAG = 'N')
                                            GROUP BY PLAN_NO, COMPANY_CODE, TO_LOCATION_CODE) A
                                          JOIN MP_ORDER_PLAN_PROCESS B
                                            ON A.PLAN_NO = B.PLAN_NO AND A.COMPANY_CODE = B.COMPANY_CODE
                                   WHERE PRODUCTION_QTY >= B.PLAN_QUANTITY)
                        )
                    )
                    WHERE RN > {offset} AND RN <= ('{offset}' + '{pageSize}')
                    ORDER BY TT";

            var data = this._dbContext.SqlQuery<PlanItemModel>(pagedSql).ToList();

            return new
            {
                data = data,
                total = totalCount
            };
        }


        public object GenerateRequisition(GenerateRequisitionRequest request)
        {
            try
            {
                //  // Validate input
                //  if (request == null)
                //  {
                //      throw new ArgumentException("Request cannot be null");
                //  }
                //  if (request.items == null || request.items.Count == 0)
                //  {
                //      throw new ArgumentException("No items to generate requisition");
                //  }
                //  if (string.IsNullOrEmpty(request.planNo))
                //  {
                //      throw new ArgumentException("Plan number is required");
                //  }
                //  var userid = _workContext.CurrentUserinformation.User_id;
                //  var company_code = _workContext.CurrentUserinformation.company_code;
                //  var branch_code = _workContext.CurrentUserinformation.branch_code;
                //  var user_type = _workContext.CurrentUserinformation.UserType;
                //  // Generate requisition number
                //  string requisitionNo = GetNewRequisitionNumber(company_code);
                //  string formCode = "200";
                //  string currencyformat = "NRS";
                //  var Grand_Total = 0;
                //  decimal exchangrate = 1;
                //  string today = DateTime.Now.ToString("dd-MMM-yyyy");
                //  bool insertedToMaster = false;
                //  string insertmasterQuery = string.Format(@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,SESSION_ROWID,SYN_ROWID,EXCHANGE_RATE,REFERENCE_NO) 
                //VALUES('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}',{8},TO_DATE({9},'DD-MON-YY hh24:mi:ss'),'{10}','{11}',{12},'{13}')",
                //      requisitionNo, Grand_Total, formCode, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code, _workContext.CurrentUserinformation.login_code.ToUpper(), 'N', currencyformat, "SYSDATE", today, "", "", exchangrate, "");
                //  var table = _dbContext.ExecuteSqlCommand(insertmasterQuery);
                //  // Insert requisition details directly (no separate header table based on existing patterns)
                //  int serialNo = 1;
                //  foreach (var item in request.items)
                //  {
                //      var insertDetailQuery = $@"
                //          INSERT INTO IP_GOODS_REQUISITION (
                //              REQUISITION_NO, 
                //              REQUISITION_DATE, 
                //              MANUAL_NO, 
                //              FROM_LOCATION_CODE, 
                //              TO_LOCATION_CODE, 
                //              SUPPLIER_CODE,
                //              ITEM_CODE, 
                //              SERIAL_NO, 
                //              MU_CODE, 
                //              QUANTITY, 
                //              UNIT_PRICE, 
                //              TOTAL_PRICE, 
                //              CALC_QUANTITY, 
                //              CALC_UNIT_PRICE, 
                //              CALC_TOTAL_PRICE, 
                //              COMPLETED_QUANTITY, 
                //              REMARKS, 
                //              FORM_CODE, 
                //              COMPANY_CODE, 
                //              BRANCH_CODE, 
                //              CREATED_BY, 
                //              CREATED_DATE, 
                //              DELETED_FLAG, 
                //              CURRENCY_CODE, 
                //              EXCHANGE_RATE, 
                //              SYN_ROWID, 
                //              TRACKING_NO, 
                //              SESSION_ROWID, 
                //              MODIFY_DATE, 
                //              MODIFY_BY, 
                //              CUSTOMER_CODE, 
                //              SECOND_QUANTITY, 
                //              THIRD_QUANTITY,
                //              SUB_PROJECT_CODE
                //          ) VALUES (
                //              '{requisitionNo}',
                //              SYSDATE,
                //              '{requisitionNo}',
                //              'PROD',
                //              'STORE',
                //              NULL,
                //              '{item.itemCode}',
                //              {serialNo},
                //              '{item.unit}',
                //              {item.requiredQuantity ?? 0},
                //              0,
                //              0,
                //              {item.requiredQuantity ?? 0},
                //              0,
                //              0,
                //              0,
                //              'Process: {item.processName} - {item.processTypeName} - Plan No: {request.planNo}',
                //              'IP_GOODS_REQUISITION',
                //              '{company_code}',
                //              '{branch_code}',
                //              '{userid}',
                //              SYSDATE,
                //              'N',
                //              'NPR',
                //              1,
                //              '{Guid.NewGuid()}',
                //              '{Guid.NewGuid()}',
                //              '{Guid.NewGuid()}',
                //              SYSDATE,
                //              '{userid}',
                //              NULL,
                //              0,
                //              0,
                //              NULL
                //          )";
                //      _dbContext.ExecuteSqlCommand(insertDetailQuery);
                //      serialNo++;
                //  }



                //  // Return success response
                //  var response = new
                //  {
                //      status = "success",
                //      requisitionNo = requisitionNo,
                //      planNo = request.planNo,
                //      count = request.items.Count,
                //      message = "Requisition generated successfully",
                //      items = request.items
                //  };

                return new object();
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while generating requisition: " + ex.StackTrace);
                throw new Exception("Failed to generate requisition: " + ex.Message);
            }
        }


        public List<FormSetupDto> GetFormMappingList()
        {
            var company_code = _workContext.CurrentUserinformation.company_code;

            string formQuery = $@"
                SELECT form_edesc, form_code
                FROM form_setup
                WHERE COMPANY_CODE = '{company_code}'
                  AND DELETED_FLAG = 'N'
                  AND form_code IN (
                      SELECT form_code
                      FROM form_detail_setup
                      WHERE table_name IN ('IP_GOODS_REQUISITION', '')
                  )
                ORDER BY form_edesc";

            // Execute the query
            var result = _dbContext.SqlQuery<FormSetupDto>(formQuery).ToList();

            return result;
        }


        public List<FormSetupDto> GetIndentFormMappingList()
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            string formQuery = $@"
                SELECT form_edesc, form_code
                FROM form_setup
                WHERE COMPANY_CODE = '{company_code}'
                  AND DELETED_FLAG = 'N'
                  AND form_code IN (
                      SELECT form_code
                      FROM form_detail_setup
                      WHERE table_name IN ('IP_PURCHASE_REQUEST')
                  )
                ORDER BY form_edesc";

            // Execute the query
            var result = _dbContext.SqlQuery<FormSetupDto>(formQuery).ToList();

            return result;
        }

        public string InsertProductionPreference(ProductionPreference prefObj)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;
                var user_type = _workContext.CurrentUserinformation.login_code;

                var existsQuery = $@"
                                    SELECT COUNT(1) 
                                    FROM MP_PRODUCTION_PREFERENCE
                                    WHERE BRANCH_CODE = '{branch_code}' AND COMPANY_CODE='{company_code}'";

                var count = this._dbContext.SqlQuery<int?>(existsQuery).FirstOrDefault();

                if (count != null && count > 0)
                {
                    var updateQuery = $@"
                            UPDATE MP_PRODUCTION_PREFERENCE
                            SET 
                                INDENT_FORM_CODE = '{prefObj.INDENT_FORM_CODE}',
                                REQUISITION_FORM_CODE = '{prefObj.REQUISITION_FORM_CODE}',
                                MODIFY_BY        = '{user_type}',
                                MODIFY_DATE      = SYSDATE,
                                SYN_ROWID        = '',
                                REQ_FROM_LOCATION_CODE = '{prefObj.REQ_FROM_LOCATION_CODE}',
                                REQ_TO_LOCATION_CODE = '{prefObj.REQ_TO_LOCATION_CODE}',
                                IND_FROM_LOCATION_CODE = '{prefObj.IND_FROM_LOCATION_CODE}',
                                IND_TO_LOCATION_CODE = '{prefObj.IND_TO_LOCATION_CODE}'
                              WHERE BRANCH_CODE = '{branch_code}' AND COMPANY_CODE='{company_code}'";

                    _dbContext.ExecuteSqlCommand(updateQuery);

                }
                else
                {
                    var insertQuery = $@"
                                INSERT INTO MP_PRODUCTION_PREFERENCE
                                (
                                    REQUISITION_FORM_CODE,
                                    INDENT_FORM_CODE,
                                    BRANCH_CODE,
                                    CREATED_BY,
                                    CREATED_DATE,
                                    MODIFY_DATE,
                                    MODIFY_BY,
                                    SYN_ROWID,
                                    COMPANY_CODE,
                                    REQ_FROM_LOCATION_CODE,
                                    REQ_TO_LOCATION_CODE,
                                    IND_FROM_LOCATION_CODE,
                                    IND_TO_LOCATION_CODE
                                )
                                VALUES
                                (
                                    '{prefObj.REQUISITION_FORM_CODE}',
                                    '{prefObj.INDENT_FORM_CODE}',
                                    '{branch_code}',
                                    '{user_type}',
                                    SYSDATE,
                                    '',
                                    '',
                                    '',
                                    '{company_code}',
                                    '{prefObj.REQ_FROM_LOCATION_CODE}',
                                    '{prefObj.REQ_TO_LOCATION_CODE}',
                                    '{prefObj.IND_FROM_LOCATION_CODE}',
                                    '{prefObj.IND_TO_LOCATION_CODE}'
                                )";

                    _dbContext.ExecuteSqlCommand(insertQuery);
                }

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            return "Saved";
        }


        public ProductionPreference GetProductionPreferences()
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var userid = _workContext.CurrentUserinformation.User_id;
            var user_type = _workContext.CurrentUserinformation.UserType;

            string query = $@"
                    SELECT REQUISITION_FORM_CODE,
                           INDENT_FORM_CODE,
                           BRANCH_CODE,
                           CREATED_BY,
                           CREATED_DATE,
                           MODIFY_DATE,
                           MODIFY_BY,
                           SYN_ROWID,
                           MODULEORSCREEN,
                           REQ_FROM_LOCATION_CODE,
                           REQ_TO_LOCATION_CODE,
                           IND_FROM_LOCATION_CODE,
                           IND_TO_LOCATION_CODE 
                      FROM MP_PRODUCTION_PREFERENCE
                     WHERE BRANCH_CODE = '{branch_code}'        -- optional filter
                       AND COMPANY_CODE = '{company_code}'      -- optional filter
                     ORDER BY CREATED_DATE DESC";

            // Execute the query
            var result = _dbContext.SqlQuery<ProductionPreference>(query).FirstOrDefault();
            if (result != null)
            {
                string query1 = $@"SELECT table_name
                                FROM form_detail_setup
                                WHERE Form_code = '{result.REQUISITION_FORM_CODE}'
                                AND ROWNUM = 1";
                string tableName = _dbContext.SqlQuery<string>(query1).FirstOrDefault();
                result.TABLE_NAME = tableName;


                string query11 = $@"SELECT table_name
                                FROM form_detail_setup
                                WHERE Form_code = '{result.INDENT_FORM_CODE}'
                                AND ROWNUM = 1";
                string tableNameIndent = _dbContext.SqlQuery<string>(query11).FirstOrDefault();
                result.TABLE_NAME_INDENT = tableNameIndent;


                string queryMODULE_CODE = $@"SELECT MODULE_CODE
                                FROM form_setup
                                WHERE Form_code = '{result.REQUISITION_FORM_CODE}'
                                AND ROWNUM = 1";
                string MODULE_CODE_REQ = _dbContext.SqlQuery<string>(queryMODULE_CODE).FirstOrDefault();
                if (MODULE_CODE_REQ != null)
                {
                    result.MODULE_CODE_REQUISITION = MODULE_CODE_REQ;
                }


                string queryMODULE_CODE_IND = $@"SELECT MODULE_CODE
                                FROM form_setup
                                WHERE Form_code = '{result.INDENT_FORM_CODE}'
                                AND ROWNUM = 1";
                string MODULE_CODE_IND = _dbContext.SqlQuery<string>(queryMODULE_CODE_IND).FirstOrDefault();
                if (MODULE_CODE_IND != null)
                {
                    result.MODULE_CODE_INDENT = MODULE_CODE_IND;
                }

            }

            return result;
        }



















































        #region PrivateMethod region
        private List<VarianceInfoModel> PrepareVarianceInfoForInsertItemRecursiveM(
            string company_code,
            string main_item_code,
            decimal plan_code,
            decimal requestedQty,
            string item_code = null,
            List<ItemIndexCapacityModel> itemIndexCapacityModelList = null,
            List<VarianceInfoModel> varianceInfoModelList = null)
        {
            if (varianceInfoModelList == null)
            {
                varianceInfoModelList = new List<VarianceInfoModel>();
            }
            var user_type = _workContext.CurrentUserinformation.login_code;

            try
            {

                string checkExistQuery = $@"
                                SELECT COUNT(*) 
                                FROM MP_VARIANCE_INFO 
                                WHERE COMPANY_CODE = '{company_code}' 
                                  AND PLAN_CODE = '{plan_code}'";

                int count = _dbContext.SqlQuery<int>(checkExistQuery).FirstOrDefault();
                bool exists = count > 0;
                if (exists)
                {
                    string varianceInfoDeleteQuery = $@"DELETE MP_VARIANCE_INFO WHERE COMPANY_CODE = '{company_code}' AND PLAN_CODE = '{plan_code}'";
                    var d = _dbContext.ExecuteSqlCommand(varianceInfoDeleteQuery);
                }


                string itemGInfo = $@"SELECT PROCESS_CODE, LOCATION_CODE, PROD_GROUP FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{item_code}' AND COMPANY_CODE = '{company_code}'";
                var GroupInfoModel = _dbContext.SqlQuery<GroupInfoModel>(itemGInfo).FirstOrDefault();

                List<ItemQtyProcessCodeModel> itemQtyProcessCodeModelList = new List<ItemQtyProcessCodeModel>();
                if (GroupInfoModel != null)
                {
                    string itemQuery = $@"SELECT ITEM_CODE, QUANTITY, PROCESS_CODE, SERIAL_NO  
                                        FROM MP_ROUTINE_INPUT_SETUP WHERE PROCESS_CODE = 
                                        (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{item_code}' AND COMPANY_CODE = '{company_code}' AND PROD_GROUP = '{GroupInfoModel.PROD_GROUP}') 
                                        AND COMPANY_CODE = '{company_code}' order by serial_no";
                    itemQtyProcessCodeModelList = _dbContext.SqlQuery<ItemQtyProcessCodeModel>(itemQuery).ToList();
                }



                //string itemQry = $@"SELECT QUANTITY FROM MP_ROUTINE_OUTPUT_SETUP WHERE PROCESS_CODE = 
                //                    (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE ='{item_code}' AND COMPANY_CODE = '{company_code}') 
                //                    AND COMPANY_CODE = '{company_code}' AND ITEM_CODE ='{item_code}'";

                string itemQry = $@"SELECT PROCESS_CODE,INDEX_ITEM_CODE, INDEX_CAPACITY FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE = '{item_code}' AND COMPANY_CODE = '{company_code}'";
                var result = _dbContext.SqlQuery<ItemIndexCapacityModel>(itemQry).FirstOrDefault();

                if (itemIndexCapacityModelList == null)
                    itemIndexCapacityModelList = new List<ItemIndexCapacityModel>();

                var objModel = new ItemIndexCapacityModel()
                {
                    INDEX_CAPACITY = result != null ? result.INDEX_CAPACITY : 0,
                    PROCESS_CODE = result != null ? result.PROCESS_CODE : "",
                    INDEX_ITEM_CODE = result != null ? result.INDEX_ITEM_CODE : "",
                    IS_REQUESTED = true,
                    REQUESTED_INPUT_QTY = requestedQty,
                    ITEM_CODE = item_code,
                    PARENT_ITEM_CODE = main_item_code == item_code ? null : main_item_code
                };
                itemIndexCapacityModelList.Add(objModel);


                if (itemQtyProcessCodeModelList.Count > 0)
                {
                    foreach (var item in itemQtyProcessCodeModelList)
                    {
                        // sNo += 1;
                        string categoryCodeQuery = $@"SELECT CATEGORY_CODE FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{item.ITEM_CODE}' AND COMPANY_CODE = '{company_code}'";
                        var CategoryCode = _dbContext.SqlQuery<string>(categoryCodeQuery).FirstOrDefault();


                        decimal RequiredQuantityValue = 0;
                        if (item.QUANTITY != null)
                        {

                            var planQty = requestedQty; // main input Qty from UI/ Plan Qty


                            var outPutItemDtl = varianceInfoModelList.Where(x => x.RawItemCode == item_code).FirstOrDefault();
                            if (outPutItemDtl != null && outPutItemDtl.RequiredQuantity > 0)
                            {
                                planQty = Convert.ToDecimal(outPutItemDtl.RequiredQuantity);
                            }
                            // Prepare ration and multiply with requested Qty
                            var dataCapacityModel = itemIndexCapacityModelList.Where(x => x.ITEM_CODE == item_code).FirstOrDefault();
                            RequiredQuantityValue = (Convert.ToDecimal(item.QUANTITY) / dataCapacityModel.INDEX_CAPACITY) * planQty;
                        }


                        var varianceObj = new VarianceInfoModel()
                        {
                            PlanCode = plan_code.ToString(),
                            FinishedItemCode = main_item_code,
                            FinishedQuantity = requestedQty,
                            RawItemCode = item.ITEM_CODE,
                            RequiredQuantity = RequiredQuantityValue,
                            CategoryCode = CategoryCode,
                            ProcessCode = item.PROCESS_CODE,
                            CompanyCode = company_code,
                            CreatedBy = user_type,
                            CreatedDate = DateTime.Now,
                            DeletedFlag = "N",
                            ModifiedBy = null,
                            ModifyDate = null,
                            ModifyEntry = null
                        };
                        varianceObj.SerialNo = varianceInfoModelList.Count() + 1;
                        varianceInfoModelList.Add(varianceObj);

                        string outputQuery = $@"SELECT QUANTITY FROM MP_ROUTINE_OUTPUT_SETUP WHERE PROCESS_CODE = (SELECT PROCESS_CODE FROM MP_PROCESS_SETUP WHERE ROWNUM = 1 AND INDEX_ITEM_CODE = '{item.ITEM_CODE}' AND COMPANY_CODE = '{company_code}') AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}'";
                        var outputItemQty = _dbContext.SqlQuery<decimal?>(outputQuery).FirstOrDefault();

                        if (outputItemQty != null)
                        {
                            //var mainParentItemCode = main_item_code; // main requested item_code
                            PrepareVarianceInfoForInsertItemRecursiveM(company_code, main_item_code, plan_code, requestedQty, item.ITEM_CODE, itemIndexCapacityModelList, varianceInfoModelList);
                        }
                    }
                }

                return varianceInfoModelList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Get error while getting production item details  " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        private int InsertVarianceInfoTable(List<VarianceInfoModel> varianceInfoList)
        {
            int i = 0;
            foreach (var varianceObj in varianceInfoList)
            {
                var createdDate = varianceObj.CreatedDate.ToString("MM/dd/yyyy");
                var modifyDateValue = varianceObj.ModifyDate.HasValue
                    ? $"TO_DATE('{varianceObj.ModifyDate:MM/dd/yyyy}', 'MM/dd/yyyy')"
                    : "";

                var modifiedByValue = string.IsNullOrEmpty(varianceObj.ModifiedBy)
                    ? ""
                    : $"'{varianceObj.ModifiedBy}'";

                var requiredQtyValue = varianceObj.RequiredQuantity;

                var insertStatementQuery = $@"INSERT INTO MP_VARIANCE_INFO (
                                                    CATEGORY_CODE,
                                                    COMPANY_CODE,
                                                    CREATED_BY,
                                                    CREATED_DATE,
                                                    DELETED_FLAG,
                                                    FINISHED_ITEM_CODE,
                                                    FINISHED_QUANTITY,
                                                    MODIFY_BY,
                                                    MODIFY_DATE,
                                                    PLAN_CODE,
                                                    PROCESS_CODE,
                                                    RAW_ITEM_CODE,
                                                    REQUIRED_QUANTITY,
                                                    SERIAL_NO
                                                ) VALUES (
                                                    '{varianceObj.CategoryCode}',
                                                    '{varianceObj.CompanyCode}',
                                                    '{varianceObj.CreatedBy}',
                                                     SYSDATE,
                                                    '{varianceObj.DeletedFlag}',
                                                    '{varianceObj.FinishedItemCode}',
                                                    '{varianceObj.FinishedQuantity}',
                                                    '{modifiedByValue}',
                                                    '{modifyDateValue}',
                                                    '{varianceObj.PlanCode}',
                                                    '{varianceObj.ProcessCode}',
                                                    '{varianceObj.RawItemCode}',
                                                    '{requiredQtyValue}',
                                                    '{varianceObj.SerialNo}'
                                                )";
                try
                {
                    i += _dbContext.ExecuteSqlCommand(insertStatementQuery);
                }
                catch (Exception ex)
                {
                    _logErp.ErrorInDB("Get error while getting production item required calculation  " + ex.StackTrace);
                    throw new Exception(ex.Message);
                }
            }
            return i;
        }

        private string GetOracleDate(DateTime? date)
        {
            return date.HasValue ? $"TO_DATE('{date.Value:MM/dd/yyyy}', 'MM/dd/yyyy')" : "NULL";
        }

        private string GetQueryStringForOrderListByOrderNo(string orderNo, string companyCode)
        {
            string itemQuery = $@" SELECT*
                           FROM(SELECT A.ORDER_NO,
                                          A.ORDER_DATE,
                                          A.DELIVERY_DATE,
                                          A.ITEM_CODE,
                                          E.ITEM_EDESC,
                                          A.FORM_CODE,
                                          A.CUSTOMER_CODE,
                                          B.CUSTOMER_EDESC,
                                          A.QUANTITY,
                                          (SELECT COUNT(*)
                                             FROM REFERENCE_DETAIL
                                            WHERE REFERENCE_NO = A.ORDER_NO
                                                  AND REFERENCE_FORM_CODE = A.FORM_CODE
                                                  AND COMPANY_CODE = A.COMPANY_CODE)
                                              TAKEN,
                                           SUM(NVL(D.PLAN_QUANTITY, 0)) PLAN_QUANTITY
                                    FROM SA_SALES_ORDER A,
                                         SA_CUSTOMER_SETUP B,
                                           MASTER_TRANSACTION C,
                                           MP_PLAN_WISE_ORDER D,
                                           IP_ITEM_MASTER_SETUP E
                                     WHERE a.CUSTOMER_CODE = b.CUSTOMER_CODE
                                           AND A.COMPANY_CODE = B.COMPANY_CODE
                                           AND A.ORDER_NO = C.VOUCHER_NO
                                           AND A.FORM_CODE = C.FORM_CODE
                                           AND A.COMPANY_CODE = C.COMPANY_CODE
                                           AND C.AUTHORISED_BY IS NOT NULL
                                           AND A.ITEM_CODE = E.ITEM_CODE
                                           AND A.COMPANY_CODE = E.COMPANY_CODE
                                           AND A.COMPANY_CODE = '{companyCode}'
                                           AND A.COMPANY_CODE = D.COMPANY_CODE(+)
                                           AND A.ORDER_NO = D.ORDER_NO(+)
                                           AND A.FORM_CODE = D.FORM_CODE(+)
                                           AND A.ITEM_CODE = D.ITEM_CODE(+)
                                           AND C.VOUCHER_NO = '{orderNo}'
                                  GROUP BY A.ORDER_NO,
                                           A.ORDER_DATE,
                                           A.DELIVERY_DATE,
                                           A.ITEM_CODE,
                                           E.ITEM_EDESC,
                                           A.FORM_CODE,
                                           A.CUSTOMER_CODE,
                                           B.CUSTOMER_EDESC,
                                           A.QUANTITY,
                                           A.COMPANY_CODE)
                           WHERE TAKEN = 0 AND NVL(PLAN_QUANTITY, 0) <> QUANTITY
                        ORDER BY ORDER_NO";

            return itemQuery;
        }


        private string GetQueryStringForPipeListItemsByOrderNo(string order_no, string plan_no, string company_code)
        {
            string itemQuery = $@"SELECT A.LOCATION_CODE,
                                     B.LOCATION_EDESC,
                                     A.ITEM_EDESC,
                                     A.MU_CODE,
                                     SUM (IN_QUANTITY - OUT_QUANTITY) BALANCE_QUANTITY,
                                     (SELECT SUM (PLAN_QUANTITY)
                                        FROM MP_ORDER_PLAN_PROCESS
                                       WHERE     ITEM_CODE = A.ITEM_CODE
                                             AND COMPANY_CODE = A.COMPANY_CODE
                                             AND PLAN_NO <> '{plan_no}'
                                             AND PLAN_NO NOT IN
                                                    (SELECT NVL (PLAN_NO, 0)
                                                       FROM IP_PRODUCTION_MRR
                                                      WHERE COMPANY_CODE = '{company_code}' AND DELETED_FLAG = 'N'))
                                        PLAN_QUANTITY
                                FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A, IP_LOCATION_SETUP B
                               WHERE     A.LOCATION_CODE = B.LOCATION_CODE
                                     AND A.COMPANY_CODE = B.COMPANY_CODE
                                     AND A.COMPANY_CODE = '{company_code}'
                                     AND A.ITEM_CODE IN
                                            (SELECT ITEM_CODE
                                               FROM SA_SALES_ORDER
                                              WHERE COMPANY_CODE = '{company_code}'
                                                    AND ORDER_NO = '{order_no}')
                            GROUP BY A.LOCATION_CODE,
                                     B.LOCATION_EDESC,
                                     A.ITEM_EDESC,
                                     A.MU_CODE,
                                     A.ITEM_CODE,
                                     A.COMPANY_CODE
                            ORDER BY B.LOCATION_EDESC";

            return itemQuery;
        }


        private string GetPlanSearchWithQueryWhileIssueItemForPPlan(string searchText)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var user_type = _workContext.CurrentUserinformation.UserType;

            return $@"
                      SELECT DISTINCT PLAN_NO, PLAN_NO CODE, PLAN_NAME TT
                        FROM MP_ORDER_PLAN_PROCESS A, MP_VARIANCE_INFO B, MP_PROCESS_SETUP C
                       WHERE     A.COMPANY_CODE = '{company_code}'
                             AND A.PLAN_NO = B.PLAN_CODE
                             AND A.COMPANY_CODE = B.COMPANY_CODE
                             AND B.PROCESS_CODE = C.PROCESS_CODE
                             AND B.COMPANY_CODE = C.COMPANY_CODE
                             AND (UPPER (PLAN_NAME) LIKE '%{searchText}%' OR TO_CHAR (PLAN_NO) LIKE '%{searchText}%')
                             AND A.DELETED_FLAG = 'N'
                             AND (PLAN_NO, LOCATION_CODE) NOT IN
                                    (SELECT A.PLAN_NO, TO_LOCATION_CODE
                                       FROM (  SELECT PLAN_NO,
                                                      COMPANY_CODE,
                                                      TO_LOCATION_CODE,
                                                      SUM (PRODUCTION_QTY) PRODUCTION_QTY
                                                 FROM (SELECT DISTINCT ISSUE_NO,
                                                                       COMPANY_CODE,
                                                                       NVL (PLAN_NO, 0) PLAN_NO,
                                                                       PRODUCTION_QTY,
                                                                       TO_LOCATION_CODE
                                                         FROM IP_PRODUCTION_ISSUE
                                                        WHERE     COMPANY_CODE = '{company_code}'
                                                              AND DELETED_FLAG = 'N')
                                             GROUP BY PLAN_NO, COMPANY_CODE, TO_LOCATION_CODE) A,
                                            MP_ORDER_PLAN_PROCESS B
                                      WHERE     a.PLAN_NO = b.PLAN_NO
                                            AND A.COMPANY_CODE = B.COMPANY_CODE
                                            AND PRODUCTION_QTY >= B.PLAN_QUANTITY)
                    ORDER BY PLAN_NAME";
        }






        #endregion
    }
}
