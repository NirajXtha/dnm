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
    public class InternalInspectionSetupRepo: IInternalInspectionSetupRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public InternalInspectionSetupRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }

        public List<ProductDetails> GetProductDetails()
        {
            try
            {
                string query = $@"select PRODUCT_ID AS PARAM_CODE, PRODUCT AS PRODUCT_NAME from IP_PRODUCT_MASTER WHERE DELETED_FLAG = 'N'";
                List<ProductDetails> qc = this._dbContext.SqlQuery<ProductDetails>(query).ToList();
                return qc;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<ItemSetup> GetItemLists()
        {
            try
            {
                List<ItemSetup> itemList = new List<ItemSetup>();
                string query = $@"select ITEM_CODE,ITEM_EDESC from ip_product_item_master_setup where category_code in ('FG','IP') order by item_code asc";
                itemList = this._dbContext.SqlQuery<ItemSetup>(query).ToList();
                return itemList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool InsertInternalInspectionDetails(ProductDetails data)
        {
            try
            {
                string query_id = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(PRODUCT_ID)), 0) + 1) FROM IP_PRODUCT_MASTER WHERE REGEXP_LIKE(PRODUCT_ID, '^\d+$')";
                int id = Convert.ToInt32(this._dbContext.SqlQuery<string>(query_id).FirstOrDefault());

                if (data.PARAM_CODE == "")
                {
                    string insertMasterQuery = string.Format(@"
                            INSERT INTO IP_PRODUCT_MASTER (PRODUCT_ID
                                , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,PRODUCT)
                            VALUES('{0}', '{1}',TO_DATE('{2}', 'DD-MON-YYYY'),'{3}','{4}','{5}','{6}')",
                                id, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                        , 'N', _workContext.CurrentUserinformation.company_code,
                        _workContext.CurrentUserinformation.branch_code, data.PRODUCT_NAME);  // change hard code 498
                    _dbContext.ExecuteSqlCommand(insertMasterQuery);


                    foreach (var raw in data.ITEMSETUPS)
                    {
                        string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_ITEMS_MAP (PRODUCT_ID,ITEM_ID)
                        VALUES('{0}', '{1}')",
                                       id, raw);
                        _dbContext.ExecuteSqlCommand(insertParam);
                    }
                    int i = 1;
                    foreach (var para in data.ParameterDetailsList)
                    {
                            string insertQuery = string.Format(@"
                            INSERT INTO IP_PRODUCT_PARAMETER_DETAILS (SERIAL_NO,PRODUCT_ID,PARAMETERS,SPECIFICATION,UNIT,TARGET,TOLERENCE
                                , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                            VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}', TO_DATE('{8}', 'DD-MON-YYYY'),'{9}','{10}','{11}')",
                                        i, id, para.PARAMETERS, para.SPECIFICATION,para.UNIT, para.TARGET, para.TOLERENCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code);  // change hard code 498
                            _dbContext.ExecuteSqlCommand(insertQuery);
                            i++;
                    }

                }
                else
                {
                    string deleteInternalInspectionMapQuery = $@"Delete from PRODUCT_ITEMS_MAP WHERE PRODUCT_ID = '{data.PARAM_CODE}' ";
                    _dbContext.ExecuteSqlCommand(deleteInternalInspectionMapQuery);
                    foreach (var raw in data.ITEMSETUPS)
                    {
                        string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_ITEMS_MAP (PRODUCT_ID,ITEM_ID)
                        VALUES('{0}', '{1}')",
                                       data.PARAM_CODE, raw);
                        _dbContext.ExecuteSqlCommand(insertParam);
                    }
                    string deleteInternalInspectionDetailsQuery = $@"Delete from IP_PRODUCT_PARAMETER_DETAILS WHERE PRODUCT_ID = '{data.PARAM_CODE}' ";
                    _dbContext.ExecuteSqlCommand(deleteInternalInspectionDetailsQuery);
                    int i = 1;
                    foreach (var para in data.ParameterDetailsList)
                    {
                        string insertQuery = string.Format(@"
                            INSERT INTO IP_PRODUCT_PARAMETER_DETAILS (SERIAL_NO,PRODUCT_ID,PARAMETERS,SPECIFICATION,UNIT,TARGET,TOLERENCE
                                , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                            VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}', TO_DATE('{8}', 'DD-MON-YYYY'),'{9}','{10}','{11}')",
                                    i, data.PARAM_CODE, para.PARAMETERS, para.SPECIFICATION,para.UNIT, para.TARGET, para.TOLERENCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                            , 'N', _workContext.CurrentUserinformation.company_code,
                            _workContext.CurrentUserinformation.branch_code);  // change hard code 498
                        _dbContext.ExecuteSqlCommand(insertQuery);
                        i++;
                    }
                    string updateQuery = $@"UPDATE IP_PRODUCT_MASTER 
                           SET           
                           PRODUCT = '{data.PRODUCT_NAME}', 
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                            WHERE PRODUCT_ID = '{data.PARAM_CODE}' ";
                    _dbContext.ExecuteSqlCommand(updateQuery);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ProductDetails GetInternalInspectionSetupById(string id)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                ProductDetails pdetails = new ProductDetails();
                List<ProductDetails> FormDetailList = new List<ProductDetails>();
                string query = $@"SELECT PRODUCT_ID AS PARAM_CODE,PRODUCT AS PRODUCT_NAME,ITEM_CODE, ITEM_CODE_LIST
                                FROM (
                                    SELECT
                                    IIQPM.PRODUCT_ID,
                                    IIQPM.PRODUCT,
                                        LISTAGG(IIMS.ITEM_CODE, ', ') 
                                            WITHIN GROUP (ORDER BY IIMS.ITEM_CODE) AS ITEM_CODE,
                                        LISTAGG(IIMS.ITEM_CODE, ', ') 
                                            WITHIN GROUP (ORDER BY IIMS.ITEM_CODE) AS ITEM_CODE_LIST,
                                        ROW_NUMBER() OVER (PARTITION BY IIQPM.PRODUCT ORDER BY IIQPM.PRODUCT) AS RN
                                    FROM ip_product_item_master_setup IIMS
                                    LEFT JOIN PRODUCT_ITEMS_MAP IIQC
                                           ON IIMS.ITEM_CODE = IIQC.ITEM_ID
                                    LEFT JOIN IP_PRODUCT_MASTER IIQPM
                                           ON IIQPM.PRODUCT_ID = IIQC.PRODUCT_ID
                                    WHERE 
                                     IIQC.PRODUCT_ID = '{id}'
                                    GROUP BY IIQPM.PRODUCT_ID,IIQPM.PRODUCT
                                )
                                WHERE RN = 1
                                ORDER BY PRODUCT";
                pdetails = this._dbContext.SqlQuery<ProductDetails>(query).FirstOrDefault();
                //FormDetailList = this._dbContext.SqlQuery<ProductDetails>(query).ToList();

                string query_parameters = $@"select distinct parameters,specification,unit,target,tolerence,PRODUCT_ID from IP_PRODUCT_PARAMETER_DETAILS where PRODUCT_ID = '{id}' and deleted_flag ='N'";

                pdetails.ParameterDetailsList = this._dbContext.SqlQuery<ParameterDetails>(query_parameters).ToList();

                return pdetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeleteInternalInspectionSetupById(string id)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE IP_PRODUCT_MASTER SET DELETED_FLAG ='Y' WHERE PRODUCT_ID='{id}'";
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
