using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Planning.Service.Models;
using System.Runtime.CompilerServices;
using NeoErp.Data;
using NeoErp.Core;
using NeoErp.Planning.Service.Interface;
using System.Data.Common;
using System.Globalization;
using NeoErp.Core.Models;
namespace NeoErp.Planning.Service.Repository
{

    public class PlanSetupRepo : IPlanSetup
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        private NeoErpCoreEntity _objContext { get; set; }
        public PlanSetupRepo(IDbContext dbContext, IWorkContext _iWorkContext, NeoErpCoreEntity objContext)
        {
            this._workcontext = _iWorkContext;
            this._dbContext = dbContext;
            _objContext = objContext;
        }

        public List<ItemModel> getItemByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"SELECT DISTINCT LEVEL, 
                        INITCAP(ITEM_EDESC) AS ITEM_EDESC,
                        ITEM_CODE ,
                        MASTER_ITEM_CODE, 
                        PRE_ITEM_CODE ,
                        GROUP_SKU_FLAG 
                        FROM IP_ITEM_MASTER_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        and ROWNUM<100         
                        START WITH (ITEM_CODE = '{filter.ToString()}' OR PRE_ITEM_CODE like (SELECT MASTER_ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{filter.ToString()}') || '%')
                        CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
                        ORDER BY INITCAP(LEVEL)";
                var result = _dbContext.SqlQuery<ItemModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PlanSetupTitleModel> getTitleValues(string planCode)
        {
            try
            {
                var sqlquery = $@"SELECT TO_CHAR(PL.ITEM_CODE)ITEM_CODE, TO_CHAR(ITM.ITEM_EDESC)ITEM_EDESC, TO_CHAR(PL.START_DATE)START_DATE, TO_CHAR(PL.END_DATE)END_DATE,
                                    TO_CHAR(PL.PLAN_TYPE)PLAN_TYPE,TO_CHAR(PL.TIME_FRAME_CODE)TIME_FRAME_CODE, TO_CHAR(PTF.TIME_FRAME_EDESC)TIME_FRAME_EDESC
                                    FROM PL_PLAN PL, PL_TIME_FRAME PTF, IP_ITEM_MASTER_SETUP ITM
                                    WHERE PL.ITEM_CODE = ITM.ITEM_CODE
                                    AND PL.TIME_FRAME_CODE= PTF.TIME_FRAME_CODE
                                    AND PL.PLAN_CODE = '{planCode}' ";
                var result = _dbContext.SqlQuery<PlanSetupTitleModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MyColumnSettings> GetFrequencyTitle(string planCode)
        {
            try
            {

                var sqlquery = $@"SELECT DISTINCT TO_CHAR (pl.PLAN_CODE) PLAN_CODE,
                               pl.PLAN_EDESC,
                               TO_CHAR (pl.START_DATE) START_DATE,
                               TO_CHAR (pl.END_DATE) END_DATE,
                               TO_CHAR (ptf.TIME_FRAME_CODE) TIME_FRAME_CODE,
                               ptf.TIME_FRAME_EDESC as FREQUENCY,
                               TO_CHAR((trunc(pl.END_DATE) - to_date(pl.START_DATE))+1) as DAYS,
                               TO_CHAR(ptf.DAYS) AS TIME_FRAME_DAYS
                         FROM PL_PLAN pl, PL_TIME_FRAME ptf
                         WHERE 
                         pl.TIME_FRAME_CODE = ptf.TIME_FRAME_CODE AND PLAN_CODE = '{planCode}'";
                var results = _dbContext.SqlQuery<FrequencyColumnModel>(sqlquery).ToList();
                var obj = new List<MyColumnSettings>();
                if (results.Count() == 1)
                {
                    //foreach (var it in results)
                    //{
                    if (results[0].FREQUENCY.ToLower() == "week")
                    {
                        var weekQuery = $@"SELECT DISTINCT TO_CHAR(DAYS+(ROWNUM-1+7)/7,'IW')  WEEKS 
                            ,TO_CHAR(DAYS,'MM') MONTHINT
                            ,TO_CHAR(DAYS,'MON') MONTH
                            ,TO_CHAR(DAYS,'YYYY') YEAR 
                              FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS FROM ALL_OBJECTS, (SELECT  START_DATE,END_DATE FROM PL_PLAN WHERE PLAN_CODE='{planCode}') FISCAL_YEAR
                                     WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR
                            ORDER BY TO_CHAR(DAYS+(ROWNUM-1+7)/7,'IW') ";
                        var weekResult = this._dbContext.SqlQuery<FrequencyColumnModel>(weekQuery).ToList();
                        int yearCount = 0, monthCount = 0;
                        string tempYear = "", tempMonth = "", tempWeek = "";
                        if (weekResult.Count > 0)
                        {
                            bool duplicateWeek = false;
                            foreach (var it in weekResult)
                            {
                                string year = it.YEAR;
                                string month = it.MONTH;
                                string week = it.WEEKS;
                                if (tempWeek != week) // for removing duplicate week.
                                {
                                    if (year != tempYear && !duplicateWeek)
                                    {
                                        yearCount = weekResult.Count(a => a.YEAR == year);
                                        weekResult.Where(a => a.YEAR == year).ToList().ForEach(p => p.YEARCOUNT = yearCount.ToString());
                                        tempYear = year;
                                    }
                                    else
                                    {
                                        yearCount = weekResult.Count(a => a.YEAR == year);
                                        weekResult.Where(a => a.YEAR == year).ToList().ForEach(p => p.YEARCOUNT = (yearCount - 1).ToString());
                                        tempYear = year;
                                    }
                                    if (month != tempMonth && !duplicateWeek)
                                    {
                                        monthCount = weekResult.Count(a => a.MONTH == month);
                                        weekResult.Where(a => a.MONTH == month).ToList().ForEach(p => p.MONTHCOUNT = monthCount.ToString());
                                        tempMonth = month;
                                    }
                                    else
                                    {
                                        tempMonth = month;
                                        duplicateWeek = false;
                                    }

                                    var objTemp = new MyColumnSettings()
                                    {
                                        Title = "Week " + it.WEEKS,
                                        //PropertyName = it.MONTH.ToUpper() + "_" + it.MONTHINT
                                        PropertyName = "WEEK_" + Convert.ToInt32(it.WEEKS),
                                        FrequencyName = "WEEK",
                                    };
                                    obj.Add(objTemp);
                                    var periodObj = new periodClass()
                                    {
                                        YEAR = it.YEAR,
                                        MONTH = it.MONTH,
                                        YEARCOUNT = it.YEARCOUNT,
                                        MONTHCOUNT = it.MONTHCOUNT,
                                        WEEK = it.WEEKS,
                                    };
                                    objTemp.getPeriod.Add(periodObj);
                                    tempWeek = week;
                                }
                                else if (tempWeek == week)
                                {
                                    monthCount = weekResult.Count(a => a.MONTH == month);
                                    weekResult.Where(a => a.MONTH == month).ToList().ForEach(p => p.MONTHCOUNT = (monthCount - 1).ToString());
                                    duplicateWeek = true;
                                }
                            }
                        }
                        //var frqCount = GetFrequency(Convert.ToInt32(results[0].DAYS), Convert.ToInt32(7));
                        //if (frqCount > 0)
                        //{
                        //    for (var i = 1; i <= frqCount; i++)
                        //    {
                        //        var objTemp = new MyColumnSettings()
                        //        {
                        //            Title = results[0].FREQUENCY + i,
                        //            PropertyName = results[0].FREQUENCY.ToUpper() + "_" + i,
                        //        };
                        //        obj.Add(objTemp);
                        //    }
                        //}
                    }
                    else
                    {
                        var monthQuery = $@"select to_char(add_months( start_date, level-1 ),'MON') MONTH,
                                                to_char(add_months( start_date, level-1 ),'MM') MONTHINT, to_char(add_months( start_date, level-1 ),'YYYY') YEAR,  to_char(count(*) over (partition by to_char(add_months( start_date, level-1 ),'YYYY')))YEARCOUNT
                                              from (select  start_date,
                                                            end_date
                                                      from pl_plan where plan_code='{planCode}')
                                             connect by level <= months_between(
                                                                   trunc(end_date,'MM'),
                                                                   trunc(start_date,'MM') ) + 1
                          group by  to_char(add_months( start_date, level-1 ),'YYYY')
                                                                   , to_char(add_months( start_date, level-1 ),'MON')
                                                                    ,to_char(add_months( start_date, level-1 ),'MM') 
                                                                    ,to_char(add_months( start_date, level-1 ),'YYYYMM')
                                                                    order by  to_char(add_months( start_date, level-1 ),'YYYYMM')";
                        var monthResult = _dbContext.SqlQuery<FrequencyColumnModel>(monthQuery).ToList();
                        if (monthResult.Count > 0)
                        {
                            foreach (var it in monthResult)
                            {
                                var objTemp = new MyColumnSettings()
                                {
                                    Title = it.MONTH,
                                    //PropertyName = it.MONTH.ToUpper() + "_" + it.MONTHINT
                                    PropertyName = it.MONTH.ToUpper(),
                                    FrequencyName = "MONTH",
                                };
                                obj.Add(objTemp);
                                var periodObj = new periodClass()
                                {
                                    YEAR = it.YEAR,
                                    MONTH = it.MONTH,
                                    YEARCOUNT = it.YEARCOUNT
                                };
                                objTemp.getPeriod.Add(periodObj);
                            }
                        }
                    }
                    //}

                }

                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PlanModels> getPlanList(string filter,string startDate, string endDate)
        {
            try
            {
                var info = _workcontext.CurrentUserinformation;
                var condition = string.Empty;
                try
                {
                    if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                    else { condition = $@" AND MASTER_PLAN_EDESC LIKE '%{filter}%'"; }
                    var sqlquery = $@"SELECT PLAN_CODE , PLAN_EDESC  FROM PL_SALES_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE='{info.company_code}'
                                    AND START_DATE >= TO_DATE('{startDate}','DD-Mon-YYYY') AND END_DATE <=TO_DATE('{endDate}','DD-Mon-YYYY') {condition} 
                                    ORDER BY CREATED_DATE DESC";

                    var result = _dbContext.SqlQuery<PlanModels>(sqlquery).ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MasterSalesPlan> getAllMasterPlanNames(string filter, string startDate, string endDate)
        {
            var info = _workcontext.CurrentUserinformation;
            var condition = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                else{condition = $@" AND MASTER_PLAN_EDESC LIKE '%{filter}%'";}
                //var sqlquery = $@"SELECT DISTINCT MSP.MASTER_PLAN_CODE ,MSP.MASTER_PLAN_EDESC ,SP.TIME_FRAME_CODE,
                //TO_CHAR(MSP.START_DATE,'YYYY-Mon-DD') START_DATE,TO_CHAR(MSP.END_DATE,'YYYY-Mon-DD') END_DATE,SP.CALENDAR_TYPE FROM PL_MASTER_SALES_PLAN MSP
                //INNER JOIN PL_SALES_PLAN_MAP SPM ON MSP.MASTER_PLAN_CODE = SPM.MASTER_PLAN_CODE
                //INNER JOIN PL_SALES_PLAN SP ON SPM.PLAN_CODE= SP.PLAN_CODE
                //WHERE MSP.DELETED_FLAG='N' AND MSP.COMPANY_CODE='{info.company_code}' {condition}";
                var sqlquery = $@"SELECT MASTER_PLAN_CODE , MASTER_PLAN_EDESC  FROM PL_MASTER_SALES_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE='{info.company_code}'
                                    AND START_DATE >= TO_DATE('{startDate}','DD-Mon-YYYY') AND END_DATE <=TO_DATE('{endDate}','DD-Mon-YYYY') {condition} 
                                    ORDER BY CREATED_DATE DESC";

                var result = _dbContext.SqlQuery<MasterSalesPlan>(sqlquery).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<freqNameVlaue> getFrequency(string filter)
        {
            try
            {
                var sqlquery = $@"SELECT TO_CHAR(TIME_FRAME_CODE) as fvalue, TIME_FRAME_EDESC as fname FROM PL_TIME_FRAME
                               WHERE TIME_FRAME_CODE = '{filter}'";
                var result = _dbContext.SqlQuery<freqNameVlaue>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static int GetFrequency(int days, int fq)
        {
            var frq = (int)Math.Ceiling((double)days / fq);
            return frq;
        }


        public List<ItemModel> getItem()
        {
            try
            {
                var sqlquery = $@"SELECT LEVEL, 
                         INITCAP(ITEM_EDESC) as ITEM_EDESC ,
                         ITEM_CODE ,
                         MASTER_ITEM_CODE, 
                         PRE_ITEM_CODE , 
                         GROUP_SKU_FLAG 
                         FROM IP_ITEM_MASTER_SETUP ims
                         --WHERE ims.DELETED_FLAG = 'N'                  
                         --AND GROUP_SKU_FLAG = 'G'
                         --AND LEVEL = 1
                         START WITH PRE_ITEM_CODE = '00'
                         CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
                var result = _dbContext.SqlQuery<ItemModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ItemModel> getItemByCode(string planCode, string itemCode)
        {
            var sqlquery = $@"SELECT DISTINCT 
                        INITCAP(ITEM_EDESC) AS ITEM_EDESC,
                        ITEM_CODE ,
                        MASTER_ITEM_CODE, 
                        PRE_ITEM_CODE ,
                        GROUP_SKU_FLAG 
                        ,(SELECT IS_ALL_CHILD_SELECTED FROM PL_PLAN WHERE PLAN_CODE = '{planCode}' ) as IS_CHILD_SELECTED
                        FROM IP_ITEM_MASTER_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        AND ITEM_CODE in (SELECT ITEM_CODE FROM PL_PLAN_ITEM_MAPPING WHERE PLAN_CODE = '{planCode}' )
                       
                        ORDER BY PRE_ITEM_CODE";
            var result = _dbContext.SqlQuery<ItemModel>(sqlquery).ToList();

            return result;

        }

        public string SavePlan(List<savePlan> sv, string planCode, string time_frame_code)
        {
            try
            {
                var result = "";
                foreach (var item in sv)
                {
                    var userID = _workcontext.CurrentUserinformation.User_id;
                    var branchCode = _workcontext.CurrentUserinformation.branch_code;
                    var companyCode = _workcontext.CurrentUserinformation.company_code;

                    foreach (var freq in item.frequency)
                    {
                        try
                        {
                            var nextValQuery = $@"SELECT PL_PLAN_DTL_SEQ.nextval as PL_PLAN_NEXT_CODE FROM DUAL";
                            var id = _dbContext.SqlQuery<planDetailModel>(nextValQuery).ToList().FirstOrDefault();

                            var sqlquery = $@"INSERT INTO PL_PLAN_DTL (PLAN_DTL_CODE, PLAN_CODE, ITEM_CODE, TIME_FRAME_CODE, TIME_FRAME_VALUE, TARGET_VALUE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE,DELETED_FLAG)
                                VALUES({id.PL_PLAN_NEXT_CODE},'{planCode}','{item.itemCode }','{time_frame_code}',{freq.fname},{freq.fvalue},'{companyCode}','{branchCode}','{userID}',TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss'),'N')";
                            result = _dbContext.ExecuteSqlCommand(sqlquery).ToString();
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("unique constraint"))
                                throw;
                            else
                            {
                                var sqlquery = $@"UPDATE PL_PLAN_DTL SET TARGET_VALUE='{freq.fvalue}',LAST_MODIFIED_BY='{userID}', LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss') where PLAN_CODE='{planCode}' AND ITEM_CODE='{item.itemCode}' AND TIME_FRAME_CODE='{time_frame_code}' AND TIME_FRAME_VALUE='{freq.fname}'";
                                result = _dbContext.ExecuteSqlCommand(sqlquery).ToString();
                                //result = "constraint";
                            }
                        }

                    }
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getItemCode(string name)
        {
            string[] arr = new string[3];
            arr = name.Split('_');
            return arr[1].ToString();
        }

        public static string getTimeFrameVal(string fname)
        {
            string[] arr = new string[3];
            arr = fname.Split('_');
            return arr[1].ToString();
        }

        public List<PlanModels> GetPlanDetailVale(string plancode)
        {
            List<PlanModels> list = new List<PlanModels>();
            string query = $@"SELECT PLAN_DTL_CODE,PLAN_CODE,ITEM_CODE,TIME_FRAME_CODE,TIME_FRAME_VALUE,TARGET_VALUE FROM PL_PLAN_DTL WHERE PLAN_CODE='{plancode}'";
            list = this._dbContext.SqlQuery<PlanModels>(query).ToList();
            return list;
        }

        public List<MyColumnSettings> GetFrequencyTitle(string startDate, string endDate, string timeFrameCode, string timeFrameName, string datetype = "BS")
        {
            try
            {
                var obj = new List<MyColumnSettings>();
                if (timeFrameName.ToLower() == "week")
                {
                    string weekQuery = string.Empty;

                    if (datetype == "BS" || datetype == "LOC")
                    {
                        weekQuery = $@"SELECT DISTINCT TO_CHAR(FLOOR (nepali_week / 7) + 1) WEEKS
                                        ,substr(bs_date(daysitem),6,2) as bsm
                                        ,fn_bs_month(substr(bs_date(DAYSITEM),6,2)) as MONTH
                                        ,TO_CHAR(SUBSTR(BS_DATE(DAYSITEM),0,4)) YEAR
                                        FROM (SELECT (  fiscal_year.start_date
                                                    + ROWNUM
                                                    - 1
                                                    - NEXT_DAY (start_date, 'SUN'))
                                                      nepali_week,
                                                   TO_DATE (fiscal_year.start_date + ROWNUM - 1) DAYSITEM
                                              FROM hr_fiscal_year_code fiscal_year, all_objects
                                                WHERE sysdate BETWEEN start_date AND end_date
                                                   AND ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1)
                                        WHERE nepali_week >= 0
                                        and DAYSITEM between TO_DATE('{startDate}','MM-DD-YYYY') and TO_DATE('{endDate}','MM-DD-YYYY')
                                        order by (FLOOR (nepali_week / 7) + 1)";
                        //weekQuery = $@"SELECT DISTINCT TO_CHAR(FLOOR (nepali_week / 7) + 1) WEEKS--,  TO_CHAR (DAYSITEM, 'WW') NWEEKS
                        //                ,TO_CHAR(DAYSITEM,'MM') MONTHINT
                        //                --,TO_CHAR(DAYSITEM,'MON') MONTH
                        //                ,fn_bs_month(substr(bs_date(DAYSITEM),6,2)) as MONTH
                        //                ,TO_CHAR(DAYSITEM,'MON') MONTH
                        //                --,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                        //                --,to_char(to_date( bs_date(DAYSITEM),'YYYY-MM-DD'),'YYYY') YEAR
                        //                 ,TO_CHAR(SUBSTR(BS_DATE(DAYSITEM),0,4)) YEAR
                        //                ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                        //                --,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                        //                  FROM (SELECT (  fiscal_year.start_date
                        //                                + ROWNUM
                        //                                - 1
                        //                                - NEXT_DAY (start_date, 'SUN'))
                        //                                  nepali_week,
                        //                               TO_DATE (fiscal_year.start_date + ROWNUM - 1) DAYSITEM
                        //                          FROM hr_fiscal_year_code fiscal_year, all_objects
                        //                            WHERE sysdate BETWEEN start_date AND end_date
                        //                               AND ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1)
                        //                 WHERE nepali_week >= 0
                        //                 and DAYSITEM between TO_DATE('{startDate}','MM-DD-YYYY') and TO_DATE('{endDate}','MM-DD-YYYY')
                        //                 order by weeks";
                        //weekQuery = $@"select  distinct WEEKS,MONTHINT, MONTH,YEAR,  concat(year,monthint) YEARMONTH, 
                        //                to_number(concat(concat(year,monthint),weeks )) YEARMONTHweeks   from (  SELECT
                        //                SUBSTR(BS_DATE(a),6,2) as MONTHINT,
                        //                fn_bs_month(substr(bs_date(a),6,2)) as MONTH,
                        //                substr(bs_date(a),0,4) as YEAR,
                        //                to_char(trunc((a- TO_DATE('{startDate}', 'MM-DD-YYYY'))/7)+1)  WEEKS 
                        //                FROM(
                        //                        SELECT ROWNUM - 1 + TO_DATE ('{startDate}', 'MM-DD-YYYY') a 
                        //                        FROM all_objects 
                        //                        WHERE ROWNUM < TO_DATE ('{endDate}', 'MM-DD-YYYY') - TO_DATE ('{startDate}', 'MM-DD-YYYY')
                        //                    + 2)) order by YEARMONTHweeks";
                    }
                    else
                    {

                        //weekQuery = $@"SELECT FLOOR (nepali_week / 7) + 1 NWEEKS,  TO_CHAR (DAYSITEM, 'WW') WEEKS
                        //                ,TO_CHAR(DAYSITEM,'MM') MONTHINT
                        //                ,TO_CHAR(DAYSITEM,'MON') MONTH
                        //                ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                        //                ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                        //                ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                        //                  FROM (SELECT (  fiscal_year.start_date
                        //                                + ROWNUM
                        //                                - 1
                        //                                - NEXT_DAY (start_date, 'SUN'))
                        //                                  nepali_week,
                        //                               TO_DATE (fiscal_year.start_date + ROWNUM - 1) DAYSITEM
                        //                          FROM hr_fiscal_year_code fiscal_year, all_objects
                        //                            WHERE sysdate BETWEEN start_date AND end_date
                        //                               AND ROWNUM <= fiscal_year.end_date - fiscal_year.start_date + 1)
                        //                 WHERE nepali_week >= 0
                        //                 and DAYSITEM between TO_DATE('{startDate}','MM-DD-YYYY') and TO_DATE('{endDate}','MM-DD-YYYY')";

                        weekQuery = $@"SELECT DISTINCT TO_CHAR(DAYS+(ROWNUM-1+7)/7,'IW')  WEEKS 
                                            ,TO_CHAR(DAYS,'MM') MONTHINT
                                            ,TO_CHAR(DAYS,'MON') MONTH
                                            ,TO_CHAR(DAYS,'YYYY') YEAR 
                                            ,TO_CHAR(DAYS,'YYYYMM') YEARMONTH 
                                              FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS FROM ALL_OBJECTS 
                                              ,(SELECT TO_DATE('{startDate}','MM-DD-YYYY') START_DATE, TO_DATE('{endDate}','MM-DD-YYYY') END_DATE FROM DUAL) FISCAL_YEAR
                                              WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR
                                            ORDER BY
                                            TO_CHAR(DAYS,'YYYYMM')";
                    }
                    var weekResult = this._dbContext.SqlQuery<FrequencyColumnModel>(weekQuery).ToList();
                    int yearCount = 0, monthCount = 0;
                    string tempYear = "", tempMonth = "", tempWeek = "";
                    if (weekResult.Count > 0)
                    {
                        bool duplicateWeek = false;
                        foreach (var it in weekResult)
                        {
                            string year = it.YEAR;
                            string month = it.MONTH;
                            string week = it.WEEKS;
                            if (tempWeek != week) // for removing duplicate week.
                            {
                                if (year != tempYear && !duplicateWeek)
                                {
                                    yearCount = weekResult.Count(a => a.YEAR == year);
                                    weekResult.Where(a => a.YEAR == year).ToList().ForEach(p => p.YEARCOUNT = yearCount.ToString());
                                    tempYear = year;
                                }
                                else
                                {
                                    yearCount = weekResult.Count(a => a.YEAR == year);
                                    weekResult.Where(a => a.YEAR == year).ToList().ForEach(p => p.YEARCOUNT = (yearCount - 1).ToString());
                                    tempYear = year;
                                }
                                if (month != tempMonth && !duplicateWeek)
                                {
                                    monthCount = weekResult.Count(a => a.MONTH == month);
                                    weekResult.Where(a => a.MONTH == month).ToList().ForEach(p => p.MONTHCOUNT = monthCount.ToString());
                                    tempMonth = month;
                                }
                                else
                                {
                                    tempMonth = month;
                                    duplicateWeek = false;
                                }

                                var objTemp = new MyColumnSettings()
                                {
                                    Title = "Week " + it.WEEKS,
                                    //PropertyName = it.MONTH.ToUpper() + "_" + it.MONTHINT
                                    PropertyName = "WEEK_" + Convert.ToInt32(it.WEEKS),
                                    FrequencyName = "WEEK",
                                };
                                obj.Add(objTemp);
                                var periodObj = new periodClass()
                                {
                                    YEAR = it.YEAR,
                                    MONTH = it.MONTH,
                                    YEARCOUNT = it.YEARCOUNT,
                                    MONTHCOUNT = it.MONTHCOUNT,
                                    WEEK = it.WEEKS,
                                };
                                objTemp.getPeriod.Add(periodObj);
                                tempWeek = week;
                            }
                            else if (tempWeek == week)
                            {
                                monthCount = weekResult.Count(a => a.MONTH == month);
                                weekResult.Where(a => a.MONTH == month).ToList().ForEach(p => p.MONTHCOUNT = (monthCount - 1).ToString());
                                duplicateWeek = true;
                            }
                        }
                    }
                }
                else
                {
                    string monthQuery = string.Empty;

                    if (datetype == "BS" || datetype == "LOC")
                    {
                        monthQuery = $@"SELECT  distinct
                                        SUBSTR(BS_DATE(a), 6, 2) as MONTHINT,
                                        fn_bs_month(substr(bs_date(a), 6, 2)) as MONTH,
                                        substr(bs_date(a), 0, 4) as YEAR
                                        FROM(
                                                SELECT ROWNUM - 1 + TO_DATE('{startDate}', 'MM-DD-YYYY') a
                                                FROM all_objects
                                                WHERE ROWNUM < TO_DATE('{endDate}', 'MM-DD-YYYY') - TO_DATE('{startDate}', 'MM-DD-YYYY')
                                            + 2) order by YEAR,MONTHINT";
                    }
                    else
                    {
                        monthQuery = $@"SELECT TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1), 'MON') MONTH,
                                               TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1), 'MM') MONTHINT,
                                               TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1), 'YYYY') YEAR,
                                               TO_CHAR (COUNT (*)OVER (PARTITION BY TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1),'YYYY'))) YEARCOUNT
                                          FROM (SELECT TO_DATE('{startDate}','MM-DD-YYYY') START_DATE, TO_DATE('{endDate}','MM-DD-YYYY') END_DATE FROM DUAL) DA
                                            CONNECT BY LEVEL <= MONTHS_BETWEEN (TRUNC (DA.END_DATE, 'MM'),TRUNC (DA.START_DATE, 'MM'))+ 1
                                            GROUP BY TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1), 'YYYY'),
                                               TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1), 'MON'),
                                               TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1), 'MM'),
                                               TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1), 'YYYYMM')
                                            ORDER BY TO_CHAR (ADD_MONTHS (DA.START_DATE, LEVEL - 1), 'YYYYMM')";
                    }
                    var monthResult = _dbContext.SqlQuery<FrequencyColumnModel>(monthQuery).ToList();
                    if (monthResult.Count > 0)
                    {
                        var yearCount = monthResult.GroupBy(a => a.YEAR).Select(g => new { g.Key, Count = g.Count() });

                        foreach (var it in monthResult)
                        {

                            var objTemp = new MyColumnSettings()
                            {
                                Title = it.MONTH,
                                //PropertyName = it.MONTH.ToUpper() + "_" + it.MONTHINT
                                PropertyName = it.MONTH.ToUpper(),
                                FrequencyName = "MONTH",
                                MONTHINT = it.MONTHINT
                            };
                            obj.Add(objTemp);
                            var periodObj = new periodClass()
                            {
                                YEAR = it.YEAR,
                                MONTH = it.MONTH,
                                YEARCOUNT = !string.IsNullOrEmpty(it.YEARCOUNT) ? it.YEARCOUNT : (yearCount.Where(x => x.Key == it.YEAR).FirstOrDefault() == null ? "1" : yearCount.Where(x => x.Key == it.YEAR).First().Count.ToString())


                            };
                            objTemp.getPeriod.Add(periodObj);
                        }
                    }
                }
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string SavePlan(List<savePlan> sv, SalesPlan sp)
        {
            try
            {
                var userID = _workcontext.CurrentUserinformation.User_id;
                var branchCode = _workcontext.CurrentUserinformation.branch_code;
                var spbranchCode = sp.branchCode == "" ? _workcontext.CurrentUserinformation.branch_code : sp.branchCode;
                var companyCode = _workcontext.CurrentUserinformation.company_code;
                var result = "";
                int planCode = 0;
                string time_frame_code = sp.TIME_FRAME_CODE;

                var company_code = _workcontext.CurrentUserinformation.company_code;

                // first insertion to pl_sales_plan table
                string checkPlanQuery = $@"SELECT COUNT(*) FROM PL_SALES_PLAN WHERE DELETED_FLAG='N' AND PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                //var checkPlanQueryResult = this._dbContext.ExecuteSqlCommand(checkPlanQuery);
                var checkPlanQueryResult = this._dbContext.SqlQuery<int>(checkPlanQuery).First();
                if (checkPlanQueryResult == 0 && string.IsNullOrEmpty(sp.PLAN_CODE))
                {
                    if (sp.PLAN_FOR.ToLower() == "quantity")
                    {
                        sp.SALES_AMOUNT = string.Empty;
                    }
                    else if (sp.PLAN_FOR.ToLower() == "amount")
                    {
                        sp.SALES_QUANTITY = string.Empty;
                    }
                    string sales_plan_query = $@"INSERT INTO PL_SALES_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    LAST_MODIFIED_BY,LAST_MODIFIED_DATE,
                    DELETED_FLAG,SALES_PRICE_TYPE)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_SALES_PLAN),'{sp.PLAN_EDESC}','','{sp.SALES_QUANTITY}','{sp.SALES_AMOUNT}','{sp.TIME_FRAME_CODE}','{sp.CALENDAR_TYPE}',
                    TO_DATE('{sp.START_DATE}','YYYY-Mon-DD'),TO_DATE('{sp.END_DATE}','YYYY-Mon-DD'),'{sp.REMARKS}','{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'N','{sp.salesRateType}')";

                    var insertResult = this._dbContext.ExecuteSqlCommand(sales_plan_query);

                    string fetchSalesPlan = $@"SELECT PLAN_CODE FROM PL_SALES_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                    planCode = this._dbContext.SqlQuery<int>(fetchSalesPlan).First();

                }
                else
                {
                    //planCode = Convert.ToInt32(sp.PLAN_CODE);

                    if (!string.IsNullOrEmpty(sp.PLAN_CODE) && sp.PLAN_CODE != "0")
                    {
                        var checkApprovedFlagQry = $@"SELECT APPROVED_FLAG FROM PL_SALES_PLAN WHERE PLAN_CODE='{sp.PLAN_CODE}'";
                        var approvedFlag = _dbContext.SqlQuery<string>(checkApprovedFlagQry).FirstOrDefault();
                        if (approvedFlag == "Y")
                        {
                            return result = "APPROVED_FLAG";
                        }
                        planCode = Convert.ToInt32(sp.PLAN_CODE);
                        string updateQuery = $@"UPDATE PL_SALES_PLAN SET PLAN_EDESC = '{sp.PLAN_EDESC}' , REMARKS = '{sp.REMARKS}' , SALES_PRICE_TYPE ='{sp.salesRateType}' 
                        ,TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' , CALENDAR_TYPE='{sp.CALENDAR_TYPE}', START_DATE=TO_DATE('{sp.START_DATE}','YYYY-MON-DD') , END_DATE=TO_DATE('{sp.END_DATE}','YYYY-MON-DD')
                        WHERE PLAN_CODE='{sp.PLAN_CODE}'";
                        var update_result = this._dbContext.ExecuteSqlCommand(updateQuery);
                    }
                    else
                    {
                        string plancode_query = $@"SELECT PLAN_CODE FROM PL_SALES_PLAN WHERE PLAN_EDESC='{sp.PLAN_EDESC}' 
                AND TIME_FRAME_CODE='{sp.TIME_FRAME_CODE}' AND START_DATE=TO_DATE('{sp.START_DATE}','YYYY-Mon-DD') AND END_DATE=TO_DATE('{sp.END_DATE}','YYYY-Mon-DD')";
                        planCode = this._dbContext.SqlQuery<int>(plancode_query).First();
                    }

                    string delete_detail_already_set = $@"DELETE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE='{planCode}'";
                    this._dbContext.ExecuteSqlCommand(delete_detail_already_set);
                }

                string sbInsertQuery = string.Empty;
                //sbInsertQuery += "INSERT ALL ";
                //sbInsertQuery += @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                //    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,REMARKS,
                //    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) ";


                List<sa_sales_invoice_viewmodel> salesInvoice = new List<sa_sales_invoice_viewmodel>();
                string averageQuery = $@"SELECT CALC_TOTAL_PRICE,CALC_QUANTITY, ITEM_CODE FROM SA_SALES_INVOICE WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.itemCode))})";
                salesInvoice = this._dbContext.SqlQuery<sa_sales_invoice_viewmodel>(averageQuery).ToList();

                string itemQuery = $@"SELECT PURCHASE_PRICE AS CALC_TOTAL_PRICE,ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}'  AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.itemCode))})";
                var itemsFromItemMaster = this._dbContext.SqlQuery<sa_sales_invoice_viewmodel>(itemQuery).ToList();
                //
                List<FrequencyColumnModel> allDaysObj = new List<FrequencyColumnModel>();
                List<YearMonthWeekModel> allYearWeekObj = new List<YearMonthWeekModel>();
                //eachday from start to end date
                allDaysObj = getAllDaysBetweenDateRange(sp.dateFormat, sp.TIME_FRAME_EDESC, sp.START_DATE, sp.END_DATE);
                //all year-month or year-week list from start to end date
                allYearWeekObj = getAllYearMonthWeek(sp.dateFormat, sp.TIME_FRAME_EDESC, sp.START_DATE, sp.END_DATE);
                //
                string[] insert_query_array = new string[sv.Count()];
                int itemCount = 0,
                    itemRange = 5,
                    array_count = 0,
                    itteration = 0,
                    remainderCount = 0;
                remainderCount = (sv.Count() % itemRange);
                foreach (var item in sv)
                {
                    itemCount++;
                    itteration++;
                    // start sales price per item.
                    string averagePriceQuery = string.Empty;
                    var salesPriceResult = string.Empty;
                    //if (sp.salesRateType == "AVERAGE_SALES_PRICE")
                    //{
                    //    salesPriceResult = salesInvoice.Where(a => a.ITEM_CODE == item.itemCode).FirstOrDefault() != null ? (salesInvoice.Where(a => a.ITEM_CODE == item.itemCode).Sum(a => a.CALC_TOTAL_PRICE) / salesInvoice.Where(a => a.ITEM_CODE == item.itemCode).Sum(a => a.CALC_QUANTITY)).ToString() : null;
                    //    if (string.IsNullOrEmpty(salesPriceResult))
                    //    {
                    //        averagePriceQuery = $@"SELECT TO_CHAR(SALES_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.itemCode}' AND ROWNUM <=1 ORDER BY APP_DATE DESC ";
                    //        salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                    //        if (salesPriceResult == null)
                    //        {
                    //            averagePriceQuery = $@"SELECT TO_CHAR(STANDARD_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_SCHEDULE_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.itemCode}' AND ROWNUM <= 1 ORDER BY EFFECTIVE_DATE DESC";
                    //            salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                    //            if (salesPriceResult == null)
                    //            {
                    //                salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).First().CALC_TOTAL_PRICE.ToString();
                    //            }
                    //        }
                    //    }
                    //}
                    //else if (sp.salesRateType == "STANDARD_SALES_PRICE")
                    //{
                    //    averagePriceQuery = $@"SELECT TO_CHAR(SALES_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.itemCode}' AND ROWNUM <=1 ORDER BY APP_DATE DESC ";
                    //    salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                    //    if (salesPriceResult == null)
                    //    {
                    //        averagePriceQuery = $@"SELECT TO_CHAR(STANDARD_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_SCHEDULE_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.itemCode}' AND ROWNUM <= 1 ORDER BY EFFECTIVE_DATE DESC";
                    //        salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();

                    //        if (salesPriceResult == null)
                    //        {
                    //            salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).First().CALC_TOTAL_PRICE.ToString();
                    //        }
                    //    }
                    //}
                    //else if (sp.salesRateType == "LANDED_COST")
                    //{
                    //    salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.itemCode).First().CALC_TOTAL_PRICE.ToString();
                    //}


                    if (string.IsNullOrEmpty(salesPriceResult) || salesPriceResult == "0")
                    {
                        salesPriceResult = "1";
                    }
                    // end
                    var totalMonthQty = 0M;
                    foreach (var freq in item.frequency)
                    {
                        if (freq.fvalue != "")
                        {
                            totalMonthQty = totalMonthQty + Convert.ToDecimal(freq.fvalue);
                        }
                    }
                    foreach (var freq in item.frequency)
                    {
                        try
                        {
                            string[] freq_date = null;
                            int interval = 0;
                            //getEachDateFrequency(freq, sp.START_DATE, sp.END_DATE, sp.TIME_FRAME_CODE, sp.TIME_FRAME_EDESC, sp.dateFormat, out freq_date, out interval);
                            //interval = freq_date.Length;
                            string freValue = freq.fname.Split('_')[0]; // month or week
                            string freYear = freq.fname.Split('_')[1]; // year

                            string freYearVal = freYear + '-' + freValue;

                            List<FrequencyColumnModel> allYearByNYM = new List<FrequencyColumnModel>();
                            allYearByNYM = getAllDaysByNepaliYearMonth(sp.dateFormat, sp.TIME_FRAME_CODE, sp.START_DATE, sp.END_DATE);

                            if (sp.TIME_FRAME_EDESC.ToLower() == "week" && (sp.dateFormat.ToLower() == "ad" || sp.dateFormat.ToLower() == "eng"))
                            {
                                freYear = freq.fname.Split('_')[0];
                                freValue = freq.fname.Split('_')[1];
                            }
                            if (sp.dateFormat.ToLower() == "loc" || sp.dateFormat.ToLower() == "bs")
                            {
                                if (sp.TIME_FRAME_EDESC.ToLower() == "week")
                                {
                                    string yearmonth = allYearWeekObj.Where(a => a.NWEEK == freYear).First().YEARWEEK;
                                    freValue = yearmonth.Split('-')[1];
                                    freYear = yearmonth.Split('-')[0];
                                }
                                else
                                {
                                    string bsyearmonth = allYearWeekObj.Where(a => a.BS_MONTH == freYear + "-" + freValue).First().YEARWEEK;
                                    freValue = bsyearmonth.Split('-')[1];
                                    freYear = bsyearmonth.Split('-')[0];
                                }
                            }

                            if (sp.TIME_FRAME_EDESC.Trim().ToLower() == "week")
                            {
                                freq_date = allDaysObj.Where(a => Convert.ToInt32(a.WEEKS) == Convert.ToInt32(freValue) && a.YEAR == freYear).Select(a => a.DAYS).ToArray();
                            }
                            else if (sp.TIME_FRAME_EDESC.Trim().ToLower() == "month")
                            {
                                freq_date = allDaysObj.Where(a => Convert.ToInt32(a.MONTHINT) == Convert.ToInt32(freValue) && a.YEAR == freYear).Select(a => a.DAYS).ToArray();

                                if (sp.dateFormat.ToLower() == "loc" || sp.dateFormat.ToLower() == "bs")
                                {
                                    freq_date = allYearByNYM.Where(a => a.YEARWEEK == freYearVal).Select(a => a.DAYS).ToArray();
                                }
                            }
                            interval = freq_date.Length;
                            // each day insertion loop.
                            foreach (var date in freq_date)
                            {
                                var eachday_value = Math.Round((Convert.ToDecimal(freq.fvalue) / interval), 5);

                                var eachday_quantity_value = 0M;
                                var eachday_amount_value = 0M;


                                if (sp.PLAN_FOR.ToLower() == "quantity")
                                {
                                    eachday_quantity_value = eachday_value;
                                    var eachday_amount = 0M;
                                    var rate = Math.Round(((Convert.ToDecimal(freq.fvalue_amt) / totalMonthQty)), 5);
                                    if (freq.fvalue_amt != null)
                                        eachday_amount = Math.Round(((rate * (Convert.ToDecimal(freq.fvalue) / interval))), 5);
                                    else
                                        eachday_amount = eachday_value;
                                    eachday_amount_value = eachday_amount;//Math.Round(eachday_value * Convert.ToDecimal(salesPriceResult), 5);
                                }
                                else if (sp.PLAN_FOR.ToLower() == "amount")
                                {
                                    eachday_quantity_value = Math.Round(eachday_value / Convert.ToDecimal(salesPriceResult), 5);
                                    eachday_amount_value = eachday_value;
                                }
                                //string frequency_json = @"{frequency_name:'" + sp.TIME_FRAME_EDESC +
                                //    "',frequency_code:'" + sp.TIME_FRAME_CODE +
                                //    "',frequency_value:'" + freq.fname + "',amount_quantity_value:'" + freq.fvalue + "'}";
                                string frequency_json = @"fname__" + freq.fname + "__fvalue__" + freq.fvalue;

                                //insert for each day
                                //string insertinto_plandtl = $@"INTO PL_SALES_PLAN_DTL
                                //(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                                //CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,REMARKS,
                                //COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED)
                                //VALUES
                                //('{planCode}',TO_DATE('{date}','DD-MON-YYYY'),'{eachday_quantity_value}','{eachday_amount_value}','{item.itemCode}',
                                //'{sp.customerCode}','{sp.employeeCode}','{sp.divisionCode}','',
                                //'{companyCode}','{spbranchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N','{frequency_json}','{salesPriceResult}') ";
                                string insertinto_plandtl = $@"SELECT '{planCode}',TO_DATE('{date}','DD-MON-YYYY'),'{eachday_quantity_value}','{eachday_amount_value}','{item.itemCode}',
                                '{sp.customerCode}','{sp.employeeCode}','{sp.divisionCode}','{sp.partytypeCode}','{sp.agentCode}','',
                                '{companyCode}','{spbranchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N','{frequency_json}','{salesPriceResult}' FROM DUAL UNION ALL ";

                                sbInsertQuery += insertinto_plandtl;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("unique constraint"))
                                throw;
                            else
                            {
                                var sqlquery = $@"UPDATE PL_PLAN_DTL SET TARGET_VALUE='{freq.fvalue}',LAST_MODIFIED_BY='{userID}', LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss') where PLAN_CODE='{planCode}' AND ITEM_CODE='{item.itemCode}' AND TIME_FRAME_CODE='{time_frame_code}' AND TIME_FRAME_VALUE='{freq.fname}'";
                            }
                        }
                    }
                    if (itteration <= (sv.Count() - remainderCount))
                    {
                        while (itemCount == itemRange)
                        {
                            string query = @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,AGENT_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) "
+ sbInsertQuery;
                            query = query.Substring(0, query.Length - 11);
                            //sbInsertQuery = sbInsertQuery.Substring(0, sbInsertQuery.Length - 11);
                            insert_query_array[array_count] = query;
                            array_count++;
                            itemCount = 0;
                            sbInsertQuery = string.Empty;

                        }
                    }
                    else
                    {
                        while (itemCount == remainderCount)
                        {
                            string query = @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,AGENT_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) "
+ sbInsertQuery;
                            query = query.Substring(0, query.Length - 11);
                            insert_query_array[array_count] = query;
                            array_count++;
                            itemCount = 0;
                            sbInsertQuery = string.Empty;
                        }
                    }
                }
                try
                {
                    //sbInsertQuery += "SELECT * FROM DUAL";
                    //sbInsertQuery = sbInsertQuery.Substring(0, sbInsertQuery.Length - 11);
                    //string inserinto_plandtl = sbInsertQuery.ToString();
                    //int stringLength = inserinto_plandtl.Length;
                    //result = _dbContext.ExecuteSqlCommand(inserinto_plandtl).ToString();
                    var insertItems = insert_query_array.Where(a => a != null);
                    foreach (var item in insertItems)
                    {
                        result = _dbContext.ExecuteSqlCommand(item.ToString()).ToString();

                    }
                    //string totalInsertQuery = string.Join("", insert_query_array);
                    //totalInsertQuery = totalInsertQuery.Substring(0, totalInsertQuery.Length - 11);
                    //result = _dbContext.ExecuteSqlCommand(totalInsertQuery).ToString();
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;
                }

                string update_amout_quantity = string.Empty;
                if (sp.PLAN_FOR.ToLower() == "quantity")
                {
                    update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_QUANTITY = (
                        SELECT ROUND(SUM(SPD.PER_DAY_QUANTITY)) FROM PL_SALES_PLAN_DTL SPD, IP_ITEM_MASTER_SETUP IMS
                        WHERE 
                        IMS.GROUP_SKU_FLAG ='I'
                        AND SPD.ITEM_CODE = IMS.ITEM_CODE
                           AND SPD.COMPANY_CODE=IMS.COMPANY_CODE
                        AND SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_AMOUNT=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
                }
                else if (sp.PLAN_FOR.ToLower() == "amount")
                {
                    update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_AMOUNT = (
                        SELECT ROUND(SUM(SPD.PER_DAY_AMOUNT)) FROM PL_SALES_PLAN_DTL SPD, IP_ITEM_MASTER_SETUP IMS
                        WHERE 
                        IMS.GROUP_SKU_FLAG ='I'
                        AND SPD.ITEM_CODE = IMS.ITEM_CODE
                          AND SPD.COMPANY_CODE=IMS.COMPANY_CODE
                        AND SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_QUANTITY=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
                }
                this._dbContext.ExecuteSqlCommand(update_amout_quantity);
                //subin changes
                var preference = GetPreferenceSetups().ToList().Where(x => x.PL_NAME == "SALES_PLAN").FirstOrDefault();
                if (preference.SHOW_ITEM == "N")
                {
                    if (sp.PLAN_FOR.ToLower() == "quantity")
                    {
                        update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_QUANTITY = (
                        SELECT ROUND(SUM(SPD.PER_DAY_QUANTITY)) FROM PL_SALES_PLAN_DTL SPD
                        WHERE SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_AMOUNT=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
                    }
                    else if (sp.PLAN_FOR.ToLower() == "amount")
                    {
                        update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_AMOUNT = (
                        SELECT ROUND(SUM(SPD.PER_DAY_AMOUNT)) FROM PL_SALES_PLAN_DTL SPD
                        WHERE SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_QUANTITY=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
                    }
                    this._dbContext.ExecuteSqlCommand(update_amout_quantity);
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<FrequencyColumnModel> getAllDaysByNepaliYearMonth(string dateFormat, string tIME_FRAME_EDESC, string sTART_DATE, string eND_DATE)
        {
            List<FrequencyColumnModel> allYearMonthWeekObj = new List<FrequencyColumnModel>();
            if (dateFormat == "LOC" || dateFormat == "BS")
            {
                if (tIME_FRAME_EDESC.ToLower() == "week")
                {
                    string engDateQuery = $@"select DISTINCT yearWeek,to_char(nweek) nweek,to_char(eweek) eweek from (SELECT  FLOOR (nepali_week / 7) + 1 nweek, days, TO_CHAR (days+1, 'IW') eweek, bs_date(days) nepali_date,  fn_bs_month(substr(bs_date(days),6,2)) as NMONTH--, to_char(days,'day') dd
                                                 ,(TO_CHAR(days,'YYYY')||'-'|| TO_CHAR (days+1, 'IW')) yearWeek                                                      
                                                    FROM (SELECT (  fiscal_year.start_date
                                                                    + ROWNUM
                                                                    - 1
                                                                    - NEXT_DAY (start_date, 'SUN'))
                                                                      nepali_week,
                                                                   TO_DATE (fiscal_year.start_date + ROWNUM - 1) days
                                                              FROM hr_fiscal_year_code fiscal_year, all_objects
                                                             WHERE sysdate BETWEEN start_date AND end_date
                                                                   AND ROWNUM <=
                                                                          fiscal_year.end_date - fiscal_year.start_date + 1)
                                                     WHERE nepali_week >= 0)
                                                     where days between TO_DATE('{sTART_DATE}','YYYY-Mon-DD') and TO_DATE('{eND_DATE}','YYYY-Mon-DD')";
                    allYearMonthWeekObj = this._dbContext.SqlQuery<FrequencyColumnModel>(engDateQuery).ToList();
                }
                else
                {
                    var query = $@"SELECT 
                                TO_CHAR(DAYSITEM,'MM') MONTHINT
                                ,TO_CHAR(DAYSITEM,'MON') MONTH
                                ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                                ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                                ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                                ,TO_CHAR(DAYSITEM,'DD') DAY
                                ,substr(bs_date(daysitem),0,7) YEARWEEK
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYSITEM FROM ALL_OBJECTS 
                                  ,(SELECT TO_DATE('{sTART_DATE}','YYYY-Mon-DD') START_DATE, TO_DATE('{eND_DATE}','YYYY-Mon-DD') END_DATE FROM DUAL) FISCAL_YEAR
                                  WHERE  ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR  
                                ORDER BY 
                                TO_CHAR(DAYSITEM,'YYYYMM'),DAYS";
                    allYearMonthWeekObj = this._dbContext.SqlQuery<FrequencyColumnModel>(query).ToList();
                }

            }
            return allYearMonthWeekObj;

        }
        private List<YearMonthWeekModel> getAllYearMonthWeek(string dateFormat, string tIME_FRAME_EDESC, string sTART_DATE, string eND_DATE)
        {
            List<YearMonthWeekModel> allYearMonthWeekObj = new List<YearMonthWeekModel>();
            if (dateFormat == "LOC" || dateFormat == "BS")
            {
                if (tIME_FRAME_EDESC.ToLower() == "week")
                {
                    string engDateQuery = $@"select DISTINCT yearWeek,to_char(nweek) nweek,to_char(eweek) eweek from (SELECT  FLOOR (nepali_week / 7) + 1 nweek, days, TO_CHAR (days+1, 'IW') eweek, bs_date(days) nepali_date,  fn_bs_month(substr(bs_date(days),6,2)) as NMONTH--, to_char(days,'day') dd
                                                 ,(TO_CHAR(days,'YYYY')||'-'|| TO_CHAR (days+1, 'IW')) yearWeek                                                      
                                                    FROM (SELECT (  fiscal_year.start_date
                                                                    + ROWNUM
                                                                    - 1
                                                                    - NEXT_DAY (start_date, 'SUN'))
                                                                      nepali_week,
                                                                   TO_DATE (fiscal_year.start_date + ROWNUM - 1) days
                                                              FROM hr_fiscal_year_code fiscal_year, all_objects
                                                             WHERE sysdate BETWEEN start_date AND end_date
                                                                   AND ROWNUM <=
                                                                          fiscal_year.end_date - fiscal_year.start_date + 1)
                                                     WHERE nepali_week >= 0)
                                                     where days between TO_DATE('{sTART_DATE}','YYYY-Mon-DD') and TO_DATE('{eND_DATE}','YYYY-Mon-DD')";
                    allYearMonthWeekObj = this._dbContext.SqlQuery<YearMonthWeekModel>(engDateQuery).ToList();
                }
                else
                {
                    string engDateQuery = $@"SELECT TO_CHAR(AD_DATE,'YYYY-MM') yearWeek,TO_CHAR(BS_MONTH) BS_MONTH FROM CALENDAR_SETUP ";
                    allYearMonthWeekObj = this._dbContext.SqlQuery<YearMonthWeekModel>(engDateQuery).ToList();
                }

            }
            return allYearMonthWeekObj;
        }


        private List<FrequencyColumnModel> getAllDaysBetweenDateRange(string dateFormat, string time_frame_edesc, string sTART_DATE, string eND_DATE)
        {
            List<FrequencyColumnModel> allDaysObj = new List<FrequencyColumnModel>();
            if (time_frame_edesc.ToLower() == "week")
            {
                var weekQuery = $@"SELECT DISTINCT TO_CHAR(DAYSITEM+(ROWNUM-1+7)/7,'IW')  WEEKS 
                                            ,TO_CHAR(DAYSITEM,'MM') MONTHINT
                                            ,TO_CHAR(DAYSITEM,'MON') MONTH
                                            ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                                            ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                                            ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                                            ,substr(bs_date(daysitem),0,7) YEARWEEK
                                              FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYSITEM FROM ALL_OBJECTS 
                                              ,(SELECT TO_DATE('{sTART_DATE}','YYYY-Mon-DD') START_DATE, TO_DATE('{eND_DATE}','YYYY-Mon-DD') END_DATE FROM DUAL) FISCAL_YEAR
                                              WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR
                                            ORDER BY 
                                            TO_CHAR(DAYSITEM,'YYYYMM'),DAYS";
                allDaysObj = this._dbContext.SqlQuery<FrequencyColumnModel>(weekQuery).ToList();
            }
            else if (time_frame_edesc.ToLower() == "month")
            {
                var monthQuery = $@"SELECT 
                                TO_CHAR(DAYSITEM,'MM') MONTHINT
                                ,TO_CHAR(DAYSITEM,'MON') MONTH
                                ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                                ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                                ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                                ,substr(bs_date(daysitem),0,7) YEARWEEK
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYSITEM FROM ALL_OBJECTS 
                                  ,(SELECT TO_DATE('{sTART_DATE}','YYYY-Mon-DD') START_DATE, TO_DATE('{eND_DATE}','YYYY-Mon-DD') END_DATE FROM DUAL) FISCAL_YEAR
                                  WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR
                                ORDER BY 
                                TO_CHAR(DAYSITEM,'YYYYMM'),DAYS";
                allDaysObj = this._dbContext.SqlQuery<FrequencyColumnModel>(monthQuery).ToList();
            }
            return allDaysObj;
        }
        //private void getAllDateFrequency(string sTART_DATE, string eND_DATE, string tIME_FRAME_CODE, string tIME_FRAME_EDESC, string CALANDER_TYPE, out string[] freq_date)
        //{

        //}

        private void getEachDateFrequency(freqNameVlaue freq, string sTART_DATE, string eND_DATE, string tIME_FRAME_CODE, string tIME_FRAME_EDESC, string CALANDER_TYPE, out string[] freq_date, out int interval)
        {
            try
            {
                string current_freq = freq.fname; // either week num or month no.
                DateTime inputStartDate = Convert.ToDateTime(sTART_DATE);
                DateTime inputEndDate = Convert.ToDateTime(eND_DATE);
                string[] dateRangeOfFreq = null;
                int dateInterval = 0;
                string freValue = freq.fname.Split('_')[0]; // month or week
                string freYear = freq.fname.Split('_')[1]; // year
                if (tIME_FRAME_EDESC.ToLower() == "week" && (CALANDER_TYPE.ToLower() == "ad" || CALANDER_TYPE.ToLower() == "eng"))
                {
                    freYear = freq.fname.Split('_')[0];
                    freValue = freq.fname.Split('_')[1];
                }

                // if calander_type=loc
                // get AD freValue & freYear w.r.t BS freValue & freyear
                if (CALANDER_TYPE == "LOC" || CALANDER_TYPE == "BS")
                {
                    if (tIME_FRAME_EDESC.ToLower() == "week")
                    {
                        string engDateQuery = $@"select DISTINCT yearWeek from (SELECT  FLOOR (nepali_week / 7) + 1 nweek, days, TO_CHAR (days+1, 'IW') eweek, bs_date(days) nepali_date,  fn_bs_month(substr(bs_date(days),6,2)) as NMONTH--, to_char(days,'day') dd
                                                 ,(TO_CHAR(days,'YYYY')||'-'|| TO_CHAR (days+1, 'IW')) yearWeek                                                      
                                                    FROM (SELECT (  fiscal_year.start_date
                                                                    + ROWNUM
                                                                    - 1
                                                                    - NEXT_DAY (start_date, 'SUN'))
                                                                      nepali_week,
                                                                   TO_DATE (fiscal_year.start_date + ROWNUM - 1) days
                                                              FROM hr_fiscal_year_code fiscal_year, all_objects
                                                             WHERE sysdate BETWEEN start_date AND end_date
                                                                   AND ROWNUM <=
                                                                          fiscal_year.end_date - fiscal_year.start_date + 1)
                                                     WHERE nepali_week >= 0)
                                                     where NWEEK={freYear} and days between TO_DATE('{sTART_DATE}','YYYY-Mon-DD') and TO_DATE('{eND_DATE}','YYYY-Mon-DD')";
                        string engDateResult = this._dbContext.SqlQuery<String>(engDateQuery).First();
                        freValue = engDateResult.Split('-')[1];
                        freYear = engDateResult.Split('-')[0];
                    }
                    else
                    {
                        string engDateQuery = $@"SELECT TO_CHAR(AD_DATE,'YYYY-MM') AD_DATE FROM CALENDAR_SETUP
                                        WHERE BS_MONTH='{freYear}-{freValue}'";
                        string engDateResult = this._dbContext.SqlQuery<String>(engDateQuery).First();
                        freValue = engDateResult.Split('-')[1];
                        freYear = engDateResult.Split('-')[0];
                    }

                }
                if (tIME_FRAME_EDESC.ToLower() == "week")
                {
                    var weekQuery = $@"SELECT DISTINCT TO_CHAR(DAYSITEM+(ROWNUM-1+7)/7,'IW')  WEEKS 
                                            ,TO_CHAR(DAYSITEM,'MM') MONTHINT
                                            ,TO_CHAR(DAYSITEM,'MON') MONTH
                                            ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                                            ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                                            ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                                              FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYSITEM FROM ALL_OBJECTS 
                                              ,(SELECT TO_DATE('{sTART_DATE}','YYYY-Mon-DD') START_DATE, TO_DATE('{eND_DATE}','YYYY-Mon-DD') END_DATE FROM DUAL) FISCAL_YEAR
                                              WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR
                                            ORDER BY
                                            TO_CHAR(DAYSITEM,'YYYYMM')";
                    var weekResult = this._dbContext.SqlQuery<FrequencyColumnModel>(weekQuery).ToList();
                    dateRangeOfFreq = weekResult.Where(a => Convert.ToInt32(a.WEEKS) == Convert.ToInt32(freValue) && a.YEAR == freYear).Select(a => a.DAYS).ToArray();
                    dateInterval = dateRangeOfFreq.Length;
                }
                else if (tIME_FRAME_EDESC.ToLower() == "month")
                {
                    var monthQuery = $@"SELECT 
                                TO_CHAR(DAYSITEM,'MM') MONTHINT
                                ,TO_CHAR(DAYSITEM,'MON') MONTH
                                ,TO_CHAR(DAYSITEM,'YYYY') YEAR 
                                ,TO_CHAR(DAYSITEM,'YYYYMM') YEARMONTH 
                                ,TO_CHAR(DAYSITEM,'DD-MON-YYYY') DAYS
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYSITEM FROM ALL_OBJECTS 
                                  ,(SELECT TO_DATE('{sTART_DATE}','YYYY-Mon-DD') START_DATE, TO_DATE('{eND_DATE}','YYYY-Mon-DD') END_DATE FROM DUAL) FISCAL_YEAR
                                  WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) ALLDAYOFYEAR
                                ORDER BY
                                TO_CHAR(DAYSITEM,'YYYYMM')";
                    var monthResult = this._dbContext.SqlQuery<FrequencyColumnModel>(monthQuery).ToList();
                    dateRangeOfFreq = monthResult.Where(a => Convert.ToInt32(a.MONTHINT) == Convert.ToInt32(freValue) && a.YEAR == freYear).Select(a => a.DAYS).ToArray();
                    dateInterval = DateTime.DaysInMonth(Convert.ToInt32(freYear), Convert.ToInt32(freValue));
                }


                freq_date = dateRangeOfFreq;
                interval = dateInterval;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SalesPlan GetPlanDetailValueByPlanCode(int plancode)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            SalesPlan salesPlanList = new SalesPlan();
            string queryPlan = $@"SELECT 
                TO_CHAR(PLAN_CODE) PLAN_CODE, PLAN_EDESC,PLAN_NDESC,TO_CHAR(SALES_QUANTITY) SALES_QUANTITY,TO_CHAR(SALES_AMOUNT) SALES_AMOUNT,TO_CHAR(TIME_FRAME_CODE) TIME_FRAME_CODE,SALES_PRICE_TYPE AS salesRateType,
                (SELECT TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = PSP.TIME_FRAME_CODE) TIME_FRAME_EDESC,
                (SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = (SELECT CUSTOMER_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PSP.PLAN_CODE AND COMPANY_CODE='{company_code}' AND ROWNUM=1) AND COMPANY_CODE='{company_code}') customerCode,
                (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = (SELECT CUSTOMER_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PSP.PLAN_CODE  AND COMPANY_CODE='{company_code}' AND ROWNUM=1) AND COMPANY_CODE='{company_code}') CUSTOMER_EDESC,
                CALENDAR_TYPE,TO_CHAR(START_DATE,'YYYY-Mon-DD') START_DATE,TO_CHAR(END_DATE,'YYYY-Mon-DD') END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE
                FROM PL_SALES_PLAN PSP
                WHERE PLAN_CODE= '{plancode}'";
            salesPlanList = this._dbContext.SqlQuery<SalesPlan>(queryPlan).FirstOrDefault();

            List<SalesPlanDetail> salesPlanDetailList = new List<SalesPlanDetail>();
            string querySalesPlanDetail = $@"SELECT TO_CHAR(SPD.PLAN_CODE) PLAN_CODE,TO_CHAR(SPD.PLAN_DATE) PLAN_DATE,TO_CHAR(PER_DAY_QUANTITY) PER_DAY_QUANTITY,TO_CHAR(PER_DAY_AMOUNT) PER_DAY_AMOUNT,SPD.AGENT_CODE,
                SPD.ITEM_CODE,SPD.CUSTOMER_CODE,SPD.EMPLOYEE_CODE,SPD.DIVISION_CODE,SPD.PARTY_TYPE_CODE,SPD.REMARKS,SPD.COMPANY_CODE,SPD.BRANCH_CODE,SPD.CREATED_BY,TO_CHAR(SPD.CREATED_DATE) CREATED_DATE, FREQUENCY_JSON, IMS.GROUP_SKU_FLAG
                FROM PL_SALES_PLAN_DTL SPD,IP_ITEM_MASTER_SETUP IMS WHERE IMS.ITEM_CODE = SPD.ITEM_CODE  AND IMS.COMPANY_CODE = SPD.COMPANY_CODE AND SPD.DELETED_FLAG='N' AND PLAN_CODE = '{plancode}'";
            salesPlanDetailList = this._dbContext.SqlQuery<SalesPlanDetail>(querySalesPlanDetail).ToList();

            List<SalesPlanItems> salesPlanItem = new List<SalesPlanItems>();
            string queryPlanItem = $@"SELECT 
            DISTINCT TO_CHAR(PSPD.ITEM_CODE) ITEM_CODE,INITCAP(IMS.ITEM_EDESC) ITEM_EDESC,TO_CHAR(IMS.MASTER_ITEM_CODE) MASTER_ITEM_CODE,TO_CHAR(IMS.PRE_ITEM_CODE) PRE_ITEM_CODE,TO_CHAR(IMS.GROUP_SKU_FLAG) GROUP_SKU_FLAG
            FROM PL_SALES_PLAN_DTL PSPD,IP_ITEM_MASTER_SETUP IMS WHERE PSPD.PLAN_CODE='{plancode}' AND PSPD.DELETED_FLAG='N' AND IMS.DELETED_FLAG='N' AND IMS.ITEM_CODE=PSPD.ITEM_CODE AND IMS.COMPANY_CODE = PSPD.COMPANY_CODE AND PSPD.COMPANY_CODE = '{company_code}'
            ORDER BY PRE_ITEM_CODE,MASTER_ITEM_CODE,ITEM_CODE";
            
            salesPlanItem = this._dbContext.SqlQuery<SalesPlanItems>(queryPlanItem).ToList();

            salesPlanList.selectedItemsList = salesPlanItem;
            salesPlanList.salesPlanDetail = salesPlanDetailList;

            return salesPlanList;
        }
        public SalesPlan GetSalesPlanDetailValueByPlanCode(int plancode)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            SalesPlan salesPlanList = new SalesPlan();
            string queryPlan = $@"SELECT 
                TO_CHAR(PLAN_CODE) PLAN_CODE, PLAN_EDESC,PLAN_NDESC,TO_CHAR(SALES_QUANTITY) SALES_QUANTITY,TO_CHAR(SALES_AMOUNT) SALES_AMOUNT,TO_CHAR(TIME_FRAME_CODE) TIME_FRAME_CODE,SALES_PRICE_TYPE AS salesRateType,
                (SELECT TIME_FRAME_EDESC FROM PL_TIME_FRAME WHERE TIME_FRAME_CODE = PSP.TIME_FRAME_CODE) TIME_FRAME_EDESC,
                (SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = (SELECT CUSTOMER_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PSP.PLAN_CODE AND COMPANY_CODE='{company_code}' AND ROWNUM=1) AND COMPANY_CODE='{company_code}') customerCode,
                (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = (SELECT CUSTOMER_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PSP.PLAN_CODE  AND COMPANY_CODE='{company_code}' AND ROWNUM=1) AND COMPANY_CODE='{company_code}') CUSTOMER_EDESC,
                CALENDAR_TYPE,TO_CHAR(START_DATE,'YYYY-Mon-DD') START_DATE,TO_CHAR(END_DATE,'YYYY-Mon-DD') END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE
                FROM PL_SALES_PLAN PSP
                WHERE PLAN_CODE= '{plancode}'";
            salesPlanList = this._dbContext.SqlQuery<SalesPlan>(queryPlan).FirstOrDefault();

            List<SalesPlanDetail> salesPlanDetailList = new List<SalesPlanDetail>();
            string querySalesPlanDetail = $@"SELECT TO_CHAR(SPD.PLAN_CODE) PLAN_CODE,TO_CHAR(SPD.PLAN_DATE) PLAN_DATE,TO_CHAR(PER_DAY_QUANTITY) PER_DAY_QUANTITY,TO_CHAR(PER_DAY_AMOUNT) PER_DAY_AMOUNT,SPD.AGENT_CODE,
                SPD.ITEM_CODE,SPD.CUSTOMER_CODE,SPD.EMPLOYEE_CODE,SPD.DIVISION_CODE,SPD.PARTY_TYPE_CODE,SPD.REMARKS,SPD.COMPANY_CODE,SPD.BRANCH_CODE,SPD.CREATED_BY,TO_CHAR(SPD.CREATED_DATE) CREATED_DATE, FREQUENCY_JSON, IMS.GROUP_SKU_FLAG
                FROM PL_SALES_PLAN_DTL SPD,IP_ITEM_MASTER_SETUP IMS WHERE IMS.ITEM_CODE = SPD.ITEM_CODE  AND IMS.COMPANY_CODE = SPD.COMPANY_CODE AND SPD.DELETED_FLAG='N' AND PLAN_CODE = '{plancode}'";
            salesPlanDetailList = this._dbContext.SqlQuery<SalesPlanDetail>(querySalesPlanDetail).ToList();

            List<SalesPlanItems> salesPlanItem = new List<SalesPlanItems>();
            string queryPlanItem = $@"SELECT 
            DISTINCT TO_CHAR(PSPD.ITEM_CODE) ITEM_CODE,INITCAP(IMS.ITEM_EDESC) ITEM_EDESC,TO_CHAR(IMS.MASTER_ITEM_CODE) MASTER_ITEM_CODE,TO_CHAR(IMS.PRE_ITEM_CODE) PRE_ITEM_CODE,TO_CHAR(IMS.GROUP_SKU_FLAG) GROUP_SKU_FLAG
            FROM PL_SALES_PLAN_DTL PSPD,IP_ITEM_MASTER_SETUP IMS WHERE PSPD.PLAN_CODE='{plancode}' AND PSPD.DELETED_FLAG='N' AND IMS.DELETED_FLAG='N' AND IMS.ITEM_CODE=PSPD.ITEM_CODE AND IMS.COMPANY_CODE = PSPD.COMPANY_CODE AND PSPD.COMPANY_CODE = '{company_code}'
            ORDER BY PRE_ITEM_CODE,MASTER_ITEM_CODE,ITEM_CODE";

            salesPlanItem = this._dbContext.SqlQuery<SalesPlanItems>(queryPlanItem).ToList();
            var parentItemsQry = $@" SELECT DISTINCT TO_CHAR(ITEM_CODE)ITEM_CODE, INITCAP(ITEM_EDESC) ITEM_EDESC,MASTER_ITEM_CODE,PRE_ITEM_CODE, GROUP_SKU_FLAG FROM IP_ITEM_MASTER_SETUP
                                    WHERE COMPANY_CODE='{company_code}' AND DELETED_FLAG='N' AND GROUP_SKU_FLAG='G'
                                    START WITH MASTER_ITEM_CODE IN (SELECT MASTER_ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE IN
                                    ('{String.Join("','", salesPlanItem.Select(s => s.ITEM_CODE))}') AND DELETED_FLAG='N' AND COMPANY_CODE='{company_code}')
                                    CONNECT BY PRIOR PRE_ITEM_CODE =  MASTER_ITEM_CODE AND DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'";
            var parentItemList = _dbContext.SqlQuery<SalesPlanItems>(parentItemsQry).ToList();
            salesPlanItem.AddRange(parentItemList);
            salesPlanItem = salesPlanItem.GroupBy(x => x.ITEM_CODE).Select(y => y.First()).Distinct().OrderBy(p => Convert.ToInt32(p.ITEM_CODE)).ToList();
            salesPlanList.selectedItemsList = salesPlanItem;
            salesPlanList.salesPlanDetail = salesPlanDetailList;

            return salesPlanList;
        }
        public SalesPlan GetMasterPlanDetailValueByMasterPlanCode(int plancode)
        {
            try
            {
                var company_code = this._workcontext.CurrentUserinformation.company_code;
                SalesPlan salesPlanList = new SalesPlan();

                string queryPlan = $@"SELECT TO_CHAR(MSP.MASTER_PLAN_CODE) PLAN_CODE, MSP.MASTER_PLAN_EDESC PLAN_EDESC,TO_CHAR(MSP.START_DATE,'YYYY-Mon-DD') START_DATE,TO_CHAR(MSP.END_DATE,'YYYY-Mon-DD') END_DATE,
                                 TO_CHAR((SELECT SUM(SALES_QUANTITY) FROM PL_SALES_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND PLAN_CODE IN (SELECT PLAN_CODE FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE = '{plancode}'))) SALES_QUANTITY, 
                                  TO_CHAR((SELECT SUM(SALES_AMOUNT) FROM PL_SALES_PLAN WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND PLAN_CODE IN (SELECT PLAN_CODE FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE = '{plancode}'))) SALES_AMOUNT
                                 FROM PL_MASTER_SALES_PLAN MSP
                                INNER JOIN PL_SALES_PLAN_MAP SPM ON MSP.MASTER_PLAN_CODE = SPM.MASTER_PLAN_CODE
                                WHERE MSP.DELETED_FLAG='N' AND MSP.COMPANY_CODE='{company_code}' AND MSP.MASTER_PLAN_CODE='{plancode}'
                                GROUP BY MSP.MASTER_PLAN_CODE,MSP.MASTER_PLAN_EDESC,MSP.START_DATE,MSP.END_DATE";
                salesPlanList = this._dbContext.SqlQuery<SalesPlan>(queryPlan).FirstOrDefault();

                List<SalesPlanDetail> salesPlanDetailList = new List<SalesPlanDetail>();
                string querySalesPlanDetail = $@"SELECT TO_CHAR(SPD.PLAN_CODE) PLAN_CODE,TO_CHAR(SPD.PLAN_DATE) PLAN_DATE,TO_CHAR(PER_DAY_QUANTITY) PER_DAY_QUANTITY,TO_CHAR(PER_DAY_AMOUNT) PER_DAY_AMOUNT,
                        SPD.ITEM_CODE,SPD.CUSTOMER_CODE,SPD.EMPLOYEE_CODE,SPD.DIVISION_CODE,SPD.PARTY_TYPE_CODE,SPD.REMARKS,SPD.COMPANY_CODE,SPD.BRANCH_CODE,SPD.CREATED_BY,TO_CHAR(SPD.CREATED_DATE) CREATED_DATE, FREQUENCY_JSON, IMS.GROUP_SKU_FLAG
                        FROM PL_SALES_PLAN_DTL SPD,IP_ITEM_MASTER_SETUP IMS WHERE IMS.ITEM_CODE = SPD.ITEM_CODE  AND IMS.COMPANY_CODE = SPD.COMPANY_CODE AND SPD.DELETED_FLAG='N' 
                        AND PLAN_CODE IN (SELECT DISTINCT PLAN_CODE FROM PL_SALES_PLAN_MAP
                        START WITH MASTER_PLAN_CODE = '{plancode}'
                        CONNECT BY PRIOR PARENT_PLAN_CODE = MASTER_PLAN_CODE)";
                salesPlanDetailList = this._dbContext.SqlQuery<SalesPlanDetail>(querySalesPlanDetail).ToList();

                List<SalesPlanItems> salesPlanItem = new List<SalesPlanItems>();
                string queryPlanItem = $@"SELECT 
                        DISTINCT TO_CHAR(PSPD.ITEM_CODE) ITEM_CODE,INITCAP(IMS.ITEM_EDESC) ITEM_EDESC,TO_CHAR(IMS.MASTER_ITEM_CODE) MASTER_ITEM_CODE,TO_CHAR(IMS.PRE_ITEM_CODE) PRE_ITEM_CODE,TO_CHAR(IMS.GROUP_SKU_FLAG) GROUP_SKU_FLAG
                        FROM PL_SALES_PLAN_DTL PSPD,IP_ITEM_MASTER_SETUP IMS WHERE PSPD.PLAN_CODE IN(SELECT DISTINCT PLAN_CODE FROM PL_SALES_PLAN_MAP
                        START WITH MASTER_PLAN_CODE = '{plancode}'
                        CONNECT BY PRIOR PARENT_PLAN_CODE = MASTER_PLAN_CODE) AND PSPD.DELETED_FLAG='N' AND IMS.GROUP_SKU_FLAG='I'
                        AND IMS.DELETED_FLAG='N' AND IMS.ITEM_CODE=PSPD.ITEM_CODE AND IMS.COMPANY_CODE = PSPD.COMPANY_CODE AND PSPD.COMPANY_CODE = '{company_code}'
                        ORDER BY PRE_ITEM_CODE,MASTER_ITEM_CODE,ITEM_CODE";
                

                salesPlanItem = this._dbContext.SqlQuery<SalesPlanItems>(queryPlanItem).ToList();

                var parentItemsQry = $@" SELECT DISTINCT TO_CHAR(ITEM_CODE)ITEM_CODE, INITCAP(ITEM_EDESC) ITEM_EDESC,MASTER_ITEM_CODE,PRE_ITEM_CODE, GROUP_SKU_FLAG FROM IP_ITEM_MASTER_SETUP
                                    WHERE COMPANY_CODE='{company_code}' AND DELETED_FLAG='N' AND GROUP_SKU_FLAG='G'
                                    START WITH MASTER_ITEM_CODE IN (SELECT MASTER_ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE IN
                                    ('{String.Join("','",salesPlanItem.Select(s=>s.ITEM_CODE))}') AND DELETED_FLAG='N' AND COMPANY_CODE='{company_code}')
                                    CONNECT BY PRIOR PRE_ITEM_CODE =  MASTER_ITEM_CODE AND DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'";
                var parentItemList = _dbContext.SqlQuery<SalesPlanItems>(parentItemsQry).ToList();
                salesPlanItem.AddRange(parentItemList);

                salesPlanItem = salesPlanItem.Select(x => x).Distinct().OrderBy(p=>p.PRE_ITEM_CODE).ToList();

                salesPlanList.selectedItemsList = salesPlanItem;
                salesPlanList.salesPlanDetail = salesPlanDetailList;

                return salesPlanList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public string getSalesInvoiceQueryForReference(string dateFormat, string frequency, string FiscalYear, string itemList, string optionalCondition, string startDate, string endDate)
        {
            var refrenceQuery = string.Empty;
            if (dateFormat.ToUpper() == "BS" || dateFormat.ToUpper() == "LOC")
            {
                if (frequency.ToLower() == "week")
                {
                    refrenceQuery = $@" SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' || NEPALI_WEEKS.WEEKS ||'_'||  TO_CHAR(SUBSTR(BS_DATE(SI.SALES_DATE),0,4) )  COLNAME, SI.ITEM_CODE,
                                         IMS.ITEM_EDESC,
                                         SI.CUSTOMER_CODE,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS,
                                         TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                        TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                    FROM {FiscalYear}.SA_SALES_INVOICE SI, {FiscalYear}.IP_ITEM_MASTER_SETUP IMS, (SELECT DAYS, ROWNUM  DAY_NO , TRUNC((ROWNUM+7)/7)  WEEKS
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS
                                          FROM ALL_OBJECTS,
                                               (SELECT START_DATE, END_DATE
                                                  FROM HR_FISCAL_YEAR_CODE
                                                 WHERE SYSDATE BETWEEN START_DATE AND END_DATE) FISCAL_YEAR
                                         WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) )NEPALI_WEEKS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                  and IMS.COMPANY_CODE=SI.COMPANY_CODE
                                    AND NEPALI_WEEKS.DAYS = SI.SALES_DATE
                                    AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                                GROUP BY SI.ITEM_CODE,
                                         IMS.ITEM_EDESC,
                                         SI.CUSTOMER_CODE,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS
                                        ,SUBSTR(BS_DATE(SI.SALES_DATE),0,4)  
                                 ORDER BY   NEPALI_WEEKS.WEEKS";
                }
                else if (frequency.ToLower() == "month")
                {
                    refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.SALES_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.SALES_DATE),0,4) COLNAME ,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    -- SUBSTR(TO_BS(SI.SALES_DATE),7,7) NEPALI_MONTH,
                                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM {FiscalYear}.SA_SALES_INVOICE SI, {FiscalYear}.IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                 and IMS.COMPANY_CODE=SI.COMPANY_CODE
                                AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     SUBSTR(BS_DATE(SI.SALES_DATE),0,4),
                                     fn_bs_month(SUBSTR(BS_DATE(SI.SALES_DATE),6,2))";
                }
            }
            else if (dateFormat.ToUpper() == "AD" || dateFormat.ToUpper() == "ENG")
            {
                if (frequency.ToLower() == "week")
                {
                    refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' ||  TO_CHAR(SI.SALES_DATE,'IW') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME, SI.ITEM_CODE,
                                       IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.SALES_DATE,'YYYY') AD_YEAR,
                                     TO_CHAR(SI.SALES_DATE,'IW') SALES_WEEK,
                                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM {FiscalYear}.SA_SALES_INVOICE SI, {FiscalYear}.IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                and IMS.COMPANY_CODE=SI.COMPANY_CODE
                                AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                           GROUP BY SI.ITEM_CODE,
                             IMS.ITEM_EDESC,
                             SI.CUSTOMER_CODE,
                             SI.DIVISION_CODE,
                             SI.BRANCH_CODE,
                             TO_CHAR(SI.SALES_DATE,'YYYY'),
                             TO_CHAR(SI.SALES_DATE,'IW')
                     ORDER BY  TO_CHAR(SI.SALES_DATE,'YYYY'), TO_CHAR(SI.SALES_DATE,'IW')";
                }
                else if (frequency.ToLower() == "month")
                {
                    refrenceQuery = $@"SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_' ||  TO_CHAR(SI.SALES_DATE,'MON') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME ,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    -- SUBSTR(TO_BS(SI.SALES_DATE),7,7) NEPALI_MONTH,
                                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM {FiscalYear}.SA_SALES_INVOICE SI, {FiscalYear}.IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                and IMS.COMPANY_CODE=SI.COMPANY_CODE
                                AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.SALES_DATE,'YYYY'),
                                     TO_CHAR(SI.SALES_DATE,'MON')";
                }
            }
            return refrenceQuery;
        }
        public string getSalesOrderQueryForReference(string dateFormat, string frequency, string FiscalYear, string itemList, string optionalCondition, string startDate, string endDate)
        {
            var refrenceQuery = string.Empty;
            if (dateFormat.ToUpper() == "BS" || dateFormat.ToUpper() == "LOC")
            {
                if (frequency.ToLower() == "week")
                {
                    refrenceQuery = $@" SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' || NEPALI_WEEKS.WEEKS ||'_'||  TO_CHAR(SUBSTR(BS_DATE(SI.SALES_DATE),0,4) )  COLNAME, SI.ITEM_CODE,
                                         IMS.ITEM_EDESC,
                                         SI.CUSTOMER_CODE,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS,
                                         TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                        TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                    FROM {FiscalYear}.SA_SALES_INVOICE SI, {FiscalYear}.IP_ITEM_MASTER_SETUP IMS, (SELECT DAYS, ROWNUM  DAY_NO , TRUNC((ROWNUM+7)/7)  WEEKS
                                  FROM (SELECT FISCAL_YEAR.START_DATE + ROWNUM - 1 DAYS
                                          FROM ALL_OBJECTS,
                                               (SELECT START_DATE, END_DATE
                                                  FROM HR_FISCAL_YEAR_CODE
                                                 WHERE SYSDATE BETWEEN START_DATE AND END_DATE) FISCAL_YEAR
                                         WHERE ROWNUM <= FISCAL_YEAR.END_DATE - FISCAL_YEAR.START_DATE + 1) )NEPALI_WEEKS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                  and IMS.COMPANY_CODE=SI.COMPANY_CODE
                                    AND NEPALI_WEEKS.DAYS = SI.SALES_DATE
                                    AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                                GROUP BY SI.ITEM_CODE,
                                         IMS.ITEM_EDESC,
                                         SI.CUSTOMER_CODE,
                                         SI.DIVISION_CODE,
                                         SI.BRANCH_CODE,
                                        NEPALI_WEEKS.WEEKS
                                        ,SUBSTR(BS_DATE(SI.SALES_DATE),0,4)  
                                 ORDER BY   NEPALI_WEEKS.WEEKS";
                }
                else if (frequency.ToLower() == "month")
                {
                    refrenceQuery = $@" SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_' ||  UPPER(fn_bs_month(SUBSTR(BS_DATE(SI.ORDER_DATE),6,2))) ||'_'|| SUBSTR(BS_DATE(SI.ORDER_DATE),0,4) COLNAME ,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    -- SUBSTR(TO_BS(SI.SALES_DATE),7,7) NEPALI_MONTH,
                                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM SA_SALES_ORDER SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                 and IMS.COMPANY_CODE=SI.COMPANY_CODE
                                AND SI.ORDER_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     SUBSTR(BS_DATE(SI.ORDER_DATE),0,4),
                                     fn_bs_month(SUBSTR(BS_DATE(SI.ORDER_DATE),6,2))";
                    //var refrenceQuery = $@"SELECT SI.ITEM_CODE,
                    //                     IMS.ITEM_EDESC,
                    //                     SI.CUSTOMER_CODE,
                    //                     SI.DIVISION_CODE,
                    //                     SI.BRANCH_CODE,
                    //                     SUBSTR(TO_BS(SI.SALES_DATE),4,7) NEPALI_MONTH,
                    //                     SUM (SI.CALC_QUANTITY) QTY
                    //                FROM SA_SALES_INVOICE SI, IP_ITEM_MASTER_SETUP IMS
                    //                WHERE SI.CUSTOMER_CODE = {customerCode}
                    //                AND IMS.ITEM_CODE IN ({itemList})
                    //                AND SI.DIVISION_CODE = '{divisionCode}'
                    //                AND SI.BRANCH_CODE = '{branchCode}'
                    //                AND SI.ITEM_CODE = IMS.ITEM_CODE
                    //            GROUP BY SI.ITEM_CODE,
                    //                     IMS.ITEM_EDESC,
                    //                     SI.CUSTOMER_CODE,
                    //                     SI.DIVISION_CODE,
                    //                     SI.BRANCH_CODE,
                    //                     SUBSTR(TO_BS(SI.SALES_DATE),4,7)";
                }
            }
            else if (dateFormat.ToUpper() == "AD" || dateFormat.ToUpper() == "ENG")
            {
                if (frequency.ToLower() == "week")
                {
                    refrenceQuery = $@"SELECT 'freqItemNum_'||SI.ITEM_CODE ||'_WEEK_' ||  TO_CHAR(SI.SALES_DATE,'IW') ||'_'||  TO_CHAR(SI.SALES_DATE,'YYYY') COLNAME, SI.ITEM_CODE,
                                       IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.SALES_DATE,'YYYY') AD_YEAR,
                                     TO_CHAR(SI.SALES_DATE,'IW') SALES_WEEK,
                                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM {FiscalYear}.SA_SALES_INVOICE SI, {FiscalYear}.IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                and IMS.COMPANY_CODE=SI.COMPANY_CODE
                                AND SI.SALES_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                           GROUP BY SI.ITEM_CODE,
                             IMS.ITEM_EDESC,
                             SI.CUSTOMER_CODE,
                             SI.DIVISION_CODE,
                             SI.BRANCH_CODE,
                             TO_CHAR(SI.SALES_DATE,'YYYY'),
                             TO_CHAR(SI.SALES_DATE,'IW')
                     ORDER BY  TO_CHAR(SI.SALES_DATE,'YYYY'), TO_CHAR(SI.SALES_DATE,'IW')";
                }
                else if (frequency.ToLower() == "month")
                {
                    refrenceQuery = $@"SELECT  'freqItemNum_'||SI.ITEM_CODE ||'_' ||  TO_CHAR(SI.ORDER_DATE,'MON') ||'_'||  TO_CHAR(SI.ORDER_DATE,'YYYY') COLNAME ,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                    -- SUBSTR(TO_BS(SI.SALES_DATE),7,7) NEPALI_MONTH,
                                     TO_CHAR(SUM (SI.CALC_TOTAL_PRICE)) AMOUNT,
                                     TO_CHAR(SUM (SI.CALC_QUANTITY)) QTY
                                FROM SA_SALES_ORDER SI, IP_ITEM_MASTER_SETUP IMS
                                WHERE IMS.ITEM_CODE IN ({itemList}) {optionalCondition} 
                                AND SI.ITEM_CODE = IMS.ITEM_CODE
                                and IMS.COMPANY_CODE=SI.COMPANY_CODE
                                AND SI.ORDER_DATE BETWEEN to_date('{startDate}','YYYY-Mon-DD') AND  to_date('{endDate}','YYYY-Mon-DD')
                            GROUP BY SI.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     SI.CUSTOMER_CODE,
                                     SI.DIVISION_CODE,
                                     SI.BRANCH_CODE,
                                     TO_CHAR(SI.ORDER_DATE,'YYYY'),
                                     TO_CHAR(SI.ORDER_DATE,'MON')";
                }
            }
            return refrenceQuery;
        }

