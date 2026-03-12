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
    public class OnSiteInspectionRepo : IOnSiteInspectionRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public OnSiteInspectionRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<FormDetailSetup> GetOnSiteInspectionList()
        {
            string query_FormCode = $@"SELECT form_code FROM form_setup WHERE FORM_TYPE = 'OI'";
            string formCode = this._dbContext.SqlQuery<string>(query_FormCode).FirstOrDefault();
            string Query = $@"SELECT FDS.SERIAL_NO,
                            FS.FORM_EDESC,
                            FS.FORM_TYPE,
                            FS.NEGATIVE_STOCK_FLAG,
                           FDS.FORM_CODE,
                           FDS.TABLE_NAME,
                           FDS.COLUMN_NAME,
                           FDS.COLUMN_WIDTH,
                           FDS.COLUMN_HEADER,
                           FDS.TOP_POSITION,
                           FDS.LEFT_POSITION,
                           FDS.DISPLAY_FLAG,
                           FDS.DEFA_VALUE,
                           FDS.IS_DESC_FLAG,
                           FDS.MASTER_CHILD_FLAG,
                           FDS.FORM_CODE,
                           FDS.COMPANY_CODE,
                           CS.COMPANY_EDESC,
                            CS.TELEPHONE,
                            CS.EMAIL,
                            CS.TPIN_VAT_NO,
                            CS.ADDRESS,
                           FDS.CREATED_BY,
                           FDS.CREATED_DATE,
                           FDS.DELETED_FLAG,
                           FDS.FILTER_VALUE,
                           FDS.SYN_ROWID,
                           FDS.MODIFY_DATE,
                           FDS.MODIFY_BY,
                           FS.REFERENCE_FLAG,
                           FS.FREEZE_MASTER_REF_FLAG,
                           FS.REF_FIX_QUANTITY,
                           FS.REF_FIX_PRICE,
                           FS.DISPLAY_RATE,
                           FS.RATE_SCHEDULE_FIX_PRICE,
                           FS.PRICE_CONTROL_FLAG
                      FROM    FORM_DETAIL_SETUP FDS
                           LEFT JOIN
                              COMPANY_SETUP CS ON FDS.COMPANY_CODE = CS.COMPANY_CODE
                              LEFT JOIN FORM_SETUP FS
                               ON FDS.FORM_CODE = FS.FORM_CODE AND FDS.COMPANY_CODE = FS.COMPANY_CODE
                     WHERE  FDS.MASTER_CHILD_FLAG = 'C' AND FDS.DISPLAY_FLAG='Y' AND FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' order by FDS.SERIAL_NO";
            List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
            return entity;
        }
        public List<BatchDetails> GetBatchNoByItemCode(string itemCode)
        {
            try
            {
                var itemdata = new List<BatchDetails>();
                if (itemCode != "")
                {
                    //string query = $@"SELECT TRANSACTION_NO,item_code, REFERENCE_NO,BATCH_NO,QUANTITY,UNIT_PRICE from batch_transaction where
                    //                COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND
                    //                item_code IN (SELECT ITEM_CODE FROM IP_PRODUCT_ITEM_MASTER_SETUP where product_type = '{itemCode}')";
                    string query = $@"SELECT distinct IPM.SUPPLIER_CODE,ISS.SUPPLIER_EDESC, BT.TRANSACTION_NO,BT.item_code, BT.REFERENCE_NO,BT.BATCH_NO,NVL(BT.QUANTITY,0) AS QUANTITY,BT.UNIT_PRICE 
FROM  ip_purchase_mrr  IPM 
LEFT JOIN batch_transaction BT ON BT.REFERENCE_NO = IPM.MRR_NO
INNER JOIN IP_SUPPLIER_SETUP ISS ON ISS.SUPPLIER_CODE = IPM.SUPPLIER_CODE
where IPM.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
AND IPM.mrr_no NOT in ( select reference_no from reference_detail)
AND
BT.item_code IN (SELECT ITEM_CODE FROM IP_PRODUCT_ITEM_MASTER_SETUP where item_code in ('{itemCode}'))";
                    itemdata = this._dbContext.SqlQuery<BatchDetails>(query).ToList();
                    return itemdata;
                }
                else
                { return itemdata; }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ParameterDetails> GetParameterDetailsByPlant(string Plant)
        {
            try
            {
                var itemdata = new List<ParameterDetails>();
                if (Plant != "")
                {
                    string query = $@"SELECT 
                                            LISTAGG(IIMS.item_code, ',') WITHIN GROUP (ORDER BY IIMS.item_code) AS ITEM_CODE,
                                            LISTAGG(IIQCPD.serial_no, ',') WITHIN GROUP (ORDER BY IIQCPD.serial_no) AS SERIAL_NO,
                                            IIQCPD.PARAM_CODE,
                                            IIQCPD.PARAMETERS,IIQCPD.SPECIFICATION,IIMS.INDEX_MU_CODE AS UNIT,
                                            IIQCPD.TARGET,
                                            IIQCPD.TOLERENCE
                                        FROM IP_ITEM_QC_PARAMETER_DETAILS IIQCPD
                                        INNER JOIN IP_PRODUCT_ITEM_MASTER_SETUP IIMS 
                                               ON IIMS.item_code = IIQCPD.item_code
                                        INNER JOIN IP_ITEM_QC_PARAMETER_MASTER IIQPM 
                                               ON IIQPM.PARAM_CODE = IIQCPD.PARAM_CODE
                                        WHERE IIMS.product_type =  '{Plant}'
                                          AND IIQPM.deleted_flag = 'N' AND IIMS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                        GROUP BY IIQCPD.PARAMETERS,IIQCPD.SPECIFICATION,IIMS.INDEX_MU_CODE, IIQCPD.TARGET, IIQCPD.TOLERENCE";
                    itemdata = this._dbContext.SqlQuery<ParameterDetails>(query).ToList();
                    return itemdata;
                }
                else
                { return itemdata; }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertOnSiteInspectionData(OnSiteInspection data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    //string query_id = $@"SELECT TO_CHAR(MIN(t.num) ) AS NEXT_PARAM_CODE FROM (SELECT LEVEL AS num FROM dual CONNECT BY LEVEL <= ( SELECT NVL(MAX(TO_NUMBER(PARAM_CODE)), 0) + 1 FROM IP_ITEM_QC_PARAMETER_MASTER WHERE REGEXP_LIKE(PARAM_CODE, '^\d+$'))) t WHERE NOT EXISTS (SELECT 1 FROM IP_ITEM_QC_PARAMETER_MASTER m WHERE REGEXP_LIKE(m.PARAM_CODE, '^\d+$') AND TO_NUMBER(m.PARAM_CODE) = t.num )";
                    //int id = Convert.ToInt32(this._dbContext.SqlQuery<string>(query_id).FirstOrDefault());

                    //string query = $@"select INSPECTION_NO from ONSITEINSPECTION WHERE DELETED_FLAG ='N' AND INSPECTION_NO ='{data.Inspection_No}'";
                    string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.Inspection_No}'";
                    string inspection_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                    string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'OI'";
                    string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();


                    if (inspection_no == null)
                    {
                        string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND form_code ='{form_code}'";
                        string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();

                        string insertMasterQuery = string.Format(@"
                            INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO,ITEM_CODE,BATCH_NO,SHIFT,REFERENCE_NO
                                , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,QC_CODE,FORM_CODE)
                            VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}',TO_DATE('{6}', 'DD-MON-YYYY'),'{7}','{8}','{9}','{10}','{11}','{12}')",
                                   data.Inspection_No, data.Plant_Id, data.Batch_No, data.Shift, data.Reference_No, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                           , 'N', _workContext.CurrentUserinformation.company_code,
                           _workContext.CurrentUserinformation.branch_code, serial_no_qc_setup, serial_no_qc_setup, form_code);
                        _dbContext.ExecuteSqlCommand(insertMasterQuery);

                        int i = 1;

                        foreach (var para in data.ParameterDetailsList)
                        {
                                string insertQuery = string.Format(@"
                        INSERT INTO ONSITEINSPECTIONDETAILS (SERIAL_NO,PARAM_CODE,ITEM_CODE,INSPECTION_NO,TARGET,TOLERENCE,RESULTS,VARIANCE
                            , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                        VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}', TO_DATE('{9}', 'DD-MON-YYYY'),'{10}','{11}','{12}')",
                                            para.SERIAL_NO,para.PARAM_CODE,para.ITEM_CODE, data.Inspection_No, para.TARGET, para.TOLERENCE, para.RESULTS, para.VARIANCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                    , 'N', _workContext.CurrentUserinformation.company_code,
                                    _workContext.CurrentUserinformation.branch_code);
                                _dbContext.ExecuteSqlCommand(insertQuery);
                                i++;
                        }
                        foreach (var raw in data.ITEMSETUPS)
                        {
                            string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_PARAM_MAP (INSPECTION_NO,PRODUCT_ID)
                        VALUES('{0}', '{1}')",
                                           data.Inspection_No, raw);
                            _dbContext.ExecuteSqlCommand(insertParam);
                        }
                    }
                    else
                    {
                        string deleteParameterDetailsQuery = $@"Delete from ONSITEINSPECTIONDETAILS WHERE INSPECTION_NO = '{data.Inspection_No}' ";
                        _dbContext.ExecuteSqlCommand(deleteParameterDetailsQuery);
                        //string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND form_code ='{form_code}'";
                        //string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();
                        int i = 1;
                        foreach (var para in data.ParameterDetailsList)
                        {
                            string insertQuery = string.Format(@"
                        INSERT INTO ONSITEINSPECTIONDETAILS (SERIAL_NO,PARAM_CODE,ITEM_CODE,INSPECTION_NO,TARGET,TOLERENCE,RESULTS,VARIANCE
                            , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                        VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}', TO_DATE('{9}', 'DD-MON-YYYY'),'{10}','{11}','{12}')",
                                        para.SERIAL_NO, para.PARAM_CODE, para.ITEM_CODE, data.Inspection_No, para.TARGET, para.TOLERENCE, para.RESULTS, para.VARIANCE, _workContext.CurrentUserinformation.login_code, data.CREATED_DATE.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code);
                            _dbContext.ExecuteSqlCommand(insertQuery);
                            i++;
                        }

                        string deleteItemSetupsQuery = $@"Delete from PRODUCT_PARAM_MAP WHERE INSPECTION_NO = '{data.Inspection_No}' ";
                        _dbContext.ExecuteSqlCommand(deleteItemSetupsQuery);
                        foreach (var raw in data.ITEMSETUPS)
                        {
                            string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_PARAM_MAP (INSPECTION_NO,PRODUCT_ID)
                        VALUES('{0}', '{1}')",
                                           data.Inspection_No, raw);
                            _dbContext.ExecuteSqlCommand(insertParam);
                        }

                        string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                           SET          
                           ITEM_CODE = '{data.Plant_Id}', 
                           BATCH_NO = '{data.Batch_No}',
                           SHIFT   = '{data.Shift}',
                           REFERENCE_NO  = '{data.Reference_No}',
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                            WHERE TRANSACTION_NO = '{data.Inspection_No}' ";
                        _dbContext.ExecuteSqlCommand(updateQuery);
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Transaction failed: " + ex.Message, ex);
                }
            }
        }

        public OnSiteInspection GetEditOnSiteInspection(string transactionno)
        {
            OnSiteInspection raw = new OnSiteInspection();
            String query1 = $@"select TRANSACTION_NO as Inspection_No,ITEM_CODE,REFERENCE_NO,CREATED_DATE,SHIFT,BATCH_NO,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<OnSiteInspection>(query1).FirstOrDefault();

            String query2 = $@" select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}'";
            List<string> rawMaterials = new List<string>();
            rawMaterials = _dbContext.SqlQuery<string>(query2).ToList();
            String details = $@" select OSI.SERIAL_NO,OSI.PARAM_CODE,OSI.ITEM_CODE,IIQPD.PARAMETERS,OSI.TARGET,OSI.TOLERENCE,OSI.RESULTS,OSI.VARIANCE from ONSITEINSPECTIONDETAILS OSI
                                LEFT JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQPD ON IIQPD.SERIAL_NO = OSI.SERIAL_NO AND
                                IIQPD.PARAM_CODE = OSI.PARAM_CODE AND IIQPD.ITEM_CODE = OSI.ITEM_CODE
                             WHERE OSI.INSPECTION_NO= '{transactionno}'";
            List<ParameterDetails> detailsRaw = new List<ParameterDetails>();
            detailsRaw = _dbContext.SqlQuery<ParameterDetails>(details).ToList();
            String query3 = $@"select ITEM_CODE,ITEM_EDESC  from IP_PRODUCT_ITEM_MASTER_SETUP WHERE ITEM_CODE IN (select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}')";
            raw.PRODUCTS = _dbContext.SqlQuery<PRODUCTSLIST>(query3).ToList();
            raw.ITEMSETUPS = rawMaterials;
            raw.ParameterDetailsList = detailsRaw;
            return raw;
        }
        public OnSiteInspection GetOnSiteInspectionReport(string transactionno)
        {
            OnSiteInspection raw = new OnSiteInspection();
            String query1 = $@"select TRANSACTION_NO as Inspection_No,ITEM_CODE,REFERENCE_NO,CREATED_DATE,CASE WHEN SHIFT ='N' THEN 'NIGHT' ELSE 'DAY' END SHIFT,BATCH_NO,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<OnSiteInspection>(query1).FirstOrDefault();

            String query2 = $@" select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}'";
            List<string> rawMaterials = new List<string>();
            rawMaterials = _dbContext.SqlQuery<string>(query2).ToList();
            String details = $@" select OSI.SERIAL_NO,OSI.PARAM_CODE,OSI.ITEM_CODE,IIQPD.PARAMETERS,OSI.TARGET,OSI.TOLERENCE,OSI.RESULTS,OSI.VARIANCE from ONSITEINSPECTIONDETAILS OSI
                                LEFT JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQPD ON IIQPD.SERIAL_NO = OSI.SERIAL_NO AND
                                IIQPD.PARAM_CODE = OSI.PARAM_CODE AND IIQPD.ITEM_CODE = OSI.ITEM_CODE
                             WHERE OSI.INSPECTION_NO= '{transactionno}'";
            List<ParameterDetails> detailsRaw = new List<ParameterDetails>();
            detailsRaw = _dbContext.SqlQuery<ParameterDetails>(details).ToList();
            String query3 = $@"select ITEM_CODE,ITEM_EDESC  from IP_PRODUCT_ITEM_MASTER_SETUP WHERE ITEM_CODE IN (select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}')";
            raw.PRODUCTS = _dbContext.SqlQuery<PRODUCTSLIST>(query3).ToList();
            raw.ITEMSETUPS = rawMaterials;
            raw.ParameterDetailsList = detailsRaw;
            return raw;
        }
    }
}
