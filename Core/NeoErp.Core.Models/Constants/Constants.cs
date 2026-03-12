using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models.Constants
{
    public class Constants
    {

        public const string CalendarReportMonthlyPartyWiseSalesQuery = @"SELECT INITCAP(CS.CUSTOMER_EDESC) AS CustomerName,
                                                                              CS.CUSTOMER_CODE AS CustomerCode,
                                                                              TO_CHAR(SI.SALES_DATE,'Month') as MonthName, 
                                                                              TO_CHAR(SI.SALES_DATE,'yyyy') as YearName,
                                                                              SUM(SI.QUANTITY) AS Quantity,
                                                                              SUM(SI.CALC_TOTAL_PRICE) AS Price,
                                                                              SUM(SI.CALC_TOTAL_PRICE)*SUM(SI.QUANTITY) AS Total
                                                                              FROM SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS
                                                                              WHERE SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                                                                              AND SI.COMPANY_CODE='01' 
                                                                              AND SI.BRANCH_CODE='01.01' 
                                                                              AND TO_DATE(TO_CHAR(SALES_DATE,'DD-MON-YYYY')) <= TO_DATE('10-JUN-2016')  
                                                                              GROUP BY 
                                                                              TO_CHAR(SI.SALES_DATE,'Month'), CS.CUSTOMER_EDESC,
                                                                              TO_CHAR(SI.SALES_DATE,'yyyy'), 
                                                                              TO_DATE(TO_CHAR(SI.SALES_DATE,'Mon-yyyy'),'Mon-yyyy'),
                                                                              CS.CUSTOMER_CODE
                                                                              ORDER BY TO_DATE(TO_CHAR(SI.SALES_DATE,'Mon-yyyy'),'Mon-yyyy')";

        public const string CalendarTypeADVsBSWeeksQuery = @"SELECT  BS_DATE(a) as Miti,a as AdDate,
                                                SUBSTR(BS_DATE(a),6,2) as NepaliMonthInt,
                                                TO_CHAR(a ,'MM') as MonthInt,
                                                fn_bs_month(substr(bs_date(a),6,2)) as Nepalimonth,
                                                TO_CHAR(a ,'Month') as month,  
                                                substr(bs_date(a),0,4) as Nepaliyear,
                                                TO_CHAR(a ,'YYYY') as Englishyear,  
                                                trunc((a- TO_DATE('{0}'))/7)+1  weeks 
                                                FROM(
                                                     SELECT ROWNUM - 1 + TO_DATE ('{0}', 'dd-mon-yyyy') a 
                                                     FROM all_objects 
                                                     WHERE ROWNUM < TO_DATE ('{1}', 'dd-mon-yyyy') - TO_DATE ('{0}', 'dd-mon-yyyy')
                                                 + 2)";

        public const string GetWeekNumberQuery = @"WITH yourtable as
                                        (
                                        SELECT to_date('{0}','DD-MM-YYYY')+(rownum-1) dat 
                                        from dual connect by to_date('{0}','DD-MM-YYYY')+(rownum-1)<=to_date('{1}','DD-MM-YYYY'))
                                        select
                                        distinct     TO_CHAR(dat,'WW') as WeekNumber , TO_CHAR(dat,'Month') as MonthName,TO_CHAR(dat,'YYYY') as Year
                                        FROM yourtable
                                        ORDER BY TO_CHAR(dat,'WW')";

        public const string GetMonthNumberQuery = @"WITH yourtable as
                                        (
                                        SELECT to_date('{0}','DD-MM-YYYY')+(rownum-1) dat 
                                        from dual connect by to_date('{0}','DD-MM-YYYY')+(rownum-1)<=to_date('{1}','DD-MM-YYYY'))
                                        select distinct to_char(dat,'MM') as MonthNumber , 
                                        to_char(dat,'Month') as MonthName,
                                        to_char(dat,'YYYY') as Year
                                        from yourtable
                                        order by to_char(dat,'MM'),to_char(dat,'YYYY')";

        public const string GetCalendarReportDetailsQuery = @"SELECT 
                                         REPORT_EDESC, SOURCE_DATA_ENTITY, FORM_FILTER_FLAG, 
                                         FORM_CODE, CALENDAR_TYPE_FLAG, 
                                         FROM_DATE, TO_DATE,  
                                         FIRST_HOR_PERIOD_FLAG, SECOND_HOR_PERIOD_FLAG, 
                                         VERTICAL_TABLE_NAME, 
                                         GROUP_WISE_FLAG, DATE_FORMAT, 
                                         SOURCE_DATA_COLUMN,REMARKS,TRANSACTION_TYPE, 
                                         SOURCE_DATA_COLUMN2, 
                                         COLUMN1_HEADING, COLUMN2_HEADING,
                                         MULTI_REPORT, OPEN_CLS_FLAG 
                                         FROM CALENDAR_REPORT_SETUP
                                         WHERE COMPANY_CODE = '01'  
                                         AND GROUP_SKU_FLAG = 'I'  
                                         AND DELETED_FLAG = 'N' ";

        public const string GetDateFiltersQuery = @"SELECT StartDate,EndDate ,RangeName, SortOrder FROM(
                                                 SELECT CS.AD_DATE StartDate,
                                                        (CS.AD_DATE + CS.DAYS_NO-1) EndDate,
                                                       FN_BS_MONTH(SUBSTR(CS.BS_MONTH,-2,2)) RangeName, 1 AS SortOrder
                                                   FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{0}'
                                                 UNION ALL
                                                 select  START_DATE, END_DATE,'This Year' date_name , 2 AS SortOrder  from HR_FISCAL_YEAR_CODE where FISCAL_YEAR_CODE = '{1}'
                                                 UNION ALL
                                                 select AD_DATE START_DATE , (AD_DATE+DAYS_NO-1) END_DATE , 'This Month' date_name, 3 AS SortOrder FROM CALENDAR_SETUP where sysdate between AD_DATE and (AD_DATE+DAYS_NO-1) 
                                                 UNION ALL
                                                 SELECT distinct
                                                   FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE DESC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                   FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'Last Month' DATE_NAME, 4 AS SortOrder
                                                    FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{2}'
                                                       AND  CS.AD_DATE< sysdate-DAYS_NO
                                                 UNION ALL
                                                 select trunc((next_day(sysdate,'SUN')-7)) start_date ,trunc(sysdate) end_date, 'This Week' date_name, 5 AS SortOrder  from dual
                                                 UNION ALL
                                                 select trunc((next_day(sysdate,'SUN')-14)) start_date ,trunc( (next_day(sysdate,'SAT')-7)) end_date, 'Last Week' date_name, 6  AS SortOrder  from dual
                                                 UNION ALL
                                                 SELECT DISTINCT
                                                        FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                        FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'First Quarter' DATE_NAME, 7 AS SortOrder
                                                 FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{3}'
                                                        AND SUBSTR(CS.BS_MONTH,-2,2) >='04'
                                                         AND SUBSTR(CS.BS_MONTH,-2,2) <'07'
                                                 UNION ALL
                                                 SELECT DISTINCT
                                                        FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                        FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'Second Quarter' DATE_NAME, 8 AS SortOrder
                                                 FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{4}'
                                                        AND SUBSTR(CS.BS_MONTH,-2,2) >='07'
                                                         AND SUBSTR(CS.BS_MONTH,-2,2) <'10'
                                                 UNION ALL 
                                                  SELECT DISTINCT
                                                        FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                        FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'Third Quarter' DATE_NAME, 9 AS SortOrder
                                                 FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{5}'
                                                        AND SUBSTR(CS.BS_MONTH,-2,2) >='10'
                                                         AND SUBSTR(CS.BS_MONTH,-2,2) <='12'
                                                 UNION ALL     
                                                 SELECT DISTINCT
                                                        FIRST_VALUE(CS.AD_DATE)  OVER (ORDER BY CS.AD_DATE ASC, (CS.AD_DATE + CS.DAYS_NO-1) DESC) START_DATE,
                                                        FIRST_VALUE(CS.AD_DATE + CS.DAYS_NO-1) OVER (ORDER BY (CS.AD_DATE + CS.DAYS_NO-1) DESC, CS.AD_DATE ASC) END_DATE, 'Forth Quarter' DATE_NAME, 10 AS SortOrder
                                                 FROM HR_FISCAL_YEAR_CODE FY, CALENDAR_SETUP CS
                                                 WHERE CS.AD_DATE BETWEEN FY.START_DATE AND FY.END_DATE
                                                        AND FY.FISCAL_YEAR_CODE = '{6}'
                                                        AND SUBSTR(CS.BS_MONTH,-2,2) >='01'
                                                         AND SUBSTR(CS.BS_MONTH,-2,2) <='03') ORDER BY SortOrder, StartDate";
        
        public const string GetAdVersusBsCalendarQuery = @"SELECT  BS_DATE(a) AS Miti,
                                                            a AS AdDate,substr(bs_date(a),6,2) AS NepaliMonthInt,
                                                            TO_CHAR(a ,'MM') AS MonthInt,
                                                            fn_bs_month(substr(bs_date(a),6,2)) AS Nepalimonth,
                                                            TO_CHAR(a ,'Month') AS month,  
                                                            SUBSTR(bs_date(a),0,4) AS Nepaliyear,
                                                            TO_CHAR(a ,'YYYY') AS Englishyear, 
                                                            TRUNC((a- TO_DATE('17-Jul-2015'))/7)+1  weeks 
                                                            FROM(
                                                               SELECT ROWNUM - 1 + TO_DATE ('17-Jul-2015', 'dd-mon-yyyy') a 
                                                               FROM all_objects 
                                                               WHERE ROWNUM < TO_DATE ('15-Jul-2016', 'dd-mon-yyyy') - TO_DATE ('17-Jul-2015', 'dd-mon-yyyy')
                                                               + 2) ";

        public const string ItemWise_Calendar_Query = @"SELECT fa.ITEM_CODE as Id, fa.MASTER_ITEM_CODE as MasterCode, FA.PRE_ITEM_CODE as PreMasterCode, fa.ITEM_EDESC as Description,  
                                                            #calenderWise,  
                                                            SUM(vl.{0}) AS Quantity,
                                                            SUM (vl.{1}) AS NetAmount 
                                                            FROM {2} vl,
                                                            {3} fa,
                                                            ({4}) np  
                                                            WHERE vl.ITEM_CODE=fa.ITEM_CODE and fa.deleted_flag='N'  
                                                            AND vl.{5} >= TO_DATE('{6}', 'dd-mon-yyyy') 
                                                            AND vl.{5} <= TO_DATE('{7}', 'dd-mon-yyyy') 
                                                            AND   np.adDate = vl.{5} and fa.company_code='{8}' ";

        public const string CustomerWise_Calendar_Query = @"SELECT  fa.customer_code as Id, fa.master_customer_code as MasterCode, FA.PRE_CUSTOMER_CODE as PreMasterCode, fa.CUSTOMER_EDESC as Description,
                                                             #calenderWise, 
                                                            SUM(vl.{0}) AS Quantity,
                                                            SUM (vl.{1}) AS NetAmount 
                                                            FROM {2} vl,
                                                            {3} fa,
                                                            ({4}) np   
                                                            WHERE vl.CUSTOMER_CODE=fa.CUSTOMER_CODE and fa.deleted_flag='N' 
                                                            AND vl.{5} >= TO_DATE('{6}', 'dd-mon-yyyy') 
                                                            AND vl.{5} <= TO_DATE('{7}', 'dd-mon-yyyy') 
                                                            AND   np.adDate = vl.{5} and fa.company_code='{8}' ";

        public const string SupplierWise_Calendar_Query = @"SELECT  fa.SUPPLIER_CODE as Id, fa.MASTER_SUPPLIER_CODE as MasterCode, FA.PRE_SUPPLIER_CODE as PreMasterCode, fa.SUPPLIER_EDESC as Description,
                                                             #calenderWise, 
                                                             SUM(vl.{0}) AS Quantity,
                                                             SUM (vl.{1}) AS NetAmount 
                                                             FROM {2} vl,
                                                             {3} fa,
                                                             ({4}) np  
                                                             WHERE vl.SUPPLIER_CODE=fa.SUPPLIER_CODE 
                                                             AND fa.deleted_flag='N' 
                                                             AND vl.{5} >= TO_DATE('{6}', 'dd-mon-yyyy') 
                                                             AND vl.{5} <= TO_DATE('{7}', 'dd-mon-yyyy') 
                                                             AND   np.adDate = vl.{5} 
                                                             AND fa.company_code='{8}'";

        public const string Account_Wise_CalendarQuery = @"select  fa.ACC_CODE as Id,fa.MASTER_ACC_CODE as MasterCode,FA.PRE_ACC_CODE as PreMasterCode,fa.ACC_EDESC as Description,
                                                             #calenderWise, 
                                                             SUM(vl.{0}) AS Quantity,
                                                             SUM (vl.{1}) AS NetAmount 
                                                             FROM {2} vl,
                                                             {3} fa,
                                                             ({4}) np  
                                                             WHERE vl.ACC_CODE=fa.ACC_CODE 
                                                             AND fa.deleted_flag='N' 
                                                             AND vl.{5} >= TO_DATE('{6}', 'dd-mon-yyyy') 
                                                             AND vl.{5} <= TO_DATE('{7}', 'dd-mon-yyyy') 
                                                             AND   np.adDate = vl.{5} and fa.company_code='{8}'";
        public const string WebPrefranceSetting = "webprefrence";
    }
    public enum NepaliMonths { Shrawan, Bhadra, Ashoj, Kartik, Mangsir, Poush, Magh, Falgun, Chaitra, Baisakh, Jestha, Ashadh };
    
}
