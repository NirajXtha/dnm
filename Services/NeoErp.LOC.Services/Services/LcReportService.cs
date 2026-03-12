using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.LOC.Services.Models;
using NeoErp.Data;
using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Core.Domain;

namespace NeoErp.LOC.Services.Services
{
    public class LcReportService : ILcReportService
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        public LcReportService(IDbContext dbContext, IWorkContext workContext)
        {
            _dbContext = dbContext;
            _workContext = workContext;
        }



        //public List<DueInvoiceReportModels> GetAllDueInvoiceReports(ReportFiltersModel reportFilters)
        //{
        //    var sqlquery = $@"SELECT LL.LC_TRACK_NO,FB.BANK_NAME,
        //                   LL.LC_NUMBER,
        //                    IPS.SUPPLIER_EDESC,
        //                   LI.INVOICE_DATE,
        //                   LI.INVOICE_NUMBER,
        //                   LPI.CURRENCY_CODE,
        //                   LI.AMOUNT,
        //                   IIS.BRAND_NAME,
        //                   IMS.PRE_ITEM_CODE,
        //                   (LI.INVOICE_DATE + LL.CREDIT_DAYS) DUE_DATE,
        //                   ( (SELECT EXCHANGE_RATE
        //                        FROM EXCHANGE_DETAIL_SETUP
        //                       WHERE CURRENCY_CODE = LPI.CURRENCY_CODE
        //                             AND EXCHANGE_DATE <= LI.INVOICE_DATE
        //                             AND EXCHANGE_DATE =
        //                                    (SELECT MAX (EXCHANGE_DATE)
        //                                       FROM EXCHANGE_DETAIL_SETUP
        //                                      WHERE CURRENCY_CODE = LPI.CURRENCY_CODE))
        //                    * LI.AMOUNT)  AS NPR_VALUE,
        //                   LL.CREDIT_DAYS,
        //                   FN_FETCH_GROUP_DESC ('01', 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE)
        //                      AS PARENT_NAME
        //              FROM LC_LOC LL
        //                  LEFT JOIN FA_BANK_SETUP FB
        //                      ON LL.ISSUING_BANK_CODE = FB.BANK_CODE
        //                   JOIN LC_INVOICE LI
        //                      ON LL.LC_TRACK_NO = LI.LC_TRACK_NO
        //                      JOIN LC_PERFOMA_INVOICE LPI 
        //                      ON LL.LC_TRACK_NO = LPI.LC_TRACK_NO
        //                   JOIN IP_ITEM_MASTER_SETUP IMS
        //                      ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
        //                      JOIN  IP_ITEM_SPEC_SETUP IIS 
        //                      ON LI.ITEM_CODE = IIS.ITEM_CODE AND LI.COMPANY_CODE = IIS.COMPANY_CODE
        //                         JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE AND IPS.PARTY_TYPE_CODE = 'LC' AND LL.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

        //    if (!string.IsNullOrEmpty(reportFilters.FromDate))
        //        sqlquery = sqlquery + " AND LI.INVOICE_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

        //    if (reportFilters.CompanyFilter.Count > 0)
        //        sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";

        //    if (reportFilters.CurrencyFilter.Count > 0)
        //        sqlquery = sqlquery + " AND  LPI.CURRENCY_CODE IN('" + string.Join("','", reportFilters.CurrencyFilter).ToString() + "')";

        //    if (reportFilters.BrandFilter.Count > 0)
        //        sqlquery = sqlquery + " AND IIS.BRAND_NAME IN('" + string.Join("','", reportFilters.BrandFilter).ToString().ToUpper() + "')";

        //    var lclist = _dbContext.SqlQuery<DueInvoiceReportModels>(sqlquery).ToList();
        //    return lclist;
        //}


        public List<DueInvoiceReportModels> GetAllDueInvoiceReports(ReportFiltersModel reportFilters)
        {
            var sqlquery = $@"SELECT UNIQUE LI.INVOICE_NUMBER
                                            ,TO_CHAR(LI.INVOICE_DATE,'MM/DD/YYYY') AS INVOICE_DATE
	                                        ,LI.LC_NUMBER
	                                        ,PO.ORDER_NO
	                                        ,TO_CHAR(PO.ORDER_DATE,'MM/DD/YYYY') AS ORDER_DATE
	                                        ,PO.CREDIT_DAYS
	                                        ,TO_CHAR(PO.ORDER_DATE + PO.CREDIT_DAYS,'MM/DD/YYYY' ) AS DUE_DATE
                                        FROM LC_INVOICE LI
	                                        ,LC_PURCHASE_ORDER PO
                                        WHERE LI.LC_TRACK_NO = PO.LC_TRACK_NO
	                                        AND PO.PTERMS_CODE = '122'";

            if (!string.IsNullOrEmpty(reportFilters.FromDate))
                sqlquery = sqlquery + " AND PO.ORDER_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

            if (reportFilters.InvoiceNumberFilter.Count > 0)
                sqlquery = sqlquery + " AND LI.INVOICE_NUMBER IN('" + string.Join("','", reportFilters.InvoiceNumberFilter).ToString() + "')";

            if (reportFilters.LcNumberFilter.Count > 0)
                sqlquery = sqlquery + " AND LI.LC_NUMBER IN('" + string.Join("','", reportFilters.LcNumberFilter).ToString() + "')";

            //if (reportFilters.CompanyFilter.Count > 0)
            //    sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";

            //if (reportFilters.CurrencyFilter.Count > 0)
            //    sqlquery = sqlquery + " AND  LPI.CURRENCY_CODE IN('" + string.Join("','", reportFilters.CurrencyFilter).ToString() + "')";

            //if (reportFilters.BrandFilter.Count > 0)
            //    sqlquery = sqlquery + " AND IIS.BRAND_NAME IN('" + string.Join("','", reportFilters.BrandFilter).ToString().ToUpper() + "')";

            var lclist = _dbContext.SqlQuery<DueInvoiceReportModels>(sqlquery).ToList();
            return lclist;
        }

        public List<PendingLcReportModels> GetAllPendingLcReports(ReportFiltersModel reportFilters)
        {
            var sqlquery = $@"SELECT DISTINCT  LPI.PINVOICE_NO, LPI.PINVOICE_DATE,
                                                NVL(SUM(LI.QUANTITY),0) AS QUANTITY,
                                                NVL(SUM(LI.AMOUNT) ,0)AS AMOUNT,
                                               (NVL(SUM(LI.QUANTITY),0)*NVL(SUM(LI.AMOUNT) ,0))AS TOTAL_AMOUNT,
                                               LPI.LC_TRACK_NO,
                                               LPI.CURRENCY_CODE
                                               FROM LC_PERFOMA_INVOICE LPI,
                                               LC_ITEM LI,
                                               IP_ITEM_MASTER_SETUP IMS
                                        WHERE LPI.LC_TRACK_NO = LI.LC_TRACK_NO
                                               AND IMS.GROUP_SKU_FLAG = 'I'
                                               AND IMS.DELETED_FLAG = 'N'
                                               AND LI.ITEM_CODE = IMS.ITEM_CODE
                                                AND LI.COMPANY_CODE = IMS.COMPANY_CODE
                                               AND LPI.LC_TRACK_NO NOT IN (SELECT LC_TRACK_NO FROM LC_LOC) AND LPI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

            if (!string.IsNullOrEmpty(reportFilters.FromDate))
                sqlquery = sqlquery + " AND LPI.PINVOICE_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

            //if (reportFilters.SupplierFilter.Count > 0)
            //    sqlquery = sqlquery + " AND LPI.SUPPLIER_CODE IN('" + string.Join("','", reportFilters.SupplierFilter).ToString() + "')";

            if (reportFilters.CompanyFilter.Count > 0)
                sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";

            if (reportFilters.CurrencyFilter.Count > 0)
                sqlquery = sqlquery + " AND  LPI.CURRENCY_CODE IN('" + string.Join("','", reportFilters.CurrencyFilter).ToString() + "')";

            //if (reportFilters.BrandFilter.Count > 0)
            //    sqlquery = sqlquery + " AND ISS.BRAND_NAME IN('" + string.Join("','", reportFilters.BrandFilter).ToString().ToUpper() + "')"; 

            sqlquery += " GROUP BY LPI.PINVOICE_NO, LPI.PINVOICE_DATE,LPI.LC_TRACK_NO,LPI.CURRENCY_CODE";


            var lclist = _dbContext.SqlQuery<PendingLcReportModels>(sqlquery).ToList();
            return lclist;
        }




