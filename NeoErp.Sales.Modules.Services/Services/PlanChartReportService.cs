using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.SalesDashBoard;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class PlanChartReportService: IPlanChartReport
    {
        private IWorkContext _workcontext;
        private NeoErpCoreEntity _objectEntity;
        private string PreviousDataBaseUser;
        public PlanChartReportService(NeoErpCoreEntity objectEntity, IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            this._workcontext = workContext;
            this.PreviousDataBaseUser = ConfigurationManager.AppSettings["PreviousUserId"];
            if(!string.IsNullOrEmpty(PreviousDataBaseUser))
            {
                PreviousDataBaseUser = PreviousDataBaseUser + ".";
            }
        }
        public List<PlanReportModel> getMonthlySalesPlanChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  MONTH,MONTHINT,YEAR,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";

            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += $@" GROUP BY MONTH,MONTHINT,YEAR,COMPANY_CODE";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getMonthlyProductSalesPlanChart(ReportFiltersModel model, string DateFormat,int month)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  ITEM_EDESC AS MONTH,YEAR,COMPANY_CODE,MONTH AS MONTHINT
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) AND MONTHINT={month} ";

            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT DISTINCT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += $@" GROUP BY ITEM_EDESC,YEAR,COMPANY_CODE,MONTH";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<PlanReportModel> getEmployeeWiseSalesPlanChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            var userId = _workcontext.CurrentUserinformation.User_id;

            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();


            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  EMPLOYEE_CODE,EMPLOYEE_EDESC as DESCRIPTION,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";

            // added by chandra for load  only related report of logined person if not admin
            if(superFlag!="Y" && model.EmployeeFilter.Count==0)
            {
                Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
            }

            if (model.CustomerFilter.Count > 0 )
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += $@" GROUP BY EMPLOYEE_CODE,EMPLOYEE_EDESC,COMPANY_CODE";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getEmployeeCustomerWiseSalesPlanChart(ReportFiltersModel model, string DateFormat,string EmpCode)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  CUSTOMER_CODE,CUSTOMER_EDESC as DESCRIPTION,EMPLOYEE_EDESC,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND SPD.EMPLOYEE_CODE='{EmpCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            //if (model.EmployeeFilter.Count > 0)
            //{
            //    Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            //}
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += $@" GROUP BY EMPLOYEE_EDESC,COMPANY_CODE,CUSTOMER_CODE,CUSTOMER_EDESC";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getDivisionWiseSalesPlanChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

            

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  DIVISION_CODE,DIVISION_EDESC as DESCRIPTION
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";
            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += $@" GROUP BY DIVISION_CODE,DIVISION_EDESC";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getDivisionEmployeeWiseSalesPlanChart(ReportFiltersModel model, string DateFormat,string divisionCode)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();


            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  DIVISION_EDESC,EMPLOYEE_EDESC as DESCRIPTION
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.DIVISION_CODE='{divisionCode}' AND SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0)";
            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            //if (model.DivisionFilter.Count > 0)
            //{
            //    Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            //}

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += $@" GROUP BY EMPLOYEE_CODE,EMPLOYEE_EDESC,DIVISION_EDESC";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<PlanReportModel> getMonthlyProcurementPlanChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  MONTH,YEAR,MONTHINT,COMPANY_CODE
                                  FROM TEMP_PL_PORCRUMENT_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}'";
            
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            Query += $@" GROUP BY MONTH,MONTHINT,YEAR,COMPANY_CODE";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getMonthlyProductionPlanChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  MONTH,YEAR,MONTHINT,COMPANY_CODE
                                  FROM TEMP_PL_PRO_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}'";

            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            Query += $@" GROUP BY MONTH,YEAR,MONTHINT,COMPANY_CODE";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getMonthlyBudgetPlanChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  MONTH,YEAR,MONTHINT,COMPANY_CODE
                                  FROM TEMP_PL_B_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}'";

            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }
            
            Query += $@" GROUP BY MONTH,YEAR,MONTHINT,COMPANY_CODE";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getMonthlyLedgerPlanChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  MONTH,YEAR,MONTHINT,COMPANY_CODE
                                  FROM TEMP_PL_LB_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}'";

            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            Query += $@" GROUP BY MONTH,YEAR,MONTHINT,COMPANY_CODE";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanReportModel> getMonthlySalesPlanVsSalesChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  MONTH,MONTHINT,YEAR,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";
            var SalesQuery = $@"SELECT   SUM(TOTAL_PRICE) PER_DAY_AMOUNT,
                                  SUM(QUANTITY) PER_DAY_QUANTITY,  TO_CHAR(fn_bs_month(substr(bs_date(SPD.SALES_DATE), 6, 2))) MONTH,
                                        SUBSTR(BS_DATE(SPD.SALES_DATE), 6, 2) MONTHINT,
                                        TO_CHAR(substr(bs_date(SPD.SALES_DATE), 0, 4)) as YEAR,COMPANY_CODE
                                  FROM {PreviousDataBaseUser}sa_sales_invoice SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}'";

            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
               var filter = $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
                Query += filter;
                SalesQuery += filter;
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                SalesQuery=SalesQuery+ string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
                SalesQuery=SalesQuery+ string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
                SalesQuery += string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                SalesQuery += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            SalesQuery += PreviousFiscalYearDateFilter();
            Query += $@" GROUP BY MONTH,MONTHINT,YEAR,COMPANY_CODE";
            SalesQuery += $@" group by TO_CHAR(fn_bs_month(substr(bs_date(SPD.SALES_DATE), 6, 2))),SUBSTR(BS_DATE(SPD.SALES_DATE), 6, 2),TO_CHAR(substr(bs_date(SPD.SALES_DATE), 0, 4)),COMPANY_CODE";

            var result = new List<PlanReportModel>();
            var sales= new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
                sales = _objectEntity.SqlQuery<PlanReportModel>(SalesQuery).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            foreach(var item in result)
            {
                item.PER_DAY_SALES_AMOUNT = sales.Where(x => x.MONTH == item.MONTH).Select(x => x.PER_DAY_AMOUNT).FirstOrDefault();
                item.PER_DAY_SALES_QUANTITY = sales.Where(x => x.MONTH == item.MONTH).Select(x => x.PER_DAY_QUANTITY).FirstOrDefault();
            }
            return result;
        }
        public List<PlanReportModel> getMonthlySalesPlanVsSalesProductChart(ReportFiltersModel model, string DateFormat, int month)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  ITEM_EDESC AS MONTH,YEAR,COMPANY_CODE,MONTH AS MONTHINT,ITEM_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) AND MONTHINT={month} ";

            var SalesQuery = $@"SELECT   SUM(TOTAL_PRICE) PER_DAY_AMOUNT,
                                  SUM(QUANTITY) PER_DAY_QUANTITY, 
                                    ITEM_CODE
                                 --ITEM_EDESC MONTH,
                                    -- TO_CHAR(fn_bs_month(substr(bs_date(SPD.SALES_DATE), 6, 2))) MONTHINT,
                                      --  SUBSTR(BS_DATE(SPD.SALES_DATE), 6, 2) MONTHINT,
                                        --TO_CHAR(substr(bs_date(SPD.SALES_DATE), 0, 4)) as YEAR,COMPANY_CODE
                                  FROM {PreviousDataBaseUser}sa_sales_invoice SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND SUBSTR(BS_DATE(SPD.SALES_DATE), 6, 2)={month}";

            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                var filterQuery= $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT DISTINCT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
                Query += filterQuery;
                SalesQuery += filterQuery;
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                SalesQuery = SalesQuery + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
                SalesQuery = SalesQuery + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
                SalesQuery += string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                SalesQuery += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            SalesQuery += PreviousFiscalYearDateFilter();
            Query += $@" GROUP BY ITEM_EDESC, MONTH,YEAR,COMPANY_CODE,ITEM_CODE";
            SalesQuery += $@" group by ITEM_CODE";

            var result = new List<PlanReportModel>();
            var sales = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
                sales = _objectEntity.SqlQuery<PlanReportModel>(SalesQuery).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            foreach (var item in result)
            {
                item.PER_DAY_SALES_AMOUNT = sales.Where(x => x.ITEM_CODE == item.ITEM_CODE).Select(x => x.PER_DAY_AMOUNT).FirstOrDefault();
                item.PER_DAY_SALES_QUANTITY = sales.Where(x => x.ITEM_CODE == item.ITEM_CODE).Select(x => x.PER_DAY_QUANTITY).FirstOrDefault();
            }
            return result;
        }

        public List<PlanReportModel> getEmployeeWiseSalesPlanVsPreviousSalesChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            var userId = _workcontext.CurrentUserinformation.User_id;

            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();


            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  EMPLOYEE_CODE,EMPLOYEE_EDESC as DESCRIPTION,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";
            var SalesQuery = $@"SELECT   SUM(TOTAL_PRICE) PER_DAY_AMOUNT,
                                  SUM(QUANTITY) PER_DAY_QUANTITY, 
                                    Employee_Code
                                  FROM {PreviousDataBaseUser}sa_sales_invoice SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}'";

            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
              var  FilterQuery = $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
                Query += FilterQuery;
                SalesQuery += FilterQuery;
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                SalesQuery+= string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString()); 
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            SalesQuery += PreviousFiscalYearDateFilter();
            Query += $@" GROUP BY EMPLOYEE_CODE,EMPLOYEE_EDESC,COMPANY_CODE";
            SalesQuery += $@" GROUP BY EMPLOYEE_CODE";

            var result = new List<PlanReportModel>();
            var sales = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
                sales = _objectEntity.SqlQuery<PlanReportModel>(SalesQuery).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            foreach (var item in result)
            {
                item.PER_DAY_SALES_AMOUNT = sales.Where(x => x.EMPLOYEE_CODE == item.EMPLOYEE_CODE).Select(x => x.PER_DAY_AMOUNT).FirstOrDefault();
                item.PER_DAY_SALES_QUANTITY = sales.Where(x => x.EMPLOYEE_CODE == item.EMPLOYEE_CODE).Select(x => x.PER_DAY_QUANTITY).FirstOrDefault();
            }
            return result;
        }
        public List<PlanReportModel> getEmployeeCustomerWiseSalesPlanVsPreviousSalesChart(ReportFiltersModel model, string DateFormat, string EmpCode)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  CUSTOMER_CODE,CUSTOMER_EDESC as DESCRIPTION,EMPLOYEE_EDESC,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND SPD.EMPLOYEE_CODE='{EmpCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";

            var SalesQuery = $@"SELECT   SUM(TOTAL_PRICE) PER_DAY_AMOUNT,
                                  SUM(QUANTITY) PER_DAY_QUANTITY, 
                                    CUSTOMER_CODE
                                  FROM {PreviousDataBaseUser}sa_sales_invoice SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND SPD.EMPLOYEE_CODE='{EmpCode}'";
            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                SalesQuery+= string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            //if (model.EmployeeFilter.Count > 0)
            //{
            //    Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            //}
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                SalesQuery+= Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            SalesQuery += PreviousFiscalYearDateFilter();
            Query += $@" GROUP BY EMPLOYEE_EDESC,COMPANY_CODE,CUSTOMER_CODE,CUSTOMER_EDESC";
            SalesQuery += $@" GROUP BY CUSTOMER_CODE";

            var result = new List<PlanReportModel>();
            var sales = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
                sales = _objectEntity.SqlQuery<PlanReportModel>(SalesQuery).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            foreach (var item in result)
            {
                item.PER_DAY_SALES_AMOUNT = sales.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Select(x => x.PER_DAY_AMOUNT).FirstOrDefault();
                item.PER_DAY_SALES_QUANTITY = sales.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Select(x => x.PER_DAY_QUANTITY).FirstOrDefault();
            }
            return result;
        }
        public List<PlanReportModel> getDivisionWiseSalesPlanVsPreviousYearSalesChartChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();



            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  DIVISION_CODE,DIVISION_EDESC as DESCRIPTION
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";

            var SalesQuery = $@"SELECT   SUM(TOTAL_PRICE) PER_DAY_AMOUNT,
                                  SUM(QUANTITY) PER_DAY_QUANTITY, 
                                    DIVISION_CODE
                                  FROM {PreviousDataBaseUser}sa_sales_invoice SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}'";
            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
               var FilterQuery = $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
                Query += FilterQuery;
                SalesQuery += FilterQuery;
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                SalesQuery+= Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            SalesQuery += PreviousFiscalYearDateFilter();
            Query += $@" GROUP BY DIVISION_CODE,DIVISION_EDESC";
            SalesQuery += $@" GROUP BY DIVISION_CODE";

            var result = new List<PlanReportModel>();
            var sales = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
                sales = _objectEntity.SqlQuery<PlanReportModel>(SalesQuery).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            foreach (var item in result)
            {
                item.PER_DAY_SALES_AMOUNT = sales.Where(x => x.DIVISION_CODE == item.DIVISION_CODE).Select(x => x.PER_DAY_AMOUNT).FirstOrDefault();
                item.PER_DAY_SALES_QUANTITY = sales.Where(x => x.DIVISION_CODE == item.DIVISION_CODE).Select(x => x.PER_DAY_QUANTITY).FirstOrDefault();
            }
            return result;
        }
        public List<PlanReportModel> getDivisionEmployeeWiseSalesPlanVsPreviousYearSalesChartChart(ReportFiltersModel model, string DateFormat, string divisionCode)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();


            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  DIVISION_EDESC,EMPLOYEE_EDESC as DESCRIPTION,Employee_Code
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.DIVISION_CODE='{divisionCode}' AND SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0)";
            var SalesQuery = $@"SELECT   SUM(TOTAL_PRICE) PER_DAY_AMOUNT,
                                  SUM(QUANTITY) PER_DAY_QUANTITY, 
                                    DIVISION_CODE,EMPLOYEE_CODE
                                  FROM {PreviousDataBaseUser}sa_sales_invoice SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND DIVISION_CODE='{divisionCode}'";
            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
               var FilterQuery = $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
                Query += FilterQuery;
                SalesQuery += FilterQuery;
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                SalesQuery+= string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers)); ;
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            //if (model.DivisionFilter.Count > 0)
            //{
            //    Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            //}

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                SalesQuery+= string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            SalesQuery += PreviousFiscalYearDateFilter();
            Query += $@" GROUP BY EMPLOYEE_CODE,EMPLOYEE_EDESC,DIVISION_EDESC";
            SalesQuery += $@" GROUP BY DIVISION_CODE,EMPLOYEE_CODE";

            var result = new List<PlanReportModel>();
            var sales = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
                sales = _objectEntity.SqlQuery<PlanReportModel>(SalesQuery).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            foreach (var item in result)
            {
                item.PER_DAY_SALES_AMOUNT = sales.Where(x => x.EMPLOYEE_CODE == item.EMPLOYEE_CODE).Select(x => x.PER_DAY_AMOUNT).FirstOrDefault();
                item.PER_DAY_SALES_QUANTITY = sales.Where(x => x.EMPLOYEE_CODE == item.EMPLOYEE_CODE).Select(x => x.PER_DAY_QUANTITY).FirstOrDefault();
            }
            return result;
        }
        public List<PlanReportModel> getItemWiseQuantitySalesPlanChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();



            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  ITEM_CODE,ITEM_EDESC as DESCRIPTION
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";
            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += $@" GROUP BY ITEM_CODE,ITEM_EDESC";

            var result = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private string PreviousFiscalYearDateFilter()
        {
            string result = string.Empty;
            string previousFiscalYearCode = ConfigurationManager.AppSettings["PreviousFiscalYear"].ToString();
            string sqlQuery = $@"SELECT START_DATE,END_DATE FROM PL_FISCAL_YEAR_CODE WHERE FISCAL_YEAR_CODE='{previousFiscalYearCode}' AND ACTIVE='Y' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
            var data = _objectEntity.SqlQuery<FiscalYearModel>(sqlQuery).FirstOrDefault();
            if(data !=null)
            {

                result = $@" AND SPD.SALES_DATE>= TO_DATE('{data.START_DATE.ToString("MM/dd/yyyy")}','MM/DD/YYYY') AND SPD.SALES_DATE<= TO_DATE('{data.END_DATE.ToString("MM/dd/yyyy")}','MM/DD/YYYY') ";
            }
            return result;
        }
        public List<PlanReportModel> getMonthlySalesVsTargetChart(ReportFiltersModel model, string DateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  MONTH,MONTHINT,YEAR,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) ";
            var SalesQuery = $@"SELECT   SUM(TOTAL_PRICE) PER_DAY_AMOUNT,
                                  SUM(QUANTITY) PER_DAY_QUANTITY,  TO_CHAR(fn_bs_month(substr(bs_date(SPD.SALES_DATE), 6, 2))) MONTH,
                                        SUBSTR(BS_DATE(SPD.SALES_DATE), 6, 2) MONTHINT,
                                        TO_CHAR(substr(bs_date(SPD.SALES_DATE), 0, 4)) as YEAR,COMPANY_CODE
                                  FROM sa_sales_invoice SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}'";

            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                var filter = $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
                Query += filter;
                SalesQuery += filter;
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                SalesQuery = SalesQuery + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
                SalesQuery = SalesQuery + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
                SalesQuery += string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                SalesQuery += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
           // SalesQuery += PreviousFiscalYearDateFilter();
            Query += $@" GROUP BY MONTH,MONTHINT,YEAR,COMPANY_CODE";
            SalesQuery += $@" group by TO_CHAR(fn_bs_month(substr(bs_date(SPD.SALES_DATE), 6, 2))),SUBSTR(BS_DATE(SPD.SALES_DATE), 6, 2),TO_CHAR(substr(bs_date(SPD.SALES_DATE), 0, 4)),COMPANY_CODE";

            var result = new List<PlanReportModel>();
            var sales = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
                sales = _objectEntity.SqlQuery<PlanReportModel>(SalesQuery).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            foreach (var item in sales)
            {
                item.PER_DAY_SALES_AMOUNT = result.Where(x => x.MONTH == item.MONTH).Select(x => x.PER_DAY_AMOUNT).FirstOrDefault();
                item.PER_DAY_SALES_QUANTITY = result.Where(x => x.MONTH == item.MONTH).Select(x => x.PER_DAY_QUANTITY).FirstOrDefault();
            }
            return sales;
        }
        public List<PlanReportModel> getMonthlySalesVsTargetByProductChart(ReportFiltersModel model, string DateFormat, int month)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _objectEntity.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _objectEntity.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

            var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  ITEM_EDESC AS MONTH,YEAR,COMPANY_CODE,MONTH AS MONTHINT,ITEM_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) AND MONTHINT={month} ";

            var SalesQuery = $@"SELECT   SUM(TOTAL_PRICE) PER_DAY_AMOUNT,
                                  SUM(QUANTITY) PER_DAY_QUANTITY, 
                                    SPD.ITEM_CODE,
                                    IP.ITEM_EDESC MONTH
                                 --ITEM_EDESC MONTH,
                                    -- TO_CHAR(fn_bs_month(substr(bs_date(SPD.SALES_DATE), 6, 2))) MONTHINT,
                                      --  SUBSTR(BS_DATE(SPD.SALES_DATE), 6, 2) MONTHINT,
                                        --TO_CHAR(substr(bs_date(SPD.SALES_DATE), 0, 4)) as YEAR,COMPANY_CODE
                                  FROM sa_sales_invoice SPD  ,ip_item_master_setup ip
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND SUBSTR(BS_DATE(SPD.SALES_DATE), 6, 2)={month} and spd.deleted_flag='N' and IP.ITEM_CODE=SPD.ITEM_CODE and ip.company_code=spd.company_code and ip.deleted_flag='N' and IP.GROUP_SKU_FLAG='I'";

            // added by chandra for load  only related report of logined person if not admin
            if (superFlag != "Y" && model.EmployeeFilter.Count == 0)
            {
                var filterQuery = $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT DISTINCT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    )";
                Query += filterQuery;
                SalesQuery += filterQuery;
            }

            if (model.CustomerFilter.Count > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                SalesQuery = SalesQuery + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
            }

            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
                SalesQuery = SalesQuery + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
                SalesQuery += string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                SalesQuery += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            //SalesQuery += PreviousFiscalYearDateFilter();
            Query += $@" GROUP BY ITEM_EDESC, MONTH,YEAR,COMPANY_CODE,ITEM_CODE";
            SalesQuery += $@" group by SPD.ITEM_CODE,IP.ITEM_EDESC";

            var result = new List<PlanReportModel>();
            var sales = new List<PlanReportModel>();
            try
            {
                result = _objectEntity.SqlQuery<PlanReportModel>(Query).ToList();
                sales = _objectEntity.SqlQuery<PlanReportModel>(SalesQuery).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            foreach (var item in sales)
            {
                item.PER_DAY_SALES_AMOUNT = result.Where(x => x.ITEM_CODE == item.ITEM_CODE).Select(x => x.PER_DAY_AMOUNT).FirstOrDefault();
                item.PER_DAY_SALES_QUANTITY = result.Where(x => x.ITEM_CODE == item.ITEM_CODE).Select(x => x.PER_DAY_QUANTITY).FirstOrDefault();
            }
            return sales;
        }
    }
}
