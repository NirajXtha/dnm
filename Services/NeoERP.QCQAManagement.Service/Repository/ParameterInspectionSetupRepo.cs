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
    public class ParameterInspectionSetupRepo : IParameterInspectionSetupRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public ParameterInspectionSetupRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<ParameterInspectionSetup> GetItemCheckListDetails()
        {
            try
            {
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                string query = $@"select IPD.INS_PARAM_DETAIL_NO,IP.PARAMETERS from INSPECTION_PARAM_DETAILS IPD INNER JOIN INSPECTION_PARAMETERS IP ON IPD.PARAMETER_ID =  IP.INSPECTION_PARAM_NO";
                tableList = this._dbContext.SqlQuery<ParameterInspectionSetup>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParameterInspectionSetup> GetItemCheckList()
        {
            try
            {
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                string query = $@"SELECT '' AS INS_PARAM_DETAIL_NO, '' AS PARAMETER_ID FROM DUAL";
                tableList = this._dbContext.SqlQuery<ParameterInspectionSetup>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParameterInspectionSetup> GetParameterInspectionList()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<ParameterInspectionSetup> tableList = new List<ParameterInspectionSetup>();
                string query = $@"select INSPECTION_PARAM_NO,PARAMETERS  from INSPECTION_PARAMETERS";
                tableList = this._dbContext.SqlQuery<ParameterInspectionSetup>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool InsertParameterInspectionSetupData(ParameterInspectionSetup data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string query_id = $@"SELECT NVL(MAX(TO_NUMBER(INS_PARAM_DETAIL_NO)), 0) + 1 AS INS_PARAM_DETAIL_NO FROM INSPECTION_PARAM_DETAILS";
                    int id = Convert.ToInt32(this._dbContext.SqlQuery<int>(query_id).FirstOrDefault());

                    string insertMasterQuery = string.Format(@"
                            INSERT INTO INSPECTION_PARAM_DETAILS (INS_PARAM_DETAIL_NO,PARAMETER_ID,
                                CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                            VALUES('{0}','{1}', '{2}',TO_DATE('{3}', 'DD-MON-YYYY'),'{4}','{5}','{6}')",
                                id,data.PARAMETER_ID, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                        , 'N', _workContext.CurrentUserinformation.company_code,
                        _workContext.CurrentUserinformation.branch_code);  // change hard code 498
                    _dbContext.ExecuteSqlCommand(insertMasterQuery);

                    int p = 1;
                    foreach (var raw in data.ParameterItemDetailsList)
                    {
                        string insertQuery = string.Format(@"
                        INSERT INTO PARAM_ITEM_MAP (PARAM_ITEM_NO,ITEM_NAME,COLUMN_HEADER,PARAM_NO, CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,SERIAL_NO)
                        VALUES('{0}', '{1}', '{2}','{3}','{4}',TO_DATE('{5}', 'DD-MON-YYYY'),'{6}','{7}','{8}','{9}')",
                                        id, raw.ITEMS, string.Join("_", raw.ITEMS.Split(new[] { ' ', '/', '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries)), data.PARAMETER_ID, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                        , 'N', _workContext.CurrentUserinformation.company_code,
                        _workContext.CurrentUserinformation.branch_code,p);  // change hard code 498
                            _dbContext.ExecuteSqlCommand(insertQuery);
                        p++;
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

        public ParameterInspectionSetup GetParameterInspectionById(string id)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                ParameterInspectionSetup pdetails = new ParameterInspectionSetup();
                List<ParameterInspectionSetup> FormDetailList = new List<ParameterInspectionSetup>();
                string query = $@"select INS_PARAM_DETAIL_NO,PARAMETER_ID from INSPECTION_PARAM_DETAILS WHERE INS_PARAM_DETAIL_NO= '{id}'";
                pdetails = this._dbContext.SqlQuery<ParameterInspectionSetup>(query).FirstOrDefault();
                //FormDetailList = this._dbContext.SqlQuery<ProductDetails>(query).ToList();

                String query_parameters = $@"select ITEM_NAME AS ITEMS FROM PARAM_ITEM_MAP WHERE PARAM_ITEM_NO = '{id}'";

                pdetails.ParameterItemDetailsList = this._dbContext.SqlQuery<ParameterItemDetails>(query_parameters).ToList();

                return pdetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
