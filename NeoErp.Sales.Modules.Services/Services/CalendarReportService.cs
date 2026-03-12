using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using NeoErp.Sales.Modules.Services.Models.CalendarReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class CalendarReportService : ICalendarReportService
    {
        private NeoErpCoreEntity _objectEntity;
        private ICacheManager _cacheManager { get; set; }
        private IAgeingFactory _ageingFactory;
        private IVoucherService _voucherService { get; set; }
        private IConstants _constants { get; set; }
        private IWorkContext _workContext { get; set; }

        public CalendarReportService(NeoErpCoreEntity objectEntity, IVoucherService voucherService, ICacheManager cacheManager,IAgeingFactory ageingFactory, IConstants constants, IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            this._cacheManager = cacheManager;
            this._ageingFactory = ageingFactory;
            this._voucherService = voucherService;
            this._constants = constants;
            this._workContext = workContext;
        }

        public List<ReportModel> GetReportTitle()
        {

            var user = this._workContext.CurrentUserinformation;

            string query = string.Format(@"SELECT 
                            REPORT_CODE AS ReportCode,
                            INITCAP(REPORT_EDESC) AS ReportName
                            FROM CALENDAR_REPORT_SETUP
                            WHERE 
                            COMPANY_CODE='{0}'
                            --AND DELETED_FLAG='N'
                            AND VERTICAL_TABLE_NAME IS NOT NULL
                            AND SOURCE_DATA_ENTITY IS NOT NULL
                            AND SOURCE_DATA_COLUMN IS NOT NULL
                            AND SOURCE_DATA_COLUMN2 IS NOT NULL
                            AND COLUMN1_HEADING IS NOT NULL
                            AND COLUMN2_HEADING IS NOT NULL", user!= null && !string.IsNullOrEmpty(user.company_code)?user.company_code:"01");
            var reportTitleList = _objectEntity.SqlQuery<ReportModel>(query).ToList();
            return reportTitleList;
        }

        public List<ReportSetupModel> GetReportTitleNodes()
        {
            string query = @"SELECT LEVEL,
                               LPAD (' ', 2 * (LEVEL - 1)) || REPORT_EDESC ReportName,
                               --INITCAP(REPORT_EDESC) AS ReportName,
                               REPORT_CODE AS ReportCode,
                               MASTER_REPORT_CODE AS MasterReportCode, 
                               PRE_REPORT_CODE AS PreReportCode, 
                               GROUP_SKU_FLAG AS GroupFlag
                               FROM CALENDAR_REPORT_SETUP
                               WHERE DELETED_FLAG = 'N' 
                               AND COMPANY_CODE = '01'
                               --AND GROUP_SKU_FLAG = 'G'
                               AND VERTICAL_TABLE_NAME IS NOT NULL
                               AND SOURCE_DATA_ENTITY IS NOT NULL
                               AND SOURCE_DATA_COLUMN IS NOT NULL
                               AND SOURCE_DATA_COLUMN2 IS NOT NULL
                               AND COLUMN1_HEADING IS NOT NULL
                               AND COLUMN2_HEADING IS NOT NULL             
                               START WITH PRE_REPORT_CODE = '00'
                               CONNECT BY PRIOR MASTER_REPORT_CODE = PRE_REPORT_CODE
                               AND COMPANY_CODE = '01'";
            var reportTitleNodes = _objectEntity.SqlQuery<ReportSetupModel>(query).ToList();
            return reportTitleNodes;
        }

        public List<ReportSetupModel> GetReportTitleNodes(User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = @"SELECT LEVEL,
                               LPAD (' ', 2 * (LEVEL - 1)) || REPORT_EDESC ReportName,
                               --INITCAP(REPORT_EDESC) AS ReportName,
                               REPORT_CODE AS ReportCode,
                               MASTER_REPORT_CODE AS MasterReportCode, 
                               PRE_REPORT_CODE AS PreReportCode, 
                               GROUP_SKU_FLAG AS GroupFlag
                               FROM CALENDAR_REPORT_SETUP
                               WHERE DELETED_FLAG = 'N' 
                               AND COMPANY_CODE = '01'
                               --AND GROUP_SKU_FLAG = 'G'
                               AND VERTICAL_TABLE_NAME IS NOT NULL
                               AND SOURCE_DATA_ENTITY IS NOT NULL
                               AND SOURCE_DATA_COLUMN IS NOT NULL
                               AND SOURCE_DATA_COLUMN2 IS NOT NULL
                               AND COLUMN1_HEADING IS NOT NULL
                               AND COLUMN2_HEADING IS NOT NULL             
                               START WITH PRE_REPORT_CODE = '00'
                               CONNECT BY PRIOR MASTER_REPORT_CODE = PRE_REPORT_CODE
                               AND COMPANY_CODE = '"+userinfo.company_code+"'";
            var reportTitleNodes = _objectEntity.SqlQuery<ReportSetupModel>(query).ToList();
            return reportTitleNodes;
        }

        public IEnumerable<CalendarColumnRange> GenerateColumns(string firstHorizontalPeriod, string secondHorizontalPeriod, string asOnDate)
        {
           
            List<CalendarColumnRange> calendarColumns = new List<CalendarColumnRange>();
            List<CalendarCustomerDataModel> customerList = new List<CalendarCustomerDataModel>();

            if (firstHorizontalPeriod == PeriodFilter.Year.ToString() && secondHorizontalPeriod == PeriodFilter.Month.ToString())
            {
                customerList = _objectEntity.SqlQuery<CalendarCustomerDataModel>(_constants.CalendarReportMonthlyPartyWiseSalesQuery()).ToList();
                foreach (var cust in customerList)
                {
                    calendarColumns.Add(new CalendarColumnRange()
                    {
                        Description = cust.CustomerName,
                        Id = Convert.ToInt32(cust.CustomerCode),
                        SubColumnFirst = cust.Price.ToString(),
                        SubColumnSecond = cust.Quantity.ToString(),
                        FirstHorizontalPeriod = cust.YearName.ToString(),
                        SecondHorizontalPeriod = cust.MonthName,
                        Total = cust.Total
                    });
                }
            }
            if ((firstHorizontalPeriod == PeriodFilter.Year.ToString() || firstHorizontalPeriod == PeriodFilter.Month.ToString()) && secondHorizontalPeriod == PeriodFilter.Week.ToString())
            {


                string Query = string.Format(@"with yourtable as(select to_date('{0}','YYYY-MM-DD')+(rownum-1) dat from dual connect by to_date('{0}','YYYY-MM-DD')+(rownum-1)<=to_date('{1}','YYYY-MM-DD'))select
                                         distinct     to_char(dat,'WW') as WeekNumber , to_char(dat,'Month') as MonthName,to_char(dat,'YYYY') as Year
                                         from yourtable
                                        order by to_char(dat,'WW')", "2015-9-1", "2015-12-1");
                var AllWeeks = _objectEntity.SqlQuery<WeekNumberByMonth>(Query).ToList();
                var calendarColumnweek = new CalendarColumnRange();
                foreach (var year in AllWeeks.GroupBy(customer => customer.Year).Select(group => group.First()))
                {
                    var Horizontal = new FirstHorizontalPeriodModel();
                    Horizontal.HeaderName = year.Year;
                    foreach (var month in AllWeeks.GroupBy(customer => customer.MonthName).Select(group => group.First()))
                    {
                        var secondHorizantal = new SecondHOrizontalPeriodModel();
                        secondHorizantal.HeaderName = month.MonthName;
                        var monthWise = AllWeeks.Where(x => x.MonthName == month.MonthName && x.Year == year.Year);
                        foreach (var week in monthWise)
                        {
                            var thirdHorizontal = new ThirdHorizontalColumnModel();
                            thirdHorizontal.HeaderName = "Week " + week.WeekNumber;

                            var forthcol = new ForthHorizontalColumnModel();
                            forthcol.HeaderName = "Qty";
                            forthcol.HeaderVlaue = week.WeekNumber.Trim() + month.MonthName.Trim() + year.Year.Trim();
                            thirdHorizontal.forthHorizontalColumn.Add(forthcol);
                            var forthcol1 = new ForthHorizontalColumnModel();
                            forthcol1.HeaderName = "Amount";
                            forthcol1.HeaderVlaue = week.WeekNumber.Trim() + month.MonthName.Trim() + year.Year.Trim();
                            thirdHorizontal.forthHorizontalColumn.Add(forthcol1);

                            secondHorizantal.ThirdHorizontalColumn.Add(thirdHorizontal);
                        }
                        Horizontal.secondHorizontalPeriodModel.Add(secondHorizantal);

                    }
                    calendarColumnweek.FirstColumnlist.Add(Horizontal);
                }
                calendarColumns.Add(calendarColumnweek);
                return calendarColumns;
            }

            return calendarColumns;
        }

        /// <summary>
        /// Renders the rows and columns required for the report to be used by Header partial page
        /// </summary>
        /// <param name="calenderFilter"></param>
        /// <returns>CalendarColumnRange</returns>
        public CalendarColumnRange GenerateColumnsGeneric(CalendarFilterModel calenderFilter)
        {
            var calendarColumnRange = new CalendarColumnRange();
            int filterId = 0;
            if (calenderFilter.ReportId != null)
                filterId = Convert.ToInt16(calenderFilter.ReportId);
            var columnNames = this.GetCalenderSetup(filterId).FirstOrDefault();

            if (columnNames == null)
                return calendarColumnRange;

            if (Convert.ToChar(calenderFilter.FirstHorizontalPeriod) == (Char)PeriodFilter.Year && Convert.ToChar(calenderFilter.SecondHorizontalPeriod) == (char)PeriodFilter.Month)
            {
                calendarColumnRange = this.GetMonthlyCalendarColumnRange(calenderFilter, columnNames);
            }

            else if ((Convert.ToChar(calenderFilter.FirstHorizontalPeriod) == (Char)PeriodFilter.Year || Convert.ToChar(calenderFilter.FirstHorizontalPeriod) == (Char)PeriodFilter.Month) && Convert.ToChar(calenderFilter.SecondHorizontalPeriod) == (char)PeriodFilter.Week)
            {
                calendarColumnRange = this.GetWeeklyCalendarColumnRange(calenderFilter, columnNames);
            }

            else if ((Convert.ToChar(calenderFilter.FirstHorizontalPeriod) == (char)PeriodFilter.Year || Convert.ToChar(calenderFilter.FirstHorizontalPeriod) == (char)PeriodFilter.Month || Convert.ToChar(calenderFilter.FirstHorizontalPeriod) == (char)PeriodFilter.Week) && Convert.ToChar(calenderFilter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
            {
                calendarColumnRange = this.GetDailyCalendarColumnRange(calenderFilter, columnNames);
            }

            calendarColumnRange.SubColumnFirst = columnNames.column1_heading;
            calendarColumnRange.SubColumnSecond = columnNames.column2_heading;
            calendarColumnRange.ReportName = columnNames.report_edesc;
            return calendarColumnRange;
        }

        public IEnumerable<CalendarDataViewModel> GetCalendarViewReport(CalendarFilterModel model)
        {

            List<CalendarDataViewModel> calendarReportDataViewList = new List<CalendarDataViewModel>();
            List<CalendarCustomerDataModel> customerDataList = new List<CalendarCustomerDataModel>();

            List<CalendarColumnRange> calendarColumns = this.GenerateColumns(model.FirstHorizontalPeriod, model.SecondHorizontalPeriod, model.formDate).ToList();


            var distinctCode = calendarColumns.Select(q => q.Id).Distinct();
            foreach (var subCode in distinctCode)
            {
                var calendarItemByColumn = new CalendarDataViewModel()
                {
                    Id = subCode.ToString(),
                    Description = calendarColumns.Where(q => q.Id == subCode).FirstOrDefault() != null ? calendarColumns.Where(q => q.Id == subCode).FirstOrDefault().Description : ""
                };

                foreach (var col in calendarColumns.Where(x => x.Id == subCode))
                {

                    calendarItemByColumn.RangeColumnData.Add(new CalendarDataViewModel.CalendarColumnRangeData
                    {
                        ColumnRangeNameFirst = col.FirstHorizontalPeriod,
                        ColumnRangeNameSecond = col.SecondHorizontalPeriod,
                        NetAmount = col.Total,
                        SubColumnFirst = col.SubColumnFirst,
                        SubColumnSecond = col.SubColumnSecond

                    });
                }

                calendarItemByColumn.NetAmount = calendarItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                calendarReportDataViewList.Add(calendarItemByColumn);
            }
            return calendarReportDataViewList.ToList();
        }

        public List<CalendarCustomerDataModel> GetWeekWiseCalenderReport(string calenderType = "A")
        {
            string Query = string.Empty;
            if (calenderType == CalenderType.Ad.ToString())
            {
                Query = "SELECT cs.customer_code as Id,CS.CUSTOMER_EDESC  as Description,  to_char(SI.SALES_DATE - 7/24,'IW') as week, to_char(SI.SALES_DATE ,'Month') as month, to_char(SI.SALES_DATE ,'YYYY') as year,SUM(SI.QUANTITY) as Quantity, SUM(SI.CALC_TOTAL_PRICE ) as NetAmount FROM SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS" +
                                  " WHERE SI.CUSTOMER_CODE = CS.CUSTOMER_CODE" +
                                  " AND SI.SALES_DATE >= TO_DATE ('2015-9-1', 'YYYY - MM - DD')" +
                                   " AND SI.SALES_DATE <= TO_DATE('2015-12-1', 'YYYY-MM-DD')" +
                              " GROUP BY CS.CUSTOMER_EDESC,  cs.customer_code, to_char(SI.SALES_DATE - 7 / 24, 'IW'), to_char(SI.SALES_DATE, 'Month'), to_char(SI.SALES_DATE, 'YYYY')";
            }
            else
            {
                Query = "SELECT cs.customer_code AS Id, CS.CUSTOMER_EDESC AS Description, to_char(np.weeks) as week,np.Nepalimonth AS month,np.Nepaliyear AS year," +
                        " SUM(SI.QUANTITY) AS Quantity,SUM (SI.CALC_TOTAL_PRICE)AS NetAmount" +
                        " FROM SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS, (select  BS_DATE(a) as Miti, a as AdDate, fn_bs_month(substr(bs_date(a), 6, 2)) as Nepalimonth, to_char(a, 'Month') as month, substr(bs_date(a), 0, 4) as Nepaliyear, trunc((a - TO_DATE('17-Jul-2015')) / 7) + 1  weeks from(" +
                        " SELECT ROWNUM - 1 + TO_DATE('17-Jul-2015', 'dd-mon-yyyy') a FROM all_objects" +
                        " WHERE ROWNUM < TO_DATE('15-Jul-2016', 'dd-mon-yyyy')" +
                        " - TO_DATE('17-Jul-2015', 'dd-mon-yyyy') + 2) ) NP" +
                        " WHERE     SI.CUSTOMER_CODE = CS.CUSTOMER_CODE" +
                        " AND SI.SALES_DATE >= TO_DATE('17-Jul-2015', 'dd-mon-yyyy')" +
                        "  AND SI.SALES_DATE <= TO_DATE('15-Jul-2016', 'dd-mon-yyyy')" +
                        " and np.adDate = si.sales_date" +
                        " GROUP BY CS.CUSTOMER_EDESC, cs.customer_code," +
                        " np.weeks, np.Nepalimonth ,np.Nepaliyear ";
            }
            var calenderdata = _objectEntity.SqlQuery<CalendarDataViewModel>(Query).ToList();
            var abcParent = new List<CalendarCustomerDataModel>();
            foreach (var calender in calenderdata.GroupBy(customer => customer.Id).Select(group => group.First()))
            {

                var abc = new CalendarCustomerDataModel();
                abc.id = Convert.ToInt64(calender.Id);
                abc.parentId = 0;
                abc.CustomerCode = calender.Id.ToString();
                abc.CustomerName = calender.Description;
                var allIndivsual = calenderdata.Where(x => x.Id == calender.Id).ToList();

                foreach (var indv in allIndivsual)
                {
                    var cust = new CustomerWeekWise();
                    cust.MonthName = indv.month;
                    cust.yearname = indv.year;
                    cust.weekName = indv.week;
                    cust.totalQty = indv.Quantity;
                    cust.TotalAmount = indv.NetAmount;
                    abc.additionalData.Add(cust);

                }
                abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                abc.Price = abc.additionalData.Sum(x => x.totalQty);
                abcParent.Add(abc);

            }
            return abcParent;
        }

        public List<CalendarCustomerDataModel> GetDaysWiseCalenderReport(string calenderType = "A")
        {
            string Query = string.Empty;
            if (calenderType == CalenderType.Ad.ToString())
            {
                Query = "SELECT cs.customer_code as Id,CS.CUSTOMER_EDESC  as Description,  to_char(SI.SALES_DATE - 7/24,'IW') as week, to_char(SI.SALES_DATE ,'Month') as month, to_char(SI.SALES_DATE ,'YYYY') as year,SUM(SI.QUANTITY) as Quantity, SUM(SI.CALC_TOTAL_PRICE ) as NetAmount FROM SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS" +
                                  " WHERE SI.CUSTOMER_CODE = CS.CUSTOMER_CODE" +
                                  " AND SI.SALES_DATE >= TO_DATE ('2015-9-1', 'YYYY - MM - DD')" +
                                   " AND SI.SALES_DATE <= TO_DATE('2015-12-1', 'YYYY-MM-DD')" +
                              " GROUP BY CS.CUSTOMER_EDESC,  cs.customer_code, to_char(SI.SALES_DATE - 7 / 24, 'IW'), to_char(SI.SALES_DATE, 'Month'), to_char(SI.SALES_DATE, 'YYYY')";
            }
            else
            {

                Query = "SELECT cs.customer_code AS Id, CS.CUSTOMER_EDESC AS Description, to_char(np.adDate) as week,np.Nepalimonth AS month,np.Nepaliyear AS year," +
                        " SUM(SI.QUANTITY) AS Quantity,SUM (SI.CALC_TOTAL_PRICE)AS NetAmount" +
                        " FROM SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS, (select  BS_DATE(a) as Miti, a as AdDate, fn_bs_month(substr(bs_date(a), 6, 2)) as Nepalimonth, to_char(a, 'Month') as month, substr(bs_date(a), 0, 4) as Nepaliyear, trunc((a - TO_DATE('17-Jul-2015')) / 7) + 1  weeks from(" +
                        " SELECT ROWNUM - 1 + TO_DATE('17-Jul-2015', 'dd-mon-yyyy') a FROM all_objects" +
                        " WHERE ROWNUM < TO_DATE('15-Jul-2016', 'dd-mon-yyyy')" +
                        " - TO_DATE('17-Jul-2015', 'dd-mon-yyyy') + 2) ) NP" +
                        " WHERE     SI.CUSTOMER_CODE = CS.CUSTOMER_CODE" +
                        " AND SI.SALES_DATE >= TO_DATE('17-Jul-2015', 'dd-mon-yyyy')" +
                        "  AND SI.SALES_DATE <= TO_DATE('15-Jul-2016', 'dd-mon-yyyy')" +
                        " and np.adDate = si.sales_date" +
                        " GROUP BY CS.CUSTOMER_EDESC, cs.customer_code," +
                        " np.adDate, np.Nepalimonth ,np.Nepaliyear ";
            }
            var calenderdata = _objectEntity.SqlQuery<CalendarDataViewModel>(Query).ToList();
            var abcParent = new List<CalendarCustomerDataModel>();
            foreach (var calender in calenderdata.GroupBy(customer => customer.Id).Select(group => group.First()))
            {

                var abc = new CalendarCustomerDataModel();
                abc.id = Convert.ToInt64(calender.Id);
                abc.parentId = 0;
                abc.CustomerCode = calender.Id.ToString();
                abc.CustomerName = calender.Description;
                var allIndivsual = calenderdata.Where(x => x.Id == calender.Id).ToList();

                foreach (var indv in allIndivsual)
                {
                    var cust = new CustomerWeekWise();
                    cust.MonthName = indv.month;
                    cust.yearname = indv.year;
                    cust.weekName = indv.week;
                    cust.totalQty = indv.Quantity;
                    cust.TotalAmount = indv.NetAmount;
                    abc.additionalData.Add(cust);

                }
                abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                abc.Price = abc.additionalData.Sum(x => x.totalQty);
                abcParent.Add(abc);

            }
            return abcParent;
        }
        
        private string GetQueryIfStartsWith_SA(CalendarFilterModel filter, CalenderReportSetup calenderReport, string calenderQuery, DatabaseColumn dateColumn)
        {

            var user = this._workContext.CurrentUserinformation;
            string query = string.Empty;
            var formDate = Convert.ToDateTime(filter.formDate).ToString("dd-MMM-yyyy");
            var toDate = Convert.ToDateTime(filter.toDate).ToString("dd-MMM-yyyy");
            if (calenderReport.vertical_table_name.Trim() == "SA_CUSTOMER_SETUP")
            {
                query = string.Format(Constants.CustomerWise_Calendar_Query,
                                        calenderReport.source_data_column,
                                        calenderReport.source_data_column2,
                                        calenderReport.source_data_entity,
                                        calenderReport.vertical_table_name,
                                        calenderQuery,
                                        dateColumn.COLUMN_NAME,
                                        formDate,
                                       toDate,
                                        user != null && !string.IsNullOrEmpty(user.company_code)?user.company_code:"01");

                if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Bs)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = "to_char(np.NepaliMonthInt) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.CUSTOMER_EDESC, fa.customer_code," +
                                        " np.NepaliMonthInt, np.Nepalimonth ,np.Nepaliyear,fa.master_customer_code,FA.PRE_CUSTOMER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.CUSTOMER_EDESC, fa.customer_code," +
                                        " np.weeks, np.Nepalimonth ,np.Nepaliyear,fa.master_customer_code,FA.PRE_CUSTOMER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var monthwiseQuery = "to_char(np.AdDate) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.CUSTOMER_EDESC, fa.customer_code," +
                                        " np.AdDate, np.Nepalimonth ,np.Nepaliyear,fa.master_customer_code,FA.PRE_CUSTOMER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                }
                // Code for Ad Calendar
                else if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Ad)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = " np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE," +
                                    " np.month, np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE," +
                                        " np.weeks, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var monthwiseQuery = "to_char(np.AdDate) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE," +
                                        " np.AdDate, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                }
            }
            else if (calenderReport.vertical_table_name.Trim() == "IP_ITEM_MASTER_SETUP")
            {
                query = string.Format(Constants.ItemWise_Calendar_Query,
                                       calenderReport.source_data_column,
                                       calenderReport.source_data_column2,
                                       calenderReport.source_data_entity,
                                       calenderReport.vertical_table_name,
                                       calenderQuery,
                                       dateColumn.COLUMN_NAME,
                                     formDate,
                                       toDate,
                                       user != null && !string.IsNullOrEmpty(user.company_code) ? user.company_code : "01");

                if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Bs)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = "to_char(np.NepaliMonthInt) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE," +
                                        " np.NepaliMonthInt, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var weekwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE," +
                                        " np.weeks, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE";
                        query = query.Replace("#calenderWise", weekwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var daywiseQuery = "to_char(np.AdDate) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE," +
                                        " np.AdDate, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE";
                        query = query.Replace("#calenderWise", daywiseQuery);
                        query = query + groupwise;
                    }
                }
                // Code for Ad Calendar
                else if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Ad)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = " np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                    " np.month, np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var weekwiseQuery = "to_char(np.weeks) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                    " np.weeks, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", weekwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var daywiseQuery = "to_char(np.AdDate) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                        " np.AdDate, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", daywiseQuery);
                        query = query + groupwise;
                    }
                }
            }

            return query;
        }
        
        private string GetQueryIfStartsWith_IP(CalendarFilterModel filter, CalenderReportSetup calenderReport, string calenderQuery, DatabaseColumn dateColumn)
        {

            var user = this._workContext.CurrentUserinformation;
            string query = string.Empty;
            var formDate = Convert.ToDateTime(filter.formDate).ToString("dd-MMM-yyyy");
            var toDate = Convert.ToDateTime(filter.toDate).ToString("dd-MMM-yyyy");
            if (calenderReport.vertical_table_name.Trim() == "IP_SUPPLIER_SETUP")
            {
                query = string.Format(Constants.SupplierWise_Calendar_Query,
                                        calenderReport.source_data_column,
                                        calenderReport.source_data_column2,
                                        calenderReport.source_data_entity,
                                        calenderReport.vertical_table_name,
                                        calenderQuery,
                                        dateColumn.COLUMN_NAME,
                                      formDate,
                                        toDate,
                                        user != null && !string.IsNullOrEmpty(user.company_code) ? user.company_code : "01");

                if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Bs)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = "to_char(np.NepaliMonthInt) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE," +
                                        " np.NepaliMonthInt, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE," +
                                        " np.weeks, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var monthwiseQuery = "to_char(np.AdDate) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE," +
                                        " np.AdDate, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                }
                // Code for Ad Calendar
                else if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Ad)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = " np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE," +
                                    " np.month, np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var weekwiseQuery = "to_char(np.weeks) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE," +
                                    " np.weeks, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", weekwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var daywiseQuery = "to_char(np.AdDate) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE," +
                                        " np.AdDate, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", daywiseQuery);
                        query = query + groupwise;
                    }
                }
            }
            else if (calenderReport.vertical_table_name.Trim() == "IP_ITEM_MASTER_SETUP")
            {
                query = string.Format(Constants.ItemWise_Calendar_Query,
                                        calenderReport.source_data_column,
                                        calenderReport.source_data_column2,
                                        calenderReport.source_data_entity,
                                        calenderReport.vertical_table_name,
                                        calenderQuery,
                                        dateColumn.COLUMN_NAME,
                                       formDate,
                                       toDate,
                                        user != null && !string.IsNullOrEmpty(user.company_code) ? user.company_code : "01");

                if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Bs)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                         " np.weeks, np.Nepalimonth ,np.Nepaliyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                        " np.weeks, np.Nepalimonth ,np.Nepaliyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var monthwiseQuery = "to_char(np.AdDate) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                        " np.AdDate, np.Nepalimonth ,np.Nepaliyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                }
                // Code for Ad Calendar
                else if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Ad)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = " np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                    " np.month, np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var weekwiseQuery = "to_char(np.weeks) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                    " np.weeks, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", weekwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var daywiseQuery = "to_char(np.AdDate) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                        " np.AdDate, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", daywiseQuery);
                        query = query + groupwise;
                    }
                }
            }
            return query;
        }


        private string GetQueryIfStartsWith_V(CalendarFilterModel filter, CalenderReportSetup calenderReport, string calenderQuery, DatabaseColumn dateColumn)
        {

            var user = this._workContext.CurrentUserinformation;
            string query = string.Empty;
            var formDate = Convert.ToDateTime(filter.formDate).ToString("dd-MMM-yyyy");
            var toDate = Convert.ToDateTime(filter.toDate).ToString("dd-MMM-yyyy");
            if (calenderReport.vertical_table_name.Trim() == "SA_CUSTOMER_SETUP")
            {
                query = string.Format(Constants.CustomerWise_Calendar_Query,
                                        calenderReport.source_data_column,
                                        calenderReport.source_data_column2,
                                        calenderReport.source_data_entity,
                                        calenderReport.vertical_table_name,
                                        calenderQuery,
                                        dateColumn.COLUMN_NAME,
                                       formDate,
                                        toDate,
                                        user != null && !string.IsNullOrEmpty(user.company_code) ? user.company_code : "01");

                if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Bs)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = "to_char(np.NepaliMonthInt) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE," +
                                        " np.NepaliMonthInt, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE," +
                                        " np.weeks, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var monthwiseQuery = "to_char(np.AdDate) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE," +
                                        " np.AdDate, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                }
                // Code for Ad Calendar
                else if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Ad)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = " np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE," +
                                    " np.month, np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var weekwiseQuery = "to_char(np.weeks) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE," +
                                    " np.weeks, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", weekwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var daywiseQuery = "to_char(np.AdDate) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.CUSTOMER_EDESC, fa.CUSTOMER_CODE,fa.MASTER_CUSTOMER_CODE,FA.PRE_CUSTOMER_CODE," +
                                        " np.AdDate, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", daywiseQuery);
                        query = query + groupwise;
                    }
                }
            }
            else if (calenderReport.vertical_table_name.Trim() == "IP_ITEM_MASTER_SETUP")
            {
                query = string.Format(Constants.ItemWise_Calendar_Query,
                                        calenderReport.source_data_column,
                                        calenderReport.source_data_column2,
                                        calenderReport.source_data_entity,
                                        calenderReport.vertical_table_name,
                                        calenderQuery,
                                        dateColumn.COLUMN_NAME,
                                       formDate,
                                       toDate,
                                        user != null && !string.IsNullOrEmpty(user.company_code) ? user.company_code : "01");

                if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Bs)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = "to_char(np.NepaliMonthInt) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE," +
                                        " np.NepaliMonthInt, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE," +
                                        " np.weeks, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var monthwiseQuery = "to_char(np.AdDate) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE," +
                                        " np.AdDate, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                }
                // Code for Ad Calendar
                else if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Ad)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = " np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                    " np.month, np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var weekwiseQuery = "to_char(np.weeks) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                    " np.weeks, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", weekwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var daywiseQuery = "to_char(np.AdDate) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ITEM_EDESC, fa.ITEM_CODE,fa.MASTER_ITEM_CODE,FA.PRE_ITEM_CODE," +
                                        " np.AdDate, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", daywiseQuery);
                        query = query + groupwise;
                    }
                }
            }
            else if (calenderReport.vertical_table_name.Trim() == "IP_SUPPLIER_SETUP")
            {
                query = string.Format(Constants.SupplierWise_Calendar_Query,
                                        calenderReport.source_data_column,
                                        calenderReport.source_data_column2,
                                        calenderReport.source_data_entity,
                                        calenderReport.vertical_table_name,
                                        calenderQuery,
                                        dateColumn.COLUMN_NAME,
                                      formDate,
                                       toDate,
                                        user != null && !string.IsNullOrEmpty(user.company_code) ? user.company_code : "01");

                if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Bs)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = "to_char(np.NepaliMonthInt) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE," +
                                        " np.NepaliMonthInt, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE," +
                                        " np.weeks, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var monthwiseQuery = "to_char(np.AdDate) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE," +
                                        " np.AdDate, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                }
                // Code for Ad Calendar
                else if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Ad)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = " np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE," +
                                    " np.month, np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var weekwiseQuery = "to_char(np.weeks) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE," +
                                    " np.weeks, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", weekwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var daywiseQuery = "to_char(np.AdDate) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.SUPPLIER_EDESC, fa.SUPPLIER_CODE,fa.MASTER_SUPPLIER_CODE,FA.PRE_SUPPLIER_CODE," +
                                        " np.AdDate, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", daywiseQuery);
                        query = query + groupwise;
                    }
                }
            }
            else if (calenderReport.vertical_table_name.Trim() == "FA_CHART_OF_ACCOUNTS_SETUP")
            {
                
                query = string.Format(Constants.Account_Wise_CalendarQuery,
                                        calenderReport.source_data_column,
                                        calenderReport.source_data_column2,
                                        calenderReport.source_data_entity,
                                        calenderReport.vertical_table_name,
                                        calenderQuery,
                                        dateColumn.COLUMN_NAME,
                                       formDate,
                                       toDate,
                                        user != null && !string.IsNullOrEmpty(user.company_code) ? user.company_code : "01");
                if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Bs)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = "to_char(np.NepaliMonthInt) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ACC_EDESC, fa.ACC_CODE," +
                                        " np.NepaliMonthInt, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ACC_CODE,FA.PRE_ACC_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var monthwiseQuery = "to_char(np.weeks) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ACC_EDESC, fa.ACC_CODE," +
                                        " np.weeks, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ACC_CODE,FA.PRE_ACC_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var monthwiseQuery = "to_char(np.AdDate) as week, np.Nepalimonth AS month,np.Nepaliyear AS year";
                        var groupwise = " GROUP BY fa.ACC_EDESC, fa.ACC_CODE," +
                                        " np.AdDate, np.Nepalimonth ,np.Nepaliyear,fa.MASTER_ACC_CODE,FA.PRE_ACC_CODE";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                }
                // Code for Ad Calendar
                else if (Convert.ToChar(filter.CalenderTypeValue) == (Char)CalenderType.Ad)
                {
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        var monthwiseQuery = " np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ACC_EDESC, fa.ACC_CODE,fa.MASTER_ACC_CODE,FA.PRE_ACC_CODE," +
                                    " np.month, np.Englishyear";
                        query = query.Replace("#calenderWise", monthwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        var weekwiseQuery = "to_char(np.weeks) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ACC_EDESC, fa.ACC_CODE,fa.MASTER_ACC_CODE,FA.PRE_ACC_CODE," +
                                    " np.weeks, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", weekwiseQuery);
                        query = query + groupwise;
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        var daywiseQuery = "to_char(np.AdDate) as week, np.month AS month, np.Englishyear AS year";
                        var groupwise = @" GROUP BY fa.ACC_EDESC, fa.ACC_CODE,fa.MASTER_ACC_CODE,FA.PRE_ACC_CODE," +
                                        " np.AdDate, np.month ,np.Englishyear";
                        query = query.Replace("#calenderWise", daywiseQuery);
                        query = query + groupwise;
                    }
                }
            }
            return query;
        }



        private static object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        /// <summary>
        /// Renders the Model Data required for the particular report selected to be used by the Dynamic-Column-View-Partial Page
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Data Model for the customers listed</returns>
        public List<CalendarCustomerDataModel> GetCalenderReport(CalendarFilterModel filter)
        {
            var abcParent = new List<CalendarCustomerDataModel>();
            try
            {
                var CalendarData = new List<CalendarCustomerDataModel>();
                string query = string.Empty;
                int ReportId = 0;
                if (filter.ReportId != null)
                    ReportId = Convert.ToInt16(filter.ReportId);
                var calenderReport = GetCalenderSetup(ReportId).FirstOrDefault();
                if (calenderReport == null)
                    return CalendarData;
                var dateColumn = this.GetColumnNameFromDb(calenderReport.source_data_entity).FirstOrDefault();
                if (dateColumn == null)
                    return CalendarData;
                string calenderQuery = this._constants.GetAdVersusBsCalendarQuery;

                if (calenderReport.source_data_entity.StartsWith("SA"))
                {
                    query = this.GetQueryIfStartsWith_SA(filter, calenderReport, calenderQuery, dateColumn);
                }

                else if (calenderReport.source_data_entity.StartsWith("IP"))
                {
                    query = this.GetQueryIfStartsWith_IP(filter, calenderReport, calenderQuery, dateColumn);
                }

                else if (calenderReport.source_data_entity.StartsWith("V"))
                {
                    query = this.GetQueryIfStartsWith_V(filter, calenderReport, calenderQuery, dateColumn);
                }

                var calenderdata = _objectEntity.SqlQuery<CalendarDataViewModel>(query).ToList();
              
                IAgeingReportDataService dataService = this._ageingFactory.GetAgeingDataService(AgeingReportType.Customer);
                if (filter.ShowGroup)
                {
                    var columnName = Enum.GetName(typeof(PeriodFilter),Convert.ToChar(filter.SecondHorizontalPeriod)).ToLowerInvariant();
                    //(x => GetPropertyValue(x, columnName));
                    var CustomerGroupData = new List<AgeingGroupData>();
                    if (calenderReport.vertical_table_name.Trim() == "SA_CUSTOMER_SETUP")
                    {
                        CustomerGroupData = dataService.GetGroupData().ToList();

                    }
                    else if (calenderReport.vertical_table_name.Trim() == "IP_ITEM_MASTER_SETUP")
                    {
                        IAgeingReportDataService dataServiceProduct = this._ageingFactory.GetAgeingDataService(AgeingReportType.Product);
                        CustomerGroupData = dataServiceProduct.GetGroupData().ToList();
                    }
                    else if (calenderReport.vertical_table_name.Trim() == "IP_SUPPLIER_SETUP")
                    {
                        IAgeingReportDataService dataServiceProduct = this._ageingFactory.GetAgeingDataService(AgeingReportType.Product);
                        CustomerGroupData = dataServiceProduct.GetGroupData().ToList();
                    }
                    else if (calenderReport.vertical_table_name.Trim() == "FA_CHART_OF_ACCOUNTS_SETUP")
                    {
                        // IAgeingReportDataService dataServiceProduct = this._ageingFactory.GetAgeingDataService(AgeingReportType.Product);
                        CustomerGroupData = _voucherService.GetAccountData().ToList();
                    }
                    foreach (var customergroup in CustomerGroupData.Where(x => x.PreCodeWithoutReplace == "00"))
                    {
                        var abc = new CalendarCustomerDataModel();
                        abc.id = customergroup.MasterCode;
                        // abc.parentId = 0;
                        abc.PreCodeWithoutConversion = customergroup.MasterCodeWithoutReplace;
                        abc.CustomerCode = customergroup.Code.ToString();
                        abc.CustomerName = customergroup.Description;
                        abc.hasChildren = true;
                        var allIndivsual = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).ToList();
                        if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                        {
                            foreach (var indv in allIndivsual)
                            {
                                var cust = new CustomerWeekWise();
                                cust.MonthName = indv.month;
                                cust.yearname = indv.year;
                                cust.weekName = indv.week;
                              
                                    cust.totalQty = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                                    cust.TotalAmount = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                               
                                abc.additionalData.Add(cust);
                                abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                                abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                            }
                        }
                        else
                        {
                            foreach (var indv in allIndivsual.GroupBy(x => GetPropertyValue(x, columnName)).Select(group => group.First()))
                            {
                                var cust = new CustomerWeekWise();
                                cust.MonthName = indv.month;
                                cust.yearname = indv.year;
                                cust.weekName = indv.week;
                                if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                                {
                                    cust.totalQty = calenderdata.Where(x => x.week == indv.week).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                                    cust.TotalAmount = calenderdata.Where(x => x.week == indv.week).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                                }
                                else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                                {
                                    cust.totalQty = calenderdata.Where(x => x.month == indv.month).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                                    cust.TotalAmount = calenderdata.Where(x => x.month == indv.month).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                                }
                                else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                                {
                                    cust.totalQty = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                                    cust.TotalAmount = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                                }

                                abc.additionalData.Add(cust);
                                abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                                abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                            }
                        }
                            //if(filter.SecondHorizontalPeriod==ho)
                          
                        abcParent.Add(abc);


                    }

                    foreach (var childItem in abcParent.ToList())
                    {
                        var abcParenttest = new List<CalendarCustomerDataModel>();
                        receriveData(CustomerGroupData, calenderdata, childItem.PreCodeWithoutConversion, abcParenttest,filter);
                        abcParent.AddRange(abcParenttest);

                    }

                    return abcParent;

                }

                foreach (var calender in calenderdata.GroupBy(customer => customer.Id).Select(group => group.First()))
                {

                    var abc = new CalendarCustomerDataModel();
                    abc.id = Convert.ToInt64(calender.Id);
                    // abc.parentId = 0;
                    abc.CustomerCode = calender.Id.ToString();
                    abc.CustomerName = calender.Description;
                    var allIndivsual = calenderdata.Where(x => x.Id == calender.Id).ToList();

                    foreach (var indv in allIndivsual)
                    {
                        var cust = new CustomerWeekWise();
                        cust.MonthName = indv.month;
                        cust.yearname = indv.year;
                        cust.weekName = indv.week;
                        cust.totalQty = indv.Quantity;
                        cust.TotalAmount = indv.NetAmount;
                        abc.additionalData.Add(cust);

                        abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                        abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                    }
                    //abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                    //abc.Price = abc.additionalData.Sum(x => x.totalQty);
                    abcParent.Add(abc);

                }
                return abcParent;
            }
            catch(Exception ex)
            {
                return abcParent;
            }
        }
        public List<CalendarCustomerDataModel> GetCalenderReport(CalendarFilterModel filter,User userinfo)
        {
            var abcParent = new List<CalendarCustomerDataModel>();
            try
            {
                var CalendarData = new List<CalendarCustomerDataModel>();
                string query = string.Empty;
                int ReportId = 0;
                if (filter.ReportId != null)
                    ReportId = Convert.ToInt16(filter.ReportId);
                var calenderReport = GetCalenderSetup(ReportId).FirstOrDefault();
                if (calenderReport == null)
                    return CalendarData;
                var dateColumn = this.GetColumnNameFromDb(calenderReport.source_data_entity).FirstOrDefault();
                if (dateColumn == null)
                    return CalendarData;
                string calenderQuery = this._constants.GetAdVersusBsCalendarQuery;

                if (calenderReport.source_data_entity.StartsWith("SA"))
                {
                    query = this.GetQueryIfStartsWith_SA(filter, calenderReport, calenderQuery, dateColumn);
                }

                else if (calenderReport.source_data_entity.StartsWith("IP"))
                {
                    query = this.GetQueryIfStartsWith_IP(filter, calenderReport, calenderQuery, dateColumn);
                }

                else if (calenderReport.source_data_entity.StartsWith("V"))
                {
                    query = this.GetQueryIfStartsWith_V(filter, calenderReport, calenderQuery, dateColumn);
                }

                var calenderdata = _objectEntity.SqlQuery<CalendarDataViewModel>(query).ToList();

                IAgeingReportDataService dataService = this._ageingFactory.GetAgeingDataService(AgeingReportType.Customer);
                if (filter.ShowGroup)
                {
                    var columnName = Enum.GetName(typeof(PeriodFilter), Convert.ToChar(filter.SecondHorizontalPeriod)).ToLowerInvariant();
                    //(x => GetPropertyValue(x, columnName));
                    var CustomerGroupData = new List<AgeingGroupData>();
                    if (calenderReport.vertical_table_name.Trim() == "SA_CUSTOMER_SETUP")
                    {
                        CustomerGroupData = dataService.GetGroupData().ToList();

                    }
                    else if (calenderReport.vertical_table_name.Trim() == "IP_ITEM_MASTER_SETUP")
                    {
                        IAgeingReportDataService dataServiceProduct = this._ageingFactory.GetAgeingDataService(AgeingReportType.Product);
                        CustomerGroupData = dataServiceProduct.GetGroupData().ToList();
                    }
                    else if (calenderReport.vertical_table_name.Trim() == "IP_SUPPLIER_SETUP")
                    {
                        IAgeingReportDataService dataServiceProduct = this._ageingFactory.GetAgeingDataService(AgeingReportType.Product);
                        CustomerGroupData = dataServiceProduct.GetGroupData().ToList();
                    }
                    else if (calenderReport.vertical_table_name.Trim() == "FA_CHART_OF_ACCOUNTS_SETUP")
                    {
                        // IAgeingReportDataService dataServiceProduct = this._ageingFactory.GetAgeingDataService(AgeingReportType.Product);
                        CustomerGroupData = _voucherService.GetAccountData().ToList();
                    }
                    foreach (var customergroup in CustomerGroupData.Where(x => x.PreCodeWithoutReplace == "00"))
                    {
                        var abc = new CalendarCustomerDataModel();
                        abc.id = customergroup.MasterCode;
                        // abc.parentId = 0;
                        abc.PreCodeWithoutConversion = customergroup.MasterCodeWithoutReplace;
                        abc.CustomerCode = customergroup.Code.ToString();
                        abc.CustomerName = customergroup.Description;
                        abc.hasChildren = true;
                        var allIndivsual = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).ToList();
                        if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                        {
                            foreach (var indv in allIndivsual)
                            {
                                var cust = new CustomerWeekWise();
                                cust.MonthName = indv.month;
                                cust.yearname = indv.year;
                                cust.weekName = indv.week;

                                cust.totalQty = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                                cust.TotalAmount = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);

                                abc.additionalData.Add(cust);
                                abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                                abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                            }
                        }
                        else
                        {
                            foreach (var indv in allIndivsual.GroupBy(x => GetPropertyValue(x, columnName)).Select(group => group.First()))
                            {
                                var cust = new CustomerWeekWise();
                                cust.MonthName = indv.month;
                                cust.yearname = indv.year;
                                cust.weekName = indv.week;
                                if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                                {
                                    cust.totalQty = calenderdata.Where(x => x.week == indv.week).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                                    cust.TotalAmount = calenderdata.Where(x => x.week == indv.week).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                                }
                                else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                                {
                                    cust.totalQty = calenderdata.Where(x => x.month == indv.month).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                                    cust.TotalAmount = calenderdata.Where(x => x.month == indv.month).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                                }
                                else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                                {
                                    cust.totalQty = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                                    cust.TotalAmount = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                                }

                                abc.additionalData.Add(cust);
                                abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                                abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                            }
                        }
                        //if(filter.SecondHorizontalPeriod==ho)

                        abcParent.Add(abc);


                    }

                    foreach (var childItem in abcParent.ToList())
                    {
                        var abcParenttest = new List<CalendarCustomerDataModel>();
                        receriveData(CustomerGroupData, calenderdata, childItem.PreCodeWithoutConversion, abcParenttest, filter);
                        abcParent.AddRange(abcParenttest);

                    }

                    return abcParent;

                }

                foreach (var calender in calenderdata.GroupBy(customer => customer.Id).Select(group => group.First()))
                {

                    var abc = new CalendarCustomerDataModel();
                    abc.id = Convert.ToInt64(calender.Id);
                    // abc.parentId = 0;
                    abc.CustomerCode = calender.Id.ToString();
                    abc.CustomerName = calender.Description;
                    var allIndivsual = calenderdata.Where(x => x.Id == calender.Id).ToList();

                    foreach (var indv in allIndivsual)
                    {
                        var cust = new CustomerWeekWise();
                        cust.MonthName = indv.month;
                        cust.yearname = indv.year;
                        cust.weekName = indv.week;
                        cust.totalQty = indv.Quantity;
                        cust.TotalAmount = indv.NetAmount;
                        abc.additionalData.Add(cust);

                        abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                        abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                    }
                    //abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                    //abc.Price = abc.additionalData.Sum(x => x.totalQty);
                    abcParent.Add(abc);

                }
                return abcParent;
            }
            catch (Exception ex)
            {
                return abcParent;
            }
        }
        public void receriveData(IEnumerable<AgeingGroupData> CustomerGroupData,List<CalendarDataViewModel> calenderdata,string PrecodeWithoutconversion, List<CalendarCustomerDataModel> abcParenttest,CalendarFilterModel filter)
        {
            var columnName = Enum.GetName(typeof(PeriodFilter), Convert.ToChar(filter.SecondHorizontalPeriod)).ToLowerInvariant();
            if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
            {
                foreach (var customergroup in CustomerGroupData.Where(x => x.PreCodeWithoutReplace == PrecodeWithoutconversion))
                {
                    var abc = new CalendarCustomerDataModel();
                    abc.id = customergroup.MasterCode;
                    abc.parentId = customergroup.PreCode;
                    abc.CustomerCode = customergroup.Code.ToString();
                    abc.CustomerName = customergroup.Description;
                    abc.PreCodeWithoutConversion = customergroup.MasterCodeWithoutReplace;
                    abc.hasChildren = true;
                    var allIndivsual = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).ToList();

                    foreach (var indv in allIndivsual)
                    {
                        var cust = new CustomerWeekWise();
                        cust.MonthName = indv.month;
                        cust.yearname = indv.year;
                        cust.weekName = indv.week;
                    
                            cust.totalQty = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                            cust.TotalAmount = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                       // }

                        abc.additionalData.Add(cust);

                        abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                        abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                    }
                    //abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                    //abc.Price = abc.additionalData.Sum(x => x.totalQty);
                    abcParenttest.Add(abc);
                    receriveData(CustomerGroupData, calenderdata, abc.PreCodeWithoutConversion, abcParenttest, filter);
                }
            }
            else
            {
                foreach (var customergroup in CustomerGroupData.Where(x => x.PreCodeWithoutReplace == PrecodeWithoutconversion))
                {
                    var abc = new CalendarCustomerDataModel();
                    abc.id = customergroup.MasterCode;
                    abc.parentId = customergroup.PreCode;
                    abc.CustomerCode = customergroup.Code.ToString();
                    abc.CustomerName = customergroup.Description;
                    abc.PreCodeWithoutConversion = customergroup.MasterCodeWithoutReplace;
                    abc.hasChildren = true;
                    var allIndivsual = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).ToList();

                    foreach (var indv in allIndivsual.GroupBy(x => GetPropertyValue(x, columnName)).Select(group => group.First()))
                    {
                        var cust = new CustomerWeekWise();
                        cust.MonthName = indv.month;
                        cust.yearname = indv.year;
                        cust.weekName = indv.week;
                        if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                        {
                            cust.totalQty = calenderdata.Where(x => x.week == indv.week).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                            cust.TotalAmount = calenderdata.Where(x => x.week == indv.week).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                        }
                        else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                        {
                            cust.totalQty = calenderdata.Where(x => x.month == indv.month).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                            cust.TotalAmount = calenderdata.Where(x => x.month == indv.month).Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                        }
                        else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                        {
                            cust.totalQty = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.Quantity);
                            cust.TotalAmount = calenderdata.Where(x => x.MasterCode.StartsWith(customergroup.MasterCodeWithoutReplace)).Sum(x => x.NetAmount);
                        }

                        abc.additionalData.Add(cust);

                        abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                        abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                    }
                    //abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                    //abc.Price = abc.additionalData.Sum(x => x.totalQty);
                    abcParenttest.Add(abc);
                    receriveData(CustomerGroupData, calenderdata, abc.PreCodeWithoutConversion, abcParenttest, filter);
                }
            }
                //(x => GetPropertyValue(x, columnName));
            
            foreach (var calender in calenderdata.Where(x=>x.PreMasterCode== PrecodeWithoutconversion).GroupBy(customer => customer.Id).Select(group => group.First()))
            {

                var abc = new CalendarCustomerDataModel();
                abc.id = Convert.ToInt64(calender.Id);
                abc.parentId = Convert.ToInt64(calender.PreMasterCode.Replace(".", "").Trim());
                abc.CustomerCode = calender.Id.ToString();
                abc.CustomerName = calender.Description;
                var allIndivsual = calenderdata.Where(x => x.Id == calender.Id).ToList();
                abc.hasChildren = false;
                foreach (var indv in allIndivsual.GroupBy(x=>GetPropertyValue(x, columnName)).Select(group => group.First()))
                {
                    var cust = new CustomerWeekWise();
                    cust.MonthName = indv.month;
                    cust.yearname = indv.year;
                    cust.weekName = indv.week;
                    if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Week)
                    {
                        cust.totalQty = calenderdata.Where(x => x.week == indv.week).Sum(x => x.Quantity);
                        cust.TotalAmount = calenderdata.Where(x => x.week == indv.week).Sum(x => x.NetAmount);
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Month)
                    {
                        cust.totalQty = calenderdata.Where(x => x.month == indv.month).Sum(x => x.Quantity);
                        cust.TotalAmount = calenderdata.Where(x => x.month == indv.month).Sum(x => x.NetAmount);
                    }
                    else if (Convert.ToChar(filter.SecondHorizontalPeriod) == (Char)PeriodFilter.Day)
                    {
                        cust.totalQty = indv.Quantity;
                        cust.TotalAmount = indv.NetAmount;
                    }
                    cust.totalQty = allIndivsual.Where(x => x.month == indv.month).Sum(x => x.Quantity);
                    cust.TotalAmount = allIndivsual.Where(x => x.month == indv.month).Sum(x => x.NetAmount);
                    abc.additionalData.Add(cust);

                    abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                    abc.Quantity = abc.additionalData.Sum(x => x.totalQty);
                }
                //abc.Total = abc.additionalData.Sum(x => x.TotalAmount);
                //abc.Price = abc.additionalData.Sum(x => x.totalQty);
                abcParenttest.Add(abc);

            }

        }
        public List<CalenderReportSetup> GetCalenderSetup(int reportid = 0)
        {
            string query = string.Empty;
            if (reportid > 0)
            {
                query = this._constants.GetCalendarReportDetailsQuery + " AND report_code =" + reportid;
            }
            else
            {
                query = this._constants.GetCalendarReportDetailsQuery;
            }
            return _objectEntity.SqlQuery<CalenderReportSetup>(query).ToList();
        }

        public List<DatabaseColumn> GetColumnNameFromDb(string tableName)
        {
            string query = @"SELECT COLUMN_NAME FROM SYS.ALL_TAB_COLUMNS WHERE TABLE_NAME='" + tableName + "'  AND COLUMN_NAME LIKE '%_DATE' " +
                "AND NOT COLUMN_NAME = 'CREATED_DATE'AND OWNER IN(SELECT USER FROM DUAL) ORDER BY COLUMN_ID";
            return _objectEntity.SqlQuery<DatabaseColumn>(query).ToList();
        }


        //Private function to Get Monthly Calendar Column Range for Year & Month chosen
        private CalendarColumnRange GetMonthlyCalendarColumnRange(CalendarFilterModel calenderFilter, CalenderReportSetup columnNames)
        {
            var user = this._workContext.CurrentUserinformation;
            var calendarColumnMonth = new CalendarColumnRange();
            if (Convert.ToChar(calenderFilter.CalenderTypeValue) == (Char)CalenderType.Bs)
            {
                string Query = this._constants.CalendarTypeADVsBSWeeksQuery;

                if (!_cacheManager.IsSet("NepaliYearMonthCalendar"))
                {
                    _cacheManager.Set("NepaliYearMonthCalendar", _objectEntity.SqlQuery<CalenderDB>(Query).ToList(), 60);

                }
                var NepaliCalender = _cacheManager.Get<List<CalenderDB>>("NepaliYearMonthCalendar");
                var formDate = Convert.ToDateTime(calenderFilter.formDate);
                var toDate = Convert.ToDateTime(calenderFilter.toDate);
                NepaliCalender = NepaliCalender.Where(x => x.AdDate >= formDate && x.AdDate <= toDate).ToList();
                foreach (var year in NepaliCalender.GroupBy(customer => customer.Nepaliyear).Select(group => group.First()))
                {
                    var secondHorizontal = new SecondHOrizontalPeriodModel();
                    secondHorizontal.HeaderName = year.Nepaliyear;
                    var yearWise = NepaliCalender.Where(x => x.Nepaliyear == year.Nepaliyear);
                    foreach (var month in yearWise.GroupBy(customer => customer.Nepalimonth).Select(group => group.First()))
                    {
                        var thirdHorizontal = new ThirdHorizontalColumnModel();
                        thirdHorizontal.HeaderName = month.Nepalimonth;
                        if (!string.IsNullOrEmpty(columnNames.column1_heading))
                        {
                            var forthcol = new ForthHorizontalColumnModel();
                            forthcol.HeaderName = columnNames.column1_heading;
                            forthcol.HeaderVlaue = month.Nepalimonth.Trim() + year.Nepaliyear.Trim();
                            thirdHorizontal.forthHorizontalColumn.Add(forthcol);
                        }
                        if (!string.IsNullOrEmpty(columnNames.column1_heading))
                        {
                            var forthcol1 = new ForthHorizontalColumnModel();
                            forthcol1.HeaderName = columnNames.column2_heading;
                            forthcol1.HeaderVlaue = month.Nepalimonth.Trim() + year.Nepaliyear.Trim();
                            thirdHorizontal.forthHorizontalColumn.Add(forthcol1);
                        }
                        secondHorizontal.ThirdHorizontalColumn.Add(thirdHorizontal);
                    }
                    calendarColumnMonth.SecondColumnlist.Add(secondHorizontal);

                }

            }
            else if (Convert.ToChar(calenderFilter.CalenderTypeValue) == (Char)CalenderType.Ad)
            {
                string Query = string.Format(Constants.GetMonthNumberQuery,user!=null && user.startFiscalYear!=null?user.startFiscalYear.ToString("dd-MMM-yyyy"):DateTime.Now.ToString("dd-MMM-yyyy"),
                    user != null && user.endfiscalyear !=null?user.endfiscalyear.ToString("dd-MMM-yyyy"):DateTime.Now.ToString(""));

                if (!_cacheManager.IsSet("EnglishYearMonthCalendar"))
                {
                    _cacheManager.Set("EnglishYearMonthCalendar", _objectEntity.SqlQuery<MonthNumberByYear>(Query).ToList(), 60);

                }
                var AllMonths = _cacheManager.Get<List<MonthNumberByYear>>("EnglishYearMonthCalendar");
                //var formDate = Convert.ToDateTime(calenderFilter.formDate);
                //var toDate = Convert.ToDateTime(calenderFilter.toDate);
                //AllMonths = AllMonths.Where(x => x. >= formDate && x.AdDate <= toDate).ToList();
                foreach (var year in AllMonths.GroupBy(customer => customer.Year).Select(group => group.First()))
                {
                    var secondHorizontal = new SecondHOrizontalPeriodModel();
                    secondHorizontal.HeaderName = year.Year;
                    foreach (var month in AllMonths.GroupBy(customer => customer.MonthName).Select(group => group.First()))
                    {
                        var thirdHorizontal = new ThirdHorizontalColumnModel();
                        thirdHorizontal.HeaderName = month.MonthName;
                        if (!string.IsNullOrEmpty(columnNames.column1_heading))
                        {
                            var forthcol = new ForthHorizontalColumnModel();
                            forthcol.HeaderName = columnNames.column1_heading;
                            forthcol.HeaderVlaue = month.MonthName.Trim() + year.Year.Trim();
                            thirdHorizontal.forthHorizontalColumn.Add(forthcol);
                        }
                        if (!string.IsNullOrEmpty(columnNames.column2_heading))
                        {
                            var forthcol1 = new ForthHorizontalColumnModel();
                            forthcol1.HeaderName = columnNames.column2_heading;
                            forthcol1.HeaderVlaue = month.MonthName.Trim() + year.Year.Trim();
                            thirdHorizontal.forthHorizontalColumn.Add(forthcol1);
                        }
                        secondHorizontal.ThirdHorizontalColumn.Add(thirdHorizontal);
                    }
                    calendarColumnMonth.SecondColumnlist.Add(secondHorizontal);
                }

            }

            return calendarColumnMonth;
        }


        // Private function to get Weekly Calendar Column Range generation when Year and Week chosen
        private CalendarColumnRange GetWeeklyCalendarColumnRange(CalendarFilterModel calenderFilter, CalenderReportSetup columnNames)
        {
            var calendarColumnRange = new CalendarColumnRange();
            if (Convert.ToChar(calenderFilter.CalenderTypeValue) == (Char)CalenderType.Bs)
            {
                //string Query = string.Format(Constants.CalendarTypeADVsBSWeeksQuery, "17-Jul-2016", "15-Jul-2017");
                string Query = this._constants.CalendarTypeADVsBSWeeksQuery;

                if (!_cacheManager.IsSet("NepaliYearWeekCalender"))
                {
                    _cacheManager.Set("NepaliYearWeekCalender", _objectEntity.SqlQuery<CalenderDB>(Query).ToList(), 60);

                }
                var NepaliCalender = _cacheManager.Get<List<CalenderDB>>("NepaliYearWeekCalender");
                var formDate = Convert.ToDateTime(calenderFilter.formDate);
                var toDate = Convert.ToDateTime(calenderFilter.toDate);
                NepaliCalender = NepaliCalender.Where(x => x.AdDate >= formDate && x.AdDate <= toDate).ToList();
                foreach (var year in NepaliCalender.GroupBy(customer => customer.Nepaliyear).Select(group => group.First()))
                {
                    var Horizontal = new FirstHorizontalPeriodModel();
                    Horizontal.HeaderName = year.Nepaliyear;
                    var yearWise = NepaliCalender.Where(x => x.Nepaliyear == year.Nepaliyear);
                    foreach (var month in yearWise.GroupBy(customer => customer.Nepalimonth).Select(group => group.First()))
                    {
                        var secondHorizantal = new SecondHOrizontalPeriodModel();
                        secondHorizantal.HeaderName = month.Nepalimonth;
                        var monthWise = NepaliCalender.Where(x => x.Nepalimonth == month.Nepalimonth && x.Nepaliyear == year.Nepaliyear);
                        foreach (var week in monthWise.GroupBy(customer => customer.weeks).Select(group => group.First()))
                        {
                            var thirdHorizontal = new ThirdHorizontalColumnModel();
                            thirdHorizontal.HeaderName = "Week " + week.weeks;
                            if (!string.IsNullOrEmpty(columnNames.column1_heading))
                            {
                                var forthcol = new ForthHorizontalColumnModel();
                                forthcol.HeaderName = columnNames.column1_heading;
                                forthcol.HeaderVlaue = week.weeks + month.Nepalimonth.Trim() + year.Nepaliyear.Trim();
                                thirdHorizontal.forthHorizontalColumn.Add(forthcol);
                            }
                            if (!string.IsNullOrEmpty(columnNames.column1_heading))
                            {
                                var forthcol1 = new ForthHorizontalColumnModel();
                                forthcol1.HeaderName = columnNames.column2_heading;
                                forthcol1.HeaderVlaue = week.weeks + month.Nepalimonth.Trim() + year.Nepaliyear.Trim();
                                thirdHorizontal.forthHorizontalColumn.Add(forthcol1);
                            }


                            secondHorizantal.ThirdHorizontalColumn.Add(thirdHorizontal);
                        }
                        Horizontal.secondHorizontalPeriodModel.Add(secondHorizantal);

                    }
                    calendarColumnRange.FirstColumnlist.Add(Horizontal);
                }

            }
            else if (Convert.ToChar(calenderFilter.CalenderTypeValue) == (Char)CalenderType.Ad)
            {
                string Query = this._constants.CalendarTypeADVsBSWeeksQuery;

                if (!_cacheManager.IsSet("EnglishYearWeekCalendar"))
                {
                    _cacheManager.Set("EnglishYearWeekCalendar", _objectEntity.SqlQuery<CalenderDB>(Query).ToList(), 60);

                }

                var EnglishCalendar = _cacheManager.Get<List<CalenderDB>>("EnglishYearWeekCalendar");
                var formDate = Convert.ToDateTime(calenderFilter.formDate);
                var toDate = Convert.ToDateTime(calenderFilter.toDate);
                EnglishCalendar = EnglishCalendar.Where(x => x.AdDate >= formDate && x.AdDate <= toDate).ToList();
                foreach (var year in EnglishCalendar.GroupBy(customer => customer.Englishyear).Select(group => group.First()))
                {
                    var Horizontal = new FirstHorizontalPeriodModel();
                    Horizontal.HeaderName = year.Englishyear;
                    var yearWise = EnglishCalendar.Where(x => x.Englishyear == year.Englishyear);
                    foreach (var month in yearWise.GroupBy(customer => customer.month).Select(group => group.First()))
                    {
                        var secondHorizantal = new SecondHOrizontalPeriodModel();
                        secondHorizantal.HeaderName = month.month;
                        var monthWise = EnglishCalendar.Where(x => x.month == month.month && x.Englishyear == year.Englishyear);
                        foreach (var week in monthWise.GroupBy(customer => customer.weeks).Select(group => group.First()))
                        //foreach (var week in monthWise)
                        {
                            var thirdHorizontal = new ThirdHorizontalColumnModel();
                            thirdHorizontal.HeaderName = "Week " + week.weeks;
                            if (!string.IsNullOrEmpty(columnNames.column1_heading))
                            {
                                var forthcol = new ForthHorizontalColumnModel();
                                forthcol.HeaderName = columnNames.column1_heading;
                                forthcol.HeaderVlaue = week.weeks + month.month.Trim() + year.Englishyear.Trim();
                                thirdHorizontal.forthHorizontalColumn.Add(forthcol);
                            }
                            if (!string.IsNullOrEmpty(columnNames.column2_heading))
                            {
                                var forthcol1 = new ForthHorizontalColumnModel();
                                forthcol1.HeaderName = columnNames.column2_heading;
                                forthcol1.HeaderVlaue = week.weeks + month.month.Trim() + year.Englishyear.Trim();
                                thirdHorizontal.forthHorizontalColumn.Add(forthcol1);
                            }

                            secondHorizantal.ThirdHorizontalColumn.Add(thirdHorizontal);
                        }
                        Horizontal.secondHorizontalPeriodModel.Add(secondHorizantal);

                    }
                    calendarColumnRange.FirstColumnlist.Add(Horizontal);
                }

            }
            return calendarColumnRange;
        }



        //Private funciton get Day wise Calendar Column Range generation when Year and Day chosen
        private CalendarColumnRange GetDailyCalendarColumnRange(CalendarFilterModel calenderFilter, CalenderReportSetup columnNames)
        {
            var calendarColumnRange = new CalendarColumnRange();
            if (Convert.ToChar(calenderFilter.CalenderTypeValue) == (Char)CalenderType.Bs)
            {
                string Query = this._constants.CalendarTypeADVsBSWeeksQuery;

                if (!_cacheManager.IsSet("NepaliYearDaysCalender"))
                {
                    _cacheManager.Set("NepaliYearDaysCalender", _objectEntity.SqlQuery<CalenderDB>(Query).ToList(), 60);

                }
                var NepaliCalender = _cacheManager.Get<List<CalenderDB>>("NepaliYearDaysCalender");
                var formDate = Convert.ToDateTime(calenderFilter.formDate);
                var toDate = Convert.ToDateTime(calenderFilter.toDate);
                NepaliCalender = NepaliCalender.Where(x => x.AdDate >= formDate && x.AdDate <= toDate).ToList();
                foreach (var year in NepaliCalender.GroupBy(customer => customer.Nepaliyear).Select(group => group.First()))
                {
                    var Horizontal = new FirstHorizontalPeriodModel();
                    Horizontal.HeaderName = year.Nepaliyear;
                    var yearWise = NepaliCalender.Where(x => x.Nepaliyear == year.Nepaliyear);
                    foreach (var month in yearWise.GroupBy(customer => customer.Nepalimonth).Select(group => group.First()))
                    {
                        var secondHorizantal = new SecondHOrizontalPeriodModel();
                        secondHorizantal.HeaderName = month.Nepalimonth;
                        var monthWise = NepaliCalender.Where(x => x.Nepalimonth == month.Nepalimonth && x.Nepaliyear == year.Nepaliyear);
                        foreach (var day in monthWise)
                        {
                            var thirdHorizontal = new ThirdHorizontalColumnModel();
                            thirdHorizontal.HeaderName = day.AdDate.ToString("ddd") + "-" + day.Miti.Substring(8);
                            if (!string.IsNullOrEmpty(columnNames.column1_heading))
                            {
                                var forthcol = new ForthHorizontalColumnModel();
                                forthcol.HeaderName = columnNames.column1_heading;
                                forthcol.HeaderVlaue = day.AdDate.ToString("dd-MMM-yy").ToUpper() + month.Nepalimonth.Trim() + year.Nepaliyear.Trim();
                                thirdHorizontal.forthHorizontalColumn.Add(forthcol);
                            }
                            if (!string.IsNullOrEmpty(columnNames.column2_heading))
                            {
                                var forthcol1 = new ForthHorizontalColumnModel();
                                forthcol1.HeaderName = columnNames.column2_heading;
                                forthcol1.HeaderVlaue = day.AdDate.ToString("dd-MMM-yy").ToUpper() + month.Nepalimonth.Trim() + year.Nepaliyear.Trim();
                                thirdHorizontal.forthHorizontalColumn.Add(forthcol1);
                            }
                            secondHorizantal.ThirdHorizontalColumn.Add(thirdHorizontal);
                        }
                        Horizontal.secondHorizontalPeriodModel.Add(secondHorizantal);

                    }
                    calendarColumnRange.FirstColumnlist.Add(Horizontal);
                }
            }
            else if (Convert.ToChar(calenderFilter.CalenderTypeValue) == (Char)CalenderType.Ad)
            {
                string Query = this._constants.CalendarTypeADVsBSWeeksQuery;

                if (!_cacheManager.IsSet("EnglishYearDaysCalendar"))
                {
                    _cacheManager.Set("EnglishYearDaysCalendar", _objectEntity.SqlQuery<CalenderDB>(Query).ToList(), 60);

                }
                var EnglishCalender = _cacheManager.Get<List<CalenderDB>>("EnglishYearDaysCalendar");
                var formDate = Convert.ToDateTime(calenderFilter.formDate);
                var toDate = Convert.ToDateTime(calenderFilter.toDate);
                EnglishCalender = EnglishCalender.Where(x => x.AdDate >= formDate && x.AdDate <= toDate).ToList();
                foreach (var year in EnglishCalender.GroupBy(customer => customer.Englishyear).Select(group => group.First()))
                {
                    var Horizontal = new FirstHorizontalPeriodModel();
                    Horizontal.HeaderName = year.Englishyear;
                    var yearWise = EnglishCalender.Where(x => x.Englishyear == year.Englishyear);
                    foreach (var month in yearWise.GroupBy(customer => customer.month).Select(group => group.First()))
                    {
                        var secondHorizantal = new SecondHOrizontalPeriodModel();
                        secondHorizantal.HeaderName = month.month;
                        var monthWise = EnglishCalender.Where(x => x.month == month.month && x.Englishyear == year.Englishyear);
                        foreach (var day in monthWise)
                        {
                            var thirdHorizontal = new ThirdHorizontalColumnModel();
                            thirdHorizontal.HeaderName = day.AdDate.ToString("ddd") + "-" + day.AdDate.ToString("dd");
                            if (!string.IsNullOrEmpty(columnNames.column1_heading))
                            {
                                var forthcol = new ForthHorizontalColumnModel();
                                forthcol.HeaderName = columnNames.column1_heading;
                                forthcol.HeaderVlaue = day.AdDate.ToString("dd-MMM-yy").ToUpper() + month.month.Trim() + year.Englishyear.Trim();
                                thirdHorizontal.forthHorizontalColumn.Add(forthcol);
                            }
                            if (!string.IsNullOrEmpty(columnNames.column2_heading))
                            {
                                var forthcol1 = new ForthHorizontalColumnModel();
                                forthcol1.HeaderName = columnNames.column2_heading;
                                forthcol1.HeaderVlaue = day.AdDate.ToString("dd-MMM-yy").ToUpper() + month.month.Trim() + year.Englishyear.Trim();
                                thirdHorizontal.forthHorizontalColumn.Add(forthcol1);
                            }
                            secondHorizantal.ThirdHorizontalColumn.Add(thirdHorizontal);
                        }
                        Horizontal.secondHorizontalPeriodModel.Add(secondHorizantal);

                    }
                    calendarColumnRange.FirstColumnlist.Add(Horizontal);
                }
            }
            return calendarColumnRange;
        }

        public string GetVerticalTableNameByTitleId(int titleId)
        {
            var calendarColumnRange = new CalendarColumnRange();
            //int filterId = 0;
            //if (calenderFilter.ReportId != null)
            //    filterId = Convert.ToInt16(calenderFilter.ReportId);
            var columnNames = this.GetCalenderSetup(titleId).FirstOrDefault();
            return columnNames.vertical_table_name;
        }

        public List<ChartOfAccountSetupModel> ChartOfAccountSelect()
        {
            string query = @"SELECT INITCAP(CA.ACC_EDESC) AS ACC_EDESC,
                                    CA.ACC_CODE AS ACC_CODE,
                                    CA.ACC_TYPE_FLAG,
                                    CA.MASTER_ACC_CODE,
                                    CA.PRE_ACC_CODE, 
                                    CA.BRANCH_CODE, LEVEL
                            FROM FA_CHART_OF_ACCOUNTS_SETUP CA
                            WHERE CA.DELETED_FLAG = 'N' AND CA.COMPANY_CODE = '01'
                            START WITH PRE_ACC_CODE = '00'
                            CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                            ORDER SIBLINGS BY ACC_EDESC";
            var chartOfAccountSelect = _objectEntity.SqlQuery<ChartOfAccountSetupModel>(query).ToList();
            return chartOfAccountSelect;
        }
        public List<ChartOfAccountSetupModel> ChartOfAccountSelect(User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = @"SELECT INITCAP(CA.ACC_EDESC) AS ACC_EDESC,
                                    CA.ACC_CODE AS ACC_CODE,
                                    CA.ACC_TYPE_FLAG,
                                    CA.MASTER_ACC_CODE,
                                    CA.PRE_ACC_CODE, 
                                    CA.BRANCH_CODE, LEVEL
                            FROM FA_CHART_OF_ACCOUNTS_SETUP CA
                            WHERE CA.DELETED_FLAG = 'N' AND CA.COMPANY_CODE = '"+userinfo.company_code+@"'
                            AND CA.Branch_code='"+userinfo.branch_code+@"'
                            START WITH PRE_ACC_CODE = '00'
                            CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                            ORDER SIBLINGS BY ACC_EDESC";
            var chartOfAccountSelect = _objectEntity.SqlQuery<ChartOfAccountSetupModel>(query).ToList();
            return chartOfAccountSelect;
        }

        public List<ChartOfAccountSetupModel> GetAccountListByAccCode(string level, string master_code)
        {
            string query = string.Format(@"SELECT INITCAP(CA.ACC_EDESC) AS ACC_EDESC,
                                    CA.ACC_CODE AS ACC_CODE,
                                    CA.ACC_TYPE_FLAG,
                                    CA.MASTER_ACC_CODE,
                                    CA.PRE_ACC_CODE, 
                                    CA.BRANCH_CODE, LEVEL
                            FROM FA_CHART_OF_ACCOUNTS_SETUP CA
                            WHERE CA.DELETED_FLAG = 'N' AND CA.COMPANY_CODE = '01'
                            AND LEVEL ={0}
                            START WITH PRE_ACC_CODE = '{1}'
                            CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                            ORDER SIBLINGS BY ACC_EDESC", level.ToString(), master_code.ToString());
            var accountListNodes = _objectEntity.SqlQuery<ChartOfAccountSetupModel>(query).ToList();
            return accountListNodes;
        }
        public List<ChartOfAccountSetupModel> GetAccountListByAccCode(string level, string master_code,User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = string.Format(@"SELECT INITCAP(CA.ACC_EDESC) AS ACC_EDESC,
                                    CA.ACC_CODE AS ACC_CODE,
                                    CA.ACC_TYPE_FLAG,
                                    CA.MASTER_ACC_CODE,
                                    CA.PRE_ACC_CODE, 
                                    CA.BRANCH_CODE, LEVEL
                            FROM FA_CHART_OF_ACCOUNTS_SETUP CA
                            WHERE CA.DELETED_FLAG = 'N' AND CA.COMPANY_CODE = '" + userinfo.company_code + @"'
                            AND CA.Branch_code='" + userinfo.branch_code + @"'
                            AND LEVEL ={0}
                            START WITH PRE_ACC_CODE = '{1}'
                            CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                            ORDER SIBLINGS BY ACC_EDESC", level.ToString(), master_code.ToString());
            var accountListNodes = _objectEntity.SqlQuery<ChartOfAccountSetupModel>(query).ToList();
            return accountListNodes;
        }
    }
}

