using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using NeoErp.Data;
using NeoErp.Core;
using NeoErp.Core.Domain;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace NeoErp.Planning.Service.Repository
{
    public class TargetExcelUploadRepo : ITargetExcelUploadRepo
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;

        public TargetExcelUploadRepo(IDbContext dbContext, IWorkContext workContext)
        {
            this._dbContext = dbContext;
            this._workcontext = workContext;
        }

        // INSERT METHOD
        public int InsertTargetExcelUpload(TargetExcelUpload model)
        {
            try
            {
                string query = $@"
                INSERT INTO TARGET_EXCEL_UPLOAD
                (
                    TARGETID,
                    FROM_DATE,
                    TO_DATE,
                    EMP_CODE,
                    DISTRIBUTOR_CODE,
                    ITEM_CODE,
                    QTY,
                    PRICE,
                    BRANCH_CODE,
                    COMPANY_CODE,
                    DELETED_FLAG,
                    CREATED_BY,
                    CREATED_DATE
                )
                VALUES
                (
                    (SELECT TARGET_EXCEL_SEQ.NEXTVAL || '-0001' FROM DUAL),
                    TO_DATE('{model.FROM_DATE:MM/dd/yyyy}','MM/DD/YYYY'),
                    TO_DATE('{model.TO_DATE:MM/dd/yyyy}','MM/DD/YYYY'),
                    '{model.EMP_CODE}',
                    '{model.DISTRIBUTOR_CODE}',
                    '{model.ITEM_CODE}',
                    {model.QTY},
                    {model.PRICE},
                    '{model.BRANCH_CODE}',
                    '{_workcontext.CurrentUserinformation.company_code}',
                    'N',
                    '{_workcontext.CurrentUserinformation.UserName}',
                    SYSDATE
                )";

                return _dbContext.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // BULK INSERT METHOD - Optimized with INSERT ALL
        public int InsertTargetExcelUploadList(List<TargetExcelUpload> modelList)
        {
            try
            {
                if (modelList == null || modelList.Count == 0)
                    return 0;

                // Get next sequence value once for the entire batch
                string getSeqQuery = "SELECT TARGET_EXCEL_SEQ.NEXTVAL FROM DUAL";
                var seqValue = _dbContext.SqlQuery<decimal>(getSeqQuery).FirstOrDefault();

                // Oracle INSERT ALL syntax for bulk insert
                var query = "INSERT ALL ";

                for (int i = 0; i < modelList.Count; i++)
                {
                    var model = modelList[i];
                    // Concatenate sequence value with row index using dash separator
                    // Example: 2-0001, 2-0002, 2-0003
                    string uniqueTargetId = $"{seqValue}-{(i + 1).ToString().PadLeft(6, '0')}";

                    query += $@"
                    INTO TARGET_EXCEL_UPLOAD
                    (
                        TARGETID,
                        FROM_DATE,
                        TO_DATE,
                        EMP_CODE,
                        DISTRIBUTOR_CODE,
                        ITEM_CODE,
                        QTY,
                        PRICE,
                        BRANCH_CODE,
                        COMPANY_CODE,
                        DELETED_FLAG,
                        CREATED_BY,
                        CREATED_DATE
                    )
                    VALUES
                    (
                        '{uniqueTargetId}',
                        TO_DATE('{model.FROM_DATE:MM/dd/yyyy}','MM/DD/YYYY'),
                        TO_DATE('{model.TO_DATE:MM/dd/yyyy}','MM/DD/YYYY'),
                        '{model.EMP_CODE}',
                        '{model.DISTRIBUTOR_CODE}',
                        '{model.ITEM_CODE}',
                        {model.QTY},
                        {model.PRICE},
                        '{model.BRANCH_CODE}',
                        '{_workcontext.CurrentUserinformation.company_code}',
                        'N',
                        '{_workcontext.CurrentUserinformation.UserName}',
                        SYSDATE
                    ) ";
                }

                query += " SELECT 1 FROM DUAL";

                return _dbContext.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // VALIDATION METHODS
        public List<DistributorValidationModel> ValidateDistributors(string companyCode)
        {
            var result = new List<DistributorValidationModel>();
            try
            {
                string query = $@"
                    SELECT customer_code DISTRIBUTOR_CODE, customer_edesc DISTRIBUTOR_NAME 
                    FROM 
                    (
                        SELECT a.CUSTOMER_CODE, a.CUSTOMER_EDESC, b.DISTRIBUTOR_CODE 
                        FROM SA_CUSTOMER_SETUP a,
                        DIST_DISTRIBUTOR_MASTER b
                        WHERE a.COMPANY_CODE='{companyCode}' 
                        AND a.customer_code=b.DISTRIBUTOR_CODE(+)
                        AND a.company_code=b.company_code(+) 
                        AND a.GROUP_SKU_FLAG='I'
                    ) 
                    WHERE distributor_code IS NOT NULL";

                var data = _dbContext.SqlQuery<DistributorValidationModel>(query).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ItemValidationModel> ValidateItems(string companyCode)
        {
            var result = new List<ItemValidationModel>();
            try
            {
                string query = $@"
                    SELECT ITEM_CODE, ITEM_EDESC ITEM_NAME 
                    FROM IP_ITEM_MASTER_SETUP 
                    WHERE DELETED_FLAG='N' 
                    AND COMPANY_CODE='{companyCode}' 
                    AND GROUP_SKU_FLAG='I' ";

                var data = _dbContext.SqlQuery<ItemValidationModel>(query).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BranchValidationModel> ValidateBranches(string companyCode)
        {
            var result = new List<BranchValidationModel>();
            try
            {
                string query = $@"
                    SELECT BRANCH_CODE, BRANCH_EDESC BRANCH_NAME 
                    FROM FA_BRANCH_SETUP 
                    WHERE COMPANY_CODE='{companyCode}' 
                    AND GROUP_SKU_FLAG='I' 
                    AND DELETED_FLAG='N'";

                var data = _dbContext.SqlQuery<BranchValidationModel>(query).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeValidationModel> ValidateEmployees(string companyCode)
        {
            var result = new List<EmployeeValidationModel>();
            try
            {
                //string query = $@"
                //    SELECT EMPLOYEE_CODE, EMPLOYEE_EDESC EMPLOYEE_NAME 
                //    FROM HR_EMPLOYEE_SETUP 
                //    WHERE COMPANY_CODE='{companyCode}' 
                //    AND GROUP_SKU_FLAG='I' 
                //    AND DELETED_FLAG='N' and employee_code in ('10052','10053')";

                string query = $@"select sp_code EMPLOYEE_CODE,full_name EMPLOYEE_NAME from dist_login_user WHERE COMPANY_CODE='{companyCode}'  ";

                var data = _dbContext.SqlQuery<EmployeeValidationModel>(query).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // NEW API METHODS

        // 1. Get Employee Filter List with debouncing support
        public List<EmployeeValidationModel> GetEmployeeFilterList(string companyCode, int limit)
        {
            var result = new List<EmployeeValidationModel>();
            try
            {
                string query = $@"
                    SELECT sp_code EMPLOYEE_CODE, full_name EMPLOYEE_NAME 
                    FROM dist_login_user 
                    WHERE sp_code IN (SELECT DISTINCT emp_code FROM TARGET_EXCEL_UPLOAD)
                    AND COMPANY_CODE='{companyCode}'
                    AND ROWNUM <= {limit}";

                var data = _dbContext.SqlQuery<EmployeeValidationModel>(query).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 2. Get Employee Target Data
        public List<TargetExcelUpload> GetEmployeeTargetData(string companyCode, string empCode)
        {
            var result = new List<TargetExcelUpload>();
            try
            {
                string query = $@"
                    SELECT TARGETID, FROM_DATE, TO_DATE, EMP_CODE, DISTRIBUTOR_CODE, 
                           ITEM_CODE, QTY, PRICE, BRANCH_CODE
                    FROM TARGET_EXCEL_UPLOAD 
                    WHERE EMP_CODE = '{empCode}' 
                    AND COMPANY_CODE = '{companyCode}'";

                var data = _dbContext.SqlQuery<TargetExcelUpload>(query).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 3. Delete By Target ID
        public int DeleteByTargetId(string targetId)
        {
            try
            {
                string query = $@"DELETE FROM TARGET_EXCEL_UPLOAD WHERE TARGETID = '{targetId}'";
                return _dbContext.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
