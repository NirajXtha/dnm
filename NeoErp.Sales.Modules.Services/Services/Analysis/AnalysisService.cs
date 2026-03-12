using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.AnalysisReport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services.Analysis
{
    public class AnalysisService : IAnalysisService
    {
        private readonly NeoErpCoreEntity _objectEntity;
        public AnalysisService(NeoErpCoreEntity objectEntity)
        {
            _objectEntity = objectEntity;
        }

        public IList<AvgAgingAndPaymentRotationSOSMWiseModel> GetAverageAgingAndPaymentRotationSalesManagerAndOfficerWise(ReportFiltersModel model, User userInfo)
        {
            var avgAgingAndPaymentRotationSOSMWiseData = new List<AvgAgingAndPaymentRotationSOSMWiseModel>();
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var Query = string.Empty;
            if (model.PartyTypeFilter.Count > 0)
            {
                Query = @"SELECT A.PARTY_TYPE_CODE AS PartyTypeCode
								,A.PRE_PARTY_CODE AS PrePartyCode
								,A.MASTER_PARTY_CODE AS MasterPartyCode
								,A.REMARKS AS Remarks
								,A.PARTY_TYPE_EDESC AS PartyTypeName
								,A.PARTY_NAME AS PartyName
								,TO_CHAR(A.CREDIT_LIMIT) AS CreditLimit
								,A.BALANCE AS Balance
								,A.QUANITTY AS Quantity
								,A.PAY_ROTATION_DAYS AS PayRotationDays
								,A.IS_CHILD AS IsChild
							FROM (
								SELECT PARTY_TYPE_CODE
									,PRE_PARTY_CODE
									,MASTER_PARTY_CODE
									,REMARKS
									,PARTY_TYPE_EDESC
									,RPAD('*', 2 * LEVEL, '*') || PARTY_TYPE_EDESC PARTY_NAME
									,(
										SELECT SUM(P2.CREDIT_LIMIT)
										FROM IP_PARTY_TYPE_CODE P2 START WITH P2.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE CONNECT BY PRIOR P2.MASTER_PARTY_CODE = P2.PRE_PARTY_CODE
										) CREDIT_LIMIT
									,(
										SELECT SUM(DR_AMOUNT - CR_AMOUNT)
										FROM MV$VIRTUAL_SUB_DEALER_LEDGER
										WHERE VOUCHER_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
											AND PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
										) BALANCE
									,(
										SELECT SUM(QUANTITY)
										FROM V$SALES_INVOICE_REPORT
										WHERE PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
											AND SALES_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
										) QUANITTY
									,FN_AVG_PAYBACKDAYS('D', P1.PARTY_TYPE_CODE, TO_DATE('{0}', 'YYYY-MM-DD')) PAY_ROTATION_DAYS
									,CONNECT_BY_ISLEAF IS_CHILD
								FROM IP_PARTY_TYPE_CODE P1
								WHERE P1.PARTY_TYPE_CODE IN ('{2}') START
								WITH P1.PRE_PARTY_CODE LIKE '00'
									AND P1.PARTY_TYPE_FLAG = 'D'
									AND P1.GROUP_SKU_FLAG = 'G'
									AND P1.PRE_PARTY_CODE = '00' CONNECT BY PRIOR P1.MASTER_PARTY_CODE = P1.PRE_PARTY_CODE
								) A";
                Query = string.Format(Query, model.FromDate, model.ToDate, string.Join("','", model.PartyTypeFilter));
            }
            else
            {
                Query = @"SELECT A.PARTY_TYPE_CODE AS PartyTypeCode
								,A.PRE_PARTY_CODE AS PrePartyCode
								,A.MASTER_PARTY_CODE AS MasterPartyCode
								,A.REMARKS AS Remarks
								,A.PARTY_TYPE_EDESC AS PartyTypeName
								,A.PARTY_NAME AS PartyName
								,TO_CHAR(A.CREDIT_LIMIT) AS CreditLimit
								,A.BALANCE AS Balance
								,A.QUANITTY AS Quantity
								,A.PAY_ROTATION_DAYS AS PayRotationDays
								,A.IS_CHILD AS IsChild
							FROM (
								SELECT PARTY_TYPE_CODE
									,PRE_PARTY_CODE
									,MASTER_PARTY_CODE
									,REMARKS
									,PARTY_TYPE_EDESC
									,RPAD('*', 2 * LEVEL, '*') || PARTY_TYPE_EDESC PARTY_NAME
									,(
										SELECT SUM(P2.CREDIT_LIMIT)
										FROM IP_PARTY_TYPE_CODE P2 START WITH P2.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE CONNECT BY PRIOR P2.MASTER_PARTY_CODE = P2.PRE_PARTY_CODE
										) CREDIT_LIMIT
									,(
										SELECT SUM(DR_AMOUNT - CR_AMOUNT)
										FROM MV$VIRTUAL_SUB_DEALER_LEDGER
										WHERE VOUCHER_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
											AND PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
										) BALANCE
									,(
										SELECT SUM(QUANTITY)
										FROM V$SALES_INVOICE_REPORT
										WHERE PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
											AND SALES_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
										) QUANITTY
									,FN_AVG_PAYBACKDAYS('D', P1.PARTY_TYPE_CODE, TO_DATE('{0}', 'YYYY-MM-DD')) PAY_ROTATION_DAYS
									,CONNECT_BY_ISLEAF IS_CHILD
								FROM IP_PARTY_TYPE_CODE P1
								WHERE P1.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE START
								WITH P1.PRE_PARTY_CODE LIKE '00'
									AND P1.PARTY_TYPE_FLAG = 'D'
									AND P1.GROUP_SKU_FLAG = 'G'
									AND P1.PRE_PARTY_CODE = '00' CONNECT BY PRIOR P1.MASTER_PARTY_CODE = P1.PRE_PARTY_CODE
								) A";
                Query = string.Format(Query, model.FromDate, model.ToDate);
            }

            avgAgingAndPaymentRotationSOSMWiseData = this._objectEntity.SqlQuery<AvgAgingAndPaymentRotationSOSMWiseModel>(Query).ToList();

            return avgAgingAndPaymentRotationSOSMWiseData;

        }

        public IList<DealerSalesRangeModel> GetDealerSalesRange(ReportFiltersModel model, User userInfo)
        {
            var dealerSalesRangeData = new List<DealerSalesRangeModel>();
            var Query = string.Empty;
            if (model.PartyTypeFilter.Count > 0)
            {
                Query = @"SELECT DEALER_SALES_RANGE AS DealerSalesRange
								,COUNT(*) Count
								,(ROUND(COUNT(*) / PCNT, 2) * 100) DealerCountRatio
								,SUM(PTSALES) TotalSales
								,ROUND((SUM(PTSALES) / GSALES) * 100, 2) SalesRatio
								,SUM(OUTSTANDING_AMT) TotalOutStanding
								,ROUND((SUM(OUTSTANDING_AMT) / TOTAL_OUTSTATINGS) * 100, 2) OutStandingRatio
								,ROUND(DAVG_DAYS, 2) AverageAging
								,PAYMENT_ROTATION_DAY PaymentRotationDay
							FROM (
								SELECT PTB.*
									,SUM(OUTSTANDING_AMT) OVER () TOTAL_OUTSTATINGS
									,COUNT(PARTY_TYPE_CODE) OVER () PCNT
									,AVG(AVG_DAYS) OVER (PARTITION BY DEALER_SALES_RANGE) DAVG_DAYS
								FROM (
									SELECT DISTINCT PARTY_TYPE_CODE
										,GSALES
										,PTSALES
										,(
											CASE 
												WHEN PTSALES <= 5000
													THEN 'A: 0 - 5000'
												WHEN PTSALES > 5000
													AND PTSALES <= 10000
													THEN 'B: 5000 - 10000'
												WHEN PTSALES > 10000
													AND PTSALES <= 20000
													THEN 'C: 10000 - 20000'
												WHEN PTSALES > 20000
													AND PTSALES <= 30000
													THEN 'D: 20000 - 30000'
												WHEN PTSALES > 30000
													AND PTSALES <= 40000
													THEN 'E: 30000 - 40000'
												WHEN PTSALES > 40000
													AND PTSALES <= 50000
													THEN 'F: 40000 - 50000'
												WHEN PTSALES > 50000
													AND PTSALES <= 75000
													THEN 'G: 50000 - 75000'
												WHEN PTSALES > 75000
													AND PTSALES <= 100000
													THEN 'H: 75000 - 100000'
												WHEN PTSALES > 100000
													THEN 'I: 100000 - ...'
												END
											) DEALER_SALES_RANGE
										,(
											SELECT SUM(DR_AMOUNT - CR_AMOUNT) CLOSING_BALANCE
											FROM MV$VIRTUAL_SUB_DEALER_LEDGER
											WHERE VOUCHER_DATE >= TO_DATE ('{0}','YYYY-MM-DD')
                                                AND VOUCHER_DATE <= TO_DATE ('{1}','YYYY-MM-DD')
												AND PARTY_TYPE_CODE = PT.PARTY_TYPE_CODE
											) OUTSTANDING_AMT
										,FN_AVG_PAYBACKDAYS('D', PT.PARTY_TYPE_CODE, TO_DATE ('{0}','YYYY-MM-DD')) AVG_DAYS
										,0 PAYMENT_ROTATION_DAY
									FROM (
										SELECT DISTINCT PARTY_TYPE_CODE
											,SUM(QUANTITY) OVER () GSALES
                                            ,SUM(QUANTITY) OVER( PARTITION BY PARTY_TYPE_CODE) PTSALES
											,SUM(GROSS_SALES) OVER (PARTITION BY PARTY_TYPE_CODE) ATSALES
										FROM V$SALES_INVOICE_REPORT3
										) PT
									) PTB
								) FRPT WHERE PARTY_TYPE_CODE IN ('{2}')
							GROUP BY DEALER_SALES_RANGE
								,PCNT
								,GSALES
								,TOTAL_OUTSTATINGS
								,DAVG_DAYS
								,PAYMENT_ROTATION_DAY
							ORDER BY DEALER_SALES_RANGE";
                Query = string.Format(Query, model.FromDate, model.ToDate, string.Join("','", model.PartyTypeFilter));
            }
            else
            {
                Query = @"SELECT DEALER_SALES_RANGE AS DealerSalesRange
								,COUNT(*) Count
								,(ROUND(COUNT(*) / PCNT, 2) * 100) DealerCountRatio
								,SUM(PTSALES) TotalSales
								,ROUND((SUM(PTSALES) / GSALES) * 100, 2) SalesRatio
								,SUM(OUTSTANDING_AMT) TotalOutStanding
								,ROUND((SUM(OUTSTANDING_AMT) / TOTAL_OUTSTATINGS) * 100, 2) OutStandingRatio
								,ROUND(DAVG_DAYS, 2) AverageAging
								,PAYMENT_ROTATION_DAY PaymentRotationDay
							FROM (
								SELECT PTB.*
									,SUM(OUTSTANDING_AMT) OVER () TOTAL_OUTSTATINGS
									,COUNT(PARTY_TYPE_CODE) OVER () PCNT
									,AVG(AVG_DAYS) OVER (PARTITION BY DEALER_SALES_RANGE) DAVG_DAYS
								FROM (
									SELECT DISTINCT PARTY_TYPE_CODE
										,GSALES
										,PTSALES
										,(
											CASE 
												WHEN PTSALES <= 5000
													THEN 'A: 0 - 5000'
												WHEN PTSALES > 5000
													AND PTSALES <= 10000
													THEN 'B: 5000 - 10000'
												WHEN PTSALES > 10000
													AND PTSALES <= 20000
													THEN 'C: 10000 - 20000'
												WHEN PTSALES > 20000
													AND PTSALES <= 30000
													THEN 'D: 20000 - 30000'
												WHEN PTSALES > 30000
													AND PTSALES <= 40000
													THEN 'E: 30000 - 40000'
												WHEN PTSALES > 40000
													AND PTSALES <= 50000
													THEN 'F: 40000 - 50000'
												WHEN PTSALES > 50000
													AND PTSALES <= 75000
													THEN 'G: 50000 - 75000'
												WHEN PTSALES > 75000
													AND PTSALES <= 100000
													THEN 'H: 75000 - 100000'
												WHEN PTSALES > 100000
													THEN 'I: 100000 - ...'
												END
											) DEALER_SALES_RANGE
										,(
											SELECT SUM(DR_AMOUNT - CR_AMOUNT) CLOSING_BALANCE
											FROM MV$VIRTUAL_SUB_DEALER_LEDGER
											WHERE VOUCHER_DATE >= TO_DATE ('{0}','YYYY-MM-DD')
                                                AND VOUCHER_DATE <= TO_DATE ('{1}','YYYY-MM-DD')
												AND PARTY_TYPE_CODE = PT.PARTY_TYPE_CODE
											) OUTSTANDING_AMT
										,FN_AVG_PAYBACKDAYS('D', PT.PARTY_TYPE_CODE, TO_DATE ('{0}','YYYY-MM-DD')) AVG_DAYS
										,0 PAYMENT_ROTATION_DAY
									FROM (
										SELECT DISTINCT PARTY_TYPE_CODE
											,SUM(QUANTITY) OVER () GSALES
                                            ,SUM(QUANTITY) OVER( PARTITION BY PARTY_TYPE_CODE) PTSALES
											,SUM(GROSS_SALES) OVER (PARTITION BY PARTY_TYPE_CODE) ATSALES
										FROM V$SALES_INVOICE_REPORT3
										) PT
									) PTB
								) FRPT
							GROUP BY DEALER_SALES_RANGE
								,PCNT
								,GSALES
								,TOTAL_OUTSTATINGS
								,DAVG_DAYS
								,PAYMENT_ROTATION_DAY
							ORDER BY DEALER_SALES_RANGE";
                Query = string.Format(Query, model.FromDate, model.ToDate);
            }


            dealerSalesRangeData = this._objectEntity.SqlQuery<DealerSalesRangeModel>(Query).ToList();

            return dealerSalesRangeData;
        }

        public IList<ProductRealizationAreaWiseModel> GetProductRealizationAreaWise(ReportFiltersModel model, User userInfo)
        {
            var productRealizeAreaData = new List<ProductRealizationAreaWiseModel>();


            var Query = @"SELECT AREA_EDESC AS AreaName
                                             ,TO_NUMBER(OPC) OPC
                                             ,TO_NUMBER(PPC) PPC
                                             ,TO_NUMBER(PSC) PSC
                                            FROM (
                                             SELECT FN_FETCH_GROUP_DESC(SA.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.MASTER_ITEM_CODE) GROUP_EDESC
                                              ,AST.AREA_EDESC
                                              ,SA.CALC_QUANTITY
                                             FROM SA_SALES_INVOICE SA
                                              ,IP_ITEM_MASTER_SETUP IMS
                                              ,AREA_SETUP AST
                                             WHERE SA.ITEM_CODE = IMS.ITEM_CODE
                                              AND SA.AREA_CODE = AST.AREA_CODE
                                              AND SA.COMPANY_CODE = IMS.COMPANY_CODE
                                              AND SA.COMPANY_CODE = AST.COMPANY_CODE
                                              AND IMS.CATEGORY_CODE = 'FG'
                                              AND SA.SALES_DATE <= TO_DATE ('{0}','YYYY-MM-DD')
                                             )
                                            PIVOT(COUNT(CALC_QUANTITY) FOR GROUP_EDESC IN (
                                               'OPC Cement' AS OPC
                                               ,'PPC Cement' AS PPC
                                               ,'PSC Brij' AS PSC
                                               ))
                                            ORDER BY AREA_EDESC";

            //var Query = @"SELECT AREA_EDESC AS AreaName
            //                                 ,TO_NUMBER(OPC) OPC
            //                                 ,TO_NUMBER(PPC) PPC
            //                                 --,TO_NUMBER(PSC) PSC
            //                                FROM (
            //                                 SELECT FN_FETCH_GROUP_DESC(SA.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.MASTER_ITEM_CODE) GROUP_EDESC
            //                                  ,AST.AREA_EDESC
            //                                  ,SA.CALC_QUANTITY
            //                                 FROM SA_SALES_INVOICE SA
            //                                  ,IP_ITEM_MASTER_SETUP IMS
            //                                  ,AREA_SETUP AST
            //                                 WHERE SA.ITEM_CODE = IMS.ITEM_CODE
            //                                  AND SA.AREA_CODE = AST.AREA_CODE
            //                                  AND SA.COMPANY_CODE = IMS.COMPANY_CODE
            //                                  AND SA.COMPANY_CODE = AST.COMPANY_CODE
            //                                  AND IMS.CATEGORY_CODE = 'FG'
            //                                  AND SA.SALES_DATE <= TO_DATE ('{0}','YYYY-MM-DD')
            //                                 )
            //                                PIVOT(COUNT(CALC_QUANTITY) FOR GROUP_EDESC IN (
            //                                   'T.M.T. Bar' AS OPC
            //                                   ,'Torkari' AS PPC
            //                                   --,'PSC Brij' AS PSC
            //                                   ))
            //                                ORDER BY AREA_EDESC";
            Query = string.Format(Query, model.FromDate);
            productRealizeAreaData = this._objectEntity.SqlQuery<ProductRealizationAreaWiseModel>(Query).ToList();

            return productRealizeAreaData;

        }

        public IList<ProductRealizationSOSMWiseModel> GetProductRealizationSalesManagerAndOfficerWise(ReportFiltersModel model, User userInfo)
        {
            var productRealizeSOSMData = new List<ProductRealizationSOSMWiseModel>();
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //FOR BRIJ CEMENT
            var Query = @"SELECT SO AS SalesOfficer
                             ,SM AS SalesManager
                             ,TO_NUMBER(OPC) AS OPC
                             ,TO_NUMBER(PPC) AS PPC
                             ,TO_NUMBER(PSC) AS PSC
                            FROM (
                             SELECT EGM.GROUP_EDESC SO
                              ,FN_FETCH_GROUP_DESC(SA.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.MASTER_ITEM_CODE) ITEM_GROUP
                              ,HE.EMPLOYEE_EDESC SM
                              ,SA.CALC_QUANTITY
                             FROM SA_SALES_INVOICE SA
                              ,IP_ITEM_MASTER_SETUP IMS
                              ,HR_EMPLOYEE_SETUP HE
                              ,HR_EMPLOYEE_GROUP_MAP EGM
                             WHERE SA.ITEM_CODE = IMS.ITEM_CODE
                              AND SA.EMPLOYEE_CODE = HE.EMPLOYEE_CODE
                              AND SA.COMPANY_CODE = IMS.COMPANY_CODE
                              AND SA.COMPANY_CODE = HE.COMPANY_CODE
                              AND IMS.CATEGORY_CODE = 'FG'
                              AND HE.EMPLOYEE_CODE = EGM.EMPLOYEE_CODE
                              AND SA.COMPANY_CODE = EGM.COMPANY_CODE
                              AND SA.SALES_DATE <= TO_DATE ('{0}','YYYY-MM-DD')
                             )
                            PIVOT(COUNT(CALC_QUANTITY) FOR ITEM_GROUP IN (
                               'OPC Cement' AS OPC
                               ,'PPC Cement' AS PPC
                               ,'PSC Brij' AS PSC
                               ))
                            ORDER BY SO";
            //FOR SRG
            //var Query = @"SELECT SO AS SalesOfficer
            //                 ,SM AS SalesManager
            //                 ,TO_NUMBER(OPC) AS OPC
            //                 ,TO_NUMBER(PPC) AS PPC
            //                 --,TO_NUMBER(PSC) AS PSC
            //                FROM (
            //                 SELECT EGM.GROUP_EDESC SO
            //                  ,FN_FETCH_GROUP_DESC(SA.COMPANY_CODE, 'IP_ITEM_MASTER_SETUP', IMS.MASTER_ITEM_CODE) ITEM_GROUP
            //                  ,HE.EMPLOYEE_EDESC SM
            //                  ,SA.CALC_QUANTITY
            //                 FROM SA_SALES_INVOICE SA
            //                  ,IP_ITEM_MASTER_SETUP IMS
            //                  ,HR_EMPLOYEE_SETUP HE
            //                  ,HR_EMPLOYEE_GROUP_MAP EGM
            //                 WHERE SA.ITEM_CODE = IMS.ITEM_CODE
            //                  AND SA.EMPLOYEE_CODE = HE.EMPLOYEE_CODE
            //                  AND SA.COMPANY_CODE = IMS.COMPANY_CODE
            //                  AND SA.COMPANY_CODE = HE.COMPANY_CODE
            //                  AND IMS.CATEGORY_CODE = 'FG'
            //                  AND HE.EMPLOYEE_CODE = EGM.EMPLOYEE_CODE
            //                  AND SA.COMPANY_CODE = EGM.COMPANY_CODE
            //                  AND SA.SALES_DATE <= TO_DATE ('{0}','YYYY-MM-DD')
            //                 )
            //                PIVOT(COUNT(CALC_QUANTITY) FOR ITEM_GROUP IN (
            //                   'T.M.T. Bar' AS OPC
            //                   ,'Torkari' AS PPC
            //                  -- ,'PSC Brij' AS PSC
            //                   ))
            //                ORDER BY SO";
            Query = string.Format(Query, model.FromDate);
            productRealizeSOSMData = this._objectEntity.SqlQuery<ProductRealizationSOSMWiseModel>(Query).ToList();

            return productRealizeSOSMData;
        }
        public IList<QuantityBalancePaymentRotationDayModel> GetQuantityBalanceAndPaymentRotationDay(ReportFiltersModel model, User userInfo)
        {
            var quantityBalancePaymentRotationDayData = new List<QuantityBalancePaymentRotationDayModel>();
            var Query = string.Empty;
            if (model.PartyTypeFilter.Count > 0)
            {
                Query = @"SELECT A.PARTY_TYPE_CODE AS PartyTypeCode
								,A.PRE_PARTY_CODE AS PrePartyCode
								,A.MASTER_PARTY_CODE AS MasterPartyCode
								,A.REMARKS AS Remarks
								,A.PARTY_TYPE_EDESC AS PartyTypeName
								,A.PARTY_NAME AS PartyName
								,TO_CHAR(A.CREDIT_LIMIT) AS CreditLimit
								,A.BALANCE AS Balance
								,A.QUANITTY AS Quantity
								,A.PAY_ROTATION_DAYS AS PayRotationDays
								,A.IS_CHILD AS IsChild
							FROM (
								SELECT PARTY_TYPE_CODE
									,PRE_PARTY_CODE
									,MASTER_PARTY_CODE
									,REMARKS
									,PARTY_TYPE_EDESC
									,RPAD('*', 2 * LEVEL, '*') || PARTY_TYPE_EDESC PARTY_NAME
									,(
										SELECT SUM(P2.CREDIT_LIMIT)
										FROM IP_PARTY_TYPE_CODE P2 START WITH P2.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE CONNECT BY PRIOR P2.MASTER_PARTY_CODE = P2.PRE_PARTY_CODE
										) CREDIT_LIMIT
									,(
										SELECT SUM(DR_AMOUNT - CR_AMOUNT)
										FROM MV$VIRTUAL_SUB_DEALER_LEDGER
										WHERE VOUCHER_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
											AND PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
										) BALANCE
									,(
										SELECT SUM(QUANTITY)
										FROM V$SALES_INVOICE_REPORT
										WHERE PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
											AND SALES_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
										) QUANITTY
									,FN_AVG_PAYBACKDAYS('D', P1.PARTY_TYPE_CODE, TO_DATE('{0}', 'YYYY-MM-DD')) PAY_ROTATION_DAYS
									,CONNECT_BY_ISLEAF IS_CHILD
								FROM IP_PARTY_TYPE_CODE P1
								WHERE P1.PARTY_TYPE_CODE IN ('{2}') START
								WITH P1.PRE_PARTY_CODE LIKE '00'
									AND P1.PARTY_TYPE_FLAG = 'D'
									AND P1.GROUP_SKU_FLAG = 'G'
									AND P1.PRE_PARTY_CODE = '00' CONNECT BY PRIOR P1.MASTER_PARTY_CODE = P1.PRE_PARTY_CODE
								) A";
                Query = string.Format(Query, model.FromDate, model.ToDate, string.Join("','", model.PartyTypeFilter));
            }
            else
            {
                Query = @"SELECT A.PARTY_TYPE_CODE AS PartyTypeCode
								,A.PRE_PARTY_CODE AS PrePartyCode
								,A.MASTER_PARTY_CODE AS MasterPartyCode
								,A.REMARKS AS Remarks
								,A.PARTY_TYPE_EDESC AS PartyTypeName
								,A.PARTY_NAME AS PartyName
								,TO_CHAR(A.CREDIT_LIMIT) AS CreditLimit
								,A.BALANCE AS Balance
								,A.QUANITTY AS Quantity
								,A.PAY_ROTATION_DAYS AS PayRotationDays
								,A.IS_CHILD AS IsChild
							FROM (
								SELECT PARTY_TYPE_CODE
									,PRE_PARTY_CODE
									,MASTER_PARTY_CODE
									,REMARKS
									,PARTY_TYPE_EDESC
									,RPAD('*', 2 * LEVEL, '*') || PARTY_TYPE_EDESC PARTY_NAME
									,(
										SELECT SUM(P2.CREDIT_LIMIT)
										FROM IP_PARTY_TYPE_CODE P2 START WITH P2.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE CONNECT BY PRIOR P2.MASTER_PARTY_CODE = P2.PRE_PARTY_CODE
										) CREDIT_LIMIT
									,(
										SELECT SUM(DR_AMOUNT - CR_AMOUNT)
										FROM MV$VIRTUAL_SUB_DEALER_LEDGER
										WHERE VOUCHER_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
											AND PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
										) BALANCE
									,(
										SELECT SUM(QUANTITY)
										FROM V$SALES_INVOICE_REPORT
										WHERE PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
											AND SALES_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
										) QUANITTY
									,FN_AVG_PAYBACKDAYS('D', P1.PARTY_TYPE_CODE, TO_DATE('{0}', 'YYYY-MM-DD')) PAY_ROTATION_DAYS
									,CONNECT_BY_ISLEAF IS_CHILD
								FROM IP_PARTY_TYPE_CODE P1
								WHERE P1.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE START
								WITH P1.PRE_PARTY_CODE LIKE '00'
									AND P1.PARTY_TYPE_FLAG = 'D'
									AND P1.GROUP_SKU_FLAG = 'G'
									AND P1.PRE_PARTY_CODE = '00' CONNECT BY PRIOR P1.MASTER_PARTY_CODE = P1.PRE_PARTY_CODE
								) A where ROWNUM<10";
                Query = string.Format(Query, model.FromDate, model.ToDate);
            }

            quantityBalancePaymentRotationDayData = this._objectEntity.SqlQuery<QuantityBalancePaymentRotationDayModel>(Query).ToList();

            return quantityBalancePaymentRotationDayData;

        }
        public List<QuantityBalancePaymentRotationDayModel> GetQuantityBalanceAndPaymentRotationDayT(ReportFiltersModel model, User userInfo)
        {
            var quantityBalancePaymentRotationDayData = new List<QuantityBalancePaymentRotationDayModel>();
            var Query = string.Empty;
            if (model.PartyTypeFilter.Count > 0)
            {
                Query = @"SELECT A.PARTY_TYPE_CODE AS PartyTypeCode
			,A.PRE_PARTY_CODE AS PrePartyCode
            ,A.PARTY_TYPE_CODE AS parentId
            ,TO_NUMBER(REPLACE(A.PRE_PARTY_CODE, '.', '')) AS ParentIdInt
            ,TO_NUMBER(REPLACE(A.MASTER_PARTY_CODE, '.', '')) AS Id
			,A.MASTER_PARTY_CODE AS MasterPartyCode
			,A.REMARKS AS Remarks
			,A.PARTY_TYPE_EDESC AS account_head
			,A.PARTY_NAME AS PartyName
			,A.CREDIT_LIMIT AS CreditLimit
			,TO_NUMBER(A.BALANCE) AS Balance
			,A.QUANITTY AS Quantity
			,A.PAY_ROTATION_DAYS AS PayRotationDays
			,A.IS_CHILD AS IsChild
            ,A.TREE_LEVEL AS TreeLevel
		FROM (
			SELECT PARTY_TYPE_CODE
				,PRE_PARTY_CODE
				,MASTER_PARTY_CODE
				,REMARKS
				,PARTY_TYPE_EDESC
				,RPAD('*', 2 * LEVEL, '*') || PARTY_TYPE_EDESC PARTY_NAME
                ,LEVEL TREE_LEVEL 
				,(
					SELECT SUM(P2.CREDIT_LIMIT)
					FROM IP_PARTY_TYPE_CODE P2 START WITH P2.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE CONNECT BY PRIOR P2.MASTER_PARTY_CODE = P2.PRE_PARTY_CODE
					) CREDIT_LIMIT
				,(
					SELECT SUM(DR_AMOUNT - CR_AMOUNT)
					FROM MV$VIRTUAL_SUB_DEALER_LEDGER
					WHERE VOUCHER_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
						AND PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
					) BALANCE
				,(
					SELECT SUM(QUANTITY)
					FROM V$SALES_INVOICE_REPORT
					WHERE PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
						AND SALES_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
					) QUANITTY
				,FN_AVG_PAYBACKDAYS('D', P1.PARTY_TYPE_CODE, TO_DATE('{0}', 'YYYY-MM-DD')) PAY_ROTATION_DAYS
				,CONNECT_BY_ISLEAF IS_CHILD
			FROM IP_PARTY_TYPE_CODE P1
			WHERE P1.PARTY_TYPE_CODE IN ('{2}') START
			WITH P1.PRE_PARTY_CODE LIKE '00'
				AND P1.PARTY_TYPE_FLAG = 'D'
				AND P1.GROUP_SKU_FLAG = 'G'
				AND P1.PRE_PARTY_CODE = '00' CONNECT BY PRIOR P1.MASTER_PARTY_CODE = P1.PRE_PARTY_CODE
			) A";
                Query = string.Format(Query, model.FromDate, model.ToDate, string.Join("','", model.PartyTypeFilter));
            }
            else
            {
                Query = @"SELECT A.PARTY_TYPE_CODE AS PartyTypeCode
            			,A.PRE_PARTY_CODE AS PrePartyCode
            			,A.MASTER_PARTY_CODE AS MasterPartyCode
                        ,A.PARTY_TYPE_CODE AS parentId
            	        ,TO_NUMBER(REPLACE(A.PRE_PARTY_CODE, '.', '')) AS ParentIdInt
            	        ,TO_NUMBER(REPLACE(A.MASTER_PARTY_CODE, '.', '')) AS Id
            			,A.REMARKS AS Remarks
            			,A.PARTY_TYPE_EDESC AS account_head
            			,A.PARTY_NAME AS PartyName
            			,A.CREDIT_LIMIT AS CreditLimit
            			,TO_NUMBER(NVL(A.BALANCE,0)) AS Balance
            			,TO_NUMBER(NVL(A.QUANITTY,0)) AS Quantity
            			,TO_NUMBER(NVL(A.PAY_ROTATION_DAYS,0)) AS PayRotationDays
            			,A.IS_CHILD AS IsChild
                        ,A.TREE_LEVEL AS TreeLevel
            		FROM (
            			SELECT PARTY_TYPE_CODE
            				,PRE_PARTY_CODE
            				,MASTER_PARTY_CODE
            				,REMARKS
            				,PARTY_TYPE_EDESC
            				,RPAD('*', 2 * LEVEL, '*') || PARTY_TYPE_EDESC PARTY_NAME
                            ,LEVEL TREE_LEVEL 
            				,(
            					SELECT SUM(P2.CREDIT_LIMIT)
            					FROM IP_PARTY_TYPE_CODE P2 START WITH P2.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE CONNECT BY PRIOR P2.MASTER_PARTY_CODE = P2.PRE_PARTY_CODE
            					) CREDIT_LIMIT
            				,(
            					SELECT SUM(DR_AMOUNT - CR_AMOUNT)
            					FROM MV$VIRTUAL_SUB_DEALER_LEDGER
            					WHERE VOUCHER_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
            						AND PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
            					) BALANCE
            				,(
            					SELECT SUM(QUANTITY)
            					FROM V$SALES_INVOICE_REPORT
            					WHERE PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE
            						AND SALES_DATE <= TO_DATE('{0}', 'YYYY-MM-DD')
            					) QUANITTY
            				,FN_AVG_PAYBACKDAYS('D', P1.PARTY_TYPE_CODE, TO_DATE('{0}', 'YYYY-MM-DD')) PAY_ROTATION_DAYS
            				,CONNECT_BY_ISLEAF IS_CHILD
            			FROM IP_PARTY_TYPE_CODE P1
            			WHERE P1.PARTY_TYPE_CODE = P1.PARTY_TYPE_CODE START
            			WITH 
            --TO_NUMBER(REPLACE(P1.PRE_PARTY_CODE, '.', '')) = '0'
                        P1.PRE_PARTY_CODE LIKE '00'
            				AND P1.PARTY_TYPE_FLAG = 'D'
            				AND P1.GROUP_SKU_FLAG = 'G'
            				AND P1.PRE_PARTY_CODE = '00' CONNECT BY PRIOR TRIM(P1.MASTER_PARTY_CODE) = TRIM(P1.PRE_PARTY_CODE)
            --PRIOR P1.MASTER_PARTY_CODE = P1.PRE_PARTY_CODE
            			) A";
                Query = string.Format(Query, model.FromDate, model.ToDate);
            }

            //                     {
            //                         Query = @"SELECT LPAD(' ', 3 * LEVEL - 3) || ES.EMPLOYEE_EDESC account_head
            //            		,ES.EMPLOYEE_CODE PartyTypeCode
            //            		,ES.MASTER_EMPLOYEE_CODE MasterPartyCode
            //            		,ES.EMPLOYEE_CODE parentId
            //            		,TO_NUMBER(REPLACE(ES.PRE_EMPLOYEE_CODE, '.', '')) AS ParentIdInt
            //                    ,TO_NUMBER(REPLACE(CASE WHEN ES.MASTER_EMPLOYEE_CODE='01.00' THEN '104' ELSE ES.MASTER_EMPLOYEE_CODE END, '.', '')) AS Id
            //            		,NVL(SPD.PER_DAY_AMOUNT, 0) Balance
            //            --		,DECODE(SUBSTR(BS_DATE(PLAN_DATE), 0, 7), '0000-00', NULL, SUBSTR(BS_DATE(PLAN_DATE), 0, 7)) BS_MONTH
            //            		,LEVEL TreeLevel
            //            	FROM HR_EMPLOYEE_TREE ET
            //            		,HR_EMPLOYEE_SETUP ES
            //            		,PL_SALES_PLAN_DTL SPD
            //            	WHERE ES.EMPLOYEE_CODE = ET.EMPLOYEE_CODE
            //            		AND ES.COMPANY_CODE = ET.COMPANY_CODE
            //            		AND ES.EMPLOYEE_CODE = SPD.EMPLOYEE_CODE(+)
            //AND TO_NUMBER(REPLACE(ES.PRE_EMPLOYEE_CODE, '.', ''))<>1
            //            		AND ES.COMPANY_CODE = SPD.COMPANY_CODE(+) CONNECT BY PRIOR ES.MASTER_EMPLOYEE_CODE = ES.PRE_EMPLOYEE_CODE START
            //            	WITH PARENT_EMPLOYEE_CODE IS  NULL";
            //                         Query = string.Format(Query, model.FromDate, model.ToDate);
            //                     }



            quantityBalancePaymentRotationDayData = this._objectEntity.SqlQuery<QuantityBalancePaymentRotationDayModel>(Query).ToList();
            var balanceTotal = quantityBalancePaymentRotationDayData.Select(x => x.Balance).Sum();
            quantityBalancePaymentRotationDayData.Select(x => { x.BalanceTotal = balanceTotal; return x; }).ToList();
            var quantityTotal = quantityBalancePaymentRotationDayData.Select(x => x.Quantity).Sum();
            quantityBalancePaymentRotationDayData.Select(x => { x.QuantityTotal = quantityTotal; return x; }).ToList();
            var level1Balance = quantityBalancePaymentRotationDayData.Where(x => x.TreeLevel == 2 && x.PrePartyCode == "12").Sum(x => x.Balance);
            quantityBalancePaymentRotationDayData.Where(x => x.PartyTypeCode == "134").Select(c => { c.Balance = level1Balance; return c; }).ToList();
            //var firstBalance = 0.00; 
            decimal? firstBalance = 0m;
            decimal? firstQuantity = 0m;
            decimal? firstRotation = 0m;
            decimal? secondBalance = 0m;
            decimal? secondQuantity = 0m;
            decimal? secondRotation = 0m;
            decimal? thirdBalance = 0m;
            decimal? thirdQuantity = 0m;
            decimal? thirdRotation = 0m;
            decimal? forthBalance = 0m;
            decimal? forthQuantity = 0m;
            decimal? forthRotation = 0m;
            var partyTypeCode = quantityBalancePaymentRotationDayData.Where(x => x.TreeLevel == 1).Select(x => x.MasterPartyCode);
            var subGParty = "";

            //foreach(var secPartyTypeCode in )
            //quantityBalancePaymentRotationDayData.ba= quantityBalancePaymentRotationDayData.Where(x=>x.TreeLevel==2||x.TreeLevel==3).Sum(x=>x.Balance)
            IEnumerable<QuantityBalancePaymentRotationDayModel> newGroupWiseList = null;
            foreach (var subParty in partyTypeCode)
            {
                subGParty = subParty;
                newGroupWiseList = quantityBalancePaymentRotationDayData.Where(x => x.MasterPartyCode.StartsWith(subParty + ".") || x.MasterPartyCode == subParty);
                firstBalance = newGroupWiseList.Where(x => x.TreeLevel == 2 || x.TreeLevel == 3).Sum(x => x.Balance);
                quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 1 && c.IsChild == 0 && c.MasterPartyCode == subGParty && c.PrePartyCode == "00").Select(c => { c.Balance = firstBalance; return c; }).ToList();
                firstQuantity = newGroupWiseList.Where(x => x.TreeLevel == 2 || x.TreeLevel == 3).Sum(x => x.Quantity);
                quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 1 && c.IsChild == 0 && c.MasterPartyCode == subGParty && c.PrePartyCode == "00").Select(c => { c.Quantity = firstQuantity; return c; }).ToList();
                firstRotation = newGroupWiseList.Where(x => x.TreeLevel == 2 || x.TreeLevel == 3).Sum(x => x.PayRotationDays);
                quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 1 && c.IsChild == 0 && c.MasterPartyCode == subGParty && c.PrePartyCode == "00").Select(c => { c.PayRotationDays = firstRotation; return c; }).ToList();
                var partyTypeCodeSub = newGroupWiseList.Where(x => x.TreeLevel == 2 && x.IsChild == 0).Select(x => x.MasterPartyCode);
                if (partyTypeCodeSub.Any())
                {
                    foreach (var subPartyGroup in partyTypeCodeSub)
                    {
                        var thirdLevelpartyTypeCodeSub = newGroupWiseList.Where(x => x.TreeLevel == 3 && x.IsChild == 0).Select(x => x.MasterPartyCode);
                        var sgroup = subPartyGroup + ".00";
                        secondBalance = newGroupWiseList.Where(y => y.TreeLevel == 3 && y.MasterPartyCode.Contains(sgroup)).Sum(y => y.Balance);
                        quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 2 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.Balance = secondBalance; return c; }).ToList();
                        secondQuantity = newGroupWiseList.Where(y => y.TreeLevel == 3 && y.MasterPartyCode.Contains(sgroup)).Sum(y => y.Quantity);
                        quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 2 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.Quantity = secondQuantity; return c; }).ToList();
                        secondRotation = newGroupWiseList.Where(y => y.TreeLevel == 3 && y.MasterPartyCode.Contains(sgroup)).Sum(y => y.PayRotationDays);
                        quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 2 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.PayRotationDays = secondRotation; return c; }).ToList();
                        if (thirdLevelpartyTypeCodeSub.Any())
                        {
                            foreach (var thirdLevelSubPartyGroup in thirdLevelpartyTypeCodeSub)
                            {
                                var fourthLevelpartyTypeCodeSub = newGroupWiseList.Where(x => x.TreeLevel == 4 && x.IsChild == 0).Select(x => x.MasterPartyCode);
                                var thirdsgroup = subPartyGroup + ".00";
                                thirdBalance = newGroupWiseList.Where(y => y.TreeLevel == 4 && y.MasterPartyCode.Contains(thirdsgroup)).Sum(y => y.Balance);
                                quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 3 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.Balance = thirdBalance; return c; }).ToList();
                                thirdQuantity = newGroupWiseList.Where(y => y.TreeLevel == 4 && y.MasterPartyCode.Contains(thirdsgroup)).Sum(y => y.Quantity);
                                quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 3 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.Quantity = thirdQuantity; return c; }).ToList();
                                thirdRotation = newGroupWiseList.Where(y => y.TreeLevel == 4 && y.MasterPartyCode.Contains(thirdsgroup)).Sum(y => y.PayRotationDays);
                                quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 3 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.PayRotationDays = thirdRotation; return c; }).ToList();
                                if (fourthLevelpartyTypeCodeSub.Any())
                                {
                                    foreach (var fourthLevelSubPartyGroup in fourthLevelpartyTypeCodeSub)
                                    {
                                        var fourthsgroup = subPartyGroup + ".00";
                                        forthBalance = newGroupWiseList.Where(y => y.TreeLevel == 5 && y.MasterPartyCode.Contains(fourthsgroup)).Sum(y => y.Balance);
                                        quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 4 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.Balance = forthBalance; return c; }).ToList();
                                        forthQuantity = newGroupWiseList.Where(y => y.TreeLevel == 5 && y.MasterPartyCode.Contains(fourthsgroup)).Sum(y => y.Quantity);
                                        quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 4 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.Quantity = forthQuantity; return c; }).ToList();
                                        forthRotation = newGroupWiseList.Where(y => y.TreeLevel == 5 && y.MasterPartyCode.Contains(fourthsgroup)).Sum(y => y.PayRotationDays);
                                        quantityBalancePaymentRotationDayData.Where(c => c.TreeLevel == 4 && c.IsChild == 0 && c.MasterPartyCode == subPartyGroup).Select(c => { c.PayRotationDays = forthRotation; return c; }).ToList();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return quantityBalancePaymentRotationDayData;

        }
        public IList<AreaWiseSalesAndCollectionModel> GetAreaWiseSalesAndCollection(ReportFiltersModel model, User userInfo)
        {
            var areaWiseSalesAndCollectionModel = new List<AreaWiseSalesAndCollectionModel>();
            var Query = string.Empty;
            if (model.AreaTypeFilter.Count > 0)
            {
                Query = @"SELECT TO_CHAR(AREA_CODE) AreaCode
	                            ,AREA_EDESC AreaName
	                            ,COMPANY_CODE CompanyCode
	                            ,TODAY_SALES DailySales
	                            ,TODAY_COLLECTION DailyCollection
	                            ,MTD_SALES MonthlySales
	                            ,MTD_COLLECTION MonthlyCollection
	                            ,YTD_SALES YearlySales
	                            ,YTD_COLLECTION YearlyCollection
                            FROM (
	                            SELECT ASE.*
		                            ,NVL(TODAYSALES.SALES_AMT, 0) TODAY_SALES
		                            ,NVL(TODAY_COLLECTION.COLLECTION_AMT, 0) TODAY_COLLECTION
		                            ,NVL(MTDSALES.SALES_AMT, 0) MTD_SALES
		                            ,NVL(MTDCOL.COLLECTION_AMT, 0) MTD_COLLECTION
		                            ,NVL(YTDSALES.SALES_AMT, 0) YTD_SALES
		                            ,NVL(YTDCOL.COLLECTION_AMT, 0) YTD_COLLECTION
	                            FROM AREA_SETUP ASE
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(CR_AMOUNT) COLLECTION_AMT
			                            FROM V$VIRTUAL_SUB_LEDGER VSL
				                            ,SA_CUSTOMER_SETUP CS
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
				                            AND VSL.COMPANY_CODE = CS.COMPANY_CODE
				                            AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND VSL.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND VSL.COMPANY_CODE = AST.COMPANY_CODE
				                            AND SUBSTR(TO_BS(VSL.VOUCHER_DATE), 4, 2) = SUBSTR(TO_BS(TO_DATE ('{0}','YYYY-MM-DD')), 4, 2)
				                            AND VSL.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ) MTDCOL
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(CR_AMOUNT) COLLECTION_AMT
			                            FROM V$VIRTUAL_SUB_LEDGER VSL
				                            ,SA_CUSTOMER_SETUP CS
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
				                            AND VSL.COMPANY_CODE = CS.COMPANY_CODE
				                            AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND VSL.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND VSL.COMPANY_CODE = AST.COMPANY_CODE
				                            AND VSL.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ) YTDCOL
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(CR_AMOUNT) COLLECTION_AMT
			                            FROM V$VIRTUAL_SUB_LEDGER VSL
				                            ,SA_CUSTOMER_SETUP CS
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
				                            AND TRUNC(VSL.VOUCHER_DATE) = TRUNC(TO_DATE ('{0}','YYYY-MM-DD'))
				                            AND VSL.COMPANY_CODE = CS.COMPANY_CODE
				                            AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND VSL.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND VSL.COMPANY_CODE = AST.COMPANY_CODE
				                            AND VSL.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ) TODAY_COLLECTION
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(NET_SALES) SALES_AMT
			                            FROM V$SALES_INVOICE_REPORT3 SI
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE SI.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND SI.COMPANY_CODE = AST.COMPANY_CODE
				                            AND SI.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ORDER BY AST.AREA_EDESC
			                            ) YTDSALES
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(NET_SALES) SALES_AMT
			                            FROM V$SALES_INVOICE_REPORT3 SI
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE SI.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND SI.COMPANY_CODE = AST.COMPANY_CODE
				                            AND SUBSTR(TO_BS(SI.SALES_DATE), 4, 2) = SUBSTR(TO_BS(TO_DATE ('{0}','YYYY-MM-DD')), 4, 2)
				                            AND SI.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ORDER BY AST.AREA_EDESC
			                            ) MTDSALES
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(NET_SALES) SALES_AMT
			                            FROM V$SALES_INVOICE_REPORT3 SI
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE TRUNC(SI.SALES_DATE) = TRUNC(TO_DATE ('{0}','YYYY-MM-DD'))
				                            AND SI.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND SI.COMPANY_CODE = AST.COMPANY_CODE
				                            AND SI.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ) TODAYSALES
	                            WHERE ASE.AREA_CODE = MTDCOL.AREA_CODE(+)
		                            AND ASE.AREA_CODE = TODAY_COLLECTION.AREA_CODE(+)
		                            AND ASE.AREA_CODE = YTDCOL.AREA_CODE(+)
		                            AND ASE.AREA_CODE = YTDSALES.AREA_CODE(+)
		                            AND ASE.AREA_CODE = MTDSALES.AREA_CODE(+)
		                            AND ASE.AREA_CODE = TODAYSALES.AREA_CODE(+)
	                            )WHERE AREA_CODE IN ('{1}')
                            ORDER BY AREA_EDESC ASC";
                Query = string.Format(Query, model.FromDate, string.Join("','", model.AreaTypeFilter));
            }
            else
            {
                Query = @"SELECT TO_CHAR(AREA_CODE) AreaCode
	                            ,AREA_EDESC AreaName
	                            ,COMPANY_CODE CompanyCode
	                            ,TODAY_SALES DailySales
	                            ,TODAY_COLLECTION DailyCollection
	                            ,MTD_SALES MonthlySales
	                            ,MTD_COLLECTION MonthlyCollection
	                            ,YTD_SALES YearlySales
	                            ,YTD_COLLECTION YearlyCollection
                            FROM (
	                            SELECT ASE.*
		                            ,NVL(TODAYSALES.SALES_AMT, 0) TODAY_SALES
		                            ,NVL(TODAY_COLLECTION.COLLECTION_AMT, 0) TODAY_COLLECTION
		                            ,NVL(MTDSALES.SALES_AMT, 0) MTD_SALES
		                            ,NVL(MTDCOL.COLLECTION_AMT, 0) MTD_COLLECTION
		                            ,NVL(YTDSALES.SALES_AMT, 0) YTD_SALES
		                            ,NVL(YTDCOL.COLLECTION_AMT, 0) YTD_COLLECTION
	                            FROM AREA_SETUP ASE
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(CR_AMOUNT) COLLECTION_AMT
			                            FROM V$VIRTUAL_SUB_LEDGER VSL
				                            ,SA_CUSTOMER_SETUP CS
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
				                            AND VSL.COMPANY_CODE = CS.COMPANY_CODE
				                            AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND VSL.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND VSL.COMPANY_CODE = AST.COMPANY_CODE
				                            AND SUBSTR(TO_BS(VSL.VOUCHER_DATE), 4, 2) = SUBSTR(TO_BS(TO_DATE ('{0}','YYYY-MM-DD')), 4, 2)
				                            AND VSL.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ) MTDCOL
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(CR_AMOUNT) COLLECTION_AMT
			                            FROM V$VIRTUAL_SUB_LEDGER VSL
				                            ,SA_CUSTOMER_SETUP CS
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
				                            AND VSL.COMPANY_CODE = CS.COMPANY_CODE
				                            AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND VSL.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND VSL.COMPANY_CODE = AST.COMPANY_CODE
				                            AND VSL.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ) YTDCOL
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(CR_AMOUNT) COLLECTION_AMT
			                            FROM V$VIRTUAL_SUB_LEDGER VSL
				                            ,SA_CUSTOMER_SETUP CS
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
				                            AND TRUNC(VSL.VOUCHER_DATE) = TRUNC(TO_DATE ('{0}','YYYY-MM-DD'))
				                            AND VSL.COMPANY_CODE = CS.COMPANY_CODE
				                            AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND VSL.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND VSL.COMPANY_CODE = AST.COMPANY_CODE
				                            AND VSL.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ) TODAY_COLLECTION
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(NET_SALES) SALES_AMT
			                            FROM V$SALES_INVOICE_REPORT3 SI
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE SI.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND SI.COMPANY_CODE = AST.COMPANY_CODE
				                            AND SI.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ORDER BY AST.AREA_EDESC
			                            ) YTDSALES
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(NET_SALES) SALES_AMT
			                            FROM V$SALES_INVOICE_REPORT3 SI
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE SI.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND SI.COMPANY_CODE = AST.COMPANY_CODE
				                            AND SUBSTR(TO_BS(SI.SALES_DATE), 4, 2) = SUBSTR(TO_BS(TO_DATE ('{0}','YYYY-MM-DD')), 4, 2)
				                            AND SI.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ORDER BY AST.AREA_EDESC
			                            ) MTDSALES
		                            ,(
			                            SELECT PTC.AREA_CODE AREA_CODE
				                            ,AST.AREA_EDESC
				                            ,SUM(NET_SALES) SALES_AMT
			                            FROM V$SALES_INVOICE_REPORT3 SI
				                            ,IP_PARTY_TYPE_CODE PTC
				                            ,AREA_SETUP AST
			                            WHERE TRUNC(SI.SALES_DATE) = TRUNC(TO_DATE ('{0}','YYYY-MM-DD'))
				                            AND SI.COMPANY_CODE = PTC.COMPANY_CODE
				                            AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
				                            AND PTC.AREA_CODE = AST.AREA_CODE
				                            AND SI.COMPANY_CODE = AST.COMPANY_CODE
				                            AND SI.DELETED_FLAG = 'N'
			                            GROUP BY PTC.AREA_CODE
				                            ,AST.AREA_EDESC
			                            ) TODAYSALES
	                            WHERE ASE.AREA_CODE = MTDCOL.AREA_CODE(+)
		                            AND ASE.AREA_CODE = TODAY_COLLECTION.AREA_CODE(+)
		                            AND ASE.AREA_CODE = YTDCOL.AREA_CODE(+)
		                            AND ASE.AREA_CODE = YTDSALES.AREA_CODE(+)
		                            AND ASE.AREA_CODE = MTDSALES.AREA_CODE(+)
		                            AND ASE.AREA_CODE = TODAYSALES.AREA_CODE(+)
	                            )
                            ORDER BY AREA_EDESC ASC";
                //Query = string.Format(Query, model.FromDate, model.ToDate);
                Query = string.Format(Query, model.FromDate);
            }

            //if (model.AreaTypeFilter.Count > 0)
            //{
            //    Query = @"SELECT to_char(AREA_CODE) AreaCode
            //                 ,AREA_EDESC AreaName
            //                 ,COMPANY_CODE CompanyCode
            //                 ,TO_NUMBER(TODAY_SALES) DailySales
            //                 ,TO_NUMBER(TODAY_COLLECTION) DailyCollection
            //                 ,TO_NUMBER(MTD_SALES) MonthlySales
            //                 ,TO_NUMBER(MTD_COLLECTION) MonthlyCollection
            //                 ,TO_NUMBER(YTD_SALES) YearlySales
            //                 ,TO_NUMBER(YTD_COLLECTION) YearlyCollection
            //                FROM (
            //                 SELECT ASE.*
            //                  ,NVL(TODAYSALES.SALES_AMT, 0) TODAY_SALES
            //                  ,NVL(TODAY_COLLECTION.COLLECTION_AMT, 0) TODAY_COLLECTION
            //                  ,NVL(MTDSALES.SALES_AMT, 0) MTD_SALES
            //                  ,NVL(MTDCOL.COLLECTION_AMT, 0) MTD_COLLECTION
            //                  ,NVL(YTDSALES.SALES_AMT, 0) YTD_SALES
            //                  ,NVL(YTDCOL.COLLECTION_AMT, 0) YTD_COLLECTION
            //                 FROM AREA_SETUP ASE
            //                  ,(
            //                   SELECT PTC.AREA_CODE AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(CR_AMOUNT) COLLECTION_AMT
            //                   FROM V$VIRTUAL_SUB_LEDGER VSL
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                    ,CALENDAR_SETUP CAS
            //                   WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
            //                    AND VSL.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
            //                    AND PTC.AREA_CODE = AST.AREA_CODE
            //                    AND VSL.COMPANY_CODE = AST.COMPANY_CODE
            //                    --AND TRUNC(VSL.VOUCHER_DATE) = TO_DATE ('{0}','YYYY-MM-DD')
            //                    AND TO_DATE ('{0}','YYYY-MM-DD') BETWEEN CAS.AD_DATE
            //                     AND CAS.AD_DATE + CAS.DAYS_NO
            //                    AND SUBSTR(TO_BS(VSL.VOUCHER_DATE), 4, 2) = SUBSTR(CAS.BS_MONTH, 6, 2)
            //                   GROUP BY PTC.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                   ) MTDCOL
            //                  ,(
            //                   SELECT PTC.AREA_CODE AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(CR_AMOUNT) COLLECTION_AMT
            //                   FROM V$VIRTUAL_SUB_LEDGER VSL
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                   WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
            //                    AND VSL.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
            //                    AND PTC.AREA_CODE = AST.AREA_CODE
            //                    AND VSL.COMPANY_CODE = AST.COMPANY_CODE
            //                   GROUP BY PTC.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                   ) YTDCOL
            //                  ,(
            //                   SELECT PTC.AREA_CODE AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(CR_AMOUNT) COLLECTION_AMT
            //                   FROM V$VIRTUAL_SUB_LEDGER VSL
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                   WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
            //                    AND VSL.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
            //                    AND PTC.AREA_CODE = AST.AREA_CODE
            //                    AND VSL.COMPANY_CODE = AST.COMPANY_CODE
            //                    AND VSL.VOUCHER_DATE = TO_DATE ('{0}','YYYY-MM-DD')
            //                   GROUP BY PTC.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                   ) TODAY_COLLECTION
            //                  ,(
            //                   SELECT AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(NVL(SI.NET_SALES, 0)) SALES_AMT
            //                   FROM V$SALES_INVOICE_REPORT3 SI
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                    ,SA_CUSTOMER_SETUP CS
            //                   WHERE TRIM(SI.CUSTOMER_CODE) = TRIM(CS.CUSTOMER_CODE)
            //                    AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
            //                    AND AST.AREA_CODE = PTC.AREA_CODE
            //                    AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
            //                    AND PTC.COMPANY_CODE = AST.COMPANY_CODE
            //                    AND SI.COMPANY_CODE = CS.COMPANY_CODE
            //                   GROUP BY AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    --ORDER BY AST.AREA_EDESC
            //                   ) YTDSALES
            //                  ,(
            //                   SELECT AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(NVL(SI.NET_SALES, 0)) SALES_AMT
            //                   FROM V$SALES_INVOICE_REPORT3 SI
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,CALENDAR_SETUP CAS
            //                   WHERE TRIM(SI.CUSTOMER_CODE) = TRIM(CS.CUSTOMER_CODE)
            //                    AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
            //                    AND AST.AREA_CODE = PTC.AREA_CODE
            //                    AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
            //                    AND PTC.COMPANY_CODE = AST.COMPANY_CODE
            //                    AND SI.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND TO_DATE ('{0}','YYYY-MM-DD') BETWEEN CAS.AD_DATE
            //                     AND CAS.AD_DATE + CAS.DAYS_NO
            //                    AND SUBSTR(TO_BS(SI.SALES_DATE), 4, 2) = SUBSTR(CAS.BS_MONTH, 6, 2)
            //                   GROUP BY AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    --	ORDER BY AST.AREA_EDESC
            //                   ) MTDSALES
            //                  ,(
            //                   SELECT AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(NVL(SI.NET_SALES, 0)) SALES_AMT
            //                   FROM V$SALES_INVOICE_REPORT3 SI
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,CALENDAR_SETUP CAS
            //                   WHERE TRIM(SI.CUSTOMER_CODE) = TRIM(CS.CUSTOMER_CODE)
            //                    AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
            //                    AND AST.AREA_CODE = PTC.AREA_CODE
            //                    AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
            //                    AND PTC.COMPANY_CODE = AST.COMPANY_CODE
            //                    AND SI.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND TRUNC(SI.SALES_DATE) = TO_DATE((TO_DATE ('{0}','YYYY-MM-DD')), 'DD-MM-YYYY')
            //                   GROUP BY AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    --ORDER BY AST.AREA_EDESC
            //                   ) TODAYSALES
            //                 WHERE ASE.AREA_CODE = MTDCOL.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = TODAY_COLLECTION.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = YTDCOL.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = YTDSALES.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = MTDSALES.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = TODAYSALES.AREA_CODE(+)
            //                 ) WHERE AREA_CODE in ('{1}')
            //                ORDER BY AREA_EDESC ASC";
            //    Query = string.Format(Query, model.FromDate, string.Join("','", model.AreaTypeFilter));
            //}
            //else
            //{
            //    Query = @"SELECT to_char(AREA_CODE) AreaCode
            //                 ,AREA_EDESC AreaName
            //                 ,COMPANY_CODE CompanyCode
            //                 ,TO_NUMBER(TODAY_SALES) DailySales
            //                 ,TO_NUMBER(TODAY_COLLECTION) DailyCollection
            //                 ,TO_NUMBER(MTD_SALES) MonthlySales
            //                 ,TO_NUMBER(MTD_COLLECTION) MonthlyCollection
            //                 ,TO_NUMBER(YTD_SALES) YearlySales
            //                 ,TO_NUMBER(YTD_COLLECTION) YearlyCollection
            //                FROM (
            //                 SELECT ASE.*
            //                  ,NVL(TODAYSALES.SALES_AMT, 0) TODAY_SALES
            //                  ,NVL(TODAY_COLLECTION.COLLECTION_AMT, 0) TODAY_COLLECTION
            //                  ,NVL(MTDSALES.SALES_AMT, 0) MTD_SALES
            //                  ,NVL(MTDCOL.COLLECTION_AMT, 0) MTD_COLLECTION
            //                  ,NVL(YTDSALES.SALES_AMT, 0) YTD_SALES
            //                  ,NVL(YTDCOL.COLLECTION_AMT, 0) YTD_COLLECTION
            //                 FROM AREA_SETUP ASE
            //                  ,(
            //                   SELECT PTC.AREA_CODE AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(CR_AMOUNT) COLLECTION_AMT
            //                   FROM V$VIRTUAL_SUB_LEDGER VSL
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                    ,CALENDAR_SETUP CAS
            //                   WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
            //                    AND VSL.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
            //                    AND PTC.AREA_CODE = AST.AREA_CODE
            //                    AND VSL.COMPANY_CODE = AST.COMPANY_CODE
            //                    --AND TRUNC(VSL.VOUCHER_DATE) = TRUNC(TO_DATE ('{0}','YYYY-MM-DD'))
            //                    AND TO_DATE ('{0}','YYYY-MM-DD') BETWEEN CAS.AD_DATE
            //                     AND CAS.AD_DATE + CAS.DAYS_NO
            //                    AND SUBSTR(TO_BS(VSL.VOUCHER_DATE), 4, 2) = SUBSTR(CAS.BS_MONTH, 6, 2)
            //                   GROUP BY PTC.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                   ) MTDCOL
            //                  ,(
            //                   SELECT PTC.AREA_CODE AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(CR_AMOUNT) COLLECTION_AMT
            //                   FROM V$VIRTUAL_SUB_LEDGER VSL
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                   WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
            //                    AND VSL.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
            //                    AND PTC.AREA_CODE = AST.AREA_CODE
            //                    AND VSL.COMPANY_CODE = AST.COMPANY_CODE
            //                   GROUP BY PTC.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                   ) YTDCOL
            //                  ,(
            //                   SELECT PTC.AREA_CODE AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(CR_AMOUNT) COLLECTION_AMT
            //                   FROM V$VIRTUAL_SUB_LEDGER VSL
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                   WHERE TRIM(VSL.SUB_CODE) = TRIM(CS.LINK_SUB_CODE)
            //                    AND VSL.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND VSL.COMPANY_CODE = PTC.COMPANY_CODE
            //                    AND PTC.AREA_CODE = AST.AREA_CODE
            //                    AND VSL.COMPANY_CODE = AST.COMPANY_CODE
            //                    AND VSL.VOUCHER_DATE = TO_DATE ('{0}','YYYY-MM-DD')
            //                   GROUP BY PTC.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                   ) TODAY_COLLECTION
            //                  ,(
            //                   SELECT AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(NVL(SI.NET_SALES, 0)) SALES_AMT
            //                   FROM V$SALES_INVOICE_REPORT3 SI
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                    ,SA_CUSTOMER_SETUP CS
            //                   WHERE TRIM(SI.CUSTOMER_CODE) = TRIM(CS.CUSTOMER_CODE)
            //                    AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
            //                    AND AST.AREA_CODE = PTC.AREA_CODE
            //                    AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
            //                    AND PTC.COMPANY_CODE = AST.COMPANY_CODE
            //                    AND SI.COMPANY_CODE = CS.COMPANY_CODE
            //                   GROUP BY AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    --ORDER BY AST.AREA_EDESC
            //                   ) YTDSALES
            //                  ,(
            //                   SELECT AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(NVL(SI.NET_SALES, 0)) SALES_AMT
            //                   FROM V$SALES_INVOICE_REPORT3 SI
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,CALENDAR_SETUP CAS
            //                   WHERE TRIM(SI.CUSTOMER_CODE) = TRIM(CS.CUSTOMER_CODE)
            //                    AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
            //                    AND AST.AREA_CODE = PTC.AREA_CODE
            //                    AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
            //                    AND PTC.COMPANY_CODE = AST.COMPANY_CODE
            //                    AND SI.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND TO_DATE ('{0}','YYYY-MM-DD') BETWEEN CAS.AD_DATE
            //                     AND CAS.AD_DATE + CAS.DAYS_NO
            //                    AND SUBSTR(TO_BS(SI.SALES_DATE), 4, 2) = SUBSTR(CAS.BS_MONTH, 6, 2)
            //                   GROUP BY AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    --	ORDER BY AST.AREA_EDESC
            //                   ) MTDSALES
            //                  ,(
            //                   SELECT AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    ,SUM(NVL(SI.NET_SALES, 0)) SALES_AMT
            //                   FROM V$SALES_INVOICE_REPORT3 SI
            //                    ,IP_PARTY_TYPE_CODE PTC
            //                    ,AREA_SETUP AST
            //                    ,SA_CUSTOMER_SETUP CS
            //                    ,CALENDAR_SETUP CAS
            //                   WHERE TRIM(SI.CUSTOMER_CODE) = TRIM(CS.CUSTOMER_CODE)
            //                    AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE
            //                    AND AST.AREA_CODE = PTC.AREA_CODE
            //                    AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
            //                    AND PTC.COMPANY_CODE = AST.COMPANY_CODE
            //                    AND SI.COMPANY_CODE = CS.COMPANY_CODE
            //                    AND TRUNC(SI.SALES_DATE) = TO_DATE((TO_DATE ('{0}','YYYY-MM-DD')), 'DD-MM-YYYY')
            //                   GROUP BY AST.AREA_CODE
            //                    ,AST.AREA_EDESC
            //                    --ORDER BY AST.AREA_EDESC
            //                   ) TODAYSALES
            //                 WHERE ASE.AREA_CODE = MTDCOL.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = TODAY_COLLECTION.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = YTDCOL.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = YTDSALES.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = MTDSALES.AREA_CODE(+)
            //                  AND ASE.AREA_CODE = TODAYSALES.AREA_CODE(+)
            //                 )
            //                ORDER BY AREA_EDESC ASC";
            //    //Query = string.Format(Query, model.FromDate, model.ToDate);
            //    Query = string.Format(Query, model.FromDate);
            //}


            areaWiseSalesAndCollectionModel = this._objectEntity.SqlQuery<AreaWiseSalesAndCollectionModel>(Query).ToList();

            return areaWiseSalesAndCollectionModel;
        }

        //public IList<BrandWiseGrossProfitModel> GetBrandWiseGrossProfit(ReportFiltersModel model, User userInfo)
        //{
        //    var brandWiseGrossProfit = new List<BrandWiseGrossProfitModel>();
        //    var Query = string.Empty;
        //    var companyCode = userInfo.company_code;
        //    var branchCode = userInfo.branch_code;
        //    var itemCode = string.Empty;
        //    if (model.CompanyFilter.Count > 0)
        //    {
        //        companyCode = string.Join("','", model.CompanyFilter);
        //    }
        //    if (model.BranchFilter.Count > 0)
        //    {
        //        branchCode = string.Join("','", model.BranchFilter);
        //    }
        //    if (model.ProductFilter.Count > 0)
        //    {
        //        itemCode = string.Join("','", model.ProductFilter);
        //    }
        //    if (model.ProductFilter.Count > 0)
        //    {
        //        Query = @"SELECT TO_CHAR(X.ITEM_CODE) ItemCode
        //                     ,X.ITEM_EDESC ItemName
        //                     ,TO_CHAR(nvl(X.BRAND_NAME,'-')) BrandName
        //                     ,X.GROUP_SKU_FLAG GroupSkuFlag
        //                     ,X.INDEX_MU_CODE IndexMuCode
        //                     ,X.MASTER_ITEM_CODE MasterItemCode
        //                     ,TO_NUMBER(X.LEVEL_ROW) LevelRow
        //                        ,TO_NUMBER(NVL(TRIM(X.AVG_SALES_RATE), 0)) AverageSalesRate
        //                     --,TO_NUMBER(NVL(TRIM(X.AVG_PUR_RATE),0))  AveragePurchaseRate
        //                     ,TO_NUMBER(NVL(TRIM(X.SALES_QUANTITY), 0)) SalesQuantity
        //                     ,TO_NUMBER(NVL(TRIM(X.PURCHASE_QUANTITY),0)) PurchaseQuantity
        //                     ,TO_NUMBER(NVL(TRIM(X.TOTAL_PURCHASE_AMOUNT),0)) TotalPurchaseAmount
        //                     ,TO_NUMBER(NVL(TRIM(X.TOTAL_SALES_AMOUNT), 0)) TotalSalesAmount
        //                     ,TO_NUMBER(NVL(TRIM(X.CLOSING_VALUE),0)) ClosingValue
        //                     ,TO_NUMBER(NVL(TRIM(X.ITEM_WISE_PROFIT), 0)) ItemWiseProfit
        //                        ,TO_NUMBER(NVL(TRIM(X.NEW_ITEM_WISE_PROFIT),0)) NewItemWiseProfit
        //                     ,TO_NUMBER(NVL(TRIM(X.PROFIT_PER), 0)) ProfitPer
        //                    FROM (
        //                     SELECT B.ITEM_CODE
        //                      ,B.ITEM_EDESC
        //                      ,C.BRAND_NAME
        //                      ,B.GROUP_SKU_FLAG
        //                      ,B.INDEX_MU_CODE
        //                      ,B.MASTER_ITEM_CODE
        //                      ,(LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE, '.', ''))) LEVEL_ROW
        //                      ,AVG_SALES_RATE
        //                      ,AVG_PUR_RATE
        //                      ,SALES_QUANTITY
        //                      ,PURCHASE_QUANTITY
        //                      ,TOTAL_PURCHASE_AMOUNT
        //                      ,TOTAL_SALES_AMOUNT
        //                      ,CLOSING_VALUE
        //                      ,ROUND((AVG_SALES_RATE - AVG_PUR_RATE) * SALES_QUANTITY, 2) ITEM_WISE_PROFIT
        //                      ,ROUND(TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE), 2) NEW_ITEM_WISE_PROFIT
        //                      ,CASE 
        //                       WHEN TOTAL_SALES_AMOUNT <> 0
        //                        THEN CASE 
        //                          WHEN TOTAL_PURCHASE_AMOUNT <> 0
        //                           AND (TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE)) <> 0
        //                           THEN
        //                            --ROUND( ((TOTAL_SALES_AMOUNT- TOTAL_PURCHASE_AMOUNT)/DECODE(TOTAL_PURCHASE_AMOUNT,0,1,TOTAL_PURCHASE_AMOUNT) ) *100,2)
        //                            ROUND(TOTAL_SALES_AMOUNT / (ROUND(TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE), 2)) * 100, 2)
        //                          ELSE 100
        //                          END
        //                       ELSE 0
        //                       END PROFIT_PER
        //                     FROM (
        //                      SELECT ITEM_CODE
        //                       ,COMPANY_CODE
        //                       ,NVL((
        //                         SELECT SUM((NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0)) - (NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)))
        //                         FROM IMSGROUP7879.V$VIRTUAL_VALUE_STOCK_LEDGER
        //                         WHERE COMPANY_CODE = A.COMPANY_CODE
        //                          AND ITEM_CODE = A.ITEM_CODE
        //                          AND (TRUNC(VOUCHER_DATE) <= TO_DATE('{1}','YYYY-MON-DD'))
        //                                            AND COMPANY_CODE IN ('{2}')
        //                          AND BRANCH_CODE IN ('{3}')
        //                          AND DELETED_FLAG = 'N'
        //                         ), 0) CLOSING_VALUE
        //                       ,SUM(SALES_QUANTITY) SALES_QUANTITY
        //                       ,SUM(TOTAL_SALES_AMOUNT) TOTAL_SALES_AMOUNT
        //                       ,SUM(AVG_SALES_RATE) AVG_SALES_RATE
        //                       ,SUM(PURCHASE_QUANTITY) PURCHASE_QUANTITY
        //                       ,SUM(TOTAL_PURCHASE_AMOUNT) TOTAL_PURCHASE_AMOUNT
        //                       ,SUM(AVG_PUR_RATE) AVG_PUR_RATE
        //                      FROM (
        //                       SELECT ITEM_CODE
        //                        ,COMPANY_CODE
        //                        ,SUM(QUANTITY) SALES_QUANTITY
        //                        ,SUM(TOTAL_AMOUNT) TOTAL_SALES_AMOUNT
        //                        ,SUM(TOTAL_AMOUNT) / DECODE(SUM(QUANTITY), 0, 1, SUM(QUANTITY)) AVG_SALES_RATE
        //                        ,0 PURCHASE_QUANTITY
        //                        ,0 AVG_PUR_RATE
        //                        ,0 TOTAL_PURCHASE_AMOUNT
        //                       FROM IMSGROUP7879.V_ITEM_WISE_OUT_VALUES
        //                       WHERE 1 = 1
        //                        AND COMPANY_CODE IN ('{2}')
        //                        AND BRANCH_CODE IN ('{3}')
        //                        AND VOUCHER_DATE BETWEEN TO_DATE('{0}','YYYY-MON-DD')
        //                         AND TO_DATE('{1}','YYYY-MON-DD')
        //                        AND DELETED_FLAG = 'N'
        //                       GROUP BY ITEM_CODE
        //                        ,COMPANY_CODE

        //                       UNION ALL

        //                       SELECT ITEM_CODE
        //                        ,COMPANY_CODE
        //                        ,0 SALES_QUANTITY
        //                        ,0 TOTAL_SALES_AMOUNT
        //                        ,0 AVG_SALES_RATE
        //                        ,SUM(QUANTITY) PURCHASE_QUANTITY
        //                        ,SUM(TOTAL_AMOUNT) / DECODE(SUM(QUANTITY), 0, 1, SUM(QUANTITY)) AVG_PUR_RATE
        //                        ,SUM(TOTAL_AMOUNT) TOTAL_PURCHASE_AMOUNT
        //                       FROM (
        //                        SELECT ITEM_CODE
        //                         ,COMPANY_CODE
        //                         ,NVL(SUM(NVL(IN_QUANTITY, 0)) - SUM(NVL(OUT_QUANTITY, 0)), 0) QUANTITY
        //                         ,NVL(SUM(NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0)) - SUM(NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)), 0) TOTAL_AMOUNT
        //                        FROM IMSGROUP7879.IP_TEMP_VALUE_LEDGER
        //                        WHERE 1 = 1
        //                         AND COMPANY_CODE IN ('{2}')
        //                         AND BRANCH_CODE IN ('{3}')
        //                         AND METHOD = 'FIFO'
        //                         AND (
        //                          VOUCHER_DATE < TO_DATE('{1}','YYYY-MON-DD')
        //                          OR FORM_CODE = '0'
        //                          )
        //                         AND DELETED_FLAG = 'N'
        //                        GROUP BY ITEM_CODE
        //                         ,COMPANY_CODE

        //                        UNION ALL

        //                        SELECT ITEM_CODE
        //                         ,COMPANY_CODE
        //                         ,SUM(QUANTITY) QUANTITY
        //                         ,SUM(TOTAL_AMOUNT) TOTAL_AMOUNT
        //                        FROM IMSGROUP7879.V_ITEM_WISE_IN_VALUES
        //                        WHERE 1 = 1
        //                         AND COMPANY_CODE IN ('{2}')
        //                         AND BRANCH_CODE IN ('{3}')
        //                         AND VOUCHER_DATE BETWEEN TO_DATE('{0}','YYYY-MON-DD')
        //                          AND TO_DATE('{1}','YYYY-MON-DD')
        //                         AND DELETED_FLAG = 'N'
        //                         AND FORM_CODE <> '0'
        //                        GROUP BY ITEM_CODE
        //                         ,COMPANY_CODE
        //                        )
        //                       GROUP BY ITEM_CODE
        //                        ,COMPANY_CODE
        //                       ) A
        //                      GROUP BY ITEM_CODE
        //                       ,COMPANY_CODE
        //                      ) A
        //                      ,IMSGROUP7879.IP_ITEM_MASTER_SETUP B
        //                      ,IMSGROUP7879.IP_ITEM_SPEC_SETUP C
        //                     WHERE 1 = 1
        //                      AND B.ITEM_CODE = A.ITEM_CODE(+)
        //                      AND B.COMPANY_CODE = A.COMPANY_CODE(+)
        //                      AND B.COMPANY_CODE IN ('{2}')
        //                      AND B.COMPANY_CODE = C.COMPANY_CODE
        //                      AND B.ITEM_CODE = C.ITEM_CODE
        //                      AND B.ITEM_CODE IN ('{4}')
        //                     ORDER BY B.MASTER_ITEM_CODE
        //                     ) X";
        //        Query = string.Format(Query, model.FromDate, model.ToDate, companyCode, branchCode, itemCode);

        //    }
        //    else
        //    {
        //        Query = @"SELECT TO_CHAR(REGEXP_REPLACE(X.ITEM_CODE, '[^a-zA-Z ]', '')) ItemCode
        //                     ,X.ITEM_EDESC ItemName
        //                     ,TO_CHAR(nvl(REGEXP_REPLACE(X.BRAND_NAME, '[^a-zA-Z ]', ''),'-')) BrandName
        //                     ,X.GROUP_SKU_FLAG GroupSkuFlag
        //                     ,X.INDEX_MU_CODE IndexMuCode
        //                     ,X.MASTER_ITEM_CODE MasterItemCode
        //                     ,TO_NUMBER(X.LEVEL_ROW) LevelRow
        //                        ,TO_NUMBER(NVL(TRIM(REGEXP_REPLACE(X.AVG_SALES_RATE, '[^a-zA-Z ]', '')), 0)) AverageSalesRate
        //                     --,TO_NUMBER(NVL(TRIM(X.AVG_PUR_RATE),0))  AveragePurchaseRate
        //                     ,TO_NUMBER(NVL(TRIM(X.SALES_QUANTITY), 0)) SalesQuantity
        //                     ,TO_NUMBER(NVL(TRIM(X.PURCHASE_QUANTITY),0)) PurchaseQuantity
        //                     ,TO_NUMBER(NVL(TRIM(X.TOTAL_PURCHASE_AMOUNT),0)) TotalPurchaseAmount
        //                     ,TO_NUMBER(NVL(TRIM(X.TOTAL_SALES_AMOUNT), 0)) TotalSalesAmount
        //                     ,TO_NUMBER(NVL(TRIM(X.CLOSING_VALUE),0)) ClosingValue
        //                     ,TO_NUMBER(NVL(TRIM(X.ITEM_WISE_PROFIT), 0)) ItemWiseProfit
        //                        ,TO_NUMBER(NVL(TRIM(X.NEW_ITEM_WISE_PROFIT),0)) NewItemWiseProfit
        //                     ,TO_NUMBER(NVL(TRIM(X.PROFIT_PER), 0)) ProfitPer
        //                    FROM (
        //                     SELECT B.ITEM_CODE
        //                      ,B.ITEM_EDESC
        //                      ,C.BRAND_NAME
        //                      ,B.GROUP_SKU_FLAG
        //                      ,B.INDEX_MU_CODE
        //                      ,B.MASTER_ITEM_CODE
        //                      ,(LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE, '.', ''))) LEVEL_ROW
        //                      ,AVG_SALES_RATE
        //                      ,AVG_PUR_RATE
        //                      ,SALES_QUANTITY
        //                      ,PURCHASE_QUANTITY
        //                      ,TOTAL_PURCHASE_AMOUNT
        //                      ,TOTAL_SALES_AMOUNT
        //                      ,CLOSING_VALUE
        //                      ,ROUND((AVG_SALES_RATE - AVG_PUR_RATE) * SALES_QUANTITY, 2) ITEM_WISE_PROFIT
        //                      ,ROUND(TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE), 2) NEW_ITEM_WISE_PROFIT
        //                      ,CASE 
        //                       WHEN TOTAL_SALES_AMOUNT <> 0
        //                        THEN CASE 
        //                          WHEN TOTAL_PURCHASE_AMOUNT <> 0
        //                           AND (TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE)) <> 0
        //                           THEN
        //                            --ROUND( ((TOTAL_SALES_AMOUNT- TOTAL_PURCHASE_AMOUNT)/DECODE(TOTAL_PURCHASE_AMOUNT,0,1,TOTAL_PURCHASE_AMOUNT) ) *100,2)
        //                            ROUND(TOTAL_SALES_AMOUNT / (ROUND(TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE), 2)) * 100, 2)
        //                          ELSE 100
        //                          END
        //                       ELSE 0
        //                       END PROFIT_PER
        //                     FROM (
        //                      SELECT ITEM_CODE
        //                       ,COMPANY_CODE
        //                       ,NVL((
        //                         SELECT SUM((NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0)) - (NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)))
        //                         FROM IMSGROUP7879.V$VIRTUAL_VALUE_STOCK_LEDGER
        //                         WHERE COMPANY_CODE = A.COMPANY_CODE
        //                          AND ITEM_CODE = A.ITEM_CODE
        //                          AND (TRUNC(VOUCHER_DATE) <= TO_DATE('{1}','YYYY-MON-DD'))
        //                          AND BRANCH_CODE IN ('{3}')
        //                          AND DELETED_FLAG = 'N'
        //                         ), 0) CLOSING_VALUE
        //                       ,SUM(SALES_QUANTITY) SALES_QUANTITY
        //                       ,SUM(TOTAL_SALES_AMOUNT) TOTAL_SALES_AMOUNT
        //                       ,SUM(AVG_SALES_RATE) AVG_SALES_RATE
        //                       ,SUM(PURCHASE_QUANTITY) PURCHASE_QUANTITY
        //                       ,SUM(TOTAL_PURCHASE_AMOUNT) TOTAL_PURCHASE_AMOUNT
        //                       ,SUM(AVG_PUR_RATE) AVG_PUR_RATE
        //                      FROM (
        //                       SELECT ITEM_CODE
        //                        ,COMPANY_CODE
        //                        ,SUM(QUANTITY) SALES_QUANTITY
        //                        ,SUM(TOTAL_AMOUNT) TOTAL_SALES_AMOUNT
        //                        ,SUM(TOTAL_AMOUNT) / DECODE(SUM(QUANTITY), 0, 1, SUM(QUANTITY)) AVG_SALES_RATE
        //                        ,0 PURCHASE_QUANTITY
        //                        ,0 AVG_PUR_RATE
        //                        ,0 TOTAL_PURCHASE_AMOUNT
        //                       FROM IMSGROUP7879.V_ITEM_WISE_OUT_VALUES
        //                       WHERE 1 = 1
        //                        AND COMPANY_CODE IN ('{2}')
        //                        AND BRANCH_CODE IN ('{3}')
        //                        AND VOUCHER_DATE BETWEEN TO_DATE('{0}','YYYY-MON-DD')
        //                         AND TO_DATE('{1}','YYYY-MON-DD')
        //                        AND DELETED_FLAG = 'N'
        //                       GROUP BY ITEM_CODE
        //                        ,COMPANY_CODE

        //                       UNION ALL

        //                       SELECT ITEM_CODE
        //                        ,COMPANY_CODE
        //                        ,0 SALES_QUANTITY
        //                        ,0 TOTAL_SALES_AMOUNT
        //                        ,0 AVG_SALES_RATE
        //                        ,SUM(QUANTITY) PURCHASE_QUANTITY
        //                        ,SUM(TOTAL_AMOUNT) / DECODE(SUM(QUANTITY), 0, 1, SUM(QUANTITY)) AVG_PUR_RATE
        //                        ,SUM(TOTAL_AMOUNT) TOTAL_PURCHASE_AMOUNT
        //                       FROM (
        //                        SELECT ITEM_CODE
        //                         ,COMPANY_CODE
        //                         ,NVL(SUM(NVL(IN_QUANTITY, 0)) - SUM(NVL(OUT_QUANTITY, 0)), 0) QUANTITY
        //                         ,NVL(SUM(NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0)) - SUM(NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)), 0) TOTAL_AMOUNT
        //                        FROM IMSGROUP7879.IP_TEMP_VALUE_LEDGER
        //                        WHERE 1 = 1
        //                         AND COMPANY_CODE IN ('{2}')
        //                         AND BRANCH_CODE IN ('{3}')
        //                         AND METHOD = 'FIFO'
        //                         AND (
        //                          VOUCHER_DATE < TO_DATE('{1}','YYYY-MON-DD')
        //                          OR FORM_CODE = '0'
        //                          )
        //                         AND DELETED_FLAG = 'N'
        //                        GROUP BY ITEM_CODE
        //                         ,COMPANY_CODE

        //                        UNION ALL

        //                        SELECT ITEM_CODE
        //                         ,COMPANY_CODE
        //                         ,SUM(QUANTITY) QUANTITY
        //                         ,SUM(TOTAL_AMOUNT) TOTAL_AMOUNT
        //                        FROM IMSGROUP7879.V_ITEM_WISE_IN_VALUES
        //                        WHERE 1 = 1
        //                         AND COMPANY_CODE IN ('{2}')
        //                         AND BRANCH_CODE IN ('{3}')
        //                         AND VOUCHER_DATE BETWEEN TO_DATE('{0}','YYYY-MON-DD')
        //                          AND TO_DATE('{1}','YYYY-MON-DD')
        //                         AND DELETED_FLAG = 'N'
        //                         AND FORM_CODE <> '0'
        //                        GROUP BY ITEM_CODE
        //                         ,COMPANY_CODE
        //                        )
        //                       GROUP BY ITEM_CODE
        //                        ,COMPANY_CODE
        //                       ) A
        //                      GROUP BY ITEM_CODE
        //                       ,COMPANY_CODE
        //                      ) A
        //                      ,IMSGROUP7879.IP_ITEM_MASTER_SETUP B
        //                      ,IMSGROUP7879.IP_ITEM_SPEC_SETUP C
        //                     WHERE 1 = 1
        //                      AND B.ITEM_CODE = A.ITEM_CODE(+)
        //                      AND B.COMPANY_CODE = A.COMPANY_CODE(+)
        //                      AND B.COMPANY_CODE IN ('{2}')
        //                      AND B.COMPANY_CODE = C.COMPANY_CODE
        //                      AND B.ITEM_CODE = C.ITEM_CODE
        //                     ORDER BY B.MASTER_ITEM_CODE
        //                     ) X";
        //        Query = string.Format(Query, model.FromDate, model.ToDate, companyCode, branchCode);
        //    }


        //    brandWiseGrossProfit = this._objectEntity.SqlQuery<BrandWiseGrossProfitModel>(Query).ToList();

        //    return brandWiseGrossProfit;
        //}

        public IList<BrandWiseGrossProfitModel> GetBrandWiseGrossProfit(ReportFiltersModel model, User userInfo)
        {
            var brandWiseGrossProfit = new List<BrandWiseGrossProfitModel>();
            var Query = string.Empty;
            var companyFilter = string.Empty;
            var itemFilter = string.Empty;
            var branchFilter = string.Empty;
            var companyCode = userInfo.company_code;
            var branchCode = userInfo.branch_code;
            var itemCode = string.Empty;
            var fromDate = $"TO_DATE('{model.FromDate}', 'YYYY-MON-DD')";
            var toData = $"TO_DATE('{model.ToDate}', 'YYYY-MON-DD')";
            if (model.CompanyFilter.Count > 0)
            {
                companyCode = string.Join("','", model.CompanyFilter);
                //companyFilter = $"AND COMPANY_CODE IN ('{ string.Join("','", model.CompanyFilter).ToString()}')";
            }
            //else
            //{
            //    companyFilter = $"AND COMPANY_CODE IN ('{companyCode}')";
            //}
            if (model.BranchFilter.Count > 0)
            {
                branchCode = string.Join("','", model.BranchFilter);
                branchFilter = $"AND BRANCH_CODE IN ('{ string.Join("','", model.BranchFilter).ToString()}')";
            }
            if (model.ProductFilter.Count > 0)
            {
                itemCode = string.Join("','", model.ProductFilter);
                itemFilter = $"AND B.ITEM_CODE IN ('{ string.Join("','", model.ProductFilter).ToString()}')";
            }
            //previous group one
            //Query = $@"SELECT TO_CHAR(X.ITEM_CODE) ItemCode
            //                 ,X.ITEM_EDESC ItemName
            //                 ,TO_CHAR(nvl(REGEXP_REPLACE(X.NewBrandName, '[^a-zA-Z ]', ''),'-')) BrandName
            //                 ,X.GROUP_SKU_FLAG GroupSkuFlag
            //                 ,X.INDEX_MU_CODE IndexMuCode
            //                 ,X.MASTER_ITEM_CODE MasterItemCode
            //                 ,TO_NUMBER(X.LEVEL_ROW) LevelRow
            //                    ,ROUND(TO_NUMBER(NVL(TRIM(X.AVG_SALES_RATE), 0)),2) AverageSalesRate
            //                    --,TO_NUMBER(NVL(TRIM(REGEXP_REPLACE(X.AVG_SALES_RATE, '[^[:digit:]]', '')), 0)) AverageSalesRate
            //                 ,ROUND(TO_NUMBER(NVL(TRIM(X.AVG_PUR_RATE),0)),2)  AveragePurchaseRate
            //                 ,ROUND(TO_NUMBER(NVL(TRIM(X.SALES_QUANTITY), 0)),2) SalesQuantity
            //                 ,ROUND(TO_NUMBER(NVL(TRIM(X.PURCHASE_QUANTITY),0)),2) PurchaseQuantity
            //                 ,ROUND(TO_NUMBER(NVL(TRIM(X.TOTAL_PURCHASE_AMOUNT),0)),2) TotalPurchaseAmount
            //                 ,ROUND(TO_NUMBER(NVL(TRIM(X.TOTAL_SALES_AMOUNT), 0)),2) TotalSalesAmount
            //                 ,ROUND(TO_NUMBER(NVL(TRIM(X.CLOSING_VALUE),0)),2) ClosingValue
            //                 ,ROUND(TO_NUMBER(NVL(TRIM(X.ITEM_WISE_PROFIT), 0)),2) ItemWiseProfit
            //                    ,ROUND(TO_NUMBER(NVL(TRIM(X.NEW_ITEM_WISE_PROFIT),0)),2) NewItemWiseProfit
            //                 ,ROUND(TO_NUMBER(NVL(TRIM(X.PROFIT_PER), 0)),2) ProfitPer
            //                FROM (
            //                 SELECT B.ITEM_CODE
            //                  ,B.ITEM_EDESC
            //                  ,C.BRAND_NAME
            //                        ,FN_FETCH_BRAND_DESC(B.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', B.PRE_ITEM_CODE) NewBrandName
            //                  ,B.GROUP_SKU_FLAG
            //                  ,B.INDEX_MU_CODE
            //                  ,B.MASTER_ITEM_CODE
            //                  ,(LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE, '.', ''))) LEVEL_ROW
            //                  ,AVG_SALES_RATE
            //                  ,AVG_PUR_RATE
            //                  ,SALES_QUANTITY
            //                  ,PURCHASE_QUANTITY
            //                  ,TOTAL_PURCHASE_AMOUNT
            //                  ,TOTAL_SALES_AMOUNT
            //                  ,CLOSING_VALUE
            //                  ,ROUND((AVG_SALES_RATE - AVG_PUR_RATE) * SALES_QUANTITY, 2) ITEM_WISE_PROFIT
            //                  ,ROUND(TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE), 2) NEW_ITEM_WISE_PROFIT
            //                  ,CASE 
            //                   WHEN TOTAL_SALES_AMOUNT <> 0
            //                    THEN CASE 
            //                      WHEN TOTAL_PURCHASE_AMOUNT <> 0
            //                       AND (TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE)) <> 0
            //                       THEN
            //                        ROUND( ((TOTAL_SALES_AMOUNT- TOTAL_PURCHASE_AMOUNT)/DECODE(TOTAL_PURCHASE_AMOUNT,0,1,TOTAL_PURCHASE_AMOUNT) ) *100,2)
            //                        --ROUND(TOTAL_SALES_AMOUNT / (ROUND(TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE), 2)) * 100, 2)
            //                      ELSE 100
            //                      END
            //                   ELSE 0
            //                   END PROFIT_PER
            //                 FROM (
            //                  SELECT ITEM_CODE
            //                   ,COMPANY_CODE
            //                   ,NVL((
            //                     SELECT SUM((NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0)) - (NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)))
            //                     FROM IMSGROUP7879.V$VIRTUAL_VALUE_STOCK_LEDGER
            //                     WHERE COMPANY_CODE = A.COMPANY_CODE
            //                      AND ITEM_CODE = A.ITEM_CODE
            //                                        AND COMPANY_CODE IN ('{companyCode}')
            //                                        {branchFilter}
            //                      AND DELETED_FLAG = 'N'
            //                     ), 0) CLOSING_VALUE
            //                   ,SUM(SALES_QUANTITY) SALES_QUANTITY
            //                   ,SUM(TOTAL_SALES_AMOUNT) TOTAL_SALES_AMOUNT
            //                   ,SUM(AVG_SALES_RATE) AVG_SALES_RATE
            //                   ,SUM(PURCHASE_QUANTITY) PURCHASE_QUANTITY
            //                   ,SUM(TOTAL_PURCHASE_AMOUNT) TOTAL_PURCHASE_AMOUNT
            //                   ,SUM(AVG_PUR_RATE) AVG_PUR_RATE
            //                  FROM (
            //                   SELECT ITEM_CODE
            //                    ,COMPANY_CODE
            //                    ,SUM(QUANTITY) SALES_QUANTITY
            //                    ,SUM(TOTAL_AMOUNT) TOTAL_SALES_AMOUNT
            //                    ,SUM(TOTAL_AMOUNT) / DECODE(SUM(QUANTITY), 0, 1, SUM(QUANTITY)) AVG_SALES_RATE
            //                    ,0 PURCHASE_QUANTITY
            //                    ,0 AVG_PUR_RATE
            //                    ,0 TOTAL_PURCHASE_AMOUNT
            //                   FROM IMSGROUP7879.V_ITEM_WISE_OUT_VALUES
            //                   WHERE 1 = 1
            //                   AND COMPANY_CODE IN ('{companyCode}')
            //                               {branchFilter}
            //                   AND DELETED_FLAG = 'N'
            //                   GROUP BY ITEM_CODE
            //                    ,COMPANY_CODE

            //                   UNION ALL

            //                   SELECT ITEM_CODE
            //                    ,COMPANY_CODE
            //                    ,0 SALES_QUANTITY
            //                    ,0 TOTAL_SALES_AMOUNT
            //                    ,0 AVG_SALES_RATE
            //                    ,SUM(QUANTITY) PURCHASE_QUANTITY
            //                    ,SUM(TOTAL_AMOUNT) / DECODE(SUM(QUANTITY), 0, 1, SUM(QUANTITY)) AVG_PUR_RATE
            //                    ,SUM(TOTAL_AMOUNT) TOTAL_PURCHASE_AMOUNT
            //                   FROM (
            //                    SELECT ITEM_CODE
            //                     ,COMPANY_CODE
            //                     ,NVL(SUM(NVL(IN_QUANTITY, 0)) - SUM(NVL(OUT_QUANTITY, 0)), 0) QUANTITY
            //                     ,NVL(SUM(NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0)) - SUM(NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)), 0) TOTAL_AMOUNT
            //                    FROM IMSGROUP7879.IP_TEMP_VALUE_LEDGER
            //                    WHERE 1 = 1
            //                                    AND COMPANY_CODE IN ('{companyCode}')
            //                                    {branchFilter}
            //                     AND METHOD = 'FIFO'
            //                     AND (
            //                      VOUCHER_DATE Between {fromDate} and {toData}
            //                      -- OR FORM_CODE = '0'   not sure why it is kept so commented(Animesh)
            //                      )
            //                     AND DELETED_FLAG = 'N'
            //                    GROUP BY ITEM_CODE
            //                     ,COMPANY_CODE

            //                    UNION ALL

            //                    SELECT ITEM_CODE
            //                     ,COMPANY_CODE
            //                     ,SUM(QUANTITY) QUANTITY
            //                     ,SUM(TOTAL_AMOUNT) TOTAL_AMOUNT
            //                    FROM IMSGROUP7879.V_ITEM_WISE_IN_VALUES
            //                    WHERE 1 = 1
            //                     AND COMPANY_CODE IN ('{companyCode}')
            //                                    {branchFilter}
            //                     AND DELETED_FLAG = 'N'
            //                     AND FORM_CODE <> '0'
            //                    GROUP BY ITEM_CODE
            //                     ,COMPANY_CODE
            //                    )
            //                   GROUP BY ITEM_CODE
            //                    ,COMPANY_CODE
            //                   ) A
            //                  GROUP BY ITEM_CODE
            //                   ,COMPANY_CODE
            //                  ) A
            //                  ,IMSGROUP7879.IP_ITEM_MASTER_SETUP B
            //                  ,IMSGROUP7879.IP_ITEM_SPEC_SETUP C
            //                 WHERE 1 = 1
            //                  AND B.ITEM_CODE = A.ITEM_CODE(+)
            //                  AND B.COMPANY_CODE = A.COMPANY_CODE(+)
            //                  AND B.COMPANY_CODE IN ('{companyCode}')
            //                  AND B.COMPANY_CODE = C.COMPANY_CODE
            //                  AND B.ITEM_CODE = C.ITEM_CODE
            //                  {itemFilter}
            //                 ORDER BY B.MASTER_ITEM_CODE
            //                 ) X";

            //new one removed IMSGroup (Animesh)
            Query = $@"SELECT TO_CHAR(X.ITEM_CODE) ItemCode
	                            ,X.ITEM_EDESC ItemName
	                            ,TO_CHAR(nvl(REGEXP_REPLACE(X.NewBrandName, '[^a-zA-Z ]', ''),'-')) BrandName
	                            ,X.GROUP_SKU_FLAG GroupSkuFlag
	                            ,X.INDEX_MU_CODE IndexMuCode
	                            ,X.MASTER_ITEM_CODE MasterItemCode
	                            ,TO_NUMBER(X.LEVEL_ROW) LevelRow
                                ,ROUND(TO_NUMBER(NVL(TRIM(X.AVG_SALES_RATE), 0)),2) AverageSalesRate
                                --,TO_NUMBER(NVL(TRIM(REGEXP_REPLACE(X.AVG_SALES_RATE, '[^[:digit:]]', '')), 0)) AverageSalesRate
	                            ,ROUND(TO_NUMBER(NVL(TRIM(X.AVG_PUR_RATE),0)),2)  AveragePurchaseRate
	                            ,ROUND(TO_NUMBER(NVL(TRIM(X.SALES_QUANTITY), 0)),2) SalesQuantity
	                            ,ROUND(TO_NUMBER(NVL(TRIM(X.PURCHASE_QUANTITY),0)),2) PurchaseQuantity
	                            ,ROUND(TO_NUMBER(NVL(TRIM(X.TOTAL_PURCHASE_AMOUNT),0)),2) TotalPurchaseAmount
	                            ,ROUND(TO_NUMBER(NVL(TRIM(X.TOTAL_SALES_AMOUNT), 0)),2) TotalSalesAmount
	                            ,ROUND(TO_NUMBER(NVL(TRIM(X.CLOSING_VALUE),0)),2) ClosingValue
	                            ,ROUND(TO_NUMBER(NVL(TRIM(X.ITEM_WISE_PROFIT), 0)),2) ItemWiseProfit
                                ,ROUND(TO_NUMBER(NVL(TRIM(X.NEW_ITEM_WISE_PROFIT),0)),2) NewItemWiseProfit
	                            ,ROUND(TO_NUMBER(NVL(TRIM(X.PROFIT_PER), 0)),2) ProfitPer
                            FROM (
	                            SELECT B.ITEM_CODE
		                            ,B.ITEM_EDESC
		                            ,C.BRAND_NAME
                                    ,FN_FETCH_BRAND_DESC(B.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', B.PRE_ITEM_CODE) NewBrandName
		                            ,B.GROUP_SKU_FLAG
		                            ,B.INDEX_MU_CODE
		                            ,B.MASTER_ITEM_CODE
		                            ,(LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE, '.', ''))) LEVEL_ROW
		                            ,AVG_SALES_RATE
		                            ,AVG_PUR_RATE
		                            ,SALES_QUANTITY
		                            ,PURCHASE_QUANTITY
		                            ,TOTAL_PURCHASE_AMOUNT
		                            ,TOTAL_SALES_AMOUNT
		                            ,CLOSING_VALUE
		                            ,ROUND((AVG_SALES_RATE - AVG_PUR_RATE) * SALES_QUANTITY, 2) ITEM_WISE_PROFIT
		                            ,ROUND(TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE), 2) NEW_ITEM_WISE_PROFIT
		                            ,CASE 
			                            WHEN TOTAL_SALES_AMOUNT <> 0
				                            THEN CASE 
						                            WHEN TOTAL_PURCHASE_AMOUNT <> 0
							                            AND (TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE)) <> 0
							                            THEN
								                            ROUND( ((TOTAL_SALES_AMOUNT- TOTAL_PURCHASE_AMOUNT)/DECODE(TOTAL_PURCHASE_AMOUNT,0,1,TOTAL_PURCHASE_AMOUNT) ) *100,2)
								                            --ROUND(TOTAL_SALES_AMOUNT / (ROUND(TOTAL_SALES_AMOUNT - (TOTAL_PURCHASE_AMOUNT - CLOSING_VALUE), 2)) * 100, 2)
						                            ELSE 100
						                            END
			                            ELSE 0
			                            END PROFIT_PER
	                            FROM (
		                            SELECT ITEM_CODE
			                            ,COMPANY_CODE
			                            ,NVL((
					                            SELECT SUM((NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0)) - (NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)))
					                            FROM V$VIRTUAL_VALUE_STOCK_LEDGER
					                            WHERE COMPANY_CODE = A.COMPANY_CODE
						                            AND ITEM_CODE = A.ITEM_CODE
                                                    AND COMPANY_CODE IN ('{companyCode}')
                                                    {branchFilter}
						                            AND DELETED_FLAG = 'N'
					                            ), 0) CLOSING_VALUE
			                            ,SUM(SALES_QUANTITY) SALES_QUANTITY
			                            ,SUM(TOTAL_SALES_AMOUNT) TOTAL_SALES_AMOUNT
			                            ,SUM(AVG_SALES_RATE) AVG_SALES_RATE
			                            ,SUM(PURCHASE_QUANTITY) PURCHASE_QUANTITY
			                            ,SUM(TOTAL_PURCHASE_AMOUNT) TOTAL_PURCHASE_AMOUNT
			                            ,SUM(AVG_PUR_RATE) AVG_PUR_RATE
		                            FROM (
			                            SELECT ITEM_CODE
				                            ,COMPANY_CODE
				                            ,SUM(QUANTITY) SALES_QUANTITY
				                            ,SUM(TOTAL_AMOUNT) TOTAL_SALES_AMOUNT
				                            ,SUM(TOTAL_AMOUNT) / DECODE(SUM(QUANTITY), 0, 1, SUM(QUANTITY)) AVG_SALES_RATE
				                            ,0 PURCHASE_QUANTITY
				                            ,0 AVG_PUR_RATE
				                            ,0 TOTAL_PURCHASE_AMOUNT
			                            FROM V_ITEM_WISE_OUT_VALUES
			                            WHERE 1 = 1
				                           AND COMPANY_CODE IN ('{companyCode}')
                                           {branchFilter}
				                           AND DELETED_FLAG = 'N'
			                            GROUP BY ITEM_CODE
				                            ,COMPANY_CODE
			
			                            UNION ALL
			
			                            SELECT ITEM_CODE
				                            ,COMPANY_CODE
				                            ,0 SALES_QUANTITY
				                            ,0 TOTAL_SALES_AMOUNT
				                            ,0 AVG_SALES_RATE
				                            ,SUM(QUANTITY) PURCHASE_QUANTITY
				                            ,SUM(TOTAL_AMOUNT) / DECODE(SUM(QUANTITY), 0, 1, SUM(QUANTITY)) AVG_PUR_RATE
				                            ,SUM(TOTAL_AMOUNT) TOTAL_PURCHASE_AMOUNT
			                            FROM (
				                            SELECT ITEM_CODE
					                            ,COMPANY_CODE
					                            ,NVL(SUM(NVL(IN_QUANTITY, 0)) - SUM(NVL(OUT_QUANTITY, 0)), 0) QUANTITY
					                            ,NVL(SUM(NVL(IN_QUANTITY, 0) * NVL(IN_UNIT_PRICE, 0)) - SUM(NVL(OUT_QUANTITY, 0) * NVL(OUT_UNIT_PRICE, 0)), 0) TOTAL_AMOUNT
				                            FROM IP_TEMP_VALUE_LEDGER
				                            WHERE 1 = 1
                                                AND COMPANY_CODE IN ('{companyCode}')
                                                {branchFilter}
					                            AND METHOD = 'FIFO'
					                            AND (
						                            VOUCHER_DATE Between {fromDate} and {toData}
						                            -- OR FORM_CODE = '0'   not sure why it is kept so commented(Animesh)
						                            )
					                            AND DELETED_FLAG = 'N'
				                            GROUP BY ITEM_CODE
					                            ,COMPANY_CODE
				
				                            UNION ALL
				
				                            SELECT ITEM_CODE
					                            ,COMPANY_CODE
					                            ,SUM(QUANTITY) QUANTITY
					                            ,SUM(TOTAL_AMOUNT) TOTAL_AMOUNT
				                            FROM V_ITEM_WISE_IN_VALUES
				                            WHERE 1 = 1
					                            AND COMPANY_CODE IN ('{companyCode}')
                                                {branchFilter}
					                            AND DELETED_FLAG = 'N'
					                            AND FORM_CODE <> '0'
				                            GROUP BY ITEM_CODE
					                            ,COMPANY_CODE
				                            )
			                            GROUP BY ITEM_CODE
				                            ,COMPANY_CODE
			                            ) A
		                            GROUP BY ITEM_CODE
			                            ,COMPANY_CODE
		                            ) A
		                            ,IP_ITEM_MASTER_SETUP B
		                            ,IP_ITEM_SPEC_SETUP C
	                            WHERE 1 = 1
		                            AND B.ITEM_CODE = A.ITEM_CODE(+)
		                            AND B.COMPANY_CODE = A.COMPANY_CODE(+)
		                            AND B.COMPANY_CODE IN ('{companyCode}')
		                            AND B.COMPANY_CODE = C.COMPANY_CODE
		                            AND B.ITEM_CODE = C.ITEM_CODE
		                            {itemFilter}
	                            ORDER BY B.MASTER_ITEM_CODE
	                            ) X";

            Query = string.Format(Query);
            brandWiseGrossProfit = this._objectEntity.SqlQuery<BrandWiseGrossProfitModel>(Query).ToList();
            return brandWiseGrossProfit;
        }
        public IList<BrandWiseSalesModel> GetBrandWiseSales(ReportFiltersModel model, User userInfo)
        {
            var brandWiseSalesData = new List<BrandWiseSalesModel>();
            var Query = string.Empty;
            var companyCode = userInfo.company_code;
            var fromDate = $"TO_DATE('{model.FromDate}', 'YYYY-MON-DD')";
            var toData = $"TO_DATE('{model.ToDate}', 'YYYY-MON-DD')";
            if (model.CompanyFilter.Count > 0)
            {

                    //Here the query is not changed because the compnay filter is removed and can be modified later if needed Animesh
                Query = @"SELECT TO_CHAR(NVL(TRIM(A.BRAND_NAME), '***')) BrandName
	                                ,A.ITEM_CODE ItemCode
	                                ,A.ITEM_EDESC ItemName
	                                ,A.UNIT Unit
	                                ,TO_NUMBER(A.AVG_SALES_AMOUNT*A.QUANTITY) AverageSalesAmount
                                    ,TO_NUMBER(TRIM(A.QUANTITY)) Quantity
                                    ,TO_NUMBER(TRIM(A.AVG_SALES_AMOUNT)) AverageSalesRate
                                FROM (
                                    SELECT ISS.BRAND_NAME
		                                ,SI.ITEM_CODE
		                                ,IMS.ITEM_EDESC
		                                ,IMS.INDEX_MU_CODE UNIT
		                                ,CASE WHEN SUM(QUANTITY)<>0 THEN ROUND(SUM(NET_SALES) / SUM(QUANTITY), 2) ELSE 0 END AVG_SALES_AMOUNT
                                        ,SUM(QUANTITY) QUANTITY
	                                FROM IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI
		                                ,IMSGROUP7879.IP_ITEM_SPEC_SETUP ISS
		                                ,IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS
	                                WHERE SI.ITEM_CODE = ISS.ITEM_CODE
		                                AND SI.COMPANY_CODE = ISS.COMPANY_CODE
		                                AND SI.COMPANY_CODE IN ('{0}')
		                                AND SI.ITEM_CODE = IMS.ITEM_CODE
		                                AND SI.COMPANY_CODE = IMS.COMPANY_CODE
	                                GROUP BY SI.ITEM_CODE
		                                ,ISS.BRAND_NAME
		                                ,IMS.ITEM_EDESC
		                                ,IMS.INDEX_MU_CODE

	                                ) A";
                Query = string.Format(Query, string.Join("','", model.CompanyFilter));
                //Query = @"SELECT TO_CHAR(NVL(TRIM(A.BRAND_NAME), '***')) BrandName
                //                 ,A.ITEM_CODE ItemCode
                //                 ,A.ITEM_EDESC ItemName
                //                 ,A.UNIT Unit
                //                 ,TO_NUMBER(TRIM(A.AVG_SALES_AMOUNT)) AverageSalesRate
                //                FROM (
                //                 SELECT ISS.BRAND_NAME
                //                  ,SI.ITEM_CODE
                //                  ,IMS.ITEM_EDESC
                //                  ,IMS.INDEX_MU_CODE UNIT
                //                  ,ROUND(SUM(NET_SALES) / SUM(QUANTITY), 2) AVG_SALES_AMOUNT
                //                 FROM V$SALES_INVOICE_REPORT3_MCL SI
                //                  ,IP_ITEM_SPEC_SETUP ISS
                //                  ,IP_ITEM_MASTER_SETUP IMS
                //                 WHERE SI.ITEM_CODE = ISS.ITEM_CODE
                //                  AND SI.COMPANY_CODE = ISS.COMPANY_CODE
                //                  AND SI.COMPANY_CODE IN ('{0}')
                //                  AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                  AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                //                 GROUP BY SI.ITEM_CODE
                //                  ,ISS.BRAND_NAME
                //                  ,IMS.ITEM_EDESC
                //                  ,IMS.INDEX_MU_CODE
                //                    UNION ALL
                //                    SELECT ISS.BRAND_NAME
                //                  ,SI.ITEM_CODE
                //                  ,IMS.ITEM_EDESC
                //                  ,IMS.INDEX_MU_CODE UNIT
                //                  ,ROUND(SUM(NET_SALES) / SUM(QUANTITY), 2) AVG_SALES_AMOUNT
                //                 FROM IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI
                //                  ,IMSGROUP7879.IP_ITEM_SPEC_SETUP ISS
                //                  ,IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS
                //                 WHERE SI.ITEM_CODE = ISS.ITEM_CODE
                //                  AND SI.COMPANY_CODE = ISS.COMPANY_CODE
                //                  AND SI.COMPANY_CODE IN ('{0}')
                //                  AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                  AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                //                 GROUP BY SI.ITEM_CODE
                //                  ,ISS.BRAND_NAME
                //                  ,IMS.ITEM_EDESC
                //                  ,IMS.INDEX_MU_CODE

                //                 ) A";
                //Query = string.Format(Query, string.Join("','", model.CompanyFilter));
            }
            else
            {
                //Old query that fetched data from IMS group.
                //Added filter and extracted brand name (Animesh)
                //Query = @"SELECT * FROM(SELECT TO_CHAR(NVL(TRIM(FN_FETCH_BRAND_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', A.PRE_ITEM_CODE)), '***')) BrandName
                //                                 ,A.ITEM_CODE ItemCode
                //                                 ,A.ITEM_EDESC ItemName
                //                                 ,A.UNIT Unit
                //                                    ,TO_NUMBER(A.AVG_SALES_AMOUNT*A.QUANTITY) AverageSalesAmount
                //                                    ,TO_NUMBER(TRIM(A.QUANTITY)) Quantity
                //                                    ,TO_NUMBER(TRIM(A.AVG_SALES_AMOUNT)) AverageSalesRate
                //                                FROM (
                //                                 SELECT ISS.BRAND_NAME
                //                                  ,SI.ITEM_CODE
                //                                        ,IMS.COMPANY_CODE
                //                                         ,IMS.PRE_ITEM_CODE
                //                                  ,IMS.ITEM_EDESC
                //                                  ,IMS.INDEX_MU_CODE UNIT
                //                                  ,CASE WHEN SUM(QUANTITY)<>0 THEN ROUND(SUM(NET_SALES) / SUM(QUANTITY), 2) ELSE 0 END AVG_SALES_AMOUNT
                //                                        ,SUM(QUANTITY) QUANTITY
                //                                 FROM IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI
                //                                  ,IMSGROUP7879.IP_ITEM_SPEC_SETUP ISS
                //                                  ,IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS
                //                                 WHERE SI.ITEM_CODE = ISS.ITEM_CODE
                //                                  AND SI.COMPANY_CODE = ISS.COMPANY_CODE
                //                                  AND SI.COMPANY_CODE = '{0}'
                //                                  AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                                  AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                //                                        AND SI.SALES_DATE Between {1} and {2}
                //                                 GROUP BY SI.ITEM_CODE
                //                                  ,ISS.BRAND_NAME
                //                                  ,IMS.ITEM_EDESC
                //                                  ,IMS.INDEX_MU_CODE
                //                                        ,IMS.COMPANY_CODE
                //                                        ,IMS.PRE_ITEM_CODE
                //                                        ,SI.SALES_DATE
                //                                 ) A
                //                                )B
                //                                ORDER BY B.BrandName ASC";


                //new one that fetch data of company and not from group(Animesh)
                Query = @"SELECT * FROM(SELECT TO_CHAR(NVL(TRIM(FN_FETCH_BRAND_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', A.PRE_ITEM_CODE)), '***')) BrandName
	                                          ,A.ITEM_CODE ItemCode
	                                          ,A.ITEM_EDESC ItemName
	                                          ,A.UNIT Unit
                                              ,TO_NUMBER(A.AVG_SALES_AMOUNT*A.QUANTITY) AverageSalesAmount
                                              ,TO_NUMBER(TRIM(A.QUANTITY)) Quantity
                                              ,TO_NUMBER(TRIM(A.AVG_SALES_AMOUNT)) AverageSalesRate
                                          FROM (
	                                          SELECT ISS.BRAND_NAME
		                                          ,SI.ITEM_CODE
                                                  ,IMS.COMPANY_CODE
                                                   ,IMS.PRE_ITEM_CODE
		                                          ,IMS.ITEM_EDESC
		                                          ,IMS.INDEX_MU_CODE UNIT
		                                          ,CASE WHEN SUM(QUANTITY)<>0 THEN ROUND(SUM(NET_SALES) / SUM(QUANTITY), 2) ELSE 0 END AVG_SALES_AMOUNT
                                                  ,SUM(QUANTITY) QUANTITY
	                                          FROM V$SALES_INVOICE_REPORT3_MCL SI
		                                          ,IP_ITEM_SPEC_SETUP ISS
		                                          ,IP_ITEM_MASTER_SETUP IMS
	                                          WHERE SI.ITEM_CODE = ISS.ITEM_CODE
		                                          AND SI.COMPANY_CODE = ISS.COMPANY_CODE
		                                          AND SI.COMPANY_CODE = '{0}'
		                                          AND SI.ITEM_CODE = IMS.ITEM_CODE
		                                          AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                                                  AND SI.SALES_DATE Between {1} and {2}
	                                          GROUP BY SI.ITEM_CODE
		                                          ,ISS.BRAND_NAME
		                                          ,IMS.ITEM_EDESC
		                                          ,IMS.INDEX_MU_CODE
                                                  ,IMS.COMPANY_CODE
                                                  ,IMS.PRE_ITEM_CODE
                                                  ,SI.SALES_DATE
	                                          ) A
                                          )B
                                          ORDER BY B.BrandName ASC";


                //Query = @"SELECT TO_CHAR(NVL(TRIM(A.BRAND_NAME), '***')) BrandName
                //                                 ,A.ITEM_CODE ItemCode
                //                                 ,A.ITEM_EDESC ItemName
                //                                 ,A.UNIT Unit
                //                                 ,TO_NUMBER(TRIM(A.AVG_SALES_AMOUNT)) AverageSalesAmount
                //                                FROM (
                //                                 SELECT ISS.BRAND_NAME
                //                                  ,SI.ITEM_CODE
                //                                  ,IMS.ITEM_EDESC
                //                                  ,IMS.INDEX_MU_CODE UNIT
                //                                  ,ROUND(SUM(NET_SALES) / SUM(QUANTITY), 2) AVG_SALES_AMOUNT
                //                                 FROM V$SALES_INVOICE_REPORT3_MCL SI
                //                                  ,IP_ITEM_SPEC_SETUP ISS
                //                                  ,IP_ITEM_MASTER_SETUP IMS
                //                                 WHERE SI.ITEM_CODE = ISS.ITEM_CODE
                //                                  AND SI.COMPANY_CODE = ISS.COMPANY_CODE
                //                                  AND SI.COMPANY_CODE = '{0}'
                //                                  AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                                  AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                //                                 GROUP BY SI.ITEM_CODE
                //                                  ,ISS.BRAND_NAME
                //                                  ,IMS.ITEM_EDESC
                //                                  ,IMS.INDEX_MU_CODE

                //                                 UNION ALL

                //                                 SELECT ISS.BRAND_NAME
                //                                  ,SI.ITEM_CODE
                //                                  ,IMS.ITEM_EDESC
                //                                  ,IMS.INDEX_MU_CODE UNIT
                //                                  ,ROUND(SUM(NET_SALES) / SUM(QUANTITY), 2) AVG_SALES_AMOUNT
                //                                 FROM IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI
                //                                  ,IMSGROUP7879.IP_ITEM_SPEC_SETUP ISS
                //                                  ,IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS
                //                                 WHERE SI.ITEM_CODE = ISS.ITEM_CODE
                //                                  AND SI.COMPANY_CODE = ISS.COMPANY_CODE
                //                                  AND SI.COMPANY_CODE = '{0}'
                //                                  AND SI.ITEM_CODE = IMS.ITEM_CODE
                //                                  AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                //                                 GROUP BY SI.ITEM_CODE
                //                                  ,ISS.BRAND_NAME
                //                                  ,IMS.ITEM_EDESC
                //                                  ,IMS.INDEX_MU_CODE
                //                                 ) A
                //                                ORDER BY BRAND_NAME ASC";
                Query = string.Format(Query, companyCode, fromDate, toData);
            }


            brandWiseSalesData = this._objectEntity.SqlQuery<BrandWiseSalesModel>(Query).ToList();

            return brandWiseSalesData;
        }

        public IList<PartyWiseSalesModel> GetPartyWiseSales(ReportFiltersModel model, User userInfo)
        {
            var partyWiseSalesData = new List<PartyWiseSalesModel>();
            var Query = string.Empty;
            var companyCode = userInfo.company_code;
            var fromDate = $"TO_DATE('{model.FromDate}', 'YYYY-MON-DD')";
            var toDate = $"TO_DATE('{model.ToDate}', 'YYYY-MON-DD')";
            if (model.CompanyFilter.Count > 0)
            {
                Query = @"SELECT B.CUSTOMER_GROUP CustomerGroup
	                            ,B.CUSTOMER_NAME CustomerName
	                            ,TO_NUMBER(B.TOTAL_SALES_AMT) TotalSalesAmount
	                            ,TO_NUMBER(B.GROUP_SALES_AMT) GroupSalesAmount
                            FROM (
	                            SELECT CUSTOMER_GROUP
		                            ,CUSTOMER_NAME
		                            ,TOTAL_SALES_AMT
		                            ,SUM(TOTAL_SALES_AMT) OVER (PARTITION BY CUSTOMER_GROUP) AS GROUP_SALES_AMT
	                            FROM (
		                            SELECT SA.CUSTOMER_CODE
			                            ,FN_FETCH_GROUP_DESC('{0}', 'SA_CUSTOMER_SETUP', SA.PRE_CUSTOMER_CODE) CUSTOMER_GROUP
			                            ,CUSTOMER_EDESC CUSTOMER_NAME
			                            ,SUM(TAXABLE_SALES) TOTAL_SALES_AMT
		                            FROM V$SALES_INVOICE_REPORT3 SI
			                            ,SA_CUSTOMER_SETUP SA
		                            WHERE SI.CUSTOMER_CODE = SA.CUSTOMER_CODE
			                            AND SI.COMPANY_CODE = SA.COMPANY_CODE
			                            AND SA.COMPANY_CODE = '{0}'
			                            AND SI.DELETED_FLAG = 'N'
                                        AND SI.SALES_DATE Between {1} and {2}
		                            GROUP BY SA.CUSTOMER_CODE
			                            ,CUSTOMER_EDESC
			                            ,FN_FETCH_GROUP_DESC('{0}', 'SA_CUSTOMER_SETUP', SA.PRE_CUSTOMER_CODE)
		                            ) A
	                            ) B";
                Query = string.Format(Query, string.Join("','", model.CompanyFilter), fromDate, toDate);
            }
            else
            {
                Query = @"SELECT B.CUSTOMER_GROUP CustomerGroup
	                            ,B.CUSTOMER_NAME CustomerName
	                            ,TO_NUMBER(B.TOTAL_SALES_AMT) TotalSalesAmount
	                            ,TO_NUMBER(B.GROUP_SALES_AMT) GroupSalesAmount
                            FROM (
	                            SELECT CUSTOMER_GROUP
		                            ,CUSTOMER_NAME
		                            ,TOTAL_SALES_AMT
		                            ,SUM(TOTAL_SALES_AMT) OVER (PARTITION BY CUSTOMER_GROUP) AS GROUP_SALES_AMT
	                            FROM (
		                            SELECT SA.CUSTOMER_CODE
			                            ,FN_FETCH_GROUP_DESC('{0}', 'SA_CUSTOMER_SETUP', SA.PRE_CUSTOMER_CODE) CUSTOMER_GROUP
			                            ,CUSTOMER_EDESC CUSTOMER_NAME
			                            ,SUM(TAXABLE_SALES) TOTAL_SALES_AMT
		                            FROM V$SALES_INVOICE_REPORT3 SI
			                            ,SA_CUSTOMER_SETUP SA
		                            WHERE SI.CUSTOMER_CODE = SA.CUSTOMER_CODE
			                            AND SI.COMPANY_CODE = SA.COMPANY_CODE
			                            AND SA.COMPANY_CODE = '{0}'
			                            AND SI.DELETED_FLAG = 'N'
                                        AND SI.SALES_DATE Between {1} and {2}
		                            GROUP BY SA.CUSTOMER_CODE
			                            ,CUSTOMER_EDESC
			                            ,FN_FETCH_GROUP_DESC('{0}', 'SA_CUSTOMER_SETUP', SA.PRE_CUSTOMER_CODE)
		                            ) A
	                            ) B";
                Query = string.Format(Query, companyCode, fromDate, toDate);
            }


            partyWiseSalesData = this._objectEntity.SqlQuery<PartyWiseSalesModel>(Query).ToList();

            return partyWiseSalesData;
        }

        public IList<DebtorAgingPerDayModel> GetDebtorAgingPerDay(ReportFiltersModel model, User userInfo)
        {
            var debtorAgingPerDayData = new List<DebtorAgingPerDayModel>();
            var Query = string.Empty;
            var companyCode = userInfo.company_code;
            if (model.CompanyFilter.Count > 0)
            {
                StringBuilder queryString = new StringBuilder("SELECT A.CUSTOMER_CODE  , A.CUSTOMER_NAME CustomerName , TO_NUMBER(NVL(TRIM(A.\"'1'\"), 0)) One , TO_NUMBER(NVL(TRIM(A.\"'2'\"), 0)) Two , TO_NUMBER(NVL(TRIM(A.\"'3'\"), 0)) Three  , TO_NUMBER(NVL(TRIM(A.\"'4'\"), 0)) Four , TO_NUMBER(NVL(TRIM(A.\"'5'\"), 0)) Five  , TO_NUMBER(NVL(TRIM(A.\"'6'\"), 0)) Six , TO_NUMBER(NVL(TRIM(A.\"'7'\"), 0)) Seven  , TO_NUMBER(NVL(TRIM(A.\"'8'\"), 0)) Eight  , TO_NUMBER(NVL(TRIM(A.\"'9'\"), 0)) Nine , TO_NUMBER(NVL(TRIM(A.\"'10'\"), 0)) Ten , TO_NUMBER(NVL(TRIM(A.\"'11'\"), 0)) Eleven  , TO_NUMBER(NVL(TRIM(A.\"'12'\"), 0)) Twelve  , TO_NUMBER(NVL(TRIM(A.\"'13'\"), 0)) Thirteen , TO_NUMBER(NVL(TRIM(A.\"'14'\"), 0)) Fourteen , TO_NUMBER(NVL(TRIM(A.\"'15'\"), 0)) Fifteen , TO_NUMBER(NVL(TRIM(A.\"'16'\"), 0)) Sixteen  , TO_NUMBER(NVL(TRIM(A.\"'17'\"), 0)) Seventeen  ,TO_NUMBER(NVL(TRIM(A.\"'18'\"), 0)) Eighteen, TO_NUMBER(NVL(TRIM(A.\"'19'\"), 0)) Nineteen  , TO_NUMBER(NVL(TRIM(A.\"'20+'\"), 0)) TwentyPlus FROM( SELECT CUSTOMER_CODE  , ( SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP  WHERE MASTER_CUSTOMER_CODE IN( SELECT PRE_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP  WHERE CUSTOMER_CODE = AA.CUSTOMER_CODE AND COMPANY_CODE = '{1}' )AND COMPANY_CODE = '{1}' ) CUSTOMER_GROUP  , (  SELECT CUSTOMER_EDESC  FROM SA_CUSTOMER_SETUP  WHERE CUSTOMER_CODE = AA.CUSTOMER_CODE  AND COMPANY_CODE = '{1}'  ) CUSTOMER_NAME  , ( CASE  WHEN DUE_DAYS = 1 THEN '1'  WHEN DUE_DAYS = 2 THEN '2'  WHEN DUE_DAYS = 3 THEN '3'  WHEN DUE_DAYS = 4 THEN '4' WHEN DUE_DAYS = 5 THEN '5' WHEN DUE_DAYS = 6 THEN '6' WHEN DUE_DAYS = 7  THEN '7'  WHEN DUE_DAYS = 8  THEN '8'   WHEN DUE_DAYS = 9 THEN '9' WHEN DUE_DAYS = 10 THEN '10' WHEN DUE_DAYS = 11  THEN '11'  WHEN DUE_DAYS = 12  THEN '12'  WHEN DUE_DAYS = 13    THEN '13' WHEN DUE_DAYS = 14 THEN '14' WHEN DUE_DAYS =  15 THEN '15' WHEN DUE_DAYS = 16 THEN '16' WHEN DUE_DAYS = 17  THEN '17'  WHEN DUE_DAYS = 18 THEN '18' WHEN DUE_DAYS = 19   THEN '19' WHEN DUE_DAYS >= 20  THEN '20+' END  ) AGES , BALANCE_AMT BALANCE FROM( SELECT CUSTOMER_CODE , VOUCHER_DATE , (TO_DATE('{0}', 'YYYY-MON-DD') - VOUCHER_DATE) DUE_DAYS , SUM(CUSTOMER_BALANCE) BALANCE_AMT FROM V$QUICK_CUSTOMER_BALANCE WHERE COMPANY_CODE IN('{1}')  GROUP BY CUSTOMER_CODE , VOUCHER_DATE  ) AA ) PIVOT(SUM(BALANCE) FOR AGES IN( '1' ,'2' ,'3' ,'4' ,'5' ,'6' ,'7' ,'8' ,'9' ,'10' ,'11' ,'12' ,'13' ,'14' ,'15' ,'16' ,'17' ,'18' ,'19' ,'20+' )) A");
                Query = string.Format(queryString.ToString(), model.FromDate, string.Join("','", model.CompanyFilter[0]));
            }
            else
            {
                StringBuilder queryString = new StringBuilder("SELECT A.CUSTOMER_CODE  , A.CUSTOMER_NAME CustomerName , TO_NUMBER(NVL(TRIM(A.\"'1'\"), 0)) One , TO_NUMBER(NVL(TRIM(A.\"'2'\"), 0)) Two , TO_NUMBER(NVL(TRIM(A.\"'3'\"), 0)) Three  , TO_NUMBER(NVL(TRIM(A.\"'4'\"), 0)) Four , TO_NUMBER(NVL(TRIM(A.\"'5'\"), 0)) Five  , TO_NUMBER(NVL(TRIM(A.\"'6'\"), 0)) Six , TO_NUMBER(NVL(TRIM(A.\"'7'\"), 0)) Seven  , TO_NUMBER(NVL(TRIM(A.\"'8'\"), 0)) Eight  , TO_NUMBER(NVL(TRIM(A.\"'9'\"), 0)) Nine , TO_NUMBER(NVL(TRIM(A.\"'10'\"), 0)) Ten , TO_NUMBER(NVL(TRIM(A.\"'11'\"), 0)) Eleven  , TO_NUMBER(NVL(TRIM(A.\"'12'\"), 0)) Twelve  , TO_NUMBER(NVL(TRIM(A.\"'13'\"), 0)) Thirteen , TO_NUMBER(NVL(TRIM(A.\"'14'\"), 0)) Fourteen , TO_NUMBER(NVL(TRIM(A.\"'15'\"), 0)) Fifteen , TO_NUMBER(NVL(TRIM(A.\"'16'\"), 0)) Sixteen  , TO_NUMBER(NVL(TRIM(A.\"'17'\"), 0)) Seventeen  ,TO_NUMBER(NVL(TRIM(A.\"'18'\"), 0)) Eighteen, TO_NUMBER(NVL(TRIM(A.\"'19'\"), 0)) Nineteen  , TO_NUMBER(NVL(TRIM(A.\"'20+'\"), 0)) TwentyPlus FROM( SELECT CUSTOMER_CODE  , ( SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP  WHERE MASTER_CUSTOMER_CODE IN( SELECT PRE_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP  WHERE CUSTOMER_CODE = AA.CUSTOMER_CODE AND COMPANY_CODE = '{1}' )AND COMPANY_CODE = '{1}' ) CUSTOMER_GROUP  , (  SELECT CUSTOMER_EDESC  FROM SA_CUSTOMER_SETUP  WHERE CUSTOMER_CODE = AA.CUSTOMER_CODE  AND COMPANY_CODE = '{1}'  ) CUSTOMER_NAME  , ( CASE  WHEN DUE_DAYS = 1 THEN '1'  WHEN DUE_DAYS = 2 THEN '2'  WHEN DUE_DAYS = 3 THEN '3'  WHEN DUE_DAYS = 4 THEN '4' WHEN DUE_DAYS = 5 THEN '5' WHEN DUE_DAYS = 6 THEN '6' WHEN DUE_DAYS = 7  THEN '7'  WHEN DUE_DAYS = 8  THEN '8'   WHEN DUE_DAYS = 9 THEN '9' WHEN DUE_DAYS = 10 THEN '10' WHEN DUE_DAYS = 11  THEN '11'  WHEN DUE_DAYS = 12  THEN '12'  WHEN DUE_DAYS = 13    THEN '13' WHEN DUE_DAYS = 14 THEN '14' WHEN DUE_DAYS =  15 THEN '15' WHEN DUE_DAYS = 16 THEN '16' WHEN DUE_DAYS = 17  THEN '17'  WHEN DUE_DAYS = 18 THEN '18' WHEN DUE_DAYS = 19   THEN '19' WHEN DUE_DAYS >= 20  THEN '20+' END  ) AGES , BALANCE_AMT BALANCE FROM( SELECT CUSTOMER_CODE , VOUCHER_DATE , (TO_DATE('{0}', 'YYYY-MON-DD') - VOUCHER_DATE) DUE_DAYS , SUM(CUSTOMER_BALANCE) BALANCE_AMT FROM V$QUICK_CUSTOMER_BALANCE WHERE COMPANY_CODE IN('{1}')  GROUP BY CUSTOMER_CODE , VOUCHER_DATE  ) AA ) PIVOT(SUM(BALANCE) FOR AGES IN( '1' ,'2' ,'3' ,'4' ,'5' ,'6' ,'7' ,'8' ,'9' ,'10' ,'11' ,'12' ,'13' ,'14' ,'15' ,'16' ,'17' ,'18' ,'19' ,'20+' )) A");
                Query = string.Format(queryString.ToString(), model.FromDate, companyCode);
            }


            debtorAgingPerDayData = this._objectEntity.SqlQuery<DebtorAgingPerDayModel>(Query).ToList();

            return debtorAgingPerDayData;
        }

        public IList<DebtorAgingModel> GetDebtorAging(ReportFiltersModel model, User userInfo)
        {
            var debtorAgingData = new List<DebtorAgingModel>();
            var Query = string.Empty;
            var companyCode = userInfo.company_code;
            if (model.CompanyFilter.Count > 0)
            {
                Query = $@"";
                Query = string.Format(Query, model.FromDate, model.ToDate, string.Join("','", model.PartyTypeFilter));
                StringBuilder queryString = new StringBuilder("SELECT DISTINCT A.CUSTOMER_CODE CustomerCode ,A.CUSTOMER_EDESC CustomerName ,TO_NUMBER(NVL(TRIM(A.\"'0-15'\"),0)) ZeroToFifteen ,TO_NUMBER(NVL(TRIM(A.\"'16-30'\"),0)) SisteenToThirty , TO_NUMBER(NVL(TRIM(A.\"'31-45'\"),0)) ThirtyOneToFourtyFive , TO_NUMBER(NVL(TRIM(A.\"'46-60'\"),0)) FourtySixToSixty , TO_NUMBER(NVL(TRIM(A.\"'61-75'\"),0)) SixtyOneToSeventyFive  , TO_NUMBER(NVL(TRIM(A.\"'76-90'\"),0)) SeventySixToNinty , TO_NUMBER(NVL(TRIM(A.\"'90+'\"),0)) NintyPlus FROM(  SELECT CUSTOMER_CODE , CUSTOMER_EDESC , ( CASE WHEN AGEGROUP = 1 THEN '0-15'  WHEN AGEGROUP = 2 THEN '16-30' WHEN AGEGROUP = 3 THEN '31-45'  WHEN AGEGROUP = 4 THEN '46-60' WHEN AGEGROUP = 5 THEN '61-75' WHEN AGEGROUP = 6 THEN '76-90' WHEN AGEGROUP > 7 THEN '90+' END ) AGES , SUM(BALANCE_AMT) BALANCE FROM( SELECT CUSTOMER_CODE  , CUSTOMER_EDESC , TRUNC(SYSDATE) - VOUCHER_DATE DUE_DATE , BALANCE_AMT  , WIDTH_BUCKET(TRUNC(SYSDATE) - VOUCHER_DATE, 0, 3675, 245) AGEGROUP  FROM( SELECT VOUCHER_DATE , SUM(CUSTOMER_BALANCE) BALANCE_AMT , CUSTOMER_CODE  , CUSTOMER_EDESC  FROM V$QUICK_CUSTOMER_BALANCE   WHERE COMPANY_CODE IN('{0}') GROUP BY CUSTOMER_CODE , CUSTOMER_EDESC , VOUCHER_DATE  ) A  )  GROUP BY CUSTOMER_CODE , CUSTOMER_EDESC , DUE_DATE  , AGEGROUP  ) PIVOT(SUM(BALANCE) FOR AGES IN( '0-15' , '16-30' , '31-45' , '46-60' , '61-75'  , '76-90' , '90+'  )) A");
                Query = string.Format(queryString.ToString(), string.Join("','", model.CompanyFilter));
            }
            else
            {
                StringBuilder queryString = new StringBuilder("SELECT DISTINCT A.CUSTOMER_CODE CustomerCode ,A.CUSTOMER_EDESC CustomerName ,TO_NUMBER(NVL(TRIM(A.\"'0-15'\"),0)) ZeroToFifteen ,TO_NUMBER(NVL(TRIM(A.\"'16-30'\"),0)) SisteenToThirty , TO_NUMBER(NVL(TRIM(A.\"'31-45'\"),0)) ThirtyOneToFourtyFive , TO_NUMBER(NVL(TRIM(A.\"'46-60'\"),0)) FourtySixToSixty , TO_NUMBER(NVL(TRIM(A.\"'61-75'\"),0)) SixtyOneToSeventyFive  , TO_NUMBER(NVL(TRIM(A.\"'76-90'\"),0)) SeventySixToNinty , TO_NUMBER(NVL(TRIM(A.\"'90+'\"),0)) NintyPlus FROM(  SELECT CUSTOMER_CODE , CUSTOMER_EDESC , ( CASE WHEN AGEGROUP = 1 THEN '0-15'  WHEN AGEGROUP = 2 THEN '16-30' WHEN AGEGROUP = 3 THEN '31-45'  WHEN AGEGROUP = 4 THEN '46-60' WHEN AGEGROUP = 5 THEN '61-75' WHEN AGEGROUP = 6 THEN '76-90' WHEN AGEGROUP > 7 THEN '90+' END ) AGES , SUM(BALANCE_AMT) BALANCE FROM( SELECT CUSTOMER_CODE  , CUSTOMER_EDESC , TRUNC(SYSDATE) - VOUCHER_DATE DUE_DATE , BALANCE_AMT  , WIDTH_BUCKET(TRUNC(SYSDATE) - VOUCHER_DATE, 0, 3675, 245) AGEGROUP  FROM( SELECT VOUCHER_DATE , SUM(CUSTOMER_BALANCE) BALANCE_AMT , CUSTOMER_CODE  , CUSTOMER_EDESC  FROM V$QUICK_CUSTOMER_BALANCE   WHERE COMPANY_CODE IN('{0}') GROUP BY CUSTOMER_CODE , CUSTOMER_EDESC , VOUCHER_DATE  ) A  )  GROUP BY CUSTOMER_CODE , CUSTOMER_EDESC , DUE_DATE  , AGEGROUP  ) PIVOT(SUM(BALANCE) FOR AGES IN( '0-15' , '16-30' , '31-45' , '46-60' , '61-75'  , '76-90' , '90+'  )) A");
                Query = string.Format(queryString.ToString(), companyCode);
            }


            debtorAgingData = this._objectEntity.SqlQuery<DebtorAgingModel>(Query).ToList();

            return debtorAgingData;
        }
        public IList<WeekSalesModel> GetWeekOfSales(ReportFiltersModel model, User userInfo)
        {
            var weekSalesData = new List<WeekSalesModel>();
            var Query = string.Empty;
            var abc = DateTime.Now.ToString("yyyy-MMM-dd");
            //if (model.FromDate ==  DateTime.Now.ToString("yyyy-MMM-dd"))
            //{
            //    model.FromDate="2020-JUL-16";
            //}
            var companyCode = userInfo.company_code;
            if (model.CompanyFilter.Count > 0)
            {

                StringBuilder queryString = new StringBuilder("SELECT AA.*,  B.TOTAL_SALES_QTY TotalSalesQuantity, B.AVG_WEEKLY_SALES AverageWeeklySales,  C.LAST_FOUR_MONTH_SALES LastFourMonthSales, C.AVG__LAST_FOUR_MONTH_SALES AverageLastFourMonthSales,  D.TOTAL_PURCHASE_QTY TotalPurchareQuantity ,(D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY) ClosingStockQuantity, ROUND(CASE WHEN TO_NUMBER(B.AVG_WEEKLY_SALES)>0 THEN TO_NUMBER((D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY)/ TO_NUMBER(B.AVG_WEEKLY_SALES)) ELSE 0 END,2) AS  WOS1,ROUND( CASE WHEN TO_NUMBER(C.AVG__LAST_FOUR_MONTH_SALES)>0 THEN TO_NUMBER((D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY)/ TO_NUMBER(C.AVG__LAST_FOUR_MONTH_SALES)) ELSE 0 END ,2)AS  WOS2  FROM(SELECT MODEL_NAME MODEL , CATEGORY_EDESC CATEGORY , \"'01'\" ONE  , \"'02'\" TWO  , \"'03'\" THREE , \"'04'\" FOUR  , \"'05'\" FIVE  , \"'06'\" SIX , \"'07'\" SEVEN , \"'08'\" EIGHT  , \"'09'\" NINE  , \"'10'\" TEN  , \"'11'\" ELEVEN , \"'12'\" TWELVE , \"'13'\" THIRTEEN , \"'14'\" FOURTEEN , \"'15'\" FIFTEEN , \"'16'\" SIXTEEN  , \"'17'\" SEVENTEEN  , \"'18'\" EIGHTEEN  , \"'19'\" NINETEEN  , \"'20'\" TWENTY , \"'21'\" TWENTYONE , \"'22'\" TWENTYTWO , \"'23'\" TWENTYTHREE  , \"'24'\" TWENTYFOUR  , \"'25'\" TWENTYFIVE  , \"'26'\" TWENTYSIX , \"'27'\" TWENTYSEVEN  , \"'28'\" TWENTYEIGHT  , \"'29'\" TWENTYNINE , \"'30'\" THIRTY , \"'31'\" THIRTYONE  , \"'32'\" THIRTYTWO , \"'33'\" THIRTYTHREE  , \"'34'\" THIRTYFOUR , \"'35'\" THIRTYFIVE , \"'36'\" THIRTYSIX  , \"'37'\" THIRTYSEVE , \"'38'\" THIRTYEIGH  , \"'39'\" THIRTYNINE , \"'40'\" FOURTY   , \"'41'\" FOURTYONE , \"'42'\" FOURTYTWO , \"'43'\" FOURTYTHREE  , \"'44'\" FOURTYFOUR  , \"'45'\" FOURTYFIVE   , \"'46'\" FOURTYSIX  , \"'47'\" FOURTYSEVEN , \"'48'\" FOURTYEIGHT , \"'49'\" FOURTYNINE , \"'50'\" FIFTY  , \"'51'\" FIFTYONE , \"'52'\" FIFTYTWO  , \"'53'\" FIFTYTHREE FROM( SELECT DISTINCT MODEL_NAME , CATEGORY_EDESC , WEEK , SUM(QUANTITY) OVER( PARTITION BY WEEK  , MODEL_NAME   ) AS WEEK_SALES_QTY  , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_SALES_QTY , ROUND(AVG(QUANTITY) OVER(PARTITION BY MODEL_NAME), 2) AS AVG_WEEKLY_SALES FROM( SELECT MODEL_NAME , CC.CATEGORY_EDESC  , to_char(to_date(SALES_DATE), 'WW') WEEK   , QUANTITY  FROM( SELECT IIS.ITEM_CODE , IIS.CATEGORY_CODE , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME  , IMS.COMPANY_CODE FROM IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS  , IMSGROUP7879.IP_ITEM_MASTER_SETUP IIS WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G' AND IIS.GROUP_SKU_FLAG = 'I' AND IIS.COMPANY_CODE = '{2}' ) IMS , IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI  , IMSGROUP7879.IP_CATEGORY_CODE CC WHERE IMS.ITEM_CODE = SI.ITEM_CODE AND IMS.COMPANY_CODE = SI.COMPANY_CODE   AND SI.COMPANY_CODE = '{2}' AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+) AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+) ) ) A PIVOT(MAX(WEEK_SALES_QTY) FOR WEEK IN( '01'  , '02'  , '03'  , '04'  , '05' , '06' , '07' , '08' , '09'  , '10'  , '11'  , '12'  , '13'  , '14'  , '15' , '16' , '17' , '18' , '19' , '20' , '21'  , '22'  , '23' , '24' , '25'  , '26'  , '27'  , '28'  , '29'  , '30' , '31'  , '32' , '33' , '34' , '35' , '36' , '37' , '38' , '39' , '40' , '41' , '42'  , '43' , '44' , '45' , '46' , '47'   , '48' , '49' , '50' , '51'  , '52' , '53' )))AA,  (SELECT * FROM(SELECT DISTINCT MODEL_NAME MODEL   , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_SALES_QTY  , ROUND(AVG(QUANTITY) OVER(PARTITION BY MODEL_NAME), 2) AS AVG_WEEKLY_SALES FROM( SELECT MODEL_NAME , CC.CATEGORY_EDESC  , to_char(to_date(SALES_DATE), 'WW-YYYY') WEEK , QUANTITY FROM( SELECT IIS.ITEM_CODE , IIS.CATEGORY_CODE   , IMS.ITEM_EDESC MODEL_NAME  , IIS.ITEM_EDESC ITEM_NAME  , IMS.COMPANY_CODE FROM IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS   , IMSGROUP7879.IP_ITEM_MASTER_SETUP IIS   WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G'  AND IIS.GROUP_SKU_FLAG = 'I'  AND IIS.COMPANY_CODE = '01'   ) IMS  , IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI  , IMSGROUP7879.IP_CATEGORY_CODE CC  WHERE IMS.ITEM_CODE = SI.ITEM_CODE  AND IMS.COMPANY_CODE = SI.COMPANY_CODE AND SI.COMPANY_CODE = '01' AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+)  AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+)  )))B,  (SELECT DISTINCT MODEL_NAME MODEL , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS LAST_FOUR_MONTH_SALES , (SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME)) / 4 AS AVG__LAST_FOUR_MONTH_SALES FROM(  SELECT MODEL_NAME , CC.CATEGORY_EDESC , to_char(to_date(SALES_DATE), 'WW-YYYY') WEEK , QUANTITY  FROM(  SELECT IIS.ITEM_CODE  , IIS.CATEGORY_CODE  , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME   , IMS.COMPANY_CODE  FROM IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS  , IMSGROUP7879.IP_ITEM_MASTER_SETUP IIS WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N'  AND IMS.GROUP_SKU_FLAG = 'G'  AND IIS.GROUP_SKU_FLAG = 'I' AND IIS.COMPANY_CODE = '{2}' ) IMS  , IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI   , IMSGROUP7879.IP_CATEGORY_CODE CC  WHERE IMS.ITEM_CODE = SI.ITEM_CODE AND IMS.COMPANY_CODE = SI.COMPANY_CODE  AND SI.COMPANY_CODE = '{2}'  AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+)  AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+) AND SI.SALES_DATE BETWEEN TO_DATE('{0}', 'YYYY-MON-DD') and TO_DATE('{1}', 'YYYY-MON-DD') ))C, (SELECT DISTINCT MODEL_NAME MODEL ,SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_PURCHASE_QTY FROM(  SELECT IIS.ITEM_CODE  , IIS.CATEGORY_CODE  , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME , IMS.COMPANY_CODE  FROM IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS   , IMSGROUP7879.IP_ITEM_MASTER_SETUP IIS  WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)   AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G' AND IIS.GROUP_SKU_FLAG = 'I'  AND IIS.COMPANY_CODE = '{2}'  ) IMS ,IMSGROUP7879.V$IP_PURCHASE_INVOICE PI WHERE IMS.ITEM_CODE = PI.ITEM_CODE AND IMS.COMPANY_CODE = PI.COMPANY_CODE ORDER BY MODEL_NAME)D WHERE AA.MODEL = B.MODEL AND AA.MODEL = C.MODEL   AND AA.MODEL = D.MODEL(+)");
                Query = string.Format(queryString.ToString(), model.FromDate, model.ToDate, string.Join("','", model.CompanyFilter[0]));
            }
            else
            {
                //StringBuilder queryString = new StringBuilder("SELECT AA.*,  B.TOTAL_SALES_QTY TotalSalesQuantity, B.AVG_WEEKLY_SALES AverageWeeklySales,  C.LAST_FOUR_MONTH_SALES LastFourMonthSales, C.AVG__LAST_FOUR_MONTH_SALES AverageLastFourMonthSales,  D.TOTAL_PURCHASE_QTY TotalPurchareQuantity ,(D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY) ClosingStockQuantity, ROUND(CASE WHEN TO_NUMBER(B.AVG_WEEKLY_SALES)>0 THEN TO_NUMBER((D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY)/ TO_NUMBER(B.AVG_WEEKLY_SALES)) ELSE 0 END,2) AS  WOS1,ROUND( CASE WHEN TO_NUMBER(C.AVG__LAST_FOUR_MONTH_SALES)>0 THEN TO_NUMBER((D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY)/ TO_NUMBER(C.AVG__LAST_FOUR_MONTH_SALES)) ELSE 0 END ,2)AS  WOS2  FROM(SELECT MODEL_NAME MODEL , CATEGORY_EDESC CATEGORY , \"'01'\" ONE  , \"'02'\" TWO  , \"'03'\" THREE , \"'04'\" FOUR  , \"'05'\" FIVE  , \"'06'\" SIX , \"'07'\" SEVEN , \"'08'\" EIGHT  , \"'09'\" NINE  , \"'10'\" TEN  , \"'11'\" ELEVEN , \"'12'\" TWELVE , \"'13'\" THIRTEEN , \"'14'\" FOURTEEN , \"'15'\" FIFTEEN , \"'16'\" SIXTEEN  , \"'17'\" SEVENTEEN  , \"'18'\" EIGHTEEN  , \"'19'\" NINETEEN  , \"'20'\" TWENTY , \"'21'\" TWENTYONE , \"'22'\" TWENTYTWO , \"'23'\" TWENTYTHREE  , \"'24'\" TWENTYFOUR  , \"'25'\" TWENTYFIVE  , \"'26'\" TWENTYSIX , \"'27'\" TWENTYSEVEN  , \"'28'\" TWENTYEIGHT  , \"'29'\" TWENTYNINE , \"'30'\" THIRTY , \"'31'\" THIRTYONE  , \"'32'\" THIRTYTWO , \"'33'\" THIRTYTHREE  , \"'34'\" THIRTYFOUR , \"'35'\" THIRTYFIVE , \"'36'\" THIRTYSIX  , \"'37'\" THIRTYSEVE , \"'38'\" THIRTYEIGH  , \"'39'\" THIRTYNINE , \"'40'\" FOURTY   , \"'41'\" FOURTYONE , \"'42'\" FOURTYTWO , \"'43'\" FOURTYTHREE  , \"'44'\" FOURTYFOUR  , \"'45'\" FOURTYFIVE   , \"'46'\" FOURTYSIX  , \"'47'\" FOURTYSEVEN , \"'48'\" FOURTYEIGHT , \"'49'\" FOURTYNINE , \"'50'\" FIFTY  , \"'51'\" FIFTYONE , \"'52'\" FIFTYTWO  , \"'53'\" FIFTYTHREE FROM( SELECT DISTINCT MODEL_NAME , CATEGORY_EDESC , WEEK , SUM(QUANTITY) OVER( PARTITION BY WEEK  , MODEL_NAME   ) AS WEEK_SALES_QTY  , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_SALES_QTY , ROUND(AVG(QUANTITY) OVER(PARTITION BY MODEL_NAME), 2) AS AVG_WEEKLY_SALES FROM( SELECT MODEL_NAME , CC.CATEGORY_EDESC  , to_char(to_date(SALES_DATE), 'WW') WEEK   , QUANTITY  FROM( SELECT IIS.ITEM_CODE , IIS.CATEGORY_CODE , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME  , IMS.COMPANY_CODE FROM IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS  , IMSGROUP7879.IP_ITEM_MASTER_SETUP IIS WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G' AND IIS.GROUP_SKU_FLAG = 'I' AND IIS.COMPANY_CODE = '{2}' ) IMS , IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI  , IMSGROUP7879.IP_CATEGORY_CODE CC WHERE IMS.ITEM_CODE = SI.ITEM_CODE AND IMS.COMPANY_CODE = SI.COMPANY_CODE   AND SI.COMPANY_CODE = '{2}' AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+) AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+) ) ) A PIVOT(MAX(WEEK_SALES_QTY) FOR WEEK IN( '01'  , '02'  , '03'  , '04'  , '05' , '06' , '07' , '08' , '09'  , '10'  , '11'  , '12'  , '13'  , '14'  , '15' , '16' , '17' , '18' , '19' , '20' , '21'  , '22'  , '23' , '24' , '25'  , '26'  , '27'  , '28'  , '29'  , '30' , '31'  , '32' , '33' , '34' , '35' , '36' , '37' , '38' , '39' , '40' , '41' , '42'  , '43' , '44' , '45' , '46' , '47'   , '48' , '49' , '50' , '51'  , '52' , '53' )))AA,  (SELECT * FROM(SELECT DISTINCT MODEL_NAME MODEL   , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_SALES_QTY  , ROUND(AVG(QUANTITY) OVER(PARTITION BY MODEL_NAME), 2) AS AVG_WEEKLY_SALES FROM( SELECT MODEL_NAME , CC.CATEGORY_EDESC  , to_char(to_date(SALES_DATE), 'WW-YYYY') WEEK , QUANTITY FROM( SELECT IIS.ITEM_CODE , IIS.CATEGORY_CODE   , IMS.ITEM_EDESC MODEL_NAME  , IIS.ITEM_EDESC ITEM_NAME  , IMS.COMPANY_CODE FROM IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS   , IMSGROUP7879.IP_ITEM_MASTER_SETUP IIS   WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G'  AND IIS.GROUP_SKU_FLAG = 'I'  AND IIS.COMPANY_CODE = '01'   ) IMS  , IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI  , IMSGROUP7879.IP_CATEGORY_CODE CC  WHERE IMS.ITEM_CODE = SI.ITEM_CODE  AND IMS.COMPANY_CODE = SI.COMPANY_CODE AND SI.COMPANY_CODE = '01' AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+)  AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+)  )))B,  (SELECT DISTINCT MODEL_NAME MODEL , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS LAST_FOUR_MONTH_SALES , (SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME)) / 4 AS AVG__LAST_FOUR_MONTH_SALES FROM(  SELECT MODEL_NAME , CC.CATEGORY_EDESC , to_char(to_date(SALES_DATE), 'WW-YYYY') WEEK , QUANTITY  FROM(  SELECT IIS.ITEM_CODE  , IIS.CATEGORY_CODE  , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME   , IMS.COMPANY_CODE  FROM IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS  , IMSGROUP7879.IP_ITEM_MASTER_SETUP IIS WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N'  AND IMS.GROUP_SKU_FLAG = 'G'  AND IIS.GROUP_SKU_FLAG = 'I' AND IIS.COMPANY_CODE = '{2}' ) IMS  , IMSGROUP7879.V$SALES_INVOICE_REPORT3_MCL SI   , IMSGROUP7879.IP_CATEGORY_CODE CC  WHERE IMS.ITEM_CODE = SI.ITEM_CODE AND IMS.COMPANY_CODE = SI.COMPANY_CODE  AND SI.COMPANY_CODE = '{2}'  AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+)  AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+) AND SI.SALES_DATE BETWEEN TO_DATE('{0}', 'YYYY-MON-DD') and TO_DATE('{1}', 'YYYY-MON-DD') ))C, (SELECT DISTINCT MODEL_NAME MODEL ,SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_PURCHASE_QTY FROM(  SELECT IIS.ITEM_CODE  , IIS.CATEGORY_CODE  , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME , IMS.COMPANY_CODE  FROM IMSGROUP7879.IP_ITEM_MASTER_SETUP IMS   , IMSGROUP7879.IP_ITEM_MASTER_SETUP IIS  WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)   AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G' AND IIS.GROUP_SKU_FLAG = 'I'  AND IIS.COMPANY_CODE = '{2}'  ) IMS ,IMSGROUP7879.V$IP_PURCHASE_INVOICE PI WHERE IMS.ITEM_CODE = PI.ITEM_CODE AND IMS.COMPANY_CODE = PI.COMPANY_CODE ORDER BY MODEL_NAME)D WHERE AA.MODEL = B.MODEL AND AA.MODEL = C.MODEL   AND AA.MODEL = D.MODEL(+)");
                StringBuilder queryString = new StringBuilder("SELECT AA.*,  B.TOTAL_SALES_QTY TotalSalesQuantity, B.AVG_WEEKLY_SALES AverageWeeklySales,  C.LAST_FOUR_MONTH_SALES LastFourMonthSales, C.AVG__LAST_FOUR_MONTH_SALES AverageLastFourMonthSales,  D.TOTAL_PURCHASE_QTY TotalPurchareQuantity ,(D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY) ClosingStockQuantity, ROUND(CASE WHEN TO_NUMBER(B.AVG_WEEKLY_SALES)>0 THEN TO_NUMBER((D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY)/ TO_NUMBER(B.AVG_WEEKLY_SALES)) ELSE 0 END,2) AS  WOS1,ROUND( CASE WHEN TO_NUMBER(C.AVG__LAST_FOUR_MONTH_SALES)>0 THEN TO_NUMBER((D.TOTAL_PURCHASE_QTY-B.TOTAL_SALES_QTY)/ TO_NUMBER(C.AVG__LAST_FOUR_MONTH_SALES)) ELSE 0 END ,2)AS  WOS2  FROM(SELECT MODEL_NAME MODEL , CATEGORY_EDESC CATEGORY , \"'01'\" ONE  , \"'02'\" TWO  , \"'03'\" THREE , \"'04'\" FOUR  , \"'05'\" FIVE  , \"'06'\" SIX , \"'07'\" SEVEN , \"'08'\" EIGHT  , \"'09'\" NINE  , \"'10'\" TEN  , \"'11'\" ELEVEN , \"'12'\" TWELVE , \"'13'\" THIRTEEN , \"'14'\" FOURTEEN , \"'15'\" FIFTEEN , \"'16'\" SIXTEEN  , \"'17'\" SEVENTEEN  , \"'18'\" EIGHTEEN  , \"'19'\" NINETEEN  , \"'20'\" TWENTY , \"'21'\" TWENTYONE , \"'22'\" TWENTYTWO , \"'23'\" TWENTYTHREE  , \"'24'\" TWENTYFOUR  , \"'25'\" TWENTYFIVE  , \"'26'\" TWENTYSIX , \"'27'\" TWENTYSEVEN  , \"'28'\" TWENTYEIGHT  , \"'29'\" TWENTYNINE , \"'30'\" THIRTY , \"'31'\" THIRTYONE  , \"'32'\" THIRTYTWO , \"'33'\" THIRTYTHREE  , \"'34'\" THIRTYFOUR , \"'35'\" THIRTYFIVE , \"'36'\" THIRTYSIX  , \"'37'\" THIRTYSEVE , \"'38'\" THIRTYEIGH  , \"'39'\" THIRTYNINE , \"'40'\" FOURTY   , \"'41'\" FOURTYONE , \"'42'\" FOURTYTWO , \"'43'\" FOURTYTHREE  , \"'44'\" FOURTYFOUR  , \"'45'\" FOURTYFIVE   , \"'46'\" FOURTYSIX  , \"'47'\" FOURTYSEVEN , \"'48'\" FOURTYEIGHT , \"'49'\" FOURTYNINE , \"'50'\" FIFTY  , \"'51'\" FIFTYONE , \"'52'\" FIFTYTWO  , \"'53'\" FIFTYTHREE FROM( SELECT DISTINCT MODEL_NAME , CATEGORY_EDESC , WEEK , SUM(QUANTITY) OVER( PARTITION BY WEEK  , MODEL_NAME   ) AS WEEK_SALES_QTY  , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_SALES_QTY , ROUND(AVG(QUANTITY) OVER(PARTITION BY MODEL_NAME), 2) AS AVG_WEEKLY_SALES FROM( SELECT MODEL_NAME , CC.CATEGORY_EDESC  , to_char(to_date(SALES_DATE), 'WW') WEEK   , QUANTITY  FROM( SELECT IIS.ITEM_CODE , IIS.CATEGORY_CODE , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME  , IMS.COMPANY_CODE FROM IP_ITEM_MASTER_SETUP IMS  , IP_ITEM_MASTER_SETUP IIS WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G' AND IIS.GROUP_SKU_FLAG = 'I' AND IIS.COMPANY_CODE = '{2}' ) IMS , V$SALES_INVOICE_REPORT3_MCL SI  , IP_CATEGORY_CODE CC WHERE IMS.ITEM_CODE = SI.ITEM_CODE AND IMS.COMPANY_CODE = SI.COMPANY_CODE   AND SI.COMPANY_CODE = '{2}' AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+) AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+) ) ) A PIVOT(MAX(WEEK_SALES_QTY) FOR WEEK IN( '01'  , '02'  , '03'  , '04'  , '05' , '06' , '07' , '08' , '09'  , '10'  , '11'  , '12'  , '13'  , '14'  , '15' , '16' , '17' , '18' , '19' , '20' , '21'  , '22'  , '23' , '24' , '25'  , '26'  , '27'  , '28'  , '29'  , '30' , '31'  , '32' , '33' , '34' , '35' , '36' , '37' , '38' , '39' , '40' , '41' , '42'  , '43' , '44' , '45' , '46' , '47'   , '48' , '49' , '50' , '51'  , '52' , '53' )))AA,  (SELECT * FROM(SELECT DISTINCT MODEL_NAME MODEL   , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_SALES_QTY  , ROUND(AVG(QUANTITY) OVER(PARTITION BY MODEL_NAME), 2) AS AVG_WEEKLY_SALES FROM( SELECT MODEL_NAME , CC.CATEGORY_EDESC  , to_char(to_date(SALES_DATE), 'WW-YYYY') WEEK , QUANTITY FROM( SELECT IIS.ITEM_CODE , IIS.CATEGORY_CODE   , IMS.ITEM_EDESC MODEL_NAME  , IIS.ITEM_EDESC ITEM_NAME  , IMS.COMPANY_CODE FROM IP_ITEM_MASTER_SETUP IMS   , IP_ITEM_MASTER_SETUP IIS   WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G'  AND IIS.GROUP_SKU_FLAG = 'I'  AND IIS.COMPANY_CODE = '01'   ) IMS  , V$SALES_INVOICE_REPORT3_MCL SI  , IP_CATEGORY_CODE CC  WHERE IMS.ITEM_CODE = SI.ITEM_CODE  AND IMS.COMPANY_CODE = SI.COMPANY_CODE AND SI.COMPANY_CODE = '01' AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+)  AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+)  )))B,  (SELECT DISTINCT MODEL_NAME MODEL , SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS LAST_FOUR_MONTH_SALES , (SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME)) / 4 AS AVG__LAST_FOUR_MONTH_SALES FROM(  SELECT MODEL_NAME , CC.CATEGORY_EDESC , to_char(to_date(SALES_DATE), 'WW-YYYY') WEEK , QUANTITY  FROM(  SELECT IIS.ITEM_CODE  , IIS.CATEGORY_CODE  , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME   , IMS.COMPANY_CODE  FROM IP_ITEM_MASTER_SETUP IMS  , IP_ITEM_MASTER_SETUP IIS WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)  AND IMS.DELETED_FLAG = 'N'  AND IMS.GROUP_SKU_FLAG = 'G'  AND IIS.GROUP_SKU_FLAG = 'I' AND IIS.COMPANY_CODE = '{2}' ) IMS  , V$SALES_INVOICE_REPORT3_MCL SI   , IP_CATEGORY_CODE CC  WHERE IMS.ITEM_CODE = SI.ITEM_CODE AND IMS.COMPANY_CODE = SI.COMPANY_CODE  AND SI.COMPANY_CODE = '{2}'  AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE(+)  AND IMS.COMPANY_CODE = CC.COMPANY_CODE(+) AND SI.SALES_DATE BETWEEN TO_DATE('{0}', 'YYYY-MON-DD') and TO_DATE('{1}', 'YYYY-MON-DD') ))C, (SELECT DISTINCT MODEL_NAME MODEL ,SUM(QUANTITY) OVER(PARTITION BY MODEL_NAME) AS TOTAL_PURCHASE_QTY FROM(  SELECT IIS.ITEM_CODE  , IIS.CATEGORY_CODE  , IMS.ITEM_EDESC MODEL_NAME , IIS.ITEM_EDESC ITEM_NAME , IMS.COMPANY_CODE  FROM IP_ITEM_MASTER_SETUP IMS   , IP_ITEM_MASTER_SETUP IIS  WHERE IMS.COMPANY_CODE = IIS.COMPANY_CODE  AND TRIM(IMS.MASTER_ITEM_CODE) = TRIM(IIS.PRE_ITEM_CODE)   AND IMS.DELETED_FLAG = 'N' AND IMS.GROUP_SKU_FLAG = 'G' AND IIS.GROUP_SKU_FLAG = 'I'  AND IIS.COMPANY_CODE = '{2}'  ) IMS ,V$IP_PURCHASE_INVOICE PI WHERE IMS.ITEM_CODE = PI.ITEM_CODE AND IMS.COMPANY_CODE = PI.COMPANY_CODE ORDER BY MODEL_NAME)D WHERE AA.MODEL = B.MODEL AND AA.MODEL = C.MODEL   AND AA.MODEL = D.MODEL(+)");
                Query = string.Format(queryString.ToString(), model.FromDate, model.ToDate, companyCode);
            }


            weekSalesData = this._objectEntity.SqlQuery<WeekSalesModel>(Query).ToList();

            return weekSalesData;
        }
    }
}
