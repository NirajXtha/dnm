using NeoErp.Planning.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Models;
using NeoErp.Planning.Service.Models;
using NeoErp.Data;
using NeoErp.Core;

namespace NeoErp.Planning.Service.Repository
{
    public class PlanReportRepo : IPlanReport
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public PlanReportRepo(IDbContext idbContext, IWorkContext iworkContext)
        {
            this._dbContext = idbContext;
            this._workcontext = iworkContext;
        }
        public List<PlanReportModel> getAllSalesPlanReport(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            string Query =
                $@"SELECT DISTINCT TO_CHAR(SPD.PLAN_CODE) PLAN_CODE, SP.PLAN_EDESC, TO_CHAR(SPD.PLAN_DATE,'DD/MM/YYYY')PLAN_DATE,TO_CHAR(TO_BS(SPD.PLAN_DATE)) MITI ,
                                   SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT, SUM(SPD.PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                   SPD.ITEM_CODE, IMS.ITEM_EDESC, SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_GROUP_EDESC,
                                    SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_SUBGROUP_EDESC, SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC  
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
                                        AND PL.COMPANY_CODE='{companyCode}'
                                     AND SPD.PLAN_DATE >= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
                                    AND SPD.PLAN_DATE <= TO_DATE('{model.ToDate}',' YYYY-MON-DD')
                                    AND IMS.GROUP_SKU_FLAG = 'I'";

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
            Query += $@" GROUP BY SPD.PLAN_CODE, SPD.PLAN_DATE, SPD.ITEM_CODE, IMS.ITEM_EDESC, SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,TO_CHAR(TO_BS(SPD.PLAN_DATE)),
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC  ,SP.PLAN_EDESC ,SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE), 1, 30) ,
                                    SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE), 1, 30) ";

            var result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
            return result;
        }

        public List<PlanReportModel> getMonthlySalesPlanReport(ReportFiltersModel model, string dateType)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var PLAN_DATE = string.Empty;
            var MITI = string.Empty;
            var MONTHINT = string.Empty;
            if (dateType == "BS")
            {
                PLAN_DATE = "TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4))";
                MITI = "UPPER(TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))))";
                MONTHINT = "SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2)";
            }
            else {
                PLAN_DATE = "TO_CHAR(SPD.PLAN_DATE, 'YYYY')";
                MITI = "TO_CHAR(SPD.PLAN_DATE, 'MON')";
                MONTHINT = "TO_CHAR(SPD.PLAN_DATE, 'MM')";
            }

            string Query = $@"SELECT DISTINCT {PLAN_DATE}PLAN_DATE,SPD.ITEM_CODE, IMS.ITEM_EDESC,
                                {MITI} MITI ,
                                {MONTHINT} MONTHINT,
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
                                     AND SPD.PLAN_DATE >= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
                                    AND SPD.PLAN_DATE <= TO_DATE('{model.ToDate}',' YYYY-MON-DD')
                                    AND IMS.GROUP_SKU_FLAG = 'I'";
            Query += $@" GROUP BY {MITI},SPD.ITEM_CODE, IMS.ITEM_EDESC,
                                    {PLAN_DATE},{MONTHINT}
                                    ORDER BY {PLAN_DATE} , {MONTHINT} asc";
            //Query += $@" GROUP BY SPD.ITEM_CODE, IMS.ITEM_EDESC, SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))),TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)),SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2),
            //                       SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC,
            //                       ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC  ,SP.PLAN_EDESC ,SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE), 1, 30) ,
            //                        SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE), 1, 30) 
            //                        ORDER BY ITEM_CODE,TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) , SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) asc";

            var result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
            return result;
        }
        public List<PlanReportModel> getFavSalesPlanReport(ReportFiltersModel model,int page,int size)
        {
            //int page = 1, size = 100;
            int start = (page - 1) * size + 1, end = page * size;
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            // var Query = $@"SELECT * FROM TEMP_PL_SALES_PLAN_REPORT SPD WHERE SPD.COMPANY_CODE='{companyCode}'";
            //added by chandra for item grouo edesc and item subgroup edesc TO_CHAR(a.PLAN_DATE) PLAN_DATE,
            var Query = $@"SELECT * FROM(SELECT SPD1.*, Z.ITEM_EDESC ITEM_GROUP_EDESC,ROWNUM RN
                          FROM    (SELECT X.*,
                                          Y.ITEM_EDESC ITEM_SUBGROUP_EDESC,
                                          y.pre_item_code ITEM_GROUP_CODE
                                     FROM    (SELECT TO_CHAR(a.PLAN_DATE) PLAN_DATE,a.COMPANY_CODE, a.PLAN_EDESC,a.ITEM_EDESC,a.PER_DAY_AMOUNT,a.PER_DAY_QUANTITY, B.MASTER_ITEM_CODE, B.PRE_ITEM_CODE,CUSTOMER_EDESC,EMPLOYEE_EDESC,DIVISION_EDESC,BRANCH_EDESC
                                                FROM    TEMP_PL_SALES_PLAN_REPORT A
                                                     INNER JOIN
                                                        IP_ITEM_MASTER_SETUP B
                                                     ON A.ITEM_CODE = B.ITEM_CODE AND A.COMPANY_CODE=B.COMPANY_CODE AND B.GROUP_SKU_FLAG='I' AND B.DELETED_FLAG='N') X
                                          INNER JOIN
                                             IP_ITEM_MASTER_SETUP y
                                          ON x.Pre_item_code = y.MASTER_ITEM_CODE) SPD1
                               INNER JOIN
                                  IP_ITEM_MASTER_SETUP Z
                               ON Z.MASTER_ITEM_CODE = SPD1.item_group_code
                                WHERE SPD1.COMPANY_CODE='{companyCode}' AND (PER_DAY_AMOUNT>0 OR PER_DAY_QUANTITY>0)
                                )SPD WHERE SPD.RN BETWEEN {start} AND {end}";

            var TotalQuery = $@"SELECT count(*) 
                          FROM    (SELECT X.*,
                                          Y.ITEM_EDESC ITEM_SUBGROUP_EDESC,
                                          y.pre_item_code ITEM_GROUP_CODE
                                     FROM    (SELECT TO_CHAR(a.PLAN_DATE) PLAN_DATE,a.COMPANY_CODE, a.PLAN_EDESC,a.ITEM_EDESC,a.PER_DAY_AMOUNT,a.PER_DAY_QUANTITY, B.MASTER_ITEM_CODE, B.PRE_ITEM_CODE,CUSTOMER_EDESC,EMPLOYEE_EDESC,DIVISION_EDESC,BRANCH_EDESC
                                                FROM    TEMP_PL_SALES_PLAN_REPORT A
                                                     INNER JOIN
                                                        IP_ITEM_MASTER_SETUP B
                                                     ON A.ITEM_CODE = B.ITEM_CODE AND A.COMPANY_CODE=B.COMPANY_CODE AND B.GROUP_SKU_FLAG='I' AND B.DELETED_FLAG='N') X
                                          INNER JOIN
                                             IP_ITEM_MASTER_SETUP y
                                          ON x.Pre_item_code = y.MASTER_ITEM_CODE) SPD1
                               INNER JOIN
                                  IP_ITEM_MASTER_SETUP Z
                               ON Z.MASTER_ITEM_CODE = SPD1.item_group_code
                                WHERE SPD1.COMPANY_CODE='{companyCode}' AND (PER_DAY_AMOUNT>0 OR PER_DAY_QUANTITY>0)";

            int totalRecord = _dbContext.SqlQuery<int>(TotalQuery).FirstOrDefault();
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
                //var isTableExist = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                //if(isTableExist.Count>0)
                //    result = isTableExist.Where(a => a.COMPANY_CODE == companyCode).ToList();
            }
            catch (Exception ex)
            {
                var createTableQry = $@"CREATE TABLE TEMP_PL_SALES_PLAN_REPORT AS SELECT DISTINCT 
                                        TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,
                                        SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
                                        TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR,   SPD.ITEM_CODE, IMS.ITEM_EDESC,    SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC, FS.BRANCH_CODE, SPD.COMPANY_CODE,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC  ,
                                   SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT, 
                                   SUM(SPD.PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                    SP.PLAN_EDESC
                                   --SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_GROUP_EDESC,
                                   -- SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_SUBGROUP_EDESC, 
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
                                   --AND SPD.PLAN_DATE >= TO_DATE('2017-Jul-16', 'YYYY-MON-DD')
                                   --AND SPD.PLAN_DATE <= TO_DATE('2018-Jul-15',' YYYY-MON-DD')
                                    AND IMS.GROUP_SKU_FLAG = 'I' 
                                    --AND SPD.COMPANY_CODE = '{companyCode}'
                                    GROUP BY  TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))),
                                    TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) ,
                                    SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2),
                                    SPD.ITEM_CODE, IMS.ITEM_EDESC,    SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC ,FS.BRANCH_CODE , SPD.COMPANY_CODE,SP.PLAN_EDESC";
                 var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if(result.Count()>0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            result[0].TotalRecord = totalRecord;
            return result;
        }

        public List<PlanReportModel> getMasterSalesPlanReport(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            var Query = $@"SELECT * FROM TEMP_PL_SALES_PLAN_REPORT SPD WHERE SPD.COMPANY_CODE='{companyCode}'";
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
                var createTableQry = $@"CREATE TABLE TEMP_PL_SALES_PLAN_REPORT AS SELECT DISTINCT 
                                        TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,
                                        SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
                                        TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR,   SPD.ITEM_CODE, IMS.ITEM_EDESC,    SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC, FS.BRANCH_CODE, SPD.COMPANY_CODE,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC  ,
                                   SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT, 
                                   SUM(SPD.PER_DAY_QUANTITY) PER_DAY_QUANTITY
                                   --SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_GROUP_EDESC,
                                   -- SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_SUBGROUP_EDESC, 
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
                                   --AND SPD.PLAN_DATE >= TO_DATE('2017-Jul-16', 'YYYY-MON-DD')
                                   --AND SPD.PLAN_DATE <= TO_DATE('2018-Jul-15',' YYYY-MON-DD')
                                    AND IMS.GROUP_SKU_FLAG = 'I' 
                                    --AND SPD.COMPANY_CODE = '{companyCode}'
                                    GROUP BY  TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))),
                                    TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) ,
                                    SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2),
                                    SPD.ITEM_CODE, IMS.ITEM_EDESC,    SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC ,FS.BRANCH_CODE , SPD.COMPANY_CODE";
                var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (result.Count() > 0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            return result;
        }
        public List<PlanReportModel> getMonthlySalesPlanChart(ReportFiltersModel model)
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
        public List<PlanReportModel> getEmployeeWiseSalesPlanChart(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT * FROM TEMP_EMPLOYEEWISE_SPC_REPORT SPD WHERE SPD.COMPANY_CODE='{companyCode}'";
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
                var createTableQry = $@"CREATE TABLE TEMP_EMPLOYEEWISE_SPC_REPORT AS SELECT DISTINCT SPD.EMPLOYEE_CODE,
                                   SPD.COMPANY_CODE,
                                   ES.EMPLOYEE_EDESC,  
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
                                    GROUP BY   ES.EMPLOYEE_EDESC,SPD.EMPLOYEE_CODE,
                                   ES.EMPLOYEE_EDESC, SPD.COMPANY_CODE";
                var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (result.Count() > 0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            return result;
        }
        public List<PlanReportModel> getDivisionWiseSalesPlanChart(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT * FROM TEMP_DIVISIONWISE_SPC_REPORT SPD WHERE SPD.COMPANY_CODE='{companyCode}'";
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
                var createTableQry = $@"CREATE TABLE TEMP_DIVISIONWISE_SPC_REPORT AS SELECT DISTINCT SPD.DIVISION_CODE,
                                     DS.DIVISION_EDESC,
                                   SPD.COMPANY_CODE,
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
                                    GROUP BY  SPD.DIVISION_CODE, DS.DIVISION_EDESC,SPD.COMPANY_CODE";
                var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (result.Count() > 0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            return result;
        }
        public List<PlanReportModel> getFavBrandingPlanReport(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            string Query =
                $@"SELECT DISTINCT TO_CHAR(SPD.PLAN_CODE) PLAN_CODE, SP.PLAN_EDESC, TO_CHAR(SPD.PLAN_DATE,'DD/MM/YYYY')PLAN_DATE,TO_CHAR(TO_BS(SPD.PLAN_DATE)) MITI ,
                                   SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT, SUM(SPD.PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                   SPD.ITEM_CODE, IMS.ITEM_EDESC, SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_GROUP_EDESC,
                                    SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_SUBGROUP_EDESC, SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC  
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
                                     AND SPD.PLAN_DATE >= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
                                    AND SPD.PLAN_DATE <= TO_DATE('{model.ToDate}',' YYYY-MON-DD')
                                    AND IMS.GROUP_SKU_FLAG = 'I'";

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
            Query += $@" GROUP BY SPD.PLAN_CODE, SPD.PLAN_DATE, SPD.ITEM_CODE, IMS.ITEM_EDESC, SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,TO_CHAR(TO_BS(SPD.PLAN_DATE)),
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC  ,SP.PLAN_EDESC ,SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE), 1, 30) ,
                                    SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE), 1, 30) ";

            var result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
            return result;
        }
        //public List<PlanReportModel> getFavLedgerPlanReport(ReportFiltersModel model)
        //{
        //    var companyCode = _workcontext.CurrentUserinformation.company_code;
        //    var branchCode = _workcontext.CurrentUserinformation.branch_code;
        //    string Query =
        //        $@"SELECT DISTINCT TO_CHAR(SPD.PLAN_DATE,'DD/MM/YYYY')PLAN_DATE,TO_CHAR(TO_BS(SPD.PLAN_DATE)) MITI , SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
        //                           SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT,TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR,
        //                           SPD.ACC_CODE ITEM_CODE, IMS.ACC_EDESC ITEM_EDESC, SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', IMS.PRE_ACC_CODE),1,30) ITEM_GROUP_EDESC,
        //                            SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', IMS.PRE_ACC_CODE),1,30) ITEM_SUBGROUP_EDESC, 
        //                           SPD.DIVISION_CODE,DS.DIVISION_EDESC, FS.BRANCH_EDESC  
        //                      FROM PL_COA_PLAN_DTL SPD,
        //                           FA_CHART_OF_ACCOUNTS_SETUP IMS,
        //                           FA_DIVISION_SETUP DS,
        //                           FA_BRANCH_SETUP FS
        //                           WHERE  SPD.DELETED_FLAG = 'N' 
        //                                                 AND DS.DIVISION_CODE (+)= SPD.DIVISION_CODE
        //                                                 AND FS.BRANCH_CODE (+)= SPD.BRANCH_CODE
        //                                                 AND IMS.ACC_CODE = SPD.ACC_CODE
        //                                                 AND SPD.COMPANY_CODE = IMS.COMPANY_CODE
        //                             AND SPD.PLAN_DATE >= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
        //                            AND SPD.PLAN_DATE <= TO_DATE('{model.ToDate}',' YYYY-MON-DD')
        //                            AND IMS.ACC_TYPE_FLAG = 'T'";

        //    if (model.CustomerFilter.Count > 0)
        //    {

        //        var customers = model.CustomerFilter;
        //        var customerConditionQuery = string.Empty;
        //        for (int i = 0; i < customers.Count; i++)
        //        {

        //            if (i == 0)
        //                customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
        //            else
        //            {
        //                customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
        //            }
        //        }

        //        Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
        //    }

        //    if (model.EmployeeFilter.Count > 0)
        //    {
        //        Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
        //    }
        //    if (model.DivisionFilter.Count > 0)
        //    {
        //        Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
        //    }

        //    if (model.BranchFilter.Count > 0)
        //    {
        //        Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
        //    }
        //    Query += $@"  GROUP BY SPD.PLAN_DATE, SPD.ACC_CODE, IMS.ACC_EDESC, fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2)) ,substr(bs_date(SPD.PLAN_DATE), 0, 4), SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) ,
        //                           SPD.DIVISION_CODE,  DS.DIVISION_EDESC,TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) ,TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)),
        //                            FS.BRANCH_EDESC  ,SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE, 'FA_CHART_OF_ACCOUNTS_SETUP', IMS.PRE_ACC_CODE), 1, 30) ,
        //                            SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE, 'FA_CHART_OF_ACCOUNTS_SETUP', IMS.PRE_ACC_CODE), 1, 30)";

        //    var result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
        //    return result;
        //}
        public List<PlanReportModel> getFavLedgerPlanReport(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT * FROM TEMP_PL_LB_PLAN_REPORT";
            var result = new List<PlanReportModel>();
            try
            {
                var isTableExist = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (isTableExist.Count > 0)
                    result = isTableExist.Where(a => a.COMPANY_CODE == companyCode).ToList();
            }
            catch (Exception ex)
            {
                var createTableQry = $@"CREATE TABLE TEMP_PL_LB_PLAN_REPORT AS
                                            SELECT CAS.ACC_CODE ITEM_CODE,CAS.ACC_EDESC ITEM_EDESC,PLD.COMPANY_CODE,
                                         DS.DIVISION_CODE,DS.DIVISION_EDESC,FS.BRANCH_CODE,FS.BRANCH_EDESC,
                                         TO_CHAR(fn_bs_month(substr(bs_date(PLD.PLAN_DATE), 6, 2))) MONTH,
                                         TO_CHAR(substr(bs_date(PLD.PLAN_DATE), 0, 4)) as YEAR, 
                                    SUM(PLD.PER_DAY_AMOUNT) PER_DAY_AMOUNT
                                    FROM PL_COA_PLAN_DTL PLD,
                                        FA_CHART_OF_ACCOUNTS_SETUP CAS,
                                        FA_DIVISION_SETUP DS,
                                        FA_BRANCH_SETUP FS 
                                   WHERE PLD.DELETED_FLAG='N' 
                                   AND  PLD.ACC_CODE = CAS.ACC_CODE
                                   AND PLD.COMPANY_CODE = CAS.COMPANY_CODE
                                   AND PLD.DIVISION_CODE=DS.DIVISION_CODE(+)          
                                   AND PLD.BRANCH_CODE=FS.BRANCH_CODE(+)
                                   AND CAS.ACC_TYPE_FLAG='T'
                                   GROUP BY  
                                   DS.DIVISION_CODE,DS.DIVISION_EDESC,FS.BRANCH_CODE,
                                   FS.BRANCH_EDESC,CAS.ACC_CODE,CAS.ACC_EDESC,PLD.COMPANY_CODE,
                                  TO_CHAR(fn_bs_month(substr(bs_date(PLD.PLAN_DATE), 6, 2))) ,
                                  TO_CHAR(substr(bs_date(PLD.PLAN_DATE), 0, 4))";
                var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (result.Count() > 0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            return result;
        }
        //public List<PlanReportModel> getFavBudgetPlanReport(ReportFiltersModel model)
        //{
        //    var companyCode = _workcontext.CurrentUserinformation.company_code;
        //    var branchCode = _workcontext.CurrentUserinformation.branch_code;
        //    string Query =
        //        $@"SELECT DISTINCT TO_CHAR(SPD.PLAN_DATE,'DD/MM/YYYY')PLAN_DATE,TO_CHAR(TO_BS(SPD.PLAN_DATE)) MITI , SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
        //                           SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT,TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR,
        //                           SPD.ACC_CODE ITEM_CODE, IMS.ACC_EDESC ITEM_EDESC, SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', IMS.PRE_ACC_CODE),1,30) ITEM_GROUP_EDESC,
        //                            SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'FA_CHART_OF_ACCOUNTS_SETUP', IMS.PRE_ACC_CODE),1,30) ITEM_SUBGROUP_EDESC, 
        //                           SPD.DIVISION_CODE,DS.DIVISION_EDESC, FS.BRANCH_EDESC  ,BCS.BUDGET_EDESC
        //                      FROM PL_COA_SUB_PLAN_DTL SPD,
        //                            BC_BUDGET_CENTER_SETUP BCS,
        //                           FA_CHART_OF_ACCOUNTS_SETUP IMS,
        //                           FA_DIVISION_SETUP DS,
        //                           FA_BRANCH_SETUP FS
        //                           WHERE  SPD.DELETED_FLAG = 'N' 
        //                                                 AND SPD.BUDGET_CODE = BCS.BUDGET_CODE
        //                                                 AND DS.DIVISION_CODE (+)= SPD.DIVISION_CODE
        //                                                 AND FS.BRANCH_CODE (+)= SPD.BRANCH_CODE
        //                                                 AND IMS.ACC_CODE = SPD.ACC_CODE
        //                                                 AND SPD.COMPANY_CODE = IMS.COMPANY_CODE
        //                                                 AND SPD.COMPANY_CODE = BCS.COMPANY_CODE
        //                                                 AND BCS.GROUP_SKU_FLAG = 'I'
        //                             AND SPD.PLAN_DATE >= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
        //                            AND SPD.PLAN_DATE <= TO_DATE('{model.ToDate}',' YYYY-MON-DD')
        //                            AND IMS.ACC_TYPE_FLAG = 'T'";

        //    if (model.CustomerFilter.Count > 0)
        //    {

        //        var customers = model.CustomerFilter;
        //        var customerConditionQuery = string.Empty;
        //        for (int i = 0; i < customers.Count; i++)
        //        {

        //            if (i == 0)
        //                customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
        //            else
        //            {
        //                customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
        //            }
        //        }

        //        Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
        //    }

        //    if (model.EmployeeFilter.Count > 0)
        //    {
        //        Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
        //    }
        //    if (model.DivisionFilter.Count > 0)
        //    {
        //        Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
        //    }

        //    if (model.BranchFilter.Count > 0)
        //    {
        //        Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
        //    }
        //    Query += $@"  GROUP BY SPD.PLAN_DATE, SPD.ACC_CODE, IMS.ACC_EDESC ,BCS.BUDGET_EDESC ,  fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2)) ,substr(bs_date(SPD.PLAN_DATE), 0, 4), SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) ,
        //                           SPD.DIVISION_CODE,  DS.DIVISION_EDESC,TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) ,TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)),
        //                            FS.BRANCH_EDESC  ,SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE, 'FA_CHART_OF_ACCOUNTS_SETUP', IMS.PRE_ACC_CODE), 1, 30) ,
        //                            SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE, 'FA_CHART_OF_ACCOUNTS_SETUP', IMS.PRE_ACC_CODE), 1, 30)   ";

        //    var result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
        //    return result;
        //}
        public List<PlanReportModel> getFavBudgetPlanReport(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT * FROM TEMP_PL_B_PLAN_REPORT";
            var result = new List<PlanReportModel>();
            try
            {
                var isTableExist = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (isTableExist.Count > 0)
                    result = isTableExist.Where(a => a.COMPANY_CODE == companyCode).ToList();
            }
            catch (Exception ex)
            {
                var createTableQry = $@"CREATE TABLE TEMP_PL_B_PLAN_REPORT AS
                                    SELECT CAS.ACC_CODE ITEM_CODE, CAS.ACC_EDESC ITEM_EDESC, PLD.COMPANY_CODE,
                                        DS.DIVISION_CODE, DS.DIVISION_EDESC, BCS.BUDGET_CODE,BCS.BUDGET_EDESC,
                                        TO_CHAR (fn_bs_month (SUBSTR (bs_date (PLD.PLAN_DATE), 6, 2))) MONTH,
                                        TO_CHAR (SUBSTR (bs_date (PLD.PLAN_DATE), 0, 4)) AS YEAR,
                                        SUM (PLD.PER_DAY_AMOUNT) PER_DAY_AMOUNT
                                       FROM PL_COA_SUB_PLAN_DTL PLD
                                                        INNER JOIN FA_CHART_OF_ACCOUNTS_SETUP CAS ON CAS.ACC_CODE = PLD.ACC_CODE AND CAS.COMPANY_CODE = PLD.COMPANY_CODE
                                                        INNER JOIN BC_BUDGET_CENTER_SETUP BCS ON BCS.BUDGET_CODE = PLD.BUDGET_CODE AND BCS.COMPANY_CODE = PLD.COMPANY_CODE
                                                        LEFT JOIN  FA_DIVISION_SETUP DS ON PLD.DIVISION_CODE =DS.DIVISION_CODE AND PLD.COMPANY_CODE=DS.COMPANY_CODE
                                        WHERE   PLD.DELETED_FLAG = 'N' AND CAS.ACC_TYPE_FLAG = 'T'  AND BCS.GROUP_SKU_FLAG='I' 
                                       GROUP BY DS.DIVISION_CODE, DS.DIVISION_EDESC, 
                                                     CAS.ACC_CODE, CAS.ACC_EDESC,PLD.COMPANY_CODE,BCS.BUDGET_CODE,BCS.BUDGET_EDESC,
                                                     TO_CHAR (fn_bs_month (SUBSTR (bs_date (PLD.PLAN_DATE), 6, 2))),
                                                      TO_CHAR (SUBSTR (bs_date (PLD.PLAN_DATE), 0, 4))";
                var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (result.Count() > 0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            return result;
        }
        //public List<PlanReportModel> getFavProcurementPlanReport(ReportFiltersModel model)
        //{
        //    var companyCode = _workcontext.CurrentUserinformation.company_code;
        //    var branchCode = _workcontext.CurrentUserinformation.branch_code;
        //    string Query =
        //        $@"SELECT DISTINCT  SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
        //                           SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT,
        //                           TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,
        //                           TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR,
        //                           SPD.ITEM_CODE, IMS.ITEM_EDESC, 
        //                            SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_GROUP_EDESC,
        //                            SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_SUBGROUP_EDESC, 
        //                           SPD.DIVISION_CODE,DS.DIVISION_EDESC, FS.BRANCH_EDESC  
        //                      FROM PL_BRD_PRCMT_PLAN_DTL SPD,
        //                           IP_ITEM_MASTER_SETUP IMS,
        //                           FA_DIVISION_SETUP DS,
        //                           FA_BRANCH_SETUP FS
        //                           WHERE  SPD.DELETED_FLAG = 'N' 
        //                                                 AND DS.DIVISION_CODE (+)= SPD.DIVISION_CODE
        //                                                 AND FS.BRANCH_CODE (+)= SPD.BRANCH_CODE
        //                                                 AND IMS.ITEM_CODE = SPD.ITEM_CODE
        //                                                 AND SPD.COMPANY_CODE = IMS.COMPANY_CODE
        //                             AND SPD.PLAN_DATE >= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
        //                            AND SPD.PLAN_DATE <= TO_DATE('{model.ToDate}',' YYYY-MON-DD')
        //                            AND IMS.GROUP_SKU_FLAG = 'I'";

        //    //if (model.CustomerFilter.Count > 0)
        //    //{

        //    //    var customers = model.CustomerFilter;
        //    //    var customerConditionQuery = string.Empty;
        //    //    for (int i = 0; i < customers.Count; i++)
        //    //    {

        //    //        if (i == 0)
        //    //            customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
        //    //        else
        //    //        {
        //    //            customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
        //    //        }
        //    //    }

        //    //    Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
        //    //}

        //    //if (model.EmployeeFilter.Count > 0)
        //    //{
        //    //    Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
        //    //}
        //    if (model.DivisionFilter.Count > 0)
        //    {
        //        Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
        //    }

        //    if (model.BranchFilter.Count > 0)
        //    {
        //        Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
        //    }
        //    Query += $@"  GROUP BY SPD.ITEM_CODE, IMS.ITEM_EDESC, 
        //                            SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2),
        //                            SPD.DIVISION_CODE,  DS.DIVISION_EDESC,
        //                            TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))),
        //                            TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)),
        //                            SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ,
        //                            SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ,
        //                            FS.BRANCH_EDESC ";

        //    var result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
        //    return result;
        //}
        public List<PlanReportModel> getFavProcurementPlanReport(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT * FROM TEMP_PL_PORCRUMENT_PLAN_REPORT SPD WHERE SPD.COMPANY_CODE='{companyCode}'";

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
                //if (isTableExist.Count > 0)
                //    result = isTableExist.Where(a => a.COMPANY_CODE == companyCode).ToList();
            }
            catch (Exception ex)
            {
                var createTableQry = $@"CREATE TABLE TEMP_PL_PORCRUMENT_PLAN_REPORT AS  SELECT DISTINCT 
                                        TO_CHAR(fn_bs_month(substr(bs_date(PPD.PLAN_DATE), 6, 2))) MONTH,
                                        TO_CHAR(substr(bs_date(PPD.PLAN_DATE), 0, 4)) as YEAR,   PPD.ITEM_CODE, IMS.ITEM_EDESC,                                
                                   SUM(PPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT,PPD.DIVISION_CODE,DS.DIVISION_EDESC, FS.BRANCH_EDESC, PPD.COMPANY_CODE  
                                   FROM PL_BRD_PRCMT_PLAN_DTL PPD,
                                   IP_ITEM_MASTER_SETUP IMS,
                                    PL_BRD_PRCMT_PLAN PL,
                                   FA_DIVISION_SETUP DS,
                                   FA_BRANCH_SETUP FS
                                      WHERE  PL.DELETED_FLAG = 'N' 
                                                         AND PL.PLAN_CODE = PPD.PLAN_CODE
                                                         AND DS.DIVISION_CODE (+)= PPD.DIVISION_CODE
                                                         AND FS.BRANCH_CODE (+)= PPD.BRANCH_CODE
                                                         AND IMS.ITEM_CODE = PPD.ITEM_CODE
                                                         AND PPD.COMPANY_CODE = IMS.COMPANY_CODE               
                                                         AND DS.COMPANY_CODE (+)=PPD.COMPANY_CODE
                                                         AND FS.COMPANY_CODE (+)=PPD.COMPANY_CODE
                                                             AND IMS.GROUP_SKU_FLAG = 'I' 
                                    GROUP BY  TO_CHAR(fn_bs_month(substr(bs_date(PPD.PLAN_DATE), 6, 2))),
                                    TO_CHAR(substr(bs_date(PPD.PLAN_DATE), 0, 4)) ,
                                    PPD.ITEM_CODE, IMS.ITEM_EDESC,  
                                   PPD.DIVISION_CODE, DS.DIVISION_EDESC,
                                   FS.BRANCH_EDESC,PPD.COMPANY_CODE";
                var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (result.Count() > 0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            return result;
        }
        //public List<PlanReportModel> getFavProductionPlanReport(ReportFiltersModel model)
        //{
        //    var companyCode = _workcontext.CurrentUserinformation.company_code;
        //    var branchCode = _workcontext.CurrentUserinformation.branch_code;
        //    string Query =
        //        $@"  SELECT DISTINCT SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
        //                           SUM(SPD.PER_DAY_QUANTITY) PER_DAY_QUANTITY,
        //                           TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,
        //                           TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR,SPD.ITEM_CODE, IMS.ITEM_EDESC, 
        //                            SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_GROUP_EDESC,
        //                            SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ITEM_SUBGROUP_EDESC, 
        //                           SPD.DIVISION_CODE,DS.DIVISION_EDESC, FS.BRANCH_EDESC  
        //                      FROM PL_PRO_PLAN_DTL SPD,
        //                           IP_ITEM_MASTER_SETUP IMS,
        //                           FA_DIVISION_SETUP DS,
        //                           FA_BRANCH_SETUP FS
        //                           WHERE  SPD.DELETED_FLAG = 'N' 
        //                                                 AND DS.DIVISION_CODE (+)= SPD.DIVISION_CODE
        //                                                 AND FS.BRANCH_CODE (+)= SPD.BRANCH_CODE
        //                                                 AND IMS.ITEM_CODE = SPD.ITEM_CODE
        //                                                 AND SPD.COMPANY_CODE = IMS.COMPANY_CODE
        //                             AND SPD.PLAN_DATE >= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
        //                            AND SPD.PLAN_DATE <= TO_DATE('{model.ToDate}',' YYYY-MON-DD')
        //                            AND IMS.GROUP_SKU_FLAG = 'I'";

        //    //if (model.CustomerFilter.Count > 0)
        //    //{

        //    //    var customers = model.CustomerFilter;
        //    //    var customerConditionQuery = string.Empty;
        //    //    for (int i = 0; i < customers.Count; i++)
        //    //    {

        //    //        if (i == 0)
        //    //            customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
        //    //        else
        //    //        {
        //    //            customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
        //    //        }
        //    //    }

        //    //    Query = Query + string.Format(@" AND SPD.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
        //    //}

        //    //if (model.EmployeeFilter.Count > 0)
        //    //{
        //    //    Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
        //    //}
        //    if (model.DivisionFilter.Count > 0)
        //    {
        //        Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
        //    }

        //    if (model.BranchFilter.Count > 0)
        //    {
        //        Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
        //    }
        //    Query += $@" GROUP BY  SPD.ITEM_CODE, IMS.ITEM_EDESC,  SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2), 
        //                           TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))),
        //                           TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)),
        //                           SUBSTR(FN_FETCH_GROUP_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ,
        //                           SUBSTR(FN_FETCH_PRE_DESC(SPD.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE),1,30) ,
        //                           SPD.DIVISION_CODE, DS.DIVISION_EDESC, FS.BRANCH_EDESC";

        //    var result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
        //    return result;
        //}
        public List<PlanReportModel> getFavProductionPlanReport(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var Query = $@"SELECT * FROM TEMP_PL_PRO_PLAN_REPORT SPD WHERE SPD.COMPANY_CODE='{companyCode}'";

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
                //if (isTableExist.Count > 0)
                //    result = isTableExist.Where(a => a.COMPANY_CODE == companyCode).ToList();
            }
            catch (Exception ex)
            {
                var createTableQry = $@"CREATE TABLE TEMP_PL_PRO_PLAN_REPORT AS SELECT DISTINCT 
                                        TO_CHAR(fn_bs_month(substr(bs_date(PLD.PLAN_DATE), 6, 2))) MONTH,
                                        TO_CHAR(substr(bs_date(PLD.PLAN_DATE), 0, 4)) as YEAR,   PLD.ITEM_CODE, IMS.ITEM_EDESC,  
                                   PLD.DIVISION_CODE,  DS.DIVISION_EDESC, FS.BRANCH_CODE, PLD.COMPANY_CODE,
                                   FS.BRANCH_EDESC ,
                                   SUM(PLD.PER_DAY_QUANTITY) PER_DAY_QUANTITY
                              FROM  PL_PRO_PLAN_DTL PLD,
                                   IP_ITEM_MASTER_SETUP IMS,
                                  PL_PRO_PLAN PL,
                                   FA_DIVISION_SETUP DS,
                                   FA_BRANCH_SETUP FS
                                   WHERE  PL.DELETED_FLAG = 'N' 
                                                         AND PL.PLAN_CODE = PLD.PLAN_CODE
                                                         AND DS.DIVISION_CODE (+)= PLD.DIVISION_CODE
                                                         AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
                                                         AND IMS.ITEM_CODE = PLD.ITEM_CODE
                                                         AND PLD.COMPANY_CODE = IMS.COMPANY_CODE
                                                         AND DS.COMPANY_CODE (+)=PLD.COMPANY_CODE
                                                         AND FS.COMPANY_CODE (+)=PLD.COMPANY_CODE
                                    AND IMS.GROUP_SKU_FLAG = 'I' 
                                    GROUP BY  TO_CHAR(fn_bs_month(substr(bs_date(PLD.PLAN_DATE), 6, 2))),
                                    TO_CHAR(substr(bs_date(PLD.PLAN_DATE), 0, 4)) ,
                                    PLD.ITEM_CODE, IMS.ITEM_EDESC,    
                                   PLD.DIVISION_CODE,DS.DIVISION_EDESC,FS.BRANCH_CODE,
                                   FS.BRANCH_EDESC , PLD.COMPANY_CODE";
                var response = this._dbContext.ExecuteSqlCommand(createTableQry);
                result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                if (result.Count() > 0)
                    result = result.Where(x => x.COMPANY_CODE == companyCode).ToList();
            }
            return result;
        }
        public List<PlanReportModel> getFavMaterialPlanReport(ReportFiltersModel model)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            try
            {
                var query = $@"SELECT  ITEM_CODE,ITEM_EDESC,MATERIAL_QUANTITY,PO_PENDING,STOCK,
                                  NVL(MATERIAL_QUANTITY-NVL((PO_PENDING+STOCK),0),0)PO FROM 
                                  (  SELECT ITEM_CODE,ITEM_EDESC,MATERIAL_QUANTITY,REQUIRED_QUANTITY,
                             (SELECT NVL(SUM(NVL(QUANTITY,0)),0)PO_PENDING FROM IP_PURCHASE_ORDER WHERE ITEM_CODE = t.ITEM_CODE AND COMPANY_CODE =t.COMPANY_CODE AND DELETED_FLAG='N'  
                                  AND ORDER_NO NOT IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL WHERE COMPANY_CODE = t.COMPANY_CODE AND DELETED_FLAG = 'N'))PO_PENDING,
                                (SELECT NVL(SUM(NVL(IN_QUANTITY,0))-SUM(NVL(OUT_QUANTITY,0)),0)STOCK FROM V$VIRTUAL_STOCK_WIP_LEDGER WHERE ITEM_CODE = t.ITEM_CODE 
                                AND COMPANY_CODE = t.COMPANY_CODE AND DELETED_FLAG='N')STOCK
                                 FROM (
                              SELECT DISTINCT  MPD.ITEM_CODE,IMS.ITEM_EDESC,MPD.COMPANY_CODE,
                               SUM(MPD.MATERIAL_QUANTITY) MATERIAL_QUANTITY,
                              SUM(MPD.CALC_QUANTITY)REQUIRED_QUANTITY 
                              FROM PL_MATERIAL_PLAN MP 
                              INNER JOIN PL_MATERIAL_PLAN_DTL MPD ON MPD.PLAN_CODE = MP.PLAN_CODE
                              INNER JOIN IP_ITEM_MASTER_SETUP IMS ON IMS.ITEM_CODE = MPD.ITEM_CODE
                              GROUP BY  MPD.ITEM_CODE,IMS.ITEM_EDESC, MPD.COMPANY_CODE)t)";
                var result = this._dbContext.SqlQuery<PlanReportModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string CreateTempSalesChartPlanReportTable(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var response = "";
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
            try
            {
                var tableExistsQry = $@"SELECT * FROM TEMP_SALES_PLAN_CHART_REPORT";
                var result = this._dbContext.SqlQuery<ProductionPlan>(tableExistsQry).ToList();
                if (result != null)
                {
                    var dropQry = $@"DROP TABLE TEMP_SALES_PLAN_CHART_REPORT";
                    var dropResponse = this._dbContext.ExecuteSqlCommand(dropQry);
                    var createResponse = this._dbContext.ExecuteSqlCommand(createTableQry);
                    response = "TableDropedAndCreated";
                }
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("table or view does not exist"))
                {
                    var result = this._dbContext.ExecuteSqlCommand(createTableQry);
                    response = "TableCreated";
                }
                else
                {
                    var dropQry = $@"DROP TABLE TEMP_SALES_PLAN_CHART_REPORT";
                    var dropResponse = this._dbContext.ExecuteSqlCommand(dropQry);
                    response = ex.Message;
                }
            }
            return response;
        }
        public List<PlanReportModel> getMonthlySalesPlanByItemGroup(ReportFiltersModel filters, string groupItemCode)
        {
            var result= new List<PlanReportModel>();
            if (string.IsNullOrEmpty(groupItemCode) || groupItemCode == "0")
            {
                return result;
            }
            else
            {
                var companyCode = _workcontext.CurrentUserinformation.company_code;
                var branchCode = _workcontext.CurrentUserinformation.branch_code;

                var userId = _workcontext.CurrentUserinformation.User_id;
                var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
                var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
                var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
                var loginEmpCode = _dbContext.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

                var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  MONTH,MONTHINT,YEAR,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                    WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) AND ITEM_CODE IN
                                    (SELECT Distinct ITEM_CODE
                                    FROM IP_ITEM_MASTER_SETUP ims
                                    WHERE ims.DELETED_FLAG = 'N' and group_sku_flag='I'
                                    AND ims.COMPANY_CODE = '{companyCode}' 
                                    START WITH ITEM_CODE = '{groupItemCode}' AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'
                                    CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'
                                    ) ";

                // added by chandra for load  only related report of logined person if not admin
                if (superFlag != "Y" && filters.EmployeeFilter.Count == 0)
                {
                    Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    ) ";
                }

                if (filters.CustomerFilter.Count > 0)
                {
                    var customers = filters.CustomerFilter;
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

                if (filters.EmployeeFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
                }
                if (filters.DivisionFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
                }

                if (filters.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
                }
                Query += $@" GROUP BY MONTH,MONTHINT,YEAR,COMPANY_CODE";
                try
                {
                    result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //return result;
            }
            return result;
        }
        public List<PlanReportModel> getMonthlySalesPlanCompareByItemGroup(ReportFiltersModel filters,string groupItemCode)
        {
            var result = new List<PlanReportModel>();
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _dbContext.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();
            var itemGroup = GetItemGroup();
            foreach(var item in itemGroup)
            {
                var res= new List<PlanReportModel>();
                var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                MONTH,MONTHINT,YEAR,COMPANY_CODE,'{item.ITEM_EDESC}' PLAN_EDESC
                                FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) AND ITEM_CODE IN
                                (SELECT Distinct ITEM_CODE
                                FROM IP_ITEM_MASTER_SETUP ims
                                WHERE ims.DELETED_FLAG = 'N' and group_sku_flag='I'
                                AND ims.COMPANY_CODE = '{companyCode}' 
                                START WITH ITEM_CODE = '{item.ITEM_CODE}' AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'
                                CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'
                                ) ";

                // added by chandra for load  only related report of logined person if not admin
                if (superFlag != "Y" && filters.EmployeeFilter.Count == 0)
                {
                    Query += $@" AND SPD.EMPLOYEE_CODE IN(
                SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                    UNION ALL
                SELECT EMPLOYEE_CODE FROM(
                SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                    START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                ) ";
                }

                if (filters.CustomerFilter.Count > 0)
                {
                    var customers = filters.CustomerFilter;
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

                if (filters.EmployeeFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
                }
                if (filters.DivisionFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
                }

                if (filters.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
                }
                Query += $@" GROUP BY MONTH,MONTHINT,YEAR,COMPANY_CODE";
                try
                {
                    res = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                    result.AddRange(res);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
            return result;
        }
        public List<PlanReportModel> getEmpwiseSalesPlanByItemGroup(ReportFiltersModel filters, string groupItemCode)
        {
            var result = new List<PlanReportModel>();
            if (string.IsNullOrEmpty(groupItemCode) || groupItemCode=="0")
            {
                return result;
            }
            else
            {
                var companyCode = _workcontext.CurrentUserinformation.company_code;
                var branchCode = _workcontext.CurrentUserinformation.branch_code;

                var userId = _workcontext.CurrentUserinformation.User_id;
                var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
                var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
                var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
                var loginEmpCode = _dbContext.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();

                var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                  SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                  EMPLOYEE_CODE,EMPLOYEE_EDESC,COMPANY_CODE
                                  FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                    WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) AND ITEM_CODE IN
                                    (SELECT Distinct ITEM_CODE
                                    FROM IP_ITEM_MASTER_SETUP ims
                                    WHERE ims.DELETED_FLAG = 'N' and group_sku_flag='I'
                                    AND ims.COMPANY_CODE = '{companyCode}' 
                                    START WITH ITEM_CODE = '{groupItemCode}' AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'
                                    CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'
                                    ) ";

                // added by chandra for load  only related report of logined person if not admin
                if (superFlag != "Y" && filters.EmployeeFilter.Count == 0)
                {
                    Query += $@" AND SPD.EMPLOYEE_CODE IN(
                    SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                     START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                    ) ";
                }

                if (filters.CustomerFilter.Count > 0)
                {
                    var customers = filters.CustomerFilter;
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

                if (filters.EmployeeFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
                }
                if (filters.DivisionFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
                }

                if (filters.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
                }
                Query += $@" GROUP BY EMPLOYEE_CODE,EMPLOYEE_EDESC,COMPANY_CODE";
                try
                {
                    result = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //return result;
            }
            return result;
        }
        public List<PlanReportModel> getEmpwiseSalesPlanCompareByItemGroup(ReportFiltersModel filters, string groupItemCode)
        {
            var result = new List<PlanReportModel>();
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;

            var userId = _workcontext.CurrentUserinformation.User_id;
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{companyCode}'";
            var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCodeQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'";
            var loginEmpCode = _dbContext.SqlQuery<string>(loginEmpCodeQuery).FirstOrDefault();
            var itemGroup = GetItemGroup();
            foreach (var item in itemGroup)
            {
                var res = new List<PlanReportModel>();
                var Query = $@"SELECT SUM(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                                SUM(PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                EMPLOYEE_CODE,EMPLOYEE_EDESC,COMPANY_CODE,'{item.ITEM_EDESC}' PLAN_EDESC
                                FROM TEMP_PL_SALES_PLAN_REPORT SPD
                                WHERE SPD.COMPANY_CODE='{companyCode}' AND (SPD.PER_DAY_AMOUNT>0 OR SPD.PER_DAY_QUANTITY>0) AND ITEM_CODE IN
                                (SELECT Distinct ITEM_CODE
                                FROM IP_ITEM_MASTER_SETUP ims
                                WHERE ims.DELETED_FLAG = 'N' and group_sku_flag='I'
                                AND ims.COMPANY_CODE = '{companyCode}' 
                                START WITH ITEM_CODE = '{item.ITEM_CODE}' AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'
                                CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N'
                                ) ";

                // added by chandra for load  only related report of logined person if not admin
                if (superFlag != "Y" && filters.EmployeeFilter.Count == 0)
                {
                    Query += $@" AND SPD.EMPLOYEE_CODE IN(
                SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{companyCode}'
                    UNION ALL
                SELECT EMPLOYEE_CODE FROM(
                SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                    START WITH PARENT_EMPLOYEE_CODE='{loginEmpCode}' )X WHERE X.COMPANY_CODE='{companyCode}'
                ) ";
                }

                if (filters.CustomerFilter.Count > 0)
                {
                    var customers = filters.CustomerFilter;
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

                if (filters.EmployeeFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND SPD.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
                }
                if (filters.DivisionFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND SPD.DIVISION_CODE IN ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
                }

                if (filters.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND SPD.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
                }
                Query += $@" GROUP BY EMPLOYEE_CODE,EMPLOYEE_EDESC,COMPANY_CODE";
                try
                {
                    res = _dbContext.SqlQuery<PlanReportModel>(Query).ToList();
                    result.AddRange(res);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }
        public List<ItemGroupModel> GetItemGroup()
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var query = $"SELECT ITEM_CODE,ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE PRE_ITEM_CODE='00' AND COMPANY_CODE='{companyCode}' AND DELETED_FLAG='N' ORDER BY ITEM_EDESC";
            var result= _dbContext.SqlQuery<ItemGroupModel>(query).ToList();
            return result;
        }
    }
}
