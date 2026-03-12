using System;
using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoERP.QuotationManagement
{
    public class QuotationManagementPlugin: BasePlugin
    {
        private NeoErpCoreEntity _objectEntity;
        public QuotationManagementPlugin(NeoErpCoreEntity objectEntity)
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
            var queryControl = $@" delete from WEB_MENU_CONTROL where menu_no in (select  Menu_no from WEB_MENU_MANAGEMENT where module_code='{base.PluginDescriptor.ModuleCode}')";
            var rowNumber = _objectEntity.ExecuteSqlCommand(queryControl);
            var query = "delete from WEB_MENU_MANAGEMENT where MODULE_CODE='" + base.PluginDescriptor.ModuleCode + "' ";
            var RowNum = _objectEntity.ExecuteSqlCommand(query);
            base.Uninstall();
        }

        public int InsertMenu()
        {
            try
            {
                //**************New Query Start here**********************//
                //string Query = $@"WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.01', 'Account', 'Account', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/AccountSetup', '/DocumentTemplate/Home/Index#!DT/AccountSetup', 'fa fa-edit', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'ACC', '#808080   ')
                //                         into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.02', 'Item', 'Item', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/ItemSetup', '/DocumentTemplate/Home/Index#!DT/ItemSetup', 'fa fa-cubes', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'IS', '#808080   ')
                //                         into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.03', 'Customer', 'Customer', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/CustomerSetup', '/DocumentTemplate/Home/Index#!DT/CustomerSetup', 'fa fa-group', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CS', '#808080   ')
                //                        into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.04', 'Budget Center', 'Budget Center', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/BudgetCenterSetup', '/DocumentTemplate/Home/Index#!DT/BudgetCenterSetup', 'fa fa-folder', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'BS', '#808080   ')
                //                         into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.05', 'Supplier', 'Supplier', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/SupplierSetup', '/DocumentTemplate/Home/Index#!DT/SupplierSetup', 'fa fa-database', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'SS', '#808080   ')
                //                        into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.06', 'Area', 'Area', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/AreaSetup', '/DocumentTemplate/Home/Index#!DT/AreaSetup', 'fa fa-map', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'ARS', '#808080   ')
                //                        into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.07', 'Agent', 'Agent', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/AgentSetup', '/DocumentTemplate/Home/Index#!DT/AgentSetup', 'fa fa-male', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'AGS', '#808080   ')
                //                        into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.08', 'Location', 'Location', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/LocationSetup', '/DocumentTemplate/Home/Index#!DT/LocationSetup', 'fa fa-arrows', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'LS', '#808080   ')                   
                //                        into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.10', 'Resource', 'Resource', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/ResourceSetup', '/DocumentTemplate/Home/Index#!DT/ResourceSetup', 'fa fa-sitemap', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'RS', '#808080   ')
                //                        into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.11', 'Transporter', 'Transporter', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/TransporterSetup', '/DocumentTemplate/Home/Index#!DT/TransporterSetup', 'fa fa-automobile', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'TS', '#808080   ')
                //                        into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.12', 'Regional', 'Regional', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/RegionalSetup', '/DocumentTemplate/Home/Index#!DT/RegionalSetup', 'fa fa-object-group', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'RGS', '#808080   ')

                //                           into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.13', 'Branch', 'Branch', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/BranchSetup', '/DocumentTemplate/Home/Index#!DT/BranchSetup', 'fa fa-arrows', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'BS', '#808080   ')
                //                           into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.14', 'Company', 'Company', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/CompanySetup', '/DocumentTemplate/Home/Index#!DT/CompanySetup', 'fa fa-credit-card', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CMS', '#808080   ')
                //                            into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.15', 'Division', 'Division', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/DivisionSetup', '/DocumentTemplate/Home/Index#!DT/DivisionSetup', 'fa fa-database', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'DS', '#808080   ')
                //                            into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.16', 'Vehicle', 'Vehicle', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/VehicleSetup', '/DocumentTemplate/Home/Index#!DT/VehicleSetup', 'fa fa-truck', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'VS', '#808080   ')
                //                            into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.17', 'Vehicle Registration', 'Vehicle Registration', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/VehicleRegistration', '/DocumentTemplate/Home/Index#!DT/VehicleRegistration', 'fa fa-train', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'VRS', '#808080   ')
                //                            into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.18', 'Preference', 'Preference', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/PreferenceSetup', '/DocumentTemplate/Home/Index#!DT/PreferenceSetup', 'fa fa-gears', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'PS', '#808080   ')
                //                            into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.19', 'Order Dispatch', 'Order Dispatch', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/OrderDispatchmanagement', '/DocumentTemplate/Home/Index#!DT/OrderDispatchmanagement', 'fa fa-laptop', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'ODS', '#808080   ')
                //                             into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                           ('05.20', 'Price', 'Price', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/PriceSetup', '/DocumentTemplate/Home/Index#!DT/PriceSetup', 'fa fa-keyboard-o', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'PCS', '#808080   ')
                //                            into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.21', 'Currency', 'Currency', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/CurrencySetup', '/DocumentTemplate/Home/Index#!DT/CurrencySetup', 'fa fa-recycle', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CUR', '#808080   ')
                //                              into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.22', 'Category', 'Category', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/CategorySetup', '/DocumentTemplate/Home/Index#!DT/CategorySetup', 'fa fa-newspaper-o', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CAT', '#808080   ')
                //                               into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.23', 'Priority', 'Priority', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/PrioritySetup', '/DocumentTemplate/Home/Index#!DT/PrioritySetup', 'fa fa-tachometer', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'PRI', '#808080   ')
                //                              into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.24', 'issueType', 'issueType', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/issueTypeSetup', '/DocumentTemplate/Home/Index#!DT/issueTypeSetup', 'fa fa-bug', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'ISU', '#808080   ')
                //                               into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.25', 'city', 'city', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/citySetup', '/DocumentTemplate/Home/Index#!DT/citySetup', 'fa fa-cab', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CT', '#808080   ')
                //                               into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.26', 'unit Of Measurement', 'unit Of Measurement', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/unitOfMeasurement', '/DocumentTemplate/Home/Index#!DT/unitOfMeasurement', 'fa fa-balance-scale', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CT', '#808080   ')
                //                                 into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.27', 'charge Type Definition', 'charge Type Definition', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/chargeTypeDefinition', '/DocumentTemplate/Home/Index#!DT/chargeTypeDefinition', 'fa fa-money', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CT', '#808080   ')
                //                                into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.28', 'ThirdPartyPreference', 'ThirdPartyPreference', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/ThirdPartyPreferenceSetup', '/DocumentTemplate/Home/Index#!DT/ThirdPartyPreferenceSetup', 'fa fa-soccer-ball-o', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CT', '#808080   ')
                //                             into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.29', 'RejectableItem', 'RejectableItem', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/RejectableItemSetup', '/DocumentTemplate/Home/Index#!DT/RejectableItemSetup', 'fa fa-flag', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CT', '#808080   ')
                //              into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.30', 'Party Type', 'Party Type', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/PartyTypeSetup', '/DocumentTemplate/Home/Index#!DT/PartyTypeSetup', 'fa fa-flag', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CT', '#808080   ')
                //           into WEB_MENU_MANAGEMENT
                //                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR)
                //                         Values
                //                             ('05.31', 'Dealer', 'Dealer', '05', 
                //                            '/DocumentTemplate/Home/Index#!DT/DealerSetup', '/DocumentTemplate/Home/Index#!DT/DealerSetup', 'fa fa-flag', 'I', '00', 
                //                            '01', '01',sysdate , 1, 'CT', '#808080   ')
                //                           ";




                var RowNum = 0;
                //if (!string.IsNullOrEmpty(Query))
                //{
                //    string insertallquery = "INSERT ALL " + Query + " SELECT * FROM DUAL";
                //    RowNum = this._objectEntity.ExecuteSqlCommand(insertallquery);
                //}
                return RowNum;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}