using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Repository.ThirdPartyApi
{


    public  class OSPreferenceSetup: IOSPreferenceSetup
    {

        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private IDbContext _dbContext;
        public OSPreferenceSetup(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        #region Preference Setup
        //Save OS
        public string SavePreferenceSetup(OS_PREFERENCE_SETUP model)
        {
            if (model.SMERGE_ITEM_FLAG == "True")
            {
                model.SMERGE_ITEM_FLAG = "Y";
            }
            else
            {
                model.SMERGE_ITEM_FLAG = "N";
            }

            if (model.OMERGE_ITEM_FLAG == "True")
            {
                model.OMERGE_ITEM_FLAG = "Y";
            }
            else
            {
                model.OMERGE_ITEM_FLAG = "N";
            }
            //show Opera symphony
            if (model.S_DISPLAY_FLAG == "True")
            {
                model.S_DISPLAY_FLAG = "Y";
            }
            else
            {
                model.S_DISPLAY_FLAG = "N";
            }

            if (model.O_DISPLAY_FLAG == "True")
            {
                model.O_DISPLAY_FLAG = "Y";
            }
            else
            {
                model.O_DISPLAY_FLAG = "N";
            }
            //ird
            if (model.ENABLE_IRD == "True")
            {
                model.ENABLE_IRD = "Y";
            }
            else
            {
                model.ENABLE_IRD = "N";
            }

            if (model.ENABLE_REALTIME == "True")
            {
                model.ENABLE_REALTIME = "Y";
            }
            else
            {
                model.ENABLE_REALTIME = "N";
            }
            if (model.ENABLE_SYN == "True")
            {
                model.ENABLE_SYN = "Y";
            }
            else
            {
                model.ENABLE_SYN = "N";
            }

            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                var message = string.Empty;
                var newpreferencetname = $@"SELECT OS_ID from OS_PREFERENCE_SETUP  where COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and LOWER(ACC_EDESC) =LOWER('{model.STABLE_NAME}')";
                var result = this._dbContext.SqlQuery<string>(newpreferencetname).FirstOrDefault();
                var validateData = string.IsNullOrWhiteSpace(result) ? null : result;
                if (validateData == "" || validateData == null)
                {
                var maxOScode = $@"SELECT NVL(MAX(TO_NUMBER(REGEXP_SUBSTR(TO_NUMBER(OS_ID), '[^.]+', 1, 1))),0)+1 as MAX_OS_ID from OS_PREFERENCE_SETUP  WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                var newOSId = this._dbContext.SqlQuery<int>(maxOScode).FirstOrDefault().ToString();

                string Query = $@"INSERT INTO OS_PREFERENCE_SETUP (OS_ID,SFORM_CODE,STABLE_NAME,SCUS_ACC_CODE, SITEM_ACC_CODE, SMERGE_ITEM_FLAG,
                               S_ISPARENTCUSTOMER_CREATED, S_ISPARENTITEM_CREATED,S_DISPLAY_FLAG, O_DISPLAY_FLAG, COMPANY_CODE,CREATED_BY,CREATED_DATE,OFORM_CODE,
                               OTABLE_NAME,OCUS_ACC_CODE,OITEM_ACC_CODE,OMERGE_ITEM_FLAG,SVAT_CHARGE_ACC_CODE,SDISCOUNT_ACC_CODE,SSERCIVE_ACC_CODE,OVAT_CHARGE_ACC_CODE,
                               ODISCOUNT_ACC_CODE,OSERCIVE_ACC_CODE,IRD_URL,IRD_USER,IRD_PASSWORD,IRD_PAN_NO,ENABLE_IRD,ENABLE_REALTIME,ENABLE_SYN) 
                               VALUES('{newOSId}','{model.SFORM_CODE}','{model.STABLE_NAME}','{model.SCUS_ACC_CODE}','{model.SITEM_ACC_CODE}','{model.SMERGE_ITEM_FLAG}',
                              '{model.S_ISPARENTCUSTOMER_CREATED}','{model.S_ISPARENTITEM_CREATED}','{model.S_DISPLAY_FLAG}','{model.O_DISPLAY_FLAG}',
                              '{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),
                              '{model.OFORM_CODE}','{model.OTABLE_NAME}','{model.OCUS_ACC_CODE}','{model.OITEM_ACC_CODE}','{model.OMERGE_ITEM_FLAG}','{model.SVAT_CHARGE_ACC_CODE}',
                              '{model.SDISCOUNT_ACC_CODE}','{model.SSERCIVE_ACC_CODE}','{model.OVAT_CHARGE_ACC_CODE}','{model.ODISCOUNT_ACC_CODE}','{model.OSERCIVE_ACC_CODE}','{model.IRD_URL}',
                              '{model.IRD_USER}','{model.IRD_PASSWORD}','{model.IRD_PAN_NO}','{model.ENABLE_IRD}','{model.ENABLE_REALTIME}','{model.ENABLE_SYN}')";
                var rowCount = _dbContext.ExecuteSqlCommand(Query);

                if (rowCount > 0)
                {
                    message = "INSERTED";
                }

                return message;
            }
                else
                {
                    message = "INVALIDINSERTED";
                    return message;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //Update OS
        public string UpdatePreferenceSetup(OS_PREFERENCE_SETUP model)
        {
            if (model.SMERGE_ITEM_FLAG == "True")
            {
                model.SMERGE_ITEM_FLAG = "Y";
            }
            else
            {
                model.SMERGE_ITEM_FLAG = "N";
            }

            if (model.OMERGE_ITEM_FLAG == "True")
            {
                model.OMERGE_ITEM_FLAG = "Y";
            }
            else
            {
                model.OMERGE_ITEM_FLAG = "N";
            }

            //show Opera symphony
            if (model.S_DISPLAY_FLAG == "True")
            {
                model.S_DISPLAY_FLAG = "Y";
            }
            else
            {
                model.S_DISPLAY_FLAG = "N";
            }

            if (model.O_DISPLAY_FLAG == "True")
            {
                model.O_DISPLAY_FLAG = "Y";
            }
            else
            {
                model.O_DISPLAY_FLAG = "N";
            }
            //ird
            if (model.ENABLE_IRD == "True")
            {
                model.ENABLE_IRD = "Y";
            }
            else
            {
                model.ENABLE_IRD = "N";
            }

            if (model.ENABLE_REALTIME == "True")
            {
                model.ENABLE_REALTIME = "Y";
            }
            else
            {
                model.ENABLE_REALTIME = "N";
            }
            if (model.ENABLE_SYN == "True")
            {
                model.ENABLE_SYN = "Y";
            }
            else
            {
                model.ENABLE_SYN = "N";
            }

            try
            {
                var message = string.Empty;
                string Query = $@"UPDATE OS_PREFERENCE_SETUP SET SFORM_CODE='{model.SFORM_CODE}',STABLE_NAME='{model.STABLE_NAME}',
                                   SCUS_ACC_CODE='{model.SCUS_ACC_CODE}',SITEM_ACC_CODE='{model.SITEM_ACC_CODE}',
                                   SMERGE_ITEM_FLAG='{model.SMERGE_ITEM_FLAG}',S_DISPLAY_FLAG='{model.S_DISPLAY_FLAG}',
                                   OFORM_CODE='{model.OFORM_CODE}', OTABLE_NAME='{model.OTABLE_NAME}',
                                   OCUS_ACC_CODE='{model.OCUS_ACC_CODE}', MODIFY_BY='{_workContext.CurrentUserinformation.login_code}',MODIFY_DATE = SYSDATE,
                                   OITEM_ACC_CODE='{model.OITEM_ACC_CODE}',OMERGE_ITEM_FLAG='{model.OMERGE_ITEM_FLAG}',O_DISPLAY_FLAG='{model.O_DISPLAY_FLAG}',
                                   SVAT_CHARGE_ACC_CODE='{model.SVAT_CHARGE_ACC_CODE}',SDISCOUNT_ACC_CODE='{model.SDISCOUNT_ACC_CODE}',
                                   SSERCIVE_ACC_CODE='{model.SSERCIVE_ACC_CODE}',
                                   OVAT_CHARGE_ACC_CODE='{model.OVAT_CHARGE_ACC_CODE}',ODISCOUNT_ACC_CODE='{model.ODISCOUNT_ACC_CODE}',
                                   OSERCIVE_ACC_CODE='{model.OSERCIVE_ACC_CODE}',IRD_URL='{model.IRD_URL}',IRD_USER='{model.IRD_USER}',
                                   IRD_PASSWORD='{model.IRD_PASSWORD}',IRD_PAN_NO='{model.IRD_PAN_NO}',ENABLE_IRD='{model.ENABLE_IRD}',
                                   ENABLE_REALTIME='{model.ENABLE_REALTIME}',ENABLE_SYN='{model.ENABLE_SYN}'
                                   WHERE OS_ID = '{model.OS_ID}'";

                var rowCount = _dbContext.ExecuteSqlCommand(Query);

                if (rowCount > 0)
                {
                    message = "UPDATED";
                }

                return message;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<OS_PREFERENCE_SETUP> GetPreferenceSetup()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                string query = $@"SELECT OS_ID,SFORM_CODE,STABLE_NAME,SITEM_ACC_CODE From OS_PREFERENCE_SETUP  WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                var PreffList = _dbContext.SqlQuery<OS_PREFERENCE_SETUP>(query).ToList();
                return PreffList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //Get data
        public OS_PREFERENCE_SETUP getPreffitem(string prefId)
        {
            try
            {
               
                if (string.IsNullOrEmpty(prefId)) { prefId = string.Empty; }
                string Query = $@"SELECT OS_ID,SFORM_CODE,STABLE_NAME,SCUS_ACC_CODE,SITEM_ACC_CODE,SMERGE_ITEM_FLAG,S_DISPLAY_FLAG,
                                  O_DISPLAY_FLAG,OFORM_CODE,OTABLE_NAME,OCUS_ACC_CODE,OITEM_ACC_CODE,OMERGE_ITEM_FLAG,SVAT_CHARGE_ACC_CODE,
                                  SDISCOUNT_ACC_CODE,SSERCIVE_ACC_CODE,OVAT_CHARGE_ACC_CODE,ODISCOUNT_ACC_CODE,OSERCIVE_ACC_CODE,
                                  IRD_URL,IRD_USER,IRD_PASSWORD,IRD_PAN_NO,ENABLE_IRD,ENABLE_REALTIME,ENABLE_SYN
                                  from OS_PREFERENCE_SETUP WHERE OS_ID='{prefId}'";
                OS_PREFERENCE_SETUP entity = this._dbContext.SqlQuery<OS_PREFERENCE_SETUP>(Query).FirstOrDefault();
                return entity;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        #endregion
    }


}

