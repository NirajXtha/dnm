using NeoErp.Planning.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Planning.Service.Models;
using NeoErp.Data;
using NeoErp.Core;
using NeoErp.Core.Domain;
using System.Web;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace NeoErp.Planning.Service.Repository
{
    public class SubPlanRepo : ISubPlanRepo
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public SubPlanRepo(IDbContext dbContext, IWorkContext _iWorkContext)
        {
            this._workcontext = _iWorkContext;
            this._dbContext = dbContext;
        }


        public List<PlanModels> getSubPlanTimeFrame(string PLAN_CODE)
        {
            string query = $@"SELECT DISTINCT PPD.TIME_FRAME_CODE,PTF.TIME_FRAME_EDESC AS TIME_FRAME,
                PTF.TIME_FRAME_EDESC || ' ' || PPD.TIME_FRAME_VALUE AS TIME_FRAME_EDESC ,
                PPD.TIME_FRAME_VALUE
                FROM PL_PLAN_DTL PPD
                JOIN PL_TIME_FRAME PTF ON PPD.TIME_FRAME_CODE = PTF.TIME_FRAME_CODE WHERE PPD.PLAN_CODE = '{PLAN_CODE}'
                order by PPD.TIME_FRAME_VALUE";
            var timeframes = _dbContext.SqlQuery<PlanModels>(query).ToList();
            return timeframes;
        }

        public List<PlanModels> getPlanList(string filter)
        {
            try
            {
                //if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                //                var sqlquery = string.Format(@"SELECT 
                //                        DISTINCT PP.PLAN_CODE,
                //                    --PP.ITEM_CODE,
                //                    PP.PLAN_EDESC ||' between '||TO_CHAR(PP.START_DATE,'DD-MON-YYYY') ||' to '||TO_CHAR(PP.END_DATE,'DD-MON-YYYY') AS ITEM_EDESC
                //                    FROM PL_PLAN PP
                //                    --JOIN IP_ITEM_MASTER_SETUP IMS ON PP.ITEM_CODE = IMS.ITEM_CODE
                //                    where 
                //                        PP.deleted_flag='N' and (PP.PLAN_CODE LIKE '%{0}%'
                //                        --OR UPPER(IMS.ITEM_EDESC) LIKE '%{0}%'
                //)", filter.ToUpperInvariant());

                string sqlquery = string.Empty;
                if (string.IsNullOrEmpty(filter))
                {
                    sqlquery = $@"SELECT DISTINCT PP.PLAN_CODE,PP.ITEM_CODE,PP.PLAN_EDESC ||' between '|| PP.START_DATE ||' to '||PP.END_DATE AS ITEM_EDESC ,PP.TIME_FRAME_CODE, PP.ITEM_PRE_CODE AS ITEM_PRE_CODE 
                    ,pp.created_date
                    FROM  PL_PLAN PP
                    inner join pl_plan_dtl ppd
                    on pp.plan_code = ppd.plan_code
                    where PP.deleted_flag='N' 
                    order by pp.created_date desc";
                }
                else
                {
                    sqlquery = $@"SELECT DISTINCT PP.PLAN_CODE,PP.ITEM_CODE,PP.PLAN_EDESC ||' between '|| PP.START_DATE ||' to '||PP.END_DATE AS ITEM_EDESC ,PP.TIME_FRAME_CODE, PP.ITEM_PRE_CODE AS ITEM_PRE_CODE 
                    ,pp.created_date
                    FROM  PL_PLAN PP
                    inner join pl_plan_dtl ppd
                    on pp.plan_code = ppd.plan_code
                    where PP.deleted_flag='N' and (PP.PLAN_CODE LIKE '{filter}') 
                    order by pp.created_date desc";
                }


                var result = _dbContext.SqlQuery<PlanModels>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public PlanModels getPlan(string plancode)
        {
            try
            {
                PlanModels obj = new PlanModels();
                //string query = $@"select * from pl_plan
                //                 where plan_code = {plancode}";
                //string query = $@"select pp.plan_code,pp.plan_edesc,pp.plan_ndesc,pp.start_date,pp.end_date,pp.Item_code,pp.item_pre_code,pp.item_master_code,pp.plan_type,pp.plan_for,
                //                ptf.time_frame_edesc,ptf.time_frame_type,ptf.days ,
                //                (select to_char((trunc(TO_DATE(end_date, 'DD/MM/YYYY'), 'd') - trunc(TO_DATE(start_date,
                //                 'DD/MM/YYYY'), 'd')) / 7)  countinous_weeks from pl_plan where plan_code='{plancode}') frequency
                //                from pl_plan pp,pl_time_frame ptf
                //                where 
                //                pp.deleted_flag ='N'
                //                and ptf.deleted_flag='N'
                //                and ptf.time_frame_code=pp.time_frame_code
                //                and plan_code = '{plancode}'";
                string query = string.Empty;
                if (string.IsNullOrEmpty(plancode))
                {
                    query = $@"SELECT DISTINCT PP.PLAN_CODE,PP.ITEM_CODE,PP.PLAN_EDESC ||' between '|| PP.START_DATE ||' to '||PP.END_DATE AS ITEM_EDESC ,PP.TIME_FRAME_CODE, PP.ITEM_PRE_CODE AS ITEM_PRE_CODE 
                    ,pp.created_date
                    FROM  PL_PLAN PP
                    inner join pl_plan_dtl ppd
                    on pp.plan_code = ppd.plan_code
                    where PP.deleted_flag='N' 
                    order by pp.created_date desc";
                }
                else
                {
                    query = $@"SELECT DISTINCT PP.PLAN_CODE,PP.ITEM_CODE,PP.PLAN_EDESC ||' between '|| PP.START_DATE ||' to '||PP.END_DATE AS ITEM_EDESC ,PP.TIME_FRAME_CODE, PP.ITEM_PRE_CODE AS ITEM_PRE_CODE 
                    ,pp.created_date
                    FROM  PL_PLAN PP
                    inner join pl_plan_dtl ppd
                    on pp.plan_code = ppd.plan_code
                    where PP.deleted_flag='N' and (PP.PLAN_CODE LIKE '{plancode}') 
                    order by pp.created_date desc";
                }
                obj = this._dbContext.SqlQuery<PlanModels>(query).FirstOrDefault();
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetFrequencyColumnByPlanCode(string plancode)
        {
            try
            {
                string columnNumber = string.Empty;
                string query = $@"select to_char((trunc(TO_DATE(end_date, 'DD/MM/YYYY'), 'd') - trunc(TO_DATE(start_date,
                                    'DD/MM/YYYY'), 'd')) / 7)  countinous_weeks from pl_plan where plan_code = {plancode}";
                columnNumber = this._dbContext.SqlQuery<string>(query).FirstOrDefault().ToString();
                return columnNumber;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SubPlanModels> getTargetValue(SubPlanModels details)
        {
            var timeframe = string.Join("", details.TIME_FRAME_EDESC.ToCharArray().Where(Char.IsDigit));
            string query = string.Format(@"SELECT PPD.PLAN_CODE,PPD.ITEM_CODE,PP.PLAN_EDESC,PP.ITEM_MASTER_CODE,PP.ITEM_PRE_CODE,PPD.TIME_FRAME_CODE,PPD.TIME_FRAME_VALUE,PPD.TARGET_VALUE
                    FROM PL_PLAN_DTL PPD
                    JOIN PL_PLAN PP ON PPD.PLAN_CODE = PP.PLAN_CODE
                     WHERE PPD.DELETED_FLAG = 'N' AND PPD.PLAN_CODE = '" + details.PLAN_CODE + "' AND PPD.TIME_FRAME_CODE = '" + details.TIME_FRAME_CODE + "' AND PPD.TIME_FRAME_VALUE = '" + timeframe + "'");
            var result = _dbContext.SqlQuery<SubPlanModels>(query).ToList();
            return result;
        }

        public List<SubPlanModels> ViewSubPlanReport(SubPlanModels viewdetails)
        {
            throw new NotImplementedException();
        }

        public string SaveSubPlan(string plancode, string SubPlanName)
        {
            var nextValQuery = $@"SELECT PL_SUBPLAN_SEQ.nextval FROM DUAL";
            var id = _dbContext.SqlQuery<int>(nextValQuery).FirstOrDefault();
            var insertQuery = string.Format(@"INSERT INTO PL_SUBPLAN(SUBPLAN_CODE,PLAN_DTL_CODE,SUBPLAN_EDESC,SUBPLAN_NDESC,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,PLAN_CODE)
                VALUES('{0}', '{1}', '{2}','{3}','{4}', '{5}','{6}',TO_DATE('{7}', 'mm/dd/yyyy'),'{8}','{9}')", id, "", SubPlanName, SubPlanName, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.login_code, DateTime.Now.ToString("MM/dd/yyyy"), "N", plancode);
            var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
            _dbContext.SaveChanges();

            return id.ToString();
        }



        public List<SubPlanModels> getPlanDetailCode(string PlanCode)
        {
            string query = @"SELECT PLAN_CODE,PLAN_DTL_CODE,TIME_FRAME_CODE,TIME_FRAME_VALUE,TARGET_VALUE,ITEM_CODE FROM PL_PLAN_DTL WHERE PLAN_CODE = '" + PlanCode + "' ";
            var plandetailcode = _dbContext.SqlQuery<SubPlanModels>(query).ToList();
            return plandetailcode;
        }

        public string SaveSubGroupWiseSubPlan(string subPlanCode, string itemCode, string preItemCode, string masterItemCode, string Code, string preCode, string masterCode, string targetValue, DateTime dateRange, string subgrouptype)
        {
            if (subgrouptype == "CUSTOMER")
            {
                var insertQuery = string.Format(@"INSERT INTO PL_CUSTOMER_PLAN(SUBPLAN_CODE,CUSTOMER_CODE,CUSTOMER_PRE_CODE,CUSTOMER_MASTER_CODE,ITEM_CODE,ITEM_PRE_CODE,ITEM_MASTER_CODE,PLAN_DATE,TARGET_VALUE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                VALUES('{0}', '{1}', '{2}','{3}','{4}', '{5}','{6}',TO_DATE('{7}', 'mm/dd/yyyy'),'{8}','{9}','{10}','{11}',TO_DATE('{12}', 'mm/dd/yyyy'),'{13}')", subPlanCode, Code, preCode, masterCode, itemCode, preItemCode, masterItemCode, dateRange.ToString("MM/dd/yyyy"), targetValue, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.login_code, DateTime.Now.ToString("MM/dd/yyyy"), "N");
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
                return "message";
            }
            else if (subgrouptype == "DIVISION")
            {
                var insertQuery = string.Format(@"INSERT INTO PL_DIVISION_PLAN(SUBPLAN_CODE,DIVISION_CODE,ITEM_CODE,ITEM_PRE_CODE,ITEM_MASTER_CODE,PLAN_DATE,TARGET_VALUE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                VALUES('{0}', '{1}','{2}', '{3}','{4}',TO_DATE('{5}', 'mm/dd/yyyy'),'{6}','{7}','{8}','{9}',TO_DATE('{10}', 'mm/dd/yyyy'),'{11}')", subPlanCode, Code, itemCode, preItemCode, masterItemCode, dateRange.ToString("MM/dd/yyyy"), targetValue, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.login_code, DateTime.Now.ToString("MM/dd/yyyy"), "N");
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
                return "message";
            }
            else if (subgrouptype == "EMPLOYEE")
            {
                var insertQuery = string.Format(@"INSERT INTO PL_EMPLOYEE_PLAN(SUBPLAN_CODE,EMPLOYEE_CODE,PRE_EMPLOYEE_CODE,MASTER_EMPLOYEE_CODE,ITEM_CODE,ITEM_PRE_CODE,ITEM_MASTER_CODE,PLAN_DATE,TARGET_VALUE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                VALUES('{0}', '{1}', '{2}','{3}','{4}', '{5}','{6}',TO_DATE('{7}', 'mm/dd/yyyy'),'{8}','{9}','{10}','{11}',TO_DATE('{12}', 'mm/dd/yyyy'),'{13}')", subPlanCode, Code, preCode, masterCode, itemCode, preItemCode, masterItemCode, dateRange.ToString("MM/dd/yyyy"), targetValue, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.login_code, DateTime.Now.ToString("MM/dd/yyyy"), "N");
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
                return "message";
            }
            return "";
        }

        public List<DateRangeModels> dateRange(string startDate, string endDate)
        {
            string query = @"select to_date('" + startDate + "','dd-MM-YY') +rownum - 1 AS DATE_RANGES FROM ALL_OBJECTS WHERE ROWNUM <= TO_DATE('" + endDate + "', 'dd-mon-yy') - TO_DATE('" + startDate + "', 'dd-mon-yy') + 1 ";
            var dateList = _dbContext.SqlQuery<DateRangeModels>(query).ToList();
            return dateList;
        }

        public List<SubPlanModels> getSubPlans(string plancode)
        {
            string query = $@"SELECT DISTINCT PLAN_CODE, SUBPLAN_EDESC FROM PL_SUBPLAN WHERE PLAN_CODE = '{plancode}'";
            List<SubPlanModels> subplanList = this._dbContext.SqlQuery<SubPlanModels>(query).OrderBy(a => a.PLAN_CODE).ToList();
            return subplanList;
            //string query = $@"select distinct PD.PLAN_CODE,SP.SUBPLAN_EDESC 
            //                    from pl_subplan sp,pl_plan_dtl pd
            //                    where SP.PLAN_DTL_CODE=PD.PLAN_DTL_CODE
            //                    and PD.PLAN_CODE='{plancode}'";
            //List<SubPlanModels> subplanList = this._dbContext.SqlQuery<SubPlanModels>(query).OrderBy(a => a.PLAN_CODE).ToList();
            //return subplanList;
        }

        public List<AccountSetup> getAllAccounts()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ACC_EDESC) AS ACC_EDESC,
                        ACC_CODE
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var accountList = _dbContext.SqlQuery<AccountSetup>(query).ToList();
            return accountList;
        }
        public List<CustomerModels> getAllCustomer()
        {
            string query = @"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        CUSTOMER_CODE ,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG 
                        FROM SA_CUSTOMER_SETUP
                        WHERE DELETED_FLAG = 'N'
                        CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
                        ORDER BY PRE_CUSTOMER_CODE";
            var customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();
            return customerList;
        }

        public List<EmployeeModels> getAllEmployees()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(EMPLOYEE_EDESC) AS EMPLOYEE_EDESC,
                        EMPLOYEE_CODE ,
                        MASTER_EMPLOYEE_CODE, 
                        PRE_EMPLOYEE_CODE,
                        GROUP_SKU_FLAG 
                        FROM HR_EMPLOYEE_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        CONNECT BY PRIOR MASTER_EMPLOYEE_CODE = PRE_EMPLOYEE_CODE
                        ORDER BY PRE_EMPLOYEE_CODE";
            var employeeList = _dbContext.SqlQuery<EmployeeModels>(query).ToList();
            return employeeList;
        }

        public List<DivisionModels> getAllDivisions()
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            string query = $@"SELECT DISTINCT 
                        INITCAP(DIVISION_EDESC) AS DIVISION_EDESC,
                        DIVISION_CODE , 
                        PRE_DIVISION_CODE,
                        GROUP_SKU_FLAG 
                        FROM FA_DIVISION_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{company_code}' 
                        CONNECT BY PRIOR DIVISION_CODE = PRE_DIVISION_CODE
                        ORDER BY PRE_DIVISION_CODE";
            var divisionList = _dbContext.SqlQuery<DivisionModels>(query).ToList();
            return divisionList;
        }

        public List<DateRangeModels> getAllDynamicColumnHeader(string plancode)
        {
            string query = $@"SELECT TO_CHAR((RD.START_DATE) - 1 + ROWNUM) AS DATES,TO_CHAR(EXTRACT(YEAR FROM ((RD.START_DATE) - 1 + ROWNUM))) AS YEAR,TO_CHAR(EXTRACT(MONTH FROM ((RD.START_DATE) - 1 + ROWNUM))) AS MONTH,TO_CHAR(((RD.START_DATE) - 1 + ROWNUM),'MONTH') AS MONTH_NAME,to_char(((RD.START_DATE) - 1 + ROWNUM),'WW') AS WEEK,TO_CHAR(EXTRACT(DAY FROM ((RD.START_DATE) - 1 + ROWNUM))) AS DAY FROM ALL_OBJECTS O, PL_PLAN RD WHERE TO_DATE(RD.START_DATE) - 1 + ROWNUM <= TO_DATE(RD.END_DATE) AND RD.PLAN_CODE={plancode} ORDER BY ((RD.START_DATE) - 1 + ROWNUM)";
            var daterangelist = _dbContext.SqlQuery<DateRangeModels>(query).ToList();
            return daterangelist;
        }

        public List<CustomerModels> getAllCustomer(string filter)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            string condition = string.Empty;
            if (!String.IsNullOrEmpty(filter))
            {
                condition = $@"AND ( LOWER(CUSTOMER_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(CUSTOMER_CODE) LIKE '%{filter.ToLower()}%')";
            }
            string query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        CUSTOMER_CODE ,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG 
                        FROM SA_CUSTOMER_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{company_code}' AND GROUP_SKU_FLAG='I' {condition}
                        CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
                        ORDER BY PRE_CUSTOMER_CODE";
            var customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();
            return customerList;
        }

        public List<CustomerModels> getAllCustomerForPlan(string filter)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            string condition = string.Empty;
            if (!String.IsNullOrEmpty(filter))
            {
                condition = $@"AND ( LOWER(CUSTOMER_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(CUSTOMER_CODE) LIKE '%{filter.ToLower()}%')";
            }
            string query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        CUSTOMER_CODE ,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG 
                        FROM SA_CUSTOMER_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{company_code}' AND GROUP_SKU_FLAG='I' {condition}
                        --CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
                        ORDER BY PRE_CUSTOMER_CODE";
            var customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();
            return customerList;
        }

        public List<DivisionModels> getAllDivision(string filter)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            string condition = string.Empty;
            if (!String.IsNullOrEmpty(filter))
            {
                condition = $@"AND ( LOWER(DIVISION_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(DIVISION_CODE) LIKE '%{filter.ToLower()}%')";
            }
            string query = $@"SELECT DISTINCT 
                        INITCAP(DIVISION_EDESC) AS DIVISION_EDESC,
                        DIVISION_CODE ,
                        PRE_DIVISION_CODE,
                        GROUP_SKU_FLAG 
                        FROM FA_DIVISION_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{company_code}' AND GROUP_SKU_FLAG='I' {condition}
                        ORDER BY DIVISION_EDESC";
            var customerList = _dbContext.SqlQuery<DivisionModels>(query).ToList();
            return customerList;
        }

        public List<AgentModels> getAllAgent(string filter)
        {
            try
            {
                var info = this._workcontext.CurrentUserinformation;
                string condition = string.Empty;
                if (!String.IsNullOrEmpty(filter))
                {
                    condition = $@"AND ( LOWER(AGENT_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(AGENT_CODE) LIKE '%{filter.ToLower()}%')";
                }
                string query = $@"SELECT DISTINCT 
                        INITCAP(AGENT_EDESC) AS AGENT_EDESC,
                        AGENT_CODE 
                        FROM AGENT_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{info.company_code}' {condition}
                        ORDER BY AGENT_EDESC";
                var agentList = _dbContext.SqlQuery<AgentModels>(query).ToList();
                return agentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BranchModels> getAllBranch(string filter)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            string condition = string.Empty;
            if (!String.IsNullOrEmpty(filter))
            {
                condition = $@"AND ( LOWER(BRANCH_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(BRANCH_CODE) LIKE '%{filter.ToLower()}%')";
            }
            string query = $@"SELECT DISTINCT 
                        INITCAP(BRANCH_EDESC) AS BRANCH_EDESC,
                        BRANCH_CODE ,
                        PRE_BRANCH_CODE,
                        GROUP_SKU_FLAG 
                        FROM FA_BRANCH_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{company_code}' AND GROUP_SKU_FLAG='I' {condition}
                        ORDER BY BRANCH_EDESC";
            var customerList = _dbContext.SqlQuery<BranchModels>(query).ToList();
            return customerList;
        }

        public List<PARTY_TYPE_MODELS> GetChiledLevelPartyType(string filter)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var company_code = _workcontext.CurrentUserinformation.company_code;
            string condition = string.Empty;
            if (!String.IsNullOrEmpty(filter))
            {
                condition = $@"AND ( LOWER(PARTY_TYPE_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(PARTY_TYPE_CODE) LIKE '%{filter.ToLower()}%')";
            }
            string query = $@"SELECT DISTINCT 
                        INITCAP(PARTY_TYPE_EDESC) AS PARTY_TYPE_EDESC,
                        PARTY_TYPE_CODE 
                        FROM IP_PARTY_TYPE_CODE
                        WHERE DELETED_FLAG = 'N' {condition}    
                        AND COMPANY_CODE='{company_code}'                     
                        ORDER BY PARTY_TYPE_EDESC ASC";
            var List = _dbContext.SqlQuery<PARTY_TYPE_MODELS>(query).ToList();
            return List;
        }
        public List<EmployeeModels> getAllEmployee(string filter)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var company_code = _workcontext.CurrentUserinformation.company_code;
            string condition = string.Empty;
            if (!String.IsNullOrEmpty(filter))
            {
                condition = $@"AND ( LOWER(EMPLOYEE_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(EMPLOYEE_EDESC) LIKE '%{filter.ToLower()}%')";
            }
            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userNo}' AND COMPANY_CODE = '{company_code}'";
            var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            string query = $@"SELECT DISTINCT 
                        INITCAP(EMPLOYEE_EDESC) AS EMPLOYEE_EDESC,
                        EMPLOYEE_CODE ,
                        MASTER_EMPLOYEE_CODE, 
                        PRE_EMPLOYEE_CODE,
                        GROUP_SKU_FLAG 
                        FROM HR_EMPLOYEE_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}'  {condition}
                         AND EMPLOYEE_CODE in (
                            SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userNo}' AND COMPANY_CODE='{company_code}'
                              UNION ALL
                             SELECT HES.EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP HES, HR_EMPLOYEE_TREE ET
                                WHERE HES.EMPLOYEE_CODE = ET.EMPLOYEE_CODE 
                                 --AND PARENT_EMPLOYEE_CODE=(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userNo}' AND COMPANY_CODE='{company_code}')
                                    START WITH PARENT_EMPLOYEE_CODE =(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userNo}' AND COMPANY_CODE='{company_code}')
                                CONNECT BY PRIOR ET.EMPLOYEE_CODE  = PARENT_EMPLOYEE_CODE)
                        ORDER BY PRE_EMPLOYEE_CODE";
            var customerList = new List<EmployeeModels>();
            if(superFlag!="Y")
            {
                customerList= _dbContext.SqlQuery<EmployeeModels>(query).ToList();
            }
            //if (customerList.Count <= 0)
            else
            {
                //string isSuperAdminQ = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE COMPANY_CODE='{company_code}' AND USER_NO= '{_workcontext.CurrentUserinformation.User_id}'";
                //var superAdmin = _dbContext.SqlQuery<string>(isSuperAdminQ).First();
               // if (superAdmin == "Y")
               // {
                    string query1 = $@"SELECT DISTINCT 
                        INITCAP(EMPLOYEE_EDESC) AS EMPLOYEE_EDESC,
                        EMPLOYEE_CODE ,
                        MASTER_EMPLOYEE_CODE, 
                        PRE_EMPLOYEE_CODE,
                        GROUP_SKU_FLAG 
                        FROM HR_EMPLOYEE_SETUP
                        WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND GROUP_SKU_FLAG= 'I' {condition}
                        ORDER BY PRE_EMPLOYEE_CODE";
                    customerList = _dbContext.SqlQuery<EmployeeModels>(query1).ToList();
                //}
            }
            return customerList;
        }

        public List<EmployeeModels> getAllEmployeeForHrEmployeeTree(string filter)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var company_code = _workcontext.CurrentUserinformation.company_code;
            string condition = string.Empty;
            if (!String.IsNullOrEmpty(filter))
            {
                condition = $@"AND ( LOWER(EMPLOYEE_EDESC) LIKE '%{filter.ToLower()}%' OR LOWER(EMPLOYEE_EDESC) LIKE '%{filter.ToLower()}%')";
            }
            string query = $@"SELECT DISTINCT 
                        INITCAP(EMPLOYEE_EDESC) AS EMPLOYEE_EDESC,
                        EMPLOYEE_CODE ,
                        MASTER_EMPLOYEE_CODE, 
                        PRE_EMPLOYEE_CODE,
                        GROUP_SKU_FLAG 
                        FROM HR_EMPLOYEE_SETUP
                        WHERE DELETED_FLAG = 'N' {condition}    
                        AND COMPANY_CODE='{company_code}'                     
                        ORDER BY PRE_EMPLOYEE_CODE";
            var customerList = _dbContext.SqlQuery<EmployeeModels>(query).ToList();
            return customerList;
        }
        public List<PlanRegisterProductModel> PlanRegisterProducts(User userinfo, string pageNameId)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            string item_filter_condition = "";
            try
            {
                var url = HttpContext.Current.Request.Url.AbsoluteUri;

                string xmlpath = HttpContext.Current.Server.MapPath("~/Areas/NeoERP.Planning/ProductCondition.xml");
                var xml = XDocument.Load(xmlpath);
                var condition_query = from c in xml.Root.Descendants("Vendor")
                                      where (string)c.Attribute("ID") == pageNameId //&& (int)c.Attribute("UserRole") == userinfo.User_id
                                      select c.Element("ConditionQuery").Value;
                var result = condition_query.FirstOrDefault();

                if (result != null)
                {
                    item_filter_condition = result;
                }
            }
            catch (Exception)
            {
                item_filter_condition = "";
            }

            string query = $@"SELECT DISTINCT --LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag,
             CATEGORY_CODE
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND ims.COMPANY_CODE = '{userinfo.company_code}' {item_filter_condition}           
            --START WITH PRE_ITEM_CODE = '00' AND COMPANY_CODE = '{userinfo.company_code}' AND DELETED_FLAG='N'
            --CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND DELETED_FLAG='N' AND COMPANY_CODE='{userinfo.company_code}'";
            var productListNodes = _dbContext.SqlQuery<PlanRegisterProductModel>(query).ToList();
            return productListNodes;
        }
        public List<PlanRegisterProductModel> PlanRegisterProductsIndividual()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;

            string query = $@"SELECT DISTINCT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND GROUP_SKU_FLAG = 'I'      
            START WITH PRE_ITEM_CODE = '00'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND COMPANY_CODE='{company_code}'";
            var productListNodes = _dbContext.SqlQuery<PlanRegisterProductModel>(query).ToList();
            return productListNodes;
        }
        public int createEntities(string systemName)
        {
            TextInfo ProperCase = new CultureInfo("en-US", false).TextInfo;
            systemName = ProperCase.ToTitleCase(systemName.ToLower());
            FileInfo file = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + $"Areas\\NeoErp.{systemName}\\App_Data\\ENTITES.sql");
            var stream = file.OpenText();
            string script = stream.ReadToEnd();
            stream.Close();
            stream.Dispose();

            var individualStmt = script.Split('#').ToList();
            var count = 0;
            foreach (var stmt in individualStmt)
            {
                try
                {
                    var success = _dbContext.ExecuteSqlCommand(stmt);
                    count += success;
                }
                catch (Exception ex) { }
            }
            return count;
        }
        public IEnumerable<DateFilterModel> GetNextFiscalYearDateFilters(string fiscalYear, string nFiscalYear, string textToAppend = "", bool appendText = false, int substractYear = 0)
        {
            if (string.IsNullOrEmpty(fiscalYear))
                return null;
            string query = $@" SELECT DISTINCT STARTDATE,ENDDATE ,RANGENAME, SORTORDER FROM(
                                                 SELECT CS.AD_DATE STARTDATE,
                                                        (CS.AD_DATE + CS.DAYS_NO-1) ENDDATE,
                                                       FN_BS_MONTH(SUBSTR(CS.BS_MONTH,-2,2)) RANGENAME, 1 AS SORTORDER
                                                   FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{fiscalYear}'
                                                 UNION ALL
                                                 SELECT  START_DATE, END_DATE,'This Year' DATE_NAME , 2 AS SORTORDER  FROM HR_FISCAL_YEAR_CODE WHERE FISCAL_YEAR_CODE = '{fiscalYear}'                                                 
                                                 UNION ALL
                                                 SELECT AD_DATE START_DATE , (AD_DATE+DAYS_NO-1) END_DATE , 'This Month' DATE_NAME, 3 AS SORTORDER FROM CALENDAR_SETUP WHERE ADD_MONTHS(SYSDATE, 0 * 12) BETWEEN AD_DATE AND (AD_DATE+DAYS_NO) 
                                                 UNION ALL
                                                 SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE DESC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'Last Month' DATE_NAME, 4 AS SORTORDER
                                                    FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{fiscalYear}'
                                                       AND  CS.AD_DATE< SYSDATE-DAYS_NO
                                                 UNION ALL
                                                 SELECT TRUNC((NEXT_DAY(ADD_MONTHS(SYSDATE, 0 * 12),'SUN')-7)) START_DATE ,TRUNC(ADD_MONTHS(SYSDATE,0 * 12)) END_DATE, 'This Week' DATE_NAME, 5 AS SORTORDER  FROM DUAL
                                                 UNION ALL
                                                 SELECT TRUNC((NEXT_DAY(ADD_MONTHS(SYSDATE, 0 * 12),'SUN')-14)) START_DATE ,TRUNC( (NEXT_DAY(ADD_MONTHS(SYSDATE, 0 * 12),'SAT')-7)) END_DATE, 'Last Week' DATE_NAME, 6  AS SORTORDER  FROM DUAL
                                                 UNION ALL
                                                 SELECT DISTINCT
                                                        FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                        FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'First Quarter' DATE_NAME, 7 AS SORTORDER
                                                 FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{fiscalYear}'
                                                        AND SUBSTR(CS.BS_MONTH,-2,2) >='04'
                                                         AND SUBSTR(CS.BS_MONTH,-2,2) <'07'
                                                 UNION ALL
                                                 SELECT DISTINCT
                                                        FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                        FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'Second Quarter' DATE_NAME, 8 AS SORTORDER
                                                 FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{fiscalYear}'
                                                        AND SUBSTR(CS.BS_MONTH,-2,2) >='07'
                                                         AND SUBSTR(CS.BS_MONTH,-2,2) <'10'
                                                 UNION ALL 
                                                  SELECT DISTINCT
                                                        FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                        FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'Third Quarter' DATE_NAME, 9 AS SORTORDER
                                                 FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{fiscalYear}'
                                                        AND SUBSTR(CS.BS_MONTH,-2,2) >='10'
                                                         AND SUBSTR(CS.BS_MONTH,-2,2) <='12'
                                                 UNION ALL     
                                                 SELECT DISTINCT
                                                        FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                        FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'Forth Quarter' DATE_NAME, 10 AS SORTORDER
                                                 FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{fiscalYear}'
                                                        AND SUBSTR(CS.BS_MONTH,-2,2) >='01'
                                                         AND SUBSTR(CS.BS_MONTH,-2,2) <='03' 
                                                UNION ALL     
                                                  SELECT  START_DATE, END_DATE,'Custom' DATE_NAME , -100 AS SORTORDER  FROM HR_FISCAL_YEAR_CODE WHERE FISCAL_YEAR_CODE = '{fiscalYear}'
                                                  UNION ALL
                                                  SELECT CS.AD_DATE MONTH_START,
                                                   (CS.AD_DATE + CS.DAYS_NO-1) MONTH_END,
                                                  'NFY.'||FN_BS_MONTH(SUBSTR(CS.BS_MONTH,-2,2)) MONTH_NAME, 11 AS SORTORDER
                                              FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{nFiscalYear}'
                                             UNION ALL
                                             SELECT  START_DATE, END_DATE,'NFY Year' DATE_NAME, 12 AS SORTORDER  FROM PL_FISCAL_YEAR_CODE WHERE FISCAL_YEAR_CODE = '{nFiscalYear}' 
                                            UNION ALL
                                            SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'NFY.First Quarter' DATE_NAME, 13 AS SORTORDER
                                             FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{nFiscalYear}'
                                                   AND SUBSTR(CS.BS_MONTH,-2,2) >='04'
                                                    AND SUBSTR(CS.BS_MONTH,-2,2) <'07'
                                            UNION ALL
                                             SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'NFY.Second Quarter' DATE_NAME, 14 AS SORTORDER
                                             FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{nFiscalYear}'
                                                   AND SUBSTR(CS.BS_MONTH,-2,2) >='07'
                                                    AND SUBSTR(CS.BS_MONTH,-2,2) <'10'
                                            UNION ALL 
                                             SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'NFY.Third Quarter' DATE_NAME, 15 AS SORTORDER
                                             FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{nFiscalYear}'
                                                   AND SUBSTR(CS.BS_MONTH,-2,2) >='10'
                                                    AND SUBSTR(CS.BS_MONTH,-2,2) <='12'
                                            UNION ALL     
                                            SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'NFY.Forth Quarter' DATE_NAME, 16 AS SORTORDER
                                             FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{nFiscalYear}'
                                                   AND SUBSTR(CS.BS_MONTH,-2,2) >='01'
                                                    AND SUBSTR(CS.BS_MONTH,-2,2) <='03') ORDER BY SORTORDER,STARTDATE";

            var data = this._dbContext.SqlQuery<DateFilterModel>(query).ToList();
            foreach (var item in data)
            {
                item.StartDateString = item.StartDate.ToString("yyyy-MM-dd");
                item.EndDateString = item.EndDate.ToString("yyyy-MM-dd");
                if (appendText && !string.IsNullOrEmpty(textToAppend))
                {
                    item.RangeName = textToAppend + " " + item.RangeName;
                }
            }
            data.Add(new DateFilterModel()
            {
                SortOrder = data.Max(q => q.SortOrder) + 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                StartDateString = DateTime.Now.ToString("yyyy-MM-dd"),
                EndDateString = DateTime.Now.ToString("yyyy-MM-dd"),
                RangeName = "Today",
            });
            return data;
        }

        public IEnumerable<DateFilterModel> GetFiscalYearDateFilters(string fiscalYear, string nFiscalYear, string textToAppend = "", bool appendText = false, int substractYear = 0)
        {
            var info = _workcontext.CurrentUserinformation;
            var qry = $@"SELECT * FROM PL_FISCAL_YEAR_CODE WHERE ACTIVE='Y' AND DELETED_FLAG='N' AND COMPANY_CODE='{info.company_code}'";
            var fiscalYearList = this._dbContext.SqlQuery<plFiscalYearModel>(qry).ToList();
            var fiscalYearQuery = $@"SELECT DISTINCT STARTDATE,ENDDATE ,RANGENAME, SORTORDER FROM( ";
            var count = 0;
            foreach (var fisYear in fiscalYearList)
            {
                count++;
                var nfy_name = string.Empty;
                if (fisYear.FISCAL_YEAR_CODE != fiscalYear)
                {
                    nfy_name = $@"({fisYear.FISCAL_YEAR_CODE.Substring(2, 5).ToString()}) ";
                }
                fiscalYearQuery += $@"SELECT CS.AD_DATE STARTDATE,
                                                        (CS.AD_DATE + CS.DAYS_NO-1) ENDDATE,
                                                       '{nfy_name}'||FN_BS_MONTH(SUBSTR(CS.BS_MONTH,-2,2)) RANGENAME, {count} AS SORTORDER
                                                   FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{fisYear.FISCAL_YEAR_CODE}'
                                                 UNION ALL
                                            SELECT  START_DATE, END_DATE,'{nfy_name}This Year' DATE_NAME, {count+1} AS SORTORDER  FROM PL_FISCAL_YEAR_CODE WHERE FISCAL_YEAR_CODE = '{fisYear.FISCAL_YEAR_CODE}' 
                                            UNION ALL
                                            SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, '{nfy_name}First Quarter' DATE_NAME, {count+1} AS SORTORDER
                                             FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{fisYear.FISCAL_YEAR_CODE}'
                                                   AND SUBSTR(CS.BS_MONTH,-2,2) >='04'
                                                    AND SUBSTR(CS.BS_MONTH,-2,2) <'07'
                                            UNION ALL
                                             SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, '{nfy_name}Second Quarter' DATE_NAME, {count+1} AS SORTORDER
                                             FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{fisYear.FISCAL_YEAR_CODE}'
                                                   AND SUBSTR(CS.BS_MONTH,-2,2) >='07'
                                                    AND SUBSTR(CS.BS_MONTH,-2,2) <'10'
                                            UNION ALL 
                                             SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, '{nfy_name}Third Quarter' DATE_NAME, {count+1} AS SORTORDER
                                             FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{fisYear.FISCAL_YEAR_CODE}'
                                                   AND SUBSTR(CS.BS_MONTH,-2,2) >='10'
                                                    AND SUBSTR(CS.BS_MONTH,-2,2) <='12'
                                            UNION ALL     
                                            SELECT DISTINCT
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, '{nfy_name}Forth Quarter' DATE_NAME, {count+1} AS SORTORDER
                                             FROM PL_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                             WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                   AND FY.FISCAL_YEAR_CODE = '{fisYear.FISCAL_YEAR_CODE}'
                                                   AND SUBSTR(CS.BS_MONTH,-2,2) >='01'
                                                    AND SUBSTR(CS.BS_MONTH,-2,2) <='03'";
                if (fiscalYearList.Count() > 1) {
                    fiscalYearQuery += $@" UNION ALL ";
                }
            }

            if (fiscalYearList.Count() > 1)
            {
                fiscalYearQuery = fiscalYearQuery.Substring(0, fiscalYearQuery.Length - 11);
            }
            fiscalYearQuery += $@") ORDER BY SORTORDER,STARTDATE";
            
            var data = this._dbContext.SqlQuery<DateFilterModel>(fiscalYearQuery).ToList();
            foreach (var item in data)
            {
                item.StartDateString = item.StartDate.ToString("yyyy-MM-dd");
                item.EndDateString = item.EndDate.ToString("yyyy-MM-dd");
                if (appendText && !string.IsNullOrEmpty(textToAppend))
                {
                    item.RangeName = textToAppend + " " + item.RangeName;
                }
            }
            data.Add(new DateFilterModel()
            {
                SortOrder = data.Max(q => q.SortOrder) + 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                StartDateString = DateTime.Now.ToString("yyyy-MM-dd"),
                EndDateString = DateTime.Now.ToString("yyyy-MM-dd"),
                RangeName = "Today",
            });
            return data;
        }

        public IEnumerable<DateFilterModel> GetEnglishDateFilters(string fiscalYear, string textToAppend = "", bool appendText = false)
        {
            if (string.IsNullOrEmpty(fiscalYear))
                return null;

            string query = string.Format(@"SELECT StartDate,
                                                     EndDate,
                                                     RangeName,
                                                     SortOrder
                                                FROM (
                                                --
                                                select to_date(first.StartDate,'MM/DD/YYYY') StartDate,to_date(second.EndDate,'MM/DD/YYYY') EndDate,first.RangeName,1 AS SortOrder from (
                                             select MonthEnglish as RangeName,ymd as StartDate from (SELECT   
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day   ,
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON-YYY') MonthEnglish ,
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM/DD/YYYY') ymd
                                                , ROW_NUMBER()
                                                            OVER (PARTITION BY TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON-YYY')
                                                                      ORDER BY  TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM/DD/YYYY')) AS rownumber
                                            FROM all_objects,
                                               (SELECT start_date, end_date
                                                  FROM HR_FISCAL_YEAR_CODE
                                                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                                            WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                                            group by 
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')
                                                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON-YYY') 
                                                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM/DD/YYYY')
                                            ORDER BY
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM/DD/YYYY'))
                                              WHERE rownumber = 1) first
                                              inner join 
                                              ( select MonthEnglish as RangeName,ymd as EndDate from (SELECT   
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD') day   ,
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON-YYY') MonthEnglish ,
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM/DD/YYYY') ymd
                                                , ROW_NUMBER()
                                                            OVER (PARTITION BY TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON-YYY')
                                                                      ORDER BY  TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM/DD/YYYY') desc) AS rownumber
                                            FROM all_objects,
                                               (SELECT start_date, end_date
                                                  FROM HR_FISCAL_YEAR_CODE
                                                 WHERE sysdate BETWEEN start_date AND end_date) fiscal_year
                                            WHERE ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1
                                            group by 
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'DD')
                                                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MON-YYY')    
                                                ,TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM/DD/YYYY')
                                            ORDER BY
                                                TO_CHAR(fiscal_year.start_date + ROWNUM - 1, 'MM/DD/YYYY'))
                                              WHERE rownumber = 1) second on first.RangeName = second.RangeName
                                                --
                                                      UNION ALL
                                                      SELECT START_DATE,
                                                             END_DATE,
                                                             'This Year' date_name,
                                                             2 AS SortOrder
                                                        FROM HR_FISCAL_YEAR_CODE
                                                       WHERE FISCAL_YEAR_CODE = '{0}'
                                                      UNION ALL
                                                      SELECT AD_DATE START_DATE,
                                                             (AD_DATE + DAYS_NO - 1) END_DATE,
                                                             'This Month' date_name,
                                                             3 AS SortOrder
                                                        FROM CALENDAR_SETUP
                                                       WHERE SYSDATE BETWEEN AD_DATE AND (AD_DATE + DAYS_NO - 1)
                                                      UNION ALL
                                                      SELECT DISTINCT
                                                             FIRST_VALUE (
                                                                CS.AD_DATE)
                                                             OVER (
                                                                ORDER BY
                                                                   CS.AD_DATE DESC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC)
                                                                START_DATE,
                                                             FIRST_VALUE (
                                                                CS.AD_DATE + CS.DAYS_NO - 1)
                                                             OVER (
                                                                ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC)
                                                                END_DATE,
                                                             'Last Month' DATE_NAME,
                                                             4 AS SortOrder
                                                        FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                       WHERE     CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                             AND FY.FISCAL_YEAR_CODE = '{0}'
                                                             AND CS.AD_DATE < SYSDATE - DAYS_NO
                                                      UNION ALL
                                                      SELECT TRUNC ( (NEXT_DAY (SYSDATE, 'SUN') - 7)) start_date,
                                                             TRUNC (SYSDATE) end_date,
                                                             'This Week' date_name,
                                                             5 AS SortOrder
                                                        FROM DUAL
                                                      UNION ALL
                                                      SELECT TRUNC ( (NEXT_DAY (SYSDATE, 'SUN') - 14)) start_date,
                                                             TRUNC ( (NEXT_DAY (SYSDATE, 'SAT') - 7)) end_date,
                                                             'Last Week' date_name,
                                                             6 AS SortOrder
                                                        FROM DUAL
                                                      UNION ALL
                                                      SELECT DISTINCT
                                                             FIRST_VALUE (
                                                                CS.AD_DATE)
                                                             OVER (
                                                                ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC)
                                                                START_DATE,
                                                             FIRST_VALUE (
                                                                CS.AD_DATE + CS.DAYS_NO - 1)
                                                             OVER (
                                                                ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC)
                                                                END_DATE,
                                                             'First Quarter' DATE_NAME,
                                                             7 AS SortOrder
                                                        FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                       WHERE     CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                             AND FY.FISCAL_YEAR_CODE = '{0}'
                                                             AND SUBSTR (CS.BS_MONTH, -2, 2) >= '04'
                                                             AND SUBSTR (CS.BS_MONTH, -2, 2) < '07'
                                                      UNION ALL
                                                      SELECT DISTINCT
                                                             FIRST_VALUE (
                                                                CS.AD_DATE)
                                                             OVER (
                                                                ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC)
                                                                START_DATE,
                                                             FIRST_VALUE (
                                                                CS.AD_DATE + CS.DAYS_NO - 1)
                                                             OVER (
                                                                ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC)
                                                                END_DATE,
                                                             'Second Quarter' DATE_NAME,
                                                             8 AS SortOrder
                                                        FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                       WHERE     CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                             AND FY.FISCAL_YEAR_CODE = '{0}'
                                                             AND SUBSTR (CS.BS_MONTH, -2, 2) >= '07'
                                                             AND SUBSTR (CS.BS_MONTH, -2, 2) < '10'
                                                      UNION ALL
                                                      SELECT DISTINCT
                                                             FIRST_VALUE (
                                                                CS.AD_DATE)
                                                             OVER (
                                                                ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC)
                                                                START_DATE,
                                                             FIRST_VALUE (
                                                                CS.AD_DATE + CS.DAYS_NO - 1)
                                                             OVER (
                                                                ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC)
                                                                END_DATE,
                                                             'Third Quarter' DATE_NAME,
                                                             9 AS SortOrder
                                                        FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                       WHERE     CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                             AND FY.FISCAL_YEAR_CODE = '{0}'
                                                             AND SUBSTR (CS.BS_MONTH, -2, 2) >= '10'
                                                             AND SUBSTR (CS.BS_MONTH, -2, 2) <= '12'
                                                      UNION ALL
                                                      SELECT DISTINCT
                                                             FIRST_VALUE (
                                                                CS.AD_DATE)
                                                             OVER (
                                                                ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO - 1) DESC)
                                                                START_DATE,
                                                             FIRST_VALUE (
                                                                CS.AD_DATE + CS.DAYS_NO - 1)
                                                             OVER (
                                                                ORDER BY (CS.AD_DATE + CS.DAYS_NO - 1) DESC, CS.AD_DATE ASC)
                                                                END_DATE,
                                                             'Forth Quarter' DATE_NAME,
                                                             10 AS SortOrder
                                                        FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                       WHERE     CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                             AND FY.FISCAL_YEAR_CODE = '{0}'
                                                             AND SUBSTR (CS.BS_MONTH, -2, 2) >= '01'
                                                             AND SUBSTR (CS.BS_MONTH, -2, 2) <= '03'
                                                      UNION ALL
                                                      SELECT START_DATE,
                                                             END_DATE,
                                                             'Custom' date_name,
                                                             -100 AS SortOrder
                                                        FROM HR_FISCAL_YEAR_CODE
                                                       WHERE FISCAL_YEAR_CODE = '{0}')
                                            ORDER BY SortOrder, StartDate", fiscalYear);


            var data = this._dbContext.SqlQuery<DateFilterModel>(query).ToList();
            foreach (var item in data)
            {
                item.StartDateString = item.StartDate.ToString("yyyy-MM-dd");
                item.EndDateString = item.EndDate.ToString("yyyy-MM-dd");
                if (appendText && !string.IsNullOrEmpty(textToAppend))
                {
                    item.RangeName = textToAppend + " " + item.RangeName;
                }
            }
            data.Add(new DateFilterModel()
            {
                SortOrder = data.Max(q => q.SortOrder) + 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                StartDateString = DateTime.Now.ToString("yyyy-MM-dd"),
                EndDateString = DateTime.Now.ToString("yyyy-MM-dd"),
                RangeName = "Today",


            });

            //this._cacheManager.Set("calender-date-filter", data, 10000);
            return data;
        }
    }
}