        public List<PendingLcReportModels> GetAllOpenLCReports(ReportFiltersModel reportFilters)
        {
            var sqlquery = $@"SELECT DISTINCT NVL(SUM(LI.QUANTITY) ,0)AS QUANTITY,
                               NVL(SUM(LI.AMOUNT),0) AS AMOUNT,
                               (NVL(SUM(LI.QUANTITY) ,0)*NVL(SUM(LI.AMOUNT),0)) AS TOTAL_AMOUNT,
                               NVL(IPS.SUPPLIER_EDESC, LL.LC_NUMBER) AS LC_NUMBER,
                               IPS.SUPPLIER_EDESC,
                               TO_CHAR(LL.OPEN_DATE,'MM-DD-YYYY') AS OPEN_DATE,
                               TO_CHAR(LL.EXPIRY_DATE,'MM-DD-YYYY') AS EXPIRY_DATE,
                               TO_CHAR(LL.LAST_SHIPMENT_DATE,'MM-DD-YYYY') AS LAST_SHIPMENT_DATE,
                                LL.LC_TRACK_NO,
                                LPI.CURRENCY_CODE,
                                LB.BNF_EDESC AS BENIFICARY
                                FROM LC_PERFOMA_INVOICE LPI,
                               LC_ITEM LI,
                               IP_ITEM_SPEC_SETUP ISS,
                               IP_ITEM_MASTER_SETUP IMS,
                               LC_LOC LL,
                              LC_BENEFICIARY LB,
                              IP_SUPPLIER_SETUP IPS 
                        WHERE   LPI.LC_TRACK_NO = LI.LC_TRACK_NO
                               AND LPI.LC_TRACK_NO = LL.LC_TRACK_NO
                               AND IMS.GROUP_SKU_FLAG = 'I'
                               AND IMS.DELETED_FLAG = 'N'
                               AND LI.ITEM_CODE = IMS.ITEM_CODE
                                AND LI.COMPANY_CODE = IMS.COMPANY_CODE
                               AND LI.ITEM_CODE = ISS.ITEM_CODE
                               AND LPI.BNF_CODE=LB.BNF_CODE
                                AND LI.COMPANY_CODE = ISS.COMPANY_CODE
                                --AND LL.LC_NUMBER(+)= IPS.SUPPLIER_CODE
                               AND LL.LC_NUMBER(+)= IPS.SUPPLIER_EDESC
                               AND LL.PINVOICE_CODE = LPI.PINVOICE_CODE AND LPI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

            if (!string.IsNullOrEmpty(reportFilters.FromDate))
                sqlquery = sqlquery + " AND LL.OPEN_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

            //if (reportFilters.SupplierFilter.Count > 0)
            //    sqlquery = sqlquery + " AND LPI.SUPPLIER_CODE IN('" + string.Join("','", reportFilters.SupplierFilter).ToString() + "')";

            if (reportFilters.CompanyFilter.Count > 0)
                sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";

            if (reportFilters.CurrencyFilter.Count > 0)
                sqlquery = sqlquery + " AND  LPI.CURRENCY_CODE IN('" + string.Join("','", reportFilters.CurrencyFilter).ToString() + "')";

            //if (reportFilters.BrandFilter.Count > 0)
            //    sqlquery = sqlquery + " AND ISS.BRAND_NAME IN('" + string.Join("','", reportFilters.BrandFilter).ToString().ToUpper() + "')";

            sqlquery += "     GROUP BY   LL.LC_NUMBER,IPS.SUPPLIER_EDESC,LL.OPEN_DATE,LL.EXPIRY_DATE,LL.LC_TRACK_NO,LPI.CURRENCY_CODE,LL.LAST_SHIPMENT_DATE,LB.BNF_EDESC";
            var lclist = _dbContext.SqlQuery<PendingLcReportModels>(sqlquery).ToList();
            return lclist;
        }

        //public List<LcStatusReportModels> GetAllLcStatusReports(ReportFiltersModel reportFilters)
        //{
        //    var sqlquery = $@"SELECT LL.LC_TRACK_NO,LL.LC_NUMBER,IPS.SUPPLIER_EDESC,LL.OPEN_DATE,LL.EXPIRY_DATE,LL.STATUS_CODE,LS.STATUS_EDESC,(SUM(LI.AMOUNT)*SUM(LI.QUANTITY)) AS PFI_AMOUNT,SUM(LI.QUANTITY) AS TOTAL_QTY FROM LC_LOC  LL
        //                    JOIN LC_STATUS LS ON LL.STATUS_CODE = LS.STATUS_CODE
        //                    JOIN LC_ITEM LI ON LL.LC_TRACK_NO = LI.LC_TRACK_NO
        //                    LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE AND IPS.PARTY_TYPE_CODE = 'LC' AND LL.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

        //    if (!string.IsNullOrEmpty(reportFilters.FromDate))
        //        sqlquery = sqlquery + " WHERE LL.OPEN_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

        //    if (reportFilters.LcStatusFilter.Count > 0)
        //        sqlquery = sqlquery + " AND LL.STATUS_CODE IN('" + string.Join("','", reportFilters.LcStatusFilter).ToString() + "')";

        //    if (reportFilters.CompanyFilter.Count > 0)
        //        sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";

        //    sqlquery += "GROUP BY LL.LC_TRACK_NO,LL.LC_NUMBER,LL.OPEN_DATE,LL.EXPIRY_DATE,LL.STATUS_CODE,LS.STATUS_EDESC,IPS.SUPPLIER_EDESC";

        //    //if (reportFilters.BrandFilter.Count > 0)
        //    //    sqlquery = sqlquery + " AND ISS.BRAND_NAME IN('" + string.Join("','", reportFilters.BrandFilter).ToString().ToUpper() + "')";


        //    var lclist = _dbContext.SqlQuery<LcStatusReportModels>(sqlquery).ToList();
        //    return lclist;
        //}

        public List<LcStatusReportModels> GetAllLcStatusReports(ReportFiltersModel reportFilters)
        {
            var sqlquery = $@"SELECT NVL(ISS.SUPPLIER_EDESC, LL.LC_NUMBER) AS LC_NUMBER
	                                ,LL.LC_TRACK_NO
	                                ,TO_CHAR(LL.OPEN_DATE,'MM/DD/YYYY') AS OPEN_DATE
	                                ,TO_CHAR(LL.EXPIRY_DATE,'MM/DD/YYYY') AS EXPIRY_DATE
	                                ,TO_CHAR(LL.LAST_SHIPMENT_DATE,'MM/DD/YYYY') AS LAST_SHIPMENT_DATE
	                                ,LS.STATUS_EDESC
	                                ,SUM(LINV.AMOUNT * LINV.QUANTITY) AS TOTAL_RECEIVED_AMOUNT
	                                ,LI.CURRENCY_CODE LC_CURRENCY
	                                ,SUM(LI.AMOUNT * LI.QUANTITY) AS LC_AMOUNT
	                                ,SUM(LI.AMOUNT * LI.QUANTITY) - SUM(LINV.AMOUNT * LINV.QUANTITY) AS  BALANCE_AMOUNT   
                                FROM LC_LOC LL
	                                ,LC_ITEM LI
	                                ,LC_STATUS LS
	                                ,IP_SUPPLIER_SETUP ISS
	                                ,LC_INVOICE LINV
	                                ,LC_PERFOMA_INVOICE LPI
                                WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO
	                                AND LL.STATUS_CODE = LS.STATUS_CODE
	                                AND LL.LC_TRACK_NO = LINV.LC_TRACK_NO(+)
	                                AND LL.LC_TRACK_NO = LPI.LC_TRACK_NO
	                                AND LL.LC_NUMBER = ISS.SUPPLIER_CODE(+)";

            if (reportFilters.LcStatusFilter.Count > 0)
                sqlquery = sqlquery + " AND LL.STATUS_CODE IN('" + string.Join("','", reportFilters.LcStatusFilter).ToString() + "')";
            if (!string.IsNullOrEmpty(reportFilters.FromDate))
                sqlquery = sqlquery + " AND LL.OPEN_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

            if (reportFilters.LcNumberFilter.Count > 0)
                sqlquery = sqlquery + " AND ISS.SUPPLIER_EDESC  IN('" + string.Join("','", reportFilters.LcNumberFilter).ToString() + "')";
            sqlquery += "GROUP BY ISS.SUPPLIER_EDESC,LL.LC_NUMBER,LL.LC_TRACK_NO,LL.OPEN_DATE,LL.EXPIRY_DATE,LL.LAST_SHIPMENT_DATE,LS.STATUS_EDESC,LI.CURRENCY_CODE";


            var lclist = _dbContext.SqlQuery<LcStatusReportModels>(sqlquery).ToList();
            return lclist;
        }

        public List<LcStatusReportModels> GetAllLcChildStatus(string lctrackno)
        {
            var sqlquery = $@"SELECT LI.LC_TRACK_NO, LI.ITEM_CODE,
                                    IMS.ITEM_EDESC,
                                   LI.AMOUNT AS PFI_AMOUNT,
                                   LI.QUANTITY AS TOTAL_QTY,
                                   IIS.BRAND_NAME,
                                   LIN.INVOICE_DATE,
                                   LIN.INVOICE_NUMBER,
                                   (LIN.AMOUNT * LIN.QUANTITY) AS INVOICE_AMOUNT,
                                   LIN.QUANTITY,
                                   LIN.INVOICE_CURRENCY,
                                   LL.CREDIT_DAYS,
                                   (LIN.INVOICE_DATE + LL.CREDIT_DAYS) AS DUE_DATE,
                                   FN_FETCH_GROUP_DESC ('{_workContext.CurrentUserinformation.company_code}', 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE)
                                      AS PARENT_NAME
                              FROM LC_ITEM LI
                                   JOIN IP_ITEM_SPEC_SETUP IIS
                                      ON LI.ITEM_CODE = IIS.ITEM_CODE AND LI.COMPANY_CODE = IIS.COMPANY_CODE
                                   LEFT JOIN LC_INVOICE LIN
                                      ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO
                                         AND LI.ITEM_CODE = LIN.ITEM_CODE
                                            AND LI.SNO = LIN.SNO
                                   JOIN LC_LOC LL
                                      ON LI.LC_TRACK_NO = LL.LC_TRACK_NO
                                   JOIN IP_ITEM_MASTER_SETUP IMS
                                      ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
                             WHERE LI.LC_TRACK_NO = '{lctrackno}' AND LI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                             ORDER BY LIN.INVOICE_DATE, LIN.INVOICE_NUMBER";

            var lclist = _dbContext.SqlQuery<LcStatusReportModels>(sqlquery).ToList();
            return lclist;
        }

