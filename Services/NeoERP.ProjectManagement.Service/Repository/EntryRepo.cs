using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.ProjectManagement.Service.Interface;
using NeoERP.ProjectManagement.Service.Models;
using NeoERP.DocumentTemplate.Service.Models;
using NeoERP.DocumentTemplate.Service.Interface;
using System;
using NeoErp.Core.Services.CommonSetting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Core.Models.CustomModels;
using System.Text;

namespace NeoERP.ProjectManagement.Service.Repository
{
  public class EntryRepo : IEntryRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        NeoErpCoreEntity _coreEntity;
        private ISettingService _settingService;
        private ICacheManager _cacheManager;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        public EntryRepo(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity coreentity, ISettingService settingService)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._cacheManager = cacheManager;
            _coreEntity = coreentity;
            this._settingService = settingService;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }
        public SubmitResponse SaveWebPrefrence(WebPrefrence model)
        {
            var response = new SubmitResponse();
            response.Success = true;
            response.Message = "Saved successFully";
            var filename = Constants.WebPrefranceSetting;
            try
            {
                // var setting = _settingService.LoadSetting<UserDashboardSetting>(filename);
                var userDashboardSetting = new WebPrefrenceSetting();
                userDashboardSetting.Userid = model.Userid;
                userDashboardSetting.ShowAdvanceSearch = model.ShowAdvanceSearch;
                userDashboardSetting.ShowAdvanceAutoComplete = model.ShowAdvanceAutoComplete;
                if (_settingService.DeleteSetting(filename))
                {
                    _settingService.SaveSetting<WebPrefrenceSetting>(userDashboardSetting, filename);
                }


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;

        }
        public string updateAreaSetup(AreaModels model)
        {
            using (var trans = _coreEntity.Database.BeginTransaction())
            {
                try
                {
                    var company_code = _workContext.CurrentUserinformation.company_code;
                    var message = string.Empty;
                    string Query = $@"UPDATE AREA_SETUP SET AREA_EDESC='{model.AREA_EDESC}', REMARKS='{model.REMARKS}', MODIFY_BY='{_workContext.CurrentUserinformation.login_code}',MODIFY_DATE = SYSDATE  WHERE AREA_CODE = '{model.AREA_CODE}'";
                    var entity = this._coreEntity.ExecuteSqlCommand(Query);
                    if (entity > 0)
                    {
                        message = "UPDATED";
                    }
                    trans.Commit();
                    return message;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }

        }
        public List<FormControlModels> GetFormControls(string formcode)
        {
            string Query = $@"SELECT CREATE_FLAG,READ_FLAG,UPDATE_FLAG,DELETE_FLAG,POST_FLAG,UNPOST_FLAG,CHECK_FLAG,VERIFY_FLAG FROM SC_FORM_CONTROL WHERE USER_NO='{_workContext.CurrentUserinformation.User_id}' AND FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG='N'";
            _logErp.InfoInFile("Getting Form control using : " + Query + " :query");
            List<FormControlModels> record = this._dbContext.SqlQuery<FormControlModels>(Query).ToList();
            _logErp.InfoInFile(record.Count() + " Form controls fetched");
            return record;
        }
        public List<FORM_SETUP_REFERENCE> GetRefrenceFlag(string formcode)
        {
            try
            {
                var sqlquery = $@"SELECT FREEZE_BACK_DAYS,DECIMAL_PLACE,NEGATIVE_STOCK_FLAG,FREEZE_MANUAL_ENTRY_FLAG,DISCOUNT_SCHEDULE_FLAG,PRICE_CONTROL_FLAG,RATE_DIFF_FLAG,REFERENCE_FLAG, REF_TABLE_NAME, REF_FORM_CODE, FREEZE_MASTER_REF_FLAG,REF_FIX_QUANTITY,REF_FIX_PRICE,SERIAL_TRACKING_FLAG,BATCH_TRACKING_FLAG,RT_CONTROL_FLAG FROM FORM_SETUP WHERE FORM_CODE='{formcode}' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<FORM_SETUP_REFERENCE>(sqlquery).ToList();
                if (result.Count > 0)
                {
                    var sqlqueryRef = $@"SELECT SIM_FLAG FROM PREFERENCE_SETUP WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";
                    var resultRef = _dbContext.SqlQuery<FORM_SETUP_REFERENCE>(sqlqueryRef).ToList();
                    if (resultRef.Count > 0)
                    {
                        result[0].SIM_FLAG = resultRef[0].SIM_FLAG;
                    }


                    var subPreference = $@"SELECT
                        DEALER_SYSTEM_FLAG 
                        FROM PREFERENCE_SUB_SETUP
                        where COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}'";
                    var dealer_system_flag = this._dbContext.SqlQuery<string>(subPreference).FirstOrDefault();
                    if (dealer_system_flag == "Y")
                    {
                        result[0].Dealer_system_flag = "Y";
                    }
                    else
                    {
                        result[0].Dealer_system_flag = "N";
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ChargeOnSales> GetChargesData(string formCode, string voucherNo)
        {
            try
            {
                string query = string.Empty;
                var result = new List<ChargeOnSales>();
                if (voucherNo != "undefined")
                {
                    //AA previous query where the data where repeated so query was changed slightly 
                    //query = $@" SELECT DISTINCT CS.CHARGE_CODE, IC.CHARGE_EDESC, CS.CHARGE_TYPE_FLAG, CS.CALCULATE_BY VALUE_PERCENT_FLAG, ROUND(CS.CHARGE_AMOUNT,2)CHARGE_AMOUNT,0 VALUE_PERCENT_AMOUNT, CS.SERIAL_NO PRIORITY_INDEX_NO,CS.ACC_CODE,
                    //              CS.APPLY_ON, CS.GL_FLAG,CS.NON_GL_FLAG FROM CHARGE_TRANSACTION CS, IP_CHARGE_CODE IC 
                    //              WHERE CS.CHARGE_CODE = IC.CHARGE_CODE AND CS.COMPANY_CODE = IC.COMPANY_CODE AND  CS.FORM_CODE= '{formCode}' AND REFERENCE_NO = '{voucherNo}' AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                    //              AND  CS.APPLY_ON = 'D' ORDER BY CS.SERIAL_NO ASC";

                    query = $@" SELECT DISTINCT CS.CHARGE_CODE, IC.CHARGE_EDESC, CS.CHARGE_TYPE_FLAG, CS.CALCULATE_BY VALUE_PERCENT_FLAG, ROUND(CS.CHARGE_AMOUNT,2)CHARGE_AMOUNT,0 VALUE_PERCENT_AMOUNT, CS.SERIAL_NO PRIORITY_INDEX_NO,CS.ACC_CODE,
                               CS.APPLY_ON, CS.GL_FLAG,CS.NON_GL_FLAG FROM CHARGE_TRANSACTION CS, IP_CHARGE_CODE IC 
                                  WHERE CS.CHARGE_CODE = IC.CHARGE_CODE AND CS.COMPANY_CODE = IC.COMPANY_CODE AND  CS.FORM_CODE= '{formCode}' AND REFERENCE_NO = '{voucherNo}' AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                                  AND  CS.APPLY_ON = 'D'  AND cs.serial_no IS NOT NULL ORDER BY CS.SERIAL_NO ASC";
                    _logErp.InfoInFile("Query to get charges on sales data contains:  " + query);
                    result = _dbContext.SqlQuery<ChargeOnSales>(query).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting charges data : " + ex.Message);
                throw ex;
            }
        }
        public List<ChargeOnSales> GetChargesData(string formCode)
        {
            try
            {
                string query = string.Empty;
                var result = new List<ChargeOnSales>();
                query = $@"SELECT DISTINCT CS.CHARGE_CODE, IC.CHARGE_EDESC, CS.CHARGE_TYPE_FLAG, CS.VALUE_PERCENT_FLAG,0 CHARGE_AMOUNT, CS.VALUE_PERCENT_AMOUNT, CS.PRIORITY_INDEX_NO,CA.ACC_CODE, CA.ACC_EDESC,CS.GL_FLAG,CS.NON_GL_FLAG,
                                  CS.CHARGE_APPLY_ON, CS.APPLY_FROM_DATE,CS.APPLY_TO_DATE,  CS.APPLY_ON,  CS.charge_active_flag FROM CHARGE_SETUP CS, IP_CHARGE_CODE IC ,FA_CHART_OF_ACCOUNTS_SETUP CA
                                  WHERE  CS.CHARGE_CODE = IC.CHARGE_CODE AND CS.COMPANY_CODE = IC.COMPANY_CODE  AND CS.ACC_CODE=CA.ACC_CODE(+) AND CS.COMPANY_CODE=CA.COMPANY_CODE(+) AND  CS.FORM_CODE= '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND CS.APPLY_ON = 'D' ORDER BY CS.PRIORITY_INDEX_NO ASC";
                _logErp.InfoInFile("Query to get charge data for given formcode include : " + query);
                result = _dbContext.SqlQuery<ChargeOnSales>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting charge for sales : " + ex.Message);
                throw ex;
            }
        }

        public List<FinancialBudgetTransaction> Getbudgetdetail(string voucherno)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"select ACC_CODE, BUDGET_FLAG, BUDGET_CODE,BUDGET_AMOUNT as QUANTITY,PARTICULARS as NARRATION,SERIAL_NO  from budget_transaction where REFERENCE_NO = '{voucherno}'";
            var entity = this._dbContext.SqlQuery<FinancialBudgetTransaction>(Query).ToList();
            return entity;
        }
        public List<BATCH_TRANSACTION_DATA> Getbatchtrackingdetail(string voucherno)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"SELECT distinct ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.BATCH_NO AS TRACKING_SERIAL_NO,BT.EXPIRY_DATE FROM BATCH_TRANSACTION BT,IP_ITEM_MASTER_SETUP ISS 
WHERE ISS.ITEM_CODE=BT.ITEM_CODE AND 
ISS.COMPANY_CODE=BT.COMPANY_CODE AND
BT.REFERENCE_NO='{voucherno}' AND
BT.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND
BT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND BT.BATCH_SERIAL_FLAG='Y'";
            var entity = this._dbContext.SqlQuery<BATCH_TRANSACTION_DATA>(Query).ToList();
            return entity;
        }
        public List<BATCHTRANSACTIONDATA> Getbatchdetail(string voucherno)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"SELECT ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.ITEM_SERIAL_NO AS TRACKING_SERIAL_NO FROM BATCH_TRANSACTION BT,IP_ITEM_MASTER_SETUP ISS 
WHERE ISS.ITEM_CODE=BT.ITEM_CODE AND 
ISS.COMPANY_CODE=BT.COMPANY_CODE AND
BT.REFERENCE_NO='{voucherno}' AND
BT.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND
BT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND BT.ITEM_SERIAL_FLAG='Y'";
            var entity = this._dbContext.SqlQuery<BATCHTRANSACTIONDATA>(Query).ToList();
            return entity;
        }
        public string GetLoactionNameByCode(string code)
        {
            try
            {
                if (code != "undefined" && code != "null")
                {
                    string ACCQuery = $@"SELECT LOCATION_EDESC FROM ip_location_setup WHERE LOCATION_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  AND DELETED_FLAG='N'";
                    var ACCdata = this._dbContext.SqlQuery<string>(ACCQuery).FirstOrDefault().ToString();
                    return string.IsNullOrEmpty(ACCdata) ? "" : ACCdata;

                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }


        }
        public List<CITY> GetAllCityDetailsByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<CITY> CityList = new List<CITY>();

                //string query = string.Format(@"
                // select 
                //    COALESCE(CITY_CODE,' ') as CITY_CODE
                //    ,COALESCE(CITY_EDESC,' ') as CITY_EDESC
                //    from CITY_CODE
                //    where deleted_flag='N'                    
                //    and
                //    (
                //        upper(CITY_CODE) like '%{0}%'
                //        or upper(CITY_EDESC) like '%{0}%'

                //    ) order by CITY_CODE", filter.ToUpperInvariant());
                //CityList = this._dbContext.SqlQuery<CITY>(query).ToList();
                //return CityList;
                string query = string.Format(@"
                 select 
                    COALESCE(CITY_CODE,' ') as CITY_CODE
                    ,COALESCE(CITY_EDESC,' ') as CITY_EDESC
                    from CITY_CODE
                    where                                       
                    (
                        upper(CITY_CODE) like '%{0}%'
                        or upper(CITY_EDESC) like '%{0}%'
                     
                    )  AND DELETED_FLAG='N' order by CITY_CODE", filter.ToUpperInvariant());
                CityList = this._dbContext.SqlQuery<CITY>(query).ToList();
                return CityList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<VECHILES> GetAllVechDetailsByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<VECHILES> VechilesList = new List<VECHILES>();

                string query = string.Format(@"
select 
                    COALESCE(VEHICLE_CODE,' ') as VEHICLE_CODE
                    ,COALESCE(VEHICLE_EDESC,' ') as VEHICLE_EDESC
                  ,COALESCE(VEHICLE_TYPE,' ') as VEHICLE_TYPE
                     ,COALESCE(VEHICLE_ID,' ') as VEHICLE_ID
                      ,COALESCE(DRIVER_NAME,' ') as DRIVER_NAME
                       ,COALESCE(DRIVER_LICENCE_NO,' ') as DRIVER_LICENCE_NO
                      ,COALESCE(DRIVER_MOBILE_NO,' ') as DRIVER_MOBILE_NO
                         ,COALESCE(OWNER_NAME,' ') as VEHICLE_OWNER_NAME
                         ,COALESCE(OWNER_MOBILE_NO,' ') as VEHICLE_OWNER_NO 
                    from IP_VEHICLE_CODE
                    where deleted_flag='N'
                    AND company_code='{1}'
                    and
                    (
                        upper(VEHICLE_CODE) like '%{0}%'
                        or upper(VEHICLE_EDESC) like '%{0}%'
                     
                    ) order by VEHICLE_CODE", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
                VechilesList = this._dbContext.SqlQuery<VECHILES>(query).ToList();
                return VechilesList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<TRANSPORTER> GetAllTransporterDetailsByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<TRANSPORTER> TransporterList = new List<TRANSPORTER>();

                string query = string.Format(@"
                 select 
                    COALESCE(TRANSPORTER_CODE,' ') as TRANSPORTER_CODE
                    ,COALESCE(TRANSPORTER_EDESC,' ') as TRANSPORTER_EDESC
                    from TRANSPORTER_SETUP
                    where deleted_flag='N'
                    AND company_code='{1}'
                    and
                    (
                        upper(TRANSPORTER_CODE) like '%{0}%'
                        or upper(TRANSPORTER_EDESC) like '%{0}%'
                     
                    ) order by TRANSPORTER_CODE", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
                TransporterList = this._dbContext.SqlQuery<TRANSPORTER>(query).ToList();
                return TransporterList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public string NewVoucherNo(string companycode, string formcode, string transactiondate, string tablename)
        //{
        //    try
        //    {
        //        if (companycode != "" && formcode != "" && transactiondate != "" && tablename != "")
        //        {
        //            string query = string.Format(@"select FN_NEW_VOUCHER_NO('{0}','{1}','{2}','{3}') FROM DUAL", companycode, formcode, transactiondate, tablename);
        //            string voucherNo = this._dbContext.SqlQuery<string>(query).First();
        //            return voucherNo;
        //        }
        //        else
        //        { return ""; }

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public string GetNewSequence()
        {
            var query = "select MYSEQUENCE .NEXTVAL from dual";
            _logErp.InfoInFile("GetNewSequence ====================================:" + _dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString());
            return _dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString();
        }
        public bool ItemNoExistsOrNot(string itemcode)
        {
            try
            {
                var Count_query = $@"select count(item_code) from ip_item_master_setup where item_code='{itemcode}' and serial_flag='Y' and company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N'";
                int Result = _dbContext.SqlQuery<int>(Count_query).FirstOrDefault();
                if (Result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public bool BatchItemNoExistsOrNot(string itemcode)
        {
            try
            {
                var Count_query = $@"select count(item_code) from ip_item_master_setup where item_code='{itemcode}' and batch_serial_flag='Y' and company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N'";
                int Result = _dbContext.SqlQuery<int>(Count_query).FirstOrDefault();
                if (Result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
            }



        }
        public List<Division> GetAllDivisionSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Division> DivisionList = new List<Division>();
                string query = string.Format(@"Select
                        COALESCE(DIVISION_CODE,' ') as DIVISION_CODE, 
                        COALESCE(DIVISION_EDESC,' ') as DIVISION_EDESC 
                        from FA_DIVISION_SETUP
                        where DELETED_FLAG='N' and COMPANY_CODE='{1}'and GROUP_SKU_FLAG = 'I'
                        and(upper(DIVISION_EDESC) like '%{0}%' or upper(DIVISION_CODE) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                DivisionList = this._dbContext.SqlQuery<Division>(query).ToList();
                return DivisionList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Customers> getALLSupplierListByFlterForReference(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = string.Format(@"SELECT DISTINCT
                                    COALESCE(SUPPLIER_CODE,' ') CustomerCode,
                                    COALESCE(SUPPLIER_EDESC,' ') CustomerName,
                                    COALESCE(REGD_OFFICE_EADDRESS,' ') REGD_OFFICE_EADDRESS,
                                    COALESCE(TEL_MOBILE_NO1,' ') TEL_MOBILE_NO1,
                                    '' as Type
                                    from IP_SUPPLIER_SETUP
                                    where deleted_flag='N' and GROUP_SKU_FLAG='I' and SUPPLIER_CODE like '%{0}%' and COMPANY_CODE='{1}'
                                    or upper(SUPPLIER_EDESC)like '%{0}%' order by CustomerCode", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                var result = _dbContext.SqlQuery<Customers>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Suppliers> getALLSupplierListByFlter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = string.Format(@"SELECT DISTINCT
                                    COALESCE(SUPPLIER_CODE,' ') SUPPLIER_CODE,
                                    COALESCE(SUPPLIER_EDESC,' ') SUPPLIER_EDESC,
                                    COALESCE(REGD_OFFICE_EADDRESS,' ') REGD_OFFICE_EADDRESS,
                                    COALESCE(TEL_MOBILE_NO1,' ') TEL_MOBILE_NO1,
                                   COALESCE(LINK_SUB_CODE,' ')  LINK_SUB_CODE,
                                    '' as Type
                                    from IP_SUPPLIER_SETUP
                                    where deleted_flag='N' and GROUP_SKU_FLAG='I' and (SUPPLIER_CODE like '%{0}%' and COMPANY_CODE='{1}'
                                    or upper(SUPPLIER_EDESC)like '%{0}%') order by SUPPLIER_CODE", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                var result = _dbContext.SqlQuery<Suppliers>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Currency> getCurrencyListByFlter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }



                var sqlquery = $@"SELECT CS.CURRENCY_CODE AS CURRENCY_CODE,CS.CURRENCY_EDESC AS CURRENCY_EDESC,NVL(EXCHANGE_RATE,1) AS EXCHANGE_RATE  FROM CURRENCY_SETUP CS,(SELECT DISTINCT CURRENCY_CODE, EXCHANGE_RATE, COMPANY_CODE FROM EXCHANGE_DETAIL_SETUP A WHERE EXCHANGE_DATE = (SELECT MAX(EXCHANGE_DATE) FROM EXCHANGE_DETAIL_SETUP WHERE CURRENCY_CODE = A.CURRENCY_CODE )) T WHERE CS.CURRENCY_CODE = T.CURRENCY_CODE(+) AND CS.COMPANY_CODE = T.COMPANY_CODE(+) AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                            AND CS.deleted_flag='N' AND (CS.CURRENCY_CODE like '%{filter.ToUpperInvariant()}%' 
                            OR upper(CS.CURRENCY_EDESC) like '%{filter.ToUpperInvariant()}%')";
                var result = _dbContext.SqlQuery<Currency>(sqlquery).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<AccountSetup> getALLAccountSetupByFlter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<AccountSetup> result = new List<AccountSetup>();
            string query = string.Format(@"Select
                        COALESCE(ACC_CODE,' ') as ACC_CODE, 
                        COALESCE(ACC_EDESC,' ') as ACC_EDESC 
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        where DELETED_FLAG='N' AND FREEZE_FLAG='N' and COMPANY_CODE='{1}' and ACC_TYPE_FLAG='T' and FREEZE_FLAG='N'
                        and (ACC_CODE ='{0}' or upper(ACC_EDESC) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
            result = this._dbContext.SqlQuery<AccountSetup>(query).ToList();
            return result;


        }
        public List<AccountSetup> getALLAccountForInvBudgetTrans()
        {

            List<AccountSetup> result = new List<AccountSetup>();
            //string query = string.Format(@"Select
            //            COALESCE(ACC_CODE,' ') as ACC_CODE, 
            //            COALESCE(ACC_EDESC,' ') as ACC_EDESC 
            //            FROM FA_CHART_OF_ACCOUNTS_SETUP
            //            where DELETED_FLAG='N' and COMPANY_CODE='{1}' 
            //            and (ACC_CODE ='{0}' or upper(ACC_EDESC) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);

            string query = $@"Select
                        COALESCE(ACC_CODE,' ') as ACC_CODE, 
                        COALESCE(ACC_EDESC,' ') as ACC_EDESC 
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        where DELETED_FLAG='N' AND FREEZE_FLAG='N' and ACC_TYPE_FLAG='T' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
            result = this._dbContext.SqlQuery<AccountSetup>(query).ToList();
            return result;


        }
        public string getSubledgerCodeByAccCode(string accCode)
        {
            var result = string.Empty;
            string query = $@"SELECT TO_CHAR(count(*))
                        FROM FA_SUB_LEDGER_MAP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}' AND ACC_CODE = '{accCode}'";
            result = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }
        public List<BudgetCenter> GetAllBudgetCenterForLocationByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<BudgetCenter> result = new List<BudgetCenter>();
            string query = $@"SELECT
                        COALESCE(BUDGET_CODE,' ') as BUDGET_CODE, 
                        COALESCE(BUDGET_EDESC,' ') as BUDGET_EDESC 
                        FROM BC_BUDGET_CENTER_SETUP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                        and(upper(BUDGET_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(BUDGET_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<BudgetCenter>(query).ToList();
            return result;
        }
        public List<SubLedger> GetAllSubLedgerByFilter(string filter, string accCode)
        {
            var condition = $@" AND SUB_CODE IN (SELECT
                        COALESCE(SUB_CODE,' ') as SUB_CODE
                        FROM FA_SUB_LEDGER_MAP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' AND  ACC_CODE = '{accCode}')";
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<SubLedger> result = new List<SubLedger>();
            string query = $@"SELECT
                        COALESCE(SUB_CODE,' ') as SUB_CODE, 
                        COALESCE(SUB_EDESC,' ') as SUB_EDESC 
                        FROM FA_SUB_LEDGER_SETUP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'  {condition}
                        and(upper(SUB_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(SUB_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<SubLedger>(query).ToList();
            return result;
        }
        public List<TemplateDraftModel> GetDraftList(string modulecode, string formCode)
        {
            try
            {
                string query = $@"SELECT FT.FORM_CODE,FT.TEMPLATE_NO TEMPLATE_CODE, FT.TEMPLATE_EDESC,  FS.MODULE_CODE FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='{modulecode}' AND FS.FORM_CODE='{formCode}'
                  AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                  AND FT.ASSIGNEE='{_workContext.CurrentUserinformation.User_id}' ORDER BY TO_NUMBER(FT.FORM_CODE) ASC";
                var result = _dbContext.SqlQuery<TemplateDraftModel>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string InsertQuickSetup(QuickSetupModel model)
        {
            using (var trans = _coreEntity.Database.BeginTransaction())
            {
                try
                {
                    if (model.FLAG == "C")
                    {
                        var newmaxitemcode = string.Empty;
                        var newmaxitemcodequery = $@"SELECT MAX(TO_NUMBER(CUSTOMER_CODE))+1 as MASTER_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP";
                        newmaxitemcode = this._coreEntity.SqlQuery<int>(newmaxitemcodequery).FirstOrDefault().ToString();
                        var newprecustomercode = model.MASTER_CODE;
                        var newmastercustomercode = model.MASTER_CODE + "." + "00";
                        var customersetupquery = $@"INSERT INTO SA_CUSTOMER_SETUP (CUSTOMER_CODE,
                                  CUSTOMER_EDESC,CUSTOMER_NDESC,
                                   REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,EMAIL,
                                  GROUP_SKU_FLAG,
                                  MASTER_CUSTOMER_CODE,
                                  PRE_CUSTOMER_CODE,
                                  COMPANY_CODE,
                                  CREATED_BY,
                                  CREATED_DATE,
                                  DELETED_FLAG
                                 )
                                VALUES('{newmaxitemcode}','{model.ENG_NAME}','{model.NEP_NAME}','{model.REGD_OFFICE_EADDRESS}','{model.TEL_MOBILE_NO1}','{model.EMAIL}','I','{newmastercustomercode}',
                                       '{newprecustomercode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),                                      'N')";

                        var insertcustomersetup = _coreEntity.ExecuteSqlCommand(customersetupquery);

                        return "C_SUCCESS";


                    }
                    else if (model.FLAG == "I")
                    {
                        var newmaxitemcode = string.Empty;
                        var newmaxitemcodequery = $@"SELECT MAX(TO_NUMBER(ITEM_CODE))+1 as MASTER_ITEM_CODE FROM IP_ITEM_MASTER_SETUP";
                        newmaxitemcode = this._coreEntity.SqlQuery<int>(newmaxitemcodequery).FirstOrDefault().ToString();
                        var newpreitemcode = model.MASTER_CODE;
                        var newmasteritemcode = model.MASTER_CODE + "." + "00";
                        var suppliersetupquery = $@"INSERT INTO IP_ITEM_MASTER_SETUP (ITEM_CODE,
                                  ITEM_EDESC,ITEM_NDESC,
                                    CATEGORY_CODE,INDEX_MU_CODE,
                                  GROUP_SKU_FLAG,
                                  MASTER_ITEM_CODE,
                                  PRE_ITEM_CODE,
                                  COMPANY_CODE,
                                  CREATED_BY,
                                  CREATED_DATE,
                                  DELETED_FLAG
                                 )
                                VALUES('{newmaxitemcode}','{model.ENG_NAME}','{model.NEP_NAME}','{model.CATEGORY_CODE}','{model.INDEX_MU_CODE}','I','{newmasteritemcode}',
                                       '{newpreitemcode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),                                      'N')";

                        var insertsuppliersetup = _coreEntity.ExecuteSqlCommand(suppliersetupquery);

                        return "I_SUCCESS";
                    }
                    else if (model.FLAG == "S")
                    {
                        var newmaxitemcode = string.Empty;
                        var newmaxitemcodequery = $@"SELECT MAX(TO_NUMBER(SUPPLIER_CODE))+1 as MASTER_SUPPLIER_CODE FROM IP_SUPPLIER_SETUP";
                        newmaxitemcode = this._coreEntity.SqlQuery<int>(newmaxitemcodequery).FirstOrDefault().ToString();

                        var newpresuppliercode = model.MASTER_CODE;
                        var newmastersuppliercode = model.MASTER_CODE + "." + "00";
                        var suppliersetupquery = $@"INSERT INTO IP_SUPPLIER_SETUP (SUPPLIER_CODE,
                                  SUPPLIER_EDESC,SUPPLIER_NDESC,
                                    REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,EMAIL,
                                  GROUP_SKU_FLAG,
                                  MASTER_SUPPLIER_CODE,
                                  PRE_SUPPLIER_CODE,
                                  COMPANY_CODE,
                                  CREATED_BY,
                                  CREATED_DATE,
                                  DELETED_FLAG
                                 )
                                VALUES('{newmaxitemcode}','{model.ENG_NAME}','{model.NEP_NAME}','{model.REGD_OFFICE_EADDRESS}','{model.TEL_MOBILE_NO1}','{model.EMAIL}','I','{newmastersuppliercode}',
                                       '{newpresuppliercode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N')";

                        var insertsuppliersetup = _coreEntity.ExecuteSqlCommand(suppliersetupquery);

                        return "S_SUCCESS";
                    }
                    trans.Commit();
                    return "INSERTED";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }



        }
        public string GetPrimaryDateByTableName(string tablename)
        {
            var primarycolumn = "";
            if (tablename == "FA_SINGLE_VOUCHER" || tablename == "FA_DOUBLE_VOUCHER" || tablename == "FA_JOB_ORDER")
            {
                primarycolumn = "VOUCHER_DATE";
            }
            else if (tablename == "IP_PURCHASE_MRR" || tablename == "IP_ADVICE_MRR" || tablename == "IP_PRODUCTION_MRR")
            {
                primarycolumn = "MRR_DATE";
            }
            else if (tablename == "IP_PURCHASE_REQUEST")
            {
                primarycolumn = "REQUEST_DATE";
            }
            else if (tablename == "IP_PURCHASE_INVOICE")
            {
                primarycolumn = "INVOICE_DATE";
            }
            else if (tablename == "IP_PURCHASE_RETURN" || tablename == "SA_SALES_RETURN")
            {
                primarycolumn = "RETURN_DATE";
            }
            else if (tablename == "IP_GOODS_REQUISITION")
            {
                primarycolumn = "REQUISITION_DATE";
            }
            else if (tablename == "IP_QUOTATION_INQUIRY")
            {
                primarycolumn = "QUOTE_DATE";
            }
            else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY" || tablename == "IP_RETURNABLE_GOODS_RETURN" || tablename == "IP_RETURNABLE_GOODS_ISSUE")
            {
                primarycolumn = "ISSUE_DATE";
            }
            else if (tablename == "IP_PURCHASE_ORDER" || tablename == "SA_SALES_ORDER")
            {
                primarycolumn = "ORDER_DATE";
            }
            else if (tablename == "SA_SALES_CHALAN")
            {
                primarycolumn = "CHALAN_DATE";
            }
            else if (tablename == "SA_SALES_INVOICE")
            {
                primarycolumn = "SALES_DATE";
            }
            else if (tablename == "FA_ADJUSTMENT_NOTE")
            {
                primarycolumn = "NOTE_DATE";
            }

            return primarycolumn;
        }
        public string GetPrimaryColumnByTableName(string tablename)
        {

            var primarycolumn = string.Empty;
            if (tablename == "FA_SINGLE_VOUCHER" || tablename == "FA_DOUBLE_VOUCHER" || tablename == "FA_PAY_ORDER")
            {
                primarycolumn = "VOUCHER_NO";
            }
            else if (tablename == "IP_PURCHASE_MRR" || tablename == "IP_ADVICE_MRR" || tablename == "IP_PRODUCTION_MRR")
            {
                primarycolumn = "MRR_NO";
            }
            else if (tablename == "IP_PURCHASE_REQUEST")
            {
                primarycolumn = "REQUEST_NO";
            }
            else if (tablename == "IP_PURCHASE_INVOICE")
            {
                primarycolumn = "INVOICE_NO";
            }
            else if (tablename == "IP_PURCHASE_RETURN" || tablename == "SA_SALES_RETURN" || tablename == "IP_GOODS_ISSUE_RETURN")
            {
                primarycolumn = "RETURN_NO";
            }
            else if (tablename == "IP_GOODS_REQUISITION")
            {
                primarycolumn = "REQUISITION_NO";
            }
            else if (tablename == "IP_QUOTATION_INQUIRY")
            {
                primarycolumn = "QUOTE_NO";
            }
            else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY" || tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_PRODUCTION_ISSUE" || tablename == "IP_RETURNABLE_GOODS_ISSUE" || tablename == "IP_RETURNABLE_GOODS_RETURN")
            {
                primarycolumn = "ISSUE_NO";
            }
            else if (tablename == "IP_PURCHASE_ORDER" || tablename == "SA_SALES_ORDER")
            {
                primarycolumn = "ORDER_NO";
            }
            else if (tablename == "SA_SALES_CHALAN")
            {
                primarycolumn = "CHALAN_NO";
            }
            else if (tablename == "SA_SALES_INVOICE")
            {
                primarycolumn = "SALES_NO";
            }

            return primarycolumn;
        }
        public Inventory MapMasterColumnWithValue(string masterColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<Inventory>(masterColumn);
                return masterColVal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<Inventory> MapChildColumnWithValue(string childColumn)
        {
            try
            {
                var childColVal = JsonConvert.DeserializeObject<List<Inventory>>(childColumn);
                return childColVal;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<CustomOrderColumn> MapCustomTransactionWithValue(string customTransaction)
        {
            try
            {
                // var customColVal = JsonConvert.DeserializeObject<CustomOrderColumn>(custom_col_val.Replace(' ','_'));
                var customCol = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(customTransaction);
                // var customColDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(custom_col_val);
                CustomOrderColumn customOrderCol = null;
                var customOrderColList = new List<CustomOrderColumn>();
                foreach (var cc in customCol)
                {
                    customOrderCol = new CustomOrderColumn
                    {
                        FieldName = cc.Key,
                        FieldValue = cc.Value.ToString(),
                    };
                    customOrderColList.Add(customOrderCol);
                }
                return customOrderColList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<FinancialBudgetTransaction> MapBudgetTransactionColumnValue(string transactionValue)
        {
            try
            {
                List<FinancialBudgetTransaction> fa = null;
                if (transactionValue != null) fa = JsonConvert.DeserializeObject<List<FinancialBudgetTransaction>>(transactionValue);

                return fa;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<BATCHTRANSACTIONDATA> MapBatchTransactionValue(string batchValue)
        {
            try
            {
                List<BATCHTRANSACTIONDATA> fa = null;
                if (batchValue != null) fa = JsonConvert.DeserializeObject<List<BATCHTRANSACTIONDATA>>(batchValue);

                return fa;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<BATCH_TRANSACTION_DATA> MapBatchTransValue(string batchTransValue)
        {
            try
            {
                List<BATCH_TRANSACTION_DATA> fa = null;
                if (batchTransValue != null) fa = JsonConvert.DeserializeObject<List<BATCH_TRANSACTION_DATA>>(batchTransValue);

                return fa;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }


        }
        public List<ChargeOnSales> MapChargesColumnWithValue(string charges)
        {
            try
            {
                var chargesCol = JsonConvert.DeserializeObject<List<ChargeOnSales>>(charges);
                return chargesCol;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
        public ShippingDetails MapShippingDetailsColumnValue(string shippingDetails)
        {
            try
            {
                var shippingCol = JsonConvert.DeserializeObject<ShippingDetails>(shippingDetails);
                // var shippingColDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(shippingDetails);
                return shippingCol;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool SaveChildColumnValue(List<Inventory> childColumnValue, Inventory masterColumnValue, CommonFieldsForInventory commonValue, FormDetails model, string primarydatecolumn, string primarycolname, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                int serialno = 1;
                bool insertedToChild = false;
                Inventory inventoryChildDetails = new Inventory();
                foreach (var childCol in childColumnValue)
                {
                    inventoryChildDetails.VOUCHER_NO = (commonValue.VoucherNumber == "undefined") ? commonValue.NewVoucherNumber : commonValue.VoucherNumber;
                    inventoryChildDetails.MANUAL_NO = string.IsNullOrEmpty(childCol.MANUAL_NO) ? masterColumnValue.MANUAL_NO : childCol.MANUAL_NO;
                    inventoryChildDetails.SERIAL_NO = string.IsNullOrEmpty(childCol.SERIAL_NO) ? serialno.ToString() : childCol.SERIAL_NO;
                    inventoryChildDetails.TO_LOCATION_CODE = string.IsNullOrEmpty(childCol.TO_LOCATION_CODE) ? masterColumnValue.TO_LOCATION_CODE : childCol.TO_LOCATION_CODE;
                    inventoryChildDetails.REMARKS = string.IsNullOrEmpty(childCol.REMARKS) ? masterColumnValue.REMARKS : childCol.REMARKS;
                    inventoryChildDetails.CALC_TOTAL_PRICE = childCol.CALC_TOTAL_PRICE;
                    inventoryChildDetails.CALC_UNIT_PRICE = childCol.CALC_UNIT_PRICE;
                    inventoryChildDetails.CALC_QUANTITY = childCol.CALC_QUANTITY;
                    inventoryChildDetails.TOTAL_PRICE = childCol.TOTAL_PRICE;
                    inventoryChildDetails.UNIT_PRICE = childCol.UNIT_PRICE;
                    inventoryChildDetails.SUB_PROJECT_CODE = childCol.SUB_PROJECT_CODE;
                    inventoryChildDetails.QUANTITY = childCol.QUANTITY;
                    inventoryChildDetails.MU_CODE = string.IsNullOrEmpty(childCol.MU_CODE) ? masterColumnValue.MU_CODE : childCol.MU_CODE;
                    inventoryChildDetails.ITEM_CODE = string.IsNullOrEmpty(childCol.ITEM_CODE) ? masterColumnValue.ITEM_CODE : childCol.ITEM_CODE;

                    if (string.IsNullOrEmpty(masterColumnValue.MODIFY_DATE))
                    {
                        inventoryChildDetails.MODIFY_DATE = "''";
                    }
                    else
                    {
                        inventoryChildDetails.MODIFY_DATE = masterColumnValue.MODIFY_DATE;
                    }
                    if (string.IsNullOrEmpty(masterColumnValue.MODIFY_BY))
                    {
                        inventoryChildDetails.MODIFY_BY = "";
                    }
                    else
                    {
                        inventoryChildDetails.MODIFY_BY = masterColumnValue.MODIFY_BY;
                    }
                    inventoryChildDetails.DELETED_FLAG = "N";
                    inventoryChildDetails.CREATED_DATE = commonValue.VoucherDate;
                    inventoryChildDetails.CREATED_BY = string.IsNullOrEmpty(childCol.CREATED_BY) ? masterColumnValue.CREATED_BY : _workContext.CurrentUserinformation.login_code.ToUpper();
                    inventoryChildDetails.BRANCH_CODE = string.IsNullOrEmpty(childCol.BRANCH_CODE) ? _workContext.CurrentUserinformation.branch_code : childCol.BRANCH_CODE;
                    inventoryChildDetails.MRR_NO = string.IsNullOrEmpty(childCol.MRR_NO) ? masterColumnValue.MRR_NO : childCol.MRR_NO;
                    inventoryChildDetails.COMPLETED_QUANTITY = childCol.COMPLETED_QUANTITY;
                    inventoryChildDetails.FORM_CODE = string.IsNullOrEmpty(commonValue.FormCode) ? masterColumnValue.FORM_CODE : commonValue.FormCode;
                    inventoryChildDetails.COMPANY_CODE = string.IsNullOrEmpty(childCol.COMPANY_CODE) ? _workContext.CurrentUserinformation.company_code : childCol.COMPANY_CODE;


                    inventoryChildDetails.SYN_ROWID = string.IsNullOrEmpty(childCol.SYN_ROWID) ? masterColumnValue.SYN_ROWID : childCol.SYN_ROWID;
                    inventoryChildDetails.SESSION_ROWID = string.IsNullOrEmpty(childCol.SESSION_ROWID) ? masterColumnValue.SESSION_ROWID : childCol.SESSION_ROWID;
                    inventoryChildDetails.DIVISION_CODE = string.IsNullOrEmpty(childCol.DIVISION_CODE) ? masterColumnValue.DIVISION_CODE : childCol.DIVISION_CODE;
                    inventoryChildDetails.INVOICE_NO = string.IsNullOrEmpty(childCol.INVOICE_NO) ? masterColumnValue.INVOICE_NO : childCol.INVOICE_NO;
                    inventoryChildDetails.INVOICE_DATE = string.IsNullOrEmpty(childCol.INVOICE_DATE) ? masterColumnValue.INVOICE_DATE : childCol.INVOICE_DATE;
                    inventoryChildDetails.SUPPLIER_CODE = string.IsNullOrEmpty(childCol.SUPPLIER_CODE) ? masterColumnValue.SUPPLIER_CODE : childCol.SUPPLIER_CODE;
                    inventoryChildDetails.SUPPLIER_INV_NO = string.IsNullOrEmpty(childCol.SUPPLIER_INV_NO) ? masterColumnValue.SUPPLIER_INV_NO : childCol.SUPPLIER_INV_NO;
                    inventoryChildDetails.SUPPLIER_INV_DATE = string.IsNullOrEmpty(childCol.SUPPLIER_INV_DATE) ? (masterColumnValue.SUPPLIER_INV_DATE == "Invalid date" ? null : masterColumnValue.SUPPLIER_INV_DATE) : childCol.SUPPLIER_INV_DATE;
                    inventoryChildDetails.SUPPLIER_BUDGET_FLAG = string.IsNullOrEmpty(childCol.SUPPLIER_BUDGET_FLAG) ? masterColumnValue.SUPPLIER_BUDGET_FLAG : childCol.SUPPLIER_BUDGET_FLAG;
                    inventoryChildDetails.BUDGET_FLAG = string.IsNullOrEmpty(inventoryChildDetails.BUDGET_FLAG) ? inventoryChildDetails.BUDGET_FLAG : inventoryChildDetails.BUDGET_FLAG;
                    inventoryChildDetails.DUE_DATE = string.IsNullOrEmpty(childCol.DUE_DATE) ? (masterColumnValue.DUE_DATE == "Invalid date" ? null : masterColumnValue.DUE_DATE) : childCol.DUE_DATE;
                    inventoryChildDetails.CURRENCY_CODE = string.IsNullOrEmpty(childCol.CURRENCY_CODE) ? "NRS" : masterColumnValue.CURRENCY_CODE;
                    inventoryChildDetails.EXCHANGE_RATE = string.IsNullOrEmpty(childCol.EXCHANGE_RATE) ? "1" : masterColumnValue.EXCHANGE_RATE;
                    inventoryChildDetails.TERMS_DAY = string.IsNullOrEmpty(childCol.TERMS_DAY) ? masterColumnValue.TERMS_DAY : childCol.TERMS_DAY;
                    inventoryChildDetails.TRACKING_NO = string.IsNullOrEmpty(childCol.TRACKING_NO) ? masterColumnValue.TRACKING_NO : childCol.TRACKING_NO;
                    inventoryChildDetails.BATCH_NO = string.IsNullOrEmpty(childCol.BATCH_NO) ? masterColumnValue.BATCH_NO : childCol.BATCH_NO;
                    inventoryChildDetails.LOT_NO = string.IsNullOrEmpty(childCol.LOT_NO) ? masterColumnValue.LOT_NO : childCol.LOT_NO;
                    inventoryChildDetails.SUPPLIER_MRR_NO = string.IsNullOrEmpty(childCol.SUPPLIER_MRR_NO) ? masterColumnValue.SUPPLIER_MRR_NO : childCol.SUPPLIER_MRR_NO;
                    inventoryChildDetails.PP_NO = string.IsNullOrEmpty(childCol.PP_NO) ? masterColumnValue.PP_NO : childCol.PP_NO;
                    inventoryChildDetails.P_TYPE = string.IsNullOrEmpty(childCol.P_TYPE) ? masterColumnValue.P_TYPE : childCol.P_TYPE;
                    inventoryChildDetails.PP_DATE = string.IsNullOrEmpty(childCol.PP_DATE) ? masterColumnValue.PP_DATE : childCol.PP_DATE;
                    inventoryChildDetails.NET_GROSS_RATE = string.IsNullOrEmpty(childCol.NET_GROSS_RATE) ? masterColumnValue.NET_GROSS_RATE : childCol.NET_GROSS_RATE;
                    inventoryChildDetails.NET_SALES_RATE = string.IsNullOrEmpty(childCol.NET_SALES_RATE) ? masterColumnValue.NET_SALES_RATE : childCol.NET_SALES_RATE;
                    inventoryChildDetails.NET_TAXABLE_RATE = string.IsNullOrEmpty(childCol.NET_TAXABLE_RATE) ? masterColumnValue.NET_TAXABLE_RATE : childCol.NET_TAXABLE_RATE;
                    inventoryChildDetails.MASTER_PP_NO = string.IsNullOrEmpty(childCol.MASTER_PP_NO) ? masterColumnValue.MASTER_PP_NO : childCol.MASTER_PP_NO;
                    inventoryChildDetails.SECOND_QUANTITY = string.IsNullOrEmpty(childCol.SECOND_QUANTITY) ? masterColumnValue.SECOND_QUANTITY : childCol.SECOND_QUANTITY;
                    inventoryChildDetails.THIRD_QUANTITY = string.IsNullOrEmpty(childCol.THIRD_QUANTITY) ? masterColumnValue.THIRD_QUANTITY : childCol.THIRD_QUANTITY;
                    inventoryChildDetails.RECONCILE_DATE = string.IsNullOrEmpty(childCol.RECONCILE_DATE) ? masterColumnValue.RECONCILE_DATE : childCol.RECONCILE_DATE;
                    inventoryChildDetails.RECONCILE_FLAG = string.IsNullOrEmpty(childCol.RECONCILE_FLAG) ? masterColumnValue.RECONCILE_FLAG : childCol.RECONCILE_FLAG;
                    inventoryChildDetails.RECONCILE_BY = string.IsNullOrEmpty(childCol.RECONCILE_BY) ? masterColumnValue.RECONCILE_BY : childCol.RECONCILE_BY;
                    inventoryChildDetails.PHOTO_FILE_NAME1 = string.IsNullOrEmpty(childCol.PHOTO_FILE_NAME1) ? masterColumnValue.PHOTO_FILE_NAME1 : childCol.PHOTO_FILE_NAME1;
                    inventoryChildDetails.PHOTO_FILE_NAME2 = string.IsNullOrEmpty(childCol.PHOTO_FILE_NAME2) ? masterColumnValue.PHOTO_FILE_NAME2 : childCol.PHOTO_FILE_NAME2;
                    inventoryChildDetails.SPECIFICATION = string.IsNullOrEmpty(childCol.SPECIFICATION) ? masterColumnValue.SPECIFICATION : childCol.SPECIFICATION;
                    inventoryChildDetails.SUPPLIER_MRR_DATE = string.IsNullOrEmpty(childCol.SUPPLIER_MRR_DATE) ? masterColumnValue.SUPPLIER_MRR_DATE : childCol.SUPPLIER_MRR_DATE;
                    inventoryChildDetails.BRAND_NAME = string.IsNullOrEmpty(childCol.BRAND_NAME) ? masterColumnValue.BRAND_NAME : childCol.BRAND_NAME;
                    inventoryChildDetails.BRAND_ACCEPT_FLAG = string.IsNullOrEmpty(childCol.BRAND_ACCEPT_FLAG) ? masterColumnValue.BRAND_ACCEPT_FLAG : childCol.BRAND_ACCEPT_FLAG;
                    inventoryChildDetails.BRAND_REMARKS = string.IsNullOrEmpty(childCol.BRAND_REMARKS) ? masterColumnValue.BRAND_REMARKS : childCol.BRAND_REMARKS;
                    inventoryChildDetails.RACK_QTY = string.IsNullOrEmpty(childCol.RACK_QTY) ? masterColumnValue.RACK_QTY : childCol.RACK_QTY;
                    inventoryChildDetails.RACK2_QTY = string.IsNullOrEmpty(childCol.RACK2_QTY) ? masterColumnValue.RACK2_QTY : childCol.RACK2_QTY;
                    inventoryChildDetails.GATE_ENTRY_NO = string.IsNullOrEmpty(childCol.GATE_ENTRY_NO) ? masterColumnValue.GATE_ENTRY_NO : childCol.GATE_ENTRY_NO;
                    inventoryChildDetails.ISSUE_NO = string.IsNullOrEmpty(childCol.ISSUE_NO) ? masterColumnValue.ISSUE_NO : childCol.ISSUE_NO;
                    inventoryChildDetails.ISSUE_DATE = string.IsNullOrEmpty(childCol.ISSUE_DATE) ? masterColumnValue.ISSUE_DATE : childCol.ISSUE_DATE;
                    inventoryChildDetails.ISSUE_TYPE_CODE = string.IsNullOrEmpty(childCol.ISSUE_TYPE_CODE) ? masterColumnValue.ISSUE_TYPE_CODE : childCol.ISSUE_TYPE_CODE;
                    inventoryChildDetails.FROM_LOCATION_CODE = string.IsNullOrEmpty(childCol.FROM_LOCATION_CODE) ? masterColumnValue.FROM_LOCATION_CODE : childCol.FROM_LOCATION_CODE;
                    inventoryChildDetails.TO_BUDGET_FLAG = string.IsNullOrEmpty(childCol.TO_BUDGET_FLAG) ? masterColumnValue.TO_BUDGET_FLAG : childCol.TO_BUDGET_FLAG;
                    inventoryChildDetails.REQ_QUANTITY = string.IsNullOrEmpty(childCol.REQ_QUANTITY) ? masterColumnValue.REQ_QUANTITY : childCol.REQ_QUANTITY;
                    inventoryChildDetails.PRODUCTION_QTY = string.IsNullOrEmpty(childCol.PRODUCTION_QTY) ? masterColumnValue.PRODUCTION_QTY : childCol.PRODUCTION_QTY;
                    inventoryChildDetails.PRODUCT_CODE = string.IsNullOrEmpty(childCol.PRODUCT_CODE) ? masterColumnValue.PRODUCT_CODE : childCol.PRODUCT_CODE;
                    inventoryChildDetails.USE_PLACE = string.IsNullOrEmpty(childCol.USE_PLACE) ? masterColumnValue.USE_PLACE : childCol.USE_PLACE;
                    inventoryChildDetails.CUSTOMER_CODE = string.IsNullOrEmpty(childCol.CUSTOMER_CODE) ? masterColumnValue.CUSTOMER_CODE : childCol.CUSTOMER_CODE;
                    inventoryChildDetails.EMPLOYEE_CODE = string.IsNullOrEmpty(childCol.EMPLOYEE_CODE) ? masterColumnValue.EMPLOYEE_CODE : childCol.EMPLOYEE_CODE;
                    inventoryChildDetails.ISSUE_SLIP_NO = string.IsNullOrEmpty(childCol.ISSUE_SLIP_NO) ? masterColumnValue.ISSUE_SLIP_NO : childCol.ISSUE_SLIP_NO;
                    inventoryChildDetails.REFERENCE_NO = string.IsNullOrEmpty(childCol.REFERENCE_NO) ? masterColumnValue.REFERENCE_NO : childCol.REFERENCE_NO;
                    inventoryChildDetails.RETURN_NO = string.IsNullOrEmpty(childCol.RETURN_NO) ? masterColumnValue.RETURN_NO : childCol.RETURN_NO;
                    inventoryChildDetails.RETURN_DATE = string.IsNullOrEmpty(childCol.RETURN_DATE) ? masterColumnValue.RETURN_DATE : childCol.RETURN_DATE;
                    inventoryChildDetails.BUDGET_CODE = string.IsNullOrEmpty(childCol.BUDGET_CODE) ? masterColumnValue.BUDGET_CODE : childCol.BUDGET_CODE;
                    inventoryChildDetails.TERMS_DAYS = string.IsNullOrEmpty(childCol.TERMS_DAYS) ? masterColumnValue.TERMS_DAYS : childCol.TERMS_DAYS;
                    inventoryChildDetails.REQUISITION_NO = string.IsNullOrEmpty(childCol.REQUISITION_NO) ? masterColumnValue.REQUISITION_NO : childCol.REQUISITION_NO;
                    inventoryChildDetails.REQUISITION_DATE = string.IsNullOrEmpty(childCol.REQUISITION_DATE) ? masterColumnValue.REQUISITION_DATE : childCol.REQUISITION_DATE;
                    inventoryChildDetails.BUYERS_NAME = string.IsNullOrEmpty(childCol.BUYERS_NAME) ? masterColumnValue.BUYERS_NAME : childCol.BUYERS_NAME;
                    inventoryChildDetails.BUYERS_ADDRESS = string.IsNullOrEmpty(childCol.BUYERS_ADDRESS) ? masterColumnValue.BUYERS_ADDRESS : childCol.BUYERS_ADDRESS;
                    inventoryChildDetails.ACTUAL_QUANTITY = string.IsNullOrEmpty(childCol.ACTUAL_QUANTITY) ? masterColumnValue.ACTUAL_QUANTITY : childCol.ACTUAL_QUANTITY;
                    inventoryChildDetails.ACKNOWLEDGE_BY = string.IsNullOrEmpty(childCol.ACKNOWLEDGE_BY) ? masterColumnValue.ACKNOWLEDGE_BY : childCol.ACKNOWLEDGE_BY;
                    inventoryChildDetails.ACKNOWLEDGE_DATE = string.IsNullOrEmpty(childCol.ACKNOWLEDGE_DATE) ? masterColumnValue.ACKNOWLEDGE_DATE : childCol.ACKNOWLEDGE_DATE;
                    inventoryChildDetails.OPENING_DATA_FLAG = string.IsNullOrEmpty(childCol.OPENING_DATA_FLAG) ? masterColumnValue.OPENING_DATA_FLAG : childCol.OPENING_DATA_FLAG;
                    inventoryChildDetails.TO_FORM_CODE = string.IsNullOrEmpty(childCol.TO_FORM_CODE) ? masterColumnValue.TO_FORM_CODE : childCol.TO_FORM_CODE;
                    inventoryChildDetails.ORDER_NO = string.IsNullOrEmpty(childCol.ORDER_NO) ? masterColumnValue.ORDER_NO : childCol.ORDER_NO;
                    inventoryChildDetails.ORDER_DATE = string.IsNullOrEmpty(childCol.ORDER_DATE) ? masterColumnValue.ORDER_DATE : childCol.ORDER_DATE;
                    inventoryChildDetails.DELIVERY_DATE = string.IsNullOrEmpty(childCol.DELIVERY_DATE) ? masterColumnValue.DELIVERY_DATE : childCol.DELIVERY_DATE;
                    inventoryChildDetails.DELIVERY_TERMS = string.IsNullOrEmpty(childCol.DELIVERY_TERMS) ? masterColumnValue.DELIVERY_TERMS : childCol.DELIVERY_TERMS;
                    inventoryChildDetails.CANCEL_QUANTITY = string.IsNullOrEmpty(childCol.CANCEL_QUANTITY) ? masterColumnValue.CANCEL_QUANTITY : childCol.CANCEL_QUANTITY;
                    inventoryChildDetails.ADJUST_QUANTITY = string.IsNullOrEmpty(childCol.ADJUST_QUANTITY) ? masterColumnValue.ADJUST_QUANTITY : childCol.ADJUST_QUANTITY;
                    inventoryChildDetails.CANCEL_FLAG = string.IsNullOrEmpty(childCol.CANCEL_FLAG) ? masterColumnValue.CANCEL_FLAG : childCol.CANCEL_FLAG;
                    inventoryChildDetails.CANCEL_BY = string.IsNullOrEmpty(childCol.CANCEL_BY) ? masterColumnValue.CANCEL_BY : childCol.CANCEL_BY;
                    inventoryChildDetails.CANCEL_DATE = string.IsNullOrEmpty(childCol.CANCEL_DATE) ? masterColumnValue.CANCEL_DATE : childCol.CANCEL_DATE;
                    inventoryChildDetails.PARTY_CODE = string.IsNullOrEmpty(childCol.PARTY_CODE) ? masterColumnValue.PARTY_CODE : childCol.PARTY_CODE;
                    inventoryChildDetails.MRR_DATE = string.IsNullOrEmpty(childCol.MRR_DATE) ? masterColumnValue.MRR_DATE : childCol.MRR_DATE;
                    inventoryChildDetails.REQUEST_DATE = string.IsNullOrEmpty(childCol.REQUEST_DATE) ? masterColumnValue.REQUEST_DATE : childCol.REQUEST_DATE;
                    inventoryChildDetails.QUOTE_DATE = string.IsNullOrEmpty(childCol.QUOTE_DATE) ? masterColumnValue.QUOTE_DATE : "SYSDATE";
                    if (commonValue.TableName.ToUpper() == "IP_ADVICE_MRR")
                    {
                        var insertQuery = $@"INSERT INTO IP_ADVICE_MRR (TO_LOCATION_CODE,REMARKS,MANUAL_NO,MRR_DATE,MRR_NO,COMPLETED_QUANTITY,CALC_TOTAL_PRICE,CALC_UNIT_PRICE
                            ,CALC_QUANTITY ,TOTAL_PRICE,UNIT_PRICE,QUANTITY,MU_CODE,ITEM_CODE,SERIAL_NO, FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG
                            ,MODIFY_DATE,MODIFY_BY)
                            VALUES ('{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.REMARKS}','{inventoryChildDetails.MANUAL_NO}',TO_DATE('{inventoryChildDetails.MRR_DATE}','DD-MON-YY hh24:mi:ss')
                            ,'{commonValue.NewVoucherNumber}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.FORM_CODE}'
                            ,'{_workContext.CurrentUserinformation.company_code}', '{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}',{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.MODIFY_BY}')";

                        dbcontext.ExecuteSqlCommand(insertQuery);

                        insertedToChild = true;
                    }
                    //else if (commonValue.TableName.ToUpper() == "IP_PURCHASE_INVOICE")
                    //{
                    //    var insertQuery = $@"INSERT INTO IP_PURCHASE_INVOICE (INVOICE_NO,INVOICE_DATE, MANUAL_NO, SUPPLIER_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE,SUPPLIER_BUDGET_FLAG,
                    //        SERIAL_NO, BUDGET_FLAG, ITEM_CODE, MU_CODE,QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE, COMPLETED_QUANTITY,
                    //        REMARKS, FORM_CODE, COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, DUE_DATE,CURRENCY_CODE, EXCHANGE_RATE, TERMS_DAY,
                    //        SYN_ROWID, TRACKING_NO,TO_LOCATION_CODE, SESSION_ROWID, BATCH_NO, MODIFY_DATE, LOT_NO,SUPPLIER_MRR_NO, PP_NO, MODIFY_BY, P_TYPE, PP_DATE,
                    //        DIVISION_CODE, NET_GROSS_RATE, NET_SALES_RATE, NET_TAXABLE_RATE, MASTER_PP_NO,SECOND_QUANTITY, THIRD_QUANTITY, RECONCILE_DATE, RECONCILE_FLAG,
                    //        RECONCILE_BY)
                    //        VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.INVOICE_DATE}','DD-MON-YY'),'{inventoryChildDetails.MANUAL_NO}','{inventoryChildDetails.SUPPLIER_CODE}'
                    //        ,'{inventoryChildDetails.SUPPLIER_INV_NO}','{inventoryChildDetails.SUPPLIER_INV_DATE}','{inventoryChildDetails.SUPPLIER_BUDGET_FLAG}',{serialno}
                    //        ,'{inventoryChildDetails.BUDGET_FLAG}','{inventoryChildDetails.ITEM_CODE}', '{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                    //        ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                    //        ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}','{inventoryChildDetails.FORM_CODE}'
                    //        ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                    //        ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.DUE_DATE}','{inventoryChildDetails.CURRENCY_CODE}'
                    //        ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.TERMS_DAY}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                    //        ,'{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}',{inventoryChildDetails.MODIFY_DATE}
                    //        ,'{inventoryChildDetails.LOT_NO}','{inventoryChildDetails.SUPPLIER_MRR_NO}','{inventoryChildDetails.PP_NO}','{inventoryChildDetails.MODIFY_BY}'
                    //        ,'{inventoryChildDetails.P_TYPE}','{inventoryChildDetails.PP_DATE}','{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.NET_GROSS_RATE}'
                    //        ,'{inventoryChildDetails.NET_SALES_RATE}','{inventoryChildDetails.NET_TAXABLE_RATE}','{inventoryChildDetails.MASTER_PP_NO}','{inventoryChildDetails.SECOND_QUANTITY}'
                    //        ,'{inventoryChildDetails.THIRD_QUANTITY}','{inventoryChildDetails.RECONCILE_DATE}','{inventoryChildDetails.RECONCILE_FLAG}','{inventoryChildDetails.RECONCILE_BY}'
                    //        )";
                    //    dbcontext.ExecuteSqlCommand(insertQuery);
                    //    serialno++;
                    //    insertedToChild = true;
                    //} for sub projecty code for construction system
                    else if (commonValue.TableName.ToUpper() == "IP_PURCHASE_INVOICE")
                    {
                        var insertQuery = $@"INSERT INTO IP_PURCHASE_INVOICE (INVOICE_NO,INVOICE_DATE, MANUAL_NO, SUPPLIER_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE,SUPPLIER_BUDGET_FLAG,
                            SERIAL_NO, BUDGET_FLAG, ITEM_CODE, MU_CODE,QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE, COMPLETED_QUANTITY,
                            REMARKS, FORM_CODE, COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, DUE_DATE,CURRENCY_CODE, EXCHANGE_RATE, TERMS_DAY,
                            SYN_ROWID, TRACKING_NO,TO_LOCATION_CODE, SESSION_ROWID, BATCH_NO, MODIFY_DATE, LOT_NO,SUPPLIER_MRR_NO, PP_NO, MODIFY_BY, P_TYPE, PP_DATE,
                            DIVISION_CODE, NET_GROSS_RATE, NET_SALES_RATE, NET_TAXABLE_RATE, MASTER_PP_NO,SECOND_QUANTITY, THIRD_QUANTITY, RECONCILE_DATE, RECONCILE_FLAG,
                            RECONCILE_BY,SUB_PROJECT_CODE)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.INVOICE_DATE}','DD-MON-YY'),'{inventoryChildDetails.MANUAL_NO}','{inventoryChildDetails.SUPPLIER_CODE}'
                            ,'{inventoryChildDetails.SUPPLIER_INV_NO}','{inventoryChildDetails.SUPPLIER_INV_DATE}','{inventoryChildDetails.SUPPLIER_BUDGET_FLAG}',{serialno}
                            ,'{inventoryChildDetails.BUDGET_FLAG}','{inventoryChildDetails.ITEM_CODE}', '{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}','{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.DUE_DATE}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.TERMS_DAY}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}',{inventoryChildDetails.MODIFY_DATE}
                            ,'{inventoryChildDetails.LOT_NO}','{inventoryChildDetails.SUPPLIER_MRR_NO}','{inventoryChildDetails.PP_NO}','{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.P_TYPE}','{inventoryChildDetails.PP_DATE}','{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.NET_GROSS_RATE}'
                            ,'{inventoryChildDetails.NET_SALES_RATE}','{inventoryChildDetails.NET_TAXABLE_RATE}','{inventoryChildDetails.MASTER_PP_NO}','{inventoryChildDetails.SECOND_QUANTITY}'
                            ,'{inventoryChildDetails.THIRD_QUANTITY}','{inventoryChildDetails.RECONCILE_DATE}','{inventoryChildDetails.RECONCILE_FLAG}','{inventoryChildDetails.RECONCILE_BY}','{inventoryChildDetails.SUB_PROJECT_CODE}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_GOODS_ISSUE_RETURN")
                    {
                        var insertQuery = $@"INSERT INTO IP_GOODS_ISSUE_RETURN (
                        RETURN_NO,RETURN_DATE,MANUAL_NO,FROM_LOCATION_CODE,TO_LOCATION_CODE,TO_BUDGET_FLAG,ITEM_CODE,SERIAL_NO,MU_CODE,QUANTITY,UNIT_PRICE
                       ,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,COMPANY_CODE,BRANCH_CODE
                       ,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,SYN_ROWID,TRACKING_NO,SESSION_ROWID,BATCH_NO,MODIFY_DATE
                       ,MODIFY_BY,EMPLOYEE_CODE,CUSTOMER_CODE,DIVISION_CODE,ISSUE_TYPE_CODE,SECOND_QUANTITY,THIRD_QUANTITY)
                        VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.RETURN_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                            ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.TO_BUDGET_FLAG}','{inventoryChildDetails.ITEM_CODE}'
                            ,{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}'
                            ,'{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}','{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}'
                            ,'{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}','{inventoryChildDetails.EXCHANGE_RATE}'
                            ,'{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}','{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}'
                            ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.EMPLOYEE_CODE}','{inventoryChildDetails.CUSTOMER_CODE}'
                            ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.ISSUE_TYPE_CODE}','{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_GOODS_ISSUE")
                    {
                        //var insertQuery = $@"INSERT INTO IP_GOODS_ISSUE (ISSUE_NO,ISSUE_DATE, MANUAL_NO, ISSUE_TYPE_CODE, FROM_LOCATION_CODE, TO_LOCATION_CODE, 
                        //    TO_BUDGET_FLAG, ITEM_CODE, SERIAL_NO, MU_CODE, REQ_QUANTITY,QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, 
                        //    CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, 
                        //    EXCHANGE_RATE, SYN_ROWID, TRACKING_NO, SESSION_ROWID, BATCH_NO, MODIFY_DATE, PRODUCT_CODE, MODIFY_BY, USE_PLACE, CUSTOMER_CODE, 
                        //    EMPLOYEE_CODE, SUPPLIER_CODE, ISSUE_SLIP_NO, DIVISION_CODE, REFERENCE_NO, SECOND_QUANTITY, THIRD_QUANTITY)
                        //    VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ISSUE_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}','{inventoryChildDetails.ISSUE_TYPE_CODE}'
                        //    ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.TO_BUDGET_FLAG}','{inventoryChildDetails.ITEM_CODE}'
                        //    ,{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.REQ_QUANTITY}','{inventoryChildDetails.QUANTITY}'
                        //    ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                        //    ,'{inventoryChildDetails.CALC_TOTAL_PRICE}'
                        //    ,'{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}', '{inventoryChildDetails.FORM_CODE}', '{inventoryChildDetails.COMPANY_CODE}'
                        //    ,'{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                        //    ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}','{inventoryChildDetails.EXCHANGE_RATE}'
                        //    ,'{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}','{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}'
                        //    ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.PRODUCT_CODE}','{inventoryChildDetails.MODIFY_BY}','{inventoryChildDetails.USE_PLACE}'
                        //    ,'{inventoryChildDetails.CUSTOMER_CODE}','{inventoryChildDetails.EMPLOYEE_CODE}','{inventoryChildDetails.SUPPLIER_CODE}','{inventoryChildDetails.ISSUE_SLIP_NO}'
                        //    ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.REFERENCE_NO}','{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                        //   )";
                        var insertQuery = $@"INSERT INTO IP_GOODS_ISSUE (ISSUE_NO,ISSUE_DATE, MANUAL_NO, ISSUE_TYPE_CODE, FROM_LOCATION_CODE, TO_LOCATION_CODE, 
                            TO_BUDGET_FLAG, ITEM_CODE, SERIAL_NO, MU_CODE, REQ_QUANTITY,QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, 
                            CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, 
                            EXCHANGE_RATE, SYN_ROWID, TRACKING_NO, SESSION_ROWID, BATCH_NO, MODIFY_DATE, PRODUCT_CODE, MODIFY_BY, USE_PLACE, CUSTOMER_CODE, 
                            EMPLOYEE_CODE, SUPPLIER_CODE, ISSUE_SLIP_NO, DIVISION_CODE, REFERENCE_NO, SECOND_QUANTITY, THIRD_QUANTITY,SUB_PROJECT_CODE)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ISSUE_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}','{inventoryChildDetails.ISSUE_TYPE_CODE}'
                            ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.TO_BUDGET_FLAG}','{inventoryChildDetails.ITEM_CODE}'
                            ,{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.REQ_QUANTITY}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}'
                            ,'{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}', '{inventoryChildDetails.FORM_CODE}', '{inventoryChildDetails.COMPANY_CODE}'
                            ,'{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}','{inventoryChildDetails.EXCHANGE_RATE}'
                            ,'{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}','{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}'
                            ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.PRODUCT_CODE}','{inventoryChildDetails.MODIFY_BY}','{inventoryChildDetails.USE_PLACE}'
                            ,'{inventoryChildDetails.CUSTOMER_CODE}','{inventoryChildDetails.EMPLOYEE_CODE}','{inventoryChildDetails.SUPPLIER_CODE}','{inventoryChildDetails.ISSUE_SLIP_NO}'
                            ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.REFERENCE_NO}','{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                            ,'{inventoryChildDetails.SUB_PROJECT_CODE}')";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_PURCHASE_RETURN")
                    {
                        var insertQuery = $@"INSERT INTO IP_PURCHASE_RETURN (RETURN_NO,RETURN_DATE, MANUAL_NO, SUPPLIER_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE,SUPPLIER_BUDGET_FLAG,
                            FROM_LOCATION_CODE, SERIAL_NO, ITEM_CODE, BUDGET_FLAG,MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,
                            COMPLETED_QUANTITY, BUDGET_CODE, REMARKS,FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE,DELETED_FLAG, CURRENCY_CODE,EXCHANGE_RATE,
                            SYN_ROWID, TRACKING_NO,SESSION_ROWID, TERMS_DAYS, MODIFY_DATE, MODIFY_BY, P_TYPE,DIVISION_CODE, SUPPLIER_MRR_NO, NET_GROSS_RATE, NET_SALES_RATE,
                            NET_TAXABLE_RATE,SECOND_QUANTITY, THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.RETURN_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}','{inventoryChildDetails.SUPPLIER_CODE}'
                            ,'{inventoryChildDetails.SUPPLIER_INV_NO}','{inventoryChildDetails.SUPPLIER_INV_DATE}','{inventoryChildDetails.SUPPLIER_BUDGET_FLAG}'
                            ,'{inventoryChildDetails.FROM_LOCATION_CODE}',{serialno}
                            ,'{inventoryChildDetails.ITEM_CODE}','{inventoryChildDetails.BUDGET_FLAG}','{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.BUDGET_CODE}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}','{masterColumnValue.TERMS_DAYS}',{inventoryChildDetails.MODIFY_DATE}
                            ,'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.P_TYPE}','{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.SUPPLIER_MRR_NO}','{inventoryChildDetails.NET_GROSS_RATE}'
                            ,'{inventoryChildDetails.NET_SALES_RATE}','{inventoryChildDetails.NET_TAXABLE_RATE}','{inventoryChildDetails.SECOND_QUANTITY}'
                            ,'{inventoryChildDetails.THIRD_QUANTITY}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_PURCHASE_MRR")
                    {
                        var insertQuery = $@"INSERT INTO IP_PURCHASE_MRR (MRR_NO, 
                            MRR_DATE, MANUAL_NO, SUPPLIER_CODE, TO_LOCATION_CODE, SERIAL_NO, 
                            ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, 
                            CALC_UNIT_PRICE, CALC_QUANTITY, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, FORM_CODE, 
                            COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, 
                            CURRENCY_CODE, EXCHANGE_RATE, TERMS_DAY, SYN_ROWID, 
                            TRACKING_NO, SESSION_ROWID, BATCH_NO, MODIFY_DATE, LOT_NO, 
                            SUPPLIER_MRR_NO, PHOTO_FILE_NAME1, PHOTO_FILE_NAME2, SPECIFICATION, MODIFY_BY, 
                            PP_NO, SUPPLIER_MRR_DATE, BRAND_NAME, SUPPLIER_INV_DATE, BRAND_ACCEPT_FLAG, 
                            BRAND_REMARKS, SUPPLIER_INV_NO, DIVISION_CODE, RACK_QTY, RACK2_QTY, 
                            GATE_ENTRY_NO, MASTER_PP_NO, SECOND_QUANTITY, THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.MRR_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}','{inventoryChildDetails.SUPPLIER_CODE}'
                            ,'{inventoryChildDetails.TO_LOCATION_CODE}',{serialno}
                            ,'{inventoryChildDetails.ITEM_CODE}','{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_UNIT_PRICE}','{inventoryChildDetails.CALC_QUANTITY}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.TERMS_DAY}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}',{inventoryChildDetails.MODIFY_DATE}
                            ,'{inventoryChildDetails.LOT_NO}','{inventoryChildDetails.SUPPLIER_MRR_NO}','{inventoryChildDetails.PHOTO_FILE_NAME1}'
                            ,'{inventoryChildDetails.PHOTO_FILE_NAME2}','{inventoryChildDetails.SPECIFICATION}'
                            ,'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.PP_NO}'
                            ,'{inventoryChildDetails.SUPPLIER_MRR_DATE}','{inventoryChildDetails.BRAND_NAME}','{inventoryChildDetails.SUPPLIER_INV_DATE}'
                            ,'{inventoryChildDetails.BRAND_ACCEPT_FLAG}'
                            ,'{inventoryChildDetails.BRAND_REMARKS}','{inventoryChildDetails.SUPPLIER_INV_NO}'
                            ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.RACK_QTY}','{inventoryChildDetails.RACK2_QTY}'
                            ,'{inventoryChildDetails.GATE_ENTRY_NO}','{inventoryChildDetails.MASTER_PP_NO}','{inventoryChildDetails.SECOND_QUANTITY}'
                            ,'{inventoryChildDetails.THIRD_QUANTITY}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    //else if (commonValue.TableName.ToUpper() == "IP_GOODS_REQUISITION")
                    //{
                    //    var insertQuery = $@"INSERT INTO IP_GOODS_REQUISITION (REQUISITION_NO, 
                    //            REQUISITION_DATE, MANUAL_NO, FROM_LOCATION_CODE, TO_LOCATION_CODE, SUPPLIER_CODE,ITEM_CODE, 
                    //            SERIAL_NO, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, 
                    //            CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, 
                    //            FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                    //            DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SYN_ROWID, TRACKING_NO, 
                    //            SESSION_ROWID, MODIFY_DATE,  MODIFY_BY, CUSTOMER_CODE, 
                    //            SECOND_QUANTITY, THIRD_QUANTITY)
                    //        VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.REQUISITION_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                    //            ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.SUPPLIER_CODE}'                            
                    //        ,'{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                    //        ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                    //        ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                    //        ,'{inventoryChildDetails.FORM_CODE}'
                    //        ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                    //        ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                    //        ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                    //        ,'{inventoryChildDetails.SESSION_ROWID}',{inventoryChildDetails.MODIFY_DATE}
                    //        ,'{inventoryChildDetails.MODIFY_BY}'
                    //        ,'{inventoryChildDetails.CUSTOMER_CODE}','{inventoryChildDetails.SECOND_QUANTITY}'
                    //        ,'{inventoryChildDetails.THIRD_QUANTITY}'
                    //       )";
                    //    dbcontext.ExecuteSqlCommand(insertQuery);
                    //    serialno++;
                    //    insertedToChild = true;
                    //}  For sub Project code for construction setup
                    else if (commonValue.TableName.ToUpper() == "IP_GOODS_REQUISITION")
                    {
                        var insertQuery = $@"INSERT INTO IP_GOODS_REQUISITION (REQUISITION_NO, 
                                REQUISITION_DATE, MANUAL_NO, FROM_LOCATION_CODE, TO_LOCATION_CODE, SUPPLIER_CODE,ITEM_CODE, 
                                SERIAL_NO, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, 
                                CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, 
                                FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SYN_ROWID, TRACKING_NO, 
                                SESSION_ROWID, MODIFY_DATE,  MODIFY_BY, CUSTOMER_CODE, 
                                SECOND_QUANTITY, THIRD_QUANTITY,SUB_PROJECT_CODE)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.REQUISITION_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                                ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.SUPPLIER_CODE}'                            
                            ,'{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}',{inventoryChildDetails.MODIFY_DATE}
                            ,'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.CUSTOMER_CODE}','{inventoryChildDetails.SECOND_QUANTITY}'
                            ,'{inventoryChildDetails.THIRD_QUANTITY}','{inventoryChildDetails.SUB_PROJECT_CODE}'
                           )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }

                    else if (commonValue.TableName.ToUpper() == "IP_TRANSFER_ISSUE")
                    {
                        var insertQuery = $@"INSERT INTO IP_TRANSFER_ISSUE (ISSUE_NO, 
                            ISSUE_DATE, MANUAL_NO, ISSUE_TYPE_CODE, FROM_LOCATION_CODE, FROM_BUDGET_FLAG, 
                            TO_LOCATION_CODE, TO_BUDGET_FLAG, ITEM_CODE, SERIAL_NO, MU_CODE, 
                            QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, 
                            CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, FORM_CODE, COMPANY_CODE, 
                            BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, 
                            EXCHANGE_RATE, SYN_ROWID, TRACKING_NO, TO_BRANCH_CODE, SESSION_ROWID, 
                            BATCH_NO, MODIFY_DATE, BUYERS_NAME, BUYERS_ADDRESS, MODIFY_BY, 
                            ACKNOWLEDGE_FLAG, SUPPLIER_CODE, ACTUAL_QUANTITY, ACKNOWLEDGE_BY, ACKNOWLEDGE_DATE, 
                            CUSTOMER_CODE, DIVISION_CODE, OPENING_DATA_FLAG, REFERENCE_NO, TO_FORM_CODE, 
                            SECOND_QUANTITY, THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ISSUE_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}', '{inventoryChildDetails.ISSUE_TYPE_CODE}'
                            ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.FROM_BUDGET_FLAG}','{inventoryChildDetails.TO_LOCATION_CODE}'
                            ,'{inventoryChildDetails.FROM_BUDGET_FLAG}','{inventoryChildDetails.ITEM_CODE}'
                            ,{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}'
                            ,'{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}','{inventoryChildDetails.FORM_CODE}','{inventoryChildDetails.COMPANY_CODE}'
                            ,'{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}','{inventoryChildDetails.EXCHANGE_RATE}'
                            ,'{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{masterColumnValue.TO_BRANCH_CODE}','{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}'
                            ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.BUYERS_NAME}'
                            ,'{inventoryChildDetails.BUYERS_ADDRESS}','{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.ACKNOWLEDGE_FLAG}'
                            ,'{inventoryChildDetails.SUPPLIER_CODE}','{inventoryChildDetails.ACTUAL_QUANTITY}','{inventoryChildDetails.ACKNOWLEDGE_BY}'
                            ,'{inventoryChildDetails.ACKNOWLEDGE_DATE}','{inventoryChildDetails.CUSTOMER_CODE}'
                            ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.OPENING_DATA_FLAG}'
                            ,'{inventoryChildDetails.REFERENCE_NO}','{inventoryChildDetails.TO_FORM_CODE}','{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_PURCHASE_ORDER")
                    {
                        var insertQuery = $@"INSERT INTO IP_PURCHASE_ORDER (ORDER_NO, 
                                    ORDER_DATE, MANUAL_NO, SUPPLIER_CODE, SERIAL_NO, ITEM_CODE, 
                                    PRIORITY_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, 
                                    CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, 
                                    FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                    DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, TERMS_DAY, SYN_ROWID, 
                                    TRACKING_NO, SESSION_ROWID, BATCH_NO, MODIFY_DATE, DELIVERY_DATE, 
                                    DELIVERY_TERMS, SPECIFICATION, MODIFY_BY, CANCEL_QUANTITY, ADJUST_QUANTITY, 
                                    CANCEL_FLAG, CANCEL_BY, CANCEL_DATE, OPENING_DATA_FLAG, BRAND_NAME, 
                                    DIVISION_CODE, SECOND_QUANTITY, THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ORDER_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                            ,'{inventoryChildDetails.SUPPLIER_CODE}',{serialno}                            
                            ,'{inventoryChildDetails.ITEM_CODE}','{masterColumnValue.PRIORITY_CODE}','{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.TERMS_DAY}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}',{inventoryChildDetails.MODIFY_DATE}
                            ,'{inventoryChildDetails.DELIVERY_DATE}','{masterColumnValue.PRIORITY_CODE}','{inventoryChildDetails.SPECIFICATION}'
                            ,'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.CANCEL_QUANTITY}','{inventoryChildDetails.ADJUST_QUANTITY}','{inventoryChildDetails.CANCEL_FLAG}'
                            ,'{inventoryChildDetails.CANCEL_BY}'
                            ,'{inventoryChildDetails.CANCEL_DATE}','{inventoryChildDetails.OPENING_DATA_FLAG}','{inventoryChildDetails.BRAND_NAME}'
                            ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_RETURNABLE_GOODS_RETURN")
                    {
                        var insertQuery = $@"INSERT INTO IP_RETURNABLE_GOODS_RETURN (ISSUE_NO,ISSUE_DATE,MANUAL_NO,PARTY_CODE,FROM_LOCATION_CODE,TO_LOCATION_CODE,ITEM_CODE,           
                            SERIAL_NO,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,
                            COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                    DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SYN_ROWID, 
                                    TRACKING_NO, SESSION_ROWID, MODIFY_DATE,MODIFY_BY,
                                    DIVISION_CODE, SECOND_QUANTITY, THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ISSUE_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                            ,'{masterColumnValue.PARTY_CODE}','{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.TO_LOCATION_CODE}'                  
                            ,'{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}'
                            ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                           )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_QUOTATION_INQUIRY")
                    {
                        var insertQuery = $@"INSERT INTO IP_QUOTATION_INQUIRY (QUOTE_NO,QUOTE_DATE,ORDER_NO,REQUEST_NO,MANUAL_NO,SUPPLIER_CODE,
                            ADDRESS,CONTACT_PERSON,PHONE_NO,SERIAL_NO,ITEM_CODE,SPECIFICATION,         
                            MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,REMARKS,FORM_CODE,
                            COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                    DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SYN_ROWID, 
                                    TRACKING_NO, SESSION_ROWID, MODIFY_DATE,MODIFY_BY,BRAND_NAME,CREDIT_DAYS,DELIVERY_TERMS,DELIVERY_DATE,DELIVERY_DAYS,
                                    RANK_VALUE,APPROVED_FLAG,APPROVED_BY,APPROVED_DATE,
                                    SECOND_QUANTITY, THIRD_QUANTITY)

                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.QUOTE_DATE}','DD-MON-YY hh24:mi:ss')
                            ,'{inventoryChildDetails.ORDER_NO}','{inventoryChildDetails.REQUEST_NO}','{inventoryChildDetails.MANUAL_NO}'
                            ,'{inventoryChildDetails.SUPPLIER_CODE}' ,'{masterColumnValue.ADDRESS}','{masterColumnValue.CONTACT_PERSON}' 
                             ,'{masterColumnValue.PHONE_NO}',{serialno}
                            ,'{inventoryChildDetails.ITEM_CODE}','{inventoryChildDetails.SPECIFICATION}','{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}'
                            ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.BRAND_NAME}','{inventoryChildDetails.CREDIT_DAYS}','{inventoryChildDetails.DELIVERY_TERMS}'
                            ,'{inventoryChildDetails.DELIVERY_DATE}','{inventoryChildDetails.DELIVERY_DAYS}'
                            ,'{inventoryChildDetails.RANK_VALUE}','{inventoryChildDetails.APPROVED_FLAG}','{inventoryChildDetails.APPROVED_BY}'
                            ,'{inventoryChildDetails.APPROVED_DATE}'
                            ,'{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_PURCHASE_REQUEST")
                    {
                        var insertQuery = $@"INSERT INTO IP_PURCHASE_REQUEST (REQUEST_NO,REQUEST_DATE,MANUAL_NO,FROM_LOCATION_CODE,TO_LOCATION_CODE,SERIAL_NO,ITEM_CODE,           
                            SPECIFICATION,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,
                            COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                    DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SYN_ROWID, 
                                    TRACKING_NO, SESSION_ROWID, BATCH_NO, MODIFY_DATE,MODIFY_BY,SUPPLIER_CODE,
                                    CANCEL_QUANTITY,ADJUST_QUANTITY,CANCEL_FLAG,CANCEL_BY,CANCEL_DATE,OPENING_DATA_FLAG,DEMAND_SLIP_NO,
                                    DIVISION_CODE, SECOND_QUANTITY, THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE ('{inventoryChildDetails.REQUEST_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                            ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.TO_LOCATION_CODE}',{serialno}               
                            ,'{inventoryChildDetails.ITEM_CODE}','{inventoryChildDetails.SPECIFICATION}','{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}'
                            ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.MODIFY_BY}','{inventoryChildDetails.SUPPLIER_CODE}'
                            ,'{inventoryChildDetails.CANCEL_QUANTITY}','{inventoryChildDetails.ADJUST_QUANTITY}','{inventoryChildDetails.CANCEL_FLAG}'
                            ,'{inventoryChildDetails.CANCEL_BY}','{inventoryChildDetails.CANCEL_DATE}'
                            ,'{inventoryChildDetails.OPENING_DATA_FLAG}','{inventoryChildDetails.DEMAND_SLIP_NO}'
                            ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_GATE_PASS_ENTRY")
                    {
                        var insertQuery = $@"INSERT INTO IP_GATE_PASS_ENTRY (ISSUE_NO,ISSUE_DATE,REFERENCE_NO,
                                REFERENCE_FORM_CODE,REFERENCE_PARTY_CODE,PARTY_FLAG,MANUAL_NO,DOCUMENT_TYPE_CODE,REMARKS,FROM_LOCATION_CODE,SERIAL_NO,ITEM_CODE,          
                            MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE, CURRENCY_CODE, EXCHANGE_RATE,FORM_CODE,
                            COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG,SYN_ROWID,
                                    SESSION_ROWID,TRACKING_NO,
                                     MODIFY_DATE,MODIFY_BY, 
                                    DIVISION_CODE)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ISSUE_DATE}','DD-MON-YY hh24:mi:ss')
                            ,'{inventoryChildDetails.REFERENCE_NO}','{inventoryChildDetails.REFERENCE_FORM_CODE}','{masterColumnValue.REFERENCE_PARTY_CODE}'
                            ,'{inventoryChildDetails.PARTY_FLAG}'
                            ,'{inventoryChildDetails.MANUAL_NO}'
                            ,'{masterColumnValue.DOCUMENT_TYPE_CODE}','{inventoryChildDetails.REMARKS}','{inventoryChildDetails.FROM_LOCATION_CODE}'                  
                            ,{serialno},'{inventoryChildDetails.ITEM_CODE}','{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}'
                            ,'{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}'
                            ,'{inventoryChildDetails.SYN_ROWID}' ,'{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.TRACKING_NO}'                           
                            ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.DIVISION_CODE}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }

                    else if (commonValue.TableName.ToUpper() == "IP_RETURNABLE_GOODS_ISSUE")
                    {
                        var insertQuery = $@"INSERT INTO IP_RETURNABLE_GOODS_ISSUE (ISSUE_NO,ISSUE_DATE,MANUAL_NO,PARTY_CODE,FROM_LOCATION_CODE,TO_LOCATION_CODE,ITEM_CODE,           
                            SERIAL_NO,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,
                            COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                    DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SYN_ROWID, 
                                    TRACKING_NO, SESSION_ROWID,MODIFY_DATE, MODIFY_BY,ACKNOWLEDGE_REMARKS,ACKNOWLEDGE_BY,ACKNOWLEDGE_DATE,
                                    OPENING_DATA_FLAG,DIVISION_CODE,REFERENCE_NO, EST_DELIVERY_DATE,SECOND_QUANTITY, THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ISSUE_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                            ,'{masterColumnValue.PARTY_CODE}','{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.TO_LOCATION_CODE}'                  
                            ,'{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}'
                            ,{inventoryChildDetails.MODIFY_DATE},'{inventoryChildDetails.MODIFY_BY}'
                            ,'{inventoryChildDetails.ACKNOWLEDGE_REMARKS}','{inventoryChildDetails.ACKNOWLEDGE_BY}','{inventoryChildDetails.ACKNOWLEDGE_DATE}'
                             ,'{inventoryChildDetails.OPENING_DATA_FLAG}'    
                            ,'{inventoryChildDetails.DIVISION_CODE}'
                            ,'{inventoryChildDetails.REFERENCE_NO}','{masterColumnValue.EST_DELIVERY_DATE}'
                            ,'{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}'
                            )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }

                    else if (commonValue.TableName.ToUpper() == "IP_PRODUCTION_ISSUE")
                    {
                        var insertQuery = $@"INSERT INTO IP_PRODUCTION_ISSUE (  ISSUE_NO,ISSUE_DATE,MANUAL_NO,ISSUE_TYPE_CODE,FROM_LOCATION_CODE,FROM_BUDGET_FLAG,TO_LOCATION_CODE,TO_BUDGET_FLAG,ITEM_CODE,SERIAL_NO,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,TRACKING_NO,SESSION_ROWID,LOT_NO,DIVISION_CODE,PRODUCTION_QTY,REFERENCE_NO,SECOND_QUANTITY,THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ISSUE_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                            ,'{masterColumnValue.ISSUE_TYPE_CODE}','{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.FROM_BUDGET_FLAG}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.TO_BUDGET_FLAG}'                  
                            ,'{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.LOT_NO}'
                           ,'{inventoryChildDetails.DIVISION_CODE}',{inventoryChildDetails.PRODUCTION_QTY},'{inventoryChildDetails.REFERENCE_NO}'
                       ,'{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}')";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_PRODUCTION_MRR")
                    {
                        var insertQuery = $@"INSERT INTO IP_PRODUCTION_MRR (MRR_NO,MRR_DATE,MANUAL_NO,FROM_LOCATION_CODE,FROM_BUDGET_FLAG, TO_LOCATION_CODE,TO_BUDGET_FLAG,ITEM_CODE,SERIAL_NO,MU_CODE,QUANTITY,UNIT_PRICE,                        TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,                                                        DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,TRACKING_NO,SESSION_ROWID,LOT_NO,DIVISION_CODE,REFERENCE_NO,SECOND_QUANTITY,THIRD_QUANTITY)
                            VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.MRR_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                            ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.FROM_BUDGET_FLAG}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.TO_BUDGET_FLAG}'                  
                            ,'{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                            ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                            ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                            ,'{inventoryChildDetails.FORM_CODE}'
                            ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                            ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                            ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.TRACKING_NO}'
                            ,'{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.LOT_NO}'
                           ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.REFERENCE_NO}'
                       ,'{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}')";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else
                    {
                        if (model.Order_No != "undefined") DeleteChildTransaction(commonValue, dbcontext);
                        insertedToChild = SaveInventoryFormDataOld(commonValue, model, primarydatecolumn, primarycolname, dbcontext);
                        return insertedToChild;
                    }
                }
                serialno++;
                return insertedToChild;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public bool DeleteChildTransaction(CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                string deletequery = string.Format(@"DELETE FROM " + commonValue.TableName + " where " + commonValue.PrimaryColumn + "='{0}' and COMPANY_CODE='{1}'", commonValue.VoucherNumber, _workContext.CurrentUserinformation.company_code);
                dbcontext.ExecuteSqlCommand(deletequery);
                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public bool SaveInventoryFormDataOld(CommonFieldsForInventory commonValue, FormDetails model, string primarydatecolumn, string primarycolname, NeoErpCoreEntity dbcontext = null)
        {
            bool insertedToChild = false;
            var voucherno = model.Order_No;
            string primarydate = string.Empty, primarycolumn = string.Empty, today = DateTime.Now.ToString("dd-MMM-yyyy"), createddatestring = "TO_DATE('" + today + "'" + ",'DD-MON-YYYY hh24:mi:ss')", todaystring = System.DateTime.Now.ToString("yyyyMMddHHmmss"), manualno = string.Empty, currencyformat = "NRS", VoucherDate = createddatestring, grandtotal = model.Grand_Total;
            decimal exchangrate = 1;
            var quantityvalue = 0.00;

            Newtonsoft.Json.Linq.JObject mastercolumn = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Master_COLUMN_VALUE);
            Newtonsoft.Json.Linq.JObject customcolumn = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Custom_COLUMN_VALUE);

            dynamic childcolumnvalues = JsonConvert.DeserializeObject(model.Child_COLUMN_VALUE);
            //dynamic childbudgetcentervalues = JsonConvert.DeserializeObject(model.BUDGET_TRANS_VALUE);
            StringBuilder Columnbuilder = new StringBuilder();
            StringBuilder valuesbuilder = new StringBuilder();
            bool insertmaintable = false, insertmastertable = false;
            var staticsalesordercolumns = "SERIAL_NO, FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG";
            foreach (var m in mastercolumn)
            {
                if (m.Key.ToString() == "CURRENCY_CODE")
                {
                    Columnbuilder.Append(m.Key.ToString()).Append(",");
                }
                else if (m.Key.ToString() == "EXCHANGE_RATE")
                {
                    Columnbuilder.Append(m.Key.ToString()).Append(",");
                }
                else Columnbuilder.Append(m.Key.ToString()).Append(",");
            }
            var getPrevDataQuery = $@"SELECT VOUCHER_NO,SESSION_ROWID, CREATED_BY, CREATED_DATE FROM MASTER_TRANSACTION WHERE VOUCHER_NO = '{voucherno}'";
            var defaultData = this._coreEntity.SqlQuery<SalesOrderDetail>(getPrevDataQuery).ToList();
            var defaultCol = "MODIFY_BY,MODIFY_DATE";
            string createdDateForEdit = string.Empty, createdByForEdit = string.Empty, voucherNoForEdit = string.Empty;
            var sessionRowIDForedit = 0;
            foreach (var def in defaultData)
            {
                voucherNoForEdit = def.VOUCHER_NO.ToString();
                createdDateForEdit = "TO_DATE('" + def.CREATED_DATE.ToString() + "', 'MM-DD-YYYY hh12:mi:ss pm')";
                createdByForEdit = def.CREATED_BY.ToString().ToUpper();
                sessionRowIDForedit = Convert.ToInt32(def.SESSION_ROWID);
            }
            Columnbuilder.Append(model.Child_COLUMNS);
            Columnbuilder.Append(staticsalesordercolumns);
            foreach (var v in mastercolumn)
            {
                if (v.Key == primarycolname)
                {
                    primarycolumn = v.Value.ToString();
                }
                string lastName = v.Key.Split('_').Last();
                if (lastName == "DATE")
                {
                    if (v.Value.ToString() == "")
                    {
                        valuesbuilder.Append("SYSDATE").Append(",");
                    }
                    else
                    {
                        if (v.Key == primarydatecolumn)
                        {
                            primarydate = v.Value.ToString();
                            VoucherDate = "trunc(TO_DATE(" + "'" + primarydate + "'" + ",'DD-MON-YYYY hh24:mi:ss'))";
                            valuesbuilder.Append("TO_DATE(" + "'" + primarydate + "'" + ",'DD-MON-YYYY hh24:mi:ss')").Append(",");
                        }
                        else
                        {
                            valuesbuilder.Append("TO_DATE(" + "'" + v.Value + "'" + ",'DD-MON-YYYY hh24:mi:ss')").Append(",");
                        }
                    }
                }
                else if (v.Key.ToString() == primarycolname)
                {
                    if (v.Value.ToString() == "")
                    {
                        valuesbuilder.Append("'" + commonValue.NewVoucherNumber + "'").Append(",");
                    }
                    else
                    {
                        valuesbuilder.Append("'" + v.Value + "'").Append(",");
                    }
                }
                else if (v.Key.ToString() == "MANUAL_NO")
                {
                    valuesbuilder.Append("'" + v.Value + "'").Append(",");
                    manualno = v.Value.ToString();
                }
                else if (v.Key.ToString() == "CURRENCY_CODE")
                {
                    valuesbuilder.Append("'" + v.Value + "'").Append(",");
                    currencyformat = v.Value.ToString();
                }
                else if (v.Key.ToString() == "EXCHANGE_RATE")
                {
                    valuesbuilder.Append("'" + v.Value + "'").Append(",");
                    exchangrate = Convert.ToDecimal(v.Value.ToString());
                }
                else { valuesbuilder.Append("'" + v.Value + "'").Append(","); }
            }
            int serialno = 1;
            //if (voucherno == "undefined")
            //{
            foreach (var item in childcolumnvalues)
            {
                StringBuilder childvaluesbuilder = new StringBuilder();
                StringBuilder masterchildvaluesbuilder = new StringBuilder();
                var itemarray = JsonConvert.DeserializeObject(item.ToString());
                var budget_flag = "L";
                foreach (var data in itemarray)
                {
                    var dataname = data.Name.ToString();
                    string[] datanamesplit = dataname.Split('_');
                    string datalastName = datanamesplit.Last();
                    var datavalue = data.Value;
                    if (datalastName == "DATE")
                    {
                        if (datavalue.Value.ToString() == "")
                        {
                            childvaluesbuilder.Append("SYSDATE").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append("TO_DATE(" + "'" + datavalue.Value + "'" + ",'DD-MON-YYYY hh24:mi:ss')").Append(",");
                        }
                    }
                    else if (datalastName == "PRICE")
                    {
                        if (dataname == "UNIT_PRICE")
                        {
                            if (datavalue.Value == null)
                            {
                                childvaluesbuilder.Append("0.00").Append(",");
                            }
                            else
                            {
                                childvaluesbuilder.Append(datavalue.Value).Append(",");
                            }
                        }
                        if (dataname == "TOTAL_PRICE")
                        {
                            if (datavalue.Value == null)
                            {
                                childvaluesbuilder.Append("0.00").Append(",");

                            }
                            else
                            {
                                childvaluesbuilder.Append(datavalue.Value).Append(",");
                            }
                        }
                        else if (dataname == "CALC_UNIT_PRICE")
                        {
                            if (datavalue.Value.ToString() == "")
                            {
                                childvaluesbuilder.Append("0.00").Append(",");
                            }
                            else
                            {
                                childvaluesbuilder.Append(datavalue.Value).Append(",");
                            }
                        }
                        else if (dataname == "CALC_TOTAL_PRICE")
                        {
                            if (datavalue.Value.ToString() == "")
                            {
                                childvaluesbuilder.Append("0.00").Append(",");
                            }
                            else
                            {
                                childvaluesbuilder.Append(datavalue.Value).Append(",");
                            }
                        }
                    }
                    else if (dataname == "AMOUNT")
                    {
                        if (datavalue.Value.ToString() == "")
                        {
                            childvaluesbuilder.Append("0.00").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append(datavalue.Value).Append(",");
                        }
                    }
                    else if (dataname == "QUANTITY")
                    {
                        if (datavalue.Value == null)
                        {
                            childvaluesbuilder.Append(quantityvalue).Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append(datavalue.Value).Append(",");
                        }

                    }
                    else if (dataname == "CALC_QUANTITY")
                    {
                        if (datavalue.Value == null)
                        {
                            childvaluesbuilder.Append(quantityvalue).Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append(datavalue.Value).Append(",");
                        }
                    }
                    else if (dataname == "COMPLETED_QUANTITY")
                    {

                        if (datavalue.Value == null)
                        {
                            childvaluesbuilder.Append(quantityvalue).Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append(datavalue.Value).Append(",");
                        }
                    }

                    else if (dataname == "BUDGET_FLAG" && datavalue.Value.ToString() == "")
                    {
                        if (datavalue.Value.ToString() == "")
                        {
                            childvaluesbuilder.Append("'" + budget_flag + "'").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append("'" + datavalue.Value + "'").Append(",");
                        }
                    }
                    else if (dataname == "PARTICULARS")
                    {
                        if (datavalue.Value.ToString() == null)
                        {
                            childvaluesbuilder.Append("' '").Append(",");
                        }
                        else
                        {
                            childvaluesbuilder.Append("'" + datavalue.Value + "'").Append(",");
                        }
                    }

                    else
                    {
                        if (datavalue.Value.ToString() == "")
                        {
                            childvaluesbuilder.Append("' '").Append(",");

                        }
                        else
                        {
                            childvaluesbuilder.Append("'" + datavalue.Value + "'").Append(",");
                        }
                    }
                }
                var values = "";
                masterchildvaluesbuilder.Append(valuesbuilder);
                masterchildvaluesbuilder.Append(childvaluesbuilder);
                values = masterchildvaluesbuilder.ToString().TrimEnd(',');
                var insertQuery = string.Format(@"insert into " + model.Table_Name + "({0}) values({1},{2},{3},{4},{5},{6},{7},{8})", Columnbuilder, values, serialno, "'" + model.Form_Code + "'", "'" + this._workContext.CurrentUserinformation.company_code + "'", "'" + this._workContext.CurrentUserinformation.branch_code + "'", "'" + this._workContext.CurrentUserinformation.login_code.ToUpper() + "'", createddatestring, "'N'");
                dbcontext.ExecuteSqlCommand(insertQuery);
                serialno++;
                insertedToChild = true;
            }
            //}
            return insertedToChild;
        }
        public bool SaveMasterColumnValue(Inventory masterColumnValue, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                bool insertedToMaster = false;
                string insertmasterQuery = string.Format(@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,SESSION_ROWID,SYN_ROWID,EXCHANGE_RATE,REFERENCE_NO) 
                     VALUES('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}',{8},TO_DATE({9},'DD-MON-YY hh24:mi:ss'),'{10}','{11}',{12},'{13}')",
                    commonValue.NewVoucherNumber, commonValue.Grand_Total, commonValue.FormCode, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code, _workContext.CurrentUserinformation.login_code.ToUpper(), 'N', commonValue.CurrencyFormat, "SYSDATE", commonValue.VoucherDate, masterColumnValue.MANUAL_NO, '1', commonValue.ExchangeRate, masterColumnValue.MANUAL_NO);
                dbcontext.ExecuteSqlCommand(insertmasterQuery);
                //insertedToMaster = true;

                //  Doubt why this is here
                if (!string.IsNullOrEmpty(commonValue.TempCode))
                {
                    string UpdateQuery = $@"UPDATE FORM_TEMPLATE_SETUP  SET SAVED_DRAFT='Y' WHERE TEMPLATE_NO='{commonValue.TempCode}'  AND  COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
                    dbcontext.ExecuteSqlCommand(UpdateQuery);
                }
                insertedToMaster = true;
                return insertedToMaster;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public void GetFormReference(CommonFieldsForInventory commonValue, List<REF_MODEL_DEFAULT> REF_MODEL, NeoErpCoreEntity dbcontext = null)
        {
            var serialNo = "1";
            foreach (var Ref in REF_MODEL)
            {
                serialNo = Ref.SERIAL_NO;
                if (Ref.TABLE_NAME == "IP_PURCHASE_ORDER")
                {
                    var purchaseOrderRef = $@"select ORDER_NO,TO_CHAR(ORDER_DATE) ORDER_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,PRIORITY_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_PURCHASE_ORDER where ITEM_CODE='{Ref.ITEM_CODE}'  AND order_no='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.ORDER_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.ORDER_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                        //_logErp.WarnInDB("Reference for sales order saved successfully by : " + _workContext.CurrentUserinformation.login_code);
                    }
                }
                if (Ref.TABLE_NAME == "IP_PURCHASE_REQUEST")
                {
                    var purchaseOrderRef = $@"select REQUEST_NO,TO_CHAR(REQUEST_DATE) REQUEST_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_PURCHASE_REQUEST where ITEM_CODE='{Ref.ITEM_CODE}' AND REQUEST_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.REQUEST_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.REQUEST_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
                if (Ref.TABLE_NAME == "IP_GOODS_ISSUE_RETURN")
                {
                    var purchaseOrderRef = $@"select RETURN_NO,TO_CHAR(RETURN_DATE) RETURN_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_GOODS_ISSUE_RETURN where ITEM_CODE='{Ref.ITEM_CODE}'  AND RETURN_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.RETURN_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.RETURN_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
                if (Ref.TABLE_NAME == "IP_GOODS_REQUISITION")
                {
                    var purchaseOrderRef = $@"select REQUISITION_NO,TO_CHAR(REQUISITION_DATE) REQUISITION_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_GOODS_REQUISITION where ITEM_CODE='{Ref.ITEM_CODE}' AND SERIAL_NO='{Ref.SERIAL_NO}' AND REQUISITION_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.REQUISITION_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.REQUISITION_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
                if (Ref.TABLE_NAME == "IP_GATE_PASS_ENTRY")
                {
                    var purchaseOrderRef = $@"select ISSUE_NO,TO_CHAR(ISSUE_DATE) ISSUE_DATE,MANUAL_NO,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_GATE_PASS_ENTRY where ITEM_CODE='{Ref.ITEM_CODE}'  AND ISSUE_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.ISSUE_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.ISSUE_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
                if (Ref.TABLE_NAME == "IP_PURCHASE_INVOICE")
                {
                    var purchaseOrderRef = $@"select INVOICE_NO,TO_CHAR(INVOICE_DATE) INVOICE_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_PURCHASE_INVOICE where ITEM_CODE='{Ref.ITEM_CODE}'  AND INVOICE_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.INVOICE_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.INVOICE_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
                if (Ref.TABLE_NAME == "IP_PURCHASE_RETURN")
                {
                    var purchaseOrderRef = $@"select RETURN_NO,TO_CHAR(RETURN_DATE) RETURN_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_PURCHASE_RETURN where ITEM_CODE='{Ref.ITEM_CODE}'  AND RETURN_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.RETURN_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.RETURN_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
                if (Ref.TABLE_NAME == "IP_RETURNABLE_GOODS_RETURN")
                {
                    var purchaseOrderRef = $@"select ISSUE_NO,TO_CHAR(ISSUE_DATE) ISSUE_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_RETURNABLE_GOODS_RETURN where ITEM_CODE='{Ref.ITEM_CODE}'  AND ISSUE_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.ISSUE_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.ISSUE_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
                if (Ref.TABLE_NAME == "IP_GOODS_ISSUE")
                {
                    var purchaseOrderRef = $@"select ISSUE_NO,TO_CHAR(ISSUE_DATE) ISSUE_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_GOODS_ISSUE where ITEM_CODE='{Ref.ITEM_CODE}'  AND ISSUE_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE,SUB_PROJECT_CODE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.ISSUE_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.ISSUE_DATE}','DD-MON-YYYY hh24:mi:ss'),'{Ref.SUB_PROJECT_CODE}')";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
                if (Ref.TABLE_NAME == "IP_PURCHASE_MRR")
                {
                    var purchaseOrderRef = $@"select MRR_NO,TO_CHAR(MRR_DATE) MRR_DATE,MANUAL_NO,SUPPLIER_CODE,TO_CHAR(SERIAL_NO),ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,CURRENCY_CODE,TO_CHAR(EXCHANGE_RATE) EXCHANGE_RATE,TO_CHAR(SERIAL_NO) SERIAL_NO,TO_CHAR(BRANCH_CODE) BRANCH_CODE from IP_PURCHASE_MRR where ITEM_CODE='{Ref.ITEM_CODE}'  AND MRR_NO='{Ref.VOUCHER_NO}'";
                    var RefPurchaseOrder = dbcontext.SqlQuery<Inventory>(purchaseOrderRef).FirstOrDefault();
                    if (RefPurchaseOrder != null)
                    {
                        var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                        var maxRefNo = dbcontext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                        var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,
                                                    REFERENCE_ITEM_CODE,
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.MRR_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.MRR_DATE}','DD-MON-YYYY hh24:mi:ss'))";

                        dbcontext.ExecuteSqlCommand(refInsQuery);
                    }
                }
            }
        }
        public void SaveBudgetTransactionColumnValue(List<FinancialBudgetTransaction> budgetTransaction, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                var budSerial = 1;
                string maxtransnoquery = string.Format(@"SELECT TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO )+1),1))) as TRANSACTIONNO FROM BUDGET_TRANSACTION");
                int newMaxTransNoForBudget = _coreEntity.SqlQuery<int>(maxtransnoquery).FirstOrDefault();
                foreach (var btrans in budgetTransaction)
                {
                    var budgetflag = btrans.BUDGET_FLAG == "" ? "L" : btrans.BUDGET_FLAG;
                    if (btrans.BUDGET != null)
                    {
                        if (btrans.BUDGET.Count > 0)
                        {
                            foreach (var bud in btrans.BUDGET)
                            {
                                if (bud.BUDGET_CODE != "")
                                {
                                    var VoucherNumber = (commonValue.VoucherNumber == "undefined") ? commonValue.NewVoucherNumber : commonValue.VoucherNumber;
                                    string insertbudgettransQuery = $@"INSERT INTO BUDGET_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,BUDGET_FLAG,SERIAL_NO,BUDGET_CODE,
                                                                              BUDGET_AMOUNT,PARTICULARS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              CURRENCY_CODE,EXCHANGE_RATE,VALIDATION_FLAG,ACC_CODE,SESSION_ROWID,SUB_PROJECT_CODE)
                                                                              VALUES('{newMaxTransNoForBudget}','{commonValue.FormCode}','{VoucherNumber}','{budgetflag}',{budSerial++},'{bud.BUDGET_CODE}',
                                                                             {bud.QUANTITY},'{bud.NARRATION}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','NRS',{1},'Y','{bud.ACC_CODE}','','{btrans.SUB_PROJECT_CODE}')";
                                    dbcontext.ExecuteSqlCommand(insertbudgettransQuery);
                                    newMaxTransNoForBudget = newMaxTransNoForBudget + 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public void SaveBatchTransactionValues(Inventory masterColumnValue, List<BATCHTRANSACTIONDATA> batchTransaction, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null)
        {
            try
            {//var batchSerial = 1;
                var VoucherNumber = (commonValue.VoucherNumber == "undefined") ? commonValue.NewVoucherNumber : commonValue.VoucherNumber;
                string maxtransnoquery = string.Format(@"SELECT TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO )+1),1))) as TRANSACTIONNO FROM BATCH_TRANSACTION");
                int newMaxTransNoForBatch = _coreEntity.SqlQuery<int>(maxtransnoquery).FirstOrDefault();
                foreach (var btrans in batchTransaction)
                {
                    if (btrans.TRACK != null)
                    {
                        if (btrans.TRACK.Count > 0)
                        {

                            var mucode = btrans.MU_CODE == "" ? "PCS" : btrans.MU_CODE;
                            foreach (var bud in btrans.TRACK)
                            {
                                if (bud.TRACKING_SERIAL_NO != "")
                                {

                                    if (commonValue.MODULE_CODE == "02" && masterColumnValue.FROM_LOCATION_CODE != null && masterColumnValue.TO_LOCATION_CODE != null)
                                    {
                                        string insertbatchtransQueryIn = $@"INSERT INTO BATCH_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,SERIAL_NO,ITEM_CODE,                                             BATCH_NO,MU_CODE,QUANTITY,SOURCE_FLAG,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              ITEM_SERIAL_NO,NON_STOCK_FLAG,LOCATION_CODE,ITEM_SERIAL_FLAG)
                                                                              VALUES('{newMaxTransNoForBatch}','{commonValue.FormCode}','{VoucherNumber}',{bud.SERIAL_NO},'{btrans.ITEM_CODE}',                                                                          '{mucode}','{mucode}',1,'I','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','{bud.TRACKING_SERIAL_NO}','N','{btrans.LOCATION_CODE}','Y')";
                                        dbcontext.ExecuteSqlCommand(insertbatchtransQueryIn);
                                        newMaxTransNoForBatch++;


                                        string insertbatchtransQueryOut = $@"INSERT INTO BATCH_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,SERIAL_NO,ITEM_CODE,                                              BATCH_NO,MU_CODE,QUANTITY,SOURCE_FLAG,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              ITEM_SERIAL_NO,NON_STOCK_FLAG,LOCATION_CODE,ITEM_SERIAL_FLAG)
                                                                              VALUES('{newMaxTransNoForBatch}','{commonValue.FormCode}','{VoucherNumber}',{bud.SERIAL_NO},'{btrans.ITEM_CODE}',                                                                           '{mucode}','{mucode}',1,'O','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','{bud.TRACKING_SERIAL_NO}','N','{btrans.LOCATION_CODE}','Y')";
                                        dbcontext.ExecuteSqlCommand(insertbatchtransQueryOut);
                                        newMaxTransNoForBatch++;
                                    }
                                    else
                                    {
                                        string insertbatchtransQuery = $@"INSERT INTO BATCH_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,SERIAL_NO,ITEM_CODE,                                              BATCH_NO,MU_CODE,QUANTITY,SOURCE_FLAG,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              ITEM_SERIAL_NO,NON_STOCK_FLAG,LOCATION_CODE,ITEM_SERIAL_FLAG)
                                                                              VALUES('{newMaxTransNoForBatch}','{commonValue.FormCode}','{VoucherNumber}',{bud.SERIAL_NO},'{btrans.ITEM_CODE}',                                                                           '{mucode}','{mucode}',1,'I','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','{bud.TRACKING_SERIAL_NO}','N','{btrans.LOCATION_CODE}','Y')";
                                        dbcontext.ExecuteSqlCommand(insertbatchtransQuery);
                                        newMaxTransNoForBatch++;
                                    }
                                }

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public void SaveBatchTransValues(List<Inventory> childColumnValue, Inventory masterColumnValue, List<BATCH_TRANSACTION_DATA> batchTrans, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                var VoucherNumber = (commonValue.VoucherNumber == "undefined") ? commonValue.NewVoucherNumber : commonValue.VoucherNumber;
                string maxtransnoquery = string.Format(@"SELECT TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO )+1),1))) as TRANSACTIONNO FROM BATCH_TRANSACTION");
                int newMaxTransNoForBatch = _coreEntity.SqlQuery<int>(maxtransnoquery).FirstOrDefault();
                foreach (var cv in childColumnValue)
                {


                    foreach (var btrans in batchTrans)
                    {
                        if (btrans.BATCH_NO == null)
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(btrans.BATCH_NO))
                        {
                            continue;
                        }
                        if (cv.ITEM_CODE == btrans.ITEM_CODE)
                        {
                            for (int q = 0; q < cv.QUANTITY; q++)
                            {
                                var mucode = btrans.MU_CODE == "" ? "PCS" : btrans.MU_CODE;
                                if (commonValue.MODULE_CODE == "02" && masterColumnValue.FROM_LOCATION_CODE != null && masterColumnValue.TO_LOCATION_CODE != null)
                                {
                                    string insertbatchtransQueryIn = $@"INSERT INTO BATCH_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,SERIAL_NO,ITEM_CODE,                                             BATCH_NO,MU_CODE,QUANTITY,SOURCE_FLAG,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              NON_STOCK_FLAG,LOCATION_CODE,EXPIRY_DATE,BATCH_SERIAL_FLAG)
                                                                              VALUES('{newMaxTransNoForBatch}','{commonValue.FormCode}','{VoucherNumber}',{btrans.SERIAL_NO},'{btrans.ITEM_CODE}',                                                                          '{btrans.BATCH_NO}','{mucode}',1,'I','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','N','{btrans.LOCATION_CODE}',TO_DATE('{btrans.EXPIRY_DATE}','MM/DD/YYYY hh12:mi:ss pm'),'Y')";
                                    dbcontext.ExecuteSqlCommand(insertbatchtransQueryIn);
                                    newMaxTransNoForBatch++;


                                    string insertbatchtransQueryOut = $@"INSERT INTO BATCH_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,SERIAL_NO,ITEM_CODE,                                             BATCH_NO,MU_CODE,QUANTITY,SOURCE_FLAG,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              NON_STOCK_FLAG,LOCATION_CODE,EXPIRY_DATE,BATCH_SERIAL_FLAG)
                                                                              VALUES('{newMaxTransNoForBatch}','{commonValue.FormCode}','{VoucherNumber}',{btrans.SERIAL_NO},'{btrans.ITEM_CODE}',                                                                          '{btrans.BATCH_NO}','{mucode}',1,'I','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','N','{btrans.LOCATION_CODE}',TO_DATE('{btrans.EXPIRY_DATE}','MM/DD/YYYY hh12:mi:ss pm'),'Y')";
                                    dbcontext.ExecuteSqlCommand(insertbatchtransQueryOut);
                                    newMaxTransNoForBatch++;
                                }
                                else
                                {
                                    string insertbatchtransQuery = $@"INSERT INTO BATCH_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,SERIAL_NO,ITEM_CODE,                                             BATCH_NO,MU_CODE,QUANTITY,SOURCE_FLAG,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              NON_STOCK_FLAG,LOCATION_CODE,EXPIRY_DATE,BATCH_SERIAL_FLAG)
                                                                              VALUES('{newMaxTransNoForBatch}','{commonValue.FormCode}','{VoucherNumber}',{btrans.SERIAL_NO},'{btrans.ITEM_CODE}',                                                                          '{btrans.BATCH_NO}','{mucode}',1,'I','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','N','{btrans.LOCATION_CODE}',TO_DATE('{btrans.EXPIRY_DATE}','MM/DD/YYYY hh12:mi:ss pm'),'Y')";
                                    dbcontext.ExecuteSqlCommand(insertbatchtransQuery);
                                    newMaxTransNoForBatch++;
                                }
                            }
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public void SaveChargeColumnValue(List<ChargeOnSales> chargeCol, CommonFieldsForInventory commonField, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                bool insertedToCharges = false;
                string currencyformat = "NRS";
                int chargeSerialNo = 0;
                // var chargeCol = JsonConvert.DeserializeObject<List<ChargeOnSales>>(chargeOnSales);
                foreach (var cc in chargeCol)
                {
                    //string transquery = string.Format(@"select to_number((max(to_number(TRANSACTION_NO)) + 1)) ORDER_NO from CHARGE_TRANSACTION");
                    string transquery = string.Format(@"select TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO )+1),1))) ORDER_NO from CHARGE_TRANSACTION");

                    int newtransno = dbcontext.SqlQuery<int>(transquery).FirstOrDefault();
                    string insertChargeQuery = $@"INSERT INTO CHARGE_TRANSACTION(TRANSACTION_NO,TABLE_NAME,REFERENCE_NO,APPLY_ON,ACC_CODE,CHARGE_CODE,CHARGE_TYPE_FLAG,CHARGE_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,CALCULATE_BY,GL_FLAG,NON_GL_FLAG) VALUES('{newtransno}','{commonField.TableName}','{commonField.NewVoucherNumber}','{cc.APPLY_ON}','{cc.ACC_CODE}','{cc.CHARGE_CODE}','{cc.CHARGE_TYPE_FLAG}', {cc.CHARGE_AMOUNT},'{commonField.FormCode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N','{currencyformat}',{commonField.ExchangeRate},'{cc.VALUE_PERCENT_FLAG}','{cc.GL_FLAG}','{cc.NON_GL_FLAG}')";
                    dbcontext.ExecuteSqlCommand(insertChargeQuery);
                    insertedToCharges = true;
                    chargeSerialNo++;
                }
                //_logErp.WarnInDB("Charges for sales order saved successfully by : " + _workContext.CurrentUserinformation.login_code);
                //return insertedToCharges;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void SaveShippingDetailsColumnValue(ShippingDetails shippingDetails, CommonFieldsForInventory commonFieldForSales, NeoErpCoreEntity Dbcontext = null)
        {
            try
            {
                if (string.IsNullOrEmpty(shippingDetails.VEHICLE_CODE) || string.IsNullOrEmpty(commonFieldForSales.NewVoucherNumber) || string.IsNullOrEmpty(commonFieldForSales.FormCode))
                    return;
                shippingDetails.TRANSPORT_INVOICE_DATE = string.IsNullOrEmpty(shippingDetails.TRANSPORT_INVOICE_DATE) ? "null" : "TO_DATE('" + shippingDetails.TRANSPORT_INVOICE_DATE + "', 'DD-MON-YYYY')";
                shippingDetails.WB_DATE = string.IsNullOrEmpty(shippingDetails.WB_DATE) ? "null" : "TO_DATE('" + shippingDetails.WB_DATE + "', 'DD-MON-YYYY')";
                shippingDetails.GATE_ENTRY_DATE = string.IsNullOrEmpty(shippingDetails.GATE_ENTRY_DATE) ? "null" : "TO_DATE('" + shippingDetails.GATE_ENTRY_DATE + "', 'DD-MON-YYYY')";
                shippingDetails.DELIVERY_INVOICE_DATE = string.IsNullOrEmpty(shippingDetails.DELIVERY_INVOICE_DATE) ? "null" : "TO_DATE('" + shippingDetails.DELIVERY_INVOICE_DATE + "', 'DD-MON-YYYY')";
                string insertSDQuery = $@"INSERT INTO SHIPPING_TRANSACTION (VOUCHER_NO, FORM_CODE, VEHICLE_CODE, VEHICLE_OWNER_NAME, VEHICLE_OWNER_NO, DRIVER_NAME, DRIVER_LICENSE_NO, DRIVER_MOBILE_NO, TRANSPORTER_CODE, FREGHT_AMOUNT, START_FORM, DESTINATION, COMPANY_CODE, BRANCH_CODE, CREATED_DATE, CREATED_BY, DELETED_FLAG, TRANSPORT_INVOICE_NO, CN_NO, TRANSPORT_INVOICE_DATE, DELIVERY_INVOICE_DATE,WB_WEIGHT, WB_NO, WB_DATE,FREIGHT_RATE, VOUCHER_DATE,VEHICLE_NO, LOADING_SLIP_NO, GATE_ENTRY_NO, GATE_ENTRY_DATE,SHIPPING_TERMS) 
                VALUES ('{commonFieldForSales.NewVoucherNumber}','{commonFieldForSales.FormCode}','{shippingDetails.VEHICLE_CODE}','{shippingDetails.VEHICLE_OWNER_NAME}','{shippingDetails.VEHICLE_OWNER_NO}','{shippingDetails.DRIVER_NAME}',
                '{shippingDetails.DRIVER_LICENCE_NO}','{shippingDetails.DRIVER_MOBILE_NO}','{shippingDetails.TRANSPORTER_CODE}','{shippingDetails.FREGHT_AMOUNT}','{shippingDetails.START_FORM}','{shippingDetails.DESTINATION}',
                 '{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}',SYSDATE,'{_workContext.CurrentUserinformation.login_code}','N',
                '{shippingDetails.TRANSPORT_INVOICE_NO}','{shippingDetails.CN_NO}',{shippingDetails.TRANSPORT_INVOICE_DATE},{shippingDetails.DELIVERY_INVOICE_DATE},'{shippingDetails.WB_WEIGHT}',
                '{shippingDetails.WB_NO}',{shippingDetails.WB_DATE},'{shippingDetails.FREIGHT_RATE}',{commonFieldForSales.VoucherDate},'{shippingDetails.VEHICLE_NO}','{shippingDetails.LOADING_SLIP_NO}','{shippingDetails.GATE_ENTRY_NO}',{shippingDetails.GATE_ENTRY_DATE},'{shippingDetails.SHIPPING_TERMS}')";
                Dbcontext.ExecuteSqlCommand(insertSDQuery);
                //_logErp.WarnInDB("shipping details for sales order saved successfully by : " + _workContext.CurrentUserinformation.login_code);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void UpdateShippingDetailsColumnValue(ShippingDetails shippingDetails, CommonFieldsForInventory commonFieldForSales, NeoErpCoreEntity Dbcontext = null)
        {
            try
            {
                if (string.IsNullOrEmpty(shippingDetails.VEHICLE_CODE) || string.IsNullOrEmpty(commonFieldForSales.VoucherNumber) || string.IsNullOrEmpty(commonFieldForSales.FormCode))
                    return;
                shippingDetails.TRANSPORT_INVOICE_DATE = string.IsNullOrEmpty(shippingDetails.TRANSPORT_INVOICE_DATE) ? "null" : "TO_DATE('" + shippingDetails.TRANSPORT_INVOICE_DATE + "', 'DD-MON-YYYY')";
                shippingDetails.WB_DATE = string.IsNullOrEmpty(shippingDetails.WB_DATE) ? "null" : "TO_DATE('" + shippingDetails.WB_DATE + "', 'DD-MON-YYYY')";
                shippingDetails.GATE_ENTRY_DATE = string.IsNullOrEmpty(shippingDetails.GATE_ENTRY_DATE) ? "null" : "TO_DATE('" + shippingDetails.GATE_ENTRY_DATE + "', 'DD-MON-YYYY')";
                shippingDetails.DELIVERY_INVOICE_DATE = string.IsNullOrEmpty(shippingDetails.DELIVERY_INVOICE_DATE) ? "null" : "TO_DATE('" + shippingDetails.DELIVERY_INVOICE_DATE + "', 'DD-MON-YYYY')";
                string updateSDQuery = $@"UPDATE SHIPPING_TRANSACTION SET VEHICLE_CODE='{shippingDetails.VEHICLE_CODE}',VEHICLE_OWNER_NAME='{shippingDetails.VEHICLE_OWNER_NAME}',VEHICLE_OWNER_NO='{shippingDetails.VEHICLE_OWNER_NO}',DRIVER_NAME='{shippingDetails.DRIVER_NAME}',DRIVER_LICENSE_NO='{shippingDetails.DRIVER_LICENCE_NO}',DRIVER_MOBILE_NO='{shippingDetails.DRIVER_MOBILE_NO}',TRANSPORTER_CODE='{shippingDetails.TRANSPORTER_CODE}',FREGHT_AMOUNT='{shippingDetails.FREGHT_AMOUNT}',START_FORM='{shippingDetails.START_FORM}',DESTINATION='{shippingDetails.DESTINATION}',TRANSPORT_INVOICE_NO='{shippingDetails.TRANSPORT_INVOICE_NO}',CN_NO='{shippingDetails.CN_NO}',TRANSPORT_INVOICE_DATE={shippingDetails.TRANSPORT_INVOICE_DATE},DELIVERY_INVOICE_DATE={shippingDetails.DELIVERY_INVOICE_DATE},WB_WEIGHT='{shippingDetails.WB_WEIGHT}',WB_NO='{shippingDetails.WB_NO}',WB_DATE={shippingDetails.WB_DATE},FREIGHT_RATE='{shippingDetails.FREIGHT_RATE}',VEHICLE_NO='{shippingDetails.VEHICLE_NO}',LOADING_SLIP_NO='{shippingDetails.LOADING_SLIP_NO}',GATE_ENTRY_NO='{shippingDetails.GATE_ENTRY_NO}',GATE_ENTRY_DATE={shippingDetails.GATE_ENTRY_DATE}, MODIFY_DATE=SYSDATE,MODIFY_BY='{this._workContext.CurrentUserinformation.login_code}' WHERE VOUCHER_NO='{commonFieldForSales.VoucherNumber}'";
                Dbcontext.ExecuteSqlCommand(updateSDQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void SaveCustomTransaction(List<CustomOrderColumn> customcolumn, CommonFieldsForInventory commom, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                int serialNo = 1;
                foreach (var r in customcolumn)
                {
                    string insertQuery = $@"INSERT INTO CUSTOM_TRANSACTION(
                                                              VOUCHER_NO,FIELD_NAME,FIELD_VALUE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SERIAL_NO,MODIFY_DATE)
                                                              VALUES('{commom.NewVoucherNumber}','{r.FieldName}','{r.FieldValue}','{commom.FormCode}','{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.branch_code}',
                                                         '{this._workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','{serialNo++}',NULL)";
                    dbcontext.ExecuteSqlCommand(insertQuery);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<Inventory> GetMasterTransactionByVoucherNo(string voucherNumber)
        {
            try
            {
                var getPrevDataQuery = $@"SELECT VOUCHER_NO,SESSION_ROWID, CREATED_BY, TO_CHAR(CREATED_DATE, 'DD-MON-YY hh12:mi:ss PM') as CREATED_DATE FROM MASTER_TRANSACTION WHERE VOUCHER_NO= '{voucherNumber}'";
                var defaultData = _coreEntity.SqlQuery<Inventory>(getPrevDataQuery).ToList();
                return defaultData;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                throw new Exception(ex.Message);
            }

        }
        public bool UpdateMasterTransaction(CommonFieldsForInventory commonUpdateValue, NeoErpCoreEntity neoErpCoreEntity = null)
        {
            try
            {
                string query = $@"UPDATE MASTER_TRANSACTION SET VOUCHER_AMOUNT={commonUpdateValue.DrTotal},VOUCHER_DATE={commonUpdateValue.VoucherDate},MODIFY_BY = '{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYN_ROWID='{commonUpdateValue.ManualNumber}',REFERENCE_NO='{commonUpdateValue.ManualNumber}' , MODIFY_DATE = SYSDATE,CURRENCY_CODE='{commonUpdateValue.CurrencyFormat}',EXCHANGE_RATE={commonUpdateValue.ExchangeRate} where VOUCHER_NO='{commonUpdateValue.VoucherNumber}'  and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var rowCount = neoErpCoreEntity.ExecuteSqlCommand(query);

                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public void DeleteBudgetTransaction(string voucherNo, NeoErpCoreEntity coreEntity)
        {
            try
            {
                string deletebudgetcenterquery = string.Format(@"DELETE FROM BUDGET_TRANSACTION where REFERENCE_NO='{0}' and COMPANY_CODE='{1}'", voucherNo, _workContext.CurrentUserinformation.company_code);
                coreEntity.ExecuteSqlCommand(deletebudgetcenterquery);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public void DeleteChargeTransaction(string voucherNo, NeoErpCoreEntity coreEntity)
        {
            try
            {
                string deletebudgetcenterquery = string.Format(@"DELETE FROM CHARGE_TRANSACTION where REFERENCE_NO='{0}' and COMPANY_CODE='{1}'", voucherNo, _workContext.CurrentUserinformation.company_code);
                coreEntity.ExecuteSqlCommand(deletebudgetcenterquery);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public void DeleteCustomTransaction(string voucherNo, NeoErpCoreEntity coreEntity = null)
        {
            try
            {
                string deletecustomcolumn = string.Format(@"DELETE FROM CUSTOM_TRANSACTION where VOUCHER_NO='{0}' and COMPANY_CODE='{1}'", voucherNo, this._workContext.CurrentUserinformation.company_code);
                coreEntity.ExecuteSqlCommand(deletecustomcolumn);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<ShippingDetailsViewModel> GetShippingData(string formCode, string voucherNo)
        {
            try
            {
                string query = string.Empty;
                var result = new List<ShippingDetailsViewModel>();
                if (voucherNo != "undefined")
                {
                    query = $@" SELECT ST.VEHICLE_CODE, ST.VEHICLE_OWNER_NAME, ST.VEHICLE_OWNER_NO, ST.DRIVER_NAME, ST.DRIVER_LICENSE_NO, ST.DRIVER_MOBILE_NO, ST.TRANSPORTER_CODE,TS.TRANSPORTER_EDESC,
FREGHT_AMOUNT, START_FORM, DESTINATION, TRANSPORT_INVOICE_NO, CN_NO, TRANSPORT_INVOICE_DATE, DELIVERY_INVOICE_DATE, 
TRANSPORT_INVOICE_NO, WB_WEIGHT, WB_NO, WB_DATE, FREIGHT_RATE,FREIGHT_PAY_REFERENCE_NO, VEHICLE_NO, LOADING_SLIP_NO,VC.VEHICLE_EDESC,
GATE_ENTRY_NO, GATE_ENTRY_DATE,SHIPPING_TERMS  FROM SHIPPING_TRANSACTION ST,TRANSPORTER_SETUP TS,IP_VEHICLE_CODE VC
WHERE ST.TRANSPORTER_CODE=TS.TRANSPORTER_CODE(+) 
AND ST.COMPANY_CODE=TS.COMPANY_CODE(+)
AND ST.VEHICLE_CODE=VC.VEHICLE_CODE
AND ST.COMPANY_CODE=VC.COMPANY_CODE AND VOUCHER_NO = '{voucherNo}' AND ST.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND ST.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";

                    result = _dbContext.SqlQuery<ShippingDetailsViewModel>(query).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Products> GetProductDataByProductCode(string productcode)
        {
            try
            {
                var productdata = new List<Products>();
                if (productcode != "")
                {
                    string query = $@"SELECT distinct  IT.ITEM_CODE AS ItemCode,
                                   IT.ITEM_EDESC AS ItemDescription,
                                   IT.INDEX_MU_CODE AS ItemUnit,
                                  IT.MULTI_MU_CODE AS MultiItemUnit,
                                  IC.CATEGORY_EDESC  AS Category,
                                   IT.CREATED_BY AS CreatedBy,
                                   TO_CHAR (IT.CREATED_DATE, 'dd-Mon-yyyy') AS CreatedDate
                              FROM ip_item_master_setup IT, IP_CATEGORY_CODE IC
                             WHERE IT.deleted_flag = 'N'
                             AND IT.CATEGORY_CODE= IC.CATEGORY_CODE
                             AND IT.COMPANY_CODE = IC.COMPANY_CODE
                              AND IT.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                               AND IT.item_code ='{productcode}'";
                    productdata = this._dbContext.SqlQuery<Products>(query).ToList();
                    return productdata;
                }
                else
                { return productdata; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<FormCustomSetup> GetFormCustomSetup(string formCode, string voucherNo)
        {
            var customObj = new List<FormCustomSetup>();
            string Query = $@"SELECT * FROM FORM_CUSTOM_SETUP WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
            _logErp.InfoInFile(Query + " is a query to get form custom setup for " + formCode + " formcode ");
            var entity = this._dbContext.SqlQuery<FormCustomSetup>(Query).ToList();

            if (voucherNo != "undefined")
            {
                string transactionQuery = $@"SELECT * FROM CUSTOM_TRANSACTION WHERE FORM_CODE ='{formCode}' AND VOUCHER_NO='{voucherNo}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                _logErp.InfoInFile(Query + " is a query to get form custom setup for " + formCode + " formcode and " + voucherNo + " voucher number");
                customObj = this._dbContext.SqlQuery<FormCustomSetup>(transactionQuery).Where(a => a.FIELD_VALUE != null).ToList();
            }
            if (customObj.Count() > 0)
            {
                foreach (var it in entity)
                {
                    foreach (var i in customObj)
                    {
                        if (it.FIELD_NAME == i.FIELD_NAME)
                            it.FIELD_VALUE = i.FIELD_VALUE;
                    }
                }
            }

            return entity;
        }

        public void DeleteBatchTransaction(string voucherNo, NeoErpCoreEntity coreEntity)
        {
            try
            {
                string deletebatchtransquery = string.Format(@"DELETE FROM BATCH_TRANSACTION where REFERENCE_NO='{0}' and COMPANY_CODE='{1}'", voucherNo, _workContext.CurrentUserinformation.company_code);
                coreEntity.ExecuteSqlCommand(deletebatchtransquery);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<FinanceVoucherReference> GetFinanceVoucherReferenceList(string formcode)
        {
            string Query = $@"  select  f.voucher_no,f.reference_form_code ,FS.FORM_EDESC  from FINANCIAL_REFERENCE_DETAIL  f ,
                form_setup  fs
               where fs.company_code=f.company_code
               and f.deleted_flag=fs.deleted_flag
               and f.form_code=fs.form_code
               and f.company_code='{_workContext.CurrentUserinformation.company_code}'
               and f.form_code=" + formcode;

            List<FinanceVoucherReference> entity = this._dbContext.SqlQuery<FinanceVoucherReference>(Query).ToList();
            return entity;

        }
        public List<REFERENCE_DETAILS> GetReference_Details_For_VoucherNo(string VoucherNo, string formcode)
        {
            List<REFERENCE_DETAILS> result = new List<REFERENCE_DETAILS>();
            string query = $@"SELECT RD.REFERENCE_NO,RD.REFERENCE_ITEM_CODE,IMS.ITEM_EDESC,RD.REFERENCE_QUANTITY, RD.REFERENCE_MU_CODE,RD.REFERENCE_UNIT_PRICE,RD.REFERENCE_TOTAL_PRICE,RD.REFERENCE_CALC_UNIT_PRICE,RD.REFERENCE_CALC_TOTAL_PRICE,RD.REFERENCE_REMARKS FROM REFERENCE_DETAIL RD,IP_ITEM_MASTER_SETUP IMS WHERE RD.VOUCHER_NO='{VoucherNo}' AND IMS.ITEM_CODE=RD.REFERENCE_ITEM_CODE AND RD.COMPANY_CODE=IMS.COMPANY_CODE AND RD.BRANCH_CODE=IMS.BRANCH_CODE AND IMS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND IMS.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
            result = this._dbContext.SqlQuery<REFERENCE_DETAILS>(query).ToList();
            return result;

        }
        public List<PartyType> GetAllPartyTypes()
        {
            var dealer_system_flag = string.Empty;
            string dealer_system_flag_query = $@"SELECT
                        DEALER_SYSTEM_FLAG 
                        FROM PREFERENCE_SUB_SETUP
                        where COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}'";
            dealer_system_flag = this._dbContext.SqlQuery<string>(dealer_system_flag_query).FirstOrDefault();
            if (dealer_system_flag == "Y")
            {

                List<PartyType> result = new List<PartyType>();
                string query = $@"SELECT SA.CUSTOMER_CODE AS PARTY_TYPE_CODE, 
       FA.SUB_CODE,
       SA.CUSTOMER_EDESC AS PARTY_TYPE_EDESC,
       IP.PARTY_TYPE_FLAG
  FROM FA_SUB_LEDGER_DEALER_MAP FA,
       SA_CUSTOMER_SETUP SA,
       IP_PARTY_TYPE_CODE IP
 WHERE     FA.COMPANY_CODE = SA.COMPANY_CODE
       AND TRIM (FA.SUB_CODE) = TRIM (sa.link_sub_code)
       AND SA.COMPANY_CODE = IP.COMPANY_CODE
          AND IP.COMPANY_CODE = FA.COMPANY_CODE
       AND FA.PARTY_TYPE_CODE = IP.PARTY_TYPE_CODE
       AND IP.GROUP_SKU_FLAG='I'
       AND IP.DELETED_FLAG = 'N'
       AND IP.PARTY_TYPE_FLAG='D'
       AND FA.DELETED_FLAG = 'N'
       AND SA.DELETED_FLAG = 'N'
       AND IP.DELETED_FLAG='N' AND IP.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
       ORDER BY TO_NUMBER(SA.CUSTOMER_CODE) ASC";
                result = this._dbContext.SqlQuery<PartyType>(query).ToList();
                return result;
            }
            else
            {

                List<PartyType> result = new List<PartyType>();
                string query = $@"SELECT SA.CUSTOMER_CODE AS PARTY_TYPE_CODE, 
       FA.SUB_CODE,
       SA.CUSTOMER_EDESC AS PARTY_TYPE_EDESC,
       IP.PARTY_TYPE_FLAG
  FROM FA_SUB_LEDGER_DEALER_MAP FA,
       SA_CUSTOMER_SETUP SA,
       IP_PARTY_TYPE_CODE IP
 WHERE     FA.COMPANY_CODE = SA.COMPANY_CODE
       AND TRIM (FA.SUB_CODE) = TRIM (sa.link_sub_code)
       AND SA.COMPANY_CODE = IP.COMPANY_CODE
          AND IP.COMPANY_CODE = FA.COMPANY_CODE
       AND FA.PARTY_TYPE_CODE = IP.PARTY_TYPE_CODE
       AND IP.GROUP_SKU_FLAG='I'
       AND IP.DELETED_FLAG = 'N'
       AND IP.PARTY_TYPE_FLAG='P'
       AND FA.DELETED_FLAG = 'N'
       AND SA.DELETED_FLAG = 'N'
       AND IP.DELETED_FLAG='N' AND IP.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
       ORDER BY TO_NUMBER(SA.CUSTOMER_CODE) ASC";
                result = this._dbContext.SqlQuery<PartyType>(query).ToList();
                return result;
            }
        }
        public List<COMMON_COLUMN> GetProductionFormDetail(string formCode, string TableName, string RoutingCode, decimal ProductQty = 0)
        {
            if (string.IsNullOrEmpty(TableName))
                throw new Exception("Table name is Empty");

            if (string.IsNullOrEmpty(formCode))
                throw new Exception("Form Code is Empty");
            if (string.IsNullOrEmpty(RoutingCode))
                throw new Exception("RoutingCode is Empty");
            //if (ProductQty<=0)
            //    throw new Exception("production Qty is Empty");

            var entityCode = new List<COMMON_COLUMN>();
            if (TableName.ToUpper() == "IP_PRODUCTION_ISSUE")
            {
                var query = $@"SELECT  (select defa_value as FROM_LOCATION_CODE  from form_detail_setup where form_code='{formCode}' and company_code=a.company_code and column_name='FROM_LOCATION_CODE') as FROM_LOCATION_CODE,A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE as MU_CODE, E.MU_EDESC,  case when a.quantity<>0 then to_number(((nvl(a.quantity, 1) / nvl(b.quantity, 1)) * 12)) else 0 end AS quantity,
   case when a.quantity<>0 then to_number(((nvl(a.quantity, 1) / nvl(b.quantity, 1)) * 12))else 0 end AS calc_quantity,0 as UNIT_PRICE,0 as TOTAL_PRICE,0 as CALC_UNIT_PRICE,0 as CALC_TOTAL_PRICE    FROM MP_ROUTINE_INPUT_SETUP A,  MP_ROUTINE_OUTPUT_SETUP B, MP_PROCESS_SETUP C, IP_ITEM_MASTER_SETUP D, IP_MU_CODE E
                                WHERE A.PROCESS_CODE = C.PROCESS_CODE
                                AND A.COMPANY_CODE = C.COMPANY_CODE
                                AND A.COMPANY_CODE = B.COMPANY_CODE
                                AND A.PROCESS_CODE = B.PROCESS_CODE
                                AND A.ITEM_CODE = D.ITEM_CODE
                                AND C.INDEX_ITEM_CODE = B.ITEM_CODE
                                AND C.COMPANY_CODE = B.COMPANY_CODE
                                AND A.COMPANY_CODE = C.COMPANY_CODE
                                AND D.INDEX_MU_CODE = E.MU_CODE
                                AND D.COMPANY_CODE = E.COMPANY_CODE
                                AND C.LOCATION_CODE = '{RoutingCode}'
                                AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                var entity = this._dbContext.SqlQuery<COMMON_COLUMN>(query).ToList();
                return entity;

            }
            else if (TableName.ToUpper() == "IP_PRODUCTION_MRR")
            {
                var query = $@"    SELECT (select defa_value as TO_LOCATION_CODE  from form_detail_setup where form_code='478'
and company_code=a.company_code 
 and column_name='TO_LOCATION_CODE') TO_LOCATION_CODE,A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE AS MU_CODE, E.MU_EDESC FROM  MP_ROUTINE_OUTPUT_SETUP A, MP_PROCESS_SETUP C, IP_ITEM_MASTER_SETUP D, IP_MU_CODE E 
                                WHERE A.PROCESS_CODE = C.PROCESS_CODE 
                                AND A.COMPANY_CODE = C.COMPANY_CODE 
                                AND A.ITEM_CODE = D.ITEM_CODE
                            AND A.COMPANY_CODE = C.COMPANY_CODE 
                            AND D.INDEX_MU_CODE = E.MU_CODE
                    AND D.COMPANY_CODE = E.COMPANY_CODE 
                    AND C.LOCATION_CODE='{RoutingCode}' 
                        AND A.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var entity = this._dbContext.SqlQuery<COMMON_COLUMN>(query).ToList();
                return entity;

            }




            return entityCode;
        }
        public string GetcustomerNameByCode(string code)
        {
            if (code != "undefined" && code != "null")
            {
                //string CUSQuery = $@"SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG='N'";
                string CUSQuery = $@"SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  AND DELETED_FLAG='N'";
                string CUSdata = this._dbContext.SqlQuery<string>(CUSQuery).FirstOrDefault().ToString();
                return CUSdata;
            }
            else
            {
                return "";
            }
        }

        public string GetItemNameByCode(string code)
        {
            if (code != "undefined" && code != "null")
            {
                //string ITEMQuery = $@"SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG='N'";
                string ITEMQuery = $@"SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N'";
                string ITEMdata = this._dbContext.SqlQuery<string>(ITEMQuery).FirstOrDefault().ToString();
                return ITEMdata;
            }
            else
            {
                return "";
            }
        }
        public string CheckVoucherNoReferenced(string voucherno)
        {
            try
            {
                var Count_query = $@"select voucher_no from reference_detail where reference_no='{voucherno}'
                          AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                string Result = _dbContext.SqlQuery<string>(Count_query).FirstOrDefault();
                return Result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public bool CheckVoucherNoPosted(string voucherno)
        {
            try
            {
                var Count_query = $@"select count(*) from sa_posted_transaction where voucher_no='{voucherno}'
                          AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                int Result = _dbContext.SqlQuery<int>(Count_query).FirstOrDefault();
                if (Result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public List<FormDetailSetup> GetFormDetailSetup(string formCode)
        {
            string Query = $@"SELECT FDS.SERIAL_NO,
                            FS.FORM_EDESC,
                            FS.FORM_TYPE,
                            FS.NEGATIVE_STOCK_FLAG,
                           FDS.FORM_CODE,
                           FDS.TABLE_NAME,
                           FDS.COLUMN_NAME,
                           FDS.COLUMN_WIDTH,
                           FDS.COLUMN_HEADER,
                           FDS.TOP_POSITION,
                           FDS.LEFT_POSITION,
                           FDS.DISPLAY_FLAG,
                           FDS.DEFA_VALUE,
                           FDS.IS_DESC_FLAG,
                           FDS.MASTER_CHILD_FLAG,
                           FDS.FORM_CODE,
                           FDS.COMPANY_CODE,
                           CS.COMPANY_EDESC,
                            CS.TELEPHONE,
                            CS.EMAIL,
                            CS.TPIN_VAT_NO,
                            CS.ADDRESS,
                           FDS.CREATED_BY,
                           FDS.CREATED_DATE,
                           FDS.DELETED_FLAG,
                           FDS.FILTER_VALUE,
                           FDS.SYN_ROWID,
                           FDS.MODIFY_DATE,
                           FDS.MODIFY_BY,
                           FS.REFERENCE_FLAG,
                           FS.FREEZE_MASTER_REF_FLAG,
                           FS.REF_FIX_QUANTITY,
                           FS.REF_FIX_PRICE                          
                      FROM    FORM_PROJECT_SETUP FDS
                           LEFT JOIN
                              COMPANY_SETUP CS ON FDS.COMPANY_CODE = CS.COMPANY_CODE
                              LEFT JOIN FORM_SETUP FS
                               ON FDS.FORM_CODE = FS.FORM_CODE AND FDS.COMPANY_CODE = FS.COMPANY_CODE
                     WHERE FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            _logErp.InfoInFile(Query + " is a query for fetching form details setup for " + formCode + " formcode");
            List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
            return entity;
        }
        public bool deletevouchernoInv(string tablename, string formcode, string voucherno, string primarycolumnname)
        {
            using (var trans = _coreEntity.Database.BeginTransaction())
            {
                try
                {
                    var deletmaintableequery = $@"UPDATE {tablename} SET DELETED_FLAG='Y',MODIFY_DATE=SYSDATE,MODIFY_BY='{_workContext.CurrentUserinformation.login_code
                        }' WHERE {primarycolumnname}='{voucherno}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.Company}'";
                    var maintablerowCount = _coreEntity.ExecuteSqlCommand(deletmaintableequery);

                    if (maintablerowCount > 0)
                    {
                        var deletemastertable = $@"UPDATE MASTER_TRANSACTION SET DELETED_FLAG='Y', MODIFY_DATE=SYSDATE,MODIFY_BY='{_workContext.CurrentUserinformation.login_code
                        }' WHERE VOUCHER_NO='{voucherno}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.Company}'";
                        var mastertablerowCount = _coreEntity.ExecuteSqlCommand(deletemastertable);
                    }
                    string deletebudgetcenterquery = string.Format(@"DELETE FROM BUDGET_TRANSACTION where REFERENCE_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    var budgetcenterrowCount = _coreEntity.ExecuteSqlCommand(deletebudgetcenterquery);

                    string deletebatchtransquery = string.Format(@"DELETE FROM BATCH_TRANSACTION where REFERENCE_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    _coreEntity.ExecuteSqlCommand(deletebatchtransquery);

                    string deletecustomcolumn = string.Format(@"DELETE FROM CUSTOM_TRANSACTION where VOUCHER_NO='{0}' and COMPANY_CODE='{1}'", voucherno, this._workContext.CurrentUserinformation.company_code);
                    _coreEntity.ExecuteSqlCommand(deletecustomcolumn);

                    string deleteshippingtransquery = string.Format(@"DELETE FROM SHIPPING_TRANSACTION where  VOUCHER_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    _coreEntity.ExecuteSqlCommand(deleteshippingtransquery);

                    string deletechargequery = string.Format(@"UPDATE CHARGE_TRANSACTION SET DELETED_FLAG = 'Y' where  VOUCHER_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    _coreEntity.ExecuteSqlCommand(deletechargequery);

                    trans.Commit();
                    return true;

                }

                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }
        public List<DraftFormModel> GetDraftFormDetailSetup(string formCode)
        {
            string Query = $@"SELECT SERIAL_NO,FORM_CODE,TABLE_NAME,COLUMN_NAME,COLUMN_VALUE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG FROM FORM_TEMPLATE_DETAIL_SETUP WHERE TEMPLATE_NO ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
            List<DraftFormModel> entity = this._dbContext.SqlQuery<DraftFormModel>(Query).ToList();
            string CUSdata = string.Empty;
            string ITEMdata = string.Empty;
            //foreach (var en in entity)
            //{
            //    if (en.COLUMN_NAME == "CUSTOMER_CODE")
            //    {
            //        string CUSQuery = $@"SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE='{en.COLUMN_VALUE}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG='N'";
            //         CUSdata = this._dbContext.SqlQuery<string>(CUSQuery).FirstOrDefault().ToString();


            //    }
            //    if (en.COLUMN_NAME == "ITEM_CODE")
            //    {
            //        string CUSQuery = $@"SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='{en.COLUMN_VALUE}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG='N'";
            //        ITEMdata = this._dbContext.SqlQuery<string>(CUSQuery).FirstOrDefault().ToString();


            //    }
            //}
            //entity.Add(new DraftFormModel() { COLUMN_NAME = "CUSTOMER_EDESC", COLUMN_VALUE = CUSdata });
            //entity.Add(new DraftFormModel() { COLUMN_NAME = "ITEM_EDESC", COLUMN_VALUE = ITEMdata });
            return entity;
        }
        public List<FormSetup> GetFormSetupByFormCode(string formCode)
        {
            string Query = $@"SELECT FORM_CODE,FORM_EDESC,MASTER_FORM_CODE,PRE_FORM_CODE,MODULE_CODE,GROUP_SKU_FLAG,START_ID_FLAG,ID_GENERATION_FLAG,CUSTOM_SUFFIX_TEXT,START_DATE,
            LAST_DATE,REF_COLUMN_NAME,PRINT_REPORT_FLAG,PRIMARY_MANUAL_FLAG,COMPANY_CODE,CREATED_DATE,DELETED_FLAG,REFERENCE_FLAG,FORM_ACTION_FLAG,MODIFY_DATE FROM FORM_SETUP WHERE FORM_CODE='{formCode}' AND COMPANY_CODE={_workContext.CurrentUserinformation.company_code}  ORDER BY TO_NUMBER(FORM_CODE) ASC";
            _logErp.InfoInFile(Query + " is query to fetch formsetupby form code");
            List<FormSetup> entity = this._dbContext.SqlQuery<FormSetup>(Query).ToList();
            return entity;
        }
        public List<COMMON_COLUMN> GetSalesOrderFormDetail(string formCode, string orderno)
        {
            string columname = $@"SELECT COLUMN_NAME, TABLE_NAME FROM FORM_PROJECT_SETUP WHERE FORM_CODE='{formCode}' and company_code='{_workContext.CurrentUserinformation.company_code}' and display_flag='Y' ORDER BY SERIAL_NO ASC";
            List<FORM_DETAIL_SETUP_COLUMN> columnameentity = this._dbContext.SqlQuery<FORM_DETAIL_SETUP_COLUMN>(columname).ToList();
            var tableName = "";
            tableName = columnameentity[0].TABLE_NAME;
            string Query = string.Empty;
            StringBuilder condition = new StringBuilder();

            //if (tableName == "SA_SALES_ORDER")
            //{
            StringBuilder sb = new StringBuilder();
            foreach (var item in columnameentity)
            {
                sb.Append("SO.").Append(item.COLUMN_NAME).Append(",");
            }
            var primarycolname = GetPrimaryColumnByTableName(tableName);
            var columns = sb.ToString().TrimEnd(',');
            tableName = tableName + " SO";
            Query = $@"SELECT {columns} FROM {tableName}  WHERE FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}'";

            if (Query.Contains("SO.AREA_CODE"))
            {

                Query.Replace("SO.AREA_CODE", "COALESCE(TO_CHAR(SO.AREA_CODE),' ') as SO.AREA_CODE");
            }

            if (Query.Contains("SO.CUSTOMER_CODE"))
            {
                columns = columns + ",CS.CUSTOMER_EDESC,CS.REGD_OFFICE_EADDRESS,CS.TPIN_VAT_NO,CS.TEL_MOBILE_NO1,CS.CUSTOMER_NDESC";
                tableName = tableName + ",SA_CUSTOMER_SETUP CS";
                condition.Append("AND SO.CUSTOMER_CODE=CS.CUSTOMER_CODE AND SO.COMPANY_CODE=CS.COMPANY_CODE AND  CS.DELETED_FLAG='N'");
                Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
            }
            if (Query.Contains("SO.ITEM_CODE"))
            {
                columns = columns + ",IMS.ITEM_EDESC";
                tableName = tableName + ", IP_ITEM_MASTER_SETUP IMS";
                condition.Append("AND SO.ITEM_CODE=IMS.ITEM_CODE AND SO.COMPANY_CODE=IMS.COMPANY_CODE AND  IMS.DELETED_FLAG='N'");
                Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
            }
            if (Query.Contains("SO.PARTY_TYPE_CODE"))
            {
                columns = columns + ",IPC.PARTY_TYPE_EDESC";
                tableName = tableName + ", IP_PARTY_TYPE_CODE IPC";
                condition.Append("AND SO.PARTY_TYPE_CODE=IPC.PARTY_TYPE_CODE(+) AND SO.COMPANY_CODE=IPC.COMPANY_CODE(+)");
                Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
            }
            if (Query.Contains("SO.SUB_PROJECT_CODE"))
            {
                columns = columns + ",SPS.SUB_PROJECT_NAME";
                tableName = tableName + ", SUB_PROJECT_SETUP SPS";
                tableName = tableName + ", PROJECT_SETUP PS";
                condition.Append("AND SO.SUB_PROJECT_CODE=SPS.SUB_PROJECT_ID  AND SPS.DELETED_FLAG='N'");
                condition.Append("AND PS.ID=SPS.PROJECT_ID  AND PS.STATUS='E'");
                Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
            }
            //if (Query.Contains("SO.FROM_LOCATION_CODE"))
            //{
            //    columns = columns + ",ILS.FROM_LOCATION_EDESC";
            //    tableName = tableName + ", IP_LOCATION_SETUP ILS";
            //    condition.Append("AND SO.LOCATION_CODE=ILS.LOCATION_CODE(+) AND SO.COMPANY_CODE=ILS.COMPANY_CODE(+)");
            //    Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
            //}
            //if (Query.Contains("SO.TO_LOCATION_CODE"))
            //{
            //    columns = columns + ",ILS.TO_LOCATION_EDESC";
            //    tableName = tableName + ", IP_LOCATION_SETUP ILS";
            //    condition.Append("AND SO.LOCATION_CODE=ILS.LOCATION_CODE(+) AND SO.COMPANY_CODE=ILS.COMPANY_CODE(+)");
            //    Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
            //}
            //}
            var entity = this._dbContext.SqlQuery<COMMON_COLUMN>(Query).ToList();

            List<DocumentTransaction> imagelist = new List<DocumentTransaction>();
            if (entity.Count > 0)
            {
                string imagequery = $@"SELECT * FROM DOCUMENT_TRANSACTION WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and VOUCHER_NO='{orderno}' and DELETED_FLAG='N'";
                imagelist = this._dbContext.SqlQuery<DocumentTransaction>(imagequery).ToList();
                entity[0].IMAGES_LIST = imagelist;
            }
            return entity;
        }
        public List<BudgetCenter> getBudgetCodeByAccCode(string accCode)
        {
            var result = new List<BudgetCenter>();
            string query = $@"SELECT
                        COALESCE(BUDGET_CODE,' ') as BUDGET_CODE
                        FROM BC_BUDGET_CENTER_MAP
                        where DELETED_FLAG='N'  AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}' AND ACC_CODE = '{accCode}'";
            result = this._dbContext.SqlQuery<BudgetCenter>(query).ToList();
            return result;
        }
        public List<DraftFormModel> getDraftDataByFormCodeAndTempCode(string formCode, string tempCode)
        {
            List<DraftFormModel> Record = new List<DraftFormModel>();
            string query = $@"select TEMPLATE_NO,TO_CHAR(FORM_CODE) AS FORM_CODE,TABLE_NAME,SERIAL_NO,COLUMN_NAME,COLUMN_VALUE from FORM_TEMPLATE_DETAIL_SETUP where  FORM_CODE='{formCode}' AND  template_no='{tempCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' ORDER BY SERIAL_NO";
            Record = this._dbContext.SqlQuery<DraftFormModel>(query).ToList();
            return Record;
        }
        public int? GetTotalVoucher(string form_code, string table_name)
        {

            string Query = $@"SELECT COUNT(DISTINCT INVOICE_NO) AS TOTAL FROM {table_name} WHERE FORM_CODE = '{form_code}' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' ";


            int? entity = this._dbContext.SqlQuery<int>(Query).FirstOrDefault();
            return entity;
        }
        public List<SubProjectData> GetSubProjectList()
        {
            try
            {
                string query = $@"SELECT SPS.SUB_PROJECT_ID AS SubProjectId,SPS.SUB_PROJECT_NAME AS SUB_PROJECTNAME FROM SUB_PROJECT_SETUP SPS 
                                  LEFT JOIN PROJECT_SETUP PS ON (SPS.PROJECT_ID=PS.ID) WHERE PS.STATUS='E' ORDER BY SubProjectId DESC";
                List<SubProjectData> entity = this._dbContext.SqlQuery<SubProjectData>(query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

    }
}
    