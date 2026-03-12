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
    public class InternalInspectionRepo : IInternalInspectionRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public InternalInspectionRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<FormDetailSetup> GetInternalInspectionList()
        {
            string query_FormCode = $@"SELECT form_code FROM form_setup WHERE FORM_TYPE = 'II'";
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
        public List<INTERNALPRODUCTLIST> GetProductsByProductType(string productType)
        {
            String query2 = $@"select PRODUCT_TYPE,PRODUCT FROM PRODUCT_TYPE_PRO_MAP
                             WHERE PRODUCT_TYPE= '{productType}'";
            List<INTERNALPRODUCTLIST> internalProducts = new List<INTERNALPRODUCTLIST>();
            internalProducts = _dbContext.SqlQuery<INTERNALPRODUCTLIST>(query2).ToList();
            return internalProducts;
        }
        public List<ItemSetup> GetVendorDetailsList(string Product)
        {
            try
            {
                //string query_productType = $@"select PRODUCT_TYPE from ip_product_item_master_setup  where ITEM_CODE ='{ProductType}'";
                ////string query = $@"select ITEM_CODE,ITEM_EDESC from ip_product_item_master_setup  where Product_type ='{ProductType}'";
                //var product_type = this._dbContext.SqlQuery<String>(query_productType).FirstOrDefault();

                List<ItemSetup> productList = new List<ItemSetup>();
                //string query = $@"select ITEM_CODE,ITEM_EDESC from ip_product_item_master_setup  where PRODUCT_TYPE ='{ProductType}' order by item_code asc";
                string query = $@"select ITEM_CODE,ITEM_EDESC from ip_product_item_master_setup  where ITEM_CODE  IN (SELECT PIM.ITEM_ID FROM IP_PRODUCT_MASTER IPM
LEFT JOIN PRODUCT_ITEMS_MAP PIM ON PIM.PRODUCT_ID = IPM.PRODUCT_ID
WHERE IPM.PRODUCT_ID = '{Product}') order by item_code asc";
                // string query = $@"select ITEM_CODE,ITEM_EDESC from ip_product_item_master_setup  where Product_type ='{product_type}'";
                productList = this._dbContext.SqlQuery<ItemSetup>(query).ToList();
                return productList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParameterDetails> GetParameterDetailsByItemCode(string ProductId)
        {
            try
            {
                var itemdata = new List<ParameterDetails>();
                    string query = $@"SELECT IIQCPD.SERIAL_NO  AS PARAMETER_ID
        ,LISTAGG(IIMS.item_code, ',') WITHIN GROUP (ORDER BY IIMS.item_code) AS ITEM_CODE
        ,IIQCPD.SERIAL_NO, IIQCPD.PRODUCT_ID AS PARAM_CODE,
        IIQCPD.PARAMETERS,IIQCPD.SPECIFICATION,IIQCPD.UNIT
        --,IIMS.INDEX_MU_CODE AS UNIT,
        ,IIQCPD.TARGET,
        IIQCPD.TOLERENCE
,IIQCPD.TARGET AS RESULTS, '0' AS VARIANCE
    FROM IP_PRODUCT_PARAMETER_DETAILS IIQCPD
     INNER JOIN PRODUCT_ITEMS_MAP PIM 
           ON PIM.PRODUCT_ID = IIQCPD.PRODUCT_ID 
    INNER JOIN IP_PRODUCT_ITEM_MASTER_SETUP IIMS 
    -- INNER JOIN IP_ITEM_MASTER_SETUP IIMS 
           ON IIMS.item_code = PIM.ITEM_ID
    INNER JOIN IP_PRODUCT_MASTER IIQPM 
           ON IIQPM.PRODUCT_ID = IIQCPD.PRODUCT_ID
    WHERE IIQCPD.PRODUCT_ID ='{ProductId}'
      AND IIQPM.deleted_flag = 'N' AND IIMS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
    GROUP BY IIQCPD.SERIAL_NO, IIQCPD.PRODUCT_ID,IIQCPD.PARAMETERS,IIQCPD.SPECIFICATION,IIQCPD.UNIT
           --,IIMS.INDEX_MU_CODE
           , IIQCPD.TARGET
           , IIQCPD.TOLERENCE,IIQCPD.SERIAL_NO";
                    itemdata = this._dbContext.SqlQuery<ParameterDetails>(query).ToList();
                    return itemdata;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool InsertInternalInspectionData(OnSiteInspection data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.Inspection_No}'";
                    string inspection_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                    string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'II'";
                    string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();

                    if (inspection_no == null)
                    {
                        string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND form_code ='{form_code}'";
                        string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();

                        string insertMasterQuery = string.Format(@"
                            INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO,ITEM_CODE,BATCH_NO,SHIFT,REFERENCE_NO
                                , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,QC_CODE,FORM_CODE,DISPATCH_PERSON,REMARKS)
                            VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}',TO_DATE('{6}', 'DD-MON-YYYY'),'{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')",
                                   data.Inspection_No, data.Plant_Id, data.Batch_No, data.Shift, string.IsNullOrWhiteSpace(data.Reference_No) ? "0" : data.Reference_No, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                           , 'N', _workContext.CurrentUserinformation.company_code,
                           _workContext.CurrentUserinformation.branch_code, serial_no_qc_setup, serial_no_qc_setup, form_code,data.DISPATCH_PERSON,data.REMARKS);
                        int rowsInserted = _dbContext.ExecuteSqlCommand(insertMasterQuery);
                        if (rowsInserted > 0)
                        {
                            int i = 1;
                            foreach (var para in data.ParameterDetailsList)
                            {
                                string insertQuery = string.Format(@"
                                INSERT INTO INTERNALINSPECTION (SERIAL_NO,INSPECTION_NO,PRODUCT_ID,SPECIFICATION,UNIT,TARGET,TOLERENCE,RESULTS,VARIANCE
                                    , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                                VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}', TO_DATE('{10}', 'DD-MON-YYYY'),'{11}','{12}','{13}')",
                                            para.SERIAL_NO, data.Inspection_No, para.PARAM_CODE,para.SPECIFICATION, para.UNIT, para.TARGET, para.TOLERENCE, para.RESULTS, para.VARIANCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                    , 'N', _workContext.CurrentUserinformation.company_code,
                                    _workContext.CurrentUserinformation.branch_code);
                                _dbContext.ExecuteSqlCommand(insertQuery);
                                i++;
                            }
                            foreach (var raw in data.InternalItemSetupList)
                            {
                                string insertParam = string.Format(@"
                                    INSERT INTO INTERNAL_PRODUCT_MAP (INSPECTION_NO,ITEM_CODE,QUANTITY)
                                    VALUES('{0}', '{1}', '{2}')",
                                               data.Inspection_No, raw.ITEM_CODE,raw.QUANTITY);
                                _dbContext.ExecuteSqlCommand(insertParam);
                            }
                        }
                        else {
                            transaction.Rollback();
                            throw new Exception("Transaction failed");
                        }
                    }
                    else
                    {                       
                        string deleteParameterDetailsQuery = $@"Delete from INTERNALINSPECTION WHERE INSPECTION_NO = '{data.Inspection_No}' ";
                        _dbContext.ExecuteSqlCommand(deleteParameterDetailsQuery);
                        int i = 1;
                        foreach (var para in data.ParameterDetailsList)
                        {
                            string insertQuery = string.Format(@"
                                INSERT INTO INTERNALINSPECTION (SERIAL_NO,INSPECTION_NO,PRODUCT_ID,SPECIFICATION,UNIT,TARGET,TOLERENCE,RESULTS,VARIANCE
                                    , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                                VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}', TO_DATE('{10}', 'DD-MON-YYYY'),'{11}','{12}','{13}')",
                                        para.SERIAL_NO, data.Inspection_No, para.PARAM_CODE, para.SPECIFICATION, para.UNIT, para.TARGET, para.TOLERENCE, para.RESULTS, para.VARIANCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code);
                            _dbContext.ExecuteSqlCommand(insertQuery);
                            i++;
                        }
                        string deleteInternalItemSetupsQuery = $@"Delete from INTERNAL_PRODUCT_MAP WHERE INSPECTION_NO = '{data.Inspection_No}' ";
                        _dbContext.ExecuteSqlCommand(deleteInternalItemSetupsQuery);
                        foreach (var raw in data.InternalItemSetupList)
                        {
                            string insertParam = string.Format(@"
                                    INSERT INTO INTERNAL_PRODUCT_MAP (INSPECTION_NO,ITEM_CODE,QUANTITY)
                                    VALUES('{0}', '{1}', '{2}')",
                                           data.Inspection_No, raw.ITEM_CODE, raw.QUANTITY);
                            _dbContext.ExecuteSqlCommand(insertParam);
                        }

                        string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                           SET          
                           ITEM_CODE = '{data.Plant_Id}', 
                           BATCH_NO = '{data.Batch_No}',
                           SHIFT   = '{data.Shift}',
                           REMARKS   = '{data.REMARKS}', 
                           DISPATCH_PERSON   = '{data.DISPATCH_PERSON}', 
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

        public OnSiteInspection GetEditInternalInspection(string transactionno)
        {
            OnSiteInspection raw = new OnSiteInspection();
            String query1 = $@"select TRANSACTION_NO as Inspection_No,ITEM_CODE,TO_CHAR(SERIAL_NO) AS SERIAL_NO,REFERENCE_NO,CREATED_DATE,SHIFT,BATCH_NO,DISPATCH_PERSON,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<OnSiteInspection>(query1).FirstOrDefault();

            String query2 = $@"select  IPM.INSPECTION_NO,IPM.ITEM_CODE,IPIMS.ITEM_EDESC,IPM.QUANTITY from INTERNAL_PRODUCT_MAP IPM
INNER JOIN ip_product_item_master_setup IPIMS ON IPIMS.ITEM_CODE = IPM.ITEM_CODE
WHERE IPM.INSPECTION_NO= '{transactionno}'";
            List<InternalItemSetup> rawMaterials = new List<InternalItemSetup>();
            rawMaterials = _dbContext.SqlQuery<InternalItemSetup>(query2).ToList();
            String details = $@"select OSI.SERIAL_NO,OSI.PRODUCT_ID AS PARAM_CODE,IIQPD.SERIAL_NO  AS PARAMETER_ID
 ,IIQPD.PARAMETERS,OSI.TARGET,OSI.TOLERENCE
 ,OSI.RESULTS,OSI.VARIANCE,OSI.UNIT,OSI.SPECIFICATION from INTERNALINSPECTION OSI
                                LEFT JOIN IP_PRODUCT_PARAMETER_DETAILS IIQPD ON IIQPD.SERIAL_NO = OSI.SERIAL_NO AND
                                IIQPD.PRODUCT_ID = OSI.PRODUCT_ID 
                             WHERE OSI.INSPECTION_NO = '{transactionno}'";
            List<ParameterDetails> detailsRaw = new List<ParameterDetails>();
            detailsRaw = _dbContext.SqlQuery<ParameterDetails>(details).ToList();
            raw.InternalItemSetupList = rawMaterials;
            raw.ParameterDetailsList = detailsRaw;
            return raw;
        }

        public OnSiteInspection GetInternalInspectionReport(string transactionno)
        {
            OnSiteInspection raw = new OnSiteInspection();
            String query1 = $@"select TRANSACTION_NO as Inspection_No,ITEM_CODE,TO_CHAR(SERIAL_NO) AS SERIAL_NO,REFERENCE_NO,CREATED_DATE,CASE WHEN SHIFT ='N' THEN 'NIGHT' ELSE 'DAY' END SHIFT,BATCH_NO,DISPATCH_PERSON,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<OnSiteInspection>(query1).FirstOrDefault();

            String query2 = $@" select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}'";
            List<string> rawMaterials = new List<string>();
            rawMaterials = _dbContext.SqlQuery<string>(query2).ToList();
            String details = $@" select OSI.SERIAL_NO
 ,IIQPD.SERIAL_NO  AS PARAMETER_ID
 ,IIQPD.PARAMETERS,OSI.TARGET,OSI.TOLERENCE
 ,OSI.RESULTS,OSI.VARIANCE,OSI.UNIT,OSI.SPECIFICATION from INTERNALINSPECTION OSI
                                INNER JOIN IP_PRODUCT_PARAMETER_DETAILS IIQPD ON IIQPD.SERIAL_NO = OSI.SERIAL_NO
                                AND
                                IIQPD.PRODUCT_ID = OSI.PRODUCT_ID
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