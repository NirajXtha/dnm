using NeoERP.DocumentTemplate.Service.Interface.CustomForm;
using NeoERP.DocumentTemplate.Service.Models.CustomForm;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.DocumentTemplate.Controllers.Api
{
    public class CustomFormApiController : ApiController
    {
        private IPostDataCheque _postDataCheque;

        public CustomFormApiController(IPostDataCheque postDataCheque)
        {
            _postDataCheque = postDataCheque;
        }

        #region POST DATE CHEQUE

        [HttpGet]
        public List<CustomerModal> GetCustomerForPostedDate()
        {
            try
            {
                var allCustomer = _postDataCheque.GetCustomerSubLedger();
                return allCustomer;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<PartyTypeCodeModal> GetAllPartyType()
        {
            try
            {
                var partyType = _postDataCheque.GetAllPartyType();
                return partyType;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<CustomerModal> GetAllCustomer()
        {
            try
            {
                var partyType = _postDataCheque.GetAllCustomer();
                return partyType;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<SupplierModel> GetAllSupplier()
        {
            try
            {
                var supplier = _postDataCheque.GetAllSupplier();
                return supplier;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<PartyTypeCodeModal> GetAllDealerType()
        {
            try
            {
                var partyType = _postDataCheque.GetAllDealerType();
                return partyType;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }
        }

        [HttpPost]
        public List<PDCFormSaveModal> GetAllPDCFormDetail(PDCFilter filter)
        {
            try
            {
                var detailData = _postDataCheque.GetAllPDCFormDetail(filter);
                return detailData;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<string> GenerateNewReceipt()
        {
            try
            {
                var newReceipt = _postDataCheque.GenerateNewReceipt();
                return newReceipt;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public List<string> GenerateNewPaymentNo()
        {
            try
            {
                var newReceipt = _postDataCheque.GenerateNewPaymentNo();
                return newReceipt;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<string> GenerateNewBGVoucher()
        {
            try
            {
                var newReceipt = _postDataCheque.GenerateNewBGVoucher();
                return newReceipt;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<int> GenerateReminderPriorDays()
        {
            try
            {
                var days = Enumerable.Range(1, 90).ToList();
                return days;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public List<PDCFormSaveModal> SearchPDCDetail(PDCFilter filter)
        {
            try
            {
                var searchResult = _postDataCheque.SearchAllPDCDetail(filter);
                return searchResult;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public string SaveNewPDCForm(PDCFormSaveModal modal)
        {
            try
            {
                var savedResponse = _postDataCheque.SaveNewPDCForm(modal);
                return savedResponse;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public PDCVoucherResponse GeneratePdcOdcVoucher(PDCFormSaveModal modal)
        {
            try
            {
                var voucher = _postDataCheque.GeneratePdcOdcVoucher(modal);
                return voucher;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public PDCFormSaveModal EditPDCFormDetail(string pdcId,string pdcStatus)
        {
            try
            {
                var editedResponse = _postDataCheque.EditPDCFormDetail(pdcId,pdcStatus);
                return editedResponse;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public string DeletePDCFormDetail(string pdcId)
        {
            try
            {
                var delResponse = _postDataCheque.DeletePDCFormDetail(pdcId);
                return delResponse;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region COLUMN SETTING 
        [HttpPost]
        public string BulkInsertSettings(List<FormDetailEditModal> models)
        {
            try
            {
                return "";
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public string CreateAllFormDetail(List<FormDetailEditModal> models)
        {
            try
            {
                //JObject obj = JObject.Parse(json);
                var createResponse = _postDataCheque.SaveAllFormDetail(models);
                //var createResponse = string.Empty;
                return createResponse;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<FormDetailModal> GetAllFormDetail()
        {
            try
            {
                var formDetail = _postDataCheque.GetAllFormDetail();
                return formDetail;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public List<Erp_TableName> GetAllTableDetail()
        {
            try
            {
                var tableDetail = _postDataCheque.GetAllTableName();
                return tableDetail;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public List<FormDetailEditModal> GetAllFormDetailToEdit(string formCode = "", string tableName = "")
        {
            try
            {
                var detailToEdit = _postDataCheque.GetAllFormDetailToEdit(formCode, tableName);
                return detailToEdit;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public string EditAllFormDetail(List<FormDetailEditModal> models)
        {
            try
            {
                var saveResponse = _postDataCheque.SaveAllFormDetail(models);
                return saveResponse;
                //var editResponse = _postDataCheque.EditAllFormDetail(modal);
                //return editResponse;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<ColumnNameAsDDL> GetColumnNameForDDL()
        {
            try
            {
                var columnList = _postDataCheque.GetColumnNameForDDL();
                return columnList;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        public List<TableNameAsDDL> GetTableNameForDDL()
        {
            try
            {
                var tableList = _postDataCheque.GetTableNameForDDL();
                return tableList;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public bool DeleteAllFormDetail(FormDetailEditModal modal)
        {
            try
            {
                var deleteResponse = _postDataCheque.DeleteAllFormDetail(modal);
                return deleteResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region CASH BANK SETUP

        [HttpGet]
        public List<CashBankSelectedAccountModal> GetSelectedAccountGrid()
        {
            try
            {
                var selectedAccount = _postDataCheque.GetSelectedAccountGrid();
                //selectedAccount[0].ASSOCIATED_ACCOUNTS = "Test";
                //selectedAccount[0].ASSOCIATED_ACCOUNTS_ID = "01";
                return selectedAccount;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<CashBankRootDetailModal> GetCashBankRootDetail()
        {
            try
            {
                var cashBankRoot = _postDataCheque.GetCashBankRootDetail();
                return cashBankRoot;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public List<CashBankAccountDetail> GetCashBankAccountDetail(string cb_code)
        {
            try
            {
                var accDetailData = _postDataCheque.GetCashBankAccountDetail(cb_code);
                return accDetailData;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public string GenerateCBID()
        {
            try
            {
                var cbId = _postDataCheque.GenerateCBId();
                return cbId;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public string SaveCashBankAccountDetail(CashBankAccountDetailSaveModal modal)
        {
            try
            {
                var saveResponse = _postDataCheque.SaveCashBankAccountDetail(modal);
                return saveResponse;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        //[HttpPost]
        //public string UpdateCash
        [HttpGet]
        public HttpResponseMessage DeleteCashBankSetup(string cbCode)
        {
            try
            {
                var result = _postDataCheque.DeleteCashBankDetail(cbCode);

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }



        #endregion


        #region BANK RECONCILATION

        [HttpGet]
        public List<BankDetailForReconcilation> GetBankDetailForFilter()
        {
            try
            {
                var bankDetail = _postDataCheque.GetBankDetailForReconcilation();
                return bankDetail;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public List<ReconcilationGridModel> GetDataForReconcilation()
        {
            try
            {
                var reconsileDetail = _postDataCheque.GetDataForReconcilation();
                return reconsileDetail;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public string UpdateBankReconcilation(string voucherNo)
        {
            try
            {
                var brUpInfo = _postDataCheque.UpdateBankReconcilation(voucherNo);
                return brUpInfo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion

        #region PROJECT MANAGEMENT

        [HttpGet]
        public List<BankGuranteeModal> GetProjecManagementList()
        {
            try
            {
                var guranteeList = _postDataCheque.GetBankGuranteeList();
                return guranteeList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region BANK GURANTEE

        [HttpGet]
        public List<BankGuranteeModal> GetBankGuranteeList()
        {
            try
            {
                var guranteeList = _postDataCheque.GetBankGuranteeList();
                return guranteeList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public string SaveNewBankGuaranteeForm(BankGuranteeModal modal)
        {
            try
            {
                var savedResponse = _postDataCheque.SaveBankGurantee(modal);
                return savedResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        #endregion
    }
}
