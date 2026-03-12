using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.DistributorServices
{
   public class Dashboard:IDashboard
    {
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;
        public Dashboard(NeoErpCoreEntity objectEntity, IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            _workContext = workContext;
        }
        public List<DistributorTragetChartModel> GetDistributorTraget( User userInfo, string customerCode = "0")
        {
            var whereclause = string.Empty;
            if(customerCode!="0")
            {
                whereclause = $" where customer_code='{customerCode}' and company_code='{userInfo.Company}'";
            }
            //var query = $@"SELECT SUM(TRAGET) TargetAmount,SUM(ACHIVE) ActualAmount,FN_BS_MONTH(MONTHNEPALI) as NepaliMonth,CUSTOMER_CODE as CustomerCode  FROM ( SELECT SUM (PER_DAY_QUANTITY) TRAGET,0 ACHIVE,SUBSTR(BS_DATE(PLAN_DATE),6,2) MONTHNEPALI ,DT. CUSTOMER_CODE
            //                FROM PL_SALES_PLAN_DTL DT where DT.company_code='{userInfo.Company}'
            //                  GROUP BY SUBSTR(BS_DATE(PLAN_DATE),6,2),DT.CUSTOMER_CODE
            //                 UNION ALL select  0 TRAGET,SUM(CALC_TOTAL_PRICE) ACHIVE,SUBSTR(BS_DATE(trunc(SALES_DATE)),6,2) MONTHNEPALI,CUSTOMER_CODE
            //                 FROM SA_SALES_INVOICE  where company_code='{userInfo.Company}'
            //                 GROUP BY SUBSTR(BS_DATE(trunc(SALES_DATE)),6,2),CUSTOMER_CODE ) {whereclause} GROUP BY MONTHNEPALI,CUSTOMER_CODE";

            //var query = $@"SELECT SUM(TRAGET) TargetAmount,SUM(ACHIVE) ActualAmount,FN_BS_MONTH(SUBSTR(MONTHNEPALI,5,2)) as NepaliMonth,CUSTOMER_CODE as CustomerCode,QUANTITY ,MONTHNEPALI as nepalimonthint FROM ( SELECT SUM (PER_DAY_AMOUNT) TRAGET,0 ACHIVE,to_number(replace(SUBSTR(BS_DATE(PLAN_DATE),0,7),'-')) MONTHNEPALI ,CUSTOMER_CODE,SUM(PER_DAY_QUANTITY) AS QUANTITY
            //                FROM PL_SALES_PLAN_DTL DT {whereclause}
            //                  GROUP BY to_number(replace(SUBSTR(BS_DATE(PLAN_DATE),0,7),'-')),DT.CUSTOMER_CODE
            //                 UNION ALL select  0 TRAGET,SUM(CALC_TOTAL_PRICE) ACHIVE,to_number(replace(SUBSTR(BS_DATE(sales_date),0,7),'-')) MONTHNEPALI,CUSTOMER_CODE,SUM(QUANTITY) AS QUANTITY
            //                 FROM SA_SALES_INVOICE {whereclause}
            //                 GROUP BY to_number(replace(SUBSTR(BS_DATE(sales_date),0,7),'-')),CUSTOMER_CODE )  GROUP BY MONTHNEPALI,CUSTOMER_CODE,QUANTITY order by nepalimonthint";

            var selectQuery = $@"SELECT  AA.TRAGET TargetAmount,BB.ACHIVE ActualAmount, AA.CUSTOMER_CODE CustomerCode, AA.MONTHNEPALI as nepalimonthint,  ROUND(AA.QUANTITY,0) QUANTITY , ROUND(BB.QUANTITY,0) ACHIVE_QTY,FN_BS_MONTH(SUBSTR(AA.MONTHNEPALI,5,2)) as NepaliMonth
                            FROM (SELECT SUM (PER_DAY_AMOUNT) TRAGET,to_number(replace(SUBSTR(BS_DATE(PLAN_DATE),0,7),'-')) MONTHNEPALI  ,CUSTOMER_CODE,SUM(PER_DAY_QUANTITY) AS QUANTITY
                            FROM PL_SALES_PLAN_DTL DT {whereclause}
                              GROUP BY to_number(replace(SUBSTR(BS_DATE(PLAN_DATE),0,7),'-')),DT.CUSTOMER_CODE) AA, 
                            ( select SUM(CALC_TOTAL_PRICE) ACHIVE,to_number(replace(SUBSTR(BS_DATE(sales_date),0,7),'-')) MONTHNEPALI,CUSTOMER_CODE,SUM(QUANTITY) AS QUANTITY
                             FROM SA_SALES_INVOICE  {whereclause}
                             GROUP BY to_number(replace(SUBSTR(BS_DATE(sales_date),0,7),'-')),CUSTOMER_CODE) BB
                             WHERE AA.MONTHNEPALI= BB.MONTHNEPALI
                             AND AA.CUSTOMER_CODE = BB.CUSTOMER_CODE
                             ORDER BY AA.MONTHNEPALI";



            var data = _objectEntity.SqlQuery<DistributorTragetChartModel>(selectQuery).ToList();
            return data;
        }

        public List<DashBoardWidgets> GetDashBoardWidgets(User userInfo)
        {
            var query = $@"select nvl(credit_limit,0) as Valueamout, 'Credit Limit' as Title from SA_CUSTOMER_SETUP where company_code='{userInfo.Company}' and customer_code='{userInfo.DistributerNo}' and deleted_flag='N'";
            var data = _objectEntity.SqlQuery<DashBoardWidgets>(query).ToList();
            var firstdata = data.FirstOrDefault();
            var ledgerQuery = $@"select nvl(sum(nvl(dr_amount,0)),0) DRAmount,nvl(sum(nvl(cr_amount,0)),0) CRAmount
                              from V$VIRTUAL_SUB_LEDGER where acc_code=(select link_sub_code from sa_customer_setup where company_code='{userInfo.Company}' and deleted_flag='N' and customer_code='{userInfo.DistributerNo}')
                              and deleted_flag='N' and company_code='{userInfo.Company}'";
            var ledgerAmount = _objectEntity.SqlQuery<LedgerAmount>(ledgerQuery).FirstOrDefault();

            data.Add(new DashBoardWidgets { Title="Credit Amount",Valueamout= ledgerAmount.CRAmount });
            data.Add(new DashBoardWidgets { Title = "Balance Amount", Valueamout = firstdata.Valueamout-(ledgerAmount.CRAmount-ledgerAmount.DRAmount) });
            return data;
        }

        public List<DistMatrixModel> GetDistMatrics(bool ispermission = false)
        {
            var returnList = new List<DistMatrixModel>();
            try
            {
                var sqlQuery = @"select id as ReportId,USERPERMISSION, isactive as IsActive,orderno as Orderno, quickcap_edesc as widgetsName,
            quickcap_bgcolor as WidgetsBGColor,quickcap_title as widgetsTitle,sql_statement as sqlQuery,quickcap_fontcolor as widgetsColor
            ,QUICKTYPE ,MIDBGCOLOR,MIDFONTCOLOR,MIDISBLINK,MAXBGCOLOR,MAXFONTCOLOR,MAXISBLINK,MAXVALUES,MINVALUES,MINISBLINK,LABELPOSITION,SPEEDOMETERMAXVALUE,MAXVALUEQUERY,MINVALUEQUERY,MODULE_CODE,DISTRIBUTOR_CHECK_TYPE,DISTRIBUTOR_CODE from dist_quickcap
            where deleted_flag = 'N'";
                var userID = _workContext.CurrentUserinformation.User_id;

                //var Widgets = _objectEntity.SqlQuery<MetricWidgetsModel>(sqlQuery).ToList().Where(x => x.USERPERMISSION == null ? false : x.USERPERMISSION.Contains(userID.ToString())).ToList();
                var Widgets = _objectEntity.SqlQuery<DistMatrixModel>(sqlQuery).ToList();

                return Widgets;
              
            }
            catch (Exception ex)
            {
                return returnList;
            }
        }
    }
}
