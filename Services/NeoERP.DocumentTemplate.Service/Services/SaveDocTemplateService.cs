using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoERP.DocumentTemplate.Service.Services
{
    public class SaveDocTemplateService : ISaveDocTemplate
    {

        private static IWorkContext _workContext;
        private NeoErpCoreEntity _objectEntity;


        public SaveDocTemplateService(IWorkContext workContext)
        {

            _workContext = workContext;
            _objectEntity = new NeoErpCoreEntity();
        }

        public Dictionary<string, string> MapMasterColumnToDic(string masterColumn)
        {
            try
            {
                var masterCol = JsonConvert.DeserializeObject<Dictionary<string, string>>(masterColumn);
                return masterCol;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public List<Dictionary<string, object>> MapChildColumnToDict(string childColumn)
        {
            try
            {
                var childCol = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(childColumn);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<SalesOrderDetail> GetMasterTransactionByOrderNo(string orderNumber)
        {
            try
            {
                var getPrevDataQuery = $@"SELECT VOUCHER_NO,SESSION_ROWID, CREATED_BY, CREATED_DATE FROM MASTER_TRANSACTION WHERE VOUCHER_NO= '{orderNumber}'";
                var defaultData = this._objectEntity.SqlQuery<SalesOrderDetail>(getPrevDataQuery).ToList();
                return defaultData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
        public List<REF_MODEL_DEFAULT> MapRefrenceModel(string ReFModel)
        {
            try
            {
                var childCol = JsonConvert.DeserializeObject<List<REF_MODEL_DEFAULT>>(ReFModel);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<SalesOrderDetail> MapOrderChildColumnWithValue(string childColumn)
        {
            try
            {
                var childCol = JsonConvert.DeserializeObject<List<SalesOrderDetail>>(childColumn);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<SalesBlanketDetail> MapBlanketChildColumnWithValue(string childColumn)
        {
            try
            {
                var childCol = JsonConvert.DeserializeObject<List<SalesBlanketDetail>>(childColumn);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<SalesChalanDetail> MapChalanChildColumnWithValue(string childColumn)
        {
            try
            {
                // var dynamicObject = JsonConvert.DeserializeObject(childColumn);
                var childCol = JsonConvert.DeserializeObject<List<SalesChalanDetail>>(childColumn);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SalesInvoiceDetail> MapInvoiceChildColumnWithValue(string childColumn)
        {
            try
            {
                // var dynamicObject = JsonConvert.DeserializeObject(childColumn);
                var childCol = JsonConvert.DeserializeObject<List<SalesInvoiceDetail>>(childColumn);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SalesInvoiceJewelleryDetail> MapInvoiceJewelleryChildColumnWithValue(string childColumn)
        {
            try
            {
                // var dynamicObject = JsonConvert.DeserializeObject(childColumn);
                var childCol = JsonConvert.DeserializeObject<List<SalesInvoiceJewelleryDetail>>(childColumn);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SalesReturnDetail> MapReturnChildColumnWithValue(string childColumn)
        {
            try
            {
                // var dynamicObject = JsonConvert.DeserializeObject(childColumn);
                var childCol = JsonConvert.DeserializeObject<List<SalesReturnDetail>>(childColumn);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SalesReturnJewelleryDetail> MapReturnJewelleryChildColumnWithValue(string childColumn)
        {
            try
            {
                // var dynamicObject = JsonConvert.DeserializeObject(childColumn);
                var childCol = JsonConvert.DeserializeObject<List<SalesReturnJewelleryDetail>>(childColumn);
                return childCol;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<CustomOrderColumn> MapCustomOrderColumnWithValue(string custom_col_val)
        {
            try
            {
                // var customColVal = JsonConvert.DeserializeObject<CustomOrderColumn>(custom_col_val.Replace(' ','_'));
                var customCol = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(custom_col_val);
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
        public List<CustomBlanketColumn> MapCustomBlanketColumnWithValue(string custom_col_val)
        {
            try
            {
                // var customColVal = JsonConvert.DeserializeObject<CustomOrderColumn>(custom_col_val.Replace(' ','_'));
                var customCol = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(custom_col_val);
                // var customColDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(custom_col_val);
                CustomBlanketColumn customOrderCol = null;
                var customOrderColList = new List<CustomBlanketColumn>();
                foreach (var cc in customCol)
                {
                    customOrderCol = new CustomBlanketColumn
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

        public SalesOrderDetail MapSalesOrderMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<SalesOrderDetail>(masterColumn);
                //var masterColValDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(masterColumn);
                return masterColVal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SalesBlanketDetail MapSalesBlanketMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<SalesBlanketDetail>(masterColumn);
                //var masterColValDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(masterColumn);
                return masterColVal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SalesReturnDetail MapSalesReturnMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<SalesReturnDetail>(masterColumn);
                //var masterColValDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(masterColumn);
                return masterColVal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SalesReturnJewelleryDetail MapSalesReturnJewelleryMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<SalesReturnJewelleryDetail>(masterColumn);
                //var masterColValDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(masterColumn);
                return masterColVal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SalesChalanDetail MapSalesChalanMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<SalesChalanDetail>(masterColumn);
                //var masterColValDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(masterColumn);
                return masterColVal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SalesInvoiceDetail MapSalesInvoiceMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<SalesInvoiceDetail>(masterColumn);
                //var masterColValDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(masterColumn);
                return masterColVal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public SalesInvoiceJewelleryDetail MapSalesInvoiceJewelleryMasterColumnWithValue(string masterColumn, string primaryDateColumn, string primaryColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<SalesInvoiceJewelleryDetail>(masterColumn);
                //var masterColValDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(masterColumn);
                return masterColVal;

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
        public string GetTemplateNo()
        {
            try
            {
                var templateNo = string.Empty;
                var maxTemplateQry = $@"SELECT MAX(TO_NUMBER(TEMPLATE_NO)+1) MAX_NO FROM FORM_TEMPLATE_SETUP";
                templateNo = _objectEntity.SqlQuery<int>(maxTemplateQry).FirstOrDefault().ToString();
                return templateNo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public string AddToFormTemplateSetup(FormDetails model, string templateNo)
        {
            try
            {
                var templateInsertQry = $@"INSERT INTO FORM_TEMPLATE_SETUP(TEMPLATE_NO,TEMPLATE_EDESC,TEMPLATE_NDESC,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,ASSIGNEE,ASSIGNED_DATE)
                                        VALUES('{templateNo}','{model.FORM_TEMPLATE.TEMPLATE_EDESC}','{model.FORM_TEMPLATE.TEMPLATE_NDESC}','{model.Form_Code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.User_id}',SYSDATE,'N','{model.FORM_TEMPLATE.TEMPLATE_ASSIGNEE}',SYSDATE)";
                var result = this._objectEntity.ExecuteSqlCommand(templateInsertQry);

                return "Successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public string UpdateFormTemlateSetup(FormDetails model)
        {
            try
            {
                var templateInsertQry = $@"UPDATE FORM_TEMPLATE_SETUP SET TEMPLATE_EDESC='{model.FORM_TEMPLATE.TEMPLATE_EDESC}',TEMPLATE_NDESC='{model.FORM_TEMPLATE.TEMPLATE_NDESC}',MODIFY_BY='{_workContext.CurrentUserinformation.User_id}',MODIFY_DATE=SYSDATE,ASSIGNEE='{model.FORM_TEMPLATE.TEMPLATE_ASSIGNEE}',ASSIGNED_DATE= SYSDATE WHERE TEMPLATE_NO='{model.FORM_TEMPLATE.TEMPLATE_NO}'";
                var result = this._objectEntity.ExecuteSqlCommand(templateInsertQry);

                return "Successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public DraftFormModel GetFormTemplateDetailsByTemplateNo(string templateNo)
        {
            try
            {
                var Qry = $@"SELECT * FROM FORM_TEMPLATE_DETAIL_SETUP WHERE TEMPLATE_NO='{templateNo}'";
                var cResult = this._objectEntity.SqlQuery<DraftFormModel>(Qry).FirstOrDefault();
                return cResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public string AddMasterColumnFormSetup(Dictionary<string, string> model, DraftFormModel draftFormModel)
        {
            try
            {
                foreach (var m in model)
                {
                    if (m.Value != null || m.Value != "")
                    {
                        var insertDraftQry = $@"INSERT INTO FORM_TEMPLATE_DETAIL_SETUP (TEMPLATE_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,COLUMN_NAME,COLUMN_VALUE,TABLE_NAME,DELETED_FLAG,SYN_ROWID ,CREATED_BY, CREATED_DATE)
                                               VALUES('{draftFormModel.TEMPLATE_NO}','{draftFormModel.FORM_CODE}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','0','{m.Key}','{m.Value}','{draftFormModel.TABLE_NAME}','N','','{draftFormModel.CREATED_BY}',SYSDATE)";
                        this._objectEntity.ExecuteSqlCommand(insertDraftQry);
                    }
                }
                return "Successfulll";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }


        public string AddChildColumnFormSetup(List<Dictionary<string, object>> model, DraftFormModel childColumnValue)
        {
            try
            {
                foreach (var m in model)
                {
                    foreach (var mc in m)
                    {
                        var insertDraftQry = $@"INSERT INTO FORM_TEMPLATE_DETAIL_SETUP (TEMPLATE_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,COLUMN_NAME,COLUMN_VALUE,TABLE_NAME,DELETED_FLAG,SYN_ROWID,CREATED_BY, CREATED_DATE)
                                                        VALUES('{childColumnValue.TEMPLATE_NO}','{childColumnValue.FORM_CODE}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{childColumnValue.SERIAL_NO}','{mc.Key}','{mc.Value}','{childColumnValue.TABLE_NAME}','N','','{childColumnValue.CREATED_BY}',SYSDATE)";
                        this._objectEntity.ExecuteSqlCommand(insertDraftQry);
                    }
                }
                //foreach(var cc in childColumnValue)
                //{
                //    var insertDraftQry = $@"INSERT INTO FORM_TEMPLATE_DETAIL_SETUP (TEMPLATE_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,COLUMN_NAME,COLUMN_VALUE,TABLE_NAME,DELETED_FLAG,SYN_ROWID,CREATED_BY, CREATED_DATE)
                //                                        VALUES('{templateNo}','{model.Form_Code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{cc.SERIAL_NO}','{dataname}','{datavalue}','{model.Table_Name}','N','',{defaultVal})";
                //    this._objectEntity.ExecuteSqlCommand(insertDraftQry);
                //}

                return "Done";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public void DeleteFromFormTemplateSetupByTemplateNo(string templateNo)
        {
            try
            {
                var deleteQry = $@"DELETE FORM_TEMPLATE_DETAIL_SETUP WHERE TEMPLATE_NO='{templateNo}'";
                this._objectEntity.ExecuteSqlCommand(deleteQry);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
    }
}