        public List<PlanSalesRefrenceModel> getSalesItemDataForRefrences(SalesItemForReferenceModel model)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var result = new List<PlanSalesRefrenceModel>();
            try
            {
                var refrenceQuery = string.Empty;
                string optionalCondition = string.Empty;
                if (!string.IsNullOrEmpty(model.customerCode))
                {
                    optionalCondition += $@"  AND SI.CUSTOMER_CODE = '{model.customerCode}'";
                }
                if (!string.IsNullOrEmpty(model.divisionCode))
                {
                    optionalCondition += $@"  AND SI.DIVISION_CODE = '{model.divisionCode}'";
                }
                if (!string.IsNullOrEmpty(model.branchCode))
                {
                    optionalCondition += $@"  AND SI.BRANCH_CODE in ({model.branchCode})";
                }

                if (model.salesFlag == "sales_invoice")
                {
                    refrenceQuery = getSalesInvoiceQueryForReference(model.dateFormat, model.frequency, model.FiscalYear, model.ItemList, optionalCondition, model.startDate, model.endDate);
                }
                else
                {
                    refrenceQuery = getSalesOrderQueryForReference(model.dateFormat, model.frequency, model.FiscalYear, model.ItemList, optionalCondition, model.startDate, model.endDate);
                }
                result = this._dbContext.SqlQuery<PlanSalesRefrenceModel>(refrenceQuery).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanSalesRefrenceModel> getSalesItemDataForRefrence(string itemList, string startDate, string endDate, string customerCode, string divisionCode, string branchCode, string dateFormat, string frequency, string FiscalYear, string salesFlag)
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var result = new List<PlanSalesRefrenceModel>();
            try
            {
                var refrenceQuery = string.Empty;
                string optionalCondition = string.Empty;
                if (!string.IsNullOrEmpty(customerCode))
                {
                    optionalCondition += $@"  AND SI.CUSTOMER_CODE = '{customerCode}'";
                }
                if (!string.IsNullOrEmpty(divisionCode))
                {
                    optionalCondition += $@"  AND SI.DIVISION_CODE = '{divisionCode}'";
                }
                if (!string.IsNullOrEmpty(branchCode))
                {
                    optionalCondition += $@"  AND SI.BRANCH_CODE in ({branchCode})";
                }

                if (salesFlag == "sales_invoice")
                {
                    refrenceQuery = getSalesInvoiceQueryForReference(dateFormat, frequency, FiscalYear, itemList, optionalCondition, startDate, endDate);
                }
                else
                {
                    refrenceQuery = getSalesOrderQueryForReference(dateFormat, frequency, FiscalYear, itemList, optionalCondition, startDate, endDate);
                }
                result = this._dbContext.SqlQuery<PlanSalesRefrenceModel>(refrenceQuery).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public LcPreferenceSetup GetItemGoupEntryPreferenceSetup()
        {
            var companyCode = this._workcontext.CurrentUserinformation.company_code;
            var branchCode = this._workcontext.CurrentUserinformation.branch_code;
            string query = $@"SELECT COMPANY_CODE,BRANCH_CODE,ITEM_GROUP_ENTRY FROM PL_PREFERENCE_SETUP WHERE COMPANY_CODE='{companyCode}' AND BRANCH_CODE='{branchCode}'";
            LcPreferenceSetup setup = new LcPreferenceSetup();
            setup = this._dbContext.SqlQuery<LcPreferenceSetup>(query).FirstOrDefault();
            return setup;
        }
        public List<PreferenceSetupModel> GetPreferenceSetups()
        {
            var companyCode = this._workcontext.CurrentUserinformation.company_code;
            var branchCode = this._workcontext.CurrentUserinformation.branch_code;
            string query = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE COMPANY_CODE='{companyCode}' AND BRANCH_CODE='{branchCode}'";
            List<PreferenceSetupModel> preSetup = new List<PreferenceSetupModel>();
            preSetup = this._dbContext.SqlQuery<PreferenceSetupModel>(query).ToList();
            preSetup.Add(new PreferenceSetupModel { PL_NAME = "TARGET_PERCENT_PLAN" });
            return preSetup;
        }
        public PreferenceSetupModel GetPreferenceSetup(string pl_name)
        {
            var companyCode = this._workcontext.CurrentUserinformation.company_code;
            var branchCode = this._workcontext.CurrentUserinformation.branch_code;
            string query = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE COMPANY_CODE='{companyCode}' AND BRANCH_CODE='{branchCode}' AND PL_NAME = '{pl_name}'";
            var preSetup = new PreferenceSetupModel();
            preSetup = this._dbContext.SqlQuery<PreferenceSetupModel>(query).FirstOrDefault();
            return preSetup;
        }

        public int SavePreferenceSetup(PreferenceSetupModel model)
        {
            var companyCode = this._workcontext.CurrentUserinformation.company_code;
            var branchCode = this._workcontext.CurrentUserinformation.branch_code;
            int result = 0;
            try
            {
                string query = $@"UPDATE PL_PREFERENCE_SETUP SET ITEM_GROUP_ENTRY='{model.ITEM_GROUP_ENTRY}' ,SHOW_CUSTOMER='{model.SHOW_CUSTOMER}', SHOW_BRANCH='{model.SHOW_BRANCH}', SHOW_EMPLOYEE='{model.SHOW_EMPLOYEE}', SHOW_ITEM='{model.SHOW_ITEM}', PL_NAME='{model.PL_NAME}', SHOW_DIVISION='{model.SHOW_DIVISION}', SHOW_PARTY_TYPE='{model.SHOW_PARTY_TYPE}', SHOW_AGENT='{model.SHOW_AGENT}',PARTIAL_EDIT='{model.PARTIAL_EDIT}',EDIT_VALUE='{model.EDIT_VALUE}' WHERE PL_NAME='{model.PL_NAME}' AND COMPANY_CODE='{companyCode}' AND BRANCH_CODE='{branchCode}'";
                result = this._dbContext.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                if(ex.Message.ToLower().Contains("edit_value"))
                {
                    string query = $@"UPDATE PL_PREFERENCE_SETUP SET ITEM_GROUP_ENTRY='{model.ITEM_GROUP_ENTRY}' ,SHOW_CUSTOMER='{model.SHOW_CUSTOMER}', SHOW_BRANCH='{model.SHOW_BRANCH}', SHOW_EMPLOYEE='{model.SHOW_EMPLOYEE}', SHOW_ITEM='{model.SHOW_ITEM}', PL_NAME='{model.PL_NAME}', SHOW_DIVISION='{model.SHOW_DIVISION}', SHOW_PARTY_TYPE='{model.SHOW_PARTY_TYPE}', SHOW_AGENT='{model.SHOW_AGENT}' WHERE PL_NAME='{model.PL_NAME}' AND COMPANY_CODE='{companyCode}' AND BRANCH_CODE='{branchCode}'";
                    result = this._dbContext.ExecuteSqlCommand(query);
                    return result;
                }
                throw ex;
            }

            return result;
        }

        public int SaveAllPreferenceSetup()
        {
            var companyCode = this._workcontext.CurrentUserinformation.company_code;
            var branchCode = this._workcontext.CurrentUserinformation.branch_code;
            int result = 0;
            try
            {
                string dq = $@"DELETE FROM PL_PREFERENCE_SETUP WHERE COMPANY_CODE = '{companyCode}' AND BRANCH_CODE = '{branchCode}'";
                this._dbContext.ExecuteSqlCommand(dq);

                string query = $@"INSERT ALL INTO PL_PREFERENCE_SETUP (COMPANY_CODE, BRANCH_CODE, ITEM_GROUP_ENTRY,SHOW_EMPLOYEE,SHOW_BRANCH,SHOW_CUSTOMER, SHOW_ITEM,PL_NAME,SHOW_DIVISION,SHOW_PARTY_TYPE,SHOW_AGENT) VALUES ('{companyCode}', '{branchCode}', 'N','N','N','N','N','SALES_PLAN','N','N','N')
                                INTO PL_PREFERENCE_SETUP (COMPANY_CODE, BRANCH_CODE, ITEM_GROUP_ENTRY,SHOW_EMPLOYEE,SHOW_BRANCH,SHOW_CUSTOMER, SHOW_ITEM,PL_NAME,SHOW_DIVISION) VALUES ('{companyCode}', '{branchCode}', 'N','N','N','N','N','LEDGER_PLAN','N')
                                INTO PL_PREFERENCE_SETUP (COMPANY_CODE, BRANCH_CODE, ITEM_GROUP_ENTRY,SHOW_EMPLOYEE,SHOW_BRANCH,SHOW_CUSTOMER, SHOW_ITEM,PL_NAME,SHOW_DIVISION) VALUES ('{companyCode}', '{branchCode}', 'N','N','N','N','N','PROCUREMENT_PLAN','N')
                                INTO PL_PREFERENCE_SETUP (COMPANY_CODE, BRANCH_CODE, ITEM_GROUP_ENTRY,SHOW_EMPLOYEE,SHOW_BRANCH,SHOW_CUSTOMER, SHOW_ITEM,PL_NAME,SHOW_DIVISION) VALUES ('{companyCode}', '{branchCode}', 'N','N','N','N','N','PRODUCTION_PLAN','N')
                                INTO PL_PREFERENCE_SETUP (COMPANY_CODE, BRANCH_CODE, ITEM_GROUP_ENTRY,SHOW_EMPLOYEE,SHOW_BRANCH,SHOW_CUSTOMER, SHOW_ITEM,PL_NAME,SHOW_DIVISION) VALUES ('{companyCode}', '{branchCode}', 'N','N','N','N','N','BUDGET_PLAN','N')
                                INTO PL_PREFERENCE_SETUP (COMPANY_CODE, BRANCH_CODE, ITEM_GROUP_ENTRY,SHOW_EMPLOYEE,SHOW_BRANCH,SHOW_CUSTOMER, SHOW_ITEM,PL_NAME,SHOW_DIVISION) VALUES ('{companyCode}', '{branchCode}', 'N','N','N','N','N','BRANDING_PLAN','N')
                                INTO PL_PREFERENCE_SETUP (COMPANY_CODE, BRANCH_CODE, ITEM_GROUP_ENTRY,SHOW_EMPLOYEE,SHOW_BRANCH,SHOW_CUSTOMER, SHOW_ITEM,PL_NAME,SHOW_DIVISION) VALUES ('{companyCode}', '{branchCode}', 'N','N','N','N','N','COLLECTION_PLAN','N')
                                SELECT 1 FROM DUAL";
                result = this._dbContext.ExecuteSqlCommand(query);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public string SavePlanFromExcellAll(List<ImportItems> listItems, List<ImportItems> distinctItems)
        {
            var result = string.Empty;
            var trans = _objContext.Database.BeginTransaction();
           
            List<string> existPlanName = new List<string>();
            try
            {
               
                foreach (var a in distinctItems)
                {
                    var getStartEndDateByFiscalYear = $@"SELECT START_DATE,END_DATE FROM PL_FISCAL_YEAR_CODE WHERE ROWNUM=1 AND ACTIVE ='Y' AND FISCAL_YEAR_CODE='{ a.FISCAL_YEAR}' ORDER BY FISCAL_YEAR_CODE DESC";
                    var fiscal_year_date = this._objContext.SqlQuery<fiscalYearModel>(getStartEndDateByFiscalYear).FirstOrDefault();
                    if (fiscal_year_date == null)
                        throw new Exception($@"Fiscal year is not valid");
                    var getItemList = listItems.Where(x => x.PLAN_NAME == a.PLAN_NAME).ToList();
                    int count = _objContext.SqlQuery<int>($"select count(*) from PL_SALES_PLAN where PLAN_EDESC='{getItemList[0].PLAN_NAME}' and start_date<= TO_DATE('{fiscal_year_date.START_DATE}','MM/DD/YYYY HH:MI:SS PM') and end_date>=TO_DATE('{fiscal_year_date.END_DATE}','MM/DD/YYYY HH:MI:SS PM') and Deleted_Flag='N' ").FirstOrDefault();
                    if (count > 0)
                    {
                        existPlanName.Add(getItemList[0].PLAN_NAME);
                        continue;
                    }
                    result = SavePlanFromExcell(getItemList);
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            if(existPlanName.Count>0)
            {
                throw new Exception(string.Join(",",existPlanName)+ " Plan is/are Already Exists. Unique Plan Name are added successfully");
            }

            return result;
        }
        public string SavePlanFromExcell(List<ImportItems> sv)
        {
            var userID = _workcontext.CurrentUserinformation.User_id;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            var spbranchCode = _workcontext.CurrentUserinformation.branch_code;
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var result = "";
            int planCode = 0;
            var company_code = _workcontext.CurrentUserinformation.company_code;

            var getStartEndDateByFiscalYear = $@"SELECT START_DATE,END_DATE FROM PL_FISCAL_YEAR_CODE WHERE ROWNUM=1 AND ACTIVE ='Y' AND FISCAL_YEAR_CODE='{sv.Select(a => a.FISCAL_YEAR).FirstOrDefault()}' ORDER BY FISCAL_YEAR_CODE DESC";
            var fiscal_year_date = this._objContext.SqlQuery<fiscalYearModel>(getStartEndDateByFiscalYear).FirstOrDefault();

            if (fiscal_year_date == null)
                throw new Exception($@"Fiscal year is not valid");

            string sbInsertQuery = string.Empty;

            List<sa_sales_invoice_viewmodel> salesInvoice = new List<sa_sales_invoice_viewmodel>();
            string averageQuery = $@"SELECT CALC_TOTAL_PRICE,CALC_QUANTITY, ITEM_CODE FROM SA_SALES_INVOICE WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' --AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.ITEM_CODE))})";
            salesInvoice = this._objContext.SqlQuery<sa_sales_invoice_viewmodel>(averageQuery).ToList();

            string itemQuery = $@"SELECT PURCHASE_PRICE AS CALC_TOTAL_PRICE,ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}'  --AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.ITEM_CODE))})";

            var itemsFromItemMaster = this._objContext.SqlQuery<sa_sales_invoice_viewmodel>(itemQuery).ToList();

            string[] insert_query_array = new string[sv.Count()];
            int itemCount = 0,
                itemRange = 5,
                array_count = 0,
                itteration = 0,
                remainderCount = 0;
            remainderCount = (sv.Count() % itemRange);
            //int count = _objContext.SqlQuery<int>($"select count(*) from PL_SALES_PLAN where PLAN_EDESC='{sv[0].PLAN_NAME}'").FirstOrDefault();
            //if(count>0)
            //{
            //    throw new Exception("Plan Name Already Exists!!!");
            //}

            string sales_plan_query = $@"INSERT INTO PL_SALES_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    LAST_MODIFIED_BY,LAST_MODIFIED_DATE,
                    DELETED_FLAG,SALES_PRICE_TYPE)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_SALES_PLAN),'{sv[0].PLAN_NAME}','','{sv[0].SALES_QUANTITY}','{sv[0].SALES_AMOUNT}','{sv[0].TIME_FRAME}','{sv[0].CALENDAR_TYPE}',
                    TO_DATE('{fiscal_year_date.START_DATE}','MM/DD/YYYY HH:MI:SS PM'),TO_DATE('{fiscal_year_date.END_DATE}','MM/DD/YYYY HH:MI:SS PM'),'{sv[0].REMARKS}','{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',SYSDATE,'N','{sv[0].SALES_TYPE}')";
            try
            {
                var insertResult = this._objContext.ExecuteSqlCommand(sales_plan_query);
            }
            catch (Exception ex)
            {
                throw new Exception($@"{ex.Message}, Plan Name = {sv[0].PLAN_NAME}");
            }
            

            string fetchSalesPlan = $@"SELECT PLAN_CODE FROM PL_SALES_PLAN WHERE PLAN_EDESC='{sv[0].PLAN_NAME}' 
                AND TIME_FRAME_CODE='{sv[0].TIME_FRAME}' AND START_DATE=TO_DATE('{fiscal_year_date.START_DATE}','MM/DD/YYYY HH:MI:SS PM') AND END_DATE=TO_DATE('{fiscal_year_date.END_DATE}','MM/DD/YYYY HH:MI:SS PM')";
            planCode = this._objContext.SqlQuery<int>(fetchSalesPlan).First();

            foreach (var item in sv)
            {
                itemCount++;
                itteration++;

                item.START_DATE = fiscal_year_date.START_DATE;
                item.END_DATE = fiscal_year_date.END_DATE;

                var ITEM_CODE = string.Empty;
                var CUSTOMER_CODE = string.Empty;
                var DIVISION_CODE = string.Empty;
                var EMPLOYEE_CODE = string.Empty;

                var startDate = Convert.ToDateTime(item.START_DATE).ToString("yyyy-MMM-dd").ToString();
                var endDate = Convert.ToDateTime(item.END_DATE).ToString("yyyy-MMM-dd").ToString();
                List<FrequencyColumnModel> allDaysObj = new List<FrequencyColumnModel>();
                allDaysObj = getAllDaysBetweenDateRange("LOC", "month", startDate, endDate);

                if (item.ITEM_CODE != null)
                {
                    var getItemQry = $@"SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{companyCode}' AND (UPPER(ITEM_CODE) = UPPER('{item.ITEM_CODE.Trim()}') OR UPPER(ITEM_EDESC) LIKE UPPER('{item.ITEM_CODE.Trim()}'))";
                    ITEM_CODE = _objContext.SqlQuery<string>(getItemQry).FirstOrDefault();
                    if (ITEM_CODE == null)
                        throw new Exception($@"Item Not Found {item.ITEM_CODE} plan name={item.PLAN_NAME}");
                    item.ITEM_CODE = ITEM_CODE;
                }
                if (item.DIVISION_CODE != null)
                {
                    var getDivisionQry = $@"SELECT DIVISION_CODE FROM FA_DIVISION_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{companyCode}' AND (UPPER(DIVISION_CODE) = UPPER('{item.DIVISION_CODE.Trim()}') OR UPPER(DIVISION_EDESC) LIKE UPPER('{item.DIVISION_CODE.Trim()}'))";
                    DIVISION_CODE = _objContext.SqlQuery<string>(getDivisionQry).FirstOrDefault();
                    if (DIVISION_CODE == null)
                        throw new Exception($@"Division Not Found {item.DIVISION_CODE} plan name={item.PLAN_NAME}");
                    item.DIVISION_CODE = DIVISION_CODE;
                }
                if (item.CUSTOMER_CODE != null)
                {
                    var getCustomerQry = $@"SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{companyCode}' AND (UPPER(CUSTOMER_CODE) = UPPER('{item.CUSTOMER_CODE.Trim()}') OR UPPER(CUSTOMER_EDESC) LIKE UPPER('{item.CUSTOMER_CODE.Trim()}'))";
                    CUSTOMER_CODE = _objContext.SqlQuery<string>(getCustomerQry).FirstOrDefault();
                    if (CUSTOMER_CODE == null)
                        throw new Exception($@"Customer Not Found {item.CUSTOMER_CODE} plan name={item.PLAN_NAME}");
                    item.CUSTOMER_CODE = CUSTOMER_CODE;
                }
                if (item.EMPLOYEE_CODE != null)
                {
                    var getEmployeeQry = $@"SELECT EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{companyCode}' AND (UPPER(EMPLOYEE_CODE) = UPPER('{item.EMPLOYEE_CODE.Trim()}') OR UPPER(EMPLOYEE_EDESC) LIKE UPPER('{item.EMPLOYEE_CODE.Trim()}'))";
                    EMPLOYEE_CODE = _objContext.SqlQuery<string>(getEmployeeQry).FirstOrDefault();
                    if (EMPLOYEE_CODE == null)
                        throw new Exception($@"Employee Not Found {item.EMPLOYEE_CODE} plan name={item.PLAN_NAME}");
                    item.EMPLOYEE_CODE = EMPLOYEE_CODE;
                }

                salesInvoice = salesInvoice.Where(x => x.ITEM_CODE == item.ITEM_CODE).ToList();
                itemsFromItemMaster = itemsFromItemMaster.Where(x => x.ITEM_CODE == item.ITEM_CODE).ToList();

                string averagePriceQuery = string.Empty;
                var salesPriceResult = string.Empty;
                if (item.RATE != null)
                {
                    salesPriceResult = item.RATE.ToString();
                }
                else
                {
                    if (item.SALES_TYPE == "AVERAGE_SALES_PRICE")
                    {

                        salesPriceResult = salesInvoice.Where(a => a.ITEM_CODE == item.ITEM_CODE).FirstOrDefault() != null ? (salesInvoice.Where(a => a.ITEM_CODE == item.ITEM_CODE).Sum(a => a.CALC_TOTAL_PRICE) / salesInvoice.Where(a => a.ITEM_CODE == item.ITEM_CODE).Sum(a => a.CALC_QUANTITY)).ToString() : null;
                        if (string.IsNullOrEmpty(salesPriceResult))
                        {
                            averagePriceQuery = $@"SELECT TO_CHAR(SALES_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}' AND ROWNUM <=1 ORDER BY APP_DATE DESC ";
                            salesPriceResult = this._objContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                            if (salesPriceResult == null)
                            {
                                averagePriceQuery = $@"SELECT TO_CHAR(STANDARD_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_SCHEDULE_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}' AND ROWNUM <= 1 ORDER BY EFFECTIVE_DATE DESC";
                                salesPriceResult = this._objContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                                if (salesPriceResult == null)
                                {
                                    salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).First().CALC_TOTAL_PRICE.ToString();
                                }
                            }
                        }
                    }
                    else if (item.SALES_TYPE == "STANDARD_SALES_PRICE")
                    {
                        averagePriceQuery = $@"SELECT TO_CHAR(SALES_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}' AND ROWNUM <=1 ORDER BY APP_DATE DESC ";
                        salesPriceResult = this._objContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                        if (salesPriceResult == null)
                        {
                            averagePriceQuery = $@"SELECT TO_CHAR(STANDARD_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_SCHEDULE_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}' AND ROWNUM <= 1 ORDER BY EFFECTIVE_DATE DESC";
                            salesPriceResult = this._objContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();

                            if (salesPriceResult == null)
                            {
                                salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).First().CALC_TOTAL_PRICE.ToString();
                            }
                        }
                    }
                    else if (item.SALES_TYPE == "LANDED_COST")
                    {
                        salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).First().CALC_TOTAL_PRICE.ToString();
                    }
                }
                if (string.IsNullOrEmpty(salesPriceResult) || salesPriceResult == "0")
                {
                    salesPriceResult = "1";
                }
                try
                {
                    int interval = 0;

                    interval = allDaysObj.Count();
                    foreach (var date in allDaysObj)
                    {
                        var yearMonth = date.YEARWEEK.Split('-');
                        var month_year = yearMonth[1] + "_" + yearMonth[0];

                        string freValue = yearMonth[1];
                        string freYear = yearMonth[0];

                        string freYearVal = freYear + '-' + freValue;

                        var monthinterval = allDaysObj.Where(a => a.YEARWEEK == freYearVal).Select(a => a.DAYS).ToArray();

                        var amtQty = 0M;
                        var invalid = false;
                        var switch_on = (freValue != null && freValue != "") ? freValue.Trim() : freValue;
                        switch (switch_on)
                        {
                            case "01":
                                amtQty = Convert.ToDecimal(item.BAISAKH);
                                break;
                            case "02":
                                amtQty = Convert.ToDecimal(item.JESTHA);
                                break;
                            case "03":
                                amtQty = Convert.ToDecimal(item.ASHAD);
                                break;
                            case "04":
                                amtQty = Convert.ToDecimal(item.SHRAWAN);
                                break;
                            case "05":
                                amtQty = Convert.ToDecimal(item.BHADRA);
                                break;
                            case "06":
                                amtQty = Convert.ToDecimal(item.ASHOJ);
                                break;
                            case "07":
                                amtQty = Convert.ToDecimal(item.KARTIK);
                                break;
                            case "08":
                                amtQty = Convert.ToDecimal(item.MANGSIR);
                                break;
                            case "09":
                                amtQty = Convert.ToDecimal(item.POUSH);
                                break;
                            case "10":
                                amtQty = Convert.ToDecimal(item.MAGH);
                                break;
                            case "11":
                                amtQty = Convert.ToDecimal(item.FALGUN);
                                break;
                            case "12":
                                amtQty = Convert.ToDecimal(item.CHAITRA);
                                break;
                            default:
                                invalid = true;
                                break;
                        }
                        if (invalid)
                            return "Excelsheet Month name not valid";
                        //if (amtQty == 0)
                        //    continue;

                        var eachday_value = Math.Round((Convert.ToDecimal(amtQty) / monthinterval.Length), 5);//Math.Round((Convert.ToDecimal(freq.fvalue) / interval), 3);

                        var eachmonth_value = Math.Round((eachday_value * monthinterval.Length), 5);

                        var eachday_quantity_value = 0M;
                        var eachday_amount_value = 0M;

                        if (item.AMOUNT_QUANTITY.Trim().ToLower() == "qty")
                        {
                            eachday_quantity_value = eachday_value;
                            eachday_amount_value = Math.Round(eachday_value * Convert.ToDecimal(salesPriceResult), 5);
                        }
                        else if (item.AMOUNT_QUANTITY.Trim().ToLower() == "amt")
                        {
                            eachday_quantity_value = Math.Round(eachday_value / Convert.ToDecimal(salesPriceResult), 5);
                            eachday_amount_value = eachday_value;
                        }

                        string frequency_json = @"fname__" + month_year + "__fvalue__" + amtQty;

                        string insertinto_plandtl = $@"SELECT '{planCode}',TO_DATE('{date.DAYS}','DD-MON-YYYY'),'{eachday_quantity_value}','{eachday_amount_value}','{item.ITEM_CODE}',
                                '{item.CUSTOMER_CODE}','{item.EMPLOYEE_CODE}','{item.DIVISION_CODE}','{item.PARTY_TYPE}','',
                                '{companyCode}','{spbranchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N','{frequency_json}','{salesPriceResult}' FROM DUAL UNION ALL ";

                        sbInsertQuery += insertinto_plandtl;
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;

                }
                if (itteration <= (sv.Count() - remainderCount))
                {
                    while (itemCount == itemRange)
                    {
                        string query = @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) "
+ sbInsertQuery;
                        query = query.Substring(0, query.Length - 11);
                        insert_query_array[array_count] = query;
                        array_count++;
                        itemCount = 0;
                        sbInsertQuery = string.Empty;
                    }
                }
                else
                {
                    while (itemCount == remainderCount)
                    {
                        string query = @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) "
+ sbInsertQuery;
                        query = query.Substring(0, query.Length - 11);
                        insert_query_array[array_count] = query;
                        array_count++;
                        itemCount = 0;
                        sbInsertQuery = string.Empty;
                    }
                }
            }
            try
            {
                var insertItems = insert_query_array.Where(a => a != null);
                foreach (var item in insertItems)
                {
                    result = _objContext.ExecuteSqlCommand(item.ToString()).ToString();
                }

            }
            catch (Exception ex)
            {
                var commonItemCode = sv.GroupBy(x=>x.ITEM_CODE).Where(z => z.Count() > 1).SelectMany(z => z).FirstOrDefault();
                throw new Exception(ex.Message+$@" Plan Name="+sv[0].PLAN_NAME+" Item code="+ commonItemCode.ITEM_CODE);
            }

