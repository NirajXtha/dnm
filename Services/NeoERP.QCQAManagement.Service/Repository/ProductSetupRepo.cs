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
    public class ProductSetupRepo : IProductSetupRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public ProductSetupRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }

        public List<ItemDetails> GetItemDetails()
        {
            try
            {
                string query = $@"SELECT 
    t.PRODUCT_TYPE,
    t.TYPE,
    LISTAGG(t.ITEM_EDESC, ', ') WITHIN GROUP (ORDER BY t.ITEM_EDESC) AS ITEM_EDESC,
    t.PARAM_CODE
FROM (
    SELECT DISTINCT 
        IIMS.PRODUCT_TYPE,
        IIMS.ITEM_EDESC,
        IIQC.PARAM_CODE,
        CASE WHEN  IIQM.TYPE ='OI' THEN 'OnSite Inspection' WHEN TYPE = 'II' THEN 'Internal Inspection' WHEN TYPE ='DW' THEN 'Daily Wastage' ELSE '' END  TYPE
    FROM ip_product_item_master_setup IIMS
    INNER JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQC 
        ON IIMS.ITEM_CODE = IIQC.ITEM_CODE
    INNER JOIN IP_ITEM_QC_PARAMETER_MASTER IIQM 
        ON IIQM.PARAM_CODE = IIQC.PARAM_CODE
    WHERE 
        IIQM.DELETED_FLAG = 'N'
        AND IIMS.PRODUCT_TYPE = IIQC.PRODUCT_TYPE
) t
GROUP BY 
    t.PRODUCT_TYPE,
    t.TYPE,
    t.PARAM_CODE";
                List<ItemDetails> qc = this._dbContext.SqlQuery<ItemDetails>(query).ToList();
                return qc;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<ProductSetup> GetProductTypeLists()
        {
            try
            {
                List<ProductSetup> productList = new List<ProductSetup>();
                //string query = $@"select distinct PRODUCT_TYPE from ip_product_item_master_setup where Product_type is not null";
                //string query = $@"SELECT MIN(ITEM_CODE) AS ITEM_CODE,PRODUCT_TYPE FROM ip_product_item_master_setup WHERE PRODUCT_TYPE IS NOT NULL GROUP BY PRODUCT_TYPE";
                string query = $@"SELECT PRODUCT_TYPE,
       ITEM_CODE_LIST as ITEM_CODE
FROM (
    SELECT 
        IIMS.PRODUCT_TYPE,
        IIMS.ITEM_CODE,
        LISTAGG(IIMS.ITEM_CODE, ', ') 
            WITHIN GROUP (ORDER BY IIMS.ITEM_CODE) OVER (PARTITION BY IIMS.PRODUCT_TYPE) AS ITEM_CODE_LIST,
        ROW_NUMBER() OVER (PARTITION BY IIMS.PRODUCT_TYPE ORDER BY IIMS.ITEM_CODE) AS RN
    FROM ip_product_item_master_setup IIMS
    LEFT JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQC
           ON IIMS.ITEM_CODE = IIQC.ITEM_CODE
    WHERE IIMS.PRODUCT_TYPE IS NOT NULL
) t
WHERE RN = 1
ORDER BY PRODUCT_TYPE";
                productList = this._dbContext.SqlQuery<ProductSetup>(query).ToList();
                return productList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ItemSetup> GetProductLists(string ProductType,string Category_Code)
        {
            try
            {
                List<ItemSetup> productList = new List<ItemSetup>();
                //string query = $@"select ITEM_CODE,ITEM_EDESC from ip_product_item_master_setup  where PRODUCT_TYPE ='{ProductType}' order by item_code asc";
                string query = $@"select ITEM_CODE,ITEM_EDESC from ip_product_item_master_setup  where PRODUCT_TYPE ='{ProductType}'  AND( '{Category_Code}' IS NULL or CATEGORY_CODE IN ( SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE  WHERE CATEGORY_TYPE = '{Category_Code}')) ORDER BY ITEM_CODE ASC";
                productList = this._dbContext.SqlQuery<ItemSetup>(query).ToList();
                return productList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ProductDetails> GetParameterList()
        {
            string Query = $@"SELECT IIQPD.ITEM_CODE,
       IIQPD.PARAMETERS,
      CASE WHEN  IIQPM.TYPE ='OI' THEN 'OnSite Inspection' WHEN TYPE = 'II' THEN 'Internal Inspection' WHEN TYPE ='DW' THEN 'Daily Wastage' ELSE '' END  TYPE,    
       IIQPD.TARGET,
       IIQPD.TOLERENCE
FROM IP_ITEM_QC_PARAMETER_DETAILS IIQPD
LEFT JOIN IP_ITEM_QC_PARAMETER_MASTER IIQPM ON IIQPM.PARAM_CODE = IIQPD.PARAM_CODE

UNION ALL

SELECT '' AS ITEM_CODE,
       '' AS PARAMETERS,
       '' AS TYPE,
       ''    AS TARGET,
       '' AS TOLERENCE
FROM DUAL";
            List<ProductDetails> entity = this._dbContext.SqlQuery<ProductDetails>(Query).ToList();
            return entity;
        }
        public bool InsertProductDetails(ProductDetails data)
        {
            try
            {
                //string query_serial_no = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM IP_ITEM_QC_PARAMETER_DETAILS WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                //int serial_no = Convert.ToInt32(this._dbContext.SqlQuery<string>(query_serial_no).FirstOrDefault());

                //string query_id = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(PARAM_CODE)), 0) + 1) FROM IP_ITEM_QC_PARAMETER_MASTER WHERE REGEXP_LIKE(PARAM_CODE, '^\d+$')";
                string query_id = $@"SELECT TO_CHAR(MIN(t.num) ) AS NEXT_PARAM_CODE FROM (SELECT LEVEL AS num FROM dual CONNECT BY LEVEL <= ( SELECT NVL(MAX(TO_NUMBER(PARAM_CODE)), 0) + 1 FROM IP_ITEM_QC_PARAMETER_MASTER WHERE REGEXP_LIKE(PARAM_CODE, '^\d+$'))) t WHERE NOT EXISTS (SELECT 1 FROM IP_ITEM_QC_PARAMETER_MASTER m WHERE REGEXP_LIKE(m.PARAM_CODE, '^\d+$') AND TO_NUMBER(m.PARAM_CODE) = t.num )";
                int id = Convert.ToInt32(this._dbContext.SqlQuery<string>(query_id).FirstOrDefault());

                if (data.ParameterDetailsList[0].PARAM_CODE == "")
                {
                    string insertMasterQuery = string.Format(@"
                            INSERT INTO IP_ITEM_QC_PARAMETER_MASTER (PARAM_CODE
                                , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,PRODUCT_TYPE,TYPE)
                            VALUES('{0}', '{1}',TO_DATE('{2}', 'DD-MON-YYYY'),'{3}','{4}','{5}','{6}','{7}')",
                                id, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                        , 'N', _workContext.CurrentUserinformation.company_code,
                        _workContext.CurrentUserinformation.branch_code,data.PRODUCT_TYPE,data.TYPE);  // change hard code 498
                    _dbContext.ExecuteSqlCommand(insertMasterQuery);

                    int i = 1;
                    foreach (var raw in data.ITEMSETUPS)
                    {
                        foreach (var para in data.ParameterDetailsList)
                        {
                            if (para.PARAM_CODE == "")
                            {
                                string insertQuery = string.Format(@"
                            INSERT INTO IP_ITEM_QC_PARAMETER_DETAILS (SERIAL_NO,PARAM_CODE,ITEM_CODE,PARAMETERS,SPECIFICATION,TARGET,TOLERENCE
                                , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,PRODUCT_TYPE)
                            VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}', TO_DATE('{8}', 'DD-MON-YYYY'),'{9}','{10}','{11}','{12}')",
                                            i, id, raw, para.PARAMETERS,para.SPECIFICATION, para.TARGET, para.TOLERENCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                    , 'N', _workContext.CurrentUserinformation.company_code,
                                    _workContext.CurrentUserinformation.branch_code,data.PRODUCT_TYPE);  // change hard code 498
                                _dbContext.ExecuteSqlCommand(insertQuery);
                                i++;
                            }
                        }
                    }
                }
               
                else
                {
                    string deleteParameterDetailsQuery = $@"Delete from IP_ITEM_QC_PARAMETER_DETAILS WHERE PARAM_CODE = '{data.ParameterDetailsList[0].PARAM_CODE}' ";
                    _dbContext.ExecuteSqlCommand(deleteParameterDetailsQuery);
                    int i = 1;
                    foreach (var raw in data.ITEMSETUPS)
                    {
                        foreach (var para in data.ParameterDetailsList)
                        {                          
                            string insertQuery = string.Format(@"
                        INSERT INTO IP_ITEM_QC_PARAMETER_DETAILS (SERIAL_NO,PARAM_CODE,ITEM_CODE,PARAMETERS,SPECIFICATION,TARGET,TOLERENCE
                            , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,PRODUCT_TYPE)
                        VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}','{7}', TO_DATE('{8}', 'DD-MON-YYYY'),'{9}','{10}','{11}','{12}')",
                                        i, data.ParameterDetailsList[0].PARAM_CODE, raw, para.PARAMETERS,para.SPECIFICATION, para.TARGET, para.TOLERENCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code,data.PRODUCT_TYPE);  // change hard code 498
                            _dbContext.ExecuteSqlCommand(insertQuery);
                            i++;                           
                        }
                    }
                    //PARAM_CODE = '{data.ParameterDetailsList[0].PARAM_CODE}',
                    string updateQuery = $@"UPDATE IP_ITEM_QC_PARAMETER_MASTER 
                           SET           
                           PRODUCT_TYPE = '{data.PRODUCT_TYPE}', 
                           TYPE = '{data.TYPE}',
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                            WHERE PARAM_CODE = '{data.ParameterDetailsList[0].PARAM_CODE}' ";
                    _dbContext.ExecuteSqlCommand(updateQuery);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //public List<ProductDetails> GetProductById(string id)
        public ProductDetails GetProductById(string id)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                ProductDetails pdetails = new ProductDetails();
                List<ProductDetails> FormDetailList = new List<ProductDetails>();
                //                string query = $@"SELECT 
                //    IIMS.PRODUCT_TYPE,
                //    (SELECT ITEM_CODE_LIST
                //FROM (
                //    SELECT 
                //        IIMS.PRODUCT_TYPE,
                //        LISTAGG(IIMS.ITEM_CODE, ', ') 
                //            WITHIN GROUP (ORDER BY IIMS.ITEM_CODE) AS ITEM_CODE_LIST
                //    FROM ip_product_item_master_setup IIMS
                //    LEFT JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQC
                //           ON IIMS.ITEM_CODE = IIQC.ITEM_CODE
                //    WHERE IIMS.PRODUCT_TYPE IS NOT NULL
                //    AND IIQC.PARAM_CODE = '{id}'
                //    GROUP BY IIMS.PRODUCT_TYPE
                //    ORDER BY IIMS.PRODUCT_TYPE
                //)
                //WHERE ROWNUM = 1) AS ITEM_CODE,
                //    LISTAGG(IIMS.ITEM_CODE, ', ') 
                //        WITHIN GROUP (ORDER BY IIMS.ITEM_CODE) AS ITEM_CODE_LIST
                //FROM ip_product_item_master_setup IIMS
                //INNER JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQC
                //       ON IIMS.ITEM_CODE = IIQC.ITEM_CODE
                //WHERE IIMS.PRODUCT_TYPE IS NOT NULL
                //  AND IIQC.PARAM_CODE = '{id}'
                //GROUP BY IIMS.PRODUCT_TYPE
                //ORDER BY IIMS.PRODUCT_TYPE";
                string query = $@"SELECT PRODUCT_TYPE, TYPE,ITEM_CODE, ITEM_CODE_LIST
FROM (
    SELECT
        IIQPM.PRODUCT_TYPE,
        IIQPM.TYPE,
        LISTAGG(IIMS.ITEM_CODE, ', ') 
            WITHIN GROUP (ORDER BY IIMS.ITEM_CODE) AS ITEM_CODE,
        LISTAGG(IIMS.ITEM_CODE, ', ') 
            WITHIN GROUP (ORDER BY IIMS.ITEM_CODE) AS ITEM_CODE_LIST,
        ROW_NUMBER() OVER (PARTITION BY IIQPM.PRODUCT_TYPE ORDER BY IIQPM.TYPE) AS RN
    FROM ip_product_item_master_setup IIMS
    LEFT JOIN IP_ITEM_QC_PARAMETER_DETAILS IIQC
           ON IIMS.ITEM_CODE = IIQC.ITEM_CODE
    LEFT JOIN IP_ITEM_QC_PARAMETER_MASTER IIQPM
           ON IIQPM.PARAM_CODE = IIQC.PARAM_CODE
    WHERE IIQPM.PRODUCT_TYPE IS NOT NULL
      AND IIQC.PARAM_CODE = '{id}'
       AND IIQPM.PARAM_CODE = '{id}'
    GROUP BY IIQPM.PRODUCT_TYPE, IIQPM.TYPE
)
WHERE RN = 1
ORDER BY PRODUCT_TYPE";
                pdetails = this._dbContext.SqlQuery<ProductDetails>(query).FirstOrDefault();
                //FormDetailList = this._dbContext.SqlQuery<ProductDetails>(query).ToList();

                String query_parameters = $@"select distinct PARAMETERS,SPECIFICATION,TARGET,TOLERENCE,PARAM_CODE FROM IP_ITEM_QC_PARAMETER_DETAILS where PARAM_CODE = '{id}' AND deleted_flag ='N'";

                pdetails.ParameterDetailsList = this._dbContext.SqlQuery<ParameterDetails>(query_parameters).ToList();

                return pdetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteProductSetupById(string id)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE IP_ITEM_QC_PARAMETER_MASTER SET DELETED_FLAG ='Y' WHERE PARAM_CODE='{id}'";
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
