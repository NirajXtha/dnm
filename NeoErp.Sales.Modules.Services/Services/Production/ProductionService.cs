using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models.Production;

namespace NeoErp.Sales.Modules.Services.Services.Production
{
    public class ProductionService : IProductionService
    {
        private NeoErpCoreEntity _objectEntity;
        public ProductionService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }
        public List<ProductionRegisterModel> GetProductionRegister(ReportFiltersModel filters, User userInfo)
        {
            //var companyCode = string.Join(",", filters.CompanyFilter);
            //if (string.IsNullOrEmpty(companyCode))
            //    companyCode = userInfo.company_code;
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = @"SELECT 
        A.MRR_NO AS MrrNo,
        A.MRR_DATE AS MrrDate,
        BS_DATE(A.MRR_DATE)  AS Miti,
        A.MANUAL_NO AS ManualNo,
        FN_FETCH_DESC(A.COMPANY_CODE,'IP_LOCATION_SETUP',A.FROM_LOCATION_CODE) AS FromLocation,
        A.FROM_LOCATION_CODE,
        A.FROM_BUDGET_FLAG AS FromBudget,
        FN_FETCH_DESC(A.COMPANY_CODE,'IP_LOCATION_SETUP',A.TO_LOCATION_CODE) AS ToLocation,
        A.TO_LOCATION_CODE,
        A.SERIAL_NO AS SerialNo,
        B.ITEM_EDESC AS Item,
        B.ITEM_CODE,
        SUBSTR(FN_FETCH_GROUP_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', B.PRE_ITEM_CODE),1,100) AS ItemGroup,
        SUBSTR(FN_FETCH_PRE_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', B.PRE_ITEM_CODE),1,100) AS ItemSubGroup,
        A.MU_CODE AS MuCode,
        A.QUANTITY AS Quantity,
        A.UNIT_PRICE AS UnitPrice,
        A.TOTAL_PRICE AS TotalPrice,
        A.CALC_QUANTITY AS CalcQuantity,
        A.CALC_UNIT_PRICE AS CalcUnitPrice,
        A.CALC_TOTAL_PRICE AS CalcTotalPrice,
        A.REMARKS AS Remarks,
        A.FORM_CODE AS FormCode,
        A.COMPANY_CODE AS CompanyCode,
        FN_FETCH_DESC(A.COMPANY_CODE,'COMPANY_SETUP',A.COMPANY_CODE) AS Company,
        A.BRANCH_CODE AS BranchCode,
        FN_FETCH_DESC(A.COMPANY_CODE,'FA_BRANCH_SETUP',A.BRANCH_CODE) AS Branch,
        A.CURRENCY_CODE AS CurrencyCode,
        A.EXCHANGE_RATE AS ExchangeRate,
        A.TRACKING_NO AS TrackingNo,
        A.BATCH_NO AS BatchNo,
        A.LOT_NO AS LotNo,
        A.ROLL_QTY AS RollQty,
        A.DIVISION_CODE AS DivisionCode,
        C.RESOURCE_CODE AS ResourceCode,
        C.START_TIME AS StartTime,
        C.END_TIME AS EndTime,
        C.TOTAL_HOURS AS TotalHours,
        D.RESOURCE_EDESC AS ResourceName
FROM IP_PRODUCTION_MRR A, IP_ITEM_MASTER_SETUP B, MP_RESOURCE_DETAIL_ENTRY C, MP_RESOURCE_SETUP D
WHERE A.ITEM_CODE = B.ITEM_CODE
        AND A.COMPANY_CODE = B.COMPANY_CODE
        AND A.MRR_NO = C.VOUCHER_NO(+)
        AND A.FORM_CODE = C.FORM_CODE(+)
        AND A.COMPANY_CODE = C.COMPANY_CODE(+)
        AND A.BRANCH_CODE = C.BRANCH_CODE(+)
        AND C.RESOURCE_CODE = D.RESOURCE_CODE(+)
        AND C.COMPANY_CODE = D.COMPANY_CODE(+)
        AND A.DELETED_FLAG='N'";

            if (!string.IsNullOrEmpty(filters.FromDate))
                Query = Query + " AND A.MRR_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and A.MRR_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";
            if (filters.ProductFilter.Count() > 0)
            {
                var products = filters.ProductFilter;
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

                Query = Query + string.Format(@" AND B.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join("','", products));
                //Query += " and (";
                //Query += "(B.ITEM_CODE IN (" + string.Join(",", filters.ProductFilter) + "))) ";
                //Query = Query.Substring(0, Query.Length - 1);
            }
            if (filters.LocationFilter.Count() > 0)
            {
                var locations = filters.LocationFilter;
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
                Query = Query + string.Format(@" AND A.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //Query += " AND (A.FROM_LOCATION_CODE IN (" + string.Join(",", filters.LocationFilter) + ")) OR A.TO_LOCATION_CODE IN (" + string.Join(",", filters.LocationFilter) + "))";
                //Query = Query.Substring(0, Query.Length - 1);
            }
            if (filters.CategoryFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND C.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            Query = Query + string.Format(@" AND A.COMPANY_CODE IN ({0}) ", string.Join("','", companyCode).ToString());
            var productionRegisters = _objectEntity.SqlQuery<ProductionRegisterModel>(Query).ToList();
            return productionRegisters;
        }

        public IList<ProductionStockInOutSummaryModel> GetProductionStockInOutSummary(ReportFiltersModel model, User userInfo)
        {
            
            var stockData = new List<ProductionStockInOutSummaryModel>();
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            if (model.BranchFilter.Count > 0)
            {

                var Query = @"SELECT ITEM_CODE SKU_CODE 
	                                        ,ITEM_EDESC
	                                        ,INDEX_MU_CODE 
	                                        ,MASTER_ITEM_CODE
	                                        ,PRE_ITEM_CODE
	                                        ,GROUP_SKU_FLAG
	                                        ,TO_CHAR(MY_LEVEL) MY_LEVEL
	                                        ,SERVICE_ITEM_FLAG
	                                        ,NVL(OPE_QTY, 0) OPE_QTY
	                                        ,NVL(OPE_VAL, 0) OPE_VAL
	                                        ,NVL(IN_QTY, 0) IN_QTY
	                                        ,NVL(IN_VAL, 0) IN_VAL
	                                        ,NVL(OUT_QTY, 0) OUT_QTY
	                                        ,NVL(OUT_VAL, 0) OUT_VAL
	                                        ,(NVL(OPE_QTY, 0) + NVL(IN_QTY, 0) - NVL(OUT_QTY, 0) * DECODE(SERVICE_ITEM_FLAG, 'Y', 0, 1)) CLO_QTY
	                                        ,(NVL(OPE_VAL, 0) + NVL(IN_VAL, 0) - NVL(OUT_VAL, 0) * DECODE(SERVICE_ITEM_FLAG, 'Y', 0, 1)) CLO_VAL
                                        FROM (
	                                        SELECT A.ITEM_CODE
		                                        ,A.ITEM_EDESC
		                                        ,A.INDEX_MU_CODE
		                                        ,A.MASTER_ITEM_CODE
		                                        ,A.PRE_ITEM_CODE
		                                        ,A.GROUP_SKU_FLAG
		                                        ,LEVEL MY_LEVEL
		                                        ,NVL(A.SERVICE_ITEM_FLAG, 'N') SERVICE_ITEM_FLAG
		                                        ,DECODE((
				                                        SELECT SUM(NVL(IN_QUANTITY, 0) - NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND (
						                                        TRUNC(VOUCHER_DATE) < TO_DATE ('{2}','YYYY-MM-DD')
						                                        OR FORM_CODE = '0'
						                                        )
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), NULL, (
				                                        SELECT SUM(NVL(IN_QUANTITY, 0) - NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND PRE_ITEM_CODE LIKE A.MASTER_ITEM_CODE || '%'
					                                        AND (
						                                        TRUNC(VOUCHER_DATE) < TO_DATE ('{2}','YYYY-MM-DD')
						                                        OR FORM_CODE = '0'
						                                        )
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), (
				                                        SELECT SUM(NVL(IN_QUANTITY, 0) - NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND (
						                                        TRUNC(VOUCHER_DATE) < TO_DATE ('{2}','YYYY-MM-DD')
						                                        OR FORM_CODE = '0'
						                                        )
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        )) * DECODE(A.SERVICE_ITEM_FLAG, 'Y', 0, 1) OPE_QTY
		                                        ,DECODE((
				                                        SELECT SUM(NVL(IN_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), NULL, (
				                                        SELECT SUM(NVL(IN_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND PRE_ITEM_CODE LIKE A.MASTER_ITEM_CODE || '%'
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), (
				                                        SELECT SUM(NVL(IN_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        )) IN_QTY
		                                        ,DECODE((
				                                        SELECT SUM(NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), NULL, (
				                                        SELECT SUM(NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND PRE_ITEM_CODE LIKE A.MASTER_ITEM_CODE || '%'
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), (
				                                        SELECT SUM(NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        )) OUT_QTY
		                                        ,0 OPE_VAL
		                                        ,0 IN_VAL
		                                        ,0 OUT_VAL
	                                        FROM IP_ITEM_MASTER_SETUP A
	                                        WHERE A.COMPANY_CODE IN({0})
		                                        AND COMPANY_CODE IN({0})
		                                        AND LEVEL = 1
		                                        AND CATEGORY_CODE IN (
			                                        'RM'
			                                        ,'SS'
			                                        ) START
	                                        WITH A.PRE_ITEM_CODE = '00'
		                                        AND COMPANY_CODE IN({0}) CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
		                                        AND COMPANY_CODE IN({0})";

                if (model.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
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
                    Query = Query + string.Format(@" AND A.LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
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

                    Query = Query + string.Format(@" AND A.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ({1}) AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join(",", products));

                }
                Query += "ORDER BY MASTER_ITEM_CODE,INITCAP(TRIM(ITEM_EDESC)))";
                Query = string.Format(Query, companyCode, string.Join("','", model.BranchFilter), model.FromDate, model.ToDate);
                stockData = this._objectEntity.SqlQuery<ProductionStockInOutSummaryModel>(Query).ToList();

            }
            else
            {

                var Query = @"SELECT ITEM_CODE SKU_CODE 
	                                        ,ITEM_EDESC
	                                        ,INDEX_MU_CODE 
	                                        ,MASTER_ITEM_CODE
	                                        ,PRE_ITEM_CODE
	                                        ,GROUP_SKU_FLAG
	                                        ,TO_CHAR(MY_LEVEL) MY_LEVEL
	                                        ,SERVICE_ITEM_FLAG
	                                        ,NVL(OPE_QTY, 0) OPE_QTY
	                                        ,NVL(OPE_VAL, 0) OPE_VAL
	                                        ,NVL(IN_QTY, 0) IN_QTY
	                                        ,NVL(IN_VAL, 0) IN_VAL
	                                        ,NVL(OUT_QTY, 0) OUT_QTY
	                                        ,NVL(OUT_VAL, 0) OUT_VAL
	                                        ,(NVL(OPE_QTY, 0) + NVL(IN_QTY, 0) - NVL(OUT_QTY, 0) * DECODE(SERVICE_ITEM_FLAG, 'Y', 0, 1)) CLO_QTY
	                                        ,(NVL(OPE_VAL, 0) + NVL(IN_VAL, 0) - NVL(OUT_VAL, 0) * DECODE(SERVICE_ITEM_FLAG, 'Y', 0, 1)) CLO_VAL
                                        FROM (
	                                        SELECT A.ITEM_CODE
		                                        ,A.ITEM_EDESC
		                                        ,A.INDEX_MU_CODE
		                                        ,A.MASTER_ITEM_CODE
		                                        ,A.PRE_ITEM_CODE
		                                        ,A.GROUP_SKU_FLAG
		                                        ,LEVEL MY_LEVEL
		                                        ,NVL(A.SERVICE_ITEM_FLAG, 'N') SERVICE_ITEM_FLAG
		                                        ,DECODE((
				                                        SELECT SUM(NVL(IN_QUANTITY, 0) - NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND (
						                                        TRUNC(VOUCHER_DATE) < TO_DATE ('{2}','YYYY-MM-DD')
						                                        OR FORM_CODE = '0'
						                                        )
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), NULL, (
				                                        SELECT SUM(NVL(IN_QUANTITY, 0) - NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND PRE_ITEM_CODE LIKE A.MASTER_ITEM_CODE || '%'
					                                        AND (
						                                        TRUNC(VOUCHER_DATE) < TO_DATE ('{2}','YYYY-MM-DD')
						                                        OR FORM_CODE = '0'
						                                        )
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), (
				                                        SELECT SUM(NVL(IN_QUANTITY, 0) - NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND (
						                                        TRUNC(VOUCHER_DATE) < TO_DATE ('{2}','YYYY-MM-DD')
						                                        OR FORM_CODE = '0'
						                                        )
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        )) * DECODE(A.SERVICE_ITEM_FLAG, 'Y', 0, 1) OPE_QTY
		                                        ,DECODE((
				                                        SELECT SUM(NVL(IN_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), NULL, (
				                                        SELECT SUM(NVL(IN_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND PRE_ITEM_CODE LIKE A.MASTER_ITEM_CODE || '%'
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), (
				                                        SELECT SUM(NVL(IN_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        )) IN_QTY
		                                        ,DECODE((
				                                        SELECT SUM(NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), NULL, (
				                                        SELECT SUM(NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND PRE_ITEM_CODE LIKE A.MASTER_ITEM_CODE || '%'
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        ), (
				                                        SELECT SUM(NVL(OUT_QUANTITY, 0))
				                                        FROM V$VIRTUAL_STOCK_WIP_LEDGER1
				                                        WHERE COMPANY_CODE = A.COMPANY_CODE
					                                        AND ITEM_CODE = A.ITEM_CODE
					                                        AND TRUNC(VOUCHER_DATE) BETWEEN TO_DATE ('{2}','YYYY-MM-DD')
						                                        AND TO_DATE ('{3}','YYYY-MM-DD')
					                                        AND FORM_CODE <> '0'
					                                        AND DELETED_FLAG = 'N'
					                                        AND BRANCH_CODE IN ('{1}')
					                                        AND TABLE_NAME IN ('IP_TRANSFER_ISSUE')
				                                        )) OUT_QTY
		                                        ,0 OPE_VAL
		                                        ,0 IN_VAL
		                                        ,0 OUT_VAL
	                                        FROM IP_ITEM_MASTER_SETUP A
	                                        WHERE A.COMPANY_CODE IN({0})
		                                        AND COMPANY_CODE IN({0})
		                                        AND LEVEL = 1
		                                        AND CATEGORY_CODE IN (
			                                        'RM'
			                                        ,'SS'
			                                        ) START
	                                        WITH A.PRE_ITEM_CODE = '00'
		                                        AND COMPANY_CODE IN({0}) CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
		                                        AND COMPANY_CODE IN({0})";

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

				Query += "ORDER BY MASTER_ITEM_CODE,INITCAP(TRIM(ITEM_EDESC)))";
				Query = string.Format(Query, companyCode, string.Join("','", model.BranchFilter), model.FromDate, model.ToDate);
                stockData = this._objectEntity.SqlQuery<ProductionStockInOutSummaryModel>(Query).ToList();
            }
            return stockData;
        }

    }
}
