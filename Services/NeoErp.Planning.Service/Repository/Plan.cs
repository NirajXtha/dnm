using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using NeoErp.Data;
using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Xml.XPath;

namespace NeoErp.Planning.Service.Repository
{
    public class Plan : IPlan
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        private NeoErpCoreEntity _objectEntity;
        public Plan(IDbContext dbContext, IWorkContext _iWorkContext, NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
            this._workcontext = _iWorkContext;
            this._dbContext = dbContext;
        }

        //public string generatePlan(PlanModels plandetail)
        //{
        //    if (plandetail.PLAN_CODE == 0 || !plandetail.PLAN_CODE.HasValue)
        //    {
        //        string message = "";
        //        string defaultVal = "00";
        //        var query = string.Format(@"SELECT START_DATE,END_DATE,ITEM_CODE,PLAN_TYPE FROM PL_PLAN where ITEM_CODE='" + plandetail.ITEM_CODE + "' and deleted_flag='N'");
        //        var queryresult = _dbContext.SqlQuery<PlanModels>(query).ToList();
        //        //foreach (var results in queryresult)
        //        //{
        //        //    if (plandetail.START_DATE > results.START_DATE || plandetail.END_DATE > results.END_DATE && plandetail.ITEM_CODE == results.ITEM_CODE && plandetail.PLAN_TYPE == results.PLAN_TYPE)
        //        //    {
        //        //        message = "validation";
        //        //        return message;
        //        //    }
        //        //}

        //        var sqlquery = string.Format(@"SELECT MASTER_ITEM_CODE AS ITEM_MASTER_CODE,PRE_ITEM_CODE AS ITEM_PRE_CODE FROM IP_ITEM_MASTER_SETUP where ITEM_CODE='" + plandetail.ITEM_CODE + "' and deleted_flag='N'");
        //        var result = _dbContext.SqlQuery<PlanModels>(sqlquery).FirstOrDefault();

        //        var nextValQuery = $@"SELECT PLAN_SEQ.nextval as PL_PLAN_NEXT_CODE FROM DUAL";
        //        var id = _dbContext.SqlQuery<planDetailModel>(nextValQuery).ToList().FirstOrDefault();

        //        var insertQuery = string.Format(@"INSERT INTO PL_PLAN(PLAN_CODE,PLAN_EDESC, PLAN_NDESC,START_DATE,END_DATE, ITEM_CODE,ITEM_PRE_CODE,ITEM_MASTER_CODE,PLAN_TYPE,PLAN_FOR,TIME_FRAME_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG, IS_ALL_CHILD_SELECTED)
        //        VALUES({0}, '{1}', '{2}',TO_DATE('{3}', 'mm/dd/yyyy'), TO_DATE('{4}', 'mm/dd/yyyy'),'{5}','{6}','{7}', '{8}','{9}','{10}','{11}','{12}', '{13}','{14}',TO_DATE('{15}', 'mm/dd/yyyy '),'{16}','{17}')",
        //        id.PL_PLAN_NEXT_CODE, plandetail.PLAN_EDESC, plandetail.PLAN_EDESC, plandetail.START_DATE.ToString("MM/dd/yyyy"), plandetail.END_DATE.ToString("MM/dd/yyyy"), defaultVal, defaultVal, defaultVal, plandetail.PLAN_TYPE, plandetail.PLAN_FOR, plandetail.TIME_FRAME_CODE, plandetail.REMARKS, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), "N", plandetail.IS_ITEMS_VISIBLE_ONLY);
        //        var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
        //        _dbContext.SaveChanges();
        //        message = id.PL_PLAN_NEXT_CODE.ToString();

        //        var itemCodeArr = plandetail.ITEM_CODE.Split(',');
        //        for (int i = 0; i < itemCodeArr.Length; i++)
        //        {
        //            if (string.IsNullOrEmpty(itemCodeArr[i]) || itemCodeArr[i]=="")
        //            {
        //                continue;
        //            }
        //            if (itemCodeArr[i].Contains("_X"))
        //            {
        //                var newItemCode = itemCodeArr[i].Split('_')[0];
        //                var getPreCodeQuery = string.Format(@"SELECT MASTER_ITEM_CODE AS ITEM_MASTER_CODE,PRE_ITEM_CODE AS ITEM_PRE_CODE FROM IP_ITEM_MASTER_SETUP where ITEM_CODE='" + newItemCode + "' and deleted_flag='N'");
        //                var getResult = _dbContext.SqlQuery<PlanModels>(getPreCodeQuery).FirstOrDefault();
        //                var insertiteminplan = $@"INSERT INTO PL_PLAN_ITEM_MAPPING(PLAN_CODE, ITEM_CODE, PRE_ITEM_CODE, MASTER_ITEM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE ,DELETED_FLAG)
        //                    VALUES({message}, '{newItemCode}','{getResult.ITEM_PRE_CODE}','{getResult.ITEM_MASTER_CODE}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}',
        //                '{_workcontext.CurrentUserinformation.User_id}',SYSDATE,'N')";
        //                _dbContext.ExecuteSqlCommand(insertiteminplan);
        //            }
        //            else
        //            {
        //                var allChildItemResult = $@"SELECT DISTINCT 
        //                INITCAP(ITEM_EDESC) AS ITEM_EDESC,
        //                ITEM_CODE ,
        //                MASTER_ITEM_CODE, 
        //                PRE_ITEM_CODE ,
        //                GROUP_SKU_FLAG 
        //                FROM IP_ITEM_MASTER_SETUP
        //                WHERE DELETED_FLAG = 'N' 
        //                START WITH (ITEM_CODE = '{itemCodeArr[i].ToString()}'  OR PRE_ITEM_CODE like (SELECT MASTER_ITEM_CODE FROM IP_ITEM_MASTER_SETUP
        //                 WHERE ITEM_CODE = '{itemCodeArr[i].ToString()}') || '%')
        //                CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
        //                ORDER BY PRE_ITEM_CODE";
        //                var itemResult = _dbContext.SqlQuery<ItemModel>(allChildItemResult).ToList();
        //                foreach (var item in itemResult)
        //                {
        //                    var insertiteminplan = $@"INSERT INTO PL_PLAN_ITEM_MAPPING(PLAN_CODE, ITEM_CODE, PRE_ITEM_CODE, MASTER_ITEM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE ,DELETED_FLAG)
        //                    VALUES({message}, '{item.ITEM_CODE}','{item.PRE_ITEM_CODE}','{item.MASTER_ITEM_CODE}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}',
        //                '{_workcontext.CurrentUserinformation.User_id}',SYSDATE,'N')";
        //                    _dbContext.ExecuteSqlCommand(insertiteminplan);
        //                }
        //            }

        //        }

        //        return message;
        //    }
        //    else
        //    {
        //        string message = "";
        //        var query = string.Format(@"SELECT PLAN_CODE,START_DATE,END_DATE,ITEM_CODE,PLAN_TYPE FROM PL_PLAN where ITEM_CODE= '" + plandetail.ITEM_CODE + "' and PLAN_CODE NOT IN (SELECT PLAN_CODE FROM PL_PLAN WHERE PLAN_CODE = '" + plandetail.PLAN_CODE + "') AND deleted_flag='N'");
        //        var queryresult = _dbContext.SqlQuery<PlanModels>(query).ToList();
        //        foreach (var results in queryresult)
        //        {
        //            if (plandetail.START_DATE > results.START_DATE)
        //            {
        //                if (plandetail.END_DATE < results.END_DATE || plandetail.ITEM_CODE == results.ITEM_CODE && plandetail.PLAN_TYPE == results.PLAN_TYPE)
        //                {
        //                    message = "validation";
        //                    return message;
        //                }
        //            }
        //        }
        //        var sqlquery = string.Format(@"SELECT MASTER_ITEM_CODE AS ITEM_MASTER_CODE,PRE_ITEM_CODE AS ITEM_PRE_CODE FROM IP_ITEM_MASTER_SETUP where ITEM_CODE='" + plandetail.ITEM_CODE + "' and deleted_flag='N'");
        //        var result = _dbContext.SqlQuery<PlanModels>(sqlquery).FirstOrDefault();
        //        var insertQuery = string.Format(@"UPDATE PL_PLAN SET PLAN_EDESC='{0}',PLAN_NDESC='{1}',START_DATE=TO_DATE('{2}', 'mm/dd/yyyy'),END_DATE=TO_DATE('{3}', 'mm/dd/yyyy'), ITEM_CODE='{4}',ITEM_PRE_CODE='{5}',ITEM_MASTER_CODE='{6}',PLAN_TYPE='{7}',PLAN_FOR='{8}',TIME_FRAME_CODE='{9}',REMARKS='{10}',COMPANY_CODE='{11}',BRANCH_CODE='{12}',LAST_MODIFIED_BY='{13}',LAST_MODIFIED_DATE=TO_DATE('{14}', 'mm/dd/yyyy ') WHERE PLAN_CODE='{15}'", plandetail.PLAN_EDESC, plandetail.PLAN_EDESC, plandetail.START_DATE.ToString("MM/dd/yyyy"), plandetail.END_DATE.ToString("MM/dd/yyyy"), plandetail.ITEM_CODE, result.ITEM_PRE_CODE, result.ITEM_MASTER_CODE, plandetail.PLAN_TYPE, plandetail.PLAN_FOR, plandetail.TIME_FRAME_CODE, plandetail.REMARKS, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), plandetail.PLAN_CODE);
        //        var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
        //        _dbContext.SaveChanges();
        //        message = plandetail.PLAN_CODE.ToString();
        //        message = "Success";
        //        return message;
        //    }

