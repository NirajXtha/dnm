using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Repository
{
    public class FinishedGoodsInspectionRepo : IFinishedGoodsInspectionRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public FinishedGoodsInspectionRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<FinishedGoodsInspection> GetFinishedGoodsInspectionField()
        {
            List<FinishedGoodsInspection> raw = new List<FinishedGoodsInspection>();
            String query_globalProducts = $@"select IPD.PARAMETER_ID,IP.PARAMETERS from FINISHED_PARAM_DETAILS IPD
                                        INNER JOIN FINISHED_INSPECTION_PARAMETERS IP ON IPD.PARAMETER_ID = IP.FINISHED_PARAM_NO ORDER BY IPD.PARAMETER_ID";
            var raws = _dbContext.SqlQuery<PreDispatchInspection>(query_globalProducts).ToList();

            String query_globalWeight1 = $@"select FINISH_GOODS_INSP_NO,PACK_CONDITION,PACK_COND_REMARKS
,LABEL_ACCURACY,LABEL_ACC_REMARKS,PRODUCT_APPEARANCE,PRODUCT_APP_REMARKS,DIMENSIONS
,DIMENSIONS_REMARKS,COMPLIANCE_CERTIFICATES,COMP_CERT_REMARKS,VENDOR_TEST,VENDOR_TEST_REMARKS
,SAMPLING_METHOD,SAMP_METHOD_REMARKS,SAMPLE_SIZE,SAMP_SIZE_REMARKS,NUMBER_PASSED
,NUMBER_PASSED_REMARKS,DEFECT_TYPE,DEFECT_TYPE_REMARKS,ACTION_TAKEN,ACTION_TAKEN_REMARKS
,REMARKS,FINAL_REMARKS,SUPPLIER_CODE
 from FINISH_GOODS_INSPECTION";

            var raws1 = _dbContext.SqlQuery<PreDispatchInspectionDetails>(query_globalWeight1).FirstOrDefault();

            if (raws != null)
            {
                foreach (var pp in raws)
                {
                    FinishedGoodsInspection rawmodel = new FinishedGoodsInspection();
                    rawmodel.PARAMETER_ID = pp.PARAMETER_ID;
                    rawmodel.PARAMETERS = pp.PARAMETERS;
                    String query_globalWeight = $@"select FINISH_ITEM_NO AS PARAM_ITEM_NO,ITEM_NAME AS ITEM_EDESC,COLUMN_HEADER,'' Status, '' Remarks from FINISH_ITEM_MAP where FINISH_ITEM_NO='{pp.PARAMETER_ID}'";
                    rawmodel.FinishedGoodsInspectionDetailsList = this._dbContext.SqlQuery<FinishedGoodsInspectionDetails>(query_globalWeight).ToList();
                    raw.Add(rawmodel);
                }
            }
            return raw;
        }

        public List<ItemSetup> GetProductWithCategoryFilter(string ProductType)
        {
            try
            {
                List<ItemSetup> productList = new List<ItemSetup>();
                string query = $@"select ITEM_CODE,ITEM_EDESC from ip_product_item_master_setup  where PRODUCT_TYPE ='{ProductType}' AND 
                                CATEGORY_CODE IN ( SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE  WHERE CATEGORY_TYPE = 'FG')";
                productList = this._dbContext.SqlQuery<ItemSetup>(query).ToList();
                return productList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool InsertFinishedGoodsInspectionData(FinishedGoodsInspection data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.FINISH_GOODS_INSP_NO}'";
                    string FINISH_GOODS_INSP_NO = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                    string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'FI'";
                    string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();

                    if (FINISH_GOODS_INSP_NO == null)
                    {
                        string insertMasterQuery = string.Format(@"
                            INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO,ITEM_CODE,BATCH_NO,REFERENCE_NO
                                ,CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,QC_CODE,FORM_CODE,GRN_NO,QUANTITY,MFG_DATE,EXP_DATE,VENDOR_NAME,RECEIPT_DATE)
                            VALUES('{0}', '{1}', '{2}', '{3}', '{4}',TO_DATE('{5}', 'DD-MON-YYYY'),'{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                   data.FINISH_GOODS_INSP_NO, data.Plant_Id, data.Batch_No, data.REFERENCE_NO == "" ? "0" : data.REFERENCE_NO, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                           , 'N', _workContext.CurrentUserinformation.company_code,
                           _workContext.CurrentUserinformation.branch_code, 1, 1, form_code,data.GRN_NO,data.QUANTITY,data.MFG_DATE.ToString("dd-MMM-yyyy"), data.EXP_DATE.ToString("dd-MMM-yyyy"), data.VENDOR_NAME,data.RECEIPT_DATE.ToString("dd-MMM-yyyy"));
                        _dbContext.ExecuteSqlCommand(insertMasterQuery);

                        string insertFinishedGoodsQuery = string.Format(@"
                            INSERT INTO FINISH_GOODS_INSPECTION (FINISH_GOODS_INSP_NO,ITEM_CODE,PACK_CONDITION,PACK_COND_REMARKS,LABEL_ACCURACY
                                ,LABEL_ACC_REMARKS,PRODUCT_APPEARANCE,PRODUCT_APP_REMARKS,DIMENSIONS,DIMENSIONS_REMARKS
                                ,COMPLIANCE_CERTIFICATES,COMP_CERT_REMARKS,VENDOR_TEST,VENDOR_TEST_REMARKS,SAMPLING_METHOD
                                ,SAMP_METHOD_REMARKS,SAMPLE_SIZE,SAMP_SIZE_REMARKS,NUMBER_PASSED,NUMBER_PASSED_REMARKS
                                ,DEFECT_TYPE,DEFECT_TYPE_REMARKS,ACTION_TAKEN,ACTION_TAKEN_REMARKS,REMARKS,FINAL_REMARKS
                                ,SUPPLIER_CODE
                                ,CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,FORM_CODE)
                            VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}','{16}', '{17}', '{18}', '{19}', '{20}','{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}',TO_DATE('{28}', 'DD-MON-YYYY'),'{29}','{30}','{31}','{32}','{33}')",
                                      data.FINISH_GOODS_INSP_NO, data.ITEM_CODE, data.PACK_CONDITION, data.PACK_COND_REMARKS, data.LABEL_ACCURACY,
                                      data.LABEL_ACC_REMARKS, data.PRODUCT_APPEARANCE, data.PRODUCT_APP_REMARKS, data.DIMENSIONS, data.DIMENSIONS_REMARKS,
                                      data.COMPLIANCE_CERTIFICATES, data.COMP_CERT_REMARKS, data.VENDOR_TEST, data.VENDOR_TEST_REMARKS, data.SAMPLING_METHOD,
                                      data.SAMP_METHOD_REMARKS, data.SAMPLE_SIZE, data.SAMP_SIZE_REMARKS, data.NUMBER_PASSED, data.NUMBER_PASSED_REMARKS,
                                       data.DEFECT_TYPE, data.DEFECT_TYPE_REMARKS, data.ACTION_TAKEN, data.ACTION_TAKEN_REMARKS, data.REMARKS, data.FINAL_REMARKS,
                                       data.SUPPLIER_CODE,
                                      _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                              , 'N', _workContext.CurrentUserinformation.company_code,
                              _workContext.CurrentUserinformation.branch_code, 1, form_code);
                        _dbContext.ExecuteSqlCommand(insertFinishedGoodsQuery);

                        if (data.ITEM_CODE != null)
                        {
                            string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_PARAM_MAP (INSPECTION_NO,PRODUCT_ID)
                        VALUES('{0}', '{1}')",
                                           data.FINISH_GOODS_INSP_NO, data.ITEM_CODE);
                            _dbContext.ExecuteSqlCommand(insertParam);
                        }
                    }
                    else
                    {
                        string updatePreDispatchQuery = $@"UPDATE FINISH_GOODS_INSPECTION 
                       SET 
                           ITEM_CODE = '{data.ITEM_CODE}',PACK_CONDITION ='{data.PACK_CONDITION}' 
                            ,PACK_COND_REMARKS ='{data.PACK_COND_REMARKS}',LABEL_ACCURACY ='{data.LABEL_ACCURACY}'
                            ,LABEL_ACC_REMARKS ='{data.LABEL_ACC_REMARKS}',PRODUCT_APPEARANCE ='{data.PRODUCT_APPEARANCE}'
                            ,PRODUCT_APP_REMARKS ='{data.PRODUCT_APP_REMARKS}'
                            ,DIMENSIONS ='{data.DIMENSIONS}',DIMENSIONS_REMARKS ='{data.DIMENSIONS_REMARKS}'
                            ,COMPLIANCE_CERTIFICATES ='{data.COMPLIANCE_CERTIFICATES}',COMP_CERT_REMARKS ='{data.COMP_CERT_REMARKS}'
                            ,VENDOR_TEST ='{data.VENDOR_TEST}',VENDOR_TEST_REMARKS ='{data.VENDOR_TEST_REMARKS}'
                            ,SAMPLING_METHOD ='{data.SAMPLING_METHOD}',SAMP_METHOD_REMARKS ='{data.SAMP_METHOD_REMARKS}'
                            ,SAMPLE_SIZE ='{data.SAMPLE_SIZE}',SAMP_SIZE_REMARKS ='{data.SAMP_SIZE_REMARKS}'
                            ,NUMBER_PASSED ='{data.NUMBER_PASSED}',NUMBER_PASSED_REMARKS ='{data.NUMBER_PASSED_REMARKS}'
                            ,DEFECT_TYPE ='{data.DEFECT_TYPE}',DEFECT_TYPE_REMARKS ='{data.DEFECT_TYPE_REMARKS}'
                            ,ACTION_TAKEN ='{data.ACTION_TAKEN}',ACTION_TAKEN_REMARKS ='{data.ACTION_TAKEN_REMARKS}'
                            ,REMARKS ='{data.REMARKS}',FINAL_REMARKS ='{data.FINAL_REMARKS}'
                            ,SUPPLIER_CODE ='{data.SUPPLIER_CODE}',
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE FINISH_GOODS_INSP_NO = '{data.FINISH_GOODS_INSP_NO}' ";
                        _dbContext.ExecuteSqlCommand(updatePreDispatchQuery);

                        if (data.ITEM_CODE != null)
                        {
                            string updateCode = $@"
                        UPDATE PRODUCT_PARAM_MAP SET PRODUCT_ID = '{data.ITEM_CODE}' WHERE INSPECTION_NO = '{data.FINISH_GOODS_INSP_NO}' ";
                            _dbContext.ExecuteSqlCommand(updateCode);
                        }

                        string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                       SET 
                           ITEM_CODE = '{data.Plant_Id}',BATCH_NO = '{data.Batch_No}',RECEIPT_DATE = '{data.RECEIPT_DATE.ToString("dd-MMM-yyyy")}'
                            ,GRN_NO = '{data.GRN_NO}',QUANTITY = '{data.QUANTITY}',VENDOR_NAME = '{data.VENDOR_NAME}'
                           ,REMARKS = '{data.REMARKS}', 
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE TRANSACTION_NO = '{data.FINISH_GOODS_INSP_NO}' ";
                        _dbContext.ExecuteSqlCommand(updateQuery);

                    }
                   
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public FinishedGoodsInspection GetEditFinishedGoodsInspection(string transactionno)
        {
            FinishedGoodsInspection raw = new FinishedGoodsInspection();
            String qc_query = $@"select PT.TRANSACTION_NO as FINISH_GOODS_INSP_NO,PT.ITEM_CODE AS Plant_Id,PPM.PRODUCT_ID AS ITEM_CODE,PT.RECEIPT_DATE AS RECEIPT_DATE,PT.GRN_NO,PT.VENDOR_NAME,PT.REFERENCE_NO,PT.BATCH_NO,PT.MFG_DATE,PT.EXP_DATE,PT.QUANTITY from QC_PARAMETER_TRANSACTION PT LEFT JOIN PRODUCT_PARAM_MAP PPM ON PPM.INSPECTION_NO = PT.TRANSACTION_NO where PT.TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<FinishedGoodsInspection>(qc_query).FirstOrDefault();
            var  raws = _dbContext.SqlQuery<FinishedGoodsInspection>(qc_query).ToList();
            String finish_query = $@" select FINISH_GOODS_INSP_NO,PACK_CONDITION,PACK_COND_REMARKS,LABEL_ACCURACY,LABEL_ACC_REMARKS,PRODUCT_APPEARANCE,PRODUCT_APP_REMARKS,DIMENSIONS,DIMENSIONS_REMARKS,COMPLIANCE_CERTIFICATES,COMP_CERT_REMARKS,VENDOR_TEST,VENDOR_TEST_REMARKS,SAMPLING_METHOD,SAMP_METHOD_REMARKS,SAMPLE_SIZE,SAMP_SIZE_REMARKS,NUMBER_PASSED,NUMBER_PASSED_REMARKS,DEFECT_TYPE,DEFECT_TYPE_REMARKS,ACTION_TAKEN,ACTION_TAKEN_REMARKS,REMARKS,FINAL_REMARKS,SUPPLIER_CODE from FINISH_GOODS_INSPECTION WHERE FINISH_GOODS_INSP_NO= '{transactionno}'";
            List<FinishedGoodsInspectionDetails> finishGoods = new List<FinishedGoodsInspectionDetails>();
            finishGoods = this._dbContext.SqlQuery<FinishedGoodsInspectionDetails>(finish_query).ToList();
            raw.FinishedGoodsInspectionDetailsList = finishGoods;
            return raw;
        }
        public FinishedGoodsInspection GetFinishedGoodsInspectionReport(string transactionno)
        {
            FinishedGoodsInspection raw = new FinishedGoodsInspection();
            String qc_query = $@"select DISTINCT PT.TRANSACTION_NO as FINISH_GOODS_INSP_NO,PT.ITEM_CODE AS Plant_Id,PPM.PRODUCT_ID AS ITEM_CODE,IPIMS.ITEM_EDESC,PT.RECEIPT_DATE AS RECEIPT_DATE,PT.GRN_NO,PT.VENDOR_NAME,PT.REFERENCE_NO,PT.BATCH_NO,PT.MFG_DATE,PT.EXP_DATE,PT.QUANTITY,PT.CHECKED_BY,PT.AUTHORISED_BY,PT.POSTED_BY from QC_PARAMETER_TRANSACTION PT LEFT JOIN PRODUCT_PARAM_MAP PPM ON PPM.INSPECTION_NO = PT.TRANSACTION_NO  LEFT JOIN IP_PRODUCT_ITEM_MASTER_SETUP IPIMS ON IPIMS.ITEM_CODE = PRODUCT_ID where PT.TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<FinishedGoodsInspection>(qc_query).FirstOrDefault();
            var raws = _dbContext.SqlQuery<FinishedGoodsInspection>(qc_query).ToList();
            String finish_query = $@" select FINISH_GOODS_INSP_NO,PACK_CONDITION,PACK_COND_REMARKS,LABEL_ACCURACY,LABEL_ACC_REMARKS,PRODUCT_APPEARANCE,PRODUCT_APP_REMARKS,DIMENSIONS,DIMENSIONS_REMARKS,COMPLIANCE_CERTIFICATES,COMP_CERT_REMARKS,VENDOR_TEST,VENDOR_TEST_REMARKS,CASE WHEN SAMPLING_METHOD = 'R' THEN 'Random' ELSE 'AllSamples' END SAMPLING_METHOD,SAMP_METHOD_REMARKS,SAMPLE_SIZE,SAMP_SIZE_REMARKS,NUMBER_PASSED,NUMBER_PASSED_REMARKS,CASE WHEN DEFECT_TYPE = 'Qty_Variance' THEN 'Qty Variance' WHEN DEFECT_TYPE = 'Print_Mistake' THEN 'Print Mistake' WHEN DEFECT_TYPE = 'Packing_Defect' THEN 'Packing Defect' ELSE 'Others' END DEFECT_TYPE,DEFECT_TYPE_REMARKS,ACTION_TAKEN,ACTION_TAKEN_REMARKS,REMARKS,FINAL_REMARKS,SUPPLIER_CODE from FINISH_GOODS_INSPECTION WHERE FINISH_GOODS_INSP_NO= '{transactionno}'";
            List<FinishedGoodsInspectionDetails> finishGoods = new List<FinishedGoodsInspectionDetails>();
            finishGoods = this._dbContext.SqlQuery<FinishedGoodsInspectionDetails>(finish_query).ToList();
            raw.FinishedGoodsInspectionDetailsList = finishGoods;
            return raw;
        }
    }
}