        public List<VehicleMovementReportModels> GetAllVehicleMovementReports(ReportFiltersModel reportFilters)
        {
            try
            {
                var sqlquery = $@"SELECT DISTINCT LLD.LC_TRACK_NO
	                                                ,LLD.INVOICE_NO
	                                                ,TO_CHAR(LI.INVOICE_DATE, 'MM/DD/YYYY') AS INVOICE_DATE
	                                                ,LI.LC_NUMBER
	                                                ,LLS.LOCATION_EDESC AS FROM_LOCATION_EDESC
	                                                ,TOLLS.LOCATION_EDESC AS TO_LOCATION_EDESC
	                                                ,TO_CHAR(LLD.SRC_ETA, 'MM/DD/YYYY') AS SRC_ETA
	                                                ,TO_CHAR(LLD.SRC_ETD, 'MM/DD/YYYY') AS SRC_ETD
	                                                ,TO_CHAR(LLD.SRC_ATA, 'MM/DD/YYYY') AS SRC_ATA
	                                                ,TO_CHAR(LLD.SRC_ATD, 'MM/DD/YYYY') AS SRC_ATD
	                                                ,TO_CHAR(LLD.SRC_ETD_DES, 'MM/DD/YYYY') AS SRC_ETD_DES
	                                                ,TO_CHAR(LLD.DES_ETA, 'MM/DD/YYYY') AS DES_ETA
	                                                ,TO_CHAR(LLD.DES_ETD, 'MM/DD/YYYY') AS DES_ETD
	                                                ,TO_CHAR(LLD.DES_ATA, 'MM/DD/YYYY') AS DES_ATA
	                                                ,TO_CHAR(LLD.DES_ATD, 'MM/DD/YYYY') AS DES_ATD
	                                                ,TO_CHAR(LLD.DES_ETD_NEXT_DES, 'MM/DD/YYYY') AS DES_ETD_NEXT_DES
                                                FROM LC_LOGISTIC_DETAIL LLD
                                                INNER JOIN LC_LOCATION_SETUP LLS ON LLD.FROM_LOCATION_CODE = LLS.LOCATION_CODE
                                                INNER JOIN LC_LOCATION_SETUP TOLLS ON LLD.TO_LOCATION_CODE = TOLLS.LOCATION_CODE
                                                INNER JOIN LC_INVOICE LI ON LLD.LC_TRACK_NO = LI.LC_TRACK_NO
	                                                AND LLD.INVOICE_NO = LI.INVOICE_NUMBER
                                                    AND LLD.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

                if (!string.IsNullOrEmpty(reportFilters.FromDate))
                    sqlquery = sqlquery + " WHERE LI.INVOICE_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

                //if (reportFilters.LcStatusFilter.Count > 0)
                //    sqlquery = sqlquery + " AND LL.STATUS_CODE IN('" + string.Join("','", reportFilters.LcStatusFilter).ToString() + "')";

                if (reportFilters.CompanyFilter.Count > 0)
                    sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";
                if (reportFilters.InvoiceNumberFilter.Count > 0)
                    sqlquery = sqlquery + " AND LLD.INVOICE_NO IN('" + string.Join("','", reportFilters.InvoiceNumberFilter).ToString() + "')";

                if (reportFilters.LcLocationFilter.Count > 0)
                {
                    if (reportFilters.LcLocationFilter.Count == 1)
                    {
                        reportFilters.LcLocationFilter.Add("");
                        sqlquery = sqlquery + $@"AND LLD.TO_LOCATION_CODE =" + reportFilters.LcLocationFilter[0] + " OR LLD.FROM_LOCATION_CODE =" + reportFilters.LcLocationFilter[0] + "";
                    }
                    else
                    {
                        sqlquery = sqlquery + " AND LLD.FROM_LOCATION_CODE IN(" + reportFilters.LcLocationFilter[0] + ") AND LLD.TO_LOCATION_CODE= " + reportFilters.LcLocationFilter[1] + "";
                    }

                }
                var lclist = _dbContext.SqlQuery<VehicleMovementReportModels>(sqlquery).ToList();
                return lclist;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public List<VehicleMovementReportModels> GetAllURVehicleMovementReports(ReportFiltersModel reportFilters)
        {
            try
            {
                var sqlquery = $@"SELECT DISTINCT LLD.LC_TRACK_NO
	                                                ,LLD.INVOICE_NO
	                                                ,TO_CHAR(LI.INVOICE_DATE, 'MM/DD/YYYY') AS INVOICE_DATE
	                                                ,LI.LC_NUMBER
	                                                ,LLS.LOCATION_EDESC AS FROM_LOCATION_EDESC
	                                                ,TOLLS.LOCATION_EDESC AS TO_LOCATION_EDESC
	                                                ,TO_CHAR(LLD.SRC_ETA, 'MM/DD/YYYY') AS SRC_ETA
	                                                ,TO_CHAR(LLD.SRC_ETD, 'MM/DD/YYYY') AS SRC_ETD
	                                                ,TO_CHAR(LLD.SRC_ATA, 'MM/DD/YYYY') AS SRC_ATA
	                                                ,TO_CHAR(LLD.SRC_ATD, 'MM/DD/YYYY') AS SRC_ATD
	                                                ,TO_CHAR(LLD.SRC_ETD_DES, 'MM/DD/YYYY') AS SRC_ETD_DES
	                                                ,TO_CHAR(LLD.DES_ETA, 'MM/DD/YYYY') AS DES_ETA
	                                                ,TO_CHAR(LLD.DES_ETD, 'MM/DD/YYYY') AS DES_ETD
	                                                ,TO_CHAR(LLD.DES_ATA, 'MM/DD/YYYY') AS DES_ATA
	                                                ,TO_CHAR(LLD.DES_ATD, 'MM/DD/YYYY') AS DES_ATD
	                                                ,TO_CHAR(LLD.DES_ETD_NEXT_DES, 'MM/DD/YYYY') AS DES_ETD_NEXT_DES
                                                FROM LC_LOGISTIC_DETAIL LLD
                                                INNER JOIN LC_LOCATION_SETUP LLS ON LLD.FROM_LOCATION_CODE = LLS.LOCATION_CODE
                                                INNER JOIN LC_LOCATION_SETUP TOLLS ON LLD.TO_LOCATION_CODE = TOLLS.LOCATION_CODE
                                                INNER JOIN LC_INVOICE LI ON LLD.LC_TRACK_NO = LI.LC_TRACK_NO
                                                    AND DES_ATA IS NULL
	                                                AND LLD.INVOICE_NO = LI.INVOICE_NUMBER
                                                    AND LLD.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' ";

                if (!string.IsNullOrEmpty(reportFilters.FromDate))
                    sqlquery = sqlquery + " WHERE LI.INVOICE_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

                //if (reportFilters.LcStatusFilter.Count > 0)
                //    sqlquery = sqlquery + " AND LL.STATUS_CODE IN('" + string.Join("','", reportFilters.LcStatusFilter).ToString() + "')";

                if (reportFilters.CompanyFilter.Count > 0)
                    sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";
                if (reportFilters.InvoiceNumberFilter.Count > 0)
                    sqlquery = sqlquery + " AND LLD.INVOICE_NO IN('" + string.Join("','", reportFilters.InvoiceNumberFilter).ToString() + "')";

                if (reportFilters.LcLocationFilter.Count > 0)
                {
                    if (reportFilters.LcLocationFilter.Count == 1)
                    {
                        reportFilters.LcLocationFilter.Add("");
                        sqlquery = sqlquery + $@"AND LLD.TO_LOCATION_CODE =" + reportFilters.LcLocationFilter[0] + " OR LLD.FROM_LOCATION_CODE =" + reportFilters.LcLocationFilter[0] + "";
                    }
                    else
                    {
                        sqlquery = sqlquery + " AND LLD.FROM_LOCATION_CODE IN(" + reportFilters.LcLocationFilter[0] + ") AND LLD.TO_LOCATION_CODE= " + reportFilters.LcLocationFilter[1] + "";
                    }

                }
                var lclist = _dbContext.SqlQuery<VehicleMovementReportModels>(sqlquery).ToList();
                return lclist;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public List<VehicleMovementReportModels> GetAllVehicleChildStatus(string lctrackno, string cinumber)
        {
            var sqlquery = $@"
                        SELECT DISTINCT QUANTITY,FROM_LOCATION,TO_LOCATION,LC_TRACK_NO,EST_DAY,SHIPMENT_TYPE,LOAD_TYPE FROM LC_SHIPMENT WHERE LC_TRACK_NO = {lctrackno} AND INVOICE_NO = '{cinumber}' AND INVOICE_NO IS NOT NULL";

            var lclist = _dbContext.SqlQuery<VehicleMovementReportModels>(sqlquery).ToList();
            return lclist;
        }

        public List<ItemReportModels> GetAllLcChildPendingLc(string lcnumber, string brandname)
        {
            var sqlquery = $@"SELECT DISTINCT LI.LC_TRACK_NO,
                               LI.SNO,
                               LI.ITEM_CODE,
                               IMS.ITEM_EDESC,
                               LI.MU_CODE,
                               LI.QUANTITY,
                               LI.CURRENCY_CODE,
                               LI.AMOUNT,
                               LI.HS_CODE,
                               LI.COUNTRY_OF_ORIGIN,
                               CS.COUNTRY_EDESC,
                               IIS.BRAND_NAME
                          FROM LC_ITEM LI
                               LEFT JOIN IP_ITEM_MASTER_SETUP IMS
                                  ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
                               LEFT JOIN COUNTRY_SETUP CS
                                  ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE
                                    LEFT JOIN IP_ITEM_SPEC_SETUP IIS 
                                     ON LI.ITEM_CODE = IIS.ITEM_CODE AND LI.COMPANY_CODE = IIS.COMPANY_CODE
                         WHERE LI.LC_TRACK_NO = '{lcnumber}' AND LI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' ORDER BY LI.SNO";


            var lclist = _dbContext.SqlQuery<ItemReportModels>(sqlquery).ToList();
            return lclist;
        }

        public List<ItemReportModels> GetAllLcChildOpenLc(string lcnumber, string brandname)
        {
            if (brandname == "null")
            {
                brandname = "IIS.BRAND_NAME is null";
            }
            else
            {
                brandname = $@"IIS.BRAND_NAME = '{brandname}'";
            }
            var sqlquery = $@"SELECT DISTINCT LI.LC_TRACK_NO,
                               LI.SNO,
                               LI.ITEM_CODE,
                               IMS.ITEM_EDESC,
                               LI.MU_CODE,
                               LI.QUANTITY,
                               LI.CURRENCY_CODE,
                               LI.AMOUNT,
                               LI.HS_CODE,
                               LI.COUNTRY_OF_ORIGIN,
                               CS.COUNTRY_EDESC,
                               IIS.BRAND_NAME
                          FROM LC_ITEM LI
                               JOIN IP_ITEM_MASTER_SETUP IMS
                                  ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE  
                               JOIN COUNTRY_SETUP CS
                                  ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE
                                    JOIN IP_ITEM_SPEC_SETUP IIS 
                                     ON LI.ITEM_CODE = IIS.ITEM_CODE AND LI.COMPANY_CODE = IIS.COMPANY_CODE
                         WHERE LI.LC_TRACK_NO = '{lcnumber}' AND IMS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' ORDER BY LI.SNO";

            var lclist = _dbContext.SqlQuery<ItemReportModels>(sqlquery).ToList();
            return lclist;
        }

        public List<LcProductWiseReportModels> GetAllLcProductWiseReport(ReportFiltersModel reportFilters)
        {
            var sqlquery = $@"SELECT LL.LOC_CODE
	                                ,TO_CHAR(LL.OPEN_DATE,'MM/DD/YYYY') AS OPEN_DATE
	                                ,TO_CHAR(LL.EXPIRY_DATE,'MM/DD/YYYY') AS EXPIRY_DATE
	                                --,LL.LC_NUMBER
                                    ,ISS.SUPPLIER_EDESC AS LC_NUMBER   
	                                ,IMS.ITEM_EDESC
	                                ,FN_FETCH_PRE_DESC('{_workContext.CurrentUserinformation.company_code}', 'IP_ITEM_MASTER_SETUP', IMS.PRE_ITEM_CODE) PRE_DESC
	                                ,LI.ITEM_CODE
	                                ,NVL(LI.QUANTITY,0) AS QUANTITY
	                                ,TO_CHAR(NVL(LI.AMOUNT,0)) AS AMOUNT
	                                ,LI.CURRENCY_CODE
	                                ,TO_CHAR(LI.QUANTITY * LI.AMOUNT) AS TOTAL_AMOUNT
                                FROM LC_LOC LL
	                                ,LC_ITEM LI
	                                ,IP_ITEM_MASTER_SETUP IMS
                                    ,IP_SUPPLIER_SETUP ISS
                                WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO
	                                AND LL.COMPANY_CODE = LI.COMPANY_CODE
                                    --AND LL.LC_NUMBER = ISS.SUPPLIER_CODE
                                      AND LL.LC_NUMBER = ISS.SUPPLIER_EDESC
	                                AND LI.ITEM_CODE = IMS.ITEM_CODE
                                    AND IMS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";

            if (!string.IsNullOrEmpty(reportFilters.FromDate))
                sqlquery = sqlquery + " AND LL.OPEN_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";
            if (reportFilters.LcNumberFilter.Count > 0)
                sqlquery = sqlquery + " AND ISS.SUPPLIER_EDESC IN('" + string.Join("','", reportFilters.LcNumberFilter).ToString() + "')";
            if (reportFilters.ItemNameFilter.Count > 0)
                sqlquery = sqlquery + " AND IMS.ITEM_EDESC IN('" + string.Join("','", reportFilters.ItemNameFilter).ToString() + "')";
            sqlquery += "ORDER BY LL.OPEN_DATE DESC";
            //if (!string.IsNullOrEmpty(reportFilters.FromDate))
            //    sqlquery = sqlquery + " AND LL.OPEN_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

            //if (reportFilters.CompanyFilter.Count > 0)
            //    sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";

            //sqlquery += "GROUP BY LL.LC_TRACK_NO,LL.LC_NUMBER,LL.OPEN_DATE,LL.EXPIRY_DATE,LL.STATUS_CODE,LS.STATUS_EDESC,IPS.SUPPLIER_EDESC";

            //if (reportFilters.BrandFilter.Count > 0)
            //    sqlquery = sqlquery + " AND ISS.BRAND_NAME IN('" + string.Join("','", reportFilters.BrandFilter).ToString().ToUpper() + "')";


            var lclist = _dbContext.SqlQuery<LcProductWiseReportModels>(sqlquery).ToList();
            return lclist;
        }

        public List<PendingCommercialInvoiceReportModels> GetAllPendingCommercialInvoiceReport(ReportFiltersModel reportFilters)
        {
            var sqlquery = $@"SELECT ISS.SUPPLIER_EDESC AS LC_NUMBER
                                    ,LINV.INVOICE_NUMBER
                                    ,TO_CHAR(LINV.INVOICE_DATE,'MM-DD-YYYY') AS INVOICE_DATE
                                    ,TO_CHAR(SUM(LI.QUANTITY * LI.AMOUNT)) TOTAL_AMT
                                    ,LI.CURRENCY_CODE AS CURRENCY_CODE
                                    ,DECODE(LL.PTERMS_CODE,121, 'AT SIGHT',122, 'USANCE') AS PTERMS
                                    ,TO_CHAR(LL.CREDIT_DAYS) AS CREDIT_DAYS,
                                    TO_CHAR(LINV.INVOICE_DATE+LL.CREDIT_DAYS,'MM-DD-YYYY') AS DUE_DATE
                                    ,LB.BNF_EDESC AS BENIFICARY
                                    ,FBS.BANK_NAME AS ISSUING_BANK
                                    FROM LC_LOC LL
                                    ,LC_BENEFICIARY LB
                                    ,LC_PERFOMA_INVOICE LPI
                                    ,LC_ITEM LI
                                    ,IP_ITEM_MASTER_SETUP IMS
                                    ,LC_INVOICE LINV
                                    ,IP_SUPPLIER_SETUP ISS
                                   , LC_PAYMENT_SETTLEMENT LPS
                                    ,FA_BANK_SETUP FBS
                                WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO
                                     AND LINV.LC_TRACK_NO = LI.LC_TRACK_NO
                                    AND LI.ITEM_CODE = IMS.ITEM_CODE
                                    AND LI.COMPANY_CODE = IMS.COMPANY_CODE
                                    AND LL.LC_NUMBER = ISS.SUPPLIER_CODE
                                    AND LI.ITEM_CODE = LINV.ITEM_CODE(+)
                                    AND LPS.INVOICE_CODE <> LINV.INVOICE_CODE
                                    AND LPI.PINVOICE_CODE = LL.PINVOICE_CODE
                                    AND LPI.BNF_CODE=LB.BNF_CODE
                                    AND FBS.BANK_CODE(+)=LL.ISSUING_BANK_CODE";

            if (!string.IsNullOrEmpty(reportFilters.FromDate))
                sqlquery = sqlquery + " AND LL.CREATED_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";
            if (reportFilters.LcNumberFilter.Count > 0)
                sqlquery = sqlquery + " AND ISS.SUPPLIER_EDESC IN('" + string.Join("','", reportFilters.LcNumberFilter).ToString() + "')";
            //if (reportFilters.LcStatusFilter.Count > 0)
            //    sqlquery = sqlquery + " AND LL.STATUS_CODE IN('" + string.Join("','", reportFilters.LcStatusFilter).ToString() + "')";
            //if (!string.IsNullOrEmpty(reportFilters.FromDate))
            //    sqlquery = sqlquery + " AND LL.OPEN_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

            //if (reportFilters.CompanyFilter.Count > 0)
            //    sqlquery = sqlquery + " AND LL.COMPANY_CODE IN('" + string.Join("','", reportFilters.CompanyFilter).ToString() + "')";

            sqlquery += " GROUP BY ISS.SUPPLIER_EDESC ,LINV.INVOICE_NUMBER ,LI.ITEM_CODE ,IMS.ITEM_EDESC ,LI.CURRENCY_CODE,LINV.INVOICE_NUMBER,LINV.INVOICE_DATE,LL.PTERMS_CODE,LL.CREDIT_DAYS,LB.BNF_EDESC,FBS.BANK_NAME";

            //if (reportFilters.BrandFilter.Count > 0)
            //    sqlquery = sqlquery + " AND ISS.BRAND_NAME IN('" + string.Join("','", reportFilters.BrandFilter).ToString().ToUpper() + "')";
            var lclist = _dbContext.SqlQuery<PendingCommercialInvoiceReportModels>(sqlquery).ToList();
            return lclist;
        }

        public List<POPendingReportModel> POPendingReport(ReportFiltersModel reportFilters)
        {
            var COMPANY_CODE = _workContext.CurrentUserinformation.company_code;
            var BRANCH_CODE = _workContext.CurrentUserinformation.branch_code;
            List<POPendingReportModel> Record = new List<POPendingReportModel>();

            #region SELECT PIVOT DATA FROM  TEMPTABLE
            string SELECTQUERY = $@"SELECT TO_CHAR(LC_NUMBER) AS LC_NUMBER,ITEM_EDESC AS ITEM_DESC, TO_CHAR(CREATED_DATE,'DD-MM-YYYY') AS CREATED_DATE,TO_CHAR(PROCESS_1) AS TOTAL_ITEM,TO_CHAR(NVL(PROCESS_1,0)-(NVL(PROCESS_2,0)+NVL(PROCESS_3,0)+NVL(PROCESS_4,0))) AS PROCESS_1,TO_CHAR(PROCESS_2) AS PROCESS_2,TO_CHAR(PROCESS_3) AS PROCESS_3,TO_CHAR(PROCESS_4) AS PROCESS_4 FROM
                     (
                     SELECT TEMPTABLE.LC_NUMBER,TEMPTABLE.ITEM_EDESC,TEMPTABLE.CREATED_DATE,SUM(TEMPTABLE.QUANTITY) AS QUANTITY , TEMPTABLE.PROCESSNO FROM    ( SELECT  LC_NUMBER,ITEM_EDESC,QUANTITY,PROCESSNO,CREATED_DATE FROM ( SELECT  NVL(ISS.SUPPLIER_EDESC, LL.LC_NUMBER) AS LC_NUMBER,IM.ITEM_CODE,IM.ITEM_EDESC AS ITEM_EDESC, LI.QUANTITY  ,'NO CI' AS PROCESS, 1 AS PROCESSNO,TO_DATE(LL.CREATED_DATE) AS CREATED_DATE   FROM LC_ITEM LI, LC_PERFOMA_INVOICE PO,  LC_LOC LL,IP_ITEM_MASTER_SETUP IM,IP_SUPPLIER_SETUP ISS
                     WHERE PO.LC_TRACK_NO=LI.LC_TRACK_NO 
                    -- AND PO.LC_TRACK_NO NOT IN (SELECT DISTINCT LINV.LC_TRACK_NO FROM  LC_INVOICE LINV)
                     AND LI.ITEM_CODE= IM.ITEM_CODE
                     AND LI.COMPANY_CODE =IM.COMPANY_CODE
                     AND PO.LC_TRACK_NO =LL.LC_TRACK_NO
                     AND PO.COMPANY_CODE =LL.COMPANY_CODE
                     AND LL.LC_NUMBER = ISS.SUPPLIER_CODE(+)   
                     UNION
                     SELECT  T.LC_NUMBER,T.ITEM_CODE ,T.ITEM_EDESC, T.QUANTITY,'TRANSIT' AS PROCESS, 2 AS PROCESSNO,T.CREATED_DATE  FROM    ( SELECT  NVL(ISS.SUPPLIER_EDESC, LINV.LC_NUMBER) AS LC_NUMBER,LINV.ITEM_CODE,IM.ITEM_EDESC ,SUM(LINV.QUANTITY) AS QUANTITY,LL.CREATED_DATE
                     FROM  LC_INVOICE LINV ,IP_ITEM_MASTER_SETUP IM,IP_SUPPLIER_SETUP ISS,LC_LOC LL 
                     WHERE  LINV.INVOICE_NUMBER NOT IN ( SELECT INVOICE_NO FROM LC_LOGISTIC_DETAIL)
                     AND IM.ITEM_CODE=LINV.ITEM_CODE
                     AND IM.COMPANY_CODE =LINV.COMPANY_CODE
                     AND LINV.LC_NUMBER = ISS.SUPPLIER_CODE(+)
                     AND LL.LC_TRACK_NO=LINV.LC_TRACK_NO 
                     GROUP BY ISS.SUPPLIER_EDESC,LINV.LC_NUMBER, LINV.ITEM_CODE,IM.ITEM_EDESC,LL.CREATED_DATE)T
                     UNION
                     SELECT NVL(ISS.SUPPLIER_EDESC, LL.LC_NUMBER) AS LC_NUMBER,IM.ITEM_CODE ,IM.ITEM_EDESC, LINV.QUANTITY,'CI WITH ATA NO PP NO' AS PROCESS, 3 AS PROCESSNO,TO_DATE(LL.CREATED_DATE) AS CREATED_DATE  FROM  LC_LOC LL, LC_INVOICE LINV ,IP_ITEM_MASTER_SETUP IM,IP_SUPPLIER_SETUP ISS,LC_ITEM LI WHERE  LINV.INVOICE_NUMBER IN ( SELECT INVOICE_NO FROM LC_LOGISTIC_DETAIL)
                     AND LL.LC_TRACK_NO=LINV.LC_TRACK_NO
                     AND IM.ITEM_CODE=LINV.ITEM_CODE
                     AND IM.COMPANY_CODE =LINV.COMPANY_CODE
                     AND LI.ITEM_CODE =LINV.ITEM_CODE
                     AND LI.COMPANY_CODE =LINV.COMPANY_CODE
                     AND LL.LC_NUMBER = ISS.SUPPLIER_CODE(+)
                     AND LINV.PP_NO IS  NULL       
                     UNION
                     SELECT  NVL(ISS.SUPPLIER_EDESC, LL.LC_NUMBER) AS LC_NUMBER,IM.ITEM_CODE ,IM.ITEM_EDESC,LINV.QUANTITY, 'CI WITH ATA AND PP NO AND GRN' AS PROCESS,4 AS PROCESSNO ,TO_DATE(LL.CREATED_DATE) AS CREATED_DATE   FROM LC_LOC LL, LC_INVOICE LINV ,IP_ITEM_MASTER_SETUP IM,IP_SUPPLIER_SETUP ISS,LC_ITEM LI 
                     WHERE LINV.INVOICE_CODE IN (SELECT DISTINCT LG.INVOICE_CODE  FROM LC_GRN LG) AND LINV.PP_NO IS NOT NULL
                     AND LL.LC_TRACK_NO =LINV.LC_TRACK_NO
                     AND LINV.ITEM_CODE=IM.ITEM_CODE
                     AND LINV.COMPANY_CODE=IM.COMPANY_CODE
                     AND LI.ITEM_CODE =LINV.ITEM_CODE
                     AND LI.COMPANY_CODE =LINV.COMPANY_CODE
                     AND LL.LC_NUMBER = ISS.SUPPLIER_CODE(+) 
                     ORDER BY LC_NUMBER,PROCESSNO)) TEMPTABLE
                      group by TEMPTABLE.LC_NUMBER,TEMPTABLE.ITEM_EDESC,TEMPTABLE.CREATED_DATE,TEMPTABLE.PROCESSNO
                      )
                     PIVOT 
                      (
                        MAX(QUANTITY)
                        FOR PROCESSNO
                        IN (1 AS PROCESS_1,2 AS PROCESS_2,3 AS PROCESS_3,4 AS PROCESS_4)
                       )PIV";
            if (!string.IsNullOrEmpty(reportFilters.FromDate))
                SELECTQUERY = SELECTQUERY + " WHERE CREATED_DATE BETWEEN TO_DATE('" + reportFilters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE('" + reportFilters.ToDate + "', 'YYYY-MM-DD')";

            if (reportFilters.LcNumberFilter.Count > 0)
                SELECTQUERY = SELECTQUERY + " AND LC_NUMBER IN('" + string.Join("','", reportFilters.LcNumberFilter).ToString() + "')";
            //order by TO_DATE(PIV.CREATED_DATE, 'DD-MM-YYYY') DESC 
            SELECTQUERY = SELECTQUERY + " ORDER BY TO_DATE(PIV.CREATED_DATE, 'DD-MM-YYYY') DESC ";
            Record = _dbContext.SqlQuery<POPendingReportModel>(SELECTQUERY).ToList();
            #endregion

            return Record;
        }
        public List<PendingCIReportViewModel> GetPendingCIReports(ReportFiltersModel reportfilters)
        {
            var query = string.Empty;
            var pendingCIData = new List<PendingCIReportViewModel>();
            if (reportfilters.LcNumberFilter.Count > 0)
            {
                query = $@"SELECT LL.LC_NUMBER LcNumber
	                                ,LI.ITEM_CODE ItemCode
	                                ,IMS.ITEM_EDESC ItemName
	                                ,SUM(LI.QUANTITY) TotalQuantity
	                                ,NVL(SUM(LI.QUANTITY) - SUM(LINV.QUANTITY), 0) BalanceQuantity   
	                                ,LI.CURRENCY_CODE CurrencyCode
	                                ,SUM(LI.QUANTITY * LI.AMOUNT) TotalAmount
	                                ,NVL(SUM(LI.QUANTITY * LI.AMOUNT) - SUM(LINV.QUANTITY * LINV.AMOUNT), 0)  BalanceAmount
                                FROM LC_LOC LL
	                                ,LC_ITEM LI
	                                ,IP_ITEM_MASTER_SETUP IMS
	                                ,LC_INVOICE LINV
                                WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO
	                                AND LI.ITEM_CODE = IMS.ITEM_CODE
	                                AND LI.COMPANY_CODE = IMS.COMPANY_CODE
	                                AND LL.LC_NUMBER IN ('{string.Join("','", reportfilters.LcNumberFilter)}')
	                                AND LI.ITEM_CODE = LINV.ITEM_CODE(+)
                                GROUP BY LL.LC_NUMBER
	                                ,LI.ITEM_CODE
	                                ,IMS.ITEM_EDESC
	                                ,LI.CURRENCY_CODE";
                query = string.Format(query);
            }
            else
            {
                query = $@"SELECT LL.LC_NUMBER LcNumber
	                                ,LI.ITEM_CODE ItemCode
	                                ,IMS.ITEM_EDESC ItemName
	                                ,SUM(LI.QUANTITY) TotalQuantity
	                                ,NVL(SUM(LI.QUANTITY) - SUM(LINV.QUANTITY), 0) BalanceQuantity
	                                ,LI.CURRENCY_CODE CurrencyCode
	                                ,SUM(LI.QUANTITY * LI.AMOUNT) TotalAmount
	                                ,NVL(SUM(LI.QUANTITY * LI.AMOUNT) - SUM(LINV.QUANTITY * LINV.AMOUNT), 0) BalanceAmount
                                FROM LC_LOC LL
	                                ,LC_ITEM LI
	                                ,IP_ITEM_MASTER_SETUP IMS
	                                ,LC_INVOICE LINV
                                WHERE LL.LC_TRACK_NO = LI.LC_TRACK_NO
	                                AND LI.ITEM_CODE = IMS.ITEM_CODE
	                                AND LI.COMPANY_CODE = IMS.COMPANY_CODE
	                                AND LL.LC_NUMBER =  LL.LC_NUMBER
	                                AND LI.ITEM_CODE = LINV.ITEM_CODE(+)
                                GROUP BY LL.LC_NUMBER
	                                ,LI.ITEM_CODE
	                                ,IMS.ITEM_EDESC
	                                ,LI.CURRENCY_CODE";
                query = string.Format(query);
            }
            pendingCIData = this._dbContext.SqlQuery<PendingCIReportViewModel>(query).ToList();
            return pendingCIData;
        }

        public List<ExchangeGainLossReportViewModel> GetExchangeGainLossReports(ReportFiltersModel reportfilters)
        {
            var query = string.Empty;
            var exchangeGainLossData = new List<ExchangeGainLossReportViewModel>();
            if (reportfilters.CurrencyFilter.Count > 0)
            {
                query = $@"SELECT INVOICE_DATE BillDate
	                                ,INVOICE_CURRENCY || ' ' || SUM(AMOUNT) CommercialInvoiceAmount
	                                ,EXCHANGE_RATE PaymentRate
	                                ,{reportfilters.ExchangeRate} SalesRate
	                                ,SUM(AMOUNT) * ({reportfilters.ExchangeRate} - EXCHANGE_RATE) ExchangeGainLoss
                                FROM LC_INVOICE
                                WHERE INVOICE_DATE >   TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD')
	                                AND INVOICE_DATE <=  TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD')
	                                AND INVOICE_CURRENCY IN ('{string.Join("','", reportfilters.CurrencyFilter)}')
                                GROUP BY INVOICE_DATE
	                                ,EXCHANGE_RATE
	                                ,INVOICE_CURRENCY
                                ORDER BY INVOICE_DATE";
                query = string.Format(query, reportfilters.FromDate, reportfilters.ToDate, reportfilters.LcNumberFilter);
            }
            else
            {
                query = $@"SELECT   INVOICE_DATE BillDate
	                                ,INVOICE_CURRENCY || ' ' || SUM(AMOUNT) CommercialInvoiceAmount
	                                ,EXCHANGE_RATE PaymentRate
	                                ,{reportfilters.ExchangeRate} SalesRate
	                                ,SUM(AMOUNT) * {reportfilters.ExchangeRate} -(EXCHANGE_RATE) ExchangeGainLoss
                                FROM LC_INVOICE
                                WHERE INVOICE_DATE > TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD')
	                                AND INVOICE_DATE <= TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD')
	                                AND INVOICE_CURRENCY =INVOICE_CURRENCY
                                GROUP BY INVOICE_DATE
	                                ,EXCHANGE_RATE
	                                ,INVOICE_CURRENCY
                                ORDER BY INVOICE_DATE";
                query = string.Format(query);
            }
            exchangeGainLossData = this._dbContext.SqlQuery<ExchangeGainLossReportViewModel>(query).ToList();
            return exchangeGainLossData;
        }
        public List<ExgGainLossReportVModel> GetExgGLReports(ReportFiltersModel reportfilters)
        {
            var query = string.Empty;
            var exchangeGainLossData = new List<ExgGainLossReportVModel>();
            // Animesh
            //If in case the duplicate data is shown or if more than reuired data is shown of same invoice add (LI.ITEM_CODE=LD.ITEM_CODE) condition 
            if (reportfilters.CurrencyFilter.Count > 0 && reportfilters.ProductFilter.Count > 0)
            {
                query = $@"SELECT INVOICE_DATE, INVOICE_NUMBER, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.AMOUNT * LI.QUANTITY AMT, LI.INVOICE_CURRENCY,
EXCHANGE_RATE PAYMENT_EXCHANGE_RATE, SALES_EXG_RATE SELLING_EXCHANGE_RATE, 
(NVL( EXCHANGE_RATE ,0)-NVL(SALES_EXG_RATE,0))* LI.AMOUNT * LI.QUANTITY EXNG_GAIN_LOSS,TO_CHAR(LI.PAYMENT_DATE,'MM-DD-YYYY') AS PAYMENT_DATE FROM LC_INVOICE LI, IP_ITEM_MASTER_SETUP IMS 
WHERE LI.DELETED_FLAG = 'N' AND LI.ITEM_CODE= IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE AND 
LI.INVOICE_CURRENCY IN ('{string.Join("','", reportfilters.CurrencyFilter)}')  AND LI.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND LI.DELETED_FLAG='N' AND 
LI.PAYMENT_DATE > =TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD') AND LI.PAYMENT_DATE <TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD') 
AND IMS.ITEM_CODE IN ('{string.Join("','", reportfilters.ProductFilter)}') order by PAYMENT_DATE asc";
                query = string.Format(query, reportfilters.FromDate, reportfilters.ToDate, reportfilters.ProductFilter);
            }

            else if (reportfilters.CurrencyFilter.Count > 0)
            {
                query = $@"SELECT INVOICE_DATE, INVOICE_NUMBER, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.AMOUNT * LI.QUANTITY AMT, LI.INVOICE_CURRENCY,
EXCHANGE_RATE PAYMENT_EXCHANGE_RATE, SALES_EXG_RATE SELLING_EXCHANGE_RATE, 
(NVL( EXCHANGE_RATE ,0)-NVL(SALES_EXG_RATE,0))* LI.AMOUNT * LI.QUANTITY EXNG_GAIN_LOSS,TO_CHAR(LI.PAYMENT_DATE,'MM-DD-YYYY') AS PAYMENT_DATE FROM LC_INVOICE LI, IP_ITEM_MASTER_SETUP IMS 
WHERE LI.DELETED_FLAG = 'N' AND LI.ITEM_CODE= IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE AND 
LI.INVOICE_CURRENCY IN ('{string.Join("','", reportfilters.CurrencyFilter)}') AND LI.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND LI.DELETED_FLAG='N' AND 
LI.PAYMENT_DATE > =TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD') AND LI.PAYMENT_DATE <TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD') order by PAYMENT_DATE asc";
                query = string.Format(query, reportfilters.FromDate, reportfilters.ToDate, reportfilters.LcNumberFilter);
            }
            else if (reportfilters.ProductFilter.Count > 0)
            {
                query = $@"SELECT INVOICE_DATE, INVOICE_NUMBER, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.AMOUNT * LI.QUANTITY AMT, LI.INVOICE_CURRENCY,
EXCHANGE_RATE PAYMENT_EXCHANGE_RATE, SALES_EXG_RATE SELLING_EXCHANGE_RATE, 
(NVL( EXCHANGE_RATE ,0)-NVL(SALES_EXG_RATE,0))* LI.AMOUNT * LI.QUANTITY EXNG_GAIN_LOSS,TO_CHAR(LI.PAYMENT_DATE,'MM-DD-YYYY') AS PAYMENT_DATE FROM LC_INVOICE LI, IP_ITEM_MASTER_SETUP IMS 
WHERE LI.DELETED_FLAG = 'N' AND LI.ITEM_CODE= IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE  AND LI.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND LI.DELETED_FLAG='N' AND 
LI.PAYMENT_DATE > =TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD') AND LI.PAYMENT_DATE <TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD') 
AND IMS.ITEM_CODE IN ('{string.Join("','", reportfilters.ProductFilter)}') order by PAYMENT_DATE asc";
                query = string.Format(query, reportfilters.FromDate, reportfilters.ToDate, reportfilters.ProductFilter);
            }
            else
            {
                query = $@"SELECT INVOICE_DATE, INVOICE_NUMBER, LI.ITEM_CODE, IMS.ITEM_EDESC, LI.AMOUNT * LI.QUANTITY AMT, LI.INVOICE_CURRENCY,
EXCHANGE_RATE PAYMENT_EXCHANGE_RATE, SALES_EXG_RATE SELLING_EXCHANGE_RATE, 
(NVL( EXCHANGE_RATE ,0)-NVL(SALES_EXG_RATE,0))* LI.AMOUNT * LI.QUANTITY EXNG_GAIN_LOSS,TO_CHAR(LI.PAYMENT_DATE,'MM-DD-YYYY') AS PAYMENT_DATE FROM LC_INVOICE LI, IP_ITEM_MASTER_SETUP IMS 
WHERE LI.DELETED_FLAG = 'N' AND LI.ITEM_CODE= IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE  AND LI.COMPANY_CODE = {_workContext.CurrentUserinformation.company_code} AND LI.DELETED_FLAG='N' AND 
LI.PAYMENT_DATE > =TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD') AND LI.PAYMENT_DATE <TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD') order by PAYMENT_DATE asc";
                query = string.Format(query);
            }
            exchangeGainLossData = this._dbContext.SqlQuery<ExgGainLossReportVModel>(query).ToList();
            return exchangeGainLossData;
        }

