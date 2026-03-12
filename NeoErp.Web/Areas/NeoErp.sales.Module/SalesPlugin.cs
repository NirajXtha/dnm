using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.sales.Module
{
    public class SalesPlugin : BasePlugin
    {
        private NeoErpCoreEntity _objectEntity;
        public SalesPlugin(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }
        public override void Install()
        {
            var rowaffected = InsertMenu();
            if (rowaffected <= 0)
            {
                DeleteMenu();
                return;
            }

            base.Install();
        }
        public override void Uninstall()
        {
            DeleteMenu();
            base.Uninstall();
        }
        public int InsertMenu()
        {
            try
            {
                //**************New Query Start here**********************//
                string Query = $@"Insert All
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('02', 'Dashboard', 'Dashboard', '01', 
                                            '/Dashboard', 'javascript:', 'fa fa-bar-chart-o', 'G', '00', 
                                            '01', '01',sysdate , 1, 'NA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.25', 'ProductWise Price List', 'ProductWise Price List', '01', 
                                            '/sales/SalesHome/ProductWisePriceList', '#/SalesHome/ProductWisePriceList', 'glyfter-Production-Planning', 'I', '04', 
                                            '01', '01', sysdate, 75, 'SA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.26', 'Purchase Register', 'Purchase Register', '01', 
                                            '/sales/Purchase/PurchaseRegister', '#/Purchase/PurchaseRegister', 'fa fa-user', 'I', '04', 
                                            '01', '01',sysdate , 76, 'PR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.06', 'Sales Register', 'Sales Register', '01', 
                                            '/sales/SalesHome/SalesRegister', '#/Sales/SalesRegister', 'fa fa-file-text-o', 'I', '04', 
                                            '01', '01', sysdate, 55, 'SA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.02', 'Vat Register', 'Vat Register', '01', 
                                            '/sales/Purchase/PurchaseVatRegister', '#/Purchase/PurchaseVatRegister', 'fa fa-credit-card', 'I', '04', 
                                            '01', '01',sysdate , 50, 'PR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.05', 'Landed Cost Analysis', 'Landed Cost Analysis', '01', 
                                            '/sales/Purchase/PurchaseItemsSummary', '#/Purchase/PurchaseItemsSummary', 'fa fa-trello', 'I', '04', 
                                            '01', '01', sysdate, 54, 'PR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.13', 'Location Vs Branch Wise Stock Report', 'Location Vs Branch Wise Stock Report', '01', 
                                            '/sales/Stock/LocationVsBranchWiseStockReport', '#/StockReport/LocationVsBranchWiseStockReport', 'icon-basket', 'I', '04', 
                                            '01', '01', sysdate, 62, 'ST', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.14', 'ProductionRegister', 'ProductionRegister', '01', 
                                            '/sales/Production/ProductionRegister', '#/ProductionReport/ProductionRegister', 'fa fa-user', 'I', '04', 
                                            '01', '01', sysdate, 63, 'ST', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03.07', 'CustomerWise Profit Analysis Report', 'CustomerWise Profit Analysis Report', '01', 
                                            'sales/SalesHome/CustomerWiseProfitAnalysisReport', '#/SalesHome/CustomerWiseProfileAnalysis', 'fa fa-sellsy', 'I', '03', 
                                            '01', '01', sysdate, 46, 'AR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.16', 'Trial Balance', 'Trial Balance', '01', 
                                            'sales/TrialBalanceReport/TreelistViewTrialBalance', '#/TrialBalance/TreelistViewTrialBalance', 'fa fa-file-text-o', 'I', '04', 
                                            '01', '01', sysdate, 65, 'FA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.20', 'Contract Forecast', 'Contract Forecast', '01', 
                                            'sales/Contract/ContactIncomeForeCast', '#/Contract/ContactIncomeForeCast', 'icon-basket', 'I', '04', 
                                            '01', '01',sysdate , 69, 'NA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.11', 'Goods Receipt Notes', 'Goods Receipt Notes', '01', 
                                            '/Sales/SalesSummaryReport/GoodsReceiptNotes', '#/Sales/GoodsReceiptNotes', 'icon-basket', 'I', '04', 
                                            '01', '01',sysdate , 60, 'NA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.21', 'Consumption Issue Register', 'Consumption Issue Register', '01', 
                                            '/Sales/consumption/ConsumptionIssueRegister', '#/consumption/ConsumptionIssueRegister', 'icon-basket', 'I', '04', 
                                            '01', '01', sysdate, 70, 'NA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.15', 'Journal Day Book', 'Journal Day Book', '01', 
                                            '/sales/JournalVoucher/Daybook', '#/journal/daybook', 'fa fa-file-image-o', 'I', '04', 
                                            '01', '01',sysdate , 64, 'FA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04', 'Tabular', 'Tabular', '01', 
                                            '/Others', 'javascript:', 'fa fa-table', 'G', '00', 
                                            '01', '01', sysdate, 3, 'NA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.17', 'Ledger', 'Ledger', '01', 
                                            '/sales/Ledger/LedgerIndex', '#/Ledger/LedgerIndex', 'fa fa-file-code-o', 'I', '04', 
                                            '01', '01', sysdate, 66, 'FA', '#808080   ')                                         
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.09', 'Purchase Register List', 'Purchase Register List', '01', 
                                            '/sales/Purchase/purchaseRegisterPrivot', '#/Purchase/purchaseRegisterPrivot', 'fa fa-money', 'I', '04', 
                                            '01', '01',sysdate , 58, 'PR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.22', 'Receipt Schedule Report', 'Receipt Schedule Report', '01', 
                                            '/sales/ScheduleReport/ReceiptSchedule', '#/ScheduleReport/ReceiptScheduleReport', 'icon-calendar', 'I', '04', 
                                            '01', '01', sysdate, 71, 'NA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03.06', 'ProductWise Profit Analysis Reprot', 'ProductWise Profit Analysis Reprot', '01', 
                                            '/sales/SalesSummaryReport/PartyWiseGPAnalysisSalesSummary', '#/Sales/ProductWiseProfitAnalysisReprot', 'fa fa-line-chart', 'I', '03', 
                                            '01', '01',sysdate , 45, 'AR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.01', 'Sales Report', 'Sales Report', '01', 
                                            '/Sales/SalesHome/FinalSalesReport', '#/Sales/FinalSalesReport', 'fa fa-file-archive-o', 'I', '04', 
                                            '01', '01', sysdate, 51, 'SA', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.20', 'Materialize Report', 'Materialize Report', '01', 
                                            '/Sales/SalesHome/MaterializeReport', '#/Sales/Materialize', 'fa fa-file-archive-o', 'I', '04', 
                                            '01', '01', sysdate, 51, 'MR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.10', 'Daily Sales Report', 'Daily Sales Report', '01', 
                                            '/Sales/SalesHome/SalesRegisterPrivot', '#/Sales/SalesRegisterPrivotSalesRegisterPrivot', 'fa fa-file-o', 'I', '04', 
                                            '01', '01',sysdate , 59, 'SA', '#808080')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03', 'Analysis', 'Analysis', '01', 
                                            '/Analysis Charts', 'javascript:', 'fa fa-bar-chart-o', 'G', '00', 
                                            '01', '01',sysdate , 2, 'NA', '#808080')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.19', 'Activity Monitor', 'Activity Monitor', '01', 
                                            '/sales/SalesHome/SalesProcessingRegisterContainer', '#/Sales/ActivityMoniter', 'icon-basket', 'I', '04', 
                                            '01', '01', sysdate, 68, 'AC', '#808080')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.08', 'Invoice Wise Landed Cost Analysis', 'Invoice Wise Landed Cost Analysis', '01', 
                                            '/sales/Purchase/PurchaseInvoiceSummary', '#/Purchase/PurchaseInvoiceSummary', 'fa fa-adn', 'I', '04', 
                                            '01', '01', sysdate, 57, 'PR', '#808080')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03.02', 'Compare Chart', 'Compare Chart', '01', 
                                            '/Main/CompareCharts?mainMenu=true', 'Main/CompareCharts?mainMenu=true', 'fa fa-area-chart', 'I', '03', 
                                            '01', '01',sysdate , 41, 'SA', '#808080')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03.05', 'Debtors Chart', 'Debtors Chart', '01', 
                                            '/sales/AgeingReport/DebtorsAgingChart', '#/Aging/DebtorsAgingChart', 'fa fa-bar-chart-o', 'I', '03', 
                                            '01', '01',sysdate , 44, 'SA', '#808080')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03.03', 'Profit Analysis Chart', 'Profit Analysis Chart', '01', 
                                            '/sales/SalesHome/ProfitAnalysisChart', '#/SalesHome/ProfitAnalysisChart', 'fa fa-industry', 'I', '03', 
                                            '01', '01', sysdate, 42, 'AC', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.07', 'Vat Registration', 'Vat Registration', '01', 
                                            '/Sales/SalesHome/VatRegistrationReport', '#/Sales/VatRegister', 'fa fa-hand-o-up', 'I', '04', 
                                            '01', '01', sysdate, 56, 'SA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.03', 'Partywise Vat Sales Report', 'Partywise Vat Sales Report', '01', 
                                            '/sales/SalesHome/SalesSummaryCustomerWise', '#/Sales/SalesSummaryCustomerWise', 'fa fa-file-powerpoint-o', 'I', '04', 
                                            '01', '01',sysdate , 53, 'SA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.18', 'Ageing Report', 'Ageing Report', '01', 
                                            '/sales/AgeingReport/index', '#/Ageing/AgeingReport', 'fa fa-amazon', 'I', '04', 
                                            '01', '01', sysdate, 67, 'FA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03.04', ' Ageing Analysis Report', ' Ageing Analysis Report', '01', 
                                            '/sales/AgeingReport/MonthWiseDebtorsIndex', '#/Ageing/MonthlyAgeingReport', 'icon-screen-tablet', 'I', '03', 
                                            '01', '01', sysdate, 43, 'AR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.23', 'Payment Schedule Report', 'Payment Schedule Report', '01', 
                                            '/sales/ScheduleReport/PaymentSchedule', '#/ScheduleReport/PaymentScheduleReport', 'icon-calendar', 'I', '04', 
                                            '01', '01', sysdate, 74, 'NA', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.12', 'LocationWise Stock Report', 'LocationWise Stock Report', '01', 
                                            '/sales/Stock/LocationWiseStockReport', '#/StockReport/LocationWiseStockReport', 'fa fa-location-arrow', 'I', '04', 
                                            '01', '01',sysdate , 61, 'ST', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('04.24', 'CustomerWise Price List', 'CustomerWise Price List', '01', 
                                            '/sales/SalesHome/CustomerWisePriceList', '#/SalesHome/CustomerWisePriceList', 'fa fa-file-powerpoint-o', 'I', '04', 
                                            '01', '01',sysdate , 73, 'NA', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03.09', 'Top Sales Dealer', 'Top Sales Dealer', '01', 
                                            '/sales/SalesSummaryReport/TopSalesDealer', '#/Sales/TopSalesDealer', 'fa fa-signal', 'I', '03', 
                                            '01', '01', sysdate, 48, 'SA', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('03.08', 'Top Sales Employee', 'Top Sales Employee', '01', 
                                            '/sales/SalesSummaryReport/TopSalesEmployee', '#/Sales/TopSalesEmployee', 'fa fa-linux', 'I', '03', 
                                            '01', '01', sysdate, 47, 'SA', '#808080   ')
                                            into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('03.09', 'Region Wise Sales', 'Region Wise Sales', '01', 
                                                '/sales/SalesSummaryReport/RegionProductSales', '#/SalesSummaryReport/RegionProductSales', 'fa fa-road', 'I', '03', 
                                                '01', '01', sysdate, 49, 'SA', '#808080   ')
                                            into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                             Values
                                               ('03.08', 'Sales Analysis', 'Sales Analysis', '01', 
                                                '/sales/SalesSummaryReport/SalesAnalysis', '#/SalesHome/SalesAnalysis', 'fa fa-flash', 'I', '03', 
                                                '01', 
                                                '01', sysdate, 50, 'SA', '#808080   ', 'Item Groupwise Sales Analysis')
                                             into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                     Values
                                                       ('04.27', 'Weekly Expenses Report', 'Weekly Expenses Report', '01', 
                                                        '/sales/SalesHome/WeeklyExpensesAnalysisReport', '#/Sales/WeeklyExpensesAnalysisReport', 'fa fa-stack-overflow', 'I', '04', 
                                                        '01', '01',sysdate, 'AC', '#808080   ')
                                               into WEB_MENU_MANAGEMENT
                                                  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                                Values
                                                  ('13', 'Organizer', 'Organizer', '01', 
                                                   '/sales/saleshome/dashboard', 'sales/saleshome/dashboard', 'fa fa-times', 'I', '13', 
                                                   '01', 
                                                   '01', TO_DATE('04/25/2017 12:33:12', 'MM/DD/YYYY HH24:MI:SS'), 101, 'SM', '#808080   ', 'Organizer')
                                                into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                             Values
                                               ('10', 'Compare Charts', 'Compare Charts', '01', 
                                                '/Main/CompareCharts', 'Main/CompareCharts', 'fa fa-bar-chart-o', 'I', '10', 
                                                '01', 
                                                '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'), 103, 'SM', '#808080   ', 'COMPARE CHARTS')
                                                                                            into WEB_MENU_MANAGEMENT
                                                  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                                                Values
                                                  ('12', 'Compare Charts Division CollectionWise', 'Compare Charts Division CollectionWise', '01', 
                                                   '/Main/CompareChartDivisionCollectionWise', 'Main/CompareChartDivisionCollectionWise', 'fa fa-bar-chart-o', 'I', '12', 
                                                   '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'), 104, 'SM', '#808080   ')
                                                 into WEB_MENU_MANAGEMENT
                                                  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                                Values
                                                  ('11', 'Compare Charts Division', 'Compare Charts Division', '01', 
                                                   '/Main/CompareChartDivision', 'Main/CompareChartDivision', 'fa fa-bar-chart-o', 'I', '11', 
                                                   '01', 
                                                   '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'), 108, 'SM', '#808080   ', 'COMPARE DIVISION CHARTS')
                                            into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                             Values
                                               ('04.28', 'Weekly Vendor Payment Analysis', 'Weekly Vendor Payment Analysis', '01', 
                                                '/sales/SalesHome/WeeklyVendorPaymentAnalysisReport', '#/Sales/WeeklyVendorPaymentAnalysis', 'fa fa-stack-exchange', 'I', '04', 
                                                '01', '01', sysdate, 'FA', '#808080   ')     
                                                    into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                     Values
                                                       ('04.30', 'Subsidiary Ledger', 'Subsidiary Ledger', '01', 
                                                        '/sales/SubsidiaryLedger/SubsidiaryLedgerIndex', '#/SubsidiaryLedger/SubsidiaryLedgerIndex', 'fa fa-stack-overflow', 'I', '04', 
                                                        '01', '01',sysdate, 'SL', '#808080   ')
                                                    into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                     Values
                                                       ('04.31', 'NCR Report', 'NCR Report', '01', 
                                                        '/Sales/SalesHome/DailySalesReport', '#/Sales/DailySalesReport', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'SL', '#808080')
                                                    
                                                    into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.32', 'Ageing Report Dealer', 'Ageing Report Dealer', '01', 
                                                        '/Sales/AgeingReport/AgeingReportDealer', '#/Ageing/AgeingReportDealer', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')
                                                    into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.33', 'Materilized View', 'Materilized View', '01', 
                                                        '/Sales/SalesHome/MaterilizedViewReport', '#/Sales/MaterilizedViewReport', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')
                                                    into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.34', 'Vat Registration IRD', 'Vat Registration IRD', '01', 
                                                        '/Sales/SalesHome/IRDVatRegistration', '#/Sales/VatRegistrationIRD', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')
                                                    into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.35', 'Agent Sales Register', 'Agent Sales Register', '01', 
                                                        '/Sales/SalesHome/AgentWiseSalesRegister', '#/Sales/AgentWiseSalesRegister', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')
                                                    into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.36', 'Purchase Return Register', 'Purchase Return Register', '01', 
                                                        '/Sales/SalesHome/PurchaseReturnRegister', '#/Sales/PurchaseReturnRegister', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')
                                                     into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.37', 'Purchase Register', 'Purchase Register', '01', 
                                                        '/Sales/SalesHome/PurchaseRegister', '#/Sales/PurchaseRegister', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')

                                                    into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.38', 'Purchase Pending Report', 'Purchase Pending Report', '01', 
                                                        '/Sales/SalesHome/PurchasePendingReport', '#/Sales/PurchasePendingReport', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')
                                            
                                                     into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.39', 'Purchase Order List', 'Purchase Order List', '01', 
                                                        '/Sales/SalesHome/PurchaseOrderReport', '#/Sales/PurchaseOrderReport', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')
                                                        
                                                     into WEB_MENU_MANAGEMENT
                                                        (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                        ('04.40', 'Purchase Vat Registeration','Purchase Vat Registeration', '01', 
                                                        '/Sales/SalesHome/PurchaseVatRegistrationReport', '#/Sales/PurchaseVatRegistrationReport', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'FA', '#808080')

                                                        into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.41', 'ProductWise Sales Summary', 'ProductWise Sales Summary', '01', 
                                                        '/Sales/SalesHome/ProductWiseSalesSummary', '#/Sales/ProductWiseSalesSummary', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'PWSS', '#808080')

                                                        into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.42', 'DateWise Sales Details', 'DateWise Sales Details', '01', 
                                                        '/Sales/SalesHome/DateWiseSalesDetails', '#/Sales/DateWiseSalesDetails', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'DSD', '#808080')
                                                         into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.43', 'DateWise Sales Return Details', 'DateWise Sales Return Details', '01', 
                                                        '/Sales/SalesHome/DateWiseSalesReturnDetails', '#/Sales/DateWiseSalesReturnDetails', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'DSRD', '#808080')
                                                        into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.44', 'BillWise Sales Summary', 'BillWise Sales Summary', '01', 
                                                        '/Sales/SalesHome/BillWiseSalesSummary', '#/Sales/BillWiseSalesSummary', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'BSS', '#808080')
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.45', 'BillWise Sales Return Summary', 'BillWise Sales Return Summary', '01', 
                                                        '/Sales/SalesHome/BillWiseSalesReturnSummary', '#/Sales/BillWiseSalesReturnSummary', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'BSRS', '#808080')
                                                        
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.46', 'Monthwise CustProd NetSales', 'Monthwise CustProd NetSales', '01', 
                                                        '/Sales/SalesHome/MonthwiseCustProdNetSales', '#/Sales/MonthwiseCustProdNetSales', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'BSRS', '#808080')
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.47', 'Branchwise Employee NetSales', 'Branchwise Employee NetSales', '01', 
                                                        '/Sales/SalesHome/BranchwiseEmployeeNetSales', '#/Sales/BranchwiseEmployeeNetSales', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'BENS', '#808080')
                                                        into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.48', 'Vat Sales Register', 'Vat Sales Register', '01', 
                                                        '/Sales/SalesHome/VatSalesRegister', '#/Sales/VatSalesRegister', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'VSRN', '#808080')
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.49', 'Vat Sales Return Register New', 'Vat Sales Return Register New', '01', 
                                                        '/Sales/SalesHome/VatSalesReturnRegisterNew', '#/Sales/VatSalesReturnRegisterNew', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'VSRR', '#808080')
                                                             into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.50', 'Datewise Sales Detail', 'Datewise Sales Detail', '01', 
                                                        '/Sales/SalesHome/DatewiseSalesDetail', '#/Sales/DatewiseSalesDetail', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'DSDJ', '#808080')
                                                         into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.51', 'Monthwise Customer SalesCollection', 'Monthwise Customer SalesCollection', '01', 
                                                        '/Sales/SalesHome/MonthwiseCustomerSalesCollection', '#/Sales/MonthwiseCustomerSalesCollection', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'MCSC', '#808080')
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.52', 'Vat Purchase Register', 'Vat Purchase Register', '01', 
                                                        '/Sales/SalesHome/VatPurchaseRegister', '#/Sales/VatPurchaseRegister', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'VPRN', '#808080')
                                                           into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.53', 'Vat Purchase Return Register New', 'Vat Purchase Return Register New', '01', 
                                                        '/Sales/SalesHome/VatPurchaseReturnRegisterNew', '#/Sales/VatPurchaseReturnRegisterNew', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'VPRR', '#808080')
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.54', 'Groupwise Stock Summary', 'Groupwise Stock Summary', '01', 
                                                        '/Sales/SalesHome/GroupwiseStockSummary', '#/Sales/GroupwiseStockSummary', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'GWSS', '#808080')
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.55', 'Billwise Purchase Summary', 'Billwise Purchase Summary', '01', 
                                                        '/Sales/SalesHome/BillwisePurchaseSummary', '#/Sales/BillwisePurchaseSummary', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'GWSS', '#808080')
                                                            into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.56', 'Net Sales Reports', 'Net Sales Reports', '01', 
                                                        '/Sales/SalesHome/NetSalesReports', '#/Sales/NetSalesReports', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'DNS', '#808080')
                                                           into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.57', 'Order Tracking Reports', 'Order Tracking Reports', '01', 
                                                        '/Sales/SalesHome/OrderTrackingReports', '#/Sales/OrderTrackingReports', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'OTR', '#808080')
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.58', 'NetSales Summary Reports', 'NetSales Summary Reports', '01', 
                                                        '/Sales/SalesHome/NetSalesSummaryReports', '#/Sales/NetSalesSummaryReports', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'NSR', '#808080')
                                                           into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.59', 'Net Purchase Reports', 'Net Purchase Reports', '01', 
                                                        '/Sales/SalesHome/NetPurchaseReports', '#/Sales/NetPurchaseReports', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'NSR', '#808080')
                                                           into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.60', 'Purchase Summary Reports', 'Purchase Summary Reports', '01', 
                                                        '/Sales/SalesHome/PurchaseSummaryReports', '#/Sales/PurchaseSummaryReports', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'PSR', '#808080')
                                                        into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.61', 'Purchase Above OneLakh Report', 'Purchase Above OneLakh Report', '01', 
                                                        '/Sales/SalesHome/PurchaseAboveOneLakhReport', '#/Sales/PurchaseAboveOneLakhReport', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'PAOLR', '#808080')
                                                          
                                                          into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.62', 'Audit Trasaction Report', 'Audit Trasaction Report', '01', 
                                                        '/Sales/SalesHome/AuditTrasactionReport', '#/Sales/AuditTrasactionReport', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'ATR', '#808080')
                                                        
                                                        into WEB_MENU_MANAGEMENT
                                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                                                        Values
                                                       ('04.63', 'Groupstock Summary Report', 'Groupstock Summary Report', '01', 
                                                        '/Sales/SalesHome/GroupstockSummaryReport', '#/Sales/GroupstockSummaryReport', 'fa fa-times', 'I', '04', 
                                                        '01', '01',sysdate, 'GSSR', '#808080')
                                                       
                                           select 1 from dual";



                var RowNum = _objectEntity.ExecuteSqlCommand(Query);
                return RowNum;


                //************New Query End Here




                //                // Activity Monitor
                //               var  query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('02', 'Activity Monitor', 'Activity Monitor',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/SalesHome/SalesProcessingRegisterContainer', '#/Sales/ActivityMoniter', 'icon-basket', 'I', '00', 
                //    '01', '01',sysdate,1)";
                //              var  RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // PurchaseInvoiceSummary
                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('04.04', 'Invoice wise Landed Cost Analysis', 'Invoice wise Landed Cost Analysis',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Purchase/PurchaseInvoiceSummary', '#/Purchase/PurchaseInvoiceSummary', 'fa fa-bar-chart-o', 'I', '04', 
                //    '01', '01',SYSDATE,21)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // purchase Register privoit

                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('04.05', 'Purchase Register List', 'Purchase Register List',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Purchase/purchaseRegisterPrivot', '#/Purchase/purchaseRegisterPrivot', 'fa fa-bar-chart-o', 'I', '04', 
                //    '01', '01',SYSDATE,46)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // ReceiptSchedule
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('10.01', 'Receipt Schedule Report', 'Receipt Schedule Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/ScheduleReport/ReceiptSchedule', '#/ScheduleReport/ReceiptScheduleReport', 'icon-calendar', 'I', '10', 
                //    '01', '01',SYSDATE,22)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // PartyWiseGPAnalysisSalesSummary
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('13.02', 'ProductWise Profit Analysis Reprot', 'ProductWise Profit Analysis Reprot',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/SalesSummaryReport/PartyWiseGPAnalysisSalesSummary', '#/Sales/ProductWiseProfitAnalysisReprot', 'glyfter-HRIS', 'I', '13', 
                //    '01', '01', SYSDATE,20)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                // FinalSalesReport
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('03.01', 'Sales Report', 'Sales Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/Sales/SalesHome/FinalSalesReport', '#/Sales/FinalSalesReport', 'fa fa-file-o', 'I', '03', 
                //    '01', '01',SYSDATE,15)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // FinalSalesReport privoit
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('03.05', 'Sales Register List', 'Sales Register List',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/Sales/SalesHome/SalesRegisterPrivot', '#/Sales/SalesRegisterPrivot', 'fa fa-file-o', 'I', '03', 
                //    '01', '01',SYSDATE,47)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //Analysis Chart's goes here

                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('16', 'Analysis Charts', 'Analysis Charts', '" + base.PluginDescriptor.ModuleCode+@"', 
                //    '/analysis charts', 'javascript:;', 'icon-basket', 'G', '00', 
                //    '01', '01', SYSDATE,0)";
                //         RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE ,ORDERBY)
                // Values
                //   ('16.01', 'Compare Chart', 'Compare Chart', '"+base.PluginDescriptor.ModuleCode+ @"', 
                //    '/Main/CompareCharts?mainMenu=true', 'Main/CompareCharts?mainMenu=true', 'fa fa-bar-chart-o', 'I', '16', 
                //    '01', '01',SYSDATE,40)";

                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('16.02', 'Debtors Chart', 'Debtors Chart', '" + base.PluginDescriptor.ModuleCode+ @"', 
                //    '/sales/AgeingReport/DebtorsAgingChart', '#/Aging/DebtorsAgingChart', 'fa fa-bar-chart-o', 'I', '16', 
                //    '01', '01',SYSDATE,41)";

                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('16.03', 'Profit Analysis Chart', 'Profit Analysis Chart', '"+base.PluginDescriptor.ModuleCode+@"', 
                //    '/sales/SalesHome/ProfitAnalysisChart', '#/SalesHome/ProfitAnalysisChart', 'glyfter-chart', 'I', '16', 
                //    '01', '01', SYSDATE,42)";

                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //Analysis Chart End here



                //                // Vat Registration
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('03.04', 'Vat Registration', 'Vat Registration',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/Sales/SalesHome/VatRegistrationReport', '#/Sales/VatRegister', 'fa fa-hand-o-up', 'I', '03', 
                //    '01', '01',SYSDATE,18)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // PurchaseRegister
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('04.02', 'Purchase Register', 'Purchase Register',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Purchase/PurchaseRegister', '#/Purchase/PurchaseRegister', 'fa fa-user', 'I', '04', 
                //    '01', '01', SYSDATE,19)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // AgeingReport
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('07', 'Ageing Report', 'Ageing Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/AgeingReport/index', '#/Ageing/AgeingReport', 'icon-screen-tablet', 'I', '00', 
                //    '01', '01', SYSDATE,6)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('16.04', ' Ageing Analysis Report', ' Ageing Analysis Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/AgeingReport/MonthWiseDebtorsIndex', '#/Ageing/MonthlyAgeingReport', 'icon-screen-tablet', 'I', '16', 
                //    '01', '01', SYSDATE,47)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                //               // Calender Report
                //                //               query = @"Insert into  WEB_MENU_MANAGEMENT
                //                //  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                //                //Values
                //                //  ('08', 'Calender Report', 'Calender Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //                //   '/Calender Report', 'javascript:;', 'icon-calendar', 'G', '00', 
                //                //   '01', '01', SYSDATE,7)";
                //                //               RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // Daybook Report
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('09', 'Journal Day Book', 'Journal Day Book',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/JournalVoucher/Daybook', '#/journal/daybook', 'fa fa-book', 'I', '00', 
                //    '01', '01', SYSDATE,8)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // Schedule Report
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('10', 'Schedule Report', 'Schedule Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/Schedule Report', 'javascript:;', 'icon-notebook', 'G', '00', 
                //    '01', '01', SYSDATE,9)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                // PaymentSchedule
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('10.02', 'Payment Schedule Report', 'Payment Schedule Report', '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/ScheduleReport/PaymentSchedule', '#/ScheduleReport/PaymentScheduleReport', 'icon-calendar', 'I', '10', 
                //    '01', '01', SYSDATE,23)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // LocationWiseStockReport
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('11.01', 'LocationWise Stock Report', 'LocationWise Stock Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Stock/LocationWiseStockReport', '#/StockReport/LocationWiseStockReport', 'fa fa-location-arrow', 'I', '11', 
                //    '01', '01',SYSDATE,24)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // CustomerWisePriceList
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('12.01', 'CustomerWise Price List', 'CustomerWise Price List',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/SalesHome/CustomerWisePriceList', '#/SalesHome/CustomerWisePriceList', 'glyfter-search', 'I', '12', 
                //    '01', '01', SYSDATE,26)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // ProductWisePriceList
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('12.02', 'ProductWise Price List', 'ProductWise Price List',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/SalesHome/ProductWisePriceList', '#/SalesHome/ProductWisePriceList', 'glyfter-Production-Planning', 'I', '12', 
                //    '01', '01', SYSDATE,27)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // Analysis Report
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('13', 'Analysis Report', 'Analysis Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/Analysis Report', 'javascript:;', 'icon-feed', 'G', '00', 
                //    '01', '01', SYSDATE,12)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //Purchase
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('04', 'Purchase', 'Purchase', '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/Purchase', 'javascript:;', 'icon-basket-loaded', 'G', '00', 
                //    '01', '01', SYSDATE,3)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                //SalesSummaryCustomerWise
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('03.02', 'PartyWise vat sales Report', 'PartyWise vat sales Report', '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/SalesHome/SalesSummaryCustomerWise', '#/Sales/SalesSummaryCustomerWise', 'fa fa-file-pdf-o', 'I', '03', 
                //    '01', '01', SYSDATE,16)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //SalesRegister
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('03.03', 'Sales Register', 'Sales Register',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/SalesHome/SalesRegister', '#/Sales/SalesRegister', 'icon-basket', 'I', '03', 
                //    '01', '01',SYSDATE,17)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //PurchaseVatRegister
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('04.01', 'Vat Register', 'Vat Register',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Purchase/PurchaseVatRegister', '#/Purchase/PurchaseVatRegister', 'fa fa-user', 'I', '04', 
                //    '01', '01',SYSDATE,19)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //PurchaseItemsSummary
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('04.03', 'Landed Cost Analysis', 'Landed Cost Analysis', '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Purchase/PurchaseItemsSummary', '#/Purchase/PurchaseItemsSummary', 'fa fa-trello', 'I', '04', 
                //    '01', '01',SYSDATE,20)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //Sales
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('03', 'Sales', 'Sales', '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/Sales', 'javascript:;', 'icon-home', 'G', '00', 
                //    '01', '01',SYSDATE,2)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //LedgerIndex
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('05', 'Ledger', 'Ledger',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Ledger/LedgerIndex', '#/Ledger/LedgerIndex', 'glyphicon glyphicon-book lcolor', 'I', '00', 
                //    '01', '01', SYSDATE,4)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //stock Ledger
                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('11', 'Stock Report', 'Stock Report', '01', 
                //    '/Stock Report', 'javascript:;', 'glyfter-Sales-revenue', 'G', '00', 
                //    '01', '01',SYSDATE,10)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //LocationVsBranchWiseStockReport
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY)
                // Values
                //   ('11.02', 'Location vs Branch Wise Stock Report', 'Location vs Branch Wise Stock Report',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Stock/LocationVsBranchWiseStockReport', '#/StockReport/LocationVsBranchWiseStockReport', 'glyfter-Fixed-Assets', 'I', '11', 
                //    '01', '01',SYSDATE,25)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                ////query for production

                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH,ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY)
                // Values
                //   ('15', 'Production', 'Production', '" + base.PluginDescriptor.ModuleCode+ @"', 
                //    '/Production', 'javascript:;','fa fa-briefcase', 'G', '00', 
                //    '01', '01', SYSDATE,50)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH,ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY)
                // Values
                //   ('15.01', 'ProductionRegister', 'ProductionRegister', '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/sales/Production/ProductionRegister', '#/ProductionReport/ProductionRegister','fa fa-user', 'I', '15', 
                //    '01', '01',SYSDATE,51)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);
                // //               //CalendarReportIndex
                // //               query = @"Insert into  WEB_MENU_MANAGEMENT
                // //  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // //Values
                // //  ('08.01', 'Product Wise Monthly Sales Analysis', 'Product Wise Monthly Sales Analysis', '" + base.PluginDescriptor.ModuleCode + @"', 
                // //   '/sales/CalendarReport/CalendarReportIndex', '#/Calendar/CalendarReportParam/8', 'icon-calendar', 'I', '08', 
                // //   '01', '01', SYSDATE,51)";
                // //               RowNum = _objectEntity.ExecuteSqlCommand(query);


                // //               //CalendarReportIndex
                // //               query = @"Insert into  WEB_MENU_MANAGEMENT
                // //  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // //Values
                // //  ('08.02', 'Product Wise Monthly Purchase Analaysis', 'Product Wise Monthly Purchase Analaysis', '" + base.PluginDescriptor.ModuleCode + @"', 
                // //   '/sales/CalendarReport/CalendarReportIndex', '#/Calendar/CalendarReportParam/5', 'icon-calendar', 'I', '08', 
                // //   '01', '01',SYSDATE,51)";
                // //               RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //Price List
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('12', 'Price List', 'Price List',  '" + base.PluginDescriptor.ModuleCode + @"', 
                //    '/Price List', 'javascript:;', 'glyfter-cashhand', 'G', '00', 
                //    '01', '01',SYSDATE,11)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                //Price List
                //                query = @"Insert into  WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // Values
                //   ('13.01', 'CustomerWise Profit Analysis Report', 'CustomerWise Profit Analysis Report', '" + base.PluginDescriptor.ModuleCode + @"', 
                //    'sales/SalesHome/CustomerWiseProfitAnalysisReport', '#/SalesHome/CustomerWiseProfileAnalysis', 'glyfter-CRM', 'I', '13', 
                //    '01', '01',SYSDATE,29)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                // Profit Analysis Chart
                // //               query = @"Insert into WEB_MENU_MANAGEMENT
                // //  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // //Values
                // //  ('15', 'Profit Analysis Chart', 'Profit Analysis Chart', '01',
                // //   '/sales/SalesHome/ProfitAnalysisChart', '#/SalesHome/ProfitAnalysisChart', 'glyfter-chart', 'I', '00',
                // //   '01', '01', SYSDATE,13)";
                // //               RowNum = _objectEntity.ExecuteSqlCommand(query);



                //                //Aging Chart
                // //               query = @"Insert into  WEB_MENU_MANAGEMENT
                // //  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // //Values
                // //  ('14', 'Aging Chart', 'Aging Chart',  '" + base.PluginDescriptor.ModuleCode + @"', 
                // //   '/Aging Chart', 'javascript:;', 'fa fa-bar-chart-o', 'G', '00', 
                // //   '01', '01', SYSDATE,40)";
                // //               RowNum = _objectEntity.ExecuteSqlCommand(query);


                //                //DebtorsAgingChart
                // //               query = @"Insert into  WEB_MENU_MANAGEMENT
                // //  (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,ORDERBY)
                // //Values
                // //  ('14.01', 'Debtors Chart', 'Debtors Chart',  '" + base.PluginDescriptor.ModuleCode + @"', 
                // //   '/sales/AgeingReport/DebtorsAgingChart', '#/Aging/DebtorsAgingChart', 'fa-bar-chart-o', 'I', '14', 
                // //   '01', '01', SYSDATE,41)";
                // //               RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                //TRIAL BALANCE
                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY)
                // Values
                //   ('17', 'Trial Balance', 'Trial Balance',  '" + base.PluginDescriptor.ModuleCode + @"',
                //    'sales/TrialBalanceReport/TreelistViewTrialBalance', '#/TrialBalance/TreelistViewTrialBalance', 'icon-basket', 'I', '00',
                //    '01', '01', sysdate,5)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY)
                // Values
                //   ('01', 'Contract Forecast', 'Contract Forecast',  '" + base.PluginDescriptor.ModuleCode + @"',
                //    'sales/Contract/ContactIncomeForeCast', '#/Contract/ContactIncomeForeCast', 'icon-basket', 'I', '00',
                //    '01', '01', sysdate,45)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);



                //                //GOODS RECEIPT NOTES REPORT
                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY)
                // Values
                //   ('03.06', 'Goods Receipt Notes', 'Goods Receipt Notes',  '" + base.PluginDescriptor.ModuleCode + @"',
                //    '/Sales/SalesSummaryReport/GoodsReceiptNotes', '#/Sales/GoodsReceiptNotes', 'icon-basket', 'I', '03',
                //    '01', '01', sysdate,5)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);



                //                //CONSUMPTION REPORT
                //                query = @"Insert into WEB_MENU_MANAGEMENT
                //   (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY)
                // Values
                //   ('18', 'Consumption Issue Register', 'Consumption Issue Register',  '" + base.PluginDescriptor.ModuleCode + @"',
                //    '/Sales/consumption/ConsumptionIssueRegister', '#/consumption/ConsumptionIssueRegister', 'icon-basket', 'I', '00',
                //    '01', '01', sysdate,5)";
                //                RowNum = _objectEntity.ExecuteSqlCommand(query);

                //   return RowNum;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int DeleteMenu()
        {
            var queryControl = $@" delete from WEB_MENU_CONTROL where menu_no in (select  Menu_no from WEB_MENU_MANAGEMENT where module_code='{base.PluginDescriptor.ModuleCode}')";
            var rowNumber = _objectEntity.ExecuteSqlCommand(queryControl);
            var query = "delete from WEB_MENU_MANAGEMENT where MODULE_CODE='" + base.PluginDescriptor.ModuleCode + "' ";
            var RowNum = _objectEntity.ExecuteSqlCommand(query);
            //delete from dashboard widgets (temporary )
            if (base.PluginDescriptor.ModuleCode == "01")
            {
                var queryWidgets = "delete FROM DASHBOARD_WIDGETS";
                _objectEntity.ExecuteSqlCommand(queryWidgets);
            }
            return RowNum;
        }
    }
}