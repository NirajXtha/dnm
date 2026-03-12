using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoERP.LOC
{
    public class LOCPlugin : BasePlugin
    {
        private NeoErpCoreEntity _objectEntity;
        public LOCPlugin(NeoErpCoreEntity objectEntity)
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
                string Query = $@" into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('15', 'Master Setup', 'Master Setup', '02', 
                                            '/Loc/Home/Dashboard#!Loc/LcSetup', '/Loc/Home/Dashboard#!Loc/LcSetup', 'fa fa-cog fa-fw', 'G', '00', 
                                            '01', '01',sysdate , 1, 'MS', '#808080   ','N')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('16', 'Purchase Order', 'Purchase Order', '02', 
                                            '/Loc/Home/Dashboard#!Loc/PurchaseOrder', '/Loc/Home/Dashboard#!Loc/PurchaseOrder', 'fa fa-list-alt', 'G', '00', 
                                            '01', '01', sysdate, 75, 'PO', '#808080   ','N')
                                         into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('17', 'Proforma Invoice', 'Proforma Invoice', '02', 
                                            '/Loc/Home/Dashboard#!Loc/ProformaInvoice', '/Loc/Home/Dashboard#!Loc/ProformaInvoice', 'fa fa-pencil-square-o', 'G', '00', 
                                            '01', '01',sysdate , 76, 'PI', '#808080   ','N')
                                             into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('18', 'LC', 'LC', '02', 
                                            '/Loc/Home/Dashboard#!Loc/LcEntry', '/Loc/Home/Dashboard#!Loc/LcEntry', 'fa fa-envelope', 'G', '00', 
                                            '01', '01',sysdate , 76, 'LC', '#808080   ','N')
                                             into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('19', 'Logistic Plan', 'Logistic Plan', '02', 
                                            '/Loc/Home/Dashboard#!Loc/LogisticPlan', '/Loc/Home/Dashboard#!Loc/LogisticPlan', 'fa fa-calendar-minus-o', 'G', '00', 
                                            '01', '01',sysdate , 76, 'LP', '#808080   ','N')
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('20', 'Commercial Invoice', 'Commercial Invoice', '02', 
                                            '/Loc/Home/Dashboard#!Loc/CommercialInvoice', '/Loc/Home/Dashboard#!Loc/CommercialInvoice', 'fa fa-envelope', 'G', '00', 
                                            '01', '01',sysdate , 76, 'CI', '#808080   ','N')
                                             into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                        Values
                                           ('21', 'Logistics', 'Logistics', '02', 
                                            '/Loc/Home/Dashboard#!Loc/Logistics', '/Loc/Home/Dashboard#!Loc/Logistics', 'fa fa-trello', 'G', '00', 
                                            '01', '01',sysdate , 76, 'LG', '#808080   ','N')
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('22', 'Good Receive Note', 'General Recieve Note', '02', 
                                            '/Loc/Home/Dashboard#!Loc/GRN', '/Loc/Home/Dashboard#!Loc/GRN', 'fa fa-sticky-note-o', 'G', '00', 
                                            '01', '01',sysdate , 76, 'GRN', '#808080   ','N')
                                             into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                            Values
                                           ('23', 'CI Payment Settlement', 'CI Payment Settlement', '02', 
                                            '/Loc/Home/Dashboard#!Loc/PaymentSettlement', '/Loc/Home/Dashboard#!Loc/PaymentSettlement', 'fa fa-money', 'G', '00', 
                                            '01', '01',sysdate , 76, 'PS', '#808080   ','N')
                                             into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('24', 'Shipment', 'Shipment', '02', 
                                            '/Loc/Home/Dashboard#!Loc/Shipment', '/Loc/Home/Dashboard#!Loc/Shipment', 'fa fa-ship', 'G', '00', 
                                            '01', '01',sysdate , 76, 'PR', '#808080   ','Y')
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('25', 'Reports', 'Reports', '02', 
                                            'javascript:;', 'javascript:;', 'fa fa-sellsy', 'G', '00', 
                                            '01', '01',sysdate , 1, 'NA', '#808080   ','N')
                                           into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('25.01', 'Pending LC', 'Pending LC', '02', 
                                            '/Loc/Home/Dashboard#!Loc/PendingLcReport', '/Loc/Home/Dashboard#!Loc/PendingLcReport', 'fa fa-sellsy', 'I', '25', 
                                            '01', '01',sysdate , 1, 'PL', '#808080   ','N')
                                          into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('25.02', 'Opened LC', 'Opened LC', '02', 
                                            '/Loc/Home/Dashboard#!Loc/OpenLcReport', '/Loc/Home/Dashboard#!Loc/OpenLcReport', 'fa fa-cogs', 'I', '25', 
                                            '01', '01',sysdate , 1, 'OL', '#808080   ','N')
                                            into WEB_MENU_MANAGEMENT
                                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                          ('25.03', 'Lc Status', 'Lc Status', '02', 
                                            '/Loc/Home/Dashboard#!Loc/LcStatusReport', '/Loc/Home/Dashboard#!Loc/LcStatusReport', 'fa fa-file-archive-o', 'I', '25', 
                                            '01', '01',sysdate , 1, 'LS', '#808080   ','N')
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('25.04', 'Reached Vehicle Movement', 'Vehicle Movement', '02', 
                                            '/Loc/Home/Dashboard#!Loc/VMovReport', '/Loc/Home/Dashboard#!Loc/VMovReport', 'fa fa-road', 'I', '25', 
                                            '01', '01',sysdate , 1, 'RM', '#808080   ','N')
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('25.05', 'UnReached Vehicle Movement', 'UnReached Vehicle Movement', '02', 
                                            '/Loc/Home/Dashboard#!Loc/URVMovReport', '/Loc/Home/Dashboard#!Loc/URVMovReport', 'fa fa-road', 'I', '25', 
                                            '01', '01',sysdate , 1, 'NM', '#808080   ','N')
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('25.06', 'Due Invoice', 'Due Invoice', '02', 
                                            '/Loc/Home/Dashboard#!Loc/PendingCommercialInvoiceReport', '/Loc/Home/Dashboard#!Loc/PendingCommercialInvoiceReport', 'fa fa-table', 'I', '25', 
                                            '01', '01',sysdate , 1, 'DI', '#808080   ','N')
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                         Values
                                           ('25.07', 'LC Product Wise', 'LC Product Wise', '02', 
                                            '/Loc/Home/Dashboard#!Loc/LcProductWiseReport', '/Loc/Home/Dashboard#!Loc/LcProductWiseReport', 'fa fa-shopping-cart', 'I', '25', 
                                            '01', '01',sysdate , 1, 'LP', '#808080   ','N')
                                             into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                            Values
                                           ('25.08', 'PO Pending', 'PO Pending', '02', 
                                            '/Loc/Home/Dashboard#!Loc/PoPendingReport', '/Loc/Home/Dashboard#!Loc/PoPendingReport', 'fa fa-archive', 'I', '25', 
                                            '01', '01',sysdate , 1, 'PP', '#808080   ','N')       
                                            
                                           into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                            Values
                                           ('25.09', 'LC Pending', 'LC Pending', '02', 
                                            '/Loc/Home/Dashboard#!Loc/LcPendingReport', '/Loc/Home/Dashboard#!Loc/LcPendingReport', 'fa fa-archive', 'I', '25', 
                                            '01', '01',sysdate , 1, 'PP', '#808080   ','N')
                                            
                                            into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                            Values
                                           ('25.10', 'CI Pending', 'CI Pending', '02', 
                                            '/Loc/Home/Dashboard#!Loc/PendingCIReport', '/Loc/Home/Dashboard#!Loc/PendingCIReport', 'fa fa-archive', 'I', '25', 
                                            '01', '01',sysdate , 1, 'PP', '#808080   ','N')

                                        into WEB_MENU_MANAGEMENT
                                           (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, CREATED_BY, CREATED_DATE, ORDERBY, MODULE_ABBR, COLOR,DELETED_FLAG)
                                            Values
                                           ('25.11', 'MIT', 'MIT', '02', 
                                            '/Loc/Home/Dashboard#!Loc/MITReport', '/Loc/Home/Dashboard#!Loc/MITReport', 'fa fa-archive', 'I', '25', 
                                            '01', '01',sysdate , 1, 'PP', '#808080   ','N')

                                   
                                            ";
                var RowNum = 0;
                if (!string.IsNullOrEmpty(Query))
                {
                    string insertallquery = "INSERT ALL " + Query + " SELECT * FROM DUAL";
                    RowNum = this._objectEntity.ExecuteSqlCommand(insertallquery);
                }
                return RowNum;


                //************New Query End Here


            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}