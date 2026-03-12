using NeoErp.Core.Plugins;
using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Distribution
{
    public class DistributionPlugin : BasePlugin
    {
        private NeoErpCoreEntity _objectEntity;
        public DistributionPlugin(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }
        public override void Install()
        {
            var rowaffected = InsertMenu();
            if (rowaffected <= 0)
            {
                var query = "delete from WEB_MENU_MANAGEMENT where MODULE_CODE='" + base.PluginDescriptor.ModuleCode + "' ";
                var RowNum = _objectEntity.ExecuteSqlCommand(query);
                return;
            }

            base.Install();
        }
        public override void Uninstall()
        {
            var query = "delete from WEB_MENU_MANAGEMENT where MODULE_CODE='" + base.PluginDescriptor.ModuleCode + "' ";
            var RowNum = _objectEntity.ExecuteSqlCommand(query);
            base.Uninstall();
        }

        public int InsertMenu()
        {
            try
            {
                //Please strictly maintain indentation, MENU_NUMBER and ORDER_BY in the menu insertion Query
                //I have already wasted a complete day just organizing the query correctly
                //Thanks and Best Regards,
                //Rabin Khatiwada
                
                string insertQuery = @"INSERT ALL
                --Organizer
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('14', 'Organizer', 'Organizer', '10', '/Distribution/Home/dashboardlayout#!Distribution/', '/Organiser', 'fa fa-times', 'I', '14', '01', '01', SYSDATE, 1, 'O', '#808080', 'All navigation in distribution')
                --Company Activity
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('06', 'Company Activity', 'Company Activity', '10', '/Distribution/Home/Dashboard#!Distribution', 'javascript:;', 'fa fa-building-o', 'G', '00', '01', '01', SYSDATE, 2, 'CA', '#808080', 'Company activity of distribution module')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('06.01', 'Generate Sales Order', 'Generate Sales Order', '10', '/Distribution/Home/Dashboard#!Distribution/Purchase/POIndex', '/Purchase/POIndex', 'fa fa-file-archive-o', 'I', '06', '01', '01', SYSDATE, 1, 'GSO', '#808080', 'Generate sales Order of distribution')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('06.02', 'Cancelled Purchase Order', 'Cancelled Purchase Order', '10', '/Distribution/Home/Dashboard#!Distribution/Purchase/CancelledSalesOrder', '/Purchase/CancelledSalesOrder', 'fa fa-file-word-o', 'I', '06', '01', '01', SYSDATE, 2, 'CPO', '#808080', 'Cancelled purchase order of distribution')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('06.03', 'Approved Sales Order', 'Approved Sales Order', '10', '/Distribution/Home/Dashboard#!Distribution/Purchase/ApprovedSalesOrder', '/Purchase/ApprovedSalesOrder', 'fa fa-file-archive-o', 'I', '06', '01', '01', SYSDATE, 3, 'ASO', '#808080', 'Approved sales order of distribution')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('06.05', 'Dealer Sales Order', 'Dealer Sales Order', '10', '/Distribution/Home/Dashboard#!Distribution/Purchase/DealerSalesOrder', '/Purchase/DealerSalesOrder', 'fa fa-file-archive-o', 'I', '06', '01', '01', SYSDATE, 5, 'DSO', '#808080', 'Dealer sales Order of distribution')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('06.04', 'Dashboard', 'Dashboard', '10', '/Distribution/Home/Dashboard#!Distribution/Dashboard', '/Dashboard', 'fa fa-home', 'I', '06', '01', '01', SYSDATE, 4, 'DB', '#808080', 'Dashboard of distribution')
                --Reports
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07', 'Reports', 'Reports', '10', '/Distribution/Home/Dashboard#!Distribution', 'javascript:;', 'fa icon-doc', 'G', '00', '01', '10', SYSDATE, 3, 'R', '#808080', 'Reports')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.01', 'Route Visit Summary', 'Route Visit Summary', '10', '/Distribution/Home/Dashboard#!Distribution/VisitSummary', '/VisitSummary', 'fa fa-road', 'I', '07', '01', '10', SYSDATE, 1, 'RVS', '#808080', 'Route visit summary of sales persons')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.02', 'Dues Collection Report', 'Dues Collection Report', '10', '/Distribution/Home/Dashboard#!Distribution/collectionreport', '/collectionreport', 'fa fa-money', 'I', '07', '01', '10', SYSDATE, 2, 'DCR', '#808080', 'Dues collection report of distribution')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.03', 'Survey Report', 'Survey Report', '10', '/Distribution/Home/Dashboard#!Distribution/QuestionnaireReport', '/QuestionnaireReport', 'fa fa-question-circle', 'I', '07', '01', '10', SYSDATE, 3, 'QR', '#808080', 'Questionnaire report of distribution')
              INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.31', 'Web Question Answer Report', 'Web Question Answer Report', '10', '/Distribution/Home/Dashboard#!Distribution/webQuestionnaireReport', '/webQuestionnaireReport', 'fa fa-question-circle', 'I', '07', '01', '10', SYSDATE, 3, 'QR', '#808080', 'Web Questionnaire report of distribution')

                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.04', 'Purchase Order Summary', 'Purchase Order Summary', '10', '/Distribution/Home/Dashboard#!Distribution/getSalesOrderDetails', '/getSalesOrderDetails', 'fa fa-cart-arrow-down', 'I', '07', '01', '10', SYSDATE, 4, 'PO', '#808080', 'purchase order summary of distribution')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.05', 'Performance Report', 'Performance Report', '10', '/Distribution/Home/Dashboard#!Distribution/performanceReport', '/performanceReport', 'fa fa-line-chart', 'I', '07', '01', '10', SYSDATE, 5, 'PR', '#808080', 'Performance Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.06', 'EOD', 'EOD', '10', '/Distribution/Home/Dashboard#!Distribution/EOD', '/EOD', 'fa fa-file-text-o', 'I', '07', '01', '10', SYSDATE, 6, 'PO', '#808080', 'End of day Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.07', 'Sales Person Report', 'Sales Person Report', '10', '/Distribution/Home/Dashboard#!Distribution/SalesPersonPO', '/SalesPersonPO', 'fa fa-adn', 'I', '07', '01', '10', SYSDATE, 7, 'DM', '#808080', 'Sales Person Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.08', 'Item Cumulative Report', 'Item Cumulative Report', '10', '/Distribution/Home/Dashboard#!Distribution/ItemCumulativeReport', '/ItemCumulativeReport', 'fa fa-align-center', 'I', '07', '01', '10', SYSDATE, 8, 'CR', '#808080', 'Item cumulative report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.09', 'Mr Visit Map', 'Mr Visit Map', '10', '/Distribution/Home/Dashboard#!Distribution/MrVisitTracking', '/MrVisitTracking', 'fa fa-map-marker', 'I', '07', '01', '10', SYSDATE, 9, 'VM', '#808080', 'Map displaying the visit of salespersons') 
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.10', 'Reseller Po Summary', 'Reseller Po Summary', '10', '/Distribution/Home/Dashboard#!Distribution/resellerordersummary', '/resellerordersummary', 'fa fa-anchor', 'I', '07', '01', '10', SYSDATE, 10, 'RP', '#808080', 'Order summary of resellers')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.11', 'Attendance Report', 'Attendance Report', '10', '/Distribution/Home/Dashboard#!Distribution/AttendanceReport', '/AttendanceReport', 'fa fa-check-square-o', 'I', '07', '01', '10', SYSDATE, 11, 'AT', '#808080', 'Attandance report of salespersons')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.12', 'MR Visit Tracking', 'MR Visit Tracking', '10', '/Distribution/Home/Dashboard#!Distribution/MRVisitTrackingMap', '/MRVisitTrackingMap', 'fa fa-road', 'I', '07', '01', '10', SYSDATE, 12, 'VT', '#808080', 'Map displaying the latest locations of salespersons')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.13', 'MR Visit Map', 'MR Visit Map', '10', '/Distribution/Home/Dashboard#!Distribution/MRVisitMap', '/MRVisitMap', 'fa fa-road', 'I', '07', '01', '10', SYSDATE, 13, 'VT', '#808080', 'Map displaying the latest locations of salespersons')
                INTO WEB_MENU_MANAGEMENT(MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.14', 'Cummulative DSR', 'Cummulative DSR', '10', '/Distribution/Home/Dashboard#!Distribution/EmployeeWisePerformance', '/EmployeeWisePerformance', 'fa fa-user', 'I', '07', '01', '10', SYSDATE, 14, 'DSR', '#808080', 'Employeewise performance report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.15', 'Daily Activity Report', 'Daily Activity Report', '10', '/Distribution/Home/Dashboard#!Distribution/DailyActivityReport', '/DailyActivityReport', '	fa fa-calendar-o', 'I', '07', '01', '10', SYSDATE, 15, 'DA', '#808080', 'Daily activity')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.16', 'Closing Stock Report', 'Closing Stock Report', '10', '/Distribution/Home/Dashboard#!Distribution/ClosingStock', '/ClosingStock', 'fa fa-line-chart', 'I', '07', '01', '10', SYSDATE, 16, 'CS', '#808080', 'Closing Stock report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.17', 'Sales Register', 'Sales Register', '10', '/Distribution/Home/Dashboard#!Distribution/SalesRegister', '/SalesRegister', 'fa fa-book', 'I', '07', '01', '10', SYSDATE, 17, 'SR', '#808080', 'Sales register report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.18', 'Outlet Closing Stock', 'Outlet Closing Stock', '10', '/Distribution/Home/Dashboard#!Distribution/OutletClosingReport', '/OutletClosingReport', 'fa fa-building', 'I', '07', '01', '10', SYSDATE, 18, 'SR', '#808080', 'Closing stock report of outlets')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.19', 'Contract Q/A Report', 'Contract Q/A Report', '10', '/Distribution/Home/Dashboard#!Distribution/ContractAnsReport', '/ContractAnsReport', 'fa fa-pencil-square-o', 'I', '07', '01', '10', SYSDATE, 19, 'CQR', '#808080', 'Q/A report of schemes')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.20', 'Scheme Report', 'Scheme Report', '10', '/Distribution/Home/Dashboard#!Distribution/SchemeReport', '/SchemeReport', 'fa fa-file-word-o', 'I', '07', '01', '10', SYSDATE, 20, 'SR', '#808080', 'Scheme report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.21', 'Visit Image Gallery', 'Visit Image Gallery', '10', '/Distribution/Home/Dashboard#!Distribution/VisitImageGallery', '/VisitImageGallery', 'fa fa-file-image-o', 'I', '07', '01', '10', SYSDATE, 21, 'IG', '#808080', 'Visit Image gallary')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.22', 'Competitor Item Report', 'Competitor Item Report', '10', '/Distribution/Home/Dashboard#!Distribution/CompetitorReport', '/CompetitorReport', 'fa fa-flag-checkered', 'I', '07', '01', '10', SYSDATE, 22, 'IG', '#808080', 'Competitor Item Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.23', 'Sales Person Distance Report', 'Sales Person Distance Report', '10', '/Distribution/Home/Dashboard#!Distribution/DistanceReport', '/DistanceReport', 'fa fa-adn', 'I', '07', '01', '10', SYSDATE, 23, 'SDR', '#808080', 'Sales Person Distance Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.24', 'Sales Person Route Report', 'Sales Person Route Report', '10', '/Distribution/Home/Dashboard#!Distribution/RouteReport', '/RouteReport', 'fa fa-road', 'I', '07', '01', '10', SYSDATE, 24, 'SRR', '#808080', 'Sales Person Route Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.25', 'Reseller Detail Report', 'Reseller Detail Report', '10', '/Distribution/Home/Dashboard#!Distribution/ResellerDetailReport', '/ResellerDetailReport', 'fa fa-file-word-o', 'I', '07', '01', '10', SYSDATE, 25, 'RDR', '#808080', 'Reseller Detail Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.26', 'Competitor Item Monthly Report', 'Competitor Item Monthly Report', '10', '/Distribution/Home/Dashboard#!Distribution/CompetitorMonthlyReport', '/CompetitorMonthlyReport', 'fa fa-flag-checkered', 'I', '07', '01', '10', SYSDATE, 26, 'CMR', '#808080', 'Competitor Item Monthly Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.27', 'Items MinMax Report', 'Items MinMax Report', '10', '/Distribution/Home/Dashboard#!Distribution/ItemsMinMax', '/ItemsMinMax', 'fa fa-road', 'I', '07', '01', '10', SYSDATE, 27, 'CIF', '#808080', 'Get Min Max Questions Itemwise')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.28', 'Daily Sales Report', 'Daily Sales Report', '10', '/Distribution/Home/Dashboard#!Distribution/EmployeeDetailReport', '/EmployeeDetailReport', 'fa fa-road', 'I', '07', '01', '10', SYSDATE, 28, 'DSR', '#808080', 'Daily Sales Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.29', 'Employee Route Report', 'Employee Route Report', '10', '/Distribution/Home/Dashboard#!Distribution/CalendarRouteReport', '/CalendarRouteReport', 'fa fa-calendar', 'I', '07', '01', '10', SYSDATE, 29, 'ERR', '#808080', 'Employee Calendar Route Report')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('07.30', 'Device Log Report', 'Device Log Report', '10', '/Distribution/Home/Dashboard#!Distribution/DeviceLog', '/DeviceLog', 'fa fa-road', 'I', '07', '01', '10', SYSDATE, 30, 'DLR', '#808080', 'Sales person mobile logs')
                --setup
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08', 'Setup', 'Setup', '10', '/Distribution/Home/Dashboard#!Distribution', 'javascript:;', 'fa fa-cogs', 'G', '00', '01', '10', SYSDATE, 4, 'S', '#808080', 'Setup section of distribution module')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.01', 'Area Setup', 'Area Setup', '10', '/Distribution/Home/Dashboard#!Distribution/AreaSetup', '/AreaSetup', 'fa fa-area-chart', 'I', '08', '01', '10', SYSDATE, 1, 'AS', '#808080', 'area setup of distribution')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.02', 'Distributor Route Setup', 'Distributor Route Setup', '10', '/Distribution/Home/Dashboard#!Distribution/RouteSetup', '/RouteSetup', 'fa fa-road', 'I', '08', '01', '10', SYSDATE, 2, 'DRS', '#808080', 'Route setup for distribution')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.03', 'Branding Route Setup', 'Branding Route Setup', '10', '/Distribution/Home/Dashboard#!Distribution/BrandingRouteSetup', '/BrandingRouteSetup', 'fa fa-road', 'I', '08', '01', '10', SYSDATE, 3, 'BRS', '#808080', 'Route setup for branding')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.04', 'Distributor Setup', 'Distributor Setup', '10', '/Distribution/Home/Dashboard#!Distribution/DistributorSetup', '/DistributorSetup', 'fa fa-truck', 'I', '08', '01', '10', SYSDATE, 4, 'DIS', '#808080', 'Distributor setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.05', 'Outlet Setup', 'Outlet Setup', '10', '/Distribution/Home/Dashboard#!Distribution/ResellerSetup', '/ResellerSetup', 'fa fa-building', 'I', '08', '01', '10', SYSDATE, 5, 'OUS', '#808080', 'Outlet setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.06', 'Dealer Setup', 'Dealer Setup', '10', '/Distribution/Home/Dashboard#!Distribution/DealerSetup', '/DealerSetup', 'fa fa-suitcase', 'I', '08', '01', '10', SYSDATE, 6, 'DES', '#808080', 'Dealer setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.07', 'OutletType Setup', 'OutletType Setup', '10', '/Distribution/Home/Dashboard#!Distribution/getOutLet', '/getOutLet', 'fa fa-shopping-cart', 'I', '08', '01', '10', SYSDATE, 7, 'OTS', '#808080', 'Type setup for outlets')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.08', 'Questionnaire Setup', 'Questionnaire Setup', '10', '/Distribution/Home/Dashboard#!Distribution/QuestionList', '/QuestionList', 'fa fa-question-circle', 'I', '08', '01', '10', SYSDATE, 8, 'QS', '#808080', 'Question setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.09', 'Preference Setup', 'Preference Setup', '10', '/Distribution/Home/Dashboard#!Distribution/PreferenceSetup', '/PreferenceSetup', 'fa fa-sliders', 'I', '08', '01', '10', SYSDATE, 9, 'PS', '#808080', 'Distribution preference setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, ORDERBY, CREATED_DATE, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.10', 'Zone Setup', 'Zone Setup', '10', '/Distribution/Home/Dashboard#!Distribution/GroupSetup', '/GroupSetup', 'fa fa-globe', 'I', '08', '01', '10', 10, SYSDATE, 'ZS', '#808080', 'Distribution zone setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.11', 'Image Category Setup', 'Image Category Setup', '10', '/Distribution/Home/Dashboard#!Distribution/ImageCategorySetup', '/ImageCategorySetup', 'fa fa-globe', 'I', '08', '01', '10', SYSDATE, 11, 'ICS', '#808080', 'Category for image setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.12', 'User Setup Tree', 'User Setup Tree', '10', '/Distribution/Home/Dashboard#!Distribution/UserSetupTree', '/UserSetupTree', 'fa fa-user', 'I', '08', '01', '10', SYSDATE, 12, 'UST', '#808080', 'Distribution user setup tree')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.13', 'Opening Stock Setup', 'Opening Stock Setup', '10', '/Distribution/Home/Dashboard#!Distribution/OpeningStockSetup', '/OpeningStockSetup', 'fa fa-line-chart', 'I', '08', '01', '10', SYSDATE, 13, 'OSS', '#808080', 'Opening stock setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.14', 'Closing Stock Setup', 'Closing Stock Setup', '10', '/Distribution/Home/Dashboard#!Distribution/ClosingStockSetup', '/ClosingStockSetup', 'fa fa-line-chart', 'I', '08', '01', '10', SYSDATE, 14, 'CSS', '#808080', 'Closing stock setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.15', 'Branding Activity', 'Branding Activity', '10', '/Distribution/Home/Dashboard#!Distribution/BrandingActivity', '/BrandingActivity', 'fa fa-tag', 'I', '08', '01', '10', SYSDATE, 15, 'BA', '#808080', 'Branding Activity')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.16', 'Scheme Contract Setup', 'Scheme Setup', '10', '/Distribution/Home/Dashboard#!Distribution/ContractSetup', '/ContractSetup', 'fa fa-book', 'I', '08', '01', '10', SYSDATE, 16, 'SCS', '#808080', 'Scheme contract setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.17', 'Event Contract Setup', 'Event Setup', '10', '/Distribution/Home/Dashboard#!Distribution/EventContractSetup', '/EventContractSetup', 'fa fa-book', 'I', '08', '01', '10', SYSDATE, 17, 'ECS', '#808080', 'Event contract setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.18', 'Other Contract Setup', 'Other Contract Setup', '10', '/Distribution/Home/Dashboard#!Distribution/OtherContractSetup', '/OtherContractSetup', 'fa fa-book', 'I', '08', '01', '10', SYSDATE, 18, 'OCS', '#808080', 'Other contract setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.19', 'Contract Summary', 'Contract Summary', '10', '/Distribution/Home/Dashboard#!Distribution/ContractSummary', '/ContractSummary', 'fa fa-file-o', 'I', '08', '01', '10', SYSDATE, 19, 'CS', '#808080', 'constract Summary')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.20', 'Notification Setup', 'Notification Setup', '10', '/Distribution/Home/Dashboard#!Distribution/Notifications', '/Notifications', 'fa fa-bell-o', 'I', '08', '01', '10', SYSDATE, 20, 'NO', '#808080', 'Notifications for Mobile')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.21', 'Device Registration', 'Device Registration', '10', '/Distribution/Home/Dashboard#!Distribution/DeviceRegistration', '/DeviceRegistration', 'fa fa-mobile', 'I', '08', '01', '10', SYSDATE, 21, 'CS', '#808080', 'Mobile device registration for salespersons')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.22', 'Query Builder Setup', 'Query Builder Setup', '10', '/Distribution/Home/Dashboard#!Distribution/DistQueryBuilder', '/DistQueryBuilder', 'fa fa-align-center', 'I', '08', '01', '10', SYSDATE, 22, 'QB', '#808080', 'Metric query builder')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.23', 'Other Entity Setup', 'Other Entity Setup', '10', '/Distribution/Home/Dashboard#!Distribution/OtherEntitySetup', '/OtherEntitySetup', 'fa fa-book', 'I', '08', '01', '10', SYSDATE, 23, 'OES', '#808080', 'Other Entity setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.24', 'Competitor Item Setup', 'Competitor Item Setup', '10', '/Distribution/Home/Dashboard#!Distribution/CompItemSetup', '/CompItemSetup', 'fa fa-flag-checkered', 'I', '08', '01', '10', SYSDATE, 24, 'CIS', '#808080', 'Competitor Item Setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.25', 'Competitor Item Map', 'Competitor Item Map', '10', '/Distribution/Home/Dashboard#!Distribution/CompItemMap', '/CompItemMap', 'fa fa-flag-checkered', 'I', '08', '01', '10', SYSDATE, 25, 'CIM', '#808080', 'Competitor Item Map')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.26', 'Competitor Item Field setup', 'Competitor Item Field setup', '10', '/Distribution/Home/Dashboard#!Distribution/CompFieldSetup', '/CompFieldSetup', 'fa fa-flag-checkered', 'I', '08', '01', '10', SYSDATE, 26, 'CIF', '#808080', 'Competitor Item Field setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.27', 'Group Mapping', 'Group Mapping', '10', '/Distribution/Home/Dashboard#!Distribution/GroupMapping', '/GroupMapping', 'fa fa-group', 'I', '08', '01', '10', SYSDATE, 27, 'CIF', '#808080', 'Group Mapping')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.28', 'Quick Setup', 'Quick Setup', '10', '/Distribution/Home/Dashboard#!Distribution/QuickSetup', '/QuickSetup', 'fa fa-cogs', 'I', '08', '01', '10', SYSDATE, 28, 'QS', '#808080', 'Quick Setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.29', 'Item Setup', 'Item Setup', '10', '/Distribution/Home/Dashboard#!Distribution/ItemSetup', '/ItemSetup', 'fa fa-cogs', 'I', '08', '01', '10', SYSDATE, 29, 'QS', '#808080', 'Item Setup')
                INTO WEB_MENU_MANAGEMENT (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR, DESCRIPTION)
                                         VALUES ('08.30', 'Survey Setup', 'Survey Setup', '10', '/Distribution/Home/Dashboard#!Distribution/SurveySetup', '/SurveySetup', 'fa fa-cogs', 'I', '08', '01', '10', SYSDATE, 30, 'QS', '#808080', 'Survey Setup')
                --dist sales return 
                INTO WEB_MENU_MANAGEMENT (MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,VIRTUAL_PATH,ICON_PATH,GROUP_SKU_FLAG,PRE_MENU_NO,COMPANY_CODE,CREATED_BY,CREATED_DATE,ORDERBY,MODULE_ABBR,COLOR,DESCRIPTION)
                                         VALUES('06.06','Dist Sales Return','Dist Sales Return','10','/Distribution/Home/Dashboard#!Distribution/Purchase/DistSalesReturn','/Purchase/DistSalesReturn','fa fa-file-archive-o','I','06','01','02',SYSDATE,5,'GSR','#808080','Dist Sales Return')
                SELECT 1 FROM DUAL";

                var rowCount = _objectEntity.ExecuteSqlCommand(insertQuery);
                return rowCount;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}