            string update_amout_quantity = string.Empty;
            if (sv[0].AMOUNT_QUANTITY.Trim().ToLower() == "qty")
            {
                update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_QUANTITY = (
                        SELECT ROUND(SUM(SPD.PER_DAY_QUANTITY)) FROM PL_SALES_PLAN_DTL SPD, IP_ITEM_MASTER_SETUP IMS
                        WHERE 
                        IMS.GROUP_SKU_FLAG ='I'
                        AND SPD.ITEM_CODE = IMS.ITEM_CODE
                           AND SPD.COMPANY_CODE=IMS.COMPANY_CODE
                        AND SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_AMOUNT=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
            }
            else if (sv[0].AMOUNT_QUANTITY.Trim().ToLower() == "amt")
            {
                update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_AMOUNT = (
                        SELECT ROUND(SUM(SPD.PER_DAY_AMOUNT)) FROM PL_SALES_PLAN_DTL SPD, IP_ITEM_MASTER_SETUP IMS
                        WHERE 
                        IMS.GROUP_SKU_FLAG ='I'
                        AND SPD.ITEM_CODE = IMS.ITEM_CODE
                          AND SPD.COMPANY_CODE=IMS.COMPANY_CODE
                        AND SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_QUANTITY=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
            }
            this._objContext.ExecuteSqlCommand(update_amout_quantity);
            //subin changes
            var preference = GetPreferenceSetups().ToList().Where(x => x.PL_NAME == "SALES_PLAN").FirstOrDefault();
            if(preference!=null)
            {
                if (preference.SHOW_ITEM == "N")
                {
                    if (sv[0].AMOUNT_QUANTITY.Trim().ToLower() == "qty")
                    {
                        update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_QUANTITY = (
                        SELECT ROUND(SUM(SPD.PER_DAY_QUANTITY)) FROM PL_SALES_PLAN_DTL SPD
                        WHERE SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_AMOUNT=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
                    }
                    else if (sv[0].AMOUNT_QUANTITY.Trim().ToLower() == "amt")
                    {
                        update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_AMOUNT = (
                        SELECT ROUND(SUM(SPD.PER_DAY_AMOUNT)) FROM PL_SALES_PLAN_DTL SPD
                        WHERE SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_QUANTITY=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
                    }
                    this._objContext.ExecuteSqlCommand(update_amout_quantity);
                }
            }
            return result.ToString();
        }
        public string SavePlanFromExcell_old(List<ImportItems> sv)
        {
            var userID = _workcontext.CurrentUserinformation.User_id;
            var branchCode = _workcontext.CurrentUserinformation.branch_code;
            var spbranchCode = _workcontext.CurrentUserinformation.branch_code;
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var result = "";
            int planCode = 0;
            var company_code = _workcontext.CurrentUserinformation.company_code;

            var getStartEndDateByFiscalYear = $@"SELECT START_DATE,END_DATE FROM HR_FISCAL_YEAR_CODE WHERE ROWNUM=1 ORDER BY FISCAL_YEAR_CODE DESC";
            var fiscal_year_date = this._dbContext.SqlQuery<fiscalYearModel>(getStartEndDateByFiscalYear).FirstOrDefault();


            string sbInsertQuery = string.Empty;

            List<sa_sales_invoice_viewmodel> salesInvoice = new List<sa_sales_invoice_viewmodel>();
            string averageQuery = $@"SELECT CALC_TOTAL_PRICE,CALC_QUANTITY, ITEM_CODE FROM SA_SALES_INVOICE WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' --AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.ITEM_CODE))})";
            salesInvoice = this._dbContext.SqlQuery<sa_sales_invoice_viewmodel>(averageQuery).ToList();

            string itemQuery = $@"SELECT PURCHASE_PRICE AS CALC_TOTAL_PRICE,ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}'  --AND ITEM_CODE IN ({string.Join(",", sv.Select(a => a.ITEM_CODE))})";

            var itemsFromItemMaster = this._dbContext.SqlQuery<sa_sales_invoice_viewmodel>(itemQuery).ToList();

            string[] insert_query_array = new string[sv.Count()];
            int itemCount = 0,
                itemRange = 5,
                array_count = 0,
                itteration = 0,
                remainderCount = 0;
            remainderCount = (sv.Count() % itemRange);

            string sales_plan_query = $@"INSERT INTO PL_SALES_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    LAST_MODIFIED_BY,LAST_MODIFIED_DATE,
                    DELETED_FLAG,SALES_PRICE_TYPE)
                    VALUES((SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_SALES_PLAN),'{sv[0].PLAN_NAME}','','{sv[0].SALES_QUANTITY}','{sv[0].SALES_AMOUNT}','{sv[0].TIME_FRAME}','{sv[0].CALENDAR_TYPE}',
                    TO_DATE('{fiscal_year_date.START_DATE}','MM/DD/YYYY HH:MI:SS PM'),TO_DATE('{fiscal_year_date.END_DATE}','MM/DD/YYYY HH:MI:SS PM'),'{sv[0].REMARKS}','{companyCode}','{branchCode}','{userID}',
                    TO_DATE('{DateTime.Today.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'{userID}',SYSDATE,'N','{sv[0].SALES_TYPE}')";

            var insertResult = this._dbContext.ExecuteSqlCommand(sales_plan_query);

            string fetchSalesPlan = $@"SELECT PLAN_CODE FROM PL_SALES_PLAN WHERE PLAN_EDESC='{sv[0].PLAN_NAME}' 
                AND TIME_FRAME_CODE='{sv[0].TIME_FRAME}' AND START_DATE=TO_DATE('{fiscal_year_date.START_DATE}','MM/DD/YYYY HH:MI:SS PM') AND END_DATE=TO_DATE('{fiscal_year_date.END_DATE}','MM/DD/YYYY HH:MI:SS PM')";
            planCode = this._dbContext.SqlQuery<int>(fetchSalesPlan).First();

            foreach (var item in sv)
            {
                itemCount++;
                itteration++;

                item.START_DATE = fiscal_year_date.START_DATE;
                item.END_DATE = fiscal_year_date.END_DATE;

                var ITEM_CODE = string.Empty;
                var CUSTOMER_CODE = string.Empty;
                var DIVISION_CODE = string.Empty;
                var EMPLOYEE_CODE = string.Empty;

                var startDate = Convert.ToDateTime(item.START_DATE).ToString("yyyy-MMM-dd").ToString();
                var endDate = Convert.ToDateTime(item.END_DATE).ToString("yyyy-MMM-dd").ToString();
                List<FrequencyColumnModel> allDaysObj = new List<FrequencyColumnModel>();
                allDaysObj = getAllDaysBetweenDateRange("LOC", "month", startDate, endDate);

                if (item.ITEM_CODE != null)
                {
                    var getItemQry = $@"SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{companyCode}' AND (UPPER(ITEM_CODE) = UPPER('{item.ITEM_CODE.Trim()}') OR UPPER(ITEM_EDESC) LIKE UPPER('{item.ITEM_CODE.Trim()}'))";
                    ITEM_CODE = _dbContext.SqlQuery<string>(getItemQry).FirstOrDefault();
                    if (ITEM_CODE == null)
                        throw new Exception($@"Item Not Found {item.ITEM_CODE}");
                    item.ITEM_CODE = ITEM_CODE;
                }
                if (item.DIVISION_CODE != null)
                {
                    var getDivisionQry = $@"SELECT DIVISION_CODE FROM FA_DIVISION_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{companyCode}' AND (UPPER(DIVISION_CODE) = UPPER('{item.DIVISION_CODE.Trim()}') OR UPPER(DIVISION_EDESC) LIKE UPPER('{item.DIVISION_CODE.Trim()}'))";
                    DIVISION_CODE = _dbContext.SqlQuery<string>(getDivisionQry).FirstOrDefault();
                    if (DIVISION_CODE == null)
                        throw new Exception($@"Division Not Found {item.DIVISION_CODE}");
                    item.DIVISION_CODE = DIVISION_CODE;
                }
                if (item.CUSTOMER_CODE != null)
                {
                    var getCustomerQry = $@"SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{companyCode}' AND (UPPER(CUSTOMER_CODE) = UPPER('{item.CUSTOMER_CODE.Trim()}') OR UPPER(CUSTOMER_EDESC) LIKE UPPER('{item.CUSTOMER_CODE.Trim()}'))";
                    CUSTOMER_CODE = _dbContext.SqlQuery<string>(getCustomerQry).FirstOrDefault();
                    if (CUSTOMER_CODE == null)
                        throw new Exception($@"Customer Not Found {item.CUSTOMER_CODE}");
                    item.CUSTOMER_CODE = CUSTOMER_CODE;
                }
                if (item.EMPLOYEE_CODE != null)
                {
                    var getEmployeeQry = $@"SELECT EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE='{companyCode}' AND (UPPER(EMPLOYEE_CODE) = UPPER('{item.EMPLOYEE_CODE.Trim()}') OR UPPER(EMPLOYEE_EDESC) LIKE UPPER('{item.EMPLOYEE_CODE.Trim()}'))";
                    EMPLOYEE_CODE = _dbContext.SqlQuery<string>(getEmployeeQry).FirstOrDefault();
                    if (EMPLOYEE_CODE == null)
                        throw new Exception($@"Employee Not Found {item.EMPLOYEE_CODE}");
                    item.EMPLOYEE_CODE = EMPLOYEE_CODE;
                }

                salesInvoice = salesInvoice.Where(x => x.ITEM_CODE == item.ITEM_CODE).ToList();
                itemsFromItemMaster = itemsFromItemMaster.Where(x => x.ITEM_CODE == item.ITEM_CODE).ToList();

                string averagePriceQuery = string.Empty;
                var salesPriceResult = string.Empty;
                if (item.SALES_TYPE == "AVERAGE_SALES_PRICE")
                {

                    salesPriceResult = salesInvoice.Where(a => a.ITEM_CODE == item.ITEM_CODE).FirstOrDefault() != null ? (salesInvoice.Where(a => a.ITEM_CODE == item.ITEM_CODE).Sum(a => a.CALC_TOTAL_PRICE) / salesInvoice.Where(a => a.ITEM_CODE == item.ITEM_CODE).Sum(a => a.CALC_QUANTITY)).ToString() : null;
                    if (string.IsNullOrEmpty(salesPriceResult))
                    {
                        averagePriceQuery = $@"SELECT TO_CHAR(SALES_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}' AND ROWNUM <=1 ORDER BY APP_DATE DESC ";
                        salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                        if (salesPriceResult == null)
                        {
                            averagePriceQuery = $@"SELECT TO_CHAR(STANDARD_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_SCHEDULE_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}' AND ROWNUM <= 1 ORDER BY EFFECTIVE_DATE DESC";
                            salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                            if (salesPriceResult == null)
                            {
                                salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).First().CALC_TOTAL_PRICE.ToString();
                            }
                        }
                    }
                }
                else if (item.SALES_TYPE == "STANDARD_SALES_PRICE")
                {
                    averagePriceQuery = $@"SELECT TO_CHAR(SALES_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}' AND ROWNUM <=1 ORDER BY APP_DATE DESC ";
                    salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();
                    if (salesPriceResult == null)
                    {
                        averagePriceQuery = $@"SELECT TO_CHAR(STANDARD_RATE) SALES_PRICE_RATE FROM IP_ITEM_RATE_SCHEDULE_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}' AND ITEM_CODE = '{item.ITEM_CODE}' AND ROWNUM <= 1 ORDER BY EFFECTIVE_DATE DESC";
                        salesPriceResult = this._dbContext.SqlQuery<string>(averagePriceQuery).FirstOrDefault();

                        if (salesPriceResult == null)
                        {
                            salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).First().CALC_TOTAL_PRICE.ToString();
                        }
                    }
                }
                else if (item.SALES_TYPE == "LANDED_COST")
                {
                    salesPriceResult = itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).FirstOrDefault() == null ? "1" : itemsFromItemMaster.Where(a => a.ITEM_CODE == item.ITEM_CODE).First().CALC_TOTAL_PRICE.ToString();
                }
                if (string.IsNullOrEmpty(salesPriceResult) || salesPriceResult == "0")
                {
                    salesPriceResult = "1";
                }
                try
                {
                    int interval = 0;

                    interval = allDaysObj.Count();
                    foreach (var date in allDaysObj)
                    {
                        var yearMonth = date.YEARWEEK.Split('-');
                        var month_year = yearMonth[1] + "_" + yearMonth[0];

                        string freValue = yearMonth[1];
                        string freYear = yearMonth[0];

                        string freYearVal = freYear + '-' + freValue;

                        var monthinterval = allDaysObj.Where(a => a.YEARWEEK == freYearVal).Select(a => a.DAYS).ToArray();

                        var amtQty = 0M;
                        var invalid = false;
                        var switch_on = (freValue != null && freValue != "") ? freValue.Trim() : freValue;
                        switch (switch_on)
                        {
                            case "01":
                                amtQty = Convert.ToDecimal(item.BAISAKH);
                                break;
                            case "02":
                                amtQty = Convert.ToDecimal(item.JESTHA);
                                break;
                            case "03":
                                amtQty = Convert.ToDecimal(item.ASHAD);
                                break;
                            case "04":
                                amtQty = Convert.ToDecimal(item.SHRAWAN);
                                break;
                            case "05":
                                amtQty = Convert.ToDecimal(item.BHADRA);
                                break;
                            case "06":
                                amtQty = Convert.ToDecimal(item.ASHOJ);
                                break;
                            case "07":
                                amtQty = Convert.ToDecimal(item.KARTIK);
                                break;
                            case "08":
                                amtQty = Convert.ToDecimal(item.MANGSIR);
                                break;
                            case "09":
                                amtQty = Convert.ToDecimal(item.POUSH);
                                break;
                            case "10":
                                amtQty = Convert.ToDecimal(item.MAGH);
                                break;
                            case "11":
                                amtQty = Convert.ToDecimal(item.FALGUN);
                                break;
                            case "12":
                                amtQty = Convert.ToDecimal(item.CHAITRA);
                                break;
                            default:
                                invalid = true;
                                break;
                        }
                        if (invalid)
                            return "Excelsheet Month name not valid";
                        //if (amtQty == 0)
                        //    continue;

                        var eachday_value = Math.Round((Convert.ToDecimal(amtQty) / monthinterval.Length), 5);//Math.Round((Convert.ToDecimal(freq.fvalue) / interval), 3);

                        var eachmonth_value = Math.Round((eachday_value * monthinterval.Length), 5);

                        var eachday_quantity_value = 0M;
                        var eachday_amount_value = 0M;

                        if (item.AMOUNT_QUANTITY.Trim().ToLower() == "qty")
                        {
                            eachday_quantity_value = eachday_value;
                            eachday_amount_value = Math.Round(eachday_value * Convert.ToDecimal(salesPriceResult), 5);
                        }
                        else if (item.AMOUNT_QUANTITY.Trim().ToLower() == "amt")
                        {
                            eachday_quantity_value = Math.Round(eachday_value / Convert.ToDecimal(salesPriceResult), 5);
                            eachday_amount_value = eachday_value;
                        }

                        string frequency_json = @"fname__" + month_year + "__fvalue__" + amtQty;

                        string insertinto_plandtl = $@"SELECT '{planCode}',TO_DATE('{date.DAYS}','DD-MON-YYYY'),'{eachday_quantity_value}','{eachday_amount_value}','{item.ITEM_CODE}',
                                '{item.CUSTOMER_CODE}','{item.EMPLOYEE_CODE}','{item.DIVISION_CODE}','{item.PARTY_TYPE}','',
                                '{companyCode}','{spbranchCode}','{userID}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/YYYY'),'','','N','{frequency_json}','{salesPriceResult}' FROM DUAL UNION ALL ";

                        sbInsertQuery += insertinto_plandtl;
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("unique constraint"))
                        throw;

                }
                if (itteration <= (sv.Count() - remainderCount))
                {
                    while (itemCount == itemRange)
                    {
                        string query = @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) "
+ sbInsertQuery;
                        query = query.Substring(0, query.Length - 11);
                        insert_query_array[array_count] = query;
                        array_count++;
                        itemCount = 0;
                        sbInsertQuery = string.Empty;
                    }
                }
                else
                {
                    while (itemCount == remainderCount)
                    {
                        string query = @"insert into PL_SALES_PLAN_DTL(PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                    CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON,SALES_PRICE_APPLIED) "
+ sbInsertQuery;
                        query = query.Substring(0, query.Length - 11);
                        insert_query_array[array_count] = query;
                        array_count++;
                        itemCount = 0;
                        sbInsertQuery = string.Empty;
                    }
                }
            }
            try
            {
                var insertItems = insert_query_array.Where(a => a != null);
                foreach (var item in insertItems)
                {
                    result = _dbContext.ExecuteSqlCommand(item.ToString()).ToString();
                }

            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("unique constraint"))
                    throw;
                else
                {
                    var delQ = $@"DELETE FROM PL_SALES_PLAN WHERE PLAN_CODE='{planCode}' AND COMPANY_CODE='{companyCode}'";
                    _dbContext.ExecuteSqlCommand(delQ);
                    var deldQ = $@"DELETE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE='{planCode}' AND COMPANY_CODE='{companyCode}'";
                    _dbContext.ExecuteSqlCommand(deldQ);
                }
            }

            string update_amout_quantity = string.Empty;
            if (sv[0].AMOUNT_QUANTITY.Trim().ToLower() == "qty")
            {
                update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_QUANTITY = (
                        SELECT ROUND(SUM(SPD.PER_DAY_QUANTITY)) FROM PL_SALES_PLAN_DTL SPD, IP_ITEM_MASTER_SETUP IMS
                        WHERE 
                        IMS.GROUP_SKU_FLAG ='I'
                        AND SPD.ITEM_CODE = IMS.ITEM_CODE
                           AND SPD.COMPANY_CODE=IMS.COMPANY_CODE
                        AND SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_AMOUNT=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
            }
            else if (sv[0].AMOUNT_QUANTITY.Trim().ToLower() == "amt")
            {
                update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_AMOUNT = (
                        SELECT ROUND(SUM(SPD.PER_DAY_AMOUNT)) FROM PL_SALES_PLAN_DTL SPD, IP_ITEM_MASTER_SETUP IMS
                        WHERE 
                        IMS.GROUP_SKU_FLAG ='I'
                        AND SPD.ITEM_CODE = IMS.ITEM_CODE
                          AND SPD.COMPANY_CODE=IMS.COMPANY_CODE
                        AND SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_QUANTITY=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
            }
            this._dbContext.ExecuteSqlCommand(update_amout_quantity);
            //subin changes
            var preference = GetPreferenceSetups().ToList().Where(x => x.PL_NAME == "SALES_PLAN").FirstOrDefault();
            if (preference.SHOW_ITEM == "N")
            {
                if (sv[0].AMOUNT_QUANTITY.Trim().ToLower() == "qty")
                {
                    update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_QUANTITY = (
                        SELECT ROUND(SUM(SPD.PER_DAY_QUANTITY)) FROM PL_SALES_PLAN_DTL SPD
                        WHERE SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_AMOUNT=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
                }
                else if (sv[0].AMOUNT_QUANTITY.Trim().ToLower() == "amt")
                {
                    update_amout_quantity = $@"UPDATE PL_SALES_PLAN PSP SET PSP.SALES_AMOUNT = (
                        SELECT ROUND(SUM(SPD.PER_DAY_AMOUNT)) FROM PL_SALES_PLAN_DTL SPD
                        WHERE SPD.PLAN_CODE='{planCode}')
                        , PSP.SALES_QUANTITY=NULL
                        WHERE PSP.PLAN_CODE='{planCode}'";
                }
                this._dbContext.ExecuteSqlCommand(update_amout_quantity);
            }

            return result.ToString();
        }

        public List<ProductSetupModel> GetProductRateByPlanCode(int plancode)
        {

            string query = $@"select sum(per_day_amount)/sum(per_day_quantity) Rate, spd.item_code from pl_sales_plan_dtl spd,IP_ITEM_MASTER_SETUP ims where plan_code = '{plancode} and ims.item_code = spd.item_code and ims.company_code = spd.company_code and ims.group_sku_flag='I' group by spd.item_code";

            var planlist = this._dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return planlist;
        }

        public string CreateTempSalesPlanReportTable(ReportFiltersModel model)
        {
            var companyCode = _workcontext.CurrentUserinformation.company_code;
            var response = "";
            var createTableQry = $@"CREATE TABLE TEMP_PL_SALES_PLAN_REPORT AS SELECT DISTINCT 
                                        TO_CHAR(fn_bs_month(substr(bs_date(SPD.PLAN_DATE), 6, 2))) MONTH,
                                        SUBSTR(BS_DATE(SPD.PLAN_DATE), 6, 2) MONTHINT,
                                        TO_CHAR(substr(bs_date(SPD.PLAN_DATE), 0, 4)) as YEAR,   SPD.ITEM_CODE, IMS.ITEM_EDESC,    SPD.CUSTOMER_CODE, SPD.EMPLOYEE_CODE,
                                   SPD.DIVISION_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC, FS.BRANCH_CODE, SPD.COMPANY_CODE,
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC  ,
                                   SUM(SPD.PER_DAY_AMOUNT) PER_DAY_AMOUNT, 
                                   SUM(SPD.PER_DAY_QUANTITY) PER_DAY_QUANTITY,
                                    SP.PLAN_EDESC,
                                    SPD.PLAN_DATE
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
                                   ES.EMPLOYEE_EDESC, FS.BRANCH_EDESC ,FS.BRANCH_CODE , SPD.COMPANY_CODE,SP.PLAN_EDESC,SPD.PLAN_DATE";
            try
            {
                var tableExistsQry = $@"SELECT * FROM TEMP_PL_SALES_PLAN_REPORT";
                var result = this._dbContext.SqlQuery<PlanReportModel>(tableExistsQry).ToList();
                if (result != null)
                {
                    var dropQry = $@"DROP TABLE TEMP_PL_SALES_PLAN_REPORT";
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
                    var dropQry = $@"DROP TABLE TEMP_PL_SALES_PLAN_REPORT";
                    var dropResponse = this._dbContext.ExecuteSqlCommand(dropQry);
                    response = ex.Message;
                }

            }
            return response;
        }

        public string SaveEmployeeHandover(EmployeeHandoverModel model)
        {
            var result = string.Empty;
            var info = this._workcontext.CurrentUserinformation;
            var from_date = model.FROM_DATE;
            if (string.IsNullOrEmpty(model.FROM_EMPLOYEE_CODE) || string.IsNullOrEmpty(model.TO_EMPLOYEE_CODE))
                return result = "Please choose employee.";
            if (model.FROM_EMPLOYEE_CODE == model.TO_EMPLOYEE_CODE)
                return result = "Same employee not allowed to migrate.";
            var trans = _objContext.Database.BeginTransaction();
            try
            {
                var count = 0;
                var planListQry = $@"SELECT DISTINCT PLAN_CODE FROM PL_SALES_PLAN_DTL WHERE EMPLOYEE_CODE='{model.FROM_EMPLOYEE_CODE}' AND TO_DATE('{from_date}','YYYY-MM-DD') >= PLAN_DATE AND DELETED_FLAG='N' AND COMPANY_CODE='{info.company_code}'";
                var planList = _objContext.SqlQuery<PlanListModelForMigrate>(planListQry).ToList();
                foreach (var item in planList)
                {
                    string existingPlanNameQuery = $@"SELECT PLAN_EDESC FROM PL_SALES_PLAN WHERE PLAN_CODE='{item.PLAN_CODE}'";
                    string existingPlanName = this._objContext.SqlQuery<String>(existingPlanNameQuery).FirstOrDefault();
                    if (existingPlanName != null)
                    {
                        string copyPlanName = existingPlanName + "_migrate";
                        string maxPlanCodeQuery = "SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_SALES_PLAN";
                        int maxPlanCode = this._objContext.SqlQuery<int>(maxPlanCodeQuery).First();

                        string copySalesPlanQuery = $@"INSERT INTO PL_SALES_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                                            CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                                            DELETED_FLAG)
                                            SELECT '{maxPlanCode}','{copyPlanName}', PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                                            CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                                            DELETED_FLAG FROM PL_SALES_PLAN WHERE DELETED_FLAG='N' AND TO_DATE('{from_date}','YYYY-MM-DD') BETWEEN START_DATE AND END_DATE AND COMPANY_CODE='{info.company_code}' AND PLAN_CODE='{item.PLAN_CODE}'";
                        var insertCopiedPlan = this._objContext.ExecuteSqlCommand(copySalesPlanQuery);
                        if (insertCopiedPlan <= 0)
                            continue;
                        int copiedPlanCode = maxPlanCode;
                        if (copiedPlanCode > 0)
                        {
                            string copyinto_plandtl = $@"INSERT INTO PL_SALES_PLAN_DTL
                                (PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                                CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON)
                                SELECT '{copiedPlanCode}', PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                                CUSTOMER_CODE,'{model.TO_EMPLOYEE_CODE}',DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON FROM PL_SALES_PLAN_DTL WHERE DELETED_FLAG='N'
                                AND COMPANY_CODE='{info.company_code}' AND PLAN_CODE='{item.PLAN_CODE}' AND plan_date >= TO_DATE('{from_date}','YYYY-MM-DD')";

                           var insertedDataCount = _objContext.ExecuteSqlCommand(copyinto_plandtl);
                            if (insertedDataCount <= 0)
                                throw new Exception("There is no data");

                            string updateQry = $@"DELETE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE='{item.PLAN_CODE}' AND EMPLOYEE_CODE='{model.FROM_EMPLOYEE_CODE}' AND COMPANY_CODE='{info.company_code}' AND DELETED_FLAG='N' AND PLAN_DATE >= TO_DATE('{from_date}','YYYY-MM-DD')";
                            var updatedDataCount = _objContext.ExecuteSqlCommand(updateQry);
                            if (updatedDataCount <= 0)
                                throw new Exception("There is no data for update");
                        }
                        var masterPlanQry = $@"SELECT TO_CHAR(MASTER_PLAN_CODE)MASTER_PLAN_CODE,TO_CHAR(PLAN_CODE)PLAN_CODE,TO_CHAR(PARENT_PLAN_CODE)PARENT_PLAN_CODE,PLAN_FLAG FROM PL_SALES_PLAN_MAP WHERE PLAN_CODE='{item.PLAN_CODE}'";
                        var masterMapResult = _objContext.SqlQuery<SalesPlanMap>(masterPlanQry).ToList();
                        if (masterMapResult.Count() > 0)
                        {
                            var insertQry = $@"INSERT INTO PL_SALES_PLAN_MAP
                                                SELECT MASTER_PLAN_CODE, {copiedPlanCode} PLAN_CODE,PARENT_PLAN_CODE,PLAN_FLAG FROM PL_SALES_PLAN_MAP WHERE PLAN_CODE='{item.PLAN_CODE}'";
                        }
                    }
                    var query = $@"INSERT INTO PL_EMPLOYEE_HANDOVER(HANDOVER_CODE,FROM_EMPLOYEE_CODE,TO_EMPLOYEE_CODE,FROM_DATE,PLAN_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                                VALUES((SELECT NVL(MAX(HANDOVER_CODE),0)+1 FROM PL_EMPLOYEE_HANDOVER),'{model.FROM_EMPLOYEE_CODE}','{model.TO_EMPLOYEE_CODE}', TO_DATE('{from_date}','YYYY-MM-DD'),'{item.PLAN_CODE}','{info.company_code}','{info.User_id}',SYSDATE,'N')";
                    var response = this._objContext.ExecuteSqlCommand(query);
                    count = count + response;
                }
                if (count > 0)
                {
                    trans.Commit();
                    result = "success";
                }
                else
                {
                    trans.Rollback();
                    result = "";
                }   
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            return result;
        }
        public plFiscalYearModel GetFiscalYear(string fiscalYearCode)
        {
            string sqlQuery = $@"SELECT START_DATE,END_DATE FROM PL_FISCAL_YEAR_CODE WHERE FISCAL_YEAR_CODE='{fiscalYearCode}' AND ACTIVE='Y' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
            var data = _dbContext.SqlQuery<plFiscalYearModel>(sqlQuery).FirstOrDefault();
            return data;
        }
    }
}
