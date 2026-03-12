using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using NeoErp.Data;
using NeoErp.Sales.Modules.Services.Models.SalesDashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.SalesService
{
    public class ModuleService
    {
        public static string _descriptionFilePath = "D:/Subash/TFS/NeoProject/NeoErp.Web/Areas/NeoErp.sales.Module/Description.txt";
        IDbContext _dbContext;
        public ModuleService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public string SaveModuleDetail()
        {
            var en = PluginFileParser.ParsePluginDescriptionFile(_descriptionFilePath);
            string query = "INSERT INTO WEB_MODULE_SETUP (MODULE_CODE, MODULE_EDESC, MODULE_NDESC, SYSTEM_NAME, NAME_SPACE, ICON_PATH, COLOR, COMPANY_CODE, CREATED_BY) VALUES('" + en.ModuleCode + "','" + en.ModuleCode + "','" + en.ModuleCode + "','" + en.ModuleCode + "','" + en.ModuleCode + "','" + en.ModuleCode + "','" + en.ModuleCode + "','" + en.ModuleCode + "','" + en.ModuleCode + "')";
            //'", '" + en.Group + "',' " + en.FriendlyName + "',' " + en.FriendlyName + "', '" + en.FriendlyName + "', '" + en.IconPath + "', '" + en.FriendlyName + "',' " + en.FriendlyName + "', '" + 1 + "')";

            //, "I", "CREATED_BY", Convert.ToDateTime("10/10/2016"), "MODIFY_BY", Convert.ToDateTime("10/10/2016")
            var a = 0;
            try
            {
                //var sql = "INSERT INTO WEB_MODULE_SETUP (MODULE_CODE,COMPANY_CODE,CREATED_BY) VALUES('" + en.ModuleCode + "','" + en.ModuleCode + "','" + en.ModuleCode + "')";
                //a = _dbContext.ExecuteSqlCommand(sql);
                var sss = "select count(*) as MODULE_COUNT from WEB_MODULE_SETUP";
                // var asd = this._dbContext.ExecuteSqlCommand(sss);
                var asds = this._dbContext.SqlQuery<int>(sss).First();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            return a.ToString();
        }

        public int DeleteModuleByUninstall()
        {
            var en = PluginFileParser.ParsePluginDescriptionFile(_descriptionFilePath);
            string query = string.Format(@"DELETE FROM WEB_MODULE_SETUP WHERE MODULE_CODE = {0}", en.ModuleCode);
            return this._dbContext.ExecuteSqlCommand(query);
        }
    }
    public class count
    {
        public string MODULE_CODE { get; set; }
        public string MODULE_EDESC { get; set; }
        public int MODULE_COUNT { get; set; }

    }

}