using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Repository
{
    public class ChartReportRepo: IChartReport
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public ChartReportRepo(IDbContext dbContext, IWorkContext workContext)
        {
            this._workcontext = workContext;
            this._dbContext = dbContext;
        }
        public List<PlanReportModel> getMonthlySalesPlanChart(ReportFiltersModel model, string dateFormat)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT * FROM TEMP_SALES_PLAN_CHART_REPORT SPD WHERE SPD.COMPANY_CODE='{companyCode}'";
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
            var result = new List<PlanReportModel>();
            try
            {
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                var createTableQry = $@"CREATE TABLE TEMP_SALES_PLAN_CHART_REPORT AS SELECT DISTINCT 
                                        TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,
                                        SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
                                        TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR, SPD.COMPANY_CODE,
                                   SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT, 
                                   SUM(SPD.PER_DAY_QUANTITY) PER_DAY_QUANTITY
                              FROM PL_SALES_PLAN_DTL SPD,
                                   IP_ITEM_MASTER_SETUP IMS,
                                   PL_SALES_PLAN SP,
                                   SA_CUSTOMER_SETUP CS,
                                   FA_DIVISION_SETUP DS,
                                   HR_EMPLOYEE_SETUP ES,
                                   FA_BRANCH_SETUP FS
                                   WHERE  SP.DELETED_FLAG = 'N' 
                                                         AND SP.PLAN_CODE = SPD.PLAN_CODE
                                                         AND CS.CUSTOMER_CODE (+) = SPD.CUSTOMER_CODE
                                                         AND DS.DIVISION_CODE (+)= SPD.DIVISION_CODE
                                                         AND ES.EMPLOYEE_CODE (+)= SPD.EMPLOYEE_CODE
                                                         AND FS.BRANCH_CODE (+)= SPD.BRANCH_CODE
                                                         AND IMS.ITEM_CODE = SPD.ITEM_CODE
                                                         AND SPD.COMPANY_CODE = IMS.COMPANY_CODE
                                                         AND CS.COMPANY_CODE (+)=SPD.COMPANY_CODE
                                                         AND DS.COMPANY_CODE (+)=SPD.COMPANY_CODE
                                                         AND ES.COMPANY_CODE (+)=SPD.COMPANY_CODE
                                                         AND FS.COMPANY_CODE (+)=SPD.COMPANY_CODE
                                    AND IMS.GROUP_SKU_FLAG = 'I' 
                                    GROUP BY  TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))),
                                    TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) ,
                                    SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2),
                                    SPD.COMPANY_CODE";
                var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (result.Count() > 0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            return result;
        }
    }
}