        //}
        public string generatePlan(PlanModels plandetail)
        {
            if (plandetail.PLAN_CODE == 0 || !plandetail.PLAN_CODE.HasValue)
            {
                string message = "";
                string defaultVal = "00";
                var query = string.Format(@"SELECT START_DATE,END_DATE,ITEM_CODE,PLAN_TYPE FROM PL_PLAN where ITEM_CODE='" + plandetail.ITEM_CODE + "' and deleted_flag='N'");
                var queryresult = _dbContext.SqlQuery<PlanModels>(query).ToList();
                //foreach (var results in queryresult)
                //{
                //    if (plandetail.START_DATE > results.START_DATE || plandetail.END_DATE > results.END_DATE && plandetail.ITEM_CODE == results.ITEM_CODE && plandetail.PLAN_TYPE == results.PLAN_TYPE)
                //    {
                //        message = "validation";
                //        return message;
                //    }
                //}

                var sqlquery = string.Format(@"SELECT MASTER_ITEM_CODE AS ITEM_MASTER_CODE,PRE_ITEM_CODE AS ITEM_PRE_CODE FROM IP_ITEM_MASTER_SETUP where ITEM_CODE='" + plandetail.ITEM_CODE + "' and deleted_flag='N'");
                var result = _dbContext.SqlQuery<PlanModels>(sqlquery).FirstOrDefault();

                var nextValQuery = $@"SELECT PLAN_SEQ.nextval as PL_PLAN_NEXT_CODE FROM DUAL";
                var id = _dbContext.SqlQuery<planDetailModel>(nextValQuery).ToList().FirstOrDefault();

                var insertQuery = string.Format(@"INSERT INTO PL_PLAN(PLAN_CODE,PLAN_EDESC, PLAN_NDESC,START_DATE,END_DATE, ITEM_CODE,ITEM_PRE_CODE,ITEM_MASTER_CODE,PLAN_TYPE,PLAN_FOR,TIME_FRAME_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG, IS_ALL_CHILD_SELECTED)
                VALUES({0}, '{1}', '{2}',TO_DATE('{3}', 'mm/dd/yyyy'), TO_DATE('{4}', 'mm/dd/yyyy'),'{5}','{6}','{7}', '{8}','{9}','{10}','{11}','{12}', '{13}','{14}',TO_DATE('{15}', 'mm/dd/yyyy '),'{16}','{17}')",
                id.PL_PLAN_NEXT_CODE, plandetail.PLAN_EDESC, plandetail.PLAN_EDESC, plandetail.START_DATE.ToString("MM/dd/yyyy"), plandetail.END_DATE.ToString("MM/dd/yyyy"), defaultVal, defaultVal, defaultVal, plandetail.PLAN_TYPE, plandetail.PLAN_FOR, plandetail.TIME_FRAME_CODE, plandetail.REMARKS, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), "N", plandetail.IS_ITEMS_VISIBLE_ONLY);
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
                message = id.PL_PLAN_NEXT_CODE.ToString();

                var itemCodeArr = plandetail.ITEM_CODE.Split(',');
                for (int i = 0; i < itemCodeArr.Length; i++)
                {
                    if (string.IsNullOrEmpty(itemCodeArr[i]) || itemCodeArr[i] == "")
                    {
                        continue;
                    }
                    if (itemCodeArr[i].Contains("_X"))
                    {
                        var newItemCode = itemCodeArr[i].Split('_')[0];
                        var getPreCodeQuery = string.Format(@"SELECT MASTER_ITEM_CODE AS ITEM_MASTER_CODE,PRE_ITEM_CODE AS ITEM_PRE_CODE FROM IP_ITEM_MASTER_SETUP where ITEM_CODE='" + newItemCode + "' and deleted_flag='N'");
                        var getResult = _dbContext.SqlQuery<PlanModels>(getPreCodeQuery).FirstOrDefault();
                        var insertiteminplan = $@"INSERT INTO PL_PLAN_ITEM_MAPPING(PLAN_CODE, ITEM_CODE, PRE_ITEM_CODE, MASTER_ITEM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE ,DELETED_FLAG)
                            VALUES({message}, '{newItemCode}','{getResult.ITEM_PRE_CODE}','{getResult.ITEM_MASTER_CODE}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}',
                        '{_workcontext.CurrentUserinformation.User_id}',SYSDATE,'N')";
                        _dbContext.ExecuteSqlCommand(insertiteminplan);
                    }
                    else
                    {
                        var allChildItemResult = $@"SELECT DISTINCT 
                        INITCAP(ITEM_EDESC) AS ITEM_EDESC,
                        ITEM_CODE ,
                        MASTER_ITEM_CODE, 
                        PRE_ITEM_CODE ,
                        GROUP_SKU_FLAG 
                        FROM IP_ITEM_MASTER_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        START WITH (ITEM_CODE = '{itemCodeArr[i].ToString()}'  OR PRE_ITEM_CODE like (SELECT MASTER_ITEM_CODE FROM IP_ITEM_MASTER_SETUP
                         WHERE ITEM_CODE = '{itemCodeArr[i].ToString()}') || '%')
                        CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
                        ORDER BY PRE_ITEM_CODE";
                        var itemResult = _dbContext.SqlQuery<ItemModel>(allChildItemResult).ToList();
                        foreach (var item in itemResult)
                        {
                            var insertiteminplan = $@"INSERT INTO PL_PLAN_ITEM_MAPPING(PLAN_CODE, ITEM_CODE, PRE_ITEM_CODE, MASTER_ITEM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE ,DELETED_FLAG)
                            VALUES({message}, '{item.ITEM_CODE}','{item.PRE_ITEM_CODE}','{item.MASTER_ITEM_CODE}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}',
                        '{_workcontext.CurrentUserinformation.User_id}',SYSDATE,'N')";
                            _dbContext.ExecuteSqlCommand(insertiteminplan);
                        }
                    }

                }

                return message;
            }
            else
            {
                string message = "";
                var query = string.Format(@"SELECT PLAN_CODE,START_DATE,END_DATE,ITEM_CODE,PLAN_TYPE FROM PL_PLAN where ITEM_CODE= '" + plandetail.ITEM_CODE + "' and PLAN_CODE NOT IN (SELECT PLAN_CODE FROM PL_PLAN WHERE PLAN_CODE = '" + plandetail.PLAN_CODE + "') AND deleted_flag='N'");
                var queryresult = _dbContext.SqlQuery<PlanModels>(query).ToList();
                foreach (var results in queryresult)
                {
                    if (plandetail.START_DATE > results.START_DATE)
                    {
                        if (plandetail.END_DATE < results.END_DATE || plandetail.ITEM_CODE == results.ITEM_CODE && plandetail.PLAN_TYPE == results.PLAN_TYPE)
                        {
                            message = "validation";
                            return message;
                        }
                    }
                }
                var sqlquery = string.Format(@"SELECT MASTER_ITEM_CODE AS ITEM_MASTER_CODE,PRE_ITEM_CODE AS ITEM_PRE_CODE FROM IP_ITEM_MASTER_SETUP where ITEM_CODE='" + plandetail.ITEM_CODE + "' and deleted_flag='N'");
                var result = _dbContext.SqlQuery<PlanModels>(sqlquery).FirstOrDefault();
                var insertQuery = string.Format(@"UPDATE PL_PLAN SET PLAN_EDESC='{0}',PLAN_NDESC='{1}',START_DATE=TO_DATE('{2}', 'mm/dd/yyyy'),END_DATE=TO_DATE('{3}', 'mm/dd/yyyy'), ITEM_CODE='{4}',ITEM_PRE_CODE='{5}',ITEM_MASTER_CODE='{6}',PLAN_TYPE='{7}',PLAN_FOR='{8}',TIME_FRAME_CODE='{9}',REMARKS='{10}',COMPANY_CODE='{11}',BRANCH_CODE='{12}',LAST_MODIFIED_BY='{13}',LAST_MODIFIED_DATE=TO_DATE('{14}', 'mm/dd/yyyy ') WHERE PLAN_CODE='{15}'", plandetail.PLAN_EDESC, plandetail.PLAN_EDESC, plandetail.START_DATE.ToString("MM/dd/yyyy"), plandetail.END_DATE.ToString("MM/dd/yyyy"), plandetail.ITEM_CODE, result.ITEM_PRE_CODE, result.ITEM_MASTER_CODE, plandetail.PLAN_TYPE, plandetail.PLAN_FOR, plandetail.TIME_FRAME_CODE, plandetail.REMARKS, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), plandetail.PLAN_CODE);
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
                message = plandetail.PLAN_CODE.ToString();
                message = "Success";
                return message;
            }

        }
        public List<PlanModels> getAllPlans()
        {
            //var sqlquery = $@"SELECT PP.PLAN_CODE,PP.PLAN_EDESC,PP.START_DATE,PP.END_DATE, PP.ITEM_CODE,
            //    '' as ITEM_EDESC,
            //    PP.PLAN_TYPE,PP.PLAN_FOR,PP.TIME_FRAME_CODE,PF.TIME_FRAME_EDESC,PP.REMARKS,
            //  CASE
            //    WHEN  (select TO_CHAR(COUNT(*)) COUNT from PL_SUBPLAN psp, PL_PLAN_DTL ppd where ppd.PLAN_DTL_CODE = psp.PLAN_DTL_CODE AND ppd.PLAN_CODE = PP.PLAN_CODE) >0 THEN 'SUB_PLAN' 
            //     WHEN  (select TO_CHAR(COUNT(*))COUNT from  PL_PLAN_DTL where PLAN_CODE =PP.PLAN_CODE) >0 THEN 'PLAN_DETAIL'
            //    ELSE 'PLAN' 
            //  END   as STATUS
            // FROM PL_PLAN PP 
            //  JOIN PL_TIME_FRAME PF ON PP.TIME_FRAME_CODE = PF.TIME_FRAME_CODE
            //  --JOIN IP_ITEM_MASTER_SETUP IM ON PP.ITEM_CODE = IM.ITEM_CODE
            //  where PP.deleted_flag='N'";
            var sqlquery = $@"SELECT PP.PLAN_CODE,PP.PLAN_EDESC,SP.SUBPLAN_EDESC,PP.START_DATE,PP.END_DATE,PP.ITEM_CODE,'' AS ITEM_EDESC,PP.PLAN_TYPE,PP.PLAN_FOR,
       PP.TIME_FRAME_CODE,PF.TIME_FRAME_EDESC,PP.REMARKS,
       CASE
          WHEN (SELECT TO_CHAR (COUNT (*)) COUNT
                  FROM PL_SUBPLAN PSP, PL_PLAN_DTL PPD
                 WHERE     PPD.PLAN_DTL_CODE = PSP.PLAN_DTL_CODE
                       AND PPD.PLAN_CODE = PP.PLAN_CODE) > 0
          THEN
             'SUB_PLAN'
          WHEN (SELECT TO_CHAR (COUNT (*)) COUNT
                  FROM PL_PLAN_DTL
                 WHERE PLAN_CODE = PP.PLAN_CODE) > 0
          THEN
             'PLAN_DETAIL'
          ELSE
             'PLAN'
       END
          AS STATUS
  FROM PL_PLAN PP 
  left join PL_TIME_FRAME PF on PP.TIME_FRAME_CODE = PF.TIME_FRAME_CODE
  left join PL_SUBPLAN SP on PP.PLAN_CODE = SP.PLAN_CODE
 WHERE PP.DELETED_FLAG = 'N'
 ORDER BY PLAN_CODE";
            var result = _dbContext.SqlQuery<PlanModels>(sqlquery).ToList();
            return result;
        }

        public List<FrequencyModels> getFrequencyByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = string.Format(@"SELECT TIME_FRAME_CODE,TIME_FRAME_EDESC FROM PL_TIME_FRAME
                            where deleted_flag='N' 
                            --and COMPANY_CODE = '{1}'
                            and (TIME_FRAME_CODE like '%{0}%' 
                            or upper(TIME_FRAME_EDESC) like '%{0}%')",
                            filter.ToUpperInvariant(), _workcontext.CurrentUserinformation.company_code);
                var result = _dbContext.SqlQuery<FrequencyModels>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public string AddUpdateFrequencies(FrequencyModels model)
        {
            string message = "";
            if (model.TIME_FRAME_CODE == 0)
            {
                var currentdate = DateTime.Now.ToString("MM/dd/yyyy");
                var currentUserId = _workcontext.CurrentUserinformation.User_id;
                var company_code = _workcontext.CurrentUserinformation.company_code;
                string notdeleted = "N";
                var checkifexists = string.Format(@"SELECT count(*)COUNT from PL_TIME_FRAME where TIME_FRAME_EDESC= '{0}'", model.TIME_FRAME_EDESC);
                var CountRow = _dbContext.SqlQuery<int>(checkifexists).First();
                if (CountRow > 0)
                {
                    string query = string.Format(@"UPDATE PL_TIME_FRAME SET TIME_FRAME_EDESC  = '{0}',TIME_FRAME_NDESC= '{1}',LAST_MODIFIED_BY = '{2}',LAST_MODIFIED_DATE = TO_DATE('{3}', 'mm/dd/yyyy'),deleted_flag='N' WHERE TIME_FRAME_EDESC='{4}'",
              model.TIME_FRAME_EDESC, model.TIME_FRAME_EDESC, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), model.TIME_FRAME_EDESC);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                    message = "Success";
                }
                else
                {
                    var insertQuery = string.Format(@"INSERT INTO PL_TIME_FRAME(TIME_FRAME_CODE,TIME_FRAME_EDESC,COMPANY_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG)VALUES({0},'{1}','{2}','{3}',TO_DATE('{4}','mm/dd/yyyy'),'{5}',TO_DATE('{6}','mm/dd/yyyy'),'{7}',TO_DATE('{8}','mm/dd/yyyy'),'{9}')", "PL_TIME_FRAME_SEQ.nextval", model.TIME_FRAME_EDESC, company_code, currentUserId, currentdate, currentUserId, currentdate, currentUserId, currentdate, notdeleted);
                    var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                    _dbContext.SaveChanges();
                    message = "Success";
                }

            }
            else
            {
                var checkifexists = string.Format(@"SELECT count(*)COUNT from PL_TIME_FRAME where TIME_FRAME_EDESC= '{0}'", model.TIME_FRAME_EDESC);
                var CountRow = _dbContext.SqlQuery<int>(checkifexists).First();
                if (CountRow > 0)
                {
                    message = "ExistsButDeleted";

                }
                else
                {
                    string query = string.Format(@"UPDATE PL_TIME_FRAME SET TIME_FRAME_EDESC  = '{0}',TIME_FRAME_NDESC= '{1}',LAST_MODIFIED_BY = '{2}',LAST_MODIFIED_DATE = TO_DATE('{3}', 'mm/dd/yyyy'),deleted_flag='N' WHERE TIME_FRAME_CODE IN ({4})",
                 model.TIME_FRAME_EDESC, model.TIME_FRAME_EDESC, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), model.TIME_FRAME_CODE);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                    message = "Success";
                }
            }

            return message;
        }

        public void deleteFrequency(int freqcode)
        {
            string query = string.Format(@"UPDATE PL_TIME_FRAME SET DELETED_FLAG  = '{0}' WHERE TIME_FRAME_CODE IN ({1})",
            'Y', freqcode);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public List<FrequencyModels> getAllFrequency()
        {
            var sqlquery = "select TIME_FRAME_CODE, TIME_FRAME_EDESC from PL_TIME_FRAME where deleted_flag='N'";
            var frequencies = _dbContext.SqlQuery<FrequencyModels>(sqlquery).ToList();
            return frequencies;
        }

        public bool checkifexists(FrequencyModels model)
        {
            var checkifexists = string.Format(@"SELECT count(*)COUNT from PL_TIME_FRAME where TIME_FRAME_EDESC= '{0}' AND DELETED_FLAG='N' ", model.TIME_FRAME_EDESC);
            var CountRow = _dbContext.SqlQuery<int>(checkifexists).First();
            if (CountRow > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<ProductSetupModel> GetProductsListWithChild(User userinfo)
        {
            string query = @"SELECT LEVEL, 
                 INITCAP(ITEM_EDESC) AS ItemName,
                 ITEM_CODE AS ItemCode,
                 MASTER_ITEM_CODE AS MasterItemCode, 
                 PRE_ITEM_CODE AS PreItemCode, 
                 GROUP_SKU_FLAG AS GroupFlag,
                (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
                GROUP_SKU_FLAG='G' AND COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
                 FROM IP_ITEM_MASTER_SETUP ims
                 WHERE ims.DELETED_FLAG = 'N' 
                 --AND LEVEL=1
                 AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
                 --AND GROUP_SKU_FLAG = 'G'
                 START WITH PRE_ITEM_CODE = '00'
                 CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }


        public List<PlanModels> GetPlanDetailByPlanCode(string planCode, User userinfo)
        {
            string query = $@"SELECT PLAN_EDESC, TIME_FRAME_CODE, START_DATE, END_DATE , BRANCH_CODE FROM PL_SALES_PLAN WHERE PLAN_CODE = '{planCode}' ";
            var productListNodes = _dbContext.SqlQuery<PlanModels>(query).ToList();
            return productListNodes;
        }
        public List<ProductSetupModel> GetProductListWithChildByPreCode(string preItemCode, User userinfo)
        {
            string query = $@"SELECT LEVEL, 
                            INITCAP(ITEM_EDESC) AS ItemName,
                            ITEM_CODE AS ItemCode,
                            MASTER_ITEM_CODE AS MasterItemCode, 
                            PRE_ITEM_CODE AS PreItemCode, 
                            GROUP_SKU_FLAG AS GroupFlag,
                            (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
                             COMPANY_CODE='{userinfo.company_code}' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
                            FROM IP_ITEM_MASTER_SETUP ims
                            WHERE ims.DELETED_FLAG = 'N' 
                            AND ims.COMPANY_CODE = '{userinfo.company_code}'
                            START WITH PRE_ITEM_CODE = '{preItemCode}'
                            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
                            ORDER SIBLINGS BY ITEM_EDESC";
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }
        //public List<ConsolidateTree> CompanyListAllNodes(User userinfo)
        //{

        //    String query = $@"SELECT CC.COMPANY_CODE BRANCH_CODE, INITCAP(CC.COMPANY_EDESC) AS BRANCH_EDESC,'00' PRE_BRANCH_CODE,CC.ABBR_CODE
        //                        FROM COMPANY_SETUP CC
        //                        WHERE  CC.DELETED_FLAG = 'N'
        //                            AND CC.COMPANY_CODE IN (SELECT COMPANY_CODE FROM SC_COMPANY_CONTROL WHERE ACCESS_FLAG='Y' AND USER_NO='{userinfo.User_id}')
        //                      UNION    
        //                      SELECT DISTINCT  CS.BRANCH_CODE, INITCAP(CS.BRANCH_EDESC) AS BRANCH_EDESC ,CS.PRE_BRANCH_CODE,CS.ABBR_CODE
        //                        FROM FA_BRANCH_SETUP CS   
        //                        WHERE  CS.DELETED_FLAG = 'N'  
        //                            AND CS.PRE_BRANCH_CODE <> '00'     
        //                            AND CS.BRANCH_CODE IN (SELECT BRANCH_CODE FROM SC_BRANCH_CONTROL WHERE ACCESS_FLAG='Y' AND USER_NO = '{userinfo.User_id}' )";
        //    var consolidateListNodes = _dbContext.SqlQuery<ConsolidateTree>(query).ToList();
        //    return consolidateListNodes;
        //}
        //public List<ConsolidateSetupModel> CompanyListAllNodes(User userinfo)
        //{

        //    String query = $@"SELECT CC.COMPANY_CODE BRANCH_CODE, INITCAP(CC.COMPANY_EDESC) AS BRANCH_EDESC,'00' PRE_BRANCH_CODE,CC.ABBR_CODE, 'G' as GroupFlag
        //                        FROM COMPANY_SETUP CC
        //                        WHERE  CC.DELETED_FLAG = 'N'
        //                            AND CC.COMPANY_CODE IN (SELECT COMPANY_CODE FROM SC_COMPANY_CONTROL WHERE ACCESS_FLAG='Y' AND USER_NO='{userinfo.User_id}')
        //                      UNION    
        //                      SELECT DISTINCT  CS.BRANCH_CODE, INITCAP(CS.BRANCH_EDESC) AS BRANCH_EDESC ,CS.PRE_BRANCH_CODE,CS.ABBR_CODE,'I' as GroupFlag
        //                        FROM FA_BRANCH_SETUP CS   
        //                        WHERE  CS.DELETED_FLAG = 'N'  
        //                            AND CS.PRE_BRANCH_CODE <> '00'     
        //                            AND CS.BRANCH_CODE IN (SELECT BRANCH_CODE FROM SC_BRANCH_CONTROL WHERE ACCESS_FLAG='Y' AND USER_NO = '{userinfo.User_id}' )";
        //    var consolidateListNodes = _dbContext.SqlQuery<ConsolidateSetupModel>(query).ToList();
        //    return consolidateListNodes;
        //}

        public List<ProductSetupModel> getAllProductsWithChildItem(string pageNameId)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string item_filter_condition = "";
            try
            {
                var url = HttpContext.Current.Request.Url.AbsoluteUri;

                string xmlpath = HttpContext.Current.Server.MapPath("~/Areas/NeoERP.Planning/ProductCondition.xml");
                var xml = XDocument.Load(xmlpath);
                var condition_query = from c in xml.Root.Descendants("Vendor")
                                      where (string)c.Attribute("ID") == pageNameId
                                      select c.Element("ConditionQuery").Value;
                var result = condition_query.FirstOrDefault();

                if (result != null)
                {
                    item_filter_condition = result;
                }
            }
            catch (Exception)
            {
                item_filter_condition = "";
            }

            string query = $@"SELECT Distinct LEVEL, 
                            INITCAP(ITEM_EDESC) AS ItemName,
                            ITEM_CODE AS ItemCode,
                            MASTER_ITEM_CODE AS MasterItemCode, 
                            PRE_ITEM_CODE AS PreItemCode, 
                            GROUP_SKU_FLAG AS GroupFlag,
                            PURCHASE_PRICE As Rate,
                            (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
                             COMPANY_CODE='{company_code}'  AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
                            FROM IP_ITEM_MASTER_SETUP ims
                            WHERE ims.DELETED_FLAG = 'N' {item_filter_condition}
                            AND ims.COMPANY_CODE = '{company_code}' 
                            --AND LEVEL = {0}
                            START WITH PRE_ITEM_CODE = '00' AND COMPANY_CODE='{company_code}' AND DELETED_FLAG='N'
                            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE AND COMPANY_CODE='{company_code}' AND DELETED_FLAG='N'
                            ORDER BY ItemName";
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }

        public List<ProductSetupModel> GetProductsListWithChild(string level, string masterProductCode, User userinfo)
        {
            string query = string.Format(@"SELECT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag,
            (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
             COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
            --AND LEVEL = {0}
            START WITH PRE_ITEM_CODE = '{1}'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
            ORDER SIBLINGS BY ITEM_EDESC", level.ToString(), masterProductCode.ToString());
            var productListNodes = _dbContext.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }

        public List<SalesPlan> getAllSalesPlans(ReportFiltersModel filters)
        {
            //string query = @"SELECT TO_CHAR(PLAN_CODE) PLAN_CODE,PLAN_EDESC,PLAN_NDESC,TO_CHAR(SALES_QUANTITY) 
            //SALES_QUANTITY,TO_CHAR(SALES_AMOUNT) SALES_AMOUNT,TO_CHAR(TIME_FRAME_CODE) TIME_FRAME_CODE,
            //CALENDAR_TYPE,TO_CHAR(START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(END_DATE,'DD-MON-YYYY') END_DATE,REMARKS 
            //FROM PL_SALES_PLAN WHERE DELETED_FLAG = 'N' ORDER BY PLAN_CODE DESC";
            //string query = @"SELECT TO_CHAR(PLAN_CODE) PLAN_CODE,PLAN_EDESC,PLAN_NDESC,TO_CHAR(SALES_QUANTITY) 
            //SALES_QUANTITY,TO_CHAR(SALES_AMOUNT) SALES_AMOUNT,TO_CHAR(TIME_FRAME_CODE) TIME_FRAME_CODE,
            //CALENDAR_TYPE,TO_CHAR(START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(END_DATE,'DD-MON-YYYY') END_DATE,REMARKS,
            //(SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP
            //WHERE CUSTOMER_CODE IN (SELECT CUSTOMER_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PL_SALES_PLAN.PLAN_CODE AND ROWNUM=1)) CUSTOMER_EDESC 
            //FROM PL_SALES_PLAN WHERE DELETED_FLAG = 'N' ORDER BY TO_NUMBER(PLAN_CODE)  DESC";

            //string query = $@"SELECT TO_CHAR(PLAN_CODE) PLAN_CODE,PLAN_EDESC,PLAN_NDESC,
            //                  CASE WHEN SALES_QUANTITY IS NULL 
            //                                        THEN 'Amt'
            //                                    ELSE
            //                                       'Qty'
            //                                    END  SALES_TYPE,
            //                CASE WHEN SALES_QUANTITY IS NULL 
            //                            THEN  TO_CHAR(SALES_AMOUNT)
            //                        ELSE
            //                            TO_CHAR(SALES_QUANTITY)
            //                        END SALES_QUANTITY,
            //                TO_CHAR(TIME_FRAME_CODE) TIME_FRAME_CODE,
            //                CALENDAR_TYPE,TO_CHAR(START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(END_DATE,'DD-MON-YYYY') END_DATE,REMARKS,
            //                (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP
            //                WHERE CUSTOMER_CODE IN (SELECT CUSTOMER_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PL_SALES_PLAN.PLAN_CODE AND ROWNUM=1)) CUSTOMER_EDESC ,
            //                 (SELECT DIVISION_EDESC FROM FA_DIVISION_SETUP
            //                WHERE DIVISION_CODE IN (SELECT DIVISION_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PL_SALES_PLAN.PLAN_CODE AND ROWNUM=1)) DIVISION_EDESC ,
            //                (SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP
            //                WHERE EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PL_SALES_PLAN.PLAN_CODE AND ROWNUM=1)) EMPLOYEE_EDESC ,
            //                (SELECT BRANCH_EDESC FROM FA_BRANCH_SETUP
            //                WHERE BRANCH_CODE IN (SELECT BRANCH_CODE FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE=PL_SALES_PLAN.PLAN_CODE AND ROWNUM=1)) BRANCH_EDESC 
            //                FROM PL_SALES_PLAN WHERE DELETED_FLAG = 'N' ORDER BY TO_NUMBER(PLAN_CODE)  DESC";

            //CASE 
            //   WHEN ('{userId}'=PL.APPROVED_BY) THEN 'N' 
            //  ELSE
            //       CASE
            //        WHEN PL.APPROVED_BY IS NULL THEN 'Y' ELSE
            //           CASE 
            //               WHEN (SELECT PARENT_EMPLOYEE_CODE FROM HR_EMPLOYEE_TREE WHERE EMPLOYEE_CODE=(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND  USER_NO='{userId}')) IS NOT NULL THEN 'N' 
            //               ELSE 'Y'
            //           END
            //       END
            // END SUPER_FLAG

            var userId = _workcontext.CurrentUserinformation.User_id;
            var company_code = _workcontext.CurrentUserinformation.company_code;

            var superFlagQuery = $@"SELECT SUPER_USER_FLAG FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE = '{company_code}'";
            var superFlag = _dbContext.SqlQuery<string>(superFlagQuery).FirstOrDefault();
            var loginEmpCode = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND USER_NO='{userId}' AND COMPANY_CODE='{company_code}'";
            var loginEmpList = _dbContext.SqlQuery<EmployeeModel>(loginEmpCode).ToList();
            string query = $@"SELECT DISTINCT  TO_CHAR(PL.PLAN_CODE) PLAN_CODE, PL.PLAN_EDESC, PL.PLAN_NDESC,
                              CASE WHEN PL.SALES_QUANTITY IS NULL 
                                                    THEN 'Amt'
                                                ELSE
                                                   'Qty'
                                                END  SALES_TYPE,
                            CASE WHEN PL.SALES_QUANTITY IS NULL 
                                        THEN  TO_CHAR(PL.SALES_AMOUNT)
                                    ELSE
                                        TO_CHAR(PL.SALES_QUANTITY)
                                    END SALES_QUANTITY,
                            TO_CHAR(PL.TIME_FRAME_CODE) TIME_FRAME_CODE,
                            PL.CALENDAR_TYPE,TO_CHAR(PL.START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(PL.END_DATE,'DD-MON-YYYY') END_DATE,
                            PL.REMARKS, PLD.CUSTOMER_CODE, PLD.DIVISION_CODE, PLD.BRANCH_CODE, PLD.EMPLOYEE_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC, ES.EMPLOYEE_EDESC,PT.PARTY_TYPE_CODE as partytypeCode, PT.PARTY_TYPE_EDESC,
                            FS.BRANCH_EDESC, PL.APPROVED_FLAG,PL.APPROVED_BY APPROVED_BY_CODE,(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO=PL.APPROVED_BY AND COMPANY_CODE='{company_code}' AND DELETED_FLAG='N') APPROVED_BY_EMP_CODE,
                           (SELECT LOGIN_EDESC FROM SC_APPLICATION_USERS WHERE USER_NO=PL.APPROVED_BY AND COMPANY_CODE='{company_code}' AND DELETED_FLAG='N')APPROVED_BY,
                               'Y' SUPER_FLAG
                                -- CASE  WHEN '{superFlag}'='Y' THEN 'Y'
                                     -- WHEN (SELECT PARENT_EMPLOYEE_CODE  FROM HR_EMPLOYEE_TREE WHERE PARENT_EMPLOYEE_CODE = PLD.EMPLOYEE_CODE AND ROWNUM = 1) IS NOT NULL THEN
                                      --  CASE  WHEN (SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}' AND USER_NO='{userId}' AND EMPLOYEE_CODE=PLD.EMPLOYEE_CODE) IS NOT NULL THEN  'N' ELSE'Y' END
                                --ELSE 'Y' END SUPER_FLAG
                             --CASE WHEN '{superFlag}'='Y' THEN 'Y'  WHEN (SELECT PARENT_EMPLOYEE_CODE  FROM HR_EMPLOYEE_TREE WHERE PARENT_EMPLOYEE_CODE = PLD.EMPLOYEE_CODE AND ROWNUM = 1) IS NOT NULL THEN 'N' ELSE 'Y' END SUPER_FLAG
                            FROM PL_SALES_PLAN PL, PL_SALES_PLAN_DTL PLD, SA_CUSTOMER_SETUP CS , FA_DIVISION_SETUP DS, HR_EMPLOYEE_SETUP ES, FA_BRANCH_SETUP FS, IP_PARTY_TYPE_CODE PT
                             WHERE PL.DELETED_FLAG = 'N' 
                             AND PL.PLAN_CODE = PLD.PLAN_CODE
                             AND PL.COMPANY_CODE=PLD.COMPANY_CODE
                             AND CS.CUSTOMER_CODE(+) = PLD.CUSTOMER_CODE
                             AND DS.DIVISION_CODE(+) = PLD.DIVISION_CODE
                             AND ES.EMPLOYEE_CODE (+)= PLD.EMPLOYEE_CODE
                             AND PT.PARTY_TYPE_CODE(+) = PLD.PARTY_TYPE_CODE
                             AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
                                 AND CS.COMPANY_CODE(+) =PLd.COMPANY_CODE
                             AND DS.COMPANY_CODE(+)=PLD.COMPANY_CODE
                            AND PLD.COMPANY_CODE=ES.COMPANY_CODE(+)
                           AND PLD.COMPANY_CODE=FS.COMPANY_CODE(+)
                            AND PLD.COMPANY_CODE=PT.COMPANY_CODE(+)
                             AND PL.COMPANY_CODE='{company_code}'
                             AND PL.START_DATE BETWEEN TO_DATE('{filters.FromDate}','YYYY-MON-DD') AND TO_DATE('{filters.ToDate}', 'YYYY-MON-DD')
                             AND PL.END_DATE BETWEEN TO_DATE('{filters.FromDate}','YYYY-MON-DD') AND TO_DATE('{filters.ToDate}', 'YYYY-MON-DD')";

            if (superFlag != "Y")
            {
                //query += $@" AND PLD.EMPLOYEE_CODE in (
                //            SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{company_code}'
                //              UNION ALL
                //             SELECT HES.EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP HES, HR_EMPLOYEE_TREE ET
                //                WHERE HES.EMPLOYEE_CODE = ET.EMPLOYEE_CODE 
                //                 --AND PARENT_EMPLOYEE_CODE=(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{company_code}')
                //                    START WITH PARENT_EMPLOYEE_CODE = (SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{company_code}')
                //              CONNECT BY PRIOR ET.EMPLOYEE_CODE  = PARENT_EMPLOYEE_CODE)";

                //added by chandra for filter plan based on employee tree
                query += $@" AND PLD.EMPLOYEE_CODE IN( SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}' AND COMPANY_CODE='{company_code}'
                     UNION ALL
                    SELECT EMPLOYEE_CODE FROM(
                    SELECT EMPLOYEE_CODE,COMPANY_CODE FROM HR_EMPLOYEE_TREE ET 
                    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                      START WITH PARENT_EMPLOYEE_CODE='{loginEmpList[0].EMPLOYEE_CODE}' )X WHERE X.COMPANY_CODE='{company_code}'
                )";
            }

            if (filters.CustomerFilter.Count > 0)
            {
                query += $@" AND PLD.CUSTOMER_CODE IN ('{string.Join("','", filters.CustomerFilter)}')";

            }
            if (filters.BranchFilter.Count > 0)
            {
                query += $@" AND PLD.BRANCH_CODE IN ('{string.Join("','", filters.BranchFilter)}')";

            }
            if (filters.DivisionFilter.Count > 0)
            {
                query += $@" AND PLD.DIVISION_CODE IN ('{string.Join("','", filters.DivisionFilter)}')";

            }
            if (filters.EmployeeFilter.Count > 0)
            {
                query += $@" AND PLD.EMPLOYEE_CODE IN ('{string.Join("','", filters.EmployeeFilter)}')";

            }

            query += $@" ORDER BY TO_NUMBER (PL.PLAN_CODE)  DESC";

            List<SalesPlan> spList = new List<SalesPlan>();
            spList = this._dbContext.SqlQuery<SalesPlan>(query).ToList();

            //COMMENTED BY CHANDRA BECAUSE BELOW CODE IS FOR LIST LOGINED USER PALN LIST WHICH IS ALEADY CHECKED IN ABOVE QUERY
            //if (spList.Count <= 0)
            //{
            //    string query1 = $@"SELECT DISTINCT  TO_CHAR(PL.PLAN_CODE) PLAN_CODE, PL.PLAN_EDESC, PL.PLAN_NDESC,
            //                  CASE WHEN PL.SALES_QUANTITY IS NULL 
            //                                        THEN 'Amt'
            //                                    ELSE
            //                                       'Qty'
            //                                    END  SALES_TYPE,
            //                CASE WHEN PL.SALES_QUANTITY IS NULL 
            //                            THEN  TO_CHAR(PL.SALES_AMOUNT)
            //                        ELSE
            //                            TO_CHAR(PL.SALES_QUANTITY)
            //                        END SALES_QUANTITY,
            //                TO_CHAR(PL.TIME_FRAME_CODE) TIME_FRAME_CODE,
            //                PL.CALENDAR_TYPE,TO_CHAR(PL.START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(PL.END_DATE,'DD-MON-YYYY') END_DATE,
            //                PL.REMARKS, PLD.CUSTOMER_CODE, PLD.DIVISION_CODE, PLD.BRANCH_CODE, PLD.EMPLOYEE_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC, ES.EMPLOYEE_EDESC,PT.PARTY_TYPE_CODE, PT.PARTY_TYPE_EDESC,
            //                FS.BRANCH_EDESC,  PL.APPROVED_FLAG,PL.APPROVED_BY APPROVED_BY_CODE,(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO=PL.APPROVED_BY AND COMPANY_CODE='{company_code}' AND DELETED_FLAG='N') APPROVED_BY_EMP_CODE,
            //                (SELECT LOGIN_EDESC FROM SC_APPLICATION_USERS WHERE USER_NO=PL.APPROVED_BY AND COMPANY_CODE='{company_code}' AND DELETED_FLAG='N')APPROVED_BY,
            //                'Y' SUPER_FLAG,--CASE WHEN '{superFlag}'='Y' THEN 'Y'  WHEN (SELECT PARENT_EMPLOYEE_CODE  FROM HR_EMPLOYEE_TREE WHERE PARENT_EMPLOYEE_CODE  = PLD.EMPLOYEE_CODE AND ROWNUM = 1) IS NOT NULL THEN 'N' ELSE 'Y' END SUPER_FLAG,
            //                PL.APPROVED_FLAG
            //                FROM PL_SALES_PLAN PL, PL_SALES_PLAN_DTL PLD, SA_CUSTOMER_SETUP CS , FA_DIVISION_SETUP DS, HR_EMPLOYEE_SETUP ES, FA_BRANCH_SETUP FS, IP_PARTY_TYPE_CODE PT
            //                 WHERE PL.DELETED_FLAG = 'N' 
            //                 AND PL.PLAN_CODE = PLD.PLAN_CODE
            //                AND PL.COMPANY_CODE=PLD.COMPANY_CODE
            //                 AND CS.CUSTOMER_CODE(+) = PLD.CUSTOMER_CODE
            //                 AND DS.DIVISION_CODE(+) = PLD.DIVISION_CODE
            //                 AND ES.EMPLOYEE_CODE (+)= PLD.EMPLOYEE_CODE
            //                 AND PT.PARTY_TYPE_CODE(+) = PLD.PARTY_TYPE_CODE
            //                 AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
            //                    AND CS.COMPANY_CODE(+) =PLd.COMPANY_CODE
            //                 AND DS.COMPANY_CODE(+)=PLD.COMPANY_CODE
            //                AND PLD.COMPANY_CODE=ES.COMPANY_CODE(+)
            //               AND PLD.COMPANY_CODE=FS.COMPANY_CODE(+)
            //                AND PLD.COMPANY_CODE=PT.COMPANY_CODE(+)
            //                 AND PL.COMPANY_CODE='{company_code}'
            //                 AND PL.START_DATE BETWEEN TO_DATE('{filters.FromDate}','YYYY-MON-DD') AND TO_DATE('{filters.ToDate}', 'YYYY-MON-DD')
            //                 AND PL.END_DATE BETWEEN TO_DATE('{filters.FromDate}','YYYY-MON-DD') AND TO_DATE('{filters.ToDate}', 'YYYY-MON-DD')
            //                 --AND PLD.EMPLOYEE_CODE IN(select EMPLOYEE_CODE  from hr_employee_setup where LOWER(employee_edesc) like '%{_workcontext.CurrentUserinformation.login_code}%' and company_code ='{company_code}')
            //                 AND PLD.EMPLOYEE_CODE in (SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userId}') 
            //                 ";

            //    if (filters.CustomerFilter.Count > 0)
            //    {
            //        query1 += $@" AND PLD.CUSTOMER_CODE IN ('{string.Join("','", filters.CustomerFilter)}')";

            //    }
            //    if (filters.BranchFilter.Count > 0)
            //    {
            //        query1 += $@" AND PLD.BRANCH_CODE IN ('{string.Join("','", filters.BranchFilter)}')";

            //    }
            //    if (filters.DivisionFilter.Count > 0)
            //    {
            //        query1 += $@" AND PLD.DIVISION_CODE IN ('{string.Join("','", filters.DivisionFilter)}')";
            //    }
            //    if (filters.EmployeeFilter.Count > 0)
            //    {
            //        query1 += $@" AND PLD.EMPLOYEE_CODE IN ('{string.Join("','", filters.EmployeeFilter)}')";

            //    }
            //    query1 += $@" ORDER BY TO_NUMBER (PL.PLAN_CODE)  DESC";
            //    spList = this._dbContext.SqlQuery<SalesPlan>(query1).ToList();
            //}
            
            foreach (var item in spList)
            {
                if (item.APPROVED_FLAG == "Y")
                {
                    var response = checkPermissionForApprove(item.APPROVED_BY_EMP_CODE, loginEmpList[0].EMPLOYEE_CODE);
                    if (response)
                        item.SUPER_FLAG = "N";
                }
                else if (item.APPROVED_FLAG != "Y")
                {
                    var planName = item.PLAN_EDESC;
                    if (loginEmpList[0].EMPLOYEE_CODE == item.EMPLOYEE_CODE)
                        item.SUPER_FLAG = "N";
                }
            }
            return spList;
        }

        public bool checkPermissionForApprove(string approve_employee_code, string loginEmpCode)
        {
            bool approveFlag = false;
            var query = $@" SELECT  PARENT_EMPLOYEE_CODE EMPLOYEE_CODE FROM HR_EMPLOYEE_TREE 
                            START WITH EMPLOYEE_CODE = '{loginEmpCode}'
                            CONNECT BY  EMPLOYEE_CODE = PRIOR PARENT_EMPLOYEE_CODE";
            var result = _dbContext.SqlQuery<EmployeeModel>(query).ToList();

            result = result.Where(a => a.EMPLOYEE_CODE == approve_employee_code).ToList();
            if (result.Count() > 0)
            {
                approveFlag = true;
            }
            return approveFlag;
        }

        public bool deleteSalesPlan(int planCode)
        {
            try
            {
                string deleteYes_salesPlan = $@"UPDATE PL_SALES_PLAN SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                string deleteYes_salesPlanDtl = $@"UPDATE PL_SALES_PLAN_DTL SET DELETED_FLAG='Y' WHERE PLAN_CODE='{planCode}'";
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlanDtl);
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlan);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool approvedSalesPlan(int planCode, bool isChecked)
        {
            var config = _workcontext.CurrentUserinformation;
            try
            {
                var approvedBy = "";
                var flag = "N";
                if (isChecked)
                {
                    flag = "Y";
                    approvedBy = config.User_id.ToString();
                }

                string deleteYes_salesPlan = $@"UPDATE PL_SALES_PLAN SET APPROVED_FLAG='{flag}', APPROVED_BY='{approvedBy}',APPROVED_DATE=SYSDATE WHERE PLAN_CODE='{planCode}'";
                string deleteYes_salesPlanDtl = $@"UPDATE PL_SALES_PLAN_DTL SET APPROVED_FLAG='{flag}', APPROVED_BY='{approvedBy}',APPROVED_DATE=SYSDATE WHERE PLAN_CODE='{planCode}'";
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlanDtl);
                this._dbContext.ExecuteSqlCommand(deleteYes_salesPlan);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool cloneSalesPlan(int planCode)
        {
            try
            {
                string existingPlanNameQuery = $@"SELECT PLAN_EDESC FROM PL_SALES_PLAN WHERE PLAN_CODE='{planCode}'";
                string existingPlanName = this._dbContext.SqlQuery<String>(existingPlanNameQuery).FirstOrDefault();
                if (existingPlanName != null)
                {
                    string copyPlanName = existingPlanName + "_copy";
                    string maxPlanCodeQuery = "SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_SALES_PLAN";
                    int maxPlanCode = this._dbContext.SqlQuery<int>(maxPlanCodeQuery).First();

                    string copySalesPlanQuery = $@"INSERT INTO PL_SALES_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG)
                    SELECT '{maxPlanCode}','{copyPlanName}', PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG FROM PL_SALES_PLAN WHERE PLAN_CODE='{planCode}'";
                    var insertCopiedPlan = this._dbContext.ExecuteSqlCommand(copySalesPlanQuery);

                    int copiedPlanCode = maxPlanCode;
                    //string getCopyPlanCodeQuery = $@"SELECT PLAN_CODE FROM PL_SALES_PLAN WHERE PLAN_EDESC='{copyPlanName}'";
                    //copiedPlanCode = this._dbContext.SqlQuery<int>(getCopyPlanCodeQuery).First();
                    if (copiedPlanCode > 0)
                    {
                        string copyinto_plandtl = $@"INSERT INTO PL_SALES_PLAN_DTL
                                (PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                                CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON)
                                SELECT '{copiedPlanCode}', PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                                CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE='{planCode}'";

                        var result = _dbContext.ExecuteSqlCommand(copyinto_plandtl).ToString();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool cloneSalesPlan(string planCode, string planName, string customers, string employees, string branchs, string divisions, string partyType, string remarks)
        {
            try
            {
                remarks = remarks.Replace("\"", " ");
                string existingPlanNameQuery = $@"SELECT PLAN_EDESC FROM PL_SALES_PLAN WHERE PLAN_CODE='{planCode}'";
                string existingPlanName = this._dbContext.SqlQuery<String>(existingPlanNameQuery).FirstOrDefault();
                if (existingPlanName != null)
                {
                    string copyPlanName = existingPlanName + "_copy";
                    if (!string.IsNullOrEmpty(planName))
                        copyPlanName = planName;
                    string maxPlanCodeQuery = "SELECT COALESCE(MAX(PLAN_CODE)+1, MAX(PLAN_CODE) + 1, 1) FROM PL_SALES_PLAN";
                    int maxPlanCode = this._dbContext.SqlQuery<int>(maxPlanCodeQuery).First();

                    string copySalesPlanQuery = $@"INSERT INTO PL_SALES_PLAN(PLAN_CODE,PLAN_EDESC,PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG)
                    SELECT '{maxPlanCode}','{copyPlanName}', PLAN_NDESC,SALES_QUANTITY,SALES_AMOUNT,TIME_FRAME_CODE,
                    CALENDAR_TYPE,START_DATE,END_DATE,'{remarks}',COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                    DELETED_FLAG FROM PL_SALES_PLAN WHERE PLAN_CODE='{planCode}'";
                    var insertCopiedPlan = this._dbContext.ExecuteSqlCommand(copySalesPlanQuery);

                    int copiedPlanCode = maxPlanCode;
                    if (copiedPlanCode > 0)
                    {
                        string copyinto_plandtl = $@"INSERT INTO PL_SALES_PLAN_DTL
                                (PLAN_CODE,PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                                CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,PARTY_TYPE_CODE,REMARKS,
                                COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON)
                                SELECT '{copiedPlanCode}', PLAN_DATE,PER_DAY_QUANTITY,PER_DAY_AMOUNT,ITEM_CODE,
                                '{customers}','{employees}','{divisions}','{partyType}',REMARKS,
                                COMPANY_CODE,'{branchs}',CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,DELETED_FLAG,FREQUENCY_JSON FROM PL_SALES_PLAN_DTL WHERE PLAN_CODE='{planCode}'";

                        var result = _dbContext.ExecuteSqlCommand(copyinto_plandtl).ToString();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateSalesPlan(SalesPlan sp)
        {
            try
            {
                if (string.IsNullOrEmpty(sp.PLAN_CODE))
                {
                    return false;
                }
                else
                {
                    string checkQuery = $@"SELECT COUNT(PLAN_EDESC) FROM PL_SALES_PLAN WHERE PLAN_CODE != '{sp.PLAN_CODE}' AND PLAN_EDESC='{sp.PLAN_EDESC}'";
                    int count = this._dbContext.SqlQuery<int>(checkQuery).First();
                    if (count == 0)
                    {
                        string updateQuery = $@"UPDATE PL_SALES_PLAN SET PLAN_EDESC = '{sp.PLAN_EDESC}' , REMARKS = '{sp.REMARKS}' WHERE PLAN_CODE='{sp.PLAN_CODE}'";
                        var result = this._dbContext.ExecuteSqlCommand(updateQuery);
                        if (sp.IsCustomerProduct == "customer_product")
                        {
                            string updateDtlQuery = $@"UPDATE PL_SALES_PLAN_DTL SET CUSTOMER_CODE='{sp.customerCode}' , EMPLOYEE_CODE='{sp.employeeCode}',
 DIVISION_CODE='{sp.divisionCode}', BRANCH_CODE='{sp.branchCode}' WHERE PLAN_CODE='{sp.PLAN_CODE}'";
                            var dtlUpdateResult = this._dbContext.ExecuteSqlCommand(updateDtlQuery);
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #region preference status
        public string GetDivisionFlag()
        {
            try
            {
                var company_code = _workcontext.CurrentUserinformation.company_code;
                var branch_code = _workcontext.CurrentUserinformation.branch_code;

                string divisionFlag = $@"SELECT DIVISION_FLAG FROM PREFERENCE_SUB_SETUP WHERE COMPANY_CODE = '{company_code}' AND BRANCH_CODE = '{branch_code}'";
                return this._dbContext.SqlQuery<String>(divisionFlag).FirstOrDefault();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public List<PreferenceSetupModel> GetPreferenceSetupFlag()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string Flag = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE COMPANY_CODE = '{company_code}' AND BRANCH_CODE='{branch_code}'";
            var result = this._dbContext.SqlQuery<PreferenceSetupModel>(Flag).ToList();
            return result;
        }
        public List<PreferenceSetupModel> GetSalesPlanSetupFlag()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string Flag = $@"SELECT * FROM PL_PREFERENCE_SETUP  WHERE PL_NAME='SALES_PLAN' AND COMPANY_CODE = '{company_code}' AND BRANCH_CODE='{branch_code}'";
            var result = this._dbContext.SqlQuery<PreferenceSetupModel>(Flag).ToList();
            return result;
        }

        public List<PreferenceSetupModel> GetProcuremntSetupFlag()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string Flag = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE PL_NAME='PROCUREMENT_PLAN' AND COMPANY_CODE = '{company_code}' AND BRANCH_CODE='{branch_code}'";
            var result = this._dbContext.SqlQuery<PreferenceSetupModel>(Flag).ToList();
            return result;
        }
        public List<PreferenceSetupModel> GeLedgerPlanSetupFlag()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string Flag = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE PL_NAME='BUDGET_PLAN' AND COMPANY_CODE = '{company_code}' AND BRANCH_CODE='{branch_code}'";
            var result = this._dbContext.SqlQuery<PreferenceSetupModel>(Flag).ToList();
            return result;
        }
        public List<PreferenceSetupModel> GetBudgetPlanSetupFlag()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string Flag = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE PL_NAME='LEDGER_PLAN' AND COMPANY_CODE = '{company_code}' AND BRANCH_CODE='{branch_code}'";
            var result = this._dbContext.SqlQuery<PreferenceSetupModel>(Flag).ToList();
            return result;
        }
        public List<PreferenceSetupModel> GetProductionPlanSetupFlag()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string Flag = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE PL_NAME='PRODUCTION_PLAN' AND COMPANY_CODE = '{company_code}' AND BRANCH_CODE='{branch_code}'";
            var result = this._dbContext.SqlQuery<PreferenceSetupModel>(Flag).ToList();
            return result;
        }
        public List<PreferenceSetupModel> GetCollectionSetupFlag()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string Flag = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE PL_NAME='COLLECTION_PLAN' AND COMPANY_CODE = '{company_code}' AND BRANCH_CODE='{branch_code}'";
            var result = this._dbContext.SqlQuery<PreferenceSetupModel>(Flag).ToList();
            return result;
        }
        public List<PreferenceSetupModel> GetBrandingSetupFlag()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var branch_code = _workcontext.CurrentUserinformation.branch_code;

            string Flag = $@"SELECT * FROM PL_PREFERENCE_SETUP WHERE PL_NAME='BRANDING_PLAN' AND COMPANY_CODE = '{company_code}' AND BRANCH_CODE='{branch_code}'";
            var result = this._dbContext.SqlQuery<PreferenceSetupModel>(Flag).ToList();
            return result;

        }


        #endregion
        public List<SalesPlan> getAllSalesPlans(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
        {
            var userno = this._workcontext.CurrentUserinformation.User_id;
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            string query = $@"SELECT DISTINCT TO_CHAR(PL.PLAN_CODE), TO_CHAR(PL.PLAN_CODE)||'_SP' PLAN_CODE, PL.PLAN_EDESC, PL.PLAN_NDESC,
                              CASE WHEN PL.SALES_QUANTITY IS NULL 
                                                    THEN 'Amt'
                                                ELSE
                                                   'Qty'
                                                END  SALES_TYPE,
                            CASE WHEN PL.SALES_QUANTITY IS NULL 
                                        THEN  TO_CHAR(PL.SALES_AMOUNT)
                                    ELSE
                                        TO_CHAR(PL.SALES_QUANTITY)
                                    END SALES_QUANTITY,
                            TO_CHAR(PL.TIME_FRAME_CODE) TIME_FRAME_CODE,
                            PL.CALENDAR_TYPE,TO_CHAR(PL.START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(PL.END_DATE,'DD-MON-YYYY') END_DATE,
                            PL.REMARKS, PLD.CUSTOMER_CODE, PLD.DIVISION_CODE, PLD.BRANCH_CODE, PLD.EMPLOYEE_CODE, CS.CUSTOMER_EDESC, DS.DIVISION_EDESC, ES.EMPLOYEE_EDESC,
                            FS.BRANCH_EDESC
                            FROM PL_SALES_PLAN PL, PL_SALES_PLAN_DTL PLD, SA_CUSTOMER_SETUP CS , FA_DIVISION_SETUP DS, HR_EMPLOYEE_SETUP ES, FA_BRANCH_SETUP FS
                             WHERE PL.DELETED_FLAG = 'N' 
                             AND PL.PLAN_CODE = PLD.PLAN_CODE
                            AND PL.COMPANY_CODE=PLD.COMPANY_CODE
                             AND CS.CUSTOMER_CODE(+) = PLD.CUSTOMER_CODE
                             AND DS.DIVISION_CODE(+) = PLD.DIVISION_CODE
                             AND ES.EMPLOYEE_CODE (+)= PLD.EMPLOYEE_CODE
                             AND FS.BRANCH_CODE (+)= PLD.BRANCH_CODE
                            AND CS.COMPANY_CODE(+) =PLd.COMPANY_CODE
                             AND DS.COMPANY_CODE(+)=PLD.COMPANY_CODE
                            AND PLD.COMPANY_CODE=ES.COMPANY_CODE(+)
                           AND PLD.COMPANY_CODE=FS.COMPANY_CODE(+)
                            AND PLD.EMPLOYEE_CODE in (
                             SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userno}' AND COMPANY_CODE='{company_code}'
                              UNION ALL
                             SELECT HES.EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP HES, HR_EMPLOYEE_TREE ET
                                WHERE HES.EMPLOYEE_CODE = ET.EMPLOYEE_CODE 
                                 --AND PARENT_EMPLOYEE_CODE=(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userno}' AND COMPANY_CODE='{company_code}')
                                    START WITH PARENT_EMPLOYEE_CODE =(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userno}' AND COMPANY_CODE='{company_code}')
                                CONNECT BY PRIOR ET.EMPLOYEE_CODE  = PARENT_EMPLOYEE_CODE)
                             ";

            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(enddate))
            {
                query += $@" AND PL.START_DATE BETWEEN TO_DATE('{startdate}','YYYY-MON-DD') AND TO_DATE('{enddate}', 'YYYY-MON-DD')
                             AND PL.END_DATE BETWEEN TO_DATE('{startdate}','YYYY-MON-DD') AND TO_DATE('{enddate}', 'YYYY-MON-DD')";
                //query += $@" AND PL.START_DATE = TO_DATE('{startdate}','YYYY-MON-DD')
                //             AND PL.END_DATE = TO_DATE('{enddate}','YYYY-MON-DD') ";
            }

            if (!string.IsNullOrEmpty(customercode))
            {
                query += $@" AND PLD.CUSTOMER_CODE IN ('{customercode}')";

            }
            if (!string.IsNullOrEmpty(branchcode))
            {
                query += $@" AND PLD.BRANCH_CODE IN ('{branchcode}')";

            }
            if (!string.IsNullOrEmpty(divisioncode))
            {
                query += $@" AND PLD.DIVISION_CODE IN ('{divisioncode}')";

            }
            if (!string.IsNullOrEmpty(employeecode))
            {
                query += $@" AND PLD.EMPLOYEE_CODE IN ('{employeecode}')";

            }

            query += $@" ORDER BY TO_NUMBER (PL.PLAN_CODE)  DESC";

            List<SalesPlan> spList = new List<SalesPlan>();
            spList = this._dbContext.SqlQuery<SalesPlan>(query).ToList();

            string masterplan_query = $@"SELECT DISTINCT TO_CHAR(MSP.MASTER_PLAN_CODE)||'_MP' PLAN_CODE,
                                        MSP.MASTER_PLAN_EDESC PLAN_EDESC ,
                                        TO_CHAR (MSP.START_DATE, 'DD-MON-YYYY') START_DATE,
                                             TO_CHAR (MSP.END_DATE, 'DD-MON-YYYY') END_DATE
                                    FROM PL_MASTER_SALES_PLAN MSP ,PL_SALES_PLAN_MAP SPM
                                    WHERE MSP.MASTER_PLAN_CODE=SPM.MASTER_PLAN_CODE ";
            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(enddate))
            {
                masterplan_query += $@" AND MSP.START_DATE = TO_DATE('{startdate}','YYYY-MON-DD')
                             AND MSP.END_DATE = TO_DATE('{enddate}','YYYY-MON-DD') ";
            }
            List<SalesPlan> mpList = new List<SalesPlan>();
            mpList = this._dbContext.SqlQuery<SalesPlan>(masterplan_query).ToList();
            if (mpList.Count > 0)
            {
                foreach (var item in mpList)
                {
                    spList.Add(new SalesPlan
                    {
                        PLAN_CODE = item.PLAN_CODE,
                        PLAN_EDESC = item.PLAN_EDESC,
                        START_DATE = item.START_DATE,
                        END_DATE = item.END_DATE
                    });
                }
            }
            return spList;
        }

        public string SaveMasterPlan(string masterPlanCode, string masterPlanName, string startDate, string endDate, string customer, string employee, string division, string branch, string selectedPlans)
        {
            try
            {
                selectedPlans = selectedPlans.Replace('[', ' ').Replace(']', ' ').Replace('\"', ' ');
                string[] plans = selectedPlans.Split(',').ToArray();
                string plancodes = string.Empty;
                foreach (var item in plans)
                {
                    plancodes += item.Trim() + ",";
                }
                plancodes = plancodes.Substring(0, plancodes.Length - 1);
                string queryCheckAlreadyExist = $@"select count(*) from pl_master_sales_plan sp,pl_sales_plan_map spm
                        where sp.start_date=to_date('{startDate}','YYYY-MON-DD')
                        and sp.end_date=to_date('{endDate}','YYYY-MON-DD')
                        and sp.master_plan_code=spm.master_plan_code
                        and spm.plan_code in ({plancodes.Replace("_SP", "").Replace("_MP", "")})";

                if (string.IsNullOrEmpty(masterPlanCode))
                {
                    string nameDuplicationQuery = $@"select count(*) from pl_master_sales_plan Where UPPER(TRIM(MASTER_PLAN_EDESC)) =UPPER(TRIM('{masterPlanName}')) ";
                    int nameDuplicationCount = this._dbContext.SqlQuery<int>(nameDuplicationQuery).First();
                    if (nameDuplicationCount > 0)
                    {
                        return "Same master plan has already used. Action not done.";
                    }
                }
                else
                {
                    string nameDuplicationQuery = $@"select count(*) from pl_master_sales_plan Where UPPER(TRIM(MASTER_PLAN_EDESC)) =UPPER(TRIM('{masterPlanName}')) AND MASTER_PLAN_CODE !='{masterPlanCode}' ";
                    int nameDuplicationCount = this._dbContext.SqlQuery<int>(nameDuplicationQuery).First();
                    if (nameDuplicationCount > 0)
                    {
                        return "Same master plan has already used. Action not done.";
                    }
                }

                //int countMasterPlan = this._dbContext.SqlQuery<int>(queryCheckAlreadyExist).First();
                //if (countMasterPlan > 0)
                //{
                //    return "already exist same master-plan setup.";
                //}

                if (string.IsNullOrEmpty(masterPlanCode))
                {
                    var user = this._workcontext.CurrentUserinformation.User_id;
                    var employeeid = GetEmployeeByID(user.ToString());
                    int maxMasterPlanCode = this._dbContext.SqlQuery<int>("SELECT COALESCE(MAX(MASTER_PLAN_CODE)+1, MAX(MASTER_PLAN_CODE) + 1, 1) FROM PL_MASTER_SALES_PLAN").First();

                    string insertQuery = $@" INTO PL_MASTER_SALES_PLAN(MASTER_PLAN_CODE,MASTER_PLAN_EDESC,MASTER_PLAN_NDESC,START_DATE,END_DATE,CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,REMARKS,
                    COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_FLAG,APPROVED_BY,APPROVED_DATE,DELETED_FLAG)
                    VALUES({maxMasterPlanCode},'{masterPlanName}','{masterPlanName}',TO_DATE('{startDate}','YYYY-MON-DD'),TO_DATE('{endDate}','YYYY-MON-DD'),'{customer}',
                    '{employeeid}','{division}','','{_workcontext.CurrentUserinformation.company_code}','{branch}','{user}',TO_DATE('{DateTime.Today.ToShortDateString()}','MM/DD/YYYY'),'',
                    TO_DATE('{DateTime.Today.ToShortDateString()}','MM/DD/YYYY'),'','',
                    TO_DATE('{DateTime.Today.ToShortDateString()}','MM/DD/YYYY'),'N')";

                    foreach (var item in plans)
                    {
                        string[] pcode = item.Replace('[', ' ').Replace('"', ' ').Replace(']', ' ').Trim().Split('_');
                        int plancode = Convert.ToInt32(pcode[0]);
                        string masterOrChild = pcode[1];
                        // new line  start
                        if (masterOrChild == "MP")
                        {
                            var qry = $@"SELECT TO_CHAR(PLAN_CODE)PLAN_CODE FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE='{pcode[0]}'";
                            var resp = _dbContext.SqlQuery<SalesPlanMap>(qry).Where(x => x.PLAN_CODE != null).ToList();
                            foreach (var re in resp)
                            {
                                insertQuery += $@" INTO PL_SALES_PLAN_MAP(MASTER_PLAN_CODE,PLAN_CODE,PARENT_PLAN_CODE, PLAN_FLAG) VALUES({maxMasterPlanCode}, {re.PLAN_CODE}, {plancode},'{pcode[1]}')";
                            }
                        }
                        else // new line  end
                            insertQuery += $@" INTO PL_SALES_PLAN_MAP(MASTER_PLAN_CODE,PLAN_CODE,PLAN_FLAG) VALUES({maxMasterPlanCode},{plancode},'{pcode[1]}')";
                    }

                    string insertAll = @"INSERT ALL" + insertQuery + " SELECT * FROM DUAL ";
                    int result = this._dbContext.ExecuteSqlCommand(insertAll);
                    if (result > 0)
                        return "success";
                    else
                        return "failed to insert";
                }
                else
                {
                    var user = this._workcontext.CurrentUserinformation.User_id;
                    var employeeid = GetEmployeeByID(user.ToString());
                    //int maxMasterPlanCode = this._dbContext.SqlQuery<int>("SELECT COALESCE(MAX(MASTER_PLAN_CODE)+1, MAX(MASTER_PLAN_CODE) + 1, 1) FROM PL_MASTER_SALES_PLAN").First();

                    string updateQuery = $@"UPDATE PL_MASTER_SALES_PLAN SET MASTER_PLAN_EDESC='{masterPlanName}',START_DATE=TO_DATE('{startDate}','YYYY-MON-DD'),
                                            END_DATE=TO_DATE('{endDate}','YYYY-MON-DD'),CUSTOMER_CODE='{customer}',EMPLOYEE_CODE='{employeeid}',DIVISION_CODE='{division}',
                                            COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}',BRANCH_CODE='{branch}',
                                            LAST_MODIFIED_BY='{user}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Today.ToShortDateString()}','MM/DD/YYYY') WHERE MASTER_PLAN_CODE='{masterPlanCode}'";
                    this._dbContext.ExecuteSqlCommand(updateQuery);

                    string deleteQuery = $"DELETE FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE='{masterPlanCode}'";
                    this._dbContext.ExecuteSqlCommand(deleteQuery);
                    string insertQuery = string.Empty;
                    foreach (var item in plans)
                    {
                        string[] pcode = item.Replace('[', ' ').Replace('"', ' ').Replace(']', ' ').Trim().Split('_');
                        int plancode = Convert.ToInt32(pcode[0]);
                        string masterOrChild = pcode[1];
                        //insertQuery += $@" INTO PL_SALES_PLAN_MAP(MASTER_PLAN_CODE,PLAN_CODE,PLAN_FLAG) VALUES({masterPlanCode},{plancode},'{pcode[1]}')";
                        // new line  start
                        if (masterOrChild == "MP")
                        {
                            var qry = $@"SELECT TO_CHAR(PLAN_CODE)PLAN_CODE FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE='{pcode[0]}'";
                            var resp = _dbContext.SqlQuery<SalesPlanMap>(qry).Where(x => x.PLAN_CODE != null).ToList();
                            foreach (var re in resp)
                            {
                                insertQuery += $@" INTO PL_SALES_PLAN_MAP(MASTER_PLAN_CODE,PLAN_CODE,PARENT_PLAN_CODE, PLAN_FLAG) VALUES({masterPlanCode}, {re.PLAN_CODE}, {plancode},'{pcode[1]}')";
                            }
                        }
                        else // new line  end
                            insertQuery += $@" INTO PL_SALES_PLAN_MAP(MASTER_PLAN_CODE,PLAN_CODE,PLAN_FLAG) VALUES({masterPlanCode},{plancode},'{pcode[1]}')";
                    }

                    string insertAll = @"INSERT ALL" + insertQuery + " SELECT * FROM DUAL ";
                    int result = this._dbContext.ExecuteSqlCommand(insertAll);
                    if (result > 0)
                        return "success";
                    else
                        return "failed to insert";
                }

            }
            catch (Exception ex)
            {
                return "Error happened " + ex.Message;
            }
        }

        public List<MasterSalesPlan> getMasterSalesPlanList()
        {
            int userno = this._workcontext.CurrentUserinformation.User_id;
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            List<MasterSalesPlan> list = new List<MasterSalesPlan>();
            //string query = $@"SELECT MASTER_PLAN_CODE,MASTER_PLAN_EDESC,MASTER_PLAN_NDESC,TO_CHAR(START_DATE,'DD-MON-YYYY') START_DATE,TO_CHAR(END_DATE,'DD-MON-YYYY') END_DATE,CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,REMARKS,COMPANY_CODE FROM PL_MASTER_SALES_PLAN WHERE DELETED_FLAG='N' ORDER BY MASTER_PLAN_CODE DESC";
            string query = $@"SELECT MASTER_PLAN_CODE,
                             MASTER_PLAN_EDESC,
                             MASTER_PLAN_NDESC,
                             TO_CHAR (START_DATE, 'DD-MON-YYYY') START_DATE,
                             TO_CHAR (END_DATE, 'DD-MON-YYYY') END_DATE,
                             CUSTOMER_CODE,
                             EMPLOYEE_CODE,
                             DIVISION_CODE,
                             REMARKS,
                             COMPANY_CODE
                        FROM PL_MASTER_SALES_PLAN
                       WHERE DELETED_FLAG = 'N'
                        AND EMPLOYEE_CODE in (
                            SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userno}' AND COMPANY_CODE='{company_code}'
                              UNION ALL
                             SELECT HES.EMPLOYEE_CODE FROM HR_EMPLOYEE_SETUP HES, HR_EMPLOYEE_TREE ET
                                WHERE HES.EMPLOYEE_CODE = ET.EMPLOYEE_CODE 
                                 AND PARENT_EMPLOYEE_CODE=(SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userno}' AND COMPANY_CODE='{company_code}')
                                    START WITH PARENT_EMPLOYEE_CODE IS NULL
                                CONNECT BY PRIOR ET.EMPLOYEE_CODE  = PARENT_EMPLOYEE_CODE)
                    ORDER BY MASTER_PLAN_CODE DESC";

            list = this._dbContext.SqlQuery<MasterSalesPlan>(query).ToList();
            return list;
        }

        public bool DeleteMasterSalesPlan(string code)
        {
            try
            {
                string delete_sales_plan_map = $@"DELETE FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE='{code}'";
                string delete_master_sales_plan = $@"DELETE FROM PL_MASTER_SALES_PLAN WHERE MASTER_PLAN_CODE='{code}'";
                string uplateDeleteFlag = $@"UPDATE PL_MASTER_SALES_PLAN SET DELETED_FLAG='Y' WHERE MASTER_PLAN_CODE='{code}'";
                //int result_delete_sales_plan_map = this._dbContext.ExecuteSqlCommand(delete_sales_plan_map);
                //int result_delete_master_sales_plan = this._dbContext.ExecuteSqlCommand(delete_master_sales_plan);

                //if (result_delete_sales_plan_map == 1 && result_delete_master_sales_plan==1)
                //    return true;
                //else
                //    return false;
                int resultUpdateDeleteFlag = this._dbContext.ExecuteSqlCommand(uplateDeleteFlag);
                if (resultUpdateDeleteFlag == 1)
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public MasterSalesPlan getMasterSalesPlanDetail(string masterplancode)
        {
            string mspQuery = $@"SELECT MASTER_PLAN_CODE,MASTER_PLAN_EDESC,MASTER_PLAN_NDESC,TO_CHAR(START_DATE,'DD-MON-YYYY') START_DATE,
            TO_CHAR(END_DATE,'DD-MON-YYYY') END_DATE,CUSTOMER_CODE,EMPLOYEE_CODE,DIVISION_CODE,
            REMARKS,COMPANY_CODE FROM PL_MASTER_SALES_PLAN WHERE DELETED_FLAG='N' AND MASTER_PLAN_CODE='{masterplancode}'";

            string mspDetailQuery = $@"SELECT TO_CHAR(MASTER_PLAN_CODE) MASTER_PLAN_CODE,
             CASE WHEN PARENT_PLAN_CODE IS NULL
                THEN TO_CHAR(PLAN_CODE)
                ELSE TO_CHAR(PARENT_PLAN_CODE)
              END PLAN_CODE,
              CASE WHEN PARENT_PLAN_CODE IS NULL
                THEN 'SP'
                ELSE 'MP'
              END PLAN_FLAG
              FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE='{masterplancode}'";

            List<SalesPlanMap> spm = new List<SalesPlanMap>();
            MasterSalesPlan msp = new MasterSalesPlan();
            msp = this._dbContext.SqlQuery<MasterSalesPlan>(mspQuery).FirstOrDefault();
            spm = this._dbContext.SqlQuery<SalesPlanMap>(mspDetailQuery).ToList();

            if (msp != null)
            {
                msp.salesPlanMap = spm;
            }
            return msp;
        }

        public List<SalesPlanMap> getMasterSalesPlanDetailList(string masterplancode)
        {
            string mspQuery = $@"SELECT DISTINCT TO_CHAR (PM.PLAN_CODE) PLAN_CODE,
                                   --CASE
                                    --WHEN
                                    --PM.PLAN_FLAG='MP'
                                    --THEN
                                    --  '[MP] '||(SELECT MASTER_PLAN_EDESC FROM PL_MASTER_SALES_PLAN WHERE MASTER_PLAN_CODE IN (SELECT PLAN_CODE FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE=PM.MASTER_PLAN_CODE)) 
                                    --WHEN
                                    --    PM.PLAN_FLAG='SP'
                                    --THEN
                                      --  '[SP] '||SP.PLAN_EDESC
                                    --ELSE
                                     --   ''
                                   --END AS PLAN_EDESC,
                                   SP.PLAN_EDESC PLAN_EDESC,
                                   PM.PLAN_FLAG
                              FROM PL_MASTER_SALES_PLAN MSP, PL_SALES_PLAN_MAP PM, PL_SALES_PLAN SP
                             WHERE     MSP.DELETED_FLAG = 'N'
                                   AND PM.MASTER_PLAN_CODE = PM.MASTER_PLAN_CODE
                                   AND PM.PLAN_CODE = SP.PLAN_CODE
                                   AND PM.MASTER_PLAN_CODE ='{masterplancode}' ORDER BY PLAN_CODE";


            //string mspDetailQuery = $@"SELECT MASTER_PLAN_CODE,PLAN_CODE FROM PL_SALES_PLAN_MAP WHERE MASTER_PLAN_CODE='{masterplancode}'";
            List<SalesPlanMap> spm = new List<SalesPlanMap>();
            spm = this._dbContext.SqlQuery<SalesPlanMap>(mspQuery).ToList();

            return spm;
        }

        public List<SalesPlans_CustomersEmployees> GetDateWiseCustomerEmployeeDivisions(string startdate, string enddate)
        {
            List<SalesPlans_CustomersEmployees> list = new List<SalesPlans_CustomersEmployees>();
            string query = $@"
                SELECT DISTINCT TO_CHAR(SPD.PLAN_CODE) PLAN_CODE,
                   TO_CHAR(SPD.CUSTOMER_CODE) CUSTOMER_CODE,
                   (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE=SPD.CUSTOMER_CODE) CUSTOMER_EDESC,
                   TO_CHAR(SPD.EMPLOYEE_CODE) EMPLOYEE_CODE,
                   (SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=SPD.EMPLOYEE_CODE) EMPLOYEE_EDESC,
                   TO_CHAR(SPD.DIVISION_CODE) DIVISION_CODE,
                   (SELECT DIVISION_EDESC FROM FA_DIVISION_SETUP WHERE DIVISION_CODE=SPD.DIVISION_CODE) DIVISION_EDESC,
                   TO_CHAR(SPD.BRANCH_CODE) BRANCH_CODE,
                   (SELECT BRANCH_EDESC FROM FA_BRANCH_SETUP WHERE BRANCH_CODE=SPD.BRANCH_CODE) BRANCH_EDESC
                FROM PL_SALES_PLAN_DTL SPD
                WHERE PLAN_DATE BETWEEN TO_DATE ('{startdate}','YYYY-MM-DD')
                                 AND TO_DATE ('{enddate}','YYYY-MM-DD')";
            list = this._dbContext.SqlQuery<SalesPlans_CustomersEmployees>(query).ToList();
            return list;
        }

        public List<SalesPlans_CustomersEmployees> GetDateWiseCustomerEmployeeDivisions(string startdate, string enddate, string getFor)
        {
            var company_code = this._workcontext.CurrentUserinformation.company_code;
            List<SalesPlans_CustomersEmployees> list = new List<SalesPlans_CustomersEmployees>();
            string query = "SELECT DISTINCT ";
            string nullCondition = string.Empty;
            switch (getFor)
            {
                case "branch":
                    query += $@"
                   TO_CHAR(SPD.BRANCH_CODE) BRANCH_CODE,
                   (SELECT BRANCH_EDESC FROM FA_BRANCH_SETUP WHERE BRANCH_CODE=SPD.BRANCH_CODE AND COMPANY_CODE='" + company_code + "') BRANCH_EDESC ";
                    nullCondition = " AND SPD.BRANCH_CODE IS NOT NULL ";
                    break;
                case "division":
                    query += @"TO_CHAR(SPD.DIVISION_CODE) DIVISION_CODE,
                   (SELECT DIVISION_EDESC FROM FA_DIVISION_SETUP WHERE DIVISION_CODE=SPD.DIVISION_CODE AND COMPANY_CODE='" + company_code + "') DIVISION_EDESC ";
                    nullCondition = " AND SPD.DIVISION_CODE IS NOT NULL ";
                    break;
                case "employee":
                    query += @"TO_CHAR(SPD.EMPLOYEE_CODE) EMPLOYEE_CODE,
                   (SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=SPD.EMPLOYEE_CODE AND COMPANY_CODE='" + company_code + "') EMPLOYEE_EDESC ";
                    nullCondition = " AND SPD.EMPLOYEE_CODE IS NOT NULL ";
                    break;
                case "customer":
                    query += @"TO_CHAR(SPD.CUSTOMER_CODE) CUSTOMER_CODE,
                   (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE=SPD.CUSTOMER_CODE AND COMPANY_CODE='" + company_code + "') CUSTOMER_EDESC ";
                    nullCondition = " AND SPD.CUSTOMER_CODE IS NOT NULL ";
                    break;
                default:
                    break;
            }

            //query+=$@"FROM PL_SALES_PLAN_DTL SPD
            //    WHERE PLAN_DATE  BETWEEN TO_DATE ('{startdate}', 'YYYY-MM-DD')
            //                     AND TO_DATE ('{enddate}', 'YYYY-MM-DD')" + nullCondition;
            query += $@"FROM PL_SALES_PLAN_DTL SPD
                WHERE PLAN_DATE = TO_DATE ('{startdate}', 'YYYY-MM-DD') " + nullCondition;

            list = this._dbContext.SqlQuery<SalesPlans_CustomersEmployees>(query).ToList();
            return list;
        }

        public string GetEmployeeByID(string userId)
        {
            string query = $@" SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO = '{userId}' AND DELETED_FLAG ='N' AND  COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
            var result = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }

        public List<HREmployeeTreeModel> GetParentEmployeeTreeList()
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            List<HREmployeeTreeModel> hrEmployeeTree = new List<HREmployeeTreeModel>();
            //string query = $@"SELECT PARENT_EMPLOYEE_CODE,EMPLOYEE_CODE FROM HR_EMPLOYEE_TREE";
            //string query = $@"SELECT 
            //    TO_CHAR(ET.EMPLOYEE_CODE) EMPLOYEE_CODE,
            //    (select EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=ET.EMPLOYEE_CODE) EMPLOYEE_EDESC,
            //    TO_CHAR(ET.PARENT_EMPLOYEE_CODE) PARENT_EMPLOYEE_CODE,
            //    (select EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE) PARENT_EMPLOYEE_EDESC,
            //    LEVEL
            //    FROM HR_EMPLOYEE_TREE ET
            //    CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
            //    START WITH PARENT_EMPLOYEE_CODE IS NULL
            //    ORDER BY LEVEL, PARENT_EMPLOYEE_CODE";
            string query = $@"SELECT distinct
                TO_CHAR(ET.EMPLOYEE_CODE) EMPLOYEE_CODE,
                case 
                    when (select count(employee_code) from sc_application_users au where au.employee_code=ET.EMPLOYEE_CODE AND COMPANY_CODE='{company_code}') =0 then 
                    (select '*  '||EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=ET.EMPLOYEE_CODE AND COMPANY_CODE='{company_code}')
                    when (select count(employee_code) from sc_application_users au where au.employee_code=ET.EMPLOYEE_CODE AND COMPANY_CODE='{company_code}') >0 then 
                    (select EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=ET.EMPLOYEE_CODE AND COMPANY_CODE='{company_code}')
                    end EMPLOYEE_EDESC,
                --(select EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=ET.EMPLOYEE_CODE) EMPLOYEE_EDESC,
                TO_CHAR(ET.PARENT_EMPLOYEE_CODE) PARENT_EMPLOYEE_CODE,
                case 
                    when (select count(employee_code) from sc_application_users au where au.employee_code=ET.PARENT_EMPLOYEE_CODE AND COMPANY_CODE='{company_code}') =0 then 
                    (select '*  '||EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE AND COMPANY_CODE='{company_code}')
                    when (select count(employee_code) from sc_application_users au where au.employee_code=ET.PARENT_EMPLOYEE_CODE AND COMPANY_CODE='{company_code}') >0 then 
                    (select EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE AND COMPANY_CODE='{company_code}')
                    end PARENT_EMPLOYEE_EDESC,
                --(select EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE) PARENT_EMPLOYEE_EDESC,
                LEVEL
                FROM HR_EMPLOYEE_TREE ET
                CONNECT BY PRIOR ET.EMPLOYEE_CODE=ET.PARENT_EMPLOYEE_CODE
                START WITH PARENT_EMPLOYEE_CODE IS NULL
                ORDER BY LEVEL, PARENT_EMPLOYEE_CODE";
            hrEmployeeTree = this._dbContext.SqlQuery<HREmployeeTreeModel>(query).ToList();
            return hrEmployeeTree;
        }

        public string SaveParentEmployeeTreeList(string parentEmployeeCode, string selectedEmployees)
        {
            try
            {
                if (string.IsNullOrEmpty(parentEmployeeCode) || string.IsNullOrEmpty(selectedEmployees))
                {
                    return "please select employees for parent and child employee";
                }
                else
                {
                    var company_code = this._workcontext.CurrentUserinformation.company_code;
                    var checkParentexist = $"Select count(*) from HR_EMPLOYEE_TREE WHERE (PARENT_EMPLOYEE_CODE='{parentEmployeeCode}' OR EMPLOYEE_CODE='{parentEmployeeCode}') AND COMPANY_CODE='{company_code}'";
                    int existParentData = _dbContext.SqlQuery<int>(checkParentexist).FirstOrDefault();
                    if(existParentData==0)
                    {
                        return "Parent not exist!! Please insert parent Employee as Child Employee ";
                    }
                    string getExistedRecord = $@"SELECT EMPLOYEE_CODE FROM HR_EMPLOYEE_TREE WHERE PARENT_EMPLOYEE_CODE='{parentEmployeeCode}' AND COMPANY_CODE='{company_code}'";
                    List<HREmployeeTreeModel> existedRecordList = new List<HREmployeeTreeModel>();
                    existedRecordList = this._dbContext.SqlQuery<HREmployeeTreeModel>(getExistedRecord).ToList();
                    string[] child_employees = selectedEmployees.Split(',').ToArray();
                    string insertQuery = "INSERT INTO HR_EMPLOYEE_TREE(PARENT_EMPLOYEE_CODE,EMPLOYEE_CODE,COMPANY_CODE) ";
                    string message = string.Empty;
                    
                    for (int i = 0; i < child_employees.Length; i++)
                    {
                        //int check_employee_in_parent = this._dbContext.SqlQuery<int>("SELECT COUNT(PARENT_EMPLOYEE_CODE) FROM HR_EMPLOYEE_TREE WHERE PARENT_EMPLOYEE_CODE='" + child_employees[i] + "'").First();
                        //if (check_employee_in_parent > 0)
                        //{
                        //    message += parentEmployeeCode + " & " + child_employees[i] + " combination is already exist. \n";
                        //}
                        if ((existedRecordList.Where(a => a.EMPLOYEE_CODE == child_employees[i]).Count() < 1))// && (check_employee_in_parent == 0))
                        {
                            if (parentEmployeeCode != child_employees[i])
                            {
                                if (i == child_employees.Length - 1)
                                {
                                    insertQuery += $@"SELECT '{parentEmployeeCode}','{child_employees[i]}','{company_code}' FROM DUAL ";
                                }
                                else
                                {
                                    insertQuery += $@"SELECT '{parentEmployeeCode}','{child_employees[i]}','{company_code}' FROM DUAL UNION ALL ";
                                }
                            }
                        }
                    }
                    if (insertQuery.Contains("SELECT"))
                    {
                        var result = this._dbContext.ExecuteSqlCommand(insertQuery);
                        return "success " + message;
                    }
                    else
                    {
                        return "Employee already in tree OR " + message;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error " + ex.Message;
            }
        }

        public string DeleteEmployeeFromTree(string parent_employee_code, string employee_code)
        {
            try
            {
                var company_code = this._workcontext.CurrentUserinformation.company_code;
                string deleteQuery = $@"DELETE FROM HR_EMPLOYEE_TREE WHERE PARENT_EMPLOYEE_CODE='{parent_employee_code}' AND EMPLOYEE_CODE='{employee_code}' AND COMPANY_CODE='{company_code}'";
                var result = this._dbContext.ExecuteSqlCommand(deleteQuery);
                if (result == 1)
                {
                    return "success";
                }
                else
                {
                    return "Record delete failed.";
                }
            }
            catch (Exception ex)
            {
                return "error: " + ex.Message;
            }

        }

        public string getUsersEmployeeCode()
        {
            try
            {
                var userid = this._workcontext.CurrentUserinformation.User_id;
                var company_code = this._workcontext.CurrentUserinformation.company_code;
                string query = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userid}' AND DELETED_FLAG ='N' AND  COMPANY_CODE='{company_code}'";
                string employeeCode = this._dbContext.SqlQuery<String>(query).FirstOrDefault();
                return employeeCode;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Employee> GetEmployeesList()
        {
            var userNo = _workcontext.CurrentUserinformation.User_id;
            var company_code = _workcontext.CurrentUserinformation.company_code;
            var empListByUserCode = new List<string>();
            var empQuery = $@"SELECT EMPLOYEE_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{userNo}' AND COMPANY_CODE = '{company_code}' AND SUPER_USER_FLAG = 'Y'";
            empListByUserCode = _dbContext.SqlQuery<string>(empQuery).Where(x => x != null).ToList();

            var condition = string.Empty;

            if (empListByUserCode.Count > 0)
            {
                condition = $@" AND ET.EMPLOYEE_CODE IN ('{string.Join("','", empListByUserCode)}')";

            }
            var Query = $@"SELECT ET.EMPLOYEE_CODE as EmployeeCode,
                                  HES.EMPLOYEE_EDESC as EmployeeName                                            
                                  FROM HR_EMPLOYEE_SETUP HES, HR_EMPLOYEE_TREE ET
                                  WHERE HES.EMPLOYEE_CODE = ET.EMPLOYEE_CODE AND deleted_flag = 'N' AND HES.company_code = '{company_code}' {condition}
                                  ORDER BY employee_edesc";
            var employeeList = _dbContext.SqlQuery<Employee>(Query).ToList();

            if (employeeList.Count <= 0)
            {
                string query1 = $@"SELECT ET.employee_code as EmployeeCode,
                                      ET.employee_edesc as EmployeeName                                            
                                      FROM hr_employee_setup ET
                                      WHERE ET.deleted_flag = 'N' AND ET.company_code = '{company_code}' {condition} AND ET.GROUP_SKU_FLAG= 'I'
                                    ORDER BY ET.employee_edesc";
                employeeList = _dbContext.SqlQuery<Employee>(query1).ToList();
            }

            //if (employeeList.Count <= 0)
            //{
            //    string query1 = $@"SELECT employee_code as EmployeeCode,
            //                      employee_edesc as EmployeeName                                            
            //                      FROM hr_employee_setup 
            //                      WHERE deleted_flag = 'N' AND company_code = '{company_code}' AND GROUP_SKU_FLAG= 'I'
            //                    ORDER BY PRE_EMPLOYEE_CODE";
            //    employeeList = _dbContext.SqlQuery<Employee>(query1).ToList();
            //}
            return employeeList;
        }
        public string GetDashboardWidgets(string name, string type)
        {
            try
            {
                name = name.ToUpper();
                string Query = @"SELECT order_no from dashboard_widgets where user_id='" + name + "' and module_name='" + type + "'";
                var widgets = _dbContext.SqlQuery<string>(Query).FirstOrDefault();
                return widgets;
            }
            catch
            {
                return "";
            }
        }
        public List<AggregatePlanModel> GetAggregatePlanType(string filter)
        {
            var result = new List<AggregatePlanModel>();
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                //string query = $@"SELECT * FROM (
                //                    SELECT TO_CHAR(PLAN_CODE)PLAN_CODE,'SALES' INITIALS, PLAN_EDESC,'Sales Plan' PLAN_TYPE, START_DATE FROM PL_SALES_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'PROCUREMENT' INITIALS,PLAN_EDESC,'Production Plan' PLAN_TYPE, START_DATE FROM PL_PRO_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'LEDGER' INITIALS,PLAN_EDESC,'Ledger Plan' PLAN_TYPE, START_DATE FROM PL_COA_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE,'BUDGET' INITIALS, PLAN_EDESC,'Budget Plan' PLAN_TYPE, START_DATE FROM PL_COA_SUB_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'PROCUREMENT' INITIALS,PLAN_EDESC,'Procurement Plan' PLAN_TYPE, START_DATE FROM PL_BRD_PRCMT_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE,'MATERIAL' INITIALS,  PLAN_EDESC,'Material Plan' PLAN_TYPE, PLAN_DATE START_DATE FROM PL_MATERIAL_PLAN) ORDER BY START_DATE, PLAN_EDESC ASC";
                string query = $@"  SELECT DISTINCT * FROM (
                                    SELECT 'SALES' INITIALS, 'Sales Plan' PLAN_TYPE FROM DUAL
                                    UNION ALL
                                    SELECT   'PRODUCTION' INITIALS,'Production Plan' PLAN_TYPE FROM DUAL
                                    UNION ALL
                                    SELECT   'LEDGER' INITIALS,'COA Budget Plan' PLAN_TYPE FROM DUAL
                                    UNION ALL
                                    SELECT  'BUDGET' INITIALS, 'Cost Center Budget' PLAN_TYPE FROM DUAl
                                    UNION ALL
                                    SELECT   'PROCUREMENT' INITIALS,'Procurement Plan' PLAN_TYPE FROM DUAL
                                    UNION ALL
                                    SELECT  'MATERIAL' INITIALS,  'Material Plan' PLAN_TYPE FROM DUAL) ORDER BY INITIALS ASC";
                result = _dbContext.SqlQuery<AggregatePlanModel>(query).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        public List<AggregatePlanModel> GetAllPlansWithType()
        {
            var result = new List<AggregatePlanModel>();
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                //string query = $@"SELECT * FROM (
                //                    SELECT TO_CHAR(PLAN_CODE)PLAN_CODE,'SALES' INITIALS, PLAN_EDESC,'Sales Plan' PLAN_TYPE, TO_CHAR(START_DATE,'DD-Mon-YYYY') PLAN_DATE FROM PL_SALES_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'PROCUREMENT' INITIALS,PLAN_EDESC,'Production Plan' PLAN_TYPE, TO_CHAR(START_DATE,'DD-Mon-YYYY') PLAN_DATE FROM PL_PRO_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'LEDGER' INITIALS,PLAN_EDESC,'Ledger Plan' PLAN_TYPE, TO_CHAR(START_DATE,'DD-Mon-YYYY') PLAN_DATE FROM PL_COA_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE,'BUDGET' INITIALS, PLAN_EDESC,'Budget Plan' PLAN_TYPE, TO_CHAR(START_DATE,'DD-Mon-YYYY') PLAN_DATE FROM PL_COA_SUB_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'PROCUREMENT' INITIALS,PLAN_EDESC,'Procurement Plan' PLAN_TYPE, TO_CHAR(START_DATE,'DD-Mon-YYYY')PLAN_DATE FROM PL_BRD_PRCMT_PLAN
                //                    UNION ALL
                //                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE,'MATERIAL' INITIALS,  PLAN_EDESC,'Material Plan' PLAN_TYPE, TO_CHAR(PLAN_DATE,'DD-Mon-YYYY') FROM PL_MATERIAL_PLAN) ORDER BY INITIALS, PLAN_EDESC ASC";
                string query = $@" SELECT * FROM (
                                    SELECT TO_CHAR(PLAN_CODE)PLAN_CODE,'SALES' INITIALS, PLAN_EDESC,'Sales Plan' PLAN_TYPE, TO_CHAR(CREATED_DATE,'DD-Mon-YYYY') PLAN_DATE,TO_CHAR(BS_DATE(START_DATE))N_PLAN_DATE, '1' ORDERS,'Manual' AS REF_FLAG FROM PL_SALES_PLAN
                                    UNION ALL
                                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'PROCUREMENT' INITIALS,PLAN_EDESC,'Production Plan' PLAN_TYPE, TO_CHAR(CREATED_DATE,'Mon-DD-YYYY') PLAN_DATE,TO_CHAR(BS_DATE(START_DATE))N_PLAN_DATE,'3' ORDERS,'Manual' AS REF_FLAG FROM PL_PRO_PLAN
                                    UNION ALL
                                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'LEDGER' INITIALS,PLAN_EDESC,'Ledger Plan' PLAN_TYPE, TO_CHAR(CREATED_DATE,'Mon-DD-YYYY') PLAN_DATE,TO_CHAR(BS_DATE(START_DATE))N_PLAN_DATE,'4' ORDERS,'Manual' AS REF_FLAG FROM PL_COA_PLAN
                                    UNION ALL
                                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE,'BUDGET' INITIALS, PLAN_EDESC,'Budget Plan' PLAN_TYPE, TO_CHAR(CREATED_DATE,'Mon-DD-YYYY') PLAN_DATE,TO_CHAR(BS_DATE(START_DATE))N_PLAN_DATE,'5' ORDERS,'Manual' AS REF_FLAG FROM PL_COA_SUB_PLAN
                                    UNION ALL
                                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE, 'PROCUREMENT' INITIALS,PLAN_EDESC,'Procurement Plan' PLAN_TYPE, TO_CHAR(CREATED_DATE,'DD-Mon-YYYY') PLAN_DATE,TO_CHAR(BS_DATE(START_DATE))N_PLAN_DATE,'2' ORDERS,
                                     CASE
                                          WHEN TO_CHAR(REFERENCE_FLAG) IS NULL
                                           THEN 'Manual'
                                          ELSE 'Refrential'
                                        END REF_FLAG
                                    FROM PL_BRD_PRCMT_PLAN
                                    UNION ALL
                                    SELECT  TO_CHAR(PLAN_CODE)PLAN_CODE,'MATERIAL' INITIALS,  PLAN_EDESC,'Material Plan' PLAN_TYPE, TO_CHAR(CREATED_DATE,'Mon-DD-YYYY'),TO_CHAR(BS_DATE(PLAN_DATE))N_PLAN_DATE, '3' ORDERS,
                                     CASE
                                          WHEN TO_CHAR(REFERENCE_FLAG) IS NULL
                                           THEN 'Manual'
                                          ELSE 'Refrential'
                                        END REF_FLAG FROM PL_MATERIAL_PLAN) ORDER BY ORDERS,INITIALS, PLAN_EDESC ASC";
                result = _dbContext.SqlQuery<AggregatePlanModel>(query).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
        public List<ProductCategory> GetAllCategory(string filter = null)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                var query = $@"SELECT CATEGORY_CODE,CATEGORY_EDESC FROM IP_CATEGORY_CODE WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'";
                return _dbContext.SqlQuery<ProductCategory>(query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<User> getAllUsers(string filter = null)
        {
            var company_code = _workcontext.CurrentUserinformation.company_code;
            try
            {
                var query = $@"SELECT USER_NO, LOGIN_EDESC FROM SC_APPLICATION_USERS WHERE DELETED_FLAG='N' AND COMPANY_CODE='{company_code}'";
                return _dbContext.SqlQuery<User>(query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string SaveConfigSetup(ConfigSetupModel model)
        {
            var result = "";
            try
            {
                result = loadInXml(model);
            }
            catch (Exception)
            {

                throw;
            }

            //var info = _workcontext.CurrentUserinformation;
            //try
            //{
            //    var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/Areas/NeoERP.Planning/App_Data/");
            //    var isFileExists = File.Exists(path + @"\ProductCondition.xml");
            //    var userList = model.USER_NO.Split(',').ToList();
            //    var planTypeList = model.PLAN_NAME.Split(',').ToList();
            //    XmlWriter xmlWriter;
            //    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            //    xmlWriterSettings.Indent = true;
            //    xmlWriterSettings.NewLineOnAttributes = true;

            //    if (!isFileExists)
            //    {
            //        xmlWriter = XmlWriter.Create(path + "ProductCondition.xml", xmlWriterSettings);
            //        xmlWriter.WriteStartDocument();
            //        xmlWriter.WriteStartElement("ProductCondition");
            //        xmlWriter.WriteEndElement();
            //        xmlWriter.WriteEndDocument();
            //        xmlWriter.Flush();
            //        xmlWriter.Close();
            //    }
            //    XDocument xDocument = XDocument.Load(path + "ProductCondition.xml");
            //    XElement root = xDocument.Element("ProductCondition");
            //    foreach (var user in userList)
            //    {
            //        var namebycodeqry = $@"SELECT LOGIN_CODE FROM SC_APPLICATION_USERS WHERE USER_NO='{user}' AND DELETED_FLAG='N' AND COMPANY_CODE='{info.company_code}'";
            //        var userName = _dbContext.SqlQuery<string>(namebycodeqry).FirstOrDefault().ToString();
            //        IEnumerable<XElement> urows = root.Descendants("User");
            //        var checkUser = urows.Where(p => p.FirstAttribute.Value.ToString() == user).Select(p => p).ToList();
            //        if (checkUser.Count() > 0)
            //        {
            //            urows.Where(p => p.FirstAttribute.Value.ToString() == user).Select(p => p).ToList().Remove();
            //        }
            //        XElement userRow = new XElement(
            //                new XElement("User", new XAttribute("UID", user.ToString()), new XAttribute("Name", userName)));
            //        root.AddFirst(userRow);
            //        foreach (var type in planTypeList)
            //        {
            //            IEnumerable<XElement> prows = urows.Descendants("Plan");
            //            var checkPlan = prows.Where(p => p.LastAttribute.Value.ToString().ToUpper() == type).Select(p => p).ToList();
            //            if (checkPlan.Count() > 0)
            //            {
            //                prows.Where(p => p.LastAttribute.Value.ToString().ToUpper() == type).Select(p => p).ToList().Remove();
            //            }
            //            var planNameCode = GetCodeName(type.ToString());
            //            var categoryCode = model.CATEGORY_CODE;
            //            XElement planRow = new XElement("Plan", new XAttribute("PID", planNameCode.Item1), new XAttribute("Name", planNameCode.Item2),
            //                      new XElement("ConditionQuery", $@" AND CATEGORY_CODE IN ('{categoryCode}')"));
            //            userRow.AddFirst(planRow);
            //            xDocument.Save(path + "ProductCondition.xml");
            //        }
            //    }
            //    result = "Success";
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return result;
        }
        public string loadInXml(ConfigSetupModel model)
        {
            var result = "";
            var info = _workcontext.CurrentUserinformation;
            try
            {
                var path = HttpContext.Current.ApplicationInstance.Server.MapPath("~/Areas/NeoERP.Planning/");
                var isFileExists = File.Exists(path + @"\ProductCondition.xml");
                //var userList = model.USER_NO.Split(',').ToList();
                var planTypeList = model.PLAN_NAME.Split(',').ToList();
                XmlWriter xmlWriter;
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = true;

                if (!isFileExists)
                {
                    xmlWriter = XmlWriter.Create(path + "ProductCondition.xml", xmlWriterSettings);
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("ProductCondition");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                XDocument xDocument = XDocument.Load(path + "ProductCondition.xml");
                XElement root = xDocument.Element("ProductCondition");
                IEnumerable<XElement> urows = root.Descendants("Vendor");
                foreach (var type in planTypeList)
                {
                    var checkPlan = urows.Where(p => p.LastAttribute.Value.ToString().ToUpper() == type).Select(p => p).ToList();
                    if (checkPlan.Count() > 0)
                    {
                        urows.Where(p => p.LastAttribute.Value.ToString().ToUpper() == type).Select(p => p).ToList().Remove();
                    }
                    var planNameCode = GetCodeName(type.ToString());
                    var categoryCode = model.CATEGORY_CODE;
                    XElement planRow = new XElement("Vendor", new XAttribute("ID", planNameCode.Item1), new XAttribute("Name", info.company_name), new XAttribute("PlanName", planNameCode.Item2),
                              new XElement("ConditionQuery", $@" AND CATEGORY_CODE IN ('{categoryCode}')"));
                    root.AddFirst(planRow);
                    xDocument.Save(path + "ProductCondition.xml");
                }
                result = "Success";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<EmployeeHandoverModel> getEmployeeHandoverList()
        {
            var result = new List<EmployeeHandoverModel>();
            var info = _workcontext.CurrentUserinformation;
            try
            {
                var query = $@" SELECT DISTINCT PEH.FROM_EMPLOYEE_CODE,(SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE COMPANY_CODE='{info.company_code}' AND DELETED_FLAG='N' AND EMPLOYEE_CODE=PEH.FROM_EMPLOYEE_CODE)FROM_EMPLOYEE_EDESC
                                 ,PEH.TO_EMPLOYEE_CODE,(SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE COMPANY_CODE='{info.company_code}' AND DELETED_FLAG='N' AND EMPLOYEE_CODE=PEH.TO_EMPLOYEE_CODE)TO_EMPLOYEE_EDESC,
                                 TO_CHAR(FROM_DATE,'MM-DD-YYYY')FROM_DATE, BS_DATE(FROM_DATE)NFROM_DATE,TO_CHAR(PEH.CREATED_DATE,'DD-MM-YYYY')CREATED_DATE FROM PL_EMPLOYEE_HANDOVER PEH
                                 WHERE PEH.DELETED_FLAG='N' AND PEH.COMPANY_CODE='{info.company_code}' ORDER BY TO_CHAR(PEH.CREATED_DATE,'DD-MM-YYYY') DESC";
                result = _dbContext.SqlQuery<EmployeeHandoverModel>(query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanListModel> getEmployeeHandoverListWithPlan(string fromEmpCode, string toEmpCode)
        {
            var result = new List<PlanListModel>();
            var info = _workcontext.CurrentUserinformation;
            try
            {
                var query = $@" SELECT DISTINCT SP.PLAN_CODE, SP.PLAN_EDESC, TO_DATE(EH.FROM_DATE,'DD-MM-YY') START_DATE, TO_DATE(SP.END_DATE,'DD-MM-YY') END_DATE FROM PL_SALES_PLAN SP 
                                 LEFT JOIN PL_EMPLOYEE_HANDOVER EH ON SP.PLAN_CODE=EH.PLAN_CODE 
                                 WHERE EH.FROM_EMPLOYEE_CODE='{fromEmpCode}' AND EH.TO_EMPLOYEE_CODE='{toEmpCode}' AND SP.DELETED_FLAG='N' AND SP.COMPANY_CODE='{info.company_code}'";
                result = _dbContext.SqlQuery<PlanListModel>(query).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public static Tuple<string, string> GetCodeName(string name)
        {
            var planCode = "";
            var planName = "";
            switch (name)
            {
                case "SALES":
                    planName = "Sales";
                    planCode = "01";
                    break;
                case "PRODUCTION":
                    planName = "Production";
                    planCode = "02";
                    break;
                case "PROCUREMENT":
                    planName = "Procurement";
                    planCode = "03";
                    break;
                case "MATERIAL":
                    planName = "Material";
                    planCode = "04";
                    break;
                case "LEDGER":
                    planName = "Ledger";
                    planCode = "05";
                    break;
                case "BUDGET":
                    planName = "Budget";
                    planCode = "06";
                    break;
            }
            return new Tuple<string, string>(planCode, planName);
        }
        public Tuple<string, string> getPlanCodeName(string name)
        {
            return new Tuple<string, string>(name, name);
        }
    }
}
