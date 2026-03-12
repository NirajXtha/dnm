using System;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Caching;
using NeoErp.Data;

namespace NeoErp.Core.Services
{
    public class ControlService : IControlService
    {
        private ICacheManager _cacheManager;
        private IDbContext _dbContext;
        
        public ControlService(ICacheManager cacheManager, IDbContext dbContext)
        {
            this._cacheManager = cacheManager;
            this._dbContext = dbContext;
        }

        public IEnumerable<DateFilterModel> GetDateFilters(string fiscalYear, string textToAppend="", bool appendText = false, int substractYear = 0)
        {
            if (string.IsNullOrEmpty(fiscalYear))
                return null;

            //if (this._cacheManager.IsSet("calender-date-filter"))
            //    return this._cacheManager.Get<List<DateFilterModel>>("calender-date-filter");

            string query = string.Format(Constants.GetDateFiltersQuery, fiscalYear, substractYear);


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
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(-1),
                StartDateString = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                EndDateString = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                RangeName = "Yesterday",
            });
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

        public IEnumerable<DateFilterModel> GetDateFiltersWithWeek(string fiscalYear, string textToAppend = "", bool appendText = false, int substractYear = 0)
        {
            if (string.IsNullOrEmpty(fiscalYear))
                return null;

            //if (this._cacheManager.IsSet("calender-date-filter"))
            //    return this._cacheManager.Get<List<DateFilterModel>>("calender-date-filter");

            string query = string.Format(Constants.GetDateFiltersWithWeekQuery, fiscalYear, substractYear);


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
        public IEnumerable<DateFilterModel> GetEnglishDateFilters(string fiscalYear, string textToAppend = "", bool appendText = false)
        {
            if (string.IsNullOrEmpty(fiscalYear))
                return null;
            
            string query = string.Format(@"
  SELECT StartDate,
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

        public IEnumerable<DateFilterModel> GetNepaliDateFilters(string FiscalYear)
        {
            if (string.IsNullOrEmpty(FiscalYear))
                return null;
            string query = @"SELECT StartDate, EndDate, RangeName, SortOrder FROM(
                                                  SELECT CS.AD_DATE StartDate,
                                                         (CS.AD_DATE + CS.DAYS_NO - 1) EndDate,
                                                        FN_BS_MONTH(SUBSTR(CS.BS_MONTH, -2, 2)) RangeName, 1 AS SortOrder
 
                                                    FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
 
                                                  WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
 
                                                         AND FY.FISCAL_YEAR_CODE = '"+FiscalYear+"')";
            var data = _dbContext.SqlQuery<DateFilterModel>(query);
            return data;

        }
    }
}