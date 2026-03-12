using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class StockService : IStockService
    {
        private NeoErpCoreEntity _objectEntity;

        public StockService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }




        public IList<LocationWiseStockModel> GetLocationWiseStockReport(ReportFiltersModel model, User userInfo)
        {
            //var companyCode = string.Empty;
            //foreach (var company in model.CompanyFilter)
            //{
            //    companyCode += $@"'{company}',";
            //}

            //  var fiscalYear = ConfigurationManager.AppSettings["FiscalYear"];
            //var startFiscalYearQuery = $"SELECT  TO_CHAR(TO_DATE (START_DATE,'YYYY-MM-DD'))START_DATE FROM HR_FISCAL_YEAR_CODE  WHERE FISCAL_YEAR_CODE='{fiscalYear}' ORDER BY START_DATE DESC";
            //var startFiscalYear = this._objectEntity.SqlQuery<string>(startFiscalYearQuery).FirstOrDefault();
            var stockData = new List<LocationWiseStockModel>();
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            if (model.BranchFilter.Count > 0)
            {

                var Query = @"SELECT ITEM_CODE as SKU_CODE,STOCK.BRANCH_CODE, ITEM_EDESC, STOCK.LOCATION_CODE, LS.LOCATION_EDESC  AS LOCATION, 
nvl(OPENING_QTY,0) as OpeningQuantity, nvl(TRN_IN,0) as InQuantity, nvl(TRN_OUT,0) as OutQuantity, 
--CLOSING_QTY as ClosingQuantity
((nvl(OPENING_QTY,0) +nvl(TRN_IN,0)) -nvl(TRN_OUT,0)) ClosingQuantity

,STOCK.MU_CODE as Unit
                            FROM (SELECT ITEM_CODE,
                                        ITEM_EDESC,
                                        LOCATION_CODE,
                                        BRANCH_CODE,
                                        --INITIAL_OPENING_QTY OPENING_QTY,
                                        (NVL(INITIAL_OPENING_QTY,0) + NVL(IN_QTY_RUNNING,0)) - ( NVL(OUT_QTY_RUNNING,0)) OPENING_QTY,
                                        TRN_IN,
                                        TRN_OUT,
                                        mu_code,
                                        (NVL(INITIAL_OPENING_QTY,0) + NVL(TRN_IN,0)) - ( NVL(TRN_OUT,0)) CLOSING_QTY
                                    FROM (SELECT DISTINCT
                                                ITEM_CODE,
                                                ITEM_EDESC,
                                                LOCATION_CODE,
                                                BRANCH_CODE,
                                                mu_code,
                                                (SELECT SUM (NVL (A.IN_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        --AND A.VOUCHER_DATE >= TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE = TO_DATE ('2019-Jul-17','YYYY-MM-DD')
                                                        AND A.FORM_CODE = '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    INITIAL_OPENING_QTY, 
                                                 (SELECT SUM (NVL (A.IN_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND A.VOUCHER_DATE >= TO_DATE ('2019-Jul-17','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE < TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.FORM_CODE <> '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    IN_QTY_RUNNING,  
                                                  (SELECT SUM (NVL (A.OUT_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND A.VOUCHER_DATE >= TO_DATE ('2019-Jul-17','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE < TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.FORM_CODE <> '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    OUT_QTY_RUNNING,  
                                                (SELECT SUM (NVL (A.IN_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND A.VOUCHER_DATE >= TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE <= TO_DATE ('{3}','YYYY-MM-DD')
                                                        AND A.FORM_CODE <> '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    TRN_IN,
                                                (SELECT SUM (NVL (A.OUT_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND A.VOUCHER_DATE >= TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE <= TO_DATE ('{3}','YYYY-MM-DD')
                                                        AND A.FORM_CODE <> '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    TRN_OUT
                                            FROM V$VIRTUAL_STOCK_WIP_LEDGER1 B)
                                    WHERE    INITIAL_OPENING_QTY IS NOT NULL
                                        OR TRN_IN IS NOT NULL
                                        OR TRN_OUT IS NOT NULL) STOCK, IP_LOCATION_SETUP LS
                            WHERE STOCK.LOCATION_CODE = LS.LOCATION_CODE";

                if (model.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND STOCK.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                }
                //for LOCATION Filter
                if (model.LocationFilter.Count() > 0)
                {
                    var locations = model.LocationFilter;
                    var locationConditionQuery = string.Empty;
                    for (int i = 0; i < locations.Count; i++)
                    {

                        if (i == 0)
                            locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                        else
                        {
                            locationConditionQuery += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                        }
                    }
                    Query = Query + string.Format(@" AND STOCK.LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                    //Query += " and (";
                    ////IF CUSTOMER_SKU_FLAG = G
                    //foreach (var item in model.LocationFilter)
                    //{
                    //    Query += "STOCK.LOCATION_CODE like '" + item + "%' OR ";
                    //}
                    //Query = Query.Substring(0, Query.Length - 3) + ") ";
                }

                //if (model.ProductFilter.Count > 0)
                //{
                //    Query += string.Format(@" AND STOCK.ITEM_CODE IN ('{0}')", string.Join("','", model.ProductFilter).ToString());
                //}
                if (model.ProductFilter.Count() > 0)
                {
                    var products = model.ProductFilter;
                    var productConditionQuery = string.Empty;
                    for (int i = 0; i < products.Count; i++)
                    {

                        if (i == 0)
                            productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G' )", products[i], companyCode);
                        else
                        {
                            productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                        }
                    }

                    Query = Query + string.Format(@" AND STOCK.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ({1}) AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join(",", products));

                }

                Query = string.Format(Query, companyCode, string.Join("','", model.BranchFilter), model.FromDate, model.ToDate);
                stockData = this._objectEntity.SqlQuery<LocationWiseStockModel>(Query).ToList();

            }
            else
            {

                var Query = @"SELECT ITEM_CODE as SKU_CODE,STOCK.BRANCH_CODE, ITEM_EDESC, STOCK.LOCATION_CODE, LS.LOCATION_EDESC  AS LOCATION, 
nvl(OPENING_QTY,0) as OpeningQuantity, nvl(TRN_IN,0) as InQuantity, nvl(TRN_OUT,0) as OutQuantity, 
--CLOSING_QTY as ClosingQuantity
((nvl(OPENING_QTY,0) +nvl(TRN_IN,0)) -nvl(TRN_OUT,0)) ClosingQuantity

,STOCK.MU_CODE as Unit
                            FROM (SELECT ITEM_CODE,
                                        ITEM_EDESC,
                                        LOCATION_CODE,
                                        BRANCH_CODE,
                                        --INITIAL_OPENING_QTY OPENING_QTY,
                                        (NVL(INITIAL_OPENING_QTY,0) + NVL(IN_QTY_RUNNING,0)) - ( NVL(OUT_QTY_RUNNING,0)) OPENING_QTY,
                                        TRN_IN,
                                        TRN_OUT,
                                        mu_code,
                                        (NVL(INITIAL_OPENING_QTY,0) + NVL(TRN_IN,0)) - ( NVL(TRN_OUT,0)) CLOSING_QTY
                                    FROM (SELECT DISTINCT
                                                ITEM_CODE,
                                                ITEM_EDESC,
                                                LOCATION_CODE,
                                                BRANCH_CODE,
                                                mu_code,
                                                (SELECT SUM (NVL (A.IN_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        --AND A.VOUCHER_DATE >= TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE = TO_DATE ('2019-Jul-17','YYYY-MM-DD')
                                                        AND A.FORM_CODE = '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        --AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    INITIAL_OPENING_QTY, 
                                                 (SELECT SUM (NVL (A.IN_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND A.VOUCHER_DATE >= TO_DATE ('2019-Jul-17','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE < TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.FORM_CODE <> '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        --AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    IN_QTY_RUNNING,  
                                                  (SELECT SUM (NVL (A.OUT_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND A.VOUCHER_DATE >= TO_DATE ('2019-Jul-17','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE < TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.FORM_CODE <> '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        --AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    OUT_QTY_RUNNING,  
                                                (SELECT SUM (NVL (A.IN_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND A.VOUCHER_DATE >= TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE <= TO_DATE ('{3}','YYYY-MM-DD')
                                                        AND A.FORM_CODE <> '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        --AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    TRN_IN,
                                                (SELECT SUM (NVL (A.OUT_QUANTITY, 0))
                                                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 A
                                                    WHERE A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND A.VOUCHER_DATE >= TO_DATE ('{2}','YYYY-MM-DD')
                                                        AND A.VOUCHER_DATE <= TO_DATE ('{3}','YYYY-MM-DD')
                                                        AND A.FORM_CODE <> '0'
                                                        AND A.COMPANY_CODE IN({0})
                                                        --AND A.BRANCH_CODE IN ('{1}')
                                                        AND A.LOCATION_CODE = B.LOCATION_CODE
                                                        AND A.ITEM_CODE = B.ITEM_CODE)
                                                    TRN_OUT
                                            FROM V$VIRTUAL_STOCK_WIP_LEDGER1 B)
                                    WHERE    INITIAL_OPENING_QTY IS NOT NULL
                                        OR TRN_IN IS NOT NULL
                                        OR TRN_OUT IS NOT NULL) STOCK, IP_LOCATION_SETUP LS
                            WHERE STOCK.LOCATION_CODE = LS.LOCATION_CODE";

                if (model.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND STOCK.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                }
                //for LOCATION Filter
                if (model.LocationFilter.Count() > 0)
                {
                    var locations = model.LocationFilter;
                    var locationConditionQuery = string.Empty;
                    for (int i = 0; i < locations.Count; i++)
                    {

                        if (i == 0)
                            locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                        else
                        {
                            locationConditionQuery += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                        }
                    }
                    Query = Query + string.Format(@" AND STOCK.LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                    //Query += " and (";
                    ////IF CUSTOMER_SKU_FLAG = G
                    //foreach (var item in model.LocationFilter)
                    //{
                    //    Query += "STOCK.LOCATION_CODE like '" + item + "%' OR ";
                    //}
                    //Query = Query.Substring(0, Query.Length - 3) + ") ";
                }
                //if (model.ProductFilter.Count > 0)
                //{

                //    Query += string.Format(@" AND STOCK.ITEM_CODE IN ('{0}')", string.Join("','", model.ProductFilter).ToString());
                //}

                if (model.ProductFilter.Count() > 0)
                {
                    var products = model.ProductFilter;
                    var productConditionQuery = string.Empty;
                    for (int i = 0; i < products.Count; i++)
                    {

                        if (i == 0)
                            productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G' )", products[i], companyCode);
                        else
                        {
                            productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                        }
                    }

                    Query = Query + string.Format(@" AND STOCK.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ({1}) AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join(",", products));
                    //int first = 1;
                    //foreach (var poductItem in model.ProductFilter)
                    //{
                    //    if (first == 1)
                    //        Query = Query + "AND MASTER_ITEM_CODE LIKE (SELECT DISTINCT(MASTER_ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='" + poductItem + @"' AND COMPANY_CODE IN ('" + companyCode + "'))||'%'";
                    //    else
                    //        Query = Query + "or MASTER_ITEM_CODE LIKE (SELECT DISTINCT(MASTER_ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='" + poductItem + @"' AND COMPANY_CODE IN ('" + companyCode + "'))||'%'";
                    //    first = first + 1;

                    //}

                }


                Query = string.Format(Query, companyCode, string.Join("','", model.BranchFilter), model.FromDate, model.ToDate);
                stockData = this._objectEntity.SqlQuery<LocationWiseStockModel>(Query).ToList();
            }
            return stockData;
        }

        public IList<LocationWiseItemStockModel> GetLocationWiseStock(ReportFiltersModel model, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = @"SELECT ITEM_EDESC, ITEM_CODE,
                          STOCK.LOCATION_CODE as LocationCode, LS.LOCATION_EDESC as LOCATION,
                         SUM (IN_QUANTITY - OUT_QUANTITY) as AvilableStock,
                         STOCK.MU_CODE as Mu_code
                         FROM V$VIRTUAL_STOCK_WIP_LEDGER1 STOCK, IP_LOCATION_SETUP LS
                         WHERE STOCK.LOCATION_CODE = LS.LOCATION_CODE
                         AND stock.deleted_flag='N'
                         AND STOCK.COMPANY_CODE IN(" + companyCode + @")
                         AND  VOUCHER_DATE >= TO_DATE ('" + model.FromDate + @"','YYYY-MM-DD')
                         AND VOUCHER_DATE <=TO_DATE ('" + model.ToDate + @"','YYYY-MM-DD')";


            if (model.ProductFilter.Count() > 0)
            {
                var products = model.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G' )", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND STOCK.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ({1}) AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join(",", products));
                //int first = 1;
                //foreach (var poductItem in model.ProductFilter)
                //{
                //    if (first == 1)
                //        Query = Query + "AND MASTER_ITEM_CODE LIKE (SELECT DISTINCT(MASTER_ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='" + poductItem + @"' AND COMPANY_CODE IN ('" + companyCode + "'))||'%'";
                //    else
                //        Query = Query + "or MASTER_ITEM_CODE LIKE (SELECT DISTINCT(MASTER_ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='" + poductItem + @"' AND COMPANY_CODE IN ('" + companyCode + "'))||'%'";
                //    first = first + 1;

                //}

            }

            if (model.BranchFilter.Count() > 0)
            {
                Query = Query + string.Format(@" AND TRIM(BRANCH_CODE) IN ('{0}') ", string.Join("','", model.BranchFilter).ToString());

            }



            if (model.CategoryFilter.Count() > 0)
            {
                Query = Query + string.Format(@" AND TRIM(CATEGORY_CODE) IN ('{0}') ", string.Join("','", model.CategoryFilter).ToString());
                //  Query = Query + "AND TRIM(CATEGORY_CODE) IN (" + model.CategoryFilter + ")";
            }
            if (model.LocationFilter.Count() > 0)
            {

                var locations = model.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                Query = Query + string.Format(@" AND STOCK.LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //Query += " and (";
                ////IF CUSTOMER_SKU_FLAG = G
                //foreach (var item in model.LocationFilter)
                //{
                //    Query += "STOCK.LOCATION_CODE like '" + item + "%' OR ";
                //}
                //Query = Query.Substring(0, Query.Length - 3) + ") ";
            }

            Query = Query + " GROUP BY STOCK.LOCATION_CODE,LS.LOCATION_EDESC, ITEM_CODE,ITEM_EDESC,STOCK.MU_CODE,STOCK.COMPANY_CODE";
            var stockData = this._objectEntity.SqlQuery<LocationWiseItemStockModel>(Query).ToList();
            return stockData;


            //AND STOCK.BRANCH_CODE IN ('01.09') 
            //  AND STOCK.COMPANY_CODE IN('01') 
            //  AND TRIM(CATEGORY_CODE) IN (SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE)
            // GROUP BY STOCK.LOCATION_CODE,LS.LOCATION_EDESC, ITEM_CODE,ITEM_EDESC,
            // MU_CODE";
        }
        public IList<LocationsHeader> GetLocationHeader(ReportFiltersModel model, User userinfo)
        {

            var companyCode = "'" + String.Join("','", model.CompanyFilter) + "'";
            companyCode = companyCode == "''" ? "'" + userinfo.company_code + "'" : companyCode;
            //   string Query = @"select  location_edesc as  LocationTitle,location_code as  LocationNo  from IP_LOCATION_SETUP where deleted_flag='N' and company_code='01' and Group_sku_flag='I' and location_type_code='MS'";
            string Query = @"SELECT distinct STOCK.LOCATION_CODE as LocationNo, LS.LOCATION_EDESC as LocationTitle
                         FROM V$VIRTUAL_STOCK_WIP_LEDGER1 STOCK, IP_LOCATION_SETUP LS
                         WHERE STOCK.LOCATION_CODE = LS.LOCATION_CODE
                         and STOCK.COMPANY_CODE IN(" + companyCode + ") and stock.deleted_flag='N'";

            if (model.BranchFilter.Count() > 0)
            {
                Query = Query + string.Format(@" AND TRIM(BRANCH_CODE) IN ('{0}') ", string.Join("','", model.BranchFilter).ToString());

            }

            if (model.LocationFilter.Count() > 0)
            {
                Query += " and (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in model.LocationFilter)
                {
                    Query += "STOCK.LOCATION_CODE like '" + item + "%' OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }

            var stockData = this._objectEntity.SqlQuery<LocationsHeader>(Query).ToList();
            return stockData;
        }
    }
}
