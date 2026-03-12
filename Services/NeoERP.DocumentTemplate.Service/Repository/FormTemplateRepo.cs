using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NeoERP.DocumentTemplate.Service.Repository
{
    public class FormTemplateRepo : IFormTemplateRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        NeoErpCoreEntity _coreentity;
        private ICacheManager _cacheManager;
        private IAttributeSetup _attributeSetup;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        public FormTemplateRepo(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity coreentity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._cacheManager = cacheManager;
            _coreentity = coreentity;
            this._attributeSetup = new AttributeSetupRepo(this._dbContext, this._workContext, this._cacheManager);
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
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
                           FS.REF_FIX_PRICE,
                           FS.DISPLAY_RATE,
                           FS.RATE_SCHEDULE_FIX_PRICE,
                           FS.PRICE_CONTROL_FLAG,
                           (select Form_Code from Form_SETUP where module_code='03' and company_code='{_workContext.CurrentUserinformation.company_code}' and REF_FORM_CODE='{formCode}') as NEXT_SCRN_FORM_CODE  --- added for production module only
                      FROM    FORM_DETAIL_SETUP FDS
                           LEFT JOIN
                              COMPANY_SETUP CS ON FDS.COMPANY_CODE = CS.COMPANY_CODE
                              LEFT JOIN FORM_SETUP FS
                               ON FDS.FORM_CODE = FS.FORM_CODE AND FDS.COMPANY_CODE = FS.COMPANY_CODE
                     WHERE FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            _logErp.InfoInFile(Query + " is a query for fetching form details setup for " + formCode + " formcode");
            List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
            return entity;
        }


        public List<FormDetailSetup> GetFormDetailSalesInvoiceJewellerySetup(string formCode)
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
                      FROM    FORM_DETAIL_SETUP FDS
                           LEFT JOIN
                              COMPANY_SETUP CS ON FDS.COMPANY_CODE = CS.COMPANY_CODE
                              LEFT JOIN FORM_SETUP FS
                               ON FDS.FORM_CODE = FS.FORM_CODE AND FDS.COMPANY_CODE = FS.COMPANY_CODE
                     WHERE FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            _logErp.InfoInFile(Query + " is a query for fetching form details setup for " + formCode + " formcode");
            List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
            return entity;
        }
        //public List<FormDetailSetup> GetFormDetailSetup(string formCode)
        //{
        //    string Query = $@"SELECT FDS.SERIAL_NO,
        //                    FS.FORM_EDESC,
        //                    FS.FORM_TYPE,
        //                    FS.NEGATIVE_STOCK_FLAG,
        //                   FDS.FORM_CODE,
        //                   FDS.TABLE_NAME,
        //                   FDS.COLUMN_NAME,
        //                   FDS.COLUMN_WIDTH,
        //                   FDS.COLUMN_HEADER,
        //                   FDS.TOP_POSITION,
        //                   FDS.LEFT_POSITION,
        //                   FDS.DISPLAY_FLAG,
        //                   FDS.DEFA_VALUE,
        //                   FDS.IS_DESC_FLAG,
        //                   FDS.MASTER_CHILD_FLAG,
        //                   FDS.FORM_CODE,
        //                   FDS.COMPANY_CODE,
        //                   CS.COMPANY_EDESC,
        //                    CS.TELEPHONE,
        //                    CS.EMAIL,
        //                    CS.TPIN_VAT_NO,
        //                    CS.ADDRESS,
        //                   FDS.CREATED_BY,
        //                   FDS.CREATED_DATE,
        //                   FDS.DELETED_FLAG,
        //                   FDS.FILTER_VALUE,
        //                   FDS.SYN_ROWID,
        //                   FDS.MODIFY_DATE,
        //                   FDS.MODIFY_BY,
        //                   FS.REFERENCE_FLAG,
        //                   FS.FREEZE_MASTER_REF_FLAG,
        //                   FS.REF_FIX_QUANTITY,
        //                   FS.REF_FIX_PRICE                          
        //              FROM    FORM_DETAIL_SETUP FDS
        //                   LEFT JOIN
        //                      COMPANY_SETUP CS ON FDS.COMPANY_CODE = CS.COMPANY_CODE
        //                      LEFT JOIN FORM_SETUP FS
        //                       ON FDS.FORM_CODE = FS.FORM_CODE AND FDS.COMPANY_CODE = FS.COMPANY_CODE
        //             WHERE FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
        //    _logErp.InfoInFile(Query + " is a query for fetching form details setup for " + formCode + " formcode");
        //    List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
        //    return entity;
        //}
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
        public string GetCustomerPanNoByCustomerCode(string customerCode)
        {
            if (customerCode != "undefined" && customerCode != "null")
            {
                //string ITEMQuery = $@"SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG='N'";
                string customerQuery = $@"SELECT PAN_NO FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_EDESC = '{customerCode}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N'";
                string customerData = this._dbContext.SqlQuery<string>(customerQuery).FirstOrDefault() ?? string.Empty;
                return customerData;
            }
            else
            {
                return "";
            }
        }
        public string GetAccNameByCode(string code)
        {
            try
            {
                if (code != "undefined")
                {
                    if (code != null)
                    {
                        string ACCQuery = $@"SELECT ACC_EDESC FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE ACC_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N'";
                        var ACCdata = this._dbContext.SqlQuery<string>(ACCQuery).FirstOrDefault().ToString();
                        return string.IsNullOrEmpty(ACCdata) ? "" : ACCdata;
                    }
                    else
                    {
                        return "";
                    }
                    //string ACCQuery = $@"SELECT ACC_EDESC FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE ACC_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG='N'";
                    //var ACC_EDESC = _dbContext.SqlQuery<string>(ACCQuery);
                    //var ACCdata = ACC_EDESC.ToString();
                    //return ACCdata;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
                //return "";
            }


        }
        public string GetParytTypeNameByCode(string code)
        {
            try
            {
                if (code != "undefined" && code != "null")
                {
                    string ACCQuery = $@"SELECT PARTY_TYPE_EDESC FROM ip_party_type_code WHERE PARTY_TYPE_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  AND DELETED_FLAG='N'";
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

        public string GetBudgetNameByCode(string code)
        {
            try
            {
                if (code != "undefined" && code != "null")
                {
                    string ACCQuery = $@"SELECT BUDGET_EDESC FROM BC_BUDGET_CENTER_SETUP WHERE BUDGET_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  AND DELETED_FLAG='N'";
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
        public COMMON_COLUMN GetReferenceNoByOrderNo(string orderno)
        {
            string ReferenceNoQuery = $@"select REFERENCE_NO as ORDER_NO,REFERENCE_DATE as ORDER_DATE from reference_detail where voucher_no='{orderno}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG='N'";
            var Referencedata = this._dbContext.SqlQuery<COMMON_COLUMN>(ReferenceNoQuery).FirstOrDefault();
            return Referencedata;
        }

        public CompanyAndBranchModel GetCompanyAndBranchData()
        {
            string ReferenceNoQuery = $@"SELECT CS.COMPANY_CODE AS COMPANY_CODE, CS.COMPANY_EDESC AS COMPANY_EDESC, CS.ADDRESS AS ADDRESS, CS.TELEPHONE AS TELEPHONE,
                                     CS.EMAIL AS EMAIL,
                                     CS.FAX AS FAX,
                                     CS.LOGO_FILE_NAME AS LOGO_FILE_NAME,
                                     CS.TPIN_VAT_NO AS TPIN_VAT_NO,
                                     BS.BRANCH_CODE AS BRANCH_CODE,
                                     BS.BRANCH_EDESC AS BRANCH_EDESC,
                                     BS.ADDRESS AS BRANCH_ADDRESS,
                                     BS.TELEPHONE_NO AS BRANCH_TELEPHONE_NO,
                                     BS.EMAIL AS BRANCH_EMAIL
                                    FROM COMPANY_SETUP CS
                                    JOIN FA_BRANCH_SETUP BS
                                    ON CS.COMPANY_CODE = BS.COMPANY_CODE
                                    WHERE CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                    AND BS.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                                    AND CS.DELETED_FLAG = 'N'
                                    AND BS.DELETED_FLAG = 'N'";

            var Referencedata = this._dbContext.SqlQuery<CompanyAndBranchModel>(ReferenceNoQuery).FirstOrDefault();
            return Referencedata;
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

        public List<CUSTOM_TRANSACTION> GetCustomTransactionForReference(string formCode, string voucherNo)
        {
            string transactionQuery = $@"SELECT * FROM CUSTOM_TRANSACTION WHERE FORM_CODE ='{formCode}' AND VOUCHER_NO='{voucherNo}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
            _logErp.InfoInFile(transactionQuery + " is a query to get form custom setup for " + formCode + " formcode and " + voucherNo + " voucher number");
            var result = this._dbContext.SqlQuery<CUSTOM_TRANSACTION>(transactionQuery).Where(a => a.FIELD_VALUE != null).ToList();

            return result;
        }
        public List<Customers> GetAllCustomerSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Customers> customerList = new List<Customers>();
                string key = DocumentCacheConstants.CustomerCacheKey;
                if (this._cacheManager.IsSet(key))
                {
                    customerList = this._cacheManager.Get<List<Customers>>(key).Where(a => a.CustomerName.Contains(filter)
                    || a.CustomerCode.Contains(filter) || a.REGD_OFFICE_EADDRESS.Contains(filter)
                     || a.GuestName.Contains(filter)
                    || a.TEL_MOBILE_NO1.Contains(filter) || a.TPIN_VAT_NO.Contains(filter)
                    || a.REGION_CODE.Contains(filter) || a.ZONE_CODE.Contains(filter)
                    || a.DEALING_PERSON.Contains(filter)).ToList();
                }
                else if (!this._cacheManager.IsSet(key) || string.IsNullOrEmpty(filter))
                {
                    string query = string.Format(@"SELECT
                    INITCAP(CS.CUSTOMER_EDESC) AS CustomerName,
                    CS.CUSTOMER_CODE AS CustomerCode,
                    COALESCE(CS.REGD_OFFICE_EADDRESS,' ') REGD_OFFICE_EADDRESS,
                    COALESCE(CS.TEL_MOBILE_NO1,' ') TEL_MOBILE_NO1,
                    COALESCE(cs.TPIN_VAT_NO,' ') TPIN_VAT_NO,
                    COALESCE(CS.REGION_CODE,' ') REGION_CODE,
                    COALESCE(CS.ZONE_CODE,' ') ZONE_CODE,
                    COALESCE(CS.DEALING_PERSON,' ') DEALING_PERSON,
                    TEL_MOBILE_NO1
                    FROM SA_CUSTOMER_SETUP CS
                    where CS.DELETED_FLAG='N' 
                     AND company_code={1}
                    and (upper(CS.CUSTOMER_EDESC) like '%{0}%'
                    OR upper(CS.CUSTOMER_CODE) like '%{0}%'
                    OR upper(CS.REGD_OFFICE_EADDRESS) like '%{0}%'
                    OR upper(CS.TEL_MOBILE_NO1) like '%{0}%'
                    OR upper(cs.TPIN_VAT_NO) like '%{0}%'
                    OR upper(CS.REGION_CODE) like '%{0}%'
                    OR upper(CS.ZONE_CODE) like '%{0}%'
                    OR upper(CS.DEALING_PERSON) like '%{0}%')
                    order by Customer_code", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                    customerList = this._dbContext.SqlQuery<Customers>(query).ToList();
                }
                return customerList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Customers> GetCustomerDetail(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Customers> customerList = new List<Customers>();

                string query = string.Format(@"SELECT
                    INITCAP(CS.CUSTOMER_EDESC) AS CustomerName,
                    INITCAP(CS.CUSTOMER_NDESC) AS GuestName,
                    CS.CUSTOMER_CODE AS CustomerCode,
                    COALESCE(CS.REGD_OFFICE_EADDRESS,' ') REGD_OFFICE_EADDRESS,
                    COALESCE(CS.TEL_MOBILE_NO1,' ') TEL_MOBILE_NO1,
                    COALESCE(cs.TPIN_VAT_NO,' ') TPIN_VAT_NO,
                    COALESCE(CS.REGION_CODE,' ') REGION_CODE,
                    COALESCE(CS.EMAIL,' ') EMAIL,
                    COALESCE(CS.ZONE_CODE,' ') ZONE_CODE,
                    COALESCE(CS.DEALING_PERSON,' ') DEALING_PERSON,
                    TEL_MOBILE_NO1
                    FROM SA_CUSTOMER_SETUP CS
                    where CS.DELETED_FLAG='N' 
                     AND company_code={1}                  
                    AND CS.CUSTOMER_CODE = '{0}'", filter, _workContext.CurrentUserinformation.company_code);
                customerList = this._dbContext.SqlQuery<Customers>(query).ToList();

                return customerList;
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
        public List<IssueType> getAllIssueTypeListByFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = string.Format(@"SELECT DISTINCT COALESCE(ISSUE_TYPE_CODE,' ') ISSUE_TYPE_CODE,
                                    COALESCE(ISSUE_TYPE_EDESC,' ') ISSUE_TYPE_EDESC
                                    from ip_issue_type_code   where deleted_flag='N'  and ISSUE_TYPE_CODE like '%{0}%' and COMPANY_CODE='{1}'
                                    or upper(ISSUE_TYPE_EDESC)like '%{0}%'", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                var result = _dbContext.SqlQuery<IssueType>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Department> GetAllDepartmentSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Department> DepartmentList = new List<Department>();
                string query = string.Format($@"Select
                        COALESCE(Department_code,' ') as DepartmentCode, 
                        COALESCE(Department_edesc,' ') as DepartmentName 
                        from hr_department_code
                        where Deleted_flag='N' and COMPANY_CODE='{1}'
                        and(upper(department_edesc) like '%{0}%' or upper(department_code) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
                DepartmentList = this._dbContext.SqlQuery<Department>(query).ToList();
                return DepartmentList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Employee> GetAllEmployeeSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var query = string.Format(@"select 
                             COALESCE(EMPLOYEE_CODE,' ') EMPLOYEE_CODE
                            ,COALESCE(EMPLOYEE_EDESC,' ') EMPLOYEE_EDESC
                            ,COALESCE(EMAIL,' ') EMAIL
                            ,COALESCE(EPERMANENT_ADDRESS1,' ') EPERMANENT_ADDRESS1
                            ,COALESCE(ETEMPORARY_ADDRESS1,' ') ETEMPORARY_ADDRESS1
                            ,COALESCE(MOBILE,' ') MOBILE
                            ,COALESCE(CITIZENSHIP_NO,' ') CITIZENSHIP_NO 
                            from HR_EMPLOYEE_SETUP 
                            where deleted_flag='N' and COMPANY_CODE='{1}'
                            and Employee_code like '%{0}%' 
                            or upper(employee_edesc) like '%{0}%' 
                            or upper(epermanent_address1) like '%{0}%'
                            OR ETEMPORARY_ADDRESS1 LIKE '%{0}%' 
                            OR CITIZENSHIP_NO LIKE '%{0}%' 
                            OR EMAIL LIKE '%{0}%'  
                            OR MOBILE LIKE '%{0}%'",
                            filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
                var employeeList = this._dbContext.SqlQuery<Employee>(query).ToList();
                return employeeList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public List<Employee> GetAllEmployees(string filter)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
        //        var query = string.Format(@"select 
        //                     COALESCE(EMPLOYEE_CODE,' ') EMPLOYEE_CODE
        //                    ,COALESCE(EMPLOYEE_EDESC,' ') EMPLOYEE_EDESC
        //                    ,COALESCE(EMAIL,' ') EMAIL
        //                    ,COALESCE(EPERMANENT_ADDRESS1,' ') EPERMANENT_ADDRESS1
        //                    ,COALESCE(ETEMPORARY_ADDRESS1,' ') ETEMPORARY_ADDRESS1
        //                    ,COALESCE(MOBILE,' ') MOBILE
        //                    ,COALESCE(CITIZENSHIP_NO,' ') CITIZENSHIP_NO 
        //                    from HR_EMPLOYEE_SETUP 
        //                    where deleted_flag='N' and COMPANY_CODE='{1}'
        //                    and Employee_code like '%{0}%' 
        //                    or upper(employee_edesc) like '%{0}%' 
        //                    or upper(epermanent_address1) like '%{0}%'
        //                    OR ETEMPORARY_ADDRESS1 LIKE '%{0}%' 
        //                    OR CITIZENSHIP_NO LIKE '%{0}%' 
        //                    OR EMAIL LIKE '%{0}%'  
        //                    OR MOBILE LIKE '%{0}%'",
        //                    filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
        //        var employeeList = this._dbContext.SqlQuery<Employee>(query).ToList();
        //        return employeeList;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        public List<Location> GetAllLocationSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Location> LocationList = new List<Location>();
                string query = string.Format(@"select  
                        COALESCE(location_code,' ') as LocationCode
                        ,COALESCE(Location_edesc,' ') as LocationName 
                        ,COALESCE(Address,' ') as Address 
                        ,COALESCE(Auth_Contact_Person,' ') as Auth_Contact_Person
                        ,COALESCE(Telephone_Mobile_No,' ') as Telephone_Mobile_No
                        ,COALESCE(Email,' ') as Email
                        ,COALESCE(Fax,' ') as Fax
                        from IP_location_setup 
                        where deleted_flag='N' AND GROUP_SKU_FLAG='I' and COMPANY_CODE='{1}'
                        and (location_code like '%{0}%' or upper(location_edesc) like '%{0}%'
                                or upper(Auth_Contact_Person) like '%{0}%'
                                or Telephone_Mobile_No like '%{0}%'
                                or upper(Email) like '%{0}%'
                                or Fax like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                LocationList = this._dbContext.SqlQuery<Location>(query).ToList();
                return LocationList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Location> GetAllLocation()
        {
            try
            {
                List<Location> LocationList = new List<Location>();
                string query = string.Format(@"select  
                        COALESCE(location_code,' ') as LocationCode
                        ,COALESCE(Location_edesc,' ') as LocationName 
                        ,COALESCE(Address,' ') as Address 
                        ,COALESCE(Auth_Contact_Person,' ') as Auth_Contact_Person
                        ,COALESCE(Telephone_Mobile_No,' ') as Telephone_Mobile_No
                        ,COALESCE(Email,' ') as Email
                        ,COALESCE(Fax,' ') as Fax
                        from IP_location_setup 
                        where deleted_flag='N' AND GROUP_SKU_FLAG='I' and COMPANY_CODE='{0}'", _workContext.CurrentUserinformation.company_code);
                LocationList = this._dbContext.SqlQuery<Location>(query).ToList();
                return LocationList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Products> GetAllProducts(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Products> ProductsList = new List<Products>();

                string query = string.Format(@"select 
                    COALESCE(item_code,' ') as ItemCode
                    ,COALESCE(item_edesc,' ') as ItemDescription
                    ,COALESCE(index_mu_code,' ') as ItemUnit 
                    from ip_item_master_setup
                    where deleted_flag='N'
                    AND company_code={1} AND GROUP_SKU_FLAG = 'I'
                    and
                    (
                        upper(item_code) like '%{0}%'
                        or upper(item_edesc) like '%{0}%'
                        or upper(index_mu_code) like '%{0}%'
                    ) order by item_code", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
                ProductsList = this._dbContext.SqlQuery<Products>(query).ToList();
                return ProductsList;
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
        public List<EmployeeCodeModels> getAllEmployee()
        {
            string query = @"SELECT DISTINCT 
                        INITCAP(EMPLOYEE_EDESC) AS EMPLOYEE_EDESC,
                        EMPLOYEE_CODE ,
                        MASTER_EMPLOYEE_CODE, 
                        PRE_EMPLOYEE_CODE,
                        EPERMANENT_ADDRESS1,
                        MOBILE,
                        GROUP_SKU_FLAG 
                        FROM HR_EMPLOYEE_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        CONNECT BY PRIOR MASTER_EMPLOYEE_CODE = PRE_EMPLOYEE_CODE
                        ORDER BY EMPLOYEE_CODE";
            var employeeList = _dbContext.SqlQuery<EmployeeCodeModels>(query).ToList();
            return employeeList;
        }
        public List<CustomerModels> getAllCustomerMaster()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        INITCAP(CUSTOMER_NDESC) AS CUSTOMER_NDESC,
                        CUSTOMER_CODE,
                        PREFIX_TEXT AS CUSTOMER_PREFIX,
                        GROUP_START_NO AS CUSTOMER_STARTID,
                        CUSTOMER_FLAG,
                        ACC_CODE,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM SA_CUSTOMER_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        AND PRE_CUSTOMER_CODE = 00
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
                        ORDER BY PRE_CUSTOMER_CODE";
            var customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();
            return customerList;
        }
        public List<DivisionModels> getAllDivision()
        {
            string query = @"  SELECT DISTINCT 
                        INITCAP(DIVISION_EDESC) AS DIVISION_EDESC,
                        DIVISION_CODE ,      
                        PRE_DIVISION_CODE,
                        ADDRESS,
                        GROUP_SKU_FLAG 
                        FROM FA_DIVISION_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        CONNECT BY PRIOR DIVISION_CODE = PRE_DIVISION_CODE
                        ORDER BY PRE_DIVISION_CODE";
            var divisionList = _dbContext.SqlQuery<DivisionModels>(query).ToList();
            return divisionList;
        }

        public string createNewChildCustomerSetupInSIJ(CustomerModels model)
        {
            using (var trans = _coreentity.Database.BeginTransaction())
            {
                try
                {

                    var newMaxCustomerCode = string.Empty;

                    if (model.CUSTOMER_CODE != null && model.PRE_CUSTOMER_CODE != "")
                    {
                        var newprecode = string.Empty;
                        var newMaxMasterCustomerCode = string.Empty;
                        var newmastercode = string.Empty;

                        if (model.CUSTOMER_CODE != null && model.CUSTOMER_CODE != "")
                        {
                            var newMaxCustomerCodeQuery = $@"SELECT nvl(max(to_number(CUSTOMER_CODE))+1,1) as MAX_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                            newMaxCustomerCode = this._coreentity.SqlQuery<int>(newMaxCustomerCodeQuery).FirstOrDefault().ToString();
                            var maxprecodequery = $@"SELECT NVL(MAX(substr(MASTER_CUSTOMER_CODE,-instr(reverse(MASTER_CUSTOMER_CODE),'.')+1)),0)+1 FROM SA_CUSTOMER_SETUP WHERE PRE_CUSTOMER_CODE = '{model.MASTER_CUSTOMER_CODE}' ";
                            var maxPreCode = this._coreentity.SqlQuery<int>(maxprecodequery).FirstOrDefault().ToString();

                            if (maxPreCode != null)
                            {
                                if (Convert.ToInt32(maxPreCode) <= 9)
                                {
                                    maxPreCode = "0" + maxPreCode.ToString();
                                }

                            }
                            newmastercode = model.MASTER_CUSTOMER_CODE + "." + maxPreCode;
                            var childsqlquery = $@"INSERT INTO SA_CUSTOMER_SETUP (CUSTOMER_CODE,
                                                    CUSTOMER_EDESC,CUSTOMER_NDESC,PRE_CUSTOMER_CODE,ACC_CODE,
                                                    CUSTOMER_FLAG,PREFIX_TEXT,GROUP_START_NO,REMARKS,
                                                    MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG,COMPANY_CODE,
                                                    CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE)
                                                    VALUES('{newMaxCustomerCode}','{model.CUSTOMER_EDESC}','{model.CUSTOMER_NDESC}','{model.MASTER_CUSTOMER_CODE}','{model.CUSTOMER_ACCOUNT}','{model.CUSTOMER_TYPE}','{model.CUSTOMER_PREFIX}'
                                                        ,'{model.CUSTOMER_STARTID}','{model.REMARKS}','{newmastercode}','{model.GROUP_SKU_FLAG}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),
                                                         'N','{_workContext.CurrentUserinformation.branch_code}')";
                            var insertchild = _coreentity.ExecuteSqlCommand(childsqlquery);

                        }


                    }
                    else
                    {
                        var maxCustomerCode = string.Empty;
                        var maxC_code = string.Empty;
                        var newMaxCustomerCodeQuery1 = $@" SELECT nvl(max(to_number(CUSTOMER_CODE))+1,1)  as MAX_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                        var newMaxCustomerCode1 = this._coreentity.SqlQuery<int>(newMaxCustomerCodeQuery1).FirstOrDefault().ToString();
                        var maxMasterCustomerCode = string.Empty;
                        var newprequery = $@"SELECT  max(REGEXP_SUBSTR(MASTER_CUSTOMER_CODE, '[^.]+', 1, 1))+1 maxCustomerMasterCode  FROM SA_CUSTOMER_SETUP WHERE   COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                        var newMasterAndCustomerCode = this._coreentity.SqlQuery<CustomerModels>(newprequery).FirstOrDefault();



                        var getNewMasterCustomerCode = $@"
                                SELECT 
                                    TO_CHAR(MASTER_CUSTOMER_CODE) || '.00' AS MASTER_CUSTOMER_CODE,
                                    MASTER_CUSTOMER_CODE AS PRE_CUSTOMER_CODE
                                FROM SA_CUSTOMER_SETUP
                                WHERE CUSTOMER_CODE = '{model.MASTER_CUSTOMER_CODE}'
                                  AND GROUP_SKU_FLAG = 'G'
                                  AND DELETED_FLAG = 'N'";
                        var newMasterAndPreCustomerCode = this._coreentity.SqlQuery<CustomerModels>(getNewMasterCustomerCode).FirstOrDefault();

                        if (Convert.ToInt32(newMasterAndCustomerCode.maxCustomerCode) <= 9)
                        {
                            maxCustomerCode = "0" + newMasterAndCustomerCode.maxCustomerCode;

                        }
                        else
                        {
                            maxCustomerCode = newMasterAndCustomerCode.maxCustomerCode.ToString();
                        }
                        if (Convert.ToInt32(newMasterAndCustomerCode.maxCustomerMasterCode) <= 9)
                        {
                            maxMasterCustomerCode = "0" + newMasterAndCustomerCode.maxCustomerMasterCode;
                        }

                        else
                        {
                            maxMasterCustomerCode = newMasterAndCustomerCode.maxCustomerMasterCode.ToString();

                        }

                        var childsqlquery = $@"INSERT INTO SA_CUSTOMER_SETUP (CUSTOMER_CODE,
                                                    CUSTOMER_EDESC,CUSTOMER_NDESC,PRE_CUSTOMER_CODE,ACC_CODE,
                                                    CUSTOMER_FLAG,PREFIX_TEXT,GROUP_START_NO,REMARKS,
                                                    MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG,COMPANY_CODE,
                                                    CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE,DEALING_PERSON,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,
                                                    PAN_NO,EXCLUSIVE_FLAG,CASH_CUSTOMER_FLAG,REGION_CODE,CITY_CODE)
                                                    VALUES('{newMaxCustomerCode1}','{model.CUSTOMER_EDESC}','{model.CUSTOMER_NDESC}','{newMasterAndPreCustomerCode.PRE_CUSTOMER_CODE}','{model.CUSTOMER_ACCOUNT}','{model.CUSTOMER_TYPE}','{model.CUSTOMER_PREFIX}'
                                                        ,'{model.CUSTOMER_STARTID}','{model.REMARKS}','{newMasterAndPreCustomerCode.MASTER_CUSTOMER_CODE}','{model.GROUP_SKU_FLAG}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),
                                                         'N','{_workContext.CurrentUserinformation.branch_code}','{model.DEALING_PERSON}','{model.REGD_OFFICE_EADDRESS}','{model.TEL_MOBILE_NO1}','{model.PAN_VAT}','{model.EXCLUSIVE_FLAG}','{model.CASH_CUSTOMER_FLAG}','{model.REGION_CODE}','{model.CITY_CODE}')";
                        var insertchild = _coreentity.ExecuteSqlCommand(childsqlquery);
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
        public List<CostCenter> GetAllCostCenter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<CostCenter> CostCenterList = new List<CostCenter>();
                string query = string.Format(@"select COALESCE(Budget_code,' ') as BudgetCode,COALESCE(Budget_EDESC,' ') as BudgetName 
                                from BC_BUDGET_CENTER_SETUP 
                                where deleted_flag='N' and COMPANY_CODE={1} and
                                (budget_code like '%{0}%' or Budget_EDESC like '%{0}%')",
                                filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                CostCenterList = this._dbContext.SqlQuery<CostCenter>(query).ToList();
                return CostCenterList;
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
        public List<PurchaseType> getPurchaseTypeListByFlter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }



                var sqlquery = $@"SELECT P_TYPE_CODE AS P_TYPE, P_TYPE_EDESC FROM IP_PURCHASE_TYPE_CODE WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG = 'N'
                             AND (P_TYPE_CODE like '%{filter.ToUpperInvariant()}%' 
                            OR upper(P_TYPE_EDESC) like '%{filter.ToUpperInvariant()}%')";
                var result = _dbContext.SqlQuery<PurchaseType>(sqlquery).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<PaymentMode> getPaymentModeListByFlter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }



                var sqlquery = $@"SELECT PAYMENT_MODE_CODE,PAYMENT_MODE_EDESC from FA_PAYMENT_MODE_CODE where company_code='{_workContext.CurrentUserinformation.company_code}' and DELETED_FLAG='N'";
                var result = _dbContext.SqlQuery<PaymentMode>(sqlquery).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<Brand> getBrandListByFlter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var sqlquery = string.Format(@"SELECT DISTINCT
                                    COALESCE(ITEM_CODE,' ') ITEM_CODE,
                                    COALESCE(ITEM_EDESC,' ') ITEM_EDESC
                                    from IP_ITEM_MASTER_SETUP
                                    where deleted_flag='N' AND
                                    GROUP_SKU_FLAG = 'G' and COMPANY_CODE={1}  and ITEM_CODE like '%{0}%'
                                    or upper(ITEM_EDESC)like '%{0}%'", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
                var result = _dbContext.SqlQuery<Brand>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
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
        public List<Agent> GetAllAgentSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Agent> PriorityList = new List<Agent>();
                string query = string.Format(@"Select
                        COALESCE(AGENT_CODE,' ') as AGENT_CODE, 
                        COALESCE(AGENT_EDESC,' ') as AGENT_EDESC 
                        from AGENT_SETUP
                        where DELETED_FLAG='N' and COMPANY_CODE='{1}'
                        and(upper(AGENT_EDESC) like '%{0}%' or upper(AGENT_CODE) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                PriorityList = this._dbContext.SqlQuery<Agent>(query).ToList();
                return PriorityList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Priority> GetAllPrioritySetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<Priority> PriorityList = new List<Priority>();
                string query = string.Format(@"Select
                        COALESCE(PRIORITY_CODE,' ') as PRIORITY_CODE, 
                        COALESCE(PRIORITY_EDESC,' ') as PRIORITY_EDESC 
                        from IP_PRIORITY_CODE
                        where DELETED_FLAG='N' and COMPANY_CODE='{1}'
                        and(upper(PRIORITY_EDESC) like '%{0}%' or upper(PRIORITY_CODE) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                PriorityList = this._dbContext.SqlQuery<Priority>(query).ToList();
                return PriorityList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<TNC> GetAllTNCSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<TNC> TNCList = new List<TNC>();
                string query = string.Format(@"Select
                        COALESCE(TNC_CODE,' ') as TNC_CODE, 
                        COALESCE(TNC_EDESC,' ') as TNC_EDESC 
                        from TERMS_AND_CONDITION_SETUP
                        where DELETED_FLAG='N' and COMPANY_CODE='{1}' and GROUP_SKU_FLAG = 'G'
                        and(upper(TNC_EDESC) like '%{0}%' or upper(TNC_CODE) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                TNCList = this._dbContext.SqlQuery<TNC>(query).ToList();
                return TNCList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ItemUnit> GetItemUnit(string itemCode)
        {
            try
            {
                List<ItemUnit> itemUnit = new List<ItemUnit>();
                if (itemCode != "undefined" && itemCode != "null" && itemCode != "")
                {
                    string query = string.Format($@"SELECT ITEM_CODE, SERIAL_NO, CONVERSION_FACTOR, FRACTION FROM IP_ITEM_UNIT_SETUP WHERE ITEM_CODE = '{itemCode}' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' ORDER BY SERIAL_NO ASC");
                    itemUnit = this._dbContext.SqlQuery<ItemUnit>(query).ToList();
                    return itemUnit;
                }
                else
                {
                    return itemUnit;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<SalesType> GetAllSalesTypeSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<SalesType> SalesTypeList = new List<SalesType>();
                string query = string.Format(@"Select
                        COALESCE(SALES_TYPE_CODE,' ') as SALES_TYPE_CODE, 
                        COALESCE(SALES_TYPE_EDESC,' ') as SALES_TYPE_EDESC 
                        from SA_SALES_TYPE
                        where DELETED_FLAG='N' and COMPANY_CODE='{1}' and GROUP_SKU_FLAG = 'I'
                        and(upper(SALES_TYPE_EDESC) like '%{0}%' or upper(SALES_TYPE_CODE) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
                SalesTypeList = this._dbContext.SqlQuery<SalesType>(query).ToList();
                return SalesTypeList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string SaveFormData(string formcode, string columnname, string colunmvalue)
        {
            try
            {
                return "sucess";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string NewVoucherNo(string companycode, string formcode, string transactiondate, string tablename)
        {
            try
            {
                if (companycode != "" && formcode != "" && transactiondate != "" && tablename != "")
                {
                    string query = string.Format(@"select FN_NEW_VOUCHER_NO('{0}','{1}','{2}','{3}') FROM DUAL", companycode, formcode, transactiondate, tablename);
                    string voucherNo = this._dbContext.SqlQuery<string>(query).First();
                    return voucherNo;
                }
                else
                { return ""; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public FromLocationInfo GetFromLocationCode(string companycode, string formcode)
        {
            try
            {
                if (formcode != "" && formcode != null)
                {


                    string query = $@"SELECT fs.DEFAULT_FROM_LOCATION, ils.LOCATION_EDESC AS DEFAULT_FROM_LOCATION_EDESC
                                      FROM    FORM_SETUp fs
                                           JOIN
                                              IP_location_setup ils
                                           ON fs.DEFAULT_FROM_LOCATION = ils.LOCATION_CODE
                                     WHERE fs.FORM_CODE = '{formcode}'
                                           AND fs.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                           AND fs.DELETED_FLAG = 'N'
                                           AND ils.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                           AND ils.DELETED_FLAG = 'N'";
                    var fromLocation = this._dbContext.SqlQuery<FromLocationInfo>(query).FirstOrDefault();
                    return fromLocation;
                }

                return null;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MuCodeModel> GetMuCode()
        {
            try
            {
                string query = $@"SELECT MU_CODE,MU_EDESC,MU_NDESC,REMARKS,
                                    COMPANY_CODE,CREATED_BY,CREATED_DATE,
                                    DELETED_FLAG,SYN_ROWID,MODIFY_DATE,
                                    MODIFY_BY FROM IP_MU_CODE
                                    WHERE DELETED_FLAG='N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var list = this._dbContext.SqlQuery<MuCodeModel>(query).ToList();
                return list;
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
                                   ITS.GRADE AS ItemGrade,
                                  IC.CATEGORY_EDESC  AS Category,
                                   IT.CREATED_BY AS CreatedBy,
                                   TO_CHAR (IT.CREATED_DATE, 'dd-Mon-yyyy') AS CreatedDate
                              FROM ip_item_master_setup IT, IP_CATEGORY_CODE IC, IP_ITEM_SPEC_SETUP ITS
                             WHERE IT.deleted_flag = 'N'
                             AND IT.CATEGORY_CODE= IC.CATEGORY_CODE
                             AND IT.COMPANY_CODE = IC.COMPANY_CODE
                             AND ITS.item_code = IT.item_code
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
        public List<Products> GetProductDataByProductCodeGrade(string productcode)
        {
            try
            {
                var productdata = new List<Products>();
                if (productcode != "")
                {


                    ////Previous Code with MU Code Unit Issue
                    //string query = $@"SELECT distinct  IT.ITEM_CODE AS ItemCode,
                    //               IT.ITEM_EDESC AS ItemDescription,
                    //               IT.INDEX_MU_CODE AS ItemUnit,
                    //              IT.MULTI_MU_CODE AS MultiItemUnit,
                    //               ITS.GRADE AS ItemGrade,
                    //              IC.CATEGORY_EDESC  AS Category,
                    //               IT.CREATED_BY AS CreatedBy,
                    //               TO_CHAR (IT.CREATED_DATE, 'dd-Mon-yyyy') AS CreatedDate
                    //          FROM ip_item_master_setup IT, IP_CATEGORY_CODE IC, IP_ITEM_SPEC_SETUP ITS
                    //         WHERE IT.deleted_flag = 'N'
                    //         AND IT.CATEGORY_CODE= IC.CATEGORY_CODE
                    //         AND IT.COMPANY_CODE = IC.COMPANY_CODE
                    //         AND ITS.item_code = IT.item_code
                    //          AND IT.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                    //           AND IT.item_code ='{productcode}'";


                    string query = $@"SELECT DISTINCT
                           IT.ITEM_CODE AS ItemCode,
                           IT.ITEM_EDESC AS ItemDescription,
                           IT.INDEX_MU_CODE AS ItemUnit,
                           IT.MULTI_MU_CODE AS MultiItemUnit,
                           ITS.GRADE AS ItemGrade,
                           IC.CATEGORY_EDESC AS Category,
                           IT.CREATED_BY AS CreatedBy,
                           TO_CHAR(IT.CREATED_DATE, 'dd-Mon-yyyy') AS CreatedDate
                    FROM IP_ITEM_MASTER_SETUP IT
                           JOIN IP_CATEGORY_CODE IC
                             ON IT.CATEGORY_CODE = IC.CATEGORY_CODE
                            AND IT.COMPANY_CODE = IC.COMPANY_CODE
                           LEFT JOIN IP_ITEM_SPEC_SETUP ITS
                             ON ITS.ITEM_CODE = IT.ITEM_CODE
                    WHERE IT.DELETED_FLAG = 'N'
                      AND IT.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                      AND IT.ITEM_CODE = '{productcode}'";

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
        public List<CustomerDetail> GetCustomerDataByCustomerCode(string customerCode)
        {
            try
            {
                var customerData = new List<CustomerDetail>();
                if (customerCode != "")
                {
                    string query =
                    $@"SELECT SCS.TEL_MOBILE_NO1 AS SHIPPING_CONTACT_NO, 
                                SCS.DEALING_PERSON AS EMPLOYEE_CODE, 
                                hes.EMPLOYEE_EDESC AS EMPLOYEE_NAME,
                                CC.CITY_EDESC AS SHIPPING_ADDRESS
                                FROM SA_CUSTOMER_SETUP SCS
                                LEFT JOIN hr_employee_setup hes
                                ON hes.EMPLOYEE_CODE = SCS.DEALING_PERSON
                                AND hes.DELETED_FLAG = 'N'
                                LEFT JOIN CITY_CODE  CC
                                ON CC.CITY_CODE = SCS.CITY_CODE
                                AND CC.DELETED_FLAG = 'N'
                                WHERE SCS.CUSTOMER_CODE = '{customerCode}'
                                AND SCS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                AND SCS.DELETED_FLAG = 'N'";
                    customerData = this._dbContext.SqlQuery<CustomerDetail>(query).ToList();
                    return customerData;
                }
                else
                { return customerData; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        //public List<COMMON_COLUMN> GetSalesOrderFormDetail1(string formCode, string orderno)
        //{
        //    string columname = $@"SELECT COLUMN_NAME, TABLE_NAME FROM FORM_DETAIL_SETUP WHERE FORM_CODE='{formCode}' and company_code='{_workContext.CurrentUserinformation.company_code}' and display_flag='Y' ORDER BY SERIAL_NO ASC";
        //    List<FORM_DETAIL_SETUP_COLUMN> columnameentity = this._dbContext.SqlQuery<FORM_DETAIL_SETUP_COLUMN>(columname).ToList();
        //    var tableName = "";
        //    tableName = columnameentity[0].TABLE_NAME;
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var item in columnameentity)
        //    {
        //        sb.Append(item.COLUMN_NAME).Append(",");
        //    }
        //    var primarycolname = GetPrimaryColumnByTableName(tableName);
        //    var columns = sb.ToString().TrimEnd(',');
        //    string Query = $@"SELECT {columns} FROM {tableName} WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and {primarycolname}='{orderno}'";
        //    var entity = this._dbContext.SqlQuery<COMMON_COLUMN>(Query).ToList();
        //    List<DocumentTransaction> imagelist = new List<DocumentTransaction>();
        //    if (entity.Count > 0)
        //    {
        //        string imagequery = $@"SELECT * FROM DOCUMENT_TRANSACTION WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and VOUCHER_NO='{orderno}'";
        //        imagelist = this._dbContext.SqlQuery<DocumentTransaction>(imagequery).ToList();
        //        entity[0].IMAGES_LIST = imagelist;
        //    }
        //    return entity;
        //}

        //AA
        public List<COMMON_COLUMN> GetSalesOrderFormDetail(string formCode, string orderno)
        {
            string columname = $@"SELECT COLUMN_NAME, TABLE_NAME FROM FORM_DETAIL_SETUP WHERE FORM_CODE='{formCode}' and company_code='{_workContext.CurrentUserinformation.company_code}' and display_flag='Y' ORDER BY SERIAL_NO ASC";
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
                if (item.COLUMN_NAME != "HS_CODE")
                {
                    sb.Append("SO.").Append(item.COLUMN_NAME).Append(",");
                }
            }
            //Prem Prakash Dhakal 08/07/2024
            //sb.Append("SO.ROOMNUMBER,SO.ARRIVAL_DATE,SO.DEPARTURE_DATE, SO.REFERENCE").Append(",");
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
                columns = columns + ",IMS.ITEM_EDESC, IMS.HS_CODE, IMS.NON_VAT_FLAG";
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

            condition.Append(" ORDER BY SO.SERIAL_NO ASC");
            Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
            //if (Query.Contains("SO.FROM_LOCATION_CODE"))
            //{
            //    columns = columns + ",ILS.FROM_LOCATION_EDESC";
            //    tableName = tableName + ", IP_LOCATION_SETUP ILS";form..js
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

                if (entity[0].PLAN_NO > 0)
                {
                    string planQuery = $@"select PLAN_NAME from MP_ORDER_PLAN_PROCESS where Plan_No={entity[0].PLAN_NO}";
                    string plan_name = this._dbContext.SqlQuery<string>(planQuery).FirstOrDefault();
                    entity[0].PLAN_NAME = plan_name;
                }
            }
            return entity;
        }

        public decimal GetTotalPriceByVoucherNoFormCodeAndTableName(string voucherno, string formcode, string tableName)
        {
            try
            {
                if (voucherno != "")
                {
                    var primarycolname = GetPrimaryColumnByTableName(tableName);
                    string productdata = "";

                    // Query to check if PRICE column exists
                    string priceColumn = $@"
    SELECT CASE 
             WHEN COUNT(*) > 0 THEN 1 
             ELSE 0 
           END AS STATUS
    FROM user_tab_columns
    WHERE table_name = '{tableName}'
      AND column_name = 'PRICE'";

                    int productdataExist = this._dbContext.SqlQuery<int>(priceColumn).FirstOrDefault();

                    if (productdataExist == 1)
                    {
                        string query = $@"
        SELECT NVL(SUM(PRICE), 0) AS PRICE
        FROM {tableName}
        WHERE DELETED_FLAG = 'N'
          AND FORM_CODE = '{formcode}'
          AND {primarycolname} = '{voucherno}'
          AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

                        productdata = this._dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString();
                    }
                    else
                    {

                        string query = $@"
        SELECT NVL(SUM(TOTAL_PRICE), 0) AS PRICE
        FROM {tableName}
        WHERE DELETED_FLAG = 'N'
          AND FORM_CODE = '{formcode}'
          AND {primarycolname} = '{voucherno}'
          AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

                        productdata = this._dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString();
                    }

                    return Convert.ToDecimal(productdata);

                }

                else
                { return 0; }


            }
            catch (Exception)
            {
                throw;
            }
        }
        public IncomingMaterial GetIncomingMaterialsDetailReport(string transactionno)
        {
            string query = $@"SELECT DISTINCT QPT.item_code,
    QPT.transaction_no AS QC_NO,
    QPT.CREATED_DATE AS QC_DATE,
    QPT.reference_no AS Voucher_No,
    iss.supplier_edesc,
    iims.item_edesc,
    ipm.mrr_date AS RECEIPT_DATE,
    ipm.mrr_no AS GRN_NO,
    ipm.quantity,
    ipm.to_location_code,
    iiss.Thickness AS thickness,
    qc_data.Thickness1,
    qc_data.Thickness2,
    qc_data.Thickness3,
    qc_data.RollDiameter1,
    qc_data.RollDiameter2,
    qc_data.RollDiameter3,
    qc_data.PH1,
    qc_data.PH2,
    qc_data.PH3,
    qc_data.UnpleasantSmell1,
    qc_data.UnpleasantSmell2,
    qc_data.UnpleasantSmell3,
    qc_data.DustDirt1,
    qc_data.DustDirt2,
    qc_data.DustDirt3,
    qc_data.DamagingMaterial1,
    qc_data.DamagingMaterial2,
    qc_data.DamagingMaterial3,
    qc_data.CoreDamaging1,
    qc_data.CoreDamaging2,
    qc_data.CoreDamaging3,
    qc_data.Width1,
    qc_data.Width2,
    qc_data.Width3,
    qc_data.GSM1,
    qc_data.GSM2,
    qc_data.GSM3,
    qc_data.TensileCD1,
    qc_data.TensileCD2,
    qc_data.TensileCD3,
    qc_data.TensileMD1,
    qc_data.TensileMD2,
    qc_data.TensileMD3,
    qc_data.VisualInspection1,
    qc_data.VisualInspection2,
    qc_data.VisualInspection3,
    qc_data.Remarks1,
    qc_data.Remarks2,
    qc_data.Remarks3,
    qc_data.Remarks4,
    qc_data.Remarks5,
    qc_data.Remarks6,
    qc_data.Remarks7,
    qc_data.Remarks8,
    qc_data.Remarks9,
    qc_data.Remarks10,
    qc_data.Remarks11,
    qc_data.Remarks12,
    iiss.ROLLDIAMETER,
    iiss.PH,
    iiss.UNPLEASANT_SMELL_ODOUR,
    iiss.DUST_DIRT,
    iiss.DAMAGING_MATERIAL,
    iiss.CORE_DAMAGING,
    iiss.SIZE_WIDTH,
    iiss.GSM,
    iiss.TENSILE_CD,
    iiss.TENSILE_MD,
    iiss.VISUAL_INSPECTION,
    ipm.item_code,
    ipm.Created_By AS CreatedBy,
    ipm.SUPPLIER_INV_NO AS invoice_no,
    ipm.MANUAL_NO,
    ipm.Remarks
FROM ip_purchase_mrr ipm
LEFT JOIN ip_supplier_setup iss ON iss.supplier_code = ipm.supplier_code
LEFT JOIN ip_item_master_setup iims ON iims.item_code = ipm.item_code
LEFT JOIN IP_ITEM_SPEC_SETUP iiss ON iiss.item_code = ipm.item_code
LEFT JOIN QC_PARAMETER_TRANSACTION QPT ON QPT.reference_no = ipm.mrr_no
LEFT JOIN (
    SELECT 
        serial_no,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN ROLL1 END) AS Thickness1,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL2 END) AS Thickness2,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL3 END) AS Thickness3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL1 END) AS RollDiameter1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL2 END) AS RollDiameter2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL3 END) AS RollDiameter3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL1 END) AS PH1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL2 END) AS PH2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL3 END) AS PH3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL1 END) AS UnpleasantSmell1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL2 END) AS UnpleasantSmell2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL3 END) AS UnpleasantSmell3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL1 END) AS DustDirt1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL2 END) AS DustDirt2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL3 END) AS DustDirt3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL1 END) AS DamagingMaterial1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL2 END) AS DamagingMaterial2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL3 END) AS DamagingMaterial3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL1 END) AS CoreDamaging1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL2 END) AS CoreDamaging2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL3 END) AS CoreDamaging3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL1 END) AS Width1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL2 END) AS Width2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL3 END) AS Width3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL1 END) AS GSM1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL2 END) AS GSM2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL3 END) AS GSM3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL1 END) AS TensileCD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL2 END) AS TensileCD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL3 END) AS TensileCD3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL1 END) AS TensileMD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL2 END) AS TensileMD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL3 END) AS TensileMD3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL1 END) AS VisualInspection1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL2 END) AS VisualInspection2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL3 END) AS VisualInspection3,      
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN Remarks END) AS Remarks1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('RollDiameter') THEN Remarks END) AS Remarks2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('PH') THEN Remarks END) AS Remarks3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN Remarks END) AS Remarks4,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN Remarks END) AS Remarks5,     
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN Remarks END) AS Remarks6,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN Remarks END) AS Remarks7,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN Remarks END) AS Remarks8,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN Remarks END) AS Remarks9,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN Remarks END) AS Remarks10,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN Remarks END) AS Remarks11,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN Remarks END) AS Remarks12
        
    FROM ip_item_qc_setup
    GROUP BY serial_no
) qc_data ON qc_data.serial_no = QPT.serial_no

WHERE QPT.transaction_no ='{transactionno}'";

            // Execute the query and get the result
            List<IncomingMaterial> items = _dbContext.SqlQuery<IncomingMaterial>(query).ToList();
            IncomingMaterial qc = null;
            // Ensure there are items returned before proceeding
            if (items.Any())
            {
                qc = new IncomingMaterial
                {
                    TRANSACTION_NO = transactionno,
                    QC_NO = items.FirstOrDefault()?.QC_NO,
                    QC_DATE = items.FirstOrDefault()?.QC_DATE,
                    item_code = items.FirstOrDefault()?.item_code,
                    Voucher_No = items.FirstOrDefault()?.Voucher_No,
                    supplier_edesc = items.FirstOrDefault()?.supplier_edesc,
                    item_edesc = items.FirstOrDefault()?.item_edesc,
                    RECEIPT_DATE = items.FirstOrDefault()?.RECEIPT_DATE,
                    to_location_code = items.FirstOrDefault()?.to_location_code,
                    invoice_no = items.FirstOrDefault()?.invoice_no,
                    GRN_NO = items.FirstOrDefault()?.GRN_NO,
                    GRN_DATE = items.FirstOrDefault()?.GRN_DATE,
                    quantity = Convert.ToDecimal(items.FirstOrDefault()?.quantity),
                    REMARKS = items.FirstOrDefault()?.REMARKS,
                    thickness = items.FirstOrDefault()?.thickness,
                    RollDiameter = items.FirstOrDefault()?.RollDiameter,
                    PH = items.FirstOrDefault()?.PH,
                    UNPLEASANT_SMELL_ODOUR = items.FirstOrDefault()?.UNPLEASANT_SMELL_ODOUR,
                    Dust_Dirt = items.FirstOrDefault()?.Dust_Dirt,
                    Damaging_Material = items.FirstOrDefault()?.Damaging_Material,
                    Core_Damaging = items.FirstOrDefault()?.Core_Damaging,
                    SIZE_WIDTH = items.FirstOrDefault()?.SIZE_WIDTH,
                    GSM = items.FirstOrDefault()?.GSM,
                    Tensile_CD = items.FirstOrDefault()?.Tensile_CD,
                    Tensile_MD = items.FirstOrDefault()?.Tensile_MD,
                    Visual_Inspection = items.FirstOrDefault()?.Visual_Inspection,
                    Thickness1 = items.FirstOrDefault()?.Thickness1,
                    Thickness2 = items.FirstOrDefault()?.Thickness2,
                    Thickness3 = items.FirstOrDefault()?.Thickness3,
                    RollDiameter1 = items.FirstOrDefault()?.RollDiameter1,
                    RollDiameter2 = items.FirstOrDefault()?.RollDiameter2,
                    RollDiameter3 = items.FirstOrDefault()?.RollDiameter3,
                    PH1 = items.FirstOrDefault()?.PH1,
                    PH2 = items.FirstOrDefault()?.PH2,
                    PH3 = items.FirstOrDefault()?.PH3,
                    UnpleasantSmell1 = items.FirstOrDefault()?.UnpleasantSmell1,
                    UnpleasantSmell2 = items.FirstOrDefault()?.UnpleasantSmell2,
                    UnpleasantSmell3 = items.FirstOrDefault()?.UnpleasantSmell3,
                    DustDirt1 = items.FirstOrDefault()?.DustDirt1,
                    DustDirt2 = items.FirstOrDefault()?.DustDirt2,
                    DustDirt3 = items.FirstOrDefault()?.DustDirt3,
                    DamagingMaterial1 = items.FirstOrDefault()?.DamagingMaterial1,
                    DamagingMaterial2 = items.FirstOrDefault()?.DamagingMaterial2,
                    DamagingMaterial3 = items.FirstOrDefault()?.DamagingMaterial3,
                    CoreDamaging1 = items.FirstOrDefault()?.CoreDamaging1,
                    CoreDamaging2 = items.FirstOrDefault()?.CoreDamaging2,
                    CoreDamaging3 = items.FirstOrDefault()?.CoreDamaging3,
                    VisualInspection1 = items.FirstOrDefault()?.VisualInspection1,
                    VisualInspection2 = items.FirstOrDefault()?.VisualInspection2,
                    VisualInspection3 = items.FirstOrDefault()?.VisualInspection3,
                    Width1 = items.FirstOrDefault()?.Width1,
                    Width2 = items.FirstOrDefault()?.Width2,
                    Width3 = items.FirstOrDefault()?.Width3,
                    GSM1 = items.FirstOrDefault()?.GSM1,
                    GSM2 = items.FirstOrDefault()?.GSM2,
                    GSM3 = items.FirstOrDefault()?.GSM3,
                    TensileCD1 = items.FirstOrDefault()?.TensileCD1,
                    TensileCD2 = items.FirstOrDefault()?.TensileCD2,
                    TensileCD3 = items.FirstOrDefault()?.TensileCD3,
                    TensileMD1 = items.FirstOrDefault()?.TensileMD1,
                    TensileMD2 = items.FirstOrDefault()?.TensileMD2,
                    TensileMD3 = items.FirstOrDefault()?.TensileMD3,
                    Remarks1 = items.FirstOrDefault()?.Remarks1,
                    Remarks2 = items.FirstOrDefault()?.Remarks2,
                    Remarks3 = items.FirstOrDefault()?.Remarks3,
                    Remarks4 = items.FirstOrDefault()?.Remarks4,
                    Remarks5 = items.FirstOrDefault()?.Remarks5,
                    Remarks6 = items.FirstOrDefault()?.Remarks6,
                    Remarks7 = items.FirstOrDefault()?.Remarks7,
                    Remarks8 = items.FirstOrDefault()?.Remarks8,
                    Remarks9 = items.FirstOrDefault()?.Remarks9,
                    Remarks10 = items.FirstOrDefault()?.Remarks10,
                    Remarks11 = items.FirstOrDefault()?.Remarks11,
                    Remarks12 = items.FirstOrDefault()?.Remarks12
                };
            }


            // If no valid voucherno, return the view with a default state (qc is null)
            return qc;
        }

        public GlobalAgroProductsReport GetGlobalAgroProductsReport(string transactionno)
        {
            GlobalAgroProductsReport raw = new GlobalAgroProductsReport();
            String query_globalProducts = $@"select GPT.TRANSACTION_NO,GPT.ITEM_CODE,(SELECT DISTINCT INITCAP(item_edesc) as ITEM_EDESC FROM ip_item_master_setup WHERE 
DELETED_FLAG='N' AND ITEM_CODE = GPT.ITEM_CODE AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}') AS ITEM_EDESC,GPT.SERIAL_NO,GPT.CREATED_DATE
,GPT.VEHICLE_NO,GPT.PARTY_NAME,GPT.ADDRESS,GPT.BILL_NO,GPT.GATE_OR_GRN_NO,GPT.PHYSICAL_TEST_RAWMATERIAL,
GPT.WEIGHT,GPT.MOISTURE,GPT.TEMPERATURE,GPT.WET,GPT.FUNGUS,GPT.DUST,GPT.GRADING,GPT.SMELL,GPT.COLOR,GPT.PIECES,GPT.IMMATURITY_OF_GRAINS
,GPT.OTHER_ITEMS,GPT.ROTTEN_HOLED,GPT.DAMAGED,GPT.BROKEN,GPT.HUSK,GPT.OVERTOASTED,GPT.USEABLE,GPT.UNUSEABLE,GPT.FAT,GPT.QUALITY_OF_GOODS
,GPT.EXCELLENT,GPT.GREAT,GPT.GOODS_NORMAL,GPT.WAREHOUSE,GPT.SILO,GPT.GHAN,GPT.PROTEIN,GPT.QUALITY_OF_FIREWOOD,GPT.PRODUCT_SIZE,
GPT.PRODUCT_TYPE,GPT.DEDUCT_IN_BAG,GPT.NET_WEIGHT,GPT.ISPLASTIC_BAG,GPT.ISJUTE_BAG,GPT.ISPLASTIC_WEIGHT,GPT.ISJUTE_WEIGHT
,GPT.REMARKS,GPT.UNLOAD_UNIT from GLOBAL_PRODUCTS_TRANSACTION GPT
                        where GPT.TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<GlobalAgroProductsReport>(query_globalProducts).FirstOrDefault();

            String query_globalWeight = $@"select FIRST_WEIGHT,SECOND_WEIGHT,NET_WEIGHT,CHALLAN_WEIGHT,WEIGHT_DIFFERENCE ,REMARKS from GLOBAL_Weight_Details where GLOBAL_WEIGHT_NO='{transactionno}'";
            raw.WEIGHTDETAILSList = this._dbContext.SqlQuery<WEIGHTDETAILS>(query_globalWeight).ToList();


            String query_globalUnload = $@"select FIRST_CHHALLI,SECOND_CHHALLI,THIRD_CHHALLI,FOURTH_CHHALLI,FIFTH_CHHALLI
                    ,SIXTH_CHHALLI,SEVENTH_CHHALLI,EIGHTH_CHHALLI,NINETH_CHHALLI,TENTH_CHHALLI,ELEVEN_CHHALLI,TWELVE_CHHALLI,TOTAL
                    from GLOBAL_UNLOADED_CHHALLI where GLOBAL_UNLOADED_NO='{transactionno}'";

            raw.UNLOADEDCHHALLIList = this._dbContext.SqlQuery<UNLOADEDCHHALLI>(query_globalUnload).ToList();

            String query_dakhilaDetails = $@"select ENTRY_NO,BILL_NO,CHALAN_NO,ITEM,TOTAL_BAG,WEIGHT,REMARKS   from GLOBAL_DAKHILA_DETAILS where GLOBAL_DAKHILA_NO='{transactionno}'";
            raw.DAKHILADETAILSList = this._dbContext.SqlQuery<DAKHILADETAILS>(query_dakhilaDetails).ToList();
            return raw;

            // If no valid voucherno, return the view with a default state (qc is null)
        }
        public List<DraftFormModel> getDraftDataByFormCodeAndTempCode(string formCode, string tempCode)
        {
            List<DraftFormModel> Record = new List<DraftFormModel>();
            string query = $@"select TEMPLATE_NO,TO_CHAR(FORM_CODE) AS FORM_CODE,TABLE_NAME,SERIAL_NO,COLUMN_NAME,COLUMN_VALUE from FORM_TEMPLATE_DETAIL_SETUP where  FORM_CODE='{formCode}' AND  template_no='{tempCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' ORDER BY SERIAL_NO";
            Record = this._dbContext.SqlQuery<DraftFormModel>(query).ToList();
            return Record;
        }
        public decimal GetGrandTotalByVoucherNo(string voucherno, string formcode)
        {
            try
            {
                if (voucherno != "")
                {
                    string query = $@"SELECT VOUCHER_AMOUNT FROM MASTER_TRANSACTION where VOUCHER_NO='{voucherno}' and Form_CODE='{formcode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                    string productdata = this._dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString();
                    return Convert.ToDecimal(productdata);

                }
                else
                { return 0; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public decimal GetGrandTotalAmountByOrderNo(string orderno, string formcode, string tablename)
        {
            try
            {
                if (orderno != "")
                {

                    var primarycolname = GetPrimaryColumnByTableName(tablename);
                    string query = $@"SELECT SUM (
                                            COALESCE (AMOUNT, 0)
                                          + COALESCE (MAKING, 0)
                                          + COALESCE (DIAMOND_AMOUNT, 0)
                                          + COALESCE (STONE_AMOUNT, 0))
                                          AS GRAND_TOTAL
                                  FROM {tablename}
                                 WHERE     DELETED_FLAG = 'N'
                                       AND FORM_CODE = '{formcode}'
                                       AND {primarycolname} = '{orderno}'
                                and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                    string productdata = this._dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString();
                    return Convert.ToDecimal(productdata);

                }
                else
                { return 0; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetSalesInvoiceJewelleryDateTime(string salesno, string tableName)
        {
            try
            {
                if (salesno != "undefined" && salesno != "null" && tableName != "undefined" && tableName != "null")
                {
                    var primarycolname = GetPrimaryColumnByTableName(tableName);
                    string dateTimeQuery = $@"SELECT DISTINCT TO_CHAR(CREATED_DATE, 'YYYY/MM/DD    HH12:MI:SS AM') AS CREATED_DATE FROM {tableName} WHERE {primarycolname} = '{salesno}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  AND DELETED_FLAG='N'";
                    var dateTimeData = this._dbContext.SqlQuery<string>(dateTimeQuery).FirstOrDefault().ToString();
                    return string.IsNullOrEmpty(dateTimeData) ? "" : dateTimeData;

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
        public decimal GetRefGrandTotalByVoucherNo(string voucherno)
        {
            try
            {
                if (voucherno != "")
                {
                    string query = $@"SELECT VOUCHER_AMOUNT FROM MASTER_TRANSACTION where VOUCHER_NO='{voucherno}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                    string productdata = this._dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString();
                    return Convert.ToDecimal(productdata);

                }
                else
                { return 0; }

            }
            catch (Exception)
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
                        where DELETED_FLAG='N' AND (FREEZE_FLAG = 'N' OR FREEZE_FLAG IS NULL) and COMPANY_CODE = '{1}' and ACC_TYPE_FLAG='T'
                        and (ACC_CODE ='{0}' or upper(ACC_EDESC) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code);
            result = this._dbContext.SqlQuery<AccountSetup>(query).ToList();
            return result;

        }
        public List<AccountSetup> getALLAccountGroupForIntrestCalc()
        {

            List<AccountSetup> result = new List<AccountSetup>();
            string query = string.Format($@"SELECT ACC_CODE,ACC_EDESC FROM FA_CHART_OF_ACCOUNTS_SETUP 
                                           WHERE ACC_NATURE='AE' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND ACC_TYPE_FLAG='T'");
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
        public List<TDSCODE> getALLTDSByFlter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<TDSCODE> result = new List<TDSCODE>();
            string query = string.Format(@"Select
                        COALESCE(TDS_CODE,' ') as TDS_CODE, 
                        COALESCE(TDS_EDESC,' ') as TDS_EDESC 
                        FROM FA_TDS_CODE
                        where DELETED_FLAG='N' and COMPANY_CODE='{1}' 
                        and (TDS_CODE ='{0}' or upper(TDS_EDESC) like '%{0}%')", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
            result = this._dbContext.SqlQuery<TDSCODE>(query).ToList();
            return result;


        }
        public List<Branch> GetAllBranchCodeByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<Branch> result = new List<Branch>();
            string query = $@"SELECT
                        COALESCE(BRANCH_CODE,' ') as BRANCH_CODE, 
                        COALESCE(BRANCH_EDESC,' ') as BRANCH_EDESC,
                        TELEPHONE_NO,
                        ADDRESS
                        FROM FA_BRANCH_SETUP
                        WHERE GROUP_SKU_FLAG='I' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' 
                        or (upper(TELEPHONE_NO) like '%{filter.ToUpperInvariant()}%'
                        or upper(ADDRESS) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<Branch>(query).ToList();
            return result;
        }
        public List<Employee> GetAllEmployeeCodeByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<Employee> result = new List<Employee>();
            string query = $@"SELECT
                        COALESCE(EMPLOYEE_CODE,' ') as EMPLOYEE_CODE, 
                        COALESCE(EMPLOYEE_EDESC,' ') as EMPLOYEE_EDESC 
                        FROM hr_employee_setup
                        WHERE GROUP_SKU_FLAG='I' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'  
                        and(upper(EMPLOYEE_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(EMPLOYEE_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<Employee>(query).ToList();
            return result;
        }
        public List<CustomerModels> GetAllMasterCustomerCodeByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<CustomerModels> result = new List<CustomerModels>();
            string query = $@"SELECT
                        COALESCE(CUSTOMER_CODE,' ') as CUSTOMER_CODE, 
                        COALESCE(CUSTOMER_EDESC,' ') as CUSTOMER_EDESC 
                        FROM SA_CUSTOMER_SETUP
                        WHERE GROUP_SKU_FLAG='G' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'  
                        and(upper(CUSTOMER_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(CUSTOMER_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<CustomerModels>(query).ToList();
            return result;
        }
        public List<SalesType> GetAllSaleTypeListByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<SalesType> result = new List<SalesType>();
            string query = $@"SELECT
                        COALESCE(SALES_TYPE_CODE,' ') as SALES_TYPE_CODE, 
                        COALESCE(SALES_TYPE_EDESC,' ') as SALES_TYPE_EDESC 
                        FROM sa_sales_type
                        WHERE GROUP_SKU_FLAG='I' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'  
                        and(upper(SALES_TYPE_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(SALES_TYPE_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<SalesType>(query).ToList();
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
        public List<SubLedger> GetAllSubLedgerByFilterPartyType(string filter, string partyTypeCode)
        {
            var condition = string.Empty;
            if (!string.IsNullOrEmpty(partyTypeCode))
            {
                condition = $@" AND SUB_CODE IN (SELECT
                        COALESCE(SUB_CODE,' ') as SUB_CODE
                        FROM FA_SUB_LEDGER_DEALER_MAP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' AND  PARTY_TYPE_CODE = '{partyTypeCode}')";
            }
            else
            {
                condition = $@" AND SUB_CODE IN (SELECT
                        COALESCE(SUB_CODE,' ') as SUB_CODE
                        FROM FA_SUB_LEDGER_DEALER_MAP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";
            }
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<SubLedger> result = new List<SubLedger>();
            string query = $@"SELECT
                        COALESCE(SUB_CODE,' ') as SUB_CODE, 
                        COALESCE(SUB_EDESC,' ') as SUB_EDESC 
                        FROM FA_SUB_LEDGER_SETUP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'  {condition}
                        and(upper(SUB_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(SUB_CODE) like '%{filter.ToUpperInvariant()}%'))";
            result = this._dbContext.SqlQuery<SubLedger>(query).ToList();
            return result;
        }
        public List<BudgetCenter> GetAllBudgetCenterByFilter(string filter, string accCode)
        {
            var condition = $@" AND BUDGET_CODE IN (SELECT
                        COALESCE(BUDGET_CODE,' ') as BUDGET_CODE
                        FROM BC_BUDGET_CENTER_MAP
                        where DELETED_FLAG='N' AND ACC_CODE = '{accCode}')";
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<BudgetCenter> result = new List<BudgetCenter>();
            string query = $@"SELECT
                        COALESCE(BUDGET_CODE,' ') as BUDGET_CODE, 
                        COALESCE(BUDGET_EDESC,' ') as BUDGET_EDESC 
                        FROM BC_BUDGET_CENTER_SETUP
                        where DELETED_FLAG='N' AND GROUP_SKU_FLAG = 'I' {condition}  AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                        and(upper(BUDGET_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(BUDGET_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<BudgetCenter>(query).ToList();
            return result;
        }
        public List<BudgetCenter> GetAllMasterBudgetCenterByFilter(string filter, string accCode)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<BudgetCenter> result = new List<BudgetCenter>();
            string query = $@"SELECT DISTINCT
                        COALESCE(bcsc.BUDGET_CODE,' ') as BUDGET_CODE, 
                        COALESCE(bcsc.BUDGET_EDESC,' ') as BUDGET_EDESC 
                        FROM BC_BUDGET_CENTER_SETUP bcs
                        JOIN BC_BUDGET_CENTER_MAP bcm
                        ON bcm.BUDGET_CODE = bcs.BUDGET_CODE
                        AND bcm.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                        AND bcm.DELETED_FLAG = 'N'
                        JOIN BC_BUDGET_CENTER_SETUP bcsc
                        ON bcs.PRE_BUDGET_CODE = bcsc.BUDGET_CODE
                        AND bcsc.DELETED_FLAG = 'N'
                        AND bcsc.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        WHERE bcs.GROUP_SKU_FLAG = 'I'
                        AND bcs.DELETED_FLAG = 'N'
                        AND bcs.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        AND bcm.ACC_CODE = '{accCode}'  
                        and(upper(bcsc.BUDGET_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(bcsc.BUDGET_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<BudgetCenter>(query).ToList();
            return result;
        }
        public List<AreaSetup> GetAllAreaSetupByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<AreaSetup> result = new List<AreaSetup>();
            string query = $@"SELECT
                        COALESCE(TO_CHAR(AREA_CODE),' ') as AREA_CODE, 
                        COALESCE(AREA_EDESC,' ') as AREA_EDESC 
                        FROM AREA_SETUP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                        and(upper(AREA_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(TO_CHAR(AREA_CODE)) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<AreaSetup>(query).ToList();
            return result;
        }
        public List<PartyType> GetAllPartyTypeByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<PartyType> result = new List<PartyType>();
            string query = $@"SELECT
                        COALESCE(PARTY_TYPE_CODE,' ') as PARTY_TYPE_CODE, 
                        COALESCE(PARTY_TYPE_EDESC,' ') as PARTY_TYPE_EDESC 
                        FROM IP_PARTY_TYPE_CODE
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                        and(upper(PARTY_TYPE_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(PARTY_TYPE_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<PartyType>(query).ToList();
            return result;
        }
        public List<PartyType> GetAllPartyTypeByFilterAndCustomerCode(string filter, string customercode)
        {
            var dealer_system_flag = string.Empty;
            string dealer_system_flag_query = $@"SELECT
                        DEALER_SYSTEM_FLAG 
                        FROM PREFERENCE_SUB_SETUP
                        where COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}'";
            dealer_system_flag = this._dbContext.SqlQuery<string>(dealer_system_flag_query).FirstOrDefault();
            if (dealer_system_flag == "Y")
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<PartyType> result = new List<PartyType>();
                string query = $@"SELECT FA.PARTY_TYPE_CODE,
       FA.SUB_CODE,
       SA.CUSTOMER_EDESC,
       IP.PARTY_TYPE_EDESC,
       IP.PARTY_TYPE_FLAG
  FROM FA_SUB_LEDGER_DEALER_MAP FA,
       SA_CUSTOMER_SETUP SA,
       IP_PARTY_TYPE_CODE IP
 WHERE     FA.COMPANY_CODE = SA.COMPANY_CODE
       AND TRIM (FA.SUB_CODE) = TRIM (sa.link_sub_code)
       AND IP.COMPANY_CODE = FA.COMPANY_CODE
       AND SA.COMPANY_CODE = IP.COMPANY_CODE
       AND FA.PARTY_TYPE_CODE = IP.PARTY_TYPE_CODE
       AND IP.DELETED_FLAG = 'N'
       AND FA.DELETED_FLAG = 'N'
       AND SA.DELETED_FLAG = 'N'
       AND SA.CUSTOMER_CODE = '{customercode}'
       AND  IP.PARTY_TYPE_FLAG='D'
       AND IP.DELETED_FLAG='N' AND IP.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
       and(upper(IP.PARTY_TYPE_EDESC) like '%{filter.ToUpperInvariant()}%' or upper( FA.PARTY_TYPE_CODE) like '%{filter.ToUpperInvariant()}%')";
                result = this._dbContext.SqlQuery<PartyType>(query).ToList();
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<PartyType> result = new List<PartyType>();
                string query = $@"SELECT FA.PARTY_TYPE_CODE,
       FA.SUB_CODE,
       SA.CUSTOMER_EDESC,
       IP.PARTY_TYPE_EDESC,
       IP.PARTY_TYPE_FLAG
  FROM FA_SUB_LEDGER_DEALER_MAP FA,
       SA_CUSTOMER_SETUP SA,
       IP_PARTY_TYPE_CODE IP
 WHERE     FA.COMPANY_CODE = SA.COMPANY_CODE
       AND TRIM (FA.SUB_CODE) = TRIM (sa.link_sub_code)
       AND IP.COMPANY_CODE = FA.COMPANY_CODE
       AND SA.COMPANY_CODE = IP.COMPANY_CODE
       AND FA.PARTY_TYPE_CODE = IP.PARTY_TYPE_CODE
       AND IP.DELETED_FLAG = 'N'
       AND FA.DELETED_FLAG = 'N'
       AND SA.DELETED_FLAG = 'N'
       AND SA.CUSTOMER_CODE = '{customercode}'
       AND IP.DELETED_FLAG='N' AND IP.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
       AND ( IP.PARTY_TYPE_FLAG='P'
       or  IP.PARTY_TYPE_FLAG IS NULL)
       and(upper(IP.PARTY_TYPE_EDESC) like '%{filter.ToUpperInvariant()}%' or upper( FA.PARTY_TYPE_CODE) like '%{filter.ToUpperInvariant()}%')";
                result = this._dbContext.SqlQuery<PartyType>(query).ToList();
                return result;
            }
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

        //if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
        //List<PartyType> result = new List<PartyType>();
        //string query = $@"SELECT
        //            COALESCE(PARTY_TYPE_CODE,' ') as PARTY_TYPE_CODE, 
        //            COALESCE(PARTY_TYPE_EDESC,' ') as PARTY_TYPE_EDESC 
        //            FROM IP_PARTY_TYPE_CODE
        //            where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
        //            and(upper(PARTY_TYPE_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(PARTY_TYPE_CODE) like '%{filter.ToUpperInvariant()}%')";
        //result = this._dbContext.SqlQuery<PartyType>(query).ToList();
        //return result;
        //}
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
        public List<SubLedger> GetAllSubCodeByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<SubLedger> result = new List<SubLedger>();
            string query = $@"SELECT
                        COALESCE(SUB_CODE,' ') as SUB_CODE, 
                        COALESCE(SUB_EDESC,' ') as SUB_EDESC 
                        FROM FA_SUB_LEDGER_SETUP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                        and(upper(SUB_EDESC) like '%{filter.ToUpperInvariant()}%' or upper(SUB_CODE) like '%{filter.ToUpperInvariant()}%')";
            result = this._dbContext.SqlQuery<SubLedger>(query).ToList();
            return result;
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
        public List<BudgetCenter> checkBudgetFlagAccessByLocationCode(string locationCode)
        {
            var result = new List<BudgetCenter>();
            string query = $@"SELECT
                        COALESCE(BUDGET_CODE,' ') as BUDGET_CODE
                        FROM BC_BUDGET_CENTER_MAP
                        where DELETED_FLAG='N'  AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}' AND ACC_CODE = '{locationCode}'";
            result = this._dbContext.SqlQuery<BudgetCenter>(query).ToList();
            return result;
        }
        public List<string> getSubLedgerByacc(string accCode)
        {
            var result = new List<string>();
            string query = $@"SELECT
                        COALESCE(SUB_CODE,' ') as SUB_CODE
                        FROM FA_SUB_LEDGER_MAP
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}' AND ACC_CODE = '{accCode}'";
            result = this._dbContext.SqlQuery<string>(query).ToList();
            return result;
        }
        public List<string> getBudgetCodeByacc(string accCode)
        {
            var result = new List<string>();
            string query = $@"SELECT
                        COALESCE(BUDGET_CODE,' ') as BUDGET_CODE
                        FROM BC_BUDGET_CENTER_MAP
                        where DELETED_FLAG='N'  AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}' AND ACC_CODE = '{accCode}'";
            result = this._dbContext.SqlQuery<string>(query).ToList();
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
        public string CheckIsVATByAccCode(string accCode)
        {
            var result = string.Empty;
            string query = $@"SELECT TO_CHAR(count(*))
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        where IND_VAT_FLAG='Y' AND DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}' AND ACC_CODE = '{accCode}'";
            result = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }
        public string CheckIsTDSByAccCode(string accCode)
        {
            var result = string.Empty;
            string query = $@"SELECT TO_CHAR(count(*))
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        where IND_TDS_FLAG='Y' AND DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}' AND ACC_CODE = '{accCode}'";
            result = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }
        public List<COMMON_COLUMN> VoucherDetailByReferenceForTemplate(VoucherRefrence model)
        {
            int FormCode = 0;
            int.TryParse(model.FormCode, out FormCode);
            string FormCodeFilter = string.Empty;
            var RunableQuery = string.Empty;
            var primarycolname = GetPrimaryColumnByTableName(model.TableName);
            if (model.TableName != "SA_LOADING_SLIP_DETAIL")
            {
                if (model.TableName == "IP_GATE_ENTRY" || model.TableName == "IP_QC_ENTRY")
                {
                    model.TableName = "IP_QC_ENTRY";
                }
                if (FormCode > 0)
                {
                    FormCodeFilter = $@" FORM_CODE='{model.FormCode}'";
                }
                else
                {
                    FormCodeFilter = $@" table_name='{model.TableName}'";
                }
                //string columname = $@"SELECT COLUMN_NAME, TABLE_NAME FROM FORM_DETAIL_SETUP WHERE FORM_CODE='{model.FormCode}' and company_code='{_workContext.CurrentUserinformation.company_code}' and display_flag='Y' ORDER BY SERIAL_NO ASC";
                string columname = $@"SELECT COLUMN_NAME, TABLE_NAME FROM FORM_DETAIL_SETUP WHERE {FormCodeFilter} and company_code='{_workContext.CurrentUserinformation.company_code}' and display_flag='Y' group by COLUMN_NAME, TABLE_NAME";
                List<FORM_DETAIL_SETUP_COLUMN> columnameentity = this._dbContext.SqlQuery<FORM_DETAIL_SETUP_COLUMN>(columname).ToList();
                var tableNameMain = "";
                StringBuilder sb = new StringBuilder();
                foreach (var item in columnameentity)
                {
                    sb.Append("SO.").Append(item.COLUMN_NAME).Append(",");
                }
                sb.Append("SO.FORM_CODE");

                //if (model.TableName == "SA_SALES_INVOICE_JEWEL" || model.TableName == "SA_SALES_RETURN_JEWEL") {
                //    sb.Append(",SO.TOTAL_PRICE");

                //}

                //var columns = sb.ToString().TrimEnd(',');


                //if (model.TableName == "SA_SALES_BLANKET")
                //{
                //    sb.Replace("SO.QUANTITY", "(SO.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS QUANTITY");
                //    sb.Replace("SO.CALC_QUANTITY", "(SO.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS CALC_QUANTITY");
                //    sb.Replace("SO.ITEM_CODE", "COALESCE(C2.ITEM_CODE, SO.ITEM_CODE) AS ITEM_CODE");

                //    sb.Append(",SO.SERIAL_NO");

                //}

                var columnsMain = sb.ToString();
                string Query = string.Empty;
                StringBuilder condition;
                tableNameMain = model.TableName + " SO";

                if (model.checkList.Count() > 0)
                {
                    var voucherCount = model.checkList.Count();
                    if (model.INCLUDE_CHARGE == "True")
                    {
                        var ChareInclusiveVoucherNo = model.checkList.First().VOUCHER_NO;
                        var columns = columnsMain;
                        condition = new StringBuilder();
                        var tableName = tableNameMain;

                        Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{ChareInclusiveVoucherNo}'";

                        if (Query.Contains("SO.CUSTOMER_CODE"))
                        {
                            columns = columns + ",CS.CUSTOMER_EDESC";
                            tableName = tableName + ",SA_CUSTOMER_SETUP CS";
                            condition.Append("AND CS.CUSTOMER_CODE=SO.CUSTOMER_CODE AND CS.COMPANY_CODE=SO.COMPANY_CODE AND  CS.DELETED_FLAG='N'");

                            Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{ChareInclusiveVoucherNo}' {condition.ToString()}";
                        }
                        if (Query.Contains("SO.ITEM_CODE"))
                        {
                            columns = columns + ",IMS.ITEM_EDESC";
                            tableName = tableName + ", IP_ITEM_MASTER_SETUP IMS";
                            condition.Append("AND IMS.ITEM_CODE=SO.ITEM_CODE AND IMS.COMPANY_CODE=SO.COMPANY_CODE AND  IMS.DELETED_FLAG='N'");

                            Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{ChareInclusiveVoucherNo}' {condition.ToString()}";
                        }
                        RunableQuery += Query;

                    }
                    else
                    {

                        foreach (var union in model.checkList)
                        {
                            var columns = columnsMain;
                            condition = new StringBuilder();
                            var tableName = tableNameMain;
                            var previousVoucherNo = union.VOUCHER_NO;
                            var previousSerialNo = union.SERIAL_NO;
                            var voucherNoContinuation = "('" + union.VOUCHER_NO + "')";
                            var serialNoContinuation = "('" + union.SERIAL_NO + "')";

                            if (model.TableName == "SA_SALES_BLANKET")
                            {

                                tableNameMain = model.TableName + " SO";
                                tableName = tableNameMain;
                                if (union.IS_PARENT_ROW == "N")
                                {

                                    columns = columns.Replace("SO.QUANTITY", "0 AS QUANTITY");
                                    columns = columns.Replace("SO.CALC_QUANTITY", "0 AS CALC_QUANTITY");
                                    columns = columns + ",MB.SERIAL_NO";


                                    Query = $@" SELECT DISTINCT {columns} FROM {tableName}
                                                JOIN MASTER_BLANKET_ITEM_CONVERSION MB
                                                ON SO.ITEM_CODE = MB.MAIN_ITEM_CODE
                                                AND MB.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                                AND MB.DELETED_FLAG = 'N'
                                                    WHERE MB.SERIAL_NO IN {serialNoContinuation} AND SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname} in {voucherNoContinuation}
                                        UNION ALL";


                                    if (Query.Contains("SO.CUSTOMER_CODE"))
                                    {
                                        columns = columns + ",CS.CUSTOMER_EDESC";
                                        //tableName = tableName + ",SA_CUSTOMER_SETUP CS";


                                        condition.Append("JOIN SA_CUSTOMER_SETUP CS ON SO.CUSTOMER_CODE = CS.CUSTOMER_CODE AND SO.COMPANY_CODE = CS.COMPANY_CODE AND CS.DELETED_FLAG = 'N' ");


                                        //condition.Append("AND CS.CUSTOMER_CODE=SO.CUSTOMER_CODE AND CS.COMPANY_CODE=SO.COMPANY_CODE AND  CS.DELETED_FLAG='N'");


                                        //Query = Query + $@"JOIN SA_CUSTOMER_SETUP CS
                                        //    ON SO.CUSTOMER_CODE = CS.CUSTOMER_9CODE
                                        //   AND SO.COMPANY_CODE = CS.COMPANY_CODE
                                        //   AND CS.DELETED_FLAG = 'N'";

                                        Query = $@" SELECT DISTINCT {columns} FROM {tableName}
                                                JOIN MASTER_BLANKET_ITEM_CONVERSION MB
                                                ON SO.ITEM_CODE = MB.MAIN_ITEM_CODE
                                                AND MB.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                                AND MB.DELETED_FLAG = 'N'
                                    {condition.ToString()}
                                        WHERE MB.SERIAL_NO IN {serialNoContinuation} AND SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname} in {voucherNoContinuation}
                                        UNION ALL";

                                        //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                                    }


                                    if (Query.Contains("SO.ITEM_CODE"))
                                    {
                                        columns = Regex.Replace(columns, @"\bSO\.ITEM_CODE\b", "IM.ITEM_CODE");
                                        columns = columns + ",IM.ITEM_EDESC, IM.NON_VAT_FLAG";
                                        //tableName = tableName + ", IP_ITEM_MASTER_SETUP IMS";
                                        //condition.Append("AND IMS.ITEM_CODE=SO.ITEM_CODE AND IMS.COMPANY_CODE=SO.COMPANY_CODE AND  IMS.DELETED_FLAG='N'");

                                        condition.Append(" JOIN IP_ITEM_MASTER_SETUP IM ON MB.ITEM_CODE = IM.ITEM_CODE AND SO.COMPANY_CODE = IM.COMPANY_CODE AND IM.DELETED_FLAG = 'N'");

                                        if (Query.Contains("IP_PURCHASE_INVOICE"))
                                        {
                                            columns = columns + ",SO.COMPLETED_QUANTITY";
                                        }
                                        if (Query.Contains("IP_GOODS_REQUISITION"))
                                        {
                                            columns = columns + ",SO.SUPPLIER_CODE";
                                        }
                                        if (previousVoucherNo == union.VOUCHER_NO && int.Parse(previousSerialNo) < int.Parse(union.SERIAL_NO))
                                        {

                                        }
                                        else
                                        {
                                            Query = $@" SELECT DISTINCT {columns} FROM {tableName}
                                                JOIN MASTER_BLANKET_ITEM_CONVERSION MB
                                                ON SO.ITEM_CODE = MB.MAIN_ITEM_CODE
                                                AND MB.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                                AND MB.DELETED_FLAG = 'N'
                                    {condition.ToString()}
                                        WHERE MB.SERIAL_NO IN {serialNoContinuation} AND SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname} in {voucherNoContinuation}
                                        UNION ALL";
                                        }
                                        //Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') {condition.ToString()} UNION ALL";

                                        //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                                    }

                                }
                                else
                                {
                                    columns = columns.Replace("SO.QUANTITY", "(SO.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS QUANTITY");
                                    columns = columns.Replace("SO.CALC_QUANTITY", "(SO.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS CALC_QUANTITY");
                                    columns = columns + ",SO.SERIAL_NO";
                                    Query = $@" SELECT DISTINCT {columns} 
                                    FROM {tableName}
                                       LEFT JOIN (
                                        SELECT 
                                            REFERENCE_NO,
                                            REFERENCE_ITEM_CODE,
                                            REFERENCE_SERIAL_NO,
                                            SUM(REFERENCE_QUANTITY) AS REFERENCE_QUANTITY
                                        FROM REFERENCE_DETAIL
                                        WHERE DELETED_FLAG = 'N'
                                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                        GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE, REFERENCE_SERIAL_NO
                                    ) RD ON SO.BLANKET_NO = RD.REFERENCE_NO
                                         AND SO.ITEM_CODE = RD.REFERENCE_ITEM_CODE
                                         AND SO.SERIAL_NO = RD.REFERENCE_SERIAL_NO
                                           
                                                    WHERE SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                              AND SO.DELETED_FLAG = 'N' AND SO.SERIAL_NO IN {serialNoContinuation} and SO.{primarycolname} in {voucherNoContinuation}
                                        UNION ALL";


                                    if (Query.Contains("SO.CUSTOMER_CODE"))
                                    {
                                        columns = columns + ",CS.CUSTOMER_EDESC";
                                        //tableName = tableName + ",SA_CUSTOMER_SETUP CS";


                                        condition.Append("JOIN SA_CUSTOMER_SETUP CS ON SO.CUSTOMER_CODE = CS.CUSTOMER_CODE AND SO.COMPANY_CODE = CS.COMPANY_CODE AND CS.DELETED_FLAG = 'N' ");


                                        //condition.Append("AND CS.CUSTOMER_CODE=SO.CUSTOMER_CODE AND CS.COMPANY_CODE=SO.COMPANY_CODE AND  CS.DELETED_FLAG='N'");


                                        //Query = Query + $@"JOIN SA_CUSTOMER_SETUP CS
                                        //    ON SO.CUSTOMER_CODE = CS.CUSTOMER_9CODE
                                        //   AND SO.COMPANY_CODE = CS.COMPANY_CODE
                                        //   AND CS.DELETED_FLAG = 'N'";

                                        Query = $@" SELECT DISTINCT {columns} FROM {tableName}
                                       LEFT JOIN (
                                        SELECT 
                                            REFERENCE_NO,
                                            REFERENCE_ITEM_CODE,
                                            REFERENCE_SERIAL_NO,
                                            SUM(REFERENCE_QUANTITY) AS REFERENCE_QUANTITY
                                        FROM REFERENCE_DETAIL
                                        WHERE DELETED_FLAG = 'N'
                                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                        GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE, REFERENCE_SERIAL_NO
                                    ) RD ON SO.BLANKET_NO = RD.REFERENCE_NO
                                         AND SO.ITEM_CODE = RD.REFERENCE_ITEM_CODE
                                         AND SO.SERIAL_NO = RD.REFERENCE_SERIAL_NO
                                              {condition.ToString()}
                                                    WHERE SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                        AND SO.DELETED_FLAG = 'N' AND SO.SERIAL_NO IN {serialNoContinuation} and SO.{primarycolname} in {voucherNoContinuation}
                                        UNION ALL";

                                        //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                                    }


                                    if (Query.Contains("SO.ITEM_CODE"))
                                    {
                                        columns = Regex.Replace(columns, @"\bSO\.ITEM_CODE\b", "IM.ITEM_CODE");
                                        columns = columns + ",IM.ITEM_EDESC";
                                        //tableName = tableName + ", IP_ITEM_MASTER_SETUP IMS";
                                        //condition.Append("AND IMS.ITEM_CODE=SO.ITEM_CODE AND IMS.COMPANY_CODE=SO.COMPANY_CODE AND  IMS.DELETED_FLAG='N'");

                                        condition.Append(" JOIN IP_ITEM_MASTER_SETUP IM ON SO.ITEM_CODE = IM.ITEM_CODE AND SO.COMPANY_CODE = IM.COMPANY_CODE AND IM.DELETED_FLAG = 'N'");

                                        if (Query.Contains("IP_PURCHASE_INVOICE"))
                                        {
                                            columns = columns + ",SO.COMPLETED_QUANTITY";
                                        }
                                        if (Query.Contains("IP_GOODS_REQUISITION"))
                                        {
                                            columns = columns + ",SO.SUPPLIER_CODE";
                                        }
                                        if (previousVoucherNo == union.VOUCHER_NO && int.Parse(previousSerialNo) < int.Parse(union.SERIAL_NO))
                                        {

                                        }
                                        else
                                        {
                                            Query = $@" SELECT DISTINCT {columns}  FROM {tableName}
                                       LEFT JOIN (
                                        SELECT 
                                            REFERENCE_NO,
                                            REFERENCE_ITEM_CODE,
                                            REFERENCE_SERIAL_NO,
                                            SUM(REFERENCE_QUANTITY) AS REFERENCE_QUANTITY
                                        FROM REFERENCE_DETAIL
                                        WHERE DELETED_FLAG = 'N'
                                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                        GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE, REFERENCE_SERIAL_NO
                                    ) RD ON SO.BLANKET_NO = RD.REFERENCE_NO
                                         AND SO.ITEM_CODE = RD.REFERENCE_ITEM_CODE
                                         AND SO.SERIAL_NO = RD.REFERENCE_SERIAL_NO
                                    {condition.ToString()}
                                    WHERE SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                        AND SO.DELETED_FLAG = 'N' AND SO.SERIAL_NO IN {serialNoContinuation} and SO.{primarycolname} in {voucherNoContinuation}
                                        UNION ALL";
                                        }
                                        //Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') {condition.ToString()} UNION ALL";

                                        //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                                    }

                                }


                            }
                            else if (model.TableName == "IP_QC_ENTRY")
                            {
                                Query = $@"SELECT IGE.GATE_NO,
                           IGE.GATE_DATE,
                           IGED.ITEM_CODE,
                           IM.ITEM_EDESC,
                           IGED.BILL_QTY AS QUANTITY,
                           IGED.MU_CODE,
                           IGED.REMARKS,
                           IGED.SERIAL_NO,
                           IGE.BILL_NO,
                           IGE.BILL_DATE,
                           IGE.VEHICLE_NAME AS VEHICLE_NO,
                           IGE.SUPPLIER_CODE  FROM {union.TABLE_NAME} IGE
                           JOIN IP_GATE_ENTRY_DETAIL IGED
                              ON     IGE.GATE_NO = IGED.GATE_NO
                                 AND IGED.DELETED_FLAG = 'N'
                                 AND IGED.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                 AND IGED.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                                 AND IGED.SERIAL_NO = {union.SERIAL_NO}
                            JOIN IP_ITEM_MASTER_SETUP IM
                              ON     IGED.ITEM_CODE = IM.ITEM_CODE
                                 AND IM.DELETED_FLAG = 'N'
                                 AND IM.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                 AND IM.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                     WHERE     IGE.DELETED_FLAG = 'N'
                           AND IGE.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                           AND IGE.GATE_NO = {union.GATE_NO} UNION ALL";
                            }
                            else
                            {
                                Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') UNION ALL";
                                columns = columns + ",SO.SERIAL_NO";
                                if (Query.Contains("SO.CUSTOMER_CODE"))
                                {
                                    columns = columns + ",CS.CUSTOMER_EDESC";
                                    tableName = tableName + ",SA_CUSTOMER_SETUP CS";
                                    condition.Append("AND CS.CUSTOMER_CODE=SO.CUSTOMER_CODE AND CS.COMPANY_CODE=SO.COMPANY_CODE AND  CS.DELETED_FLAG='N'");

                                    Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') {condition.ToString()} UNION ALL";

                                    //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                                }
                                if (Query.Contains("SO.ITEM_CODE"))
                                {
                                    columns = columns + ",IMS.ITEM_EDESC, IMS.NON_VAT_FLAG";
                                    tableName = tableName + ", IP_ITEM_MASTER_SETUP IMS";
                                    condition.Append("AND IMS.ITEM_CODE=SO.ITEM_CODE AND IMS.COMPANY_CODE=SO.COMPANY_CODE AND  IMS.DELETED_FLAG='N'");
                                    if (Query.Contains("IP_PURCHASE_INVOICE"))
                                    {
                                        columns = columns + ",SO.COMPLETED_QUANTITY";
                                    }
                                    if (Query.Contains("IP_GOODS_REQUISITION"))
                                    {
                                        columns = columns + ",SO.SUPPLIER_CODE";
                                    }
                                    Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') {condition.ToString()} UNION ALL";

                                    //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                                }
                            }
                            // Query = Query;
                            RunableQuery += Query;
                            // voucherCount--;

                        }
                    }


                }
                if (RunableQuery.EndsWith("UNION ALL"))
                {
                    RunableQuery = RunableQuery.Remove(RunableQuery.Length - 10);
                }
            }
            else
            {
                foreach (var union in model.checkList)
                {
                    var Query = $@"  SELECT DISTINCT SA.SHIPPING_ADDRESS SHIPPING_ADDRESS,
       SA.MU_CODE,
       SO.QUANTITY,
       SO.UNIT_PRICE,
       SA.ORDER_DATE ORDER_DATE,
       SA.SHIPPING_CONTACT_NO SHIPPING_CONTACT_NO,
      SO.QUANTITY*  SO.UNIT_PRICE  TOTAL_PRICE,
       SO.ITEM_CODE,
       SA.STOCK_BLOCK_FLAG STOCK_BLOCK_FLAG,
       SA.QUANTITY CALC_QUANTITY,
       SO.CUSTOMER_CODE,
       SO.REFERENCE_NO,
       SA.MANUAL_NO,
       SA.UNIT_PRICE CALC_UNIT_PRICE,
       SA.SALES_TYPE_CODE,
      SO.QUANTITY*  SO.UNIT_PRICE as  CALC_TOTAL_PRICE,
       SO.REFERENCE_FORM_CODE FORM_CODE,
       CS.CUSTOMER_EDESC,
       IMS.ITEM_EDESC,
       SA.EXCHANGE_RATE,
       SA.CURRENCY_CODE,
       SA.DELIVERY_DATE,
       SA.PARTY_TYPE_CODE,
       SA.PRIORITY_CODE,
       SA.EMPLOYEE_CODE,
       SA.AGENT_CODE,
       SA.AREA_CODE,
       SA.SECOND_QUANTITY,
       SA.ORDER_NO
  FROM SA_LOADING_SLIP_DETAIL SO, SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IMS,SA_SALES_ORDER SA
 WHERE     SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
     AND SO.REFERENCE_NO = '{union.VOUCHER_NO}'
       AND SO.SERIAL_NO IN ('{union.SERIAL_NO}')
       AND CS.CUSTOMER_CODE = SO.CUSTOMER_CODE
       AND CS.COMPANY_CODE = SO.COMPANY_CODE
       AND CS.DELETED_FLAG = 'N'
       AND IMS.ITEM_CODE = SO.ITEM_CODE
       AND IMS.COMPANY_CODE = SO.COMPANY_CODE
       AND IMS.DELETED_FLAG = 'N'
       AND SO.REFERENCE_NO=SA.ORDER_NO
       AND SO.COMPANY_CODE=SA.COMPANY_CODE
       --AND SO.SERIAL_NO=SA.SERIAL_NO
       AND SO.ITEM_CODE=SA.ITEM_CODE
       AND SO.CUSTOMER_CODE=SA.CUSTOMER_CODE
       AND SA.CUSTOMER_CODE=CS.CUSTOMER_CODE
       AND  SA.ITEM_CODE=IMS.ITEM_CODE
       AND SA.COMPANY_CODE=CS.COMPANY_CODE
       AND SA.COMPANY_CODE=IMS.COMPANY_CODE UNION ALL";
                    RunableQuery += Query;
                }
                if (RunableQuery.EndsWith("UNION ALL"))
                {
                    RunableQuery = RunableQuery.Remove(RunableQuery.Length - 10);
                }
            }

            var entity = this._dbContext.SqlQuery<COMMON_COLUMN>(RunableQuery).ToList();

            if (entity.Count() > 0)
            {

                if (model.TableName == "SA_SALES_ORDER" || model.TableName == "SA_SALES_CHALAN" || model.TableName == "SA_SALES_INVOICE" || model.TableName == "SA_SALES_RETURN")
                {
                    for (int i = 0; i < entity.Count; i++)
                    {
                        var item = entity[i];
                        var prop = item.GetType().GetProperty(primarycolname);
                        var value = prop?.GetValue(item);
                        foreach (var union in model.checkList)
                        {
                            int serialNo = int.Parse(union.SERIAL_NO);
                            if (value?.ToString() == union.VOUCHER_NO && item.SERIAL_NO == serialNo && item.ITEM_CODE == union.ITEM_CODE)
                            {
                                entity[i].QUANTITY = Convert.ToDecimal(union.QUANTITY);
                                entity[i].CALC_QUANTITY = Convert.ToDecimal(union.QUANTITY);
                                entity[i].PRICE = Convert.ToDecimal(union.TOTAL_PRICE);
                                entity[i].TOTAL_PRICE = Convert.ToDecimal(union.TOTAL_PRICE);
                                entity[i].CALC_TOTAL_PRICE = Convert.ToDecimal(union.TOTAL_PRICE);
                            }

                        }
                    }
                }

                if (model.TableName == "SA_SALES_BLANKET")
                {
                    for (int i = 0; i < entity.Count; i++)
                    {
                        var item = entity[i];
                        foreach (var union in model.checkList)
                        {
                            if (union.IS_PARENT_ROW == "N")
                            {
                                int serialNo = int.Parse(union.SERIAL_NO);
                                if (item.BLANKET_NO == union.VOUCHER_NO && item.SERIAL_NO == serialNo && item.ITEM_CODE == union.ITEM_CODE)
                                {
                                    entity[i].QUANTITY = Convert.ToDecimal(union.QUANTITY);
                                    entity[i].CALC_QUANTITY = Convert.ToDecimal(union.QUANTITY);
                                    entity[i].TOTAL_PRICE = Convert.ToDecimal(union.TOTAL_PRICE);
                                    entity[i].CALC_TOTAL_PRICE = Convert.ToDecimal(union.TOTAL_PRICE);
                                    entity[i].UNIT_PRICE = Convert.ToDecimal(union.UNIT_PRICE);
                                    entity[i].CALC_UNIT_PRICE = Convert.ToDecimal(union.UNIT_PRICE);
                                }
                            }
                        }
                    }
                }

                if (model.TableName == "IP_QC_ENTRY")
                {
                    for (int i = 0; i < entity.Count; i++)
                    {
                        var item = entity[i];
                        foreach (var union in model.checkList)
                        {
                            int serialNo = int.Parse(union.SERIAL_NO);
                            if (item.GATE_NO == union.GATE_NO && item.SERIAL_NO == serialNo && item.ITEM_CODE == union.ITEM_CODE)
                            {
                                entity[i].QUANTITY = Convert.ToDecimal(union.QUANTITY);
                            }

                        }
                    }
                }

                if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "incomplete")
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        var referenceNo = entity[i].GetType().GetProperty(primarycolname).GetValue(entity[i], null);

                        var incompleteRefQry = $@"SELECT SUM(REFERENCE_QUANTITY) REFERENCE_QUANTITY ,SUM(REFERENCE_UNIT_PRICE) REFERENCE_UNIT_PRICE, SUM(REFERENCE_TOTAL_PRICE)REFERENCE_TOTAL_PRICE FROM REFERENCE_DETAIL WHERE REFERENCE_NO =('{referenceNo}')  AND REFERENCE_ITEM_CODE IN('{entity[i].ITEM_CODE}')
                                 GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE";
                        var incRefResult = this._dbContext.SqlQuery<REFERENCE_DETAIL_MODEL>(incompleteRefQry).FirstOrDefault();
                        if (incRefResult != null)
                        {
                            entity[i].QUANTITY = Convert.ToDecimal(entity[i].QUANTITY) - Convert.ToDecimal(incRefResult.REFERENCE_QUANTITY);
                            entity[i].UNIT_PRICE = Convert.ToDecimal(entity[i].UNIT_PRICE);
                            entity[i].TOTAL_PRICE = entity[i].QUANTITY * entity[i].UNIT_PRICE;
                            entity[i].CALC_QUANTITY = Convert.ToDecimal(entity[i].CALC_QUANTITY) - Convert.ToDecimal(incRefResult.REFERENCE_QUANTITY);
                            entity[i].CALC_UNIT_PRICE = entity[i].UNIT_PRICE;
                            entity[i].CALC_TOTAL_PRICE = entity[i].CALC_UNIT_PRICE * entity[i].CALC_QUANTITY;
                        }
                    }
                }
            }
            if (model.TableName == "FA_PAY_ORDER" || model.TableName == "FA_JOB_ORDER" || model.TableName == "FA_ADVICE_VOUCHER")
            {
                entity = entity.ToList();

            }
            else
            {
                entity = entity.Where(x => x.QUANTITY > 0).ToList();
            }

            List<DocumentTransaction> imagelist = new List<DocumentTransaction>();
            if (entity.Count > 0)
            {

                if (model.checkList.Count() > 1)
                {
                    foreach (var ch in model.checkList)
                    {
                        string imagequery = $@"SELECT * FROM DOCUMENT_TRANSACTION WHERE FORM_CODE ='{model.FormCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and VOUCHER_NO='{ch.VOUCHER_NO}'";
                        imagelist = this._dbContext.SqlQuery<DocumentTransaction>(imagequery).ToList();
                        entity[0].IMAGES_LIST = imagelist;
                    }
                }
            }
            try
            {
                if (model.TableName.Trim().ToLower() == "sa_sales_order")
                {
                    foreach (var ent in entity)
                    {
                        var queryorder = $@"select nvl(CANCEL_QUANTITY,0) CALCEL_QTY, NVL(ADJUST_QUANTITY,0) ADJUST_QUANTITY from sa_sales_order  where order_no='{ent.ORDER_NO}'  and item_code='{ent.ITEM_CODE}' and deleted_flag='N'  ";
                        var DataCancle = _dbContext.SqlQuery<CancleSalesOrder>(queryorder).FirstOrDefault();
                        if (DataCancle == null)
                        {
                            continue;
                        }

                        /////----Nila this needs to be fixed because its passing different TOTAL_PRICE while implementing Line Wise Calculation-----------///////
                        //ent.QUANTITY = ent.QUANTITY + DataCancle.ADJUST_QUANTITY - DataCancle.CALCEL_QTY;
                        //ent.TOTAL_PRICE = ent.QUANTITY * ent.UNIT_PRICE;
                        //ent.CALC_QUANTITY = ent.CALC_QUANTITY + DataCancle.ADJUST_QUANTITY - DataCancle.CALCEL_QTY;
                        //ent.CALC_TOTAL_PRICE = ent.CALC_UNIT_PRICE * ent.CALC_QUANTITY;

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return entity;
        }
        //done for project management sub project code
        public List<COMMON_COLUMN> VoucherDetailReferenceForTemplate(VoucherRefrence model)
        {
            int FormCode = 0;
            int.TryParse(model.FormCode, out FormCode);
            string FormCodeFilter = string.Empty;
            var RunableQuery = string.Empty;
            var primarycolname = GetPrimaryColumnByTableName(model.TableName);
            if (model.TableName != "SA_LOADING_SLIP_DETAIL")
            {
                if (FormCode > 0)
                {
                    FormCodeFilter = $@" FORM_CODE='{model.FormCode}'";
                }
                else
                {
                    FormCodeFilter = $@" table_name='{model.TableName}'";
                }
                //string columname = $@"SELECT COLUMN_NAME, TABLE_NAME FROM FORM_DETAIL_SETUP WHERE FORM_CODE='{model.FormCode}' and company_code='{_workContext.CurrentUserinformation.company_code}' and display_flag='Y' ORDER BY SERIAL_NO ASC";
                string columname = $@"SELECT COLUMN_NAME, TABLE_NAME FROM FORM_PROJECT_SETUP WHERE {FormCodeFilter} and company_code='{_workContext.CurrentUserinformation.company_code}' and display_flag='Y' group by COLUMN_NAME, TABLE_NAME";
                List<FORM_DETAIL_SETUP_COLUMN> columnameentity = this._dbContext.SqlQuery<FORM_DETAIL_SETUP_COLUMN>(columname).ToList();
                var tableNameMain = "";
                StringBuilder sb = new StringBuilder();
                foreach (var item in columnameentity)
                {
                    sb.Append("SO.").Append(item.COLUMN_NAME).Append(",");
                }
                sb.Append("SO.FORM_CODE");

                //var columns = sb.ToString().TrimEnd(',');
                var columnsMain = sb.ToString();
                string Query = string.Empty;
                StringBuilder condition;
                tableNameMain = model.TableName + " SO";

                if (model.checkList.Count() > 0)
                {
                    var voucherCount = model.checkList.Count();
                    if (model.INCLUDE_CHARGE == "True")
                    {
                        var ChareInclusiveVoucherNo = model.checkList.First().VOUCHER_NO;
                        var columns = columnsMain;
                        condition = new StringBuilder();
                        var tableName = tableNameMain;

                        Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{ChareInclusiveVoucherNo}'";

                        if (Query.Contains("SO.CUSTOMER_CODE"))
                        {
                            columns = columns + ",CS.CUSTOMER_EDESC";
                            tableName = tableName + ",SA_CUSTOMER_SETUP CS";
                            condition.Append("AND CS.CUSTOMER_CODE=SO.CUSTOMER_CODE AND CS.COMPANY_CODE=SO.COMPANY_CODE AND  CS.DELETED_FLAG='N'");

                            Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{ChareInclusiveVoucherNo}' {condition.ToString()}";
                        }
                        if (Query.Contains("SO.ITEM_CODE"))
                        {
                            columns = columns + ",IMS.ITEM_EDESC";
                            tableName = tableName + ", IP_ITEM_MASTER_SETUP IMS";
                            condition.Append("AND IMS.ITEM_CODE=SO.ITEM_CODE AND IMS.COMPANY_CODE=SO.COMPANY_CODE AND  IMS.DELETED_FLAG='N'");

                            Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{ChareInclusiveVoucherNo}' {condition.ToString()}";
                        }
                        RunableQuery += Query;

                    }
                    else
                    {
                        foreach (var union in model.checkList)
                        {
                            var columns = columnsMain;
                            condition = new StringBuilder();
                            var tableName = tableNameMain;

                            Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') UNION ALL";

                            if (Query.Contains("SO.CUSTOMER_CODE"))
                            {
                                columns = columns + ",CS.CUSTOMER_EDESC";
                                tableName = tableName + ",SA_CUSTOMER_SETUP CS";
                                condition.Append("AND CS.CUSTOMER_CODE=SO.CUSTOMER_CODE AND CS.COMPANY_CODE=SO.COMPANY_CODE AND  CS.DELETED_FLAG='N'");

                                Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') {condition.ToString()} UNION ALL";

                                //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                            }
                            if (Query.Contains("SO.SUB_PROJECT_CODE"))
                            {
                                columns = columns + ",SPS.SUB_PROJECT_NAME";
                                tableName = tableName + ", SUB_PROJECT_SETUP SPS";
                                tableName = tableName + ", PROJECT_SETUP PS";
                                condition.Append("AND SO.SUB_PROJECT_CODE=SPS.SUB_PROJECT_ID  AND SPS.DELETED_FLAG='N'");
                                condition.Append("AND PS.ID=SPS.PROJECT_ID  AND PS.STATUS='E'");
                                Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') {condition.ToString()} UNION ALL";

                                //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                            }
                            if (Query.Contains("SO.ITEM_CODE"))
                            {
                                columns = columns + ",IMS.ITEM_EDESC";
                                tableName = tableName + ", IP_ITEM_MASTER_SETUP IMS";
                                condition.Append("AND IMS.ITEM_CODE=SO.ITEM_CODE AND IMS.COMPANY_CODE=SO.COMPANY_CODE AND  IMS.DELETED_FLAG='N'");
                                if (Query.Contains("IP_PURCHASE_INVOICE"))
                                {
                                    columns = columns + ",SO.COMPLETED_QUANTITY";
                                }
                                if (Query.Contains("IP_GOODS_REQUISITION"))
                                {
                                    columns = columns + ",SO.SUPPLIER_CODE";
                                }
                                Query = $@" SELECT {columns} FROM {tableName} WHERE SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{union.VOUCHER_NO}' and SO.SERIAL_NO IN ('{union.SERIAL_NO}') {condition.ToString()} UNION ALL";

                                //Query = $@"SELECT {columns} FROM {tableName} WHERE SO.FORM_CODE ='{formCode}' and SO.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SO.{primarycolname}='{orderno}' {condition.ToString()}";
                            }
                            // Query = Query;
                            RunableQuery += Query;
                            // voucherCount--;

                        }
                    }


                }
                if (RunableQuery.EndsWith("UNION ALL"))
                {
                    RunableQuery = RunableQuery.Remove(RunableQuery.Length - 10);
                }
            }
            else
            {
                foreach (var union in model.checkList)
                {
                    var Query = $@"  SELECT DISTINCT SA.SHIPPING_ADDRESS SHIPPING_ADDRESS,
       SA.MU_CODE,
       SO.QUANTITY,
       SO.UNIT_PRICE,
       SA.ORDER_DATE ORDER_DATE,
       SA.SHIPPING_CONTACT_NO SHIPPING_CONTACT_NO,
      SO.QUANTITY*  SO.UNIT_PRICE  TOTAL_PRICE,
       SO.ITEM_CODE,
       SA.STOCK_BLOCK_FLAG STOCK_BLOCK_FLAG,
       SA.QUANTITY CALC_QUANTITY,
       SO.CUSTOMER_CODE,
       SO.REFERENCE_NO,
       SA.MANUAL_NO,
       SA.UNIT_PRICE CALC_UNIT_PRICE,
       SA.SALES_TYPE_CODE,
      SO.QUANTITY*  SO.UNIT_PRICE as  CALC_TOTAL_PRICE,
       SO.REFERENCE_FORM_CODE FORM_CODE,
       CS.CUSTOMER_EDESC,
       IMS.ITEM_EDESC,
       SA.EXCHANGE_RATE,
       SA.CURRENCY_CODE,
       SA.DELIVERY_DATE,
       SA.PARTY_TYPE_CODE,
       SA.PRIORITY_CODE,
       SA.EMPLOYEE_CODE,
       SA.AGENT_CODE,
       SA.AREA_CODE,
       SA.SECOND_QUANTITY,
       SA.ORDER_NO
  FROM SA_LOADING_SLIP_DETAIL SO, SA_CUSTOMER_SETUP CS, IP_ITEM_MASTER_SETUP IMS,SA_SALES_ORDER SA
 WHERE     SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
     AND SO.REFERENCE_NO = '{union.VOUCHER_NO}'
       AND SO.SERIAL_NO IN ('{union.SERIAL_NO}')
       AND CS.CUSTOMER_CODE = SO.CUSTOMER_CODE
       AND CS.COMPANY_CODE = SO.COMPANY_CODE
       AND CS.DELETED_FLAG = 'N'
       AND IMS.ITEM_CODE = SO.ITEM_CODE
       AND IMS.COMPANY_CODE = SO.COMPANY_CODE
       AND IMS.DELETED_FLAG = 'N'
       AND SO.REFERENCE_NO=SA.ORDER_NO
       AND SO.COMPANY_CODE=SA.COMPANY_CODE
       --AND SO.SERIAL_NO=SA.SERIAL_NO
       AND SO.ITEM_CODE=SA.ITEM_CODE
       AND SO.CUSTOMER_CODE=SA.CUSTOMER_CODE
       AND SA.CUSTOMER_CODE=CS.CUSTOMER_CODE
       AND  SA.ITEM_CODE=IMS.ITEM_CODE
       AND SA.COMPANY_CODE=CS.COMPANY_CODE
       AND SA.COMPANY_CODE=IMS.COMPANY_CODE UNION ALL";
                    RunableQuery += Query;
                }
                if (RunableQuery.EndsWith("UNION ALL"))
                {
                    RunableQuery = RunableQuery.Remove(RunableQuery.Length - 10);
                }
            }

            var entity = this._dbContext.SqlQuery<COMMON_COLUMN>(RunableQuery).ToList();
            if (entity.Count() > 0)
            {
                if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "incomplete")
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        var referenceNo = entity[i].GetType().GetProperty(primarycolname).GetValue(entity[i], null);

                        var incompleteRefQry = $@"SELECT SUM(REFERENCE_QUANTITY) REFERENCE_QUANTITY ,SUM(REFERENCE_UNIT_PRICE) REFERENCE_UNIT_PRICE, SUM(REFERENCE_TOTAL_PRICE)REFERENCE_TOTAL_PRICE FROM REFERENCE_DETAIL WHERE REFERENCE_NO =('{referenceNo}')  AND REFERENCE_ITEM_CODE IN('{entity[i].ITEM_CODE}')
                                 GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE";
                        var incRefResult = this._dbContext.SqlQuery<REFERENCE_DETAIL_MODEL>(incompleteRefQry).FirstOrDefault();
                        if (incRefResult != null)
                        {
                            entity[i].QUANTITY = Convert.ToDecimal(entity[i].QUANTITY) - Convert.ToDecimal(incRefResult.REFERENCE_QUANTITY);
                            entity[i].UNIT_PRICE = Convert.ToDecimal(entity[i].UNIT_PRICE);
                            entity[i].TOTAL_PRICE = entity[i].QUANTITY * entity[i].UNIT_PRICE;
                            entity[i].CALC_QUANTITY = Convert.ToDecimal(entity[i].CALC_QUANTITY) - Convert.ToDecimal(incRefResult.REFERENCE_QUANTITY);
                            entity[i].CALC_UNIT_PRICE = entity[i].UNIT_PRICE;
                            entity[i].CALC_TOTAL_PRICE = entity[i].CALC_UNIT_PRICE * entity[i].CALC_QUANTITY;
                        }
                    }
                }
            }
            if (model.TableName == "FA_PAY_ORDER" || model.TableName == "FA_JOB_ORDER" || model.TableName == "FA_ADVICE_VOUCHER")
            {
                entity = entity.ToList();

            }
            else
            {
                entity = entity.Where(x => x.QUANTITY > 0).ToList();
            }

            List<DocumentTransaction> imagelist = new List<DocumentTransaction>();
            if (entity.Count > 0)
            {

                if (model.checkList.Count() > 1)
                {
                    foreach (var ch in model.checkList)
                    {
                        string imagequery = $@"SELECT * FROM DOCUMENT_TRANSACTION WHERE FORM_CODE ='{model.FormCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and VOUCHER_NO='{ch.VOUCHER_NO}'";
                        imagelist = this._dbContext.SqlQuery<DocumentTransaction>(imagequery).ToList();
                        entity[0].IMAGES_LIST = imagelist;
                    }
                }
            }
            try
            {
                if (model.TableName.Trim().ToLower() == "sa_sales_order")
                {
                    foreach (var ent in entity)
                    {
                        var queryorder = $@"select nvl(CANCEL_QUANTITY,0) CALCEL_QTY, NVL(ADJUST_QUANTITY,0) ADJUST_QUANTITY from sa_sales_order  where order_no='{ent.ORDER_NO}'  and item_code='{ent.ITEM_CODE}' and deleted_flag='N'  ";
                        var DataCancle = _dbContext.SqlQuery<CancleSalesOrder>(queryorder).FirstOrDefault();
                        if (DataCancle == null)
                        {
                            continue;
                        }

                        ent.QUANTITY = ent.QUANTITY + DataCancle.ADJUST_QUANTITY - DataCancle.CALCEL_QTY;
                        ent.TOTAL_PRICE = ent.QUANTITY * ent.UNIT_PRICE;
                        ent.CALC_QUANTITY = ent.CALC_QUANTITY + DataCancle.ADJUST_QUANTITY - DataCancle.CALCEL_QTY;
                        ent.CALC_TOTAL_PRICE = ent.CALC_UNIT_PRICE * ent.CALC_QUANTITY;

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return entity;
        }
        public List<COMMON_COLUMN> getReferenceGridData(REFERENCE_MODEL model)
        {
            var column = getColName(model);
            var result = this._dbContext.SqlQuery<COMMON_COLUMN>(column).ToList();
            if (result.Count() > 0)
            {
                if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "incomplete")
                //  if (model.TABLE_NAME == "SA_SALES_ORDER" && !string.IsNullOrEmpty(model.ROW) && model.ROW == "incomplete")
                {
                    foreach (var item in result)
                    {
                        var incompleteRefQry = $@"SELECT SUM(REFERENCE_QUANTITY) REFERENCE_QUANTITY ,SUM(REFERENCE_UNIT_PRICE) REFERENCE_UNIT_PRICE, SUM(REFERENCE_TOTAL_PRICE)REFERENCE_TOTAL_PRICE FROM REFERENCE_DETAIL WHERE REFERENCE_NO =('{item.VOUCHER_NO}')  AND REFERENCE_ITEM_CODE IN('{item.ITEM_CODE}')
                                 GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE";
                        var incRefResult = this._dbContext.SqlQuery<REFERENCE_DETAIL_MODEL>(incompleteRefQry).FirstOrDefault();
                        if (incRefResult != null)
                        {
                            item.QUANTITY = Convert.ToDecimal(item.QUANTITY) - Convert.ToDecimal(incRefResult.REFERENCE_QUANTITY);
                            item.UNIT_PRICE = Convert.ToDecimal(item.UNIT_PRICE);
                            item.TOTAL_PRICE = item.QUANTITY * item.UNIT_PRICE;
                            item.CALC_QUANTITY = Convert.ToDecimal(item.CALC_QUANTITY) - Convert.ToDecimal(incRefResult.REFERENCE_QUANTITY);
                            item.CALC_UNIT_PRICE = Convert.ToDecimal(item.CALC_UNIT_PRICE);
                            item.CALC_TOTAL_PRICE = item.CALC_QUANTITY * item.CALC_UNIT_PRICE;
                        }
                    }
                }
            }
            if (model.TABLE_NAME == "FA_PAY_ORDER" || model.TABLE_NAME == "FA_JOB_ORDER" || model.TABLE_NAME == "FA_ADVICE_VOUCHER")
            {
                result = result.ToList();
            }
            else
            {
                result = result.Where(x => x.QUANTITY > 0).ToList();
            }

            return result;
        }

        public string getColName(REFERENCE_MODEL model)
        {
            string sqlquery = string.Empty, condition = string.Empty;
            if (model.DOCUMENT == "SA_LOADING_SLIP_DETAIL")
            {
                sqlquery = $@"SELECT SO.REFERENCE_NO as VOUCHER_NO, SO.ACCESS_DATE as VOUCHER_DATE, IM.ITEM_CODE,IM.ITEM_EDESC,
                                SO.SERIAL_NO,CS.CUSTOMER_CODE,CS.CUSTOMER_EDESC,
                                MU_CODE, SO.QUANTITY, SO.UNIT_PRICE, (SO.QUANTITY * SO.UNIT_PRICE) TOTAL_PRICE, SO.QUANTITY CALC_QUANTITY,
                                SO.UNIT_PRICE CALC_UNIT_PRICE, (SO.QUANTITY * SO.UNIT_PRICE) CALC_TOTAL_PRICE, SO.QUANTITY COMPLETED_QUANTITY, ''  REMARKS,SO.REFERENCE_FORM_CODE FORM_CODE,
                                SO.CREATED_DATE FROM SA_LOADING_SLIP_DETAIL SO,IP_ITEM_MASTER_SETUP IM,SA_CUSTOMER_SETUP CS
                                WHERE  SO.ITEM_CODE = IM.ITEM_CODE AND CS.CUSTOMER_CODE= SO.CUSTOMER_CODE AND SO.COMPANY_CODE = IM.COMPANY_CODE
                                AND SO.COMPANY_CODE = CS.COMPANY_CODE  
                                AND SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND SO.REFERENCE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@"AND SO.REFERENCE_NO = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND SO.REFERENCE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";

                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@"AND SO.REFERENCE_FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND SO.CUSTOMER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(SO.ACCESS_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND SO.REFERENCE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
                return sqlquery;
            }
            ///////////////----------Nila code before taking line wise Quantity--------//////////////
            //else if (model.TABLE_NAME == "SA_SALES_ORDER")
            //{
            //    sqlquery = $@"SELECT SO.ORDER_NO AS VOUCHER_NO,
            //                   SO.ORDER_DATE AS VOUCHER_DATE,
            //                   IM.ITEM_CODE,
            //                   IM.ITEM_EDESC,
            //                   SO.SERIAL_NO,
            //                   CS.CUSTOMER_CODE,
            //                   CS.CUSTOMER_EDESC,
            //                   MU_CODE,
            //                   SO.QUANTITY,
            //                   SO.UNIT_PRICE,
            //                   SO.TOTAL_PRICE,
            //                   SO.CALC_QUANTITY,
            //                   SO.CALC_UNIT_PRICE,
            //                   SO.CALC_TOTAL_PRICE,
            //                   SO.COMPLETED_QUANTITY,
            //                   SO.REMARKS,
            //                   SO.FORM_CODE,
            //                   SO.PRICE,
            //                   SO.DISCOUNT_CLASS,
            //                   SO.DISCOUNT_AMOUNT,
            //                   SO.EXCISE_DUTY_AMOUNT,
            //                   SO.LUXURY_TAX_AMOUNT,
            //                   SO.VAT_AMOUNT,
            //                   SO.CREATED_DATE
            //              FROM SA_SALES_ORDER SO, IP_ITEM_MASTER_SETUP IM, SA_CUSTOMER_SETUP CS
            //             WHERE     SO.DELETED_FLAG = 'N'
            //                   AND SO.ITEM_CODE = IM.ITEM_CODE
            //                   AND CS.CUSTOMER_CODE = SO.CUSTOMER_CODE
            //                   AND SO.COMPANY_CODE = IM.COMPANY_CODE
            //                   AND SO.COMPANY_CODE = CS.COMPANY_CODE
            //                   AND SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            //    if (!string.IsNullOrEmpty(model.VOUCHER_NO))
            //    {
            //        sqlquery = sqlquery + $@" AND SO.ORDER_NO = '{model.VOUCHER_NO}'";
            //        if (!string.IsNullOrEmpty(model.FORM_CODE))
            //        {
            //            if (model.FORM_CODE != "0")
            //                sqlquery = sqlquery + $@"AND SO.FORM_CODE = '{model.FORM_CODE}'";
            //        }
            //        if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
            //            sqlquery = sqlquery + $@" AND SO.ORDER_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";

            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(model.FORM_CODE))
            //        {
            //            if (model.FORM_CODE != "0")
            //                sqlquery = sqlquery + $@"AND SO.FORM_CODE = '{model.FORM_CODE}'";
            //        }
            //        if (!string.IsNullOrEmpty(model.NAME))
            //            sqlquery = sqlquery + $@" AND SO.CUSTOMER_CODE = '{model.NAME}'";

            //        if (!string.IsNullOrEmpty(model.ITEM_DESC))
            //            sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
            //        if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
            //            sqlquery = sqlquery + $@" AND trunc(SO.ORDER_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
            //        if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
            //            //sqlquery = sqlquery + $@" AND SO.ORDER_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
            //        if (model.COLUMNS_FILTER != null)
            //            if (model.COLUMNS_FILTER.Count > 0)
            //            {
            //                foreach (var column in model.COLUMNS_FILTER)
            //                {
            //                    if (!string.IsNullOrEmpty(column.COLUMN_NAME))
            //                    {
            //                        if (column.COLUMN_NAME.Contains("DATE"))
            //                        {
            //                            sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} =  TO_DATE('{ column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
            //                        }
            //                        else
            //                        {
            //                            sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
            //                        }
            //                    }
            //                }
            //            }
            //    }
            //}
            ///////////////----------Nila code before taking line wise Quantity--------//////////////

            ///////////////----------Nila code after taking line wise Quantity--------//////////////
            else if (model.TABLE_NAME == "SA_SALES_ORDER")
            {
                sqlquery = $@"SELECT SO.ORDER_NO AS VOUCHER_NO,
                               SO.ORDER_DATE AS VOUCHER_DATE,
                               IM.ITEM_CODE,
                               IM.ITEM_EDESC,
                               SO.SERIAL_NO,
                               CS.CUSTOMER_CODE,
                               CS.CUSTOMER_EDESC,
                               MU_CODE,
                               (SO.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS QUANTITY,
                               SO.UNIT_PRICE,
                               ((SO.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) * SO.UNIT_PRICE) AS TOTAL_PRICE,
                               (SO.CALC_QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS CALC_QUANTITY,
                               SO.CALC_UNIT_PRICE,
                               ((SO.CALC_QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) * SO.CALC_UNIT_PRICE) AS CALC_TOTAL_PRICE,
                               SO.COMPLETED_QUANTITY,
                               SO.REMARKS,
                               SO.FORM_CODE,
                               SO.PRICE,
                               SO.DISCOUNT_CLASS,
                               SO.DISCOUNT_AMOUNT,
                               SO.EXCISE_DUTY_AMOUNT,
                               SO.LUXURY_TAX_AMOUNT,
                               SO.VAT_AMOUNT,
                               SO.CREATED_DATE
                               FROM SA_SALES_ORDER SO
                               JOIN IP_ITEM_MASTER_SETUP IM
                                  ON     SO.ITEM_CODE = IM.ITEM_CODE
                                     AND SO.COMPANY_CODE = IM.COMPANY_CODE
                               JOIN SA_CUSTOMER_SETUP CS
                                  ON     CS.CUSTOMER_CODE = SO.CUSTOMER_CODE
                                     AND SO.COMPANY_CODE = CS.COMPANY_CODE
                                LEFT JOIN (  SELECT REFERENCE_NO,
                                                           REFERENCE_ITEM_CODE,
                                                           SUM (REFERENCE_QUANTITY) AS REFERENCE_QUANTITY
                                                      FROM REFERENCE_DETAIL
                                                     WHERE DELETED_FLAG = 'N'
                                                  GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE) RD
                                          ON     SO.ORDER_NO = RD.REFERENCE_NO
                                             AND SO.ITEM_CODE = RD.REFERENCE_ITEM_CODE
                         WHERE     SO.DELETED_FLAG = 'N'
                                   AND (SO.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) > 0
                               AND SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND SO.ORDER_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@"AND SO.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                    {
                        //sqlquery = sqlquery + $@" AND SO.ORDER_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@"AND SO.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND SO.CUSTOMER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(SO.ORDER_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        //sqlquery = sqlquery + $@" AND SO.ORDER_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                        if (model.COLUMNS_FILTER != null)
                            if (model.COLUMNS_FILTER.Count > 0)
                            {
                                foreach (var column in model.COLUMNS_FILTER)
                                {
                                    if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                    {
                                        if (column.COLUMN_NAME.Contains("DATE"))
                                        {
                                            sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                        }
                                        else
                                        {
                                            sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                        }
                                    }
                                }
                            }
                }
            }

            ///////////////----------Nila code after taking line wise Quantity--------//////////////

            else if (model.TABLE_NAME == "SA_SALES_BLANKET")
            {

                /////////---------------For Parent Row--------------------///////////////
                sqlquery = $@"SELECT
                                SB.BLANKET_NO AS VOUCHER_NO,
                                SB.BLANKET_DATE AS VOUCHER_DATE,
                                SB.ITEM_CODE AS ITEM_CODE,
                                IM.ITEM_EDESC,
                                SB.ITEM_CODE AS MAIN_ITEM_CODE,
                                SB.SERIAL_NO AS SERIAL_NO,
                                CS.CUSTOMER_CODE,
                                CS.CUSTOMER_EDESC,
                                SB.MU_CODE,
                                (SB.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS QUANTITY,
                                SB.UNIT_PRICE,
                                SB.TOTAL_PRICE,
                                SB.CALC_UNIT_PRICE,
                                SB.CALC_TOTAL_PRICE,
                                SB.COMPLETED_QUANTITY,
                                SB.REMARKS,
                                SB.FORM_CODE,
                                SB.CREATED_DATE,
                                0 AS RATE,
                                'Y' AS IS_PARENT_ROW,
                                CASE 
                                    WHEN MC.MAIN_ITEM_CODE IS NOT NULL THEN 'Y'
                                    ELSE 'N'
                                END AS HAS_CHILD
                               FROM SA_SALES_BLANKET SB
                                JOIN IP_ITEM_MASTER_SETUP IM ON SB.ITEM_CODE = IM.ITEM_CODE AND SB.COMPANY_CODE = IM.COMPANY_CODE
                                JOIN SA_CUSTOMER_SETUP CS ON SB.CUSTOMER_CODE = CS.CUSTOMER_CODE AND SB.COMPANY_CODE = CS.COMPANY_CODE
                                LEFT JOIN (
                                    SELECT DISTINCT MAIN_ITEM_CODE, COMPANY_CODE
                                    FROM MASTER_BLANKET_ITEM_CONVERSION
                                    WHERE DELETED_FLAG = 'N'
                                ) MC 
                                    ON SB.ITEM_CODE = MC.MAIN_ITEM_CODE 
                                    AND SB.COMPANY_CODE = MC.COMPANY_CODE 
                                AND SB.COMPANY_CODE = MC.COMPANY_CODE
                                LEFT JOIN (
                                    SELECT REFERENCE_NO, REFERENCE_ITEM_CODE, SUM(REFERENCE_QUANTITY) AS REFERENCE_QUANTITY
                                    FROM REFERENCE_DETAIL 
                                    WHERE DELETED_FLAG = 'N'
                                    GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE
                                ) RD ON SB.BLANKET_NO = RD.REFERENCE_NO AND SB.ITEM_CODE = RD.REFERENCE_ITEM_CODE
                                WHERE SB.DELETED_FLAG = 'N'
                                  AND SB.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                              ";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND SB.BLANKET_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@"AND SB.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                    {
                        //sqlquery = sqlquery + $@" AND SO.BLANKET_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                        sqlquery = sqlquery + $@" ";
                    }

                    sqlquery = sqlquery + $@" AND (SB.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) > 0";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                        {
                            sqlquery = sqlquery + $@"AND SB.FORM_CODE = '{model.FORM_CODE}'";
                        }
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND SB.CUSTOMER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(SB.BLANKET_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" ";
                    if (model.COLUMNS_FILTER != null)
                    {
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                    }

                    sqlquery = sqlquery + $@" AND (SB.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) > 0";
                }
                sqlquery = sqlquery + $@" UNION ALL ";


                /////////---------------For Child Row--------------------///////////////
                sqlquery = sqlquery + $@"SELECT
                                SB.BLANKET_NO AS VOUCHER_NO,
                                SB.BLANKET_DATE AS VOUCHER_DATE,
                                MC.ITEM_CODE AS ITEM_CODE,
                                IM.ITEM_EDESC,
                                SB.ITEM_CODE AS MAIN_ITEM_CODE,
                                MC.SERIAL_NO,
                                CS.CUSTOMER_CODE,
                                CS.CUSTOMER_EDESC,
                                SB.MU_CODE,
                                0 AS QUANTITY, -- Same quantity as parent
                                (SB.UNIT_PRICE + MC.RATE) AS UNIT_PRICE,
                                0 AS TOTAL_PRICE,
                                SB.CALC_UNIT_PRICE,
                                0 AS CALC_TOTAL_PRICE,
                                SB.COMPLETED_QUANTITY,
                                SB.REMARKS,
                                SB.FORM_CODE,
                                SB.CREATED_DATE,
                                MC.RATE AS RATE,
                                'N' AS IS_PARENT_ROW,
                                'N' AS HAS_CHILD
                            FROM SA_SALES_BLANKET SB
                            JOIN MASTER_BLANKET_ITEM_CONVERSION MC 
                                ON SB.ITEM_CODE = MC.MAIN_ITEM_CODE 
                                AND SB.COMPANY_CODE = MC.COMPANY_CODE
                            JOIN IP_ITEM_MASTER_SETUP IM ON MC.ITEM_CODE = IM.ITEM_CODE AND SB.COMPANY_CODE = IM.COMPANY_CODE
                            JOIN SA_CUSTOMER_SETUP CS ON SB.CUSTOMER_CODE = CS.CUSTOMER_CODE AND SB.COMPANY_CODE = CS.COMPANY_CODE
                            LEFT JOIN (
                                SELECT REFERENCE_NO, REFERENCE_ITEM_CODE, REFERENCE_SERIAL_NO, SUM(REFERENCE_QUANTITY) AS REFERENCE_QUANTITY
                                FROM REFERENCE_DETAIL 
                                WHERE DELETED_FLAG = 'N'
                                GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE, REFERENCE_SERIAL_NO
                            ) RD ON SB.BLANKET_NO = RD.REFERENCE_NO AND SB.ITEM_CODE = RD.REFERENCE_ITEM_CODE AND SB.SERIAL_NO = RD.REFERENCE_SERIAL_NO
                            WHERE SB.DELETED_FLAG = 'N'
                              AND SB.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                              ";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND SB.BLANKET_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@"AND SB.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                    {
                        //sqlquery = sqlquery + $@" AND SO.BLANKET_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                        sqlquery = sqlquery + $@" ";
                    }

                    sqlquery = sqlquery + $@" AND (SB.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) > 0";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                        {
                            sqlquery = sqlquery + $@"AND SB.FORM_CODE = '{model.FORM_CODE}'";
                        }
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND SB.CUSTOMER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(SB.BLANKET_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" ";
                    if (model.COLUMNS_FILTER != null)
                    {
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" AND {column.ORAND} SO.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                    }

                    sqlquery = sqlquery + $@" AND (SB.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) > 0";
                }
            }

            else if (model.TABLE_NAME == "SA_SALES_INVOICE")
            {
                sqlquery = $@"SELECT SI.SALES_NO AS VOUCHER_NO,
                               SI.SALES_DATE AS VOUCHER_DATE,
                               SI.SERIAL_NO,
                               CS.CUSTOMER_CODE,
                               CS.CUSTOMER_EDESC,
                               SI.AGENT_CODE,
                               IM.ITEM_CODE,
                               IM.ITEM_EDESC,
                               MU_CODE,
                               (SI.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS QUANTITY,
                               SI.UNIT_PRICE,
                               ((SI.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) * SI.UNIT_PRICE) AS TOTAL_PRICE,
                               (SI.CALC_QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS CALC_QUANTITY,
                               SI.CALC_UNIT_PRICE,
                               ((SI.CALC_QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) * SI.CALC_UNIT_PRICE) AS CALC_TOTAL_PRICE,
                               SI.COMPLETED_QUANTITY,
                               SI.REMARKS,
                               SI.CREATED_DATE,
                               SI.FORM_CODE
                          FROM SA_SALES_INVOICE SI JOIN IP_ITEM_MASTER_SETUP IM
                                  ON     SI.ITEM_CODE = IM.ITEM_CODE
                                     AND SI.COMPANY_CODE = IM.COMPANY_CODE
                               JOIN SA_CUSTOMER_SETUP CS
                                  ON     CS.CUSTOMER_CODE = SI.CUSTOMER_CODE
                                     AND SI.COMPANY_CODE = CS.COMPANY_CODE
                                LEFT JOIN (  SELECT REFERENCE_NO,
                                                           REFERENCE_ITEM_CODE,
                                                           SUM (REFERENCE_QUANTITY) AS REFERENCE_QUANTITY
                                                      FROM REFERENCE_DETAIL
                                                     WHERE DELETED_FLAG = 'N'
                                                  GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE) RD
                                          ON     SI.SALES_NO = RD.REFERENCE_NO
                                             AND SI.ITEM_CODE = RD.REFERENCE_ITEM_CODE
                         WHERE     SI.DELETED_FLAG = 'N'
                                   AND (SI.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) > 0
                        AND SI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND SI.SALES_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND SI.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                    {
                        //sqlquery = sqlquery + $@" AND SI.SALES_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND SI.FORM_CODE = '{model.FORM_CODE}'";
                    }

                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND SI.CUSTOMER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(SI.SALES_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                    {
                        //sqlquery = sqlquery + $@" AND SI.SALES_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    }
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} SI.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} SI.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "SA_SALES_INVOICE_JEWEL")
            {
                sqlquery = $@"SELECT SI.SALES_NO as VOUCHER_NO, SI.SALES_DATE as VOUCHER_DATE, SI.SERIAL_NO, CS.CUSTOMER_CODE,CS.CUSTOMER_EDESC,SI.AGENT_CODE, IM.ITEM_CODE,IM.ITEM_EDESC, MU_CODE, SI.QUANTITY, SI.UNIT_PRICE, SI.TOTAL_PRICE, SI.NET_AMOUNT, SI.CALC_QUANTITY, SI.CALC_UNIT_PRICE, SI.CALC_TOTAL_PRICE, SI.COMPLETED_QUANTITY, SI.REMARKS, SI.CREATED_DATE,SI.FORM_CODE FROM SA_SALES_INVOICE_JEWEL SI,SA_CUSTOMER_SETUP CS,IP_ITEM_MASTER_SETUP IM  WHERE SI.DELETED_FLAG='N' AND SI.ITEM_CODE = IM.ITEM_CODE AND CS.CUSTOMER_CODE= SI.CUSTOMER_CODE AND SI.COMPANY_CODE = IM.COMPANY_CODE AND SI.COMPANY_CODE = CS.COMPANY_CODE AND SI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND SI.SALES_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND SI.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND SI.SALES_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";

                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND SI.FORM_CODE = '{model.FORM_CODE}'";
                    }

                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND SI.CUSTOMER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(SI.SALES_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND SI.SALES_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} SI.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} SI.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "SA_SALES_CHALAN")
            {
                sqlquery = $@"SELECT DISTINCT SC.CHALAN_NO AS VOUCHER_NO,
                               SC.CHALAN_DATE AS VOUCHER_DATE,
                               SC.SERIAL_NO,
                               CS.CUSTOMER_CODE,
                               CS.CUSTOMER_EDESC,
                               IL.LOCATION_CODE,
                               IL.LOCATION_EDESC,
                               SC.MU_CODE,
                               (SC.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS QUANTITY,
                               SC.UNIT_PRICE,
                               ((SC.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) * SC.UNIT_PRICE) AS TOTAL_PRICE,
                               (SC.CALC_QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) AS CALC_QUANTITY,
                               SC.CALC_UNIT_PRICE,
                               ((SC.CALC_QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) * SC.CALC_UNIT_PRICE) AS CALC_TOTAL_PRICE,
                               SC.COMPLETED_QUANTITY,
                               SC.REMARKS,
                               SC.CREATED_DATE,
                               IP.PARTY_TYPE_CODE,
                               IP.PARTY_TYPE_EDESC,
                               IC.PRIORITY_CODE,
                               IC.PRIORITY_EDESC,
                               SC.SHIPPING_ADDRESS,
                               SC.SHIPPING_CONTACT_NO,
                               SC.SALES_TYPE_CODE,
                               SC.AGENT_CODE,
                               SC.DIVISION_CODE,
                               SC.EMPLOYEE_CODE,
                               SC.FORM_CODE,
                               SC.ITEM_CODE,
                               IM.ITEM_EDESC
                          FROM SA_SALES_CHALAN SC
                                              JOIN
                                                 IP_ITEM_MASTER_SETUP IM
                                              ON     SC.ITEM_CODE = IM.ITEM_CODE
                                                 AND SC.COMPANY_CODE = IM.COMPANY_CODE
                                          LEFT JOIN
                                              IP_PARTY_TYPE_CODE IP
                                           ON     SC.COMPANY_CODE = IP.COMPANY_CODE
                                              AND SC.COMPANY_CODE = IP.COMPANY_CODE
                                              AND SC.PARTY_TYPE_CODE = IP.PARTY_TYPE_CODE
                                       LEFT JOIN
                                           IP_PRIORITY_CODE IC
                                        ON SC.PRIORITY_CODE = IC.PRIORITY_CODE
                                    LEFT JOIN
                                        IP_LOCATION_SETUP IL
                                     ON     SC.FROM_LOCATION_CODE = IL.LOCATION_CODE
                                        AND SC.COMPANY_CODE = IL.COMPANY_CODE
                                  JOIN
                                     SA_CUSTOMER_SETUP CS
                                  ON     CS.CUSTOMER_CODE = SC.CUSTOMER_CODE
                                     AND SC.COMPANY_CODE = CS.COMPANY_CODE
                               LEFT JOIN
                                  (  SELECT REFERENCE_NO,
                                            REFERENCE_ITEM_CODE,
                                            SUM (REFERENCE_QUANTITY) AS REFERENCE_QUANTITY
                                       FROM REFERENCE_DETAIL
                                      WHERE DELETED_FLAG = 'N'
                                   GROUP BY REFERENCE_NO, REFERENCE_ITEM_CODE) RD
                               ON     SC.CHALAN_NO = RD.REFERENCE_NO
                                  AND SC.ITEM_CODE = RD.REFERENCE_ITEM_CODE
                         WHERE SC.DELETED_FLAG = 'N'     
                                   AND (SC.QUANTITY - NVL(RD.REFERENCE_QUANTITY, 0)) > 0
                  AND SC.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND SC.CHALAN_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {

                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND SC.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                    {
                        //sqlquery = sqlquery + $@" AND SC.CHALAN_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {

                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND SC.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND SC.CUSTOMER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(SC.CHALAN_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        //sqlquery = sqlquery + $@" AND SC.CHALAN_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                        if (model.COLUMNS_FILTER != null)
                            if (model.COLUMNS_FILTER.Count > 0)
                            {
                                foreach (var column in model.COLUMNS_FILTER)
                                {
                                    if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                    {
                                        if (column.COLUMN_NAME.Contains("DATE"))
                                        {
                                            sqlquery = sqlquery + $@" {column.ORAND} SC.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                        }
                                        else
                                        {
                                            sqlquery = sqlquery + $@" {column.ORAND} SC.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                        }
                                    }
                                }
                            }
                }
            }
            else if (model.TABLE_NAME == "IP_PURCHASE_ORDER")
            {

                sqlquery = $@"SELECT IPO.ORDER_NO as VOUCHER_NO, IPO.ORDER_DATE as VOUCHER_DATE, IM.ITEM_CODE,IM.ITEM_EDESC, IPO.SERIAL_NO, ISS.SUPPLIER_CODE,ISS.SUPPLIER_EDESC, IPO.MU_CODE, IPO.QUANTITY, IPO.UNIT_PRICE, IPO.TOTAL_PRICE, IPO.CALC_QUANTITY,
                                IPO.CALC_UNIT_PRICE, IPO.CALC_TOTAL_PRICE, IPO.COMPLETED_QUANTITY, IPO.REMARKS, IPO.CREATED_DATE,IPO.FORM_CODE,IPO.MANUAL_NO FROM IP_PURCHASE_ORDER IPO,IP_ITEM_MASTER_SETUP IM,IP_SUPPLIER_SETUP ISS
                            WHERE IPO.DELETED_FLAG='N' AND IPO.ITEM_CODE = IM.ITEM_CODE AND ISS.SUPPLIER_CODE= IPO.SUPPLIER_CODE AND IPO.COMPANY_CODE = IM.COMPANY_CODE
                                AND IPO.COMPANY_CODE = ISS.COMPANY_CODE AND 
                                IPO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND IPO.ORDER_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IPO.ORDER_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IPO.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND IPO.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(IPO.ORDER_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IPO.ORDER_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IPO.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IPO.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "IP_GATE_ENTRY")
            {

                sqlquery = $@"SELECT IGE.GATE_NO,
                           IGE.GATE_DATE,
                           IGED.ITEM_CODE,
                           IM.ITEM_EDESC,
                           IGED.BILL_QTY AS QUANTITY,
                           IGED.BILL_QTY AS CALC_QUANTITY,
                           IGED.MU_CODE,
                           IGED.REMARKS,
                           IGED.SERIAL_NO
                      FROM IP_GATE_ENTRY IGE
                           JOIN IP_GATE_ENTRY_DETAIL IGED
                              ON     IGE.GATE_NO = IGED.GATE_NO
                                 AND IGED.DELETED_FLAG = 'N'
                                 AND IGED.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                 AND IGED.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                           JOIN IP_ITEM_MASTER_SETUP IM
                              ON     IGED.ITEM_CODE = IM.ITEM_CODE
                                 AND IM.DELETED_FLAG = 'N'
                                 AND IM.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                 AND IM.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                     WHERE     IGE.DELETED_FLAG = 'N'
                           AND IGE.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                           AND IGE.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND IGE.GATE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IGE.GATE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    //if (!string.IsNullOrEmpty(model.FORM_CODE))
                    //{
                    //    if (model.FORM_CODE != "0")
                    //        sqlquery = sqlquery + $@" AND IPO.FORM_CODE = '{model.FORM_CODE}'";
                    //}
                    //if (!string.IsNullOrEmpty(model.NAME))
                    //    sqlquery = sqlquery + $@" AND IPO.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(IGE.GATE_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        //sqlquery = sqlquery + $@" AND IGE.GATE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                        if (model.COLUMNS_FILTER != null)
                            if (model.COLUMNS_FILTER.Count > 0)
                            {
                                foreach (var column in model.COLUMNS_FILTER)
                                {
                                    if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                    {
                                        if (column.COLUMN_NAME.Contains("DATE"))
                                        {
                                            sqlquery = sqlquery + $@" {column.ORAND} IGE.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                        }
                                        else
                                        {
                                            sqlquery = sqlquery + $@" {column.ORAND} IGE.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                        }
                                    }
                                }
                            }
                }
            }
            else if (model.TABLE_NAME == "IP_TRANSFER_ISSUE")
            {
                sqlquery = $@"SELECT ITI.ISSUE_NO AS VOUCHER_NO,ITI.ISSUE_DATE AS VOUCHER_DATE,IISTC.ISSUE_TYPE_CODE,IISTC.ISSUE_TYPE_EDESC,ILS.LOCATION_CODE as FROM_LOCATION_CODE,ILS.LOCATION_EDESC as FROM_LOCATION_EDESC,
            ITI.FROM_BUDGET_FLAG,ILS.LOCATION_CODE as TO_LOCATION_CODE,ILS.LOCATION_EDESC as TO_LOCATION_EDESC,ITI.TO_BUDGET_FLAG,IM.ITEM_CODE,IM.ITEM_EDESC,ITI.MU_CODE,ITI.QUANTITY,       ITI.UNIT_PRICE,ITI.TOTAL_PRICE,ITI.CALC_QUANTITY,ITI.CALC_UNIT_PRICE,ITI.CALC_TOTAL_PRICE,ITI.COMPLETED_QUANTITY,ITI.REMARKS,ITI.CREATED_DATE,ITI.TO_BRANCH_CODE,ISS.SUPPLIER_CODE,
       ISS.SUPPLIER_EDESC,ITI.ACTUAL_QUANTITY,SCS.CUSTOMER_CODE,SCS.CUSTOMER_EDESC,ITI.DIVISION_CODE,ITI.FORM_CODE
      FROM IP_TRANSFER_ISSUE ITI,IP_ISSUE_TYPE_CODE IISTC, IP_LOCATION_SETUP ILS,IP_ITEM_MASTER_SETUP IM,IP_SUPPLIER_SETUP ISS,SA_CUSTOMER_SETUP SCS
        WHERE 
        ITI.issue_type_code=IISTC.issue_type_code
    AND ITI.company_code=IISTC.company_code 
    AND ITI.from_location_code=ILS.location_code(+)   
    AND ITI.to_location_code=ILS.location_code(+)  
    AND ITI.company_code=ILS.company_code(+)   
    AND ITI.item_code=IM.item_code  
    AND ITI.company_code=IM.company_code  
    AND  ITI.supplier_code=ISS.supplier_code(+) 
    AND ITI.company_code=ISS.company_code(+)  
    AND ITI.customer_code=SCS.customer_code(+)
    AND ITI.company_code=SCS.company_code(+) 
    AND ITI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND ITI.ISSUE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND ITI.ISSUE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND ITI.FORM_CODE='{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND ITI.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(ITI.ISSUE_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND ITI.ISSUE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} ITI.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} ITI.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "IP_GOODS_ISSUE")
            {
                sqlquery = $@"SELECT IGI.ISSUE_NO AS VOUCHER_NO,IGI.ISSUE_DATE AS VOUCHER_DATE,IISTC.ISSUE_TYPE_CODE,IISTC.ISSUE_TYPE_EDESC,ILS.LOCATION_CODE AS FROM_LOCATION_CODE,ILS.LOCATION_EDESC AS FROM_LOCATION_EDESC,
       ILS.LOCATION_CODE AS TO_LOCATION_CODE,ILS.LOCATION_EDESC AS TO_LOCATION_EDESC,IGI.TO_BUDGET_FLAG,IM.ITEM_CODE,IM.ITEM_EDESC,IGI.MU_CODE,IGI.REQ_QUANTITY,IGI.QUANTITY,IGI.UNIT_PRICE,IGI.SERIAL_NO,       IGI.TOTAL_PRICE,IGI.CALC_QUANTITY,IGI.CALC_UNIT_PRICE,IGI.CALC_TOTAL_PRICE,IGI.COMPLETED_QUANTITY,IGI.REMARKS,IGI.CREATED_DATE,IGI.PRODUCT_CODE,SCS.CUSTOMER_CODE,SCS.CUSTOMER_EDESC,     HES.EMPLOYEE_CODE,HES.EMPLOYEE_EDESC,ISS.SUPPLIER_CODE,ISS.SUPPLIER_EDESC,IGI.ISSUE_SLIP_NO,IGI.DIVISION_CODE,IGI.FORM_CODE
        ,IGI.MANUAL_NO
FROM IP_GOODS_ISSUE IGI,
       IP_ISSUE_TYPE_CODE IISTC,
       IP_LOCATION_SETUP ILS,
       IP_ITEM_MASTER_SETUP IM,
       IP_SUPPLIER_SETUP ISS,
       SA_CUSTOMER_SETUP SCS,
       HR_EMPLOYEE_SETUP HES
 WHERE     IGI.ISSUE_TYPE_CODE = IISTC.ISSUE_TYPE_CODE(+)
       AND IGI.COMPANY_CODE = IISTC.COMPANY_CODE(+)
       AND IGI.FROM_LOCATION_CODE = ILS.LOCATION_CODE(+)
       AND IGI.TO_LOCATION_CODE = ILS.LOCATION_CODE(+)
       AND IGI.COMPANY_CODE = ILS.COMPANY_CODE(+)
       AND IGI.ITEM_CODE = IM.ITEM_CODE
       AND IGI.COMPANY_CODE = IM.COMPANY_CODE
       AND IGI.SUPPLIER_CODE = ISS.SUPPLIER_CODE(+)
       AND IGI.COMPANY_CODE = ISS.COMPANY_CODE(+)
       AND IGI.CUSTOMER_CODE = SCS.CUSTOMER_CODE(+)
       AND IGI.COMPANY_CODE=SCS.COMPANY_CODE (+)
       AND IGI.EMPLOYEE_CODE=HES.EMPLOYEE_CODE(+)
       AND IGI.COMPANY_CODE=HES.COMPANY_CODE (+) AND IGI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND IGI.ISSUE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IGI.ISSUE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND IGI.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(IGI.ISSUE_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IGI.ISSUE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IGI.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IGI.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "IP_GATE_PASS_ENTRY")
            {
                sqlquery = $@"SELECT IE.ISSUE_NO as VOUCHER_NO, IE.ISSUE_DATE as VOUCHER_DATE, IE.REFERENCE_NO, IE.REFERENCE_FORM_CODE,                      IE.REFERENCE_PARTY_CODE,
                              IE.PARTY_FLAG, IE.DOCUMENT_TYPE_CODE,IE. REMARKS, IE.FROM_LOCATION_CODE, IE.ITEM_CODE,IM.ITEM_EDESC, IE.MU_CODE, IE.QUANTITY, IE.UNIT_PRICE, IE.TOTAL_PRICE,IE.SERIAL_NO,
                               IE.DIVISION_CODE,IE.FORM_CODE,IE.MANUAL_NO FROM IP_GATE_PASS_ENTRY IE,IP_LOCATION_SETUP IL,IP_ITEM_MASTER_SETUP IM
                               WHERE IE.DELETED_FLAG='N'
                               AND IE.ITEM_CODE = IM.ITEM_CODE
                               AND IE.FROM_LOCATION_CODE=IL.LOCATION_CODE(+)
                               AND IE.COMPANY_CODE = IM.COMPANY_CODE 
                               AND IE.COMPANY_CODE = IL.COMPANY_CODE(+) 
                               AND IE.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND IE.ISSUE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IE.ISSUE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(IE.ISSUE_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IE.ISSUE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IE.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IE.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "IP_QUOTATION_INQUIRY")
            {
                sqlquery = $@"SELECT QI.QUOTE_NO as VOUCHER_NO, QI.QUOTE_DATE as VOUCHER_DATE, QI.ORDER_NO, QI.REQUEST_NO, SS.SUPPLIER_CODE,SS.SUPPLIER_EDESC, ADDRESS, CONTACT_PERSON, PHONE_NO, IM.ITEM_CODE,
IM.ITEM_EDESC, QI.SPECIFICATION, QI.MU_CODE, QUANTITY, QI.UNIT_PRICE, QI.TOTAL_PRICE, QI.REMARKS, QI.CREATED_DATE, QI.BRAND_NAME, QI.CREDIT_DAYS, QI.RANK_VALUE ,QI.FORM_CODE,QI.SERIAL_NO 
FROM IP_QUOTATION_INQUIRY QI, IP_SUPPLIER_SETUP SS,IP_ITEM_MASTER_SETUP IM
WHERE SS.SUPPLIER_CODE=QI.SUPPLIER_CODE AND
SS.COMPANY_CODE=QI.COMPANY_CODE AND 
IM.ITEM_CODE=QI.ITEM_CODE AND
IM.COMPANY_CODE=QI.COMPANY_CODE AND QI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND QI.QUOTE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND QI.QUOTE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND QI.FORM_CODE='{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND QI.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND QI.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(QI.QUOTE_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND QI.QUOTE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} QI.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} QI.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "IP_GOODS_REQUISITION")
            {
                sqlquery = $@"select  IGR.REQUISITION_NO as VOUCHER_NO, IGR.REQUISITION_DATE as VOUCHER_DATE,IGR.FROM_LOCATION_CODE,IGR.TO_LOCATION_CODE,IGR.ITEM_CODE,IGR.QUANTITY,
                            IGR.CALC_QUANTITY,IGR.MU_CODE,IGR.TOTAL_PRICE,IM.ITEM_EDESC,ISS.SUPPLIER_EDESC,IGR.SERIAL_NO,IGR.MANUAL_NO,IGR.UNIT_PRICE
                            from IP_GOODS_REQUISITION IGR,IP_ITEM_MASTER_SETUP IM
                            ,IP_SUPPLIER_SETUP ISS 
                            WHERE IGR.COMPANY_CODE=IM.COMPANY_CODE
                        AND  IGR.ITEM_CODE=IM.ITEM_CODE
                        AND IGR.SUPPLIER_CODE=ISS.SUPPLIER_CODE(+)
AND IGR.COMPANY_CODE=IM.COMPANY_CODE
AND IGR.DELETED_FLAG='N'
                  AND IGR.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND IGR.REQUISITION_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IGR.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IGR.REQUISITION_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IGR.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND IGR.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(IGR.REQUISITION_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IGR.REQUISITION_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IGR.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IGR.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "IP_PURCHASE_RETURN")
            {
                sqlquery = $@"SELECT RETURN_NO as VOUCHER_NO, RETURN_DATE as VOUCHER_DATE, SUPPLIER_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, FROM_LOCATION_CODE, ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, BUDGET_CODE, REMARKS,FORM_CODE,SERIAL_NO FROM IP_PURCHASE_RETURN";
            }
            else if (model.TABLE_NAME == "SA_SALES_RETURN")
            {
                sqlquery = $@"SELECT RETURN_NO as VOUCHER_NO, RETURN_DATE as VOUCHER_DATE, CUSTOMER_CODE, ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, TO_LOCATION_CODE, PARTY_TYPE_CODE, REASON, DIVISION_CODE,FORM_CODE,SERIAL_NO FROM SA_SALES_RETURN";
            }
            else if (model.TABLE_NAME == "IP_PURCHASE_INVOICE")
            {
                sqlquery = $@"SELECT IP.INVOICE_NO as VOUCHER_NO, IP.INVOICE_DATE as VOUCHER_DATE, IP.SUPPLIER_CODE,ISS.SUPPLIER_EDESC, IP.SUPPLIER_INV_NO, IP.SUPPLIER_INV_DATE,
                           IP.ITEM_CODE,IM.ITEM_EDESC, IP.MU_CODE, IP.QUANTITY, IP.UNIT_PRICE, IP.TOTAL_PRICE, IP.CALC_QUANTITY, IP.CALC_UNIT_PRICE, IP.CALC_TOTAL_PRICE, IP.COMPLETED_QUANTITY, IP.REMARKS,
                            IP.TO_LOCATION_CODE, IP.LOT_NO, IP.SUPPLIER_MRR_NO, IP.DIVISION_CODE,IP.FORM_CODE,IP.SERIAL_NO,IP.TERMS_DAY,IP.MANUAL_NO FROM IP_PURCHASE_INVOICE IP,IP_ITEM_MASTER_SETUP IM,IP_SUPPLIER_SETUP ISS
                            WHERE IP.DELETED_FLAG='N' AND IP.ITEM_CODE = IM.ITEM_CODE AND ISS.SUPPLIER_CODE= IP.SUPPLIER_CODE AND IP.COMPANY_CODE = IM.COMPANY_CODE
                           AND IP.COMPANY_CODE = ISS.COMPANY_CODE
                           AND IP.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND IP.INVOICE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IP.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IP.INVOICE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IP.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND IP.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IP.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(IP.INVOICE_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IP.INVOICE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IP.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IP.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "IP_GOODS_ISSUE_RETURN")
            {
                sqlquery = $@"SELECT RETURN_NO as VOUCHER_NO, RETURN_DATE as VOUCHER_DATE, CUSTOMER_CODE, ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE,COMPLETED_QUANTITY, REMARKS,FROM_LOCATION_CODE, TO_LOCATION_CODE,  DIVISION_CODE,FORM_CODE,SERIAL_NO,CURRENCY_CODE,EXCHANGE_RATE,TO_BUDGET_FLAG FROM IP_GOODS_ISSUE_RETURN;";
            }
            else if (model.TABLE_NAME == "IP_RETURNABLE_GOODS_RETURN")
            {
                sqlquery = $@"SELECT ISSUE_NO as VOUCHER_NO, ISSUE_DATE as VOUCHER_DATE, CUSTOMER_CODE, ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE,COMPLETED_QUANTITY, REMARKS,FROM_LOCATION_CODE ,TO_LOCATION_CODE,  DIVISION_CODE,FORM_CODE,SERIAL_NO,CURRENCY_CODE,EXCHANGE_RATE FROM IP_RETURNABLE_GOODS_RETURN";
            }
            else if (model.TABLE_NAME == "IP_PURCHASE_REQUEST")
            {
                sqlquery = $@"select  IGR.REQUEST_NO as VOUCHER_NO, IGR.REQUEST_DATE as VOUCHER_DATE,BS_DATE(IGR.REQUEST_DATE) as NepaliVOUCHER_DATE,IGR.FROM_LOCATION_CODE,IGR.TO_LOCATION_CODE,IGR.ITEM_CODE,IGR.QUANTITY,
                            IGR.CALC_QUANTITY,IGR.MU_CODE,IGR.TOTAL_PRICE,IM.ITEM_EDESC,ISS.SUPPLIER_EDESC,IGR.SERIAL_NO,IGR.MANUAL_NO,IGR.UNIT_PRICE
                            from IP_PURCHASE_REQUEST IGR,IP_ITEM_MASTER_SETUP IM
                            ,IP_SUPPLIER_SETUP ISS 
                            WHERE IGR.COMPANY_CODE=IM.COMPANY_CODE
                        AND  IGR.ITEM_CODE=IM.ITEM_CODE
                        AND IGR.SUPPLIER_CODE=ISS.SUPPLIER_CODE(+)
                        AND IGR.COMPANY_CODE=IM.COMPANY_CODE
                        AND IGR.DELETED_FLAG='N'
                        AND IGR.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND IGR.REQUEST_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IGR.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IGR.REQUEST_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IGR.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND IGR.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(IGR.REQUEST_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IGR.REQUEST_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IGR.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IGR.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
                // sqlquery = $@"SELECT REQUEST_NO as VOUCHER_NO, REQUEST_DATE as VOUCHER_DATE, FROM_LOCATION_CODE, TO_LOCATION_CODE, ITEM_CODE, SPECIFICATION, MU_CODE,  QUANTITY,  UNIT_PRICE,  TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, SUPPLIER_CODE, DIVISION_CODE,FORM_CODE FROM IP_PURCHASE_REQUEST";
            }
            else if (model.TABLE_NAME == "IP_PRODUCTION_MRR")
            {
                sqlquery = $@"SELECT MRR_NO as VOUCHER_NO, MRR_DATE as VOUCHER_DATE, FROM_LOCATION_CODE, TO_LOCATION_CODE, ITEM_CODE, MU_CODE,  QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, DIVISION_CODE,FORM_CODE,SERIAL_NO FROM IP_PRODUCTION_MRR";
            }
            else if (model.TABLE_NAME == "IP_ADVICE_MRR")
            {
                sqlquery = $@"SELECT AM.MRR_NO as VOUCHER_NO, AM.MRR_DATE as VOUCHER_DATE, LS.LOCATION_CODE AS TO_LOCATION_CODE, LS.LOCATION_EDESC, TO_BUDGET_FLAG, IM.ITEM_CODE,IM.ITEM_EDESC, AM.MU_CODE,  
AM.QUANTITY, AM.UNIT_PRICE, AM.TOTAL_PRICE, AM.CALC_QUANTITY, AM.CALC_UNIT_PRICE, AM.CALC_TOTAL_PRICE, AM.COMPLETED_QUANTITY,AM.SERIAL_NO
FROM IP_ADVICE_MRR AM, IP_LOCATION_SETUP LS, IP_ITEM_MASTER_SETUP IM
WHERE  LS.LOCATION_CODE=AM.TO_LOCATION_CODE AND 
IM.ITEM_CODE=AM.ITEM_CODE
 AND IPMRR.COMPANY_CODE=IL.COMPANY_CODE AND AM.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND AM.MRR_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND AM.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND AM.MRR_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND AM.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND AM.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(AM.MRR_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND AM.MRR_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} AM.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} AM.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "IP_PURCHASE_MRR")
            {
                sqlquery = $@"SELECT IPMRR.MRR_NO as VOUCHER_NO, IPMRR.MRR_DATE as VOUCHER_DATE,IPMRR.SERIAL_NO, ISS.SUPPLIER_CODE,ISS.SUPPLIER_EDESC, IL.LOCATION_CODE,IL.LOCATION_EDESC, IM.ITEM_CODE,
                           IM.ITEM_EDESC,IPMRR.MU_CODE, IPMRR.QUANTITY, IPMRR.UNIT_PRICE, IPMRR.TOTAL_PRICE, IPMRR.CALC_UNIT_PRICE, IPMRR.CALC_QUANTITY, IPMRR.COMPLETED_QUANTITY,IPMRR.REMARKS, IPMRR.CALC_TOTAL_PRICE,IPMRR.FORM_CODE,IPMRR.MANUAL_NO FROM IP_PURCHASE_MRR IPMRR,IP_SUPPLIER_SETUP ISS,IP_LOCATION_SETUP IL,IP_ITEM_MASTER_SETUP IM WHERE IPMRR.DELETED_FLAG='N' 
                              AND IPMRR.ITEM_CODE = IM.ITEM_CODE 
                              AND IPMRR.SUPPLIER_CODE= ISS.SUPPLIER_CODE 
                              AND IPMRR.COMPANY_CODE = IM.COMPANY_CODE
                              AND IPMRR.COMPANY_CODE = ISS.COMPANY_CODE 
                              AND IPMRR.TO_LOCATION_CODE=IL.LOCATION_CODE  
                              AND IPMRR.COMPANY_CODE=IL.COMPANY_CODE  
                              AND IPMRR.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND IPMRR.MRR_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IPMRR.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IPMRR.MRR_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND IPMRR.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND IPMRR.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(IPMRR.MRR_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND IPMRR.MRR_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IPMRR.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} IPMRR.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "FA_DOUBLE_VOUCHER")
            {
                sqlquery = $@"SELECT VOUCHER_NO as VOUCHER_NO, VOUCHER_DATE as VOUCHER_DATE, ACC_CODE, PARTICULARS, TRANSACTION_TYPE, AMOUNT, BUDGET_FLAG, REMARKS, CREATED_DATE, SUPPLIER_CODE, DIVISION_CODE, EMPLOYEE_CODE FROM FA_DOUBLE_VOUCHER";
            }
            else if (model.TABLE_NAME == "FA_SINGLE_VOUCHER")
            {
                sqlquery = $@"SELECT VOUCHER_NO as VOUCHER_NO, VOUCHER_DATE as VOUCHER_DATE, MASTER_ACC_CODE, MASTER_TRANSACTION_TYPE, MASTER_AMOUNT, MASTER_BUDGET_FLAG, ACC_CODE, PARTICULARS, TRANSACTION_TYPE, AMOUNT, BUDGET_FLAG, REMARKS, CREATED_DATE, ACC_ID, INVOICE_NO, DIVISION_CODE, EMPLOYEE_CODE,FORM_CODE FROM FA_SINGLE_VOUCHER";
            }
            else if (model.TABLE_NAME == "IP_PRODUCTION_ISSUE")
            {
                sqlquery = $@"SELECT ISSUE_NO as VOUCHER_NO, ISSUE_DATE as VOUCHER_DATE, ISSUE_TYPE_CODE, FROM_LOCATION_CODE, TO_LOCATION_CODE, ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, PRODUCTION_QTY, REFERENCE_NO,FORM_CODE,SERIAL_NO FROM IP_PRODUCTION_ISSUE";
            }
            else if (model.TABLE_NAME == "IP_RETURNABLE_GOODS_ISSUE")
            {
                sqlquery = $@"SELECT RG.ISSUE_NO AS VOUCHER_NO,RG.ISSUE_DATE AS VOUCHER_DATE,RG.MANUAL_NO,LS.LOCATION_CODE AS FROM_LOCATION_CODE,LS.LOCATION_EDESC AS FROM_LOCATION_EDESC, LS.LOCATION_CODE AS TO_LOCATION_EDESC,RG.SERIAL_NO,    IM.ITEM_CODE,IM.ITEM_EDESC,RG.MU_CODE,RG.QUANTITY,RG.UNIT_PRICE,RG.TOTAL_PRICE,RG.CALC_QUANTITY,RG.CALC_UNIT_PRICE,RG.CALC_TOTAL_PRICE,RG.COMPLETED_QUANTITY,RG.REMARKS,      RG.CURRENCY_CODE,RG.EXCHANGE_RATE,RG.PARTY_CODE,RG.ACKNOWLEDGE_REMARKS,RG.ACKNOWLEDGE_BY,RG.ACKNOWLEDGE_DATE,RG.OPENING_DATA_FLAG,SS.SUPPLIER_CODE,SS.SUPPLIER_EDESC,     CS.CUSTOMER_CODE,CS.CUSTOMER_EDESC,RG.DIVISION_CODE,RG.REFERENCE_NO,RG.EST_DELIVERY_DATE,RG.SECOND_QUANTITY,RG.THIRD_QUANTITY
        
        FROM IP_RETURNABLE_GOODS_ISSUE RG,
       IP_LOCATION_SETUP LS,
       IP_ITEM_MASTER_SETUP IM,
       IP_SUPPLIER_SETUP SS,
       SA_CUSTOMER_SETUP CS
      WHERE RG.TO_LOCATION_CODE=LS.LOCATION_CODE (+)
       AND  RG.FROM_LOCATION_CODE=LS.LOCATION_CODE (+)
       AND RG.COMPANY_CODE=LS.COMPANY_CODE(+)
       AND RG.ITEM_CODE=IM.ITEM_CODE
       AND RG.COMPANY_CODE=IM.COMPANY_CODE
       AND RG.SUPPLIER_CODE=SS.SUPPLIER_CODE(+)
       AND RG.COMPANY_CODE=SS.COMPANY_CODE(+)
       AND RG.CUSTOMER_CODE=CS.CUSTOMER_CODE(+)
       AND RG.COMPANY_CODE=CS.COMPANY_CODE(+)  AND RG.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND RG.ISSUE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND RG.ISSUE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND RG.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND RG.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.ITEM_DESC))
                        sqlquery = sqlquery + $@" AND IM.ITEM_EDESC = '{model.ITEM_DESC}'";
                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(RG.ISSUE_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND RG.ISSUE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} RG.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} RG.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "FA_JOB_ORDER")
            {
                sqlquery = $@"SELECT FJ.JOB_NO AS VOUCHER_NO,FJ.VOUCHER_DATE,FJ.MANUAL_NO,FJ.TRANSACTION_TYPE,FJ.SERIAL_NO,FJ.REMARKS,FJ.AUTHORISED_REMARKS,FJ.CHECKED_REMARKS,FJ.VERIFY_REMARKS,FJ.TRACKING_NO,FJ.CURRENCY_CODE,FJ.EXCHANGE_RATE,SS.SUPPLIER_CODE,SS.SUPPLIER_EDESC,FA.ACC_CODE,FA.ACC_EDESC
        FROM FA_JOB_ORDER FJ,
       IP_SUPPLIER_SETUP SS
      WHERE
        FJ.SUPPLIER_CODE=SS.SUPPLIER_CODE(+)
       AND FJ.COMPANY_CODE=SS.COMPANY_CODE(+)
       AND FJ.ACC_CODE=FA.ACC_CODE(+)
       AND FJ.COMPANY_CODE=FA.COMPANY_CODE(+)
       AND FJ.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND FJ.JOB_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND FJ.JOB_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND FJ.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND FJ.SUPPLIER_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(FJ.VOUCHER_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND FJ.JOB_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} FJ.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} RG.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "FA_ADVICE_VOUCHER")
            {
                sqlquery = $@"SELECT FJ.ADVICE_NO AS VOUCHER_NO,FJ.VOUCHER_DATE,FJ.MANUAL_NO,FJ.TRANSACTION_TYPE,FJ.SERIAL_NO,FJ.REMARKS,FJ.AUTHORISED_REMARKS,FJ.CHECKED_REMARKS,FJ.VERIFY_REMARKS,FJ.TRACKING_NO,FJ.CURRENCY_CODE,FJ.EXCHANGE_RATE,FA.ACC_CODE,FA.ACC_EDESC
        FROM FA_ADVICE_VOUCHER FJ,
       FA_CHART_OF_ACCOUNTS_SETUP FA
      WHERE
            FJ.ACC_CODE=FA.ACC_CODE(+)
       AND FJ.COMPANY_CODE=FA.COMPANY_CODE(+)
       AND FJ.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND FJ.ADVICE_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND FJ.ADVICE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND FJ.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND FJ.ACC_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(FJ.VOUCHER_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND FJ.ADVICE_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} FJ.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} RG.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            else if (model.TABLE_NAME == "FA_PAY_ORDER")
            {
                sqlquery = $@"SELECT FJ.VOUCHER_NO,FJ.VOUCHER_DATE,FJ.MANUAL_NO,FJ.TRANSACTION_TYPE,FJ.SERIAL_NO,FJ.REMARKS,FJ.CURRENCY_CODE,FJ.EXCHANGE_RATE,FA.ACC_CODE,FA.ACC_EDESC,FJ.AMOUNT,FJ.CHEQUE_NO,FJ.PAYMENT_MODE
        FROM FA_PAY_ORDER FJ,
       FA_CHART_OF_ACCOUNTS_SETUP FA
      WHERE
            FJ.ACC_CODE=FA.ACC_CODE(+)
       AND FJ.COMPANY_CODE=FA.COMPANY_CODE(+)
       AND FJ.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                if (!string.IsNullOrEmpty(model.VOUCHER_NO))
                {
                    sqlquery = sqlquery + $@" AND FJ.VOUCHER_NO = '{model.VOUCHER_NO}'";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND FJ.VOUCHER_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.FORM_CODE))
                    {
                        if (model.FORM_CODE != "0")
                            sqlquery = sqlquery + $@" AND FJ.FORM_CODE = '{model.FORM_CODE}'";
                    }
                    if (!string.IsNullOrEmpty(model.NAME))
                        sqlquery = sqlquery + $@" AND FJ.ACC_CODE = '{model.NAME}'";

                    if (!string.IsNullOrEmpty(model.FROM_DATE) && !string.IsNullOrEmpty(model.TO_DATE))
                        sqlquery = sqlquery + $@" AND trunc(FJ.VOUCHER_DATE) BETWEEN TO_DATE('" + model.FROM_DATE + "', 'YYYY-MM-DD') AND TO_DATE('" + model.TO_DATE + "', 'YYYY-MM-DD')";
                    if (!string.IsNullOrEmpty(model.ROW) && model.ROW == "nonrefrence")
                        sqlquery = sqlquery + $@" AND FJ.VOUCHER_NO  not in (select reference_no from REFERENCE_DETAIL where company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N' )";
                    if (model.COLUMNS_FILTER != null)
                        if (model.COLUMNS_FILTER.Count > 0)
                        {
                            foreach (var column in model.COLUMNS_FILTER)
                            {
                                if (!string.IsNullOrEmpty(column.COLUMN_NAME))
                                {
                                    if (column.COLUMN_NAME.Contains("DATE"))
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} FJ.{column.COLUMN_NAME} =  TO_DATE('{column.COLUMN_VALUE}', 'DD-Mon-YYYY') ";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + $@" {column.ORAND} RG.{column.COLUMN_NAME} = '{column.COLUMN_VALUE}'";
                                    }
                                }
                            }
                        }
                }
            }
            return sqlquery;
        }
        public string NewTransactionNo(string companycode, string formcode, string transactiondate)
        {
            try
            {
                if (companycode != "" && formcode != "" && transactiondate != "")
                {
                    string query = string.Format(@"select FN_TRANSACTION_UNIT_ID('{0}','{1}','{2}') FROM DUAL", companycode, formcode, transactiondate);
                    string transactionNo = this._dbContext.SqlQuery<string>(query).First();
                    return transactionNo;
                }
                else
                { return ""; }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string getBudgetCodeCountByAccCode(string accCode)
        {
            var result = string.Empty;
            string query = $@"SELECT TO_CHAR(count(*))
                        FROM BC_BUDGET_CENTER_MAP
                        where DELETED_FLAG='N'  AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}' AND ACC_CODE = '{accCode}'";
            result = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }
        public List<MenuModels> GetAllMenuItems()
        {
            try
            {
                var rslt = new List<MenuModels>();
                if (this._cacheManager.IsSet($"GetAllMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}"))
                {
                    var data = _cacheManager.Get<List<MenuModels>>($"GetAllMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}");
                    rslt = data;
                }
                else
                {
                    string query = $@" SELECT FORM_CODE,FORM_EDESC,MODULE_CODE,GROUP_SKU_FLAG,PRE_FORM_CODE,MASTER_FORM_CODE,FORM_TYPE,  (SELECT DISTINCT  FS.FORM_CODE FROM FORM_SETUP FS 
                    INNER JOIN MASTER_TRANSACTION MT ON MT.FORM_CODE=FS.FORM_CODE INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE
                  WHERE FS.MODULE_CODE=FORM_SETUP.MODULE_CODE AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' AND FS.FORM_CODE = FORM_SETUP.FORM_CODE)CHILD_FORM_CODE FROM FORM_SETUP
                    WHERE  MODULE_CODE = '01' and deleted_flag='N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND GROUP_SKU_FLAG = 'I' AND DELETED_FLAG = 'N' AND  FORM_CODE IN (
		            SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO = '{_workContext.CurrentUserinformation.User_id}' AND CREATE_FLAG = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' )";
                    rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
                    this._cacheManager.Set($"GetAllMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}", rslt, 20);
                }

                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenuModels> GetAllInventoryMenuItems()
        {
            try
            {
                var rslt = new List<MenuModels>();
                if (this._cacheManager.IsSet($"GetAllInventoryMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}"))
                {
                    var data = _cacheManager.Get<List<MenuModels>>($"GetAllInventoryMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}");
                    rslt = data;
                }
                else
                {
                    string query = $@" SELECT FORM_CODE,FORM_EDESC,MODULE_CODE,GROUP_SKU_FLAG,PRE_FORM_CODE,MASTER_FORM_CODE,FORM_TYPE,  (SELECT DISTINCT  FS.FORM_CODE FROM FORM_SETUP FS 
                    INNER JOIN MASTER_TRANSACTION MT ON MT.FORM_CODE=FS.FORM_CODE INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE
                  WHERE FS.MODULE_CODE=FORM_SETUP.MODULE_CODE AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' AND FS.FORM_CODE = FORM_SETUP.FORM_CODE)CHILD_FORM_CODE FROM FORM_SETUP
                    WHERE  MODULE_CODE = '02' and deleted_flag='N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND GROUP_SKU_FLAG = 'I' AND FORM_CODE IN (
		            SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO = '{_workContext.CurrentUserinformation.User_id}' AND CREATE_FLAG = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}')";
                    rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
                    this._cacheManager.Set($"GetAllInventoryMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}", rslt, 20);
                }
                return rslt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<MenuModels> GetAllSalesMenuItems()
        {
            try
            {
                var rslt = new List<MenuModels>();
                if (this._cacheManager.IsSet($"GetAllSalesMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}"))
                {
                    var data = _cacheManager.Get<List<MenuModels>>($"GetAllSalesMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}");
                    rslt = data;
                }
                else
                {
                    string query = $@" SELECT FORM_CODE,FORM_EDESC,MODULE_CODE,GROUP_SKU_FLAG,PRE_FORM_CODE,MASTER_FORM_CODE,FORM_TYPE,  (SELECT DISTINCT  FS.FORM_CODE FROM FORM_SETUP FS 
                            INNER JOIN MASTER_TRANSACTION MT ON MT.FORM_CODE=FS.FORM_CODE INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE
                          WHERE FS.MODULE_CODE=FORM_SETUP.MODULE_CODE AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' AND FS.FORM_CODE = FORM_SETUP.FORM_CODE)CHILD_FORM_CODE FROM FORM_SETUP
                            WHERE  MODULE_CODE = '04' and deleted_flag='N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND GROUP_SKU_FLAG = 'I' AND FORM_CODE IN (
                      SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO = '{_workContext.CurrentUserinformation.User_id}' AND CREATE_FLAG = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}')";
                    rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
                    this._cacheManager.Set($"GetAllSalesMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}", rslt, 20);
                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MenuModels> GetAllProductionMeneItems()
        {
            try
            {
                var rslt = new List<MenuModels>();
                if (this._cacheManager.IsSet($"GetAllProductionMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}"))
                {
                    var data = _cacheManager.Get<List<MenuModels>>($"GetAllProductionMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}");
                    rslt = data;
                }
                else
                {
                    string query = $@" SELECT FORM_CODE,FORM_EDESC,MODULE_CODE,GROUP_SKU_FLAG,PRE_FORM_CODE,MASTER_FORM_CODE,FORM_TYPE,  (SELECT DISTINCT  FS.FORM_CODE FROM FORM_SETUP FS 
                    INNER JOIN MASTER_TRANSACTION MT ON MT.FORM_CODE=FS.FORM_CODE INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE
                  WHERE FS.MODULE_CODE=FORM_SETUP.MODULE_CODE AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' AND FS.FORM_CODE = FORM_SETUP.FORM_CODE)CHILD_FORM_CODE FROM FORM_SETUP
                    WHERE  MODULE_CODE = '03' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND GROUP_SKU_FLAG = 'I' AND FORM_CODE IN (
		            SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO = '{_workContext.CurrentUserinformation.User_id}' AND CREATE_FLAG = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}')";
                    rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
                    this._cacheManager.Set($"GetAllProductionMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}", rslt, 20);
                }
                return rslt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<PartyType> GetAllPartyType()
        {
            List<PartyType> result = new List<PartyType>();
            string query = $@"SELECT
                        COALESCE(PARTY_TYPE_CODE,' ') as PARTY_TYPE_CODE, 
                        COALESCE(PARTY_TYPE_EDESC,' ') as PARTY_TYPE_EDESC,
                       ACC_CODE 
                        FROM IP_PARTY_TYPE_CODE
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            result = this._dbContext.SqlQuery<PartyType>(query).ToList();
            return result;
        }
        public List<PartyRating> GetAllPartyRating()
        {
            List<PartyRating> result = new List<PartyRating>();
            string query = $@"SELECT
                        COALESCE(PR_CODE,' ') as PR_CODE, 
                        COALESCE(PR_EDESC,' ') as PR_EDESC 
                        FROM IP_PARTY_RATING_CODE
                        where DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            result = this._dbContext.SqlQuery<PartyRating>(query).ToList();
            return result;
        }
        public List<DocumentType> getDocumentTypeListByFlter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                var sqlquery = string.Format(@"SELECT DISTINCT
                                    COALESCE(TYPE_CODE,' ') TYPE_CODE,
                                    COALESCE(TYPE_EDESC,' ') TYPE_EDESC
                                    from HR_DOCUMENT_TYPE_CODE
                                    where deleted_flag='N'  and COMPANY_CODE={1} and TYPE_CODE like '%{0}%'
                                    or upper(TYPE_EDESC)like '%{0}%'", filter.ToUpperInvariant(), _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code);
                var result = _dbContext.SqlQuery<DocumentType>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Voucher Transaction
        public string GetNewSequence()
        {
            var query = "select MYSEQUENCE .NEXTVAL from dual";
            _logErp.InfoInFile("GetNewSequence ====================================:" + _dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString());
            return _dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString();
        }
        #endregion
        public List<DocumentSubMenu> GetMaseterTransDetailByFormCode(string formCode, string docVer = "all")
        {
            try
            {
                string query = $@"SELECT FORM_CODE, VOUCHER_NO,VOUCHER_AMOUNT,VOUCHER_DATE,CREATED_BY,CREATED_DATE,CHECKED_BY,CHECKED_DATE,AUTHORISED_BY,POSTED_DATE,MODIFY_DATE,SYN_ROWID,REFERENCE_NO,SESSION_ROWID, COMPANY_CODE FROM MASTER_TRANSACTION WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N' ORDER BY VOUCHER_NO DESC";
                //                string query = $@"SELECT FORM_CODE, VOUCHER_NO,VOUCHER_AMOUNT,VOUCHER_DATE,CREATED_BY,CREATED_DATE,CHECKED_BY,CHECKED_DATE,AUTHORISED_BY,POSTED_DATE,MODIFY_DATE,SYN_ROWID,REFERENCE_NO,SESSION_ROWID,'' ITEM_EDESC,'' VEHICLE_NO,'' PARTY_NAME,'' ADDRESS,'' BILL_NO FROM MASTER_TRANSACTION WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N' UNION ALL
                //SELECT FORM_CODE, TRANSACTION_NO AS VOUCHER_NO,0 VOUCHER_AMOUNT,CREATED_DATE AS VOUCHER_DATE
                //,CREATED_BY,CREATED_DATE
                //,CHECKED_BY,CREATED_DATE AS CHECKED_DATE,AUTHORISED_BY,CREATED_DATE AS POSTED_DATE
                //,MODIFY_DATE,SYN_ROWID,REFERENCE_NO
                //,SESSION_ROWID,'' ITEM_EDESC,'' VEHICLE_NO,'' PARTY_NAME,'' ADDRESS,'' BILL_NO FROM QC_PARAMETER_TRANSACTION WHERE FORM_CODE ='{formCode}' 
                //and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N' 
                //UNION ALL
                //SELECT GPT.FORM_CODE, GPT.TRANSACTION_NO AS VOUCHER_NO,0 VOUCHER_AMOUNT,GPT.CREATED_DATE AS VOUCHER_DATE
                //,GPT.CREATED_BY,CREATED_DATE
                //,GPT.CHECKED_BY,GPT.CREATED_DATE AS CHECKED_DATE,GPT.AUTHORISED_BY,GPT.CREATED_DATE AS POSTED_DATE
                //,GPT.MODIFY_DATE,GPT.SYN_ROWID,GPT.REFERENCE_NO
                //,GPT.SESSION_ROWID,(SELECT DISTINCT INITCAP(item_edesc) as item_edesc FROM ip_item_master_setup WHERE 
                //DELETED_FLAG='N' AND ITEM_CODE = GPT.ITEM_CODE AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}') AS ITEM_EDESC
                //,GPT.VEHICLE_NO,GPT.PARTY_NAME,GPT.ADDRESS,GPT.BILL_NO FROM GLOBAL_PRODUCTS_TRANSACTION GPT WHERE GPT.FORM_CODE ='{formCode}' 
                //and GPT.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND GPT.DELETED_FLAG='N' 
                //ORDER BY VOUCHER_NO DESC";
                var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentSubMenu> GetMaseterTransDetailByFormCodeVer(string formCode, string docVer)
        {
            try
            {
                if (docVer == "Check")
                {
                    string query = $@"  SELECT F.MODULE_CODE, M.FORM_CODE, M.VOUCHER_NO,M.VOUCHER_AMOUNT,M.VOUCHER_DATE,M.CREATED_BY,M.CREATED_DATE,M.CHECKED_BY,M.CHECKED_DATE,M.AUTHORISED_BY,M.POSTED_DATE,M.MODIFY_DATE,
                                          M.SYN_ROWID,M.REFERENCE_NO,M.SESSION_ROWID FROM MASTER_TRANSACTION
                                          M , FORM_SETUP F WHERE M.FORM_CODE=F.FORM_CODE
                                         AND  M.FORM_CODE ='{formCode}' AND m.company_code=f.company_code and M.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND M.CHECKED_BY IS NULL AND M.CHECKED_DATE IS NULL AND M.AUTHORISED_BY IS NULL AND M. AUTHORISED_DATE IS NULL AND M.POSTED_BY IS NULL AND M.POSTED_DATE IS NULL ORDER BY M.VOUCHER_NO DESC
                                          ";
                    var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                    return result;
                }
                if (docVer == "UnCheck")
                {
                    string query = $@"  SELECT F.MODULE_CODE,M.FORM_CODE, M.VOUCHER_NO,M.VOUCHER_AMOUNT,M.VOUCHER_DATE,M.CREATED_BY,M.CREATED_DATE,M.CHECKED_BY,M.CHECKED_DATE,M.AUTHORISED_BY,M.POSTED_DATE,M.MODIFY_DATE,
                                          M.SYN_ROWID,M.REFERENCE_NO,M.SESSION_ROWID FROM MASTER_TRANSACTION
                                          M , FORM_SETUP F WHERE M.FORM_CODE=F.FORM_CODE
                                         AND  M.FORM_CODE ='{formCode}' AND m.company_code=f.company_code and M.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND M.CHECKED_BY IS NOT NULL AND M.CHECKED_DATE IS NOT NULL AND M.AUTHORISED_BY IS  NULL AND M. AUTHORISED_DATE IS  NULL AND M.POSTED_BY IS  NULL AND M.POSTED_DATE IS  NULL ORDER BY M.VOUCHER_NO DESC
                                          ";
                    var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                    return result;
                }
                else if (docVer == "Authorise")
                {
                    string query = $@"  SELECT F.MODULE_CODE,M.FORM_CODE, M.VOUCHER_NO,M.VOUCHER_AMOUNT,M.VOUCHER_DATE,M.CREATED_BY,M.CREATED_DATE,M.CHECKED_BY,M.CHECKED_DATE,M.AUTHORISED_BY,M.POSTED_DATE,M.MODIFY_DATE,
                                          M.SYN_ROWID,M.REFERENCE_NO,M.SESSION_ROWID FROM MASTER_TRANSACTION
                                          M , FORM_SETUP F WHERE M.FORM_CODE=F.FORM_CODE AND M.FORM_CODE ='{formCode}' AND m.company_code=f.company_code and M.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND M.CHECKED_BY IS NOT NULL AND M.CHECKED_DATE IS NOT NULL AND M.AUTHORISED_BY IS NULL AND M.AUTHORISED_DATE IS NULL AND M.POSTED_BY IS NULL AND M.POSTED_DATE IS NULL ORDER BY M.VOUCHER_NO DESC";
                    var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                    return result;
                }
                else if (docVer == "UnAuthorise")
                {
                    string query = $@"  SELECT F.MODULE_CODE,M.FORM_CODE, M.VOUCHER_NO,M.VOUCHER_AMOUNT,M.VOUCHER_DATE,M.CREATED_BY,M.CREATED_DATE,M.CHECKED_BY,M.CHECKED_DATE,M.AUTHORISED_BY,M.POSTED_DATE,M.MODIFY_DATE,
                                          M.SYN_ROWID,M.REFERENCE_NO,M.SESSION_ROWID FROM MASTER_TRANSACTION
                                          M , FORM_SETUP F WHERE M.FORM_CODE=F.FORM_CODE AND M.FORM_CODE ='{formCode}' AND m.company_code=f.company_code and M.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND M.CHECKED_BY IS NOT NULL AND M.CHECKED_DATE IS NOT NULL AND M.AUTHORISED_BY IS NOT NULL AND M.AUTHORISED_DATE IS NOT NULL AND M.POSTED_BY IS NULL AND M.POSTED_DATE IS NULL ORDER BY M.VOUCHER_NO DESC";
                    var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                    return result;
                }
                else if (docVer == "Post")
                {
                    string query = $@"  SELECT F.MODULE_CODE,M.FORM_CODE, M.VOUCHER_NO,M.VOUCHER_AMOUNT,M.VOUCHER_DATE,M.CREATED_BY,M.CREATED_DATE,M.CHECKED_BY,M.CHECKED_DATE,M.AUTHORISED_BY,M.POSTED_DATE,M.MODIFY_DATE,
                                          M.SYN_ROWID,M.REFERENCE_NO,M.SESSION_ROWID FROM MASTER_TRANSACTION
                                          M , FORM_SETUP F WHERE M.FORM_CODE=F.FORM_CODE AND M.FORM_CODE ='{formCode}' AND m.company_code=f.company_code and M.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND M.CHECKED_BY IS NOT NULL AND M.CHECKED_DATE IS NOT NULL AND M.AUTHORISED_BY IS NOT NULL AND M.AUTHORISED_DATE IS NOT NULL AND M.POSTED_BY IS NULL AND M.POSTED_DATE IS NULL ORDER BY M.VOUCHER_NO DESC";
                    var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                    return result;
                }
                else if (docVer == "UnPost")
                {
                    string query = $@"  SELECT F.MODULE_CODE,M.FORM_CODE, M.VOUCHER_NO,M.VOUCHER_AMOUNT,M.VOUCHER_DATE,M.CREATED_BY,M.CREATED_DATE,M.CHECKED_BY,M.CHECKED_DATE,M.AUTHORISED_BY,M.POSTED_DATE,M.MODIFY_DATE,
                                          M.SYN_ROWID,M.REFERENCE_NO,M.SESSION_ROWID FROM MASTER_TRANSACTION
                                          M , FORM_SETUP F WHERE M.FORM_CODE=F.FORM_CODE AND M.FORM_CODE ='{formCode}' AND m.company_code=f.company_code and M.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND M.CHECKED_BY IS NOT NULL AND M.CHECKED_DATE IS NOT NULL AND M.AUTHORISED_BY IS NOT NULL AND M.AUTHORISED_DATE IS NOT NULL AND M.POSTED_BY IS  NOT NULL AND M.POSTED_DATE IS NOT NULL ORDER BY M.VOUCHER_NO DESC";
                    var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                    return result;
                }
                else
                {
                    string query = $@"  SELECT F.MODULE_CODE,M.FORM_CODE, M.VOUCHER_NO,M.VOUCHER_AMOUNT,M.VOUCHER_DATE,M.CREATED_BY,M.CREATED_DATE,M.CHECKED_BY,M.CHECKED_DATE,M.AUTHORISED_BY,M.POSTED_DATE,M.MODIFY_DATE,
                                          M.SYN_ROWID,M.REFERENCE_NO,M.SESSION_ROWID FROM MASTER_TRANSACTION
                                          M , FORM_SETUP F WHERE M.FORM_CODE=F.FORM_CODE AND M.FORM_CODE ='{formCode}' AND m.company_code=f.company_code and M.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' ORDER BY M.VOUCHER_NO DESC";
                    var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentSubMenu> GetDraftDetails(string formCode)
        {
            try
            {
                string query = $@"SELECT FORM_CODE, VOUCHER_NO,VOUCHER_AMOUNT,VOUCHER_DATE,CREATED_BY,CREATED_DATE,
                                  CHECKED_BY,CHECKED_DATE,AUTHORISED_BY,POSTED_DATE,MODIFY_DATE,SYN_ROWID
                                  FROM MASTER_TRANSACTION WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' ORDER BY VOUCHER_NO DESC";
                var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<FormDetailSetup> GetDistFormTransDetailByModuleCode(string modulecode)
        {
            //            try
            //            {
            //                //string query = $@"SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME FROM FORM_SETUP FS INNER JOIN MASTER_TRANSACTION MT ON MT.FORM_CODE=FS.FORM_CODE  AND MT.COMPANY_CODE=FS.COMPANY_CODE INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
            //                // WHERE FS.MODULE_CODE='{modulecode}' AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' ORDER BY TO_NUMBER(FORM_CODE) ASC";


            //                //string query = $@"SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME FROM FORM_SETUP FS INNER JOIN MASTER_TRANSACTION MT ON MT.FORM_CODE=FS.FORM_CODE  AND MT.COMPANY_CODE=FS.COMPANY_CODE INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
            //                //  WHERE FS.MODULE_CODE='{modulecode}' AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' ORDER BY TO_NUMBER(FORM_CODE) ASC";

            //                //                string query = $@"select * from (SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME FROM FORM_SETUP FS INNER JOIN MASTER_TRANSACTION MT ON MT.FORM_CODE=FS.FORM_CODE  AND MT.COMPANY_CODE=FS.COMPANY_CODE INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
            //                //                  WHERE FS.MODULE_CODE='{modulecode}' AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' UNION ALL
            //                // SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME FROM FORM_SETUP FS 
            //                //INNER JOIN QC_PARAMETER_TRANSACTION MT 
            //                //ON MT.FORM_CODE=FS.FORM_CODE  AND MT.COMPANY_CODE=FS.COMPANY_CODE INNER JOIN FORM_DETAIL_SETUP FDS 
            //                //ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
            //                //WHERE FS.MODULE_CODE='{modulecode}' AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' )
            //                // ORDER BY TO_NUMBER(FORM_CODE) ASC";

            //                string query = $@"select * from (SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME FROM FORM_SETUP FS INNER JOIN MASTER_TRANSACTION MT ON MT.FORM_CODE=FS.FORM_CODE  AND MT.COMPANY_CODE=FS.COMPANY_CODE INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
            //                  WHERE FS.MODULE_CODE='{modulecode}' AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' UNION ALL
            // SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME FROM FORM_SETUP FS 
            //INNER JOIN QC_PARAMETER_TRANSACTION MT 
            //ON MT.FORM_CODE=FS.FORM_CODE  AND MT.COMPANY_CODE=FS.COMPANY_CODE INNER JOIN FORM_DETAIL_SETUP FDS 
            //ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
            //WHERE FS.MODULE_CODE='{modulecode}' AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'
            //UNION ALL
            //SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME FROM FORM_SETUP FS 
            //INNER JOIN GLOBAL_PRODUCTS_TRANSACTION GPT 
            //ON GPT.FORM_CODE=FS.FORM_CODE  AND GPT.COMPANY_CODE=FS.COMPANY_CODE 
            //INNER JOIN FORM_DETAIL_SETUP FDS 
            //ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
            //WHERE FS.MODULE_CODE='{modulecode}' AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND GPT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' 
            //)
            // ORDER BY TO_NUMBER(FORM_CODE) ASC";

            //                var result = _dbContext.SqlQuery<FormDetailSetup>(query).ToList();
            //                return result;
            //            }

            try
            {
                string query = $@"
            SELECT * FROM (
                SELECT DISTINCT FS.FORM_CODE, FS.FORM_EDESC, FDS.TABLE_NAME 
                FROM FORM_SETUP FS 
                INNER JOIN MASTER_TRANSACTION MT 
                    ON MT.FORM_CODE = FS.FORM_CODE 
                    AND MT.COMPANY_CODE = FS.COMPANY_CODE 
                INNER JOIN FORM_DETAIL_SETUP FDS 
                    ON FDS.FORM_CODE = FS.FORM_CODE 
                    AND FDS.COMPANY_CODE = FS.COMPANY_CODE
                INNER JOIN SC_FORM_CONTROL FC
                    ON FC.FORM_CODE = FS.FORM_CODE
                    AND FC.COMPANY_CODE = FS.COMPANY_CODE
                    AND FC.USER_NO = '{_workContext.CurrentUserinformation.User_id}'
                WHERE FS.MODULE_CODE = '{modulecode}' 
                    AND FS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                    AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                    AND (FC.READ_FLAG = 'Y' OR FC.CREATE_FLAG = 'Y' OR FC.UPDATE_FLAG = 'Y')
            )
            ORDER BY TO_NUMBER(FORM_CODE) ASC";
                var result = _dbContext.SqlQuery<FormDetailSetup>(query).ToList();
                return result;
            }

            catch (Exception)
            {
                throw;
            }
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
        public List<TemplateDraftModel> GetDraftListInventory()
        {
            try
            {
                string query = $@"SELECT FT.FORM_CODE,FT.TEMPLATE_NO TEMPLATE_CODE, FT.TEMPLATE_EDESC,  FS.MODULE_CODE FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='02' 
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
        public List<TemplateDraftModel> GetDraftListProduction()
        {
            try
            {
                string query = $@"SELECT FT.FORM_CODE,FT.TEMPLATE_NO TEMPLATE_CODE, FT.TEMPLATE_EDESC,  FS.MODULE_CODE FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='03' 
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
        public List<TemplateDraftModel> GetDraftListSales()
        {
            try
            {
                string query = $@"SELECT FT.FORM_CODE,FT.TEMPLATE_NO TEMPLATE_CODE, FT.TEMPLATE_EDESC,  FS.MODULE_CODE FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='04' 
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
        public List<TemplateDraftModel> GetDraftListFinance()
        {
            try
            {
                string query = $@"SELECT FT.FORM_CODE,FT.TEMPLATE_NO TEMPLATE_CODE, FT.TEMPLATE_EDESC,  FS.MODULE_CODE FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='01' 
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
        public string InsertSalesImage(DocumentTransaction documentdetail)
        {
            //var insertitem = $@"INSERT INTO DOCUMENT_TRANSACTION(VOUCHER_NO,VOUCHER_DATE,SERIAL_NO,FORM_CODE,DOCUMENT_NAME,DOCUMENT_FILE_NAME,COMPANY_CODE,BRANCH_CODE,                           CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID,SYN_ROWID)VALUES('{documentdetail.VOUCHER_NO}',TO_DATE('{documentdetail.VOUCHER_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'), '{documentdetail.SERIAL_NO}','{documentdetail.FORM_CODE}','{documentdetail.DOCUMENT_FILE_NAME}', '{documentdetail.DOCUMENT_NAME}','{_workContext.CurrentUserinformation.company_code}', '{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}','','')";

            //var deteteitemQUERY = $@"DELETE FROM DOCUMENT_TRANSACTION WHERE VOUCHER_NO='{documentdetail.VOUCHER_NO}' AND FORM_CODE='{documentdetail.FORM_CODE}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
            //var deteteitem = _dbContext.ExecuteSqlCommand(deteteitemQUERY);

            var insertitem = $@"INSERT INTO DOCUMENT_TRANSACTION(VOUCHER_NO,VOUCHER_DATE,SERIAL_NO,FORM_CODE,DOCUMENT_NAME,DOCUMENT_FILE_NAME,COMPANY_CODE,BRANCH_CODE,                           CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID)VALUES('{documentdetail.VOUCHER_NO}',TO_DATE('{documentdetail.VOUCHER_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'), '{documentdetail.SERIAL_NO}','{documentdetail.FORM_CODE}','{documentdetail.DOCUMENT_FILE_NAME}', '{documentdetail.DOCUMENT_NAME}','{_workContext.CurrentUserinformation.company_code}', '{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}','')";
            var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);
            return null;
        }
        public string DeleteUploadedFile(DropZoneFile model)
        {
            try
            {
                var deleteitemquery = $@"DELETE FROM DOCUMENT_TRANSACTION WHERE VOUCHER_NO='{model.VOUCHER_NO}' AND FORM_CODE='{model.FORM_CODE}' AND DOCUMENT_NAME='{model.FILE_NAME.Trim()}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
                var deleteitem = _dbContext.ExecuteSqlCommand(deleteitemquery);
                return "true";
            }
            catch (Exception)
            {

                return "false";
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
                                  WHERE CS.CHARGE_CODE = IC.CHARGE_CODE AND CS.COMPANY_CODE = IC.COMPANY_CODE AND  CS.FORM_CODE= '{formCode}' AND REFERENCE_NO = '{voucherNo}' AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND CS.APPLY_ON = 'D' 
                                  AND cs.serial_no IS NOT NULL ORDER BY CS.SERIAL_NO ASC";
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
        public List<ChargeOnSales> GetQuotationChargesData(string formCode, string voucherNo, string itemCode)
        {
            try
            {
                string query = string.Empty;
                var result = new List<ChargeOnSales>();
                if (voucherNo != "undefined")
                {
                    string query1 = $@"select count(*) as count from quotation_details where tender_no='{voucherNo}'";
                    int count = _dbContext.SqlQuery<int>(query1).FirstOrDefault();
                    if (count > 0)
                    {
                        query = $@"SELECT DISTINCT cs.charge_code, ic.charge_edesc,cs.charge_type_flag,cs.value_percent_flag,
                            CASE WHEN cs.charge_code = 'VT' THEN ROUND(qdi.vat_amount, 2)
                            WHEN cs.charge_code = 'ED' THEN ROUND(qdi.excise, 2)
                            WHEN cs.charge_code = 'DC' THEN ROUND(qdi.discount_amount, 2)
                             ELSE 0 END AS charge_amount,0 AS value_percent_amount,cs.priority_index_no,cs.acc_code,
                            cs.apply_on,cs.gl_flag,cs.non_gl_flag FROM
                            charge_setup cs,ip_charge_code ic,ip_quotation_inquiry iqi, quotation_details qd,quotation_detail_itemwise qdi 
                            WHERE cs.form_code = iqi.form_code and cs.charge_code = ic.charge_code AND cs.company_code = ic.company_code and qd.quotation_no = qdi.quotation_no AND iqi.item_code = qdi.item_code and 
                             cs.form_code = '{formCode}' AND qd.tender_no = '{voucherNo}' AND IQI.ITEM_CODE in ({itemCode}) AND cs.company_code = '{_workContext.CurrentUserinformation.company_code}'   AND qd.status = 'AP'
                             and qd.tender_no = iqi.quote_no  AND cs.apply_on = 'D' AND cs.charge_code IN ('VT', 'ED', 'DC')
                             AND cs.priority_index_no IS NOT NULL ORDER BY cs.priority_index_no ASC";
                        _logErp.InfoInFile("Query to get charges on sales data contains:  " + query);
                        result = _dbContext.SqlQuery<ChargeOnSales>(query).ToList();
                    }
                    else
                    {
                        result = GetChargesData(formCode, voucherNo);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting charges data : " + ex.Message);
                throw ex;
            }
        }
        public string GetItemNonVatFlagByItemCode(string itemCode)
        {
            try
            {
                string result = "";
                string query = string.Empty;
                if (itemCode != "undefined")
                {
                    query = $@" SELECT NON_VAT_FLAG FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = {itemCode} AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG = 'N'";
                    _logErp.InfoInFile("Query to get non vat flag contains:  " + query);
                    result = _dbContext.SqlQuery<string>(query).First();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting non vat flag : " + ex.Message);
                throw ex;
            }
        }

        public string GetItemWiseAttribute(string itemCode, string serialNo, string voucherNo, string tableName)
        {
            var primarycolname = GetPrimaryColumnByTableName(tableName);
            var checkAttributeFlag = _attributeSetup.GetAttributeFlagFromCache();
            try
            {
                string result = "";
                string query = string.Empty;
                if (voucherNo != "undefined" && checkAttributeFlag == "Y")
                {
                    query = $@" SELECT A.ATTRIBUTE_EDESC FROM {tableName} SO  
                                LEFT JOIN IP_ITEM_ATTRIBUTE_SETUP A
                                 ON SO.ATTRIBUTE_CODE = A.ATTRIBUTE_CODE
                                 AND A.DELETED_FLAG = 'N' WHERE SO.{primarycolname} = '{voucherNo}' AND SO.ITEM_CODE = '{itemCode}' AND SO.SERIAL_NO = '{serialNo}'  AND SO.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND SO.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND SO.DELETED_FLAG = 'N'";
                    _logErp.InfoInFile("Query to get attribute wise item contains:  " + query);
                    result = _dbContext.SqlQuery<string>(query).First();
                }
                else
                {
                    result = "";
                }
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting attribute wise item : " + ex.Message);
                throw ex;
            }
        }


        //subin changes due to APPLY_FROM_DATE AND APPLY_TO_DATE problem on filsca year end
        //public List<ChargeOnSales> GetChargesData(string formCode)
        //{
        //    try
        //    {
        //        string query = string.Empty;
        //        var result = new List<ChargeOnSales>();
        //        query = $@"SELECT DISTINCT CS.CHARGE_CODE, IC.CHARGE_EDESC, CS.CHARGE_TYPE_FLAG, CS.VALUE_PERCENT_FLAG,0 CHARGE_AMOUNT, CS.VALUE_PERCENT_AMOUNT, CS.PRIORITY_INDEX_NO,CA.ACC_CODE, CA.ACC_EDESC,CS.GL_FLAG,CS.NON_GL_FLAG,
        //                          CS.CHARGE_APPLY_ON, CS.APPLY_FROM_DATE,CS.APPLY_TO_DATE,  CS.APPLY_ON FROM CHARGE_SETUP CS, IP_CHARGE_CODE IC ,FA_CHART_OF_ACCOUNTS_SETUP CA
        //                          WHERE  CS.CHARGE_CODE = IC.CHARGE_CODE AND CS.COMPANY_CODE = IC.COMPANY_CODE  AND CS.ACC_CODE=CA.ACC_CODE(+) AND CS.COMPANY_CODE=CA.COMPANY_CODE(+) AND  CS.FORM_CODE= '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND SYSDATE BETWEEN CS.APPLY_FROM_DATE AND CS.APPLY_TO_DATE AND CS.APPLY_ON = 'D' ORDER BY CS.PRIORITY_INDEX_NO ASC";
        //        _logErp.InfoInFile("Query to get charge data for given formcode include : " + query);
        //        result = _dbContext.SqlQuery<ChargeOnSales>(query).ToList();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logErp.ErrorInDB("Error while getting charge for sales : " + ex.Message);
        //        throw ex;
        //    }
        //}


        public List<ChargeOnSales> GetChargesData(string formCode)
        {
            try
            {
                string query = string.Empty;
                var result = new List<ChargeOnSales>();
                ///Nila Before Line Wise Charge Calculation code removed CS.APPLY_ON = 'D' starts ------///////////
                ///
                //query = $@"SELECT DISTINCT CS.CHARGE_CODE, IC.CHARGE_EDESC, CS.CHARGE_TYPE_FLAG, CS.VALUE_PERCENT_FLAG,0 CHARGE_AMOUNT, CS.VALUE_PERCENT_AMOUNT, CS.PRIORITY_INDEX_NO,CA.ACC_CODE, CA.ACC_EDESC,CS.GL_FLAG,CS.NON_GL_FLAG,
                //                  CS.CHARGE_APPLY_ON, CS.APPLY_FROM_DATE,CS.APPLY_TO_DATE,  CS.APPLY_ON,  CS.charge_active_flag, CS.HIDE_PER_FLAG, CS.MANUAL_CALC_CHARGE  FROM CHARGE_SETUP CS, IP_CHARGE_CODE IC ,FA_CHART_OF_ACCOUNTS_SETUP CA
                //                  WHERE  CS.CHARGE_CODE = IC.CHARGE_CODE AND CS.COMPANY_CODE = IC.COMPANY_CODE  AND CS.ACC_CODE=CA.ACC_CODE(+) AND CS.COMPANY_CODE=CA.COMPANY_CODE(+) AND  CS.FORM_CODE= '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND CS.APPLY_ON = 'D' AND CS.DELETED_FLAG = 'N' ORDER BY CS.PRIORITY_INDEX_NO ASC";

                ///Nila Before Line Wise Charge Calculation code ends------///////////
                ///
                query = $@"SELECT DISTINCT CS.CHARGE_CODE, IC.CHARGE_EDESC,IC.SPECIFIC_CHARGE_FLAG, CS.CHARGE_TYPE_FLAG, CS.VALUE_PERCENT_FLAG,0 CHARGE_AMOUNT, CS.VALUE_PERCENT_AMOUNT, CS.PRIORITY_INDEX_NO,CA.ACC_CODE, CA.ACC_EDESC,CS.GL_FLAG,CS.NON_GL_FLAG,
                                  CS.CHARGE_APPLY_ON, CS.APPLY_FROM_DATE,CS.APPLY_TO_DATE,  CS.APPLY_ON,  CS.charge_active_flag, CS.CHARGE_RATE_FLAG, CS.HIDE_PER_FLAG, CS.MANUAL_CALC_CHARGE  FROM CHARGE_SETUP CS, IP_CHARGE_CODE IC ,FA_CHART_OF_ACCOUNTS_SETUP CA
                                  WHERE  CS.CHARGE_CODE = IC.CHARGE_CODE AND CS.COMPANY_CODE = IC.COMPANY_CODE  AND CS.ACC_CODE=CA.ACC_CODE(+) AND CS.COMPANY_CODE=CA.COMPANY_CODE(+) AND  CS.FORM_CODE= '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND CS.DELETED_FLAG = 'N' AND CS.APPLY_ON = 'D'  ORDER BY CS.PRIORITY_INDEX_NO ASC";
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

        public List<ChargeOnSales> GetInvChargesDataSavedvaluewise(string voucherNo, string itemcode)
        {
            try
            {
                string query = string.Empty;
                var result = new List<ChargeOnSales>();
                query = $@"SELECT DISTINCT CS.CHARGE_CODE,
                  IC.CHARGE_EDESC,
                  CS.SUB_CODE,
                   CS.BUDGET_CODE,
                   CS.GL_FLAG as GL,
                  CS.CHARGE_TYPE_FLAG as CHARGE_TYPE,
                  CS.APPORTION_FLAG,
                  CS.IMPACT_ON,
                  CS.CALCULATE_BY VALUE_PERCENT_FLAG,
                  ROUND (CS.CHARGE_AMOUNT, 2) CALC ,
                   VALUE_PERCENT_AMOUNT,
                  CS.SERIAL_NO PRIORITY_INDEX_NO,
                  CS.ACC_CODE,
                  CS.APPLY_ON
    FROM CHARGE_TRANSACTION CS, IP_CHARGE_CODE IC
   WHERE     CS.CHARGE_CODE = IC.CHARGE_CODE
         AND CS.COMPANY_CODE = IC.COMPANY_CODE
         AND CS.REFERENCE_NO = '{voucherNo}'
         AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
         AND CS.ITEM_CODE = '{itemcode}'
         AND CS.APPLY_ON = 'I'
ORDER BY CS.SERIAL_NO ASC";
                result = _dbContext.SqlQuery<ChargeOnSales>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ChargeOnSales> GetInvChargesDataSavedQuantityWise(string voucherNo, string itemcode)
        {
            try
            {
                string query = string.Empty;
                var result = new List<ChargeOnSales>();
                query = $@"SELECT DISTINCT CS.CHARGE_CODE,
                  IC.CHARGE_EDESC,
                  CS.SUB_CODE,
                   CS.BUDGET_CODE,
                   CS.GL_FLAG as GL,
                  CS.CHARGE_TYPE_FLAG as CHARGE_TYPE,
                  CS.APPORTION_FLAG,
                  CS.IMPACT_ON,
                  CS.CALCULATE_BY VALUE_PERCENT_FLAG,
                  ROUND (CS.CHARGE_AMOUNT, 2) CALC ,
                   VALUE_PERCENT_AMOUNT,
                  CS.SERIAL_NO PRIORITY_INDEX_NO,
                  CS.ACC_CODE,
                  CS.APPLY_ON
    FROM CHARGE_TRANSACTION CS, IP_CHARGE_CODE IC
   WHERE     CS.CHARGE_CODE = IC.CHARGE_CODE
         AND CS.COMPANY_CODE = IC.COMPANY_CODE
         AND CS.REFERENCE_NO = '{voucherNo}'
         AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
         AND CS.ITEM_CODE = '{itemcode}'
         AND CS.APPLY_ON = 'I'
ORDER BY CS.SERIAL_NO ASC";
                result = _dbContext.SqlQuery<ChargeOnSales>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public List<ChargeOnItem> GetInvItemChargesData(string formCode,string itemCode)
        //{
        //    try
        //    {
        //        var companyCode = _workContext.CurrentUserinformation.company_code;
        //        string query = string.Empty;
        //        string querystring = string.Empty;
        //        var result = new List<ChargeOnItem>();
        //        query = $@"   SELECT DISTINCT  ICS.CHARGE_CODE, IC.CHARGE_EDESC,ICS.CHARGE_TYPE,ICS.IMPACT_ON,ICS.APPLY_QUANTITY,ICS.ACC_CODE,ICS.SUB_CODE,ICS.VALUE_PERCENT_AMOUNT,ICS.VALUE_PERCENT_FLAG FROM IP_ITEM_CHARGE_SETUP ICS, IP_CHARGE_CODE IC 
        //         where ICS.CHARGE_CODE=IC.CHARGE_CODE AND ICS.COMPANY_CODE = IC.COMPANY_CODE 
        //         AND ICS.COMPANY_CODE='{companyCode}' 
        //         AND ICS.ITEM_CODE='{itemCode}' 
        //         AND ICS.FORM_CODE='{formCode}'
        //         AND ICS.DELETED_FLAG='N' 
        //         AND ICS.VALUE_QUANTITY_BASED='V' 
        //         ORDER BY ICS.CHARGE_TYPE,ICS.CHARGE_CODE";
        //        result = _dbContext.SqlQuery<ChargeOnItem>(query).ToList();
        //        querystring = $@"   SELECT DISTINCT  ICS.CHARGE_CODE, IC.CHARGE_EDESC,ICS.CHARGE_TYPE,ICS.IMPACT_ON,ICS.APPLY_QUANTITY,ICS.ACC_CODE,ICS.SUB_CODE,ICS.VALUE_PERCENT_AMOUNT,ICS.VALUE_PERCENT_FLAG FROM IP_ITEM_CHARGE_SETUP ICS, IP_CHARGE_CODE IC 
        //         where ICS.CHARGE_CODE=IC.CHARGE_CODE AND ICS.COMPANY_CODE = IC.COMPANY_CODE 
        //         AND ICS.COMPANY_CODE='{companyCode}' 
        //         AND ICS.ITEM_CODE='{itemCode}' 
        //         AND ICS.FORM_CODE='{formCode}'
        //         AND ICS.DELETED_FLAG='N' 
        //         AND ICS.VALUE_QUANTITY_BASED='Q' 
        //         ORDER BY ICS.CHARGE_TYPE,ICS.CHARGE_CODE";




        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public ChargeOnItem GetInvItemChargesData(string formCode, string itemCode)
        {
            try
            {
                var Record = new ChargeOnItem();
                if (this._cacheManager.IsSet($"GetInvItemChargesData_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.company_code}_{formCode}_{itemCode}"))
                {
                    var data = _cacheManager.Get<ChargeOnItem>($"GetInvItemChargesData_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{formCode}_{itemCode}");
                    Record = data;
                }
                else
                {
                    string query = string.Empty;
                    string querystring = string.Empty;
                    query = $@"   SELECT DISTINCT  ICS.CHARGE_CODE, IC.CHARGE_EDESC,ICS.CHARGE_TYPE,ICS.IMPACT_ON,ICS.APPLY_QUANTITY,ICS.ACC_CODE,ICS.SUB_CODE,ICS.VALUE_PERCENT_AMOUNT,ICS.VALUE_PERCENT_FLAG FROM IP_ITEM_CHARGE_SETUP ICS, IP_CHARGE_CODE IC 
                         where ICS.CHARGE_CODE=IC.CHARGE_CODE AND ICS.COMPANY_CODE = IC.COMPANY_CODE 
                         AND ICS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                         AND ICS.ITEM_CODE='{itemCode}' 
                         AND ICS.FORM_CODE='{formCode}'
                         AND ICS.DELETED_FLAG='N' 
                         AND ICS.VALUE_QUANTITY_BASED='V' 
                         ORDER BY ICS.CHARGE_TYPE,ICS.CHARGE_CODE";
                    Record.ChargeOnItemAmountWiseList = _dbContext.SqlQuery<ChargeOnItemAmountWise>(query).ToList();

                    querystring = $@"   SELECT DISTINCT  ICS.CHARGE_CODE, IC.CHARGE_EDESC,ICS.CHARGE_TYPE AS CHARGE_TYPE,ICS.IMPACT_ON,ICS.APPLY_QUANTITY,ICS.ACC_CODE,ICS.SUB_CODE,ICS.VALUE_PERCENT_AMOUNT,ICS.VALUE_PERCENT_FLAG FROM IP_ITEM_CHARGE_SETUP ICS, IP_CHARGE_CODE IC 
                         where ICS.CHARGE_CODE=IC.CHARGE_CODE AND ICS.COMPANY_CODE = IC.COMPANY_CODE 
                         AND ICS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                         AND ICS.ITEM_CODE='{itemCode}' 
                         AND ICS.FORM_CODE='{formCode}'
                         AND ICS.DELETED_FLAG='N' 
                         AND ICS.VALUE_QUANTITY_BASED='Q' 
                         ORDER BY ICS.CHARGE_TYPE,ICS.CHARGE_CODE";
                    Record.ChargeOnItemQuantityWiseList = _dbContext.SqlQuery<ChargeOnItemQuantityWise>(querystring).ToList();
                    this._cacheManager.Set($"GetInvItemChargesData_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{formCode}_{itemCode}", Record, 20);
                }
                return Record;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CustomerModels> getAllCustomer()
        {
            #region OLD Implementation
            //string query = $@"SELECT DISTINCT 
            //            INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
            //            INITCAP(CUSTOMER_NDESC) AS CUSTOMER_NDESC,
            //            CUSTOMER_CODE,
            //            PREFIX_TEXT AS CUSTOMER_PREFIX,
            //            GROUP_START_NO AS CUSTOMER_STARTID,
            //            CUSTOMER_FLAG,
            //            ACC_CODE,
            //            MASTER_CUSTOMER_CODE, 
            //            PRE_CUSTOMER_CODE,
            //            GROUP_SKU_FLAG,
            //            REMARKS
            //            FROM SA_CUSTOMER_SETUP 
            //            WHERE DELETED_FLAG = 'N'
            //            AND GROUP_SKU_FLAG = 'G'
            //            AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
            //            ORDER BY PRE_CUSTOMER_CODE";

            //var customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();

            //foreach (var customer in customerList)
            //{
            //    string cusQuery = $@"SELECT CUSTOMER_CODE FROM SA_CUSTOMER_SETUP where MASTER_CUSTOMER_CODE = (SELECT PRE_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE='{customer.CUSTOMER_CODE}' 
            //                    AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}')";
            //    customer.PARENT_CUSTOMER_CODE = this._dbContext.SqlQuery<string>(cusQuery).FirstOrDefault();
            //}
            //return customerList;
            #endregion

            try
            {
                string query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        INITCAP(CUSTOMER_NDESC) AS CUSTOMER_NDESC,
                        CUSTOMER_CODE,
                        PREFIX_TEXT AS CUSTOMER_PREFIX,
                        GROUP_START_NO AS CUSTOMER_STARTID,
                        CUSTOMER_FLAG,
                        ACC_CODE,
                        PARTY_TYPE_CODE,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM SA_CUSTOMER_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        CONNECT BY NOCYCLE PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
                        ORDER BY PRE_CUSTOMER_CODE";

                var customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();

                return customerList;



            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting all customer : " + ex.StackTrace);
            }
        }


        public List<SupplierModels> getAllSupplier()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(SUPPLIER_EDESC) AS SUPPLIER_EDESC,
                        SUPPLIER_CODE ,
                        MASTER_SUPPLIER_CODE, 
                        PRE_SUPPLIER_CODE,
                        GROUP_SKU_FLAG,
                        ACC_CODE,
                        PARTY_TYPE_CODE
                        FROM IP_SUPPLIER_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE
                        ORDER BY PRE_SUPPLIER_CODE";
            var customerList = _dbContext.SqlQuery<SupplierModels>(query).ToList();
            return customerList;
        }
        public List<ProductsModels> getAllProduct()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ITEM_EDESC) AS ITEM_EDESC,
                        ITEM_CODE ,
                        MASTER_ITEM_CODE, 
                        PRE_ITEM_CODE,
                        GROUP_SKU_FLAG 
                        FROM ip_item_master_setup
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
                        ORDER BY PRE_ITEM_CODE";
            var productList = _dbContext.SqlQuery<ProductsModels>(query).ToList();
            return productList;
        }
        public List<TNCModels> getAllTNC()
        {
            string query = $@"    SELECT DISTINCT INITCAP (TNC_EDESC) AS TNC_EDESC,
                    TNC_CODE,
                    MASTER_TNC_CODE,
                    PRE_TNC_CODE,
                    GROUP_SKU_FLAG
      FROM TERMS_AND_CONDITION_SETUP
     WHERE DELETED_FLAG = 'N' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
CONNECT BY PRIOR MASTER_TNC_CODE = PRE_TNC_CODE
  ORDER BY PRE_TNC_CODE";
            var productList = _dbContext.SqlQuery<TNCModels>(query).ToList();
            return productList;
        }
        public List<LocationTypeModels> getAllLocationType()
        {
            try
            {
                string query = $@"SELECT * FROM IP_LOCATION_TYPE_CODE  
                     WHERE DELETED_FLAG = 'N'
                     AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                var list = _dbContext.SqlQuery<LocationTypeModels>(query).ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<LocationModels> getAllLocation()
        {
            string query = $@"SELECT DISTINCT
                     INITCAP(LOCATION_EDESC) as LOCATION_EDESC,
                     LOCATION_CODE,
                     GROUP_SKU_FLAG,
                     PRE_LOCATION_CODE
                     from IP_location_setup    
                     WHERE DELETED_FLAG = 'N'
                     AND GROUP_SKU_FLAG = 'G'
                     AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                     CONNECT BY PRIOR LOCATION_CODE = PRE_LOCATION_CODE
                     ORDER BY PRE_LOCATION_CODE
                     ";
            var locationList = _dbContext.SqlQuery<LocationModels>(query).ToList();
            return locationList;
        }
        public List<RegionalModels> getAllRegions()
        {
            string query = $@"SELECT DISTINCT
                     INITCAP(REGION_EDESC) as REGION_EDESC,
                     REGION_CODE,
                     GROUP_SKU_FLAG,
                     PRE_REGION_CODE
                     from SA_REGIONAL_SETUP    
                     WHERE DELETED_FLAG = 'N'
                     AND GROUP_SKU_FLAG = 'G'
                     AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                     CONNECT BY PRIOR REGION_CODE = PRE_REGION_CODE
                     ORDER BY PRE_REGION_CODE
                     ";
            var regionList = _dbContext.SqlQuery<RegionalModels>(query).ToList();
            return regionList;
        }

        public List<MiscellaneousSubLedgerModels> getAllMiscellaneousSubLedgers()
        {
            string query = $@"SELECT DISTINCT
                                INITCAP(MISC_EDESC) as MISC_EDESC,
                                MISC_CODE,
                                GROUP_SKU_FLAG,
                                PRE_MISC_CODE,
                                MASTER_MISC_CODE
                                FROM FA_MISC_SUBLEDGER_SETUP
                                WHERE DELETED_FLAG = 'N'
                                AND GROUP_SKU_FLAG = 'G'
                                AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                CONNECT BY PRIOR MISC_CODE = PRE_MISC_CODE
                                ORDER BY PRE_MISC_CODE
                                ";
            var miscellaneousSubLedgerList = _dbContext.SqlQuery<MiscellaneousSubLedgerModels>(query).ToList();
            return miscellaneousSubLedgerList;
        }
        public List<AttributeModels> getAllAttributes()
        {
            string query = $@"SELECT DISTINCT
                     INITCAP(ATTRIBUTE_EDESC) as ATTRIBUTE_EDESC,
                     ATTRIBUTE_CODE,
                     GROUP_SKU_FLAG,
                     PRE_ATTRIBUTE_CODE
                     from IP_ITEM_ATTRIBUTE_SETUP
                     WHERE DELETED_FLAG = 'N'
                     AND GROUP_SKU_FLAG = 'G'
                     AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                     CONNECT BY PRIOR ATTRIBUTE_CODE = PRE_ATTRIBUTE_CODE
                     ORDER BY PRE_ATTRIBUTE_CODE
                     ";
            var attributeList = _dbContext.SqlQuery<AttributeModels>(query).ToList();
            return attributeList;
        }
        public List<ResourceModels> getAllResource()
        {
            try
            {
                string query = $@"SELECT DISTINCT
                     INITCAP(RESOURCE_EDESC) as RESOURCE_EDESC,
                     RESOURCE_CODE,
                     GROUP_SKU_FLAG,
                     PRE_RESOURCE_CODE
                     from MP_RESOURCE_SETUP    
                     WHERE DELETED_FLAG = 'N'
                     AND GROUP_SKU_FLAG = 'G'
                     AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                     CONNECT BY PRIOR RESOURCE_CODE = PRE_RESOURCE_CODE
                     ORDER BY RESOURCE_EDESC";
                var List = _dbContext.SqlQuery<ResourceModels>(query).ToList();
                return List;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProcessModels> getAllProcess()
        {
            string query = $@"SELECT DISTINCT
                     INITCAP(PROCESS_EDESC) as PROCESS_EDESC,
                     PROCESS_CODE,
                     PROCESS_FLAG,
                     PRE_PROCESS_CODE
                     from MP_PROCESS_SETUP    
                     WHERE DELETED_FLAG = 'N'
                     AND( PROCESS_FLAG = 'P'
                     OR PROCESS_FLAG = 'C')
                     AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                     ORDER BY PROCESS_EDESC";
            var processList = _dbContext.SqlQuery<ProcessModels>(query).ToList();
            return processList;
        }

        public List<DealerModel> getAllDealer()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(PARTY_TYPE_EDESC) AS PARTY_TYPE_EDESC,
                        PARTY_TYPE_CODE ,
                        PRE_PARTY_CODE,
                        GROUP_SKU_FLAG ,
                        PRE_PARTY_CODE,
                        MASTER_PARTY_CODE
                        FROM IP_PARTY_TYPE_CODE 
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var DealerList = _dbContext.SqlQuery<DealerModel>(query).ToList();
            return DealerList;
        }


        public List<BranchModels> getAllBranch()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(BRANCH_EDESC) AS BRANCH_EDESC,
                        BRANCH_CODE ,
                        PRE_BRANCH_CODE,
                        GROUP_SKU_FLAG 
                        FROM FA_BRANCH_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        CONNECT BY PRIOR BRANCH_EDESC = PRE_BRANCH_CODE
                        ORDER BY PRE_BRANCH_CODE";
            var BranchList = _dbContext.SqlQuery<BranchModels>(query).ToList();
            return BranchList;
        }
        public List<BranchModels> getAllBranchforscheme()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(BRANCH_EDESC) AS BRANCH_EDESC,
                        BRANCH_CODE ,
                        PRE_BRANCH_CODE,
                        GROUP_SKU_FLAG 
                        FROM FA_BRANCH_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'I'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var BranchList = _dbContext.SqlQuery<BranchModels>(query).ToList();
            return BranchList;
        }
        public List<FormcodeList> getAllfr()
        {
            string query = $@"SELECT FORM_CODE, FORM_EDESC, REF_TABLE_NAME From form_setup";
            var formlst = _dbContext.SqlQuery<FormcodeList>(query).ToList();
            return formlst;
        }
        public string GetPrimaryColumnByTableName(string tablename)
        {
            var primarycolumn = "";
            if (tablename == "FA_SINGLE_VOUCHER" || tablename == "FA_DOUBLE_VOUCHER" || tablename == "FA_PAY_ORDER" || tablename == "FA_JOB_ORDER")
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
            else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY" || tablename == "IP_PRODUCTION_ISSUE" || tablename == "IP_RETURNABLE_GOODS_RETURN" || tablename == "IP_RETURNABLE_GOODS_ISSUE")
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
            else if (tablename == "SA_SALES_INVOICE_JEWEL")
            {
                primarycolumn = "SALES_NO";
            }
            else if (tablename == "SA_SALES_RETURN_JEWEL")
            {
                primarycolumn = "RETURN_NO";
            }
            else if (tablename == "SA_SALES_BLANKET")
            {
                primarycolumn = "BLANKET_NO";
            }
            else if (tablename == "FA_PAY_ORDER")
            {
                primarycolumn = "VOUCHER_NO";
            }
            else if (tablename == "FA_ADJUSTMENT_NOTE")
            {
                primarycolumn = "NOTE_NO";
            }
            else if (tablename == "IP_QC_ENTRY")
            {
                primarycolumn = "ENTRY_NO";
            }
            else if (tablename == "IP_GATE_ENTRY")
            {
                primarycolumn = "GATE_NO";
            }

            return primarycolumn;
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
            else if (tablename == "IP_QC_ENTRY")
            {
                primarycolumn = "ENTRY_DATE";
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
            else if (tablename == "SA_SALES_INVOICE_JEWEL")
            {
                primarycolumn = "SALES_DATE";
            }
            else if (tablename == "FA_ADJUSTMENT_NOTE")
            {
                primarycolumn = "NOTE_DATE";
            }

            return primarycolumn;
        }
        public int? DocumentIfExists(string voucherno, string formcode)
        {
            string query = $@"SELECT MAX(SERIAL_NO) FROM DOCUMENT_TRANSACTION WHERE VOUCHER_NO = '{voucherno}' AND FORM_CODE = '{formcode}' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

            int? result = _dbContext.SqlQuery<int?>(query).FirstOrDefault();
            return result;
        }
        public List<SupplierModels> GetSupplierListBySupplierCode(string suppliercode, string supplierMasterCode, string searchText)
        {
            try
            {
                var rslt = new List<SupplierModels>();
                if (this._cacheManager.IsSet($"GetSupplierListBySupplierCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{suppliercode}_{supplierMasterCode}_{searchText}"))
                {
                    var data = _cacheManager.Get<List<SupplierModels>>($"GetSupplierListBySupplierCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{suppliercode}_{supplierMasterCode}_{searchText}");
                    rslt = data;
                }
                else
                {
                    if (String.IsNullOrEmpty(searchText))
                    {
                        if (supplierMasterCode == "undefined")
                        {
                            supplierMasterCode = "";
                        }
                        string query = $@"SELECT SUPPLIER_CODE,SUPPLIER_EDESC,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1 FROM IP_SUPPLIER_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND MASTER_SUPPLIER_CODE LIKE '{supplierMasterCode}%' AND GROUP_SKU_FLAG = 'I' AND DELETED_FLAG='N'  ORDER BY SUPPLIER_CODE ASC";
                        rslt = _dbContext.SqlQuery<SupplierModels>(query).ToList();
                        this._cacheManager.Set($"GetSupplierListBySupplierCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{suppliercode}_{supplierMasterCode}_{searchText}", rslt, 20);
                    }
                    else
                    {
                        string query = $@"SELECT DISTINCT SUPPLIER_CODE,SUPPLIER_EDESC,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1 FROM IP_SUPPLIER_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(SUPPLIER_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(REGD_OFFICE_EADDRESS)) LIKE '%{searchText.ToUpper()}%' OR (UPPER(TEL_MOBILE_NO1)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I'  AND DELETED_FLAG='N' ORDER BY SUPPLIER_CODE ASC";
                        rslt = _dbContext.SqlQuery<SupplierModels>(query).ToList();
                        this._cacheManager.Set($"GetSupplierListBySupplierCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{suppliercode}_{supplierMasterCode}_{searchText}", rslt, 20);
                    }

                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CustomerModels> GetCustomerListByCustomerCode(string customercode, string customerMasterCode, string searchText)
        {
            try
            {
                var rslt = new List<CustomerModels>();
                if (this._cacheManager.IsSet($"GetCustomerListByCustomerCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{customercode}_{customerMasterCode}_{searchText}"))
                {
                    var data = _cacheManager.Get<List<CustomerModels>>($"GetCustomerListByCustomerCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{customercode}_{customerMasterCode}_{searchText}");
                    rslt = data;
                }
                else
                {
                    if (String.IsNullOrEmpty(searchText))
                    {
                        if (customerMasterCode == "undefined")
                        {
                            customerMasterCode = "";
                        }
                        //string query = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,CREATED_BY,CREATED_DATE FROM SA_CUSTOMER_SETUP WHERE 
                        //    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND MASTER_CUSTOMER_CODE LIKE '{customerMasterCode}%' AND GROUP_SKU_FLAG = 'I' AND APPROVED_FLAG='Y'  ORDER BY CUSTOMER_CODE ASC";
                        string query = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,TPIN_VAT_NO,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,CREATED_BY,CREATED_DATE FROM SA_CUSTOMER_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND MASTER_CUSTOMER_CODE LIKE '{customerMasterCode}%' AND GROUP_SKU_FLAG = 'I'  AND DELETED_FLAG='N'   ORDER BY CUSTOMER_CODE ASC";

                        rslt = _dbContext.SqlQuery<CustomerModels>(query).ToList();
                        this._cacheManager.Set($"GetCustomerListByCustomerCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{customercode}_{customerMasterCode}_{searchText}", rslt, 20);
                    }
                    else
                    {
                        //string query = $@"SELECT DISTINCT CUSTOMER_CODE,CUSTOMER_EDESC,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,CREATED_BY,CREATED_DATE FROM SA_CUSTOMER_SETUP WHERE 
                        //    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(CUSTOMER_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(REGD_OFFICE_EADDRESS)) LIKE '%{searchText.ToUpper()}%' OR (UPPER(TEL_MOBILE_NO1)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I' AND APPROVED_FLAG='Y'  ORDER BY CUSTOMER_CODE ASC";

                        string query = $@"SELECT DISTINCT CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,TPIN_VAT_NO,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,CREATED_BY,CREATED_DATE FROM SA_CUSTOMER_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(CUSTOMER_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(REGD_OFFICE_EADDRESS)) LIKE '%{searchText.ToUpper()}%' OR (UPPER(TEL_MOBILE_NO1)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I'  AND DELETED_FLAG='N'  ORDER BY CUSTOMER_CODE ASC";
                        rslt = _dbContext.SqlQuery<CustomerModels>(query).ToList();
                        this._cacheManager.Set($"GetCustomerListByCustomerCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{customercode}_{customerMasterCode}_{searchText}", rslt, 20);
                    }
                    ;
                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<CustomerModels> GetCustomerListByCustomerCodeMaster(string customerMasterCode, string searchText)
        {
            try
            {
                var rslt = new List<CustomerModels>();
                if (this._cacheManager.IsSet($"GetCustomerListByCustomerCodeMaster_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{customerMasterCode}_{searchText}"))
                {
                    var data = _cacheManager.Get<List<CustomerModels>>($"GetCustomerListByCustomerCodeMaster_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{customerMasterCode}_{searchText}");
                    rslt = data;
                }
                else
                {
                    if (String.IsNullOrEmpty(searchText))
                    {
                        if (customerMasterCode == "undefined")
                        {
                            customerMasterCode = "";
                        }
                        //string query = $@"SELECT CUSTOMER_CODE,CUSTOMER_EDESC,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,CREATED_BY,CREATED_DATE FROM SA_CUSTOMER_SETUP WHERE 
                        //    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND MASTER_CUSTOMER_CODE LIKE '{customerMasterCode}%' AND GROUP_SKU_FLAG = 'I' AND APPROVED_FLAG='Y'  ORDER BY CUSTOMER_CODE ASC";
                        string query = $@"SELECT CUSTOMER_CODE,MASTER_CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,TPIN_VAT_NO,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,CREATED_BY,CREATED_DATE FROM SA_CUSTOMER_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND PRE_CUSTOMER_CODE = '{customerMasterCode}' AND GROUP_SKU_FLAG = 'G'  AND DELETED_FLAG='N'   ORDER BY MASTER_CUSTOMER_CODE ASC";

                        rslt = _dbContext.SqlQuery<CustomerModels>(query).ToList();
                        this._cacheManager.Set($"GetCustomerListByCustomerCodeMaster_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{customerMasterCode}_{searchText}", rslt, 20);
                    }
                    else
                    {
                        //string query = $@"SELECT DISTINCT CUSTOMER_CODE,CUSTOMER_EDESC,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,CREATED_BY,CREATED_DATE FROM SA_CUSTOMER_SETUP WHERE 
                        //    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(CUSTOMER_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(REGD_OFFICE_EADDRESS)) LIKE '%{searchText.ToUpper()}%' OR (UPPER(TEL_MOBILE_NO1)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I' AND APPROVED_FLAG='Y'  ORDER BY CUSTOMER_CODE ASC";

                        string query = $@"SELECT DISTINCT CUSTOMER_CODE,MASTER_CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,TPIN_VAT_NO,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1,CREATED_BY,CREATED_DATE FROM SA_CUSTOMER_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(CUSTOMER_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(REGD_OFFICE_EADDRESS)) LIKE '%{searchText.ToUpper()}%' OR (UPPER(TEL_MOBILE_NO1)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'G'  AND DELETED_FLAG='N'  ORDER BY MASTER_CUSTOMER_CODE ASC";
                        rslt = _dbContext.SqlQuery<CustomerModels>(query).ToList();
                        this._cacheManager.Set($"GetCustomerListByCustomerCodeMaster_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{customerMasterCode}_{searchText}", rslt, 20);
                    }
                    ;
                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<PModels> GetProductListByItemCode(string itemcode, string itemMasterCode, string searchText)
        {
            try
            {
                var rslt = new List<PModels>();
                if (this._cacheManager.IsSet($"GetProductListByItemCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{itemcode}_{itemMasterCode}_{searchText}"))
                {
                    var data = _cacheManager.Get<List<PModels>>($"GetProductListByItemCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{itemcode}_{itemMasterCode}_{searchText}");
                    rslt = data;
                }
                else
                {
                    // Get ATTRIBUTE_FLAG from cache to avoid repeated database calls
                    var checkAttributeFlag = _attributeSetup.GetAttributeFlagFromCache();

                    if (checkAttributeFlag == "Y")
                    {
                        if (String.IsNullOrEmpty(searchText))
                        {
                            if (itemMasterCode == "undefined")
                            {
                                itemMasterCode = "";
                            }
                            string query = $@"SELECT Distinct
                            A.ITEM_CODE,
                            INITCAP(A.ITEM_EDESC) || ' ' || INITCAP(D.ATTRIBUTE_EDESC) AS ITEM_EDESC,
                            CASE 
                            WHEN A.ITEM_NDESC = ''
                            THEN
                            INITCAP(A.ITEM_NDESC) || ' ' || INITCAP(D.ATTRIBUTE_NDESC)
                            ELSE
                            A.ITEM_NDESC END AS ITEM_NDESC,
                            A.MASTER_ITEM_CODE,
                            A.PRE_ITEM_CODE,
                            A.GROUP_SKU_FLAG,
                            TO_CHAR(A.CREATED_DATE, 'DD/MM/YYYY hh24:mi:ss') AS CREATED_DATE,
                            A.CREATED_BY,
                            B.MU_EDESC,
                            TO_CHAR(ROUND(TO_CHAR(A.PURCHASE_PRICE),2)) AS PURCHASE_PRICE,
                            C.CATEGORY_EDESC,
                            D.ATTRIBUTE_CODE AS ATTRIBUTE_CODE
                        FROM IP_ITEM_MASTER_SETUP A
                        JOIN IP_MU_CODE B 
                            ON A.INDEX_MU_CODE = B.MU_CODE
                        LEFT JOIN IP_CATEGORY_CODE C 
                            ON A.CATEGORY_CODE = C.CATEGORY_CODE
                        LEFT JOIN IP_ITEM_ATTRIBUTE_MAP E
                            ON E.ITEM_CODE = A.ITEM_CODE
                            AND E.DELETED_FLAG = 'N'
                        LEFT JOIN IP_ITEM_ATTRIBUTE_SETUP D 
                            ON D.ATTRIBUTE_CODE = E.ATTRIBUTE_CODE
                            AND D.DELETED_FLAG = 'N'
                        WHERE A.DELETED_FLAG = 'N'
                          AND A.GROUP_SKU_FLAG = 'I'
                          AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                          AND A.MASTER_ITEM_CODE LIKE '{itemMasterCode}%'
                        ORDER BY A.ITEM_CODE ASC";
                            rslt = _dbContext.SqlQuery<PModels>(query).ToList();
                            this._cacheManager.Set($"GetProductListByItemCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{itemcode}_{itemMasterCode}_{searchText}", rslt, 20);
                        }
                        else
                        {
                            string query = $@"SELECT Distinct
                            A.ITEM_CODE,
                            INITCAP(A.ITEM_EDESC) || ' ' || INITCAP(D.ATTRIBUTE_EDESC) AS ITEM_EDESC,
                            CASE 
                            WHEN A.ITEM_NDESC = ''
                            THEN
                            INITCAP(A.ITEM_NDESC) || ' ' || INITCAP(D.ATTRIBUTE_NDESC)
                            ELSE
                            A.ITEM_NDESC END AS ITEM_NDESC,
                            A.MASTER_ITEM_CODE,
                            A.PRE_ITEM_CODE,
                            A.GROUP_SKU_FLAG,
                            TO_CHAR(A.CREATED_DATE, 'DD/MM/YYYY hh24:mi:ss') AS CREATED_DATE,
                            A.CREATED_BY,
                            B.MU_EDESC,
                            TO_CHAR(ROUND(TO_CHAR(A.PURCHASE_PRICE),2)) AS PURCHASE_PRICE,
                            C.CATEGORY_EDESC,
                            D.ATTRIBUTE_CODE AS ATTRIBUTE_CODE 
                        FROM IP_ITEM_MASTER_SETUP A
                        JOIN IP_MU_CODE B 
                            ON A.INDEX_MU_CODE = B.MU_CODE
                        LEFT JOIN IP_CATEGORY_CODE C 
                            ON A.CATEGORY_CODE = C.CATEGORY_CODE
                        LEFT JOIN IP_ITEM_ATTRIBUTE_MAP E
                            ON E.ITEM_CODE = A.ITEM_CODE
                            AND E.DELETED_FLAG = 'N'
                        LEFT JOIN IP_ITEM_ATTRIBUTE_SETUP D 
                            ON D.ATTRIBUTE_CODE = E.ATTRIBUTE_CODE
                            AND D.DELETED_FLAG = 'N'
                        WHERE A.DELETED_FLAG = 'N'
                          AND A.GROUP_SKU_FLAG = 'I'
                          AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                          AND A.MASTER_ITEM_CODE LIKE '{itemMasterCode}%'
                        ORDER BY A.ITEM_CODE ASC";
                            rslt = _dbContext.SqlQuery<PModels>(query).ToList();
                            this._cacheManager.Set($"GetProductListByItemCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{itemcode}_{itemMasterCode}_{searchText}", rslt, 20);
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(searchText))
                        {
                            if (itemMasterCode == "undefined")
                            {
                                itemMasterCode = "";
                            }
                            string query = $@" SELECT DISTINCT
                                INITCAP(A.ITEM_EDESC) AS ITEM_EDESC,
                                INITCAP(A.ITEM_NDESC)AS ITEM_NDESC,
                                A.ITEM_CODE AS ITEM_CODE,
                                A.MASTER_ITEM_CODE AS MASTER_ITEM_CODE,
                                A.PRE_ITEM_CODE AS PRE_ITEM_CODE,
                                A.GROUP_SKU_FLAG AS GROUP_SKU_FLAG,
                                TO_CHAR(A.CREATED_DATE,'DD/MM/YYYY hh24:mi:ss') AS CREATED_DATE,
                                A.CREATED_BY AS CREATED_BY,
                                B.MU_EDESC AS MU_EDESC,
                                TO_CHAR(ROUND(TO_CHAR(A.PURCHASE_PRICE),2)) AS PURCHASE_PRICE,
                                C.CATEGORY_EDESC AS CATEGORY_EDESC
                                FROM IP_ITEM_MASTER_SETUP A, IP_MU_CODE B, IP_CATEGORY_CODE C
                                WHERE A.DELETED_FLAG = 'N'
                                AND A.INDEX_MU_CODE=B.MU_CODE
                                AND A.CATEGORY_CODE(+)=C.CATEGORY_CODE 
                                AND A.GROUP_SKU_FLAG = 'I'
                                AND A.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                                AND A.MASTER_ITEM_CODE  like '{itemMasterCode}%'
                                ORDER BY ITEM_CODE ASC";
                            rslt = _dbContext.SqlQuery<PModels>(query).ToList();
                            this._cacheManager.Set($"GetProductListByItemCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{itemcode}_{itemMasterCode}_{searchText}", rslt, 20);
                        }
                        else
                        {
                            string query = $@" SELECT DISTINCT
                                INITCAP(A.ITEM_EDESC) AS ITEM_EDESC,
                                INITCAP(A.ITEM_NDESC)AS ITEM_NDESC,
                                A.ITEM_CODE AS ITEM_CODE,
                                A.MASTER_ITEM_CODE AS MASTER_ITEM_CODE,
                                A.PRE_ITEM_CODE AS PRE_ITEM_CODE,
                                A.GROUP_SKU_FLAG AS GROUP_SKU_FLAG,
                                TO_CHAR(A.CREATED_DATE,'DD/MM/YYYY hh24:mi:ss') AS CREATED_DATE,
                                A.CREATED_BY AS CREATED_BY,
                                B.MU_EDESC AS MU_EDESC,
                                TO_CHAR(ROUND(TO_CHAR(A.PURCHASE_PRICE),2)) AS PURCHASE_PRICE,
                                C.CATEGORY_EDESC AS CATEGORY_EDESC
                                FROM IP_ITEM_MASTER_SETUP A, IP_MU_CODE B, IP_CATEGORY_CODE C
                                WHERE A.DELETED_FLAG = 'N'
                                AND A.INDEX_MU_CODE=B.MU_CODE
                                AND A.CATEGORY_CODE(+)=C.CATEGORY_CODE 
                                AND A.GROUP_SKU_FLAG = 'I'
                                AND A.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                               AND A.MASTER_ITEM_CODE  like '{itemMasterCode}%'
                                ORDER BY ITEM_CODE ASC";
                            rslt = _dbContext.SqlQuery<PModels>(query).ToList();
                            this._cacheManager.Set($"GetProductListByItemCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{itemcode}_{itemMasterCode}_{searchText}", rslt, 20);
                        }
                    }
                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public List<LocationModels> GetLocationListByLocationCode(string locationId, string locationCode, string searchText)
        //{
        //    try
        //    {
        //        var rslt = new List<LocationModels>();
        //        //if (this._cacheManager.IsSet($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}"))
        //        //{
        //        //    var data = _cacheManager.Get<List<LocationModels>>($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}");
        //        //    rslt = data;
        //        //}
        //        //else
        //        //{
        //        if (String.IsNullOrEmpty(searchText))
        //        {
        //            if (locationCode == "undefined")
        //            {
        //                locationCode = "";
        //            }
        //            string query = $@"SELECT LOCATION_CODE,LOCATION_EDESC,EMAIL,TELEPHONE_MOBILE_NO,ADDRESS,CREATED_DATE,CREATED_BY FROM IP_location_setup  WHERE 
        //                    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND LOCATION_CODE LIKE '{locationCode}%' AND GROUP_SKU_FLAG = 'I' AND DELETED_FLAG='N' ORDER BY LOCATION_CODE ASC";
        //            rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
        //            this._cacheManager.Set($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}", rslt, 20);

        //        }
        //        else
        //        {
        //            string query = $@"SELECT DISTINCT LOCATION_CODE,LOCATION_EDESC,EMAIL,TELEPHONE_MOBILE_NO,ADDRESS,CREATED_DATE,CREATED_BY FROM IP_location_setup     WHERE 
        //                    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(LOCATION_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(EMAIL)) LIKE '%{searchText.ToUpper()}%' OR (UPPER(TELEPHONE_MOBILE_NO)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I' AND DELETED_FLAG='N'  ORDER BY LOCATION_CODE ASC";
        //            rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
        //            this._cacheManager.Set($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}", rslt, 20);

        //        }

        //        //}
        //        return rslt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        //public List<LocationModels> GetLocationListByLocationCode(string locationId, string locationCode, string searchText, string routineSearchText = "")
        //{
        //    try
        //    {
        //        var rslt = new List<LocationModels>();
        //        //if (this._cacheManager.IsSet($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}"))
        //        //{
        //        //    var data = _cacheManager.Get<List<LocationModels>>($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}");
        //        //    rslt = data;
        //        //}
        //        //else
        //        //{
        //        if (searchText == "\"undefined\"" || searchText == "undefined")
        //        {
        //            searchText = "";
        //        }


        //        if (!String.IsNullOrEmpty(searchText))
        //        {
        //            string query = $@"SELECT DISTINCT LOCATION_CODE,LOCATION_EDESC,EMAIL,TELEPHONE_MOBILE_NO,ADDRESS,CREATED_DATE,CREATED_BY FROM IP_location_setup     WHERE 
        //              COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(LOCATION_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(EMAIL)) LIKE '%{searchText.ToUpper()}%' OR (UPPER(TELEPHONE_MOBILE_NO)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I' AND DELETED_FLAG='N'  ORDER BY LOCATION_CODE ASC";
        //            rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
        //            this._cacheManager.Set($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}", rslt, 20);

        //        }
        //        else if (!String.IsNullOrEmpty(routineSearchText) && !String.IsNullOrWhiteSpace(routineSearchText))
        //        {
        //            string query = $@"  SELECT pre_location_code, location_code, location_edesc, CREATED_DATE
        //                          FROM ip_location_setup
        //                         WHERE     UPPER (location_EDESC) LIKE '{routineSearchText.ToUpper()}%'
        //                               AND group_sku_flag = 'I'
        //                               AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
        //                               AND DELETED_FLAG = 'N'
        //                      ORDER BY UPPER (location_EDESC)";
        //            rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
        //            //this._cacheManager.Set($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}", rslt, 20);
        //        }
        //        else
        //        {

        //            if (locationCode == "undefined")
        //            {
        //                locationCode = "";
        //            }

        //            string query = $@"SELECT pre_location_code, location_code, location_edesc,
        //                             EMAIL,
        //                                   TELEPHONE_MOBILE_NO,
        //                                   ADDRESS,
        //                                   CREATED_DATE,
        //                                   CREATED_BY
        //                              FROM ip_location_setup
        //                             WHERE  COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

        //            if (string.IsNullOrEmpty(locationCode) || string.IsNullOrWhiteSpace(locationCode))
        //            {
        //                query = query + " AND UPPER (location_EDESC) LIKE '%'";
        //            }
        //            else
        //            {
        //                query = query + $@"  AND LOCATION_CODE LIKE '{locationCode}%'";
        //            }

        //            query = query + " AND group_sku_flag = 'I' AND DELETED_FLAG = 'N' ORDER BY UPPER (location_EDESC)";

        //            rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
        //            this._cacheManager.Set($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}", rslt, 20);

        //        }

        //        //}
        //        return rslt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        public List<LocationModels> GetLocationListByLocationCode(string locationId, string locationCode, string searchText, string routineSearchText = "", string referenceNo = "")
        {
            try
            {
                var rslt = new List<LocationModels>();
                //if (this._cacheManager.IsSet($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}"))
                //{
                //    var data = _cacheManager.Get<List<LocationModels>>($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}");
                //    rslt = data;
                //}
                //else
                //{
                if (searchText == "\"undefined\"" || searchText == "undefined" || searchText == null)
                {
                    searchText = "";
                }


                if (!String.IsNullOrEmpty(searchText))
                {
                    string query = $@"SELECT DISTINCT LOCATION_CODE,LOCATION_EDESC,EMAIL,TELEPHONE_MOBILE_NO,ADDRESS,CREATED_DATE,CREATED_BY FROM IP_location_setup     WHERE 
                      COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(LOCATION_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(EMAIL)) LIKE '%{searchText.ToUpper()}%' OR (UPPER(TELEPHONE_MOBILE_NO)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I' AND DELETED_FLAG='N'  ORDER BY LOCATION_CODE ASC";
                    rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
                    this._cacheManager.Set($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}", rslt, 20);

                }
                else if (!String.IsNullOrEmpty(referenceNo) && !String.IsNullOrWhiteSpace(referenceNo))
                {
                    string itemCodeQuery = $@"SELECT ITEM_CODE
                                                FROM IP_PRODUCTION_MRR 
                                                    WHERE MRR_NO = '{referenceNo}'";

                    var item_code = _dbContext.SqlQuery<string>(itemCodeQuery).FirstOrDefault();
                    if (item_code == null)
                    {
                        item_code = "";
                    }

                    string query = $@"SELECT ls.pre_location_code,
                                                 ls.location_code,
                                                 ls.location_edesc,
                                                 ls.created_date
                                            FROM    ip_location_setup ls
                                                 INNER JOIN
                                                    (SELECT *
                                                       FROM MP_PROCESS_SETUP
                                                      WHERE PROCESS_CODE IN (SELECT Process_Code
                                                                               FROM MP_ROUTINE_INPUT_SETUP
                                                                              WHERE ITEM_CODE = '{item_code}')) mp
                                                 ON mp.location_code = ls.location_code
                                           WHERE     UPPER (ls.location_EDESC) LIKE '{routineSearchText}%'
                                                 AND ls.group_sku_flag = 'I'
                                                 AND ls.COMPANY_CODE =  '{_workContext.CurrentUserinformation.company_code}'
                                                 AND ls.DELETED_FLAG = 'N'
                                        ORDER BY UPPER (location_EDESC)
                                        ";
                    rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
                    return rslt;
                }
                else if (!String.IsNullOrEmpty(routineSearchText) && !String.IsNullOrWhiteSpace(routineSearchText))
                {
                    string query = $@"  SELECT pre_location_code, location_code, location_edesc, CREATED_DATE
                                  FROM ip_location_setup
                                 WHERE     UPPER (location_EDESC) LIKE '{routineSearchText.ToUpper()}%'
                                       AND group_sku_flag = 'I'
                                       AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                       AND DELETED_FLAG = 'N'
                              ORDER BY UPPER (location_EDESC)";
                    rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
                    //this._cacheManager.Set($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}", rslt, 20);
                }
                else
                {

                    if (locationCode == "undefined")
                    {
                        locationCode = "";
                    }

                    string query = $@"SELECT pre_location_code, location_code, location_edesc,
                                     EMAIL,
                                           TELEPHONE_MOBILE_NO,
                                           ADDRESS,
                                           CREATED_DATE,
                                           CREATED_BY
                                      FROM ip_location_setup
                                     WHERE  COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

                    if (string.IsNullOrEmpty(locationCode) || string.IsNullOrWhiteSpace(locationCode))
                    {
                        query = query + " AND UPPER (location_EDESC) LIKE '%'";
                    }
                    else
                    {
                        query = query + $@"  AND LOCATION_CODE LIKE '{locationCode}%'";
                    }

                    query = query + " AND group_sku_flag = 'I' AND DELETED_FLAG = 'N' ORDER BY UPPER (location_EDESC)";

                    rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();
                    this._cacheManager.Set($"GetLocationListByLocationCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{locationId}_{locationCode}_{searchText}", rslt, 20);

                }

                //}
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<LocationModels> GetRoutineListByPlanCode(string locationId, string planCode, string searchText)
        {
            try
            {
                var rslt = new List<LocationModels>();

                var companyCode = _workContext.CurrentUserinformation.company_code;
                string query = $@"
                                    SELECT pre_location_code, location_code, location_edesc, CREATED_DATE
                                        FROM ip_location_setup
                                       WHERE     UPPER (location_EDESC) LIKE '%'
                                             AND group_sku_flag = 'I'
                                             AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                             AND DELETED_FLAG = 'N'
                                             AND LOCATION_CODE IN
                                                    (SELECT MPP.LOCATION_CODE
                                                       FROM MP_VARIANCE_INFO MP, MP_PROCESS_SETUP MPP
                                                      WHERE     MP.PLAN_CODE = '{planCode}'
                                                            AND MP.PROCESS_CODE = MPP.PROCESS_CODE
                                                            AND MP.COMPANY_CODE = MPP.COMPANY_CODE)
                                    ORDER BY UPPER (location_EDESC)";
                rslt = _dbContext.SqlQuery<LocationModels>(query).ToList();


                List<LocationModels> itemNeedsToRemove = new List<LocationModels>();
                for (int i = rslt.Count - 1; i >= 0; i--)
                {
                    var item = rslt[i];

                    string tempQuery = $@"select INDEX_ITEM_CODE, LOCATION_CODE  from MP_PROCESS_SETUP where LOCATION_CODE= '{item.LOCATION_CODE}' and Company_code='{_workContext.CurrentUserinformation.company_code}'";
                    var mpProcessSetupData = _dbContext.SqlQuery<MpProcessSetupDataModel>(tempQuery).FirstOrDefault();

                    string tempQuery1 = $@"SELECT SUM(QUANTITY) AS TOTAL_QUANTITY
                                            FROM IP_PRODUCTION_ISSUE
                                            WHERE TO_LOCATION_CODE = '{item.LOCATION_CODE}'
                                            AND plan_no = '{planCode}' and Company_code = '{companyCode}'";
                    var totalIssueQty = _dbContext.SqlQuery<decimal?>(tempQuery1).FirstOrDefault();

                    string tempQuery2 = $@"select RAW_ITEM_CODE, REQUIRED_QUANTITY from MP_VARIANCE_INFO where Plan_code = '{planCode}'  and Company_code = '{companyCode}' and RAW_ITEM_CODE = '{mpProcessSetupData.INDEX_ITEM_CODE}'";
                    var mpVarianceData = _dbContext.SqlQuery<MpVarianceInfoDataModel>(tempQuery2).FirstOrDefault();

                    if (mpVarianceData != null)
                    {
                        if (totalIssueQty != null && totalIssueQty >= mpVarianceData.REQUIRED_QUANTITY)
                        {
                            rslt.RemoveAt(i);
                        }
                    }

                }


                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class MpVarianceInfoDataModel
        {
            public string INDEX_ITEM_CODE { get; set; }
            public decimal? REQUIRED_QUANTITY { get; set; }
        }

        public class MpProcessSetupDataModel
        {
            public string INDEX_ITEM_CODE { get; set; }
            public string LOCATION_CODE { get; set; }
        }

        public double GetQuantityUsingPlanNoAndItemLCode(string planCode, string lCode)
        {
            try
            {
                string query = $@"SELECT QUANTITY - PROD_QTY QUANTITY  FROM V_MP_PLAN_WISE_PORD_QTY_NEW WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND LOCATION_CODE = '{lCode}' AND PLAN_CODE ='{planCode}'";

                //string query = $@"SELECT PLAN_CODE,
                //                  LOCATION_CODE,
                //                  ITEM_CODE,
                //                  ITEM_EDESC,
                //                  COMPANY_CODE,
                //                  QUANTITY,
                //                  PROD_QTY,
                //                  PROD_REC,
                //                  (PROD_QTY - PROD_REC) RECEIVED_QTY
                //             FROM (SELECT DISTINCT
                //                          MP.PLAN_CODE,
                //                          LOCATION_CODE,
                //                          INDEX_ITEM_CODE ITEM_CODE,
                //                          ITEM_EDESC,
                //                          MP.COMPANY_CODE,
                //                          CASE
                //                             WHEN MPP.INDEX_ITEM_CODE = OP.ITEM_CODE
                //                             THEN
                //                                OP.PLAN_QUANTITY
                //                             WHEN OP.ITEM_CODE = '0'
                //                             THEN
                //                                (SELECT REQUIRED_QUANTITY
                //                                   FROM MP_VARIANCE_INFO
                //                                  WHERE     PLAN_CODE = MP.PLAN_CODE
                //                                        AND (   FINISHED_ITEM_CODE =
                //                                                   MPP.INDEX_ITEM_CODE
                //                                             OR RAW_ITEM_CODE = MPP.INDEX_ITEM_CODE)
                //                                        AND COMPANY_CODE = MP.COMPANY_CODE
                //                                        AND ROWNUM = 1)
                //                             ELSE
                //                                (SELECT REQUIRED_QUANTITY
                //                                   FROM MP_VARIANCE_INFO
                //                                  WHERE     PLAN_CODE = MP.PLAN_CODE
                //                                        AND RAW_ITEM_CODE = MPP.INDEX_ITEM_CODE
                //                                        AND COMPANY_CODE = MP.COMPANY_CODE
                //                                        AND ROWNUM = 1)
                //                          END
                //                             QUANTITY,
                //                          NVL (
                //                             (SELECT SUM (PRODUCTION_QTY)
                //                                FROM IP_PRODUCTION_ISSUE
                //                               WHERE     PLAN_NO = OP.PLAN_NO
                //                                     AND COMPANY_CODE = OP.COMPANY_CODE
                //                                     AND DELETED_FLAG = 'N'
                //                                     AND SERIAL_NO = 1
                //                                     AND to_location_code= MPP.location_code),
                //                             0)
                //                             PROD_QTY,
                //                          NVL (
                //                             (SELECT SUM (PRODUCTION_QTY)
                //                                FROM IP_PRODUCTION_MRR
                //                               WHERE     PLAN_NO = OP.PLAN_NO
                //                                     AND COMPANY_CODE = OP.COMPANY_CODE
                //                                     AND DELETED_FLAG = 'N'
                //                                     AND SERIAL_NO = 1),
                //                             0)
                //                             PROD_REC
                //                     FROM MP_VARIANCE_INFO MP,
                //                          MP_PROCESS_SETUP MPP,
                //                          IP_ITEM_MASTER_SETUP IT,
                //                          MP_ORDER_PLAN_PROCESS OP
                //                    WHERE     1 = 1
                //                          AND MP.PROCESS_CODE = MPP.PROCESS_CODE
                //                          AND MP.COMPANY_CODE = MPP.COMPANY_CODE
                //                          AND MPP.INDEX_ITEM_CODE = IT.ITEM_CODE
                //                          AND MPP.COMPANY_CODE = IT.COMPANY_CODE
                //                          AND MP.PLAN_CODE = OP.PLAN_NO
                //                          AND MP.COMPANY_CODE = OP.COMPANY_CODE
                //                          AND MP.DELETED_FLAG = 'N' and MP.PLAN_CODE={planCode});
                //                          ";


                var quantity = _dbContext.SqlQuery<double>(query).FirstOrDefault();
                return quantity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<BranchModels> GetBranchListByBranchCode(string branchId, string branchCode, string searchText)
        {
            try
            {
                var rslt = new List<BranchModels>();
                if (this._cacheManager.IsSet($"GetBranchListByBranchCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{branchId}_{branchCode}_{searchText}"))
                {
                    var data = _cacheManager.Get<List<BranchModels>>($"GetBranchListByBranchCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{branchId}_{branchCode}_{searchText}");
                    rslt = data;
                }
                else
                {
                    if (String.IsNullOrEmpty(searchText))
                    {
                        if (branchCode == "undefined")
                        {
                            branchCode = "";
                        }
                        string query = $@"SELECT BRANCH_CODE,BRANCH_EDESC,EMAIL,TELEPHONE_NO,ADDRESS FROM FA_BRANCH_SETUP  WHERE 
                    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND BRANCH_CODE LIKE '{branchCode}%' AND GROUP_SKU_FLAG = 'I'  AND DELETED_FLAG='N'  ORDER BY BRANCH_CODE ASC";
                        rslt = _dbContext.SqlQuery<BranchModels>(query).ToList();
                        this._cacheManager.Set($"GetBranchListByBranchCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{branchId}_{branchCode}_{searchText}", rslt, 20);

                    }
                    else
                    {
                        string query = $@"SELECT DISTINCT BRANCH_CODE,BRANCH_EDESC,EMAIL,TELEPHONE_NO,ADDRESS FROM FA_BRANCH_SETUP     WHERE 
                    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(BRANCH_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(EMAIL)) LIKE '%{searchText.ToUpper()}%'  AND DELETED_FLAG='N' OR (UPPER(TELEPHONE_NO)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I'  ORDER BY BRANCH_CODE ASC";
                        rslt = _dbContext.SqlQuery<BranchModels>(query).ToList();
                        this._cacheManager.Set($"GetBranchListByBranchCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{branchId}_{branchCode}_{searchText}", rslt, 20);

                    }
                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<EmployeeCodeModels> GetEmployeeListByEmployeeCode(string employeecode, string employeeMasterCode, string searchText)
        {
            try
            {
                var rslt = new List<EmployeeCodeModels>();
                if (this._cacheManager.IsSet($"GetEmployeeListByEmployeeCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{employeecode}_{employeeMasterCode}_{searchText}"))
                {
                    var data = _cacheManager.Get<List<EmployeeCodeModels>>($"GetEmployeeListByEmployeeCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{employeecode}_{employeeMasterCode}_{searchText}");
                    rslt = data;
                }
                else
                {
                    if (String.IsNullOrEmpty(searchText))
                    {
                        if (employeeMasterCode == "undefined")
                        {
                            employeeMasterCode = "";
                        }
                        string query = $@"SELECT EMPLOYEE_CODE,EMPLOYEE_EDESC,EPERMANENT_ADDRESS1,MOBILE FROM HR_EMPLOYEE_SETUP  WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND MASTER_EMPLOYEE_CODE LIKE '{employeeMasterCode}%' AND GROUP_SKU_FLAG = 'I'  AND DELETED_FLAG='N'  ORDER BY EMPLOYEE_CODE ASC";
                        rslt = _dbContext.SqlQuery<EmployeeCodeModels>(query).ToList();
                        this._cacheManager.Set($"GetBranchListByBranchCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{employeecode}_{employeeMasterCode}_{searchText}", rslt, 20);

                    }
                    else
                    {
                        string query = $@"SELECT DISTINCT EMPLOYEE_CODE,EMPLOYEE_EDESC,EPERMANENT_ADDRESS1,MOBILE FROM HR_EMPLOYEE_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(EMPLOYEE_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(EPERMANENT_ADDRESS1)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I'  AND DELETED_FLAG='N'  ORDER BY EMPLOYEE_CODE ASC";
                        rslt = _dbContext.SqlQuery<EmployeeCodeModels>(query).ToList();
                        this._cacheManager.Set($"GetEmployeeListByEmployeeCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{employeecode}_{employeeMasterCode}_{searchText}", rslt, 20);
                    }

                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<DivisionModels> GetdivisionListBydivisionCode(string divisioncode, string divisionMasterCode, string searchText)
        {
            try
            {
                var rslt = new List<DivisionModels>();
                if (this._cacheManager.IsSet($"GetdivisionListBydivisionCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{divisioncode}_{divisionMasterCode}_{searchText}"))
                {
                    var data = _cacheManager.Get<List<DivisionModels>>($"GetdivisionListBydivisionCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{divisioncode}_{divisionMasterCode}_{searchText}");
                    rslt = data;
                }
                else
                {
                    if (String.IsNullOrEmpty(searchText))
                    {
                        if (divisionMasterCode == "undefined")
                        {
                            divisionMasterCode = "";
                        }
                        string query = $@"SELECT DIVISION_EDESC, DIVISION_CODE, ADDRESS,TELEPHONE_NO FROM FA_DIVISION_SETUP  WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND DIVISION_CODE LIKE '{divisionMasterCode}%' AND GROUP_SKU_FLAG = 'I'  AND DELETED_FLAG='N'  ORDER BY DIVISION_CODE ASC";
                        rslt = _dbContext.SqlQuery<DivisionModels>(query).ToList();
                        this._cacheManager.Set($"GetdivisionListBydivisionCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{divisioncode}_{divisionMasterCode}_{searchText}", rslt, 20);
                    }
                    else
                    {
                        string query = $@"SELECT DISTINCT DIVISION_EDESC, DIVISION_CODE, ADDRESS,TELEPHONE_NO FROM FA_DIVISION_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(DIVISION_EDESC)) like '%{searchText.ToUpper()}%' OR (UPPER(ADDRESS)) LIKE '%{searchText.ToUpper()}%'  AND DELETED_FLAG='N' OR (UPPER(TELEPHONE_NO)) LIKE '%{searchText.ToUpper()}%' AND GROUP_SKU_FLAG = 'I'  ORDER BY DIVISION_CODE ASC";
                        rslt = _dbContext.SqlQuery<DivisionModels>(query).ToList();
                        this._cacheManager.Set($"GetdivisionListBydivisionCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{divisioncode}_{divisionMasterCode}_{searchText}", rslt, 20);
                    }

                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<AccountCodeModels> getAllAccountCode()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ACC_EDESC) AS ACC_EDESC,
                        ACC_CODE ,
                        MASTER_ACC_CODE, 
                        PRE_ACC_CODE,
                        ACC_TYPE_FLAG,ACC_ID
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                        AND ACC_TYPE_FLAG='N'
                        CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                        ORDER BY PRE_ACC_CODE";
            var accountCodeList = _dbContext.SqlQuery<AccountCodeModels>(query).ToList();
            return accountCodeList;
        }

        public List<AccountCodeModels> getAllAccountCode(string searchText)
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ACC_EDESC) AS ACC_EDESC,
                        ACC_CODE ,
                        MASTER_ACC_CODE, 
                        PRE_ACC_CODE,
                        ACC_TYPE_FLAG,ACC_ID
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                        AND ACC_TYPE_FLAG='N'
                        CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                        AND (ACC_CODE like '%{searchText.ToUpperInvariant()}%' 
                        or upper(ACC_EDESC) like '%{searchText.ToUpperInvariant()}%') ORDER BY ACC_CODE";
            var accountCodeList = _dbContext.SqlQuery<AccountCodeModels>(query).ToList();
            return accountCodeList;
        }

        public List<AccountCodeModels> getAccountCodeByCode(string acccode)
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ACC_EDESC) AS ACC_EDESC,
                        ACC_CODE ,
                        MASTER_ACC_CODE, 
                        PRE_ACC_CODE,
                        ACC_TYPE_FLAG,ACC_ID
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        WHERE DELETED_FLAG = 'N' 
                         AND FREEZE_FLAG='N'
                        AND ACC_TYPE_FLAG='T'
                       AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND ACC_CODE='{acccode}'";
            var accountCodeList = _dbContext.SqlQuery<AccountCodeModels>(query).ToList();
            return accountCodeList;
        }
        public List<AccountCodeModels> getAllAccountCodeWithChild(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter = string.Empty;

                }
                var countryQuery = $@"SELECT DISTINCT 
                             COALESCE(ACC_EDESC,' ') ACC_EDESC
                            ,COALESCE(ACC_CODE,' ') ACC_CODE,
                             MASTER_ACC_CODE, 
                            PRE_ACC_CODE,
                            ACC_TYPE_FLAG,ACC_ID
                            FROM FA_CHART_OF_ACCOUNTS_SETUP
                            WHERE DELETED_FLAG = 'N' AND FREEZE_FLAG='N'
                            CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                            AND (ACC_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(ACC_EDESC) like '%{filter.ToUpperInvariant()}%') ORDER BY PRE_ACC_CODE";
                var result = _dbContext.SqlQuery<AccountCodeModels>(countryQuery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<AccountCodeModels> getAllAccountCodeForVs(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter = string.Empty;

                }
                var countryQuery = $@"SELECT DISTINCT 
                             COALESCE(ACC_EDESC,' ') ACC_EDESC
                            ,COALESCE(ACC_CODE,' ') ACC_CODE,
                             MASTER_ACC_CODE, 
                            PRE_ACC_CODE,
                            ACC_TYPE_FLAG,ACC_ID
                            FROM FA_CHART_OF_ACCOUNTS_SETUP
                            WHERE DELETED_FLAG = 'N' AND FREEZE_FLAG='N'
                            AND (ACC_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(ACC_EDESC) like '%{filter.ToUpperInvariant()}%') ORDER BY ACC_CODE";
                var result = _dbContext.SqlQuery<AccountCodeModels>(countryQuery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<AccountCodeModels> getAllAccountComboCodeWithChild(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter = string.Empty;

                }
                var countryQuery = $@"SELECT DISTINCT 
                             COALESCE(ACC_EDESC,' ') ACC_EDESC
                            ,COALESCE(ACC_CODE,' ') ACC_CODE,
                             MASTER_ACC_CODE, 
                            PRE_ACC_CODE,
                            ACC_TYPE_FLAG,ACC_ID
                            FROM FA_CHART_OF_ACCOUNTS_SETUP
                            WHERE DELETED_FLAG = 'N' AND FREEZE_FLAG='N'
                            CONNECT BY PRIOR MASTER_ACC_CODE = PRE_ACC_CODE
                            AND (ACC_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(ACC_EDESC) like '%{filter.ToUpperInvariant()}%') ORDER BY PRE_ACC_CODE";
                var result = _dbContext.SqlQuery<AccountCodeModels>(countryQuery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<AccountCodeModels> getAllAccounts()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ACC_EDESC) AS ACC_EDESC,
                        ACC_CODE ,
                        MASTER_ACC_CODE, 
                        PRE_ACC_CODE,
                        ACC_TYPE_FLAG,ACC_ID
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        WHERE DELETED_FLAG = 'N' AND 
                         FREEZE_FLAG='N'
                        AND ACC_TYPE_FLAG='T'
                       AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var accountCodeList = _dbContext.SqlQuery<AccountCodeModels>(query).ToList();
            return accountCodeList;
        }
        public List<AccountCodeModels> getAllAccountSupp()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ACC_EDESC) AS ACC_EDESC,
                        ACC_CODE ,
                        MASTER_ACC_CODE, 
                        PRE_ACC_CODE,
                        ACC_TYPE_FLAG
                        FROM FA_CHART_OF_ACCOUNTS_SETUP
                        WHERE DELETED_FLAG = 'N' AND 
                        FREEZE_FLAG='N'
                       AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var accountCodeList = _dbContext.SqlQuery<AccountCodeModels>(query).ToList();
            return accountCodeList;
        }
        public List<ResourceCodeModels> getAllResourceCode()
        {
            try
            {
                string query = $@"SELECT DISTINCT 
                        INITCAP(RESOURCE_EDESC) AS RESOURCE_EDESC,
                        RESOURCE_CODE ,
                        PRE_RESOURCE_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM MP_RESOURCE_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        AND GROUP_SKU_FLAG='G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        ORDER BY RESOURCE_EDESC";
                var accountCodeList = _dbContext.SqlQuery<ResourceCodeModels>(query).ToList();
                return accountCodeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Division Under Group
        public List<DivisionModels> getAllDivisionCode()
        {
            try
            {
                string query = $@"SELECT DISTINCT 
                        INITCAP(DIVISION_EDESC) AS DIVISION_EDESC,
                        DIVISION_CODE ,
                        PRE_DIVISION_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM FA_DIVISION_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        AND GROUP_SKU_FLAG='G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        ORDER BY DIVISION_EDESC";
                var accountCodeList = _dbContext.SqlQuery<DivisionModels>(query).ToList();
                return accountCodeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AreaModels> getAllArea()
        {
            string query = $@" SELECT DISTINCT 
                        INITCAP(AREA_EDESC) AS AREA_EDESC,
                        TO_CHAR(AREA_CODE)AREA_CODE,
                        REMARKS
                        FROM AREA_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var areaList = _dbContext.SqlQuery<AreaModels>(query).ToList();
            return areaList;
        }
        public List<AgentModels> getAllAgent()
        {
            string query = $@" SELECT DISTINCT 
                        INITCAP(AGENT_EDESC) AS AGENT_EDESC,AGENT_NDESC,
                        TO_CHAR(AGENT_CODE)AGENT_CODE,AGENT_TYPE,
                        AGENT_ID,CREDIT_LIMIT,CREDIT_DAYS,PAN_NO,
                        REMARKS
                       -- ,ADDRESS
                        FROM AGENT_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var agentList = _dbContext.SqlQuery<AgentModels>(query).ToList();
            return agentList;
        }

        public List<TransporterModels> getAllTransporter()
        {
            string query = $@"SELECT * FROM TRANSPORTER_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var List = _dbContext.SqlQuery<TransporterModels>(query).ToList();
            return List;
        }
        public List<BudgetCenterCodeModels> getAllBudgetCenterCode()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(BUDGET_EDESC) AS BUDGET_EDESC,
                        BUDGET_CODE ,
                        BUDGET_CODE AS  MASTER_BUDGET_CODE, 
                        PRE_BUDGET_CODE,
                        GROUP_SKU_FLAG AS BUDGET_TYPE_FLAG
                        FROM bc_budget_center_setup
                        WHERE DELETED_FLAG = 'N' 
                        AND GROUP_SKU_FLAG='G'
                        AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                        ORDER BY BUDGET_EDESC";
            var budgetCenterCodeList = _dbContext.SqlQuery<BudgetCenterCodeModels>(query).ToList();
            return budgetCenterCodeList;
        }
        public List<AccountCodeModels> GetAccountListByAccountCode(string acccode, string accMasterCode, string searchText)
        {
            try
            {
                var userid = _workContext.CurrentUserinformation.User_id;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch_code = _workContext.CurrentUserinformation.branch_code;
                var rslt = new List<AccountCodeModels>();
                if (this._cacheManager.IsSet($"GetAccountListByAccountCode_{userid}_{company_code}_{branch_code}_{acccode}_{accMasterCode}_{searchText}"))
                {
                    var data = _cacheManager.Get<List<AccountCodeModels>>($"GetAccountListByAccountCode_{userid}_{company_code}_{branch_code}_{acccode}_{accMasterCode}_{searchText}");
                    rslt = data;
                }
                else
                {
                    if (String.IsNullOrEmpty(searchText))
                    {
                        if (accMasterCode == "undefined")
                        {
                            accMasterCode = "";
                        }
                        //    string query = $@"SELECT ACC_CODE,  ACC_EDESC ,TRANSACTION_TYPE,ACC_NATURE,CREATED_BY,to_char(CREATED_DATE ,'DD/MM/RRRR') CREATED_DATE   FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE 
                        //COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND ACC_TYPE_FLAG='T'  AND DELETED_FLAG='N' AND FREEZE_FLAG='N' AND MASTER_ACC_CODE LIKE '{accMasterCode}%' ORDER BY ACC_CODE ASC";
                        //Prem Prakash Dhakal
                        //Remove the FREEZE_FLAG
                        //AND FREEZE_FLAG = 'N'
                        string query = $@"SELECT ACC_CODE,  ACC_EDESC ,TRANSACTION_TYPE,ACC_NATURE,CREATED_BY,to_char(CREATED_DATE ,'DD/MM/RRRR') CREATED_DATE   FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND ACC_TYPE_FLAG='T'  AND DELETED_FLAG='N'  AND MASTER_ACC_CODE LIKE '{accMasterCode}%' ORDER BY ACC_CODE ASC";
                        rslt = _dbContext.SqlQuery<AccountCodeModels>(query).ToList();
                        this._cacheManager.Set($"GetAccountListByAccountCode_{userid}_{company_code}_{branch_code}_{acccode}_{accMasterCode}_{searchText}", rslt, 20);
                    }
                    else
                    {
                        if (accMasterCode == "niraj")
                        {
                            string query = $@"SELECT DISTINCT ACC_CODE,ACC_EDESC,TRANSACTION_TYPE,ACC_NATURE,CREATED_BY,to_char(CREATED_DATE ,'DD/MM/RRRR') CREATED_DATE FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE 
                            COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND ACC_EDESC like '%{searchText}%'  AND DELETED_FLAG='N' AND ACC_TYPE_FLAG='T' ORDER BY ACC_CODE ASC";
                            rslt = _dbContext.SqlQuery<AccountCodeModels>(query).ToList();
                            this._cacheManager.Set($"GetAccountListByAccountCode_{userid}_{company_code}_{branch_code}_{acccode}_{accMasterCode}_{searchText}", rslt, 20);
                        }
                        else
                        {
                            string query = $@"SELECT DISTINCT ACC_CODE,ACC_EDESC,TRANSACTION_TYPE,ACC_NATURE,CREATED_BY,to_char(CREATED_DATE ,'DD/MM/RRRR') CREATED_DATE FROM FA_CHART_OF_ACCOUNTS_SETUP WHERE 
                    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND (UPPER(ACC_EDESC)) like '%{searchText.ToUpper()}%'  AND DELETED_FLAG='N' AND ACC_TYPE_FLAG='T' ORDER BY ACC_CODE ASC";
                            rslt = _dbContext.SqlQuery<AccountCodeModels>(query).ToList();
                            this._cacheManager.Set($"GetAccountListByAccountCode_{userid}_{company_code}_{branch_code}_{acccode}_{accMasterCode}_{searchText}", rslt, 20);
                        }
                    }

                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<FormDetails> getRefrenceOrderNo(string FormCode, string filter, string Table_Name)
        {
            try
            {
                if (Table_Name != null && FormCode != null)
                {
                    var primarycolname = GetPrimaryColumnByTableName(Table_Name);
                    var primaryDate = GetPrimaryDateByTableName(Table_Name);
                    if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                    var sqlquery = $@"SELECT DISTINCT
                                    COALESCE({primarycolname},' ') ORDER_CODE,
                                    COALESCE({primarycolname},' ') ORDER_EDESC
                                    from {Table_Name}
                                    where form_code={FormCode} and  deleted_flag='N' 
                                    and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                                    and((upper({primarycolname}) like '%{filter.ToUpperInvariant()}%')
                                    or (TO_CHAR({primaryDate},'MM/DD/YYYY') like '%{filter.ToUpperInvariant()}%'))";
                    var orderNoList = _dbContext.SqlQuery<FormDetails>(sqlquery).ToList();
                    return orderNoList;
                }
                else
                {
                    List<FormDetails> formdetail = new List<FormDetails>();
                    return formdetail;
                }
            }
            catch (Exception)
            {
                throw;
            }
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
        public List<RefrenceType> getRefrence(string FormCode, bool ShowDocumentType = false)
        {
            try
            {
                if (ShowDocumentType)
                {
                    var isDispatchFlag = false;
                    var orclesqlQuery = $@"select ORDER_DISPATCH_FLAG from form_setup where form_code='{FormCode}'  AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                    var dataLoadingSlip = _dbContext.SqlQuery<string>(orclesqlQuery).FirstOrDefault();
                    if (dataLoadingSlip != null)
                    {
                        isDispatchFlag = dataLoadingSlip == "Y" ? true : false;
                    }

                    if (isDispatchFlag)
                    {
                        var refrence = new RefrenceType() { REF_TABLE_NAME = "SA_LOADING_SLIP_DETAIL", REFERENCE_FLAG = "Y", REF_EDESC = "Loading Slip", REF_CODE = "SA_LOADING_SLIP_DETAIL" };
                        var abc = new List<RefrenceType>();
                        abc.Add(refrence);
                        return abc;
                    }
                    var sqlqueryDocument = $@"SELECT DISTINCT a.ref_table_name,
                                     COALESCE(a.REFERENCE_FLAG, ' ') REFERENCE_FLAG,
                                                      b.table_desc REF_EDESC,
                                                      ref_form_code REF_CODE
                                                      --AFTER_VERIFY_FLAG,
                                                      --AFTER_POSTING_FLAG
                                        FROM form_setup a, transaction_table_list b where a.ref_table_name=b.table_name  and a.form_code ='{FormCode}' AND REFERENCE_FLAG='Y' AND A.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";

                    List<RefrenceType> REFListDoc = _dbContext.SqlQuery<RefrenceType>(sqlqueryDocument).ToList();

                    return REFListDoc;
                }

                var sqlquery = $@" SELECT DISTINCT a.ref_table_name,
                                     COALESCE(a.REFERENCE_FLAG, ' ') REFERENCE_FLAG,
                                                      b.table_desc REF_EDESC,
                                                      ref_form_code REF_CODE
                                                      --AFTER_VERIFY_FLAG,
                                                      --AFTER_POSTING_FLAG
                                        FROM form_setup a, transaction_table_list b
                                       WHERE   (  a.ref_table_name =(
                                       SELECT ref_table_name
                                                                  FROM form_setup
                                                                 WHERE form_code = '{FormCode}' AND company_code = '{_workContext.CurrentUserinformation.company_code}' )                                                   
                                                                 or 
                                       a.ref_form_code = (
                                       SELECT ref_form_code
                                                                  FROM form_setup
                                                                 WHERE form_code = '{FormCode}' AND company_code = '{_workContext.CurrentUserinformation.company_code}'     
                                                                 )   )   
                                             AND a.ref_table_name = b.table_name and a.REFERENCE_FLAG='Y'
                                    ORDER BY a.ref_table_name";

                List<RefrenceType> REFList = _dbContext.SqlQuery<RefrenceType>(sqlquery).ToList();

                return REFList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<TemplateType> getTemplate(string FormCode)
        {
            try
            {
                List<RefrenceType> REFList = getRefrence(FormCode);
                var TempList = new List<TemplateType>();
                if (REFList.Count > 0)
                {
                    var sqlquery = $@"SELECT form_code TABLE_CODE, form_edesc TABLE_EDESC
                                      FROM form_setup
                                     WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                           AND form_code IN
                                                  (SELECT DISTINCT form_code
                                                     FROM form_detail_setup
                                                    WHERE     table_name = '{REFList[0].REF_TABLE_NAME}'                                                         AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}')
                                    UNION
                                    SELECT '0', 'ALL DOCUMENTS' FROM DUAL";
                    TempList = _dbContext.SqlQuery<TemplateType>(sqlquery).ToList();
                    return TempList;
                }
                else
                {
                    return TempList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        //subin changes for dynamic templates name
        public List<TemplateType> getTemplates(string FormCode, string tablename)
        {
            try
            {
                var sqlquery = $@"SELECT form_code TABLE_CODE, form_edesc TABLE_EDESC
                                      FROM form_setup
                                     WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                           AND form_code IN
                                                  (SELECT DISTINCT form_code
                                                     FROM form_detail_setup
                                                    WHERE     table_name = '{tablename}'
                                                          AND form_code <> '{FormCode}'
                                                          AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}')
                                    UNION
                                    SELECT '0', 'ALL DOCUMENTS' FROM DUAL";
                var REFList = _dbContext.SqlQuery<TemplateType>(sqlquery).ToList();
                return REFList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<SubLedger> GetSubLedger()
        {
            try
            {
                var query = $@"SELECT SUB_CODE,SUB_EDESC,SUB_NDESC,
                                SUB_LEDGER_FLAG,COMPANY_CODE,
                                CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                SYN_ROWID,MODIFY_DATE,BRANCH_CODE,
                                MODIFY_BY,SUBSTITUTE_NAME,LOCK_FLAG,SUB_ID
                                FROM FA_SUB_LEDGER_SETUP";
                var result = _dbContext.SqlQuery<SubLedger>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SubLedger> GetSubLedgerByAccountCode(string accountcode)
        {
            try
            {

                var rslt = new List<SubLedger>();
                if (this._cacheManager.IsSet($"GetSubLedgerByAccountCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{accountcode}"))
                {
                    var data = _cacheManager.Get<List<SubLedger>>($"GetSubLedgerByAccountCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{accountcode}");
                    rslt = data;
                }
                else
                {
                    var query = $@"SELECT SUB_CODE, SUB_EDESC, SUB_NDESC,
                                    SUB_LEDGER_FLAG, COMPANY_CODE,
                                    CREATED_BY, CREATED_DATE, DELETED_FLAG,
                                    SYN_ROWID, MODIFY_DATE, BRANCH_CODE,
                                    MODIFY_BY, SUBSTITUTE_NAME, LOCK_FLAG, SUB_ID
                                    FROM FA_SUB_LEDGER_SETUP WHERE SUB_CODE IN(select SUB_CODE from FA_SUB_LEDGER_MAP WHERE ACC_CODE = '{accountcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}')";
                    rslt = _dbContext.SqlQuery<SubLedger>(query).ToList();
                    this._cacheManager.Set($"GetSubLedgerByAccountCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{accountcode}", rslt, 20);
                }
                return rslt;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<ChargeSetup> GetChargeCodebyFormCode(string formcode)
        {
            try
            {
                var rslt = new List<ChargeSetup>();
                if (this._cacheManager.IsSet($"GetChargeCodebyFormCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{formcode}"))
                {
                    var data = _cacheManager.Get<List<ChargeSetup>>($"GetChargeCodebyFormCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{formcode}");
                    rslt = data;
                }
                else
                {
                    var query = $@"select A.CHARGE_CODE AS CHARGE_CODE ,A.CHARGE_EDESC AS CHARGE_EDESC  from IP_CHARGE_CODE A,charge_setup B WHERE  B.CHARGE_CODE=A.CHARGE_CODE AND B.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND B.FORM_CODE='{formcode}'
                                       GROUP BY A.CHARGE_CODE,A.CHARGE_EDESC ORDER BY A.CHARGE_CODE";
                    rslt = _dbContext.SqlQuery<ChargeSetup>(query).ToList();

                    this._cacheManager.Set($"GetChargeCodebyFormCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{formcode}", rslt, 20);
                }
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<ChargeCode> GetChargeCode()
        {
            try
            {
                var query = $@" select CHARGE_CODE,CHARGE_EDESC,
                                CHARGE_NDESC,REMARKS,COMPANY_CODE,CREATED_BY,
                                CREATED_DATE,DELETED_FLAG,SYN_ROWID,SPECIFIC_CHARGE_FLAG,
                                MODIFY_DATE,
                                MODIFY_BY from IP_CHARGE_CODE WHERE DELETED_FLAG='N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<ChargeCode>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryModel> GetCategoryCode()
        {
            try
            {
                var query = $@" SELECT CATEGORY_CODE,CATEGORY_EDESC,CATEGORY_NDESC,
                                REMARKS,COMPANY_CODE,CREATED_BY,CREATED_DATE,
                                DELETED_FLAG,SYN_ROWID,CATEGORY_TYPE,MODIFY_DATE,
                                PREFIX_TEXT,MODIFY_BY FROM IP_CATEGORY_CODE
                                WHERE DELETED_FLAG='N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<CategoryModel>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenuModels> GetAllSetupMenuItems()
        {
            try
            {
                var rslt = new List<MenuModels>();
                //if (this._cacheManager.IsSet($"GetAllSetupMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}"))
                //{
                //    var data = _cacheManager.Get<List<MenuModels>>($"GetAllSetupMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}");
                //    rslt = data;
                //}
                //else
                //{
                //    string query = $@"SELECT DISTINCT MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH FROM WEB_MENU_MANAGEMENT WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '05'
                //                      AND MENU_NO IN (SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND USER_NO='{_workContext.CurrentUserinformation.User_id}')
                //                      order by MENU_NO ASC";
                //    rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
                //    this._cacheManager.Set($"GetAllSetupMenuItems_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}", rslt, 20);
                //}
                string query = $@"SELECT DISTINCT MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH, group_sku_flag GROUP_SKU_FLAG FROM WEB_MENU_MANAGEMENT WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '05'
                                      AND MENU_NO IN (SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND USER_NO='{_workContext.CurrentUserinformation.User_id}')
                                      order by MENU_NO ASC";
                rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
                return rslt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<MenuModels> GetAllUpdateItems()
        {
            try
            {
                var result = new List<MenuModels>();
                string query = $@"SELECT DISTINCT MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH, group_sku_flag GROUP_SKU_FLAG FROM WEB_MENU_MANAGEMENT WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '09'
                                      AND MENU_NO IN (SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND USER_NO='{_workContext.CurrentUserinformation.User_id}')
                                      order by MENU_NO ASC";
                result = this._dbContext.SqlQuery<MenuModels>(query).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List<FormSetup> getAllForms()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(FORM_EDESC) AS FORM_EDESC,
                        INITCAP(FORM_CODE) AS FORM_CODE,
                        MASTER_FORM_CODE,
                        PRE_FORM_CODE,
                        MODULE_CODE,
                        GROUP_SKU_FLAG FROM FORM_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        ORDER BY FORM_CODE";

            var formList = _dbContext.SqlQuery<FormSetup>(query).ToList();
            return formList;
        }
        public List<ModuleModels> getAllModules()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(MODULE_CODE) AS MODULE_CODE,
                        INITCAP(MODULE_EDESC) AS MODULE_EDESC
                        FROM MODULE_SETUP
                        ORDER BY MODULE_CODE";

            var moduleList = _dbContext.SqlQuery<ModuleModels>(query).ToList();
            return moduleList;
        }
        public List<FormSetup> getAllFormsListAccordingToModule(string ModuleCode)
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(FORM_CODE) AS FORM_CODE,
                        INITCAP(FORM_EDESC) AS FORM_EDESC,
                        MASTER_FORM_CODE,
                        PRE_FORM_CODE,
                        MODULE_CODE,
                        GROUP_SKU_FLAG
                        FROM FORM_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        AND MODULE_CODE = '{ModuleCode}'
                        AND FORM_CODE IN 
                        (SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO = '{_workContext.CurrentUserinformation.User_id}' 
                        AND CREATE_FLAG = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' )
                        ORDER BY FORM_CODE";



            //string query = $@"SELECT DISTINCT 
            //            INITCAP(FORM_CODE) AS FORM_CODE,
            //            INITCAP(FORM_EDESC) AS FORM_EDESC,
            //            MASTER_FORM_CODE,
            //            PRE_FORM_CODE,
            //            MODULE_CODE,
            //            GROUP_SKU_FLAG
            //            FROM FORM_SETUP
            //            WHERE DELETED_FLAG = 'N'
            //            AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
            //            AND MODULE_CODE = '{ModuleCode}'
            //            AND FORM_CODE IN 
            //            (SELECT
            //                fbm.FORM_CODE
            //            FROM form_branch_map fbm
            //            INNER JOIN SC_FORM_CONTROL sc
            //                ON fbm.FORM_CODE = sc.FORM_CODE
            //                AND fbm.BRANCH_CODE = sc.BRANCH_CODE
            //                AND fbm.COMPANY_CODE = sc.COMPANY_CODE
            //            WHERE fbm.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
            //              AND fbm.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
            //              AND sc.USER_NO = '{_workContext.CurrentUserinformation.User_id}'
            //              AND fbm.DELETED_FLAG = 'N'
            //              AND sc.DELETED_FLAG = 'N'
            //              AND sc.CREATE_FLAG = 'Y')
            //            ORDER BY FORM_CODE";




            var moduleList = _dbContext.SqlQuery<FormSetup>(query).ToList();
            return moduleList;
        }
        public List<CategoryModel> GetAllItemCategoryFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter = string.Empty;

                }

                var countryQuery = $@"SELECT DISTINCT 
                             COALESCE(CATEGORY_EDESC,' ') CATEGORY_EDESC
                            ,COALESCE(CATEGORY_CODE,' ') CATEGORY_CODE
                            FROM IP_CATEGORY_CODE
                            WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND
                             (CATEGORY_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(CATEGORY_EDESC) like '%{filter.ToUpperInvariant()}%') ORDER BY CATEGORY_CODE";
                var result = _dbContext.SqlQuery<CategoryModel>(countryQuery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MuCodeModel> GetAllIndexMuFilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter = string.Empty;

                }

                var countryQuery = $@"SELECT DISTINCT 
                             COALESCE(MU_EDESC,' ') MU_EDESC
                            ,COALESCE(MU_CODE,' ') MU_CODE
                            FROM IP_MU_CODE
                            WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG = 'N' AND 
                             (MU_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(MU_EDESC) like '%{filter.ToUpperInvariant()}%') ORDER BY MU_CODE";
                var result = _dbContext.SqlQuery<MuCodeModel>(countryQuery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public string GetAccCodeByPTCode(string ptcode)
        //{
        //    var sqlquery = $@"SELECT
        //                            ACC_CODE from IP_PARTY_TYPE_CODE 
        //                            where PARTY_TYPE_CODE='{ptcode}' and  deleted_flag='N' 
        //                            and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";

        //    var code = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault().ToString();
        //    return code;
        //}
        #region Application Users
        public List<ApplicationUser> getALLUserListByFlter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                string query = $@"select USER_NO,                               
                                  LOGIN_EDESC                         
                                  from SC_APPLICATION_USERS where upper(LOGIN_EDESC) like '%{filter.ToUpperInvariant()}%'  and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and DELETED_FLAG='N'";
                var result = _dbContext.SqlQuery<ApplicationUser>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public List<MenuModels> GetAllPlanningMenuItems()
        {
            string query = $@" SELECT MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH,GROUP_SKU_FLAG,PRE_MENU_NO as PRE_FORM_CODE FROM WEB_MENU_MANAGEMENT WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '30' AND GROUP_SKU_FLAG='G' order by MENU_NO asc";
            var rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
            return rslt;
        }
        public List<MenuModels> GetAllMenuItems(string modulecode)
        {
            string query = $@" SELECT MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH,GROUP_SKU_FLAG,PRE_MENU_NO as PRE_FORM_CODE FROM WEB_MENU_MANAGEMENT 
                               WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '{modulecode}' AND GROUP_SKU_FLAG='G'
                               AND  MENU_NO IN (SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND USER_NO='{_workContext.CurrentUserinformation.User_id}')                              
                               order by MENU_NO asc";
            var rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
            return rslt;
        }
        public List<MenuModels> GetAllPlanningMenuItemsByPreMenuCode(string premenucode)
        {
            string query = $@" SELECT MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH,GROUP_SKU_FLAG,PRE_MENU_NO as PRE_FORM_CODE FROM WEB_MENU_MANAGEMENT WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '30' AND  DASHBOARD_FLAG='Y' AND PRE_MENU_NO like '{premenucode}%' order by MENU_NO asc";
            var rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
            return rslt;
        }
        public List<MenuModels> GetAllMenuItemsByPreMenuCode(string premenucode, string modulecode)
        {
            var DASHBOARD_FLAG = "";
            string query = "";
            if (modulecode == "02")
            {
                premenucode = "25";
                query = $@" SELECT MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH,GROUP_SKU_FLAG,PRE_MENU_NO as PRE_FORM_CODE
                           FROM WEB_MENU_MANAGEMENT
                           WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '{modulecode}'  AND PRE_MENU_NO like '{premenucode}%' 
                           AND MENU_NO IN (SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND USER_NO='{_workContext.CurrentUserinformation.User_id}')
                           order by MENU_NO asc";
            }
            else
            {
                DASHBOARD_FLAG = "Y";
                query = $@" SELECT MENU_NO,MENU_EDESC,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH,GROUP_SKU_FLAG,PRE_MENU_NO as PRE_FORM_CODE FROM WEB_MENU_MANAGEMENT WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '{modulecode}' AND  DASHBOARD_FLAG='{DASHBOARD_FLAG}' AND PRE_MENU_NO like '{premenucode}%' order by MENU_NO asc";
            }
            var rslt = this._dbContext.SqlQuery<MenuModels>(query).ToList();
            return rslt;
        }
        #region WEB_DESKTOP_FOLDER
        public WebDesktopFolder AddNewFolder(string FOLDER, string FOLDER_COLOR, string ICON)
        {
            WebDesktopFolder Record = new WebDesktopFolder();
            var MAXIDQUERY = $@"SELECT COALESCE(MAX(FOLDER_ID)+1,1) FROM WEB_DESKTOP_FOLDER";
            FOLDER_COLOR = "#" + FOLDER_COLOR;
            var Id = _dbContext.SqlQuery<int>(MAXIDQUERY).FirstOrDefault();
            var inserfolder = $@"INSERT INTO WEB_DESKTOP_FOLDER(FOLDER_ID,FOLDER_NAME,FOLDER_COLOR,ORDER_NO,ICON,COMPANY_CODE,BRANCH_CODE,USER_ID,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                                VALUES('{Id}','{FOLDER}','{FOLDER_COLOR}','{0}','{ICON}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.User_id}','{_workContext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N')";
            _dbContext.ExecuteSqlCommand(inserfolder);
            Record.FOLDER_NAME = FOLDER;
            Record.ICON = ICON;
            Record.FOLDER_COLOR = FOLDER_COLOR;
            return Record;
        }
        public List<WebDesktopFolder> GetFoldertByUserId()
        {
            _logErp.InfoInFile("Get Folder By User Id Started========");
            List<WebDesktopFolder> Record = new List<WebDesktopFolder>();
            var NAME_QUERY = $@"SELECT TO_CHAR(FOLDER_ID) as FOLDER_ID,FOLDER_NAME as FOLDER_NAME,ICON as Icon,FOLDER_COLOR AS FOLDER_COLOR  FROM WEB_DESKTOP_FOLDER WHERE  COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' ORDER BY ORDER_NO ASC";
            Record = _dbContext.SqlQuery<WebDesktopFolder>(NAME_QUERY).ToList();
            return Record;
        }
        public void UpdateOrderNoByFolderId(List<FOLDER_ORDER> fOLDER_ORDER)
        {
            List<WebDesktopFolder> Record = new List<WebDesktopFolder>();
            foreach (var data in fOLDER_ORDER)
            {
                var UPDATE_QUERY = $@"UPDATE WEB_DESKTOP_FOLDER
                                      SET ORDER_NO ='{data.ORDER_NO}'
                                      WHERE FOLDER_ID='{data.FOLDER_ID}' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND USER_ID='{_workContext.CurrentUserinformation.User_id}'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
            }
        }
        #endregion
        #region Web Desktop Management
        public List<WebDesktopManagement> AddWebDesktopManagement(WebDesktopManagement webDesktopManagement)
        {
            List<WebDesktopManagement> Record = new List<WebDesktopManagement>();
            MenuModels DATA = new MenuModels();
            var MAXIDQUERY = $@"SELECT COALESCE(MAX(WEB_DESKTOP_MANAGEMENT_ID)+1,1) FROM WEB_DESKTOP_MANAGEMENT";
            var Id = _dbContext.SqlQuery<int>(MAXIDQUERY).FirstOrDefault();
            if (string.IsNullOrEmpty(webDesktopManagement.MENU_EDESC))
            {
                string QUERY = $@" SELECT FORM_EDESC,MODULE_CODE,FORM_TYPE FROM FORM_SETUP FS 
                WHERE  FORM_CODE='{webDesktopManagement.FORM_CODE}'AND DELETED_FLAG = 'N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                DATA = this._dbContext.SqlQuery<MenuModels>(QUERY).FirstOrDefault();
            }
            if (DATA == null)
                return Record;
            else if (webDesktopManagement.SIDEBAR_ID == "0")
            {
                DATA.MODULE_CODE = "5";
            }
            var SIDEBAR_COUNT = 0;
            var DRAG_COUNT = 0;
            if (webDesktopManagement.SideBarMenu)
            {
                string SIDEBAR_QUERY = $@" SELECT COALESCE(MAX(SIDEBAR_ID)+1,1) FROM WEB_DESKTOP_MANAGEMENT WDM 
                                       WHERE DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                SIDEBAR_COUNT = this._dbContext.SqlQuery<int>(SIDEBAR_QUERY).FirstOrDefault();

            }
            else if (webDesktopManagement.SIDEBAR_ID != "0" || string.IsNullOrEmpty(webDesktopManagement.SIDEBAR_ID))
            {
                SIDEBAR_COUNT = Convert.ToInt32(webDesktopManagement.SIDEBAR_ID);
                string DRAG_QUERY = $@" SELECT COALESCE(MAX(DRAG_COUNT)+1,0) FROM WEB_DESKTOP_MANAGEMENT WDM 
                                       WHERE  SIDEBAR_ID='{webDesktopManagement.SIDEBAR_ID}'AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                DRAG_COUNT = this._dbContext.SqlQuery<int>(DRAG_QUERY).FirstOrDefault();
            }
            //COUNT DRAG USER WISE
            if (!string.IsNullOrEmpty(webDesktopManagement.MENU_NO))
            {
                string DRAG_QUERY = $@" SELECT COALESCE(MAX(DRAG_COUNT)+1,0) FROM WEB_DESKTOP_MANAGEMENT WDM 
                                       WHERE  MENU_NO='{webDesktopManagement.MENU_NO}'AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                DRAG_COUNT = this._dbContext.SqlQuery<int>(DRAG_QUERY).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(webDesktopManagement.TEMPLATE_CODE))
            {
                string DRAG_QUERY = $@" SELECT COALESCE(MAX(DRAG_COUNT)+1,0) FROM WEB_DESKTOP_MANAGEMENT WDM 
                               WHERE  TEMPLATE_CODE='{webDesktopManagement.TEMPLATE_CODE}'AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                DRAG_COUNT = this._dbContext.SqlQuery<int>(DRAG_QUERY).FirstOrDefault();

            }
            else if (!string.IsNullOrEmpty(webDesktopManagement.FORM_CODE))
            {
                string DRAG_QUERY = $@" SELECT COALESCE(MAX(DRAG_COUNT)+1,0) FROM WEB_DESKTOP_MANAGEMENT WDM 
                               WHERE  FORM_CODE='{webDesktopManagement.FORM_CODE}'AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                DRAG_COUNT = this._dbContext.SqlQuery<int>(DRAG_QUERY).FirstOrDefault();

            }
            // INSERT INTO WEB_DESKTOP_MANAGEMENT
            var inserfolder = $@"INSERT INTO WEB_DESKTOP_MANAGEMENT(WEB_DESKTOP_MANAGEMENT_ID,FORM_CODE,NEW_FOLDER_NAME,PREV_FOLDER_NAME,TEMPLATE_CODE,HREF,MODULE_CODE,FUNCTION_LINK,FORM_TYPE,COLOR,ICON_PATH,ABBR,FORM_EDESC,MENU_NO,MENU_EDESC,MENU_DESC,DRAG_COUNT,COMPANY_CODE,BRANCH_CODE,USER_ID,CREATED_BY,CREATED_DATE,DELETED_FLAG,UNIQUE_ID,SIDEBAR_ID)     
                                VALUES('{Id}','{webDesktopManagement.FORM_CODE}','{webDesktopManagement.NEW_FOLDER_NAME}','{webDesktopManagement.PREV_FOLDER_NAME}','{webDesktopManagement.TEMPLATE_CODE}','{webDesktopManagement.HREF}','{DATA.MODULE_CODE}','{webDesktopManagement.FUNCTION_LINK}','{DATA.FORM_TYPE}','{webDesktopManagement.COLOR}','{webDesktopManagement.ICON_PATH}','{webDesktopManagement.ABBR}','{DATA.FORM_EDESC}','{webDesktopManagement.MENU_NO}','{webDesktopManagement.MENU_EDESC}','{webDesktopManagement.MENU_DESC}','{DRAG_COUNT}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.User_id}','{_workContext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N','{webDesktopManagement.UNIQUE_ID}','{SIDEBAR_COUNT}')";
            _dbContext.ExecuteSqlCommand(inserfolder);
            //FETCH ALL DATA FROM WEB_DESKTOP_MANAGEMENT
            var WEB_DESKTOP_MANAGEMENT_QUERY = $@" SELECT TO_CHAR(WEB_DESKTOP_MANAGEMENT_ID) AS WEB_DESKTOP_MANAGEMENT_ID,MENU_DESC AS MENU_EDESC, MENU_NO,TO_CHAR(FORM_CODE) AS FORM_CODE,NEW_FOLDER_NAME,PREV_FOLDER_NAME,TO_CHAR(TEMPLATE_CODE)
                                            AS TEMPLATE_CODE,HREF,TO_CHAR(MODULE_CODE) AS MODULE_CODE,FUNCTION_LINK,FORM_TYPE,COLOR,ICON_PATH,ABBR,FORM_EDESC
                                            FROM WEB_DESKTOP_MANAGEMENT A
                                            WHERE  COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}'
                                            AND DRAG_COUNT IN (SELECT MAX(DRAG_COUNT) FROM WEB_DESKTOP_MANAGEMENT WHERE (MENU_NO = A.MENU_NO  OR  FORM_CODE = A.FORM_CODE  OR TEMPLATE_CODE = A.TEMPLATE_CODE )) 
                                           ORDER BY WEB_DESKTOP_MANAGEMENT_ID ASC";
            Record = this._dbContext.SqlQuery<WebDesktopManagement>(WEB_DESKTOP_MANAGEMENT_QUERY).ToList();
            return Record;
        }
        public List<WebDesktopManagement> GetFolderTemplateByUserId()
        {
            _logErp.InfoInFile("GetFolderTemplateByUserId started=========");
            List<WebDesktopManagement> Record = new List<WebDesktopManagement>();
            List<WebDesktopManagement> Record2 = new List<WebDesktopManagement>();
            List<WebDesktopManagement> Record3 = new List<WebDesktopManagement>();
            var WEB_DESKTOP_FOLDER_QUERY = $@" SELECT TO_CHAR(WEB_DESKTOP_MANAGEMENT_ID) AS WEB_DESKTOP_MANAGEMENT_ID,MENU_EDESC,REPLACE(MENU_EDESC,' ','-') AS MENU_DESC, MENU_NO,TO_CHAR(FORM_CODE) AS FORM_CODE,NEW_FOLDER_NAME,PREV_FOLDER_NAME,TO_CHAR(TEMPLATE_CODE)
                                            AS TEMPLATE_CODE,HREF,TO_CHAR(MODULE_CODE) AS MODULE_CODE,FUNCTION_LINK,FORM_TYPE,COLOR,ICON_PATH,ABBR,FORM_EDESC, TO_CHAR(A.UNIQUE_ID) AS UNIQUE_ID,TO_CHAR(A.SIDEBAR_ID) AS SIDEBAR_ID
                                            FROM WEB_DESKTOP_MANAGEMENT A
                                            WHERE  COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' 
                                            AND DRAG_COUNT IN (SELECT MAX(DRAG_COUNT) FROM WEB_DESKTOP_MANAGEMENT WHERE UNIQUE_ID=A.UNIQUE_ID) 
                                            AND  FORM_CODE IN (SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO = '{_workContext.CurrentUserinformation.User_id}' AND CREATE_FLAG = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' )
                                            ORDER BY TO_NUMBER(WEB_DESKTOP_MANAGEMENT_ID) ASC";
            Record = _dbContext.SqlQuery<WebDesktopManagement>(WEB_DESKTOP_FOLDER_QUERY).ToList();
            var WEB_DESKTOP_FOLDER_MENU_QUERY = $@" SELECT TO_CHAR(WEB_DESKTOP_MANAGEMENT_ID) AS WEB_DESKTOP_MANAGEMENT_ID,MENU_EDESC,REPLACE(MENU_EDESC,' ','-') AS MENU_DESC, MENU_NO,TO_CHAR(FORM_CODE) AS FORM_CODE,NEW_FOLDER_NAME,PREV_FOLDER_NAME,TO_CHAR(TEMPLATE_CODE)
                                            AS TEMPLATE_CODE,HREF,TO_CHAR(MODULE_CODE) AS MODULE_CODE,FUNCTION_LINK,FORM_TYPE,COLOR,ICON_PATH,ABBR,FORM_EDESC, TO_CHAR(A.UNIQUE_ID) AS UNIQUE_ID,TO_CHAR(A.SIDEBAR_ID) AS SIDEBAR_ID
                                            FROM WEB_DESKTOP_MANAGEMENT A
                                            WHERE  COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' 
                                            AND DRAG_COUNT IN (SELECT MAX(DRAG_COUNT) FROM WEB_DESKTOP_MANAGEMENT WHERE UNIQUE_ID=A.UNIQUE_ID) 
                                            AND A.MENU_NO IN (SELECT MENU_NO FROM WEB_MENU_CONTROL WHERE ACCESS_FLAG='Y' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND USER_NO='{_workContext.CurrentUserinformation.User_id}')
                                            ORDER BY TO_NUMBER(WEB_DESKTOP_MANAGEMENT_ID) ASC";
            Record2 = _dbContext.SqlQuery<WebDesktopManagement>(WEB_DESKTOP_FOLDER_MENU_QUERY).ToList();
            Record.AddRange(Record2);
            var WEB_DESKTOP_MENU_FOLDER_QUERY = $@"SELECT TO_CHAR(WEB_DESKTOP_MANAGEMENT_ID) AS WEB_DESKTOP_MANAGEMENT_ID, MENU_EDESC, REPLACE(MENU_EDESC, ' ', '-') AS MENU_DESC, MENU_NO, TO_CHAR(FORM_CODE) AS FORM_CODE, NEW_FOLDER_NAME, PREV_FOLDER_NAME, TO_CHAR(TEMPLATE_CODE)
                                            AS TEMPLATE_CODE, HREF, TO_CHAR(MODULE_CODE) AS MODULE_CODE, FUNCTION_LINK, FORM_TYPE, COLOR, ICON_PATH, ABBR, FORM_EDESC, TO_CHAR(A.UNIQUE_ID) AS UNIQUE_ID, TO_CHAR(A.SIDEBAR_ID) AS SIDEBAR_ID
                                            FROM WEB_DESKTOP_MANAGEMENT A
                                            WHERE   COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' 
                                            AND DRAG_COUNT IN(SELECT MAX(DRAG_COUNT) FROM WEB_DESKTOP_MANAGEMENT WHERE UNIQUE_ID = A.UNIQUE_ID) AND A.NEW_FOLDER_NAME = 'Menu'
                                             ORDER BY TO_NUMBER(WEB_DESKTOP_MANAGEMENT_ID) ASC";
            Record3 = _dbContext.SqlQuery<WebDesktopManagement>(WEB_DESKTOP_MENU_FOLDER_QUERY).ToList();
            Record.AddRange(Record3);
            return Record;
        }
        public bool MenuNoExistsOrNot(string MENU_NO)
        {
            var Count_query = $@"SELECT COUNT(MENU_NO) FROM WEB_DESKTOP_MANAGEMENT 
                          WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                          AND MENU_NO='{MENU_NO}' AND USER_ID='{_workContext.CurrentUserinformation.User_id}'";
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
        public bool FormNoExistsOrNot(string FORM_CODE)
        {
            var Count_query = $@"SELECT COUNT(FORM_CODE) FROM WEB_DESKTOP_MANAGEMENT 
                          WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                          AND FORM_CODE='{FORM_CODE}' AND USER_ID='{_workContext.CurrentUserinformation.User_id}'";
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
        public bool DraftTemplateExistsOrNot(string TEMPLATE_CODE)
        {
            var Count_query = $@"SELECT COUNT(TEMPLATE_CODE) FROM WEB_DESKTOP_MANAGEMENT 
                          WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                          AND TEMPLATE_CODE='{TEMPLATE_CODE}' AND USER_ID='{_workContext.CurrentUserinformation.User_id}'";
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
        public bool FolderExistsOrNot(string FOLDER_NAME)
        {
            var Count_query = $@"SELECT COUNT(FOLDER_NAME) FROM WEB_DESKTOP_FOLDER
                          WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                          AND FOLDER_NAME='{FOLDER_NAME}'
                          AND USER_ID='{_workContext.CurrentUserinformation.User_id}'";
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
        public void ResetWebManagement()
        {
            var Delete_Query = $@"DELETE FROM WEB_DESKTOP_MANAGEMENT
                          WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                          AND USER_ID='{_workContext.CurrentUserinformation.User_id}'";
            int Result = _dbContext.ExecuteSqlCommand(Delete_Query);

            var DeleteMenu_Query = $@"DELETE FROM WEB_DESKTOP_FOLDER
                          WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                          AND USER_ID='{_workContext.CurrentUserinformation.User_id}'";
            _dbContext.ExecuteSqlCommand(DeleteMenu_Query);
        }
        public void RemoveFolderByFolderId(string FOLDER_ID)
        {
            var DMD_query = $@"DELETE  FROM   WEB_DESKTOP_MANAGEMENT WHERE WEB_DESKTOP_MANAGEMENT_ID IN (SELECT WDM.WEB_DESKTOP_MANAGEMENT_ID AS WEB_DESKTOP_MANAGEMENT_ID FROM  WEB_DESKTOP_MANAGEMENT WDM  WHERE
                          NEW_FOLDER_NAME=(SELECT WDF.FOLDER_NAME AS FOLDER_NAME  FROM WEB_DESKTOP_FOLDER WDF WHERE WDF.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                          AND WDF.FOLDER_ID='{FOLDER_ID}' AND WDF.USER_ID='{_workContext.CurrentUserinformation.User_id}')
                          AND WDM.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND WDM.USER_ID='{_workContext.CurrentUserinformation.User_id}'
                          AND DRAG_COUNT IN (SELECT MAX(DRAG_COUNT) FROM WEB_DESKTOP_MANAGEMENT WHERE UNIQUE_ID=WDM.UNIQUE_ID)) ";
            _dbContext.ExecuteSqlCommand(DMD_query);

            var DELETE_QUERY = $@"DELETE FROM  WEB_DESKTOP_FOLDER WHERE
                          COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND FOLDER_ID='{FOLDER_ID}' AND USER_ID='{_workContext.CurrentUserinformation.User_id}'";
            _dbContext.ExecuteSqlCommand(DELETE_QUERY);
        }
        #endregion

        public decimal GetSatanderedRateByFilters(string customercode = "", string formcode = "", string areacode = "", string itemcode = "")
        {
            var query = string.Empty;

            decimal result = 0;
            query = $@"select STANDARD_RATE from IP_ITEM_RATE_SCHEDULE_SETUP where CS_CODE='{customercode}' and ITEM_CODE='{itemcode}' and FORM_CODE='{formcode}' and AREA_CODE='{areacode}' ORDER BY EFFECTIVE_DATE desc";
            result = _dbContext.SqlQuery<decimal>(query).FirstOrDefault();
            if (result <= 0)
            {
                query = $@"select STANDARD_RATE from IP_ITEM_RATE_SCHEDULE_SETUP where CS_CODE='{customercode}' and ITEM_CODE='{itemcode}' and FORM_CODE='{formcode}' ORDER BY EFFECTIVE_DATE desc";
                result = _dbContext.SqlQuery<decimal>(query).FirstOrDefault();
                if (result <= 0)
                {

                    query = $@"select STANDARD_RATE from IP_ITEM_RATE_SCHEDULE_SETUP where CS_CODE='{customercode}' and ITEM_CODE='{itemcode}'  ORDER BY EFFECTIVE_DATE desc";
                    result = _dbContext.SqlQuery<decimal>(query).FirstOrDefault();
                    if (result <= 0)
                    {
                        query = $@"select STANDARD_RATE from IP_ITEM_RATE_SCHEDULE_SETUP ORDER BY EFFECTIVE_DATE desc";
                        result = _dbContext.SqlQuery<decimal>(query).FirstOrDefault();
                        if (result <= 0)
                        {
                            return 0;
                        }
                        else
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return result;
                    }

                }
                else
                {
                    return result;
                }
            }
            else
            {
                return result;
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
        public List<ShippingDetailsViewModel> GetShippingDataByVoucherNo(string voucherNo)
        {
            try
            {
                string query = string.Empty;
                var result = new List<ShippingDetailsViewModel>();
                if (voucherNo != "undefined")
                {
                    query = $@"     SELECT ST.VEHICLE_CODE, ST.VEHICLE_OWNER_NAME, ST.VEHICLE_OWNER_NO, ST.DRIVER_NAME, ST.DRIVER_LICENSE_NO, ST.DRIVER_MOBILE_NO, ST.TRANSPORTER_CODE,TS.TRANSPORTER_EDESC,
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

        public decimal GetStockQuantity(string itemcodecode, string voucherdate, string locationcode)
        {
            try
            {
                string query = $@"SELECT SUM(IN_QUANTITY-OUT_QUANTITY) STOCK_QTY FROM V$VIRTUAL_STOCK_WIP_LEDGER1 WHERE LOCATION_CODE='{locationcode}' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND ITEM_CODE='{itemcodecode}' AND VOUCHER_DATE <=TO_DATE('" + voucherdate + "', 'YYYY-MM-DD')";
                string data = this._dbContext.SqlQuery<decimal>(query).FirstOrDefault().ToString();
                return Convert.ToDecimal(data);
            }
            catch (Exception)
            {
                return Convert.ToDecimal("9999999999");

            }
        }

        public string GetPrintTemplateByFormCode(string formcode)
        {
            try
            {

                string query = string.Format($@"select TEMPLATE_NAME from FORM_TEMPLATE_MAPPING where USER_ID='{_workContext.CurrentUserinformation.User_id}' and company_code='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}' and deleted_flag='N'");


                string voucherNo = this._dbContext.SqlQuery<string>(query).FirstOrDefault();


                return voucherNo;


            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PriceList> GetAllPriceListByFilterAndCustomerCode(string filter, string customercode)
        {


            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
            List<PriceList> result = new List<PriceList>();
            string query = $@"select MASTER_ID,COALESCE(PRICE_LIST_NAME,' ') as PRICE_LIST_NAME  FROM PRICE_SETUP_MASTER psm WHERE STATUS=1  AND MASTER_ID=(select  PRICE_LIST_ID from SA_CUSTOMER_SETUP where CUSTOMER_CODE='{customercode}' and company_code='{_workContext.CurrentUserinformation.company_code}' )  and (upper(PRICE_LIST_NAME) like '%{filter.ToUpperInvariant()}%' or upper(PRICE_LIST_NAME) like '%{filter.ToUpperInvariant()}%')  UNION ALL
          SELECT master_id, coalesce(price_list_name, ' ') AS price_list_name FROM price_setup_master WHERE status = 1 ";
            result = this._dbContext.SqlQuery<PriceList>(query).ToList();
            return result;
        }
        public decimal GetItemRateByMasterId(string masterid = "", string itemcode = "")
        {
            var query = string.Empty;

            decimal result = 0;
            query = $@"select OLD_PRICE from PRICE_SETUP_CHILD where MASTER_ID='{masterid}' and ITEM_CODE='{itemcode}' and STATUS='Yes'";
            result = _dbContext.SqlQuery<decimal>(query).FirstOrDefault();
            return result;
        }


        //public void UpdatePrintCount(string voucherno, string formcode)
        //{
        //    var countquery = $@"select nvl(PRINT_COUNT,0) as PRINT_COUNT from MASTER_TRANSACTION where VOUCHER_NO='{voucherno}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' and DELETED_FLAG='N' and FORM_CODE='{formcode}'";
        //    var cnt = _dbContext.SqlQuery<int>(countquery).FirstOrDefault() + 1;

        //    var UPDATE_QUERY = $@"UPDATE MASTER_TRANSACTION
        //                              SET PRINT_COUNT ={cnt} where VOUCHER_NO='{voucherno}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' and DELETED_FLAG='N' and FORM_CODE='{formcode}'";
        //    _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
        //}

        //public int GetPrintCountByVoucherNo(string VoucherNo, string formcode)
        //{
        //    var query = string.Empty;

        //    int result = 0;
        //    query = $@"select PRINT_COUNT from MASTER_TRANSACTION where VOUCHER_NO='{VoucherNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' and DELETED_FLAG='N' and FORM_CODE='{formcode}'";
        //    result = _dbContext.SqlQuery<int>(query).FirstOrDefault();
        //    return result;
        //}
        public void UpdatePrintCount(string voucherno, string formcode)
        {
            var printedTime = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            var countquery = $@"select nvl(PRINT_COUNT,0) as PRINT_COUNT from MASTER_TRANSACTION where VOUCHER_NO='{voucherno}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' and DELETED_FLAG='N' and FORM_CODE='{formcode}'";
            var cnt = _dbContext.SqlQuery<int>(countquery).FirstOrDefault() + 1;

            var UPDATE_QUERY = $@"UPDATE MASTER_TRANSACTION
                                      SET PRINT_COUNT ={cnt} , PRINT_FLAG='{"Y"}' , PRINTED_BY='{_workContext.CurrentUserinformation.login_code}' , PRINTED_TIME='{printedTime}',MODIFY_DATE=SYSDATE where VOUCHER_NO='{voucherno}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' and DELETED_FLAG='N' and FORM_CODE='{formcode}'";
            _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
        }

        public int GetPrintCountByVoucherNo(string VoucherNo, string formcode, string UpdatePrintCountFlag)
        {
            var query = string.Empty;

            int result = 0;
            query = $@"select nvl(PRINT_COUNT,0) as PRINT_COUNT from MASTER_TRANSACTION where VOUCHER_NO='{VoucherNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' and DELETED_FLAG='N' and FORM_CODE='{formcode}'";
            result = _dbContext.SqlQuery<int>(query).FirstOrDefault();
            if (UpdatePrintCountFlag == "true")
            {
                UpdatePrintCount(VoucherNo, formcode);
            }
            return result;
        }

        public CodeForLog CodeForLog()
        {
            try
            {
                var WEB_DESKTOP_FOLDER_QUERY = $@"SELECT TO_CHAR(FORM_CODE) AS FORM_CODE,TO_CHAR(MODULE_CODE) AS MODULE_CODE,FORM_TYPE,FORM_EDESC, TO_CHAR(A.UNIQUE_ID) AS UNIQUE_ID
                                            FROM WEB_DESKTOP_MANAGEMENT A
                                            WHERE  COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND DELETED_FLAG = 'N' AND USER_ID='{_workContext.CurrentUserinformation.User_id}' 
                                            AND  FORM_CODE IN (SELECT FORM_CODE FROM SC_FORM_CONTROL WHERE USER_NO = '{_workContext.CurrentUserinformation.User_id}' AND CREATE_FLAG = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}')";
                var codeForLog = _dbContext.SqlQuery<CodeForLog>(WEB_DESKTOP_FOLDER_QUERY).FirstOrDefault();
                return codeForLog;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public List<REFERENCE_DETAILS> GetReference_Details_For_VoucherNo(string VoucherNo, string formcode)
        {
            List<REFERENCE_DETAILS> result = new List<REFERENCE_DETAILS>();
            string query = $@"SELECT RD.REFERENCE_NO,RD.REFERENCE_ITEM_CODE,IMS.ITEM_EDESC,RD.REFERENCE_QUANTITY, RD.REFERENCE_MU_CODE,RD.REFERENCE_UNIT_PRICE,RD.REFERENCE_TOTAL_PRICE,RD.REFERENCE_CALC_UNIT_PRICE,RD.REFERENCE_CALC_TOTAL_PRICE,RD.REFERENCE_REMARKS FROM REFERENCE_DETAIL RD,IP_ITEM_MASTER_SETUP IMS WHERE RD.VOUCHER_NO='{VoucherNo}' AND IMS.ITEM_CODE=RD.REFERENCE_ITEM_CODE AND RD.COMPANY_CODE=IMS.COMPANY_CODE AND RD.BRANCH_CODE=IMS.BRANCH_CODE AND IMS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND IMS.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
            result = this._dbContext.SqlQuery<REFERENCE_DETAILS>(query).ToList();
            return result;

        }

        //public List<CHARGETRANSACTION> GetCharge_Details_For_VoucherNo(string VoucherNo, string formcode)
        //{
        //    List<CHARGETRANSACTION> result = new List<CHARGETRANSACTION>();
        //    string query = $@"SELECT 
        //                        CHARGE_CODE, 
        //                        CHARGE_TYPE_FLAG, 
        //                        CHARGE_AMOUNT, 
        //                        CURRENCY_CODE, 
        //                        EXCHANGE_RATE, 
        //                        REFERENCE_NO, 
        //                        CALC_UNIT_PRICE, 
        //                        CALC_TOTAL_PRICE,
        //                        ITEM_CODE,
        //                        QUANTITY,
        //                        FORM_CODE,
        //                        ACC_CODE,
        //                        APPLY_ON,
        //                        GL_FLAG,
        //                        APPORTION_ON,
        //                        IMPACT_ON,
        //                        SERIAL_NO
        //                    FROM CHARGE_TRANSACTION CT
        //                    WHERE CT.VOUCHER_NO='{VoucherNo}' 
        //                    AND IMS.ITEM_CODE=RD.REFERENCE_ITEM_CODE 
        //                    AND RD.COMPANY_CODE=IMS.COMPANY_CODE 
        //                    AND RD.BRANCH_CODE=IMS.BRANCH_CODE 
        //                    AND IMS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
        //                    AND IMS.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
        //}
        public void UpdateMasterTranasactionForVerification(string orderNo, string formcode, string mode, out string message, out bool status)
        {
            using (var trans = _coreentity.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(formcode))
                    {
                        string modulecodequery = $@"SELECT MODULE_CODE FROM FORM_SETUP WHERE FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.Company}'";

                        string modulecode = this._dbContext.SqlQuery<string>(modulecodequery).FirstOrDefault().ToString();
                        if (mode == "Post")
                        {

                            {
                                if (modulecode == "01")
                                {
                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
                                    if (rowCount == 1)
                                    {
                                        string queryNew = $@"INSERT INTO FA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{orderNo}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                                        var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
                                        if (rowCountNew == 1)
                                        {
                                            trans.Commit();
                                            message = "SUCCESS";
                                            status = true;
                                        }
                                        else
                                        {
                                            trans.Rollback();
                                            message = "FAILED";
                                            status = false;
                                        }
                                    }
                                    else
                                    {
                                        trans.Rollback();
                                        message = "FAILED";
                                        status = false;
                                    }
                                }
                                else
                                {
                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
                                    if (rowCount == 1)
                                    {

                                        //if (FindStopUSI() == "INVALID" || FindStopDSRT() == "INVALID" || FindStopUSR() == "INVALID")
                                        //{
                                        //    message = "INVALIDTRIGGER";
                                        //    status = false;
                                        //}
                                        //else
                                        //{
                                        //}


                                        string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{orderNo}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                                        var result = _coreentity.ExecuteSqlCommand2(queryNew);
                                        if (result.Success && result.RowsAffected > 0)
                                        {
                                            trans.Commit();
                                            message = "SUCCESS";
                                            status = true;
                                        }
                                        else
                                        {
                                            trans.Rollback();
                                            message = result.Error ?? "FAILED";
                                            status = false;
                                        }
                                    }
                                    else
                                    {
                                        trans.Rollback();
                                        message = "FAILED";
                                        status = false;
                                    }
                                }

                            }

                        }
                        else if (mode == "UnPost")
                        {
                            if (modulecode == "01")
                            {
                                string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL  where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                var rowCount = _coreentity.ExecuteSqlCommand(query);
                                if (rowCount == 1)
                                {
                                    //if (FindStopUSI() == "INVALID" || FindStopDSRT() == "INVALID" || FindStopUSR() == "INVALID")
                                    //{
                                    //    message = "INVALIDTRIGGER";
                                    //    status = false;
                                    //}
                                    //else {
                                    string queryNew = $@"DELETE FROM FA_POSTED_TRANSACTION WHERE VOUCHER_NO='{orderNo}' AND FORM_CODE='{formcode}' AND POSTED_BY='{_workContext.CurrentUserinformation.login_code}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
                                    var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
                                    if (rowCountNew == 1)
                                    {
                                        trans.Commit();
                                        message = "SUCCESS";
                                        status = true;
                                    }
                                    else
                                    {
                                        trans.Rollback();
                                        message = "FAILED";
                                        status = false;
                                    }
                                    //}

                                }
                                else
                                {
                                    trans.Rollback();
                                    message = "FAILED";
                                    status = false;
                                }
                            }
                            else
                            {
                                if (FindStopUSI() == "INVALID" || FindStopDSRT() == "INVALID" || FindStopUSR() == "INVALID")
                                {
                                    message = "INVALIDTRIGGER";
                                    status = false;
                                }
                                else
                                {
                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL  where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
                                    if (rowCount == 1)
                                    {
                                        string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{orderNo}' AND FORM_CODE='{formcode}' AND POSTED_BY='{_workContext.CurrentUserinformation.login_code}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
                                        var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
                                        if (rowCountNew == 1)
                                        {
                                            trans.Commit();
                                            message = "SUCCESS";
                                            status = true;
                                        }
                                        else
                                        {
                                            trans.Rollback();
                                            message = "FAILED";
                                            status = false;
                                        }
                                    }
                                    else
                                    {
                                        trans.Rollback();
                                        message = "FAILED";
                                        status = false;
                                    }

                                }


                            }

                        }
                        else if (mode == "Authorise")
                        {
                            string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = '{_workContext.CurrentUserinformation.login_code}', AUTHORISED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                            var rowCount = _coreentity.ExecuteSqlCommand(query);
                            if (rowCount == 1)
                            {
                                trans.Commit();
                                message = "SUCCESS";
                                status = true;
                            }
                            else
                            {
                                trans.Rollback();
                                message = "FAILED";
                                status = false;
                            }
                        }
                        else if (mode == "Authorise")
                        {
                            string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = NULL, AUTHORISED_DATE = NULL where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                            var rowCount = _coreentity.ExecuteSqlCommand(query);
                            if (rowCount == 1)
                            {
                                trans.Commit();
                                message = "SUCCESS";
                                status = true;
                            }
                            else
                            {
                                trans.Rollback();
                                message = "FAILED";
                                status = false;
                            }
                        }
                        else if (mode == "Check")
                        {
                            string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = '{_workContext.CurrentUserinformation.login_code}', CHECKED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                            var rowCount = _coreentity.ExecuteSqlCommand(query);
                            if (rowCount == 1)
                            {
                                trans.Commit();
                                message = "SUCCESS";
                                status = true;
                            }
                            else
                            {
                                trans.Rollback();
                                message = "FAILED";
                                status = false;
                            }
                        }
                        else if (mode == "UnCheck")
                        {
                            string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = NULL, CHECKED_DATE = NULL where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                            var rowCount = _coreentity.ExecuteSqlCommand(query);
                            if (rowCount == 1)
                            {
                                trans.Commit();
                                message = "SUCCESS";
                                status = true;
                            }
                            else
                            {
                                trans.Rollback();
                                message = "FAILED";
                                status = false;
                            }
                        }
                        else
                        {
                            trans.Rollback();
                            message = "FAILED";
                            status = false;
                        }
                    }
                    else
                    {
                        trans.Rollback();
                        message = "FAILED";
                        status = false;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    message = ex.Message;
                    status = false;
                }
            }
        }

        public CustomerMasterMapModel GetCustomerMasterMap()
        {
            try
            {
                string query = $@"SELECT SAS.CUSTOMER_CODE AS PARENT_GROUP,
                               SCMM.ACC_CODE AS ACC_CODE,
                               SCMM.PARTY_TYPE_CODE AS PARTY_TYPE_CODE
                          FROM    SA_CUSTOMER_MASTER_MAP SCMM
                               LEFT JOIN
                                  SA_CUSTOMER_SETUP SAS
                               ON     SCMM.PRE_CUSTOMER_CODE = SAS.MASTER_CUSTOMER_CODE
                                  AND SCMM.COMPANY_CODE = SAS.COMPANY_CODE
                                  AND SCMM.BRANCH_CODE = SAS.BRANCH_CODE
                                  AND SAS.DELETED_FLAG = 'N'
                         WHERE     SCMM.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                               AND SCMM.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                               AND SCMM.DELETED_FLAG = 'N'";
                var result = _dbContext.SqlQuery<CustomerMasterMapModel>(query).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void BulkUpdateMasterTranasactionForVerification(List<string> voucherNo, string formcode, string mode, out string message, out bool status)
        {
            using (var trans = _coreentity.Database.BeginTransaction())
            {
                try
                {
                    string Query = string.Empty;
                    if (voucherNo.Count > 0)
                    {
                        message = "";
                        status = false;
                        int count = 0;
                        foreach (var voucherNum in voucherNo)
                        {
                            if (!string.IsNullOrEmpty(voucherNum) && !string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(formcode))
                            {
                                string modulecodequery = $@"SELECT MODULE_CODE FROM FORM_SETUP WHERE FORM_CODE={formcode} AND COMPANY_CODE={_workContext.CurrentUserinformation.Company}";

                                string modulecode = this._dbContext.SqlQuery<string>(modulecodequery).FirstOrDefault().ToString();

                                if (mode == "Post")
                                {
                                    if (modulecode == "01")
                                    {
                                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
                                        if (rowCount == 1)
                                        {
                                            string queryNew = $@"INSERT INTO FA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{voucherNum}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
                                            if (rowCountNew == 1)
                                            {
                                                count++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (FindStopUSI() == "INVALID" || FindStopDSRT() == "INVALID" || FindStopUSR() == "INVALID")
                                        {
                                            message = "INVALIDTRIGGER";
                                            status = false;
                                        }
                                        else
                                        {
                                            string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                            var rowCount = _coreentity.ExecuteSqlCommand(query);
                                            if (rowCount == 1)
                                            {
                                                string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{voucherNum}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                                                var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
                                                if (rowCountNew == 1)
                                                {
                                                    count++;
                                                }
                                            }
                                        }
                                    }

                                }
                                if (mode == "UnPost")
                                {
                                    if (modulecode == "01")
                                    {
                                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
                                        if (rowCount == 1)
                                        {
                                            string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{voucherNum}' AND FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
                                            if (rowCountNew == 1)
                                            {
                                                count++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (FindStopUSI() == "INVALID" || FindStopDSRT() == "INVALID" || FindStopUSR() == "INVALID")
                                        {
                                            message = "INVALIDTRIGGER";
                                            status = false;
                                        }
                                        else
                                        {
                                            string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                            var rowCount = _coreentity.ExecuteSqlCommand(query);
                                            if (rowCount == 1)
                                            {
                                                string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{voucherNum}' AND FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                                                var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
                                                if (rowCountNew == 1)
                                                {
                                                    count++;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (mode == "Authorise")
                                {
                                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = '{_workContext.CurrentUserinformation.login_code}', AUTHORISED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
                                    if (rowCount == 1)
                                    {
                                        count++;
                                        //AutoCVP(voucherNum,formcode, _coreentity);
                                    }
                                }
                                if (mode == "UnAuthorise")
                                {
                                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = NULL, AUTHORISED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
                                    if (rowCount == 1)
                                    {
                                        count++;
                                        //AutoCVP(voucherNum,formcode, _coreentity);
                                    }
                                }
                                if (mode == "Check")
                                {
                                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = '{_workContext.CurrentUserinformation.login_code}', CHECKED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
                                    if (rowCount == 1)
                                    {
                                        count++;
                                        //AutoCVP(voucherNum,formcode, _coreentity);
                                    }
                                }
                                if (mode == "UnCheck")
                                {
                                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = NULL, CHECKED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
                                    if (rowCount == 1)
                                    {
                                        count++;
                                        //AutoCVP(voucherNum,formcode, _coreentity);
                                    }
                                }
                            }
                            else
                            {
                                message = "voucherNum is empty.";
                                status = false;
                                break;
                            }
                        }
                        if (voucherNo.Count == count)
                        {
                            //trans.Rollback();
                            trans.Commit();
                            message = string.Format("Data " + mode + " successfull.");
                            status = true;
                        }
                        else
                        {
                            trans.Rollback();
                            message = "Voucher number count doesnot match with update count.";
                            status = false;
                        }
                    }
                    else
                    {
                        message = "List empty.";
                        status = false;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    message = ex.Message;
                    status = false;
                }
            }
        }
        //public void UpdateMasterTranasactionForVerification(string orderNo, string formcode, string mode, out string message, out bool status)
        //{
        //    using (var trans = _coreentity.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var errormsg = string.Empty;
        //            if (!string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(formcode))
        //            {
        //                string tquery = $@"select distinct table_name from form_detail_setup where form_code='{formcode}' and company_code='{_workContext.CurrentUserinformation.company_code}'";
        //                string tname = this._dbContext.SqlQuery<string>(tquery).FirstOrDefault().ToString();
        //                if (mode == "Post")
        //                {

        //                    if (tname.Contains("INVOICE"))
        //                    {

        //                        if (FindStopUSI() == "INVALID")
        //                        {
        //                            errormsg = "INVALIDTRIGGER";
        //                            status = false;

        //                        }
        //                        else
        //                        {
        //                            string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                            var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                            if (rowCount == 1)
        //                            {
        //                                string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{orderNo}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                                var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                if (rowCountNew == 1)
        //                                {
        //                                    trans.Commit();
        //                                    message = "SUCCESS";
        //                                    status = true;
        //                                }
        //                                else
        //                                {
        //                                    trans.Rollback();
        //                                    message = "FAILED";
        //                                    status = false;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "FAILED";
        //                                status = false;
        //                            }
        //                        }
        //                    }
        //                    else if (tname.Contains("RETURN"))
        //                    {
        //                        if (FindStopDSRT() == "INVALID" || FindStopUSR() == "INVALID")
        //                        {
        //                            errormsg = "INVALIDTRIGGER";
        //                            status = false;
        //                        }
        //                        else
        //                        {
        //                            string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                            var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                            if (rowCount == 1)
        //                            {
        //                                string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{orderNo}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                                var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                if (rowCountNew == 1)
        //                                {
        //                                    trans.Commit();
        //                                    message = "SUCCESS";
        //                                    status = true;
        //                                }
        //                                else
        //                                {
        //                                    trans.Rollback();
        //                                    message = "FAILED";
        //                                    status = false;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "FAILED";
        //                                status = false;
        //                            }
        //                        }

        //                    }
        //                    else if (tname.Contains("CHALAN"))
        //                    {
        //                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                        if (rowCount == 1)
        //                        {
        //                            string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{orderNo}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                            if (rowCountNew == 1)
        //                            {
        //                                trans.Commit();
        //                                message = "SUCCESS";
        //                                status = true;
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "FAILED";
        //                                status = false;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            trans.Rollback();
        //                            message = "FAILED";
        //                            status = false;
        //                        }
        //                    }
        //                    else if (tname.Contains("DOUBLE_VOUCHER"))
        //                    {
        //                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                        if (rowCount == 1)
        //                        {
        //                            string queryNew = $@"INSERT INTO FA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{orderNo}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                            if (rowCountNew == 1)
        //                            {
        //                                trans.Commit();
        //                                message = "SUCCESS";
        //                                status = true;
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "FAILED";
        //                                status = false;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            trans.Rollback();
        //                            message = "FAILED";
        //                            status = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        trans.Rollback();
        //                        message = "FAILED";
        //                        status = false;
        //                    }
        //                }
        //                if (mode == "UnPost")
        //                {
        //                    if (tname.Contains("INVOICE"))
        //                    {

        //                        if (FindStopUSI() == "INVALID")
        //                        {
        //                            message = "INVALIDTRIGGER";
        //                            status = false;

        //                        }
        //                        else
        //                        {
        //                            string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL  where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                            var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                            if (rowCount == 1)
        //                            {
        //                                string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{orderNo}' AND FORM_CODE='{formcode}' AND POSTED_BY='{_workContext.CurrentUserinformation.login_code}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
        //                                var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                if (rowCountNew == 1)
        //                                {
        //                                    trans.Commit();
        //                                    message = "SUCCESS";
        //                                    status = true;
        //                                }
        //                                else
        //                                {
        //                                    trans.Rollback();
        //                                    message = "FAILED";
        //                                    status = false;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "FAILED";
        //                                status = false;
        //                            }
        //                        }
        //                    }
        //                    else if (tname.Contains("RETURN"))
        //                    {
        //                        if (FindStopDSRT() == "INVALID" || FindStopUSR() == "INVALID")
        //                        {
        //                            message = "INVALIDTRIGGER";
        //                            status = false;
        //                        }
        //                        else
        //                        {
        //                            string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL  where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                            var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                            if (rowCount == 1)
        //                            {
        //                                string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{orderNo}' AND FORM_CODE='{formcode}' AND POSTED_BY='{_workContext.CurrentUserinformation.login_code}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
        //                                var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                if (rowCountNew == 1)
        //                                {
        //                                    trans.Commit();
        //                                    message = "SUCCESS";
        //                                    status = true;
        //                                }
        //                                else
        //                                {
        //                                    trans.Rollback();
        //                                    message = "FAILED";
        //                                    status = false;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "FAILED";
        //                                status = false;
        //                            }
        //                        }
        //                    }
        //                    else if (tname.Contains("CHALAN"))
        //                    {
        //                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL  where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                        if (rowCount == 1)
        //                        {
        //                            string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{orderNo}' AND FORM_CODE='{formcode}' AND POSTED_BY='{_workContext.CurrentUserinformation.login_code}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
        //                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                            if (rowCountNew == 1)
        //                            {
        //                                trans.Commit();
        //                                message = "SUCCESS";
        //                                status = true;
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "FAILED";
        //                                status = false;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            trans.Rollback();
        //                            message = "FAILED";
        //                            status = false;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        trans.Rollback();
        //                        message = "FAILED";
        //                        status = false;
        //                    }

        //                }
        //                else if (mode == "Authorise")
        //                {
        //                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = '{_workContext.CurrentUserinformation.login_code}', AUTHORISED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                    if (rowCount == 1)
        //                    {
        //                        trans.Commit();
        //                        message = "SUCCESS";
        //                        status = true;
        //                    }
        //                    else
        //                    {
        //                        trans.Rollback();
        //                        message = "FAILED";
        //                        status = false;
        //                    }
        //                }
        //                else if (mode == "Authorise")
        //                {
        //                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = NULL, AUTHORISED_DATE = NULL where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                    if (rowCount == 1)
        //                    {
        //                        trans.Commit();
        //                        message = "SUCCESS";
        //                        status = true;
        //                    }
        //                    else
        //                    {
        //                        trans.Rollback();
        //                        message = "FAILED";
        //                        status = false;
        //                    }
        //                }
        //                else if (mode == "Check")
        //                {
        //                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = '{_workContext.CurrentUserinformation.login_code}', CHECKED_DATE = SYSDATE where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                    if (rowCount == 1)
        //                    {
        //                        trans.Commit();
        //                        message = "SUCCESS";
        //                        status = true;
        //                    }
        //                    else
        //                    {
        //                        trans.Rollback();
        //                        message = "FAILED";
        //                        status = false;
        //                    }
        //                }
        //                else if (mode == "UnCheck")
        //                {
        //                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = NULL, CHECKED_DATE = NULL where VOUCHER_NO='{orderNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                    if (rowCount == 1)
        //                    {
        //                        trans.Commit();
        //                        message = "SUCCESS";
        //                        status = true;
        //                    }
        //                    else
        //                    {
        //                        trans.Rollback();
        //                        message = "FAILED";
        //                        status = false;
        //                    }
        //                }
        //                else
        //                {
        //                    trans.Rollback();
        //                    if (errormsg != "")
        //                    {
        //                        message = errormsg;
        //                    }
        //                    else
        //                    {
        //                        message = "FAILED";
        //                    }

        //                    status = false;
        //                }
        //            }
        //            else
        //            {
        //                trans.Rollback();
        //                if (errormsg != "")
        //                {
        //                    message = errormsg;
        //                }
        //                else {
        //                    message = "FAILED";
        //                }

        //                status = false;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            trans.Rollback();
        //            message = ex.Message;
        //            status = false;
        //        }
        //    }
        //}
        //public void BulkUpdateMasterTranasactionForVerification(List<string> voucherNo, string formcode, string mode, out string message, out bool status)
        //{
        //    using (var trans = _coreentity.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(formcode))
        //            {
        //                string tquery = $@"select distinct table_name from form_detail_setup where form_code='{formcode}' and company_code='{_workContext.CurrentUserinformation.company_code}'";
        //                string tname = this._dbContext.SqlQuery<string>(tquery).FirstOrDefault().ToString();
        //                if (tname.Contains("INVOICE"))
        //                {

        //                    if (FindStopUSI() == "INVALID")
        //                    {
        //                        message = "INVALIDTRIGGER";
        //                        status = false;
        //                    }
        //                    else
        //                    {
        //                        string Query = string.Empty;
        //                        if (voucherNo.Count > 0)
        //                        {
        //                            message = "";
        //                            status = false;
        //                            int count = 0;
        //                            foreach (var voucherNum in voucherNo)
        //                            {
        //                                if (!string.IsNullOrEmpty(voucherNum) && !string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(formcode))
        //                                {
        //                                    if (mode == "Post")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{voucherNum}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                            if (rowCountNew == 1)
        //                                            {
        //                                                count++;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (mode == "UnPost")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{voucherNum}' AND FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
        //                                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                            if (rowCountNew == 1)
        //                                            {
        //                                                count++;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (mode == "Authorise")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = '{_workContext.CurrentUserinformation.login_code}', AUTHORISED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            count++;
        //                                            //AutoCVP(voucherNum,formcode, _coreentity);
        //                                        }
        //                                    }
        //                                    if (mode == "UnAuthorise")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = NULL, AUTHORISED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            count++;
        //                                            //AutoCVP(voucherNum,formcode, _coreentity);
        //                                        }
        //                                    }
        //                                    if (mode == "Check")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = '{_workContext.CurrentUserinformation.login_code}', CHECKED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            count++;
        //                                            //AutoCVP(voucherNum,formcode, _coreentity);
        //                                        }
        //                                    }
        //                                    if (mode == "UnCheck")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = NULL, CHECKED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            count++;
        //                                            //AutoCVP(voucherNum,formcode, _coreentity);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    message = "voucherNum is empty.";
        //                                    status = false;
        //                                    break;
        //                                }
        //                            }
        //                            if (voucherNo.Count == count)
        //                            {
        //                                //trans.Rollback();
        //                                trans.Commit();
        //                                message = string.Format("Data " + mode + " successfull.");
        //                                status = true;
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "Voucher number count doesnot match with update count.";
        //                                status = false;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            message = "List empty.";
        //                            status = false;
        //                        }
        //                    }
        //                }
        //                else if (tname.Contains("RETURN"))
        //                {
        //                    if (FindStopDSRT() == "INVALID" || FindStopUSR() == "INVALID")
        //                    {
        //                        message = "INVALIDTRIGGER";
        //                        status = false;
        //                    }
        //                    else
        //                    {
        //                        string Query = string.Empty;
        //                        if (voucherNo.Count > 0)
        //                        {
        //                            message = "";
        //                            status = false;
        //                            int count = 0;
        //                            foreach (var voucherNum in voucherNo)
        //                            {
        //                                if (!string.IsNullOrEmpty(voucherNum) && !string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(formcode))
        //                                {
        //                                    if (mode == "Post")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{voucherNum}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                            if (rowCountNew == 1)
        //                                            {
        //                                                count++;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (mode == "UnPost")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{voucherNum}' AND FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
        //                                            var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                            if (rowCountNew == 1)
        //                                            {
        //                                                count++;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (mode == "Authorise")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = '{_workContext.CurrentUserinformation.login_code}', AUTHORISED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            count++;
        //                                            //AutoCVP(voucherNum,formcode, _coreentity);
        //                                        }
        //                                    }
        //                                    if (mode == "UnAuthorise")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = NULL, AUTHORISED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            count++;
        //                                            //AutoCVP(voucherNum,formcode, _coreentity);
        //                                        }
        //                                    }
        //                                    if (mode == "Check")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = '{_workContext.CurrentUserinformation.login_code}', CHECKED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            count++;
        //                                            //AutoCVP(voucherNum,formcode, _coreentity);
        //                                        }
        //                                    }
        //                                    if (mode == "UnCheck")
        //                                    {
        //                                        string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = NULL, CHECKED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                        var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                        if (rowCount == 1)
        //                                        {
        //                                            count++;
        //                                            //AutoCVP(voucherNum,formcode, _coreentity);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    message = "voucherNum is empty.";
        //                                    status = false;
        //                                    break;
        //                                }
        //                            }
        //                            if (voucherNo.Count == count)
        //                            {
        //                                //trans.Rollback();
        //                                trans.Commit();
        //                                message = string.Format("Data " + mode + " successfull.");
        //                                status = true;
        //                            }
        //                            else
        //                            {
        //                                trans.Rollback();
        //                                message = "Voucher number count doesnot match with update count.";
        //                                status = false;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            message = "List empty.";
        //                            status = false;
        //                        }
        //                    }

        //                }
        //                else if (tname.Contains("CHALAN"))
        //                {
        //                    string Query = string.Empty;
        //                    if (voucherNo.Count > 0)
        //                    {
        //                        message = "";
        //                        status = false;
        //                        int count = 0;
        //                        foreach (var voucherNum in voucherNo)
        //                        {
        //                            if (!string.IsNullOrEmpty(voucherNum) && !string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(formcode))
        //                            {
        //                                if (mode == "Post")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{voucherNum}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                                        var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                        if (rowCountNew == 1)
        //                                        {
        //                                            count++;
        //                                        }
        //                                    }
        //                                }
        //                                if (mode == "UnPost")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{voucherNum}' AND FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
        //                                        var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                        if (rowCountNew == 1)
        //                                        {
        //                                            count++;
        //                                        }
        //                                    }
        //                                }
        //                                if (mode == "Authorise")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = '{_workContext.CurrentUserinformation.login_code}', AUTHORISED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "UnAuthorise")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = NULL, AUTHORISED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "Check")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = '{_workContext.CurrentUserinformation.login_code}', CHECKED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "UnCheck")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = NULL, CHECKED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                message = "voucherNum is empty.";
        //                                status = false;
        //                                break;
        //                            }
        //                        }
        //                        if (voucherNo.Count == count)
        //                        {
        //                            //trans.Rollback();
        //                            trans.Commit();
        //                            message = string.Format("Data " + mode + " successfull.");
        //                            status = true;
        //                        }
        //                        else
        //                        {
        //                            trans.Rollback();
        //                            message = "Voucher number count doesnot match with update count.";
        //                            status = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        message = "List empty.";
        //                        status = false;
        //                    }
        //                }
        //                else if (tname.Contains("DOUBLE_VOUCHER"))
        //                {
        //                    string Query = string.Empty;
        //                    if (voucherNo.Count > 0)
        //                    {
        //                        message = "";
        //                        status = false;
        //                        int count = 0;
        //                        foreach (var voucherNum in voucherNo)
        //                        {
        //                            if (!string.IsNullOrEmpty(voucherNum) && !string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(formcode))
        //                            {
        //                                if (mode == "Post")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        string queryNew = $@"INSERT INTO FA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{voucherNum}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                                        var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                        if (rowCountNew == 1)
        //                                        {
        //                                            count++;
        //                                        }
        //                                    }
        //                                }
        //                                if (mode == "UnPost")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{voucherNum}' AND FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
        //                                        var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                        if (rowCountNew == 1)
        //                                        {
        //                                            count++;
        //                                        }
        //                                    }
        //                                }
        //                                if (mode == "Authorise")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = '{_workContext.CurrentUserinformation.login_code}', AUTHORISED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "UnAuthorise")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = NULL, AUTHORISED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "Check")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = '{_workContext.CurrentUserinformation.login_code}', CHECKED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "UnCheck")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = NULL, CHECKED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                message = "voucherNum is empty.";
        //                                status = false;
        //                                break;
        //                            }
        //                        }
        //                        if (voucherNo.Count == count)
        //                        {
        //                            //trans.Rollback();
        //                            trans.Commit();
        //                            message = string.Format("Data " + mode + " successfull.");
        //                            status = true;
        //                        }
        //                        else
        //                        {
        //                            trans.Rollback();
        //                            message = "Voucher number count doesnot match with update count.";
        //                            status = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        message = "List empty.";
        //                        status = false;
        //                    }
        //                }
        //                else 
        //                {
        //                    string Query = string.Empty;
        //                    if (voucherNo.Count > 0)
        //                    {
        //                        message = "";
        //                        status = false;
        //                        int count = 0;
        //                        foreach (var voucherNum in voucherNo)
        //                        {
        //                            if (!string.IsNullOrEmpty(voucherNum) && !string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(formcode))
        //                            {
        //                                if (mode == "Post")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = '{_workContext.CurrentUserinformation.login_code}', POSTED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        string queryNew = $@"INSERT INTO SA_POSTED_TRANSACTION(VOUCHER_NO,FORM_CODE,POSTED_BY, COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{voucherNum}','{formcode}','{_workContext.CurrentUserinformation.login_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                                        var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                        if (rowCountNew == 1)
        //                                        {
        //                                            count++;
        //                                        }
        //                                    }
        //                                }
        //                                if (mode == "UnPost")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  POSTED_BY = NULL, POSTED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        string queryNew = $@"DELETE FROM SA_POSTED_TRANSACTION WHERE VOUCHER_NO='{voucherNum}' AND FORM_CODE='{formcode}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
        //                                        var rowCountNew = _coreentity.ExecuteSqlCommand(queryNew);
        //                                        if (rowCountNew == 1)
        //                                        {
        //                                            count++;
        //                                        }
        //                                    }
        //                                }
        //                                if (mode == "Authorise")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = '{_workContext.CurrentUserinformation.login_code}', AUTHORISED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "UnAuthorise")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  AUTHORISED_BY = NULL, AUTHORISED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "Check")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = '{_workContext.CurrentUserinformation.login_code}', CHECKED_DATE = SYSDATE where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                                if (mode == "UnCheck")
        //                                {
        //                                    string query = $@"UPDATE MASTER_TRANSACTION SET  CHECKED_BY = NULL, CHECKED_DATE = NULL where VOUCHER_NO='{voucherNum}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and form_code='{formcode}'";
        //                                    var rowCount = _coreentity.ExecuteSqlCommand(query);
        //                                    if (rowCount == 1)
        //                                    {
        //                                        count++;
        //                                        //AutoCVP(voucherNum,formcode, _coreentity);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                message = "voucherNum is empty.";
        //                                status = false;
        //                                break;
        //                            }
        //                        }
        //                        if (voucherNo.Count == count)
        //                        {
        //                            //trans.Rollback();
        //                            trans.Commit();
        //                            message = string.Format("Data " + mode + " successfull.");
        //                            status = true;
        //                        }
        //                        else
        //                        {
        //                            trans.Rollback();
        //                            message = "Voucher number count doesnot match with update count.";
        //                            status = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        message = "List empty.";
        //                        status = false;
        //                    }
        //                }
        //            }

        //            else
        //            {
        //                message = "List empty.";
        //                status = false;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            trans.Rollback();
        //            message = ex.Message;
        //            status = false;
        //        }
        //    }
        //}
        public void AutoCVP(string voucherNo, string formCode, NeoErpCoreEntity dbcontext = null)
        {
            string query = $@"EXEC SA_POST.PR_SALES_INVOICE('01','01.01','21','SRSI/00001/76-77','SUMMIT')";
            dbcontext.ExecuteSqlCommand(query);
        }
        //Prem Prakash Dhakal Comment code for journal voucher
        public GuestInfoFromMaterTransaction GetGuestInfoFromMasterTransaction(string formCode, string orderno)
        {
            try
            {
                //string query = $@"SELECT FORM_CODE, VOUCHER_NO,VOUCHER_AMOUNT,VOUCHER_DATE,CREATED_BY,CREATED_DATE,CHECKED_BY,CHECKED_DATE,AUTHORISED_BY,POSTED_DATE,MODIFY_DATE,SYN_ROWID,REFERENCE_NO,SESSION_ROWID,to_date(cr_lmt1,'dd-MM-yy')CR_LMT1,to_date(CR_LMT2,'dd-MM-yy') CR_LMT2, CR_LMT3,ROOMNUMBER FROM MASTER_TRANSACTION WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'and voucher_no='{orderno}' ORDER BY VOUCHER_NO DESC";
                string query = $@"SELECT FORM_CODE, VOUCHER_NO,VOUCHER_AMOUNT,VOUCHER_DATE,CREATED_BY,CREATED_DATE,CHECKED_BY,CHECKED_DATE,AUTHORISED_BY,POSTED_DATE,MODIFY_DATE,SYN_ROWID,REFERENCE_NO,SESSION_ROWID,to_date(cr_lmt1,'dd-MM-yy')CR_LMT1,to_date(CR_LMT2,'dd-MM-yy') CR_LMT2, CR_LMT3 FROM MASTER_TRANSACTION WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'and voucher_no='{orderno}' ORDER BY VOUCHER_NO DESC";
                var result = _dbContext.SqlQuery<GuestInfoFromMaterTransaction>(query).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public GuestInfoFromMaterTransaction GetGuestInfoFromMasterTransaction(string formCode, string orderno)
        //{
        //    try
        //    {
        //        string query = $@"SELECT FORM_CODE, VOUCHER_NO,VOUCHER_AMOUNT,VOUCHER_DATE,CREATED_BY,CREATED_DATE,CHECKED_BY,CHECKED_DATE,AUTHORISED_BY,POSTED_DATE,MODIFY_DATE,SYN_ROWID,REFERENCE_NO,SESSION_ROWID,to_date(cr_lmt1,'dd-MM-yy')CR_LMT1,to_date(CR_LMT2,'dd-MM-yy') CR_LMT2, CR_LMT3,ROOMNUMBER FROM MASTER_TRANSACTION WHERE FORM_CODE ='{formCode}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'and voucher_no='{orderno}' ORDER BY VOUCHER_NO DESC";
        //        var result = _dbContext.SqlQuery<GuestInfoFromMaterTransaction>(query).FirstOrDefault();
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        //Prem Prakash Dhakal Comment code for Radission
        //public GuestInfoFromMaterTransaction GetGuestInfoFromMasterTransaction(string formCode, string orderno)
        //{
        //    try
        //    {
        //        // First query to get details from MASTER_TRANSACTION
        //        string query1 = $@"SELECT FORM_CODE, VOUCHER_NO, VOUCHER_AMOUNT, VOUCHER_DATE, CREATED_BY, CREATED_DATE, 
        //                         CHECKED_BY, CHECKED_DATE, AUTHORISED_BY, POSTED_DATE, MODIFY_DATE, SYN_ROWID, 
        //                         REFERENCE_NO, SESSION_ROWID, 
        //                         to_date(cr_lmt1,'dd-MM-yy') CR_LMT1, 
        //                         to_date(CR_LMT2,'dd-MM-yy') CR_LMT2, 
        //                         CR_LMT3, ROOMNUMBER 
        //                  FROM MASTER_TRANSACTION 
        //                  WHERE FORM_CODE ='{formCode}' 
        //                  AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
        //                  AND VOUCHER_NO='{orderno}' 
        //                  ORDER BY VOUCHER_NO DESC";

        //        var masterTransactionResult = _dbContext.SqlQuery<GuestInfoFromMaterTransaction>(query1).FirstOrDefault();

        //        // Second query to get details from SA_SALES_INVOICE
        //        string query2 = $@"SELECT ARRIVAL_DATE, DEPARTURE_DATE,MANUAL_NO 
        //          FROM (SELECT ARRIVAL_DATE, DEPARTURE_DATE, MANUAL_NO 
        //                FROM SA_SALES_INVOICE 
        //                WHERE FORM_CODE ='{formCode}' 
        //                AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
        //                AND SALES_NO='{orderno}' 
        //                ORDER BY SALES_NO ASC)
        //          WHERE ROWNUM = 1";

        //        var salesInvoiceResult = _dbContext.SqlQuery<GuestInfoFromMaterTransaction>(query2).FirstOrDefault();

        //        // Combine the results
        //        if (masterTransactionResult != null && salesInvoiceResult != null)
        //        {
        //            masterTransactionResult.ARRIVAL_DATE = salesInvoiceResult.ARRIVAL_DATE;
        //            masterTransactionResult.DEPARTURE_DATE = salesInvoiceResult.DEPARTURE_DATE;
        //            masterTransactionResult.MANUAL_NO = salesInvoiceResult.MANUAL_NO;
        //        }

        //        return masterTransactionResult;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public GuestInfoFromMaterTransaction GetGuestInfoFromMasterTransaction(string formCode, string orderno)
        //{
        //    try
        //    {
        //        // First query to get details from MASTER_TRANSACTION
        //        string query1 = $@"SELECT FORM_CODE, VOUCHER_NO, VOUCHER_AMOUNT, VOUCHER_DATE, CREATED_BY, CREATED_DATE, 
        //                         CHECKED_BY, CHECKED_DATE, AUTHORISED_BY, POSTED_DATE, MODIFY_DATE, SYN_ROWID, 
        //                         REFERENCE_NO, SESSION_ROWID, 
        //                         to_date(cr_lmt1,'dd-MM-yy') CR_LMT1, 
        //                         to_date(CR_LMT2,'dd-MM-yy') CR_LMT2, 
        //                         CR_LMT3, ROOMNUMBER 
        //                  FROM MASTER_TRANSACTION 
        //                  WHERE FORM_CODE ='{formCode}' 
        //                  AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
        //                  AND VOUCHER_NO='{orderno}' 
        //                  ORDER BY VOUCHER_NO DESC";

        //        var masterTransactionResult = _dbContext.SqlQuery<GuestInfoFromMaterTransaction>(query1).FirstOrDefault();

        //        // Second query to get details from SA_SALES_INVOICE
        //        string query2 = $@"SELECT ARRIVAL_DATE, DEPARTURE_DATE,MANUAL_NO 
        //          FROM (SELECT ARRIVAL_DATE, DEPARTURE_DATE, MANUAL_NO 
        //                FROM SA_SALES_INVOICE 
        //                WHERE FORM_CODE ='{formCode}' 
        //                AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
        //                AND SALES_NO='{orderno}' 
        //                ORDER BY SALES_NO ASC)
        //          WHERE ROWNUM = 1";

        //        var salesInvoiceResult = _dbContext.SqlQuery<GuestInfoFromMaterTransaction>(query2).FirstOrDefault();

        //        // Combine the results
        //        if (masterTransactionResult != null && salesInvoiceResult != null)
        //        {
        //            masterTransactionResult.ARRIVAL_DATE = salesInvoiceResult.ARRIVAL_DATE;
        //            masterTransactionResult.DEPARTURE_DATE = salesInvoiceResult.DEPARTURE_DATE;
        //            masterTransactionResult.MANUAL_NO = salesInvoiceResult.MANUAL_NO;
        //        }

        //        return masterTransactionResult;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public CompanyInfo GetCompanyInfo()
        {
            try
            {
                string query = $@"  select * from COMPANY_SETUP where company_code='{_workContext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<CompanyInfo>(query).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> getSalesVerificationUserWise()
        {
            string query = $@"SELECT MENU_EDESC FROM SC_MENU_MANAGEMENT ";
            query += $@"WHERE GROUP_SKU_FLAG = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND TO_NUMBER(PRE_MENU_NO) = 13 ";
            query += $@"AND MENU_NO IN (SELECT MENU_NO FROM SC_MENU_CONTROL WHERE USER_NO = '{_workContext.CurrentUserinformation.User_id}' ";
            query += $@"AND ACCESS_FLAG = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' )AND SALES_FLAG = 'Y'  ORDER BY MENU_NO";
            var result = this._dbContext.SqlQuery<string>(query).ToList();
            return result;
        }
        public List<FormDetailSetup> getSalesVerificationFormcodeWise(string moduleCode, string docVer)
        {

            var ModuleFilter = "";
            if (moduleCode != "All")
            {
                ModuleFilter = $"FS.MODULE_CODE='{moduleCode}' AND ";
            }

            string query = $@"SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME
                FROM FORM_SETUP FS 
                INNER JOIN FORM_DETAIL_SETUP FDS ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
                INNER JOIN SC_FORM_CONTROL SFC ON SFC.FORM_CODE=FS.FORM_CODE AND SFC.COMPANY_CODE=FS.COMPANY_CODE
                INNER JOIN MASTER_TRANSACTION MT ON (MT.FORM_CODE=FDS.FORM_CODE AND MT.COMPANY_CODE=FDS.COMPANY_CODE) AND  (MT.FORM_CODE=SFC.FORM_CODE AND MT.COMPANY_CODE=SFC.COMPANY_CODE)
                WHERE {ModuleFilter} FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' 
                AND FDS.TABLE_NAME LIKE '%'
                --AND FDS.TABLE_NAME IN ('SA_SALES_ORDER','SA_SALES_CHALAN','SA_SALES_RETURN','SA_SALES_INVOICE')                
               -- AND FDS.TABLE_NAME NOT IN('SA_SALES_ORDER','IP_PURCHASE_ORDER','IP_QUOTATION_INQUIRY','IP_PURCHASE_REQUEST','FA_PAY_ORDER')               
                AND SFC.USER_NO = '{_workContext.CurrentUserinformation.User_id}' AND SFC.POST_FLAG = 'Y'
                AND FS.DELETED_FLAG='N'";
            if (docVer == "Check")
            {
                query += $@"AND FDS.TABLE_NAME NOT IN('IP_PURCHASE_ORDER','IP_QUOTATION_INQUIRY','IP_PURCHASE_REQUEST','FA_PAY_ORDER')
                            AND MT.POSTED_BY IS NULL AND MT.AUTHORISED_BY IS  NULL AND MT.CHECKED_BY IS NULL
                            ORDER BY TO_NUMBER(FORM_CODE) ASC ";
            }
            if (docVer == "UnCheck")
            {
                query += $@"AND FDS.TABLE_NAME NOT IN('IP_PURCHASE_ORDER','IP_QUOTATION_INQUIRY','IP_PURCHASE_REQUEST','FA_PAY_ORDER')
                            AND MT.POSTED_BY IS NULL AND MT.AUTHORISED_BY IS NULL AND MT.CHECKED_BY IS NOT NULL
                            ORDER BY TO_NUMBER(FORM_CODE) ASC ";
            }
            if (docVer == "Authorise")
            {
                query += $@"AND FDS.TABLE_NAME NOT IN('IP_PURCHASE_ORDER','IP_QUOTATION_INQUIRY','IP_PURCHASE_REQUEST','FA_PAY_ORDER')
                            AND MT.POSTED_BY IS NULL AND MT.AUTHORISED_BY IS NULL AND MT.CHECKED_BY IS NOT NULL
                            ORDER BY TO_NUMBER(FORM_CODE) ASC ";
            }
            if (docVer == "UnAuthorise")
            {
                query += $@"AND FDS.TABLE_NAME NOT IN('IP_PURCHASE_ORDER','IP_QUOTATION_INQUIRY','IP_PURCHASE_REQUEST','FA_PAY_ORDER')
                            AND MT.POSTED_BY IS NULL AND MT.AUTHORISED_BY IS NOT NULL AND MT.CHECKED_BY IS NOT  NULL
                            ORDER BY TO_NUMBER(FORM_CODE) ASC ";
            }
            if (docVer == "Post")
            {
                query += $@"AND FDS.TABLE_NAME NOT IN('SA_SALES_ORDER','IP_PURCHASE_ORDER','IP_QUOTATION_INQUIRY','IP_PURCHASE_REQUEST','FA_PAY_ORDER')
                            AND MT.POSTED_BY IS NULL AND MT.AUTHORISED_BY IS NOT NULL AND MT.CHECKED_BY IS NOT NULL
                            ORDER BY TO_NUMBER(FORM_CODE) ASC ";
            }
            if (docVer == "UnPost")
            {
                query += $@"AND FDS.TABLE_NAME NOT IN('SA_SALES_ORDER','IP_PURCHASE_ORDER','IP_QUOTATION_INQUIRY','IP_PURCHASE_REQUEST','FA_PAY_ORDER')
                            AND MT.POSTED_BY IS NOT NULL AND MT.AUTHORISED_BY IS NOT  NULL AND MT.CHECKED_BY IS NOT  NULL
                            ORDER BY TO_NUMBER(FORM_CODE) ASC ";
            }
            var result = this._dbContext.SqlQuery<FormDetailSetup>(query).ToList();
            return result;
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
                //var query = $@"SELECT  (select defa_value as FROM_LOCATION_CODE  from form_detail_setup where form_code='{formCode}' and company_code=a.company_code and column_name='FROM_LOCATION_CODE') as FROM_LOCATION_CODE,A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE as MU_CODE, E.MU_EDESC,  case when a.quantity<>0 then to_number(((nvl(a.quantity, 1) / nvl(b.quantity, 1)) * {ProductQty})) else 0 end AS quantity,
                //               case when a.quantity<>0 then to_number(((nvl(a.quantity, 1) / nvl(b.quantity, 1)) * {ProductQty}))else 0 end AS calc_quantity,0 as UNIT_PRICE,0 as TOTAL_PRICE,0 as CALC_UNIT_PRICE,0 as CALC_TOTAL_PRICE    FROM MP_ROUTINE_INPUT_SETUP A,  MP_ROUTINE_OUTPUT_SETUP B, MP_PROCESS_SETUP C, IP_ITEM_MASTER_SETUP D, IP_MU_CODE E
                //                                            WHERE A.PROCESS_CODE = C.PROCESS_CODE
                //                                            AND A.COMPANY_CODE = C.COMPANY_CODE
                //                                            AND A.COMPANY_CODE = B.COMPANY_CODE
                //                                            AND A.PROCESS_CODE = B.PROCESS_CODE
                //                                            AND A.ITEM_CODE = D.ITEM_CODE
                //                                            AND C.INDEX_ITEM_CODE = B.ITEM_CODE
                //                                            AND C.COMPANY_CODE = B.COMPANY_CODE
                //                                            AND A.COMPANY_CODE = C.COMPANY_CODE
                //                                            AND D.INDEX_MU_CODE = E.MU_CODE
                //                                            AND D.COMPANY_CODE = E.COMPANY_CODE
                //                                            AND C.LOCATION_CODE = '{RoutingCode}'
                //                                            AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";


                string query = $@"SELECT  DISTINCT
                              (SELECT defa_value AS FROM_LOCATION_CODE
                               FROM form_detail_setup
                               WHERE form_code = '{formCode}'
                                 AND company_code = a.company_code
                                 AND column_name = 'FROM_LOCATION_CODE') AS FROM_LOCATION_CODE,
    
                              a.ITEM_CODE,
                              d.ITEM_EDESC,
                              d.INDEX_MU_CODE AS MU_CODE,
                              e.MU_EDESC,
                              a.SERIAL_NO,
                              CASE
                                    WHEN a.quantity <> 0 THEN
                                        ROUND((NVL(a.quantity, 1) / NVL(b.quantity, 1)) * {ProductQty}, 4)
                                    ELSE
                                        0
                                END AS quantity,

                                CASE
                                    WHEN a.quantity <> 0 THEN
                                        ROUND((NVL(a.quantity, 1) / NVL(b.quantity, 1)) * {ProductQty}, 4)
                                    ELSE
                                        0
                                END AS calc_quantity,

                              0 AS UNIT_PRICE,
                              0 AS TOTAL_PRICE,
                              0 AS CALC_UNIT_PRICE,
                              0 AS CALC_TOTAL_PRICE

                          FROM MP_ROUTINE_INPUT_SETUP a
                          JOIN MP_ROUTINE_OUTPUT_SETUP b 
                              ON a.PROCESS_CODE = b.PROCESS_CODE 
                             AND a.COMPANY_CODE = b.COMPANY_CODE
                          JOIN MP_PROCESS_SETUP c 
                              ON a.PROCESS_CODE = c.PROCESS_CODE 
                             AND a.COMPANY_CODE = c.COMPANY_CODE
                          JOIN IP_ITEM_MASTER_SETUP d 
                              ON a.ITEM_CODE = d.ITEM_CODE
                          JOIN IP_MU_CODE e 
                              ON d.INDEX_MU_CODE = e.MU_CODE 
                             AND d.COMPANY_CODE = e.COMPANY_CODE
                          WHERE 
                              c.LOCATION_CODE = '{RoutingCode}' AND 
                              a.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' order by A.SERIAL_NO";


                var entity = this._dbContext.SqlQuery<COMMON_COLUMN>(query).ToList();
                return entity;

            }
            else if (TableName.ToUpper() == "IP_PRODUCTION_MRR")
            {
                // var query = $@"    SELECT (select defa_value as TO_LOCATION_CODE  from form_detail_setup where form_code='478'
                //and company_code=a.company_code 
                // and column_name='TO_LOCATION_CODE') TO_LOCATION_CODE,A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE AS MU_CODE, E.MU_EDESC FROM  MP_ROUTINE_OUTPUT_SETUP A, MP_PROCESS_SETUP C, IP_ITEM_MASTER_SETUP D, IP_MU_CODE E 
                //                                WHERE A.PROCESS_CODE = C.PROCESS_CODE 
                //                                AND A.COMPANY_CODE = C.COMPANY_CODE 
                //                                AND A.ITEM_CODE = D.ITEM_CODE
                //                            AND A.COMPANY_CODE = C.COMPANY_CODE 
                //                            AND D.INDEX_MU_CODE = E.MU_CODE
                //                    AND D.COMPANY_CODE = E.COMPANY_CODE 
                //                    AND C.LOCATION_CODE='{RoutingCode}' 
                //                        AND A.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";

                var query = $@"SELECT DISTINCT (SELECT defa_value AS TO_LOCATION_CODE
                            FROM form_detail_setup
                           WHERE     form_code = '{formCode}'
                                 AND company_code = a.company_code
                                 AND column_name = 'TO_LOCATION_CODE')
                            TO_LOCATION_CODE,
                         A.ITEM_CODE,
                         D.ITEM_EDESC,
                         D.INDEX_MU_CODE AS MU_CODE,
                         E.MU_EDESC
                    FROM MP_ROUTINE_OUTPUT_SETUP A,
                         MP_PROCESS_SETUP C,
                         IP_ITEM_MASTER_SETUP D,
                         IP_MU_CODE E
                   WHERE     A.PROCESS_CODE = C.PROCESS_CODE
                         AND A.COMPANY_CODE = C.COMPANY_CODE
                         AND A.ITEM_CODE = D.ITEM_CODE
                         AND A.COMPANY_CODE = C.COMPANY_CODE
                         AND D.INDEX_MU_CODE = E.MU_CODE
                         AND D.COMPANY_CODE = E.COMPANY_CODE
                         AND C.LOCATION_CODE = '{RoutingCode}'
                         AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                var entity = this._dbContext.SqlQuery<COMMON_COLUMN>(query).ToList();
                return entity;

            }

            return entityCode;
        }

        //public bool SaveInvoiceData(List<SalesInvoiceExcel> invData)
        //{
        //    using (var trans = _coreentity.Database.BeginTransaction())
        //    {
        //        if (!MasterItemExist())
        //        {
        //            InsertMasterItem();
        //        }
        //        if (!MasterCustomerExist())
        //        {
        //            InsertMasterCustomer();
        //        }
        //        try
        //        {
        //            string form_code = "21";
        //            string sales_tablename = "SA_SALES_INVOICE";
        //            int serial_no = 1;
        //            foreach (var item in invData)
        //            {

        //                InsertCustomer(item);
        //                InsertItem(item);
        //                string SquenceQuerey = $@"select MYSEQUENCE.nextval from duAL";
        //                decimal ROWID = this._coreentity.SqlQuery<decimal>(SquenceQuerey).FirstOrDefault();
        //                DateTime invoiceDate = DateTime.Now;
        //              //  DateTime invoiceDate = Convert.ToDateTime(item.INVOICE_DATE);
        //                string SALES_NO = New_Sales_No(_workContext.CurrentUserinformation.company_code, form_code, DateTime.Now.ToString("dd-MMM-yyyy"), sales_tablename);
        //                string InsertSalesInvoiceQuery = $@"INSERT INTO SA_SALES_INVOICE(SALES_NO,MANUAL_NO,MISC_CODE,SALES_DATE,CUSTOMER_CODE,SERIAL_NO,ITEM_CODE,MU_CODE,QUANTITY,
        //                                UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,TRACKING_NO,REMARKS,DESCRIPTION,CREATED_BY,CREATED_DATE,DELETED_FLAG,FROM_LOCATION_CODE,PARTY_TYPE_CODE,SESSION_ROWID,BUDGET_FLAG,BATCH_NO)
        //                                VALUES('{SALES_NO}','','{""}',TO_DATE('{invoiceDate.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{item.CUST_CODE}','{serial_no}','{item.PRODUCT_CODE}','{item.PRODUCT_UNIT}','{item.QUANTITY}',
        //                                TO_NUMBER({item.PRICE}),TO_NUMBER({Convert.ToDecimal(item.QUANTITY) * Convert.ToDecimal(item.RATE) }),'{item.QUANTITY}',TO_NUMBER({item.RATE}),TO_NUMBER({Convert.ToDecimal(item.QUANTITY)  Convert.ToDecimal(item.RATE) }),'{form_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{""}','{""}','{""}','{"ADMIN"}',sysdate,'N','01.01','02','{ROWID.ToString()}','L','')";

        //                var rowCount = _coreentity.ExecuteSqlCommand(InsertSalesInvoiceQuery);

        //                #region InsertMasterTransaction
        //                string InsertMasterTransaction = $@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,REFERENCE_NO,VOUCHER_AMOUNT,VOUCHER_DATE,CURRENCY_CODE,EXCHANGE_RATE,
        //                                                            FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID,SYN_ROWID,IS_SYNC_WITH_IRD,IS_REAL_TIME)
        //                                                            VALUES('{SALES_NO}','',TO_NUMBER({Convert.ToDecimal(item.QUANTITY) * Convert.ToDecimal(item.RATE) }),TO_DATE('{invoiceDate.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'NRS','1','{form_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{"ADMIN"}',sysdate,'N','{ROWID.ToString()}','','','')";
        //                var ROWSSS = _coreentity.ExecuteSqlCommand(InsertMasterTransaction);
        //                #endregion
        //                serial_no++;

        //            }
        //            trans.Commit();
        //        }
        //        catch (Exception ex)
        //        {

        //            trans.Rollback();
        //            return false;
        //        }
        //    }

        //    return true;
        //}
        private bool InsertMasterItem()
        {
            //values are hard coded for MASTER_ITEM
            string sql = string.Format($@"Insert into IP_ITEM_MASTER_SETUP 
                (ITEM_CODE,ITEM_EDESC,ITEM_NDESC,CATEGORY_CODE,INDEX_MU_CODE,MASTER_ITEM_CODE,PRE_ITEM_CODE,GROUP_SKU_FLAG,DELETED_FLAG,CREATED_DATE,CREATED_BY,COMPANY_CODE)
                 VALUES ('SAP_MASTER','SAP','','FG','NOS','01','00','G','N',TO_DATE({DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss")}, 'yyyy/mm/dd hh24:mi:ss'),'ADMIN','{_workContext.CurrentUserinformation.company_code}')");
            try
            {
                _coreentity.Database.ExecuteSqlCommand(sql);
                return true;
            }
            catch (Exception ex)
            {
                //return false;
                throw ex;
            }

        }
        private bool MasterCustomerExist()
        {
            try
            {
                bool exist = false;
                string query = @"select CUSTOMER_CODE from SA_CUSTOMER_SETUP where MASTER_CUSTOMER_CODE = '01' and PRE_CUSTOMER_CODE ='00' ";
                //var data = _context.SA_CUSTOMER_SETUP.Where(x => x.MASTER_CUSTOMER_CODE == "01" && x.PRE_CUSTOMER_CODE == "00").ToList();
                var data = _coreentity.Database.SqlQuery<string>(query).ToList();
                if (data.Count > 0) exist = true;
                return exist;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private bool InsertMasterCustomer()
        {
            //values are hard coded for MASTER_ITEM
            string sql = string.Format($@"Insert into SA_CUSTOMER_SETUP 
                (CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,GROUP_SKU_FLAG,TEL_MOBILE_NO1,TEL_MOBILE_NO2,FAX_NO,EMAIL,REGD_OFFICE_EADDRESS,DELETED_FLAG,CREATED_DATE,CREATED_BY,COMPANY_CODE)
                 VALUES ('SAP_MASTER_CUSTOMER','SAP_CUSTOMER','','01','00','G','','','','','','N',TO_DATE({DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss")}, 'yyyy/mm/dd hh24:mi:ss'),'ADMIN','{_workContext.CurrentUserinformation.company_code}')");
            try
            {
                _coreentity.Database.ExecuteSqlCommand(sql);
                return true;
            }
            catch (Exception ex)
            {
                //return false;
                throw ex;
            }

        }
        public void InsertItem(SalesInvoiceExcel itemList)
        {

            try
            {
                string message = string.Empty;
                //if (!MasterItemExist())
                //{
                //    InsertMasterItem();
                //}
                string baseQuery = string.Format(" select   MASTER_ITEM_CODE from IP_ITEM_MASTER_SETUP where PRE_ITEM_CODE = '01' and rownum=1 order by MASTER_ITEM_CODE desc  ");
                var baseItem = _coreentity.Database.SqlQuery<string>(baseQuery).FirstOrDefault();

                int masterItemCode = 0;
                string preItemCode = "01";
                if (baseItem != null)
                {
                    string[] arr = baseItem.Split('.');
                    if (arr.Length >= 2)
                        masterItemCode = Convert.ToInt32(arr[1]) + 1;
                    else
                        masterItemCode = 01;
                }
                else
                {
                    masterItemCode = 01;
                }
                if (_coreentity.Database.SqlQuery<string>($"select Item_code from IP_ITEM_MASTER_SETUP where Item_code='{itemList.PRODUCT_CODE}'").FirstOrDefault() == null)
                {
                    string sql = string.Format(@"Insert into IP_ITEM_MASTER_SETUP 
                    (ITEM_CODE,ITEM_EDESC,ITEM_NDESC,CATEGORY_CODE,INDEX_MU_CODE,MASTER_ITEM_CODE,PRE_ITEM_CODE,GROUP_SKU_FLAG,DELETED_FLAG,CREATED_DATE,CREATED_BY,COMPANY_CODE)
                        VALUES ('{0}','{1}','','FG','NOS','{2}','{3}','I','N',TO_DATE('{4}', 'yyyy/mm/dd hh24:mi:ss'),'{5}','{6}')", itemList.PRODUCT_CODE, itemList.PRODUCT_NAME, preItemCode + "." + masterItemCode.ToString("D2"), preItemCode, DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss"), _workContext.CurrentUserinformation.login_code, _workContext.CurrentUserinformation.company_code);
                    _coreentity.Database.ExecuteSqlCommand(sql);
                }
                // trans.Commit();
                //return message;

            }
            catch (Exception ex)
            {
                //trans.Rollback();
            }

        }
        public void InsertCustomer(SalesInvoiceExcel cusList)
        {

            try
            {
                string message = string.Empty;
                //if (!MasterCustomerExist())
                //{
                //    InsertMasterCustomer();
                //}
                string baseQuery = string.Format(" select   MASTER_CUSTOMER_CODE from SA_CUSTOMER_SETUP where PRE_CUSTOMER_CODE = '01' and rownum=1 order by MASTER_CUSTOMER_CODE desc  ");
                var baseItem = _coreentity.Database.SqlQuery<string>(baseQuery).FirstOrDefault();

                int masterItemCode = 0;
                string preItemCode = "01";
                if (baseItem != null)
                {
                    string[] arr = baseItem.Split('.');
                    if (arr.Length >= 2)
                        masterItemCode = Convert.ToInt32(arr[1]) + 1;
                    else
                        masterItemCode = 01;
                }
                else
                {
                    masterItemCode = 01;
                }

                //int i = 0;

                if (_coreentity.Database.SqlQuery<string>($"select Customer_code from SA_CUSTOMER_SETUP where Customer_code='{cusList.CUST_CODE}'").FirstOrDefault() == null)
                {
                    string sql = string.Format(@"Insert into SA_CUSTOMER_SETUP 
                    (CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,GROUP_SKU_FLAG,TEL_MOBILE_NO1,TEL_MOBILE_NO2,FAX_NO,EMAIL,REGD_OFFICE_EADDRESS,DELETED_FLAG,CREATED_DATE,CREATED_BY,COMPANY_CODE)
                        VALUES ('{0}','{1}','','{2}','{3}','I','{4}','{5}','{6}','{7}','{8}','N',TO_DATE('{9}', 'yyyy/mm/dd hh24:mi:ss'),'{10}','{11}')", cusList.CUST_CODE, cusList.CUST_NAME, preItemCode + "." + masterItemCode.ToString("D2"), preItemCode, cusList.Phone, "", "", "", "", DateTime.Now.ToString("yyyy/MM/dd/ HH:mm:ss"), _workContext.CurrentUserinformation.login_code, _workContext.CurrentUserinformation.company_code);
                    _coreentity.Database.ExecuteSqlCommand(sql);
                    //masterItemCode += 1;
                    //i++;
                }
                //trans.Commit();

                // message = i > 0 ? string.Format("{0} customers added successfully", i) : "Customers already exists";
                //  return message;

            }
            catch (Exception ex)
            {
                //trans.Rollback();
                // return "Some error occured while saving data";
            }

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
        private bool MasterItemExist()
        {
            bool exist = false;
            string itemQuery = $@"select ITEM_CODE from IP_ITEM_MASTER_SETUP where MASTER_ITEM_CODE='01' and PRE_ITEM_CODE='00' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' ";
            var data = _coreentity.Database.SqlQuery<string>(itemQuery).ToList();
            if (data.Count > 0) exist = true;
            return exist;
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
        public List<BATCHTRANSACTIONDATA> GetbatchdetailByItemCodeAndLocCode(string itemcode, string loactioncode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"SELECT BT.TRANSACTION_NO,ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.ITEM_SERIAL_NO AS TRACKING_SERIAL_NO,BT.SOURCE_FLAG FROM BATCH_TRANSACTION BT,IP_ITEM_MASTER_SETUP ISS 
WHERE ISS.ITEM_CODE=BT.ITEM_CODE AND 
ISS.COMPANY_CODE=BT.COMPANY_CODE AND
BT.ITEM_CODE='{itemcode}' AND BT.LOCATION_CODE='{loactioncode}' AND
BT.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND
BT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND BT.ITEM_SERIAL_NO NOT IN(SELECT DISTINCT ITEM_SERIAL_NO FROM BATCH_TRANSACTION WHERE SOURCE_FLAG='O' AND ITEM_CODE='{itemcode}' AND LOCATION_CODE='{loactioncode}' AND
COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND  ITEM_SERIAL_NO IS NOT NULL)";
            var entity = this._dbContext.SqlQuery<BATCHTRANSACTIONDATA>(Query).ToList();
            return entity;
        }
        public List<BATCHTRANSACTIONDATA> GetbatchdetailByItemCodeAndLocCodeforedit(string itemcode, string loactioncode, string voucherno)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"SELECT DISTINCT BT.ITEM_SERIAL_NO AS TRACKING_SERIAL_NO, BT.TRANSACTION_NO,ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.SOURCE_FLAG FROM BATCH_TRANSACTION BT,IP_ITEM_MASTER_SETUP ISS 
WHERE ISS.ITEM_CODE=BT.ITEM_CODE AND 
ISS.COMPANY_CODE=BT.COMPANY_CODE AND
BT.ITEM_CODE='{itemcode}' AND BT.LOCATION_CODE='{loactioncode}' AND
BT.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND
BT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND BT.SOURCE_FLAG='O' AND BT.REFERENCE_NO='{voucherno}'";
            var entity = this._dbContext.SqlQuery<BATCHTRANSACTIONDATA>(Query).ToList();
            return entity;
        }
        public string New_Sales_No(string companycode, string formcode, string transactiondate, string tablename)
        {
            try
            {
                if (companycode != "" && formcode != "" && transactiondate != "" && tablename != "")
                {
                    string query = string.Format(@"select FN_NEW_VOUCHER_NO('{0}','{1}','{2}','{3}') FROM DUAL", companycode, formcode, transactiondate, tablename);
                    string voucherNo = this._coreentity.SqlQuery<string>(query).First();
                    return voucherNo;
                }
                else
                { return ""; }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveInvoiceData(List<SalesInvoiceExcel> invData)
        {
            throw new NotImplementedException();
        }
        public List<BATCHTRANSACTIONDATA> GetbatchTranDataByItemCodeAndLocCode(string itemcode, string loactioncode, string refernceNo = null)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"SELECT ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,count(BT.QUANTITY) as QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE, BT.EXPIRY_DATE FROM BATCH_TRANSACTION BT,IP_ITEM_MASTER_SETUP ISS 
            WHERE ISS.ITEM_CODE=BT.ITEM_CODE AND 
            ISS.COMPANY_CODE=BT.COMPANY_CODE AND
            BT.ITEM_CODE='{itemcode}' AND
            BT.BATCH_NO is not NULL AND
            BT.DELETED_FLAG = 'N' AND
            BT.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND
            BT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND
            ISS.BATCH_SERIAL_FLAG='Y' AND";
            if (!string.IsNullOrEmpty(refernceNo) && refernceNo != "undefined")
            {
                Query += $@" BT.REFERENCE_NO = '{refernceNo}' AND BT.SOURCE_FLAG = 'O'";
            }
            else
            {
                Query += $@" BT.SOURCE_FLAG = 'I' AND BT.BATCH_NO NOT IN (SELECT DISTINCT BATCH_NO FROM BATCH_TRANSACTION WHERE SOURCE_FLAG = 'O')";
            }
            if (string.IsNullOrEmpty(loactioncode) || loactioncode == "undefined")
            {
                if (!string.IsNullOrEmpty(refernceNo))
                {
                    Query += $@" GROUP BY ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.EXPIRY_DATE,BT.REFERENCE_NO";
                }
                else
                {
                    Query += $@"  GROUP BY ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.EXPIRY_DATE";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(refernceNo))
                {
                    Query += $@" AND BT.LOCATION_CODE='{loactioncode}' GROUP BY ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.EXPIRY_DATE,BT.REFERENCE_NO";
                }
                else
                {
                    Query += $@" AND BT.LOCATION_CODE='{loactioncode}' GROUP BY ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.EXPIRY_DATE";
                }
            }
            var entity = this._dbContext.SqlQuery<BATCHTRANSACTIONDATA>(Query).ToList();
            return entity;

            // Alternate Query

            //select ITEM_CODE, count(QUANTITY) as Qty,REFERENCE_NO,BATCH_NO from batch_transaction where SOURCE_FLAG = 'I' and COMPANY_CODE = '03' and BRANCH_CODE = '03.01'
            //   and created_by = 'ADMIN' and DELETED_FLAG = 'N'and
            //   ITEM_CODE in (
            //   select distinct ITEM_CODE from IP_ITEM_MASTER_SETUP where BATCH_SERIAL_FLAG = 'Y'
            //   )
            //   group by ITEM_CODE,QUANTITY,REFERENCE_NO,BATCH_NO
            //   order by ITEM_CODE desc;
        }
        public bool BatchWiseItemCheck(string itemcode)
        {
            try
            {
                var query = $@"select item_code from ip_item_master_setup where item_code='{itemcode}' and BATCH_SERIAL_FLAG='Y' and company_code='{_workContext.CurrentUserinformation.company_code}' and deleted_flag='N'";
                string Result = _dbContext.SqlQuery<string>(query).FirstOrDefault();
                if (string.IsNullOrEmpty(Result))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<LoadingSlipModalForPrint> GetLoadingSlipListByReferenceoNo(string referenceno)

        {
            try
            {
                string query = $@"SELECT SLSD.LS_NO as LOADING_SLIP_NO,SLSD.TRANSACTION_NO,SLSD.SERIAL_NO,SLSD.REFERENCE_NO,SLSD.REFERENCE_FORM_CODE,SLSD.CUSTOMER_CODE,SCS.CUSTOMER_EDESC as CUSTOMER_EDESC,SLSD.ITEM_CODE,IIMS.ITEM_EDESC,SLSD.MU_CODE,SLSD.QUANTITY,SLSD.UNIT_PRICE,
                                  SLSD.VEHICLE_NAME,TO_DATE(SLSD.VOUCHER_DATE) as VOUCHER_DATE,SLSD.DRIVER_NAME,SLSD.DRIVER_LICENCE_NO,SLSD.TO_LOCATION,SLSD.LOAD_IN_TIME,SLSD.LOAD_OUT_TIME,SLSD.DESTINATION,SLSD.DISPATCH_NO
                                  FROM SA_LOADING_SLIP_DETAIL SLSD
                                  INNER JOIN IP_ITEM_MASTER_SETUP IIMS on IIMS.ITEM_CODE=SLSD.ITEM_CODE
                                  INNER JOIN SA_CUSTOMER_SETUP SCS on SCS.CUSTOMER_CODE=SLSD.CUSTOMER_CODE
                                  WHERE SLSD.REFERENCE_NO IS NOT NULL AND SLSD.REFERENCE_FORM_CODE IS NOT NULL AND SLSD.DELETED_FLAG='N' AND SLSD.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and SLSD.REFERENCE_NO='{referenceno}'";
                var List = _dbContext.SqlQuery<LoadingSlipModalForPrint>(query).ToList();
                return List;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting loading slip for printing : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
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
        public List<SchemeModels> getAllScheme()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(SCHEME_EDESC) AS SCHEME_EDESC,
                        TO_CHAR(SCHEME_CODE)SCHEME_CODE,SCHEME_TYPE,TYPE,STATUS,CALCULATION_DAYS,FORM_CODE,ACCOUNT_CODE,CUSTOMER_CODE,ITEM_CODE,PARTY_TYPE_CODE,AREA_CODE,BRANCH_CODE,CHARGE_CODE,CHARGE_ACCOUNT_CODE,CHARGE_RATE,EFFECTIVE_FROM,EFFECTIVE_TO,QUERY_STRING,REMARKS 
                        FROM SCHEME_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var schemeList = _dbContext.SqlQuery<SchemeModels>(query).ToList();
            return schemeList;
        }
        public List<SchemeModels> getAllSchemenotimplemented()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(SCHEME_EDESC) AS SCHEME_EDESC,
                        TO_CHAR(SCHEME_CODE)SCHEME_CODE,SCHEME_TYPE,TYPE,STATUS,CALCULATION_DAYS,FORM_CODE,ACCOUNT_CODE,CUSTOMER_CODE,ITEM_CODE,PARTY_TYPE_CODE,AREA_CODE,BRANCH_CODE,CHARGE_CODE,CHARGE_ACCOUNT_CODE,CHARGE_RATE,EFFECTIVE_FROM,EFFECTIVE_TO,QUERY_STRING,REMARKS 
                        FROM SCHEME_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var schemeList = _dbContext.SqlQuery<SchemeModels>(query).ToList();
            return schemeList;
        }
        public List<SchemeModels> getAllManualScheme(string status, string from, string to)
        {
            string query = string.Empty;
            if (!string.IsNullOrEmpty(status) && status != "undefined")
            {
                if (status == "A")
                {
                    query = $@"SELECT DISTINCT 
                        INITCAP(SCHEME_EDESC) AS SCHEME_EDESC,
                        TO_CHAR(SCHEME_CODE)SCHEME_CODE,CASE WHEN scheme_type='CD' THEN 'Cash On Discount' WHEN scheme_type='BS' THEN 'Bonous' ELSE 'Scheme' END  as scheme_type,
    CASE WHEN type='I' THEN 'Item' ELSE 'Value' END  as type,
    CASE WHEN IMPLEMENT_FLAG='Y' THEN 'Yes' ELSE 'No' END  as IMPLEMENT_FLAG,STATUS,CALCULATION_DAYS,FORM_CODE,ACCOUNT_CODE,CUSTOMER_CODE,ITEM_CODE,PARTY_TYPE_CODE,AREA_CODE,BRANCH_CODE,CHARGE_CODE,CHARGE_ACCOUNT_CODE,CHARGE_RATE,EFFECTIVE_FROM,EFFECTIVE_TO,QUERY_STRING,REMARKS 
                        FROM SCHEME_SETUP
                        WHERE DELETED_FLAG = 'N' AND STATUS='M'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                }
                else if (status == "P")
                { query = $@"SELECT DISTINCT 
                        INITCAP(SCHEME_EDESC) AS SCHEME_EDESC,
                        TO_CHAR(SCHEME_CODE)SCHEME_CODE,CASE WHEN scheme_type='CD' THEN 'Cash On Discount' WHEN scheme_type='BS' THEN 'Bonous' ELSE 'Scheme' END  as scheme_type,
    CASE WHEN type='I' THEN 'Item' ELSE 'Value' END  as type,
    CASE WHEN IMPLEMENT_FLAG='Y' THEN 'Yes' ELSE 'No' END  as IMPLEMENT_FLAG,STATUS,CALCULATION_DAYS,FORM_CODE,ACCOUNT_CODE,CUSTOMER_CODE,ITEM_CODE,PARTY_TYPE_CODE,AREA_CODE,BRANCH_CODE,CHARGE_CODE,CHARGE_ACCOUNT_CODE,CHARGE_RATE,EFFECTIVE_FROM,EFFECTIVE_TO,QUERY_STRING,REMARKS 
                        FROM SCHEME_SETUP
                        WHERE DELETED_FLAG = 'N' AND STATUS='M' AND IMPLEMENT_FLAG='N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'"; }
                else
                {
                    if (!string.IsNullOrEmpty(from) && from != "undefined" && from != null && !string.IsNullOrEmpty(to) && to != "undefined" && to != null)
                    {
                        query = $@"SELECT DISTINCT 
                        INITCAP(SCHEME_EDESC) AS SCHEME_EDESC,
                        TO_CHAR(SCHEME_CODE)SCHEME_CODE,CASE WHEN scheme_type='CD' THEN 'Cash On Discount' WHEN scheme_type='BS' THEN 'Bonous' ELSE 'Scheme' END  as scheme_type,
    CASE WHEN type='I' THEN 'Item' ELSE 'Value' END  as type,
    CASE WHEN IMPLEMENT_FLAG='Y' THEN 'Yes' ELSE 'No' END  as IMPLEMENT_FLAG,STATUS,CALCULATION_DAYS,FORM_CODE,ACCOUNT_CODE,CUSTOMER_CODE,ITEM_CODE,PARTY_TYPE_CODE,AREA_CODE,BRANCH_CODE,CHARGE_CODE,CHARGE_ACCOUNT_CODE,CHARGE_RATE,EFFECTIVE_FROM,EFFECTIVE_TO,QUERY_STRING,REMARKS 
                        FROM SCHEME_SETUP
                        WHERE DELETED_FLAG = 'N' AND STATUS='M' AND IMPLEMENT_FLAG='Y' AND EFFECTIVE_FROM  >= TO_DATE('{from}', 'DD-MON-YYYY') AND EFFECTIVE_TO <= TO_DATE('{to}','DD-MON-YYYY')
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                    }
                    else
                    {
                        query = $@"SELECT DISTINCT 
                        INITCAP(SCHEME_EDESC) AS SCHEME_EDESC,
                        TO_CHAR(SCHEME_CODE)SCHEME_CODE,CASE WHEN scheme_type='CD' THEN 'Cash On Discount' WHEN scheme_type='BS' THEN 'Bonous' ELSE 'Scheme' END  as scheme_type,
    CASE WHEN type='I' THEN 'Item' ELSE 'Value' END  as type,
    CASE WHEN IMPLEMENT_FLAG='Y' THEN 'Yes' ELSE 'No' END  as IMPLEMENT_FLAG,STATUS,CALCULATION_DAYS,FORM_CODE,ACCOUNT_CODE,CUSTOMER_CODE,ITEM_CODE,PARTY_TYPE_CODE,AREA_CODE,BRANCH_CODE,CHARGE_CODE,CHARGE_ACCOUNT_CODE,CHARGE_RATE,EFFECTIVE_FROM,EFFECTIVE_TO,QUERY_STRING,REMARKS 
                        FROM SCHEME_SETUP
                        WHERE DELETED_FLAG = 'N' AND STATUS='M' AND IMPLEMENT_FLAG='Y'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                    }

                }
            }
            var schemeList = _dbContext.SqlQuery<SchemeModels>(query).ToList();
            return schemeList;
        }
        public List<Document> getAllDocument()
        {
            try
            {
                var sqlquery = $@"SELECT DISTINCT
                               fds.form_code,fs.form_edesc
                                FROM
                               form_setup          fs,
                               form_detail_setup   fds
                               WHERE
                               fds.form_code = fs.form_code
                              AND fs.company_code = fds.company_code
                              AND fs.module_code = '01'
                              and fs.company_code= '{_workContext.CurrentUserinformation.company_code}' and     fds.table_name='FA_DOUBLE_VOUCHER'";
                var DOCList = _dbContext.SqlQuery<Document>(sqlquery).ToList();
                return DOCList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Document> getDocumentByFilter(string filter)
        {
            try
            {
                var sqlquery = $@"SELECT DISTINCT
                               fs.form_code,fs.form_edesc
                                FROM
                               form_setup          fs,
                               form_detail_setup   fds
                               WHERE
                               fds.form_code = fs.form_code
                              AND fs.company_code = fds.company_code
                              AND fs.module_code = '01'
                              and fs.company_code= '{_workContext.CurrentUserinformation.company_code}' and     fds.table_name='FA_DOUBLE_VOUCHER' and (upper(fs.form_edesc) like '%{filter.ToUpperInvariant()}%')
                    OR upper(fs.form_code) like '%{filter.ToUpperInvariant()}%'";
                var DOCList = _dbContext.SqlQuery<Document>(sqlquery).ToList();
                return DOCList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PartyType> GetAllDealer()
        {

            List<PartyType> result = new List<PartyType>();
            string query = $@"SELECT
                        COALESCE(PARTY_TYPE_CODE,' ') as PARTY_TYPE_CODE, 
                        COALESCE(PARTY_TYPE_EDESC,' ') as PARTY_TYPE_EDESC 
                        FROM IP_PARTY_TYPE_CODE
                        where PARTY_TYPE_FLAG='D' and DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            result = this._dbContext.SqlQuery<PartyType>(query).ToList();
            return result;
        }

        public string getusertype()
        {
            string query = $@"select login_edesc from sc_application_users where user_no='{_workContext.CurrentUserinformation.User_id}'";
            string usertype = this._dbContext.SqlQuery<string>(query).FirstOrDefault().ToString();
            return usertype;
        }
        public string getLoginedUser()
        {
            string query = $@"select login_code from sc_application_users where user_no='{_workContext.CurrentUserinformation.User_id}'";
            string usertype = this._dbContext.SqlQuery<string>(query).FirstOrDefault().ToString();
            return usertype;
        }

        public List<CustomerModels> getSchemeCustomerByCodes(string code)
        {


            try
            {
                string query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        INITCAP(CUSTOMER_NDESC) AS CUSTOMER_NDESC,
                        CUSTOMER_CODE,
                        PREFIX_TEXT AS CUSTOMER_PREFIX,
                        GROUP_START_NO AS CUSTOMER_STARTID,
                        CUSTOMER_FLAG,
                        ACC_CODE,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM SA_CUSTOMER_SETUP 
                        WHERE CUSTOMER_CODE IN({code}) AND DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

                var customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();

                return customerList;



            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting all customer : " + ex.StackTrace);
            }
        }

        public List<CustomerModels> getAllSchemeCustomer()
        {


            try
            {
                string query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        INITCAP(CUSTOMER_NDESC) AS CUSTOMER_NDESC,
                        CUSTOMER_CODE,
                        PREFIX_TEXT AS CUSTOMER_PREFIX,
                        GROUP_START_NO AS CUSTOMER_STARTID,
                        CUSTOMER_FLAG,
                        ACC_CODE,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM SA_CUSTOMER_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        ORDER BY CUSTOMER_CODE";

                var customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();

                return customerList;



            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting all customer : " + ex.StackTrace);
            }
        }

        public List<CustomerModels> getAllInterestCalcCustomers(string codes)
        {


            try
            {
                List<CustomerModels> customerList = new List<CustomerModels>();
                var query = string.Empty;
                if (codes != "")
                {
                    string cquery = $@"SELECT SUBSTR(SUB_CODE,2,LENGTH(SUB_CODE)) CUSTOMER_CODE FROM FA_SUB_LEDGER_MAP
                                WHERE DELETED_FLAG='N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND ACC_CODE IN({codes})";
                    var customerCodeList = _dbContext.SqlQuery<CustomerModels>(cquery).ToList();
                    string str = string.Empty;
                    foreach (var item in customerCodeList)
                        str = str + item.CUSTOMER_CODE + ",";

                    str = str.Remove(str.Length - 1);


                    query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        INITCAP(CUSTOMER_NDESC) AS CUSTOMER_NDESC,
                        CUSTOMER_CODE,
                        PREFIX_TEXT AS CUSTOMER_PREFIX,
                        GROUP_START_NO AS CUSTOMER_STARTID,
                        CUSTOMER_FLAG,
                        ACC_CODE,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM SA_CUSTOMER_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        AND CUSTOMER_CODE IN({str})
                        ORDER BY CUSTOMER_CODE";

                    customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();
                }
                else
                {

                    query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        INITCAP(CUSTOMER_NDESC) AS CUSTOMER_NDESC,
                        CUSTOMER_CODE,
                        PREFIX_TEXT AS CUSTOMER_PREFIX,
                        GROUP_START_NO AS CUSTOMER_STARTID,
                        CUSTOMER_FLAG,
                        ACC_CODE,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM SA_CUSTOMER_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        ORDER BY CUSTOMER_CODE";

                    customerList = _dbContext.SqlQuery<CustomerModels>(query).ToList();
                }



                return customerList;



            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting all customer : " + ex.StackTrace);
            }
        }


        public List<ProductsModels> getAllProductforScheme()
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ITEM_EDESC) AS ITEM_EDESC,
                        ITEM_CODE ,
                        MASTER_ITEM_CODE, 
                        PRE_ITEM_CODE,
                        GROUP_SKU_FLAG 
                        FROM ip_item_master_setup
                        WHERE DELETED_FLAG = 'N' and category_code='FG' and group_sku_flag='I'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        ORDER BY ITEM_CODE";
            var productList = _dbContext.SqlQuery<ProductsModels>(query).ToList();
            return productList;
        }

        public List<ProductsModels> getProductforSchemeByCode(string code)
        {
            string query = $@"SELECT DISTINCT 
                        INITCAP(ITEM_EDESC) AS ITEM_EDESC,
                        ITEM_CODE ,
                        MASTER_ITEM_CODE, 
                        PRE_ITEM_CODE,
                        GROUP_SKU_FLAG 
                        FROM ip_item_master_setup
                        WHERE ITEM_CODE IN({code}) AND  DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        ORDER BY ITEM_CODE";
            var productList = _dbContext.SqlQuery<ProductsModels>(query).ToList();
            return productList;
        }

        public List<PartyType> GetDealerForSchemeByCode(string code)
        {

            List<PartyType> result = new List<PartyType>();
            string query = string.Empty;
            if (code.Contains(','))
            {
                char[] delimiterChars = { ',' };
                string cb = string.Empty;
                string bb = string.Empty;
                string[] ptList = code.Split(delimiterChars);
                foreach (var p in ptList)
                {
                    cb = cb + "'" + p + "'" + ",";
                }
                bb = cb.TrimEnd(',');
                query = $@"SELECT
                        COALESCE(PARTY_TYPE_CODE,' ') as PARTY_TYPE_CODE, 
                        COALESCE(PARTY_TYPE_EDESC,' ') as PARTY_TYPE_EDESC 
                        FROM IP_PARTY_TYPE_CODE
                        where PARTY_TYPE_CODE IN({bb}) AND  PARTY_TYPE_FLAG='D' and DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            }
            else
            {
                query = $@"SELECT
                        COALESCE(PARTY_TYPE_CODE,' ') as PARTY_TYPE_CODE, 
                        COALESCE(PARTY_TYPE_EDESC,' ') as PARTY_TYPE_EDESC 
                        FROM IP_PARTY_TYPE_CODE
                        where PARTY_TYPE_CODE IN('{code}') AND  PARTY_TYPE_FLAG='D' and DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            }

            result = this._dbContext.SqlQuery<PartyType>(query).ToList();
            return result;
        }

        public List<SchemeDetailsModel> getSchemeDetailGridData(SCHEME_MODEL model)
        {
            //List<SchemeModels> result = new List<SchemeModels>();
            //        string Query = $@"SELECT sd.SALES_SCHEME_VALUE,sd.TOTAL_SALES,sd.SALES_DISCOUNT, ss.scheme_edesc,ss.scheme_code,fa.acc_code as ACCOUNT_CODE,fa.acc_edesc,iis.item_code,iis.item_edesc,sc.customer_code,sc.customer_edesc,ip.party_type_code,
            //       ip.party_type_edesc,ass.area_edesc,fb.branch_edesc,ss.item_edesc as ITEM_EDESC_MULTI, ss.customer_edesc as CUSTOMER_EDESC_MULTI,ss.party_type_edesc as PARTY_TYPE_EDESC_MULTI,ss.area_edesc as AREA_EDESC_MULTI,ss.branch_edesc as BRANCH_EDESC_MULTI FROM
            //scheme_setup_details sd,scheme_setup ss,fa_chart_of_accounts_setup fa,ip_item_master_setup 
            //iis,SA_CUSTOMER_SETUP sc,IP_PARTY_TYPE_CODE ip,AREA_SETUP ass,FA_BRANCH_SETUP fb where 
            // sd.scheme_code = ss.scheme_code and sd.account_code=fa.acc_code and sd.company_code=fa.company_code and
            // sd.item_code=iis.item_code(+) and sd.company_code=iis.company_code(+) and sd.customer_code=sc.customer_code(+) and 
            // sd.party_type_code=ip.party_type_code(+) and sd.company_code=ip.company_code(+) and sd.area_code=ass.area_code(+)  and sd.branch_code=fb.branch_code(+) and sd.company_code=fb.company_code(+) and sd.company_code='{_workContext.CurrentUserinformation.company_code}'";
            string Query = $@"SELECT sd.sales_scheme_value as sales_scheme_value, sd.total_sales as total_sales, sd.sales_discount, ss.scheme_edesc,
    ss.scheme_code, fa.acc_code AS account_code, fa.acc_edesc, sc.customer_code, sc.customer_edesc,
    ip.party_type_code, ip.party_type_edesc FROM scheme_setup_details sd, scheme_setup ss,
    fa_chart_of_accounts_setup   fa, sa_customer_setup sc,ip_party_type_code ip WHERE sd.scheme_code = ss.scheme_code
    AND sd.account_code = fa.acc_code(+) AND sd.company_code = fa.company_code(+) AND sd.customer_code = sc.customer_code(+) AND sd.company_code = sc.company_code(+)
    AND sd.party_type_code = ip.party_type_code(+) AND sd.company_code = ip.company_code(+) and sd.company_code='{_workContext.CurrentUserinformation.company_code}'";
            if (!string.IsNullOrEmpty(model.SCHEME_EDESC) && model.SCHEME_EDESC != "undefined")
            {
                Query += $@" and ss.SCHEME_EDESC = '{model.SCHEME_EDESC}'";
            }
            if (!string.IsNullOrEmpty(model.SCHEME_CODE) && model.SCHEME_CODE != "undefined")
            {
                Query += $@" and ss.SCHEME_CODE = '{model.SCHEME_CODE}'";
            }
            if (!string.IsNullOrEmpty(model.CUSTOMER_CODE) && model.CUSTOMER_CODE != "undefined")
            {
                Query += $@" and sd.CUSTOMER_CODE = '{model.CUSTOMER_CODE}'";
            }
            if (!string.IsNullOrEmpty(model.ITEM_CODE) && model.ITEM_CODE != "undefined")
            {
                Query += $@" and sd.ITEM_CODE = '{model.ITEM_CODE}')";
            }
            if (!string.IsNullOrEmpty(model.PARTY_TYPE_CODE) && model.PARTY_TYPE_CODE != "undefined")
            {
                Query += $@" and sd.PARTY_TYPE_CODE = '{model.PARTY_TYPE_CODE}'";
            }
            if (!string.IsNullOrEmpty(model.AREA_CODE) && model.AREA_CODE != "undefined")
            {
                Query += $@" and sd.AREA_CODE = '{model.AREA_CODE}'";
            }
            if (!string.IsNullOrEmpty(model.BRANCH_CODE) && model.BRANCH_CODE != "undefined")
            {
                Query += $@" and sd.BRANCH_CODE = '{model.BRANCH_CODE}'";
            }
            var entity = this._dbContext.SqlQuery<SchemeDetailsModel>(Query).ToList();
            return entity;


        }

        public List<SchemeDetailsModel> getSchemeDetailFormImpact(SCHEME_MODEL model)
        {
            //List<SchemeModels> result = new List<SchemeModels>();
            string Query = $@"SELECT sd.sales_scheme_value as sales_scheme_value, sd.total_sales as total_sales, sd.sales_discount, ss.scheme_edesc,
    ss.scheme_code, fa.acc_code AS account_code, fa.acc_edesc, sc.customer_code, sc.customer_edesc,
    ip.party_type_code, ip.party_type_edesc FROM scheme_setup_details sd, scheme_setup ss,
    fa_chart_of_accounts_setup   fa, sa_customer_setup sc,ip_party_type_code ip WHERE sd.scheme_code = ss.scheme_code
    AND sd.account_code = fa.acc_code(+) AND sd.company_code = fa.company_code(+) AND sd.customer_code = sc.customer_code(+) AND sd.company_code = sc.company_code(+)
    AND sd.party_type_code = ip.party_type_code(+) AND sd.company_code = ip.company_code(+) and sd.company_code='{_workContext.CurrentUserinformation.company_code}' and sd.impact_created='N'";
            if (!string.IsNullOrEmpty(model.SCHEME_EDESC) && model.SCHEME_EDESC != "undefined")
            {
                Query += $@" and ss.SCHEME_EDESC = '{model.SCHEME_EDESC}'";
            }
            if (!string.IsNullOrEmpty(model.SCHEME_CODE) && model.SCHEME_CODE != "undefined")
            {
                Query += $@" and ss.SCHEME_CODE = '{model.SCHEME_CODE}'";
            }
            var entity = this._dbContext.SqlQuery<SchemeDetailsModel>(Query).ToList();
            for (int i = 0; i < entity.Count; i++)
            {
                entity[i].SNO = i + 1;
            }
            return entity;


        }

        public List<TNC> getTNCDataForProduct(string tncCode)
        {
            //List<SchemeModels> result = new List<SchemeModels>();
            string Query = $@"SELECT tncG.TNC_CODE, tncG.TNC_EDESC, tncG.GROUP_SKU_FLAG, tncG.MASTER_TNC_CODE FROM TERMS_AND_CONDITION_SETUP tncG
                          WHERE tncG.TNC_CODE = '{tncCode}' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                          UNION
                          SELECT tncI.TNC_CODE, tncI.TNC_EDESC, tncI.GROUP_SKU_FLAG, tncG.MASTER_TNC_CODE FROM TERMS_AND_CONDITION_SETUP tncG
                          LEFT JOIN TERMS_AND_CONDITION_SETUP tncI
                          ON tncG.MASTER_TNC_CODE = tncI.PRE_TNC_CODE
                          WHERE tncG.TNC_CODE = '{tncCode}'
                          AND tncG.DELETED_FLAG = 'N'
                          AND tncI.DELETED_FLAG = 'N'
                          AND tncI.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                          ORDER BY GROUP_SKU_FLAG ASC";

            var entity = this._dbContext.SqlQuery<TNC>(Query).ToList();
            return entity;


        }

        public List<PartyType> GetAllPartyTypeByFilterAndSubCode(string filter, string Subcode)
        {
            var dealer_system_flag = string.Empty;
            string dealer_system_flag_query = $@"SELECT
                        DEALER_SYSTEM_FLAG 
                        FROM PREFERENCE_SUB_SETUP
                        where COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workContext.CurrentUserinformation.branch_code}'";
            dealer_system_flag = this._dbContext.SqlQuery<string>(dealer_system_flag_query).FirstOrDefault();
            if (dealer_system_flag == "Y")
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<PartyType> result = new List<PartyType>();
                string query = $@"SELECT FA.PARTY_TYPE_CODE,
       FS.SUB_CODE,
        FS.SUB_EDESC,
       SA.CUSTOMER_EDESC,
       IP.PARTY_TYPE_EDESC,
       IP.PARTY_TYPE_FLAG
  FROM FA_SUB_LEDGER_DEALER_MAP FA,
       SA_CUSTOMER_SETUP SA,
       IP_PARTY_TYPE_CODE IP,
       FA_SUB_LEDGER_SETUP FS
 WHERE     FA.COMPANY_CODE = SA.COMPANY_CODE
       AND TRIM (FA.SUB_CODE) = TRIM (sa.link_sub_code)
       AND IP.COMPANY_CODE = FA.COMPANY_CODE
       AND SA.COMPANY_CODE = IP.COMPANY_CODE
       AND FA.PARTY_TYPE_CODE = IP.PARTY_TYPE_CODE
       AND  TRIM (FA.SUB_CODE)=FS.SUB_CODE
       AND FA.COMPANY_CODE=FS.COMPANY_CODE
       AND IP.DELETED_FLAG = 'N'
       AND FA.DELETED_FLAG = 'N'
       AND SA.DELETED_FLAG = 'N'
       AND FA.SUB_CODE = '{Subcode}'
       AND  IP.PARTY_TYPE_FLAG='D'
       AND IP.DELETED_FLAG='N' AND IP.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
       and(FS.SUB_EDESC like '%{filter.Trim()}%')";
                result = this._dbContext.SqlQuery<PartyType>(query).ToList();
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
                List<PartyType> result = new List<PartyType>();
                string query = $@"SELECT FA.PARTY_TYPE_CODE,
       FA.SUB_CODE,
       SA.CUSTOMER_EDESC,
       IP.PARTY_TYPE_EDESC,
       IP.PARTY_TYPE_FLAG
  FROM FA_SUB_LEDGER_DEALER_MAP FA,
       SA_CUSTOMER_SETUP SA,
       IP_PARTY_TYPE_CODE IP
 WHERE     FA.COMPANY_CODE = SA.COMPANY_CODE
       AND TRIM (FA.SUB_CODE) = TRIM (sa.link_sub_code)
       AND IP.COMPANY_CODE = FA.COMPANY_CODE
       AND SA.COMPANY_CODE = IP.COMPANY_CODE
       AND FA.PARTY_TYPE_CODE = IP.PARTY_TYPE_CODE
       AND IP.DELETED_FLAG = 'N'
       AND FA.DELETED_FLAG = 'N'
       AND SA.DELETED_FLAG = 'N'
       AND FA.SUB_CODE = '{Subcode}'
       AND IP.DELETED_FLAG='N' AND IP.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
       AND ( IP.PARTY_TYPE_FLAG='P'
       or  IP.PARTY_TYPE_FLAG IS NULL)
       and(IP.PARTY_TYPE_EDESC like '%{filter.Trim()}%')";
                result = this._dbContext.SqlQuery<PartyType>(query).ToList();
                return result;
            }
        }


        public string GetPartyTypeNameByCode(string code)
        {
            try
            {
                if (code != "undefined" && code != "null")
                {
                    string PTQuery = $@"SELECT PARTY_TYPE_EDESC FROM IP_PARTY_TYPE_CODE WHERE PARTY_TYPE_CODE='{code}' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  AND DELETED_FLAG='N'";
                    var PTdata = this._dbContext.SqlQuery<string>(PTQuery).FirstOrDefault().ToString();
                    return string.IsNullOrEmpty(PTdata) ? "" : PTdata;

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

        public List<AreaSetup> GetAreaForSchemeByCode(string code)
        {

            List<AreaSetup> result = new List<AreaSetup>();
            string query = $@"SELECT
                        COALESCE(TO_CHAR(AREA_CODE),' ') as AREA_CODE, 
                        COALESCE(AREA_EDESC,' ') as AREA_EDESC 
                        FROM AREA_SETUP
                        where AREA_CODE IN({code}) AND DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            result = this._dbContext.SqlQuery<AreaSetup>(query).ToList();
            return result;
        }
        public List<BranchModels> GetBranchForSchemeByCode(string code)
        {
            string formatedcode = "";
            string[] words = new string[100];
            if (code.Contains(','))
            {
                words = code.Split(',');
                foreach (var word in words)
                {
                    formatedcode = formatedcode + "'" + word + "'" + ",";
                }
                formatedcode = formatedcode.ToString().TrimEnd(',');
            }
            else
            {
                formatedcode = "'" + code + "'";
            }

            List<BranchModels> result = new List<BranchModels>();
            string query = $@"SELECT
                        COALESCE(BRANCH_CODE,' ') as BRANCH_CODE, 
                        COALESCE(BRANCH_EDESC,' ') as BRANCH_EDESC 
                        FROM FA_BRANCH_SETUP
                        where BRANCH_CODE IN({formatedcode}) AND DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            result = this._dbContext.SqlQuery<BranchModels>(query).ToList();
            return result;
        }

        public List<CompanyInfo> GetCompanyList()
        {

            string query = $@"SELECT
                        COALESCE(COMPANY_CODE,' ') as COMPANY_CODE, 
                        COALESCE(COMPANY_EDESC,' ') as COMPANY_EDESC 
                        FROM COMPANY_SETUP";
            var result = this._dbContext.SqlQuery<CompanyInfo>(query).ToList();
            return result;
        }
        public List<Document> GetDocumentForSchemeByCode(string code)
        {

            List<Document> result = new List<Document>();
            string query = $@"SELECT
                        COALESCE(FORM_CODE,' ') as FORM_CODE, 
                        COALESCE(FORM_EDESC,' ') as FORM_EDESC 
                        FROM FORM_SETUP
                        where FORM_CODE IN({code}) AND DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            result = this._dbContext.SqlQuery<Document>(query).ToList();
            return result;
        }



        public string GetcustomerCodeByName(string name)
        {
            if (name != "undefined" && name != "null")
            {

                string CUSQuery = $@"SELECT CUSTOMER_CODE  FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_EDESC like '%{name.Trim()}%' AND  COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  AND DELETED_FLAG='N'";
                string CUSdata = this._dbContext.SqlQuery<string>(CUSQuery).FirstOrDefault().ToString();
                return CUSdata;
            }
            else
            {
                return "";
            }
        }
        public string GetTableNameByFormCode(string formCode)
        {
            if (formCode != "undefined" && formCode != "null")
            {

                string tableNameByFormCodeQuery = $@"SELECT DISTINCT FDS.TABLE_NAME FROM FORM_DETAIL_SETUP FDS WHERE FDS.FORM_CODE = '{formCode}' AND FDS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND FDS.DISPLAY_FLAG = 'Y' AND FDS.DELETED_FLAG = 'N'  AND DELETED_FLAG='N'";
                string tableNameByFormCodedata = this._dbContext.SqlQuery<string>(tableNameByFormCodeQuery).FirstOrDefault().ToString();
                return tableNameByFormCodedata;
            }
            else
            {
                return "";
            }
        }
        public List<CustomersTreeModel> getAllCustomerWithChildren()
        {
            try
            {
                string query = $@"SELECT DISTINCT 
                        INITCAP(CUSTOMER_EDESC) AS CUSTOMER_EDESC,
                        INITCAP(CUSTOMER_NDESC) AS CUSTOMER_NDESC,
                        CUSTOMER_CODE,
                        PREFIX_TEXT AS CUSTOMER_PREFIX,
                        GROUP_START_NO AS CUSTOMER_STARTID,
                        CUSTOMER_FLAG,
                        ACC_CODE,
                        MASTER_CUSTOMER_CODE, 
                        PRE_CUSTOMER_CODE,
                        GROUP_SKU_FLAG,
                        REMARKS
                        FROM SA_CUSTOMER_SETUP 
                        WHERE DELETED_FLAG = 'N'
                        AND GROUP_SKU_FLAG = 'G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                        CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
                        START WITH PRE_CUSTOMER_CODE='00'";

                var customerList = _dbContext.SqlQuery<CustomersTreeModel>(query).ToList();

                return customerList;

            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting all customer : " + ex.StackTrace);
            }
        }

        public List<CustomerSetupModels> CustomerListAllNodes(User userinfo)
        {
            string query = @"SELECT LEVEL,INITCAP(CS.CUSTOMER_EDESC) CUSTOMER_EDESC,CS.CUSTOMER_CODE,
            CS.GROUP_SKU_FLAG,CS.MASTER_CUSTOMER_CODE,CS.PRE_CUSTOMER_CODE, CS.BRANCH_CODE, LEVEL,
            (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE  GROUP_SKU_FLAG='G' AND COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_CUSTOMER_CODE = CS.MASTER_CUSTOMER_CODE) as Childrens
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '" + userinfo.company_code + @"'
            AND GROUP_SKU_FLAG = 'G'
            AND LEVEL = 1
            START WITH PRE_CUSTOMER_CODE = '00'
            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC";
            var customerListNodes = _dbContext.SqlQuery<CustomerSetupModels>(query).ToList();
            return customerListNodes;
        }
        public List<CustomerSetupModels> GetCustomerListByCustomerCode(string level, string masterCustomerCode, User userinfo)
        {

            string query = string.Format(@"SELECT LEVEL,INITCAP(CS.CUSTOMER_EDESC) CUSTOMER_EDESC,GROUP_SKU_FLAG,
            CUSTOMER_CODE, CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE, CS.BRANCH_CODE,
            (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE  GROUP_SKU_FLAG='G' AND COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_CUSTOMER_CODE = CS.MASTER_CUSTOMER_CODE) as Childrens
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'
            AND CS.COMPANY_CODE = '{0}'
            AND LEVEL = {1}
            START WITH PRE_CUSTOMER_CODE = '{2}'
            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC", userinfo.company_code, level.ToString(), masterCustomerCode.ToString());
            var customerListNodes = _dbContext.SqlQuery<CustomerSetupModels>(query).ToList();
            return customerListNodes;
        }

        private string FindStopDSRT()
        {
            string DSRTQuery = $@"SELECT STATUS FROM USER_OBJECTS WHERE  OBJECT_NAME = 'TRG_STOP_DELETE_SALES_RETURN' AND OBJECT_TYPE ='TRIGGER'";
            var SRTdata = this._dbContext.SqlQuery<string>(DSRTQuery).FirstOrDefault().ToString();
            return string.IsNullOrEmpty(SRTdata) ? "" : SRTdata;
        }
        private string FindStopUSI()
        {

            string USIQuery = $@"SELECT STATUS FROM USER_OBJECTS WHERE  OBJECT_NAME = 'TRG_STOP_UPDATE_SALES_INVOICE' AND OBJECT_TYPE ='TRIGGER'";
            var USIdata = this._dbContext.SqlQuery<string>(USIQuery).FirstOrDefault().ToString();
            return string.IsNullOrEmpty(USIdata) ? "" : USIdata;
        }
        private string FindStopUSR()
        {
            string USRQuery = $@"SELECT STATUS FROM USER_OBJECTS WHERE  OBJECT_NAME = 'TRG_STOP_UPDATE_SALES_RETURN' AND OBJECT_TYPE ='TRIGGER'";
            var USRdata = this._dbContext.SqlQuery<string>(USRQuery).FirstOrDefault().ToString();
            return string.IsNullOrEmpty(USRdata) ? "" : USRdata;
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
        public bool deletevouchernoFinance(string tablename, string formcode, string voucherno, string primarycolumnname)
        {
            using (var trans = _coreentity.Database.BeginTransaction())
            {
                try
                {
                    var deletmaintableequery = $@"UPDATE {tablename} SET DELETED_FLAG='Y',MODIFY_DATE=SYSDATE,MODIFY_BY='{_workContext.CurrentUserinformation.login_code}' WHERE {primarycolumnname}='{voucherno}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.Company}'";
                    var maintablerowCount = _coreentity.ExecuteSqlCommand(deletmaintableequery);

                    if (maintablerowCount > 0)
                    {
                        var deletemastertable = $@"UPDATE MASTER_TRANSACTION SET DELETED_FLAG='Y', MODIFY_DATE=SYSDATE,MODIFY_BY='{_workContext.CurrentUserinformation.login_code}' WHERE VOUCHER_NO='{voucherno}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.Company}'";
                        var mastertablerowCount = _coreentity.ExecuteSqlCommand(deletemastertable);
                    }
                    string deletebudgetcenterquery = string.Format(@"DELETE FROM BUDGET_TRANSACTION where REFERENCE_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    var budgetcenterrowCount = _coreentity.ExecuteSqlCommand(deletebudgetcenterquery);

                    string deletesubledgerquery = string.Format(@"DELETE FROM FA_VOUCHER_SUB_DETAIL where  VOUCHER_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    _coreentity.ExecuteSqlCommand(deletesubledgerquery);

                    string deletevatquery = string.Format(@"DELETE FROM FA_DC_VAT_INVOICE where  INVOICE_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    _coreentity.ExecuteSqlCommand(deletevatquery);

                    string deletetdsquery = string.Format(@"DELETE FROM FA_DC_TDS_INVOICE where  INVOICE_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    _coreentity.ExecuteSqlCommand(deletetdsquery);

                    string deletechargequery = string.Format(@"UPDATE CHARGE_TRANSACTION SET DELETED_FLAG = 'Y' where  VOUCHER_NO='{0}' and COMPANY_CODE='{1}'", voucherno, _workContext.CurrentUserinformation.company_code);
                    _coreentity.ExecuteSqlCommand(deletechargequery);

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

        public List<DocumentSubMenu> GetMasterTransactionNotSyncList()
        {
            try
            {
                var companyCode = _workContext.CurrentUserinformation.company_code;
                var branchCode = _workContext.CurrentUserinformation.branch_code;

                string query = $@"
                            SELECT FORM_CODE,
                                   VOUCHER_NO,
                                   VOUCHER_AMOUNT,
                                   VOUCHER_DATE,
                                   CREATED_BY,
                                   CREATED_DATE,
                                   CHECKED_BY,
                                   CHECKED_DATE,
                                   AUTHORISED_BY,
                                   POSTED_DATE,
                                   MODIFY_DATE,
                                   MITI,
                                   SYN_ROWID,
                                   REMARKS
                            FROM MASTER_TRANSACTION
                            WHERE (IS_SYNC_WITH_IRD = 'N' OR IS_SYNC_WITH_IRD IS NULL)
                              AND COMPANY_CODE = '{companyCode}'
                              --AND BRANCH_CODE = '{branchCode}'
                              AND FORM_CODE IN (
                                    SELECT DISTINCT FORM_CODE
                                    FROM FORM_DETAIL_SETUP
                                    WHERE TABLE_NAME IN ('SA_SALES_INVOICE','SA_SALES_RETURN') and COMPANY_CODE='{companyCode}'
                              )
                            ORDER BY CREATED_DATE DESC";

                var result = _dbContext.SqlQuery<DocumentSubMenu>(query).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }



        //public IRDLogResponseModel GetIRDLogList(int skip, int take, int page, int pageSize, string fromDate, string toDate)
        //{
        //    try
        //    {
        //        var response = new IRDLogResponseModel();
        //        string whereClause = "";

        //        if(!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
        //        {
        //             whereClause = $@" where CREATED_DATE >= to_date('{fromDate}','YYYY-MM-DD') and CREATED_DATE < to_date('{toDate}','YYYY-MM-DD') + 1";
        //        }

        //        string countQuery = $@"select count(*) from IRD_LOG {whereClause}";
        //        response.total = _dbContext.SqlQuery<int>(countQuery).FirstOrDefault();

        //        string query = $@"select * from IRD_LOG {whereClause} order by CREATED_DATE desc OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        //        response.data = _dbContext.SqlQuery<IRDLogModel>(query).ToList();

        //        return response;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw;
        //    }
        //}


        //public IRDLogResponseModel GetIRDLogList(int skip, int take, int page, int pageSize, string fromDate, string toDate)
        //{
        //    try
        //    {
        //        var response = new IRDLogResponseModel();
        //        string whereClause = "";

        //        if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
        //        {
        //            whereClause = $@" WHERE CREATED_DATE >= TO_DATE('{fromDate}','YYYY-MM-DD')
        //                      AND CREATED_DATE < TO_DATE('{toDate}','YYYY-MM-DD') + 1";
        //        }

        //        // COUNT QUERY
        //        string countQuery = $@"SELECT COUNT(*) FROM IRD_LOG {whereClause}";
        //        response.total = _dbContext.SqlQuery<int>(countQuery).FirstOrDefault();

        //        // PAGING QUERY (ROWNUM way)
        //        int minRow = skip;
        //        int maxRow = skip + take;

        //        string query = $@"
        //                        SELECT *
        //                        FROM (
        //                            SELECT t.*, ROWNUM rn
        //                            FROM (
        //                                SELECT *
        //                                FROM IRD_LOG
        //                                {whereClause}
        //                                ORDER BY CREATED_DATE DESC
        //                            ) t
        //                            WHERE ROWNUM <= {maxRow}
        //                        )
        //                        WHERE rn > {minRow}";

        //        response.data = _dbContext.SqlQuery<IRDLogModel>(query).ToList();

        //        return response;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public IRDLogResponseModel GetIRDLogList(int skip, int take, int page, int pageSize, string fromDate, string toDate, string voucherNo = null)
        {
            try
            {
                var response = new IRDLogResponseModel();
                string whereClause = "";

                // Date filter
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    whereClause = $@"
                        WHERE CREATED_DATE >= TO_DATE('{fromDate}', 'YYYY-MM-DD')
                        AND CREATED_DATE < TO_DATE('{toDate}', 'YYYY-MM-DD') + 1";
                }

                // Voucher filter
                if (!string.IsNullOrEmpty(voucherNo))
                {
                    string voucherCondition = $"UPPER(VOUCHER_NO) LIKE '%{voucherNo.ToUpper()}%'";
                    whereClause = string.IsNullOrEmpty(whereClause) ? $"WHERE {voucherCondition}" : $"{whereClause} AND {voucherCondition}";
                }

                // COUNT QUERY
                string countQuery = $"SELECT COUNT(*) FROM IRD_LOG {whereClause}";
                response.total = _dbContext.SqlQuery<int>(countQuery).FirstOrDefault();

                int minRow = skip;
                int maxRow = skip + take;

                // PAGING QUERY - latest first
                string query = $@"
                                SELECT * FROM (
                                    SELECT t.*, ROWNUM rn
                                    FROM (
                                        SELECT *
                                        FROM IRD_LOG
                                        {whereClause}
                                        ORDER BY CREATED_DATE DESC
                                    ) t
                                    WHERE ROWNUM <= {maxRow}
                                )
                                WHERE rn > {minRow}
                            ";

                response.data = _dbContext.SqlQuery<IRDLogModel>(query).ToList();

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public string IsReturnType(string document_no)
        {
            try
            {
                string query = $@"SELECT CASE 
                                     WHEN EXISTS (
                                          SELECT 1 
                                          FROM SA_SALES_RETURN 
                                          WHERE RETURN_NO = '{document_no}'
                                     )
                                     THEN 'TRUE'
                                     ELSE 'FALSE'
                                   END AS RETURN_EXISTS
                            FROM DUAL";
                var result = _dbContext.SqlQuery<string>(query).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public string IsInvoiceType(string document_no)
        {
            try
            {
                string query = $@"SELECT CASE 
                                     WHEN EXISTS (
                                          SELECT 1 
                                          FROM SA_SALES_INVOICE 
                                          WHERE SALES_NO = '{document_no}'
                                     )
                                     THEN 'TRUE'
                                     ELSE 'FALSE'
                                   END AS RETURN_EXISTS
                            FROM DUAL";
                var result = _dbContext.SqlQuery<string>(query).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        //added my mahesh
        #region added by mahesh for: adding output item issue

        public List<RelatedReferenceNoModel> GetRelatedReferenceNoList(string searchText, int formCode)
        {

            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;

            string tableName = "IP_PRODUCTION_ISSUE";

            var tableNameQuery = $@"SELECT ref_table_name, ref_column_name FROM form_setup WHERE form_code='{formCode}' AND COMPANY_CODE='{company_code}'";
            var tableNameQueryResult = this._dbContext.SqlQuery<ReferenceTableModel>(tableNameQuery).FirstOrDefault();


            string query = $@"SELECT DISTINCT
                             VOUCHER_NO REFERENCE_NO,
                             VOUCHER_NO,
                             (SELECT DISTINCT LOCATION_EDESC
                                FROM IP_PRODUCTION_ISSUE A, IP_LOCATION_SETUP B
                               WHERE     A.COMPANY_CODE = B.COMPANY_CODE
                                     AND A.TO_LOCATION_CODE = B.LOCATION_CODE
                                     AND A.COMPANY_CODE = MASTER_TRANSACTION.COMPANY_CODE
                                     AND A.ISSUE_NO = MASTER_TRANSACTION.VOUCHER_NO)
                                LOT_NO,
                             FORM_CODE
                        FROM MASTER_TRANSACTION
                       WHERE     (   UPPER (VOUCHER_NO) LIKE '%{searchText}%'
                                  OR VOUCHER_NO IN
                                        (SELECT NVL (ISSUE_NO, 0)
                                           FROM IP_PRODUCTION_ISSUE
                                          WHERE     COMPANY_CODE = '{company_code}'
                                                AND BRANCH_CODE = '{branch_code}'
                                                AND DELETED_FLAG = 'N'
                                                AND LOT_NO LIKE '%{searchText}%'))
                             AND COMPANY_CODE = '{company_code}'
                             AND BRANCH_CODE = '{branch_code}'
                             AND DELETED_FLAG = 'N'
                             AND FORM_CODE IN
                                    (SELECT FORM_CODE
                                       FROM FORM_DETAIL_SETUP
                                      WHERE     COMPANY_CODE = '{company_code}'
                                            AND TABLE_NAME = '{tableName}')
                             AND VOUCHER_NO NOT IN
                                    (SELECT NVL (REFERENCE_NO, 0)
                                       FROM IP_PRODUCTION_MRR
                                      WHERE     COMPANY_CODE = '{company_code}'
                                            AND BRANCH_CODE = '{branch_code}'
                                            AND DELETED_FLAG = 'N')
                             AND FORM_CODE IN (SELECT REF_FORM_CODE
                                                 FROM FORM_SETUP
                                                WHERE FORM_CODE = '{formCode}' AND COMPANY_CODE = '{company_code}')
                             ORDER BY VOUCHER_NO";

            if (tableNameQueryResult != null && tableNameQueryResult.REF_TABLE_NAME == "IP_PRODUCTION_MRR")
            {
                query = GetRelatedReferenceTableMRR(company_code, branch_code, tableNameQueryResult.REF_TABLE_NAME, formCode.ToString(), searchText);
            }

            var result = this._dbContext.SqlQuery<RelatedReferenceNoModel>(query).ToList();

            if (result != null && result.Count > 0)
            {
                result[0].REFERENCE_TYPE = tableNameQueryResult != null && tableNameQueryResult.REF_TABLE_NAME == "IP_PRODUCTION_MRR" ? "IP_PRODUCTION_MRR" : "IP_PRODUCTION_ISSUE";
            }

            return result;
        }

        public class ReferenceTableModel
        {
            public string REF_TABLE_NAME { get; set; }

            public string REF_COLUMN_NAME { get; set; }
        }


        public ItemLocationDetailUisngVoucherNoModel GetItemLocationDetailUsingIssueNo(string issue_no, int formCode)
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;

            var tableNameQuery = $@"SELECT ref_table_name, ref_column_name FROM form_setup WHERE form_code='{formCode}' AND COMPANY_CODE='{company_code}'";
            var tableNameQueryResult = this._dbContext.SqlQuery<ReferenceTableModel>(tableNameQuery).FirstOrDefault();

            string query = $@"SELECT A.TO_LOCATION_CODE,
                  B.LOCATION_EDESC,
                  A.BATCH_NO,
                  a.PRODUCTION_QTY,
                  WT_PER_LTR,
                  SUM(A.QUANTITY) as TOTAL_PRODUCTION_QTY
             FROM IP_PRODUCTION_ISSUE A, IP_LOCATION_SETUP B
            WHERE A.COMPANY_CODE = B.COMPANY_CODE
                  AND A.TO_LOCATION_CODE = B.LOCATION_CODE
                  AND A.COMPANY_CODE = '{company_code}'
                  AND A.ISSUE_NO = '{issue_no}'
         GROUP BY A.TO_LOCATION_CODE,
                  B.LOCATION_EDESC,
                  A.BATCH_NO,
                  a.PRODUCTION_QTY,
                  A.WT_PER_LTR";

            if (tableNameQueryResult != null && tableNameQueryResult.REF_TABLE_NAME == "IP_PRODUCTION_MRR")
            {
                query = GetRelatedTableLocationAndQty(company_code, branch_code, issue_no);
            }

            var result = this._dbContext.SqlQuery<ItemLocationDetailUisngVoucherNoModel>(query).FirstOrDefault();
            return result;
        }




        private string GetRelatedTableLocationAndQty(string companyCode, string branchCode, string voucherNo)
        {
            return $@"SELECT A.TO_LOCATION_CODE,
                                 B.LOCATION_EDESC,
                                 A.BATCH_NO,
                                 A.PLAN_NO,
                                 SUM (A.QUANTITY) TOTAL_PRODUCTION_QTY
                            FROM IP_PRODUCTION_MRR A, IP_LOCATION_SETUP B
                           WHERE     A.COMPANY_CODE = B.COMPANY_CODE
                                 AND A.TO_LOCATION_CODE = B.LOCATION_CODE
                                 AND A.COMPANY_CODE = '{companyCode}'
                                 AND A.MRR_NO = '{voucherNo}'
                        GROUP BY A.TO_LOCATION_CODE, A.BATCH_NO, A.PLAN_NO, B.LOCATION_EDESC";
        }
        private string GetRelatedReferenceTableMRR(string companyCode, string branchCode, string tblName, string fromCode, string searchText = "")
        {

            return $@" SELECT DISTINCT
                             VOUCHER_NO REFERENCE_NO,
                             VOUCHER_NO,
                                (SELECT LOCATION_EDESC from IP_LOCATION_SETUP where LOCATION_CODE = (SELECT DISTINCT FROM_LOCATION_CODE
                                FROM IP_PRODUCTION_MRR
                               WHERE     COMPANY_CODE = MASTER_TRANSACTION.COMPANY_CODE
                                     AND MRR_NO = MASTER_TRANSACTION.VOUCHER_NO))
                                LOT_NO,
                             FORM_CODE
                        FROM MASTER_TRANSACTION
                       WHERE     UPPER (VOUCHER_NO) LIKE '%{searchText}%'
                             AND COMPANY_CODE = '{companyCode}'
                             AND BRANCH_CODE = '{branchCode}'
                             AND DELETED_FLAG = 'N'
                             AND FORM_CODE IN
                                    (SELECT FORM_CODE
                                       FROM FORM_DETAIL_SETUP
                                      WHERE     COMPANY_CODE = '{companyCode}'
                                            AND TABLE_NAME = '{tblName}')
                             AND VOUCHER_NO NOT IN
                                    (SELECT NVL (REFERENCE_NO, 0)
                                       FROM IP_PRODUCTION_ISSUE
                                      WHERE     COMPANY_CODE = '{companyCode}'
                                            AND BRANCH_CODE = '{branchCode}'
                                            AND DELETED_FLAG = 'N')
                             AND FORM_CODE IN (SELECT REF_FORM_CODE
                                                 FROM FORM_SETUP
                                                WHERE FORM_CODE = '{fromCode}' AND COMPANY_CODE = '{companyCode}')
                    ORDER BY 2";
        }

        #endregion mahesh

        #region Helper Methods

        /// <summary>
        /// Gets the ATTRIBUTE_FLAG from PREFERENCE_SETUP with caching to avoid repeated database calls
        /// </summary>
        /// <returns>ATTRIBUTE_FLAG value (Y/N) or null if not found</returns>


        #endregion Helper Methods

        public AccountInfoDataModel GetAccountInfoData(string accCode, string companyCode)
        {
            string query = $@"SELECT DISTINCT
         A.ACC_CODE,
         A.ACC_ID,
         A.ACC_EDESC,
         A.LIMIT,
         A.ACC_NATURE,
         A.SHARE_VALUE,
         A.ACC_TYPE_FLAG,
         A.TPB_FLAG,
         A.TRANSACTION_TYPE,
         A.MOBILE_NO,
         A.TEL_NO,
         A.CREATED_BY,
         A.CREATED_DATE,
         A.MODIFY_BY,
         A.MODIFY_DATE,
         NVL(SUM((SELECT SUM(NVL(C.BUDGET_DRAMOUNT,0)) - SUM(NVL(C.BUDGET_CRAMOUNT,0))
           FROM FA_COA_BUDGET_SETUP C
           WHERE C.COMPANY_CODE = A.COMPANY_CODE
           AND TRIM(C.ACC_CODE) = TRIM(A.ACC_CODE)
           AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}')),0) COA_BUDGET_AMT,
         NVL(SUM((SELECT SUM(NVL(C.DR_AMOUNT,0)) - SUM(NVL(C.CR_AMOUNT,0))
           FROM V$VIRTUAL_GENERAL_LEDGER C
           WHERE C.COMPANY_CODE = A.COMPANY_CODE
           AND TRIM(C.ACC_CODE) = TRIM(A.ACC_CODE)
           AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}')),0) GENERIC_BL_AMT,
         NVL(SUM((SELECT SUM(NVL(C.DR_AMOUNT,0)) - SUM(NVL(C.CR_AMOUNT,0))
           FROM V$VIRTUAL_GENERAL_LEDGER C
           WHERE C.COMPANY_CODE = A.COMPANY_CODE
           AND TRIM(C.ACC_CODE) = TRIM(A.ACC_CODE)
           AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
           AND C.POSTED_BY IS NOT NULL)),0) POSTED_BL_AMT
    FROM FA_CHART_OF_ACCOUNTS_SETUP A
    WHERE A.DELETED_FLAG = 'N'
      AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
      AND A.ACC_CODE = '{accCode}' 
    GROUP BY A.ACC_CODE, A.ACC_ID, A.ACC_EDESC, A.LIMIT, A.ACC_NATURE,
             A.SHARE_VALUE, A.ACC_TYPE_FLAG, A.TPB_FLAG, A.TRANSACTION_TYPE,
             A.MOBILE_NO, A.TEL_NO, A.CREATED_BY, A.CREATED_DATE,
             A.MODIFY_BY, A.MODIFY_DATE";
            // Execute query, map result to AccountInfoDataModel, return it
            // Use the same pattern as GetProductInfoData


            return _dbContext.SqlQuery<AccountInfoDataModel>(query).FirstOrDefault() ?? new AccountInfoDataModel();
        }

        #region Product Info Hover - Dynamic Data Loading

        public ProductInfoDataModel GetProductInfoData(string itemCode, string companyCode, string branchCode, string voucherNo = null, string serialNo = null)
        {
            var result = new ProductInfoDataModel
            {
                ProductInfo = GetProductBasicInfo(itemCode, companyCode),
                StockByLocation = GetClosingStockByLocation(itemCode, companyCode, branchCode, voucherNo, serialNo),
                SalesHistory = GetSalesRateHistory(itemCode, companyCode)
            };

            return result;
        }

        private ProductBasicInfo GetProductBasicInfo(string itemCode, string companyCode)
        {
            string query = $@"
                SELECT 
                    INITCAP(A.ITEM_EDESC) AS ProductName,
                    B.CATEGORY_EDESC AS Category,
                    C.MU_EDESC AS Unit,
                    A.MIN_LEVEL AS MinLevel,
                    A.MAX_LEVEL AS MaxLevel,
                    A.REORDER_LEVEL AS ReorderLevel,
                    A.PRODUCT_CODE AS ProductCode,
                    A.LEAD_TIME AS LeadTime,
                    A.SERIAL_FLAG AS SerialFlag,
                    A.BATCH_SERIAL_FLAG AS BatchSerialFlag,
                    A.CREATED_BY AS CreatedBy,
                    A.CREATED_DATE AS CreatedDate,
                    A.MODIFY_BY AS ModifyBy,
                    A.MODIFY_DATE AS ModifyDate,
                    A.SERVICE_ITEM_FLAG AS ServiceItemFlag,
                    (SELECT MU_CODE FROM IP_ITEM_UNIT_SETUP 
                     WHERE ITEM_CODE = A.ITEM_CODE 
                     AND COMPANY_CODE = A.COMPANY_CODE 
                     AND SERIAL_NO = 1) AS AltMuCode,
                    A.HS_CODE AS HsCode
                FROM IP_ITEM_MASTER_SETUP A
                LEFT JOIN IP_CATEGORY_CODE B 
                    ON A.CATEGORY_CODE = B.CATEGORY_CODE 
                    AND A.COMPANY_CODE = B.COMPANY_CODE
                LEFT JOIN IP_MU_CODE C 
                    ON A.INDEX_MU_CODE = C.MU_CODE 
                    AND A.COMPANY_CODE = C.COMPANY_CODE
                WHERE A.ITEM_CODE = '{itemCode}' 
                AND A.COMPANY_CODE = '{companyCode}' 
                AND A.DELETED_FLAG = 'N'";

            return _dbContext.SqlQuery<ProductBasicInfo>(query).FirstOrDefault() ?? new ProductBasicInfo();
        }

        private List<StockLocationModel> GetClosingStockByLocation(string itemCode, string companyCode, string branchCode, string voucherNo, string serialNo)
        {
            string excludeCondition = "";
            if (!string.IsNullOrEmpty(voucherNo) && !string.IsNullOrEmpty(serialNo))
            {
                excludeCondition = $@"AND a.VOUCHER_NO||a.SERIAL_NO NOT IN(
                    SELECT VOUCHER_NO||SERIAL_NO 
                    FROM V$VIRTUAL_STOCK_WIP_LEDGER1 
                    WHERE VOUCHER_NO = '{voucherNo}' 
                    AND SERIAL_NO = '{serialNo}' 
                    AND COMPANY_CODE = '{companyCode}' 
                    AND ITEM_CODE = '{itemCode}')";
            }

            string query = $@"
                SELECT DISTINCT 
                    INITCAP(b.LOCATION_EDESC) AS LocationName,
                    NVL(SUM(NVL(a.IN_QUANTITY,0)) - SUM(NVL(a.OUT_QUANTITY,0)),0) AS Stock
                FROM V$VIRTUAL_STOCK_WIP_LEDGER1 a
                JOIN IP_LOCATION_SETUP b 
                    ON a.LOCATION_CODE = b.LOCATION_CODE 
                    AND a.COMPANY_CODE = b.COMPANY_CODE
                WHERE a.ITEM_CODE = '{itemCode}' 
                AND a.COMPANY_CODE = '{companyCode}'  
                AND a.BRANCH_CODE = '{branchCode}'
                {excludeCondition}
                GROUP BY INITCAP(b.LOCATION_EDESC) 
                ORDER BY INITCAP(b.LOCATION_EDESC)";

            return _dbContext.SqlQuery<StockLocationModel>(query).ToList();
        }

        private List<SalesRateHistoryModel> GetSalesRateHistory(string itemCode, string companyCode)
        {
            string query = $@"
                SELECT 
                    A.CUSTOMER_CODE AS CustomerCode,
                    NVL(A.CALC_UNIT_PRICE,0) * NVL(A.EXCHANGE_RATE,1) AS SalesPrice,
                    A.SALES_DATE AS SalesDate,
                    INITCAP(B.CUSTOMER_EDESC) AS CustomerName
                FROM SA_SALES_INVOICE A
                JOIN SA_CUSTOMER_SETUP B 
                    ON A.CUSTOMER_CODE = B.CUSTOMER_CODE 
                    AND A.COMPANY_CODE = B.COMPANY_CODE
                WHERE A.ITEM_CODE = '{itemCode}' 
                AND A.COMPANY_CODE = '{companyCode}'
                ORDER BY A.SALES_DATE DESC";

            // Return only top 3 for last 3 customers
            return _dbContext.SqlQuery<SalesRateHistoryModel>(query).Take(3).ToList();
        }

        #endregion Product Info Hover


    }
}
