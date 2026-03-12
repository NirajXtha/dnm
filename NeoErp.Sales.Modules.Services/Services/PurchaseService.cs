using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class PurchaseService : IPurchaseService
    {
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;

        public PurchaseService(NeoErpCoreEntity objectEntity, IWorkContext workContext)
        {
            this._objectEntity = objectEntity;
            this._workContext = workContext;
        }
        public List<PurchaseRegisterDetail> GetPurchaseRegister(string fromdate, string toDate, User userInfo)
        {
            string query = @"SELECT A.INVOICE_DATE AS InvoiceDate, BS_DATE(A.INVOICE_DATE) AS Miti,
                FN_GET_SECOND_REFERENCE_NO(a.COMPANY_CODE, a.BRANCH_CODE, a.FORM_CODE, a.INVOICE_NO, a.SERIAL_NO) AS PONo,
                FN_GET_REFERENCE_NO(a.COMPANY_CODE, a.BRANCH_CODE, a.FORM_CODE, a.INVOICE_NO, a.SERIAL_NO) AS GRNNo,
                A.INVOICE_NO AS InvoiceNo,A.MANUAL_NO AS ManualNo,A.SUPPLIER_INV_NO AS SuppInvNo,A.SUPPLIER_INV_DATE AS SuppInvDate,
                INITCAP(B.SUPPLIER_EDESC) AS SupplierName,
                INITCAP(D.LOCATION_EDESC) AS StorageLocation,
                A.REMARKS AS Remarks,
                INITCAP(C.ITEM_EDESC) AS ProductName,
                A.MU_CODE AS Unit,
                A.QUANTITY AS Quantity,
                A.CALC_QUANTITY AS NetQty, A.CALC_UNIT_PRICE AS Rate, A.CALC_TOTAL_PRICE AS GrossAmount
                FROM IP_PURCHASE_INVOICE     A,
                IP_SUPPLIER_SETUP B,
                IP_ITEM_MASTER_SETUP C,
                IP_LOCATION_SETUP D
                WHERE
                A.DELETED_FLAG = 'N' AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.SUPPLIER_CODE = B.SUPPLIER_CODE
                AND A.ITEM_CODE = C.ITEM_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.TO_LOCATION_CODE = D.LOCATION_CODE";
            if (!string.IsNullOrEmpty(fromdate))
                query = query + " AND A.INVOICE_DATE>=TO_DATE('" + fromdate + "', 'YYYY-MM-DD') and A.INVOICE_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD') ORDER BY A.INVOICE_NO";
            var purchaseRegisters = _objectEntity.SqlQuery<PurchaseRegisterDetail>(query).ToList();
            //   purchaseRegisters.Select);
            return purchaseRegisters;
        }
        public List<PurchaseRegisterDetail> GetPurchaseRegister(ReportFiltersModel filters, User userinfo)
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
            var amountRoundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter);
            var amountFigureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter);
            var quantityRoundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter);
            var quantityFigureFilter = ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter);

            string query = string.Format(@"SELECT TO_CHAR(A.INVOICE_DATE) AS InvoiceDate, BS_DATE(A.INVOICE_DATE) AS Miti,
                NVL((FN_GET_SECOND_REFERENCE_NO(a.COMPANY_CODE, a.BRANCH_CODE, a.FORM_CODE, a.INVOICE_NO, a.SERIAL_NO)),'-') AS PONo,
                NVL((FN_GET_REFERENCE_NO(a.COMPANY_CODE, a.BRANCH_CODE, a.FORM_CODE, a.INVOICE_NO, a.SERIAL_NO)),'-') AS GRNNo,
                A.INVOICE_NO AS InvoiceNo,NVL(A.MANUAL_NO,'-') AS ManualNo,A.SUPPLIER_INV_NO AS SuppInvNo,TO_CHAR(A.SUPPLIER_INV_DATE) AS SuppInvDate,
                INITCAP(B.SUPPLIER_EDESC) AS SupplierName,
                INITCAP(D.LOCATION_EDESC) AS StorageLocation,
                NVL(A.REMARKS, '-') AS Remarks,
                INITCAP(C.ITEM_EDESC) AS ProductName,
                A.MU_CODE AS Unit,
                ROUND(NVL(A.QUANTITY,0)/{0},{1}) AS Quantity,
                ROUND(NVL(A.CALC_QUANTITY,0)/{2},{3}) AS NetQty, ROUND(NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_UNIT_PRICE,0),'NRS',A.INVOICE_DATE),0)/{4},{5}) AS Rate,
                 ROUND(FN_CONVERT_CURRENCY(NVL(A.CALC_TOTAL_PRICE,0),'NRS',A.INVOICE_DATE)/{6},{7})  AS GrossAmount
                FROM IP_PURCHASE_INVOICE     A,
                IP_SUPPLIER_SETUP B,
                IP_ITEM_MASTER_SETUP C,
                IP_LOCATION_SETUP D
                WHERE
                A.DELETED_FLAG = 'N' AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.SUPPLIER_CODE = B.SUPPLIER_CODE
                AND A.ITEM_CODE = C.ITEM_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.COMPANY_CODE IN ('" + userinfo.company_code + "')" +
                " AND A.TO_LOCATION_CODE = D.LOCATION_CODE", quantityFigureFilter, quantityRoundUpFilter, quantityFigureFilter, quantityRoundUpFilter, amountFigureFilter
                , amountRoundUpFilter, amountFigureFilter, amountRoundUpFilter);

            if (filters.SupplierFilter.Count > 0)
            {
                var suppliers = filters.SupplierFilter;
                var supplierConditionQuery = string.Empty;
                for (int i = 0; i < suppliers.Count; i++)
                {

                    if (i == 0)
                        supplierConditionQuery += string.Format("MASTER_SUPPLIER_CODE like (Select MASTER_SUPPLIER_CODE || '%' from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN '{1}' AND GROUP_SKU_FLAG = 'G')", suppliers[i], userinfo.company_code);
                    else
                    {
                        supplierConditionQuery += string.Format(" OR  MASTER_SUPPLIER_CODE like (Select MASTER_SUPPLIER_CODE || '%'  from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN '{1}' AND GROUP_SKU_FLAG = 'G')", suppliers[i], userinfo.company_code);
                    }
                }

                query = query + string.Format(@" AND A.SUPPLIER_CODE IN (SELECT DISTINCT(SUPPLIER_CODE) FROM IP_SUPPLIER_SETUP WHERE {0} OR (SUPPLIER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", supplierConditionQuery, string.Join("','", suppliers));
                //query += string.Format(@" AND A.SUPPLIER_CODE IN ({0}) ", string.Join(",", filters.SupplierFilter).ToString());
            }

            if (filters.ProductFilter.Count > 0)
            {
                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select MASTER_ITEM_CODE || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN '{1}' AND GROUP_SKU_FLAG='G' )", products[i], userinfo.company_code);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN '{1}' AND GROUP_SKU_FLAG='G')", products[i], userinfo.company_code);
                    }
                }

                query = query + string.Format(@" AND A.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join("','", products));

                //query = query + string.Format(@" AND A.ITEM_CODE IN ({0}) ", string.Join(",", filters.ProductFilter).ToString());
            }

            if (filters.PurchaseDocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND A.FORM_CODE  IN  ('{0}')", string.Join("','", filters.PurchaseDocumentFilter).ToString());
            }
            if (filters.CategoryFilter.Count > 0)
            {
                query = query + string.Format(@" AND C.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {
                var locations = filters.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }

                query += string.Format(@" AND A.TO_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //query += string.Format(@" AND A.TO_LOCATION_CODE IN ('{0}')", string.Join("','",filters.LocationFilter).ToString());
            }
            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }

            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " AND A.INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and A.INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD') ";

            int min = 0, max = 0;
            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query += string.Format(@" AND NVL(A.CALC_QUANTITY,0) > {0} AND NVL(A.CALC_QUANTITY,0)<={1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query += string.Format(@" AND NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_UNIT_PRICE,0),'NRS',A.INVOICE_DATE),0)>={0} AND NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_UNIT_PRICE,0),'NRS',A.INVOICE_DATE),0)<={1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query += string.Format(@" AND NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_TOTAL_PRICE,0),'NRS',A.INVOICE_DATE),0)>= {0} and NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_TOTAL_PRICE,0),'NRS',A.INVOICE_DATE),0) <= {1}", min, max);

            query += " ORDER BY A.INVOICE_NO";


            var purchaseRegisters = _objectEntity.SqlQuery<PurchaseRegisterDetail>(query).ToList();
            //   purchaseRegisters.Select);
            return purchaseRegisters;
        }
        public List<PurchaseVatRegisters> GetPurchaseVatRegisters(string fromdate, string toDate, Core.Domain.User userinfo)
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
            string query = @"SELECT BS_DATE (INVOICE_DATE) MITI,
                     INVOICE_NO as InvoiceNo,
                     PARTY_NAME as PartyName,
                     VAT_NO as VatNo,
                     FN_CONVERT_CURRENCY (NVL (GROSS_PURCHASE, 0) * NVL (EXCHANGE_RATE, 1),
                                          'NRS',
                                          INVOICE_DATE)  as GrossPurchase,
                     FN_CONVERT_CURRENCY (
                        NVL (TAXABLE_PURCHASE, 0) * NVL (EXCHANGE_RATE, 1) ,
                        'NRS',
                        INVOICE_DATE) AS TaxablePurchase,
                     FN_CONVERT_CURRENCY (NVL (VAT, 0) * NVL (EXCHANGE_RATE, 1) ,
                                          'NRS',
                                          INVOICE_DATE) as VatAmount,
                     FN_CONVERT_CURRENCY (NVL (TOTAL_PURCHASE, 0) * NVL (EXCHANGE_RATE, 1) ,
                                          'NRS',
                                          INVOICE_DATE) as TotalPurchase,
                     FORM_CODE as FormCode,
                     P_TYPE as Ptype,
                     MANUAL_NO as ManualNo,
                     TO_CHAR(INVOICE_DATE) as InvoiceDate
                FROM V$PURCHASE_INVOICE_REPORT3
                WHERE COMPANY_CODE = '" + userinfo.company_code + "'" +
                "AND BRANCH_CODE = '" + userinfo.branch_code + "'";
            if (!string.IsNullOrEmpty(fromdate))
                query = query + " AND INVOICE_DATE>=TO_DATE('" + fromdate + "', 'YYYY-MM-DD') and INVOICE_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')ORDER BY BS_DATE (INVOICE_DATE), MANUAL_NO, INVOICE_NO ";
            var purchaseVatRegisters = _objectEntity.SqlQuery<PurchaseVatRegisters>(query).ToList();
            return purchaseVatRegisters;
        }
        public List<PurchaseVatRegisters> GetPurchaseVatRegisters(ReportFiltersModel filters, User userInfo)
        {

            var figureAmountFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter);
            var roundUpAmountFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter);
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT BS_DATE (vpi.INVOICE_DATE) MITI,
                     NVL(NULLIF(vpi.INVOICE_NO,'Null'),'-') as InvoiceNo,
                     vpi.PARTY_NAME as PartyName,
                     NVL(NULLIF((NULLIF(vpi.VAT_NO,'')),'Null'),'-') as VatNo,
                     FN_CONVERT_CURRENCY (ROUND((NVL (vpi.GROSS_PURCHASE, 0) * NVL (vpi.EXCHANGE_RATE, 1))/{0},{1}),'NRS',vpi.INVOICE_DATE)  as GrossPurchase,
                     FN_CONVERT_CURRENCY (ROUND((NVL (vpi.TAXABLE_PURCHASE, 0) * NVL (vpi.EXCHANGE_RATE, 1))/{2},{3}) ,'NRS',vpi.INVOICE_DATE) AS TaxablePurchase,                     
                     FN_CONVERT_CURRENCY (ROUND((NVL (vpi.VAT, 0) * NVL (vpi.EXCHANGE_RATE, 1))/{4},{5}) ,'NRS',vpi.INVOICE_DATE) as VatAmount,
                     FN_CONVERT_CURRENCY (ROUND((NVL (vpi.TOTAL_PURCHASE, 0) * NVL (vpi.EXCHANGE_RATE, 1))/{6},{7}) ,'NRS',vpi.INVOICE_DATE) as TotalPurchase,
                     FORM_CODE as FormCode,
                     P_TYPE as Ptype,
                        SUPPLIER_CODE,
                     MANUAL_NO as ManualNo,
                     TO_CHAR(INVOICE_DATE) as InvoiceDate
                     FROM V$PURCHASE_INVOICE_REPORT3 vpi, 
                     --IP_ITEM_MASTER_SETUP IMS, 
                     --COMPANY_SETUP cmps, 
                     FA_BRANCH_SETUP bs
                     WHERE 
                     vpi.COMPANY_CODE IN(" + companyCode + @")
                     AND vpi.BRANCH_CODE = bs.BRANCH_CODE", figureAmountFilter,
                     roundUpAmountFilter, figureAmountFilter, roundUpAmountFilter,
                     figureAmountFilter, roundUpAmountFilter, figureAmountFilter, roundUpAmountFilter);

            if (filters.SupplierFilter.Count > 0)
            {
                var suppliers = filters.SupplierFilter;
                var supplierConditionQuery = string.Empty;
                for (int i = 0; i < suppliers.Count; i++)
                {

                    if (i == 0)
                        supplierConditionQuery += string.Format("MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%' from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", suppliers[i], companyCode);
                    else
                    {
                        supplierConditionQuery += string.Format(" OR  MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%'  from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", suppliers[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND vpi.SUPPLIER_CODE IN (SELECT DISTINCT(SUPPLIER_CODE) FROM IP_SUPPLIER_SETUP WHERE {0} OR (SUPPLIER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", supplierConditionQuery, string.Join("','", suppliers));
            }

            if (filters.ProductFilter.Count > 0)
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

                query = query + string.Format(@" AND vpi.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ({1}) AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join(",", products));
            }
            if (filters.PurchaseDocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND vpi.FORM_CODE  IN  ({0})", string.Join(",", filters.PurchaseDocumentFilter).ToString());
            }
            //if (filters.CategoryFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND C.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            //}
            //if (filters.CompanyFilter.Count > 0)
            //{
            //    query += string.Format(@" AND vpi.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            //}
            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND vpi.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }

            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " AND vpi.INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') AND vpi.INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";
            int min = 0, max = 0;
            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                query += string.Format(@" AND FN_CONVERT_CURRENCY (NVL (vpi.TOTAL_PURCHASE, 0) * NVL (vpi.EXCHANGE_RATE, 1) ,
                              'NRS',
                              vpi.INVOICE_DATE) >= {0} AND FN_CONVERT_CURRENCY (NVL (vpi.TOTAL_PURCHASE, 0) * NVL (vpi.EXCHANGE_RATE, 1) ,
                              'NRS',
                              vpi.INVOICE_DATE) <= {1}", min, max);
            }

            query = query + " ORDER BY BS_DATE (vpi.INVOICE_DATE), vpi.MANUAL_NO, vpi.INVOICE_NO ";

            var purchaseVatRegisters = _objectEntity.SqlQuery<PurchaseVatRegisters>(query).ToList();
            return purchaseVatRegisters;
        }
        public List<ChargesTitle> GetChargesTitle(User userinfo)
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
            string query = @"SELECT distinct CT.charge_code as ChargesHeaderNo, CC.CHARGE_EDESC as ChargesHeaderTitle
                                  FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC
                                WHERE 1=1
                                AND CT.CHARGE_CODE = CC.CHARGE_CODE
                                AND CT.Company_Code='" + userinfo.company_code + @"'
                                AND CT.Branch_Code='" + userinfo.branch_code + @"'
                                and form_code in (select form_code from form_detail_setup where table_name = 'IP_PURCHASE_INVOICE') and CT.apply_on='I'";
            var chargeTitles = _objectEntity.SqlQuery<ChargesTitle>(query).ToList();
            return chargeTitles;
        }
        public List<ItemsLandingCostViewModel> GetPurchaseItemsSummary(string fromDate, string toDate)
        {
            string query = @"select A.ITEM_CODE as ItemCode,A.Item_name as ItemName,A.Mu_edesc as Unit,A.QUNATITY as Quantity,A.TOTAL_PRICE as GrossAmount  from (
SELECT                                     
       PI.ITEM_CODE,                                    
       FN_FETCH_DESC('01', 'IP_ITEM_MASTER_SETUP', PI.ITEM_CODE) ITEM_NAME,                                    
       MC.MU_EDESC,                                    
      SUM( PI.QUANTITY) QUNATITY,                                         
      SUM( FN_CONVERT_CURRENCY (NVL ( PI.TOTAL_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE) )TOTAL_PRICE
  FROM IP_PURCHASE_INVOICE PI,                                                                      
       IP_MU_CODE MC                                    
 WHERE   PI.COMPANY_CODE = '01'                                                            
       AND PI.DELETED_FLAG = 'N'                                                                                                
       AND PI.MU_CODE = MC.MU_CODE ";
            if (!string.IsNullOrEmpty(fromDate))
                query = query + " AND PI.INVOICE_DATE>=TO_DATE('" + fromDate + "', 'YYYY-MM-DD') and PI.INVOICE_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            query = query + " GROUP BY PI.ITEM_CODE, FN_FETCH_DESC('01', 'IP_ITEM_MASTER_SETUP', PI.ITEM_CODE), MC.MU_EDESC  ORDER BY PI.ITEM_CODE  ) A";
            var itemsSummary = _objectEntity.SqlQuery<ItemsLandingCostViewModel>(query).ToList();
            return itemsSummary;
        }
        public List<ItemsLandingCostViewModel> GetPurchaseItemsSummary(ReportFiltersModel filters, User userinfo)
        {
            //var companyCode = string.Join(",", filters.CompanyFilter);
            //if (string.IsNullOrEmpty(companyCode))
            //    companyCode = userinfo.company_code;

            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var amountRoundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter);
            var amountFigureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter);
            var quantityRoundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter);
            var quantityFigureFilter = ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter);

            string query = string.Format(@"select A.ITEM_CODE as ItemCode,A.Item_name as ItemName,
                                            A.MU_EDESC as Unit,ROUND(A.QUNATITY/{0},{1}) as Quantity,
                                            ROUND(A.TOTAL_PRICE/{2},{3}) as GrossAmount, 
                                            ROUND((A.TOTAL_PRICE/A.QUNATITY)/{4},{5}) as RatePerUnit FROM 
                                            (
                                                 SELECT                                     
                                                 PI.ITEM_CODE,                                    
                                                 FN_FETCH_DESC('01', 'IP_ITEM_MASTER_SETUP', PI.ITEM_CODE) ITEM_NAME,                                    
                                                 MC.MU_EDESC,                                    
                                                 SUM( PI.QUANTITY) QUNATITY,                                         
                                                 SUM( FN_CONVERT_CURRENCY (NVL ( PI.TOTAL_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE) )TOTAL_PRICE
                                                 FROM IP_PURCHASE_INVOICE PI, IP_MU_CODE MC, IP_ITEM_MASTER_SETUP C                                   
                                                 WHERE   
                                                 PI.COMPANY_CODE IN(" + companyCode + @")                               
                                                 AND PI.DELETED_FLAG = 'N'
                                                 AND PI.ITEM_CODE = C.ITEM_CODE                                                                                                
                                                 AND PI.MU_CODE = MC.MU_CODE ", quantityFigureFilter, quantityRoundUpFilter, amountFigureFilter, amountRoundUpFilter, amountFigureFilter, amountRoundUpFilter);

            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " AND PI.INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and PI.INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";

            if (filters.SupplierFilter.Count > 0)
            {

                var suppliers = filters.SupplierFilter;
                var supplierConditionQuery = string.Empty;
                for (int i = 0; i < suppliers.Count; i++)
                {
                    if (i == 0)
                        supplierConditionQuery += string.Format("MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%' from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", suppliers[i], companyCode);
                    else
                    {
                        supplierConditionQuery += string.Format(" OR  MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%'  from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", suppliers[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND PI.SUPPLIER_CODE IN (SELECT DISTINCT(SUPPLIER_CODE) FROM IP_SUPPLIER_SETUP WHERE {0} OR (SUPPLIER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", supplierConditionQuery, string.Join("','", suppliers));
                //query += string.Format(@" AND PI.SUPPLIER_CODE IN ({0}) ", string.Join(",", filters.SupplierFilter).ToString());
            }

            if (filters.ProductFilter.Count > 0)
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

                query = query + string.Format(@" AND PI.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join("','", products));
                //query = query + string.Format(@" AND PI.ITEM_CODE IN ({0}) ", string.Join(",", filters.ProductFilter).ToString());
            }
            if (filters.PurchaseDocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND PI.FORM_CODE  IN  ('{0}')", string.Join("','", filters.PurchaseDocumentFilter).ToString());
            }
            if (filters.CategoryFilter.Count > 0)
            {
                query = query + string.Format(@" AND C.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {
                query += string.Format(@" AND PI.TO_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
            }
            //if (filters.CompanyFilter.Count > 0)
            //{
            //    query += string.Format(@" AND PI.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            //}
            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND PI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }


            query = query + " GROUP BY PI.ITEM_CODE, FN_FETCH_DESC('01', 'IP_ITEM_MASTER_SETUP', PI.ITEM_CODE), MC.MU_EDESC  ORDER BY PI.ITEM_CODE  ) A";

            bool whereClauseDeclared = false;
            int min = 0, max = 0;
            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
            {
                query += string.Format(@" WHERE A.QUNATITY >= {0} AND A.QUNATITY <= {1}", min, max);
                whereClauseDeclared = true;
            }


            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
            {
                if (whereClauseDeclared)
                {
                    query += string.Format(@" AND NVL(A.TOTAL_PRICE/A.QUNATITY,0) >= {0} AND NVL(A.TOTAL_PRICE/A.QUNATITY,0) <= {1}", min, max);
                }
                else
                {
                    whereClauseDeclared = true;
                    query += string.Format(@" WHERE NVL(A.TOTAL_PRICE/A.QUNATITY,0) >= {0} AND NVL(A.TOTAL_PRICE/A.QUNATITY,0) <= {1}", min, max);
                }

            }

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
            {
                if (whereClauseDeclared)
                {
                    query += string.Format(@" AND A.TOTAL_PRICE >= {0} AND A.TOTAL_PRICE <= {1}", min, max);
                }
                else
                {
                    query += string.Format(@" WHERE A.TOTAL_PRICE >= {0} AND A.TOTAL_PRICE <= {1}", min, max);
                }
            }


            var itemsSummary = _objectEntity.SqlQuery<ItemsLandingCostViewModel>(query).ToList();
            return itemsSummary;

        }
        public List<PurchaseCharges> GetPurchaseCharges(string formDate, string toDate)
        {
            string query = @" SELECT                                         
         IMS.ITEM_CODE,                                                                           
         CT.CHARGE_CODE,                                        
         CC.CHARGE_EDESC,                                        
         CT.CHARGE_TYPE_FLAG,                                                
         SUM(FN_CONVERT_CURRENCY (NVL ( CT.CHARGE_AMOUNT, 0) * NVL (CT.EXCHANGE_RATE, 1), 'NPR', PI.INVOICE_DATE ) ) CHARGE_AMOUNT                                        
    FROM CHARGE_TRANSACTION CT, IP_ITEM_MASTER_SETUP IMS, IP_CHARGE_CODE CC, IP_PURCHASE_INVOICE PI                                        
   WHERE                                             
        CT.DELETED_FLAG ='N'                                        
        AND  PI.DELETED_FLAG = 'N'                                        
        AND CT.COMPANY_CODE = '01'                                        
        AND PI.COMPANY_CODE = '01'                                        
         AND CT.ITEM_CODE = IMS.ITEM_CODE                                        
         AND CT.TABLE_NAME = 'IP_PURCHASE_INVOICE'                                        
         AND CT.CHARGE_CODE = CC.CHARGE_CODE                                        
         AND CT.REFERENCE_NO = PI.INVOICE_NO                                        
         AND CT.ITEM_CODE = PI.ITEM_CODE";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " AND PI.INVOICE_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and PI.INVOICE_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            query = query + "  GROUP BY  IMS.ITEM_CODE,IMS.ITEM_EDESC,CT.CHARGE_CODE,CC.CHARGE_EDESC,CT.CHARGE_TYPE_FLAG";
            var salesRegisters = _objectEntity.SqlQuery<PurchaseCharges>(query).ToList();
            return salesRegisters;
        }

        public List<PurchaseCharges> GetPurchaseCharges(ReportFiltersModel filters, User userInfo)
        {
            var companyCode = string.Join(",", filters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            string query = string.Format(@" SELECT                                         
         IMS.ITEM_CODE,                                                                           
         CT.CHARGE_CODE,                                        
         CC.CHARGE_EDESC,                                        
         CT.CHARGE_TYPE_FLAG,                                                
         ROUND(SUM(FN_CONVERT_CURRENCY (NVL ( CT.CHARGE_AMOUNT, 0) * NVL (CT.EXCHANGE_RATE, 1), 'NPR', PI.INVOICE_DATE ) )/{0},{1}) CHARGE_AMOUNT                                        
    FROM CHARGE_TRANSACTION CT, IP_ITEM_MASTER_SETUP IMS, IP_CHARGE_CODE CC, IP_PURCHASE_INVOICE PI                                        
   WHERE                                             
        CT.DELETED_FLAG ='N'                                        
        AND  PI.DELETED_FLAG = 'N'                                        
        AND CT.COMPANY_CODE IN ({2})                                  
        AND PI.COMPANY_CODE IN({2})                                        
         AND CT.ITEM_CODE = IMS.ITEM_CODE                                        
         AND CT.TABLE_NAME = 'IP_PURCHASE_INVOICE'                                        
         AND CT.CHARGE_CODE = CC.CHARGE_CODE                                        
         AND CT.REFERENCE_NO = PI.INVOICE_NO                                        
         AND CT.ITEM_CODE = PI.ITEM_CODE", ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter), companyCode);
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " AND PI.INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and PI.INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";

            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND PI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }

            query = query + "  GROUP BY  IMS.ITEM_CODE,IMS.ITEM_EDESC,CT.CHARGE_CODE,CC.CHARGE_EDESC,CT.CHARGE_TYPE_FLAG";
            var salesRegisters = _objectEntity.SqlQuery<PurchaseCharges>(query).ToList();
            return salesRegisters;
        }

        public List<InvoiceLandedCostSummary> GetPurchaseInvoiceSummary(string fromDate, string toDate)
        {
            string query = @"SELECT INVOICE_DATE as InvoiceDate,                                
       BS_DATE (INVOICE_DATE) as Miti,                                
       PI.INVOICE_NO as InvoiceNo,                                
       SS.SUPPLIER_EDESC as SupplierName,                                
       PI.ITEM_CODE as ItemCode,                                
       MC.MU_EDESC as Unit,                                
       PI.QUANTITY as Quantity,                                
       FN_CONVERT_CURRENCY (NVL ( PI.UNIT_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE) UnitPrice,                                
       FN_CONVERT_CURRENCY (NVL ( PI.TOTAL_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE) TotalPrice,                                       
       PI.P_TYPE,
       IMS.ITEM_EDESC as ItemName                           
  FROM IP_PURCHASE_INVOICE PI,                                
       CHARGE_TRANSACTION CT,                                
       IP_SUPPLIER_SETUP SS,                                
       IP_MU_CODE MC ,
       IP_ITEM_MASTER_SETUP IMS                   
 WHERE     CT.TABLE_NAME = 'IP_PURCHASE_INVOICE'                                
       AND PI.COMPANY_CODE = '01'                                
       AND CT.COMPANY_CODE = '01'                                
       AND PI.DELETED_FLAG = 'N'                                
       AND CT.DELETED_FLAG = 'N'                                
       AND PI.INVOICE_NO = CT.REFERENCE_NO                                
       AND PI.ITEM_CODE = CT.ITEM_CODE                                
       AND PI.SUPPLIER_CODE = SS.SUPPLIER_CODE                                
       AND PI.MU_CODE = MC.MU_CODE
       and PI.ITEM_CODE = IMS.ITEM_CODE";
            if (!string.IsNullOrEmpty(fromDate))
                query = query + " AND PI.INVOICE_DATE>=TO_DATE('" + fromDate + "', 'YYYY-MM-DD') and PI.INVOICE_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";

            var invoiceSummary = _objectEntity.SqlQuery<InvoiceLandedCostSummary>(query).ToList();
            return invoiceSummary;
        }
        public List<InvoiceLandedCostSummary> GetPurchaseInvoiceSummary(ReportFiltersModel filters, User userinfo)
        {
            var amountRoundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter);
            var amountFigureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter);
            var quantityRoundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter);
            var quantityFigureFilter = ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter);
            //var companyCode = string.Join(",", filters.CompanyFilter);
            //companyCode = companyCode == "" ? userinfo.company_code : companyCode;

            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT INVOICE_DATE as InvoiceDate,                                
               BS_DATE (INVOICE_DATE) as Miti,                                
               PI.INVOICE_NO as InvoiceNo,                                
               SS.SUPPLIER_EDESC as SupplierName,                                
               PI.ITEM_CODE as ItemCode,                                
               MC.MU_EDESC as Unit,                                
               Round(NVL(PI.QUANTITY,0)/{0},{1}) as Quantity,                                
               FN_CONVERT_CURRENCY (ROUND((NVL ( PI.UNIT_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1))/{2},{3}),'NPR', PI.INVOICE_DATE) UnitPrice,                                
               FN_CONVERT_CURRENCY (ROUND((NVL ( PI.TOTAL_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1))/{4},{5}),'NPR', PI.INVOICE_DATE) TotalPrice,                                       
               PI.P_TYPE,
               IMS.ITEM_EDESC as ItemName                           
          FROM IP_PURCHASE_INVOICE PI,                                
               CHARGE_TRANSACTION CT,                                
               IP_SUPPLIER_SETUP SS,                                
               IP_MU_CODE MC ,
               IP_ITEM_MASTER_SETUP IMS                  
         WHERE     CT.TABLE_NAME = 'IP_PURCHASE_INVOICE'                                
               AND PI.COMPANY_CODE = CT.COMPANY_CODE
               AND PI.COMPANY_CODE IN(" + companyCode + @")                          
               AND PI.DELETED_FLAG = 'N'                                
               AND CT.DELETED_FLAG = 'N'                                
               AND PI.INVOICE_NO = CT.REFERENCE_NO                                
               AND PI.ITEM_CODE = CT.ITEM_CODE                                
               AND PI.SUPPLIER_CODE = SS.SUPPLIER_CODE                                
               AND PI.MU_CODE = MC.MU_CODE
               and PI.ITEM_CODE = IMS.ITEM_CODE", quantityFigureFilter, quantityRoundUpFilter, amountFigureFilter, amountRoundUpFilter, amountFigureFilter, amountRoundUpFilter);

            if (filters.SupplierFilter.Count > 0)
            {
                var suppliers = filters.SupplierFilter;
                var supplierConditionQuery = string.Empty;
                for (int i = 0; i < suppliers.Count; i++)
                {

                    if (i == 0)
                        supplierConditionQuery += string.Format("MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%' from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", suppliers[i], companyCode);
                    else
                    {
                        supplierConditionQuery += string.Format(" OR  MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%'  from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", suppliers[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND PI.SUPPLIER_CODE IN (SELECT DISTINCT(SUPPLIER_CODE) FROM IP_SUPPLIER_SETUP WHERE {0} OR (SUPPLIER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", supplierConditionQuery, string.Join("','", suppliers));
                //query += string.Format(@" AND PI.SUPPLIER_CODE IN ({0}) ", string.Join(",", filters.SupplierFilter).ToString());
            }

            if (filters.ProductFilter.Count > 0)
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

                query = query + string.Format(@" AND PI.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join("','", products));
                //query = query + string.Format(@" AND PI.ITEM_CODE IN ({0}) ", string.Join(",", filters.ProductFilter).ToString());
            }
            if (filters.PurchaseDocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND PI.FORM_CODE  IN  ('{0}')", string.Join("','", filters.PurchaseDocumentFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {
                query += string.Format(@" AND PI.TO_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
            }
            if (filters.CategoryFilter.Count > 0)
            {
                query = query + string.Format(@" AND IMS.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            //if (filters.CompanyFilter.Count > 0)
            //{
            //    query += string.Format(@" AND PI.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            //}
            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND PI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }

            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " AND PI.INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and PI.INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";


            int min = 0, max = 0;
            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query += string.Format(@" AND NVL(PI.QUANTITY,0)>= {0} AND NVL(PI.QUANTITY,0)<={1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query += string.Format(@" AND FN_CONVERT_CURRENCY (NVL( PI.UNIT_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE)>={0} AND FN_CONVERT_CURRENCY (NVL ( PI.UNIT_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE)<={1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query += string.Format(@" AND FN_CONVERT_CURRENCY (NVL( PI.TOTAL_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE) >= {0} AND FN_CONVERT_CURRENCY (NVL ( PI.TOTAL_PRICE, 0) * NVL (PI.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE)<= {1}", min, max);

            var invoiceSummary = _objectEntity.SqlQuery<InvoiceLandedCostSummary>(query).ToList();
            return invoiceSummary;
        }
        public List<PurchaseCharges> GetPurchaseInvoiceCharges(string formDate, string toDate)
        {
            string query = @"SELECT CT.REFERENCE_NO as InvoiceNo,                                        
         IMS.ITEM_CODE as ITEM_CODE,                                        
         IMS.ITEM_EDESC,                                        
         CT.CHARGE_CODE,                                        
         CC.CHARGE_EDESC,                                        
         CT.CHARGE_TYPE_FLAG,                                                
         FN_CONVERT_CURRENCY (NVL ( CT.CHARGE_AMOUNT, 0) * NVL (CT.EXCHANGE_RATE, 1),'NPR', PI.INVOICE_DATE ) CHARGE_AMOUNT                                        
    FROM CHARGE_TRANSACTION CT, IP_ITEM_MASTER_SETUP IMS, IP_CHARGE_CODE CC, IP_PURCHASE_INVOICE PI                                        
   WHERE                                             
        CT.DELETED_FLAG ='N'                                        
        AND  PI.DELETED_FLAG = 'N'                                        
        AND CT.COMPANY_CODE = '01'                                        
        AND PI.COMPANY_CODE = '01'                                       
         AND CT.ITEM_CODE = IMS.ITEM_CODE                                        
         AND CT.TABLE_NAME = 'IP_PURCHASE_INVOICE'                                        
         AND CT.CHARGE_CODE = CC.CHARGE_CODE                                        
         AND CT.REFERENCE_NO = PI.INVOICE_NO                                        
         AND CT.ITEM_CODE = PI.ITEM_CODE";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " AND PI.INVOICE_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and PI.INVOICE_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";

            return _objectEntity.SqlQuery<PurchaseCharges>(query).ToList();

        }


        public List<PurchaseCharges> GetPurchaseInvoiceCharges(ReportFiltersModel filters, User userInfo)
        {
            var companyCode = string.Join(",", filters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            string query = string.Format(@"SELECT CT.REFERENCE_NO as InvoiceNo,                                        
         IMS.ITEM_CODE as ITEM_CODE,                                        
         IMS.ITEM_EDESC,                                        
         CT.CHARGE_CODE,                                        
         CC.CHARGE_EDESC,                                        
         CT.CHARGE_TYPE_FLAG,                                                
         FN_CONVERT_CURRENCY (ROUND((NVL ( CT.CHARGE_AMOUNT, 0) * NVL (CT.EXCHANGE_RATE, 1))/{0},{1}),'NPR', PI.INVOICE_DATE ) CHARGE_AMOUNT                                        
            FROM CHARGE_TRANSACTION CT, IP_ITEM_MASTER_SETUP IMS, IP_CHARGE_CODE CC, IP_PURCHASE_INVOICE PI                                        
           WHERE                                             
        CT.DELETED_FLAG ='N'                                        
        AND  PI.DELETED_FLAG = 'N'                                        
        AND CT.COMPANY_CODE IN({2})                                        
        AND PI.COMPANY_CODE IN({2})                                      
         AND CT.ITEM_CODE = IMS.ITEM_CODE                                        
         AND CT.TABLE_NAME = 'IP_PURCHASE_INVOICE'                                        
         AND CT.CHARGE_CODE = CC.CHARGE_CODE                                        
         AND CT.REFERENCE_NO = PI.INVOICE_NO                                        
         AND CT.ITEM_CODE = PI.ITEM_CODE", ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter), companyCode);
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " AND PI.INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and PI.INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";

            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND PI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }

            return _objectEntity.SqlQuery<PurchaseCharges>(query).ToList();

        }

        public List<VoucherModel> GetPurchaseRegisterVouchers()
        {
            string query = @"SELECT 
                            DISTINCT FS.FORM_CODE VoucherCode, 
                            INITCAP(FS.FORM_EDESC) VoucherName
                            FROM FORM_DETAIL_SETUP DS, FORM_SETUP FS
                            WHERE table_name  IN ( 'IP_PURCHASE_INVOICE', 'IP_PURCHASE_RETURN')                           
                            AND FS.DELETED_FLAG = 'N'
                            AND FS.FORM_CODE = DS.FORM_CODE
                            AND FS.COMPANY_CODE = DS.COMPANY_CODE
                            ORDER BY INITCAP(FS.FORM_EDESC)
                            ";
            var voucherList = _objectEntity.SqlQuery<VoucherModel>(query).ToList();
            return voucherList;
        }
        public List<VoucherModel> GetPurchaseRegisterVouchers(User userinfo)
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
            string query = @"SELECT 
                            DISTINCT FS.FORM_CODE VoucherCode, 
                            INITCAP(FS.FORM_EDESC) VoucherName
                            FROM FORM_DETAIL_SETUP DS, FORM_SETUP FS
                            WHERE table_name  IN ( 'IP_PURCHASE_INVOICE', 'IP_PURCHASE_RETURN')
                            AND FS.COMPANY_CODE = '" + userinfo.company_code + @"'
                            AND FS.DELETED_FLAG = 'N'
                            AND FS.FORM_CODE = DS.FORM_CODE
                            AND FS.COMPANY_CODE = DS.COMPANY_CODE
                            ORDER BY INITCAP(FS.FORM_EDESC)
                            ";
            var voucherList = _objectEntity.SqlQuery<VoucherModel>(query).ToList();
            return voucherList;
        }

        public List<VoucherSetupModel> GetAllVoucherNodes()
        {
            string query = @"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(FORM_NDESC) AS VoucherName,
                                        FORM_CODE AS VoucherCode,
                                        MASTER_FORM_CODE AS MasterFormCode, 
                                        PRE_FORM_CODE AS PreFormCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM FORM_SETUP fs
                                        WHERE fs.DELETED_FLAG = 'N'                                         
                                        AND GROUP_SKU_FLAG = 'G' 
                                        AND LEVEL = 1 
                                        START WITH PRE_FORM_CODE = '00'
                                        CONNECT BY PRIOR MASTER_FORM_CODE = PRE_FORM_CODE
                                        ORDER SIBLINGS BY FORM_NDESC";
            var VoucherListNodes = _objectEntity.SqlQuery<VoucherSetupModel>(query).ToList();
            return VoucherListNodes;
        }

        public List<VoucherSetupModel> GetAllVoucherNodes(User userinfo)
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
            string query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(FORM_NDESC) AS VoucherName,
                                        FORM_CODE AS VoucherCode,
                                        MASTER_FORM_CODE AS MasterFormCode, 
                                        PRE_FORM_CODE AS PreFormCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM FORM_SETUP fs
                                        WHERE fs.DELETED_FLAG = 'N' 
                                        AND fs.COMPANY_CODE = '" + userinfo.company_code + @"'
                                        AND GROUP_SKU_FLAG = 'G' 
                                        AND LEVEL = 1 
                                        START WITH PRE_FORM_CODE = '00'
                                        CONNECT BY PRIOR MASTER_FORM_CODE = PRE_FORM_CODE
                                        ORDER SIBLINGS BY FORM_NDESC";
            var VoucherListNodes = _objectEntity.SqlQuery<VoucherSetupModel>(query).ToList();
            return VoucherListNodes;
        }

        public List<VoucherSetupModel> GetVoucherListByFormCode(string level, string masterSupplierCode, User userinfo)
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
            string query = string.Format(@"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(FORM_NDESC) AS VoucherName,
                                        FORM_CODE AS VoucherCode,
                                        MASTER_FORM_CODE AS MasterFormCode, 
                                        PRE_FORM_CODE AS PreFormCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM FORM_SETUP fs
                                        WHERE fs.DELETED_FLAG = 'N' 
                                        AND fs.COMPANY_CODE = '" + userinfo.company_code + @"'
                                        --AND GROUP_SKU_FLAG = 'G'
                                        AND LEVEL = {0} 
                                       -- AND fs.FORM_CODE IN 
                                       --   (
                                       --    SELECT DISTINCT FS.FORM_CODE
                                       --       FROM form_detail_setup DS, FORM_SETUP FS
                                       --      WHERE  FS.COMPANY_CODE = '01'
                                       --      AND FS.DELETED_FLAG = 'N'
                                       --      AND table_name IN ( 'IP_PURCHASE_INVOICE', 'IP_PURCHASE_RETURN')
                                       --      AND FS.FORM_CODE = DS.FORM_CODE
                                       --      AND FS.COMPANY_CODE = DS.COMPANY_CODE
                                       --   ) 
                                        START WITH PRE_FORM_CODE = {1}
                                        CONNECT BY PRIOR MASTER_FORM_CODE = PRE_FORM_CODE
                                        ORDER SIBLINGS BY FORM_NDESC", level.ToString(), masterSupplierCode.ToString());
            var voucherListNodes = _objectEntity.SqlQuery<VoucherSetupModel>(query).ToList();
            return voucherListNodes;
        }

        public List<LocationModel> GetStorageLocations()
        {
            string query = @"SELECT DISTINCT LOCATION_CODE AS LocationCode,
                                 LOCATION_EDESC AS LocationName
                                 FROM IP_LOCATION_SETUP
                                 WHERE DELETED_FLAG = 'N'";
            var locationList = _objectEntity.SqlQuery<LocationModel>(query).ToList();
            return locationList;
        }

        public List<LocationModel> GetStorageLocations(User userinfo)
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
            string query = @"SELECT LOCATION_CODE AS LocationCode,
                                 LOCATION_EDESC AS LocationName
                                 FROM IP_LOCATION_SETUP
                                 WHERE DELETED_FLAG = 'N'
                                 AND COMPANY_CODE = '" + userinfo.company_code + "'";
            var locationList = _objectEntity.SqlQuery<LocationModel>(query).ToList();
            return locationList;
        }

        public List<LocationSetupModel> GetAllStorageLocationNodes()
        {
            string query = @"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(LOCATION_EDESC) AS LocationName,
                                        LOCATION_CODE AS LocationCode,
                                        LOCATION_CODE AS MasterLocationCode, 
                                        PRE_LOCATION_CODE AS PreLocationCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM IP_LOCATION_SETUP ls
                                        WHERE ls.DELETED_FLAG = 'N'                                         
                                        AND LEVEL = 1 
                                        START WITH PRE_LOCATION_CODE = '00'
                                        CONNECT BY PRIOR LOCATION_CODE = PRE_LOCATION_CODE
                                       /* ORDER SIBLINGS BY LOCATION_EDESC*/ ";
            var LocationListNodes = _objectEntity.SqlQuery<LocationSetupModel>(query).ToList();
            return LocationListNodes;
        }

        public List<LocationSetupModel> GetLocationListByLocationId(string level, string masterLocationCode)
        {
            string query = string.Format(@"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(LOCATION_EDESC) AS LocationName,
                                        LOCATION_CODE AS LocationCode,
                                        LOCATION_CODE AS MasterLocationCode, 
                                        PRE_LOCATION_CODE AS PreLocationCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM IP_LOCATION_SETUP ls
                                        WHERE DELETED_FLAG = 'N'                                         
                                        AND LEVEL = {0}
                                        START WITH PRE_LOCATION_CODE = {1}
                                        CONNECT BY PRIOR LOCATION_CODE = PRE_LOCATION_CODE
                                        /*ORDER SIBLINGS BY LOCATION_EDESC*/", level.ToString(), masterLocationCode.ToString());
            var locationListNodes = _objectEntity.SqlQuery<LocationSetupModel>(query).ToList();
            return locationListNodes;
        }

        public List<CompanyModel> GetCompaniesModelList()
        {
            string query = @"SELECT 
                                COMPANY_CODE AS CompanyCode,
                                COMPANY_EDESC AS CompanyName
                                FROM COMPANY_SETUP
                                WHERE
                                DELETED_FLAG = 'N'";
            var companiesList = _objectEntity.SqlQuery<CompanyModel>(query).ToList();
            return companiesList;
        }

        public List<AccountsModel> GetAccountsList()
        {
            //string query = @"SELECT  ACC_EDESC AccountName, ACC_CODE AccountCode FROM FA_CHART_OF_ACCOUNTS_SETUP where ACC_EDESC  like '%Debtors%' or ACC_EDESC like '%TDS%' and 
            //                    DELETED_FLAG = 'N'";
            string query = $@"SELECT  ACC_EDESC AccountName, ACC_CODE AccountCode FROM FA_CHART_OF_ACCOUNTS_SETUP
                     WHERE DELETED_FLAG='N'
                     AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' 
                     AND ACC_TYPE_FLAG='T'
                     ORDER BY ACC_EDESC";
            var accountList = _objectEntity.SqlQuery<AccountsModel>(query).ToList();
            return accountList;
        }

        public List<VoucherSetupModel> GetVoucherListByFormCode(string level, string masterSupplierCode)
        {
            string query = string.Format(@"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(FORM_NDESC) AS VoucherName,
                                        FORM_CODE AS VoucherCode,
                                        MASTER_FORM_CODE AS MasterFormCode, 
                                        PRE_FORM_CODE AS PreFormCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM FORM_SETUP fs
                                        WHERE fs.DELETED_FLAG = 'N'                                         
                                        --AND GROUP_SKU_FLAG = 'G'
                                        AND LEVEL = {0} 
                                       -- AND fs.FORM_CODE IN 
                                       --   (
                                       --    SELECT DISTINCT FS.FORM_CODE
                                       --       FROM form_detail_setup DS, FORM_SETUP FS
                                       --      WHERE  FS.COMPANY_CODE = '01'
                                       --      AND FS.DELETED_FLAG = 'N'
                                       --      AND table_name IN ( 'IP_PURCHASE_INVOICE', 'IP_PURCHASE_RETURN')
                                       --      AND FS.FORM_CODE = DS.FORM_CODE
                                       --      AND FS.COMPANY_CODE = DS.COMPANY_CODE
                                       --   ) 
                                        START WITH PRE_FORM_CODE = {1}
                                        CONNECT BY PRIOR MASTER_FORM_CODE = PRE_FORM_CODE
                                        ORDER SIBLINGS BY FORM_NDESC", level.ToString(), masterSupplierCode.ToString());
            var voucherListNodes = _objectEntity.SqlQuery<VoucherSetupModel>(query).ToList();
            return voucherListNodes;
        }
        public List<PurchaseRegisterMoreDetail> GetPurchaseRegisterPivort(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            //var companyCode = string.Join(",", filters.CompanyFilter);
            //if (string.IsNullOrEmpty(companyCode))
            //    companyCode = userinfo.company_code;
            //companyCode = companyCode == "" ? userinfo.company_code : companyCode;

            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            string query = string.Format(@"SELECT A.INVOICE_NO,A.INVOICE_DATE,A.MANUAL_NO,A.SUPPLIER_CODE , B.SUPPLIER_EDESC SUPPLIER_NAME ,A.SUPPLIER_INV_NO, A.SUPPLIER_MRR_NO, A.SUPPLIER_INV_DATE , 
                    A.PP_NO, A.PP_DATE, A.REMARKS, A.DUE_DATE, A.CURRENCY_CODE, A.EXCHANGE_RATE ,C.LOCATION_EDESC,
                    D.ITEM_EDESC , Round(NVL(A.QUANTITY,0)/{0},{1}) QUANTITY ,Round(NVL(FN_CONVERT_CURRENCY(NVL(A.UNIT_PRICE,0),'NRS',A.INVOICE_DATE),0)/{2},{3}) as UNIT_PRICE, Round(NVL(FN_CONVERT_CURRENCY(NVL(A.TOTAL_PRICE,0),'NRS',A.INVOICE_DATE),0)/{2},{3}) TOTAL_PRICE, E.FORM_EDESC,
                    SUBSTR(FN_FETCH_GROUP_DESC(A.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', D.PRE_ITEM_CODE), 1, 100) ITEM_GROUP_EDESC,
                    SUBSTR(FN_FETCH_PRE_DESC(A.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', D.PRE_ITEM_CODE), 1, 100) ITEM_SUBGROUP_EDESC,
                    FN_FETCH_DESC(A.COMPANY_CODE, 'IP_CATEGORY_CODE', D.CATEGORY_CODE) CATEGORY_EDESC,
                    FN_FETCH_DESC(A.COMPANY_CODE, 'COMPANY_SETUP', A.COMPANY_CODE) AS COMPANYEDESC,
                      FN_FETCH_DESC(A.COMPANY_CODE, 'FA_BRANCH_SETUP', A.BRANCH_CODE) AS BranchEdesc
                    FROM IP_PURCHASE_INVOICE A , IP_SUPPLIER_SETUP B, IP_LOCATION_SETUP C, IP_ITEM_MASTER_SETUP D, FORM_SETUP E
                    WHERE A.SUPPLIER_CODE = B.SUPPLIER_CODE
                    AND A.COMPANY_CODE = B.COMPANY_CODE
                    AND A.TO_LOCATION_CODE = C.LOCATION_CODE(+)
                    AND A.COMPANY_CODE = C.COMPANY_CODE(+)
                    AND A.ITEM_CODE = D.ITEM_CODE
                    AND A.COMPANY_CODE = D.COMPANY_CODE
                    AND A.FORM_CODE = E.FORM_CODE
                    AND A.COMPANY_CODE = E.COMPANY_CODE
                    and A.DELETED_FLAG = 'N' AND A.COMPANY_CODE IN ({4})", ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter)
                    , ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter),companyCode);
            if (filters.SupplierFilter.Count > 0)
            {
                var suppliers = filters.SupplierFilter;
                var supplierConditionQuery = string.Empty;
                for (int i = 0; i < suppliers.Count; i++)
                {

                    if (i == 0)
                        supplierConditionQuery += string.Format("MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%' from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", suppliers[i], companyCode);
                    else
                    {
                        supplierConditionQuery += string.Format(" OR  MASTER_SUPPLIER_CODE like (Select DISTINCT(MASTER_SUPPLIER_CODE) || '%'  from IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", suppliers[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND A.SUPPLIER_CODE IN (SELECT DISTINCT(SUPPLIER_CODE) FROM IP_SUPPLIER_SETUP WHERE {0} OR (SUPPLIER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", supplierConditionQuery, string.Join("','", suppliers));
                //query += string.Format(@" AND PI.SUPPLIER_CODE IN ({0}) ", string.Join(",", filters.SupplierFilter).ToString());
            }
            if (filters.ProductFilter.Count() > 0)
            {
                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND A.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG = 'I')) ", productConditionQuery, string.Join("','", products));
                //var productFilter = @"select  DISTINCT TRIM(item_code) from IP_ITEM_MASTER_SETUP where (";
                ////IF PRODUCT_SKU_FLAG = G
                //foreach (var company in filters.CompanyFilter)
                //{
                //    foreach (var item in filters.ProductFilter)
                //    {
                //        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE ='" + company + "') OR ";
                //    }
                //}
                //productFilter = productFilter.Substring(0, productFilter.Length - 3);
                ////IF PRODUCT_SKU_FLAG = I                
                //productFilter += " OR (ITEM_CODE in (" + string.Join(",", filters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + string.Join(",", filters.CompanyFilter) + "))) ";
                //productFilter = " AND A.ITEM_CODE IN(" + productFilter + ")";

                //query += productFilter;
            }

            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            if (filters.PurchaseDocumentFilter.Count > 0)
            {
                query += string.Format(@" AND A.FORM_CODE IN ('{0}')", string.Join("','", filters.PurchaseDocumentFilter).ToString());
            }

            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " AND A.INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and A.INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";

            int min = 0, max = 0;
            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                query += string.Format(@" AND Round(NVL(FN_CONVERT_CURRENCY(NVL(A.TOTAL_PRICE,0),'NRS',A.INVOICE_DATE),0)/{2},{3}) >= {0} AND Round(NVL(FN_CONVERT_CURRENCY(NVL(A.TOTAL_PRICE,0),'NRS',A.INVOICE_DATE),0)/{2},{3}) <= {1}", min, max, ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter));
            }


            var salesRegisters = _objectEntity.SqlQuery<PurchaseRegisterMoreDetail>(query).ToList();
            return salesRegisters;
        }

        public List<ItemCode> GetItemList()
        {
            try
            {
                string selectQuery = @"SELECT ITEM_GROUP_CODE,ITEM_GROUP_NAME,GROUP_FLAG GROUP_TYPE,DELETED_FLAG FROM BI_ITEM_GROUP_SETUP ORDER BY ITEM_GROUP_CODE";
                var result = _objectEntity.SqlQuery<ItemCode>(selectQuery).ToList();
                return result;
            }
            catch(Exception ex)
            {
                return new List<ItemCode>();
            }
            
        }
        public List<ItemCode> GetItemCode(string ITEM_GROUP_CODE)
        {
            string selectQuery = $@"SELECT ITEM_CODE FROM BI_ITEM_GROUP_MAP WHERE ITEM_GROUP_CODE ='{ITEM_GROUP_CODE}' ";
            var result = _objectEntity.SqlQuery<ItemCode>(selectQuery).ToList();
            return result;
        }

        public string InsertItem(ItemSetupModal modal)
        {
            var trans = _objectEntity.Database.BeginTransaction();
            try
            {
                string maxValue = @"SELECT NVL(MAX(TO_NUMBER(ITEM_GROUP_CODE)) + 1, 1) MAXID FROM BI_ITEM_GROUP_SETUP";
                var ItemCode = _objectEntity.SqlQuery<decimal>(maxValue).FirstOrDefault();
                if (modal.ITEM_GROUP_CODE == 0)
                {
                    string existQuery = $@"SELECT count(*) FROM BI_ITEM_GROUP_SETUP WHERE ITEM_GROUP_NAME='{modal.ITEM_GROUP_NAME}'";
                    int count = _objectEntity.SqlQuery<int>(existQuery).FirstOrDefault();
                    if (count > 0)
                    {
                        return "Group Name already exsits";
                    }
                    else
                    {
                        var itemCodeList = string.Empty;
                        if (modal.ITEM_CODE.Count() > 0)
                            itemCodeList = string.Join(",", modal.ITEM_CODE).ToString();
                        string DELETED_FLAG = "N";
                        string insertQuery = $@"INSERT INTO BI_ITEM_GROUP_SETUP(ITEM_GROUP_CODE,ITEM_GROUP_NAME,GROUP_FLAG,DELETED_FLAG) VALUES('{ItemCode}','{modal.ITEM_GROUP_NAME}','{modal.GROUP_TYPE}','{DELETED_FLAG}')";
                        var data = _objectEntity.ExecuteSqlCommand(insertQuery);
                        var response = this.insertGroupMap(modal, ItemCode.ToString());
                        if (response == "Insert successfully")
                        {
                            trans.Commit();
                            return "Inserted successfully";
                        }
                        else
                            throw new Exception($@"Item not inserted.");
                    }
                }
                else
                {
                    string existQuery = $@"SELECT count(*) FROM BI_ITEM_GROUP_SETUP WHERE ITEM_GROUP_NAME='{modal.ITEM_GROUP_NAME}' AND ITEM_GROUP_CODE <>'{modal.ITEM_GROUP_CODE}' ";
                    int count = _objectEntity.SqlQuery<int>(existQuery).FirstOrDefault();
                    if (count > 0)
                    {
                        return "Group Name already exsits";
                    }
                    string updateQuery = $@"UPDATE BI_ITEM_GROUP_SETUP SET ITEM_GROUP_NAME='{modal.ITEM_GROUP_NAME}',GROUP_FLAG='{modal.GROUP_TYPE}' WHERE ITEM_GROUP_CODE='{modal.ITEM_GROUP_CODE}'";
                    var result = _objectEntity.ExecuteSqlCommand(updateQuery);
                    var response = this.insertGroupMap(modal, modal.ITEM_GROUP_CODE.ToString());
                    if (response == "Insert successfully")
                    {
                        trans.Commit();
                        return "Update SuccessFully";
                    }
                    else
                        throw new Exception($@"Item not inserted.");
                    
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                //return ex.Message;
                throw new Exception(ex.Message);
                //throw ex;
            }
        }
        public string insertGroupMap(ItemSetupModal modal, string ItemGroupCode)
        {
            try
            {
                if (modal.ITEM_GROUP_CODE != 0)
                {
                    string deleteQuery = $@"DELETE FROM BI_ITEM_GROUP_MAP WHERE ITEM_GROUP_CODE ='{modal.ITEM_GROUP_CODE}'";
                    _objectEntity.ExecuteSqlCommand(deleteQuery);
                    // _objectEntity.SaveChanges();

                }

                var subType = modal.ITEM_CODE;
                //string maxValue = @"SELECT NVL(MAX(TO_NUMBER(ITEM_GROUP_CODE)) + 1, 1) MAXID FROM BI_ITEM_GROUP_MAP";
                //var ItemCode = _objectEntity.SqlQuery<decimal>(maxValue).FirstOrDefault();
                for (int i = 0; i < modal.ITEM_CODE.Count; i++)
                {
                    var productItemCode = subType[i];

                    // for insert only item category instead of group category
                    var subList = GetChildProduct(productItemCode);
                    foreach(string itemCode in subList)
                    {
                        string insertQuery = $@"INSERT INTO BI_ITEM_GROUP_MAP(ITEM_GROUP_CODE,ITEM_CODE) VALUES('{ItemGroupCode}','{itemCode}')";
                        //string insertQuery = $@"INSERT INTO BI_ITEM_GROUP_MAP(ITEM_GROUP_CODE,ITEM_CODE) VALUES('{ItemCode}','{productItemCode}')";
                        var data = _objectEntity.ExecuteSqlCommand(insertQuery);
                    }
                    
                }
                return "Insert successfully";
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        private List<string> GetChildProduct(string itemCode)
        {
            //List<ProductModel> list = new List<ProductModel>();
            List<string> list = new List<string>();
            string query = $@"SELECT GROUP_SKU_FLAG GroupFlag,ITEM_CODE ItemCode,MASTER_ITEM_CODE MasterItemCode, PRE_ITEM_CODE PreItemCode FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE ='{itemCode}'";
            ProductModel res = _objectEntity.SqlQuery<ProductModel>(query).FirstOrDefault();
            if(res.GroupFlag.ToLower()=="i")
            {
                list.Add(itemCode);
            }
            else
            {
                string query1 = $@"SELECT DISTINCT LEVEL, 
                INITCAP(ITEM_EDESC) AS ItemName,
                ITEM_CODE AS ItemCode,
                MASTER_ITEM_CODE AS MasterItemCode, 
                PRE_ITEM_CODE AS PreItemCode, 
                GROUP_SKU_FLAG AS GroupFlag
                FROM IP_ITEM_MASTER_SETUP ims
                WHERE ims.DELETED_FLAG = 'N' 
                AND GROUP_SKU_FLAG = 'I'      
                START WITH PRE_ITEM_CODE = '{res.MasterItemCode}'
                CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
                var res1 = _objectEntity.SqlQuery<ProductModel>(query1).ToList();
                foreach(var item in res1)
                {
                    list.Add(item.ItemCode);
                }

            }
            return list;
        }

        public string DeleteItemCode(string ITEM_GROUP_CODE)
        {
            string deleteQuery = $@"DELETE BI_ITEM_GROUP_SETUP WHERE ITEM_GROUP_CODE ='{ITEM_GROUP_CODE}'";
            _objectEntity.ExecuteSqlCommand(deleteQuery);
            this.DeleteMapingCode(ITEM_GROUP_CODE);
            return "Delete Successfuly";
        }

        public string DeleteMapingCode(string ITEM_GROUP_CODE)
        {
            string deleteQuery = $@"DELETE BI_ITEM_GROUP_MAP WHERE ITEM_GROUP_CODE ='{ITEM_GROUP_CODE}'";
            _objectEntity.ExecuteSqlCommand(deleteQuery);
            return "Delete Successfully";
        }
    }
}
