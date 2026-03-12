using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Repository
{
    public class DailyWastageRepo: IDailyWastageRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public DailyWastageRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }

        public List<FormDetailSetup> GetDailyWastageList()
        {
            string query_FormCode = $@"SELECT form_code FROM form_setup WHERE FORM_TYPE = 'DW'";
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
        public List<Items> GetMaterialGroupLists()
        {
            try
            {
                List<Items> tableList = new List<Items>();
                string query = $@"SELECT master_item_code, INITCAP(item_edesc) as item_edesc, pre_item_code,item_code FROM IP_PRODUCT_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' 
                                AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND group_sku_flag ='G'
                                ORDER BY LENGTH(PRE_ITEM_CODE),PRE_ITEM_CODE, MASTER_ITEM_CODE";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool InsertDailyWastage(DailyWastage data)
        {
            try
            {
                string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.DAILYWASTAGE_NO}'";
                string dailywastage_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'DW'";
                string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();

                if (dailywastage_no == null)
                {
                    string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND form_code ='{form_code}'";
                    string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();
                    //string insertRawMaterialTranQuery = string.Format(@"INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO
                    //            ,REFERENCE_NO,ITEM_CODE,SERIAL_NO,QC_CODE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY
                    //            ,CREATED_DATE,DELETED_FLAG,CHECKED_BY,AUTHORISED_BY,BATCH_NO)
                    //            VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9:dd-MMM-yyyy}', 'DD-MON-YYYY'),'{10}','{11}','{12}','{13}')"
                    //               , data.DAILYWASTAGE_NO, string.IsNullOrWhiteSpace(data.MANUAL_NO) ? "0" : data.ITEM_CODE, data.ITEM_CODE, serial_no_qc_setup, serial_no_qc_setup, form_code, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code,
                    //               _workContext.CurrentUserinformation.login_code
                    //               , DateTime.Now.ToString("dd-MMM-yyyy"), 'N', _workContext.CurrentUserinformation.login_code, _workContext.CurrentUserinformation.login_code, data.BATCH_NO);

                    string insertRawMaterialTranQuery = string.Format(@"INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO
                                ,REFERENCE_NO,ITEM_CODE,SERIAL_NO,QC_CODE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY
                                ,CREATED_DATE,DELETED_FLAG,BATCH_NO)
                                VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9:dd-MMM-yyyy}', 'DD-MON-YYYY'),'{10}','{11}')"
                                   , data.DAILYWASTAGE_NO, string.IsNullOrWhiteSpace(data.MANUAL_NO) ? "0" : data.ITEM_CODE, data.ITEM_CODE, serial_no_qc_setup, serial_no_qc_setup, form_code, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code,
                                   _workContext.CurrentUserinformation.login_code
                                   , DateTime.Now.ToString("dd-MMM-yyyy"), 'N', data.BATCH_NO);
                    _dbContext.ExecuteSqlCommand(insertRawMaterialTranQuery);

                    int i = 1;
                    foreach (var para in data.DailyWastageList)
                    {
                        string insertQuery = string.Format(@"INSERT INTO IP_ITEM_DAILY_WASTAGE (SERIAL_NO,PARAM_CODE,ITEM_CODE,IP_ITEM_DAILY_WASTAGE_NO
                            , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,UNIT,QTY,FORM_CODE)
                        VALUES('{0}', '{1}', '{2}','{3}','{4}', TO_DATE('{5}', 'DD-MON-YYYY'),'{6}','{7}','{8}','{9}','{10}','{11}')",
                                    para.SERIAL_NO, para.PARAM_CODE, para.ITEM_CODE, data.DAILYWASTAGE_NO, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                            , 'N', _workContext.CurrentUserinformation.company_code,
                            _workContext.CurrentUserinformation.branch_code,para.UNIT,para.QTY, form_code);
                        _dbContext.ExecuteSqlCommand(insertQuery);
                        i++;

                    }
                    foreach (var raw in data.ITEMSETUPS)
                    {
                        string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_PARAM_MAP (INSPECTION_NO,PRODUCT_ID)
                        VALUES('{0}', '{1}')",
                                       data.DAILYWASTAGE_NO, raw);
                        _dbContext.ExecuteSqlCommand(insertParam);
                    }
                }
                else
                {
                    string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                       SET 
                           ITEM_CODE = '0',REFERENCE_NO ='{data.MANUAL_NO}', 
                           BATCH_NO = '{data.BATCH_NO}', 
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE TRANSACTION_NO = '{dailywastage_no}' ";
                    _dbContext.ExecuteSqlCommand(updateQuery);

                    string dailyWastageQuery = $@"Delete from IP_ITEM_DAILY_WASTAGE WHERE IP_ITEM_DAILY_WASTAGE_NO = '{dailywastage_no}' ";
                    _dbContext.ExecuteSqlCommand(dailyWastageQuery);
                    int k = 1;
                    foreach (var para in data.DailyWastageList)
                    {
                        string insertQuery = string.Format(@"INSERT INTO IP_ITEM_DAILY_WASTAGE (SERIAL_NO,PARAM_CODE,ITEM_CODE,IP_ITEM_DAILY_WASTAGE_NO
                            , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,UNIT,QTY,FORM_CODE)
                        VALUES('{0}', '{1}', '{2}','{3}','{4}', TO_DATE('{5}', 'DD-MON-YYYY'),'{6}','{7}','{8}','{9}','{10}','{11}')",
                                    para.SERIAL_NO, para.PARAM_CODE, para.ITEM_CODE, data.DAILYWASTAGE_NO, _workContext.CurrentUserinformation.login_code, data.CREATED_DATE.ToString("dd-MMM-yyyy")
                            , 'N', _workContext.CurrentUserinformation.company_code,
                            _workContext.CurrentUserinformation.branch_code, para.UNIT, para.QTY, form_code);
                        _dbContext.ExecuteSqlCommand(insertQuery);
                        k++;
                    }
                    string deleteItemSetupsQuery = $@"Delete from PRODUCT_PARAM_MAP WHERE INSPECTION_NO = '{dailywastage_no}' ";
                    _dbContext.ExecuteSqlCommand(deleteItemSetupsQuery);
                    foreach (var raw in data.ITEMSETUPS)
                    {
                        string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_PARAM_MAP (INSPECTION_NO,PRODUCT_ID)
                        VALUES('{0}', '{1}')",
                                       dailywastage_no, raw);
                        _dbContext.ExecuteSqlCommand(insertParam);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DailyWastage GetEditDailyWastage(string transactionno)
        {
            DailyWastage raw = new DailyWastage();
            //String query1 = $@"select TRANSACTION_NO as DAILYWASTAGE_NO,ITEM_CODE,SERIAL_NO,REFERENCE_NO AS MANUAL_NO,CREATED_DATE,REMARKS from QC_PARAMETER_TRANSACTION
            //            where TRANSACTION_NO='{transactionno}'";
            String query1 = $@"select TRANSACTION_NO as DAILYWASTAGE_NO,ITEM_CODE,BATCH_NO,CREATED_DATE,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<DailyWastage>(query1).FirstOrDefault();
            String query2 = $@" select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}'";
            List<string> rawMaterials = new List<string>();
            rawMaterials = _dbContext.SqlQuery<string>(query2).ToList();
            String dailywastage = $@" select IDW.SERIAL_NO,IDW.PARAM_CODE,IDW.ITEM_CODE,IIQPD.PARAMETERS,IDW.UNIT,CAST(IDW.QTY AS DECIMAL(18,2)) AS QTY FROM IP_ITEM_DAILY_WASTAGE IDW LEFT JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQPD ON IIQPD.SERIAL_NO = IDW.SERIAL_NO AND IIQPD.PARAM_CODE = IDW.PARAM_CODE AND IIQPD.ITEM_CODE = IDW.ITEM_CODE WHERE IDW.IP_ITEM_DAILY_WASTAGE_NO= '{transactionno}'";

            raw.DailyWastageList = this._dbContext.SqlQuery<DailyWastage>(dailywastage).ToList();
            raw.ITEMSETUPS = this._dbContext.SqlQuery<string>(query2).ToList();

            String query3 = $@"select ITEM_CODE,ITEM_EDESC  from IP_PRODUCT_ITEM_MASTER_SETUP WHERE ITEM_CODE IN (select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}')";
            raw.PRODUCTS = _dbContext.SqlQuery<PRODUCTSLIST>(query3).ToList();
            //raw.DailyWastageItemLists = rawMaterials;
            return raw;
        }
        public DailyWastage GetDailyWastageReport(string transactionno)
        {
            DailyWastage raw = new DailyWastage();
            String query1 = $@"select TRANSACTION_NO as DAILYWASTAGE_NO,ITEM_CODE,BATCH_NO,CREATED_DATE,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<DailyWastage>(query1).FirstOrDefault();
            String query2 = $@" select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}'";
            List<string> rawMaterials = new List<string>();
            rawMaterials = _dbContext.SqlQuery<string>(query2).ToList();
            String dailywastage = $@" select IDW.SERIAL_NO,IDW.PARAM_CODE,IDW.ITEM_CODE,IIQPD.PARAMETERS,IDW.UNIT,CAST(IDW.QTY AS DECIMAL(18,2)) AS QTY FROM IP_ITEM_DAILY_WASTAGE IDW LEFT JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQPD ON IIQPD.SERIAL_NO = IDW.SERIAL_NO AND IIQPD.PARAM_CODE = IDW.PARAM_CODE AND IIQPD.ITEM_CODE = IDW.ITEM_CODE WHERE IDW.IP_ITEM_DAILY_WASTAGE_NO= '{transactionno}'";

            raw.DailyWastageList = this._dbContext.SqlQuery<DailyWastage>(dailywastage).ToList();
            raw.ITEMSETUPS = this._dbContext.SqlQuery<string>(query2).ToList();

            String query3 = $@"select ITEM_CODE,ITEM_EDESC  from IP_PRODUCT_ITEM_MASTER_SETUP WHERE ITEM_CODE IN (select PRODUCT_ID FROM PRODUCT_PARAM_MAP
                             WHERE INSPECTION_NO= '{transactionno}')";
            raw.PRODUCTS = _dbContext.SqlQuery<PRODUCTSLIST>(query3).ToList();
            //raw.DailyWastageItemLists = rawMaterials;
            return raw;
        }
    }
}
