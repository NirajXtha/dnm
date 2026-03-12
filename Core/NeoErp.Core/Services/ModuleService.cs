using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using NeoErp.Core.Repository;
using NeoErp.Data;
using System;

namespace NeoErp.Core.Services
{
    public class ModuleService : IModuleService
    {
      
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public ModuleService(IDbContext dbContext,NeoErpCoreEntity objEntity)
        {
            _dbContext = dbContext;
            _objectEntity = objEntity;
        }

        /// <summary>
        /// <Author>Subash</Author>
        /// Insert Module data into WEB_MODULE_SETUP. And it takes parameter from DescriptionFile.
        /// </summary>
        /// <returns></returns>
        public string SaveModuleDetail(PluginDescriptor pluginDescription)
        {
            var en = pluginDescription;
            DateTime systemDate = DateTime.Today;
            var a = 0;
            try
            {
                var sql = "INSERT INTO WEB_MODULE_SETUP (MODULE_CODE, MODULE_EDESC, SYSTEM_NAME, NAME_SPACE, ICON_PATH, COLOR, COMPANY_CODE, CREATED_BY, MODIFY_BY) VALUES('" + en.ModuleCode + "','" + en.FriendlyName + "','" + en.SystemName + "','" + en.FileName + "','" + en.IconPath + "','" + en.SystemName + "','" + en.Group + "','" + en.ModuleCode + "','" + en.ModuleCode + "')";
                a = this._dbContext.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally {
                
            }
            return a.ToString();
        }
        /// <summary>
        /// <Author>Subash</Author>
        /// Delete Module from WEB_MODULE_SETUP. Parameter takes from Description File "Description.txt"
        /// </summary>
        /// <returns></returns>
        public int DeleteModuleByUninstall(PluginDescriptor pluginDescription) {
            var en = pluginDescription;
            string query = "DELETE FROM WEB_MODULE_SETUP WHERE MODULE_CODE = '"+en.ModuleCode+"'";
            var a = 0;
            try
            {
               a =  this._dbContext.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {

            }
            return a;
        }
    }


}