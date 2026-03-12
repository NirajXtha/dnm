using NeoERP.DocumentTemplate.Service.Interface;
using System;
using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Data;
using System.Collections.Generic;
using NeoERP.DocumentTemplate.Service.Models;
using System.Linq;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Core.Models;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace NeoERP.DocumentTemplate.Service.Repository
{
    public class FormSetupRepo : IFormSetupRepo
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;
        private int serialNo = 0;
        private readonly string XmlPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"Areas\NeoERP.DocumentTemplate\App_Data";
        public FormSetupRepo(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity objectEntity)
        {
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
            //this._logErp = new LogErp(this);
        }
        //public List<FormSetup> GetFormSetup()
        //{
        //    string Query = $@"SELECT FORM_CODE,FORM_EDESC,MASTER_FORM_CODE,PRE_FORM_CODE,MODULE_CODE,GROUP_SKU_FLAG,START_ID_FLAG,ID_GENERATION_FLAG,CUSTOM_SUFFIX_TEXT,START_DATE,
        //    LAST_DATE,REF_COLUMN_NAME,PRINT_REPORT_FLAG,PRIMARY_MANUAL_FLAG,COMPANY_CODE,CREATED_DATE,DELETED_FLAG,REFERENCE_FLAG,FORM_ACTION_FLAG,MODIFY_DATE FROM FORM_SETUP where COMPANY_CODE='01' ORDER BY TO_NUMBER(FORM_CODE) ASC";
        //    List<FormSetup> entity = this._dbContext.SqlQuery<FormSetup>(Query).ToList();
        //    return entity;
        //}
        public List<FormSetup> GetFormSetup()
        {
            string Query = $@"SELECT FORM_CODE,FORM_EDESC,MASTER_FORM_CODE,PRE_FORM_CODE,MODULE_CODE,GROUP_SKU_FLAG,START_ID_FLAG,ID_GENERATION_FLAG,CUSTOM_SUFFIX_TEXT,
            REF_COLUMN_NAME,PRINT_REPORT_FLAG,PRIMARY_MANUAL_FLAG,COMPANY_CODE,DELETED_FLAG,REFERENCE_FLAG,FORM_ACTION_FLAG FROM FORM_SETUP where COMPANY_CODE='01' ORDER BY TO_NUMBER(FORM_CODE) ASC";
            List<FormSetup> entity = this._dbContext.SqlQuery<FormSetup>(Query).ToList();
            return entity;
        }
        public List<FormSetup> GetFormSetupByFormCode(string formCode)
        {
            string Query = $@"SELECT FORM_CODE,FORM_EDESC,MASTER_FORM_CODE,PRE_FORM_CODE,MODULE_CODE,GROUP_SKU_FLAG,START_ID_FLAG,ID_GENERATION_FLAG,CUSTOM_SUFFIX_TEXT,START_DATE,
            LAST_DATE,REF_COLUMN_NAME,PRINT_REPORT_FLAG,PRIMARY_MANUAL_FLAG,COMPANY_CODE,CREATED_DATE,DELETED_FLAG,REFERENCE_FLAG,FORM_ACTION_FLAG,MODIFY_DATE,RECEIPT_FLAG FROM FORM_SETUP WHERE FORM_CODE='{formCode}' AND COMPANY_CODE={_workContext.CurrentUserinformation.company_code}  ORDER BY TO_NUMBER(FORM_CODE) ASC";
            _logErp.InfoInFile(Query + " is query to fetch formsetupby form code");
            List<FormSetup> entity = this._dbContext.SqlQuery<FormSetup>(Query).ToList();
            return entity;
        }

        #region Draft Menu
        #region Inventory
        public List<TemplateDraftListModel> GetAllMenuInventoryAssigneeDraftTemplateList()
        {
            string Query = $@"SELECT DISTINCT FS.MODULE_CODE AS MODULE_CODE,FT.FORM_CODE AS FORM_CODE,FS.FORM_EDESC AS FORM_EDESC,FS.FORM_TYPE, TO_CHAR(FT.TEMPLATE_NO) TEMPLATE_CODE, FT.TEMPLATE_EDESC as TEMPLATE_EDESC FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE 
                  AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  
                  AND ASSIGNEE='{_workContext.CurrentUserinformation.User_id}'
                  AND SAVED_DRAFT IS NULL
                   ORDER BY TO_NUMBER(FT.FORM_CODE) ASC";
            List<TemplateDraftListModel> record = this._dbContext.SqlQuery<TemplateDraftListModel>(Query).ToList();
            return record;
        }

        public List<TemplateDraftListModel> GetAllMenuInventoryAssigneeSavedDraftTemplateList()
        {
            string Query = $@"SELECT DISTINCT FS.MODULE_CODE AS MODULE_CODE,FT.FORM_CODE AS FORM_CODE,FS.FORM_EDESC AS FORM_EDESC,FS.FORM_TYPE, TO_CHAR(FT.TEMPLATE_NO) TEMPLATE_CODE, FT.TEMPLATE_EDESC as TEMPLATE_EDESC FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE
                  AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                  AND ASSIGNEE='{_workContext.CurrentUserinformation.User_id}'
                  AND SAVED_DRAFT IS NOT NULL
                   ORDER BY TO_NUMBER(FT.FORM_CODE) ASC";
            List<TemplateDraftListModel> record = this._dbContext.SqlQuery<TemplateDraftListModel>(Query).ToList();
            return record;
        }
        #endregion
        #region Finance Voucher
        public List<TemplateDraftListModel> GetAllMenuFinanceVoucherAssigneeDraftTemplateList()
        {
            string Query = $@"SELECT DISTINCT FS.MODULE_CODE AS MODULE_CODE,FT.FORM_CODE AS FORM_CODE,FS.FORM_EDESC AS FORM_EDESC, TO_CHAR(FT.TEMPLATE_NO) TEMPLATE_CODE, FT.TEMPLATE_EDESC as TEMPLATE_EDESC FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='01' 
                  AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  
                  AND ASSIGNEE='{_workContext.CurrentUserinformation.User_id}'
                  AND SAVED_DRAFT IS NULL
                   ORDER BY TO_NUMBER(FT.FORM_CODE) ASC";
            List<TemplateDraftListModel> record = this._dbContext.SqlQuery<TemplateDraftListModel>(Query).ToList();
            return record;
        }

        public List<TemplateDraftListModel> GetAllMenuFinanceVoucherAssigneeSavedDraftTemplateList()
        {
            string Query = $@"SELECT DISTINCT FS.MODULE_CODE AS MODULE_CODE,FT.FORM_CODE AS FORM_CODE,FS.FORM_EDESC AS FORM_EDESC, TO_CHAR(FT.TEMPLATE_NO) TEMPLATE_CODE, FT.TEMPLATE_EDESC as TEMPLATE_EDESC FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='01' 
                  AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                  AND ASSIGNEE='{_workContext.CurrentUserinformation.User_id}' 
                  AND SAVED_DRAFT IS NOT NULL
                   ORDER BY TO_NUMBER(FT.FORM_CODE) ASC";
            List<TemplateDraftListModel> record = this._dbContext.SqlQuery<TemplateDraftListModel>(Query).ToList();
            return record;
        }
        #endregion
        #region Sales

        public List<TemplateDraftListModel> GetAllMenuSalesAssigneeDraftTemplateList()
        {
            string Query = $@"SELECT DISTINCT FS.MODULE_CODE AS MODULE_CODE,FT.FORM_CODE AS FORM_CODE,FS.FORM_EDESC AS FORM_EDESC, TO_CHAR(FT.TEMPLATE_NO) TEMPLATE_CODE, FT.TEMPLATE_EDESC as TEMPLATE_EDESC FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='02' 
                  AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'  
                  AND ASSIGNEE='{_workContext.CurrentUserinformation.User_id}'
                  AND SAVED_DRAFT IS NULL
                   ORDER BY TO_NUMBER(FT.FORM_CODE) ASC";
            List<TemplateDraftListModel> record = this._dbContext.SqlQuery<TemplateDraftListModel>(Query).ToList();
            return record;
        }

        public List<TemplateDraftListModel> GetAllMenuSalesAssigneeSavedDraftTemplateList()
        {
            string Query = $@"SELECT DISTINCT FS.MODULE_CODE AS MODULE_CODE,FT.FORM_CODE AS FORM_CODE,FS.FORM_EDESC AS FORM_EDESC, TO_CHAR(FT.TEMPLATE_NO) TEMPLATE_CODE, FT.TEMPLATE_EDESC as TEMPLATE_EDESC FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='02' 
                  AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                  AND ASSIGNEE='{_workContext.CurrentUserinformation.User_id}'
                  AND SAVED_DRAFT IS NOT NULL
                   ORDER BY TO_NUMBER(FT.FORM_CODE) ASC";
            List<TemplateDraftListModel> record = this._dbContext.SqlQuery<TemplateDraftListModel>(Query).ToList();
            return record;
        }
        #endregion
        #endregion

        #region Draft 
        public List<TemplateDraftListModel> GetAllDraftTemplateListByFormCode(string formCode)
        {
            string Query = $@"SELECT DISTINCT FS.MODULE_CODE AS MODULE_CODE,FT.FORM_CODE AS FORM_CODE,FS.FORM_EDESC AS FORM_EDESC, TO_CHAR(FT.TEMPLATE_NO) TEMPLATE_CODE, FT.TEMPLATE_EDESC as TEMPLATE_EDESC FROM FORM_TEMPLATE_SETUP FT,FORM_SETUP FS
                  WHERE FT.DELETED_FLAG='N' AND FT.FORM_CODE(+)=FS.FORM_CODE
                  AND FT.COMPANY_CODE(+)=FS.COMPANY_CODE AND  FS.MODULE_CODE='02' 
                  AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                  AND FT.FORM_CODE='{formCode}'
                  ORDER BY TO_NUMBER(FT.FORM_CODE) ASC";
            List<TemplateDraftListModel> record = this._dbContext.SqlQuery<TemplateDraftListModel>(Query).ToList();
            return record;
        }

        public List<DraftFormModel> GetAllDraftTemplateDatabyTempCode(string tempCode)
        {
            string Query = $@"SELECT FTD.TEMPLATE_NO AS TEMPLATE_NO, FTD.FORM_CODE AS FORM_CODE, FTD.SERIAL_NO AS SERIAL_NO,FTD.COLUMN_NAME AS COLUMN_NAME,FTD.COLUMN_VALUE AS COLUMN_VALUE , FTD.TABLE_NAME  AS TABLE_NAME
                 from FORM_TEMPLATE_DETAIL_SETUP FTD WHERE TEMPLATE_NO='{tempCode}'  AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
            List<DraftFormModel> record = this._dbContext.SqlQuery<DraftFormModel>(Query).ToList();
            return record;
        }

        public List<CashVoucherReceipt> GetAllReceiptForCashVoucher()
        {
            string Query = $@"  SELECT R.SERIAL_NO, R.RECEIPT_NAME, R.ACC_CODE, A.ACC_EDESC
                                FROM FA_RECEIPT_MODE_SETUP R
                                JOIN FA_CHART_OF_ACCOUNTS_SETUP A
                                ON A.ACC_CODE = R.ACC_CODE
                                AND A.DELETED_FLAG = 'N'
                                AND A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                               WHERE R.DELETED_FLAG = 'N' AND R.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND R.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                            ORDER BY R.SERIAL_NO";
            List<CashVoucherReceipt> record = this._dbContext.SqlQuery<CashVoucherReceipt>(Query).ToList();
            return record;
        }


        #endregion



        public List<FormSetup> GetFormSetup(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter = string.Empty;

                }

                var countryQuery = $@"select distinct
                             COALESCE(FORM_CODE,' ') FORM_CODE
                            ,COALESCE(FORM_EDESC,' ') FORM_EDESC
                            FROM FORM_SETUP 
                            WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND (FORM_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(FORM_EDESC) like '%{filter.ToUpperInvariant()}%') ";
                var result = _dbContext.SqlQuery<FormSetup>(countryQuery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
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


        public object GetNextScreenFormCodeDetails(string formcode, string orderno = "")
        {
            var module_code = "03"; // Production has module_code='03';
            string Query = $@"select Form_Code from Form_SETUP where Module_Code ='{module_code}' and company_code='{_workContext.CurrentUserinformation.company_code}' and REF_FORM_CODE='{formcode}'";
            var resultFormCode = this._dbContext.SqlQuery<string>(Query).FirstOrDefault();
            string resultOrderNo = "";
            if (!string.IsNullOrEmpty(orderno))
            {
                string queryTemp = $@"select MRR_NO FROM IP_PRODUCTION_MRR
                                WHERE REFERENCE_NO = '{orderno}' and Company_Code='{_workContext.CurrentUserinformation.company_code}' and Branch_Code='{_workContext.CurrentUserinformation.branch_code}'";
                resultOrderNo = this._dbContext.SqlQuery<string>(queryTemp).FirstOrDefault();
            }
            return new { Form_Code = resultFormCode, Order_No = resultOrderNo };
        }

        public IssueScreenDtlModule GetIssueScreenFormCodeDetails(string orderno = "")
        {
            IssueScreenDtlModule issueScreenDtlModule = new IssueScreenDtlModule();
            if (!string.IsNullOrEmpty(orderno))
            {
                string refNoQuery = $@"select REFERENCE_NO from IP_PRODUCTION_MRR where company_code = '{_workContext.CurrentUserinformation.company_code}' and MRR_NO = '{orderno}'";

                var refNumber = this._dbContext.SqlQuery<string>(refNoQuery).FirstOrDefault();

                string queryTemp = $@"SELECT Form_Code,VOUCHER_NO
                                          FROM MASTER_TRANSACTION
                                         WHERE    
                                                company_code = '{_workContext.CurrentUserinformation.company_code}'
                                               AND VOUCHER_NO = '{refNumber}'";

                issueScreenDtlModule = this._dbContext.SqlQuery<IssueScreenDtlModule>(queryTemp).FirstOrDefault();
                if (issueScreenDtlModule != null && !string.IsNullOrEmpty(issueScreenDtlModule.VOUCHER_NO))
                {
                    issueScreenDtlModule.VOUCHER_NO = issueScreenDtlModule.VOUCHER_NO.Replace("/", "_");
                }
            }
            return issueScreenDtlModule;
        }

        public string GetNextScreenOrderNo(string orderNo)
        {
            string Query = $@"select MRR_NO FROM IP_PRODUCTION_MRR
                                WHERE REFERENCE_NO = '{orderNo}' and Company_Code='{_workContext.CurrentUserinformation.company_code}' and Branch_Code='{_workContext.CurrentUserinformation.company_code}'";
            var result = this._dbContext.SqlQuery<string>(Query).FirstOrDefault();
            return result;
        }


        public class FORM_SETUP_Model
        {

        }


        public int GetBackDaysByFormCode(string formCode)
        {
            int result = 0;
            string query = $@"select FREEZE_BACK_DAYS from form_setup where form_code='{formCode}' AND DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'";
            _logErp.InfoInFile("Query for calculating sales back days include: " + query);
            result = this._dbContext.SqlQuery<int>(query).FirstOrDefault();
            _logErp.InfoInFile(result + " backdays is fetched for " + formCode + " formcode");
            return result;
        }

        #region SALES_CHALAN_FROM_EXCEL

        //public string SaveSalesChalanFromExcel(List<SalesChalanExcelData> chalanList, string formCode, string tableName)
        //{
        //    //using (var chalanTran = _objectEntity.Database.BeginTransaction())
        //    //{
        //    try
        //    {

        //        if (chalanList.Count < 0) return "I Think No Row In Excel";

        //        var inSalesChalan = false;
        //        var inMaster = false;
        //        var inCharge = false;
        //        var chalanCount = 0;
        //        //var location = CreateStaticLocation();
        //        var commonProp = new CommonPropForChalan()
        //        {
        //            Form_Code = formCode,
        //            Table_Name = tableName,
        //            Location_Code = CreateStaticLocation(),
        //            Company_Code = _workContext.CurrentUserinformation.company_code
        //        };
        //        _logErp.InfoInFile("Import Chalan Log:====>>>common Objecet is" + commonProp);
        //        var custGrpResponse = new Dictionary<string, string>();
        //        var custIndResponse = new Dictionary<string, string>();
        //        var itmResponse = new Dictionary<string, string>();
        //        var delrResponse = new Dictionary<string, string>();
        //        foreach (var chalan in chalanList)
        //        {

        //            var dealerCode = "";
        //            var newVoucherNo = NewVoucherNo(commonProp.Company_Code, commonProp.Form_Code, DateTime.Now.ToString(), commonProp.Table_Name);
        //            _logErp.InfoInFile("Import Chalan Log:====>>>New Voucher Number Is" + newVoucherNo);
        //            commonProp.New_Voucher_No = newVoucherNo;


        //            if (!IsCustomerExists(chalan, custGrpResponse.Count == 0 ? null : custGrpResponse, out custGrpResponse, _objectEntity))
        //            {
        //                _logErp.InfoInFile("Import Chalan Log:====>>>customer doesn't exists" + chalan.CustomerName);
        //                var response = AddNewChildCustomer(chalan.CustomerName, chalan.CustomerPhoneNo, custGrpResponse, _objectEntity);
        //                commonProp.Customer_Code = response;
        //            }
        //            else commonProp.Customer_Code = custGrpResponse["CUSTOMER_CODE"];


        //            if (!IsItemExists(chalan, commonProp.Form_Code, itmResponse.Count == 0 ? null : itmResponse, out itmResponse, _objectEntity))
        //            {
        //                _logErp.InfoInFile("Import Chalan Log:====>>>item doesn't exists" + chalan.ItemName);
        //                var iteRes = AddNewItem(chalan, itmResponse, _objectEntity);
        //                commonProp.Item_Code = iteRes;
        //            }
        //            else commonProp.Item_Code = itmResponse["ITEM_CODE"];

        //            if (!IsDealerExists(chalan.Dealer, delrResponse.Count == 0 ? null : delrResponse, out delrResponse, _objectEntity))
        //            {
        //                _logErp.InfoInFile("Import Chalan Log:====>>>dealer doesn't exists" + chalan.Dealer);
        //                dealerCode = AddNewDealer(chalan.Dealer, dealerCode, delrResponse, _objectEntity);
        //                commonProp.Dealer_Code = dealerCode;

        //            }
        //            else commonProp.Dealer_Code = delrResponse["PARTY_TYPE_CODE"];


        //            inSalesChalan = SaveSalesChalanValues(chalan, commonProp, _objectEntity);
        //            if (inSalesChalan)
        //            {
        //                inMaster = SaveMasterTransactionValue(chalan, commonProp, _objectEntity);

        //                inCharge = SaveChargeColumnValue(chalan, commonProp, _objectEntity);


        //            }

        //            _objectEntity.SaveChanges();

        //        }

        //        if (inCharge & inMaster) { return "Success"; }
        //        else return "Error";

        //    }
        //    catch (Exception ex)
        //    {
        //        // chalanTran.Rollback();
        //        throw ex;
        //    }
        //    //}
        //}

        public string SaveSalesChalanFromExcel(List<SalesChalanExcelData> chalanList, string formCode, string tableName)
        {
            using (var chalanTran = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    chalanList = chalanList.Where(x => !string.IsNullOrEmpty(x.ManualNo)).ToList();
                    if (chalanList.Count < 0 || chalanList.Count == 0) return "I Think No Row In Excel";
                    var existedChalanNo = new List<string>();
                    var chalanListByCustomer = new List<SalesChalanExcelData>();
                    var chalanListByItem = new List<SalesChalanExcelData>();
                    var chalanNoQuery = $@"SELECT DISTINCT REFERENCE_NO ManualNo 
                                                        FROM MASTER_TRANSACTION MT
                                                       -- LEFT JOIN SA_SALES_CHALAN SSC ON SSC.MANUAL_NO = MT.REFERENCE_NO
                                                        WHERE  MT.DELETED_FLAG = 'N' AND MT.REFERENCE_NO IS NOT NULL";
                    existedChalanNo = _dbContext.SqlQuery<string>(chalanNoQuery).ToList();
                    chalanList = chalanList.Where(x => !existedChalanNo.Contains(x.ManualNo)).ToList();

                    if (chalanList.Count == 0) return "I Think No New Order  In Excel";

                    var newCustomerQuery = $@"SELECT DISTINCT TEL_MOBILE_NO1 FROM SA_CUSTOMER_SETUP CS WHERE CS.TEL_MOBILE_NO1 IS NOT NULL AND CS.DELETED_FLAG='N' AND CS.CUSTOMER_CODE IS NOT NULL";
                    var newCustomerList = _dbContext.SqlQuery<string>(newCustomerQuery).ToList();
                    //chalanListByCustomer = chalanList.Where(x => !newCustomerList.Contains(x.CustomerPhoneNo)).ToList();
                    chalanListByCustomer = chalanList.Where(x => !newCustomerList.Contains(x.CustomerPhoneNo)).Distinct().ToList();
                    chalanListByCustomer = chalanListByCustomer.GroupBy(x => x.CustomerPhoneNo).Select(x => x.FirstOrDefault()).Distinct().ToList();



                    var newItemQuery = $@"SELECT DISTINCT PRODUCT_CODE FROM IP_ITEM_MASTER_SETUP IMS WHERE IMS.DELETED_FLAG='N' AND IMS.PRODUCT_CODE IS NOT NULL";
                    var newItemList = _dbContext.SqlQuery<string>(newItemQuery).ToList();
                    chalanListByItem = chalanList.Where(x => !newItemList.Contains(x.ItemCode)).ToList();
                    if (chalanListByCustomer.Count > 0 || chalanListByItem.Count > 0)
                    {
                        var cust = string.Empty;
                        var item = string.Empty;
                        var newCustDealerAccountQuery = string.Empty;
                        //var newCustomer = null;
                        foreach (var newCust in chalanListByCustomer)
                        {
                            newCustDealerAccountQuery = $@"SELECT DISTINCT SA.CUSTOMER_NDESC CommonCode
	                                                ,SA.MASTER_CUSTOMER_CODE MasterCustomerCode
                                                    ,SA.PRE_CUSTOMER_CODE PreCustomerCode
                                                    ,SA.COMPANY_CODE CompanyCode
                                                    ,SA.BRANCH_CODE BranchCode
	                                                ,COA.ACC_CODE AccountCode
	                                                ,(
		                                                SELECT PTC.PARTY_TYPE_CODE
		                                                FROM IP_PARTY_TYPE_CODE PTC
		                                                WHERE PTC.PARTY_TYPE_FLAG = 'D'
			                                                AND PTC.REMARKS = '{newCust.CADCode}'
		                                                )  DealerPartyTypeCode
	                                                ,(
		                                                SELECT PTC.PARTY_TYPE_CODE
		                                                FROM IP_PARTY_TYPE_CODE PTC
		                                                WHERE PTC.PARTY_TYPE_FLAG = 'P'
			                                                AND PTC.REMARKS = '{newCust.CADCode}'
		                                                )  CustomerPartyTypeCode
                                                FROM SA_CUSTOMER_SETUP SA
	                                                ,FA_CHART_OF_ACCOUNTS_SETUP COA
	                                                ,IP_PARTY_TYPE_CODE PTC
                                                WHERE SA.CUSTOMER_NDESC = COA.ACC_SNAME
	                                                AND SA.CUSTOMER_NDESC = PTC.REMARKS
	                                                -- AND PTC.REMARKS = COA.ACC_SNAME
	                                                AND SA.COMPANY_CODE = COA.COMPANY_CODE
	                                                AND SA.COMPANY_CODE = PTC.COMPANY_CODE
	                                                AND SA.DELETED_FLAG = 'N'
	                                                AND PTC.DELETED_FLAG = 'N'
	                                                AND COA.DELETED_FLAG = 'N'
	                                                AND SA.CUSTOMER_NDESC = '{newCust.CADCode}'";
                            var newCustDealerAccount = _dbContext.SqlQuery<CustomerDealerAccountModel>(newCustDealerAccountQuery).FirstOrDefault();


                            if (newCustDealerAccount != null)
                            {
                                var customerCode = AddNewCustomerDealerAccount(newCustDealerAccount, newCust);
                                chalanList.Where(c => c.CustomerName == newCust.CustomerName && c.CustomerPhoneNo == newCust.CustomerPhoneNo).Select(c => { c.PartyTypeCode = newCustDealerAccount.DealerPartyTypeCode; return c; }).ToList();
                                chalanList.Where(c => c.CustomerName == newCust.CustomerName && c.CustomerPhoneNo == newCust.CustomerPhoneNo).Select(c => { c.CustomerCode = customerCode; return c; }).ToList();
                            }
                            else
                            {
                                cust += "OrderNo:" + newCust.ChalanNo + "    " + "DealerCode: " + newCust.Dealer + "      " + "CustomerName: " + newCust.CustomerName + "/n";
                            }
                        }
                        foreach (var newItem in chalanListByItem)
                        {
                            item += "OrderNo: " + newItem.ChalanNo + "     " + "ItemCode: " + newItem.ItemCode + "     " + "ItemName: " + newItem.ItemName + "/n";

                        }
                        if (!string.IsNullOrEmpty(cust) || !string.IsNullOrEmpty(item))
                        {
                            return cust + "/s" + item;
                        }

                    }

                    var inSalesChalan = false;
                    var inMaster = false;
                    var inCharge = false;
                    //var chalanCount = 0;
                    //var location = CreateStaticLocation();
                    var groupedChalanByOrderNumber = chalanList.GroupBy(x => x.ManualNo).ToList();
                    var commonProp = new CommonPropForChalan()
                    {
                        Form_Code = formCode,
                        Table_Name = tableName,
                        Location_Code = CreateStaticLocation(),
                        Company_Code = _workContext.CurrentUserinformation.company_code
                    };
                    _logErp.InfoInFile("Import Chalan Log:====>>>common Objecet is" + commonProp);
                    var custGrpResponse = new Dictionary<string, string>();
                    var custIndResponse = new Dictionary<string, string>();
                    var itmResponse = new Dictionary<string, string>();
                    var delrResponse = new Dictionary<string, string>();
                    foreach (var groupedChalan in groupedChalanByOrderNumber)
                    {
                        var key = groupedChalan.Key;
                        var totalVat = groupedChalan.Where(x => x.ManualNo == key).Select(x => x.VAT).Sum();
                        var totalDiscount = groupedChalan.Where(x => x.ManualNo == key).Select(x => x.Discount).Sum();
                        var totalQuantity = groupedChalan.Where(x => x.ManualNo == key).Select(x => x.Quantity).Sum();
                        var totalRate = groupedChalan.Where(x => x.ManualNo == key).Select(x => x.Rate).Sum();
                        var totalTotal = groupedChalan.Where(x => x.ManualNo == key).Select(x => x.Total).Sum();
                        var chalanData = groupedChalan.Where(x => x.ManualNo == key).FirstOrDefault().OrderDate.ToShortDateString();
                        var aaaa = groupedChalan.FirstOrDefault().OrderDate.ToShortDateString();
                        //chalanData = chalanData.FirstOrDefault(x => x.Date.ToShortDateString());
                        var newVoucherNo = NewVoucherNo(commonProp.Company_Code, commonProp.Form_Code, DateTime.Now.ToString(), commonProp.Table_Name);
                        commonProp.New_Voucher_No = newVoucherNo;
                        string insertmasterQuery = $@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,
                                                              COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,SYN_ROWID,EXCHANGE_RATE,
                                                              REFERENCE_NO,PRINT_COUNT) 
                                                  VALUES('{commonProp.New_Voucher_No}','{totalTotal}',
                               '{commonProp.Form_Code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}',
                               '{_workContext.CurrentUserinformation.login_code}','N','NRS',SYSDATE,
                               TO_DATE('{chalanData}','MM/dd/yyyy'),'{key}',
                               1,'{key}',1)";

                        var rowCount = _dbContext.ExecuteSqlCommand(insertmasterQuery);
                        //if (totalVat > 0)
                        //{
                        //    string transquery = string.Format(@"select TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO )+1),1))) ORDER_NO from CHARGE_TRANSACTION");

                        //    int newtransno = _dbContext.SqlQuery<int>(transquery).FirstOrDefault();
                        //    string insertChargeQuery = $@"INSERT INTO CHARGE_TRANSACTION(TRANSACTION_NO,TABLE_NAME,REFERENCE_NO,CHARGE_CODE,CHARGE_TYPE_FLAG,
                        //                                                CHARGE_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,SESSION_ROWID) 
                        //                        VALUES('{newtransno}','{commonProp.Table_Name}','{commonProp.New_Voucher_No}','VT','A',{totalVat},
                        //                        '{commonProp.Form_Code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N','NRS',1,{key})";
                        //    _dbContext.ExecuteSqlCommand(insertChargeQuery);
                        //}
                        foreach (var chalan in groupedChalan)
                        {
                            var dealerCode = "";
                            _logErp.InfoInFile("Import Chalan Log:====>>>New Voucher Number Is" + newVoucherNo);

                            if (string.IsNullOrEmpty(chalan.CustomerCode))
                            {
                                if (!IsCustomerExists(chalan, custGrpResponse.Count == 0 ? null : custGrpResponse, out custGrpResponse, _objectEntity))
                                {
                                    _logErp.InfoInFile("Import Chalan Log:====>>>customer doesn't exists" + chalan.CustomerName);
                                    var response = AddNewChildCustomer(chalan.CustomerName, chalan.CustomerPhoneNo, custGrpResponse, _objectEntity);
                                    commonProp.Customer_Code = response;
                                }
                                else commonProp.Customer_Code = custGrpResponse["CUSTOMER_CODE"];
                            }
                            else
                            {
                                commonProp.Customer_Code = chalan.CustomerCode;
                            }


                            if (!IsItemExists(chalan, commonProp.Form_Code, itmResponse.Count == 0 ? null : itmResponse, out itmResponse, _objectEntity))
                            {
                                _logErp.InfoInFile("Import Chalan Log:====>>>item doesn't exists" + chalan.ItemName);
                                var iteRes = AddNewItem(chalan, itmResponse, _objectEntity);
                                commonProp.Item_Code = iteRes;
                            }
                            else commonProp.Item_Code = itmResponse["ITEM_CODE"];
                            if (string.IsNullOrEmpty(chalan.PartyTypeCode))
                            {
                                if (!IsDealerExists(chalan.Dealer, delrResponse.Count == 0 ? null : delrResponse, out delrResponse, _objectEntity))
                                {
                                    _logErp.InfoInFile("Import Chalan Log:====>>>dealer doesn't exists" + chalan.Dealer);
                                    dealerCode = AddNewDealer(chalan.Dealer, dealerCode, delrResponse, _objectEntity);
                                    commonProp.Dealer_Code = dealerCode;

                                }
                                else commonProp.Dealer_Code = delrResponse["PARTY_TYPE_CODE"];
                            }
                            else
                            {
                                commonProp.Dealer_Code = chalan.PartyTypeCode;
                            }
                            inSalesChalan = SaveSalesChalanValues(chalan, commonProp, _objectEntity);
                            if (inSalesChalan)
                            {
                                //inMaster = SaveMasterTransactionValue(chalan, commonProp, _objectEntity);
                                //inCharge = SaveChargeColumnValue(chalan, commonProp, _objectEntity);
                            }

                            _objectEntity.SaveChanges();

                        }
                    }
                    //if (inCharge & inMaster) { return "Success"; }
                    //else return "Error";
                    chalanTran.Commit();
                    return "Success";
                }
                catch (Exception ex)
                {
                    chalanTran.Rollback();
                    throw ex;
                }
            }
        }
        private bool IsCustomerExists(SalesChalanExcelData chalan, Dictionary<string, string> grpCode, out Dictionary<string, string> custResponse, NeoErpCoreEntity neoEntity)
        {
            try
            {
                var query = string.Empty;
                var custGroupCode = new Dictionary<string, string>();
                //query = $"SELECT CUSTOMER_CODE AS NormalCode,MASTER_CUSTOMER_CODE AS MasterCode,ACC_CODE as AccountCode FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_EDESC like '%{chalan.CustomerName}%' OR CUSTOMER_NDESC like '%{chalan.CustomerName}%'  AND TEL_MOBILE_NO1='{chalan.CustomerPhoneNo}'";
                query = $"SELECT CUSTOMER_CODE AS NormalCode,MASTER_CUSTOMER_CODE AS MasterCode,ACC_CODE as AccountCode FROM SA_CUSTOMER_SETUP WHERE TEL_MOBILE_NO1='{chalan.CustomerPhoneNo}'";
                var cusCode = _objectEntity.SqlQuery<DbResponse>(query).FirstOrDefault();
                if (cusCode == null)
                {
                    if (grpCode == null) custResponse = CreateCustomerGroup(neoEntity);
                    else custResponse = grpCode;
                    return false;
                }
                else
                {
                    //custResponse = cusCode;
                    var dbRes = new Dictionary<string, string>
                                {
                                    {"CUSTOMER_CODE", cusCode.NormalCode },
                                      { "MASTER_CUSTOMER_CODE", cusCode.MasterCode },
                                      { "CUSTOMER_ACCOUNT", cusCode.AccountCode }

                                };
                    custResponse = dbRes;
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private Dictionary<string, string> CreateCustomerGroup(NeoErpCoreEntity neoEntity)
        {
            Dictionary<string, string> result = null;
            if (System.IO.File.Exists(XmlPath + @"\CUSTOMER.xml"))
            {

                var response = GetSavedGroupInfoFromXml("CUSTOMER");
                return response;
            }
            else
            {

                var maxCustomerCode = string.Empty;
                var maxC_code = string.Empty;
                var newMaxCustomerCodeQuery1 = $@" SELECT nvl(max(to_number(CUSTOMER_CODE))+1,1)  as MAX_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                var newMaxCustomerCode1 = neoEntity.SqlQuery<int>(newMaxCustomerCodeQuery1).FirstOrDefault().ToString();
                var maxMasterCustomerCode = string.Empty;
                var newprequery = $@"SELECT  max(REGEXP_SUBSTR(MASTER_CUSTOMER_CODE, '[^.]+', 1, 1))+1 maxCustomerMasterCode  FROM SA_CUSTOMER_SETUP WHERE   COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                var newMasterAndCustomerCode = neoEntity.SqlQuery<CustomerModels>(newprequery).FirstOrDefault();



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

                //var account = _objectEntity.SqlQuery<int>($@"SELECT MAX(TO_NUMBER(ACC_CODE)) as MAX_ACC_CODE FROM fa_chart_of_accounts_setup").FirstOrDefault();
                var account = GetLastRowOfAccountNo();


                var childsqlquery = $@"INSERT INTO SA_CUSTOMER_SETUP (CUSTOMER_CODE,
                                                    CUSTOMER_EDESC,CUSTOMER_NDESC,PRE_CUSTOMER_CODE,ACC_CODE,
                                                    REMARKS,
                                                    MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG,COMPANY_CODE,
                                                    CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE)
                                                    VALUES('{newMaxCustomerCode1}','Uploaded Customer','Uploaded Customer','00','{account}',
                                                        'Auto Generated Customer While Chalan Upload From Excel','{maxMasterCustomerCode}','G','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),
                                                         'N','{_workContext.CurrentUserinformation.branch_code}')";
                var insertchild = neoEntity.ExecuteSqlCommand(childsqlquery);
                result = new Dictionary<string, string>
            {
                {"CUSTOMER_CODE", newMaxCustomerCode1 },
                {"MASTER_CUSTOMER_CODE", maxMasterCustomerCode },
                { "CUSTOMER_ACCOUNT", account.ToString()},
                { "CUSTOMER_EDESC", account.ToString()},
                { "GROUP_SKU_FLAG", account.ToString()}

            };
                SaveGroupInfoToXml("CUSTOMER", result);
                return result;
            }


        }

        private string AddNewChildCustomer(string customerName, string phoneNo, Dictionary<string, string> grpData, NeoErpCoreEntity neoEntity)
        {
            var maxCustomerCode = string.Empty;
            var maxC_code = string.Empty;
            var newMaxCustomerCodeQuery1 = $@" SELECT nvl(max(to_number(CUSTOMER_CODE))+1,1)  as MAX_CUSTOMER_CODE FROM SA_CUSTOMER_SETUP WHERE COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var newMaxCustomerCode1 = neoEntity.SqlQuery<int>(newMaxCustomerCodeQuery1).FirstOrDefault().ToString();
            var maxMasterCustomerCode = string.Empty;
            var newprequery = $@"SELECT  max(REGEXP_SUBSTR(MASTER_CUSTOMER_CODE, '[^.]+', 1, 1))+1 maxCustomerMasterCode  FROM SA_CUSTOMER_SETUP WHERE   COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
            var newMasterAndCustomerCode = neoEntity.SqlQuery<CustomerModels>(newprequery).FirstOrDefault();

            if (Convert.ToInt32(newMasterAndCustomerCode.maxCustomerCode) <= 9) maxCustomerCode = "0" + newMasterAndCustomerCode.maxCustomerCode;
            else maxCustomerCode = newMasterAndCustomerCode.maxCustomerCode.ToString();

            if (Convert.ToInt32(newMasterAndCustomerCode.maxCustomerMasterCode) <= 9) maxMasterCustomerCode = "0" + newMasterAndCustomerCode.maxCustomerMasterCode;

            else maxMasterCustomerCode = newMasterAndCustomerCode.maxCustomerMasterCode.ToString();

            var childsqlquery = $@"INSERT INTO SA_CUSTOMER_SETUP (CUSTOMER_CODE,
                                                    CUSTOMER_EDESC,CUSTOMER_NDESC,PRE_CUSTOMER_CODE,ACC_CODE,
                                                    REMARKS,
                                                    MASTER_CUSTOMER_CODE,GROUP_SKU_FLAG,COMPANY_CODE,
                                                    CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE)
                                                    VALUES('{newMaxCustomerCode1}','{customerName}','{customerName}','{grpData["MASTER_CUSTOMER_CODE"]}','{grpData["CUSTOMER_ACCOUNT"]}'
                                                        ,'REMARKS','{maxMasterCustomerCode}','I','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),
                                                         'N','{_workContext.CurrentUserinformation.branch_code}')";
            var insertchild = neoEntity.ExecuteSqlCommand(childsqlquery);
            if (insertchild > 0)
            {
                var fa_sub_ledger_map_query = $@"INSERT INTO FA_SUB_LEDGER_MAP(SUB_CODE,ACC_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SYN_ROWID,MODIFY_DATE,BRANCH_CODE,MODIFY_BY) VALUES('C{newMaxCustomerCode1}','{grpData["CUSTOMER_ACCOUNT"]}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N','','','{_workContext.CurrentUserinformation.branch_code}','')";
                var insertedRow = neoEntity.ExecuteSqlCommand(fa_sub_ledger_map_query);
                _logErp.WarnInDB("Mapping customer to fa sub ledger map completed by : " + _workContext.CurrentUserinformation.LOGIN_EDESC);
                //data = "INSERTED";

            }

            return newMaxCustomerCode1;
        }

        private bool IsItemExists(SalesChalanExcelData chalan, string formCode, Dictionary<string, string> itmGrpCod, out Dictionary<string, string> itmRes, NeoErpCoreEntity neoEntity)
        {
            Dictionary<string, string> itmResponse = null;
            //var query = $"SELECT ITEM_CODE AS NormalCode , MASTER_ITEM_CODE AS MasterCode FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE='{chalan.ItemCode}' AND ITEM_EDESC like '%{chalan.ItemName}%'";
            var query = $"SELECT ITEM_CODE AS NormalCode , MASTER_ITEM_CODE AS MasterCode FROM IP_ITEM_MASTER_SETUP WHERE PRODUCT_CODE='{chalan.ItemCode}'";
            var itemCount = _objectEntity.SqlQuery<DbResponse>(query).FirstOrDefault();
            if (itemCount == null)
            {
                if (itmGrpCod == null)
                {
                    // itmRes = CreateCustomerGroup();
                    itmRes = CreateItemStaticGroup(chalan, neoEntity, formCode);
                }
                else
                {
                    itmRes = itmGrpCod;
                }
                return false;
            }
            else
            {
                itmResponse = new Dictionary<string, string>
                {
                    {"ITEM_CODE",itemCount.NormalCode },
                    {"MASTER_ITEM_CODE",itemCount.MasterCode},
                    {"ITEM_EDESC",itemCount.MasterCode},
                    {"GROUP_SKU_FLAG",itemCount.MasterCode}
                };
                itmRes = itmResponse;
                return true;
            }
        }

        private Dictionary<string, string> CreateItemStaticGroup(SalesChalanExcelData chalan, NeoErpCoreEntity neoEntity, string formCode)
        {
            try
            {
                Dictionary<string, string> itmGrpRes = null;
                if (System.IO.File.Exists(XmlPath + @"\ITEM.xml"))
                {
                    itmGrpRes = GetSavedGroupInfoFromXml("ITEM");
                    return itmGrpRes;
                }
                else
                {
                    var newmaxitemcode = string.Empty;
                    //var newmaxGroupitemcodequery = $@" select NVL(MAX(REGEXP_SUBSTR(TO_NUMBER(ITEM_CODE), '[^.]+', 1, 1)),0)+1 as MAX_ITEM_CODE from IP_ITEM_MASTER_SETUP";
                    var newmaxGroupitemcodequery = $@" select NVL(MAX(TO_NUMBER(ITEM_CODE)),0)+1 as MAX_ITEM_CODE from IP_ITEM_MASTER_SETUP";
                    newmaxitemcode = neoEntity.SqlQuery<int>(newmaxGroupitemcodequery).FirstOrDefault().ToString();
                    var newpre = string.Empty;
                    var newmaster = string.Empty;
                    var newprequery = $@"SELECT NVL(max(REGEXP_SUBSTR(MASTER_ITEM_CODE, '[^.]+', 1, 1)),0)+1 col_one FROM ip_item_master_setup";
                    newpre = neoEntity.SqlQuery<int>(newprequery).FirstOrDefault().ToString();
                    if (Convert.ToInt32(newpre) <= 9)
                    {
                        newpre = "0" + newpre.ToString();
                    }

                    newmaster = newpre + ".01";

                    #region Item insert query
                    // insertItem(model, newmaxitemcode, "00", newmaster);
                    var rootsqlquery = $@"INSERT INTO IP_ITEM_MASTER_SETUP
                                      (ITEM_CODE,ITEM_EDESC,ITEM_NDESC,         
                                      MASTER_ITEM_CODE,PRE_ITEM_CODE,GROUP_SKU_FLAG,     
                                      REMARKS,COMPANY_CODE,CREATED_BY,CREATED_DATE,             
                                      DELETED_FLAG)  
                                      VALUES('{newmaxitemcode}','Uploaded Item','Uploaded Item',
                                             '{newmaster}','{newpre}','G',
                                             'Auto Generated Item when chalan upload','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',
                                            SYSDATE,'N')";
                    var insertroot = neoEntity.ExecuteSqlCommand(rootsqlquery);

                    var acount = GetLastRowOfAccountNo();

                    itmGrpRes = new Dictionary<string, string>
                                {
                                    {"ITEM_CODE", newmaxitemcode },
                                    {"MASTER_ITEM_CODE", newmaxitemcode },
                                     {"ITEM_EDESC","Uploaded Item"},
                                        {"GROUP_SKU_FLAG","G"},
                                        {"ACC_CODE",acount},
                                        {"MU_CODE",chalan.Unit},
                                        {"FORM_CODE",formCode}

                                };
                }



                #endregion

                #region Item Integration insert query
                insertIntegration(itmGrpRes, neoEntity);

                #endregion

                #region Item Spec Insert Query
                insertItemSpec(itmGrpRes, neoEntity);

                #endregion

                #region Item Multi Mu insert query
                insertMultiMu(itmGrpRes, neoEntity);
                #endregion

                #region Item Charge insert query 
                insertCharges(itmGrpRes, neoEntity);
                #endregion

                SaveGroupInfoToXml("ITEM", itmGrpRes);
                return itmGrpRes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string insertCharges(Dictionary<string, string> charges, NeoErpCoreEntity neoEntity)
        {
            var SERIAL_NO = 0;
            var COMPANY_CODE = _workContext.CurrentUserinformation.company_code;

            try
            {
                var Itemchargesquery = $@"INSERT INTO IP_ITEM_CHARGE_SETUP(
                                          SERIAL_NO,             
                                          ITEM_CODE,          
                                          FORM_CODE,          
                                          CHARGE_CODE,         
                                          CHARGE_TYPE,        
                                          VALUE_QUANTITY_BASED,
                                          VALUE_PERCENT_FLAG,  
                                          COMPANY_CODE,        
                                          CREATED_BY,          
                                          CREATED_DATE,        
                                          DELETED_FLAG)
                VALUES('{SERIAL_NO}','{charges["ITEM_CODE"]}','{charges["FORM_CODE"]}','{charges["MU_CODE"]}','V','Q','V',
                '{COMPANY_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                var insertcharge = neoEntity.ExecuteSqlCommand(Itemchargesquery);
                return "SUCCESS";
            }
            catch (Exception Ex)
            {
                _logErp.InfoInFile($@" Error in insertCharges  Error {Ex.Message}");
                return "FAILED";
            }
        }

        private string insertMultiMu(Dictionary<string, string> multiMU, NeoErpCoreEntity neoEntity)
        {
            try
            {
                var itemMulMuquery = $@"INSERT INTO IP_ITEM_UNIT_SETUP
                                      (ITEM_CODE, MU_CODE, CONVERSION_FACTOR,REMARKS,COMPANY_CODE,CREATED_BY ,CREATED_DATE,             
                                      DELETED_FLAG)    
                                      VALUES('{multiMU["ITEM_CODE"]}','{multiMU["MU_CODE"]}',1,
                                             'Auto Created When Chalan uploaded','{_workContext.CurrentUserinformation.company_code}',
                                            '{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                var insertMultiMu = neoEntity.ExecuteSqlCommand(itemMulMuquery);
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                _logErp.InfoInFile($@" Error in insertIntegration  Error {ex.Message}");
                return "FAILED";
            }

        }

        private string insertIntegration(Dictionary<string, string> integration, NeoErpCoreEntity neoEntity)
        {
            try
            {
                var itemIntequery = $@"INSERT INTO IP_INTEGRATION_SETUP
                                      (ITEM_CODE, ACC_CODE, FORM_CODE, COMPANY_CODE,CREATED_BY ,CREATED_DATE,             
                                      DELETED_FLAG)    
                                      VALUES('{integration["ITEM_CODE"]}','{integration["ACC_CODE"]}',
                                             '{integration["FORM_CODE"]}','{_workContext.CurrentUserinformation.company_code}',
                                            '{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                var insertinte = neoEntity.ExecuteSqlCommand(itemIntequery);

                return "SUCCESS";
            }
            catch (Exception ex)
            {
                _logErp.InfoInFile($@" Error in insertIntegration Error {ex.Message}");
                return "FAILED";
            }
        }

        private string insertItemSpec(Dictionary<string, string> itmSpec, NeoErpCoreEntity neoEntity)
        {
            try
            {
                var itemSpecquery = $@"INSERT INTO IP_ITEM_SPEC_SETUP
                                      (ITEM_CODE,PART_NUMBER,           
                                      COLOR,GRADE,REMARKS,COMPANY_CODE,CREATED_BY ,CREATED_DATE,             
                                      DELETED_FLAG)    
                                      VALUES('{itmSpec["ITEM_CODE"]}','0',
                                             'Auto Created While Chalan Upload','{_workContext.CurrentUserinformation.company_code}',
                                            '{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                var insertispec = neoEntity.ExecuteSqlCommand(itemSpecquery);
                return "SUCCESS";

            }
            catch (Exception ex)
            {
                _logErp.InfoInFile($@" Error in insertItemSpec  Error {ex.Message}");
                return "FAILED";
            }

        }
        private string AddNewItem(SalesChalanExcelData chalan, Dictionary<string, string> itmGrpData, NeoErpCoreEntity neoEntity)
        {
            var newmaxitemcode = string.Empty;
            //var newmaxGroupitemcodequery = $@"select NVL(MAX(REGEXP_SUBSTR(TO_NUMBER(ITEM_CODE), '[^.]+', 1, 1)),0)+1 as MAX_ITEM_CODE from IP_ITEM_MASTER_SETUP";
            var newmaxGroupitemcodequery = $@"select NVL(MAX(TO_NUMBER(ITEM_CODE)),0)+1 as MAX_ITEM_CODE from IP_ITEM_MASTER_SETUP";
            newmaxitemcode = neoEntity.SqlQuery<int>(newmaxGroupitemcodequery).FirstOrDefault().ToString();
            var newpre = string.Empty;
            var newmaster = string.Empty;
            var newprequery = $@"SELECT NVL(max(REGEXP_SUBSTR(MASTER_ITEM_CODE, '[^.]+', 1, 1)),0)+1 col_one FROM ip_item_master_setup";
            newpre = neoEntity.SqlQuery<int>(newprequery).FirstOrDefault().ToString();
            if (Convert.ToInt32(newpre) <= 9)
            {
                newpre = "0" + newpre.ToString();
            }
            //if (model.GROUP_SKU_FLAG == "G")
            //{
            //    newmaster = newpre + ".01";
            //}
            //else
            //{
            //    
            //}
            newmaster = newpre + ".00";
            #region Item insert query
            var model = new ItemSetupModel();
            //insertItem(itemSetupModal, newmaxitemcode, "00", newmaster);
            var rootsqlquery = $@"INSERT INTO IP_ITEM_MASTER_SETUP
                                      (ITEM_CODE,ITEM_EDESC ,ITEM_NDESC,         
                                      MASTER_ITEM_CODE,PRE_ITEM_CODE,GROUP_SKU_FLAG,     
                                      REMARKS ,COMPANY_CODE,CREATED_BY,CREATED_DATE,             
                                      DELETED_FLAG)  
                                      VALUES('{newmaxitemcode}','{chalan.ItemName}','{chalan.ItemName}',
                                             '{newmaster}','{itmGrpData["MASTER_ITEM_CODE"]}','I',
                                             'Auto Created while chalan upload','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',
                                            SYSDATE,'N')";
            var insertroot = neoEntity.ExecuteSqlCommand(rootsqlquery);
            // return "SUCCESS";
            var acount = GetLastRowOfAccountNo();
            #endregion

            #region Item Integration insert query
            insertIntegration(itmGrpData, neoEntity);

            #endregion

            #region Item Spec Insert Query
            insertItemSpec(itmGrpData, neoEntity);

            #endregion

            #region Item Multi Mu insert query
            insertMultiMu(itmGrpData, neoEntity);
            #endregion

            #region Item Charge insert query 
            insertCharges(itmGrpData, neoEntity);
            #endregion

            return newmaxitemcode;
        }

        private bool IsDealerExists(string dealerName, Dictionary<string, string> dlrGrpCod, out Dictionary<string, string> delRes, NeoErpCoreEntity neoEntity)
        {
            //var query = $"SELECT PARTY_TYPE_CODE AS NormalCode,MASTER_PARTY_CODE AS MasterCode FROM IP_PARTY_TYPE_CODE WHERE PARTY_TYPE_EDESC like '%{dealerName}%'";
            //var query = $"SELECT PARTY_TYPE_CODE AS NormalCode,MASTER_PARTY_CODE AS MasterCode FROM IP_PARTY_TYPE_CODE WHERE REMARKS = '{dealerName}' AND PARTY_TYPE_FLAG = 'D' ";
            var query = $"SELECT PARTY_TYPE_CODE AS NormalCode,MASTER_PARTY_CODE AS MasterCode FROM IP_PARTY_TYPE_CODE WHERE PARTY_TYPE_EDESC = '{dealerName}' AND PARTY_TYPE_FLAG = 'D' ";
            var dealerCount = _objectEntity.SqlQuery<DbResponse>(query).FirstOrDefault();

            if (dealerCount == null)
            {
                if (dlrGrpCod == null) delRes = CreateDealerGroup(neoEntity);
                else delRes = dlrGrpCod;
                return false;
            }
            else
            {
                delRes = new Dictionary<string, string>
                {
                     {"PARTY_TYPE_CODE",dealerCount.NormalCode },
                    {"MASTER_PARTY_CODE",dealerCount.MasterCode },
                };
                return true;
            }
        }

        private string GetLastRowOfAccountNo()
        {
            try
            {
                var account = _objectEntity.SqlQuery<int>($@"SELECT MAX(TO_NUMBER(ACC_CODE)) as MAX_ACC_CODE FROM fa_chart_of_accounts_setup").FirstOrDefault();
                return account.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string AddNewDealer(string dealerName, string dealerCode, Dictionary<string, string> drlGrpData, NeoErpCoreEntity neoEntity)
        {
            try
            {
                var newmaxitemcode = string.Empty;
                var childDealerCode = string.Empty;
                var newmaxitemcodequery = $@"select NVL(MAX(TO_NUMBER(REGEXP_SUBSTR(TO_NUMBER(PARTY_TYPE_CODE), '[^.]+', 1, 1))),0)+1 as MASTER_DEALER_CODE from IP_PARTY_TYPE_CODE  WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                newmaxitemcode = neoEntity.SqlQuery<int>(newmaxitemcodequery).FirstOrDefault().ToString();
                var newpre = string.Empty;
                var newmaster = string.Empty;
                var newprequery = $@"SELECT  MAX(substr(MASTER_PARTY_CODE,-instr(reverse(MASTER_PARTY_CODE),'.')+1))+1 col_one FROM IP_PARTY_TYPE_CODE WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";


                newpre = neoEntity.SqlQuery<int>(newprequery).FirstOrDefault().ToString();
                if (Convert.ToInt32(newpre) <= 9) newpre = "0" + newpre.ToString();

                var rootchildsqlquery = $@"INSERT INTO IP_PARTY_TYPE_CODE (PARTY_TYPE_CODE,PARTY_TYPE_EDESC,PARTY_TYPE_NDESC,GROUP_SKU_FLAG,REMARKS,COMPANY_CODE,CREATED_BY,
                                                 CREATED_DATE,DELETED_FLAG,SYN_ROWID,PRE_PARTY_CODE,MASTER_PARTY_CODE) 
                                                 VALUES('{newmaxitemcode}','{dealerName}','{dealerName}','I','Dealear Imported From Excel',
                                                 '{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',
                                                  TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N','N','{drlGrpData["MASTER_PARTY_TYPE"]}','{newpre}')";
                var insertrootchild = neoEntity.ExecuteSqlCommand(rootchildsqlquery);
                if (insertrootchild > 0) return newmaxitemcode.ToString();
                else return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private Dictionary<string, string> CreateDealerGroup(NeoErpCoreEntity neoEntity)
        {
            try
            {
                if (System.IO.File.Exists(XmlPath))
                {
                    var response = GetSavedGroupInfoFromXml("PARTY_TYPE");
                    return response;
                }
                else
                {
                    var newmaxitemcode = string.Empty;

                    var newmaxitemcodequery = $@"select NVL(MAX(TO_NUMBER(REGEXP_SUBSTR(TO_NUMBER(PARTY_TYPE_CODE), '[^.]+', 1, 1))),0)+1 as MASTER_DEALER_CODE from IP_PARTY_TYPE_CODE  WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                    newmaxitemcode = neoEntity.SqlQuery<int>(newmaxitemcodequery).FirstOrDefault().ToString();
                    var newpre = string.Empty;
                    var newmaster = string.Empty;
                    var newprequery = $@"SELECT  MAX(substr(MASTER_PARTY_CODE,-instr(reverse(MASTER_PARTY_CODE),'.')+1))+1 col_one FROM IP_PARTY_TYPE_CODE WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";


                    newpre = neoEntity.SqlQuery<int?>(newprequery).FirstOrDefault().ToString();
                    if (newpre == null || string.IsNullOrEmpty(newpre) || newpre == "") newpre = "00";
                    else if (Convert.ToInt32(newpre) <= 9) newpre = "0" + newpre.ToString();

                    newmaster = newpre + ".01";
                    var rootchildsqlquery = $@"INSERT INTO IP_PARTY_TYPE_CODE (PARTY_TYPE_CODE,PARTY_TYPE_EDESC,PARTY_TYPE_NDESC,GROUP_SKU_FLAG,REMARKS,COMPANY_CODE,CREATED_BY,
                                                 CREATED_DATE,DELETED_FLAG,
                                                 PRE_PARTY_CODE,MASTER_PARTY_CODE) 
                                                 VALUES('{newmaxitemcode}','Uploaded Dealer','Uploaded Dealer','G','Auto Created While Uploading chalan excel',
                                                 '{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',
                                                 SYSDATE,'N','00','{newpre}')";
                    var insertrootchild = neoEntity.ExecuteSqlCommand(rootchildsqlquery);
                    var response = new Dictionary<string, string>
                                    {
                                        {"PARTY_TYPE_CODE",newmaxitemcode },
                                        {"MASTER_PARTY_TYPE",newpre },
                                        {"PARTY_TYPE_EDESC",newpre },
                                        {"GROUP_SKU_FLAG",newpre },
                                    };
                    SaveGroupInfoToXml("PARTY_TYPE", response);
                    return response;
                }



            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string CreateStaticLocation()
        {
            try
            {
                var newpre = string.Empty;
                var newmaster = string.Empty;
                var newprequery = $@"SELECT  max(REGEXP_SUBSTR(LOCATION_CODE, '[^.]+', 1, 1))+1 col_one FROM IP_LOCATION_SETUP";
                newpre = _objectEntity.SqlQuery<int>(newprequery).FirstOrDefault().ToString();
                if (Convert.ToInt32(newpre) <= 9)
                {
                    newpre = "0" + newpre.ToString();
                }
                newmaster = newpre + ".00";
                var rootquery = $@"INSERT INTO IP_LOCATION_SETUP (LOCATION_CODE,
                                                    LOCATION_EDESC,LOCATION_NDESC,PRE_LOCATION_CODE,
                                                    GROUP_SKU_FLAG,REMARKS,COMPANY_CODE,
                                                    CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE)
                                                    VALUES('{newmaster}','Uploaded Location','Uploaded Location','00',
                                                        'G','REMARKS','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),
                                                         'N','{_workContext.CurrentUserinformation.branch_code}')";
                var insertroot = _objectEntity.ExecuteSqlCommand(rootquery);
                return newmaster;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private bool SaveSalesChalanValues(SalesChalanExcelData chalanaData, CommonPropForChalan commonProp, NeoErpCoreEntity neoEntity)
        {
            try
            {
                ++serialNo;
                var chalanDate = chalanaData.OrderDate.ToShortDateString();
                bool insertedToChild = false;
                Regex regex = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                var shippingAddress = regex.Replace(chalanaData.Address, string.Empty);
                var querySalesChalan = $@"INSERT INTO SA_SALES_CHALAN
                                        (CHALAN_NO, CHALAN_DATE, CUSTOMER_CODE, FROM_LOCATION_CODE, 
                                            SERIAL_NO, ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                            DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SESSION_ROWID, PARTY_TYPE_CODE,MANUAL_NO,REMARKS,PRIORITY_CODE ,SALES_TYPE_CODE,SHIPPING_ADDRESS,SHIPPING_CONTACT_NO )
                                         VALUES
                                       ('{commonProp.New_Voucher_No}', TO_DATE('{chalanDate}', 'MM/DD/yyyy'), '{commonProp.Customer_Code}', '01.01.02', 
                                        {serialNo}, '{commonProp.Item_Code}', '{chalanaData.Unit}', {chalanaData.Quantity}, {chalanaData.Rate}, 
                                            {chalanaData.Total},{chalanaData.Quantity},{chalanaData.Rate}, {chalanaData.Total}, 
    '{commonProp.Form_Code}', '{_workContext.CurrentUserinformation.company_code}', '{_workContext.CurrentUserinformation.branch_code}', '{_workContext.CurrentUserinformation.login_code}', 
    SYSDATE, 'N', 'NRS', 1,'{chalanaData.ManualNo}', '{commonProp.Dealer_Code}','{chalanaData.ManualNo}','Chalan uploaded from excel','BT','01','{shippingAddress}','{chalanaData.CustomerPhoneNo}')";
                var InsertedData = neoEntity.ExecuteSqlCommand(querySalesChalan);
                if (InsertedData > 0) insertedToChild = true;
                return insertedToChild;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        private bool SaveBulkSalesChalanValues(List<SalesChalanDetail> childValue, SalesChalanDetail masterValue)
        {
            try
            {
                int serialNo = 1;
                bool insertedToChild = false;
                SalesChalanDetail salesChildChalan = new SalesChalanDetail();
                foreach (var cCol in childValue)
                {

                    salesChildChalan.CHALAN_DATE = string.IsNullOrEmpty(cCol.CHALAN_DATE) ? masterValue.CHALAN_DATE.ToString() : cCol.CHALAN_DATE;
                    salesChildChalan.CUSTOMER_CODE = string.IsNullOrEmpty(cCol.CUSTOMER_CODE) ? masterValue.CUSTOMER_CODE : cCol.CUSTOMER_CODE;
                    salesChildChalan.SERIAL_NO = cCol.SERIAL_NO > 0 ? cCol.SERIAL_NO : serialNo;
                    salesChildChalan.ITEM_CODE = string.IsNullOrEmpty(cCol.ITEM_CODE) ? "" : cCol.ITEM_CODE;
                    salesChildChalan.MU_CODE = string.IsNullOrEmpty(cCol.MU_CODE) ? "" : cCol.MU_CODE;
                    salesChildChalan.QUANTITY = cCol.QUANTITY > 0 ? cCol.QUANTITY : cCol.CALC_QUANTITY > 0 ? cCol.CALC_QUANTITY : 0;
                    salesChildChalan.UNIT_PRICE = cCol.UNIT_PRICE > 0 ? cCol.UNIT_PRICE : cCol.CALC_UNIT_PRICE > 0 ? cCol.CALC_UNIT_PRICE : 0;
                    salesChildChalan.TOTAL_PRICE = cCol.TOTAL_PRICE;
                    salesChildChalan.CALC_QUANTITY = cCol.CALC_QUANTITY > 0 ? cCol.CALC_QUANTITY : cCol.QUANTITY > 0 ? cCol.QUANTITY : 1;
                    salesChildChalan.CALC_UNIT_PRICE = cCol.CALC_UNIT_PRICE > 0 ? cCol.CALC_UNIT_PRICE : cCol.UNIT_PRICE > 0 ? cCol.UNIT_PRICE : 0;
                    salesChildChalan.CALC_TOTAL_PRICE = cCol.CALC_TOTAL_PRICE > 0 ? cCol.CALC_TOTAL_PRICE : cCol.TOTAL_PRICE > 0 ? cCol.TOTAL_PRICE : 0;
                    salesChildChalan.REMARKS = string.IsNullOrEmpty(cCol.REMARKS) ? masterValue.REMARKS : cCol.REMARKS;

                    salesChildChalan.COMPANY_CODE = _workContext.CurrentUserinformation.company_code;
                    salesChildChalan.BRANCH_CODE = _workContext.CurrentUserinformation.branch_code;
                    salesChildChalan.CREATED_BY = _workContext.CurrentUserinformation.login_code;
                    salesChildChalan.CREATED_DATE = DateTime.Now;
                    salesChildChalan.DELETED_FLAG = "N";
                    salesChildChalan.DELIVERY_DATE = string.IsNullOrEmpty(cCol.DELIVERY_DATE) ? masterValue.DELIVERY_DATE : cCol.DELIVERY_DATE;

                    salesChildChalan.DELIVERY_DATE = string.IsNullOrEmpty(cCol.DELIVERY_DATE) ? masterValue.DELIVERY_DATE : cCol.DELIVERY_DATE;
                    salesChildChalan.CURRENCY_CODE = string.IsNullOrEmpty(cCol.CURRENCY_CODE) ? "NRS" : cCol.CURRENCY_CODE;
                    salesChildChalan.EXCHANGE_RATE = cCol.EXCHANGE_RATE > 0 ? cCol.QUANTITY : 1;
                    salesChildChalan.TRACKING_NO = cCol.TRACKING_NO;
                    salesChildChalan.STOCK_BLOCK_FLAG = cCol.STOCK_BLOCK_FLAG;
                    salesChildChalan.SESSION_ROWID = cCol.SESSION_ROWID;
                    salesChildChalan.MODIFY_BY = cCol.MODIFY_BY;
                    salesChildChalan.DIVISION_CODE = cCol.DIVISION_CODE;
                    salesChildChalan.PARTY_TYPE_CODE = string.IsNullOrEmpty(cCol.PARTY_TYPE_CODE) ? masterValue.PARTY_TYPE_CODE : cCol.PARTY_TYPE_CODE;
                    salesChildChalan.SALES_TYPE_CODE = string.IsNullOrEmpty(cCol.SALES_TYPE_CODE) ? masterValue.SALES_TYPE_CODE : cCol.SALES_TYPE_CODE;
                    salesChildChalan.MODIFY_DATE = DateTime.Now;
                    salesChildChalan.EMPLOYEE_CODE = string.IsNullOrEmpty(cCol.EMPLOYEE_CODE) ? masterValue.EMPLOYEE_CODE : cCol.EMPLOYEE_CODE;
                    salesChildChalan.MANUAL_NO = masterValue.MANUAL_NO;

                    if (masterValue.SHIPPING_ADDRESS != null)
                    {
                        salesChildChalan.SHIPPING_ADDRESS = masterValue.SHIPPING_ADDRESS.Contains("'") ? masterValue.SHIPPING_ADDRESS.Replace("'", "' || '''' || '") : masterValue.SHIPPING_ADDRESS;
                    }
                    else
                    {
                        salesChildChalan.SHIPPING_ADDRESS = "";
                    }
                    salesChildChalan.SHIPPING_CONTACT_NO = masterValue.SHIPPING_CONTACT_NO;
                    salesChildChalan.AREA_CODE = masterValue.AREA_CODE;
                    salesChildChalan.PRIORITY_CODE = string.IsNullOrEmpty(cCol.PRIORITY_CODE) ? masterValue.PRIORITY_CODE : cCol.PRIORITY_CODE;
                    salesChildChalan.LINE_ITEM_DISCOUNT = masterValue.LINE_ITEM_DISCOUNT;
                    salesChildChalan.FROM_LOCATION_CODE = string.IsNullOrEmpty(cCol.FROM_LOCATION_CODE) ? masterValue.FROM_LOCATION_CODE : cCol.FROM_LOCATION_CODE;
                    salesChildChalan.SECTOR_CODE = string.IsNullOrEmpty(cCol.SECTOR_CODE) ? masterValue.SECTOR_CODE : cCol.SECTOR_CODE;
                    salesChildChalan.ALT1_MU_CODE = cCol.ALT1_MU_CODE;
                    salesChildChalan.ALT1_QUANTITY = cCol.ALT1_QUANTITY;


                    var querySalesChalan = $@"Insert into SA_SALES_CHALAN
                                        (CHALAN_NO, CHALAN_DATE, CUSTOMER_CODE, FROM_LOCATION_CODE, 
                                            SERIAL_NO, ITEM_CODE, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                            DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, TRACKING_NO, SESSION_ROWID, MODIFY_BY, PARTY_TYPE_CODE, PRIORITY_CODE, 
                                            SHIPPING_ADDRESS, SHIPPING_CONTACT_NO, SALES_TYPE_CODE, EMPLOYEE_CODE, AREA_CODE,MANUAL_NO,REMARKS,AGENT_CODE,DIVISION_CODE,SECTOR_CODE,ALT1_MU_CODE,ALT1_QUANTITY,SECOND_QUANTITY)
                                         Values
                                       ('{salesChildChalan.CHALAN_NO}', TO_DATE('{salesChildChalan.CHALAN_DATE}', 'DD-MON-YYYY hh24:mi:ss'), '{salesChildChalan.CUSTOMER_CODE}', '{salesChildChalan.FROM_LOCATION_CODE}', 
                                        {salesChildChalan.SERIAL_NO}, '{salesChildChalan.ITEM_CODE}', '{salesChildChalan.MU_CODE}', {salesChildChalan.QUANTITY}, {salesChildChalan.UNIT_PRICE}, 
                                            {salesChildChalan.TOTAL_PRICE}, {salesChildChalan.CALC_QUANTITY}, {salesChildChalan.CALC_UNIT_PRICE}, {salesChildChalan.CALC_TOTAL_PRICE}, 
    '{salesChildChalan.FORM_CODE}', '{salesChildChalan.COMPANY_CODE}', '{salesChildChalan.BRANCH_CODE}', '{salesChildChalan.CREATED_BY}', 
    sysdate, 'N', '{salesChildChalan.CURRENCY_CODE}', {salesChildChalan.EXCHANGE_RATE}, 
    '{salesChildChalan.TRACKING_NO}', '{salesChildChalan.SYN_ROWID}', '', '{salesChildChalan.PARTY_TYPE_CODE}', '{salesChildChalan.PRIORITY_CODE}', 
                        '{salesChildChalan.SHIPPING_ADDRESS}', '{salesChildChalan.SHIPPING_CONTACT_NO}', '{salesChildChalan.SALES_TYPE_CODE}', '{salesChildChalan.EMPLOYEE_CODE}', '{salesChildChalan.AREA_CODE}','{salesChildChalan.MANUAL_NO}','{salesChildChalan.REMARKS}','{salesChildChalan.AGENT_CODE}','{salesChildChalan.DIVISION_CODE}','{salesChildChalan.SECTOR_CODE}','{salesChildChalan.ALT1_MU_CODE}','{salesChildChalan.ALT1_QUANTITY}','{cCol.SECOND_QUANTITY}')";
                    var InsertedData = _objectEntity.ExecuteSqlCommand(querySalesChalan);
                    serialNo++;
                }
                insertedToChild = true;
                return insertedToChild;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        private List<SalesOrderDetail> GetMasterTransactionByOrderNo(string orderNumber, NeoErpCoreEntity neoEntity)
        {
            try
            {
                var getPrevDataQuery = $@"SELECT VOUCHER_NO,SESSION_ROWID, CREATED_BY, CREATED_DATE FROM MASTER_TRANSACTION WHERE VOUCHER_NO= '{orderNumber}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
                var defaultData = neoEntity.SqlQuery<SalesOrderDetail>(getPrevDataQuery).ToList();
                return defaultData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private bool SaveMasterTransactionValue(SalesChalanExcelData masterData, CommonPropForChalan commonProp, NeoErpCoreEntity neoEntity)
        {
            try
            {
                bool insertedToMaster = false;

                var defaultData = GetMasterTransactionByOrderNo(commonProp.New_Voucher_No, neoEntity);
                string defaultCol = "MODIFY_BY,MODIFY_DATE", createdDateForEdit = "", createdByForEdit = "", voucherNoForEdit = "";
                if (defaultData.Count() > 0)
                {
                    foreach (var defData in defaultData)
                    {
                        voucherNoForEdit = defData.VOUCHER_NO.ToString();
                        createdByForEdit = "TO_DATE('" + defData.CREATED_DATE.ToString() + "','MM-DD-YYYY hh12:mi:ss pm')";
                        createdByForEdit = defData.CREATED_BY.ToString().ToUpper();
                    }

                    string query = $@"UPDATE MASTER_TRANSACTION SET VOUCHER_AMOUNT='{masterData.Total}',VOUCHER_DATE = {masterData.OrderDate}, MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}',REFERENCE_NO='{masterData.ManualNo}' , MODIFY_DATE = SYSDATE,CURRENCY_CODE='NRS',EXCHANGE_RATE=1 where VOUCHER_NO='{commonProp.New_Voucher_No}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                    var rowCount = neoEntity.ExecuteSqlCommand(query);
                    if (rowCount > 0) insertedToMaster = true;
                }
                else
                {
                    string insertmasterQuery = $@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,
                                                              COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,SYN_ROWID,EXCHANGE_RATE,
                                                              REFERENCE_NO,PRINT_COUNT) 
                                                  VALUES('{commonProp.New_Voucher_No}','{masterData.Total}',
                               '{commonProp.Form_Code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}',
                               '{_workContext.CurrentUserinformation.login_code}','N','NRS',SYSDATE,
                               TO_DATE('{masterData.OrderDate.ToShortDateString()}','MM/dd/yyyy'),'{masterData.ManualNo}',
                               1,'{masterData.ManualNo}',1)";

                    var rowCount = neoEntity.ExecuteSqlCommand(insertmasterQuery);
                    if (rowCount > 0) insertedToMaster = true;
                }
                return insertedToMaster;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving master transaction : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        private bool SaveChargeColumnValue(SalesChalanExcelData chargeData, CommonPropForChalan commonProp, NeoErpCoreEntity neoEntity)
        {
            try
            {
                bool insertedToCharges = false;
                string transquery = string.Format(@"select TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO )+1),1))) ORDER_NO from CHARGE_TRANSACTION");

                int newtransno = neoEntity.SqlQuery<int>(transquery).FirstOrDefault();
                string insertChargeQuery = $@"INSERT INTO CHARGE_TRANSACTION(TRANSACTION_NO,TABLE_NAME,REFERENCE_NO,CHARGE_CODE,CHARGE_TYPE_FLAG,
                                                                        CHARGE_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,SESSION_ROWID) 
                                                VALUES('{newtransno}','{commonProp.Table_Name}','{commonProp.New_Voucher_No}','VT','A',{chargeData.Total},
                                                '{commonProp.Form_Code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N','NRS',1,{chargeData.ManualNo})";
                var rowCount = neoEntity.ExecuteSqlCommand(insertChargeQuery);
                if (rowCount > 0) insertedToCharges = true;


                _logErp.WarnInDB("Charges for sales order saved successfully by : " + _workContext.CurrentUserinformation.login_code);
                return insertedToCharges;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private string NewVoucherNo(string companycode, string formcode, string transactiondate, string tablename)
        {
            try
            {
                if (companycode != "" && formcode != "" && transactiondate != "" && tablename != "")
                {
                    string query = $@"SELECT FN_NEW_VOUCHER_NO('{companycode}','{formcode}',SYSDATE,'{tablename}') FROM DUAL";
                    string voucherNo = _objectEntity.SqlQuery<string>(query).FirstOrDefault();
                    return voucherNo;
                }
                else
                { return ""; }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string SaveGroupInfoToXml(string type, Dictionary<string, string> data)
        {
            try
            {
                if (type == "CUSTOMER")
                {
                    XmlTextWriter writer = new XmlTextWriter(XmlPath + @"\" + type + ".xml", System.Text.Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("NewCustomer");
                    createNode(data, writer, "CUSTOMER");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                }
                else if (type == "ITEM")
                {
                    XmlTextWriter writer = new XmlTextWriter(XmlPath + @"\" + type + ".xml", System.Text.Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("NEW_GROUP_ITEM");
                    createNode(data, writer, "ITEM");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();

                }
                else if (type == "PARTY_TYPE")
                {
                    XmlTextWriter writer = new XmlTextWriter(XmlPath + @"\Dealer.xml", System.Text.Encoding.UTF8);
                    //XmlTextWriter writer = new XmlTextWriter(XmlPath + @"\" + type + ".xml", System.Text.Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("NewDealer");
                    createNode(data, writer, "PARTY_TYPE");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                }
                else
                {

                }

                return "";

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private Dictionary<string, string> GetSavedGroupInfoFromXml(string type)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                if (type == "CUSTOMER")
                {
                    using (XmlReader reader = XmlReader.Create(XmlPath + @"\CUSTOMER.xml"))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {

                                switch (reader.Name.ToString())
                                {
                                    case "CUSTOMER_CODE":

                                        result.Add("CUSTOMER_CODE", reader.ReadString());
                                        break;
                                    case "MASTER_CUSTOMER_CODE":

                                        result.Add("MASTER_CUSTOMER_CODE", reader.ReadString());
                                        break;
                                    case "CUSTOMER_ACCOUNT":
                                        result.Add("CUSTOMER_ACCOUNT", reader.ReadString());
                                        break;
                                }
                            }

                        }
                    }

                    return result;
                }
                else if (type == "ITEM")
                {
                    using (XmlReader reader = XmlReader.Create(XmlPath + @"\ITEM.xml"))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {

                                switch (reader.Name.ToString())
                                {
                                    case "ITEM_CODE":

                                        result.Add("ITEM_CODE", reader.ReadString());
                                        break;
                                    case "MASTER_ITEM_CODE":

                                        result.Add("MASTER_ITEM_CODE", reader.ReadString());
                                        break;
                                    case "ACC_CODE":
                                        result.Add("ACC_CODE", reader.ReadString());
                                        break;
                                }
                            }

                        }
                    }

                    return result;
                }
                else if (type == "PARTY_TYPE")
                {
                    using (XmlReader reader = XmlReader.Create(XmlPath + @"\ITEM.xml"))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {

                                switch (reader.Name.ToString())
                                {
                                    case "PARTY_TYPE_CODE":

                                        result.Add("PARTY_TYPE_CODE", reader.ReadString());
                                        break;
                                    case "MASTER_PARTY_TYPE":

                                        result.Add("MASTER_PARTY_TYPE", reader.ReadString());
                                        break;
                                }
                            }

                        }
                    }

                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //createNode(NormalCode,MasterCode,EDESC,NDESC,GROUP_SKU_FLAG
        private void createNode(Dictionary<string, string> data, XmlTextWriter writer, string type)
        {
            if (type == "CUSTOMER")
            {
                writer.WriteStartElement(type + "_CODE");
                writer.WriteString(data["CUSTOMER_CODE"]);
                writer.WriteEndElement();
                writer.WriteStartElement("MASTER_" + type + "_CODE");
                writer.WriteString(data["MASTER_CUSTOMER_CODE"]);
                writer.WriteEndElement();
                writer.WriteStartElement("CUSTOMER_EDESC");
                writer.WriteString("Uploaded Customer");
                writer.WriteEndElement();
                writer.WriteStartElement("GROUP_SKU_FLAG");
                writer.WriteString("G");
                writer.WriteEndElement();
                writer.WriteStartElement("CUSTOMER_ACCOUNT");
                writer.WriteString(data["CUSTOMER_ACCOUNT"]);
                writer.WriteEndElement();

            }
            else if (type == "ITEM")
            {
                writer.WriteStartElement(type + "_CODE");
                writer.WriteString(data["ITEM_CODE"]);
                writer.WriteEndElement();
                writer.WriteStartElement("MASTER_" + type + "_CODE");
                writer.WriteString(data["MASTER_ITEM_CODE"]);
                writer.WriteEndElement();
                writer.WriteStartElement("ITEM_EDESC");
                writer.WriteString(data["ITEM_EDESC"]);
                writer.WriteEndElement();
                writer.WriteStartElement("GROUP_SKU_FLAG");
                writer.WriteString("G");
                writer.WriteEndElement();
                writer.WriteStartElement("ACC_CODE");
                writer.WriteString(data["ACC_CODE"]);
                writer.WriteEndElement();

            }
            else if (type == "PARTY_TYPE")
            {
                writer.WriteStartElement(type + "_CODE");
                writer.WriteString(data["PARTY_TYPE_CODE"]);
                writer.WriteEndElement();
                writer.WriteStartElement("MASTER_" + type);
                writer.WriteString(data["MASTER_PARTY_TYPE"]);
                writer.WriteEndElement();
                writer.WriteStartElement("PARTY_TYPE_EDESC");
                writer.WriteString("Uploaded Dealer");
                writer.WriteEndElement();
                writer.WriteStartElement("GROUP_SKU_FLAG");
                writer.WriteString("G");
                writer.WriteEndElement();
            }
            else
            {

            }
        }
        public string AddNewCustomerDealerAccount(CustomerDealerAccountModel custDealerAccount, SalesChalanExcelData salesChalan)
        {
            var masterCustomerCode = custDealerAccount.MasterCustomerCode;
            var preCustomerCode = custDealerAccount.PreCustomerCode;
            var maxCustomerCode = string.Empty;
            Regex regex = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            var customerName = regex.Replace(salesChalan.CustomerName, string.Empty);
            var customerAddress = regex.Replace(salesChalan.Address, string.Empty);
            //var companyCode = custDealerAccount.CompanyCode;
            //var branchCode = custDealerAccount.BranchCode;
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var branchCode = _workContext.CurrentUserinformation.branch_code;
            string CustomerCodeQuery = $@"select TO_CHAR(NVL(MAX(TO_NUMBER(C.CUSTOMER_CODE )+1),1)) AS C  from sa_customer_setup c";
            maxCustomerCode = this._dbContext.SqlQuery<string>(CustomerCodeQuery).FirstOrDefault();
            var subCode = "C" + maxCustomerCode;
            string InsertSaCustomerSetupQuery = $@"INSERT INTO SA_CUSTOMER_SETUP(CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,TEL_MOBILE_NO1,GROUP_SKU_FLAG,MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE,PARTY_TYPE_CODE,APPROVED_FLAG,CUSTOMER_ID,REGD_OFFICE_EADDRESS,LINK_SUB_CODE,ACC_CODE,TPIN_VAT_NO,REMARKS)
                            VALUES('{maxCustomerCode}','{customerName}','{customerName}','{salesChalan.CustomerPhoneNo}','I','{custDealerAccount.MasterCustomerCode}','{custDealerAccount.PreCustomerCode}','{companyCode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N','{branchCode}','{custDealerAccount.CustomerPartyTypeCode}','Y','{custDealerAccount.CommonCode}','{customerAddress}','{subCode}','{custDealerAccount.AccountCode}','{salesChalan.VAT}','Customer uploaded from chalan excel')";
            var newCustomerCount = _dbContext.ExecuteSqlCommand(InsertSaCustomerSetupQuery);
            if (newCustomerCount > 0)
            {
                var subLedgerMapQuery = $@"INSERT INTO FA_SUB_LEDGER_MAP(SUB_CODE,ACC_CODE,COMPANY_CODE,BRANCH_CODE,DELETED_FLAG,CREATED_BY,CREATED_DATE)             
            VALUES('{subCode}', '{custDealerAccount.AccountCode}', '{companyCode}', '{branchCode}','N','ADMIN', TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/dd/yyyy'))";
                var subLedgerMapInserted = this._dbContext.ExecuteSqlCommand(subLedgerMapQuery);

                var subLedgerDealerMapQuery = $@"INSERT INTO FA_SUB_LEDGER_DEALER_MAP(PARTY_TYPE_CODE,SUB_CODE,CUSTOMER_CODE,COMPANY_CODE,BRANCH_CODE,DELETED_FLAG,CREATED_BY,CREATED_DATE)             
            VALUES('{custDealerAccount.DealerPartyTypeCode}', '{subCode}','{maxCustomerCode}', '{companyCode}', '{branchCode}','N','ADMIN', TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/dd/yyyy'))";
                var subLedgerMapDealerInserted = this._dbContext.ExecuteSqlCommand(subLedgerDealerMapQuery);
            }
            return maxCustomerCode;

        }
        #endregion
    }
    public class CustomerDealerAccountModel
    {
        public string CommonCode { get; set; }
        public string MasterCustomerCode { get; set; }
        public string PreCustomerCode { get; set; }
        public string AccountCode { get; set; }
        public string DealerPartyTypeCode { get; set; }
        public string CustomerPartyTypeCode { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
    }

    public class IssueScreenDtlModule
    {
        public string FORM_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
    }
}
