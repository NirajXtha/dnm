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
    public class QCQANumberSetup : IQCQANumberRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public QCQANumberSetup(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<FORM_SETUP> GetQCNumberDetails()
        {
            try
            {
                //string query = $@"SELECT FORM_CODE,FORM_EDESC,FORM_NDESC,DR_ACC_CODE,CR_ACC_CODE,CUSTOM_PREFIX_TEXT,CUSTOM_SUFFIX_TEXT,BODY_LENGTH,MASTER_FORM_CODE,PRE_FORM_CODE FROM FORM_SETUP WHERE FORM_EDESC='Quality Control'";
                string query = $@"SELECT * FROM FORM_SETUP WHERE FORM_TYPE='QC' and DELETED_FLAG ='N'";
                List<FORM_SETUP> qc = this._dbContext.SqlQuery<FORM_SETUP>(query).ToList();
                return qc;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool InsertQCData(FORM_SETUP data)
        {
            try
            {
                string query = $@"SELECT FORM_CODE from FORM_SETUP WHERE FORM_TYPE='QC' and DELETED_FLAG ='N'";
                string check_formCode = _dbContext.SqlQuery<string>(query).FirstOrDefault();
                if (check_formCode == null)
                {
                    var idquery = $@"SELECT (MAX(NVL(TO_NUMBER(form_code), 0)) + 1) as form_code FROM form_setup";
                    int id = _dbContext.SqlQuery<int>(idquery).FirstOrDefault();

                    string insertQuery = string.Format(@"INSERT INTO FORM_SETUP (
    FORM_CODE, FORM_EDESC, FORM_NDESC, MASTER_FORM_CODE,pre_form_code, MODULE_CODE, GROUP_SKU_FLAG, 
    CUSTOM_PREFIX_TEXT, CUSTOM_SUFFIX_TEXT, PREFIX_LENGTH, SUFFIX_LENGTH, BODY_LENGTH, 
    START_NO, LAST_NO, START_DATE, LAST_DATE, COMPANY_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG,FORM_TYPE) 
                                 VALUES({0},'{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'

, TO_DATE('{14:dd-MMM-yyyy}', 'DD-MON-YYYY'),TO_DATE('{15:dd-MMM-yyyy}', 'DD-MON-YYYY')
,'{16}'
,'{17}',TO_DATE('{18}', 'DD-MON-YYYY'),'{19}','{20}')",
                                             id, "Quality Control", "Quality Control", "0", "0", "0", "I", data.CUSTOM_PREFIX_TEXT
                                             , data.CUSTOM_SUFFIX_TEXT, data.PREFIX_LENGTH
                                             , data.SUFFIX_LENGTH, data.BODY_LENGTH, data.START_NO, data.LAST_NO
                                             , data.START_DATE, data.LAST_DATE
                                             , _workContext.CurrentUserinformation.company_code
                                             , _workContext.CurrentUserinformation.login_code
                                             , DateTime.Now.ToString("dd-MMM-yyyy"), 'N', "QC");
                    _dbContext.ExecuteSqlCommand(insertQuery);
                }
                else
                {
                    if (data.FORM_CODE == null)
                        data.FORM_CODE = check_formCode;
                    string updateQuery = $@"UPDATE FORM_SETUP 
                       SET CUSTOM_PREFIX_TEXT = '{data.CUSTOM_PREFIX_TEXT}',PREFIX_LENGTH = '{data.PREFIX_LENGTH}',
CUSTOM_SUFFIX_TEXT = '{data.CUSTOM_SUFFIX_TEXT}',SUFFIX_LENGTH = '{data.SUFFIX_LENGTH}',START_NO='{data.START_NO}'
,LAST_NO='{data.LAST_NO}',BODY_LENGTH={data.BODY_LENGTH},START_DATE=TO_DATE('{data.START_DATE:dd-MMM-yyyy}','DD-MON-YYYY'),
LAST_DATE=TO_DATE('{data.LAST_DATE:dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}',
                           COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                       WHERE FORM_CODE = '{data.FORM_CODE}' ";
                    _dbContext.ExecuteSqlCommand(updateQuery);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<FORM_SETUP> GetQCQAById(string id)
        {
            try
            {
                string query = $@"SELECT * FROM FORM_SETUP WHERE FORM_CODE='{id}'";
                List<FORM_SETUP> tenders = this._dbContext.SqlQuery<FORM_SETUP>(query).ToList();
                return tenders;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteQCQAId(string id)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE FORM_SETUP SET STATUS ='Y' WHERE FORM_CODE='{id}'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
