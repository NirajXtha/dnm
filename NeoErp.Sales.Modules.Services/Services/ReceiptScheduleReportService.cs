using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class ReceiptScheduleReportService : IReceiptScheduleReportService
    {
        private NeoErpCoreEntity _objectEntity;

        public ReceiptScheduleReportService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }



        public IList<ReceiptScheduleModel> GetCustomerWiseReceiptSchedule(ReportFiltersModel model, User userInfo)
        {

            
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            
            string Query = string.Format(@"SELECT  cs.customer_edesc as CustomerName,cbl.voucher_no AS Voucher_No,cbl.manual_no as Manual_No,cs.credit_days,  CBL.voucher_date, 
                            round(NVL(FN_CONVERT_CURRENCY(nvl(CBL.dr_amount,0),'NRS',CBL.VOUCHER_DATE),0)/{0},{1}) as BillAmount, 
                            round(NVL(FN_CONVERT_CURRENCY(nvl(CBL.cr_amount,0),'NRS',CBL.VOUCHER_DATE),0)/{0},{1}) as RecAmount,
                           round(nvl(sum(NVL(FN_CONVERT_CURRENCY(CBL.dr_amount,'NRS',CBL.VOUCHER_DATE),0)-NVL(FN_CONVERT_CURRENCY(CBL.cr_amount,'NRS',CBL.VOUCHER_DATE),0)) over 
                              (PARTITION BY trim(cbl.sub_code) order by CBL.voucher_date asc rows between unbounded preceding and current row)
                           , 0)/{0},{1}) as BalanceAmount
                        FROM  V$CUSTOMER_BILLAGE_LEDGER CBL, SA_CUSTOMER_SETUP CS 
                        where trim(CBL.SUB_CODE) = trim(CS.LINK_SUB_CODE) AND CBL.COMPANY_CODE IN({2})
                        ", ReportFilterHelper.FigureFilterValue(model.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(model.AmountRoundUpFilter),companyCode);

            //Branch Filter
            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND CBL.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            //for date filter
            if (model.FromDate != null && model.ToDate != null)
            {
                Query += " and CBL.voucher_date >= to_date('" + model.FromDate + "','YYYY-MM-DD') and CBL.voucher_date <= to_date('" + model.ToDate + "','YYYY-MM-DD')";
            }

            //for customer Filter
            if (model.CustomerFilter.Count() > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {

                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND CS.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CS.CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));
                //Query += " and (";
                ////IF CUSTOMER_SKU_FLAG = G
                //foreach (var item in model.CustomerFilter)
                //{
                //    Query += "cs.master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                //}
                ////IF CUSTOMER_SKU_FLAG = I                
                //Query += "(cs.CUSTOMER_CODE IN (" + string.Join(",", model.CustomerFilter) + ") AND cs.GROUP_SKU_FLAG = 'I' AND cs.COMPANY_CODE IN(" + companyCode + ") )) ";

                //Query = Query.Substring(0, Query.Length - 1);
            }



            var min = 0;
            var max = 0;
            ReportFilterHelper.RangeFilterValue(model.AmountRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                Query = Query + string.Format(@"  AND NVL(FN_CONVERT_CURRENCY(nvl(CBL.dr_amount,0),'NRS',CBL.VOUCHER_DATE),0) <= '{0}' AND NVL(FN_CONVERT_CURRENCY(nvl(CBL.cr_amount,0),'NRS',CBL.VOUCHER_DATE),0) <= '{1}'", min, max);
            }


            Query += @" order by cbl.voucher_no";

            var salesRegisterCustomers = this._objectEntity.SqlQuery<ReceiptScheduleModel>(Query).ToList();
            return salesRegisterCustomers;
        }


        public IList<PaymentScheduleModel> GetSupplierWisePaymentSchedule(ReportFiltersModel model, User userInfo)
        {
     
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
           
            string Query = string.Format(@"SELECT  SS.SUPPLIER_EDESC as SupplierName,SBL.voucher_no AS Voucher_No,SBL.manual_no as Manual_No,nvl(ss.credit_days,0) as Credit_Days,  SBL.voucher_date, 
                            round(NVL(FN_CONVERT_CURRENCY(nvl(SBL.dr_amount,0),'NRS',SBL.VOUCHER_DATE),0)/{0},{1}) as BillAmount, 
                            round(NVL(FN_CONVERT_CURRENCY(nvl(SBL.cr_amount,0),'NRS',SBL.VOUCHER_DATE),0)/{0},{1}) as RecAmount,
                           round(nvl(sum(NVL(FN_CONVERT_CURRENCY(SBL.dr_amount,'NRS',SBL.VOUCHER_DATE),0)-NVL(FN_CONVERT_CURRENCY(SBL.cr_amount,'NRS',SBL.VOUCHER_DATE),0)) over 
                              (PARTITION BY trim(sbl.sub_code) order by SBL.voucher_no asc rows between unbounded preceding and current row)
                           , 0)/{0},{1}) as BalanceAmount
                        FROM  V$SUPPLIER_BILLAGE_LEDGER SBL, IP_SUPPLIER_SETUP SS 
                        where trim(SBL.SUB_CODE) = trim(SS.LINK_SUB_CODE) 
                        AND SBL.COMPANY_CODE IN ({2})
                        ", ReportFilterHelper.FigureFilterValue(model.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(model.AmountRoundUpFilter),companyCode);

            //Branch Filter
            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND SBL.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            //for date filter
            if (model.FromDate != null && model.ToDate != null)
            {
                Query += " and SBL.voucher_date >= to_date('" + model.FromDate + "','YYYY-MM-DD') and SBL.voucher_date <= to_date('" + model.ToDate + "','YYYY-MM-DD')";
            }

            //for supplier Filter
            if (model.SupplierFilter.Count() > 0)
            {
                var suppliers = model.SupplierFilter;
                var supplierConditionQuery = string.Empty;
                for (int i = 0; i < suppliers.Count; i++)
                {

                    if (i == 0)
                        supplierConditionQuery += string.Format("MASTER_SUPPLIER_CODE like (Select MASTER_SUPPLIER_CODE || '%' from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN ({1}) AND GROUP_SKU_FLAG = 'G')", suppliers[i], companyCode);
                    else
                    {
                        supplierConditionQuery += string.Format(" OR  MASTER_SUPPLIER_CODE like (Select MASTER_SUPPLIER_CODE || '%'  from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN ({1}) AND GROUP_SKU_FLAG = 'G')", suppliers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND Ss.SUPPLIER_CODE IN (SELECT DISTINCT(SUPPLIER_CODE) FROM IP_SUPPLIER_SETUP WHERE {0} OR (SUPPLIER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", supplierConditionQuery, string.Join("','", suppliers));
                //Query += " and (";
                ////IF CUSTOMER_SKU_FLAG = G
                //foreach (var item in model.SupplierFilter)
                //{
                //    Query += "Ss.master_supplier_code like  (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%'  from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                //}
                ////IF CUSTOMER_SKU_FLAG = I               
                //Query += "(Ss.SUPPLIER_CODE IN (" + string.Join(",", model.SupplierFilter) + ") AND ss.GROUP_SKU_FLAG = 'I' AND ss.COMPANY_CODE IN(" + companyCode + ") )) ";

                //Query = Query.Substring(0, Query.Length - 1);
            }



            var min = 0;
            var max = 0;
            ReportFilterHelper.RangeFilterValue(model.AmountRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                Query = Query + string.Format(@"  AND NVL(FN_CONVERT_CURRENCY(nvl(SBL.dr_amount,0),'NRS',SBL.VOUCHER_DATE),0) <= '{0}' AND NVL(FN_CONVERT_CURRENCY(nvl(SBL.cr_amount,0),'NRS',SBL.VOUCHER_DATE),0) <= '{1}'", min, max);
            }


            Query += @" order by SBL.voucher_no";

            var salesRegisterCustomers = this._objectEntity.SqlQuery<PaymentScheduleModel>(Query).ToList();
            return salesRegisterCustomers;
        }
    }
}
