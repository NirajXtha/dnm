using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoERP.Planning
{
    public class PlanningPlugin : BasePlugin
    {
        private NeoErpCoreEntity _objectEntity;
        public PlanningPlugin(NeoErpCoreEntity objectEntity)
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
                //**************New Query Start here**********************//
                string Query = $@"Insert All
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30', 'Planning', 'Sales Planning', '30', 
                                            '/Planning/Home/Setup', 'javascript:;', 'fa fa-calendar-plus-o', 'G', '00', 
                                            '01', '01', sysdate,'Y', 1, 'SP', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.01', 'Plans', 'Plans', '30', 
                                            '/Planning/Home/Setup#!Planning/PlanList', '/Planning/Home/Setup#!Planning/PlanList', 'fa fa-tachometer', 'I', '30', 
                                            '01', '01',sysdate ,'Y', 2, 'SM', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.02', 'Master Plans', 'Master Plans', '30', 
                                            '/Planning/Home/Setup#!Planning/MasterPlan', '/Planning/Home/Setup#!Planning/MasterPlan', 'fa fa-sitemap', 'I', '30', 
                                            '01', '01', sysdate,'Y', 3, 'MP', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('34.01', 'Frequency', 'Frequency', '30', 
                                            '/Planning/Home/Setup#!Planning/FrequencySetup', '/Planning/Home/Setup#!Planning/FrequencySetup', 'fa fa-forumbee', 'I', '34', 
                                            '01', '01',sysdate ,'Y', 4, 'FR', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('34.02', 'Employee Tree', 'Employee Tree', '30', 
                                            '/Planning/Home/Setup#!Planning/EmployeeTree', '/Planning/Home/Setup#!Planning/EmployeeTree', 'fa fa-sitemap', 'I', '34', 
                                            '01', '01', sysdate,'Y', 5, 'ET', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.05', ' Ledger Plan', 'Ledger Plan', '30', 
                                            '/Planning/Home/Setup#!Planning/BudgetPlan', '/Planning/Home/Setup#!Planning/BudgetPlan', 'fa fa-sticky-note', 'I', '30', 
                                            '01', '01', sysdate,'Y', 6, 'BP', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.06', 'Branding Plan', 'Branding Plan', '30', 
                                            '/Planning/Home/Setup#!Planning/BrandingPlan', '/Planning/Home/Setup#!Planning/BrandingPlan', 'fa  fa-columns', 'I', '30', 
                                            '01', '01', sysdate,'Y', 7, 'BP', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.07', 'Procurement Plan', 'Procurement Plan', '30', 
                                            '/Planning/Home/Setup#!Planning/ProcurementPlan', '/Planning/Home/Setup#!Planning/ProcurementPlan', 'fa fa-cube', 'I', '30', 
                                            '01', '01', sysdate,'Y', 7, 'PP', '#808080   ')
                                          into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.08', 'Production Plan', 'Production Plan', '30', 
                                            '/Planning/Home/Setup#!Planning/ProductionPlan', '/Planning/Home/Setup#!Planning/ProductionPlan', 'fa fa-bitbucket', 'I', '30', 
                                            '01', '01', sysdate,'Y', 8, 'PP', '#808080   ')
                                       into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.09', 'Budget Plan', 'Budget Plan', '30', 
                                            '/Planning/Home/Setup#!Planning/LedgerBudgetPlan', '/Planning/Home/Setup#!Planning/LedgerBudgetPlan', 'fa fa-dollar', 'I', '30', 
                                            '01', '01', sysdate,'Y', 9, 'PP', '#808080   ')
                                       into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('31.02', 'Monthly Sales Plan Report', 'Monthly Sales Plan Report', '30', 
                                            '/Planning/Home/Setup#!Planning/MonthlyWiseSalesPlanReport', '/Planning/Home/Setup#!Planning/MonthlyWiseSalesPlanReport', 'fa fa-folder-open', 'I', '31', 
                                            '01', '01', sysdate,'Y', 9, 'PP', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('34.04', 'Temp Table Calculation', 'Temp Table Calculation', '30', 
                                            '/Planning/Home/Setup#!Planning/TempTableSetup', '/Planning/Home/Setup#!Planning/TempTableSetup', 'fa fa-cog', 'I', '34', 
                                            '01', '01', sysdate,'Y', 6, 'PS', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('34.03', 'Preference Setup', 'Preference Setup', '30', 
                                            '/Planning/Home/Setup#!Planning/Setup', '/Planning/Home/Setup#!Planning/Setup', 'fa fa-cogs', 'I', '34', 
                                            '01', '01', sysdate,'Y', 7, 'PS', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.10', 'Create Budget Plan', 'Create Budget Plan', '30', 
                                            '/Planning/Home/Index#!Planning/CreateLedgerBudgetPlan','/Planning/Home/Index#!Planning/CreateLedgerBudgetPlan', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.11', 'Ledger  Plan Setup', 'Ledger  Plan Setup', '30', 
                                            '/Planning/Home/Index#!Planning/LedgerBudgetPlanSetup','/Planning/Home/Index#!Planning/LedgerBudgetPlanSetup', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                      into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.12', 'Create Production Plan', 'Create Production Plan', '30', 
                                            '/Planning/Home/Index#!Planning/CreateProductionPlan','/Planning/Home/Index#!Planning/CreateProductionPlan', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.13', 'Production Plan Setup', 'Production Plan Setup', '30', 
                                            '/Planning/Home/Index#!Planning/ProductionPlanSetup','/Planning/Home/Index#!Planning/ProductionPlanSetup', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                       into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.14', 'Create Procurement Plan', 'Create Procurement Plan', '30', 
                                            '/Planning/Home/Index#!Planning/CreateProcurementPlan','/Planning/Home/Index#!Planning/CreateProcurementPlan', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                      into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.15', 'Procurement Plan Setup', 'Procurement Plan Setup', '30', 
                                            '/Planning/Home/Index#!Planning/ProcurementPlanSetup','/Planning/Home/Index#!Planning/ProcurementPlanSetup', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                      into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.16', 'Create Ledger Plan', 'Create Ledger Plan', '30', 
                                            '/Planning/Home/Index#!Planning/CreateBudgetPlan','/Planning/Home/Index#!Planning/CreateBudgetPlan', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                      into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.17', 'Ledger Plan Setup', 'Ledger Plan Setup', '30', 
                                            '/Planning/Home/Index#!Planning/BudgetPlanSetup','/Planning/Home/Index#!Planning/BudgetPlanSetup', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                      into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.18', 'Create Plan', 'Create Plan', '30', 
                                            '/Planning/Home/Index#!Planning/CreatePlan','/Planning/Home/Index#!Planning/CreatePlan', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.19', 'Plan Setup', 'Plan Setup', '30', 
                                            '/Planning/Home/Index#!Planning/PlanSetup','/Planning/Home/Index#!Planning/PlanSetup', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.20', 'Create Branding Plan', 'Create Branding Plan', '30', 
                                            '/Planning/Home/Index#!Planning/CreateBrandingPlan','/Planning/Home/Index#!Planning/CreateBrandingPlan', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')                                      
                                       into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.21', 'Branding Plan Setup ', 'Branding Plan Setup ', '30', 
                                            '/Planning/Home/Index#!Planning/BrandingPlanSetup','/Planning/Home/Index#!Planning/BrandingPlanSetup', 'fa fa-cogs', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.22', 'Material Plan', 'Material Plan', '30', 
                                            '/Planning/Home/Index#!Planning/MaterialPlan','/Planning/Home/Index#!Planning/MaterialPlan', 'fa fa-table', 'I', '30', 
                                            '01', '01', sysdate,'Y', 7, 'PS', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.23', 'Material Plan Setup', 'Material Plan Setup', '30', 
                                            '/Planning/Home/Index#!Planning/MaterialPlanSetup','/Planning/Home/Index#!Planning/MaterialPlanSetup', 'fa fa-table', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.24', 'Create Material Plan', 'Create Material Plan', '30', 
                                            '/Planning/Home/Index#!Planning/CreateMaterialPlan','/Planning/Home/Index#!Planning/CreateMaterialPlan', 'fa fa-table', 'I', '30', 
                                            '01', '01', sysdate,'N', 7, 'PS', '#808080   ')

                                           into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.25', 'Collection Plan', 'Collection Plan', '30', 
                                            '/Planning/Home/Setup#!Planning/CollectionPlan', '/Planning/Home/Setup#!Planning/CollectionPlan', 'fa fa-cube', 'I', '30', 
                                            '01', '01', sysdate,'Y', 7, 'PP', '#808080   ')
                          into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.23', 'Material Plan PI', 'Material Plan PI', '30', 
                                            '/Planning/Home/Index#!Planning/MaterialPlanPI', '/Planning/Home/Index#!Planning/MaterialPlanPI', 'fa fa-cube', 'I', '30', 
                                            '01', '01', sysdate,'Y', 9, 'PP', '#808080   ');
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('30.26', 'Production Plan New', 'Production Plan New', '30', 
                                            '/Planning/Home/Index#!Planning/ProductionPlanNew', '/Planning/Home/Index#!Planning/ProductionPlanNew', 'fa fa-cube', 'I', '30', 
                                            '01', '01', sysdate,'Y', 10, 'PP', '#808080   ')

                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('31', 'Planning Report', 'Sales Planning Report', '30', 
                                            'javascript:;', 'javascript:;', 'fa fa-file-text', 'G', '00', 
                                            '01', '01', sysdate,'Y', 7, 'RE', '#808080   ')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('31.01', 'Plans Report', 'Plans Report', '30', 
                                            '/Planning/PlanReport/Index#!Planning/PlanReport', '/Planning/PlanReport/Index#!Planning/PlanReport', 'fa  fa-th-list', 'I', '31', 
                                            '01', '01', sysdate,'Y', 8, 'PR', '#808080   ')   
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('32', 'Visit Plan', 'Visit Plan', '30', 
                                            'javascript:;', 'javascript:;', 'fa fa-bus', 'G', '00', 
                                            '01', '01', sysdate,'Y', 8, 'VP', '#808080   ')          
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('32.01', 'Plans Routes', 'Plans Routes', '30', 
                                            '/Planning/DistributionPlaning/Index#!Planning/RouteList', '/Planning/DistributionPlaning/Index#!Planning/RouteList', 'fa fa-road', 'I', '32', 
                                            '01', '01', sysdate,'Y', 8, 'PR', '#808080   ')       
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('32.02', 'Sales Person Route', 'Sales Person Route', '30', 
                                            '/Planning/DistributionPlaning/Index#!Planning/EmployeeRouteSetup', '/Planning/DistributionPlaning/Index#!Planning/EmployeeRouteSetup', 'fa fa-bus', 'I', '32', 
                                            '01', '01', sysdate,'Y', 8, 'PR', '#808080   ')    
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('32.03', 'Sales Person Calendar Route', 'Sales Person Calendar Route', '30', 
                                            '/Planning/DistributionPlaning/Index#!Planning/CalendarRouteSetup', '/Planning/DistributionPlaning/Index#!Planning/CalendarRouteSetup', 'fa fa-outdent', 'I', '32', 
                                            '01', '01', sysdate,'Y', 9, 'CR', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('32.04', 'Other Route Plan', 'Other Route Plan', '30', 
                                            '/Planning/DistributionPlaning/Index#!Planning/CalendarBrandingRouteSetup', '/Planning/DistributionPlaning/Index#!Planning/CalendarBrandingRouteSetup', 'fa fa-globe', 'I', '32', 
                                            '01', '01', sysdate,'Y', 10, 'CR', '#808080')
                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                         Values
                                           ('34', 'Setup or Configuration', 'Setup or Configuration', '30', 
                                            'javascript:;', 'javascript:;', 'fa fa-cog', 'G', '00', 
                                            '01', '01', sysdate,'Y', 8, 'VP', '#808080   ')                            
                                         into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('33', 'Activity', 'Activity', '30', 
                                                'javascript:;', 'javascript:;', 'fa fa-bank', 'G', '00', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 1, 'SM', '#808080')     
                                          into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('33.01', 'DashBoard', 'DashBoard', '30', 
                                                '/Planning/Home/Setup#!Planning/Dashboard', '/Planning/Home/Setup#!Planning/Dashboard', 'fa fa-dashboard', 'I', '33', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 1, 'DB', '#808080')    
                                               into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('33.02', 'Organiser', 'Organiser', '30', 
                                                '/Planning/Home/PlanningDashboard', '/Planning/Home/PlanningDashboard', 'fa fa-bank', 'I', '33', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 1, 'OG', '#808080')    
                                         into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('31.04', 'Procument Plan Report', 'Procument Plan Report', '30', 
                                                '/Planning/Home/Setup#!Planning/FavProcurementPlanReport', '/Planning/Home/Setup#!Planning/FavProcurementPlanReport', 'fa  fa-server', 'I', '31', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 2, 'PR', '#808080   ')    
                                          into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('31.05', 'Production Plan Report', 'Production Plan Report', '30', 
                                                '/Planning/Home/Setup#!Planning/FavProductionPlanReport', '/Planning/Home/Setup#!Planning/FavProductionPlanReport', 'fa fa-table', 'I', '31', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 3, 'PR', '#808080   ')        
                                     into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('31.06', 'Budget Plan Report', 'Budget Plan Report', '30', 
                                                '/Planning/Home/Setup#!Planning/FavBudgetPlanReport', '/Planning/Home/Setup#!Planning/FavBudgetPlanReport', 'fa fa-briefcase', 'I', '31', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 5, 'BR', '#808080   ')        
                                       into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('31.07', 'Ledger Plan Report', 'Ledger Plan Report', '30', 
                                                '/Planning/Home/Setup#!Planning/FavLedgerPlanReport', '/Planning/Home/Setup#!Planning/FavLedgerPlanReport', 'fa fa-paste', 'I', '31', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 4, 'LR', '#808080   ')      
                                        into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('31.03', 'Sales Plan Report', 'Sales Plan Report', '30', 
                                                '/Planning/Home/Setup#!Planning/FavSalesPlanReport', '/Planning/Home/Setup#!Planning/FavSalesPlanReport', ' fa fa-map', 'I', '31', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 1, 'SR', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('31.09', 'Sales Plan Wise Monthly Report', 'Sales Plan Wise Monthly Report', '30', 
                                                '/Planning/Home/Setup#!Planning/SalesPlanWiseReport', '/Planning/Home/Setup#!Planning/SalesPlanWiseReport', ' fa fa-map', 'I', '31', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 1, 'SR', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('31.08', 'Master Plan Report', 'Master Plan Report', '30', 
                                                '/Planning/Home/Setup#!Planning/MasterPlanSetup', '/Planning/Home/Setup#!Planning/MasterPlanSetup', ' fa fa-map', 'I', '31', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 1, 'SR', '#808080   ')
                                        into WEB_MENU_MANAGEMENT
                                               (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE,DASHBOARD_FLAG, ORDERBY, MODULE_ABBR, COLOR)
                                             Values
                                               ('34.05', 'Employee Handover', 'Employee Handover', '30', 
                                                '/Planning/Setup/EmployeeHandover', '/Planning/Setup/EmployeeHandover', ' fa fa-map', 'I', '34', 
                                                '01', '01', TO_DATE('06/02/2017 17:06:52', 'MM/DD/YYYY HH24:MI:SS'),'Y', 1, 'SR', '#808080   ')
                                           select 1 from dual";

                var RowNum = _objectEntity.ExecuteSqlCommand(Query);
                return RowNum;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}