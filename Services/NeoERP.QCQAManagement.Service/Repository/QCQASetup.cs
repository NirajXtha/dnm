using NeoErp.Core;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.EntityClient;
using System.Data.OracleClient;
using NeoErp.Core.Models;
using NeoErp.Core.Domain;

namespace NeoERP.QCQAManagement.Service.Repository
{
   public class QCQASetup : IQCQARepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public QCQASetup(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<FormDetailSetup> GetQCQADetails(string TableName)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<FormDetailSetup> FormDetailList = new List<FormDetailSetup>();

                string query = $@"select DISTINCT COLUMN_NAME,COLUMN_HEADER,COLUMN_WIDTH,TOP_POSITION,LEFT_POSITION,MASTER_CHILD_FLAG
,IS_DESC_FLAG,DEFA_VALUE,FILTER_VALUE,DISPLAY_FLAG
from form_detail_setup
                    where company_code='{_workContext.CurrentUserinformation.company_code}' and TABLE_NAME ='{TableName}' and  IS_DESC_FLAG='Y' order by COLUMN_NAME";
                FormDetailList = this._dbContext.SqlQuery<FormDetailSetup>(query).ToList();
                return FormDetailList;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<FormDetailSetup> GetQCQADetailsByTableName(string tableName)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<FormDetailSetup> FormDetailList = new List<FormDetailSetup>();
                string sConnStr1 = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ConnectionString;
                var entityBuilder = new EntityConnectionStringBuilder(sConnStr1);
                string providerConnString = entityBuilder.ProviderConnectionString;
                // Extract database details from Oracle connection string
                var oracleBuilder = new OracleConnectionStringBuilder(providerConnString);
                string databaseName = oracleBuilder.UserID;
                string query = $@"SELECT COLUMN_NAME 
FROM ALL_TAB_COLUMNS 
WHERE TABLE_NAME = 'IP_PRODUCTION_ISSUE' 
AND OWNER = '{databaseName}' and COLUMN_NAME not in (select column_NAME FROM form_detail_setup
WHERE TABLE_NAME = '{tableName}' 
)";
                FormDetailList = this._dbContext.SqlQuery<FormDetailSetup>(query).ToList();
                return FormDetailList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<TableList> GetTableLists()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<TableList> tableList = new List<TableList>();
                string query = $@"select table_name,table_desc,column_name,post_required_flag
,remarks,syn_rowid,modify_by
from TRANSACTION_TABLE_LIST";
                tableList = this._dbContext.SqlQuery<TableList>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string AddColumnsToTable(List<FormDetailSetup> modal, string tableName)
        {
            var message = "";
            for (int i = 0; i < modal.Count; i++)
            {
                var count = $@"SELECT COLUMN_NAME from form_detail_setup where table_name = '{tableName}' AND COLUMN_NAME ='{modal[i].COLUMN_NAME}'";
                var Ccountdata = _objectEntity.SqlQuery<string>(count).ToList();
                if (Ccountdata.Count < 1)
                {
                    //var defaultValueForLog = new DefaultValueForLog(this._workContext);
                    string serialNo = @"SELECT NVL(MAX(TO_NUMBER(serial_no))+1, 1) SERIAL_NO FROM form_detail_setup";
                    var data = _objectEntity.SqlQuery<FormDetailSetup>(serialNo).ToList();
                    var final_serialNo = data[0].SERIAL_NO;
                    try
                    {
                        var insertQuery = $@"INSERT INTO form_detail_setup(serial_no,TABLE_NAME,COLUMN_NAME,COLUMN_HEADER,COLUMN_WIDTH,TOP_POSITION,LEFT_POSITION,MASTER_CHILD_FLAG,IS_DESC_FLAG,DEFA_VALUE,FILTER_VALUE,DISPLAY_FLAG,FORM_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,HELP_DESCRIPTION) 
                                          VALUES('{final_serialNo}','{tableName}','{modal[i].COLUMN_NAME}','{modal[i].COLUMN_HEADER}','{modal[i].COLUMN_WIDTH}','{modal[i].TOP_POSITION}','{modal[i].LEFT_POSITION}','{modal[i].MASTER_CHILD_FLAG}','{modal[i].IS_DESC_FLAG}','{modal[i].DEFA_VALUE}','{modal[i].FILTER_VALUE}','{modal[i].DISPLAY_FLAG}','{modal[i].FORM_CODE}','{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.login_code}','{DateTime.Now.ToString("dd-MMM-yyyy")}','N','HELP_DESCRIPTION')";
                        var altertable = $@"ALTER TABLE {tableName} ADD {modal[i].COLUMN_NAME} VARCHAR2(255)";

                        _objectEntity.ExecuteSqlCommand(insertQuery);
                        _objectEntity.ExecuteSqlCommand(altertable);
                        _objectEntity.SaveChanges();
                        message = "success";
                    }
                    catch (Exception e)
                    {
                        message = "failed";
                    }
                }
                else
                {
                    var SERIAL_NO = $@"SELECT SERIAL_NO from form_detail_setup where table_name = '{tableName}' AND COLUMN_NAME ='{modal[i].COLUMN_NAME}'";
                    var f_SERIAL_NO = _objectEntity.SqlQuery<int>(SERIAL_NO).FirstOrDefault();
                    try
                    {
                        string updateQuery = string.Format($@"UPDATE form_detail_setup SET COLUMN_HEADER='{modal[i].COLUMN_HEADER}',TOP_POSITION='{modal[i].TOP_POSITION}', LEFT_POSITION  = '{modal[i].LEFT_POSITION}',DISPLAY_FLAG= '{modal[i].DISPLAY_FLAG}',
                    DEFA_VALUE='{modal[i].DEFA_VALUE}',IS_DESC_FLAG = '{modal[i].IS_DESC_FLAG}',MASTER_CHILD_FLAG='{modal[i].MASTER_CHILD_FLAG}',FORM_CODE='{modal[i].FORM_CODE}'
,FILTER_VALUE='{modal[i].FILTER_VALUE}',
                    MODIFY_DATE='{DateTime.Now.ToString("dd-MMM-yyyy")}',MODIFY_BY='{this._workContext.CurrentUserinformation.login_code}'
                    WHERE TABLE_NAME ='{tableName}' and SERIAL_NO ='{f_SERIAL_NO}'");
                        _objectEntity.ExecuteSqlCommand(updateQuery);
                        _objectEntity.SaveChanges();
                        message = "success";
                    }
                    catch (Exception e)
                    {
                        message = "failed";
                    }
                }
            }
            return message;
        }
        public List<FormSetupModel> GetFormCode(User userIndo, string tableName)
        {
            string query = $@"SELECT 
                            DISTINCT FS.FORM_CODE, 
                            INITCAP(FS.FORM_EDESC) FORM_EDESC
                            FROM FORM_DETAIL_SETUP DS, FORM_SETUP FS
                            WHERE table_name  IN ( '{tableName}')                           
                            AND FS.DELETED_FLAG = 'N'
                            AND FS.FORM_CODE = DS.FORM_CODE
                            AND FS.COMPANY_CODE = DS.COMPANY_CODE
                            AND FS.COMPANY_CODE='{userIndo.Company}'
                            ORDER BY INITCAP(FS.FORM_EDESC)";
            var voucherList = _objectEntity.SqlQuery<FormSetupModel>(query).ToList();
            return voucherList;
        }
        public List<FormDetailSetup> GetQCFormDetailSetup(string formCode)
        {
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
                           FS.REF_FIX_PRICE                          
                      FROM    FORM_DETAIL_SETUP FDS
                           LEFT JOIN
                              COMPANY_SETUP CS ON FDS.COMPANY_CODE = CS.COMPANY_CODE
                              LEFT JOIN FORM_SETUP FS
                               ON FDS.FORM_CODE = FS.FORM_CODE AND FDS.COMPANY_CODE = FS.COMPANY_CODE
                     WHERE FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            //_logErp.InfoInFile(Query + " is a query for fetching form details setup for " + formCode + " formcode");
            List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
            return entity;
        }
    }
}
