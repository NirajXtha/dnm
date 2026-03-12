using NeoErp.Core;
using NeoErp.Data;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Repository
{
    public class BrandingSetupPlanRepo: IBrandingSetupPlanRepo
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public BrandingSetupPlanRepo(IDbContext dbContext, IWorkContext workContext)
        {
            this._workcontext = workContext;
            this._dbContext = dbContext;
        }
        public List<ItemModel> getItem()
        {
            try
            {
                var sqlquery = $@"SELECT LEVEL, 
                         INITCAP(ACTIVITY_EDESC) as ITEM_EDESC ,
                         ACTIVITY_CODE ITEM_CODE ,
                         MASTER_ACTIVITY_CODE as MASTER_ITEM_CODE, 
                         PARENT_ACTIVITY_CODE as PRE_ITEM_CODE , 
                         GROUP_ACTIVITY_FLAG as GROUP_SKU_FLAG 
                         FROM BRD_ACTIVITY ims
                         --WHERE ims.DELETED_FLAG = 'N'                  
                         --AND GROUP_ACTIVITY_FLAG = 'Y'
                         --AND LEVEL = 1
                         START WITH PRE_ITEM_CODE = '0'
                         CONNECT BY PRIOR MASTER_ACTIVITY_CODE = PARENT_ACTIVITY_CODE";

               var result = _dbContext.SqlQuery<ItemModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ItemModel> getItemByCode(string planCode, string itemCode)
        {
            var sqlquery = $@"SELECT DISTINCT 
                        INITCAP(ACTIVITY_EDESC) AS ITEM_EDESC,
                        ACTIVITY_CODE ITEM_CODE ,
                        MASTER_ACTIVITY_CODE as MASTER_ITEM_CODE, 
                        PARENT_ACTIVITY_CODE as PRE_ITEM_CODE , 
                        GROUP_ACTIVITY_FLAG as GROUP_SKU_FLAG 
                        ,(SELECT IS_ALL_CHILD_SELECTED FROM PL_BRD_ACTIVIY_PLAN WHERE PLAN_CODE = '{planCode}' ) as IS_CHILD_SELECTED
                        FROM BRD_ACTIVITY
                        WHERE DELETED_FLAG = 'N' 
                        AND ACTIVITY_CODE in (SELECT ACTIVITY_CODE FROM PL_BRD_ACTIVIY_PLAN_DTL WHERE PLAN_CODE = '{planCode}' )
                        ORDER BY ACTIVITY_CODE";
            var result = _dbContext.SqlQuery<ItemModel>(sqlquery).ToList();
            return result;

        }
    }
}
