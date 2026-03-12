using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Transaction.Service.Models;

namespace NeoErp.Transaction.Service.Services
{
    public class TargetServiceRepositoy : ITargetServiceRepository
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;

        public TargetServiceRepositoy(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        public List<Target> GetAllTargets()
        {
            List<Target> targetList = new List<Target>();            
            string query = @"SELECT TO_CHAR (target_code) target_code,
                   fts.NAME,
                   fts.YEAR_NAME,
                   fts.CALENDAR_TYPE,
                   fts.RANGE_TYPE,
                   TO_CHAR (fts.START_DATE, 'yyyy-mm-dd') start_date,
                   TO_CHAR (fts.END_DATE, 'yyyy-mm-dd') end_date,
                   fts.ITEM_CODE,
                   (select ims.ITEM_EDESC from ip_item_master_setup ims where fts.item_code=ims.item_code) Item_Name,
                   fts.CATEGORY_CODE,
                   (select cc.CATEGORY_EDESC  from IP_CATEGORY_CODE cc where fts.Category_code = cc.Category_code) Category_Name,
                   fts.CUSTOMER_CODE,
                   (select cs.customer_edesc from sa_customer_setup cs where FTS.CUSTOMER_CODE=cs.customer_code) Customer_Name,
                   fts.EMPLOYEE_CODE,
                   (select es.employee_edesc from hr_employee_setup es where FTS.employee_code=es.employee_code) Employee_Name,
                   TO_CHAR (NVL (fts.SALES_TARGET, 0)) sales_target,
                   TO_CHAR (NVL (fts.COLLECTION_TARGET, 0)) collection_target,
                   fts.REMARKS,
                   fts.COMPANY_CODE,
                   fts.BRANCH_CODE,
                   fts.CREATED_BY,
                   TO_CHAR (fts.CREATED_DATE, 'yyyy-mm-dd') created_date,
                   fts.LAST_MODIFIED_BY
              FROM fa_target_setup fts WHERE fts.DELETED_FLAG='N'";
            targetList = this._dbContext.SqlQuery<Target>(query).ToList();
            return targetList;
        }

        public void AddNewTarget(Target target)
        {
            try
            {
                string insertQuery = string.Format(@"INSERT INTO FA_TARGET_SETUP(
                TARGET_CODE
                ,NAME
                ,YEAR_NAME
                ,CALENDAR_TYPE
                ,RANGE_TYPE
                ,START_DATE
                ,END_DATE
                ,ITEM_CODE
                ,CATEGORY_CODE
                ,CUSTOMER_CODE
                ,EMPLOYEE_CODE
                ,SALES_TARGET
                ,COLLECTION_TARGET
                ,REMARKS
                ,COMPANY_CODE
                ,BRANCH_CODE
                ,CREATED_BY
                ,CREATED_DATE
                ,DELETED_FLAG) VALUES(SEQ_FA_TARGET_SETUP.NEXTVAL,'{0}','{1}','{2}','{3}',to_date('{4}','yyyy-MM-dd'),to_date('{5}','yyyy-MM-dd'),'{6}','{7}','{8}','{9}','{10}','{11}'
                ,'{12}','{13}','{14}','{15}',to_date('{16}','yyyy-MM-dd'),'{17}')",
                target.NAME, target.YEAR_NAME, target.CALENDAR_TYPE, target.RANGE_TYPE, target.START_DATE, target.END_DATE, target.ITEM_CODE,
                target.CATEGORY_CODE, target.CUSTOMER_CODE, target.EMPLOYEE_CODE, target.SALES_TARGET, target.COLLECTION_TARGET
                , target.REMARKS, _workContext.CurrentUserinformation.company_code, target.BRANCH_CODE,
                _workContext.CurrentUserinformation.UserName, DateTime.Now.ToString("yyyy-MM-dd"), 'N');

                var result = this._dbContext.SqlQuery(insertQuery);
                this._dbContext.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            

        }

        public void UpdateTarget(Target target)
        {
            try
            {
                string updateQuery = string.Format(@"UPDATE FA_TARGET_SETUP                
                set NAME ='{0}' 
                 , YEAR_NAME='{1}'
                 , CALENDAR_TYPE='{2}'
                 , RANGE_TYPE='{3}'
                 , START_DATE=to_date('{4}','yyyy-MM-dd')
                 , END_DATE=to_date('{5}','yyyy-MM-dd')
                 , ITEM_CODE='{6}'
                 , CATEGORY_CODE='{7}'
                 , CUSTOMER_CODE='{8}'
                 , EMPLOYEE_CODE='{9}'
                 , SALES_TARGET='{10}'
                 , COLLECTION_TARGET='{11}'
                 , REMARKS='{12}'
                 , COMPANY_CODE='{13}'
                 , BRANCH_CODE='{14}'
                 , DELETED_FLAG='{17}' WHERE TARGET_CODE='{18}'",
                target.NAME, target.YEAR_NAME, target.CALENDAR_TYPE, target.RANGE_TYPE, target.START_DATE, target.END_DATE, target.ITEM_CODE,
                target.CATEGORY_CODE, target.CUSTOMER_CODE, target.EMPLOYEE_CODE, target.SALES_TARGET, target.COLLECTION_TARGET
                , target.REMARKS, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code,
                _workContext.CurrentUserinformation.UserName, DateTime.Now.ToString("yyyy-MM-dd"), 'N',target.TARGET_CODE);

                var result = this._dbContext.SqlQuery(updateQuery);
                this._dbContext.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteTarget(Target target)
        {
            try
            {
                string deleteQuery = string.Format(@"UPDATE FA_TARGET_SETUP SET DELETED_FLAG='Y' WHERE TARGET_CODE='{0}'", target.TARGET_CODE);
                this._dbContext.SqlQuery(deleteQuery);
                this._dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
