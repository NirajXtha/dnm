using NeoErp.Core;
using NeoErp.Core.Models;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoERP.DocumentTemplate.Service.Services
{
    public class FinancialVoucherSaveService : IFinancialVoucherSaveService
    {

        private static IWorkContext _workContext;
        private static NeoErpCoreEntity _objectEntity;
        public FinancialVoucherSaveService(IWorkContext workContext)
        {
            _workContext = workContext;
            _objectEntity = new NeoErpCoreEntity();
        }

        #region MAPPING COLUMN VALUE TO OBJECT
        public List<FinancialVoucherDetail> MapChildColumnWithValue(string childColumn)
        {
            try
            {
                var childColVal = JsonConvert.DeserializeObject<List<FinancialVoucherDetail>>(childColumn);
                return childColVal;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public FinancialVoucherDetail MapMasterColumnWithValue(string masterColumn)
        {
            try
            {
                var masterColVal = JsonConvert.DeserializeObject<FinancialVoucherDetail>(masterColumn);
                return masterColVal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public List<FinancialSubLedger> MapChildSubLedgerColumnWithValue(string childSubLedgerValue)
        {
            try
            {

                List<FinancialSubLedger> fa = null;
                if (childSubLedgerValue != null) fa = JsonConvert.DeserializeObject<List<FinancialSubLedger>>(childSubLedgerValue);

                return fa;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public List<FinancialVAT> MapVaTColumnWithValue(string vatValue)
        {
            try
            {

                List<FinancialVAT> fa = null;
                if (vatValue != null) fa = JsonConvert.DeserializeObject<List<FinancialVAT>>(vatValue);

                return fa;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
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
        public List<CHARGETRANSACTION> MapChargeTransactionColumnValue(string transactionValue)
        {
            try
            {
                List<CHARGETRANSACTION> fa = null;
                if (transactionValue != null) fa = JsonConvert.DeserializeObject<List<CHARGETRANSACTION>>(transactionValue);

                return fa;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<FinancialTDS> MapTdsColumnValue(string tdsValue)
        {
            try
            {
                List<FinancialTDS> ftds = null;
                if (tdsValue != null) ftds = JsonConvert.DeserializeObject<List<FinancialTDS>>(tdsValue);

                return ftds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        #endregion


        #region SAVING COLUMN VALUE TO DB

        public bool SaveChildColumnValue(List<FinancialVoucherDetail> childColumnValue, FinancialVoucherDetail masterColumnValue, CommonFieldsForFinanacialVoucher commonValue,NeoErpCoreEntity dbContext = null)
        {
            try
            {

                int serialno = 1;
                bool insertedToChild = false;
                FinancialVoucherDetail financialChildDetails = new FinancialVoucherDetail();
                foreach (var childCol in childColumnValue)
                {
                    financialChildDetails.VOUCHER_NO = (commonValue.VoucherNumber == "undefined") ? commonValue.NewVoucherNumber : commonValue.VoucherNumber;
                    financialChildDetails.VOUCHER_DATE = string.IsNullOrEmpty(childCol.VOUCHER_DATE) ? (string.IsNullOrEmpty(masterColumnValue.VOUCHER_DATE) ? "SYSDATE" : masterColumnValue.VOUCHER_DATE) : childCol.VOUCHER_DATE;
                    financialChildDetails.MANUAL_NO = string.IsNullOrEmpty(childCol.MANUAL_NO) ? masterColumnValue.MANUAL_NO : childCol.MANUAL_NO;
                    financialChildDetails.SERIAL_NO = string.IsNullOrEmpty(childCol.SERIAL_NO) ? serialno.ToString() : childCol.SERIAL_NO;
                    financialChildDetails.ACC_CODE = childCol.ACC_CODE;
                    financialChildDetails.PARTICULARS = string.IsNullOrEmpty(childCol.PARTICULARS) ? masterColumnValue.PARTICULARS : childCol.PARTICULARS;
                    financialChildDetails.TRANSACTION_TYPE = string.IsNullOrEmpty(childCol.TRANSACTION_TYPE) ? masterColumnValue.TRANSACTION_TYPE : childCol.TRANSACTION_TYPE;
                    financialChildDetails.AMOUNT = childCol.AMOUNT;
                    financialChildDetails.BUDGET_FLAG = string.IsNullOrEmpty(childCol.BUDGET_FLAG) ? masterColumnValue.BUDGET_FLAG : childCol.BUDGET_FLAG;
                    financialChildDetails.REMARKS = string.IsNullOrEmpty(childCol.REMARKS) ? masterColumnValue.REMARKS : childCol.REMARKS;
                    financialChildDetails.FORM_CODE = string.IsNullOrEmpty(commonValue.FormCode) ? masterColumnValue.FORM_CODE : commonValue.FormCode;
                    financialChildDetails.COMPANY_CODE = string.IsNullOrEmpty(childCol.COMPANY_CODE) ? _workContext.CurrentUserinformation.company_code : childCol.COMPANY_CODE;
                    financialChildDetails.BRANCH_CODE = string.IsNullOrEmpty(childCol.BRANCH_CODE) ? _workContext.CurrentUserinformation.branch_code : childCol.BRANCH_CODE;
                    financialChildDetails.CREATED_BY = string.IsNullOrEmpty(childCol.CREATED_BY) ? _workContext.CurrentUserinformation.login_code : childCol.CREATED_BY;
                    financialChildDetails.CREATED_DATE = string.IsNullOrEmpty(childCol.CREATED_DATE) ? (string.IsNullOrEmpty(masterColumnValue.CREATED_DATE) ? "SYSDATE" : masterColumnValue.CREATED_DATE) : childCol.CREATED_DATE;
                    financialChildDetails.DELETED_FLAG = "N";
                    financialChildDetails.CURRENCY_CODE = string.IsNullOrEmpty(masterColumnValue.CURRENCY_CODE) ? "NRS" : masterColumnValue.CURRENCY_CODE;
                    financialChildDetails.EXCHANGE_RATE = string.IsNullOrEmpty(masterColumnValue.EXCHANGE_RATE) ? "1" : masterColumnValue.EXCHANGE_RATE;
                    financialChildDetails.SYN_ROWID = string.IsNullOrEmpty(childCol.SYN_ROWID) ? masterColumnValue.SYN_ROWID : childCol.SYN_ROWID;
                    financialChildDetails.OMIT_FLAG = string.IsNullOrEmpty(childCol.OMIT_FLAG) ? masterColumnValue.OMIT_FLAG : childCol.OMIT_FLAG;
                    financialChildDetails.TRACKING_NO = string.IsNullOrEmpty(childCol.TRACKING_NO) ? masterColumnValue.TRACKING_NO : childCol.TRACKING_NO;
                    financialChildDetails.SESSION_ROWID = string.IsNullOrEmpty(childCol.SESSION_ROWID) ? masterColumnValue.SESSION_ROWID : childCol.SESSION_ROWID;
                    financialChildDetails.MODIFY_DATE = string.IsNullOrEmpty(childCol.MODIFY_DATE) ? (string.IsNullOrEmpty(masterColumnValue.MODIFY_DATE) ? "SYSDATE" : masterColumnValue.MODIFY_DATE) : childCol.MODIFY_DATE;
                    financialChildDetails.CHEQUE_NO = string.IsNullOrEmpty(childCol.CHEQUE_NO) ? masterColumnValue.CHEQUE_NO : childCol.CHEQUE_NO;
                    financialChildDetails.INVOICE_NO = string.IsNullOrEmpty(childCol.INVOICE_NO) ? masterColumnValue.INVOICE_NO : childCol.INVOICE_NO;
                    financialChildDetails.DIVISION_CODE = string.IsNullOrEmpty(childCol.DIVISION_CODE) ? masterColumnValue.DIVISION_CODE : childCol.DIVISION_CODE;
                    financialChildDetails.EMPLOYEE_CODE = string.IsNullOrEmpty(childCol.EMPLOYEE_CODE) ? masterColumnValue.EMPLOYEE_CODE : childCol.EMPLOYEE_CODE;
                    financialChildDetails.EFFECTIVE_DATE = string.IsNullOrEmpty(childCol.EFFECTIVE_DATE) ? (string.IsNullOrEmpty(masterColumnValue.EFFECTIVE_DATE) ? "SYSDATE" : masterColumnValue.EFFECTIVE_DATE) : childCol.EFFECTIVE_DATE;
                    financialChildDetails.MR_ISSUED_BY = masterColumnValue.MR_ISSUED_BY;
                    financialChildDetails.MR_NO = masterColumnValue.MR_NO;

                    if (commonValue.TableName.ToUpper() == "FA_DOUBLE_VOUCHER")
                    {
                        var insertQuery = $@"INSERT INTO FA_DOUBLE_VOUCHER (EFFECTIVE_DATE,DIVISION_CODE,REMARKS,MANUAL_NO,VOUCHER_DATE,VOUCHER_NO,TRANSACTION_TYPE,PARTICULARS,
                       BUDGET_FLAG,AMOUNT,ACC_CODE,SERIAL_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID,CHEQUE_NO,MR_ISSUED_BY,MR_NO,CURRENCY_CODE,EXCHANGE_RATE)
                       VALUES ({financialChildDetails.EFFECTIVE_DATE},'{financialChildDetails.DIVISION_CODE}','{financialChildDetails.REMARKS}',
                     '{financialChildDetails.MANUAL_NO}', TO_DATE('{financialChildDetails.VOUCHER_DATE}', 'DD-MON-YYYY hh24:mi:ss'),'{financialChildDetails.VOUCHER_NO}', '{financialChildDetails.TRANSACTION_TYPE}',
                     '{financialChildDetails.PARTICULARS}', '{financialChildDetails.BUDGET_FLAG}',{financialChildDetails.AMOUNT},'{financialChildDetails.ACC_CODE}',
                     '{serialno}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}',
                     '{_workContext.CurrentUserinformation.login_code}',{financialChildDetails.CREATED_DATE}, '{financialChildDetails.DELETED_FLAG}','{financialChildDetails.SESSION_ROWID}','{financialChildDetails.CHEQUE_NO}','{financialChildDetails.MR_NO}','{financialChildDetails.MR_ISSUED_BY}','{financialChildDetails.CURRENCY_CODE}',{financialChildDetails.EXCHANGE_RATE})";

                        dbContext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else if (commonValue.TableName.ToUpper() == "FA_PAY_ORDER")
                    {
                        var insertQuery = $@"INSERT INTO FA_PAY_ORDER(VOUCHER_NO,VOUCHER_DATE,INVOICE_NO,MANUAL_NO,SERIAL_NO,ACC_CODE,ACC_ID,PARTICULARS,
                            TRANSACTION_TYPE,AMOUNT,PAYMENT_MODE,REMARKS,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,TRACKING_NO,SESSION_ROWID,CHEQUE_NO,REFERENCE_FLAG)
                            VALUES('{financialChildDetails.VOUCHER_NO}',TO_DATE('{financialChildDetails.VOUCHER_DATE}','DD-MON-YYYY hh24:mi:ss'),'{financialChildDetails.INVOICE_NO}','{financialChildDetails.MANUAL_NO}','{financialChildDetails.SERIAL_NO}','{financialChildDetails.ACC_CODE}',
                            '{financialChildDetails.ACC_ID}','{financialChildDetails.PARTICULARS}','{financialChildDetails.TRANSACTION_TYPE}','{financialChildDetails.AMOUNT}','{financialChildDetails.PAYMENT_MODE}',
                            '{financialChildDetails.REMARKS}','{financialChildDetails.FORM_CODE}','{financialChildDetails.COMPANY_CODE}','{financialChildDetails.BRANCH_CODE}','{financialChildDetails.CREATED_BY}',{financialChildDetails.CREATED_DATE},
                            '{financialChildDetails.DELETED_FLAG}','{financialChildDetails.CURRENCY_CODE}','{financialChildDetails.EXCHANGE_RATE}','{financialChildDetails.TRACKING_NO}','{financialChildDetails.SESSION_ROWID}','{financialChildDetails.CHEQUE_NO}','{financialChildDetails.REFERENCE_FLAG}')";

                        dbContext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;

                    }
                    else if (commonValue.TableName.ToUpper() == "FA_SINGLE_VOUCHER")
                    {
                        financialChildDetails.MASTER_ACC_CODE = masterColumnValue.MASTER_ACC_CODE;
                        financialChildDetails.MASTER_TRANSACTION_TYPE = string.IsNullOrEmpty(childCol.MASTER_TRANSACTION_TYPE) ? masterColumnValue.MASTER_TRANSACTION_TYPE : childCol.MASTER_TRANSACTION_TYPE;
                        financialChildDetails.MASTER_AMOUNT = (masterColumnValue.MASTER_AMOUNT > 0) ? masterColumnValue.MASTER_AMOUNT : childCol.MASTER_AMOUNT;
                        financialChildDetails.MASTER_BUDGET_FLAG = string.IsNullOrEmpty(masterColumnValue.MASTER_BUDGET_FLAG) ? childCol.MASTER_BUDGET_FLAG : masterColumnValue.MASTER_BUDGET_FLAG;

                        financialChildDetails.BUDGET_FLAG = string.IsNullOrEmpty(childCol.BUDGET_FLAG) ? masterColumnValue.BUDGET_FLAG : childCol.BUDGET_FLAG;


                        var insertQuery = $@"INSERT INTO FA_SINGLE_VOUCHER(VOUCHER_NO,VOUCHER_DATE,MANUAL_NO,SERIAL_NO,MASTER_ACC_CODE,MASTER_TRANSACTION_TYPE,MASTER_AMOUNT,MASTER_BUDGET_FLAG,BUDGET_FLAG,ACC_CODE,PARTICULARS,
                             TRANSACTION_TYPE,AMOUNT,REMARKS,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CURRENCY_CODE,EXCHANGE_RATE,TRACKING_NO,SESSION_ROWID,CHEQUE_NO,MR_ISSUED_BY,MR_NO) 
                             VALUES('{financialChildDetails.VOUCHER_NO}',TO_DATE('{financialChildDetails.VOUCHER_DATE}','DD-MON-YYYY hh24:mi:ss'),'{financialChildDetails.MANUAL_NO}','{financialChildDetails.SERIAL_NO}','{financialChildDetails.MASTER_ACC_CODE}',
                             '{financialChildDetails.MASTER_TRANSACTION_TYPE}',{financialChildDetails.MASTER_AMOUNT},'{financialChildDetails.MASTER_BUDGET_FLAG}','{financialChildDetails.BUDGET_FLAG}','{financialChildDetails.ACC_CODE}','{financialChildDetails.PARTICULARS}',
                             '{financialChildDetails.TRANSACTION_TYPE}',{financialChildDetails.AMOUNT},'{financialChildDetails.REMARKS}','{financialChildDetails.FORM_CODE}','{financialChildDetails.COMPANY_CODE}',
                             '{financialChildDetails.BRANCH_CODE}','{financialChildDetails.CREATED_BY}',{financialChildDetails.CREATED_DATE},'{financialChildDetails.DELETED_FLAG}','{financialChildDetails.CURRENCY_CODE}',{financialChildDetails.EXCHANGE_RATE},                         '{financialChildDetails.TRACKING_NO}','{financialChildDetails.SESSION_ROWID}','{financialChildDetails.CHEQUE_NO}','{financialChildDetails.MR_NO}','{financialChildDetails.MR_ISSUED_BY}')";

                        dbContext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }
                    else
                    {
                        var insertQuery = $@"INSERT INTO {commonValue.TableName} (EFFECTIVE_DATE,DIVISION_CODE,REMARKS,MANUAL_NO,VOUCHER_DATE,VOUCHER_NO,TRANSACTION_TYPE,PARTICULARS,
                       BUDGET_FLAG,AMOUNT,ACC_CODE,SERIAL_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID)
                       VALUES ({financialChildDetails.EFFECTIVE_DATE},'{financialChildDetails.DIVISION_CODE}','{financialChildDetails.REMARKS}',
                     '{financialChildDetails.MANUAL_NO}', TO_DATE('{financialChildDetails.VOUCHER_DATE}', 'DD-MON-YYYY hh24:mi:ss'),'{financialChildDetails.VOUCHER_NO}', '{financialChildDetails.TRANSACTION_TYPE}',
                     '{financialChildDetails.PARTICULARS}', '{financialChildDetails.BUDGET_FLAG}',{financialChildDetails.AMOUNT},'{financialChildDetails.ACC_CODE}',
                     '{serialno}','{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}',
                     '{_workContext.CurrentUserinformation.login_code}',{financialChildDetails.CREATED_DATE}, '{financialChildDetails.DELETED_FLAG}','{financialChildDetails.SESSION_ROWID}')";

                        dbContext.ExecuteSqlCommand(insertQuery);
                        serialno++;
                        insertedToChild = true;
                    }


                }
                return insertedToChild;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
               // return false;
            }
        }

        public bool SaveMasterColumnValue(FinancialVoucherDetail masterColumnValue, CommonFieldsForFinanacialVoucher commonValue,NeoErpCoreEntity dbContext = null)
        {
            try
            {
                bool insertedToMaster = false;
                //  commonValue.VoucherDate = string.IsNullOrEmpty(commonValue.VoucherDate) ? "SYSDATE" : commonValue.VoucherDate;
                //  string insertmasterQuery = string.Format(@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,REFERENCE_NO,EXCHANGE_RATE) 
                //   VALUES('{0}',{1},'{2}','{3}','{4}','{5}','{6}','{7}',{8},TO_DATE('{9}','DD-MON-YYYY hh24:mi:ss'),'{10}','{11}')",
                //commonValue.NewVoucherNumber, commonValue.DrTotal, commonValue.FormCode, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code, _workContext.CurrentUserinformation.login_code.ToUpper(), 'N', commonValue.CurrencyFormat, "SYSDATE", masterColumnValue.VOUCHER_DATE, masterColumnValue.MANUAL_NO, commonValue.ExchangeRate);
                string insertmasterQuery = string.Empty;
                if (commonValue.TableName == "FA_DOUBLE_VOUCHER")
                { insertmasterQuery = $@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,
                COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,EXCHANGE_RATE,
                REFERENCE_NO) VALUES('{commonValue.NewVoucherNumber}','{commonValue.DrTotal}',
                '{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}',
                '{_workContext.CurrentUserinformation.login_code}','N','{commonValue.CurrencyFormat}',SYSDATE,
                TO_DATE('{masterColumnValue.VOUCHER_DATE}','DD-MON-YYYY'),
                {commonValue.ExchangeRate},'{masterColumnValue.MANUAL_NO}')"; }

                else if (commonValue.TableName == "FA_SINGLE_VOUCHER")
                { insertmasterQuery = $@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,
                COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,EXCHANGE_RATE,
                REFERENCE_NO,IS_SYNC_WITH_IRD,IS_REAL_TIME) VALUES('{commonValue.NewVoucherNumber}','{commonValue.DrTotal}',
                '{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}',
                '{_workContext.CurrentUserinformation.login_code}','N','{commonValue.CurrencyFormat}',SYSDATE,
                TO_DATE('{masterColumnValue.VOUCHER_DATE}','DD-MON-YYYY'),
                {commonValue.ExchangeRate},'{masterColumnValue.MANUAL_NO}','N','N')"; }
                else
                { insertmasterQuery = $@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,
                COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,EXCHANGE_RATE,
                REFERENCE_NO) VALUES('{commonValue.NewVoucherNumber}','{commonValue.DrTotal}',
                '{commonValue.FormCode}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}',
                '{_workContext.CurrentUserinformation.login_code}','N','{commonValue.CurrencyFormat}',SYSDATE,
                TO_DATE('{masterColumnValue.VOUCHER_DATE}','DD-MON-YYYY'),
                {commonValue.ExchangeRate},'{masterColumnValue.MANUAL_NO}')"; }

                dbContext.ExecuteSqlCommand(insertmasterQuery);
                insertedToMaster = true;

                //  Doubt why this is here
                if (!string.IsNullOrEmpty(commonValue.TempCode))
                {
                    string UpdateQuery = $@"UPDATE FORM_TEMPLATE_SETUP  SET SAVED_DRAFT='Y' WHERE TEMPLATE_NO='{commonValue.TempCode}'  AND  COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
                    dbContext.ExecuteSqlCommand(UpdateQuery);
                }
                insertedToMaster = true;

                return insertedToMaster;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
                //return false;
            }
        }

        public void SaveBudgetTransactionColumnValue(List<FinancialBudgetTransaction> budgetTransaction, CommonFieldsForFinanacialVoucher commonValue,NeoErpCoreEntity dbContext = null)
        {
            try
            {
                var budSerial = 1;
                double transactionBudgetNo = 0;
                double PrevtransactionBudgetNo = 0;
                foreach (var btrans in budgetTransaction)
                {
                    if (btrans.BUDGET.Count > 0)
                    {
                        foreach (var bud in btrans.BUDGET)
                        {
                            if (bud.AMOUNT != null)
                            {

                                var VoucherNumber = (commonValue.VoucherNumber == "undefined") ? commonValue.NewVoucherNumber : commonValue.VoucherNumber;
                                string maxtransnoquery = string.Format(@"SELECT TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO)+1),1))as TRANSACTIONNO FROM BUDGET_TRANSACTION");

                                string newMaxTransNoForBudget = _objectEntity.SqlQuery<string>(maxtransnoquery).FirstOrDefault();
                                if (newMaxTransNoForBudget != null)
                                    double.TryParse(newMaxTransNoForBudget, out transactionBudgetNo);
                                if (PrevtransactionBudgetNo >= transactionBudgetNo)
                                {
                                    PrevtransactionBudgetNo = PrevtransactionBudgetNo + 1;
                                }
                                else
                                {
                                    PrevtransactionBudgetNo = transactionBudgetNo;
                                }
                                string insertbudgettransQuery = $@"INSERT INTO BUDGET_TRANSACTION(
                                                                              TRANSACTION_NO,FORM_CODE,REFERENCE_NO,BUDGET_FLAG,SERIAL_NO,BUDGET_CODE,
                                                                              BUDGET_AMOUNT,PARTICULARS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                              CURRENCY_CODE,EXCHANGE_RATE,VALIDATION_FLAG,ACC_CODE,SESSION_ROWID)
                                                                              VALUES('{PrevtransactionBudgetNo}','{commonValue.FormCode}','{VoucherNumber}','{btrans.BUDGET_FLAG}',{budSerial},'{bud.BUDGET_CODE}',
                                                                             {bud.AMOUNT},'{bud.NARRATION}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','NRS',{1},'Y','{btrans.ACC_CODE}','{commonValue.NewVoucherNumber}')";
                                dbContext.ExecuteSqlCommand(insertbudgettransQuery);


                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public void SaveFinancialTDSColumnValue(List<FinancialTDS> financialTDS, CommonFieldsForFinanacialVoucher commonValue,NeoErpCoreEntity dbContext = null)
        {
            try
            {
                var tdsSerial = 1;
                if (string.IsNullOrEmpty(commonValue.NewVoucherNumber))
                    commonValue.NewVoucherNumber = commonValue.VoucherNumber;
                foreach (var ftds in financialTDS)
                {
                    if (ftds.CHILDTDS.Count > 0)
                    {
                        foreach (var ctds in ftds.CHILDTDS)
                        {
                            if (ctds.TDS_AMOUNT > 0)
                            {
                                string inserttdsQuery = $@"INSERT INTO FA_DC_TDS_INVOICE(
                                                                              SERIAL_NO,INVOICE_NO,INVOICE_DATE,MANUAL_NO,FORM_CODE,CS_CODE,ACC_CODE,TDS_TYPE_CODE,TAXABLE_AMOUNT,TDS_PERCENT,TDS_AMOUNT,CURRENCY_CODE,EXCHANGE_RATE,COMPANY_CODE,BRANCH_CODE,DELETED_FLAG,CREATED_BY,CREATED_DATE,SESSION_ROWID,TRAN_TYPE)
                                                                              VALUES({tdsSerial},'{commonValue.NewVoucherNumber}',trunc(SYSDATE),'{ftds.MANUAL_NO}',{commonValue.FormCode},'{ctds.SUPPLIER_CODE}',
                                                                             '{ftds.ACC_CODE}','{ctds.TDS_TYPE_CODE}',{ctds.NET_AMOUNT},{ctds.TDS_PERCENTAGE},{ctds.TDS_AMOUNT},'NRS',{1},'{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','N','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'1234','DR')";
                                dbContext.ExecuteSqlCommand(inserttdsQuery);
                                tdsSerial++;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public void SaveFinancialVATValue(List<FinancialVAT> financialVAT, CommonFieldsForFinanacialVoucher commonValue,NeoErpCoreEntity dbContext = null)
        {
            try
            {
                var vatSerial = 1;
                if (string.IsNullOrEmpty(commonValue.NewVoucherNumber))
                    commonValue.NewVoucherNumber = commonValue.VoucherNumber;

                foreach (var fvat in financialVAT)
                {
                    if (!fvat.Enable_DirectEntry)
                    {
                        if (fvat.CHILDVAT.Count > 0)
                        {
                            foreach (var fv in fvat.CHILDVAT)
                            {
                                if (fv.VAT_AMOUNT > 0)
                                {

                                    string insertvatQuery = $@"INSERT INTO FA_DC_VAT_INVOICE(
                                                                          INVOICE_NO,MANUAL_NO,INVOICE_DATE,FORM_CODE,CS_CODE,TAXABLE_AMOUNT,VAT_AMOUNT,P_TYPE,DOC_TYPE,REMARKS,CURRENCY_CODE,EXCHANGE_RATE,COMPANY_CODE,BRANCH_CODE,DELETED_FLAG,CREATED_BY,CREATED_DATE,SERIAL_NO,SESSION_ROWID,ACC_CODE)
                                                                          VALUES('{commonValue.NewVoucherNumber}','{fvat.MANUAL_NO}', trunc(SYSDATE),'{commonValue.FormCode}',{fv.SUPPLIER_CODE},{fv.TAXABLE_AMOUNT},
                                                                         '{fv.VAT_AMOUNT}','{fvat.TYPE}','{fvat.DOC_TYPE}','{fvat.REMARKS}','NRS',1,'{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.branch_code}','N','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,{ fv.SERIAL_NO},'1234','{fvat.ACC_CODE}')";
                                    dbContext.ExecuteSqlCommand(insertvatQuery);
                                    vatSerial++;
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public void SaveFinancialSubLedgerColumnValue(List<FinancialSubLedger> financialSubLedgers, CommonFieldsForFinanacialVoucher commonValue,NeoErpCoreEntity dbContext=null)
        {
            try
            {
                var subSerial = 1;
                var VoucherNumber = (commonValue.VoucherNumber == "undefined") ? commonValue.NewVoucherNumber : commonValue.VoucherNumber;
                if (financialSubLedgers.Count > 0)
                {
                    foreach (var fsl in financialSubLedgers)
                    {
                        foreach (var sbLeg in fsl.SUBLEDGER)
                        {
                            if (sbLeg.SUB_CODE != "" && !string.IsNullOrEmpty(sbLeg.SUB_CODE))
                            {
                                string maxtransnoquerySubledger = string.Format(@"SELECT TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO)+1),1))  as TRANSACTIONNO  FROM FA_VOUCHER_SUB_DETAIL  WHERE TRANSACTION_NO IS NOT NULL");


                                var CrAmt = fsl.TRANSACTION_TYPE == "CR" ? sbLeg.AMOUNT : "0";


                                var DrAmt = fsl.TRANSACTION_TYPE == "DR" ? sbLeg.AMOUNT : "0";

                                string newMaxTransNoForSubLedger = _objectEntity.SqlQuery<string>(maxtransnoquerySubledger).FirstOrDefault();
                                string insertSubLedgerQuery = $@"INSERT INTO FA_VOUCHER_SUB_DETAIL (TRANSACTION_NO,FORM_CODE,
                                                                   VOUCHER_DATE,VOUCHER_NO,SUB_CODE,ACC_CODE,
                                                                   PARTICULARS,TRANSACTION_TYPE,DR_AMOUNT,CR_AMOUNT,
                                                                   BRANCH_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                   SERIAL_NO,CURRENCY_CODE,EXCHANGE_RATE,SYN_ROWID) 
                                                                   VALUES('{newMaxTransNoForSubLedger}','{commonValue.FormCode}',SYSDATE,
                                                                  '{VoucherNumber}','{sbLeg.SUB_CODE}','{fsl.ACC_CODE}','{sbLeg.PARTICULARS}'
                                                                  ,'{fsl.TRANSACTION_TYPE}','{DrAmt}','{CrAmt}','{_workContext.CurrentUserinformation.branch_code}'
                                                                  ,'{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code.ToUpper()}',SYSDATE,'N','{sbLeg.SERIAL_NO}','{commonValue.CurrencyFormat}',{commonValue.ExchangeRate},'')";
                                dbContext.ExecuteSqlCommand(insertSubLedgerQuery);
                                subSerial++;
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public bool SaveChargeTransaction(List<CHARGETRANSACTION> chargetransaction, CommonFieldsForFinanacialVoucher commonValue,NeoErpCoreEntity dbContext = null)
        {
            try
            {
                if (chargetransaction.Count > 0)
                {
                    foreach(var chargetran in chargetransaction)
                    {
                        if(chargetran.CALC_TOTAL_PRICE == 0 || chargetran.CHARGE_TYPE_FLAG == null || chargetran.CHARGE_AMOUNT == 0)
                        {

                        }
                        else
                        {
                            string maxTranNoQuery = $@"select TO_CHAR(NVL(MAX(TO_NUMBER(TRANSACTION_NO)+1),1)) from CHARGE_TRANSACTION";
                            string tranNo = dbContext.SqlQuery<string>(maxTranNoQuery).FirstOrDefault();
                            string updateQuery = $@"update ip_purchase_invoice set calc_unit_price ={chargetran.CALC_UNIT_PRICE},calc_total_price = {chargetran.CALC_TOTAL_PRICE}, 
                                    modify_by='{_workContext.CurrentUserinformation.login_code.ToUpper()}',modify_date = SYSDATE,
                                    COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}', BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
                            where invoice_no = '{chargetran.REFERENCE_NO}' and item_code = '{chargetran.ITEM_CODE}' and quantity = {chargetran.QUANTITY}";
                            string query = $@"Insert into CHARGE_TRANSACTION
                               (TRANSACTION_NO, 
                                TABLE_NAME, REFERENCE_NO, ITEM_CODE, SERIAL_NO, APPLY_ON, 
                                CALCULATE_BY, CHARGE_CODE, CHARGE_TYPE_FLAG, CHARGE_AMOUNT, BUDGET_CODE, 
                                GL_FLAG, ACC_CODE, SUB_CODE, VALUE_PERCENT_AMOUNT, APPORTION_FLAG, 
                                FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, 
                                DELETED_FLAG, IMPACT_ON, CURRENCY_CODE, EXCHANGE_RATE, SYN_ROWID, 
                                SESSION_ROWID, CHARGE_FLAG, MODIFY_DATE, NON_GL_FLAG, MODIFY_BY, 
                                VOUCHER_NO, MANUAL_CALC_VALUE, REF_SERIAL_NO, OPENING_DATA_FLAG, DIVISION_CODE)
                             Values
                               ('{tranNo}', 'IP_PURCHASE_INVOICE', '{chargetran.REFERENCE_NO}', NULL, NULL,NULL, 
                                'D', '{chargetran.CHARGE_CODE}', '{chargetran.CHARGE_TYPE_FLAG}', {chargetran.CHARGE_AMOUNT}, 0, 
                                NULL, 'N', NULL, NULL, NULL, 
                                '{chargetran.FORM_CODE}', '{_workContext.CurrentUserinformation.company_code}', '{_workContext.CurrentUserinformation.branch_code}', '{_workContext.CurrentUserinformation.login_code.ToUpper()}', SYSDATE, 
                                 'N', NULL, '{chargetran.CURRENCY_CODE}',{chargetran.EXCHANGE_RATE}, NULL,
                                NULL, NULL, NULL, NULL, 'Y', 
                                '{commonValue.NewVoucherNumber}', NULL, NULL, NULL, NULL
                                )";
                            dbContext.ExecuteSqlCommand(query);
                            dbContext.ExecuteSqlCommand(updateQuery);
                        }                        
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        #endregion


        #region METHOD FOR EDIT

        public List<FinancialVoucherDetail> GetMasterTransactionByVoucherNo(string voucherNumber)
        {
            try
            {
                var getPrevDataQuery = $@"SELECT VOUCHER_NO,SESSION_ROWID, CREATED_BY,TO_CHAR(CREATED_DATE,'DD-MON-YYYY') AS CREATED_DATE  FROM MASTER_TRANSACTION WHERE VOUCHER_NO= '{voucherNumber}'";
                var defaultData = _objectEntity.SqlQuery<FinancialVoucherDetail>(getPrevDataQuery).ToList();
                return defaultData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public bool DeleteChildTransaction(CommonFieldsForFinanacialVoucher commonValue,NeoErpCoreEntity dbcontext=null)
        {
            try
            {
              
                string deletequery = string.Format(@"DELETE FROM " + commonValue.TableName + " where " + commonValue.PrimaryColumn + "='{0}' and COMPANY_CODE='{1}'", commonValue.VoucherNumber, _workContext.CurrentUserinformation.company_code);
                dbcontext.ExecuteSqlCommand(deletequery);
                return true;
            }
       
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public bool UpdateMasterTransaction(CommonFieldsForFinanacialVoucher commonUpdateValue, FinancialVoucherDetail masterColumnValue, NeoErpCoreEntity dbContext = null)
        {
            try
            {
                string query = $@"UPDATE MASTER_TRANSACTION SET VOUCHER_AMOUNT='{commonUpdateValue.DrTotal}',VOUCHER_DATE=TO_DATE('{masterColumnValue.VOUCHER_DATE}','DD-MON-YYYY'), MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}',REFERENCE_NO='{commonUpdateValue.ManualNumber}' , MODIFY_DATE =SYSDATE,CURRENCY_CODE='{commonUpdateValue.CurrencyFormat}',EXCHANGE_RATE='{commonUpdateValue.ExchangeRate}' where VOUCHER_NO='{commonUpdateValue.VoucherNumber}'  and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var rowCount = dbContext.ExecuteSqlCommand(query);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public void DeleteBudgetTransaction(string voucherNo, NeoErpCoreEntity dbContext = null)
        {
            try
            {
                string deletebudgetcenterquery = string.Format(@"DELETE FROM BUDGET_TRANSACTION where REFERENCE_NO='{0}' and COMPANY_CODE='{1}'", voucherNo, _workContext.CurrentUserinformation.company_code);
                dbContext.ExecuteSqlCommand(deletebudgetcenterquery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public void DeleteSubLedgerTransaction(string voucherNo,NeoErpCoreEntity neoErpCoreEntity)
        {
            try
            {
                string deletesubledgerquery = string.Format(@"DELETE FROM FA_VOUCHER_SUB_DETAIL where  VOUCHER_NO='{0}' and COMPANY_CODE='{1}'", voucherNo, _workContext.CurrentUserinformation.company_code);
                neoErpCoreEntity.ExecuteSqlCommand(deletesubledgerquery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public void DeleteVatTransaction(string voucherNo,NeoErpCoreEntity coreEntity)
        {
            try
            {
                string deletevatquery = string.Format(@"DELETE FROM FA_DC_VAT_INVOICE where  INVOICE_NO='{0}' and COMPANY_CODE='{1}'", voucherNo, _workContext.CurrentUserinformation.company_code);
                coreEntity.ExecuteSqlCommand(deletevatquery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public void DeleteTDSTransaction(string voucherNo,NeoErpCoreEntity coreEntity)
        {
            try
            {
                string deletetdsquery = string.Format(@"DELETE FROM FA_DC_TDS_INVOICE where  INVOICE_NO='{0}' and COMPANY_CODE='{1}'", voucherNo, _workContext.CurrentUserinformation.company_code);
                coreEntity.ExecuteSqlCommand(deletetdsquery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        #endregion
    }
}
