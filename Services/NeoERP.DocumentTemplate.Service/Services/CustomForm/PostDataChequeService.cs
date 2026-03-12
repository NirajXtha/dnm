using System;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoERP.DocumentTemplate.Service.Interface.CustomForm;
using NeoERP.DocumentTemplate.Service.Models.CustomForm;
using NeoERP.DocumentTemplate.Service.Models;
using System.Text;

namespace NeoERP.DocumentTemplate.Service.Services.CustomForm
{
    public class PostDataChequeService : IPostDataCheque
    {

        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;

        public PostDataChequeService(IWorkContext workContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }

        #region POST DATE CHEQUE

        public List<string> GenerateNewReceipt()
        {
            try
            {
                //var resultList = new List<string>();
                //var newReceiptQuery = $@"SELECT NVL(MAX(TO_NUMBER(RECEIPT_NO)),0) FROM FA_PDC_RECEIPTS";
                //var newReceipt = _objectEntity.SqlQuery<int>(newReceiptQuery).FirstOrDefault().ToString();
                //if (newReceipt == null) newReceipt = "0";
                //var lastChar = newReceipt.ToString().Substring(newReceipt.Length - 1);
                //var lastCharIntoInt = Convert.ToInt32(lastChar) + 1;
                //int place = newReceipt.LastIndexOf(lastChar.ToString());

                //if (place == -1) resultList.Add(newReceipt);
                //// return newReceipt;


                //string result = newReceipt.Remove(place, 1).Insert(place, lastCharIntoInt.ToString());
                //resultList.Add(result);
                //resultList.Add(_workContext.CurrentUserinformation.login_code);
                ////return result;
                //return resultList;

                var resultList = new List<string>();

                var newReceiptQuery = @"SELECT NVL(MAX(TO_NUMBER(RECEIPT_NO)),0) FROM FA_PDC_RECEIPTS";
                int lastReceiptNumber = _objectEntity.SqlQuery<int>(newReceiptQuery).FirstOrDefault();

                // next receipt number
                int nextReceiptNumber = lastReceiptNumber + 1;

                // convert to string
                string result = nextReceiptNumber.ToString();

                resultList.Add(result);
                resultList.Add(_workContext.CurrentUserinformation.login_code);

                return resultList;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while generating receipt no : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        public List<string> GenerateNewPaymentNo()
        {
            try
            {
                var resultList = new List<string>();

                var newPaymentQuery = @"SELECT NVL(MAX(TO_NUMBER(PAYMENT_NO)),0) FROM FA_PDC_PAYMENTS";
                int lastPaymentNumber = _objectEntity.SqlQuery<int>(newPaymentQuery).FirstOrDefault();

                // next receipt number
                int nextPaymentNumber = lastPaymentNumber + 1;

                // convert to string
                string result = nextPaymentNumber.ToString();

                resultList.Add(result);
                resultList.Add(_workContext.CurrentUserinformation.login_code);

                return resultList;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while generating payment no : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }


        public List<string> GenerateNewBGVoucher()
        {
            try
            {
                var resultList = new List<string>();
                var newVoucherQuery = $@"SELECT NVL(MAX(TO_NUMBER(BG_NO)),1) FROM FA_BANK_GUARANTEE";
                var newVoucher = _objectEntity.SqlQuery<int>(newVoucherQuery).FirstOrDefault().ToString();
                if (newVoucher == null) newVoucher = "0";
                var lastChar = newVoucher.ToString().Substring(newVoucher.Length - 1);
                var lastCharIntoInt = Convert.ToInt32(lastChar) + 1;
                int place = newVoucher.LastIndexOf(lastChar.ToString());

                if (place == -1) resultList.Add(newVoucher);
                // return newReceipt;


                string result = newVoucher.Remove(place, 1).Insert(place, lastCharIntoInt.ToString());
                resultList.Add(result);
                resultList.Add(_workContext.CurrentUserinformation.login_code);
                //return result;
                return resultList;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while generating bank gurantee voucher no : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<PartyTypeCodeModal> GetAllPartyType()
        {
            try
            {
                //string partyTypeQuery = $@"SELECT IPTC.PARTY_TYPE_CODE,IPTC.PARTY_TYPE_EDESC,IPTC.COMPANY_CODE,IPTC.CREATED_BY,IPTC.CREATED_DATE,IPTC.ACC_CODE,IPTC.CREDIT_LIMIT,IPTC.CREDIT_DAYS,NVL(IPTC.ADDRESS,'N/A') ADDRESS
                //                           FROM IP_PARTY_TYPE_CODE IPTC WHERE IPTC.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND IPTC.DELETED_FLAG='N' AND IPTC.PARTY_TYPE_FLAG='P'";

                string partyTypeQuery = $@"SELECT IPTC.PARTY_TYPE_CODE,IPTC.PARTY_TYPE_EDESC,IPTC.COMPANY_CODE,IPTC.CREATED_BY,IPTC.CREATED_DATE,IPTC.ACC_CODE,IPTC.CREDIT_LIMIT,IPTC.CREDIT_DAYS,NVL(IPTC.ADDRESS,'N/A') ADDRESS
                                           FROM IP_PARTY_TYPE_CODE IPTC WHERE IPTC.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND IPTC.DELETED_FLAG='N'";
                var typeData = _objectEntity.SqlQuery<PartyTypeCodeModal>(partyTypeQuery).ToList();
                return typeData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting party type code: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<CustomerModal> GetAllCustomer()
        {
            try
            {
                string customerQuery = $@"    SELECT CUSTOMER_CODE,CUSTOMER_EDESC,REGD_OFFICE_EADDRESS,PARTY_TYPE_CODE,ACC_CODE,TPIN_VAT_NO FROM SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG='I' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N'";
                var typeData = _objectEntity.SqlQuery<CustomerModal>(customerQuery).ToList();
                return typeData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting customer code: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<SupplierModel> GetAllSupplier()
        {
            try
            {
                string supplierQuery = $@"SELECT SUPPLIER_CODE,SUPPLIER_EDESC,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1 FROM IP_SUPPLIER_SETUP WHERE GROUP_SKU_FLAG='I' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N'";
                var typeData = _objectEntity.SqlQuery<SupplierModel>(supplierQuery).ToList();
                return typeData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting supplier: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<PartyTypeCodeModal> GetAllDealerType()
        {
            try
            {
                string dealerTypeQuery = $@"SELECT IPTC.PARTY_TYPE_CODE,IPTC.PARTY_TYPE_EDESC,IPTC.COMPANY_CODE,IPTC.CREATED_BY,IPTC.CREATED_DATE,IPTC.ACC_CODE,IPTC.CREDIT_LIMIT,IPTC.CREDIT_DAYS
                                           FROM IP_PARTY_TYPE_CODE IPTC WHERE IPTC.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND IPTC.DELETED_FLAG='N' AND IPTC.PARTY_TYPE_FLAG='D'";
                var dealerData = _objectEntity.SqlQuery<PartyTypeCodeModal>(dealerTypeQuery).ToList();
                return dealerData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting dealer: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string SaveNewPDCForm(PDCFormSaveModal modal)
        {
            try
            {

                //var data = modal.BOUNCE_DATE.Value.ToShortDateString();
                StringBuilder updateQuery = new StringBuilder();
                var currentContext = _workContext.CurrentUserinformation;
                var pdcQuery = string.Empty;
                char is_odc = 'N';
                if (modal.ENCASH_DATE != null) updateQuery.Append($@",ENCASH_DATE=TRUNC(TO_DATE('{modal.ENCASH_DATE.Value.ToShortDateString()}','MM/DD/YYYY'))");

                if (modal.BOUNCE_DATE != null) { updateQuery.Append($@",BOUNCE_DATE=TRUNC(TO_DATE('{modal.BOUNCE_DATE.Value.ToShortDateString()}','MM/DD/YYYY')),BOUNCE_FLAG='Y',BOUNCE_BY='{currentContext.login_code}'");  }

                if (modal.CHECK_RETURN_DATE != null) { updateQuery.Append($@",RETURN_DATE=TRUNC(TO_DATE('{modal.CHECK_RETURN_DATE.Value.ToShortDateString()}','MM/DD/YYYY')),RETURN_FLAG='Y'");}

                if(modal.IN_TRANSIT_DATE !=null) { updateQuery.Append($@",INTRANSIT_DATE=TRUNC(TO_DATE('{modal.IN_TRANSIT_DATE.Value.ToShortDateString()}','MM/DD/YYYY')),INTRANSIT_FLAG='Y',INTRANSIT_BY='{currentContext.login_code}'"); }

                 
                if (modal.IS_UPDATE)
                {
                    if (modal.IS_ODC == "True") is_odc = 'Y';
                    pdcQuery = $@"UPDATE FA_PDC_RECEIPTS SET RECEIPT_DATE=TRUNC(TO_DATE('{modal.RECEIPT_DATE.ToShortDateString()}','MM/DD/YYYY')),CHEQUE_DATE=TRUNC(TO_DATE('{modal.CHEQUE_DATE.ToShortDateString()}','MM/DD/YYYY')),CUSTOMER_CODE='{modal.CUSTOMER_CODE}',PDC_AMOUNT='{modal.PDC_AMOUNT}',MR_ISSUED_BY='{modal.MONEY_RECEIPT_ISSUED_BY}',PDC_DETAILS='{modal.PDC_DETAILS}',
                              BANK_NAME='{modal.PARTY_BANK_NAME}',BRANCH_CODE='{currentContext.branch_code}',COMPANY_CODE='{currentContext.company_code}',MODIFY_BY='{currentContext.login_code}',MODIFY_DATE=SYSDATE,
                              ACC_CODE='{modal.SELECTED_ACCOUNT}',CHEQUE_NO='{modal.CHEQUE_NO}',PARTY_TYPE_CODE='{modal.PARTY_TYPE}',PRIOR_DAYS='{modal.REMINDER_PRIOR_DAYS}',REMARKS='{modal.REMARKS}',MR_NO='{modal.MONEY_RECEIPT_NO}',
                              IS_ODC = '{is_odc}' {updateQuery}
                              WHERE RECEIPT_NO='{modal.RECEIPT_NO}'";
                }
                else
                {
                    if (modal.IS_ODC == "True") is_odc = 'Y';
                    pdcQuery = $@"INSERT INTO FA_PDC_RECEIPTS(RECEIPT_NO,RECEIPT_DATE,CHEQUE_DATE,CUSTOMER_CODE,PDC_AMOUNT,MR_ISSUED_BY,PDC_DETAILS,BANK_NAME,BRANCH_CODE,
                                   COMPANY_CODE,CREATED_BY,CREATED_DATE,ACC_CODE,CHEQUE_NO,PARTY_TYPE_CODE,PRIOR_DAYS,REMARKS,MR_NO,IS_ODC)
                                   VALUES('{modal.RECEIPT_NO}',TRUNC(TO_DATE('{modal.RECEIPT_DATE.ToShortDateString()}','MM/DD/YYYY')),TRUNC(TO_DATE('{modal.CHEQUE_DATE.ToShortDateString()}','MM/DD/YYYY')),'{modal.CUSTOMER_CODE}','{modal.PDC_AMOUNT}','{modal.MONEY_RECEIPT_ISSUED_BY}','{modal.PDC_DETAILS}',
                                   '{modal.PARTY_BANK_NAME}','{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                    '{modal.SELECTED_ACCOUNT}','{modal.CHEQUE_NO}','{modal.PARTY_TYPE}','{modal.REMINDER_PRIOR_DAYS}','{modal.REMARKS}','{modal.MONEY_RECEIPT_NO}','{is_odc}')";
                    
                }
                _objectEntity.ExecuteSqlCommand(pdcQuery);
                _logErp.InfoInFile("PDC Detail form saved successfully by : " + _workContext.CurrentUserinformation.login_code);
                return "Successful";
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving PDF Form detail : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public PDCVoucherResponse GeneratePdcOdcVoucher(PDCFormSaveModal modal)
        {
            
            using (var trans = _objectEntity.Database.BeginTransaction())
                {

                    try
                    {
                    var formCodeQuery = $@"SELECT RECEIPT_FORM_CODE FROM FA_PDC_PREFERENCES WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
                    var formCode = _objectEntity.SqlQuery<string>(formCodeQuery).FirstOrDefault();
                    var tableNameQuery = $@"SELECT TABLE_NAME FROM FORM_DETAIL_SETUP WHERE FORM_CODE = '{formCode}'";
                    var tableName = _objectEntity.SqlQuery<string>(tableNameQuery).FirstOrDefault();
                    string query = $@"select FN_NEW_VOUCHER_NO('{_workContext.CurrentUserinformation.company_code}','{formCode}',SYSDATE,'{tableName}') FROM DUAL";
                    string voucherNo = _objectEntity.SqlQuery<string>(query).First();
                    string insertQuery = string.Empty;
                    var result = new PDCVoucherResponse();
                    if (tableName == "FA_DOUBLE_VOUCHER")
                    {
                        insertQuery = $@"INSERT INTO FA_DOUBLE_VOUCHER(VOUCHER_NO,VOUCHER_DATE,SERIAL_NO,ACC_CODE,TRANSACTION_TYPE,AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,
                                                                 EFFECTIVE_DATE,REMARKS,CHEQUE_NO,MR_ISSUED_BY,MR_NO,EXCHANGE_RATE)
                                      VALUES ('{voucherNo}',SYSDATE,'1','{modal.ACC_CODE}','CR','{modal.PDC_AMOUNT}','{formCode}','{_workContext.CurrentUserinformation.company_code}'
                                              ,'{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N',TRUNC(TO_DATE('{modal.CHEQUE_DATE.ToShortDateString()}','MM/DD/YYYY'))
                                              ,'{modal.REMARKS}','{modal.CHEQUE_NO}','{modal.MONEY_RECEIPT_ISSUED_BY}','{modal.MONEY_RECEIPT_NO}','1')";
                    }
                    else
                    {
                        insertQuery = $@"INSERT INTO FA_SINGLE_VOUCHER(VOUCHER_NO,VOUCHER_DATE,SERIAL_NO,MASTER_ACC_CODE,MASTER_TRANSACTION_TYPE,MASTER_AMOUNT,ACC_CODE,
                             TRANSACTION_TYPE,AMOUNT,REMARKS,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CHEQUE_NO,MR_ISSUED_BY,MR_NO) 
                             VALUES('{voucherNo}',SYSDATE,'1','{modal.MASTER_ACC_CODE}',
                             'CR',{modal.PDC_AMOUNT},'{modal.ACC_CODE}',
                             'DR',{modal.PDC_AMOUNT},'{modal.REMARKS}','{formCode}','{_workContext.CurrentUserinformation.company_code}',
                             '{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N','{modal.CHEQUE_NO}',                         
                             '{modal.MONEY_RECEIPT_NO}','{modal.MONEY_RECEIPT_ISSUED_BY}')";
                    }

                    var rowAffected = _objectEntity.ExecuteSqlCommand(insertQuery);
                    var updatePDCWithVoucher = $@"UPDATE FA_PDC_RECEIPTS SET VOUCHER_NO='{voucherNo}',VG_DATE=SYSDATE,ACC_CODE='{modal.ACC_CODE}' WHERE RECEIPT_NO='{modal.RECEIPT_NO}'";
                    var row = _objectEntity.ExecuteSqlCommand(updatePDCWithVoucher);
                    var masterRow = 0;
                    if(row > 0)
                    {
                        string insertmasterQuery = string.Format(@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,DELETED_FLAG,CURRENCY_CODE,CREATED_DATE,VOUCHER_DATE,SYN_ROWID,EXCHANGE_RATE,SESSION_ROWID,PRINT_COUNT) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',TO_DATE('{8}','DD-MON-YYYY hh24:mi:ss'),SYSDATE,'{9}',{10},'{11}',{12})",
                          voucherNo, modal.PDC_AMOUNT, formCode, this._workContext.CurrentUserinformation.company_code, this._workContext.CurrentUserinformation.branch_code, this._workContext.CurrentUserinformation.login_code, 'N', '1', DateTime.Now.ToString("dd-MMM-yyyy"), 1, 1, 0, 1);
                       masterRow = _objectEntity.ExecuteSqlCommand(insertmasterQuery);
                    }

                    if (rowAffected > 0 && masterRow > 0)
                    {
                        trans.Commit();
                        result.Status = "Successfull";
                        result.VoucherNo = voucherNo;
                        result.Message = "Voucher Generated Successfully";
                    }
                    else
                    {
                        trans.Rollback();
                        result.Status = "Error";
                        result.VoucherNo = voucherNo;
                        result.Message = "Error while generating voucher No";
                    }
                    return result;
                    }
                    catch (Exception ex)
                    {
                       trans.Rollback();
                        throw new Exception(ex.Message);
                    }
                    
                }

            
        }

        public List<PDCFormSaveModal> GetAllPDCFormDetail(PDCFilter filter)
        {
            try
            {
                string filterQuery = string.Empty;
                if(filter.PDCType != null)
                {
                    List<string> result = filter.PDCType.Split(',').ToList();
                    foreach(var type in result)
                    {
                        if (Convert.ToInt32(type) == (int)PDCTYPEENUM.CHEQUE_IN_HAND) filterQuery += " AND FDR.ENCASH_DATE IS NULL AND FDR.INTRANSIT_DATE IS NULL AND FDR.BOUNCE_DATE IS NULL AND FDR.RETURN_DATE IS NULL";
                        else if (Convert.ToInt32(type) == (int)PDCTYPEENUM.CHEQUE_IN_TRANSIT) filterQuery += " AND FDR.INTRANSIT_FLAG='Y'";
                        else if (Convert.ToInt32(type) == (int)PDCTYPEENUM.BOUNCE) filterQuery += " AND FDR.BOUNCE_FLAG='Y'";
                        else if (Convert.ToInt32(type) == (int)PDCTYPEENUM.RETURN) filterQuery += " AND FDR.RETURN_FLAG='Y'";
                        else if (Convert.ToInt32(type) == (int)PDCTYPEENUM.ENCASH) filterQuery += " AND FDR.ENCASH_DATE IS NOT NULL AND FDR.INTRANSIT_FLAG='N' AND FDR.BOUNCE_FLAG='N' AND FDR.RETURN_FLAG='N'";

                    }
                    
                }

                if (filter.Customer !=null)
                {
                    List<string> customers = filter.Customer.Split(',').ToList();
                    filterQuery += $@" AND FDR.CUSTOMER_CODE IN({string.Join(",", customers)})";
                }

                if(filter.Dealer != null)
                {
                    List<string> dealers = filter.Dealer.Split(',').ToList();
                    filterQuery +=  $@" AND FDR.PARTY_TYPE_CODE='{string.Join(",",dealers)}'";
                }

                if (!string.IsNullOrEmpty(filter.PDCAmount) && Convert.ToInt32(filter.PDCAmount) > 0)
                {
                    filterQuery += $@" AND FDR.PDC_AMOUNT={filter.PDCAmount}";
                }

                if (!string.IsNullOrEmpty(filter.PdcStatus))
                {
                    if ((PDCTYPEENUM) Enum.Parse(typeof(PDCTYPEENUM),filter.PdcStatus) == PDCTYPEENUM.CHEQUE_IN_HAND) filterQuery += " AND FDR.ENCASH_DATE IS NULL AND FDR.INTRANSIT_DATE IS NULL AND FDR.BOUNCE_DATE IS NULL AND FDR.RETURN_DATE IS NULL";
                    else if ((PDCTYPEENUM)Enum.Parse(typeof(PDCTYPEENUM), filter.PdcStatus) == PDCTYPEENUM.CHEQUE_IN_TRANSIT) filterQuery += " AND FDR.INTRANSIT_FLAG='Y'";
                    else if ((PDCTYPEENUM)Enum.Parse(typeof(PDCTYPEENUM), filter.PdcStatus) == PDCTYPEENUM.BOUNCE) filterQuery += " AND FDR.BOUNCE_FLAG='Y'";
                    else if ((PDCTYPEENUM)Enum.Parse(typeof(PDCTYPEENUM), filter.PdcStatus) == PDCTYPEENUM.RETURN) filterQuery += " AND FDR.RETURN_FLAG='Y'";
                    else if ((PDCTYPEENUM)Enum.Parse(typeof(PDCTYPEENUM), filter.PdcStatus) == PDCTYPEENUM.ENCASH) filterQuery += " AND FDR.ENCASH_DATE IS NOT NULL AND FDR.INTRANSIT_FLAG='N' AND FDR.BOUNCE_FLAG='N' AND FDR.RETURN_FLAG='N'";
                }

                //var detailQuery = $@"SELECT FDR.RECEIPT_NO,FDR.RECEIPT_DATE,FDR.CHEQUE_NO,FDR.CHEQUE_DATE,FDR.ENCASH_DATE,FDR.CUSTOMER_CODE,SCS.CUSTOMER_EDESC AS CUSTOMER,
                //                            FDR.BOUNCE_DATE,FDR.PARTY_TYPE_CODE,FDR.PARTY_TYPE_CODE as PARTY_TYPE,IPTC.PARTY_TYPE_EDESC AS DEALER,FDR.PDC_AMOUNT,FDR.PDC_DETAILS,TO_CHAR(FDR.IS_ODC) IS_ODC,
                //                            FDR.BANK_NAME,FDR.REMARKS,NVL(PRIOR_DAYS,0) AS REMINDER_PRIOR_DAYS,MR_ISSUED_BY AS MONEY_RECEIPT_ISSUED_BY,NVL(MR_NO,0) AS MONEY_RECEIPT_NO,VOUCHER_NO
                //                        	,CASE 
                //                          WHEN FDR.ENCASH_DATE IS NOT NULL AND (FDR.INTRANSIT_FLAG IS NULL OR FDR.INTRANSIT_FLAG='N') AND (FDR.BOUNCE_FLAG IS NULL OR FDR.BOUNCE_FLAG='N') AND (FDR.RETURN_FLAG IS NULL OR FDR.RETURN_FLAG='N')
                //                           THEN 'ENCASH'
                //                          WHEN FDR.INTRANSIT_DATE IS NOT NULL AND FDR.INTRANSIT_FLAG='Y'
                //                           THEN 'INTRANSIT'
                //                          WHEN FDR.BOUNCE_DATE IS NOT NULL AND FDR.BOUNCE_FLAG='Y'
                //                           THEN 'BOUNCE DATE'
                //                                WHEN FDR.RETURN_DATE IS NOT NULL AND FDR.RETURN_FLAG='Y'
                //                                    THEN 'RETURN DATE'
                //                          ELSE 'CHECK_IN_HAND'
                //                       END AS STATUS
                //                   FROM FA_PDC_RECEIPTS FDR 
                //                   INNER JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE=FDR.CUSTOMER_CODE 
                //                   INNER JOIN IP_PARTY_TYPE_CODE IPTC ON IPTC.PARTY_TYPE_CODE=FDR.PARTY_TYPE_CODE 
                //                  WHERE FDR.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                //                  AND TRUNC(FDR.CHEQUE_DATE) BETWEEN TRUNC(TO_DATE('{filter.DateEnglishFrom.ToShortDateString()}','MM/DD/YYYY')) 
                //                  AND TRUNC(TO_DATE('{filter.DateEnglishTo.ToShortDateString()}','MM/DD/YYYY')) {filterQuery}
                //                  ";
                var detailQuery = $@"SELECT FDR.RECEIPT_NO,FDR.RECEIPT_DATE,FDR.CHEQUE_NO,FDR.CHEQUE_DATE,FDR.ENCASH_DATE,FDR.CUSTOMER_CODE,SCS.CUSTOMER_EDESC AS CUSTOMER,
                                            FDR.BOUNCE_DATE,FDR.PARTY_TYPE_CODE,FDR.PARTY_TYPE_CODE as PARTY_TYPE,IPTC.PARTY_TYPE_EDESC AS DEALER,FDR.PDC_AMOUNT,FDR.PDC_DETAILS,TO_CHAR(FDR.IS_ODC) IS_ODC,
                                            FDR.BANK_NAME,FDR.REMARKS,NVL(PRIOR_DAYS,0) AS REMINDER_PRIOR_DAYS,MR_ISSUED_BY AS MONEY_RECEIPT_ISSUED_BY,NVL(MR_NO,0) AS MONEY_RECEIPT_NO,VOUCHER_NO
                                        	,CASE 
		                                        WHEN FDR.ENCASH_DATE IS NOT NULL AND (FDR.INTRANSIT_FLAG IS NULL OR FDR.INTRANSIT_FLAG='N') AND (FDR.BOUNCE_FLAG IS NULL OR FDR.BOUNCE_FLAG='N') AND (FDR.RETURN_FLAG IS NULL OR FDR.RETURN_FLAG='N')
			                                        THEN 'ENCASH'
		                                        WHEN FDR.INTRANSIT_DATE IS NOT NULL AND FDR.INTRANSIT_FLAG='Y'
			                                        THEN 'INTRANSIT'
		                                        WHEN FDR.BOUNCE_DATE IS NOT NULL AND FDR.BOUNCE_FLAG='Y'
			                                        THEN 'BOUNCE DATE'
                                                WHEN FDR.RETURN_DATE IS NOT NULL AND FDR.RETURN_FLAG='Y'
                                                    THEN 'RETURN DATE'
		                                        ELSE 'CHECK_IN_HAND'
		                                     END AS STATUS
                                   FROM FA_PDC_RECEIPTS FDR 
                                   INNER JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE=FDR.CUSTOMER_CODE 
                                   INNER JOIN IP_PARTY_TYPE_CODE IPTC ON IPTC.PARTY_TYPE_CODE=FDR.PARTY_TYPE_CODE 
                                  WHERE FDR.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                                  AND TRUNC(FDR.CREATED_DATE) BETWEEN TRUNC(TO_DATE('{filter.DateEnglishFrom.ToShortDateString()}','MM/DD/YYYY')) 
                                  AND TRUNC(TO_DATE('{filter.DateEnglishTo.ToShortDateString()}','MM/DD/YYYY')) {filterQuery}
                                  ";
                var detailData = _objectEntity.SqlQuery<PDCFormSaveModal>(detailQuery).ToList();
                return detailData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting PDC form detail : ");
                throw new Exception(ex.Message);
            }
        }

        public List<PDCFormSaveModal> SearchAllPDCDetail(PDCFilter filter)
        {
            try
            {
                var detailQuery = $@"SELECT FDR.RECEIPT_NO,FDR.RECEIPT_DATE,FDR.CHEQUE_NO,FDR.CHEQUE_DATE,FDR.ENCASH_DATE,FDR.CUSTOMER_CODE,
                                            FDR.BOUNCE_DATE,FDR.PARTY_TYPE_CODE as PARTY_TYPE,FDR.PDC_AMOUNT,FDR.PDC_DETAILS,
                                            FDR.BANK_NAME,FDR.REMARKS 
                                      FROM FA_PDC_RECEIPTS FDR 
                                      WHERE FDR.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND
                                      TRUNC(FDR.RECEIPT_DATE) BETWEEN TO_DATE('{filter.DateEnglishFrom}','YYYY-MM-DD') AND TO_DATE('{filter.DateEnglishTo}','YYYY-MM-DD')
                                      AND FDR.CUSTOMER_CODE='{filter.Customer}'
                                      AND FDR.PARTY_TYPE_CODE='{filter.Dealer}'";
                var detailData = _objectEntity.SqlQuery<PDCFormSaveModal>(detailQuery).ToList();
                return detailData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while search pdc detail : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<CustomerModal> GetCustomerSubLedger()
        {
            try
            {
                
                var customerNodes = new List<CustomerModal>();
                var customerSubLegrQuery = $@"SELECT DISTINCT SCS.CUSTOMER_CODE AS TYPE_CODE,SCS.CUSTOMER_CODE as id,SCS.CUSTOMER_EDESC AS TYPE_EDESC,SCS.CUSTOMER_EDESC as label,SCS.REGD_OFFICE_EADDRESS,SCS.PARTY_TYPE_CODE,SCS.LINK_SUB_CODE,SCS.MASTER_CUSTOMER_CODE,SCS.PRE_CUSTOMER_CODE,
                                             SCS.COMPANY_CODE,SCS.CREATED_BY,SCS.CREATED_DATE,SCS.ACC_CODE,
                                             FAC.ACC_EDESC,
                                             SCS.TPIN_VAT_NO,SCS.GROUP_SKU_FLAG
                                            FROM SA_CUSTOMER_SETUP SCS 
                                            Left JOIN FA_CHART_OF_ACCOUNTS_SETUP FAC ON FAC.ACC_CODE = SCS.ACC_CODE
                                            WHERE SCS.DELETED_FLAG='N' AND SCS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                                            CONNECT BY PRIOR SCS.MASTER_CUSTOMER_CODE=SCS.PRE_CUSTOMER_CODE START WITH SCS.PRE_CUSTOMER_CODE='00'";
                var customerSubLegrData = _objectEntity.SqlQuery<CustomerModal>(customerSubLegrQuery).ToList();
                return customerSubLegrData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting customer sub ledger : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public PDCFormSaveModal EditPDCFormDetail(string pdcId,string pdcStatus="")
        {
            try
            {
                var pdcToEditQuery = $@"SELECT FDR.RECEIPT_NO,FDR.RECEIPT_DATE,FDR.CHEQUE_NO,FDR.ENCASH_DATE,FDR.INTRANSIT_DATE,FDR.BOUNCE_DATE,FDR.RETURN_DATE,FDR.CHEQUE_DATE,FDR.CUSTOMER_CODE,FAC.ACC_EDESC AS MAPPED_ACC_EDESC,
                                               FDR.PARTY_TYPE_CODE as PARTY_TYPE,FDR.PDC_AMOUNT,FDR.PDC_DETAILS,MR_ISSUED_BY AS MONEY_RECEIPT_ISSUED_BY,NVL(MR_NO,0) AS MONEY_RECEIPT_NO,IS_ODC,NVL(PRIOR_DAYS,0) AS REMINDER_PRIOR_DAYS,
                                               FDR.BANK_NAME AS PARTY_BANK_NAME,FDR.CREATED_BY,FDR.CREATED_DATE,FDR.REMARKS 
                                        FROM FA_PDC_RECEIPTS FDR  
                                        LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = FDR.CUSTOMER_CODE
                                        LEFT JOIN FA_CHART_OF_ACCOUNTS_SETUP FAC ON FAC.ACC_CODE = CS.ACC_CODE  
                                        WHERE FDR.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND RECEIPT_NO='{pdcId}'";
                var pdcToEdit = _objectEntity.SqlQuery<PDCFormSaveModal>(pdcToEditQuery).FirstOrDefault();
                if (string.IsNullOrEmpty(pdcStatus)) return pdcToEdit;
                else if (pdcStatus == "encash") return pdcToEdit;
                else
                {
                    if (pdcToEdit.ENCASH_DATE == null)
                    {
                        return null;
                    }
                    else
                    {
                        return pdcToEdit;
                    }
                }
               
                
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while editing post dated cheque:" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string DeletePDCFormDetail(string pdcId)
        {
            try
            {
                //var deleQuery = $@"UPDATE FA_PDC_RECEIPTS SET DELETED_FLAG='Y' WHERE RECEIPT_NO='{pdcId}'";
                var delQuery = $@"DELETE FROM FA_PDC_RECEIPTS WHERE RECEIPT_NO='{pdcId}'";
                var rowafftected = _objectEntity.ExecuteSqlCommand(delQuery);
                if(rowafftected > 0)
                {
                    return "Successful";
                }
                else
                {
                    return "Error: No Data Found";
                }
                
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while deleting PDC Form Detail : " + ex.StackTrace);
                throw;
            }
        }


        public enum PDCTYPEENUM
        {
            ALL=01,
            CHEQUE_IN_HAND=02,
            CHEQUE_IN_TRANSIT=03,
            ENCASH=04,
            BOUNCE=05,
            RETURN=06
        }

        #endregion

        #region COLUMN SETTINGS

        public List<Erp_TableName> GetAllTableName()
        {
            try
            {
                string tableQuery = $@"SELECT DISTINCT FDS.SERIAL_NO as TABLE_ID , FDS.TABLE_NAME as TABLE_NAME FROM FORM_DETAIL_SETUP FDS WHERE FDS.DELETED_FLAG='N' AND
                                      FDS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var tableData = _objectEntity.SqlQuery<Erp_TableName>(tableQuery).ToList();
                return tableData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting table name for mapping : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<FormDetailModal> GetAllFormDetail()
        {
            try
            {
                string formQuery = $@"SELECT FS.FORM_CODE,FS.FORM_EDESC,FS.MASTER_FORM_CODE,FS.PRE_FORM_CODE,FS.MODULE_CODE,FS.GROUP_SKU_FLAG,FS.COMPANY_CODE,FS.CREATED_BY,FS.CREATED_DATE
                                      FROM FORM_SETUP FS WHERE FS.DELETED_FLAG='N' AND FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var formData = _objectEntity.SqlQuery<FormDetailModal>(formQuery).ToList();
                return formData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting form detail : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<FormDetailEditModal> GetAllFormDetailToEdit(string formCode="", string tableName="")
         {
            try
            {
                if (formCode == "" && tableName=="")
                {
                    return new List<FormDetailEditModal>();
                }
                else
                {
                    //FDS.COLUMN_NAME,
                    var columIdCount = 1;
                    var tableIdCount = 1;
                    string editQuery = $@"SELECT rownum as ROW_ID,FDS.SERIAL_NO,FDS.TABLE_NAME,FDS.COLUMN_NAME,FDS.COLUMN_WIDTH,FDS.COLUMN_HEADER,FDS.TOP_POSITION,FDS.LEFT_POSITION,
                                  FDS.DISPLAY_FLAG as CDISPLAY_FLAG,FDS.IS_DESC_FLAG as CIS_DESC_FLAG,FDS.FORM_CODE,FDS.COMPANY_CODE,FDS.CREATED_BY,FDS.CREATED_DATE FROM FORM_DETAIL_SETUP FDS
                                  WHERE FDS.DELETED_FLAG='N' AND FDS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND  FDS.TABLE_NAME='{tableName}' AND FDS.FORM_CODE='{formCode}'";
                    var editData = _objectEntity.SqlQuery<FormDetailEditModal>(editQuery).ToList();
                    foreach (var ed in editData)
                    {
                        if (ed.CDISPLAY_FLAG == 'Y') ed.DISPLAY_FLAG = true;

                        if (ed.CIS_DESC_FLAG == 'Y') ed.IS_DESC_FLAG = true;

                        ed.COLUMN_NAME.COLUMN_ID = columIdCount;
                        ed.COLUMN_NAME.COLUMN_NAME = "RequisitionNo";
                        ed.TABLE_NAME.TABLE_ID = tableIdCount;
                        ed.TABLE_NAME.TABLE_NAME = "IP_GOODS_REQUISITION";
                        columIdCount++;
                        tableIdCount++;
                    }
                    return editData;
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting form detail setup to edit : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string EditAllFormDetail(List<FormDetailEditModal> modal)
        {
            try
            {
                return "";
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving custom column : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string SaveAllFormDetail(List<FormDetailEditModal> modal)
        {
            try
            {
                var message = string.Empty;
                if (modal.Count > 0)
                {
                    foreach (var md in modal)
                    {
                        var insertQuery = $@"INSERT INTO FORM_DETAIL_SETUP(SERIAL_NO,TABLE_NAME,COLUMN_NAME,COLUMN_WIDTH,COLUMN_HEADER,TOP_POSITION,LEFT_POSITION,FORM_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE)
                                   VALUES('{md.SERIAL_NO}','{md.TABLE_NAME.TABLE_NAME}','{md.COLUMN_NAME.COLUMN_NAME}','10','{md.COLUMN_HEADER}',1,1,'{md.FORM_CODE}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE)";
                        _objectEntity.ExecuteSqlCommand(insertQuery);
                        _logErp.InfoInFile("Column settings saved successfully by : " + _workContext.CurrentUserinformation.login_code);
                        message = "Successfull";
                    }
                }
                else message = "No data to save";
                return message;
                
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving column settings : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<ColumnNameAsDDL> GetColumnNameForDDL()
        {
            try
            {
                var columnQuery = $@" SELECT rownum as COLUMN_ID , FDS.COLUMN_NAME as COLUMN_NAME FROM FORM_DETAIL_SETUP FDS WHERE FDS.DELETED_FLAG='N' AND FDS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var columnData = _objectEntity.SqlQuery<ColumnNameAsDDL>(columnQuery).ToList();
                return columnData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting column name for ddl : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<TableNameAsDDL> GetTableNameForDDL()
        {
            try
            {
                var tableQuery = $@" SELECT DISTINCT rownum as TABLE_ID,FDS.TABLE_NAME as TABLE_NAME FROM FORM_DETAIL_SETUP FDS WHERE FDS.DELETED_FLAG='N' AND FDS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var tableData = _objectEntity.SqlQuery<TableNameAsDDL>(tableQuery).ToList();
                return tableData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting table name for drop down : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        private List<ColumnNameAsDDL> GetColumnNameForList()
        {
            try
            {
                var columnQuery = $@" SELECT rownum as COLUMN_ID , FDS.COLUMN_NAME as COLUMN_NAME FROM FORM_DETAIL_SETUP FDS WHERE FDS.DELETED_FLAG='N' AND FDS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var columnData = _objectEntity.SqlQuery<ColumnNameAsDDL>(columnQuery).ToList();
                return columnData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting column name : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        
        public bool DeleteAllFormDetail(FormDetailEditModal modal)
        {
            try
            {
                var deleteQuery = $@"DELETE FROM FORM_DETAIL_SETUP FDS WHERE FDS.SERIAL_NO='{modal.SERIAL_NO}' AND FDS.TABLE_NAME='{modal.TABLE_NAME.TABLE_NAME}' AND FDS.FORM_CODE='{modal.FORM_CODE}'
                                    AND FDS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                _objectEntity.ExecuteSqlCommand(deleteQuery);
                return true;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while deleting column settings : " + ex.StackTrace);
                throw;
            }
        }


        #endregion

        #region CASH BANK SETUP
        public List<CashBankSelectedAccountModal> GetSelectedAccountGrid()
        {
            try
            {
                
                var accountList = new List<CashBankSelectedAccountModal>();
                accountList.Add(new CashBankSelectedAccountModal());
                return accountList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting selected account list : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<CashBankRootDetailModal> GetCashBankRootDetail()
        {
            try
            {
                var cashBankRootQuery = $@"SELECT FCBS.CB_CODE,FCBS.CB_EDESC,FCBS.CB_NDESC,FCBS.COMPANY_CODE,FCBS.CREATED_BY,FCBS.CREATED_DATE FROM FA_CASH_BANK_SETUP FCBS WHERE FCBS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FCBS.DELETED_FLAG='N'";
                var cashBankRootData = _objectEntity.SqlQuery<CashBankRootDetailModal>(cashBankRootQuery).ToList();
                return cashBankRootData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting cash bank setup root detail : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<CashBankAccountDetail> GetCashBankAccountDetail(string cb_code)
        {
            try
            {
                var accountDetailQuery = $@"SELECT FCBDS.CB_CODE,FCBDS.TITLE_FLAG,FCBDS.ACC_CODE,FCBDS.COMPANY_CODE,FCAS.ACC_EDESC
                                            FROM FA_CASH_BANK_DETAIL_SETUP FCBDS
                                           INNER JOIN FA_CHART_OF_ACCOUNTS_SETUP FCAS ON FCAS.ACC_CODE=FCBDS.ACC_CODE
                                           WHERE FCBDS.DELETED_FLAG='N' AND FCBDS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FCBDS.CB_CODE='{cb_code}'";
                var accountDetailData = _objectEntity.SqlQuery<CashBankAccountDetail>(accountDetailQuery).ToList();
                return accountDetailData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting cash bank account detail : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string GenerateCBId()
        {
            try
            {
                var cbIdQuery = $@"SELECT MAX(CB_CODE) FROM FA_CASH_BANK_SETUP";
                var newCBID = _objectEntity.SqlQuery<string>(cbIdQuery).FirstOrDefault();
                var lastChar = newCBID.ToString().Substring(newCBID.Length - 1);
                var lastCharIntoInt = Convert.ToInt32(lastChar) + 1;
                int place = newCBID.LastIndexOf(lastChar.ToString());

                if (place == -1)
                    return newCBID;

                string result = newCBID.Remove(place, 1).Insert(place, lastCharIntoInt.ToString());
                //return result;
                return result;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while generating cash bank shourt cut id :" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string SaveCashBankAccountDetail(CashBankAccountDetailSaveModal modal)
        {
           
                using(var cbTransaction = _objectEntity.Database.BeginTransaction())
                {
                    try
                    {
                    if (modal.CashBankDetail.IS_UPDATE)
                    {
                        var cashBankUpdate = $@"UPDATE FA_CASH_BANK_SETUP SET CB_EDESC='{modal.CashBankDetail.IN_ENGLISH}',CB_NDESC='{modal.CashBankDetail.IN_NEPALI}',MODIFY_DATE=SYSDATE,MODIFY_BY='{_workContext.CurrentUserinformation.login_code}' WHERE CB_CODE='{modal.CashBankDetail.SHORT_CUT}'";
                        var update = _objectEntity.ExecuteSqlCommand(cashBankUpdate);
                        if(modal.ListInfo.Count > 0)
                        {
                            var updateAccountDetailCB = $@"DELETE FROM FA_CASH_BANK_DETAIL_SETUP WHERE CB_CODE='{modal.CashBankDetail.SHORT_CUT}'";
                            var result1 = _objectEntity.ExecuteSqlCommand(updateAccountDetailCB);
                            foreach (var cbd in modal.ListInfo)
                            {
                                var cashBankDetailSetupQuery = $@"INSERT INTO FA_CASH_BANK_DETAIL_SETUP(CB_CODE,TITLE_FLAG,ACC_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                                                              VALUES('{modal.CashBankDetail.SHORT_CUT}','{cbd.ACC_TYPE_FLAG}','{cbd.ACC_CODE.Replace("\"", string.Empty)}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                                _objectEntity.ExecuteSqlCommand(cashBankDetailSetupQuery);

                            }
                        }
                        cbTransaction.Commit();
                        return "Successfull";
                    }
                    else
                    {
                        var cashBankSetupQuery = $@"INSERT INTO FA_CASH_BANK_SETUP(CB_CODE,CB_EDESC,CB_NDESC,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                                                    VALUES('{modal.CashBankDetail.SHORT_CUT}','{modal.CashBankDetail.IN_ENGLISH}','{modal.CashBankDetail.IN_NEPALI}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                        _objectEntity.ExecuteSqlCommand(cashBankSetupQuery);
                        foreach (var cbd in modal.ListInfo)
                        {
                            var cashBankDetailSetupQuery = $@"INSERT INTO FA_CASH_BANK_DETAIL_SETUP(CB_CODE,TITLE_FLAG,ACC_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                                                              VALUES('{modal.CashBankDetail.SHORT_CUT}','{cbd.ACC_TYPE_FLAG}','{cbd.ACC_CODE.Replace("\"", string.Empty)}','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                            _objectEntity.ExecuteSqlCommand(cashBankDetailSetupQuery);

                        }
                        cbTransaction.Commit();
                        return "Successfull";
                    }
                    }
                    catch (Exception ex)
                    {
                        cbTransaction.Rollback();
                        _logErp.ErrorInDB("Error while saving cash bank setup detail : " + ex.StackTrace);
                        throw new Exception(ex.Message);
                    }
                }
        }

        public string DeleteCashBankDetail(string cb_code)
        {
            using(var delTransaction = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    var companyCode = _workContext.CurrentUserinformation.company_code;
                    if (string.IsNullOrEmpty(cb_code)) { cb_code = string.Empty; }
                    string message = string.Empty;
                    var sqlquery = $@"UPDATE FA_CASH_BANK_SETUP SET DELETED_FLAG='Y' WHERE CB_CODE='{cb_code}'";
                    var result = _objectEntity.ExecuteSqlCommand(sqlquery);
                    var updateAccountDetailCB = $@"DELETE FROM FA_CASH_BANK_DETAIL_SETUP WHERE CB_CODE='{cb_code}'";
                    var result1 = _objectEntity.ExecuteSqlCommand(sqlquery);
                    delTransaction.Commit();
                    if (result > 0)
                    {
                        message = "DELETED";
                    }
                    return message;
                }
                catch (Exception ex)
                {
                    delTransaction.Rollback();
                    _logErp.ErrorInDB("Error while deleting cash bank setup : " + ex.StackTrace);
                    throw new Exception(ex.Message);
                }
            }
        }

        #endregion

        #region BANK RECONCILATION

        public List<BankDetailForReconcilation> GetBankDetailForReconcilation()
        {
            try
            {

                //var bankDetailList = new List<BankDetailForReconcilation>();
                var bankDetailListQuery = $@"SELECT cas.ACC_CODE,cas.ACC_CODE as id,cas.ACC_EDESC as label,cas.ACC_EDESC,cas.TRANSACTION_TYPE,cas.TPB_FLAG,cas.ACC_TYPE_FLAG,cas.MASTER_ACC_CODE,cas.PRE_ACC_CODE,cas.COMPANY_CODE,cas.PRE_ACC_CODE,
                                                   cas.CREATED_BY,cas.CREATED_DATE,cas.ACC_NATURE
                                            FROM FA_CHART_OF_ACCOUNTS_SETUP cas WHERE cas.ACC_NATURE='AC' AND cas.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var bankDetailListData = _objectEntity.SqlQuery<BankDetailForReconcilation>(bankDetailListQuery).ToList();
                return bankDetailListData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting customer sub ledger : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<ReconcilationGridModel> GetDataForReconcilation()
        {
            try
            {
                var reconcileListQuery = $@"SELECT gl.VOUCHER_DATE,gl.VOUCHER_DATE AS MITI,gl.VOUCHER_NO,gl.TRANSACTION_NO AS CHEQUE_NO,gl.PARTICULARS,
                                                   NVL(gl.DR_AMOUNT,0) AS DR_AMOUNT,NVL(gl.CR_AMOUNT,0) AS CR_AMOUNT,NVL(gl.BALANCE_AMOUNT,0) AS BALANCE_AMOUNT,
                                                   NVL(gl.RECONCILE_DATE,SYSDATE),NVL(gl.CLEARING_DATE,SYSDATE)
                                           FROM FA_GENERAL_LEDGER gl
                                           WHERE gl.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND gl.RECONCILE_BY IS NULL";
                var reconcileListData = _objectEntity.SqlQuery<ReconcilationGridModel>(reconcileListQuery).ToList();
                return reconcileListData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting reconsilation data s : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string UpdateBankReconcilation(string voucherNo)
        {
            try
            {
                string responseMessage = string.Empty;
                var brSaveQuery = $@"UPDATE FA_GENERAL_LEDGER gl SET gl.RECONCILE_BY='{_workContext.CurrentUserinformation.login_code}'";
                var rowAffected = _objectEntity.ExecuteSqlCommand(brSaveQuery);
                if (rowAffected > 0)
                {
                    responseMessage = "Successful";
                }
                else
                {
                    responseMessage = "Error: ";
                }
                return responseMessage;
            }
            catch (Exception ex)
            {

                _logErp.ErrorInDB("Error while updating general ledger data  : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
          
         }


        public BankDetailForReconcilation EditBankReconcilationDetail(string brId)
        {
            try
            {
                var brToEditQuery = $@"SELECT FDR.RECEIPT_NO,FDR.RECEIPT_DATE,FDR.CHEQUE_NO,FDR.CHEQUE_DATE,FDR.ENCASH_DATE,FDR.CUSTOMER_CODE,
                                   FDR.BOUNCE_DATE,FDR.PARTY_TYPE_CODE as PARTY_TYPE,FDR.PDC_AMOUNT,FDR.PDC_DETAILS,MR_ISSUED_BY AS MONEY_RECEIPT_ISSUED_BY,MR_NO AS MONEY_RECEIPT_NO,BOUNCE_BY,
                                   INTRANSIT_BY AS IN_TRANSIT_BY,INTRANSIT_DATE AS IN_TRANSIT_DATE,PRIOR_DAYS AS REMINDER_PRIOR_DAYS,
                                   FDR.BANK_NAME AS PARTY_BANK_NAME,FDR.REMARKS FROM FA_PDC_RECEIPTS FDR WHERE FDR.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND RECEIPT_NO='{brId}'";
                var brToEdit = _objectEntity.SqlQuery<BankDetailForReconcilation>(brToEditQuery).FirstOrDefault();
                return brToEdit;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while editing post dated cheque:" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string DeleteBankReconcilationDetail(string brId)
        {
            try
            {
                var delQuery = $@"UPDATE FA_GENERAL_LEDGER FGA SET FGA.DELETED_FLAG='Y' WHERE FGA.VOUCHER_NO='{brId}'";
                var rowafftected = _objectEntity.ExecuteSqlCommand(delQuery);
                if (rowafftected > 0)
                {
                    return "Successful";
                }
                else
                {
                    return "Error: No Data Found";
                }

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while deleting PDC Form Detail : " + ex.StackTrace);
                throw;
            }
        }

        public LoggedInInfoModal GetLoggedInInfo()
        {
            try
            {
                return new LoggedInInfoModal()
                {
                    OFFICE_NAME = _workContext.CurrentUserinformation.company_name,
                    OFFICE_ADDRESS = _workContext.CurrentUserinformation.branch_name,
                   // OFFICE_ADDRESS1 = _workContext.CurrentUserinformation.bra
                };
               
               
            }
            catch (Exception ex)
            {

                _logErp.ErrorInDB("Error while getting loging info data  : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region BANK GURANTEE

        public List<BankGuranteeModal> GetBankGuranteeList()
         {
            try
            {
                var bankGuranteeQuery = $@"  SELECT *
    FROM (SELECT A.*,to_number((A.END_DATE-A.START_DATE)) AS EXPAIRY_DUE_DAYS,A.BG_DATE as LOG_DATE_ENGLISH,BS_DATE (A.BG_DATE) AS BG_MITI,CASE CS_FLAG WHEN 'C' THEN FN_FETCH_DESC (A.COMPANY_CODE,
'SA_CUSTOMER_SETUP',A.CS_CODE) WHEN 'S' THEN FN_FETCH_DESC (A.COMPANY_CODE, 'IP_SUPPLIER_SETUP',A.CS_CODE)
END PARTY_NAME,(SELECT PARTY_TYPE_EDESC FROM IP_PARTY_TYPE_CODE WHERE PARTY_TYPE_CODE = A.PARTY_TYPE_CODE
AND COMPANY_CODE = A.COMPANY_CODE)PARTY_TYPE_EDESC,(SELECT AREA_EDESC FROM IP_PARTY_TYPE_CODE AA, AREA_SETUP BB
WHERE AA.PARTY_TYPE_CODE = A.PARTY_TYPE_CODE AND AA.COMPANY_CODE = A.COMPANY_CODE AND AA.AREA_CODE = BB.AREA_CODE
AND AA.COMPANY_CODE = BB.COMPANY_CODE)AREA_EDESC FROM FA_BANK_GUARANTEE A WHERE A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
AND a.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' AND A.DELETED_FLAG = 'N') A WHERE A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND a.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' ORDER BY A.PARTY_NAME";
                var bGList = _objectEntity.SqlQuery<BankGuranteeModal>(bankGuranteeQuery).ToList();
                return bGList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting gurantee list : " + ex.StackTrace);
                throw ex;
            }
        }

        public string SaveBankGurantee(BankGuranteeModal modal)
        {
            try
            {
                if (modal.CS_CODE == null || string.IsNullOrEmpty(modal.CS_CODE)) modal.CS_CODE = "010101";
                string responseMessage = string.Empty;
                var guranteeSaveQuery = $@"INSERT INTO FA_BANK_GUARANTEE(BG_NO,BG_DATE,CS_CODE,ACC_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,
                                                                        DELETED_FLAG,START_DATE,END_DATE,BG_AMOUNT,ALERT_PRIOR_DAYS,ADDRESS,PARTY_TYPE_CODE,BANK_GNO,REMARKs,CURRENCY_CODE,EXCHANGE_RATE,CLOSE_FLAG,CS_FLAG)
                                           VALUES('{modal.BG_NO}',TRUNC(TO_DATE('{modal.LOG_DATE_ENGLISH.ToShortDateString()}','MM/DD/YYYY')),'{modal.CS_CODE}','{modal.ACC_CODE}','{_workContext.CurrentUserinformation.company_code}',
                                           '{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N',TRUNC(TO_DATE('{modal.START_DATE_ENGLISH.ToShortDateString()}','MM/DD/YYYY')),
                                           TRUNC(TO_DATE('{modal.EXPIRY_DATE_ENGLISH.ToShortDateString()}','MM/DD/YYYY')),'{modal.BG_AMOUNT}','{modal.ALERT_PRIOR_DAYS}','{modal.ADDRESS}','{modal.PARTY_NAME}','{modal.BANK_GNO}','{modal.REMARKS}','NRS',1,'{modal.CLOSE_FLAG}','{modal.CS_FLAG}')";
               var rowAffected =  _objectEntity.ExecuteSqlCommand(guranteeSaveQuery);
                if(rowAffected > 0)
                {
                    responseMessage = "Successful";
                }
                else
                {
                    responseMessage = "Error: ";
                }
                return responseMessage;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while saving bank gurantee : " + ex.StackTrace);
                throw ex;
            }
        }

        public BankGuranteeModal EditBankGuaranteeDetail(string bgId)
        {
            try
            {
                var bgToEditQuery = $@"SELECT BG_NO AS VOUCHER_NO,BG_DATE AS LOG_DATE,PARTY_TYPE_CODE AS PARTY_NAME,ADDRESS,BG_AMOUNT,START_DATE AS START_DATE_ENGLISH,
                                       END_DATE AS EXPIRY_DATE_ENGLISH,REMARKS,ALERT_PRIOR_DAYS FROM FA_BANK_GUARANTEE FBA WHERE FBA.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND FBA.BG_NO='{bgId}'";
                var bgToEdit = _objectEntity.SqlQuery<BankGuranteeModal>(bgToEditQuery).FirstOrDefault();
                return bgToEdit;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while editing post dated cheque:" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string DeleteBankGuaranteeDetail(string bgId)
        {
            try
            {
               
                var delQuery = $@"UPDATE FA_BANK_GUARANTEE FBG SET FBG.DELETED_FLAG='Y' WHERE FBG.BG_NO='{bgId}'";
                var rowafftected = _objectEntity.ExecuteSqlCommand(delQuery);
                if (rowafftected > 0)
                {
                    return "Successful";
                }
                else
                {
                    return "Error: No Data Found";
                }

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while deleting PDC Form Detail : " + ex.StackTrace);
                throw;
            }
        }

        #endregion
    }
}
