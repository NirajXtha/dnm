using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoERP.QCQAManagement
{
    public class QCQAManagementPlugin : BasePlugin
    {
        private NeoErpCoreEntity _objectEntity;
        public QCQAManagementPlugin(NeoErpCoreEntity objectEntity)
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
                var RowNum = 0;
                return RowNum;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}