using NeoERP.DocumentTemplate.Service.Models;
using NeoERP.DocumentTemplate.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using Microsoft.Owin;
using NeoErp.Core;
using NeoErp.Core.Caching;
using System.Text;
using Newtonsoft.Json;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
//using NeoERP.Data;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class ContraVoucherApiController : ApiController
    {

        private ITestTemplateRepo _TestTemplateRepo;
        private IFormTemplateRepo _FormTemplateRepo;
        //subin change
        private IFormSetupRepo _FormSetupRepo;
        IContraVoucher _contravoucher;
        private ISalesOrderRepo _SalesOrderRepo;
        ///private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private IContraVoucher _Reference;
        private NeoErpCoreEntity _objectEntity;
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        private IFinancialVoucherSaveService _financialVoucherSaveService;
        public ContraVoucherApiController(ITestTemplateRepo TestTemplateRepo, IFormTemplateRepo FormTemplateRepo, IFormSetupRepo FormSetupRepo, 
            ISalesOrderRepo SalesOrderRepo,/* IDbContext dbContext,*/ IWorkContext workContext, ICacheManager cacheManager, IContraVoucher Reference, 
            IContraVoucher contravoucher, NeoErpCoreEntity objectEntity,IFinancialVoucherSaveService financialVoucherSave)
        {
            this._TestTemplateRepo = TestTemplateRepo;
            this._FormTemplateRepo = FormTemplateRepo;
            //subin change
            this._FormSetupRepo = FormSetupRepo;
            //this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._SalesOrderRepo = SalesOrderRepo;
            this._Reference = Reference;
            this._contravoucher = contravoucher;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule,_defaultValueForLog.FormCode);
            this._financialVoucherSaveService = financialVoucherSave;
        }
        [HttpGet]
        public List<Customers> GetAllCustomerSetupByFilter(string filter)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<Customers>();
            if (this._cacheManager.IsSet($"GetAllCustomerSetupByFilter_{userid}_{company_code}_{branch_code}_{filter}"))
            {
                var data = _cacheManager.Get<List<Customers>>($"GetAllCustomerSetupByFilter_{userid}_{company_code}_{branch_code}_{filter}");
                response = data;
            }
            else
            {
                var customerList = this._FormTemplateRepo.GetAllCustomerSetup(filter);
                this._cacheManager.Set($"GetAllCustomerSetupByFilter_{userid}_{company_code}_{branch_code}_{filter}", customerList, 20);
                response = customerList;
            }
            return response;
            // return this._FormTemplateRepo.GetAllCustomerSetup(filter);
        }
        public List<FinanceVoucherReference> GetReferenceList(string formcode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FinanceVoucherReference>();
            //if (this._cacheManager.IsSet($"GetReferenceList_{userid}_{company_code}_{branch_code}_{formcode}"))
            //{
            //    var data = _cacheManager.Get<List<FinanceVoucherReference>>($"GetReferenceList_{userid}_{company_code}_{branch_code}_{formcode}");
            //    response = data;
            //}
            //else
            //{
            //    var referenceList = this._Reference.GetFinanceVoucherReferenceList(formcode);
            //    this._cacheManager.Set($"GetReferenceList_{userid}_{company_code}_{branch_code}_{formcode}", referenceList, 20);
            //    response = referenceList;
            //}
            var referenceList = this._Reference.GetFinanceVoucherReferenceList(formcode);
            response = referenceList;
            return response;
            // return this._Reference.GetFinanceVoucherReferenceList(formcode);
        }
        [HttpGet]
        public List<FVPURCHASEEXPSHEETRERERENCE> GetReferenceListPES(string voucherno)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FVPURCHASEEXPSHEETRERERENCE>();
            var referenceList = this._Reference.GetFVReferencePurchaseexpsheet(voucherno);
            response = referenceList;            
            return response;
            // return this._Reference.GetFinanceVoucherReferenceList(formcode);
        }
        [HttpGet]
        public IHttpActionResult GetVouchersCount(string FORM_CODE, string TABLE_NAME)
        {
            try
            {
                var rslt = _contravoucher.GetTotalVoucher(FORM_CODE, TABLE_NAME);
                return Ok(rslt);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }                         
        [HttpPost]
        public HttpResponseMessage SaveAsDraftFormData(FormDetails model)
        {
            using (var transaction = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    Newtonsoft.Json.Linq.JObject mastercolumn = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Master_COLUMN_VALUE);
                    dynamic childcolumnvalues = JsonConvert.DeserializeObject(model.Child_COLUMN_VALUE);
                    var templateNo = string.Empty;
                    var maxTemplateQry = $@"SELECT MAX(TO_NUMBER(TEMPLATE_NO)+1) MAX_NO FROM FORM_TEMPLATE_SETUP";
                    templateNo = this._objectEntity.SqlQuery<int>(maxTemplateQry).FirstOrDefault().ToString();

                    var defaultCol = "CREATED_BY, CREATED_DATE";
                    var defaultVal = $@"'{this._workContext.CurrentUserinformation.login_code}',SYSDATE";
                    if (model.Save_Flag == "0")
                    {
                        var templateInsertQry = $@"INSERT INTO FORM_TEMPLATE_SETUP(TEMPLATE_NO,TEMPLATE_EDESC,TEMPLATE_NDESC,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,ASSIGNEE,ASSIGNED_DATE)
                                        VALUES('{templateNo}','{model.FORM_TEMPLATE.TEMPLATE_EDESC}','{model.FORM_TEMPLATE.TEMPLATE_NDESC}','{model.Form_Code}','{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.User_id}',SYSDATE,'N','{model.FORM_TEMPLATE.TEMPLATE_ASSIGNEE}',SYSDATE)";
                        var result = this._objectEntity.ExecuteSqlCommand(templateInsertQry);
                    }
                    else
                    {
                        var templateInsertQry = $@"UPDATE FORM_TEMPLATE_SETUP SET TEMPLATE_EDESC='{model.FORM_TEMPLATE.TEMPLATE_EDESC}',TEMPLATE_NDESC='{model.FORM_TEMPLATE.TEMPLATE_NDESC}',MODIFY_BY='{_workContext.CurrentUserinformation.User_id}',MODIFY_DATE=SYSDATE,ASSIGNEE='{model.FORM_TEMPLATE.TEMPLATE_ASSIGNEE}',ASSIGNED_DATE= SYSDATE WHERE TEMPLATE_NO='{model.FORM_TEMPLATE.TEMPLATE_NO}'";
                        var result = this._objectEntity.ExecuteSqlCommand(templateInsertQry);
                        var Qry = $@"SELECT * FROM FORM_TEMPLATE_DETAIL_SETUP WHERE TEMPLATE_NO='{model.FORM_TEMPLATE.TEMPLATE_NO}'";
                        var cResult = this._objectEntity.SqlQuery<DraftFormModel>(Qry).FirstOrDefault();
                        if (cResult != null)
                        {
                            defaultVal = $@"'{cResult.CREATED_BY}',TO_DATE('{cResult.CREATED_DATE}','MM-DD-YYYY HH:MI:SS AM'),'{this._workContext.CurrentUserinformation.login_code}',SYSDATE";
                            defaultCol = "CREATED_BY,CREATED_DATE,MODIFY_BY, MODIFY_DATE";
                        }
                        var deleteQry = $@"DELETE FORM_TEMPLATE_DETAIL_SETUP WHERE TEMPLATE_NO='{model.FORM_TEMPLATE.TEMPLATE_NO}'";
                        this._objectEntity.ExecuteSqlCommand(deleteQry);
                        templateNo = model.FORM_TEMPLATE.TEMPLATE_NO;
                    }
                    foreach (var v in mastercolumn)
                    {
                        string lastName = v.Key.Split('_').Last();

                        if (v.Value.ToString() != "")
                        {
                            var insertDraftQry = $@"INSERT INTO FORM_TEMPLATE_DETAIL_SETUP (TEMPLATE_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,COLUMN_NAME,COLUMN_VALUE,TABLE_NAME,DELETED_FLAG,SYN_ROWID,{defaultCol})
                                                     VALUES('{templateNo}','{model.Form_Code}','{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.branch_code}','0','{v.Key}','{v.Value}','{model.Table_Name}','N','',{defaultVal})";
                            this._objectEntity.ExecuteSqlCommand(insertDraftQry);
                        }
                    }
                    //if (voucherno == "undefined/undefined/undefined")
                    //{
                    int serialno = 1;
                    foreach (var item in childcolumnvalues)
                    {
                        var itemarray = JsonConvert.DeserializeObject(item.ToString());
                        foreach (var data in itemarray)
                        {
                            var dataname = data.Name.ToString();
                            string[] datanamesplit = dataname.Split('_');
                            string datalastName = datanamesplit.Last();

                            var datavalue = data.Value;
                            datavalue = datavalue.ToString();
                            if (datavalue != "")
                            {
                                var insertDraftQry = $@"INSERT INTO FORM_TEMPLATE_DETAIL_SETUP (TEMPLATE_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,COLUMN_NAME,COLUMN_VALUE,TABLE_NAME,DELETED_FLAG,SYN_ROWID,{defaultCol})
                                                     VALUES('{templateNo}','{model.Form_Code}','{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.branch_code}','{serialno}','{dataname}','{datavalue}','{model.Table_Name}','N','',{defaultVal})";
                                this._objectEntity.ExecuteSqlCommand(insertDraftQry);
                            }
                        }
                        serialno++;
                    }
                    //}
                    #region CLEAR CACHE
                    List<string> keystart = new List<string>();
                    keystart.Add("GetAllMenuItems");
                    keystart.Add("GetAllSalesOrderDetails");
                    keystart.Add("GetSalesOrderDetailFormDetailByFormCodeAndOrderNo");
                    keystart.Add("GetAllOrederNoByFlter");
                    keystart.Add("GetSubMenuList");
                    keystart.Add("GetSubMenuDetailList");
                    List<string> Record = new List<string>();
                    Record = this._cacheManager.GetAllKeys();
                    this._cacheManager.RemoveCacheByKey(keystart, Record);
                    #endregion
                    transaction.Commit();
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }

        }
  
        
        [HttpPost]
        public HttpResponseMessage SaveFinancialFormDataOld(FormDetails model)
        {
            #region variable Desc
            string primarydate = string.Empty, currencyformat = "NRS", today = DateTime.Now.ToString("dd-MMM-yyyy"), todaystring = System.DateTime.Now.ToString("yyyyMMddHHmmss"), createddatestring = "TO_DATE(SYSDATE,'DD-MON-YYYY hh24:mi:ss')", newvoucherNo = string.Empty, manualno = string.Empty, VoucherDate = createddatestring, createdDateForEdit = string.Empty, createdByForEdit = string.Empty, voucherNoForEdit = string.Empty;
            decimal exchangrate = 1;
            StringBuilder Columnbuilder = new StringBuilder();
            StringBuilder masterColumnBuilder = new StringBuilder();
            StringBuilder childColumnBuilder = new StringBuilder();
            StringBuilder valuesbuilder = new StringBuilder();
            bool insertmaintable = false, insertmastertable = false;
            var defaultCol = "MODIFY_BY,MODIFY_DATE";
            var sessionRowIDForedit = 0;
            var msg = string.Empty;
            var VoucherNumberGeneratedNo = string.Empty;
            #endregion

            using (var transaction = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                   
                    var voucherno = model.Order_No;

                    var primarydatecolumn = _FormTemplateRepo.GetPrimaryDateByTableName(model.Table_Name);
                    var primarycolname = _contravoucher.GetPrimaryColumnByTableName(model.Table_Name);
                    Newtonsoft.Json.Linq.JObject mastercolumn = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Master_COLUMN_VALUE);
                    //Newtonsoft.Json.Linq.JObject customcolumn = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Custom_COLUMN_VALUE);
                    dynamic childcolumnvalues = JsonConvert.DeserializeObject(model.Child_COLUMN_VALUE);
                    // dynamic childbudgetcentervalues = JsonConvert.DeserializeObject(model.BUDGET_TRANS_VALUE);
                    dynamic childsubledgervalues = JsonConvert.DeserializeObject(model.SUB_LEDGER_VALUE);
                    //dynamic tdsvalues = JsonConvert.DeserializeObject(model.TDS_VALUE);
                    dynamic vatvalues = JsonConvert.DeserializeObject(model.VAT_VALUE);
                    var drtotal = JsonConvert.DeserializeObject(model.DR_TOTAL_VALUE);
                    var crtotal = JsonConvert.DeserializeObject(model.CR_TOTAL_VALUE);


                    //return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK});

                    var staticColumns = "SERIAL_NO, FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID";
                    foreach (var v in mastercolumn)
                    {
                        if (v.Key == primarydatecolumn)
                        {
                            primarydate = v.Value.ToString();
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
                                VoucherDate = "trunc(TO_DATE(" + "'" + v.Value + "'" + ",'DD-MON-YYYY hh24:mi:ss'))";
                                valuesbuilder.Append("TO_DATE(" + "'" + v.Value + "'" + ",'DD-MON-YYYY hh24:mi:ss')").Append(",");
                            }
                        }
                        else if (v.Key.ToString() == primarycolname)
                        {
                             //newvoucherNo = v.Value.ToString();
                            //if (v.Value.ToString() == "")
                            //{
                            //    valuesbuilder.Append("'" + newvoucherNo + "'").Append(",");
                            //}
                            //else
                            //{
                            //    valuesbuilder.Append("'" + v.Value + "'").Append(",");
                            //}
                            if (voucherNoForEdit == "")
                            {
                                valuesbuilder.Append("'" + v.Value + "'").Append(",");
                                newvoucherNo = v.Value.ToString();
                            }
                            else
                            {
                                valuesbuilder.Append("'" + voucherNoForEdit + "'").Append(",");
                                newvoucherNo = voucherNoForEdit;
                            }
                        }
                        else if (v.Key.ToString() == "MANUAL_NO")
                        {
                            valuesbuilder.Append("'" + v.Value + "'").Append(",");
                            manualno = v.Value.ToString();
                        }
                         //else if (v.Key.ToString() == "CURRENCY_CODE")
                        //{

                        //    valuesbuilder.Append("");
                        //}
                        //else if (v.Key.ToString() == "EXCHANGE_RATE")
                        //{

                        //    valuesbuilder.Append("");
                        //}
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
                        else if (v.Key.ToString() == "Display_BFlag")
                        {
                            valuesbuilder.Append("");
                        }
                        else { valuesbuilder.Append("'" + v.Value + "'").Append(","); }
                    }
                    #region Insert Code
                    if (voucherno == "undefined")
                    {
                        // Insert Download

                        int serialno = 1;
                        foreach (var item in childcolumnvalues)
                        {
                            StringBuilder childvaluesbuilder = new StringBuilder();
                            StringBuilder masterchildvaluesbuilder = new StringBuilder();
                            var itemarray = JsonConvert.DeserializeObject(item.ToString());
                            var budget_flag = "L";
                            foreach (var data in itemarray)
                            {
                                if ((data.Name.ToString() == "Display_BFlag"))
                                {
                                    childColumnBuilder.Append("");
                                }
                                else
                                {
                                    childColumnBuilder.Append(data.Name.ToString()).Append(",");
                                }
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
                                    if (datavalue.Value.ToString() == "")
                                    {
                                        childvaluesbuilder.Append("''").Append(",");
                                    }
                                    else
                                    {
                                        childvaluesbuilder.Append(datavalue.Value).Append(",");
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
                                else if (datalastName == "QUANTITY")
                                {
                                    if (datavalue.Value.ToString() == "")
                                    {
                                        childvaluesbuilder.Append("''").Append(",");
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
                                    if (datavalue.Value == null)
                                    {
                                        childvaluesbuilder.Append("' '").Append(",");
                                    }
                                    else
                                    {
                                        childvaluesbuilder.Append("'" + datavalue.Value + "'").Append(",");
                                    }
                                }
                                else if (dataname == "Display_BFlag")
                                {
                                    childvaluesbuilder.Append("");
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
                            Columnbuilder.Append(masterColumnBuilder);
                            Columnbuilder.Append(childColumnBuilder);
                            Columnbuilder.Append(staticColumns);

                            string values = string.Empty;
                            masterchildvaluesbuilder.Append(valuesbuilder);
                            masterchildvaluesbuilder.Append(childvaluesbuilder);
                            values = masterchildvaluesbuilder.ToString().TrimEnd(',');

                            var insertQuery = string.Format(@"insert into " + model.Table_Name + "({0}) values({1},{2},{3},{4},{5},{6},{7},{8},{9})", Columnbuilder, values, serialno, "'" + model.Form_Code + "'", "'" + this._workContext.CurrentUserinformation.company_code + "'", "'" + this._workContext.CurrentUserinformation.branch_code + "'", "'" + this._workContext.CurrentUserinformation.login_code.ToUpper() + "'", createddatestring, "'N'", "'" + newvoucherNo + "'");
                            this._objectEntity.ExecuteSqlCommand(insertQuery);

                            insertmaintable = true;
                            masterchildvaluesbuilder.Length = 0;
                            masterchildvaluesbuilder.Capacity = 0;
                            childvaluesbuilder.Length = 0;
                            childvaluesbuilder.Capacity = 0;
                            Columnbuilder.Length = 0;
                            Columnbuilder.Capacity = 0;
                            masterColumnBuilder.Length = 0;
                            masterColumnBuilder.Capacity = 0;
                            childColumnBuilder.Length = 0;
                            childColumnBuilder.Capacity = 0;
                            serialno++;
                        }
                       

                        // Insert into Master transaction 
                        if (insertmaintable == true)
                        {
                            string insertmasterQuery = string.Format(@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,SESSION_ROWID,SYN_ROWID,EXCHANGE_RATE) VALUES('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}',TO_DATE('{8}','DD-MON-YYYY hh24:mi:ss'),{9},'" + newvoucherNo + "','{10}',{11})",
                              newvoucherNo, drtotal, model.Form_Code, this._workContext.CurrentUserinformation.company_code, this._workContext.CurrentUserinformation.branch_code, this._workContext.CurrentUserinformation.login_code.ToUpper(), 'N', currencyformat, today, VoucherDate, manualno, exchangrate);
                            this._objectEntity.ExecuteSqlCommand(insertmasterQuery);

                            
                            //  Doubt why this is here
                            if (!string.IsNullOrEmpty(model.TempCode))
                            {
                                string UpdateQuery = $@"UPDATE FORM_TEMPLATE_SETUP  SET SAVED_DRAFT='Y' WHERE TEMPLATE_NO='{model.TempCode}'  AND  COMPANY_CODE ='{this._workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{ this._workContext.CurrentUserinformation.branch_code}'";
                                this._objectEntity.ExecuteSqlCommand(UpdateQuery);
                            }
                            insertmastertable = true;
                        }

                        // Budget transaction 
                        if (model.BUDGET_TRANS_VALUE != null)
                        {
                            var budSerial = 1;
                            dynamic childbudgetcentervalues = JsonConvert.DeserializeObject(model.BUDGET_TRANS_VALUE);
                            foreach (var item in childbudgetcentervalues)
                            {
                                var cbcitemarray = JsonConvert.DeserializeObject(item.ToString());
                                string budgetflag = string.Empty, acccode = string.Empty;
                                foreach (var cbcdata in cbcitemarray)
                                {
                                    var cbcdataname = cbcdata.Name.ToString();
                                    var cbcdatavalue = cbcdata.Value;
                                    if (cbcdataname == "ACC_CODE")
                                    {
                                        acccode = cbcdatavalue.ToString();
                                    }
                                    else if (cbcdataname == "BUDGET_FLAG")
                                    {
                                        if (cbcdatavalue != "")
                                        {
                                            budgetflag = cbcdatavalue.ToString();
                                        }
                                        else
                                        {
                                            budgetflag = "L";
                                        }
                                    }
                                    else if (cbcdataname == "BUDGET")
                                    {
                                        //var subledgeritemarray = JsonConvert.DeserializeObject(csvdata.ToString());
                                        dynamic budgetdatas = JsonConvert.DeserializeObject(cbcdatavalue.ToString());

                                        foreach (var budgetdata in budgetdatas)
                                        {
                                            string budgetval = string.Empty, narration = string.Empty, b_flag = string.Empty;
                                            var ammount = 0.00;
                                            var budgetarray = JsonConvert.DeserializeObject(budgetdata.ToString());
                                            foreach (var bdata in budgetarray)
                                            {
                                                var taname = bdata.Name.ToString();
                                                var tavalue = bdata.Value.ToString();
                                                if (taname == "BUDGET_VAL")
                                                {
                                                    budgetval = tavalue.ToString();
                                                }
                                                else if (taname == "AMOUNT" && tavalue != "")
                                                {
                                                    ammount = Convert.ToDouble(tavalue);
                                                }
                                                if (taname == "NARRATION")
                                                {
                                                    narration = tavalue.ToString();
                                                }
                                                //else if (taname == "SERIAL_NO")
                                                //{
                                                //    serial = tavalue;
                                                //}
                                            }
                                            if (budgetval != "")
                                            {
                                                string maxtransnoquery = string.Format(@"SELECT TO_NUMBER((MAX(TO_NUMBER(TRANSACTION_NO))+1)) as TRANSACTIONNO FROM BUDGET_TRANSACTION");
                                                int newMaxTransNoForBudget = this._objectEntity.SqlQuery<int>(maxtransnoquery).FirstOrDefault();
                                                string insertbudgettransQuery = $@"INSERT INTO BUDGET_TRANSACTION(
                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,BUDGET_FLAG,SERIAL_NO,BUDGET_CODE,
                                                              BUDGET_AMOUNT,PARTICULARS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                              CURRENCY_CODE,EXCHANGE_RATE,VALIDATION_FLAG,ACC_CODE,SESSION_ROWID)
                                                              VALUES('{newMaxTransNoForBudget}','{model.Form_Code}','{newvoucherNo}','{budgetflag}',{budSerial},'{budgetval}',
                                                             {ammount},'{narration}','{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.branch_code}','{this._workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','NRS',{1},'Y','{acccode}','{newvoucherNo}')";
                                                this._objectEntity.ExecuteSqlCommand(insertbudgettransQuery);
                                            }

                                        }
                                    }
                                }
                                budSerial++;
                            }
                        }

                        // Tds transaction
                        if (model.TDS_VALUE != null)
                        {
                            var tdsSerial = 1;
                            dynamic totaltdsvaluesarr = JsonConvert.DeserializeObject(model.TDS_VALUE);
                            foreach (var alltdsarr in totaltdsvaluesarr)
                            {
                                var tds_item_array = JsonConvert.DeserializeObject(alltdsarr.ToString());
                                string acccode = string.Empty, remarks = string.Empty;
                                var totaltdsamount = 0.00;
                                foreach (var ind_tds_arr in tds_item_array)
                                {
                                    var ind_tds_arr_name = ind_tds_arr.Name.ToString();
                                    var ind_tds_arr_value = ind_tds_arr.Value;
                                    if (ind_tds_arr_name == "ACC_CODE")
                                    {
                                        acccode = ind_tds_arr_value.ToString();

                                    }
                                    if (ind_tds_arr_name == "REMARKS")
                                    {
                                        remarks = ind_tds_arr_value.ToString();
                                    }
                                    if (ind_tds_arr_name == "TOTAL_TDS_AMOUNT")
                                    {
                                        var tta = ind_tds_arr_value;
                                        if (tta != "{}" || tta != null)
                                        { totaltdsamount = Convert.ToDouble(ind_tds_arr_value); }
                                        else
                                        { totaltdsamount = 0; }

                                    }
                                    if (ind_tds_arr_name == "CHILDTDS")
                                    {
                                        dynamic childtdsdatas = JsonConvert.DeserializeObject(ind_tds_arr_value.ToString());

                                        foreach (var childtdsdata in childtdsdatas)
                                        {
                                            int tdsserialno = 1;
                                            string tdssuppliercode = string.Empty, tdsacccode = string.Empty, tdstype = string.Empty, tdsmeetingtype = string.Empty;
                                            var tdsnetamount = 0.00;
                                            var tdsamount = 0.00;
                                            var tdspercentage = 0.00;
                                            var ctdsarray = JsonConvert.DeserializeObject(childtdsdata.ToString());
                                            foreach (var ctdsdata in ctdsarray)
                                            {
                                                var ctname = ctdsdata.Name.ToString();
                                                var ctvalue = ctdsdata.Value.ToString();
                                                if (ctname == "SERIAL_NO")
                                                {
                                                    tdsserialno = Convert.ToInt32(ctvalue);
                                                }
                                                if (ctname == "SUPPLIER_CODE")
                                                {
                                                    tdssuppliercode = ctvalue;
                                                }
                                                if (ctname == "ACC_CODE")
                                                {
                                                    tdsacccode = ctvalue;
                                                }
                                                if (ctname == "TDS_TYPE_CODE")
                                                {
                                                    tdstype = ctvalue;
                                                }
                                                if (ctname == "MEETING_TYPE_CODE")
                                                {
                                                    tdstype = ctvalue;
                                                }
                                                if (ctname == "NET_AMOUNT")
                                                {
                                                    if (ctvalue != "{}" || ctvalue != null)
                                                    { tdsnetamount = Convert.ToDouble(ctvalue); }
                                                    else
                                                    { tdsnetamount = 0; }
                                                    //tdsnetamount = Convert.ToDouble(ctvalue);
                                                }
                                                if (ctname == "TDS_PERCENTAGE")
                                                {
                                                    if (ctvalue != "{}" || ctvalue != null)
                                                    { tdspercentage = Convert.ToDouble(ctvalue); }
                                                    else
                                                    { tdspercentage = 0; }
                                                    //tdspercentage = Convert.ToDouble(ctvalue);
                                                }
                                                if (ctname == "TDS_AMOUNT")
                                                {
                                                    if (ctvalue != "{}" || ctvalue != null)
                                                    { tdsamount = Convert.ToDouble(ctvalue); }
                                                    else
                                                    { tdsamount = 0; }
                                                    //tdsamount = Convert.ToDouble(ctvalue);
                                                }
                                            }
                                            string inserttdsQuery = $@"INSERT INTO FA_DC_TDS_INVOICE(
                                                              SERIAL_NO,INVOICE_NO,INVOICE_DATE,MANUAL_NO,FORM_CODE,CS_CODE,ACC_CODE,TDS_TYPE_CODE,TAXABLE_AMOUNT,TDS_PERCENT,TDS_AMOUNT,REMARKS,CURRENCY_CODE,EXCHANGE_RATE,COMPANY_CODE,BRANCH_CODE,DELETED_FLAG,CREATED_BY,CREATED_DATE,SESSION_ROWID,TRAN_TYPE)
                                                              VALUES({tdsserialno},'{newvoucherNo}',{VoucherDate},'{12345}',{model.Form_Code},'{tdssuppliercode}',
                                                             '{tdsacccode}','{tdstype}',{tdsnetamount},{tdspercentage},{tdsamount},'{remarks}','NRS',{1},'{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.branch_code}','N','{this._workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'1234','DR')";
                                            this._objectEntity.ExecuteSqlCommand(inserttdsQuery);
                                        }
                                    }

                                }
                                tdsSerial++;
                            }
                        }


                        // Vat transation
                        if (model.VAT_VALUE != null)
                        {
                            var vatSerial = 1;
                            dynamic totalvatvaluesarr = JsonConvert.DeserializeObject(model.VAT_VALUE);
                            foreach (var allvatarr in totalvatvaluesarr)
                            {
                                var vat_item_array = JsonConvert.DeserializeObject(allvatarr.ToString());
                                string vatacccode = string.Empty, vatremarks = string.Empty, vatdoctype = string.Empty, vattype = string.Empty;
                                var totalvatamount = 0.00;
                                foreach (var ind_vat_arr in vat_item_array)
                                {
                                    var ind_vat_arr_name = ind_vat_arr.Name.ToString();
                                    var ind_vat_arr_value = ind_vat_arr.Value;
                                    if (ind_vat_arr_name == "ACC_CODE")
                                    {
                                        vatacccode = ind_vat_arr_value;
                                    }
                                    if (ind_vat_arr_name == "REMARKS")
                                    {
                                        vatremarks = ind_vat_arr_value;
                                    }
                                    if (ind_vat_arr_name == "DOC_TYPE")
                                    {
                                        vatdoctype = ind_vat_arr_value;
                                    }
                                    if (ind_vat_arr_name == "TYPE")
                                    {
                                        vattype = ind_vat_arr_value;
                                    }
                                    if (ind_vat_arr_name == "TOTAL_VAT_AMOUNT")
                                    {
                                        totalvatamount = Convert.ToDouble(ind_vat_arr_value);
                                    }
                                    if (ind_vat_arr_name == "CHILDVAT")
                                    {
                                        dynamic childvatdatas = JsonConvert.DeserializeObject(ind_vat_arr_value.ToString());

                                        foreach (var childvatdata in childvatdatas)
                                        {
                                            int vatserialno = 1;
                                            string vatsuppliercode = string.Empty;
                                            var vattaxableamount = 0.00;
                                            var vatamount = 0.00;

                                            var cvatarray = JsonConvert.DeserializeObject(childvatdata.ToString());
                                            foreach (var cvatdata in cvatarray)
                                            {
                                                var cvname = cvatdata.Name.ToString();
                                                var cvvalue = cvatdata.Value.ToString();
                                                if (cvname == "SERIAL_NO")
                                                {
                                                    vatserialno = Convert.ToInt32(cvvalue);
                                                }
                                                if (cvname == "TAXABLE_AMOUNT")
                                                {
                                                    vattaxableamount = Convert.ToDouble(cvvalue);
                                                }
                                                if (cvname == "VAT_AMOUNT")
                                                {
                                                    vatamount = Convert.ToDouble(cvvalue);
                                                }
                                            }
                                            string insertvatQuery = $@"INSERT INTO FA_DC_VAT_INVOICE(
                                                              INVOICE_NO,MANUAL_NO,INVOICE_DATE,FORM_CODE,CS_CODE,TAXABLE_AMOUNT,VAT_AMOUNT,P_TYPE,DOC_TYPE,REMARKS,CURRENCY_CODE,EXCHANGE_RATE,COMPANY_CODE,BRANCH_CODE,DELETED_FLAG,CREATED_BY,CREATED_DATE,SERIAL_NO,SESSION_ROWID,ACC_CODE)
                                                              VALUES('{newvoucherNo}','{1234}',{VoucherDate},'{vatsuppliercode}',{vattaxableamount},{vatamount},
                                                             '{vattype}','{vatdoctype}','{vatremarks}','NRS',{1},'{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.branch_code}','N','{this._workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'{vatserialno}','1234','{vatacccode}')";
                                            this._objectEntity.ExecuteSqlCommand(insertvatQuery);
                                        }
                                    }
                                }

                            }
                        }
                        var subSerial = 1;
                        // subledger
                        foreach (var it in childsubledgervalues)
                        {
                            string cacccode = string.Empty, transactiontype = string.Empty;
                            var csvitemarray = JsonConvert.DeserializeObject(it.ToString());
                            foreach (var csvdata in csvitemarray)
                            {
                                var csvdataname = csvdata.Name.ToString();
                                var csvdatavalue = csvdata.Value;
                                if (csvdataname == "ACC_CODE")
                                {
                                    cacccode = csvdata.Value;
                                }
                                if (csvdataname == "TRANSACTION_TYPE")
                                {
                                    if (csvdata.Value.ToString() == "")
                                    { transactiontype = "DR"; }
                                    else { transactiontype = csvdata.Value; }
                                }
                                if (csvdataname == "SUBLEDGER")
                                {
                                    dynamic subl = JsonConvert.DeserializeObject(csvdatavalue.ToString());
                                    foreach (var sldata in subl)
                                    {
                                        double dramount = 0.00, cramount = 0.00, balanceamount = 0.00;
                                        string particular = string.Empty, subcode = string.Empty;
                                        var itemarray = JsonConvert.DeserializeObject(sldata.ToString());
                                        foreach (var ata in itemarray)
                                        {
                                            var taaname = ata.Name.ToString();
                                            var taavalue = ata.Value.ToString();
                                            if (taaname == "SUB_CODE")
                                            {
                                                subcode = ata.Value;
                                            }
                                            else if (taaname == "AMOUNT" && taavalue != "")
                                            {
                                                if (transactiontype == "DR")
                                                {
                                                    dramount = Convert.ToDouble(taavalue);
                                                    cramount = 0.00;
                                                    balanceamount = dramount - cramount;
                                                }
                                                else
                                                {
                                                    dramount = 0.00;
                                                    cramount = Convert.ToDouble(taavalue);
                                                    balanceamount = dramount - cramount;
                                                }
                                            }
                                            else if (taaname == "PARTICULARS")
                                            {
                                                particular = taavalue;
                                            }
                                            //else if (taaname == "SERIAL_NO")
                                            //{
                                            //    serial = taavalue;
                                            //}
                                        }
                                        if (subcode != "")
                                        {
                                            string maxtransnoquerySubledger = string.Format(@"SELECT TO_NUMBER((MAX(TO_NUMBER(TRANSACTION_NO))+1)) as TRANSACTIONNO  FROM FA_VOUCHER_SUB_DETAIL  WHERE TRANSACTION_NO IS NOT NULL");
                                            int newMaxTransNoForSubLedger = this._objectEntity.SqlQuery<int>(maxtransnoquerySubledger).FirstOrDefault();

                                            string insertSubLedgerQuery = $@"INSERT INTO FA_VOUCHER_SUB_DETAIL (TRANSACTION_NO,FORM_CODE,
                                                                   VOUCHER_DATE,VOUCHER_NO,SUB_CODE,ACC_CODE,
                                                                   PARTICULARS,TRANSACTION_TYPE,DR_AMOUNT,CR_AMOUNT,
                                                                   BRANCH_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                   SERIAL_NO,CURRENCY_CODE,EXCHANGE_RATE,SYN_ROWID) 
                                                                   VALUES('{newMaxTransNoForSubLedger}','{model.Form_Code}',SYSDATE,
                                                                  '{newvoucherNo}','{subcode}','{cacccode}','{particular}'
                                                                  ,'{transactiontype}',{dramount},{cramount},'{this._workContext.CurrentUserinformation.branch_code}'
                                                                  ,'{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N',{subSerial},'NRS',{1},'{newvoucherNo}')";
                                            this._objectEntity.ExecuteSqlCommand(insertSubLedgerQuery);
                                        }
                                    }
                                }
                            }
                            subSerial++;
                        }
                        #region CLEAR CACHE
                        List<string> keystart = new List<string>();
                        keystart.Add("GetAllMenuItems");
                        keystart.Add("GetAllSalesOrderDetails");
                        keystart.Add("GetSalesOrderDetailFormDetailByFormCodeAndOrderNo");
                        keystart.Add("GetAllOrederNoByFlter");
                        keystart.Add("GetSubMenuList");
                        keystart.Add("GetSubMenuDetailList");
                        List<string> Record = new List<string>();
                        Record = this._cacheManager.GetAllKeys();
                        this._cacheManager.RemoveCacheByKey(keystart, Record);
                        #endregion
                     

                      
                      //  VoucherNumberGeneratedNo

                        if (model.Save_Flag == "0")
                        {
                            // Update voucher no according to sessionrow id)
                          
                            transaction.Commit();
                            msg = "INSERTED";
                        }
                        else if (model.Save_Flag == "3")
                        {
                            transaction.Commit();
                            msg = "SAVEANDPRINT";
                        }
                        else
                        {
                            transaction.Commit();
                            msg = "INSERTEDANDCONTINUE";
                        }
                       
                    }
                    #endregion
                    //code for update 
                    #region Edit Code
                    else
                    {
                        var getPrevDataQuery = $@"SELECT VOUCHER_NO,SESSION_ROWID, CREATED_BY, CREATED_DATE FROM MASTER_TRANSACTION WHERE  VOUCHER_NO= '{voucherno}'";
                        var defaultData = this._objectEntity.SqlQuery<SalesOrderDetail>(getPrevDataQuery).ToList();
                        foreach (var def in defaultData)
                        {
                            voucherNoForEdit = def.VOUCHER_NO.ToString();
                            createdDateForEdit = "TO_DATE('" + def.CREATED_DATE.ToString() + "', 'MM-DD-YYYY hh12:mi:ss pm')";

                            createdByForEdit = def.CREATED_BY.ToString().ToUpper();
                            sessionRowIDForedit = Convert.ToInt32(def.SESSION_ROWID);
                        }

                        //delete maintable
                        string deletequery = string.Format(@"DELETE FROM " + model.Table_Name + " where " + primarycolname + "='{0}' and COMPANY_CODE='{1}'", voucherno, this._workContext.CurrentUserinformation.company_code);
                        this._objectEntity.ExecuteSqlCommand(deletequery);

                        int updateserialno = 1;
                        //for child column values
                        foreach (var item in childcolumnvalues)
                        {
                            StringBuilder childvaluesbuilder = new StringBuilder();
                            StringBuilder masterchildvaluesbuilder = new StringBuilder();
                            var itemarray = JsonConvert.DeserializeObject(item.ToString());
                            var budget_flag = "L";
                            foreach (var data in itemarray)
                            {
                                if ((data.Name.ToString() == "Display_BFlag"))
                                {
                                    Columnbuilder.Append(data.Name.ToString()).Append(",");
                                }
                                else
                                {
                                    childColumnBuilder.Append(data.Name.ToString()).Append(",");
                                }
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
                                    if (datavalue.Value.ToString() == "")
                                    {
                                        childvaluesbuilder.Append("''").Append(",");

                                    }
                                    else
                                    {
                                        childvaluesbuilder.Append(datavalue.Value).Append(",");
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
                                else if (datalastName == "QUANTITY")
                                {
                                    if (datavalue.Value.ToString() == "")
                                    {
                                        childvaluesbuilder.Append("''").Append(",");

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
                                    if (datavalue.Value == null)
                                    {
                                        childvaluesbuilder.Append("' '").Append(",");
                                    }
                                    else
                                    {
                                        childvaluesbuilder.Append("'" + datavalue.Value + "'").Append(",");
                                    }
                                }
                                else if (dataname == "Display_BFlag")
                                {
                                    childvaluesbuilder.Append("");
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
                            foreach (var m in mastercolumn)
                            {
                                masterColumnBuilder.Append(m.Key.ToString()).Append(",");
                            }
                            Columnbuilder.Append(masterColumnBuilder);
                            Columnbuilder.Append(childColumnBuilder);
                            Columnbuilder.Append(staticColumns);
                            var values = "";
                            masterchildvaluesbuilder.Append(valuesbuilder);
                            masterchildvaluesbuilder.Append(childvaluesbuilder);
                            values = NewMethod(masterchildvaluesbuilder);
                            //insert into main table 
                            var insertQuery = string.Format(@"insert into " + model.Table_Name + "({0},{12}) values({1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11})", Columnbuilder, values, updateserialno, "'" + model.Form_Code + "'", "'" + this._workContext.CurrentUserinformation.company_code + "'", "'" + this._workContext.CurrentUserinformation.branch_code + "'", "'" + createdByForEdit + "'", createdDateForEdit, "'N'", sessionRowIDForedit, "'" + this._workContext.CurrentUserinformation.login_code.ToUpper() + "'", "SYSDATE", defaultCol);
                            this._objectEntity.ExecuteSqlCommand(insertQuery);
                            masterchildvaluesbuilder.Length = 0;
                            masterchildvaluesbuilder.Capacity = 0;
                            childvaluesbuilder.Length = 0;
                            childvaluesbuilder.Capacity = 0;
                            Columnbuilder.Length = 0;
                            Columnbuilder.Capacity = 0;
                            masterColumnBuilder.Length = 0;
                            masterColumnBuilder.Capacity = 0;
                            childColumnBuilder.Length = 0;
                            childColumnBuilder.Capacity = 0;
                            updateserialno++;
                        }
                        //update master table 
                        string query = $@"UPDATE MASTER_TRANSACTION SET VOUCHER_AMOUNT={drtotal},VOUCHER_DATE={VoucherDate},MODIFY_BY = '{this._workContext.CurrentUserinformation.login_code}',SYN_ROWID='{manualno}' , MODIFY_DATE = SYSDATE,CURRENCY_CODE='{currencyformat}',EXCHANGE_RATE={exchangrate} where VOUCHER_NO='{voucherno}'  and COMPANY_CODE='{this._workContext.CurrentUserinformation.company_code}'";
                        var rowCount = _objectEntity.ExecuteSqlCommand(query);
                        //for updates in budget center.
                        //delete maintable
                        string deletebudgetcenterquery = string.Format(@"DELETE FROM BUDGET_TRANSACTION where REFERENCE_NO='{0}' and COMPANY_CODE='{1}'", voucherno, this._workContext.CurrentUserinformation.company_code);
                        this._objectEntity.ExecuteSqlCommand(deletebudgetcenterquery);
                        if (model.BUDGET_TRANS_VALUE != null)
                        {
                            dynamic childbudgetcentervalues = JsonConvert.DeserializeObject(model.BUDGET_TRANS_VALUE);
                            var budgetserial = 1;
                            foreach (var item in childbudgetcentervalues)
                            {
                                var cbcitemarray = JsonConvert.DeserializeObject(item.ToString());
                                string budgetflag = string.Empty, acccode = string.Empty;
                                foreach (var cbcdata in cbcitemarray)
                                {
                                    var cbcdataname = cbcdata.Name.ToString();
                                    var cbcdatavalue = cbcdata.Value;
                                    if (cbcdataname == "ACC_CODE")
                                    {
                                        acccode = cbcdatavalue.ToString();
                                    }
                                    else if (cbcdataname == "BUDGET_FLAG")
                                    {
                                        if (cbcdatavalue != "")
                                        {
                                            budgetflag = cbcdatavalue.ToString();
                                        }
                                        else
                                        {
                                            budgetflag = "L";
                                        }
                                    }
                                    else if (cbcdataname == "BUDGET")
                                    {
                                        dynamic budgetdatas = JsonConvert.DeserializeObject(cbcdatavalue.ToString());
                                        foreach (var budgetdata in budgetdatas)
                                        {
                                            string budgetval = string.Empty, b_flag = string.Empty, narration = string.Empty;
                                            var ammount = 0.00;
                                            var budgetarray = JsonConvert.DeserializeObject(budgetdata.ToString());
                                            foreach (var bdata in budgetarray)
                                            {
                                                var taname = bdata.Name.ToString();
                                                var tavalue = bdata.Value.ToString();

                                                if (taname == "BUDGET_VAL")
                                                {
                                                    budgetval = tavalue.ToString();
                                                }
                                                else if (taname == "AMOUNT" && tavalue != "")
                                                {
                                                    ammount = Convert.ToDouble(tavalue);
                                                }
                                                if (taname == "NARRATION")
                                                {
                                                    narration = tavalue.ToString();
                                                }
                                                //else if(taname =="SERIAL_NO")
                                                //{
                                                //    serial = tavalue;
                                                //}
                                            }
                                            if (budgetval != "")
                                            {
                                                string transquery = string.Format(@"SELECT TO_NUMBER((MAX(TO_NUMBER(TRANSACTION_NO))+1)) as TRANSACTIONNO FROM BUDGET_TRANSACTION");
                                                int newtransno = this._objectEntity.SqlQuery<int>(transquery).FirstOrDefault();

                                                string insertbudgettransQuery = $@"INSERT INTO BUDGET_TRANSACTION(
                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,BUDGET_FLAG,SERIAL_NO,BUDGET_CODE,
                                                              BUDGET_AMOUNT,PARTICULARS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                              CURRENCY_CODE,EXCHANGE_RATE,VALIDATION_FLAG,ACC_CODE,SESSION_ROWID, {defaultCol})
                                                              VALUES('{newtransno}','{model.Form_Code}','{voucherNoForEdit}','{budgetflag}',{budgetserial},'{budgetval}',
                                                             '{ammount}','{narration}','{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.branch_code}','{createdByForEdit}',{createdDateForEdit},'N','NRS',{1},'Y','{acccode}',{sessionRowIDForedit},'{this._workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE)";
                                                this._objectEntity.ExecuteSqlCommand(insertbudgettransQuery);
                                            }
                                        }
                                    }
                                }
                                budgetserial++;
                            }
                        }
                        //for sub ledger
                        string deletesubledgerquery = string.Format(@"DELETE FROM FA_VOUCHER_SUB_DETAIL where  VOUCHER_NO='{0}' and COMPANY_CODE='{1}'", voucherno, this._workContext.CurrentUserinformation.company_code);
                        this._objectEntity.ExecuteSqlCommand(deletesubledgerquery);
                        var subledgerserial = 1;
                        foreach (var it in childsubledgervalues)
                        {
                            //int newTransactionNo = latestmaxtransno + 1;
                            string cacccode = string.Empty, transactiontype = string.Empty;
                            var csvitemarray = JsonConvert.DeserializeObject(it.ToString());
                            foreach (var csvdata in csvitemarray)
                            {
                                var csvdataname = csvdata.Name.ToString();
                                var csvdatavalue = csvdata.Value;
                                if (csvdataname == "ACC_CODE")
                                {
                                    cacccode = csvdata.Value;
                                }
                                if (csvdataname == "TRANSACTION_TYPE")
                                {
                                    if (csvdata.Value.ToString() == "")
                                    { transactiontype = "DR"; }
                                    else { transactiontype = csvdata.Value; }
                                }
                                if (csvdataname == "SUBLEDGER")
                                {
                                    //var subledgeritemarray = JsonConvert.DeserializeObject(csvdata.ToString());
                                    dynamic subl = JsonConvert.DeserializeObject(csvdatavalue.ToString());
                                    foreach (var sldata in subl)
                                    {
                                        double dramount = 0.00, cramount = 0.00, balanceamount = 0.00;
                                        string particular = string.Empty, subcode = string.Empty;
                                        var itemarray = JsonConvert.DeserializeObject(sldata.ToString());
                                        foreach (var ata in itemarray)
                                        {
                                            var taaname = ata.Name.ToString();
                                            var taavalue = ata.Value.ToString();
                                            if (taaname == "SUB_CODE")
                                            {
                                                subcode = ata.Value;
                                            }
                                            else if (taaname == "AMOUNT" && taavalue != "")
                                            {
                                                if (transactiontype == "DR")
                                                {
                                                    dramount = Convert.ToDouble(taavalue);
                                                    cramount = 0.00;
                                                    balanceamount = dramount - cramount;
                                                }
                                                else
                                                {
                                                    dramount = 0.00;
                                                    cramount = Convert.ToDouble(taavalue);
                                                    balanceamount = dramount - cramount;
                                                }
                                            }
                                            else if (taaname == "PARTICULARS")
                                            {
                                                particular = taavalue;
                                            }
                                            //else if(taaname =="SERIAL_NO")
                                            //{
                                            //    serial = ata.Value;
                                            //}
                                        }
                                        if (subcode != "")
                                        {
                                            string latestTransactionNo = string.Format(@"SELECT TO_NUMBER((MAX(TO_NUMBER(TRANSACTION_NO))+1)) as TRANSACTIONNO  FROM FA_VOUCHER_SUB_DETAIL  WHERE TRANSACTION_NO IS NOT NULL");
                                            int newTransactionNo = _objectEntity.SqlQuery<int>(latestTransactionNo).FirstOrDefault();
                                            string insertSubLedgerQuery = $@"INSERT INTO FA_VOUCHER_SUB_DETAIL (TRANSACTION_NO,FORM_CODE,
                                                                   VOUCHER_DATE,VOUCHER_NO,SUB_CODE,ACC_CODE,
                                                                   PARTICULARS,TRANSACTION_TYPE,DR_AMOUNT,CR_AMOUNT,
                                                                   BRANCH_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                   SERIAL_NO,CURRENCY_CODE,EXCHANGE_RATE,SYN_ROWID) 
                                                                   VALUES('{newTransactionNo}','{model.Form_Code}',SYSDATE,
                                                                  '{newvoucherNo}','{subcode}','{cacccode}','{particular}'
                                                                  ,'{transactiontype}',{dramount},{cramount},'{this._workContext.CurrentUserinformation.branch_code}'
                                                                  ,'{this._workContext.CurrentUserinformation.company_code}','{this._workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N',{subledgerserial},'NRS',{1},'{sessionRowIDForedit}')";
                                            this._objectEntity.ExecuteSqlCommand(insertSubLedgerQuery);

                                        }
                                    }
                                }
                            }
                            subledgerserial++;
                        }
                        #region CLEAR CACHE
                        List<string> keystart = new List<string>();
                        keystart.Add("GetAllMenuItems");
                        keystart.Add("GetAllSalesOrderDetails");
                        keystart.Add("GetSalesOrderDetailFormDetailByFormCodeAndOrderNo");
                        keystart.Add("GetAllOrederNoByFlter");
                        keystart.Add("GetSubMenuList");
                        keystart.Add("GetSubMenuDetailList");
                        List<string> Record = new List<string>();
                        Record = this._cacheManager.GetAllKeys();
                        this._cacheManager.RemoveCacheByKey(keystart, Record);
                        #endregion
                        if (model.Save_Flag == "4")
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATEDANDPRINT", STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = voucherno, SessionNo = sessionRowIDForedit, VoucherDate = primarydate, FormCode = model.Form_Code });
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = voucherno, SessionNo = sessionRowIDForedit, VoucherDate = primarydate, FormCode = model.Form_Code });
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            // If everything is ok then execute this code
            string updateTransactionNo = $"select FN_GET_VOUCHER_NO('{this._workContext.CurrentUserinformation.company_code}','{model.Form_Code}',{VoucherDate},'{newvoucherNo}') from dual ";
             VoucherNumberGeneratedNo = _objectEntity.SqlQuery<string>(updateTransactionNo).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = msg, STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = VoucherNumberGeneratedNo, SessionNo = newvoucherNo, VoucherDate = primarydate, FormCode = model.Form_Code });
            //}
        }
        [HttpPost]
        public HttpResponseMessage DeleteFinanceVoucher(string voucherno, string formcode)
        {
            try
            {
                bool isposted = _FormTemplateRepo.CheckVoucherNoPosted(voucherno);
                if (isposted == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "POSTED", STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = voucherno });
                }


                string checkreferenced = _FormTemplateRepo.CheckVoucherNoReferenced(voucherno);
                if (checkreferenced!="" && checkreferenced != null && checkreferenced !="undefined")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "REFERENCED", STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = checkreferenced});
                }
                else {
                    var primarycolumn=string.Empty;
                    var fomdetails = _FormTemplateRepo.GetFormDetailSetup(formcode);
                    if (fomdetails.Count > 0)
                    {
                        primarycolumn = _FormTemplateRepo.GetPrimaryColumnByTableName(fomdetails[0].TABLE_NAME);
                    }
                    bool deleteres = _FormTemplateRepo.deletevouchernoFinance(fomdetails[0].TABLE_NAME, formcode, voucherno, primarycolumn);

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "DELETED", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                
                
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveFinancialFormData(FormDetails model)
        {
            _logErp.InfoInFile("Financial voucher Form data save method started=============");
            using(var financialTransaction = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    string primarydate = string.Empty, today = DateTime.Now.ToString("dd-MMM-yyyy"), 
                        todaystring =DateTime.Now.ToString("yyyyMMddHHmmss"), createddatestring = "TO_DATE(SYSDATE,'DD-MON-YYYY hh24:mi:ss')", 
                        newvoucherNo = string.Empty, manualno = string.Empty, VoucherDate = createddatestring, createdDateForEdit = string.Empty, 
                        createdByForEdit = string.Empty, voucherNoForEdit = string.Empty;

                    var sessionQuery = "SELECT MYSEQUENCE.NEXTVAL FROM DUAL";
                    string sessionId = this._objectEntity.SqlQuery<decimal>(sessionQuery).FirstOrDefault().ToString();

                    var defaultCol = "MODIFY_BY,MODIFY_DATE";
                    var sessionRowIDForedit = 0;
                    var msg = string.Empty;
                    var VoucherNumberGeneratedNo = string.Empty;

                    var voucherNumber = model.Order_No;
                    var primaryDateColumn = _FormTemplateRepo.GetPrimaryDateByTableName(model.Table_Name);
                    var primaryColName = _contravoucher.GetPrimaryColumnByTableName(model.Table_Name);


                    if (!string.IsNullOrEmpty(model.Master_COLUMN_VALUE) && model.Master_COLUMN_VALUE.Length > 0)
                    {
                        var masterValues = JsonConvert
                            .DeserializeObject<FinancialVoucherDetail>(model.Master_COLUMN_VALUE);

                        masterValues.SESSION_ROWID = sessionId;

                        model.Master_COLUMN_VALUE = JsonConvert.SerializeObject(masterValues);
                    }

                    if (!string.IsNullOrEmpty(model.Child_COLUMN_VALUE) && model.Child_COLUMN_VALUE.Length > 0)
                    {
                        var childValues = JsonConvert
                            .DeserializeObject<List<FinancialVoucherDetail>>(model.Child_COLUMN_VALUE);

                        foreach (var item in childValues)
                        {
                            item.SESSION_ROWID = sessionId;
                        }

                        model.Child_COLUMN_VALUE = JsonConvert.SerializeObject(childValues);
                    }

                    if (!string.IsNullOrEmpty(model.SUB_LEDGER_VALUE) && model.SUB_LEDGER_VALUE.Length > 0)
                    {
                        var subLedgers = JsonConvert
                            .DeserializeObject<List<FinancialSubLedger>>(model.SUB_LEDGER_VALUE);

                        foreach (var item in subLedgers)
                        {
                            item.SESSION_ROWID = sessionId;
                        }

                        model.SUB_LEDGER_VALUE = JsonConvert.SerializeObject(subLedgers);
                    }

                    if (!string.IsNullOrEmpty(model.VAT_VALUE) && model.VAT_VALUE.Length > 0)
                    {
                        var vatValues = JsonConvert
                            .DeserializeObject<List<FinancialVAT>>(model.VAT_VALUE);

                        foreach (var item in vatValues)
                        {
                            item.SESSION_ROWID = sessionId;
                        }

                        model.VAT_VALUE = JsonConvert.SerializeObject(vatValues);
                    }

                    if (!string.IsNullOrEmpty(model.BUDGET_TRANS_VALUE) && model.BUDGET_TRANS_VALUE.Length > 0)
                    {
                        var budgetTransactions = JsonConvert
                            .DeserializeObject<List<FinancialBudgetTransaction>>(model.BUDGET_TRANS_VALUE);

                        foreach (var item in budgetTransactions)
                        {
                            item.SESSION_ROWID = sessionId;
                        }

                        model.BUDGET_TRANS_VALUE = JsonConvert.SerializeObject(budgetTransactions);
                    }


                    if (!string.IsNullOrEmpty(model.TDS_VALUE) && model.TDS_VALUE.Length > 0)
                    {
                        var tdsValues = JsonConvert
                            .DeserializeObject<List<FinancialTDS>>(model.TDS_VALUE);

                        foreach (var item in tdsValues)
                        {
                            item.SESSION_ROWID = sessionId;
                        }

                        model.TDS_VALUE = JsonConvert.SerializeObject(tdsValues);
                    }

                    var masterColumn = _financialVoucherSaveService.MapMasterColumnWithValue(model.Master_COLUMN_VALUE);

                    var childColumn = _financialVoucherSaveService.MapChildColumnWithValue(model.Child_COLUMN_VALUE);

                    var childSubLedgerColumn = _financialVoucherSaveService.MapChildSubLedgerColumnWithValue(model.SUB_LEDGER_VALUE);

                    var vatValue = _financialVoucherSaveService.MapVaTColumnWithValue(model.VAT_VALUE);

                    var drTotal = _financialVoucherSaveService.MapDrTotalColumnValue(model.DR_TOTAL_VALUE);

                    var crTotal = _financialVoucherSaveService.MapCrTotalColumnValue(model.CR_TOTAL_VALUE);

                    var budgetTransaction = _financialVoucherSaveService.MapBudgetTransactionColumnValue(model.BUDGET_TRANS_VALUE);

                    var tdsValue = _financialVoucherSaveService.MapTdsColumnValue(model.TDS_VALUE);
                    var chargeTransaction = _financialVoucherSaveService.MapChargeTransactionColumnValue(model.charge_tran_value);

                    string currencyformat = string.IsNullOrEmpty(masterColumn.CURRENCY_CODE) ? "NRS" : masterColumn.CURRENCY_CODE;
                    string exchangerate = string.IsNullOrEmpty(masterColumn.EXCHANGE_RATE) ? "1" : masterColumn.EXCHANGE_RATE;
                    if (voucherNumber == "undefined")
                    {
                        string updateTransactionNo = _FormTemplateRepo.NewVoucherNo(this._workContext.CurrentUserinformation.company_code, model.Form_Code, DateTime.Now.ToString("dd-MMM-yyyy"), model.Table_Name);
                        VoucherNumberGeneratedNo = updateTransactionNo;
                        bool insertedToChild = false;
                        bool insertedToMaster = false;
                        bool insertedBudget = true;
                        bool insertedTds = true;
                        bool insertedVat = true;
                        bool insertedSubLedger = true;
                        bool insertedCharge = true;
                        
                        var commonValue = new CommonFieldsForFinanacialVoucher
                        {
                            FormCode = model.Form_Code,
                            TableName = model.Table_Name,
                            ExchangeRate = Convert.ToDecimal(exchangerate),
                            CurrencyFormat = currencyformat,
                            VoucherNumber = voucherNumber,
                            NewVoucherNumber = VoucherNumberGeneratedNo,
                            TempCode = model.TempCode,
                            DrTotal =drTotal
                        };                       
                        
                        if (childColumn.Count > 0)
                        {
                            insertedToChild = _financialVoucherSaveService.SaveChildColumnValue(childColumn, masterColumn, commonValue, _objectEntity);
                            if (!insertedToChild)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to save child column values");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to save child column values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }

                        if (insertedToChild)
                        {
                            insertedToMaster = _financialVoucherSaveService.SaveMasterColumnValue(masterColumn, commonValue, _objectEntity);
                            if (!insertedToMaster)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to save master column values");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to save master column values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                            primarydate = string.IsNullOrEmpty(masterColumn.VOUCHER_DATE) ? DateTime.Now.ToString("dd-MMM-yyyy") : masterColumn.VOUCHER_DATE;
                        }

                        if (budgetTransaction.Count > 0)
                        {
                            insertedBudget = _financialVoucherSaveService.SaveBudgetTransactionColumnValue(budgetTransaction, commonValue, _objectEntity);
                            if (!insertedBudget)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to save budget transaction values - no data was inserted");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to save budget transaction values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }

                        if (tdsValue.Count > 0)
                        {
                            insertedTds = _financialVoucherSaveService.SaveFinancialTDSColumnValue(tdsValue, commonValue, _objectEntity);
                            if (!insertedTds)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to save TDS values - no data was inserted");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to save TDS values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }

                        if (vatValue.Count > 0)
                        {
                            insertedVat = _financialVoucherSaveService.SaveFinancialVATValue(vatValue, commonValue, _objectEntity);
                            if (!insertedVat)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to save VAT values - no data was inserted");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to save VAT values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }

                        if (childSubLedgerColumn.Count > 0)
                        {
                            insertedSubLedger = _financialVoucherSaveService.SaveFinancialSubLedgerColumnValue(childSubLedgerColumn, commonValue, _objectEntity);
                            if (!insertedSubLedger)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to save sub-ledger values - no data was inserted");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to save sub-ledger values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }
                        
                        if (chargeTransaction != null)
                        {
                            insertedCharge = _financialVoucherSaveService.SaveChargeTransaction(chargeTransaction, commonValue, _objectEntity);
                            if (!insertedCharge)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to save charge transaction values");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to save charge transaction values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }
                        

                        #region CLEAR CACHE
                        List<string> keystart = new List<string>();
                        keystart.Add("GetAllMenuItems");
                        keystart.Add("GetAllSalesOrderDetails");
                        keystart.Add("GetSalesOrderDetailFormDetailByFormCodeAndOrderNo");
                        keystart.Add("GetAllOrederNoByFlter");
                        keystart.Add("GetSubMenuList");
                        keystart.Add("GetSubMenuDetailList");
                        List<string> Record = new List<string>();
                        Record = this._cacheManager.GetAllKeys();
                        this._cacheManager.RemoveCacheByKey(keystart, Record);
                        #endregion


                        // Verify all operations succeeded before committing
                        if (insertedToChild && insertedToMaster && insertedBudget && insertedTds && insertedVat && insertedSubLedger && insertedCharge)
                        {
                            if (model.Save_Flag == "0")
                            {
                                financialTransaction.Commit();
                                msg = "INSERTED";
                            }
                            else if (model.Save_Flag == "3")
                            {
                                financialTransaction.Commit();
                                msg = "SAVEANDPRINT";
                            }
                            else if (model.Save_Flag == "4")
                            {
                                financialTransaction.Commit();
                                msg = "UPDATEDANDPRINT";
                            }
                            else
                            {
                                financialTransaction.Commit();
                                msg = "INSERTEDANDCONTINUE";
                            }
                        }
                        else
                        {
                            financialTransaction.Rollback();
                            _logErp.ErrorInDB("One or more save operations failed. Transaction rolled back.");
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to save financial data. Transaction rolled back.", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                        }

                    }
                    else
                    {
                        bool updatedChild = false;
                        bool updatedMaster = false;
                        bool updatedBudget = true;
                        bool updatedSubLedger = true;
                        bool updatedVat = true;
                        bool updatedTds = true;
                        bool updatedCharge = true;
                        
                        var commonUpdateValue = new CommonFieldsForFinanacialVoucher
                        {
                            FormCode = model.Form_Code,
                            TableName = model.Table_Name,
                            ExchangeRate = Convert.ToDecimal(exchangerate),
                            CurrencyFormat = currencyformat,
                            VoucherNumber = voucherNumber,
                            NewVoucherNumber = newvoucherNo,
                            TempCode = model.TempCode,
                            DrTotal = drTotal,
                            PrimaryColumn = primaryColName,
                            PrimaryDateColumn = primaryDateColumn,
                            ManualNumber= masterColumn.MANUAL_NO
                        };

                        var defaultData =  _financialVoucherSaveService.GetMasterTransactionByVoucherNo(voucherNumber);
                        foreach (var def in defaultData)
                        {
                            voucherNoForEdit = def.VOUCHER_NO.ToString();
                            createdDateForEdit = "TO_DATE('" + def.CREATED_DATE.ToString() + "', 'DD-MM-YYYY hh12:mi:ss')";

                            createdByForEdit = def.CREATED_BY.ToString().ToUpper();
                            sessionRowIDForedit = Convert.ToInt32(def.SESSION_ROWID);
                        }
                        commonUpdateValue.VoucherDate = createdDateForEdit;
                        _financialVoucherSaveService.DeleteChildTransaction(commonUpdateValue,_objectEntity);
                        updatedChild = _financialVoucherSaveService.SaveChildColumnValue(childColumn, masterColumn, commonUpdateValue,_objectEntity);
                        
                        if (!updatedChild)
                        {
                            financialTransaction.Rollback();
                            _logErp.ErrorInDB("Failed to update child column values");
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to update child column values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                        }

                        if (updatedChild)
                        {
                            updatedMaster= _financialVoucherSaveService.UpdateMasterTransaction(commonUpdateValue, masterColumn,_objectEntity);
                            if (!updatedMaster)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to update master transaction");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to update master transaction", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                            primarydate = string.IsNullOrEmpty(masterColumn.VOUCHER_DATE) ? DateTime.Now.ToString("dd-MMM-yyyy") : masterColumn.VOUCHER_DATE;
                        }

                        if(budgetTransaction != null && budgetTransaction.Count > 0)
                        {
                            _financialVoucherSaveService.DeleteBudgetTransaction(commonUpdateValue.VoucherNumber,_objectEntity);
                            updatedBudget = _financialVoucherSaveService.SaveBudgetTransactionColumnValue(budgetTransaction, commonUpdateValue,_objectEntity);
                            if (!updatedBudget)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to update budget transaction values - no data was inserted");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to update budget transaction values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }

                        if(childSubLedgerColumn !=null && childSubLedgerColumn.Count > 0)
                        {
                            _financialVoucherSaveService.DeleteSubLedgerTransaction(commonUpdateValue.VoucherNumber,_objectEntity);
                            updatedSubLedger = _financialVoucherSaveService.SaveFinancialSubLedgerColumnValue(childSubLedgerColumn, commonUpdateValue,_objectEntity);
                            if (!updatedSubLedger)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to update sub-ledger values - no data was inserted");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to update sub-ledger values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }
                        
                        if (vatValue != null && vatValue.Count > 0)
                        {
                            _financialVoucherSaveService.DeleteVatTransaction(commonUpdateValue.VoucherNumber,_objectEntity);
                            updatedVat = _financialVoucherSaveService.SaveFinancialVATValue(vatValue, commonUpdateValue,_objectEntity);
                            if (!updatedVat)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to update VAT values - no data was inserted");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to update VAT values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }
                        
                        if (tdsValue != null && tdsValue.Count > 0)
                        {
                            _financialVoucherSaveService.DeleteTDSTransaction(commonUpdateValue.VoucherNumber,_objectEntity);
                            updatedTds = _financialVoucherSaveService.SaveFinancialTDSColumnValue(tdsValue, commonUpdateValue,_objectEntity);
                            if (!updatedTds)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to update TDS values - no data was inserted");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to update TDS values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }
                        
                        if (chargeTransaction != null)
                        {
                            _financialVoucherSaveService.DeleteChargeTransaction(commonUpdateValue.VoucherNumber, _objectEntity);
                            updatedCharge = _financialVoucherSaveService.SaveChargeTransaction(chargeTransaction, commonUpdateValue, _objectEntity);
                            if (!updatedCharge)
                            {
                                financialTransaction.Rollback();
                                _logErp.ErrorInDB("Failed to update charge transaction values");
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to update charge transaction values", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                            }
                        }
                        #region CLEAR CACHE
                        List<string> keystart = new List<string>();
                        keystart.Add("GetAllMenuItems");
                        keystart.Add("GetAllSalesOrderDetails");
                        keystart.Add("GetSalesOrderDetailFormDetailByFormCodeAndOrderNo");
                        keystart.Add("GetAllOrederNoByFlter");
                        keystart.Add("GetSubMenuList");
                        keystart.Add("GetSubMenuDetailList");
                        List<string> Record = new List<string>();
                        Record = this._cacheManager.GetAllKeys();
                        this._cacheManager.RemoveCacheByKey(keystart, Record);
                        #endregion
                        // Verify all update operations succeeded before committing
                        if (updatedChild && updatedMaster && updatedBudget && updatedSubLedger && updatedVat && updatedTds && updatedCharge)
                        {
                            financialTransaction.Commit();
                            if (model.Save_Flag == "4")
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATEDANDPRINT", STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = voucherNumber, SessionNo = sessionRowIDForedit, VoucherDate = primarydate, FormCode = model.Form_Code });
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "UPDATED", STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = voucherNumber, SessionNo = sessionRowIDForedit, VoucherDate = primarydate, FormCode = model.Form_Code });
                            }
                        }
                        else
                        {
                            financialTransaction.Rollback();
                            _logErp.ErrorInDB("One or more update operations failed. Transaction rolled back.");
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to update financial data. Transaction rolled back.", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = msg, STATUS_CODE = (int)HttpStatusCode.OK, VoucherNo = VoucherNumberGeneratedNo, SessionNo =newvoucherNo, VoucherDate = primarydate, FormCode = model.Form_Code });
                }
                catch(Exception ex)
                {
                    _logErp.ErrorInDB("Error while saving financial voucher: " + ex.StackTrace);
                    financialTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
        private static string NewMethod(StringBuilder masterchildvaluesbuilder)
        {
            return masterchildvaluesbuilder.ToString().TrimEnd(',');
        }
        public IHttpActionResult GetAllOptionsByFilter(string filter)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
            }
        }
        [HttpGet]
        public List<FinancialSubLedger> GetDataForSubledgerModal(string VOUCHER_NO, string accCode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FinancialSubLedger>();
            //if (this._cacheManager.IsSet($"GetDataForSubledgerModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}_{accCode}"))
            //{
            //    var data = _cacheManager.Get<List<FinancialSubLedger>>($"GetDataForSubledgerModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}_{accCode}");
            //    response = data;
            //}
            //else
            //{
            //    var financialSubLedgerList = this._contravoucher.Getsubledgerdetail(VOUCHER_NO, accCode);
            //    this._cacheManager.Set($"GetDataForSubledgerModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}_{accCode}", financialSubLedgerList, 20);
            //    response = financialSubLedgerList;
            //}
             response = this._contravoucher.Getsubledgerdetail(VOUCHER_NO, accCode);
            return response;
            //  return this._contravoucher.Getsubledgerdetail(VOUCHER_NO, accCode);
        }
        [HttpGet]
        public List<FinancialBudgetTransaction> GetDataForBudgetModal(string VOUCHER_NO, string accCode)
        {
            var userid = _workContext.CurrentUserinformation.User_id;
            var company_code = _workContext.CurrentUserinformation.company_code;
            var branch_code = _workContext.CurrentUserinformation.branch_code;
            var response = new List<FinancialBudgetTransaction>();
            //if (this._cacheManager.IsSet($"GetDataForBudgetModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}_{accCode}"))
            //{
            //    var data = _cacheManager.Get<List<FinancialBudgetTransaction>>($"GetDataForBudgetModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}_{accCode}");
            //    response = data;
            //}
            //else
            //{
            //    var financialBudgetTransactionList = this._contravoucher.Getbudgetdetail(VOUCHER_NO, accCode);
            //    this._cacheManager.Set($"GetDataForBudgetModal_{userid}_{company_code}_{branch_code}_{VOUCHER_NO}_{accCode}", financialBudgetTransactionList, 20);
            //    response = financialBudgetTransactionList;
            //}
            response = this._contravoucher.Getbudgetdetail(VOUCHER_NO, accCode);
            return response;
            //return this._contravoucher.Getbudgetdetail(VOUCHER_NO, accCode);
        }
        [HttpGet]
        public List<FinancialTDS> GetDataForTDSModal(string VOUCHER_NO, string accCode)
        {

        var data= this._contravoucher.Gettdsdetail(VOUCHER_NO, accCode);
            return data;
        }
        [HttpGet]
        public List<FinancialVAT> GetDataForVATModal(string VOUCHER_NO, string accCode)
        {

            var data = this._contravoucher.Getvatdetail(VOUCHER_NO, accCode);
            return data;
        }
        [HttpGet]
        public List<DocumentTemplateMenu> DocumentList()
        {
            var data = this._contravoucher.DocumentList();
            return data;
        }
        [HttpGet]
        public List<PurExpSheet> DocumentListDropDown(string tableName, string formCode, string fromdate, string todate, string manualNo = null, string ppNo = null, string supplierCode = null)
        {
            var data = this._contravoucher.DocumentListDropDown(tableName, formCode,fromdate, todate, manualNo, ppNo, supplierCode);
            return data;
        }
        [HttpGet]
        public List<PurExpSheet> InvoiceDetails(string invoiceNo)
        {
            var data = this._contravoucher.InvoiceDetails(invoiceNo);
            return data;
        }
        [HttpGet]
        public List<PurExpSheet> InvoiceDetailsForGrid(string invoiceNo)
        {
            var data = this._contravoucher.InvoiceDetailsForGrid(invoiceNo);
            return data;
        }
        [HttpGet]
        public List<IPChargeEdesc> GetIpChargesEdesc(string formCode)
        {
            var data = this._contravoucher.GetIPChargeEdesc(formCode);
            return data;
        }
        [HttpGet]
        public List<IPChargeCode> GetIpChargeCode(string formCode)
        {
            var data = this._contravoucher.GetIPChargeCode(formCode);
            return data;
        }
        [HttpGet]
        public List<IPChargeDtls> GetIpChargeCodedetls(string formCode)
        {
            var data = this._contravoucher.GetIPChargedetls(formCode);
            return data;
        }
        public string GetPurchaseExpensesFlag(string formCode)
        {
            var data = this._contravoucher.GetPurchaseExpensesFlag(formCode);
            return data;

        }
        [HttpGet]
        public List<PurExpSheet> GetChargeExpList(string voucherNo,string accCode = null,string accEdesc = null)
       {
            var data = this._contravoucher.GetChargeExpList(voucherNo, accCode, accEdesc);
            return data;
        }
        public List<string> GetVoucherNoFrmCharge(string invoiceNo)
        {
            var data = this._contravoucher.GetVoucherNoFrmCharge(invoiceNo);
            return data;
        }
        public List<PurExpSheet> GetChargeDtlFrmInvoice(string invoiceNo)
        {
            var data = this._contravoucher.GetChargeDtlFrmInvoice(invoiceNo);
            return data;
        }
        public List<PurExpSheet> GetChargeDtlFrmVoucherNo(string orderNo)
        {
            var data = this._contravoucher.GetChargeDtlFrmVoucherNo(orderNo);
            return data;
        }
        [HttpGet]
        public List<PurExpSheet> BindAccountCode(string formCode, string itemEdesc)
        {
            var data = this._contravoucher.BindAccountCode(formCode, itemEdesc);
            return data;
        }
    }
}             
