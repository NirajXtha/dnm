using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NeoERP.QCQAManagement.Controllers.API
{

    public class ParameterSetupAPIController : ApiController
    {
        private const string parameterSetup = "Parameter Setup";
        private IParameterRepo _IParameterRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        public ParameterSetupAPIController(IParameterRepo IParameterRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._IParameterRepo = IParameterRepo;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        //[HttpGet]
        //public List<SubMenu> GetSubMenuList()
        //{
        //    List<SubMenu> tableLists = _IParameterRepo.GetSubMenuList();
        //    //var dataList = new Items();
        //    return tableLists;
        //}
        [HttpGet]
        public List<QCQASubMenu> GetQCQADetailByFormCode(string formCode, string docVer = "All")
        {
            var response = new List<QCQASubMenu>();
            var SubMenuDetailList = this._IParameterRepo.GetQCQADetailByFormCode(formCode, docVer);
            response = SubMenuDetailList;
            return response;
        }
        [HttpGet]
        public List<DynamicMenu> MasterItemList()
        {
            List<DynamicMenu> response = new List<DynamicMenu>();
            try
            {
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<DynamicMenu> GetDynamicMenu(int userId, int level, string modular_code)
        {
            var dynamicMenu = new List<DynamicMenu>();
            var query = $@"select menu_no,FULL_PATH,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_CODE
from WEB_MENU_MANAGEMENT where menu_no in (SELECT mc.MENU_NO from WEB_MENU_CONTROL mc 
INNER JOIN WEB_MENU_MANAGEMENT mm on mm.MENU_NO = mc.MENU_NO and mm.group_sku_flag='G'
 WHERE mc.USER_NO ='{userId}' and mm.module_code='{modular_code}' ) and module_code='{modular_code}' order by ORDERBY asc";
            //  string query = "SELECT mc.MENU_NO, mm.VIRTUAL_PATH, mm.MENU_EDESC,mm.GROUP_SKU_FLAG,  mm.ICON_PATH,mm.MODULE_CODE from WEB_MENU_CONTROL mc INNER JOIN WEB_MENU_MANAGEMENT mm on mm.MENU_NO = mc.MENU_NO WHERE mc.USER_NO = " + userId + " and mm.module_code='" + modular_code + "' order by mm.ORDERBY asc";
            try
            {
                dynamicMenu = _objectEntity.SqlQuery<DynamicMenu>(query).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            return dynamicMenu;
        }

        public List<Items> GetGroupMaterialLists()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                tableList = _IParameterRepo.GetGroupMaterialLists();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Items> GetChildItems(string masterItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                tableList = _IParameterRepo.GetChildItems(masterItemCode);
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Products> GetProductDetails(string masterItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Products> tableList = new List<Products>();
                tableList = _IParameterRepo.GetProductDetails(masterItemCode);
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Items> GetSpecDetailsByItemID(string itemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                tableList = _IParameterRepo.GetSpecDetailsByItemID(itemCode);
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public IHttpActionResult saveParameterSetup(Items itemList)
        {
            try
            {
                bool isPosted = _IParameterRepo.InsertParameterData(itemList);
                if (isPosted)
                {
                    return Ok(new { success = true, message = "Data saved successfully." });
                }
                else
                {
                    return Ok(new { success = true, message = "Failed to save." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}