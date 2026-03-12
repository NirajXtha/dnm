using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace NeoERP.DocumentTemplate.Service.Repository
{
    public class InventoryVoucherRepo : IInventoryVoucher
    {
        private NeoErpCoreEntity _coreEntity;
        IDbContext _dbContext;
        IWorkContext _workContext;
        public InventoryVoucherRepo(IDbContext dbContext, IWorkContext workContext)
        {
            _dbContext = dbContext;
            _workContext = workContext;
            _coreEntity = new NeoErpCoreEntity();
           // _coreEntity = NeoErpCoreEntity.CreateForSelectedDatabase();
        }
        public int? GetTotalVoucher(string form_code, string table_name)
        {

            string Query = $@"SELECT COUNT(DISTINCT INVOICE_NO) AS TOTAL FROM {table_name} WHERE FORM_CODE = '{form_code}' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' ";


            int? entity = this._dbContext.SqlQuery<int>(Query).FirstOrDefault();
            return entity;
        }
        public List<FinancialBudgetTransaction> Getbudgetdetail(string voucherno)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"select ACC_CODE, BUDGET_FLAG, BUDGET_CODE,BUDGET_AMOUNT as QUANTITY,PARTICULARS as NARRATION,SERIAL_NO  from budget_transaction where REFERENCE_NO = '{voucherno}'";
            var entity = this._dbContext.SqlQuery<FinancialBudgetTransaction>(Query).ToList();
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
        public string InsertInventoryImage(DocumentTransaction documentdetail)
        {
            var insertitem = $@"INSERT INTO DOCUMENT_TRANSACTION(VOUCHER_NO,VOUCHER_DATE,SERIAL_NO,FORM_CODE,DOCUMENT_NAME,DOCUMENT_FILE_NAME,COMPANY_CODE,BRANCH_CODE,                           CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID,SYN_ROWID)VALUES('{documentdetail.VOUCHER_NO}',TO_DATE('{documentdetail.VOUCHER_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'), '{documentdetail.SERIAL_NO}','{documentdetail.FORM_CODE}','{documentdetail.DOCUMENT_FILE_NAME}', '{documentdetail.DOCUMENT_NAME}','{_workContext.CurrentUserinformation.company_code}', '{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.User_id}',SYSDATE,'{'N'}','','')";
            var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);
            return null;
        }

        #region MAPPING COLUMN VALUE TO OBJECT
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
        public decimal MapDrTotalColumnValue(string drValue)
        {
            try
            {
                var drColValue = JsonConvert.DeserializeObject<decimal>(drValue);
                return drColValue;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public decimal MapCrTotalColumnValue(string crValue)
        {
            try
            {
                var crColValue = JsonConvert.DeserializeObject<decimal>(crValue);
                return crColValue;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        #endregion
        #region save column data
        public bool SaveChildColumnValue(List<Inventory> childColumnValue, Inventory masterColumnValue, CommonFieldsForInventory commonValue, FormDetails model, string primarydatecolumn, string primarycolname, out int session_row_id_out_param, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                var companyCode = _workContext.CurrentUserinformation.company_code;

                int serialno = 1;
                bool insertedToChild = false;
                Inventory inventoryChildDetails = new Inventory();

                string trackingNoQuery = $@"SELECT get_next_tracking_no FROM dual";
                var tracking_no = this._dbContext.SqlQuery<string>(trackingNoQuery).FirstOrDefault();

                string SESSION_ROWIDQuery = $@"SELECT MYSEQUENCE.NEXTVAL FROM DUAL";
                var session_row_id = this._dbContext.SqlQuery<int>(SESSION_ROWIDQuery).FirstOrDefault();
                session_row_id_out_param = session_row_id;  // out parameter assign

                var BATCH_NO = string.IsNullOrEmpty(masterColumnValue.BATCH_NO) ? "" : masterColumnValue.BATCH_NO;

                if (commonValue.TableName.ToUpper() == "IP_PRODUCTION_MRR")
                {
                    //if (childColumnValue.Any(x => string.IsNullOrEmpty(x.TO_LOCATION_CODE)))
                    //{
                    //    throw new InvalidOperationException($"To Location not define! Please select to location");
                    //}

                    string trackingNoQueryMRR = $@"SELECT get_next_tracking_no_mrr FROM dual";
                    tracking_no = this._dbContext.SqlQuery<string>(trackingNoQueryMRR).FirstOrDefault();
                }

                if ((commonValue.TableName.ToUpper() == "IP_PRODUCTION_ISSUE" || commonValue.TableName.ToUpper() == "IP_PRODUCTION_MRR") && (!String.IsNullOrEmpty(masterColumnValue.REFERENCE_NO) && !String.IsNullOrWhiteSpace(masterColumnValue.REFERENCE_NO)))
                {
                    string planNoQuery = $@"select PLAN_NO from IP_PRODUCTION_MRR where MRR_NO ='{masterColumnValue.REFERENCE_NO}' ";
                    masterColumnValue.PLAN_NO = this._dbContext.SqlQuery<int?>(planNoQuery).FirstOrDefault();
                }


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
                    inventoryChildDetails.QUANTITY = childCol.QUANTITY;
                    inventoryChildDetails.VEHICLE_NO = masterColumnValue.VEHICLE_NO;
                    inventoryChildDetails.ENTRY_DATE = masterColumnValue.ENTRY_DATE;
                    inventoryChildDetails.BILL_DATE = masterColumnValue.BILL_DATE;
                    inventoryChildDetails.BILL_NO = masterColumnValue.BILL_NO;
                    inventoryChildDetails.REF_NO = masterColumnValue.REF_NO;
                    inventoryChildDetails.RECEIVED_DATE = masterColumnValue.RECEIVED_DATE;
                    inventoryChildDetails.PRODUCT_CATEGORY = masterColumnValue.PRODUCT_CATEGORY;
                    inventoryChildDetails.SAMPLE_DATE = masterColumnValue.SAMPLE_DATE;
                    inventoryChildDetails.QC_ACTION = childCol.QC_ACTION;
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
                    inventoryChildDetails.CURRENCY_CODE = string.IsNullOrEmpty(childCol.CURRENCY_CODE) ? masterColumnValue.CURRENCY_CODE : "NRS";
                    inventoryChildDetails.EXCHANGE_RATE = string.IsNullOrEmpty(childCol.EXCHANGE_RATE) ? masterColumnValue.EXCHANGE_RATE : "1";
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

                    // M_RATE value added
                    inventoryChildDetails.M_RATE = childCol.M_RATE != null ? childCol.M_RATE : null;








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
                    else if (commonValue.TableName.ToUpper() == "IP_PURCHASE_INVOICE")
                    {
                        var insertQuery = $@"INSERT INTO IP_PURCHASE_INVOICE (INVOICE_NO,INVOICE_DATE, MANUAL_NO, SUPPLIER_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE,SUPPLIER_BUDGET_FLAG,
                     SERIAL_NO, BUDGET_FLAG, ITEM_CODE, MU_CODE,QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE,CALC_TOTAL_PRICE, COMPLETED_QUANTITY,
                     REMARKS, FORM_CODE, COMPANY_CODE,BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, DUE_DATE,CURRENCY_CODE, EXCHANGE_RATE, TERMS_DAY,
                     SYN_ROWID, TRACKING_NO,TO_LOCATION_CODE, SESSION_ROWID, BATCH_NO, MODIFY_DATE, LOT_NO,SUPPLIER_MRR_NO, PP_NO, MODIFY_BY, P_TYPE, PP_DATE,
                     DIVISION_CODE, NET_GROSS_RATE, NET_SALES_RATE, NET_TAXABLE_RATE, MASTER_PP_NO,SECOND_QUANTITY, THIRD_QUANTITY, RECONCILE_DATE, RECONCILE_FLAG,
                     RECONCILE_BY)
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
                     ,'{inventoryChildDetails.THIRD_QUANTITY}','{inventoryChildDetails.RECONCILE_DATE}','{inventoryChildDetails.RECONCILE_FLAG}','{inventoryChildDetails.RECONCILE_BY}'
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
                        var insertQuery = $@"INSERT INTO IP_GOODS_ISSUE (ISSUE_NO,ISSUE_DATE, MANUAL_NO, ISSUE_TYPE_CODE, FROM_LOCATION_CODE, TO_LOCATION_CODE, 
                     TO_BUDGET_FLAG, ITEM_CODE, SERIAL_NO, MU_CODE, REQ_QUANTITY,QUANTITY, UNIT_PRICE, TOTAL_PRICE, CALC_QUANTITY, CALC_UNIT_PRICE, 
                     CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, 
                     EXCHANGE_RATE, SYN_ROWID, TRACKING_NO, SESSION_ROWID, BATCH_NO, MODIFY_DATE, PRODUCT_CODE, MODIFY_BY, USE_PLACE, CUSTOMER_CODE, 
                     EMPLOYEE_CODE, SUPPLIER_CODE, ISSUE_SLIP_NO, DIVISION_CODE, REFERENCE_NO, SECOND_QUANTITY, THIRD_QUANTITY)
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
                    )";
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
                    else if (commonValue.TableName.ToUpper() == "IP_GOODS_REQUISITION")
                    {

                        inventoryChildDetails.SESSION_ROWID = session_row_id.ToString();

                        var insertQuery = $@"INSERT INTO IP_GOODS_REQUISITION (REQUISITION_NO, 
                         REQUISITION_DATE, MANUAL_NO, FROM_LOCATION_CODE, TO_LOCATION_CODE, SUPPLIER_CODE,ITEM_CODE, 
                         SERIAL_NO, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, 
                         CALC_QUANTITY, CALC_UNIT_PRICE, CALC_TOTAL_PRICE, COMPLETED_QUANTITY, REMARKS, 
                         FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                         DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SYN_ROWID, TRACKING_NO, 
                         SESSION_ROWID, MODIFY_DATE,  MODIFY_BY, CUSTOMER_CODE, 
                         SECOND_QUANTITY, THIRD_QUANTITY)
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
                     ,'{inventoryChildDetails.THIRD_QUANTITY}'
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
                             DIVISION_CODE, SECOND_QUANTITY, THIRD_QUANTITY, TNC_CODE)
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
                     ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}',   '{masterColumnValue.TNC_CODE}'
                     )";
                        dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_QC_ENTRY")
                    {
                        var insertQuery = $@"INSERT INTO IP_QC_ENTRY (ENTRY_NO, 
                             ENTRY_DATE, VEHICLE_NO, SUPPLIER_CODE, SERIAL_NO, BILL_DATE, 
                             BILL_NO, REF_NO, RECEIVED_DATE, PRODUCT_CATEGORY, SAMPLE_DATE, 
                             ITEM_CODE, MU_CODE, QUANTITY, REMARKS, QC_ACTION, 
                             FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                             DELETED_FLAG, SYN_ROWID, 
                             TRACKING_NO, SESSION_ROWID, MODIFY_DATE, 
                              MODIFY_BY)
                     VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ENTRY_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.VEHICLE_NO}'
                     ,'{inventoryChildDetails.SUPPLIER_CODE}',{serialno}                            
                     ,'{inventoryChildDetails.BILL_DATE}','{masterColumnValue.BILL_NO}','{inventoryChildDetails.REF_NO}','{inventoryChildDetails.RECEIVED_DATE}'
                     ,'{inventoryChildDetails.PRODUCT_CATEGORY}','{inventoryChildDetails.SAMPLE_DATE}','{inventoryChildDetails.ITEM_CODE}','{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}','{inventoryChildDetails.REMARKS}'
                     ,'{inventoryChildDetails.QC_ACTION}'
                     ,'{inventoryChildDetails.FORM_CODE}'
                     ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                     ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.SYN_ROWID}','{inventoryChildDetails.TRACKING_NO}'
                     ,'{inventoryChildDetails.SESSION_ROWID}',{inventoryChildDetails.MODIFY_DATE}
                     ,'{inventoryChildDetails.MODIFY_BY}'
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
                        string planNoAsString = "";
                        int? PlanNo = masterColumnValue.PLAN_NO > 0 ? masterColumnValue.PLAN_NO : null;
                        if (PlanNo > 0)
                        {
                            planNoAsString = PlanNo.ToString();
                            inventoryChildDetails.FROM_BUDGET_FLAG = "L";
                            inventoryChildDetails.TO_BUDGET_FLAG = "L";
                        }
                        else
                        {
                            inventoryChildDetails.FROM_BUDGET_FLAG = "L";
                            inventoryChildDetails.TO_BUDGET_FLAG = "L";
                        }

                        string WT_PER_LTR = "";
                        if (masterColumnValue.WT_PER_LTR != null && masterColumnValue.WT_PER_LTR > 0)
                        {
                            WT_PER_LTR = masterColumnValue.WT_PER_LTR.ToString();
                        }

                        var insertQuery = $@"INSERT INTO IP_PRODUCTION_ISSUE (ISSUE_NO,ISSUE_DATE,MANUAL_NO,ISSUE_TYPE_CODE,FROM_LOCATION_CODE,FROM_BUDGET_FLAG,TO_LOCATION_CODE,TO_BUDGET_FLAG,ITEM_CODE,SERIAL_NO,MU_CODE,PLAN_NO,WT_PER_LTR,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,TRACKING_NO,SESSION_ROWID,BATCH_NO,LOT_NO,DIVISION_CODE,PRODUCTION_QTY,REFERENCE_NO,SECOND_QUANTITY,THIRD_QUANTITY)
                     VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.ISSUE_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                     ,'{masterColumnValue.ISSUE_TYPE_CODE}','{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.FROM_BUDGET_FLAG}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.TO_BUDGET_FLAG}'                  
                     ,'{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.MU_CODE}', '{planNoAsString}','{WT_PER_LTR}', '{inventoryChildDetails.QUANTITY}'
                     ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                     ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                     ,'{inventoryChildDetails.FORM_CODE}'
                     ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                     ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                     ,'{inventoryChildDetails.EXCHANGE_RATE}',
                      '{tracking_no}',
                      '{session_row_id}',
                      '{BATCH_NO}',
                     '{inventoryChildDetails.LOT_NO}',
                     '{inventoryChildDetails.DIVISION_CODE}',{inventoryChildDetails.PRODUCTION_QTY},'{inventoryChildDetails.REFERENCE_NO}'
                ,'{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}')";
                        var d = dbcontext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "IP_PRODUCTION_MRR")
                    {
                        string getPlanNoQuery = $@"SELECT DISTINCT PLAN_NO FROM IP_PRODUCTION_ISSUE WHERE COMPANY_CODE = '{companyCode}' AND ISSUE_NO = '{masterColumnValue.REFERENCE_NO}' AND ROWNUM = 1";
                        var planNo = this._dbContext.SqlQuery<int?>(getPlanNoQuery).FirstOrDefault();
                        inventoryChildDetails.PLAN_NO = planNo > 0 ? planNo : null;

                        inventoryChildDetails.SESSION_ROWID = session_row_id.ToString();
                        inventoryChildDetails.TRACKING_NO = tracking_no;
                        inventoryChildDetails.BATCH_NO = string.IsNullOrEmpty(BATCH_NO) ? "" : BATCH_NO;

                        string planNoAsString = "";
                        if (inventoryChildDetails.PLAN_NO > 0)
                        {
                            inventoryChildDetails.TO_BUDGET_FLAG = "L";
                            planNoAsString = inventoryChildDetails.PLAN_NO.ToString();
                        }

                        var insertQuery = $@"INSERT INTO IP_PRODUCTION_MRR (MRR_NO,MRR_DATE,MANUAL_NO,FROM_LOCATION_CODE,FROM_BUDGET_FLAG, TO_LOCATION_CODE,TO_BUDGET_FLAG,ITEM_CODE,SERIAL_NO,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,COMPLETED_QUANTITY,REMARKS,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE, DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,TRACKING_NO,SESSION_ROWID,BATCH_NO,LOT_NO,DIVISION_CODE,REFERENCE_NO,SECOND_QUANTITY,THIRD_QUANTITY,PRODUCTION_QTY,PLAN_NO,M_RATE)
                     VALUES ('{commonValue.NewVoucherNumber}',TO_DATE('{inventoryChildDetails.MRR_DATE}','DD-MON-YY hh24:mi:ss'),'{inventoryChildDetails.MANUAL_NO}'
                     ,'{inventoryChildDetails.FROM_LOCATION_CODE}','{inventoryChildDetails.FROM_BUDGET_FLAG}','{inventoryChildDetails.TO_LOCATION_CODE}','{inventoryChildDetails.TO_BUDGET_FLAG}'                  
                     ,'{inventoryChildDetails.ITEM_CODE}',{serialno},'{inventoryChildDetails.MU_CODE}','{inventoryChildDetails.QUANTITY}'
                     ,'{inventoryChildDetails.UNIT_PRICE}','{inventoryChildDetails.TOTAL_PRICE}','{inventoryChildDetails.CALC_QUANTITY}','{inventoryChildDetails.CALC_UNIT_PRICE}'
                     ,'{inventoryChildDetails.CALC_TOTAL_PRICE}','{inventoryChildDetails.COMPLETED_QUANTITY}','{inventoryChildDetails.REMARKS}'
                     ,'{inventoryChildDetails.FORM_CODE}'
                     ,'{inventoryChildDetails.COMPANY_CODE}','{inventoryChildDetails.BRANCH_CODE}','{inventoryChildDetails.CREATED_BY}'
                     ,{inventoryChildDetails.CREATED_DATE},'{inventoryChildDetails.DELETED_FLAG}','{inventoryChildDetails.CURRENCY_CODE}'
                     ,'{inventoryChildDetails.EXCHANGE_RATE}','{inventoryChildDetails.TRACKING_NO}'
                     ,'{inventoryChildDetails.SESSION_ROWID}','{inventoryChildDetails.BATCH_NO}','{inventoryChildDetails.LOT_NO}'
                    ,'{inventoryChildDetails.DIVISION_CODE}','{inventoryChildDetails.REFERENCE_NO}'
                ,'{inventoryChildDetails.SECOND_QUANTITY}','{inventoryChildDetails.THIRD_QUANTITY}','{inventoryChildDetails.QUANTITY}', '{planNoAsString}', '{inventoryChildDetails.M_RATE}')";
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
        public void SaveBudgetTransactionColumnValue(List<FinancialBudgetTransaction> budgetTransaction, CommonFieldsForInventory commonValue, NeoErpCoreEntity dbcontext = null)
        {
            try
            {
                var budSerial = 1;
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
                                    string maxtransnoquery = string.Format(@"SELECT TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO )+1),1))) as TRANSACTIONNO FROM BUDGET_TRANSACTION");
                                    int newMaxTransNoForBudget = _coreEntity.SqlQuery<int>(maxtransnoquery).FirstOrDefault();
                                    string insertbudgettransQuery = $@"INSERT INTO BUDGET_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,BUDGET_FLAG,SERIAL_NO,BUDGET_CODE,
                                                                              BUDGET_AMOUNT,PARTICULARS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              CURRENCY_CODE,EXCHANGE_RATE,VALIDATION_FLAG,ACC_CODE,SESSION_ROWID)
                                                                              VALUES('{newMaxTransNoForBudget}','{commonValue.FormCode}','{VoucherNumber}','{budgetflag}',{budSerial++},'{bud.BUDGET_CODE}',
                                                                             {bud.QUANTITY},'{bud.NARRATION}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','NRS',{1},'Y','{bud.ACC_CODE}','')";
                                    dbcontext.ExecuteSqlCommand(insertbudgettransQuery);


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
                    string insertChargeQuery = $@"INSERT INTO CHARGE_TRANSACTION(TRANSACTION_NO,TABLE_NAME,REFERENCE_NO,APPLY_ON,ACC_CODE,CHARGE_CODE,CHARGE_TYPE_FLAG,CHARGE_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,CALCULATE_BY,GL_FLAG,NON_GL_FLAG,SERIAL_NO) VALUES('{newtransno}','{commonField.TableName}','{commonField.NewVoucherNumber}','{cc.APPLY_ON}','{cc.ACC_CODE}','{cc.CHARGE_CODE}','{cc.CHARGE_TYPE_FLAG}', {cc.CHARGE_AMOUNT},'{commonField.FormCode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N','{currencyformat}',{commonField.ExchangeRate},'{cc.VALUE_PERCENT_FLAG}','{cc.GL_FLAG}','{cc.NON_GL_FLAG}',1)";
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


        public PPDetailModel MapPPDetailColumnValue(string ppDetailValue)
        {
            try
            {
                if (string.IsNullOrEmpty(ppDetailValue))
                    return null;

                var ppDetail = JsonConvert.DeserializeObject<PPDetailModel>(ppDetailValue);
                return ppDetail;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SavePPDetailColumnValue(PPDetailModel ppDetailModel, CommonFieldsForInventory commonFieldForInventory, NeoErpCoreEntity Dbcontext = null)
        {
            try
            {
                if (ppDetailModel == null || string.IsNullOrEmpty(commonFieldForInventory.NewVoucherNumber))
                    return;

                // Format dates
                string invoiceDate = string.IsNullOrEmpty(ppDetailModel.INVOICE_DATE) ? "SYSDATE" : $"TO_DATE('{ppDetailModel.INVOICE_DATE}', 'DD-MON-YYYY')";
                string createdDate = "SYSDATE";

                string insertPPDetailQuery = $@"INSERT INTO FA_PP_DETAIL_TRANSACTION (
                    REFERENCE_NO, PP_NO, SUPPLIER_INV_NO, INVOICE_DATE, CS_CODE, 
                    INVOICE_AMOUNT, IMPORT_DUTY_AMOUNT, VAT_AMOUNT, TRANSPORTATION_AMOUNT, 
                    INSURENCE_AMOUNT, FREIGHT_AMOUNT, OTHERS_AMOUNT, TOTAL_COST, ECS_AMOUNT, 
                    CUSTOM_FEE, REMARKS, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, 
                    CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SESSION_ROWID, 
                    TAXABLE_AMOUNT, CUSTOM_OFFICE, BILL_OF_EXPORT_NO, C_FORM_NO, GREEN_TAX
                ) VALUES (
                    '{ppDetailModel.REFERENCE_NO}', '{ppDetailModel.PP_NO}', '{ppDetailModel.SUPPLIER_INV_NO}', 
                    {invoiceDate}, '{ppDetailModel.CS_CODE}',
                    {ppDetailModel.INVOICE_AMOUNT}, {ppDetailModel.IMPORT_DUTY_AMOUNT}, {ppDetailModel.VAT_AMOUNT}, 
                    {ppDetailModel.TRANSPORTATION_AMOUNT}, {ppDetailModel.INSURENCE_AMOUNT}, {ppDetailModel.FREIGHT_AMOUNT}, 
                    {ppDetailModel.OTHERS_AMOUNT}, {ppDetailModel.TOTAL_COST}, {ppDetailModel.ECS_AMOUNT}, 
                    {ppDetailModel.CUSTOM_FEE}, '{ppDetailModel.REMARKS}', '{commonFieldForInventory.FormCode}', 
                    '{_workContext.CurrentUserinformation.company_code}', '{_workContext.CurrentUserinformation.branch_code}', 
                    '{_workContext.CurrentUserinformation.login_code}', {createdDate}, 'N', 
                    '{commonFieldForInventory.CurrencyFormat}', {commonFieldForInventory.ExchangeRate}, '{ppDetailModel.SESSION_ROWID}', 
                    {ppDetailModel.TAXABLE_AMOUNT}, '{ppDetailModel.CUSTOM_OFFICE}', '{ppDetailModel.BILL_OF_EXPORT_NO}', 
                    '{ppDetailModel.C_FORM_NO}', {ppDetailModel.GREEN_TAX}
                )";

                Dbcontext.ExecuteSqlCommand(insertPPDetailQuery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void UpdatePPDetailColumnValue(PPDetailModel ppDetailModel, CommonFieldsForInventory commonFieldForInventory, NeoErpCoreEntity Dbcontext = null)
        {
            try
            {
                if (ppDetailModel == null || string.IsNullOrEmpty(commonFieldForInventory.VoucherNumber))
                    return;

                // Format dates - handle null/empty dates
                string invoiceDate = "null";
                if (!string.IsNullOrEmpty(ppDetailModel.INVOICE_DATE))
                {
                    try
                    {
                        // Try to parse the date and format it correctly
                        DateTime parsedDate;
                        if (DateTime.TryParse(ppDetailModel.INVOICE_DATE, out parsedDate))
                        {
                            // Format as DD-MMM-YYYY for Oracle (e.g., "02-Dec-2025")
                            invoiceDate = $"TO_DATE('{parsedDate.ToString("dd-MMM-yyyy")}', 'DD-MON-YYYY')";
                        }
                        else
                        {
                            // If already in correct format, use it
                            invoiceDate = $"TO_DATE('{ppDetailModel.INVOICE_DATE}', 'DD-MON-YYYY')";
                        }
                    }
                    catch
                    {
                        invoiceDate = "null";
                    }
                }

                string updatePPDetailQuery = $@"UPDATE FA_PP_DETAIL_TRANSACTION SET 
            PP_NO = '{ppDetailModel.PP_NO}',
            SUPPLIER_INV_NO = '{ppDetailModel.SUPPLIER_INV_NO}',
            INVOICE_DATE = {invoiceDate},
            CS_CODE = '{ppDetailModel.CS_CODE}',
            INVOICE_AMOUNT = {ppDetailModel.INVOICE_AMOUNT},
            IMPORT_DUTY_AMOUNT = {ppDetailModel.IMPORT_DUTY_AMOUNT},
            VAT_AMOUNT = {ppDetailModel.VAT_AMOUNT},
            TRANSPORTATION_AMOUNT = {ppDetailModel.TRANSPORTATION_AMOUNT},
            INSURENCE_AMOUNT = {ppDetailModel.INSURENCE_AMOUNT},
            FREIGHT_AMOUNT = {ppDetailModel.FREIGHT_AMOUNT},
            OTHERS_AMOUNT = {ppDetailModel.OTHERS_AMOUNT},
            TOTAL_COST = {ppDetailModel.TOTAL_COST},
            ECS_AMOUNT = {ppDetailModel.ECS_AMOUNT},
            CUSTOM_FEE = {ppDetailModel.CUSTOM_FEE},
            REMARKS = '{ppDetailModel.REMARKS}',
            CURRENCY_CODE = '{commonFieldForInventory.CurrencyFormat}',
            EXCHANGE_RATE = {commonFieldForInventory.ExchangeRate},
            TAXABLE_AMOUNT = {ppDetailModel.TAXABLE_AMOUNT},
            CUSTOM_OFFICE = '{ppDetailModel.CUSTOM_OFFICE}',
            BILL_OF_EXPORT_NO = '{ppDetailModel.BILL_OF_EXPORT_NO}',
            C_FORM_NO = '{ppDetailModel.C_FORM_NO}',
            GREEN_TAX = {ppDetailModel.GREEN_TAX},
            MODIFY_DATE = SYSDATE,
            MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
        WHERE REFERENCE_NO = '{commonFieldForInventory.VoucherNumber}' 
          AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
          AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";

                Dbcontext.ExecuteSqlCommand(updatePPDetailQuery);
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
                //bool insertedToMaster = false;
                //string insertmasterQuery = string.Format(@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,SESSION_ROWID,SYN_ROWID,EXCHANGE_RATE,REFERENCE_NO) 
                //     VALUES('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}',{8},TO_DATE({9},'DD-MON-YY hh24:mi:ss'),'{10}','{11}',{12},'{13}')",
                //    commonValue.NewVoucherNumber, commonValue.Grand_Total, commonValue.FormCode, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code, _workContext.CurrentUserinformation.login_code.ToUpper(), 'N', commonValue.CurrencyFormat, "SYSDATE", commonValue.VoucherDate, masterColumnValue.MANUAL_NO, '1', commonValue.ExchangeRate, masterColumnValue.MANUAL_NO);
                //dbcontext.ExecuteSqlCommand(insertmasterQuery);


                bool insertedToMaster = false;
                string insertmasterQuery = string.Format(@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,SESSION_ROWID,SYN_ROWID,EXCHANGE_RATE,REFERENCE_NO) 
              VALUES('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}',{8},TO_DATE({9},'DD-MON-YY hh24:mi:ss'),'{10}','{11}',{12},'{13}')",
                    commonValue.NewVoucherNumber, commonValue.Grand_Total, commonValue.FormCode, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code, _workContext.CurrentUserinformation.login_code.ToUpper(), 'N', commonValue.CurrencyFormat, "SYSDATE", commonValue.VoucherDate, masterColumnValue.SESSION_ROWID, "", commonValue.ExchangeRate, masterColumnValue.MANUAL_NO);
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
        #endregion
        #region save reference data
        //public void GetFormReference(List<Inventory> childColumnValue, Inventory masterColumnValue, CommonFieldsForInventory commonValue, FormDetails model, string primarydatecolumn, string primarycolname, NeoErpCoreEntity dbcontext = null)
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
                                                     REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE) 
                                                   VALUES('{maxRefNo}','{commonValue.NewVoucherNumber}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{serialNo}','{RefPurchaseOrder.ISSUE_NO}','{RefPurchaseOrder.FORM_CODE}','{RefPurchaseOrder.ITEM_CODE}',
                                                    '{Ref.calc_qty}','{RefPurchaseOrder.MU_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                          'N','{Ref.cal_unit_price}','{Ref.Total_price}','{Ref.cal_unit_price}','{Ref.cal_total_price}','{RefPurchaseOrder.REMARKS}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{RefPurchaseOrder.BRANCH_CODE}','{RefPurchaseOrder.SERIAL_NO}','','','',TO_DATE('{RefPurchaseOrder.ISSUE_DATE}','DD-MON-YYYY hh24:mi:ss'))";

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
        #endregion

        #region METHOD FOR EDIT

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

        public bool UpdateMasterTransaction(CommonFieldsForInventory commonUpdateValue, NeoErpCoreEntity neoErpCoreEntity = null)
        {
            try
            {
                string query = $@"UPDATE MASTER_TRANSACTION SET VOUCHER_AMOUNT={commonUpdateValue.DrTotal},VOUCHER_DATE={commonUpdateValue.VoucherDate},MODIFY_BY = '{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYN_ROWID='{commonUpdateValue.ManualNumber}',SESSION_ROWID='{commonUpdateValue.SESSION_ROWID}',   REFERENCE_NO='{commonUpdateValue.ManualNumber}' , MODIFY_DATE = SYSDATE,CURRENCY_CODE='{commonUpdateValue.CurrencyFormat}',EXCHANGE_RATE={commonUpdateValue.ExchangeRate} where VOUCHER_NO='{commonUpdateValue.VoucherNumber}'  and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
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

        #endregion

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

        //        public List<BATCHTRANSACTIONDATA> Getbatchdetail(string voucherno)
        //        {
        //            var companyCode = _workContext.CurrentUserinformation.company_code;
        //            string Query = $@"SELECT ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.ITEM_SERIAL_NO AS TRACKING_SERIAL_NO FROM BATCH_TRANSACTION BT,IP_ITEM_MASTER_SETUP ISS 
        //WHERE ISS.ITEM_CODE=BT.ITEM_CODE AND 
        //ISS.COMPANY_CODE=BT.COMPANY_CODE AND
        //BT.REFERENCE_NO='{voucherno}' AND
        //BT.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND
        //BT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND BT.ITEM_SERIAL_FLAG='Y'";
        //            var entity = this._dbContext.SqlQuery<BATCHTRANSACTIONDATA>(Query).ToList();
        //            return entity;
        //        }
        //        public List<BATCH_TRANSACTION_DATA> Getbatchtrackingdetail(string voucherno)
        //        {
        //            var companyCode = _workContext.CurrentUserinformation.company_code;
        //            string Query = $@"SELECT distinct ISS.ITEM_CODE,ISS.ITEM_EDESC,BT.MU_CODE,BT.QUANTITY,BT.SERIAL_NO,BT.BATCH_NO,BT.LOCATION_CODE,BT.BATCH_NO AS TRACKING_SERIAL_NO,BT.EXPIRY_DATE FROM BATCH_TRANSACTION BT,IP_ITEM_MASTER_SETUP ISS 
        //WHERE ISS.ITEM_CODE=BT.ITEM_CODE AND 
        //ISS.COMPANY_CODE=BT.COMPANY_CODE AND
        //BT.REFERENCE_NO='{voucherno}' AND
        //BT.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND
        //BT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND BT.BATCH_SERIAL_FLAG='Y'";
        //            var entity = this._dbContext.SqlQuery<BATCH_TRANSACTION_DATA>(Query).ToList();
        //            return entity;
        //        }
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

        public bool deletevouchernoInv(string tablename, string formcode, string voucherno, string primarycolumnname)
        {
            using (var trans = _coreEntity.Database.BeginTransaction())
            {
                try
                {
                    var deletmaintableequery = $@"UPDATE {tablename} SET DELETED_FLAG='Y',MODIFY_DATE=SYSDATE,MODIFY_BY='{_workContext.CurrentUserinformation.login_code}' WHERE {primarycolumnname}='{voucherno}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.Company}'";
                    var maintablerowCount = _coreEntity.ExecuteSqlCommand(deletmaintableequery);

                    if (maintablerowCount > 0)
                    {
                        var deletemastertable = $@"UPDATE MASTER_TRANSACTION SET DELETED_FLAG='Y', MODIFY_DATE=SYSDATE,MODIFY_BY='{_workContext.CurrentUserinformation.login_code}' WHERE VOUCHER_NO='{voucherno}' AND COMPANY_CODE='{_workContext.CurrentUserinformation.Company}'";
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



        public bool DeleteIssueRecord(string voucherno)
        {
            using (var trans = _coreEntity.Database.BeginTransaction())
            {
                try
                {
                    var deleteMaster = $@"DELETE FROM MASTER_TRANSACTION WHERE VOUCHER_NO = '{voucherno}'";
                    var deleteIssue = $@"DELETE FROM IP_PRODUCTION_ISSUE WHERE ISSUE_NO = '{voucherno}'";
                    var deleteMRR = $@"DELETE FROM IP_PRODUCTION_MRR WHERE REFERENCE_NO = '{voucherno}'";

                    _coreEntity.Database.ExecuteSqlCommand(deleteIssue);
                    _coreEntity.Database.ExecuteSqlCommand(deleteMRR);
                    _coreEntity.Database.ExecuteSqlCommand(deleteMaster);

                    trans.Commit();
                    return true;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }


        // added by mahesh
        // code for resource entry 
        public void SaveResourceDetailsEntryColumnValue(List<ResourceDetail> resourceDetailList, string voucherNo, string location_code, string form_code, string resource_category_type = "", bool isSaveMainTBL = false, NeoErpCoreEntity Dbcontext = null)
        {
            try
            {

                string processCodeQuery = $@"select Process_Code from MP_PROCESS_SETUP where Location_Code='{location_code}' and company_code='{_workContext.CurrentUserinformation.company_code}'";
                var processCode = Dbcontext.SqlQuery<string>(processCodeQuery).FirstOrDefault();

                if (isSaveMainTBL == false)
                {

                    string insertQueryMpResourceEntry = $@"INSERT INTO MP_RESOURCE_ENTRY (
                                                          TRANSACTION_NO,
                                                          TRANSACTION_DATE,
                                                          PROCESS_CODE,
                                                          REMARKS,
                                                          COMPANY_CODE,
                                                          CREATED_BY,
                                                          CREATED_DATE,
                                                          DELETED_FLAG,
                                                          SYN_ROWID,
                                                          MODIFY_DATE,
                                                          MODIFY_BY
                                                        ) VALUES (
                                                          '{voucherNo}',                     -- TRANSACTION_NO (PK)
                                                          SYSDATE,                           -- TRANSACTION_DATE
                                                          '{processCode}',                  -- PROCESS_CODE (PK)
                                                          '',                                -- REMARKS
                                                          '{_workContext.CurrentUserinformation.company_code}',     -- COMPANY_CODE (PK)
                                                          '{_workContext.CurrentUserinformation.login_code}',       -- CREATED_BY
                                                          SYSDATE,                     -- CREATED_DATE
                                                          'N',                         -- DELETED_FLAG (must be 'Y' or 'N')
                                                          NULL,                        -- SYN_ROWID (18 characters)
                                                          NULL,                        -- MODIFY_DATE (optional)
                                                          NULL                         -- MODIFY_BY (optional)
                                                        )";

                    Dbcontext.ExecuteSqlCommand(insertQueryMpResourceEntry);
                }

                //int serialNo = 1;
                foreach (var itemResource in resourceDetailList)
                {
                    if (itemResource.SERIALS != null && itemResource.SERIALS.Any())
                    {
                        foreach (var itemSerial in itemResource.SERIALS)
                        {
                            string insertQuery = $@"INSERT INTO MP_RESOURCE_DETAIL_ENTRY (
                                                                              VOUCHER_NO,
                                                                              FORM_CODE,
                                                                              RESOURCE_CODE,
                                                                              START_TIME,
                                                                              END_TIME,
                                                                              TOTAL_HOURS,
                                                                              COMPANY_CODE,
                                                                              BRANCH_CODE,
                                                                              CREATED_BY,
                                                                              CREATED_DATE,
                                                                              DELETED_FLAG,
                                                                              MODIFY_DATE,
                                                                              MODIFY_BY,
                                                                              EXPECTED_QTY,
                                                                              ACTUAL_QTY,
                                                                              CATEGORY_TYPE,
                                                                              SERIAL_NO
                                                                            ) VALUES (
                                                                              '{voucherNo}',                              -- VOUCHER_NO
                                                                              '{form_code}',                              -- FORM_CODE
                                                                              '{itemResource.RESOURCE_CODE}',             -- RESOURCE_CODE
                                                                              '',                               -- START_TIME
                                                                              '',                               -- END_TIME
                                                                              '',                               -- TOTAL_HOURS
                                                                              '{_workContext.CurrentUserinformation.company_code}',             -- COMPANY_CODE
                                                                              '{_workContext.CurrentUserinformation.branch_code}',              -- BRANCH_CODE
                                                                              '{_workContext.CurrentUserinformation.login_code}',               -- CREATED_BY
                                                                              SYSDATE,                              -- CREATED_DATE
                                                                              'N',                                  -- DELETED_FLAG
                                                                              '',                                   -- MODIFY_DATE
                                                                              '',                                   -- MODIFY_BY
                                                                              '',                                   -- EXPECTED_QTY
                                                                              '{itemSerial.Qty}',                   -- ACTUAL_QTY
                                                                              '{resource_category_type}',
                                                                              '{itemSerial.SerialNo}'
                                                                            )";
                            Dbcontext.ExecuteSqlCommand(insertQuery);
                        }
                    }
                    else
                    {
                        string insertQuery = $@"INSERT INTO MP_RESOURCE_DETAIL_ENTRY (
                                                                              VOUCHER_NO,
                                                                              FORM_CODE,
                                                                              RESOURCE_CODE,
                                                                              START_TIME,
                                                                              END_TIME,
                                                                              TOTAL_HOURS,
                                                                              COMPANY_CODE,
                                                                              BRANCH_CODE,
                                                                              CREATED_BY,
                                                                              CREATED_DATE,
                                                                              DELETED_FLAG,
                                                                              MODIFY_DATE,
                                                                              MODIFY_BY,
                                                                              EXPECTED_QTY,
                                                                              ACTUAL_QTY,
                                                                              CATEGORY_TYPE,
                                                                              SERIAL_NO
                                                                            ) VALUES (
                                                                              '{voucherNo}',                              -- VOUCHER_NO
                                                                              '{form_code}',                              -- FORM_CODE
                                                                              '{itemResource.RESOURCE_CODE}',             -- RESOURCE_CODE
                                                                              '',                               -- START_TIME
                                                                              '',                               -- END_TIME
                                                                              '',                               -- TOTAL_HOURS
                                                                              '{_workContext.CurrentUserinformation.company_code}',             -- COMPANY_CODE
                                                                              '{_workContext.CurrentUserinformation.branch_code}',              -- BRANCH_CODE
                                                                              '{_workContext.CurrentUserinformation.login_code}',               -- CREATED_BY
                                                                              SYSDATE,              -- CREATED_DATE
                                                                              'N',                  -- DELETED_FLAG
                                                                              '',                   -- MODIFY_DATE
                                                                              '',                   -- MODIFY_BY
                                                                              '{itemResource.REQUIRED_INPUT_QTY}',        -- EXPECTED_QTY
                                                                              '{itemResource.ACTUAL_QTY}',                -- ACTUAL_QTY
                                                                              '{resource_category_type}',
                                                                              ''
                                                                            )";
                        Dbcontext.ExecuteSqlCommand(insertQuery);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public bool DeleteResourceList(string voucherNo, string resource_category_type, NeoErpCoreEntity coreEntity = null)
        {
            using (var trans = _coreEntity.Database.BeginTransaction())
            {
                try
                {
                    var deleteResourceEntryList = string.Format(@"DELETE FROM MP_RESOURCE_DETAIL_ENTRY where VOUCHER_NO='{0}' and CATEGORY_TYPE ='{1}' and COMPANY_CODE='{2}'", voucherNo, resource_category_type, this._workContext.CurrentUserinformation.company_code);
                    var maintablerowCount = _coreEntity.ExecuteSqlCommand(deleteResourceEntryList);
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




    }
}