        public List<MITReportViewModel> GetmitReports(ReportFiltersModel reportfilters)
        {
            var query = string.Empty;
            var mitData = new List<MITReportViewModel>();
            if (reportfilters.LcNumberFilter.Count > 0 && reportfilters.ProductFilter.Count > 0)
            {
                query = $@"SELECT DISTINCT  PO.ORDER_DATE, PO.ORDER_NO, PO.LC_TRACK_NO, LIN.LC_NUMBER WEEK_LC_NO, LD.DO_NUMBER DO_NO, LIN.INVOICE_NUMBER CI_NO, IMS.ITEM_CODE, 
IMS.ITEM_EDESC, LI.QUANTITY ORDER_QTY, LIN.QUANTITY IN_QUANTITY, (LI.QUANTITY- LIN.QUANTITY) PENDING_PO_QTY, LIN.QUANTITY- NVL(RD.REFERENCE_QUANTITY,0) MIT, LIN.QUANTITY- NVL(RD.REFERENCE_QUANTITY,0) AT_PORT,
TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) LC_WEEK, TO_NUMBER(TO_CHAR(SYSDATE,'WW')) CURRENT_WEEK, 
(CASE WHEN (TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2))) >0 
THEN TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) 
ELSE 52+TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) END ) DUE,
'' ATA, '' AWB_DATE FROM LC_PURCHASE_ORDER PO, LC_ITEM LI, LC_INVOICE LIN, IP_ITEM_MASTER_SETUP IMS, LC_DO LD, REFERENCE_DETAIL RD 
WHERE PO.LC_TRACK_NO = LI.LC_TRACK_NO AND PO.LC_TRACK_NO = LIN.LC_TRACK_NO(+) AND LI.ITEM_CODE = IMS.ITEM_CODE 
AND PO.LC_TRACK_NO = LD.LC_TRACK_NO AND PO.COMPANY_CODE = LD.COMPANY_CODE AND LI.ITEM_CODE = LIN.ITEM_CODE 
AND LIN.INVOICE_CODE = LD.INVOICE_CODE AND PO.COMPANY_CODE = LI.COMPANY_CODE AND PO.COMPANY_CODE = LIN.COMPANY_CODE(+) 
AND LI.COMPANY_CODE = IMS.COMPANY_CODE AND LIN.INVOICE_NUMBER = RD.REFERENCE_NO(+) AND LIN.ITEM_CODE = RD.REFERENCE_ITEM_CODE(+) 
AND LIN.COMPANY_CODE = RD.COMPANY_CODE(+)  AND LIN.LC_NUMBER IN ('{string.Join("','", reportfilters.LcNumberFilter)}') AND  LIN.LC_NUMBER LIKE 'W%' AND LI.ITEM_CODE  IN ('{string.Join("','", reportfilters.ProductFilter)}')
AND LIN.DELETED_FLAG='N'  AND PO.ORDER_DATE > TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD')
    AND PO.ORDER_DATE <= TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD') ORDER BY PO.LC_TRACK_NO, IMS.ITEM_CODE ASC";
                query = string.Format(query, reportfilters.LcNumberFilter, reportfilters.ProductFilter);
            }
           else if (reportfilters.LcNumberFilter.Count > 0)
            {
                query = $@"SELECT DISTINCT PO.ORDER_DATE, PO.ORDER_NO, PO.LC_TRACK_NO, LIN.LC_NUMBER WEEK_LC_NO, LD.DO_NUMBER DO_NO, LIN.INVOICE_NUMBER CI_NO, IMS.ITEM_CODE, 
IMS.ITEM_EDESC, LI.QUANTITY ORDER_QTY, LIN.QUANTITY IN_QUANTITY, (LI.QUANTITY- LIN.QUANTITY) PENDING_PO_QTY, LIN.QUANTITY- NVL(RD.REFERENCE_QUANTITY,0) MIT,  LIN.QUANTITY- NVL(RD.REFERENCE_QUANTITY,0) AT_PORT,
TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) LC_WEEK, TO_NUMBER(TO_CHAR(SYSDATE,'WW')) CURRENT_WEEK, 
(CASE WHEN (TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2))) >0 
THEN TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) 
ELSE 52+TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) END ) DUE,
'' ATA, '' AWB_DATE FROM LC_PURCHASE_ORDER PO, LC_ITEM LI, LC_INVOICE LIN, IP_ITEM_MASTER_SETUP IMS, LC_DO LD, REFERENCE_DETAIL RD 
WHERE PO.LC_TRACK_NO = LI.LC_TRACK_NO AND PO.LC_TRACK_NO = LIN.LC_TRACK_NO(+) AND LI.ITEM_CODE = IMS.ITEM_CODE 
AND PO.LC_TRACK_NO = LD.LC_TRACK_NO AND PO.COMPANY_CODE = LD.COMPANY_CODE AND LI.ITEM_CODE = LIN.ITEM_CODE 
AND LIN.INVOICE_CODE = LD.INVOICE_CODE AND PO.COMPANY_CODE = LI.COMPANY_CODE AND PO.COMPANY_CODE = LIN.COMPANY_CODE(+) 
AND LI.COMPANY_CODE = IMS.COMPANY_CODE AND LIN.INVOICE_NUMBER = RD.REFERENCE_NO(+) AND LIN.ITEM_CODE = RD.REFERENCE_ITEM_CODE(+) 
AND LIN.COMPANY_CODE = RD.COMPANY_CODE(+)  AND LIN.LC_NUMBER IN ('{string.Join("','", reportfilters.LcNumberFilter)}') AND  LIN.LC_NUMBER LIKE 'W%'
AND LIN.DELETED_FLAG='N'  AND PO.ORDER_DATE > TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD')
    AND PO.ORDER_DATE <= TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD') ORDER BY PO.LC_TRACK_NO, IMS.ITEM_CODE ASC";
                query = string.Format(query, reportfilters.LcNumberFilter);
            }
            else if (reportfilters.ProductFilter.Count > 0)
            {
                query = $@"SELECT DISTINCT  PO.ORDER_DATE, PO.ORDER_NO, PO.LC_TRACK_NO, LIN.LC_NUMBER WEEK_LC_NO, LD.DO_NUMBER DO_NO, LIN.INVOICE_NUMBER CI_NO, IMS.ITEM_CODE, 
IMS.ITEM_EDESC, LI.QUANTITY ORDER_QTY, LIN.QUANTITY IN_QUANTITY, (LI.QUANTITY- LIN.QUANTITY) PENDING_PO_QTY, LIN.QUANTITY- NVL(RD.REFERENCE_QUANTITY,0) MIT, LIN.QUANTITY- NVL(RD.REFERENCE_QUANTITY,0) AT_PORT,
TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) LC_WEEK, TO_NUMBER(TO_CHAR(SYSDATE,'WW')) CURRENT_WEEK, 
(CASE WHEN (TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2))) >0 
THEN TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) 
ELSE 52+TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) END ) DUE,
'' ATA, '' AWB_DATE FROM LC_PURCHASE_ORDER PO, LC_ITEM LI, LC_INVOICE LIN, IP_ITEM_MASTER_SETUP IMS, LC_DO LD, REFERENCE_DETAIL RD 
WHERE PO.LC_TRACK_NO = LI.LC_TRACK_NO AND PO.LC_TRACK_NO = LIN.LC_TRACK_NO(+) AND LI.ITEM_CODE = IMS.ITEM_CODE 
AND PO.LC_TRACK_NO = LD.LC_TRACK_NO AND PO.COMPANY_CODE = LD.COMPANY_CODE AND LI.ITEM_CODE = LIN.ITEM_CODE 
AND LIN.INVOICE_CODE = LD.INVOICE_CODE AND PO.COMPANY_CODE = LI.COMPANY_CODE AND PO.COMPANY_CODE = LIN.COMPANY_CODE(+) 
AND LI.COMPANY_CODE = IMS.COMPANY_CODE AND LIN.INVOICE_NUMBER = RD.REFERENCE_NO(+) AND LIN.ITEM_CODE = RD.REFERENCE_ITEM_CODE(+) 
AND LIN.COMPANY_CODE = RD.COMPANY_CODE(+)  AND LIN.ITEM_CODE IN ('{string.Join("','", reportfilters.ProductFilter)}') AND  LIN.LC_NUMBER LIKE 'W%'
AND LIN.DELETED_FLAG='N'  AND PO.ORDER_DATE > TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD')
    AND PO.ORDER_DATE <= TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD') ORDER BY PO.LC_TRACK_NO, IMS.ITEM_CODE ASC";
                query = string.Format(query, reportfilters.ProductFilter);
            }
            else
            {

                
                query = $@"SELECT DISTINCT  PO.ORDER_DATE, PO.ORDER_NO, PO.LC_TRACK_NO, LIN.LC_NUMBER WEEK_LC_NO, LD.DO_NUMBER DO_NO, LIN.INVOICE_NUMBER CI_NO, IMS.ITEM_CODE, 
IMS.ITEM_EDESC, LI.QUANTITY ORDER_QTY, LIN.QUANTITY IN_QUANTITY, (LI.QUANTITY- LIN.QUANTITY) PENDING_PO_QTY, LIN.QUANTITY- NVL(RD.REFERENCE_QUANTITY,0) MIT,  LIN.QUANTITY- NVL(RD.REFERENCE_QUANTITY,0) AT_PORT,
TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) LC_WEEK, TO_NUMBER(TO_CHAR(SYSDATE,'WW')) CURRENT_WEEK, 
(CASE WHEN (TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2))) >0 
THEN TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) 
ELSE 52+TO_NUMBER(TO_CHAR(SYSDATE,'WW')) - TO_NUMBER(SUBSTR( LIN.LC_NUMBER,2,2)) END ) DUE,
'' ATA, '' AWB_DATE FROM LC_PURCHASE_ORDER PO, LC_ITEM LI, LC_INVOICE LIN, IP_ITEM_MASTER_SETUP IMS, LC_DO LD, REFERENCE_DETAIL RD 
WHERE PO.LC_TRACK_NO = LI.LC_TRACK_NO AND PO.LC_TRACK_NO = LIN.LC_TRACK_NO(+) AND LI.ITEM_CODE = IMS.ITEM_CODE 
AND PO.LC_TRACK_NO = LD.LC_TRACK_NO AND PO.COMPANY_CODE = LD.COMPANY_CODE AND LI.ITEM_CODE = LIN.ITEM_CODE 
AND LIN.INVOICE_CODE = LD.INVOICE_CODE AND PO.COMPANY_CODE = LI.COMPANY_CODE AND PO.COMPANY_CODE = LIN.COMPANY_CODE(+) 
AND LI.COMPANY_CODE = IMS.COMPANY_CODE AND LIN.INVOICE_NUMBER = RD.REFERENCE_NO(+) AND LIN.ITEM_CODE = RD.REFERENCE_ITEM_CODE(+) 
AND LIN.COMPANY_CODE = RD.COMPANY_CODE(+) AND LI.ITEM_CODE=LD.ITEM_CODE AND  LIN.LC_NUMBER LIKE 'W%'
AND LIN.DELETED_FLAG='N'  AND PO.ORDER_DATE > TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD')
    AND PO.ORDER_DATE <= TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD') ORDER BY PO.LC_TRACK_NO, IMS.ITEM_CODE ASC";
                query = string.Format(query);
            }

            mitData = this._dbContext.SqlQuery<MITReportViewModel>(query).ToList();
            
            var previousdata = new MITReportViewModel();

            
            for (int i = 0; i < mitData.Count; i++)
            {

                for(int j = i; j < mitData.Count; j++)
                {
                    if(j != i)
                    {
                        if(mitData[i].ORDER_NO==mitData[j].ORDER_NO)
                        {
                            if(mitData[i].ITEM_CODE==mitData[j].ITEM_CODE)
                            {
                                if(mitData[i].ORDER_QTY== mitData[j].ORDER_QTY)
                                {
                                    mitData[j].PENDING_PO_QTY = mitData[i].PENDING_PO_QTY - mitData[j].IN_QUANTITY;
                                    mitData[j].ORDER_QTY = mitData[i].PENDING_PO_QTY;

                                }
                            }
                        }
                    }
                }
                
            }
            //foreach (var data in mitData)
            //{
            //    if (count ==0)
            //    {
            //        previousdata = data;
            //        count++;
            //    }
            //    else
            //    {
            //        if (previousdata.ORDER_NO == data.ORDER_NO)
            //        {
            //            //if (previousdata.)
            //        }

            //    }
            //}
            return mitData;
        }
        public List<OptimizedPendingLcModel> GetAllOptimizedPendingLcReports(ReportFiltersModel reportfilters)
        {
            var query = string.Empty;
            var optimizedPendingLcData = new List<OptimizedPendingLcModel>();
            if (reportfilters.LcNumberFilter.Count > 0)
            {
                query = $@"SELECT --TO_CHAR(LL.OPEN_DATE, 'IW') WeekNumber
	                                LL.LC_NUMBER LcNumber
	                                ,LDO.DO_NUMBER DocumentOrderNumber
	                                ,LINV.INVOICE_NUMBER CommercialInvoice
	                                ,IMS.ITEM_EDESC ItemName
	                                ,SUM(LI.QUANTITY) OrderQunatity
	                                ,NVL(SUM(LI.QUANTITY) - SUM(LINV.QUANTITY), 0) PendingPurchaseOrder
	                                ,'' MIT
	                                ,'' ATPORT
	                                ,(TO_NUMBER(TO_CHAR(SYSDATE, 'IW')) - TO_NUMBER(TO_CHAR(LL.OPEN_DATE, 'IW'))) Due
	                                ,'' ATA
	                                ,'' AwbRecievedDate
                                    --,'Week '|| LPO.WEEK_NUMBER LcWeekNumber
                                    ,CASE WHEN LPO.WEEK_NUMBER IS NOT NULL THEN 'Week '|| LPO.WEEK_NUMBER
                                    ELSE LPO.WEEK_NUMBER
                                    END LcWeekNumber
                                FROM LC_LOC LL
	                                ,LC_ITEM LI
	                                ,IP_ITEM_MASTER_SETUP IMS
	                                ,LC_INVOICE LINV
	                                ,LC_DO LDO
                                    ,LC_PURCHASE_ORDER LPO
                                WHERE LL.OPEN_DATE > TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD')
	                                AND LL.OPEN_DATE <= TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD')
	                                AND LL.LC_TRACK_NO = LI.LC_TRACK_NO
	                                AND LL.LC_TRACK_NO = LINV.LC_TRACK_NO
	                                AND LL.LC_TRACK_NO = LDO.LC_TRACK_NO
	                                AND LI.ITEM_CODE = IMS.ITEM_CODE
	                                AND LINV.INVOICE_CODE = LDO.INVOICE_CODE
	                                AND LINV.ITEM_CODE = LDO.ITEM_CODE
	                                AND LI.COMPANY_CODE = IMS.COMPANY_CODE
	                                AND LL.LC_NUMBER in ('{string.Join("','", reportfilters.LcNumberFilter)}')
                                    AND LL.LC_TRACK_NO = LPO.LC_TRACK_NO
	                                AND LI.ITEM_CODE = LINV.ITEM_CODE(+)
                                GROUP BY TO_CHAR(LL.OPEN_DATE, 'IW')
	                                ,LL.LC_NUMBER
	                                ,LDO.DO_NUMBER
	                                ,LINV.INVOICE_NUMBER
	                                ,LI.ITEM_CODE
	                                ,IMS.ITEM_EDESC
	                                ,LI.CURRENCY_CODE
                                    ,LPO.WEEK_NUMBER
                                ORDER BY TO_CHAR(LL.OPEN_DATE, 'IW') DESC
	                                ,ITEM_EDESC";
                query = string.Format(query, reportfilters.FromDate, reportfilters.ToDate, reportfilters.LcNumberFilter);
            }
            else
            {
                query = $@"SELECT TO_CHAR(LL.OPEN_DATE, 'IW') WeekNumber
	                                ,LL.LC_NUMBER LcNumber
	                                ,LDO.DO_NUMBER DocumentOrderNumber
	                                ,LINV.INVOICE_NUMBER CommercialInvoice
	                                ,IMS.ITEM_EDESC ItemName
	                                ,SUM(LI.QUANTITY) OrderQunatity
	                                ,NVL(SUM(LI.QUANTITY) - SUM(LINV.QUANTITY), 0) PendingPurchaseOrder
	                                ,'' MIT
	                                ,'' ATPORT
	                                ,(TO_NUMBER(TO_CHAR(SYSDATE, 'IW')) - TO_NUMBER(TO_CHAR(LL.OPEN_DATE, 'IW'))) Due
	                                ,'' ATA
	                                ,'' AwbRecievedDate
                                    ,CASE WHEN LPO.WEEK_NUMBER IS NOT NULL THEN 'Week '|| LPO.WEEK_NUMBER
                                    ELSE LPO.WEEK_NUMBER
                                    END LcWeekNumber
                                    --,DECODE(LPO.WEEK_NUMBER,not null,'Week '|| LPO.WEEK_NUMBER,' ') LcWeekNumber
                                FROM LC_LOC LL
	                                ,LC_ITEM LI
	                                ,IP_ITEM_MASTER_SETUP IMS
	                                ,LC_INVOICE LINV
	                                ,LC_DO LDO
                                    ,LC_PURCHASE_ORDER LPO
                                WHERE LL.OPEN_DATE > TO_DATE ('{reportfilters.FromDate}','YYYY-MM-DD')
	                                AND LL.OPEN_DATE <= TO_DATE ('{reportfilters.ToDate}','YYYY-MM-DD')
	                                AND LL.LC_TRACK_NO = LI.LC_TRACK_NO
	                                AND LL.LC_TRACK_NO = LINV.LC_TRACK_NO
	                                AND LL.LC_TRACK_NO = LDO.LC_TRACK_NO
	                                AND LI.ITEM_CODE = IMS.ITEM_CODE
	                                AND LINV.INVOICE_CODE = LDO.INVOICE_CODE
	                                AND LINV.ITEM_CODE = LDO.ITEM_CODE
	                                AND LI.COMPANY_CODE = IMS.COMPANY_CODE
	                                AND LL.LC_NUMBER =LL.LC_NUMBER
                                    AND LL.LC_TRACK_NO = LPO.LC_TRACK_NO
	                                AND LI.ITEM_CODE = LINV.ITEM_CODE(+)
                                GROUP BY TO_CHAR(LL.OPEN_DATE, 'IW')
	                                ,LL.LC_NUMBER
	                                ,LDO.DO_NUMBER
	                                ,LINV.INVOICE_NUMBER
	                                ,LI.ITEM_CODE
	                                ,IMS.ITEM_EDESC
	                                ,LI.CURRENCY_CODE
                                    ,LPO.WEEK_NUMBER
                                ORDER BY TO_CHAR(LL.OPEN_DATE, 'IW') DESC
	                                ,ITEM_EDESC";
                query = string.Format(query);
            }
            optimizedPendingLcData = this._dbContext.SqlQuery<OptimizedPendingLcModel>(query).ToList();
            return optimizedPendingLcData;
        }

    }
}